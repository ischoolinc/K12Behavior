using System.Windows.Forms;
using System.Xml;
using K12.Data.Configuration;

namespace K12.缺曠獎懲週報表
{
    public partial class WeekDisciplineReportCount : SelectWeekForm
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

        public bool Weekcix
        {
            get { return _weekcix; }
        }

        public string Remarkcix
        {
            get { return _remarkcix; }
        }

        public WeekDisciplineReportCount()
        {
            InitializeComponent();
            LoadPreference();
        }

        private void LoadPreference()
        {
            #region 讀取 Preference

            //XmlElement config = CurrentUser.Instance.Preference["獎懲週報表_列印設定"];
            //ConfigData cd = User.Configuration["獎懲週報表_列印設定"];
            ConfigData cd = K12.Data.School.Configuration["獎懲週報表_列印設定"];
            XmlElement config = cd.GetXml("XmlData", null);

            if (config != null) //如果是空的
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
                    //CurrentUser.Instance.Preference["獎懲週報表_列印設定"] = config;
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
                    cd.SetXml("XmlData", config);
                }

                XmlElement TextRemark = (XmlElement)config.SelectSingleNode("TextRemark");

                if (TextRemark != null)
                {
                    if (TextRemark.HasAttribute("Remark"))
                        _remarkcix = TextRemark.GetAttribute("Remark");
                }
                else
                {
                    XmlElement newTextRemark = config.OwnerDocument.CreateElement("TextRemark");
                    newTextRemark.SetAttribute("Remark", "false");
                    config.AppendChild(newTextRemark);
                    cd.SetXml("XmlData", config);
                }
            }
            else
            {
                #region 產生空白設定檔
                config = new XmlDocument().CreateElement("獎懲週報表_列印設定");
                XmlElement printSetup = config.OwnerDocument.CreateElement("Print");
                printSetup.SetAttribute("PaperSize", "0");
                config.AppendChild(printSetup);

                XmlElement ClassSetup = config.OwnerDocument.CreateElement("CheckClass");
                ClassSetup.SetAttribute("Class", "false");
                config.AppendChild(ClassSetup);

                XmlElement WeekSetup = config.OwnerDocument.CreateElement("CheckWeek");
                WeekSetup.SetAttribute("Week", "false");
                config.AppendChild(WeekSetup);

                XmlElement Remark = config.OwnerDocument.CreateElement("TextRemark");
                Remark.SetAttribute("Remark", "");
                config.AppendChild(Remark);

                //CurrentUser.Instance.Preference["獎懲週報表_列印設定"] = config;
                cd.SetXml("XmlData", config);
                #endregion
            }

            cd.Save();

            #endregion
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            WeekAbsenceReportConfig config = new WeekAbsenceReportConfig("獎懲週報表_列印設定", _sizeIndex, _classcix, _weekcix, _remarkcix);
            if (config.ShowDialog() == DialogResult.OK)
            {
                LoadPreference();
            }
        }
    }
}