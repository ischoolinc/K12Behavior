namespace K12.Behavior.Keyboard
{
    partial class ClassInsertForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.dgvBaseView = new Behavior.Keyboard.CustomDataGridView();
            this.dgvChangeView = new Behavior.Keyboard.CustomDataGridView();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.BaseColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BaseColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BaseColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BaseColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBaseView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvChangeView)).BeginInit();
            this.SuspendLayout();
            // 
            // labelX1
            // 
            this.labelX1.AutoSize = true;
            this.labelX1.BackColor = System.Drawing.Color.Transparent;
            this.labelX1.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labelX1.Location = new System.Drawing.Point(8, 12);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(422, 21);
            this.labelX1.TabIndex = 0;
            this.labelX1.Text = "將進行班級缺曠批次輸入，部份學生缺曠內容已存在，請確認更新內容";
            // 
            // dgvBaseView
            // 
            this.dgvBaseView.AllowUserToAddRows = false;
            this.dgvBaseView.AllowUserToDeleteRows = false;
            this.dgvBaseView.AllowUserToResizeColumns = false;
            this.dgvBaseView.AllowUserToResizeRows = false;
            this.dgvBaseView.BackgroundColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.dgvBaseView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvBaseView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBaseView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.BaseColumn1,
            this.BaseColumn2,
            this.BaseColumn3,
            this.BaseColumn4});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("微軟正黑體", 9.75F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvBaseView.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvBaseView.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.dgvBaseView.Location = new System.Drawing.Point(9, 41);
            this.dgvBaseView.Name = "dgvBaseView";
            this.dgvBaseView.RowHeadersVisible = false;
            this.dgvBaseView.RowHeadersWidth = 35;
            this.dgvBaseView.RowTemplate.Height = 24;
            this.dgvBaseView.Size = new System.Drawing.Size(771, 52);
            this.dgvBaseView.TabIndex = 13;
            // 
            // dgvChangeView
            // 
            this.dgvChangeView.AllowUserToAddRows = false;
            this.dgvChangeView.AllowUserToDeleteRows = false;
            this.dgvChangeView.AllowUserToResizeColumns = false;
            this.dgvChangeView.AllowUserToResizeRows = false;
            this.dgvChangeView.BackgroundColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.dgvChangeView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvChangeView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvChangeView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn4,
            this.dataGridViewTextBoxColumn5,
            this.dataGridViewTextBoxColumn6,
            this.dataGridViewTextBoxColumn7,
            this.Column1,
            this.Column2});
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("微軟正黑體", 9.75F);
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvChangeView.DefaultCellStyle = dataGridViewCellStyle7;
            this.dgvChangeView.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.dgvChangeView.Location = new System.Drawing.Point(9, 126);
            this.dgvChangeView.Name = "dgvChangeView";
            this.dgvChangeView.RowHeadersVisible = false;
            this.dgvChangeView.RowHeadersWidth = 35;
            this.dgvChangeView.RowTemplate.Height = 24;
            this.dgvChangeView.Size = new System.Drawing.Size(771, 184);
            this.dgvChangeView.TabIndex = 14;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "座號";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.Width = 60;
            // 
            // dataGridViewTextBoxColumn4
            // 
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.AliceBlue;
            this.dataGridViewTextBoxColumn4.DefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridViewTextBoxColumn4.HeaderText = "姓名";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.Width = 80;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.HeaderText = "請假日期";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            // 
            // dataGridViewTextBoxColumn6
            // 
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.AliceBlue;
            this.dataGridViewTextBoxColumn6.DefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridViewTextBoxColumn6.HeaderText = "學年度";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.ReadOnly = true;
            this.dataGridViewTextBoxColumn6.Width = 70;
            // 
            // dataGridViewTextBoxColumn7
            // 
            this.dataGridViewTextBoxColumn7.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.AliceBlue;
            this.dataGridViewTextBoxColumn7.DefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridViewTextBoxColumn7.HeaderText = "學期";
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            this.dataGridViewTextBoxColumn7.ReadOnly = true;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "更新為新假別";
            this.Column1.Name = "Column1";
            // 
            // Column2
            // 
            this.Column2.HeaderText = "保留原假別";
            this.Column2.Name = "Column2";
            // 
            // labelX2
            // 
            this.labelX2.AutoSize = true;
            this.labelX2.BackColor = System.Drawing.Color.Transparent;
            this.labelX2.Location = new System.Drawing.Point(8, 97);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(200, 21);
            this.labelX2.TabIndex = 15;
            this.labelX2.Text = "下列學生,假別已重覆,請確認操作";
            // 
            // BaseColumn1
            // 
            this.BaseColumn1.HeaderText = "班級";
            this.BaseColumn1.Name = "BaseColumn1";
            this.BaseColumn1.Width = 60;
            // 
            // BaseColumn2
            // 
            this.BaseColumn2.HeaderText = "請假日期";
            this.BaseColumn2.Name = "BaseColumn2";
            // 
            // BaseColumn3
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.AliceBlue;
            this.BaseColumn3.DefaultCellStyle = dataGridViewCellStyle1;
            this.BaseColumn3.HeaderText = "學年度";
            this.BaseColumn3.Name = "BaseColumn3";
            this.BaseColumn3.ReadOnly = true;
            this.BaseColumn3.Width = 70;
            // 
            // BaseColumn4
            // 
            this.BaseColumn4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.AliceBlue;
            this.BaseColumn4.DefaultCellStyle = dataGridViewCellStyle2;
            this.BaseColumn4.HeaderText = "學期";
            this.BaseColumn4.Name = "BaseColumn4";
            this.BaseColumn4.ReadOnly = true;
            // 
            // ClassInsertForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 423);
            this.Controls.Add(this.labelX2);
            this.Controls.Add(this.dgvChangeView);
            this.Controls.Add(this.dgvBaseView);
            this.Controls.Add(this.labelX1);
            this.Name = "ClassInsertForm";
            this.Text = "班級批次輸入";
            this.Load += new System.EventHandler(this.ClassInsertForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBaseView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvChangeView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevComponents.DotNetBar.LabelX labelX1;
        private CustomDataGridView dgvBaseView;
        private CustomDataGridView dgvChangeView;
        private DevComponents.DotNetBar.LabelX labelX2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn BaseColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn BaseColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn BaseColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn BaseColumn4;
    }
}