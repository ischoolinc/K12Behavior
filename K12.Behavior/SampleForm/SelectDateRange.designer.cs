namespace K12.Behavior
{
    partial class SelectDateRange
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
            this.lbStartDatetime = new DevComponents.DotNetBar.LabelX();
            this.lbEndDateTime = new DevComponents.DotNetBar.LabelX();
            this.dtStartDateTime = new DevComponents.Editors.DateTimeAdv.DateTimeInput();
            this.dtEndDateTime = new DevComponents.Editors.DateTimeAdv.DateTimeInput();
            this.groupPanel1 = new DevComponents.DotNetBar.Controls.GroupPanel();
            ((System.ComponentModel.ISupportInitialize)(this.dtStartDateTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtEndDateTime)).BeginInit();
            this.groupPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbStartDatetime
            // 
            this.lbStartDatetime.AutoSize = true;
            this.lbStartDatetime.BackColor = System.Drawing.Color.Transparent;
            this.lbStartDatetime.Location = new System.Drawing.Point(26, 36);
            this.lbStartDatetime.Name = "lbStartDatetime";
            this.lbStartDatetime.Size = new System.Drawing.Size(60, 21);
            this.lbStartDatetime.TabIndex = 1;
            this.lbStartDatetime.Text = "開始日期";
            // 
            // lbEndDateTime
            // 
            this.lbEndDateTime.AutoSize = true;
            this.lbEndDateTime.BackColor = System.Drawing.Color.Transparent;
            this.lbEndDateTime.Location = new System.Drawing.Point(241, 36);
            this.lbEndDateTime.Name = "lbEndDateTime";
            this.lbEndDateTime.Size = new System.Drawing.Size(60, 21);
            this.lbEndDateTime.TabIndex = 3;
            this.lbEndDateTime.Text = "結束日期";
            // 
            // dtStartDateTime
            // 
            this.dtStartDateTime.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.dtStartDateTime.BackgroundStyle.Class = "DateTimeInputBackground";
            this.dtStartDateTime.ButtonDropDown.Shortcut = DevComponents.DotNetBar.eShortcut.AltDown;
            this.dtStartDateTime.ButtonDropDown.Visible = true;
            this.dtStartDateTime.Location = new System.Drawing.Point(96, 35);
            // 
            // 
            // 
            this.dtStartDateTime.MonthCalendar.AnnuallyMarkedDates = new System.DateTime[0];
            // 
            // 
            // 
            this.dtStartDateTime.MonthCalendar.BackgroundStyle.BackColor = System.Drawing.SystemColors.Window;
            this.dtStartDateTime.MonthCalendar.ClearButtonVisible = true;
            // 
            // 
            // 
            this.dtStartDateTime.MonthCalendar.CommandsBackgroundStyle.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground2;
            this.dtStartDateTime.MonthCalendar.CommandsBackgroundStyle.BackColorGradientAngle = 90;
            this.dtStartDateTime.MonthCalendar.CommandsBackgroundStyle.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground;
            this.dtStartDateTime.MonthCalendar.CommandsBackgroundStyle.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.dtStartDateTime.MonthCalendar.CommandsBackgroundStyle.BorderTopColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarDockedBorder;
            this.dtStartDateTime.MonthCalendar.CommandsBackgroundStyle.BorderTopWidth = 1;
            this.dtStartDateTime.MonthCalendar.DayNames = new string[] {
        "日",
        "一",
        "二",
        "三",
        "四",
        "五",
        "六"};
            this.dtStartDateTime.MonthCalendar.DisplayMonth = new System.DateTime(2011, 4, 1, 0, 0, 0, 0);
            this.dtStartDateTime.MonthCalendar.MarkedDates = new System.DateTime[0];
            this.dtStartDateTime.MonthCalendar.MonthlyMarkedDates = new System.DateTime[0];
            // 
            // 
            // 
            this.dtStartDateTime.MonthCalendar.NavigationBackgroundStyle.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.dtStartDateTime.MonthCalendar.NavigationBackgroundStyle.BackColorGradientAngle = 90;
            this.dtStartDateTime.MonthCalendar.NavigationBackgroundStyle.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.dtStartDateTime.MonthCalendar.TodayButtonVisible = true;
            this.dtStartDateTime.MonthCalendar.WeeklyMarkedDays = new System.DayOfWeek[0];
            this.dtStartDateTime.Name = "dtStartDateTime";
            this.dtStartDateTime.Size = new System.Drawing.Size(125, 25);
            this.dtStartDateTime.TabIndex = 2;
            // 
            // dtEndDateTime
            // 
            this.dtEndDateTime.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.dtEndDateTime.BackgroundStyle.Class = "DateTimeInputBackground";
            this.dtEndDateTime.ButtonDropDown.Shortcut = DevComponents.DotNetBar.eShortcut.AltDown;
            this.dtEndDateTime.ButtonDropDown.Visible = true;
            this.dtEndDateTime.Location = new System.Drawing.Point(311, 35);
            // 
            // 
            // 
            this.dtEndDateTime.MonthCalendar.AnnuallyMarkedDates = new System.DateTime[0];
            // 
            // 
            // 
            this.dtEndDateTime.MonthCalendar.BackgroundStyle.BackColor = System.Drawing.SystemColors.Window;
            this.dtEndDateTime.MonthCalendar.ClearButtonVisible = true;
            // 
            // 
            // 
            this.dtEndDateTime.MonthCalendar.CommandsBackgroundStyle.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground2;
            this.dtEndDateTime.MonthCalendar.CommandsBackgroundStyle.BackColorGradientAngle = 90;
            this.dtEndDateTime.MonthCalendar.CommandsBackgroundStyle.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground;
            this.dtEndDateTime.MonthCalendar.CommandsBackgroundStyle.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.dtEndDateTime.MonthCalendar.CommandsBackgroundStyle.BorderTopColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarDockedBorder;
            this.dtEndDateTime.MonthCalendar.CommandsBackgroundStyle.BorderTopWidth = 1;
            this.dtEndDateTime.MonthCalendar.DayNames = new string[] {
        "日",
        "一",
        "二",
        "三",
        "四",
        "五",
        "六"};
            this.dtEndDateTime.MonthCalendar.DisplayMonth = new System.DateTime(2011, 4, 1, 0, 0, 0, 0);
            this.dtEndDateTime.MonthCalendar.MarkedDates = new System.DateTime[0];
            this.dtEndDateTime.MonthCalendar.MonthlyMarkedDates = new System.DateTime[0];
            // 
            // 
            // 
            this.dtEndDateTime.MonthCalendar.NavigationBackgroundStyle.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.dtEndDateTime.MonthCalendar.NavigationBackgroundStyle.BackColorGradientAngle = 90;
            this.dtEndDateTime.MonthCalendar.NavigationBackgroundStyle.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.dtEndDateTime.MonthCalendar.TodayButtonVisible = true;
            this.dtEndDateTime.MonthCalendar.WeeklyMarkedDays = new System.DayOfWeek[0];
            this.dtEndDateTime.Name = "dtEndDateTime";
            this.dtEndDateTime.Size = new System.Drawing.Size(125, 25);
            this.dtEndDateTime.TabIndex = 4;
            // 
            // groupPanel1
            // 
            this.groupPanel1.BackColor = System.Drawing.Color.Transparent;
            this.groupPanel1.CanvasColor = System.Drawing.SystemColors.Control;
            this.groupPanel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.groupPanel1.Controls.Add(this.dtEndDateTime);
            this.groupPanel1.Controls.Add(this.dtStartDateTime);
            this.groupPanel1.Controls.Add(this.lbStartDatetime);
            this.groupPanel1.Controls.Add(this.lbEndDateTime);
            this.groupPanel1.Location = new System.Drawing.Point(12, 12);
            this.groupPanel1.Name = "groupPanel1";
            this.groupPanel1.Size = new System.Drawing.Size(468, 121);
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
            this.groupPanel1.Style.CornerDiameter = 4;
            this.groupPanel1.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.groupPanel1.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.groupPanel1.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            this.groupPanel1.TabIndex = 2;
            this.groupPanel1.Text = "選擇日期區間";
            // 
            // SelectDateRange
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(492, 366);
            this.Controls.Add(this.groupPanel1);
            this.Name = "SelectDateRange";
            this.Text = "";
            this.Controls.SetChildIndex(this.groupPanel1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.dtStartDateTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtEndDateTime)).EndInit();
            this.groupPanel1.ResumeLayout(false);
            this.groupPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevComponents.DotNetBar.LabelX lbStartDatetime;
        private DevComponents.DotNetBar.LabelX lbEndDateTime;
        private DevComponents.Editors.DateTimeAdv.DateTimeInput dtStartDateTime;
        private DevComponents.Editors.DateTimeAdv.DateTimeInput dtEndDateTime;
        private DevComponents.DotNetBar.Controls.GroupPanel groupPanel1;
    }
}