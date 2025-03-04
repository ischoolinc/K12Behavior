namespace K12.Behavior.StuAdminExtendControls
{
    partial class PeriodConfigForm
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
            this.dataGridView = new DevComponents.DotNetBar.Controls.DataGridViewX();
            this.btnSave = new DevComponents.DotNetBar.ButtonX();
            this.btnExit = new DevComponents.DotNetBar.ButtonX();
            this.btnPrintIn = new DevComponents.DotNetBar.ButtonX();
            this.btnPrintOut = new DevComponents.DotNetBar.ButtonX();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.laHelp = new DevComponents.DotNetBar.LabelX();
            this.Col1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCoursePeriod = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Col2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Col3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Col4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Col1,
            this.colCoursePeriod,
            this.Col2,
            this.Col3,
            this.Col4});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.dataGridView.Location = new System.Drawing.Point(9, 12);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowTemplate.Height = 24;
            this.dataGridView.Size = new System.Drawing.Size(451, 412);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellEndEdit);
            this.dataGridView.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellEnter);
            // 
            // btnSave
            // 
            this.btnSave.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.BackColor = System.Drawing.Color.Transparent;
            this.btnSave.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnSave.Location = new System.Drawing.Point(304, 481);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "儲存";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnExit
            // 
            this.btnExit.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.BackColor = System.Drawing.Color.Transparent;
            this.btnExit.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnExit.Location = new System.Drawing.Point(385, 481);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 2;
            this.btnExit.Text = "離開";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnPrintIn
            // 
            this.btnPrintIn.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnPrintIn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPrintIn.BackColor = System.Drawing.Color.Transparent;
            this.btnPrintIn.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnPrintIn.Location = new System.Drawing.Point(75, 481);
            this.btnPrintIn.Name = "btnPrintIn";
            this.btnPrintIn.Size = new System.Drawing.Size(57, 23);
            this.btnPrintIn.TabIndex = 6;
            this.btnPrintIn.Text = "匯入";
            this.btnPrintIn.Click += new System.EventHandler(this.btnPrintIn_Click);
            // 
            // btnPrintOut
            // 
            this.btnPrintOut.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnPrintOut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPrintOut.BackColor = System.Drawing.Color.Transparent;
            this.btnPrintOut.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnPrintOut.Location = new System.Drawing.Point(12, 481);
            this.btnPrintOut.Name = "btnPrintOut";
            this.btnPrintOut.Size = new System.Drawing.Size(57, 23);
            this.btnPrintOut.TabIndex = 5;
            this.btnPrintOut.Text = "匯出";
            this.btnPrintOut.Click += new System.EventHandler(this.btnPrintOut_Click);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.FileName = "匯出每日節次管理";
            this.saveFileDialog1.Filter = "Excel (*.xlsx)|*.xlsx";
            // 
            // laHelp
            // 
            this.laHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.laHelp.AutoSize = true;
            this.laHelp.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.laHelp.BackgroundStyle.Class = "";
            this.laHelp.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.laHelp.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.laHelp.Location = new System.Drawing.Point(12, 433);
            this.laHelp.Name = "laHelp";
            this.laHelp.Size = new System.Drawing.Size(442, 39);
            this.laHelp.TabIndex = 7;
            this.laHelp.Text = "*.對應課程節次: 必須是數字(1,2,3,4,5) 週課表/日課表 需要數字型態的資料\r\n*.缺曠節次: 可以是文字(一,午休,第一節),用於顯示在課堂點名/缺" +
    "曠紀錄";
            // 
            // Col1
            // 
            this.Col1.HeaderText = "缺曠節次";
            this.Col1.Name = "Col1";
            // 
            // colCoursePeriod
            // 
            this.colCoursePeriod.HeaderText = "對應課程節次";
            this.colCoursePeriod.Name = "colCoursePeriod";
            this.colCoursePeriod.Width = 120;
            // 
            // Col2
            // 
            this.Col2.HeaderText = "類型";
            this.Col2.Name = "Col2";
            this.Col2.Width = 80;
            // 
            // Col3
            // 
            this.Col3.HeaderText = "顯示順序";
            this.Col3.Name = "Col3";
            this.Col3.Width = 90;
            // 
            // Col4
            // 
            this.Col4.HeaderText = "統計權重";
            this.Col4.Name = "Col4";
            this.Col4.Visible = false;
            this.Col4.Width = 90;
            // 
            // PeriodConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(472, 514);
            this.Controls.Add(this.laHelp);
            this.Controls.Add(this.btnPrintIn);
            this.Controls.Add(this.btnPrintOut);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.dataGridView);
            this.DoubleBuffered = true;
            this.MaximizeBox = true;
            this.Name = "PeriodConfigForm";
            this.Text = "每日節次管理";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PeriodConfigForm_FormClosing);
            this.Load += new System.EventHandler(this.PeriodConfigForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevComponents.DotNetBar.Controls.DataGridViewX dataGridView;
        private DevComponents.DotNetBar.ButtonX btnSave;
        private DevComponents.DotNetBar.ButtonX btnExit;
        private DevComponents.DotNetBar.ButtonX btnPrintIn;
        private DevComponents.DotNetBar.ButtonX btnPrintOut;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private DevComponents.DotNetBar.LabelX laHelp;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col1;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCoursePeriod;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col4;
    }
}