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

        K12.Data.Configuration.ConfigData cd;

        Dictionary<string, string> ClassNameDic = new Dictionary<string, string>();

        public SpeedAddFormIs()
        {
            InitializeComponent();
        }

        private void SpeedAddFormIs_Load(object sender, EventArgs e)
        {
            sMag = new StudentMag();

            K12.Presentation.NLDPanels.Student.TempSourceChanged += new EventHandler(Student_TempSourceChanged);

            ClassNameDic = DataSort.GetClassNameDic(Code);

            cd = School.Configuration[Code2];

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

            integerInput1.Enabled = checkBoxX2.Checked;

            TempSourceIpr();

            tbClassName.Focus();
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
            dataGridViewX1.Rows.Clear();

            lbCount.Text = "待處理共「" + K12.Presentation.NLDPanels.Student.TempSource.Count().ToString() + "」名學生";
            //取得待處理學生
            List<StudentRecord> StudentList = K12.Data.Student.SelectByIDs(K12.Presentation.NLDPanels.Student.TempSource);
            StudentList = SortClassIndex.K12Data_StudentRecord(StudentList);

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
            FormLocked = true;
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
            cd = School.Configuration[Code2];
            cd[Code3] = checkBoxX1.Checked.ToString();
            cd.Save();
        }

        //學號判斷
        private void checkBoxX2_CheckedChanged(object sender, EventArgs e)
        {
            integerInput1.Enabled = checkBoxX2.Checked;

            cd = School.Configuration[Code2];
            cd[Code4] = checkBoxX2.Checked.ToString();
            cd.Save();
        }

        private void dataGridViewX2_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            dataGridViewX2.FirstDisplayedScrollingRowIndex = dataGridViewX2.Rows.Count - 1;
        }

        private void dataGridViewX1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            dataGridViewX1.FirstDisplayedScrollingRowIndex = dataGridViewX1.Rows.Count - 1;
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
            cd = School.Configuration[Code2];
            cd[Code5] = integerInput1.Value.ToString();
            cd.Save();
        }
    }
}
