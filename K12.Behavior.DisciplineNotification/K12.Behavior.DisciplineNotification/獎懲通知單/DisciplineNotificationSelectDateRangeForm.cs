using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Campus.Configuration;

namespace K12.Behavior.DisciplineNotification
{
    public partial class DisciplineNotificationSelectDateRangeForm : SelectDateRangeForm
    {
        private MemoryStream _template = null;
        private MemoryStream _defaultTemplate = new MemoryStream(Properties.Resources.���g�q����);
        private byte[] _buffer = null;

        private bool _preferenceLoaded = false;

        private string _receiveName = "";
        private string _receiveAddress = "";
        private string _conditionName = "";
        private string _conditionNumber = "1";

        public string ReceiveName { get { return _receiveName; } }
        public string ReceiveAddress { get { return _receiveAddress; } }
        public string ConditionName { get { return _conditionName; } }
        public string ConditionNumber { get { return _conditionNumber; } }

        public MemoryStream Template
        {
            get
            {
                if (_useDefaultTemplate || Aspose.Words.Document.DetectFileFormat(_template) != Aspose.Words.LoadFormat.Doc)
                    return _defaultTemplate;
                else
                    return _template;
            }
        }

        private DateRangeMode _mode = DateRangeMode.Month;

        private bool _useDefaultTemplate = true;

        //���ܼƥû���True
        private bool _printHasRecordOnly = true;
        public bool PrintHasRecordOnly
        {
            get { return true; }
        }

        //�O�_�C�L�ǥͲM��
        private bool _PrintStudentList = false;
        public bool PrintStudentList
        {
            get { return _PrintStudentList; }
        }

        //�O�_�C�L�Ƶ�
        private bool _PrintRemark = false;
        public bool PrintRemark
        {
            get { return _PrintRemark; }
        }

        public DisciplineNotificationSelectDateRangeForm()
        {
            InitializeComponent();
            Text = "���y�g�ٳq����";
            LoadPreference();
            InitialDateRange();
        }

        private void LoadPreference()
        {
            #region Ū�� Preference

            //XmlElement config = CurrentUser.Instance.Preference["���g�q����"];
            Campus.Configuration.ConfigData cd = Campus.Configuration.Config.User["���g�q����"];
            XmlElement config = cd.GetXml("XmlData", null);

            if (config != null)
            {
                _useDefaultTemplate = bool.Parse(config.GetAttribute("Default"));

                XmlElement customize = (XmlElement)config.SelectSingleNode("CustomizeTemplate");
                XmlElement dateRangeMode = (XmlElement)config.SelectSingleNode("DateRangeMode");
                XmlElement receive = (XmlElement)config.SelectSingleNode("Receive");
                XmlElement conditions = (XmlElement)config.SelectSingleNode("Conditions");
                XmlElement PrintStudentList = (XmlElement)config.SelectSingleNode("PrintStudentList");
                XmlElement PrintRemark = (XmlElement)config.SelectSingleNode("PrintRemark");

                if (customize != null)
                {
                    string templateBase64 = customize.InnerText;
                    _buffer = Convert.FromBase64String(templateBase64);
                    _template = new MemoryStream(_buffer);
                }

                //�C�L�ǥͲM��
                if (PrintStudentList != null)
                {
                    if (PrintStudentList.HasAttribute("Checked"))
                    {
                        _PrintStudentList = bool.Parse(PrintStudentList.GetAttribute("Checked"));
                    }
                }
                else
                {
                    XmlElement newPrintStudentList = config.OwnerDocument.CreateElement("PrintStudentList");
                    newPrintStudentList.SetAttribute("Checked", "False");
                    config.AppendChild(newPrintStudentList);
                    cd.SetXml("XmlData", config);
                }

                //�C�L�Ƶ�
                if (PrintRemark != null)
                {
                    if (PrintRemark.HasAttribute("Checked"))
                    {
                        if (PrintRemark.GetAttribute("Checked") != "")
                        {
                            _PrintRemark = bool.Parse(PrintRemark.GetAttribute("Checked"));
                        }
                    }
                }
                else
                {
                    XmlElement newPrintStudentList = config.OwnerDocument.CreateElement("PrintRemark");
                    newPrintStudentList.SetAttribute("Checked", "False");
                    config.AppendChild(newPrintStudentList);
                    cd.SetXml("XmlData", config);
                }

                if (receive != null)
                {
                    _receiveName = receive.GetAttribute("Name");
                    _receiveAddress = receive.GetAttribute("Address");
                }
                else
                {
                    XmlElement newReceive = config.OwnerDocument.CreateElement("Receive");
                    newReceive.SetAttribute("Name", "");
                    newReceive.SetAttribute("Address", "");
                    config.AppendChild(newReceive);
                    cd.SetXml("XmlData", config);
                    //CurrentUser.Instance.Preference["���g�q����"] = config;
                }

                if (conditions != null)
                {
                    if (conditions.HasAttribute("ConditionName") && conditions.HasAttribute("ConditionNumber"))
                    {
                        _conditionName = conditions.GetAttribute("ConditionName");
                        _conditionNumber = conditions.GetAttribute("ConditionNumber");
                    }
                    else
                    {
                        _conditionName = "�j�\";
                        _conditionNumber = "1";
                    }
                }
                else
                {
                    XmlElement newConditions = config.OwnerDocument.CreateElement("Conditions");
                    newConditions.SetAttribute("ConditionName", "");
                    newConditions.SetAttribute("ConditionNumber", "1");
                    config.AppendChild(newConditions);
                    cd.SetXml("XmlData", config);
                    //CurrentUser.Instance.Preference["���g�q����"] = config;
                }

                if (dateRangeMode != null)
                {
                    _mode = (DateRangeMode)int.Parse(dateRangeMode.InnerText);
                    if (_mode != DateRangeMode.Custom)
                        dateTimeInput2.Enabled = false;
                    else
                        dateTimeInput2.Enabled = true;
                }
                else
                {
                    XmlElement newDateRangeMode = config.OwnerDocument.CreateElement("DateRangeMode");
                    newDateRangeMode.InnerText = ((int)_mode).ToString();
                    config.AppendChild(newDateRangeMode);
                    cd.SetXml("XmlData", config);
                    //CurrentUser.Instance.Preference["���g�q����"] = config;
                }
            }
            else
            {
                #region ���ͪťճ]�w��
                config = new XmlDocument().CreateElement("���g�q����");
                config.SetAttribute("Default", "true");
                XmlElement customize = config.OwnerDocument.CreateElement("CustomizeTemplate");
                XmlElement dateRangeMode = config.OwnerDocument.CreateElement("DateRangeMode");
                XmlElement receive = config.OwnerDocument.CreateElement("Receive");
                XmlElement conditions = config.OwnerDocument.CreateElement("Conditions");
                XmlElement printStudentList = config.OwnerDocument.CreateElement("PrintStudentList");
                XmlElement printRemark = config.OwnerDocument.CreateElement("PrintRemark");

                dateRangeMode.InnerText = ((int)_mode).ToString();
                receive.SetAttribute("Name", "");
                receive.SetAttribute("Address", "");
                conditions.SetAttribute("ConditionName", "");
                conditions.SetAttribute("ConditionNumber", "1");
                printStudentList.SetAttribute("Checked", "false");
                printRemark.SetAttribute("Checked", "false"); //2019/1/21 - �Ƶ����

                config.AppendChild(customize);
                config.AppendChild(dateRangeMode);
                config.AppendChild(receive);
                config.AppendChild(conditions);
                config.AppendChild(printStudentList);
                config.AppendChild(printRemark);

                cd.SetXml("XmlData", config);
                //CurrentUser.Instance.Preference["���g�q����"] = config;

                _useDefaultTemplate = true;
                _PrintStudentList = false;
                _PrintRemark = false;
                #endregion
            }

            cd.Save(); //�x�s�պA��ơC

            #endregion

            _preferenceLoaded = true;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DisciplineNotificationConfigForm configForm = new DisciplineNotificationConfigForm(
                _useDefaultTemplate, _mode, _buffer, _receiveName, _receiveAddress, _conditionName,
                _conditionNumber, _PrintStudentList, _PrintRemark);

            if (configForm.ShowDialog() == DialogResult.OK)
            {
                LoadPreference();
                InitialDateRange();
            }
        }

        private void InitialDateRange()
        {
            switch (_mode)
            {
                case DateRangeMode.Month: //��
                    {
                        DateTime a = dateTimeInput1.Value;
                        a = GetMonthFirstDay(a);
                        dateTimeInput1.Text = a.ToShortDateString();
                        dateTimeInput2.Text = a.AddMonths(1).AddDays(-1).ToShortDateString();
                        break;
                    }
                case DateRangeMode.Week: //�g
                    {
                        DateTime b = dateTimeInput1.Value;
                        b = GetWeekFirstDay(b);
                        dateTimeInput1.Text = b.ToShortDateString();
                        dateTimeInput2.Text = b.AddDays(5).ToShortDateString();
                        break;
                    }
                case DateRangeMode.Custom: //�ۭq
                    {
                        //dateTimeInput2.Text = dateTimeInput1.Text = DateTime.Today.ToShortDateString();
                        break;
                    }
                default:
                    throw new Exception("Date Range Mode Error.");
            }

            _printable = true;
            _startTextBoxOK = true;
            _endTextBoxOK = true;
        }

        private DateTime GetWeekFirstDay(DateTime inputDate)
        {
            switch (inputDate.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    return inputDate;
                case DayOfWeek.Tuesday:
                    return inputDate.AddDays(-1);
                case DayOfWeek.Wednesday:
                    return inputDate.AddDays(-2);
                case DayOfWeek.Thursday:
                    return inputDate.AddDays(-3);
                case DayOfWeek.Friday:
                    return inputDate.AddDays(-4);
                case DayOfWeek.Saturday:
                    return inputDate.AddDays(-5);
                default:
                    return inputDate.AddDays(-6);
            }
        }

        private DateTime GetMonthFirstDay(DateTime inputDate)
        {
            return DateTime.Parse(inputDate.Year + "/" + inputDate.Month + "/1");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //if (_printable)
            //    dateTimeInput1.Text = _startDate.ToShortDateString();
            //timer1.Stop();
        }

        private void buttonX2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dateTimeInput1_TextChanged(object sender, EventArgs e)
        {
            if (_startTextBoxOK && _mode != DateRangeMode.Custom)
            {
                switch (_mode)
                {
                    case DateRangeMode.Month: //��
                        {
                            _startDate = GetMonthFirstDay(DateTime.Parse(dateTimeInput1.Text));
                            _endDate = _startDate.AddMonths(1).AddDays(-1);
                            dateTimeInput1.Text = _startDate.ToShortDateString();
                            dateTimeInput2.Text = _endDate.ToShortDateString();
                            _printable = true;
                            break;
                        }
                    case DateRangeMode.Week: //�g
                        {
                            _startDate = GetWeekFirstDay(DateTime.Parse(dateTimeInput1.Text));
                            _endDate = _startDate.AddDays(4);
                            dateTimeInput1.Text = _startDate.ToShortDateString();
                            dateTimeInput2.Text = _endDate.ToShortDateString();
                            _printable = true;
                            break;
                        }
                    case DateRangeMode.Custom: //�ۭq
                        break;
                    default:
                        throw new Exception("Date Range Mode Error");
                }

                //if (dateTimeInput1.Text != _startDate.ToShortDateString() && timer1 != null)
                //    timer1.Start();
                errorProvider1.Clear();

            }
        }

        private void dateTimeInput2_TextChanged(object sender, EventArgs e)
        {
            //if (_preferenceLoaded)
            //{
            //    if (_mode == DateRangeMode.Custom)
            //    {
            //        base.dateTimeInput2_TextChanged(sender, e);
            //    }
            //    else
            //    {
            //        _endTextBoxOK = true;
            //        errorProvider2.Clear();
            //    }
            //}
        }
    }

    public enum DateRangeMode { Month, Week, Custom }
}