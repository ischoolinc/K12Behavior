using System.Windows.Forms;
using System.Xml;
using K12.Data.Configuration;

namespace K12.缺曠獎懲週報表
{
    public partial class WeekAbsenceReportCountByPeriod : SelectWeekForm
    {
        private int _sizeIndex = 0;
        private bool _classcix;
        private bool _weekcix;
        private string _remarkcix;

        public int PaperSize
        {
            get { return _sizeIndex; }
        }

        public bool ClassCix
        {
            get { return _classcix; }
        }

        public bool WeekCix
        {
            get { return _weekcix; }
        }

        public string RemarkCix
        {
            get { return _remarkcix; }
        }

        public string ConfigName = "缺曠週報表_依節次統計_列印設定";
        public string ConfigTypeName = "缺曠週報表_依節次統計_列印設定_2";

        public WeekAbsenceReportCountByPeriod()
        {
            InitializeComponent();
            LoadPreference();

            //ConfigData cd = User.Configuration["缺曠週報表_依節次統計_列印設定"];
            ConfigData cd = K12.Data.School.Configuration[ConfigName];
            if (cd.GetXml("XmlData", null) == null)
            {
                new SelectPeriodForm(ConfigName).ShowDialog();
            }
        }

        private void LoadPreference()
        {
            #region 讀取 Preference

            //XmlElement config = CurrentUser.Instance.Preference["缺曠週報表_依節次統計_列印設定"];
            //ConfigData cd = User.Configuration["缺曠週報表_依節次統計_列印設定"];
            ConfigData cd = K12.Data.School.Configuration[ConfigName];
            XmlElement config = cd.GetXml("XmlData", null);

            if (config != null)
            {
                XmlElement print = (XmlElement)config.SelectSingleNode("Print");

                if (print != null)
                {
                    if (print.HasAttribute("PaperSize"))
                        _sizeIndex = int.Parse(print.GetAttribute("PaperSize"));
                }
                else
                {
                    XmlElement newPrint = config.OwnerDocument.CreateElement("Print");
                    newPrint.SetAttribute("PaperSize", "0");
                    config.AppendChild(newPrint);
                    //CurrentUser.Instance.Preference["缺曠週報表_依節次統計_列印設定"] = config;
                    cd.SetXml("XmlData", config);
                }

                XmlElement CheckClass = (XmlElement)config.SelectSingleNode("CheckClass");

                if (CheckClass != null)
                {
                    if (CheckClass.HasAttribute("Class"))
                        _classcix = bool.Parse(CheckClass.GetAttribute("Class"));
                }
                else
                {
                    XmlElement newCheckClass = config.OwnerDocument.CreateElement("CheckClass");
                    newCheckClass.SetAttribute("Class", "false");
                    config.AppendChild(newCheckClass);
                    //CurrentUser.Instance.Preference["缺曠週報表_依缺曠別統計_列印設定"] = config;
                    cd.SetXml("XmlData", config);
                }

                XmlElement CheckWeek = (XmlElement)config.SelectSingleNode("CheckWeek");

                if (CheckWeek != null)
                {
                    if (CheckWeek.HasAttribute("Week"))
                        _weekcix = bool.Parse(CheckWeek.GetAttribute("Week"));
                }
                else
                {
                    XmlElement newCheckWeek = config.OwnerDocument.CreateElement("CheckWeek");
                    newCheckWeek.SetAttribute("Week", "false");
                    config.AppendChild(newCheckWeek);
                    //CurrentUser.Instance.Preference["缺曠週報表_依缺曠別統計_列印設定"] = config;
                    cd.SetXml("XmlData", config);
                }

                //Remark - 2020/9/21新增
                XmlElement Remark = (XmlElement)config.SelectSingleNode("TextRemark");

                if (Remark != null)
                {
                    if (Remark.HasAttribute("Remark"))
                        _remarkcix = Remark.GetAttribute("Remark");
                }
                else
                {
                    XmlElement newRemark = config.OwnerDocument.CreateElement("TextRemark");
                    newRemark.SetAttribute("Remark", "");
                    config.AppendChild(newRemark);
                    cd.SetXml("XmlData", config);
                }
            }
            else
            {
                #region 產生空白設定檔
                config = new XmlDocument().CreateElement(ConfigName);
                XmlElement printSetup = config.OwnerDocument.CreateElement("Print");
                printSetup.SetAttribute("PaperSize", "0");
                config.AppendChild(printSetup);

                XmlElement ClassSetup = config.OwnerDocument.CreateElement("CheckClass");
                ClassSetup.SetAttribute("Class", "false");
                config.AppendChild(ClassSetup);

                XmlElement WeekSetup = config.OwnerDocument.CreateElement("CheckWeek");
                WeekSetup.SetAttribute("Week", "false");
                config.AppendChild(WeekSetup);

                XmlElement RemarkSetup = config.OwnerDocument.CreateElement("TextRemark");
                RemarkSetup.SetAttribute("Remark", "");
                config.AppendChild(RemarkSetup);

                //CurrentUser.Instance.Preference["缺曠週報表_依節次統計_列印設定"] = config;
                cd.SetXml("XmlData", config);
                #endregion
            }
            cd.Save();
            #endregion
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new SelectPeriodForm(ConfigName).ShowDialog();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            WeekAbsenceReportConfig config = new WeekAbsenceReportConfig(ConfigName, _sizeIndex, _classcix, _weekcix, _remarkcix);
            if (config.ShowDialog() == DialogResult.OK)
            {
                LoadPreference();
            }
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new SelectTypeForm(ConfigTypeName).ShowDialog();
        }
    }
}