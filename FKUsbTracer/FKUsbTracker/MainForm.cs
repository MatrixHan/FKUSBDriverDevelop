using System;
using System.Collections.Generic;
using System.Windows.Forms;
//---------------------------------------------------------------------
namespace FKUsbTracer
{
    enum ENUMStartupActions
    {
        InstallDriver = 1,
        UninstallDriver = 2
    }
    public partial class MainForm : Form
    {
        public delegate void FilterTraceArrived(object sender, FKFilterTraceArrivedEventArgs e);

        #region 变量
        FKNative m_Native = null;
        FKDeviceManagement m_DevManage = null;
        IntPtr m_FuncDevNotificationsHandle = IntPtr.Zero;
        FKFilterTrace m_PrevTrace;
        uint m_nMaxTraces = 0;
        #endregion

        public MainForm()
        {
            InitializeComponent();

            // 核心对象初始化
            m_Native = new FKNative();
            m_DevManage = new FKDeviceManagement();
            m_PrevTrace = new FKFilterTrace();

            // 其他控件初始化
            if (!FKWindowsSecurity.IsAdmin())
            {
                this.Text += " (非管理员)";
                FKWindowsSecurity.AddShieldToButton(Reinstall_Button);
                FKWindowsSecurity.AddShieldToButton(Uninstall_Button);
            }
            else
                this.Text += " (管理员)";

            cbMaxTraces.SelectedIndex = 0;
        }
        // 窗口加载消息
        private void MainForm_Load(object sender, EventArgs e)
        {
            // 检查一下当前驱动状态
            CheckCurrentDriverStates();

            // 注册USB设备 增/删 消息回调
            m_DevManage.RegisterForDeviceNotifications(Handle, ref m_FuncDevNotificationsHandle);

            // 枚举设备
            EnumFilterDevices();

            // 收到新消息回调
            m_Native.FilterTraceArrived += new EventHandler<FKFilterTraceArrivedEventArgs>(RecievedFilterTracesCallback);

            bool bIsAutoTrace;
            if (m_Native.GetAutoTrace(out bIsAutoTrace))
                AutoTrace_CheckBox.Checked = bIsAutoTrace;
        }

        // 继承父类Windows消息回调
        protected override void WndProc(ref Message m)
        {
            // 如果是设备更变消息，则开启定时器刷新设备
            if (m.Msg == FKDeviceManagement.WM_DEVICECHANGE)
            {
                tmrDeviceChange.Enabled = false;
                tmrDeviceChange.Enabled = true;
            }

            //  其他消息正常处理
            base.WndProc(ref m);
        }


        #region 核心事件
        // 安装驱动
        private void InstallDriver()
        {
            bool needRestart;
            string failureReason;
            if (FKDriverManagement.InstallDriver(out needRestart, out failureReason))
            {
                if (needRestart)
                    MessageBox.Show(this, "驱动安装完成! 请重启电脑.", "安装驱动");
                else
                    MessageBox.Show(this, "驱动安装完成!", "安装驱动");
            }
            else
                MessageBox.Show(this, string.Format("驱动安装失败： ({0})", failureReason), "安装驱动失败");
        }
        // 卸载驱动
        private void UninstallDriver()
        {
            bool needRestart;
            string failureReason;
            if (FKDriverManagement.UninstallDriver(out needRestart, out failureReason))
            {
                if (needRestart)
                    MessageBox.Show(this, "驱动卸载完成! 请重启电脑.", "卸载驱动");
                else
                    MessageBox.Show(this, "驱动卸载完成!", "卸载驱动");
            }
            else
                MessageBox.Show(this, string.Format("驱动卸载失败 ({0})", failureReason), "卸载驱动失败");
        }
        // 检查当前驱动
        private void CheckCurrentDriverStates()
        {
            bool bInstallDriver = false;
            System.Diagnostics.FileVersionInfo myDriverVersion;
            // 检查驱动兼容性
            if (FKDriverManagement.IsDriverInstalled(out myDriverVersion))
            {
                string thatVersion = string.Format("{0}.{1}", myDriverVersion.FileMajorPart, myDriverVersion.FileMinorPart);
                Version assVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                string thisVersion = string.Format("{0}.{1}", assVersion.Major, assVersion.Minor);
                if (thatVersion != thisVersion)
                {
                    if (MessageBox.Show(
                            string.Format("本机 FK USB 驱动版本 ({0}) 不符合当前驱动版本 ({1}). 你是否要将其更改为 {1} ?",
                                    thatVersion,
                                    thisVersion),
                            "驱动版本不匹配",
                            MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        bInstallDriver = true;
                    }
                }
            }
            else
            {
                if (MessageBox.Show(
                        "FK USB 驱动尚未安装. 你是否要立刻安装 ?",
                        "FK USB 驱动检查",
                        MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    bInstallDriver = true;
                }
            }

            // 看看是否需要安装驱动
            if (bInstallDriver)
            {
                if (FKWindowsSecurity.IsAdmin())
                    InstallDriver();
                else
                    FKWindowsSecurity.RestartElevated(ENUMStartupActions.InstallDriver.ToString());
            }
        }
        // 枚举设备
        private void EnumFilterDevices()
        {
            SuspendLayout();

            USBDevice_TreeView.Nodes.Clear();

            List<FKDeviceID> deviceIds;
            m_Native.GetDeviceList(out deviceIds);

            for (int i = 0; i < deviceIds.Count; i++)
            {
                FKDeviceID devId = deviceIds[i];
                m_DevManage.FindDeviceProps(devId.PhysicalDeviceObjectName, out devId.HardwareId, out devId.Description, out devId.InstanceId);

                TreeNode child = new TreeNode(devId.ToString());
                child.Checked = devId.Enabled;
                child.ToolTipText = devId.HardwareId;
                child.Tag = devId;
                if (!InsertNodeInDeviceTree(devId, USBDevice_TreeView.Nodes, child))
                    USBDevice_TreeView.Nodes.Add(child);
            }
            USBDevice_TreeView.ExpandAll();

            ResumeLayout(true);
        }
        // 向 USB树 中增加一个节点
        private bool InsertNodeInDeviceTree(FKDeviceID devId, TreeNodeCollection parentNodes, TreeNode child)
        {
            for (int i = 0; i < parentNodes.Count; i++)
            {
                FKDeviceID devIdParent = (FKDeviceID)parentNodes[i].Tag;
                if (m_DevManage.IsDeviceChild(devIdParent.InstanceId, devId.InstanceId))
                {
                    parentNodes[i].Nodes.Add(child);
                    return true;
                }
                if (InsertNodeInDeviceTree(devId, parentNodes[i].Nodes, child))
                    return true;
            }
            return false;
        }
        // 收到 USB 消息
        private void RecievedFilterTracesCallback(object sender, FKFilterTraceArrivedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new FilterTraceArrived(RecievedFilterTracesCallback), new Object[] { sender, e });
            }
            else
            {
                if (MainTabControl.SelectedTab == Trace_TabPage)
                {
                    DataTrace_ListView.SuspendDrawing();

                    foreach (FKFilterTrace filterTrace in e.Traces)
                    {
                        AddFilterTrace(filterTrace);
                    }

                    DataTrace_ListView.ResumeDrawing();
                }
            }
        }
        // 添加一条USB消息
        private void AddFilterTrace(FKFilterTrace filterTrace)
        {
            // 检查是否符合筛选器的要求
            if (DoesTracePassFilters(filterTrace, FilterInclude.Include) &&
                DoesTracePassFilters(filterTrace, FilterInclude.Exclude))
            {
                if (m_nMaxTraces > 0)
                {
                    while (DataTrace_ListView.Items.Count >= m_nMaxTraces)
                        DataTrace_ListView.Items.RemoveAt(0);
                }
                // 创建一行
                ListViewItem item = new ListViewItem(filterTrace.DeviceId.ToString());
                for (int i = 1; i < DataTrace_ListView.Columns.Count; i++)
                {
                    switch (i)
                    {
                        case 1:
                            item.SubItems.Add(filterTrace.TypeToStr());
                            break;
                        case 2:
                            item.SubItems.Add(filterTrace.GetTimestampDelta(m_PrevTrace).ToString());
                            break;
                        case 3:
                            if (filterTrace.Buffer != null)
                                item.SubItems.Add(filterTrace.Buffer.Length.ToString());
                            else
                                item.SubItems.Add(Convert.ToString(0));
                            break;
                        case 4:
                            item.SubItems.Add(filterTrace.BufToHex());
                            break;
                        case 5:
                            item.SubItems.Add(filterTrace.BufToChars());
                            break;
                    }
                }
                DataTrace_ListView.TopItem = DataTrace_ListView.Items.Add(item);
                m_PrevTrace = filterTrace;
            }
        }
        // 检查一条消息是否符合筛选器
        private bool DoesTracePassFilters(FKFilterTrace filterTrace, FilterInclude include)
        {
            List<FilterMatch> filters;
            if (include == FilterInclude.Include)
                filters = fkFilterControl.IncludeFilters;
            else
                filters = fkFilterControl.ExcludeFilters;

            if (filters.Count == 0)
                return true;

            bool check = true;

            foreach (FilterMatch filter in filters)
            {
                switch (filter.FilterType)
                {
                    case FilterType.Length:
                        switch (filter.LengthMatch)
                        {
                            case LengthMatch.GreaterThen:
                                check = filterTrace.Buffer.Length > filter.Length;
                                break;
                            case LengthMatch.LessThen:
                                check = filterTrace.Buffer.Length < filter.Length;
                                break;
                            case LengthMatch.EqualTo:
                                check = filterTrace.Buffer.Length == filter.Length;
                                break;
                        }
                        break;
                    case FilterType.Hex:
                        check = filterTrace.BufToHex().Contains(filter.Filter);
                        break;
                    case FilterType.Ascii:
                        check = filterTrace.BufToChars().Contains(filter.Filter);
                        break;
                }
                if (include == FilterInclude.Include)
                {
                    if (check)
                        return true;
                    else
                        continue;
                }
                else
                {
                    if (check)
                        return false;
                    else
                        continue;
                }
            }
            if (include == FilterInclude.Include)
                return false;
            else
                return true;
        }
        // 更新消息追踪状态
        private void UpdateTracingStatus()
        {
            if (btnStartTraces.Checked)
            {
                if (MainTabControl.SelectedTab == Trace_TabPage)
                    m_Native.StartTracing();
                else
                    m_Native.StopTracing();
                m_Native.StartTraceReader();
            }
            else
                m_Native.StopTraceReader();

            btnStartTraces.Checked = btnStartTraces.Checked;
        }
        // 更变 消息 列表显示列
        private void UpdateColumn(ColumnHeader ch, CheckBox cb)
        {
            if (cb.Checked && ch.Width == 0)
                ch.Width = (int)ch.Tag;
            else if (!cb.Checked && ch.Width != 0)
            {
                ch.Tag = ch.Width;
                ch.Width = 0;
            }
        }
        #endregion

        #region 控件事件
        // 重新安装驱动 按钮按下
        private void Reinstall_Button_Click(object sender, EventArgs e)
        {
            if (FKWindowsSecurity.IsAdmin())
            {
                InstallDriver();
            }
            else
            {
                FKWindowsSecurity.RestartElevated(ENUMStartupActions.InstallDriver.ToString());
            }
        }
        // 卸载驱动 按钮按下
        private void Uninstall_Button_Click(object sender, EventArgs e)
        {
            if (FKWindowsSecurity.IsAdmin())
            {
                UninstallDriver();
            }
            else
            {
                FKWindowsSecurity.RestartElevated(ENUMStartupActions.UninstallDriver.ToString());
            }
        }
        // 单秒定时器事件
        private void tmrDeviceChange_Tick(object sender, EventArgs e)
        {
            EnumFilterDevices();
            tmrDeviceChange.Enabled = false;
        }
        // 点击USB设备 事件
        private void USBDevice_TreeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            m_Native.SetDeviceEnabled(((FKDeviceID)e.Node.Tag).DevId,e.Node.Checked);
        }
        // 启动消息 按钮按下
        private void btnStartTraces_Click(object sender, EventArgs e)
        {
            UpdateTracingStatus();
        }
        // 清除消息 按钮按下
        private void btnClearTraces_Click(object sender, EventArgs e)
        {
            DataTrace_ListView.Items.Clear();
            m_PrevTrace = new FKFilterTrace();
        }
        // 切换主Tab页
        private void MainTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateTracingStatus();
        }
        // 自动跟踪新加入设备 复选框更变
        private void AutoTrace_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            m_Native.SetAutoTrace(AutoTrace_CheckBox.Checked);
        }
        // 设置 选项卡中 显示的消息列 内复选框有任何变化
        private void cbTraceListColumn_CheckedChanged(object sender, EventArgs e)
        {
            UpdateColumn(chID, cbId);
            UpdateColumn(chType, cbType);
            UpdateColumn(chTime, cbTime);
            UpdateColumn(chLength, cbLength);
            UpdateColumn(chHex, cbHex);
            UpdateColumn(chAscii, cbAscii);
        }
        // 最大Trace长度下拉框 值更变
        private void cbMaxTraces_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbMaxTraces.SelectedIndex == 0)
                m_nMaxTraces = 0;
            else
            {
                string s = cbMaxTraces.Items[cbMaxTraces.SelectedIndex].ToString();
                m_nMaxTraces = Convert.ToUInt32(s);
            }
        }
        // 复制到剪切板
        private void btnCopyToClipboard_Click(object sender, EventArgs e)
        {
            DataTrace_ListView.CopyToClipboard(false);
        }
        // 保存到文件中
        private void btnSaveToFile_Click(object sender, EventArgs e)
        {
            SaveFileDialog fd = new SaveFileDialog();
            fd.Filter = "USB Trace Files|.fkut";
            if (fd.ShowDialog() == DialogResult.OK)
            {
                System.IO.File.WriteAllText(fd.FileName, DataTrace_ListView.CopyContents(false));
            }
        }
        #endregion
    }
}
