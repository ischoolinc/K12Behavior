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

namespace K12.Student.SpeedAddToTemp
{
    public partial class x_SpeedAddForm : BaseForm
    {
        StudentMag sMag;

        public string Code = "K12.Behavior.Keyboard.SetClassCode";

        public string Code2 = "K12.Behavior.Keyboard.Config.SpeedAddToTemp";

        string Code3 = "使用班級名稱代碼";

        K12.Data.Configuration.ConfigData cd;

        Dictionary<string, string> ClassNameDic = new Dictionary<string, string>();

        public x_SpeedAddForm()
        {
            InitializeComponent();
        }

        private void SpeedAddForm_Load(object sender, EventArgs e)
        {
            sMag = new StudentMag();

            K12.Presentation.NLDPanels.Student.TempSourceChanged += new EventHandler(Student_TempSourceChanged);

            ClassNameDic = DataSort.GetClassNameDic(Code);

            cd = School.Configuration[Code2];

            if (cd.Contains(Code3))
            {
                checkBoxX1.Checked = bool.Parse(cd[Code3]);
            }

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
                buttonX1.Enabled = value;
                buttonX2.Enabled = value;
                linkLabel1.Enabled = value;
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
                row.Cells[0].Value = sr.Class.Name;
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

        private void 移出待處理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> list = new List<string>();
            foreach (DataGridViewRow row in dataGridViewX1.SelectedRows)
            {
                list.Add("" + row.Tag);
            }

            //把移出之學生,同步由畫面上清除
            foreach (DataGridViewRow row in dataGridViewX1.SelectedRows)
            {
                dataGridViewX1.Rows.Remove(row);
            }

            K12.Presentation.NLDPanels.Student.RemoveFromTemp(list);
        }

        private void tbClassName_Enter(object sender, EventArgs e)
        {
            tbClassName.SelectAll();
        }

        private void tbSean_Enter(object sender, EventArgs e)
        {
            tbSean.SelectAll();
        }

        private void 清空待處理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridViewX1.Rows.Clear();
            K12.Presentation.NLDPanels.Student.RemoveFromTemp(K12.Presentation.NLDPanels.Student.TempSource);
        }

        private void tbClassName_TextChanged(object sender, EventArgs e)
        {
            if (tbClassName.Text.Length < 3)
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
                    tbSean.SelectAll();
                }
            }
            else
            {
                errorProvider1.Clear();
                errorProvider2.Clear();

                if (!IsTemp(sr.student_id))
                {
                    //DataGridViewRow row = new DataGridViewRow();
                    //row.CreateCells(dataGridViewX1);
                    //row.Tag = sr.student_id;
                    //row.Cells[0].Value = sr.class_name;
                    //row.Cells[1].Value = sr.student_seat_no;
                    //row.Cells[2].Value = sr.student_name;
                    //row.Cells[3].Value = sr.student_number;
                    //dataGridViewX1.Rows.Add(row);

                    K12.Presentation.NLDPanels.Student.AddToTemp(new List<string>() { sr.student_id });

                    string message = "加入待處理成功！班級「" + tbClassName.Text + "」座號「" + tbSean.Text + "」學生「" + sr.student_name + "」";
                    //DataGridViewRow Messagerow = new DataGridViewRow();
                    //Messagerow.CreateCells(dataGridViewX2);
                    //Messagerow.Cells[0].Value = message;
                    //dataGridViewX2.Rows.Add(Messagerow);

                    tbClassName.Focus();
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
                }

            }
        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            dataGridViewX2.Rows.Clear();
        }

        private void buttonX2_Click(object sender, EventArgs e)
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

        private void checkBoxX1_CheckedChanged(object sender, EventArgs e)
        {
            cd = School.Configuration[Code2];
            cd[Code3] = checkBoxX1.Checked.ToString();
            cd.Save();
        }

        private void SpeedAddForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            K12.Presentation.NLDPanels.Student.TempSourceChanged -= new EventHandler(Student_TempSourceChanged);
        }

        private void dataGridViewX2_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            dataGridViewX2.FirstDisplayedScrollingRowIndex = dataGridViewX2.Rows.Count - 1;
        }

        private void dataGridViewX1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            dataGridViewX1.FirstDisplayedScrollingRowIndex = dataGridViewX1.Rows.Count - 1;
        }
    }
}
