namespace K12.Behavior.Keyboard
{
    partial class RtAttendanceKBInput
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lbHelp1 = new DevComponents.DotNetBar.LabelX();
            this.btnClose = new DevComponents.DotNetBar.ButtonX();
            this.gpAbsence = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.cpAbsence = new SmartSchool.Common.CardPanelEx();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lbItem1DateTime2 = new DevComponents.DotNetBar.LabelX();
            this.lbItem1Semester = new DevComponents.DotNetBar.LabelX();
            this.txtItem1DateTime = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.lbItem1DateTime = new DevComponents.DotNetBar.LabelX();
            this.lbItem1SchoolYear = new DevComponents.DotNetBar.LabelX();
            this.btnItem1Insert = new DevComponents.DotNetBar.ButtonX();
            this.groupPanel1 = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.cbBoxItem1Semester = new DevComponents.Editors.IntegerInput();
            this.cbBoxItem1SchoolYear = new DevComponents.Editors.IntegerInput();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.btnSetClassNameCode = new DevComponents.DotNetBar.ButtonX();
            this.dgv = new K12.Behavior.Keyboard.CustomDataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.gpAbsence.SuspendLayout();
            this.groupPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbBoxItem1Semester)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbBoxItem1SchoolYear)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.SuspendLayout();
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "班級";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            // 
            // lbHelp1
            // 
            this.lbHelp1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbHelp1.AutoSize = true;
            this.lbHelp1.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.lbHelp1.BackgroundStyle.Class = "";
            this.lbHelp1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lbHelp1.Font = new System.Drawing.Font("微軟正黑體", 9.75F);
            this.lbHelp1.Location = new System.Drawing.Point(23, 465);
            this.lbHelp1.Name = "lbHelp1";
            this.lbHelp1.Size = new System.Drawing.Size(437, 73);
            this.lbHelp1.TabIndex = 4;
            this.lbHelp1.Text = "1.Alt+假別熱鍵(填入熱鍵假別)，Alt+A(清空全部假別)，Alt+S(即時儲存)\r\n2.將節次內容均清空後進行儲存(Alt+S)將會刪除該筆缺曠記錄\r\n3" +
    ".\"下\"鍵新增為<即時儲存>功能(20130925-new)\r\n4.新增一行時，會取\"批次設定\"內容為新行之預設內容";
            // 
            // btnClose
            // 
            this.btnClose.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnClose.Font = new System.Drawing.Font("微軟正黑體", 9.75F);
            this.btnClose.Location = new System.Drawing.Point(675, 509);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(110, 25);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "關閉(&X)";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // gpAbsence
            // 
            this.gpAbsence.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.gpAbsence.BackColor = System.Drawing.Color.Transparent;
            this.gpAbsence.CanvasColor = System.Drawing.SystemColors.Control;
            this.gpAbsence.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.gpAbsence.Controls.Add(this.cpAbsence);
            this.gpAbsence.Font = new System.Drawing.Font("微軟正黑體", 9.75F);
            this.gpAbsence.Location = new System.Drawing.Point(543, 10);
            this.gpAbsence.Name = "gpAbsence";
            this.gpAbsence.Size = new System.Drawing.Size(242, 119);
            // 
            // 
            // 
            this.gpAbsence.Style.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.gpAbsence.Style.BackColorGradientAngle = 90;
            this.gpAbsence.Style.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.gpAbsence.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpAbsence.Style.BorderBottomWidth = 1;
            this.gpAbsence.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.gpAbsence.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpAbsence.Style.BorderLeftWidth = 1;
            this.gpAbsence.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpAbsence.Style.BorderRightWidth = 1;
            this.gpAbsence.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpAbsence.Style.BorderTopWidth = 1;
            this.gpAbsence.Style.Class = "";
            this.gpAbsence.Style.CornerDiameter = 4;
            this.gpAbsence.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.gpAbsence.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.gpAbsence.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            // 
            // 
            // 
            this.gpAbsence.StyleMouseDown.Class = "";
            this.gpAbsence.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.gpAbsence.StyleMouseOver.Class = "";
            this.gpAbsence.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.gpAbsence.TabIndex = 3;
            this.gpAbsence.Text = "假別熱鍵";
            // 
            // cpAbsence
            // 
            this.cpAbsence.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cpAbsence.AutoScroll = true;
            this.cpAbsence.CardWidth = 90;
            this.cpAbsence.Location = new System.Drawing.Point(3, 3);
            this.cpAbsence.MinWidth = 2;
            this.cpAbsence.Name = "cpAbsence";
            this.cpAbsence.Size = new System.Drawing.Size(230, 86);
            this.cpAbsence.TabIndex = 0;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "座號";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.Width = 70;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "學號";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.Width = 90;
            // 
            // dataGridViewTextBoxColumn4
            // 
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.LightCyan;
            this.dataGridViewTextBoxColumn4.DefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridViewTextBoxColumn4.HeaderText = "姓名";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.Width = 110;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.HeaderText = "請假日期";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.HeaderText = "假別";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            // 
            // dataGridViewTextBoxColumn7
            // 
            this.dataGridViewTextBoxColumn7.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn7.HeaderText = "節次";
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            // 
            // lbItem1DateTime2
            // 
            this.lbItem1DateTime2.AutoSize = true;
            this.lbItem1DateTime2.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.lbItem1DateTime2.BackgroundStyle.Class = "";
            this.lbItem1DateTime2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lbItem1DateTime2.Font = new System.Drawing.Font("微軟正黑體", 9.75F);
            this.lbItem1DateTime2.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lbItem1DateTime2.Location = new System.Drawing.Point(324, 37);
            this.lbItem1DateTime2.Name = "lbItem1DateTime2";
            this.lbItem1DateTime2.Size = new System.Drawing.Size(96, 21);
            this.lbItem1DateTime2.TabIndex = 2;
            this.lbItem1DateTime2.Text = "例：20090607";
            // 
            // lbItem1Semester
            // 
            this.lbItem1Semester.AutoSize = true;
            this.lbItem1Semester.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.lbItem1Semester.BackgroundStyle.Class = "";
            this.lbItem1Semester.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lbItem1Semester.Font = new System.Drawing.Font("微軟正黑體", 9.75F);
            this.lbItem1Semester.Location = new System.Drawing.Point(178, 9);
            this.lbItem1Semester.Name = "lbItem1Semester";
            this.lbItem1Semester.Size = new System.Drawing.Size(81, 21);
            this.lbItem1Semester.TabIndex = 7;
            this.lbItem1Semester.Text = "學期：(&W)";
            // 
            // txtItem1DateTime
            // 
            // 
            // 
            // 
            this.txtItem1DateTime.Border.Class = "TextBoxBorder";
            this.txtItem1DateTime.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtItem1DateTime.Location = new System.Drawing.Point(118, 35);
            this.txtItem1DateTime.Name = "txtItem1DateTime";
            this.txtItem1DateTime.Size = new System.Drawing.Size(201, 25);
            this.txtItem1DateTime.TabIndex = 1;
            this.txtItem1DateTime.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtItem1DateTime_KeyUp);
            // 
            // lbItem1DateTime
            // 
            this.lbItem1DateTime.AutoSize = true;
            this.lbItem1DateTime.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.lbItem1DateTime.BackgroundStyle.Class = "";
            this.lbItem1DateTime.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lbItem1DateTime.Font = new System.Drawing.Font("微軟正黑體", 9.75F);
            this.lbItem1DateTime.Location = new System.Drawing.Point(14, 37);
            this.lbItem1DateTime.Name = "lbItem1DateTime";
            this.lbItem1DateTime.Size = new System.Drawing.Size(101, 21);
            this.lbItem1DateTime.TabIndex = 0;
            this.lbItem1DateTime.Text = "請假日期：(&E)";
            // 
            // lbItem1SchoolYear
            // 
            this.lbItem1SchoolYear.AutoSize = true;
            this.lbItem1SchoolYear.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.lbItem1SchoolYear.BackgroundStyle.Class = "";
            this.lbItem1SchoolYear.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lbItem1SchoolYear.Font = new System.Drawing.Font("微軟正黑體", 9.75F);
            this.lbItem1SchoolYear.Location = new System.Drawing.Point(15, 9);
            this.lbItem1SchoolYear.Name = "lbItem1SchoolYear";
            this.lbItem1SchoolYear.Size = new System.Drawing.Size(101, 21);
            this.lbItem1SchoolYear.TabIndex = 5;
            this.lbItem1SchoolYear.Text = "學 年 度 ：(&Q)";
            // 
            // btnItem1Insert
            // 
            this.btnItem1Insert.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnItem1Insert.BackColor = System.Drawing.Color.Transparent;
            this.btnItem1Insert.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnItem1Insert.Font = new System.Drawing.Font("微軟正黑體", 9.75F);
            this.btnItem1Insert.Location = new System.Drawing.Point(436, 64);
            this.btnItem1Insert.Name = "btnItem1Insert";
            this.btnItem1Insert.Size = new System.Drawing.Size(71, 23);
            this.btnItem1Insert.TabIndex = 4;
            this.btnItem1Insert.Text = "新增(&R)";
            this.btnItem1Insert.Click += new System.EventHandler(this.btnItem1Insert_Click);
            // 
            // groupPanel1
            // 
            this.groupPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupPanel1.BackColor = System.Drawing.Color.Transparent;
            this.groupPanel1.CanvasColor = System.Drawing.SystemColors.Control;
            this.groupPanel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.groupPanel1.Controls.Add(this.cbBoxItem1Semester);
            this.groupPanel1.Controls.Add(this.cbBoxItem1SchoolYear);
            this.groupPanel1.Controls.Add(this.txtItem1DateTime);
            this.groupPanel1.Controls.Add(this.labelX2);
            this.groupPanel1.Controls.Add(this.labelX1);
            this.groupPanel1.Controls.Add(this.lbItem1DateTime2);
            this.groupPanel1.Controls.Add(this.btnItem1Insert);
            this.groupPanel1.Controls.Add(this.lbItem1SchoolYear);
            this.groupPanel1.Controls.Add(this.lbItem1DateTime);
            this.groupPanel1.Controls.Add(this.lbItem1Semester);
            this.groupPanel1.Font = new System.Drawing.Font("微軟正黑體", 9.75F);
            this.groupPanel1.Location = new System.Drawing.Point(8, 10);
            this.groupPanel1.Name = "groupPanel1";
            this.groupPanel1.Size = new System.Drawing.Size(529, 119);
            // 
            // 
            // 
            this.groupPanel1.Style.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.groupPanel1.Style.BackColorGradientAngle = 90;
            this.groupPanel1.Style.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.groupPanel1.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel1.Style.BorderBottomWidth = 1;
            this.groupPanel1.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.groupPanel1.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel1.Style.BorderLeftWidth = 1;
            this.groupPanel1.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel1.Style.BorderRightWidth = 1;
            this.groupPanel1.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel1.Style.BorderTopWidth = 1;
            this.groupPanel1.Style.Class = "";
            this.groupPanel1.Style.CornerDiameter = 4;
            this.groupPanel1.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.groupPanel1.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.groupPanel1.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            // 
            // 
            // 
            this.groupPanel1.StyleMouseDown.Class = "";
            this.groupPanel1.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.groupPanel1.StyleMouseOver.Class = "";
            this.groupPanel1.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.groupPanel1.TabIndex = 0;
            this.groupPanel1.Text = "批次設定";
            // 
            // cbBoxItem1Semester
            // 
            // 
            // 
            // 
            this.cbBoxItem1Semester.BackgroundStyle.Class = "DateTimeInputBackground";
            this.cbBoxItem1Semester.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.cbBoxItem1Semester.ButtonFreeText.Shortcut = DevComponents.DotNetBar.eShortcut.F2;
            this.cbBoxItem1Semester.Location = new System.Drawing.Point(261, 7);
            this.cbBoxItem1Semester.MaxValue = 2;
            this.cbBoxItem1Semester.MinValue = 1;
            this.cbBoxItem1Semester.Name = "cbBoxItem1Semester";
            this.cbBoxItem1Semester.ShowUpDown = true;
            this.cbBoxItem1Semester.Size = new System.Drawing.Size(58, 25);
            this.cbBoxItem1Semester.TabIndex = 8;
            this.cbBoxItem1Semester.Value = 1;
            this.cbBoxItem1Semester.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cbBoxItem1Semester_KeyUp);
            // 
            // cbBoxItem1SchoolYear
            // 
            // 
            // 
            // 
            this.cbBoxItem1SchoolYear.BackgroundStyle.Class = "DateTimeInputBackground";
            this.cbBoxItem1SchoolYear.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.cbBoxItem1SchoolYear.ButtonFreeText.Shortcut = DevComponents.DotNetBar.eShortcut.F2;
            this.cbBoxItem1SchoolYear.Location = new System.Drawing.Point(118, 7);
            this.cbBoxItem1SchoolYear.MaxValue = 999;
            this.cbBoxItem1SchoolYear.MinValue = 90;
            this.cbBoxItem1SchoolYear.Name = "cbBoxItem1SchoolYear";
            this.cbBoxItem1SchoolYear.ShowUpDown = true;
            this.cbBoxItem1SchoolYear.Size = new System.Drawing.Size(58, 25);
            this.cbBoxItem1SchoolYear.TabIndex = 6;
            this.cbBoxItem1SchoolYear.Value = 90;
            this.cbBoxItem1SchoolYear.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cbBoxItem1SchoolYear_KeyUp);
            // 
            // labelX2
            // 
            this.labelX2.AutoSize = true;
            // 
            // 
            // 
            this.labelX2.BackgroundStyle.Class = "";
            this.labelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX2.Font = new System.Drawing.Font("微軟正黑體", 9.75F);
            this.labelX2.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.labelX2.Location = new System.Drawing.Point(14, 65);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(372, 21);
            this.labelX2.TabIndex = 3;
            this.labelX2.Text = "假別熱鍵：於資料欄位內鍵入Alt+假別熱鍵,即可填滿特定假別";
            // 
            // labelX1
            // 
            this.labelX1.AutoSize = true;
            this.labelX1.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.Class = "";
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Font = new System.Drawing.Font("微軟正黑體", 9.75F);
            this.labelX1.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.labelX1.Location = new System.Drawing.Point(321, 9);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(173, 21);
            this.labelX1.TabIndex = 9;
            this.labelX1.Text = "預設為系統目前學年度/學期";
            // 
            // btnSetClassNameCode
            // 
            this.btnSetClassNameCode.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnSetClassNameCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSetClassNameCode.AutoSize = true;
            this.btnSetClassNameCode.BackColor = System.Drawing.Color.Transparent;
            this.btnSetClassNameCode.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnSetClassNameCode.Location = new System.Drawing.Point(411, 10);
            this.btnSetClassNameCode.Name = "btnSetClassNameCode";
            this.btnSetClassNameCode.Size = new System.Drawing.Size(107, 27);
            this.btnSetClassNameCode.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnSetClassNameCode.TabIndex = 1;
            this.btnSetClassNameCode.Text = "班級名稱代碼表";
            this.btnSetClassNameCode.Click += new System.EventHandler(this.btnSetClassNameCode_Click);
            // 
            // dgv
            // 
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToResizeRows = false;
            this.dgv.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgv.BackgroundColor = System.Drawing.Color.White;
            this.dgv.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4,
            this.Column5,
            this.Column8,
            this.Column9});
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgv.DefaultCellStyle = dataGridViewCellStyle5;
            this.dgv.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.dgv.Location = new System.Drawing.Point(8, 135);
            this.dgv.MultiSelect = false;
            this.dgv.Name = "dgv";
            this.dgv.RowHeadersWidth = 35;
            this.dgv.RowTemplate.Height = 24;
            this.dgv.Size = new System.Drawing.Size(777, 324);
            this.dgv.TabIndex = 2;
            this.dgv.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_CellEndEdit);
            this.dgv.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dgv_KeyUp);
            // 
            // Column1
            // 
            this.Column1.HeaderText = "班級";
            this.Column1.Name = "Column1";
            this.Column1.Width = 70;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "座號";
            this.Column2.Name = "Column2";
            this.Column2.Width = 60;
            // 
            // Column3
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.LightSkyBlue;
            this.Column3.DefaultCellStyle = dataGridViewCellStyle1;
            this.Column3.HeaderText = "學號";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.Width = 80;
            // 
            // Column4
            // 
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.LightSkyBlue;
            this.Column4.DefaultCellStyle = dataGridViewCellStyle2;
            this.Column4.HeaderText = "姓名";
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            this.Column4.Width = 80;
            // 
            // Column5
            // 
            this.Column5.HeaderText = "請假日期";
            this.Column5.Name = "Column5";
            // 
            // Column8
            // 
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.LightSkyBlue;
            this.Column8.DefaultCellStyle = dataGridViewCellStyle3;
            this.Column8.HeaderText = "學年度";
            this.Column8.Name = "Column8";
            this.Column8.ReadOnly = true;
            this.Column8.Width = 70;
            // 
            // Column9
            // 
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.LightSkyBlue;
            this.Column9.DefaultCellStyle = dataGridViewCellStyle4;
            this.Column9.HeaderText = "學期";
            this.Column9.Name = "Column9";
            this.Column9.ReadOnly = true;
            this.Column9.Width = 80;
            // 
            // RtAttendanceKBInput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 546);
            this.Controls.Add(this.btnSetClassNameCode);
            this.Controls.Add(this.lbHelp1);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.gpAbsence);
            this.Controls.Add(this.dgv);
            this.Controls.Add(this.groupPanel1);
            this.DoubleBuffered = true;
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.Name = "RtAttendanceKBInput";
            this.Text = "缺曠鍵盤登錄(測試版本)";
            this.Load += new System.EventHandler(this.RtAttendanceKBInput_Load);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.gpAbsence.ResumeLayout(false);
            this.groupPanel1.ResumeLayout(false);
            this.groupPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbBoxItem1Semester)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbBoxItem1SchoolYear)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ErrorProvider errorProvider1;
        private DevComponents.DotNetBar.LabelX lbHelp1;
        private DevComponents.DotNetBar.ButtonX btnClose;
        private DevComponents.DotNetBar.Controls.GroupPanel gpAbsence;
        private SmartSchool.Common.CardPanelEx cpAbsence;
        private Behavior.Keyboard.CustomDataGridView dgv;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private DevComponents.DotNetBar.Controls.GroupPanel groupPanel1;
        private DevComponents.DotNetBar.Controls.TextBoxX txtItem1DateTime;
        private DevComponents.DotNetBar.LabelX lbItem1DateTime2;
        private DevComponents.DotNetBar.ButtonX btnItem1Insert;
        private DevComponents.DotNetBar.LabelX lbItem1SchoolYear;
        private DevComponents.DotNetBar.LabelX lbItem1DateTime;
        private DevComponents.DotNetBar.LabelX lbItem1Semester;
        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.LabelX labelX2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column8;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column9;
        private DevComponents.DotNetBar.ButtonX btnSetClassNameCode;
        private DevComponents.Editors.IntegerInput cbBoxItem1Semester;
        private DevComponents.Editors.IntegerInput cbBoxItem1SchoolYear;
    }
}