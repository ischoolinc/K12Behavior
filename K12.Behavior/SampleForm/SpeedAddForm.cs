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

namespace K12.Behavior
{
    public partial class SpeedAddForm : BaseForm
    {
        StudentMag sMag;

        public SpeedAddForm()
        {
            InitializeComponent();
        }

        private void SpeedAddForm_Load(object sender, EventArgs e)
        {
            sMag = new StudentMag();
            K12.Presentation.NLDPanels.Student.TempSourceChanged += new EventHandler(Student_TempSourceChanged);
            lbCount.Text = "待處理共" + K12.Presentation.NLDPanels.Student.TempSource.Count().ToString() + "名學生";

            foreach (StudentRecord sr in Student.SelectByIDs(K12.Presentation.NLDPanels.Student.TempSource))
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
        }

        void Student_TempSourceChanged(object sender, EventArgs e)
        {
            lbCount.Text = "待處理共" + K12.Presentation.NLDPanels.Student.TempSource.Count().ToString() + "名學生";
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tbClassName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!sMag.IsClassName(tbClassName.Text))
                {
                    lbMessage.Text = "訊息:查無此班級!!";
                    errorProvider1.SetError(tbClassName, "查無此班級!!");
                }
                else
                {
                    lbMessage.Text = "";
                    errorProvider1.Clear();
                }

                tbSean.Focus();
            }
        }

        private void tbSean_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                StudentRecord sr = sMag.IsSeatNo(tbClassName.Text, tbSean.Text);

                if (sr == null)
                {
                    lbMessage.Text = "訊息:查無此座號!!";
                    errorProvider2.SetError(tbSean, "查無此座號!!");
                }
                else
                {
                    lbMessage.Text = "";
                    errorProvider1.Clear();
                    errorProvider2.Clear();
                    K12.Presentation.NLDPanels.Student.AddToTemp(new List<string>() { sr.ID });


                    if (!IsTemp(sr.ID))
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
                    else
                    {
                        lbMessage.Text = "訊息:學生 " + sr.Name + " 重覆加入待處理!!";
                    }

                }

                tbClassName.Focus();
            }
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
            foreach(DataGridViewRow row in dataGridViewX1.SelectedRows)
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
    }
}
