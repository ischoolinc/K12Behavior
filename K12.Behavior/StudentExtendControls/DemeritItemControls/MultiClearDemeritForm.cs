using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FISCA.DSAUtil;
using System.Xml;
using DevComponents.DotNetBar;

using FISCA.LogAgent;
using K12.Data;
using FISCA.Presentation.Controls;
using DevComponents.DotNetBar.Validator;
using DevComponents.Editors.DateTimeAdv;

namespace K12.Behavior.StudentExtendControls
{
    public partial class MultiClearDemerit : FISCA.Presentation.Controls.BaseForm
    {
        private StudentRecord _student;

        public MultiClearDemerit(StudentRecord student)
        {
            InitializeComponent();
            _student = student;
            this.Text = "【" + _student.Name + "】銷過作業";
            dateTimeInput1.Value = DateTime.Today;
        }

        private void ClearDemerit_Load(object sender, EventArgs e)
        {
            List<DemeritRecord> records = Demerit.SelectByStudentIDs(new List<string>() { _student.ID });

            listView.Items.Clear();

            foreach (DemeritRecord rec in records)
            {
                if (rec.Cleared != "是")
                {
                    ListViewItem item = new ListViewItem(rec.OccurDate.ToShortDateString());
                    if (rec.DemeritA != null)
                    {
                        item.SubItems.Add(rec.DemeritA.ToString());
                    }
                    else
                    {
                        item.SubItems.Add("");
                    }
                    if (rec.DemeritB != null)
                    {
                        item.SubItems.Add(rec.DemeritB.ToString());
                    }
                    else
                    {
                        item.SubItems.Add("");
                    }
                    if (rec.DemeritC != null)
                    {
                        item.SubItems.Add(rec.DemeritC.ToString());
                    }
                    else
                    {
                        item.SubItems.Add("");
                    }
                    item.SubItems.Add(rec.Reason);
                    item.Tag = rec;

                    listView.Items.Add(item);
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void listView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            //?????
            if (listView.FocusedItem == null) return;
            if (Control.ModifierKeys == Keys.Control && e.Item.Selected)
                e.Item.Selected = false;
        }

        private bool CheckDateTimeInput()
        {
            // 2023/3/14 - 增加驗證使用者是否未輸入時間
            if (dateTimeInput1.Text == "0001/01/01 00:00:00" || dateTimeInput1.Text == "")
            {
                errorProvider1.SetError(dateTimeInput1, "請輸入時間日期");
                return false;
            }
            else
            {
                errorProvider1.SetError(dateTimeInput1, "");
            }

            return true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // 2023/3/14 - 增加驗證使用者是否未輸入時間
            if (!CheckDateTimeInput())
            {
                MsgBox.Show("請修正時間欄位,再儲存!!");
                return;
            }


            if (listView.SelectedItems.Count == 0)
            {
                MsgBox.Show("請先選擇欲銷過紀錄");
                return;
            }

            List<DemeritRecord> editors = new List<DemeritRecord>();

            foreach (ListViewItem item in listView.SelectedItems)
            {
                DemeritRecord dr = (DemeritRecord)item.Tag;

                if (dateTimeInput1.Text != "")
                {
                    dr.ClearDate = dateTimeInput1.Value;
                }
                dr.ClearReason = txtReason.Text;
                dr.Cleared = "是";
                editors.Add(dr);
            }

            try
            {
                Demerit.Update(editors);
            }
            catch (Exception ex)
            {
                MsgBox.Show("銷過作業儲存失敗:" + ex.Message);
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("學生「" +editors[0].Student.Name +"」已進行批次銷過作業");
            if (editors[0].ClearDate.HasValue)
            {
                sb.AppendLine("銷過日期「" + editors[0].ClearDate.Value.ToShortDateString() + "」");
            }
            else
            {
                sb.AppendLine("銷過日期「」");
            }
            sb.AppendLine("銷過事由「" + editors[0].ClearReason + "」");
            sb.AppendLine("銷過日期清單：");
            foreach (DemeritRecord each in editors)
            {
                sb.AppendLine("日期「" + each.OccurDate.ToShortDateString() + "」");
            }


            ApplicationLog.Log("學務系統.懲戒資料", "批次銷過作業", "sutdent", _student.ID, sb.ToString());
            MsgBox.Show("學生多筆懲戒記錄\n銷過作業成功!");
            this.Close();

            //DSXmlHelper helper = new DSXmlHelper("UpdateRequest");
            //foreach (ListViewItem item in listView.SelectedItems)
            //{
            //    helper.AddElement("Discipline");
            //    helper.AddElement("Discipline", "Field");

            //    DSXmlHelper h = new DSXmlHelper("Discipline");
            //    XmlElement element = h.AddElement("Demerit");
            //    element.SetAttribute("A", item.SubItems[1].Text);
            //    element.SetAttribute("B", item.SubItems[2].Text);
            //    element.SetAttribute("C", item.SubItems[3].Text);
            //    element.SetAttribute("Cleared", "是");
            //    element.SetAttribute("ClearDate", dateTimeTextBox1.DateString);
            //    element.SetAttribute("ClearReason", textBoxX1.Text);

            //    helper.AddElement("Discipline/Field", "Detail", h.GetRawXml(), true);
            //    helper.AddElement("Discipline", "Condition");
            //    helper.AddElement("Discipline/Condition", "ID", item.Tag.ToString());
            //}

            //try
            //{
            //    EditDiscipline.Update(new DSRequest(helper));

            //    //懲戒紀錄銷過 log
            //    StringBuilder clearDesc = new StringBuilder("");
            //    clearDesc.AppendLine("學生姓名：" + Student.Instance.Items[_student.ID].Name + " ");

            //    foreach (ListViewItem item in listView.SelectedItems)
            //    {
            //        clearDesc.AppendLine(item.SubItems[0].Text + " 事由為「" + item.SubItems[4].Text + "」的懲戒紀錄已銷過 ");
            //    }

            //    clearDesc.AppendLine("銷過日期：" + dateTimeTextBox1.Text + " ");
            //    clearDesc.AppendLine("銷過說明：" + textBoxX1.Text);

            //    //Log部份
            //    //CurrentUser.Instance.AppLog.Write(EntityType.Student, "修改獎懲紀錄", _student.ID, clearDesc.ToString(), "銷過作業", helper.GetRawXml());
            //}
            //catch (Exception ex)
            //{
            //    MsgBox.Show("銷過件業儲存失敗:" + ex.Message);
            //    return;
            //}
        }
    }
}