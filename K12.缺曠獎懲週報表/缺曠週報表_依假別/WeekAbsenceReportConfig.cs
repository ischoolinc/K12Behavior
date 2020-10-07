using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using FISCA.Presentation.Controls;
using K12.Data.Configuration;

namespace K12.缺曠獎懲週報表
{
    public partial class WeekAbsenceReportConfig : BaseForm
    {
        private string _reportName = "";
        public WeekAbsenceReportConfig(string reportname, int sizeIndex, bool CheckClass, bool CheckWeek, string Remarkcix)
        {
            InitializeComponent();

            _reportName = reportname;
            comboBoxEx1.SelectedIndex = sizeIndex;
            checkBoxX1.Checked = CheckClass;
            checkBoxX2.Checked = CheckWeek;
            textBoxX1.Text = Remarkcix;
        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            //if (textBoxX1.Text.Length > 500)
            //{
            //    MsgBox.Show(string.Format("請勿超過500字(目前字數{0})", textBoxX1.Text.Length));
            //    return;
            //}


            #region 儲存 Preference

            //XmlElement config = CurrentUser.Instance.Preference[_reportName];
            //ConfigData cd = User.Configuration[_reportName];
            ConfigData cd = K12.Data.School.Configuration[_reportName];
            XmlElement config = cd.GetXml("XmlData", null);

            if (config == null)
                config = new XmlDocument().CreateElement(_reportName);

            #region PaperSize
            XmlElement print = config.OwnerDocument.CreateElement("Print");
            print.SetAttribute("PaperSize", comboBoxEx1.SelectedIndex.ToString());

            if (config.SelectSingleNode("Print") == null)
                config.AppendChild(print);
            else
                config.ReplaceChild(print, config.SelectSingleNode("Print"));
            #endregion

            #region CheckClass
            XmlElement TestClass = config.OwnerDocument.CreateElement("CheckClass");
            TestClass.SetAttribute("Class", checkBoxX1.Checked.ToString());

            if (config.SelectSingleNode("CheckClass") == null)
                config.AppendChild(TestClass);
            else
                config.ReplaceChild(TestClass, config.SelectSingleNode("CheckClass"));
            #endregion

            #region CheckWeek
            XmlElement CheckWeek = config.OwnerDocument.CreateElement("CheckWeek");
            CheckWeek.SetAttribute("Week", checkBoxX2.Checked.ToString());

            if (config.SelectSingleNode("CheckWeek") == null)
                config.AppendChild(CheckWeek);
            else
                config.ReplaceChild(CheckWeek, config.SelectSingleNode("CheckWeek"));
            #endregion

            #region TextRemark
            XmlElement TextRemark = config.OwnerDocument.CreateElement("TextRemark");
            TextRemark.SetAttribute("Remark", textBoxX1.Text);

            if (config.SelectSingleNode("TextRemark") == null)
                config.AppendChild(TextRemark);
            else
                config.ReplaceChild(TextRemark, config.SelectSingleNode("TextRemark"));
            #endregion

            //CurrentUser.Instance.Preference[_reportName] = config;

            cd.SetXml("XmlData", config);
            cd.Save();

            #endregion

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonX2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void textBoxX1_TextChanged(object sender, EventArgs e)
        {
            labelX3.Text = string.Format("請勿超過500字(字數{0})", textBoxX1.Text.Length);

            if (textBoxX1.Text.Length > 500)
            {
                textBoxX1.ForeColor = Color.Red;
            }
            else
            {
                textBoxX1.ForeColor = labelX2.ForeColor;
            }
        }
    }
}