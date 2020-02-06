using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using K12.Data.Configuration;

namespace K12.�g�ٳq����
{
    public partial class DemeritDateRangeForm : SelectDateRangeForm
    {
        private MemoryStream _template = null;
        private byte[] _buffer = null;

        //private bool _preferenceLoaded = false;
        //private bool _printHasRecordOnly = true;

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
                MemoryStream _defaultTemplate;

                if (_useDefaultTemplate == "�ۭq�d��")
                {
                    return _template;
                }
                else if (_useDefaultTemplate == "�w�]�d��2")
                {
                    return _defaultTemplate = new MemoryStream(Properties.Resources.�g�ٳq����);
                }
                else //�w�]�d��1 
                {
                    return _defaultTemplate = new MemoryStream(Properties.Resources.�g�ٳq����_��}�����d��);
                }
            }
        }

        private DateRangeModeNew _mode = DateRangeModeNew.Month;

        private string _useDefaultTemplate = "�w�]�d��1";

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

        public DemeritDateRangeForm()
        {
            InitializeComponent();
            Text = "�g�ٳq����";
            LoadPreference();
            InitialDateRange();
        }

        private void LoadPreference()
        {
            #region Ū�� Preference

            //XmlElement config = CurrentUser.Instance.Preference["�g�ٳq����"];
            ConfigData cd = K12.Data.School.Configuration["�g�ٳq����_ForK12"];
            XmlElement config = cd.GetXml("XmlData", null);

            if (config != null)
            {
                _useDefaultTemplate = config.GetAttribute("Default");

                XmlElement customize = (XmlElement)config.SelectSingleNode("CustomizeTemplate");
                XmlElement dateRangeMode = (XmlElement)config.SelectSingleNode("DateRangeMode");
                XmlElement receive = (XmlElement)config.SelectSingleNode("Receive");
                XmlElement conditions = (XmlElement)config.SelectSingleNode("Conditions");
                XmlElement PrintStudentList = (XmlElement)config.SelectSingleNode("PrintStudentList");
                XmlElement PrintRemark = (XmlElement)config.SelectSingleNode("PrintRemark");

                //�C�L�ǥͲM��
                if (PrintStudentList != null)
                {
                    if (PrintStudentList.HasAttribute("Checked"))
                    {
                        if (PrintStudentList.GetAttribute("Checked") != "")
                        {
                            _PrintStudentList = bool.Parse(PrintStudentList.GetAttribute("Checked"));
                        }
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

                if (customize != null)
                {
                    string templateBase64 = customize.InnerText;
                    _buffer = Convert.FromBase64String(templateBase64);
                    _template = new MemoryStream(_buffer);
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
                        _conditionName = "�j�L";
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
                }

                if (dateRangeMode != null)
                {
                    _mode = (DateRangeModeNew)int.Parse(dateRangeMode.InnerText);
                    if (_mode != DateRangeModeNew.Custom)
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
                }
            }
            else
            {
                #region ���ͪťճ]�w��
                config = new XmlDocument().CreateElement("�g�ٳq����");
                config.SetAttribute("Default", "�w�]�d��1");
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
                //CurrentUser.Instance.Preference["�g�ٳq����"] = config;

                _useDefaultTemplate = "�w�]�d��1";
                //_printHasRecordOnly = true;
                _PrintStudentList = false;
                _PrintRemark = false;
                #endregion
            }

            cd.Save(); //�x�s�պA��ơC

            #endregion

            //_preferenceLoaded = true;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //�ǤJTrue�O�]�����v�T�{�����c
            DemeritConfigForm configForm = new DemeritConfigForm(
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
                case DateRangeModeNew.Month: //��
                    {
                        DateTime a = dateTimeInput1.Value;
                        a = GetMonthFirstDay(a);
                        dateTimeInput1.Text = a.ToShortDateString();
                        dateTimeInput2.Text = a.AddMonths(1).AddDays(-1).ToShortDateString();
                        break;
                    }
                case DateRangeModeNew.Week: //�g
                    {
                        DateTime b = dateTimeInput1.Value;
                        b = GetWeekFirstDay(b);
                        dateTimeInput1.Text = b.ToShortDateString();
                        dateTimeInput2.Text = b.AddDays(5).ToShortDateString();
                        break;
                    }
                case DateRangeModeNew.Custom: //�ۭq
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

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dateTimeInput1_TextChanged(object sender, EventArgs e)
        {
            if (_startTextBoxOK && _mode != DateRangeModeNew.Custom)
            {
                switch (_mode)
                {
                    case DateRangeModeNew.Month: //��
                        {
                            _startDate = GetMonthFirstDay(DateTime.Parse(dateTimeInput1.Text));
                            _endDate = _startDate.AddMonths(1).AddDays(-1);
                            dateTimeInput1.Text = _startDate.ToShortDateString();
                            dateTimeInput2.Text = _endDate.ToShortDateString();
                            _printable = true;
                            break;
                        }
                    case DateRangeModeNew.Week: //�g
                        {
                            _startDate = GetWeekFirstDay(DateTime.Parse(dateTimeInput1.Text));
                            _endDate = _startDate.AddDays(4);
                            dateTimeInput1.Text = _startDate.ToShortDateString();
                            dateTimeInput2.Text = _endDate.ToShortDateString();
                            _printable = true;
                            break;
                        }
                    case DateRangeModeNew.Custom: //�ۭq
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
            //    if (_mode == DateRangeModeNew.Custom)
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

    public enum DateRangeModeNew { Month, Week, Custom }
}