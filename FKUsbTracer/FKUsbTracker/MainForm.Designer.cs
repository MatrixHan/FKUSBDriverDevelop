namespace FKUsbTracer
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.MainTabControl = new System.Windows.Forms.TabControl();
            this.Device_TabPage = new System.Windows.Forms.TabPage();
            this.AutoTrace_CheckBox = new System.Windows.Forms.CheckBox();
            this.USBDevice_TreeView = new System.Windows.Forms.TreeView();
            this.Trace_TabPage = new System.Windows.Forms.TabPage();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnStartTraces = new System.Windows.Forms.ToolStripButton();
            this.btnClearTraces = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnCopyToClipboard = new System.Windows.Forms.ToolStripButton();
            this.btnSaveToFile = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.cbMaxTraces = new System.Windows.Forms.ToolStripComboBox();
            this.DataTrace_ListView = new FKUsbTracer.FKBufferedListView();
            this.chID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chLength = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chHex = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chAscii = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Setting_TabPage = new System.Windows.Forms.TabPage();
            this.fkFilterControl = new FKUsbTracer.FKCustomFilterControl();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbAscii = new System.Windows.Forms.CheckBox();
            this.cbHex = new System.Windows.Forms.CheckBox();
            this.cbLength = new System.Windows.Forms.CheckBox();
            this.cbTime = new System.Windows.Forms.CheckBox();
            this.cbType = new System.Windows.Forms.CheckBox();
            this.cbId = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Uninstall_Button = new System.Windows.Forms.Button();
            this.Reinstall_Button = new System.Windows.Forms.Button();
            this.tmrDeviceChange = new System.Windows.Forms.Timer(this.components);
            this.MainTabControl.SuspendLayout();
            this.Device_TabPage.SuspendLayout();
            this.Trace_TabPage.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.Setting_TabPage.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainTabControl
            // 
            this.MainTabControl.Controls.Add(this.Device_TabPage);
            this.MainTabControl.Controls.Add(this.Trace_TabPage);
            this.MainTabControl.Controls.Add(this.Setting_TabPage);
            this.MainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainTabControl.Location = new System.Drawing.Point(0, 0);
            this.MainTabControl.Name = "MainTabControl";
            this.MainTabControl.SelectedIndex = 0;
            this.MainTabControl.Size = new System.Drawing.Size(442, 423);
            this.MainTabControl.TabIndex = 0;
            this.MainTabControl.SelectedIndexChanged += new System.EventHandler(this.MainTabControl_SelectedIndexChanged);
            // 
            // Device_TabPage
            // 
            this.Device_TabPage.Controls.Add(this.AutoTrace_CheckBox);
            this.Device_TabPage.Controls.Add(this.USBDevice_TreeView);
            this.Device_TabPage.Location = new System.Drawing.Point(4, 22);
            this.Device_TabPage.Name = "Device_TabPage";
            this.Device_TabPage.Padding = new System.Windows.Forms.Padding(3);
            this.Device_TabPage.Size = new System.Drawing.Size(434, 397);
            this.Device_TabPage.TabIndex = 0;
            this.Device_TabPage.Text = "USB设备";
            this.Device_TabPage.UseVisualStyleBackColor = true;
            // 
            // AutoTrace_CheckBox
            // 
            this.AutoTrace_CheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AutoTrace_CheckBox.AutoSize = true;
            this.AutoTrace_CheckBox.Location = new System.Drawing.Point(6, 375);
            this.AutoTrace_CheckBox.Name = "AutoTrace_CheckBox";
            this.AutoTrace_CheckBox.Size = new System.Drawing.Size(150, 16);
            this.AutoTrace_CheckBox.TabIndex = 1;
            this.AutoTrace_CheckBox.Text = "自动跟踪新接入USB设备";
            this.AutoTrace_CheckBox.UseVisualStyleBackColor = true;
            this.AutoTrace_CheckBox.CheckedChanged += new System.EventHandler(this.AutoTrace_CheckBox_CheckedChanged);
            // 
            // USBDevice_TreeView
            // 
            this.USBDevice_TreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.USBDevice_TreeView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.USBDevice_TreeView.CheckBoxes = true;
            this.USBDevice_TreeView.Location = new System.Drawing.Point(3, 3);
            this.USBDevice_TreeView.Name = "USBDevice_TreeView";
            this.USBDevice_TreeView.Size = new System.Drawing.Size(428, 366);
            this.USBDevice_TreeView.TabIndex = 0;
            this.USBDevice_TreeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.USBDevice_TreeView_AfterCheck);
            // 
            // Trace_TabPage
            // 
            this.Trace_TabPage.Controls.Add(this.toolStrip1);
            this.Trace_TabPage.Controls.Add(this.DataTrace_ListView);
            this.Trace_TabPage.Location = new System.Drawing.Point(4, 22);
            this.Trace_TabPage.Name = "Trace_TabPage";
            this.Trace_TabPage.Padding = new System.Windows.Forms.Padding(3);
            this.Trace_TabPage.Size = new System.Drawing.Size(434, 397);
            this.Trace_TabPage.TabIndex = 1;
            this.Trace_TabPage.Text = "设备消息";
            this.Trace_TabPage.UseVisualStyleBackColor = true;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnStartTraces,
            this.btnClearTraces,
            this.toolStripSeparator1,
            this.btnCopyToClipboard,
            this.btnSaveToFile,
            this.toolStripSeparator2,
            this.toolStripLabel1,
            this.cbMaxTraces});
            this.toolStrip1.Location = new System.Drawing.Point(3, 3);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(428, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnStartTraces
            // 
            this.btnStartTraces.CheckOnClick = true;
            this.btnStartTraces.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnStartTraces.Image = ((System.Drawing.Image)(resources.GetObject("btnStartTraces.Image")));
            this.btnStartTraces.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStartTraces.Name = "btnStartTraces";
            this.btnStartTraces.Size = new System.Drawing.Size(23, 22);
            this.btnStartTraces.Text = "启动跟踪";
            this.btnStartTraces.Click += new System.EventHandler(this.btnStartTraces_Click);
            // 
            // btnClearTraces
            // 
            this.btnClearTraces.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnClearTraces.Image = ((System.Drawing.Image)(resources.GetObject("btnClearTraces.Image")));
            this.btnClearTraces.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnClearTraces.Name = "btnClearTraces";
            this.btnClearTraces.Size = new System.Drawing.Size(23, 22);
            this.btnClearTraces.Text = "清除数据";
            this.btnClearTraces.Click += new System.EventHandler(this.btnClearTraces_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnCopyToClipboard
            // 
            this.btnCopyToClipboard.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnCopyToClipboard.Image = ((System.Drawing.Image)(resources.GetObject("btnCopyToClipboard.Image")));
            this.btnCopyToClipboard.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCopyToClipboard.Name = "btnCopyToClipboard";
            this.btnCopyToClipboard.Size = new System.Drawing.Size(23, 22);
            this.btnCopyToClipboard.Text = "复制到剪切板";
            this.btnCopyToClipboard.Click += new System.EventHandler(this.btnCopyToClipboard_Click);
            // 
            // btnSaveToFile
            // 
            this.btnSaveToFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSaveToFile.Image = ((System.Drawing.Image)(resources.GetObject("btnSaveToFile.Image")));
            this.btnSaveToFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSaveToFile.Name = "btnSaveToFile";
            this.btnSaveToFile.Size = new System.Drawing.Size(23, 22);
            this.btnSaveToFile.Text = "保存到文件";
            this.btnSaveToFile.Click += new System.EventHandler(this.btnSaveToFile_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripLabel1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripLabel1.Image")));
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(16, 22);
            this.toolStripLabel1.Text = "最大显示行数";
            // 
            // cbMaxTraces
            // 
            this.cbMaxTraces.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMaxTraces.Items.AddRange(new object[] {
            "无限行",
            "100 行",
            "1000 行",
            "10000 行"});
            this.cbMaxTraces.Name = "cbMaxTraces";
            this.cbMaxTraces.Size = new System.Drawing.Size(121, 25);
            this.cbMaxTraces.SelectedIndexChanged += new System.EventHandler(this.cbMaxTraces_SelectedIndexChanged);
            // 
            // DataTrace_ListView
            // 
            this.DataTrace_ListView.AllowColumnReorder = true;
            this.DataTrace_ListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DataTrace_ListView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.DataTrace_ListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chID,
            this.chType,
            this.chTime,
            this.chLength,
            this.chHex,
            this.chAscii});
            this.DataTrace_ListView.FullRowSelect = true;
            this.DataTrace_ListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.DataTrace_ListView.Location = new System.Drawing.Point(6, 31);
            this.DataTrace_ListView.Name = "DataTrace_ListView";
            this.DataTrace_ListView.Size = new System.Drawing.Size(422, 363);
            this.DataTrace_ListView.TabIndex = 0;
            this.DataTrace_ListView.UseCompatibleStateImageBehavior = false;
            this.DataTrace_ListView.View = System.Windows.Forms.View.Details;
            // 
            // chID
            // 
            this.chID.Text = "ID";
            this.chID.Width = 30;
            // 
            // chType
            // 
            this.chType.Text = "类型";
            this.chType.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.chType.Width = 46;
            // 
            // chTime
            // 
            this.chTime.Text = "时间";
            this.chTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.chTime.Width = 79;
            // 
            // chLength
            // 
            this.chLength.Text = "长度";
            this.chLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.chLength.Width = 50;
            // 
            // chHex
            // 
            this.chHex.Text = "十六进制数据";
            this.chHex.Width = 100;
            // 
            // chAscii
            // 
            this.chAscii.Text = "AscII数据";
            this.chAscii.Width = 100;
            // 
            // Setting_TabPage
            // 
            this.Setting_TabPage.Controls.Add(this.fkFilterControl);
            this.Setting_TabPage.Controls.Add(this.groupBox2);
            this.Setting_TabPage.Controls.Add(this.groupBox1);
            this.Setting_TabPage.Location = new System.Drawing.Point(4, 22);
            this.Setting_TabPage.Name = "Setting_TabPage";
            this.Setting_TabPage.Size = new System.Drawing.Size(434, 397);
            this.Setting_TabPage.TabIndex = 2;
            this.Setting_TabPage.Text = "软件设置";
            this.Setting_TabPage.UseVisualStyleBackColor = true;
            // 
            // fkFilterControl
            // 
            this.fkFilterControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fkFilterControl.Filter = "";
            this.fkFilterControl.Include = FKUsbTracer.FilterInclude.Include;
            this.fkFilterControl.LengthMatch_ = FKUsbTracer.LengthMatch.GreaterThen;
            this.fkFilterControl.Location = new System.Drawing.Point(8, 162);
            this.fkFilterControl.Name = "fkFilterControl";
            this.fkFilterControl.Size = new System.Drawing.Size(418, 227);
            this.fkFilterControl.TabIndex = 2;
            this.fkFilterControl.Type = FKUsbTracer.FilterType.Length;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.cbAscii);
            this.groupBox2.Controls.Add(this.cbHex);
            this.groupBox2.Controls.Add(this.cbLength);
            this.groupBox2.Controls.Add(this.cbTime);
            this.groupBox2.Controls.Add(this.cbType);
            this.groupBox2.Controls.Add(this.cbId);
            this.groupBox2.Location = new System.Drawing.Point(8, 68);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(418, 88);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "显示的消息列";
            // 
            // cbAscii
            // 
            this.cbAscii.AutoSize = true;
            this.cbAscii.Checked = true;
            this.cbAscii.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAscii.Location = new System.Drawing.Point(164, 64);
            this.cbAscii.Name = "cbAscii";
            this.cbAscii.Size = new System.Drawing.Size(78, 16);
            this.cbAscii.TabIndex = 5;
            this.cbAscii.Text = "AscII数据";
            this.cbAscii.UseVisualStyleBackColor = true;
            this.cbAscii.CheckedChanged += new System.EventHandler(this.cbTraceListColumn_CheckedChanged);
            // 
            // cbHex
            // 
            this.cbHex.AutoSize = true;
            this.cbHex.Checked = true;
            this.cbHex.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbHex.Location = new System.Drawing.Point(164, 42);
            this.cbHex.Name = "cbHex";
            this.cbHex.Size = new System.Drawing.Size(96, 16);
            this.cbHex.TabIndex = 4;
            this.cbHex.Text = "十六进制数据";
            this.cbHex.UseVisualStyleBackColor = true;
            this.cbHex.CheckedChanged += new System.EventHandler(this.cbTraceListColumn_CheckedChanged);
            // 
            // cbLength
            // 
            this.cbLength.AutoSize = true;
            this.cbLength.Checked = true;
            this.cbLength.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbLength.Location = new System.Drawing.Point(164, 20);
            this.cbLength.Name = "cbLength";
            this.cbLength.Size = new System.Drawing.Size(48, 16);
            this.cbLength.TabIndex = 3;
            this.cbLength.Text = "长度";
            this.cbLength.UseVisualStyleBackColor = true;
            this.cbLength.CheckedChanged += new System.EventHandler(this.cbTraceListColumn_CheckedChanged);
            // 
            // cbTime
            // 
            this.cbTime.AutoSize = true;
            this.cbTime.Checked = true;
            this.cbTime.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbTime.Location = new System.Drawing.Point(16, 64);
            this.cbTime.Name = "cbTime";
            this.cbTime.Size = new System.Drawing.Size(48, 16);
            this.cbTime.TabIndex = 2;
            this.cbTime.Text = "时间";
            this.cbTime.UseVisualStyleBackColor = true;
            this.cbTime.CheckedChanged += new System.EventHandler(this.cbTraceListColumn_CheckedChanged);
            // 
            // cbType
            // 
            this.cbType.AutoSize = true;
            this.cbType.Checked = true;
            this.cbType.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbType.Location = new System.Drawing.Point(16, 42);
            this.cbType.Name = "cbType";
            this.cbType.Size = new System.Drawing.Size(48, 16);
            this.cbType.TabIndex = 1;
            this.cbType.Text = "类型";
            this.cbType.UseVisualStyleBackColor = true;
            this.cbType.CheckedChanged += new System.EventHandler(this.cbTraceListColumn_CheckedChanged);
            // 
            // cbId
            // 
            this.cbId.AutoSize = true;
            this.cbId.Checked = true;
            this.cbId.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbId.Location = new System.Drawing.Point(16, 20);
            this.cbId.Name = "cbId";
            this.cbId.Size = new System.Drawing.Size(36, 16);
            this.cbId.TabIndex = 0;
            this.cbId.Text = "ID";
            this.cbId.UseVisualStyleBackColor = true;
            this.cbId.CheckedChanged += new System.EventHandler(this.cbTraceListColumn_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.Uninstall_Button);
            this.groupBox1.Controls.Add(this.Reinstall_Button);
            this.groupBox1.Location = new System.Drawing.Point(8, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(418, 59);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "驱动管理";
            // 
            // Uninstall_Button
            // 
            this.Uninstall_Button.Location = new System.Drawing.Point(164, 22);
            this.Uninstall_Button.Name = "Uninstall_Button";
            this.Uninstall_Button.Size = new System.Drawing.Size(127, 23);
            this.Uninstall_Button.TabIndex = 1;
            this.Uninstall_Button.Text = "卸载软件驱动";
            this.Uninstall_Button.UseVisualStyleBackColor = true;
            this.Uninstall_Button.Click += new System.EventHandler(this.Uninstall_Button_Click);
            // 
            // Reinstall_Button
            // 
            this.Reinstall_Button.Location = new System.Drawing.Point(16, 22);
            this.Reinstall_Button.Name = "Reinstall_Button";
            this.Reinstall_Button.Size = new System.Drawing.Size(127, 23);
            this.Reinstall_Button.TabIndex = 0;
            this.Reinstall_Button.Text = "重新安装软件驱动";
            this.Reinstall_Button.UseVisualStyleBackColor = true;
            this.Reinstall_Button.Click += new System.EventHandler(this.Reinstall_Button_Click);
            // 
            // tmrDeviceChange
            // 
            this.tmrDeviceChange.Interval = 1000;
            this.tmrDeviceChange.Tick += new System.EventHandler(this.tmrDeviceChange_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(442, 423);
            this.Controls.Add(this.MainTabControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "FKUsbTracer";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.MainTabControl.ResumeLayout(false);
            this.Device_TabPage.ResumeLayout(false);
            this.Device_TabPage.PerformLayout();
            this.Trace_TabPage.ResumeLayout(false);
            this.Trace_TabPage.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.Setting_TabPage.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl MainTabControl;
        private System.Windows.Forms.TabPage Device_TabPage;
        private System.Windows.Forms.TabPage Trace_TabPage;
        private System.Windows.Forms.CheckBox AutoTrace_CheckBox;
        private System.Windows.Forms.TreeView USBDevice_TreeView;
        private System.Windows.Forms.TabPage Setting_TabPage;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button Uninstall_Button;
        private System.Windows.Forms.Button Reinstall_Button;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private FKBufferedListView DataTrace_ListView;
        private System.Windows.Forms.ColumnHeader chID;
        private System.Windows.Forms.ColumnHeader chType;
        private System.Windows.Forms.ColumnHeader chTime;
        private System.Windows.Forms.ColumnHeader chLength;
        private System.Windows.Forms.ColumnHeader chHex;
        private System.Windows.Forms.ColumnHeader chAscii;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox cbAscii;
        private System.Windows.Forms.CheckBox cbHex;
        private System.Windows.Forms.CheckBox cbLength;
        private System.Windows.Forms.CheckBox cbTime;
        private System.Windows.Forms.CheckBox cbType;
        private System.Windows.Forms.CheckBox cbId;
        private FKCustomFilterControl fkFilterControl;
        private System.Windows.Forms.ToolStripButton btnStartTraces;
        private System.Windows.Forms.ToolStripButton btnClearTraces;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnCopyToClipboard;
        private System.Windows.Forms.ToolStripButton btnSaveToFile;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox cbMaxTraces;
        private System.Windows.Forms.Timer tmrDeviceChange;
    }
}

