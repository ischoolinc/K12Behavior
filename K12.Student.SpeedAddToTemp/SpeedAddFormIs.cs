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
using FISCA.Presentation;

namespace K12.Student.SpeedAddToTemp
{
    public partial class SpeedAddFormIs : BaseForm
    {
        StudentMag sMag;

        public string Code = "K12.Behavior.Keyboard.SetClassCode";

        public string Code2 = "K12.Behavior.Keyboard.Config.SpeedAddToTemp";

        string Code3 = "使用班級名稱代碼";

        string Code4 = "使用學號自動判斷";

        string Code5 = "學號碼數";

        string Code6 = "排序順序";

        K12.Data.Configuration.ConfigData cd;

        int AllIndex = 0;

        Dictionary<string, string> ClassNameDic = new Dictionary<string, string>();

        BackgroundWorker bgwConfig;

        BackgroundWorker bgwLoad;

        public SpeedAddFormIs()
        {
            InitializeComponent();
        }

        private void SpeedAddFormIs_Load(object sender, EventArgs e)
        {
            K12.Presentation.NLDPanels.Student.TempSourceChanged += new EventHandler(Student_TempSourceChanged);

            bgwConfig = new BackgroundWorker();
            bgwConfig.RunWorkerCompleted += BgwConfig_RunWorkerCompleted;
            bgwConfig.DoWork += BgwConfig_DoWork;

            bgwLoad = new BackgroundWorker();
            bgwLoad.RunWorkerCompleted += BgwLoad_RunWorkerCompleted;
            bgwLoad.DoWork += BgwLoad_DoWork;

            bgwLoad.RunWorkerAsync();
        }

        private void BgwLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            sMag = new StudentMag();
            ClassNameDic = DataSort.GetClassNameDic(Code);

            cd = School.Configuration[Code2];
        }

        private void BgwLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //使用班級名稱代碼
            if (cd.Contains(Code3))
            {
                checkBoxX1.Checked = bool.Parse(cd[Code3]);
            }

            //使用學號自動判斷
            if (cd.Contains(Code4))
            {
                checkBoxX2.Checked = bool.Parse(cd[Code4]);
            }

            if (cd.Contains(Code5))
            {
                integerInput1.Value = int.Parse(cd[Code5]);
            }

            if (cd.Contains(Code6))
            {
                if (bool.Parse(cd[Code6]))
                    checkBoxX4.Checked = true;
                else
                    checkBoxX3.Checked = true;
            }
            else
            {
                checkBoxX4.Checked = true;
            }

            integerInput1.Enabled = checkBoxX2.Checked;

            TempSourceIpr();

            tbClassName.Focus();
        }

        private void BgwConfig_DoWork(object sender, DoWorkEventArgs e)
        {

            SuperD s = (SuperD)e.Argument;

            cd = School.Configuration[Code2];
            cd[s.Acode] = s.Bvalue;
            cd.Save();
        }

        private void BgwConfig_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //儲存不會錯!!
        }

        void Student_TempSourceChanged(object sender, EventArgs e)
        {
            lbCount.Text = "待處理共「" + K12.Presentation.NLDPanels.Student.TempSource.Count().ToString() + "」名學生";

            TempSourceIpr();
        }

        bool FormLocked
        {
            set
            {
                checkBoxX1.Enabled = value;
                tbClassName.Enabled = value;
                tbSean.Enabled = value;
                btnClearMessage.Enabled = value;
                btnClearTemp.Enabled = value;
                linkLabel1.Enabled = value;

                tbStudentNumber.Enabled = value;
                checkBoxX2.Enabled = value;

                integerInput2.Enabled = value;
            }
        }

        private void TempSourceIpr()
        {
            FormLocked = false;

            //2019/11/20 - 已不需要Dylan ==>
            //2016/12/27  穎驊新增，紀錄目前已經有加入dataGridViewX1 的 Tag
            //List<string> RowTag_Record = new List<string>();
            //foreach (DataGridViewRow row in dataGridViewX1.Rows)
            //{
            //    RowTag_Record.Add("" + row.Tag);
            //}
            //<===

            lbCount.Text = "待處理共「" + K12.Presentation.NLDPanels.Student.TempSource.Count().ToString() + "」名學生";
            AllIndex = K12.Presentation.NLDPanels.Student.TempSource.Count();

            RunAddData();

            FormLocked = true;
        }

        private void RunAddData()
        {
            dataGridViewX1.Rows.Clear();

            //取得待處理學生
            List<StudentRecord> StudentList = K12.Data.Student.SelectByIDs(K12.Presentation.NLDPanels.Student.TempSource);

            //2019/11/19 - Dylan增加可選擇排序依據
            if (checkBoxX4.Checked)
            {
                StudentList = SortClassIndex.K12Data_StudentRecord(StudentList);
            }

            //2019/11/19 - Dylan:穎驊邏輯錯誤,註解此內容==>

            //2016/12/27  穎驊新增，由於使用者希望，
            //能夠後加入的資料 優先顯示在dataGridViewX1，
            //因此使用前面整理的RowTag_Record List 來做比較，
            //可以優先排序出有無加入的資料。
            //StudentList.Sort((x, y) => {
            //    return RowTag_Record.Contains(x.ID).CompareTo(RowTag_Record.Contains(y.ID));
            //});

            //<=====

            foreach (StudentRecord sr in StudentList)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridViewX1);
                row.Tag = sr.ID;
                if (sr.Class != null)
                {
                    row.Cells[0].Value = sr.Class.Name;
                }
                row.Cells[1].Value = sr.SeatNo;
                row.Cells[2].Value = sr.Name;
                row.Cells[3].Value = sr.StudentNumber;
                dataGridViewX1.Rows.Add(row);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool IsTemp(string id)
        {
            foreach (DataGridViewRow row in dataGridViewX1.Rows)
            {
                if ("" + row.Tag == id)
                {
                    return true;
                }
            }

            return false;
        }

        private void tbClassName_TextChanged(object sender, EventArgs e)
        {
            //如果不鎖定3碼判斷,無法確認Focus座號的時間點
            if (tbClassName.Text.Length < integerInput2.Value)
                return;

            if (checkBoxX1.Checked)
            {
                if (ClassNameDic.ContainsKey(tbClassName.Text))
                {
                    tbClassName.Text = ClassNameDic[tbClassName.Text];
                }
            }

            if (!sMag.IsClassName(tbClassName.Text))
            {
                string message = "查無此班級「" + tbClassName.Text + "」";
                errorProvider1.SetError(tbClassName, message);

                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridViewX2);
                row.Cells[0].Value = message;
                row.ErrorText = message;
                dataGridViewX2.Rows.Add(row);

                if (tbClassName.Text.Length >= 3)
                {
                    tbClassName.SelectAll();
                }

            }
            else
            {
                errorProvider1.Clear();
                tbSean.Focus();
                tbSean.SelectAll();
            }

        }

        private void tbSean_TextChanged(object sender, EventArgs e)
        {
            if (tbSean.Text.Length < 2)
            {
                return;
            }

            studentObj sr = sMag.IsSeatNo(tbClassName.Text, tbSean.Text);

            if (sr == null)
            {
                string message = "班級「" + tbClassName.Text + "」查無此座號「" + tbSean.Text + "」";

                errorProvider2.SetError(tbSean, message);

                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridViewX2);
                row.ErrorText = message;
                row.Cells[0].Value = message;
                dataGridViewX2.Rows.Add(row);

                if (tbSean.Text.Length >= 2)
                {
                    tbClassName.Focus();
                    tbClassName.SelectAll();
                }
            }
            else
            {
                errorProvider1.Clear();
                errorProvider2.Clear();

                if (!IsTemp(sr.student_id))
                {
                    K12.Presentation.NLDPanels.Student.AddToTemp(new List<string>() { sr.student_id });

                    string message = "加入待處理成功！班級「" + tbClassName.Text + "」座號「" + tbSean.Text + "」學生「" + sr.student_name + "」";

                    tbClassName.Focus();
                    tbClassName.SelectAll();
                }
                else
                {
                    string message = "重覆加入待處理！班級「" + tbClassName.Text + "」座號「" + tbSean.Text + "」學生「" + sr.student_name + "」";
                    DataGridViewRow row = new DataGridViewRow();
                    row.ErrorText = message;
                    row.CreateCells(dataGridViewX2);
                    row.Cells[0].Value = message;
                    dataGridViewX2.Rows.Add(row);

                    tbClassName.Focus();
                    tbClassName.SelectAll();
                }

            }
        }

        private void tbStudentNumber_TextChanged(object sender, EventArgs e)
        {
            if (checkBoxX2.Checked)
            {
                if (tbStudentNumber.Text.Length < integerInput1.Value)
                    return;

                SetNumberIndex(tbStudentNumber.Text);
            }
        }

        private void SetNumberIndex(string StudentNumberValue)
        {
            studentObj sr = sMag.IsStudentNumber(StudentNumberValue);

            if (sr == null)
            {
                string message = "查無此學號「" + StudentNumberValue + "」";
                errorProvider3.SetError(tbStudentNumber, message);

                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridViewX2);
                row.ErrorText = message;
                row.Cells[0].Value = message;
                dataGridViewX2.Rows.Add(row);

                tbStudentNumber.Focus();
                tbStudentNumber.SelectAll();
            }
            else
            {
                errorProvider3.Clear();

                if (!IsTemp(sr.student_id))
                {

                    K12.Presentation.NLDPanels.Student.AddToTemp(new List<string>() { sr.student_id });

                    string message = "加入待處理成功！學號「" + StudentNumberValue + "」學生「" + sr.student_name + "」";

                    tbStudentNumber.Focus();
                    tbStudentNumber.SelectAll();
                }
                else
                {
                    string message = "重覆加入待處理！學號「" + StudentNumberValue + "」學生「" + sr.student_name + "」";
                    DataGridViewRow row = new DataGridViewRow();
                    row.ErrorText = message;
                    row.CreateCells(dataGridViewX2);
                    row.Cells[0].Value = message;
                    dataGridViewX2.Rows.Add(row);

                    tbStudentNumber.Focus();
                    tbStudentNumber.SelectAll();
                }
            }
        }

        private void btnClearMessage_Click(object sender, EventArgs e)
        {
            dataGridViewX2.Rows.Clear();
        }

        private void btnClearTemp_Click(object sender, EventArgs e)
        {
            dataGridViewX1.Rows.Clear();
            K12.Presentation.NLDPanels.Student.RemoveFromTemp(K12.Presentation.NLDPanels.Student.TempSource);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SetClassCode cc = new SetClassCode(Code);
            cc.ShowDialog();

            ClassNameDic = DataSort.GetClassNameDic(Code);
        }

        //使用班級名稱代碼
        private void checkBoxX1_CheckedChanged(object sender, EventArgs e)
        {
            SuperD s = new SuperD();
            s.Acode = Code3;
            s.Bvalue = checkBoxX1.Checked.ToString();

            if (!bgwConfig.IsBusy)
                bgwConfig.RunWorkerAsync(s);
        }

        //學號判斷
        private void checkBoxX2_CheckedChanged(object sender, EventArgs e)
        {
            integerInput1.Enabled = checkBoxX2.Checked;

            SuperD s = new SuperD();
            s.Acode = Code4;
            s.Bvalue = checkBoxX2.Checked.ToString();

            if (!bgwConfig.IsBusy)
                bgwConfig.RunWorkerAsync(s);
        }

        private void dataGridViewX2_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            dataGridViewX2.FirstDisplayedScrollingRowIndex = dataGridViewX2.Rows.Count - 1;
        }

        private void dataGridViewX1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            dataGridViewX1.FirstDisplayedScrollingRowIndex = dataGridViewX1.Rows.Count - 1;

            //2019/11/19 - Dylan新增清楚的選擇對象

            //清除選擇
            foreach (DataGridViewRow row in dataGridViewX1.Rows)
            {
                row.Selected = false;
            }
            //增加選到最下方一筆
            if (e.RowIndex == AllIndex - 1)
            {
                dataGridViewX1.Rows[dataGridViewX1.Rows.Count - 1].Selected = true;
            }

        }

        private void tbClassName_Enter(object sender, EventArgs e)
        {
            //tbClassName.SelectAll();
        }

        private void tbSean_Enter(object sender, EventArgs e)
        {
            //tbSean.SelectAll();
        }

        private void tbStudentNumber_Enter(object sender, EventArgs e)
        {
            //tbStudentNumber.SelectAll();
        }

        private void 移出待處理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> TempStudentList = new List<string>();
            foreach (DataGridViewRow row in dataGridViewX1.SelectedRows)
            {
                string sr = "" + row.Tag;
                if (!TempStudentList.Contains(sr))
                {
                    TempStudentList.Add(sr);
                }
            }
            K12.Presentation.NLDPanels.Student.RemoveFromTemp(TempStudentList);
        }

        private void 清空待處理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridViewX1.Rows.Clear();
            K12.Presentation.NLDPanels.Student.RemoveFromTemp(K12.Presentation.NLDPanels.Student.TempSource);
        }

        private void SpeedAddFormIs_FormClosing(object sender, FormClosingEventArgs e)
        {
            K12.Presentation.NLDPanels.Student.TempSourceChanged -= new EventHandler(Student_TempSourceChanged);
        }

        private void btnSelectTempStudent_Click(object sender, EventArgs e)
        {
            K12.Presentation.NLDPanels.Student.DisplayStatus = DisplayStatus.Temp;
            if (K12.Presentation.NLDPanels.Student.DisplayStatus == DisplayStatus.Temp)
                K12.Presentation.NLDPanels.Student.SelectAll();

            foreach (DataGridViewRow row in dataGridViewX1.Rows)
                row.Selected = true;
        }

        private void tbStudentNumber_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SetNumberIndex(tbStudentNumber.Text);
            }
        }

        private void integerInput1_ValueChanged(object sender, EventArgs e)
        {
            SuperD s = new SuperD();
            s.Acode = Code5;
            s.Bvalue = integerInput1.Value.ToString();

            if (!bgwConfig.IsBusy)
                bgwConfig.RunWorkerAsync(s);
        }

        private void checkBoxX4_CheckedChanged(object sender, EventArgs e)
        {
            RunAddData();

            SuperD s = new SuperD();
            s.Acode = Code6;
            s.Bvalue = checkBoxX4.Checked.ToString();

            if (!bgwConfig.IsBusy)
                bgwConfig.RunWorkerAsync(s);
        }

        private void checkBoxX3_CheckedChanged(object sender, EventArgs e)
        {
            RunAddData();

            SuperD s = new SuperD();
            s.Acode = Code6;
            s.Bvalue = checkBoxX4.Checked.ToString();

            if (!bgwConfig.IsBusy)
                bgwConfig.RunWorkerAsync(s);
        }
    }

    class SuperD
    {
        public string Acode { get; set; }
        public string Bvalue { get; set; }
    }
}
