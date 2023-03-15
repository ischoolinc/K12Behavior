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
            this.Text = "�i" + _student.Name + "�j�P�L�@�~";
            dateTimeInput1.Value = DateTime.Today;
        }

        private void ClearDemerit_Load(object sender, EventArgs e)
        {
            List<DemeritRecord> records = Demerit.SelectByStudentIDs(new List<string>() { _student.ID });

            listView.Items.Clear();

            foreach (DemeritRecord rec in records)
            {
                if (rec.Cleared != "�O")
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
            // 2023/3/14 - �W�[���ҨϥΪ̬O�_����J�ɶ�
            if (dateTimeInput1.Text == "0001/01/01 00:00:00" || dateTimeInput1.Text == "")
            {
                errorProvider1.SetError(dateTimeInput1, "�п�J�ɶ����");
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
            // 2023/3/14 - �W�[���ҨϥΪ̬O�_����J�ɶ�
            if (!CheckDateTimeInput())
            {
                MsgBox.Show("�Эץ��ɶ����,�A�x�s!!");
                return;
            }


            if (listView.SelectedItems.Count == 0)
            {
                MsgBox.Show("�Х���ܱ��P�L����");
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
                dr.Cleared = "�O";
                editors.Add(dr);
            }

            try
            {
                Demerit.Update(editors);
            }
            catch (Exception ex)
            {
                MsgBox.Show("�P�L�@�~�x�s����:" + ex.Message);
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("�ǥ͡u" +editors[0].Student.Name +"�v�w�i��妸�P�L�@�~");
            if (editors[0].ClearDate.HasValue)
            {
                sb.AppendLine("�P�L����u" + editors[0].ClearDate.Value.ToShortDateString() + "�v");
            }
            else
            {
                sb.AppendLine("�P�L����u�v");
            }
            sb.AppendLine("�P�L�ƥѡu" + editors[0].ClearReason + "�v");
            sb.AppendLine("�P�L����M��G");
            foreach (DemeritRecord each in editors)
            {
                sb.AppendLine("����u" + each.OccurDate.ToShortDateString() + "�v");
            }


            ApplicationLog.Log("�ǰȨt��.�g�ٸ��", "�妸�P�L�@�~", "sutdent", _student.ID, sb.ToString());
            MsgBox.Show("�ǥͦh���g�ٰO��\n�P�L�@�~���\!");
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
            //    element.SetAttribute("Cleared", "�O");
            //    element.SetAttribute("ClearDate", dateTimeTextBox1.DateString);
            //    element.SetAttribute("ClearReason", textBoxX1.Text);

            //    helper.AddElement("Discipline/Field", "Detail", h.GetRawXml(), true);
            //    helper.AddElement("Discipline", "Condition");
            //    helper.AddElement("Discipline/Condition", "ID", item.Tag.ToString());
            //}

            //try
            //{
            //    EditDiscipline.Update(new DSRequest(helper));

            //    //�g�٬����P�L log
            //    StringBuilder clearDesc = new StringBuilder("");
            //    clearDesc.AppendLine("�ǥͩm�W�G" + Student.Instance.Items[_student.ID].Name + " ");

            //    foreach (ListViewItem item in listView.SelectedItems)
            //    {
            //        clearDesc.AppendLine(item.SubItems[0].Text + " �ƥѬ��u" + item.SubItems[4].Text + "�v���g�٬����w�P�L ");
            //    }

            //    clearDesc.AppendLine("�P�L����G" + dateTimeTextBox1.Text + " ");
            //    clearDesc.AppendLine("�P�L�����G" + textBoxX1.Text);

            //    //Log����
            //    //CurrentUser.Instance.AppLog.Write(EntityType.Student, "�ק���g����", _student.ID, clearDesc.ToString(), "�P�L�@�~", helper.GetRawXml());
            //}
            //catch (Exception ex)
            //{
            //    MsgBox.Show("�P�L��~�x�s����:" + ex.Message);
            //    return;
            //}
        }
    }
}