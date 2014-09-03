namespace K12.Behavior.DisciplineInput
{
    partial class InputDateSettingForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvTimes = new DevComponents.DotNetBar.Controls.DataGridViewX();
            this.chStartTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chEndTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblSemester = new DevComponents.DotNetBar.LabelX();
            this.btnClose = new DevComponents.DotNetBar.ButtonX();
            this.btnSave = new DevComponents.DotNetBar.ButtonX();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cbMeritA = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.cbMeritB = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.cbMeritC = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.cbDemeritA = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.cbDemeritB = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.cbDemeritC = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.groupPanel1 = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.checkBoxX1 = new DevComponents.DotNetBar.Controls.CheckBoxX();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTimes)).BeginInit();
            this.groupPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvTimes
            // 
            this.dgvTimes.AllowUserToAddRows = false;
            this.dgvTimes.AllowUserToDeleteRows = false;
            this.dgvTimes.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvTimes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTimes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.chStartTime,
            this.chEndTime});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvTimes.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgvTimes.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.dgvTimes.Location = new System.Drawing.Point(13, 38);
            this.dgvTimes.Name = "dgvTimes";
            this.dgvTimes.RowTemplate.Height = 24;
            this.dgvTimes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvTimes.Size = new System.Drawing.Size(381, 54);
            this.dgvTimes.TabIndex = 0;
            this.dgvTimes.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTimes_CellEndEdit);
            this.dgvTimes.RowValidating += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgvTimes_RowValidating);
            // 
            // chStartTime
            // 
            this.chStartTime.HeaderText = "開始時間";
            this.chStartTime.Name = "chStartTime";
            this.chStartTime.Width = 150;
            // 
            // chEndTime
            // 
            this.chEndTime.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.chEndTime.HeaderText = "截止時間";
            this.chEndTime.Name = "chEndTime";
            // 
            // lblSemester
            // 
            this.lblSemester.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.lblSemester.BackgroundStyle.Class = "";
            this.lblSemester.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lblSemester.Font = new System.Drawing.Font("微軟正黑體", 9.75F);
            this.lblSemester.Location = new System.Drawing.Point(13, 9);
            this.lblSemester.Name = "lblSemester";
            this.lblSemester.Size = new System.Drawing.Size(142, 23);
            this.lblSemester.TabIndex = 1;
            this.lblSemester.Text = "97學年度　第１學期";
            // 
            // btnClose
            // 
            this.btnClose.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnClose.Font = new System.Drawing.Font("微軟正黑體", 9.75F);
            this.btnClose.Location = new System.Drawing.Point(319, 183);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "離開";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnSave
            // 
            this.btnSave.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnSave.BackColor = System.Drawing.Color.Transparent;
            this.btnSave.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnSave.Font = new System.Drawing.Font("微軟正黑體", 9.75F);
            this.btnSave.Location = new System.Drawing.Point(238, 183);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "儲存";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
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
            this.labelX1.Location = new System.Drawing.Point(190, 12);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(204, 21);
            this.labelX1.TabIndex = 3;
            this.labelX1.Text = "時間格式範例：2012/6/18 13:00";
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "年級";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "開始時間";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.Width = 150;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn3.HeaderText = "截止時間";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            // 
            // cbMeritA
            // 
            // 
            // 
            // 
            this.cbMeritA.BackgroundStyle.Class = "";
            this.cbMeritA.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.cbMeritA.Location = new System.Drawing.Point(4, 4);
            this.cbMeritA.Name = "cbMeritA";
            this.cbMeritA.Size = new System.Drawing.Size(100, 23);
            this.cbMeritA.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cbMeritA.TabIndex = 0;
            this.cbMeritA.Text = "大功";
            // 
            // cbMeritB
            // 
            // 
            // 
            // 
            this.cbMeritB.BackgroundStyle.Class = "";
            this.cbMeritB.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.cbMeritB.Location = new System.Drawing.Point(145, 4);
            this.cbMeritB.Name = "cbMeritB";
            this.cbMeritB.Size = new System.Drawing.Size(100, 23);
            this.cbMeritB.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cbMeritB.TabIndex = 1;
            this.cbMeritB.Text = "小功";
            // 
            // cbMeritC
            // 
            // 
            // 
            // 
            this.cbMeritC.BackgroundStyle.Class = "";
            this.cbMeritC.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.cbMeritC.Location = new System.Drawing.Point(287, 4);
            this.cbMeritC.Name = "cbMeritC";
            this.cbMeritC.Size = new System.Drawing.Size(100, 23);
            this.cbMeritC.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cbMeritC.TabIndex = 2;
            this.cbMeritC.Text = "嘉獎";
            // 
            // cbDemeritA
            // 
            // 
            // 
            // 
            this.cbDemeritA.BackgroundStyle.Class = "";
            this.cbDemeritA.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.cbDemeritA.Location = new System.Drawing.Point(4, 25);
            this.cbDemeritA.Name = "cbDemeritA";
            this.cbDemeritA.Size = new System.Drawing.Size(100, 23);
            this.cbDemeritA.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cbDemeritA.TabIndex = 3;
            this.cbDemeritA.Text = "大過";
            // 
            // cbDemeritB
            // 
            // 
            // 
            // 
            this.cbDemeritB.BackgroundStyle.Class = "";
            this.cbDemeritB.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.cbDemeritB.Location = new System.Drawing.Point(145, 25);
            this.cbDemeritB.Name = "cbDemeritB";
            this.cbDemeritB.Size = new System.Drawing.Size(100, 23);
            this.cbDemeritB.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cbDemeritB.TabIndex = 4;
            this.cbDemeritB.Text = "小過";
            // 
            // cbDemeritC
            // 
            // 
            // 
            // 
            this.cbDemeritC.BackgroundStyle.Class = "";
            this.cbDemeritC.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.cbDemeritC.Location = new System.Drawing.Point(287, 25);
            this.cbDemeritC.Name = "cbDemeritC";
            this.cbDemeritC.Size = new System.Drawing.Size(100, 23);
            this.cbDemeritC.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cbDemeritC.TabIndex = 5;
            this.cbDemeritC.Text = "警告";
            // 
            // groupPanel1
            // 
            this.groupPanel1.BackColor = System.Drawing.Color.Transparent;
            this.groupPanel1.CanvasColor = System.Drawing.SystemColors.Control;
            this.groupPanel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.groupPanel1.Controls.Add(this.cbDemeritC);
            this.groupPanel1.Controls.Add(this.cbDemeritB);
            this.groupPanel1.Controls.Add(this.cbDemeritA);
            this.groupPanel1.Controls.Add(this.cbMeritC);
            this.groupPanel1.Controls.Add(this.cbMeritB);
            this.groupPanel1.Controls.Add(this.cbMeritA);
            this.groupPanel1.Location = new System.Drawing.Point(13, 99);
            this.groupPanel1.Name = "groupPanel1";
            this.groupPanel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.groupPanel1.Size = new System.Drawing.Size(381, 78);
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
            this.groupPanel1.Style.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center;
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
            this.groupPanel1.TabIndex = 4;
            this.groupPanel1.Text = "可輸入欄位";
            // 
            // checkBoxX1
            // 
            this.checkBoxX1.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.checkBoxX1.BackgroundStyle.Class = "";
            this.checkBoxX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.checkBoxX1.Location = new System.Drawing.Point(20, 182);
            this.checkBoxX1.Name = "checkBoxX1";
            this.checkBoxX1.Size = new System.Drawing.Size(100, 23);
            this.checkBoxX1.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.checkBoxX1.TabIndex = 6;
            this.checkBoxX1.Text = "全選";
            this.checkBoxX1.CheckedChanged += new System.EventHandler(this.checkBoxX1_CheckedChanged);
            // 
            // InputDateSettingForm
            // 
            this.AcceptButton = this.btnClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnSave;
            this.ClientSize = new System.Drawing.Size(406, 217);
            this.Controls.Add(this.checkBoxX1);
            this.Controls.Add(this.groupPanel1);
            this.Controls.Add(this.labelX1);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lblSemester);
            this.Controls.Add(this.dgvTimes);
            this.DoubleBuffered = true;
            this.Name = "InputDateSettingForm";
            this.Text = "header";
            ((System.ComponentModel.ISupportInitialize)(this.dgvTimes)).EndInit();
            this.groupPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevComponents.DotNetBar.Controls.DataGridViewX dgvTimes;
        private DevComponents.DotNetBar.LabelX lblSemester;
        private DevComponents.DotNetBar.ButtonX btnClose;
        private DevComponents.DotNetBar.ButtonX btnSave;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private DevComponents.DotNetBar.LabelX labelX1;
        private System.Windows.Forms.DataGridViewTextBoxColumn chStartTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn chEndTime;
        private DevComponents.DotNetBar.Controls.CheckBoxX cbMeritA;
        private DevComponents.DotNetBar.Controls.CheckBoxX cbMeritB;
        private DevComponents.DotNetBar.Controls.CheckBoxX cbMeritC;
        private DevComponents.DotNetBar.Controls.CheckBoxX cbDemeritA;
        private DevComponents.DotNetBar.Controls.CheckBoxX cbDemeritB;
        private DevComponents.DotNetBar.Controls.CheckBoxX cbDemeritC;
        private DevComponents.DotNetBar.Controls.GroupPanel groupPanel1;
        private DevComponents.DotNetBar.Controls.CheckBoxX checkBoxX1;
    }
}