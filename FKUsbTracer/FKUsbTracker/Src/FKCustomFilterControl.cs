using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
//---------------------------------------------------------------------
namespace FKUsbTracer
{
    public enum FilterInclude
    {
        Include = 0,
        Exclude
    }

    public enum FilterType
    {
        Length = 0,
        Hex,
        Ascii
    }

    public enum LengthMatch
    {
        GreaterThen = 0,
        LessThen,
        EqualTo
    }

    public struct FilterMatch
    {
        public FilterType FilterType;
        public LengthMatch LengthMatch;
        public int Length;
        public string Filter;

        public FilterMatch(FilterType filterType, string filter, LengthMatch lengthMatch)
        {
            FilterType = filterType;
            Filter = filter;
            LengthMatch = lengthMatch;
            if (FilterType == FilterType.Length)
                Length = Convert.ToInt32(filter);
            else
                Length = 0;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            switch (FilterType)
            {
                case FilterType.Length:
                    sb.Append("消息长度 ");
                    switch (LengthMatch)
                    {
                        case LengthMatch.GreaterThen:
                            sb.Append("大于 ");
                            break;
                        case LengthMatch.LessThen:
                            sb.Append("小于 ");
                            break;
                        case LengthMatch.EqualTo:
                            sb.Append("等于 ");
                            break;
                    }
                    sb.Append(Filter);
                    break;
                case FilterType.Hex:
                case FilterType.Ascii:
                    sb.Append(Filter);
                    break;
            }

            return sb.ToString();
        }
    }

    class FKCustomFilterControl : UserControl
    {
        FilterInclude include = FilterInclude.Include;
        public FilterInclude Include
        {
            get { return include; }
            set
            {
                include = value;
                UpdateControls();
            }
        }

        FilterType type = FilterType.Length;
        public FilterType Type
        {
            get { return type; }
            set
            {
                type = value;
                UpdateControls();
            }
        }

        string filter = "";
        private GroupBox gbTraceListFilters;
        private ListBox lbExcludeFilters;
        private ListBox lbIncludeFilters;
        private Button btnAdd;
        private TextBox tbFilter;
        private ComboBox cbType;
        private ComboBox cbLengthMatch;
        private ComboBox cbInclude;

        public string Filter
        {
            get { return filter; }
            set
            {
                filter = value;
                UpdateControls();
            }
        }

        public List<FilterMatch> IncludeFilters
        {
            get
            {
                List<FilterMatch> filters = new List<FilterMatch>();
                foreach (object f in lbIncludeFilters.Items)
                    filters.Add((FilterMatch)f);
                return filters;
            }
        }

        public List<FilterMatch> ExcludeFilters
        {
            get
            {
                List<FilterMatch> filters = new List<FilterMatch>();
                foreach (object f in lbExcludeFilters.Items)
                    filters.Add((FilterMatch)f);
                return filters;
            }
        }

        LengthMatch lengthMatch = LengthMatch.GreaterThen;
        public LengthMatch LengthMatch_
        {
            get { return lengthMatch; }
            set
            {
                lengthMatch = value;
                UpdateControls();
            }
        }

        public FKCustomFilterControl()
        {
            InitializeComponent();

            UpdateControls();
        }

        private void UpdateControls()
        {
            SuspendLayout();

            cbInclude.SelectedIndex = (int)include;
            cbType.SelectedIndex = (int)type;
            cbLengthMatch.SelectedIndex = (int)lengthMatch;
            tbFilter.Text = filter;

            int originalTbFilterLeft = tbFilter.Left;
            if (type == FilterType.Length)
            {
                tbFilter.Left = cbLengthMatch.Left + cbLengthMatch.Width + 6;
                cbLengthMatch.Visible = true;
            }
            else
            {
                tbFilter.Left = 6;
                cbLengthMatch.Visible = false;
            }
            tbFilter.Width -= tbFilter.Left - originalTbFilterLeft;

            ResumeLayout(true);
        }

        private void cbInclude_SelectedIndexChanged(object sender, EventArgs e)
        {
            include = (FilterInclude)cbInclude.SelectedIndex;
            UpdateControls();
        }

        private void cbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            type = (FilterType)cbType.SelectedIndex;
            UpdateControls();
        }

        private void cbLengthMatch_SelectedIndexChanged(object sender, EventArgs e)
        {
            lengthMatch = (LengthMatch)cbLengthMatch.SelectedIndex;
            UpdateControls();
        }

        private void tbFilter_TextChanged(object sender, EventArgs e)
        {
            filter = tbFilter.Text;
            UpdateControls();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            FilterMatch ft = new FilterMatch(type, filter, lengthMatch);
            if (include == FilterInclude.Include)
                lbIncludeFilters.Items.Add(ft);
            else
                lbExcludeFilters.Items.Add(ft);
        }

        private void gbTraceListFilters_SizeChanged(object sender, EventArgs e)
        {
            SuspendLayout();

            int space = Height - lbIncludeFilters.Top - lbExcludeFilters.Margin.Bottom;
            lbIncludeFilters.Height = space / 2 - lbIncludeFilters.Margin.Bottom;
            lbExcludeFilters.Top = lbIncludeFilters.Bottom + lbExcludeFilters.Margin.Top;
            lbExcludeFilters.Height = lbIncludeFilters.Height;

            ResumeLayout(true);
        }

        private void lbIncludeFilters_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                DeleteSelectedFilter(lbIncludeFilters);
        }

        private void lbExcludeFilters_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                DeleteSelectedFilter(lbExcludeFilters);
        }

        private void DeleteSelectedFilter(ListBox lb)
        {
            if (lb.SelectedIndex >= 0)
            {
                int prevIndex = lb.SelectedIndex;
                lb.Items.RemoveAt(lb.SelectedIndex);
                if (prevIndex > 0)
                    lb.SelectedIndex = prevIndex - 1;
                else if (lb.Items.Count > 0)
                    lb.SelectedIndex = 0;
            }
        }

        private void InitializeComponent()
        {
            this.gbTraceListFilters = new System.Windows.Forms.GroupBox();
            this.lbExcludeFilters = new System.Windows.Forms.ListBox();
            this.lbIncludeFilters = new System.Windows.Forms.ListBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.tbFilter = new System.Windows.Forms.TextBox();
            this.cbType = new System.Windows.Forms.ComboBox();
            this.cbLengthMatch = new System.Windows.Forms.ComboBox();
            this.cbInclude = new System.Windows.Forms.ComboBox();
            this.gbTraceListFilters.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbTraceListFilters
            // 
            this.gbTraceListFilters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbTraceListFilters.Controls.Add(this.lbExcludeFilters);
            this.gbTraceListFilters.Controls.Add(this.lbIncludeFilters);
            this.gbTraceListFilters.Controls.Add(this.btnAdd);
            this.gbTraceListFilters.Controls.Add(this.tbFilter);
            this.gbTraceListFilters.Controls.Add(this.cbType);
            this.gbTraceListFilters.Controls.Add(this.cbLengthMatch);
            this.gbTraceListFilters.Controls.Add(this.cbInclude);
            this.gbTraceListFilters.Location = new System.Drawing.Point(3, 3);
            this.gbTraceListFilters.Name = "gbTraceListFilters";
            this.gbTraceListFilters.Size = new System.Drawing.Size(412, 221);
            this.gbTraceListFilters.TabIndex = 0;
            this.gbTraceListFilters.TabStop = false;
            this.gbTraceListFilters.Text = "信息筛选器";
            this.gbTraceListFilters.SizeChanged += new System.EventHandler(this.gbTraceListFilters_SizeChanged);
            // 
            // lbExcludeFilters
            // 
            this.lbExcludeFilters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbExcludeFilters.FormattingEnabled = true;
            this.lbExcludeFilters.ItemHeight = 12;
            this.lbExcludeFilters.Location = new System.Drawing.Point(7, 142);
            this.lbExcludeFilters.Name = "lbExcludeFilters";
            this.lbExcludeFilters.Size = new System.Drawing.Size(399, 64);
            this.lbExcludeFilters.TabIndex = 6;
            this.lbExcludeFilters.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lbExcludeFilters_KeyDown);
            // 
            // lbIncludeFilters
            // 
            this.lbIncludeFilters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbIncludeFilters.FormattingEnabled = true;
            this.lbIncludeFilters.ItemHeight = 12;
            this.lbIncludeFilters.Location = new System.Drawing.Point(7, 72);
            this.lbIncludeFilters.Name = "lbIncludeFilters";
            this.lbIncludeFilters.Size = new System.Drawing.Size(399, 64);
            this.lbIncludeFilters.TabIndex = 5;
            this.lbIncludeFilters.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lbIncludeFilters_KeyDown);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.Location = new System.Drawing.Point(336, 46);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(70, 23);
            this.btnAdd.TabIndex = 4;
            this.btnAdd.Text = "添加";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // tbFilter
            // 
            this.tbFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFilter.Location = new System.Drawing.Point(133, 46);
            this.tbFilter.Name = "tbFilter";
            this.tbFilter.Size = new System.Drawing.Size(197, 21);
            this.tbFilter.TabIndex = 3;
            this.tbFilter.TextChanged += new System.EventHandler(this.tbFilter_TextChanged);
            // 
            // cbType
            // 
            this.cbType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbType.FormattingEnabled = true;
            this.cbType.Items.AddRange(new object[] {
            "长度",
            "十六进制数据",
            "AscII数据"});
            this.cbType.Location = new System.Drawing.Point(133, 20);
            this.cbType.Name = "cbType";
            this.cbType.Size = new System.Drawing.Size(273, 20);
            this.cbType.TabIndex = 2;
            this.cbType.SelectedIndexChanged += new System.EventHandler(this.cbType_SelectedIndexChanged);
            // 
            // cbLengthMatch
            // 
            this.cbLengthMatch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLengthMatch.FormattingEnabled = true;
            this.cbLengthMatch.Items.AddRange(new object[] {
            "大于",
            "小于",
            "等于"});
            this.cbLengthMatch.Location = new System.Drawing.Point(6, 46);
            this.cbLengthMatch.Name = "cbLengthMatch";
            this.cbLengthMatch.Size = new System.Drawing.Size(121, 20);
            this.cbLengthMatch.TabIndex = 1;
            this.cbLengthMatch.SelectedIndexChanged += new System.EventHandler(this.cbLengthMatch_SelectedIndexChanged);
            // 
            // cbInclude
            // 
            this.cbInclude.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbInclude.FormattingEnabled = true;
            this.cbInclude.Items.AddRange(new object[] {
            "包含",
            "排除"});
            this.cbInclude.Location = new System.Drawing.Point(6, 20);
            this.cbInclude.Name = "cbInclude";
            this.cbInclude.Size = new System.Drawing.Size(121, 20);
            this.cbInclude.TabIndex = 0;
            this.cbInclude.SelectedIndexChanged += new System.EventHandler(this.cbInclude_SelectedIndexChanged);
            // 
            // FKCustomFilterControl
            // 
            this.Controls.Add(this.gbTraceListFilters);
            this.Name = "FKCustomFilterControl";
            this.Size = new System.Drawing.Size(418, 227);
            this.gbTraceListFilters.ResumeLayout(false);
            this.gbTraceListFilters.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}
