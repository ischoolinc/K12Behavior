using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using K12.Data;
using FISCA.LogAgent;

namespace K12.Behavior.BatchClearDemerit
{
    public partial class BatchClearDemeritFrom : BaseForm
    {
        List<string> _StudentIDList = new List<string>();

        Dictionary<string, StudentCadre_School> _StudentDic = new Dictionary<string, StudentCadre_School>();

        BackgroundWorker BGW;

        DataConfig _Config;

        //篩選出"未"銷過資料
        List<DemeritRecord> IsNotClearDemeritList;

        //篩選出"已"銷過資料
        List<DemeritRecord> DoClearDemeritList;

        public BatchClearDemeritFrom(List<string> StudentIDList)
        {
            InitializeComponent();

            _StudentIDList = StudentIDList; //使用者選擇的學生

            rbAllDemerit.Checked = true;
        }

        private void BatchClearDemeritFrom_Load(object sender, EventArgs e)
        {
            BGW = new BackgroundWorker();
            BGW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGW_RunWorkerCompleted);
            BGW.DoWork += new DoWorkEventHandler(BGW_DoWork);

            dtStartDate.Text = DateTime.Now.AddDays(-7).ToShortDateString();
            dtEndDate.Text = DateTime.Now.ToShortDateString();
            dtClearDate.Text = DateTime.Now.ToShortDateString();

            BindDate();
        }

        //查詢
        private void btnStart_Click(object sender, EventArgs e)
        {
            BindDate();
        }

        //銷過作業(Log未完成)
        private void btnClearDemerit_Click(object sender, EventArgs e)
        {
            if (dataGridViewX1.SelectedRows.Count == 0)
            {
                MsgBox.Show("請選擇要「銷過」的懲戒資料!!");
                return;
            }

            //2012/5/4 - 靜美銷過事由可不輸入
            DialogResult dr = System.Windows.Forms.DialogResult.Yes;
            if (string.IsNullOrEmpty(tbClearReason.Text))
            {
                dr = MsgBox.Show("未輸入銷過事由!!\n是否繼續執行銷過作業", MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2);
            }
            if (dr == System.Windows.Forms.DialogResult.No)
            {
                MsgBox.Show("已中止操作!!");
                return;
            }

            DialogResult result = MsgBox.Show("您確定要將已選資料 " + dataGridViewX1.SelectedRows.Count + "筆\n進行銷過動作?", MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2);

            if (result == System.Windows.Forms.DialogResult.No)
            {
                MsgBox.Show("已中止操作!!");
                return;
            }

            Dictionary<string, List<DemeritRecord>> dic = new Dictionary<string, List<DemeritRecord>>();

            List<DemeritRecord> list = new List<DemeritRecord>();
            foreach (DataGridViewRow row in dataGridViewX1.SelectedRows)
            {
                if (row.Tag != null)
                {
                    list.Add((DemeritRecord)row.Tag);
                }
            }

            foreach (DemeritRecord each in list)
            {
                each.Cleared = "是";
                each.ClearReason = tbClearReason.Text;
                each.ClearDate = dtClearDate.Value;

                if (!dic.ContainsKey(each.RefStudentID))
                {
                    dic.Add(each.RefStudentID, new List<DemeritRecord>());
                }
                dic[each.RefStudentID].Add(each);
            }

            Demerit.Update(list);

            foreach (string each in dic.Keys)
            {
                StringBuilder sb = new StringBuilder();
                if (_StudentDic.ContainsKey(each))
                {
                    sb.AppendLine("已進行批次「銷過」");
                    sb.Append("班級「" + _StudentDic[each].Class_Name + "」");
                    sb.Append("座號「" + _StudentDic[each].Student_SeatNo + "」");
                    sb.Append("姓名「" + _StudentDic[each].Student_Name + "」");
                    sb.AppendLine("學號「" + _StudentDic[each].Student_Number + "」");
                }

                sb.AppendLine("批次銷過日期「" + dtClearDate.Value.ToShortDateString() + "」");
                sb.AppendLine("批次銷過事由「" + tbClearReason.Text + "」");

                sb.AppendLine("");
                sb.AppendLine("「銷過」懲戒資料清單：");
                foreach (DemeritRecord demerit in dic[each])
                {
                    sb.AppendLine("懲戒日期「" + demerit.OccurDate.ToShortDateString() + "」");
                }
                ApplicationLog.Log("批次銷過", "修改", "student", each, sb.ToString());
            }

            BindDate();
        }

        //取消銷過(Log未完成)
        private void btnUNClearDemerit_Click(object sender, EventArgs e)
        {
            if (dataGridViewX2.SelectedRows.Count == 0)
            {
                MsgBox.Show("請選擇要[取消]銷過的懲戒資料!!");
                return;
            }

            DialogResult result = MsgBox.Show("您確定要將已選資料 " + dataGridViewX2.SelectedRows.Count + "筆\n進行取消銷過動作?", MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2);

            if (result == System.Windows.Forms.DialogResult.No)
            {
                MsgBox.Show("已中止操作!!");
                return;
            }

            Dictionary<string, List<DemeritRecord>> dic = new Dictionary<string, List<DemeritRecord>>();

            List<DemeritRecord> list = new List<DemeritRecord>();
            foreach (DataGridViewRow row in dataGridViewX2.SelectedRows)
            {
                if (row.Tag != null)
                {
                    DemeritRecord dr = (DemeritRecord)row.Tag;

                    list.Add(dr);

                    if (!dic.ContainsKey(dr.RefStudentID))
                    {
                        dic.Add(dr.RefStudentID, new List<DemeritRecord>());
                    }
                    dic[dr.RefStudentID].Add(dr);


                }
            }

            foreach (string each in dic.Keys)
            {
                StringBuilder sb = new StringBuilder();
                if (_StudentDic.ContainsKey(each))
                {
                    sb.AppendLine("已進行批次「取消銷過」");
                    sb.Append("班級「" + _StudentDic[each].Class_Name + "」");
                    sb.Append("座號「" + _StudentDic[each].Student_SeatNo + "」");
                    sb.Append("姓名「" + _StudentDic[each].Student_Name + "」");
                    sb.AppendLine("學號「" + _StudentDic[each].Student_Number + "」");
                }
                sb.AppendLine("");
                sb.AppendLine("「取消銷過」懲戒資料清單：");
                foreach (DemeritRecord demerit in dic[each])
                {
                    sb.Append("懲戒日期「" + demerit.OccurDate.ToShortDateString() + "」");
                    if (demerit.ClearDate.HasValue)
                        sb.Append("原銷過日期「" + demerit.ClearDate.Value.ToShortDateString() + "」已清除");
                    else
                        sb.Append("原銷過日期「」已清除");

                    sb.AppendLine("，原銷過事由「" + demerit.ClearReason + "」已清除");
                }
                ApplicationLog.Log("批次取消銷過", "修改", "student", each, sb.ToString());
            }

            foreach (DemeritRecord each in list)
            {
                each.Cleared = "";
                each.ClearReason = "";
                each.ClearDate = null;
            }

            Demerit.Update(list);

            BindDate();
        }

        /// <summary>
        /// 開始重新整理資料
        /// </summary>
        private void BindDate()
        {
            if (!BGW.IsBusy)
            {
                _Config = SetConfig(); //取得畫面設定

                SetupFrom = false; //鎖定畫面

                dataGridViewX1.Rows.Clear();
                dataGridViewX2.Rows.Clear();

                BGW.RunWorkerAsync();
            }
            else
            {
                MsgBox.Show("系統目前忙碌中,稍後再試!");
            }
        }

        /// <summary>
        /// 取得目前畫面上的設定內容
        /// </summary>
        /// <returns></returns>
        private DataConfig SetConfig()
        {
            DataConfig Config = new DataConfig();

            if (rbAllDemerit.Checked)
            {
                Config._Mode = DataGetMode.ALL;
            }
            else if (rbOccurDate.Checked)
            {
                Config._Mode = DataGetMode.OccurDate;
            }
            else if (rbRegisterDate.Checked)
            {
                Config._Mode = DataGetMode.RegisterDate;
            }
            else if (rbClearDate.Checked)
            {
                Config._Mode = DataGetMode.ClearDate;
            }
            Config._StartDate = dtStartDate.Value;
            Config._EndDate = dtEndDate.Value;

            return Config;
        }

        void BGW_DoWork(object sender, DoWorkEventArgs e)
        {
            #region 取得學生資料清單

            _StudentDic.Clear();

            FISCA.Data.QueryHelper _queryHelper = new FISCA.Data.QueryHelper();

            StringBuilder sb = new StringBuilder();
            sb.Append("select class.id as class_id,class.class_name,class.grade_year,student.id as student_id,student.name,student.student_number,student.seat_no,class.display_order ");
            sb.Append("from student left ");
            sb.Append("join class on student.ref_class_id=class.id ");
            sb.Append(string.Format("where student.id in('{0}')", string.Join("','", _StudentIDList)));
            DataTable dt = _queryHelper.Select(sb.ToString());

            foreach (DataRow row in dt.Rows)
            {
                StudentCadre_School ss = new StudentCadre_School(row);
                if (!_StudentDic.ContainsKey(ss.Student_ID))
                {
                    _StudentDic.Add(ss.Student_ID, ss);
                }
            }

            #endregion

            #region 取得缺曠資料清單

            List<DemeritRecord> DemeritList = GetDemerit();
            //"未"銷過資料
            IsNotClearDemeritList = new List<DemeritRecord>();
            //"已"銷過資料
            DoClearDemeritList = new List<DemeritRecord>();

            foreach (DemeritRecord each in DemeritList)
            {
                if (each.MeritFlag == "2")
                    continue;

                if (each.Cleared == "是")
                {
                    DoClearDemeritList.Add(each);
                }
                else
                {
                    IsNotClearDemeritList.Add(each);
                }
            }

            IsNotClearDemeritList.Sort(SortDemerit);

            DoClearDemeritList.Sort(SortDemerit);
            #endregion
        }

        private int SortDemerit(DemeritRecord dr1, DemeritRecord dr2)
        {
            string demeritA = "0000000000000000";
            if (_StudentDic.ContainsKey(dr1.RefStudentID))
            {
                if (!string.IsNullOrEmpty(_StudentDic[dr1.RefStudentID].grade_year))
                {
                    demeritA = _StudentDic[dr1.RefStudentID].grade_year.PadLeft(4, '9');
                }
                else
                {
                    demeritA = "0000";
                }

                demeritA += _StudentDic[dr1.RefStudentID].Class_display_order.PadLeft(4, '0');

                demeritA += _StudentDic[dr1.RefStudentID].Class_Name.PadLeft(4, '0');

                demeritA += _StudentDic[dr1.RefStudentID].Student_SeatNo.PadLeft(4, '0');
            }

            string demeritB = "0000000000000000";
            if (_StudentDic.ContainsKey(dr2.RefStudentID))
            {
                if (!string.IsNullOrEmpty(_StudentDic[dr1.RefStudentID].grade_year))
                {
                    demeritB = _StudentDic[dr2.RefStudentID].grade_year.PadLeft(4, '9');
                }
                else
                {
                    demeritB = "0000";
                }

                demeritB += _StudentDic[dr2.RefStudentID].Class_display_order.PadLeft(4, '0');

                demeritB += _StudentDic[dr2.RefStudentID].Class_Name.PadLeft(4, '0');

                demeritB += _StudentDic[dr2.RefStudentID].Student_SeatNo.PadLeft(4, '0');
            }

            return demeritA.CompareTo(demeritB);

        }

        /// <summary>
        /// 依據設定,取得懲戒資料
        /// </summary>
        /// <returns></returns>
        private List<DemeritRecord> GetDemerit()
        {
            List<DemeritRecord> DemeritList = new List<DemeritRecord>();

            if (_Config._Mode == DataGetMode.ALL)
            {
                DemeritList = Demerit.SelectByStudentIDs(_StudentIDList);
            }
            else if (_Config._Mode == DataGetMode.OccurDate)
            {
                DemeritList = Demerit.SelectByOccurDate(_StudentIDList, _Config._StartDate, _Config._EndDate);
            }
            else if (_Config._Mode == DataGetMode.RegisterDate)
            {
                DemeritList = Demerit.SelectByRegisterDate(_StudentIDList, _Config._StartDate, _Config._EndDate);
            }
            else if (_Config._Mode == DataGetMode.ClearDate)
            {
                foreach (DemeritRecord each in Demerit.SelectByStudentIDs(_StudentIDList))
                {
                    if (each.Cleared == "是")
                    {
                        if (each.ClearDate.Value >= _Config._StartDate && each.ClearDate.Value <= _Config._EndDate)
                        {
                            DemeritList.Add(each);
                        }
                    }
                }
            }

            return DemeritList;
        }

        void BGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //未銷過記錄
            tabItem1.Text = "未銷過記錄(" + IsNotClearDemeritList.Count + ")";
            foreach (DemeritRecord each in IsNotClearDemeritList)
            {
                SetDatGridViewRow(dataGridViewX1, each, true);
            }

            //已銷過記錄
            tabItem3.Text = "已銷過記錄(" + DoClearDemeritList.Count + ")";
            foreach (DemeritRecord each in DoClearDemeritList)
            {
                SetDatGridViewRow(dataGridViewX2, each, false);
            }

            SetupFrom = true;
        }

        private void SetDatGridViewRow(DataGridView dgv, DemeritRecord each, bool IsClear)
        {
            DataGridViewRow row = new DataGridViewRow();
            row.Tag = each;
            row.CreateCells(dgv);
            row.Cells[0].Value = _StudentDic[each.RefStudentID].Class_Name;
            row.Cells[1].Value = _StudentDic[each.RefStudentID].Student_SeatNo;
            row.Cells[2].Value = _StudentDic[each.RefStudentID].Student_Name;
            row.Cells[3].Value = _StudentDic[each.RefStudentID].Student_Number;
            row.Cells[4].Value = each.OccurDate.ToShortDateString();
            row.Cells[5].Value = each.DemeritA.HasValue ? each.DemeritA.Value.ToString() : "";
            row.Cells[6].Value = each.DemeritB.HasValue ? each.DemeritB.Value.ToString() : "";
            row.Cells[7].Value = each.DemeritC.HasValue ? each.DemeritC.Value.ToString() : "";
            row.Cells[8].Value = each.Reason;
            if (!IsClear)
            {
                row.Cells[9].Value = "是";
            }
            row.Cells[10].Value = each.ClearDate.HasValue ? each.ClearDate.Value.ToShortDateString() : "";
            row.Cells[11].Value = each.ClearReason;
            row.Cells[12].Value = each.RegisterDate.HasValue ? each.RegisterDate.Value.ToShortDateString() : "";
            row.Cells[13].Value = each.Remark;
            dgv.Rows.Add(row);
        }

        bool SetupFrom
        {
            set
            {
                if (value)
                {
                    this.Text = "批次銷過";
                }
                else
                {
                    this.Text = "資料讀取中...";
                }
                groupPanel1.Enabled = value;
                groupPanel2.Enabled = value;
                btnStart.Enabled = value;
                dtClearDate.Enabled = value;
                tbClearReason.Enabled = value;
                btnClearDemerit.Enabled = value;
                btnUNClearDemerit.Enabled = value;
            }
        }

        private void rbAllDemerit_CheckedChanged(object sender, EventArgs e)
        {
            dtStartDate.Enabled = !rbAllDemerit.Checked;
            dtEndDate.Enabled = !rbAllDemerit.Checked;
            SetBtnStartPulse();
        }

        private void btnExit1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnExit2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridViewX1_SelectionChanged(object sender, EventArgs e)
        {
            lbSelectInfo1.Text = string.Format("請選擇資料以進行[銷過]作業　已選擇{0}筆", dataGridViewX1.SelectedRows.Count);
        }

        private void dataGridViewX2_SelectionChanged(object sender, EventArgs e)
        {
            lbSelectInfo2.Text = string.Format("請選擇資料以進行[取消銷過]作業　已選擇{0}筆", dataGridViewX2.SelectedRows.Count);
        }

        private void rbOccurDate_CheckedChanged(object sender, EventArgs e)
        {
            SetBtnStartPulse();
        }

        private void rbRegisterDate_CheckedChanged(object sender, EventArgs e)
        {
            SetBtnStartPulse();
        }

        private void rbClearDate_CheckedChanged(object sender, EventArgs e)
        {
            SetBtnStartPulse();
        }

        private void SetBtnStartPulse()
        {
            btnStart.Pulse(10);
        }

        private void dtStartDate_TextChanged(object sender, EventArgs e)
        {
            SetBtnStartPulse();
        }

        private void dtEndDate_TextChanged(object sender, EventArgs e)
        {
            SetBtnStartPulse();
        }

        bool RowSortMode = false;

        private void dataGridViewX1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex == Column12.Index || e.ColumnIndex == Column9.Index || e.ColumnIndex == Column13.Index)
            {
                List<DataGridViewRow> rowList = new List<DataGridViewRow>();
                foreach (DataGridViewRow row in dataGridViewX1.Rows)
                {
                    rowList.Add(row);
                }

                if (e.ColumnIndex == Column12.Index)
                {       
                    if (RowSortMode)
                    {
                        RowSortMode = false;
                        rowList.Sort(SortDataGridViewRow1);
                    }
                    else
                    {
                        RowSortMode = true;
                        rowList.Sort(SortDataGridViewRow1);
                    }
                }
                else if (e.ColumnIndex == Column13.Index)
                {
                    if (RowSortMode)
                    {
                        RowSortMode = false;
                        rowList.Sort(SortDataGridViewRow2);
                    }
                    else
                    {
                        RowSortMode = true;
                        rowList.Sort(SortDataGridViewRow2);
                    }
                }
                else if (e.ColumnIndex == Column9.Index)
                {
                    if (RowSortMode)
                    {
                        RowSortMode = false;
                        rowList.Sort(SortDataGridViewRow3);
                    }
                    else
                    {
                        RowSortMode = true;
                        rowList.Sort(SortDataGridViewRow3);
                    }
                }

                dataGridViewX1.Rows.Clear();
                foreach (DataGridViewRow row in rowList)
                {
                    dataGridViewX1.Rows.Add(row);
                }
            }
        }

        private void dataGridViewX2_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex == Column12.Index || e.ColumnIndex == Column9.Index || e.ColumnIndex == Column13.Index)
            {
                List<DataGridViewRow> rowList = new List<DataGridViewRow>();
                foreach (DataGridViewRow row in dataGridViewX2.Rows)
                {
                    rowList.Add(row);
                }

                if (e.ColumnIndex == Column12.Index)
                {
                    if (RowSortMode)
                    {
                        RowSortMode = false;
                        rowList.Sort(SortDataGridViewRow1);
                    }
                    else
                    {
                        RowSortMode = true;
                        rowList.Sort(SortDataGridViewRow1);
                    }
                }
                else if (e.ColumnIndex == Column13.Index)
                {
                    if (RowSortMode)
                    {
                        RowSortMode = false;
                        rowList.Sort(SortDataGridViewRow2);
                    }
                    else
                    {
                        RowSortMode = true;
                        rowList.Sort(SortDataGridViewRow2);
                    }
                }
                else if (e.ColumnIndex == Column9.Index)
                {
                    if (RowSortMode)
                    {
                        RowSortMode = false;
                        rowList.Sort(SortDataGridViewRow3);
                    }
                    else
                    {
                        RowSortMode = true;
                        rowList.Sort(SortDataGridViewRow3);
                    }
                }

                dataGridViewX2.Rows.Clear();
                foreach (DataGridViewRow row in rowList)
                {
                    dataGridViewX2.Rows.Add(row);
                }
            }
        }
        //依發生日期排序
        private int SortDataGridViewRow1(DataGridViewRow row1, DataGridViewRow row2)
        {
            DemeritRecord demerit1 = (DemeritRecord)row1.Tag;
            DemeritRecord demerit2 = (DemeritRecord)row2.Tag;
            if (RowSortMode)
            {
                return demerit1.OccurDate.CompareTo(demerit2.OccurDate);
            }
            else
            {
                return demerit2.OccurDate.CompareTo(demerit1.OccurDate);
            }
        }
        //依登錄日期排序
        private int SortDataGridViewRow2(DataGridViewRow row1, DataGridViewRow row2)
        {
            DemeritRecord demerit1 = (DemeritRecord)row1.Tag;
            DemeritRecord demerit2 = (DemeritRecord)row2.Tag;
            DateTime dt1 = demerit1.RegisterDate.HasValue ? demerit1.RegisterDate.Value : DateTime.Now;
            DateTime dt2 = demerit2.RegisterDate.HasValue ? demerit2.RegisterDate.Value : DateTime.Now;
            if (RowSortMode)
            {
                return dt1.CompareTo(dt2);
            }
            else
            {
                return dt2.CompareTo(dt1);
            }
        }
        //依銷過日期排序
        private int SortDataGridViewRow3(DataGridViewRow row1, DataGridViewRow row2)
        {
            DemeritRecord demerit1 = (DemeritRecord)row1.Tag;
            DemeritRecord demerit2 = (DemeritRecord)row2.Tag;
            DateTime dt1 = demerit1.ClearDate.HasValue ? demerit1.ClearDate.Value : DateTime.Now;
            DateTime dt2 = demerit2.ClearDate.HasValue ? demerit2.ClearDate.Value : DateTime.Now;
            if (RowSortMode)
            {
                return dt1.CompareTo(dt2);
            }
            else
            {
                return dt2.CompareTo(dt1);
            }
        }

        private void btnReason_Click(object sender, EventArgs e)
        {
            frmSelectReason form = new frmSelectReason();

            DialogResult result = form.ShowDialog();

            if (result == DialogResult.Yes)
            {
                tbClearReason.Text = form._reason;
            }
        }
    }

    enum DataGetMode
    {
        ALL, OccurDate, RegisterDate, ClearDate
    };

    class StudentCadre_School
    {
        /// <summary>
        /// 年級
        /// </summary>
        public string grade_year { get; set; }

        /// <summary>
        /// 班級系統編號
        /// </summary>
        public string Class_ID { get; set; }

        /// <summary>
        /// 班級名稱
        /// </summary>
        public string Class_Name { get; set; }

        /// <summary>
        /// 班級排序
        /// </summary>
        public string Class_display_order { get; set; }

        /// <summary>
        /// 學生系統編號
        /// </summary>
        public string Student_ID { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Student_Name { get; set; }

        /// <summary>
        /// 座號
        /// </summary>
        public string Student_SeatNo { get; set; }

        /// <summary>
        /// 學號
        /// </summary>
        public string Student_Number { get; set; }

        public StudentCadre_School(DataRow row)
        {
            Class_ID = "" + row["class_id"];
            Class_Name = "" + row["class_name"];
            Student_ID = "" + row["student_id"];
            Student_Name = "" + row["name"];
            Student_SeatNo = "" + row["seat_no"];
            Student_Number = "" + row["student_number"];
            Class_display_order = "" + row["display_order"];
            grade_year = "" + row["grade_year"];
        }
    }
}
