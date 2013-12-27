using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.DSAUtil;
using K12.Data;
using System.Xml;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;

namespace K12.Behavior.AttendanceEdit
{
    static class SingleEditorMethod
    {
        /// <summary>
        /// 傳入學生,取得學生為主的標頭字串
        /// </summary>
        public static string GetStudentTitleName(StudentRecord student)
        {
            StringBuilder sb = new StringBuilder();
            string ClassName = student.Class != null ? student.Class.Name : "";
            string SeatNo = student.SeatNo.HasValue ? student.SeatNo.Value.ToString() : "";
            sb.Append("班級：<b>" + ClassName + "</b>　");
            sb.Append("座號：<b>" + SeatNo + "</b>　");
            sb.Append("姓名：<b>" + student.Name + "</b>　");
            sb.Append("學號：<b>" + student.StudentNumber + "</b>");
            return sb.ToString();

        }

        /// <summary>
        /// 取得缺曠[單人多天]之設定檔,預設XML資料
        /// </summary>
        public static string GetSingleEditorDef()
        {
            DSXmlHelper helper = new DSXmlHelper("xml");
            helper.AddElement("StartDate");
            helper.AddText("StartDate", DateTime.Today.AddDays(-6).ToShortDateString());

            helper.AddElement("EndDate");
            helper.AddText("EndDate", DateTime.Today.ToShortDateString());

            helper.AddElement("Locked");
            helper.AddText("Locked", "false");

            return helper.BaseElement.OuterXml;
        }

        /// <summary>
        /// 取得缺曠[多人單天]之設定檔,預設XML資料
        /// </summary>
        public static string GetMutiEditorDef()
        {
            DSXmlHelper helper = new DSXmlHelper("xml");
            helper.AddElement("Date");
            helper.AddText("Date", DateTime.Today.ToShortDateString());
            helper.AddElement("Locked");
            helper.AddText("Locked", "false");
            return helper.BaseElement.OuterXml;
        }

        /// <summary>
        /// 儲存使用者之日期使用資料
        /// </summary>
        public static void SetSingleEditor(async a)
        {
            BackgroundWorker BGW = new BackgroundWorker();
            BGW.DoWork += new DoWorkEventHandler(BGW_DoWork);
            BGW.RunWorkerAsync(a);
        }

        static void BGW_DoWork(object sender, DoWorkEventArgs e)
        {
            async b = (async)e.Argument;
            K12.Data.Configuration.ConfigData DateConfig = K12.Data.School.Configuration[b.def];
            DSXmlHelper helper = new DSXmlHelper("xml");
            helper.AddElement("StartDate");
            helper.AddText("StartDate", b.dt1.ToShortDateString());
            helper.AddElement("EndDate");
            helper.AddText("EndDate", b.dt2.ToShortDateString());
            helper.AddElement("Locked");
            helper.AddText("Locked", b.locked.ToString());
            DateConfig["SingleEditor"] = helper.BaseElement.OuterXml;
            DateConfig.Save(); //儲存此預設檔
        }

        /// <summary>
        /// 儲存使用者之日期使用資料
        /// </summary>
        public static void SetMutiEditor(asyncMuti a)
        {
            BackgroundWorker BGW1 = new BackgroundWorker();
            BGW1.DoWork += new DoWorkEventHandler(BGW1_DoWork);
            BGW1.RunWorkerAsync(a);
        }

        static void BGW1_DoWork(object sender, DoWorkEventArgs e)
        {
            asyncMuti b = (asyncMuti)e.Argument;
            K12.Data.Configuration.ConfigData DateConfig = K12.Data.School.Configuration[b.def];
            DSXmlHelper helper = new DSXmlHelper("xml");
            helper.AddElement("Date");
            helper.AddText("Date", b.dt1.ToShortDateString());
            helper.AddElement("Locked");
            helper.AddText("Locked", b.locked.ToString());
            DateConfig["MutiEditor"] = helper.BaseElement.OuterXml;
            DateConfig.Save(); //儲存此預設檔
        }

        /// <summary>
        /// 依據開始與結束時間,判斷資料是否超過1500日
        /// </summary>
        public static bool CheckDateTime(DateTime start, DateTime end)
        {
            //計算日期區間
            bool check = false;
            TimeSpan ts = end - start;
            if (ts.Days > 1500) //大於1500天則警告
            {
                check = true; //true為錯誤
            }
            return check;
        }

        /// <summary>
        /// 依據英文星期名稱,回傳該星期之中文名稱
        /// </summary>
        public static string GetDayOfWeekInChinese(DayOfWeek day)
        {
            switch (day)
            {
                case DayOfWeek.Monday:
                    return "一";
                case DayOfWeek.Tuesday:
                    return "二";
                case DayOfWeek.Wednesday:
                    return "三";
                case DayOfWeek.Thursday:
                    return "四";
                case DayOfWeek.Friday:
                    return "五";
                case DayOfWeek.Saturday:
                    return "六";
                default:
                    return "日";
            }
        }

        /// <summary>
        /// 排序每日節次對照表的順序
        /// </summary>
        public static int SortByOrder(PeriodMappingInfo info1, PeriodMappingInfo info2)
        {
            return info1.Sort.CompareTo(info2.Sort);
        }

        /// <summary>
        /// 取得星期對照表
        /// </summary>
        public static List<DayOfWeek> GetDayOfWeek(string setupString)
        {
            K12.Data.Configuration.ConfigData _WeekConfig = K12.Data.School.Configuration[setupString];
            List<string> list = GetWeekDay(_WeekConfig);
            List<DayOfWeek> DOW = new List<DayOfWeek>();
            foreach (string each in list)
            {
                if (each == "星期一")
                {
                    DOW.Add(DayOfWeek.Monday);
                }
                else if (each == "星期二")
                {
                    DOW.Add(DayOfWeek.Tuesday);
                }
                else if (each == "星期三")
                {
                    DOW.Add(DayOfWeek.Wednesday);
                }
                else if (each == "星期四")
                {
                    DOW.Add(DayOfWeek.Thursday);
                }
                else if (each == "星期五")
                {
                    DOW.Add(DayOfWeek.Friday);
                }
                else if (each == "星期六")
                {
                    DOW.Add(DayOfWeek.Saturday);
                }
                else if (each == "星期日")
                {
                    DOW.Add(DayOfWeek.Sunday);
                }
            }

            return DOW;
        }

        /// <summary>
        /// 依據"星期設定"取得星期清單
        /// </summary>
        /// <returns></returns>
        public static List<string> GetWeekDay(K12.Data.Configuration.ConfigData cd)
        {
            List<string> list = new List<string>();
            K12.Data.Configuration.ConfigData _cd = cd;
            string cdIN = cd["星期設定"];

            XmlElement day;

            if (cdIN != "")
            {
                day = XmlHelper.LoadXml(cdIN);
            }
            else
            {
                day = null;
            }

            if (day != null)
            {
                foreach (XmlNode each in day.SelectNodes("Day"))
                {
                    XmlElement each2 = each as XmlElement;
                    list.Add(each2.GetAttribute("Detail"));
                }
            }
            else
            {
                list.AddRange(new string[] { "星期一", "星期二", "星期三", "星期四", "星期五", "星期六", "星期日" });
            }

            return list;
        }

        /// <summary>
        /// 傳入缺曠,開始與結束,以取得符合之資料
        /// </summary>
        public static List<AttendanceRecord> GetFilterAttendance(List<AttendanceRecord> list, DateTime Start, DateTime End)
        {
            List<AttendanceRecord> attendList = new List<AttendanceRecord>();
            //缺曠資料清單,並判斷是否為開始結束日期之資料
            foreach (AttendanceRecord each in list)
            {
                //當缺曠日期(大於/等於)起始日期/缺曠日期(小於等於)
                //CompareTo => 大於為1 / 等於為0 / 小於為-1
                if (each.OccurDate.CompareTo(Start) != -1 && each.OccurDate.CompareTo(End) != 1)
                {
                    attendList.Add(each);
                }
            }
            return attendList;
        }

        /// <summary>
        /// 傳入使用者按下之鈕
        /// 將NumPad(x)與D(x)整合為相同之熱鍵鈕
        /// </summary>
        public static string GetKeyMapping(KeyEventArgs key)
        {
            switch (key.KeyData)
            {
                case Keys.NumPad0:
                    return "0";
                case Keys.NumPad1:
                    return "1";
                case Keys.NumPad2:
                    return "2";
                case Keys.NumPad3:
                    return "3";
                case Keys.NumPad4:
                    return "4";
                case Keys.NumPad5:
                    return "5";
                case Keys.NumPad6:
                    return "6";
                case Keys.NumPad7:
                    return "7";
                case Keys.NumPad8:
                    return "8";
                case Keys.NumPad9:
                    return "9";
                case Keys.D0:
                    return "0";
                case Keys.D1:
                    return "1";
                case Keys.D2:
                    return "2";
                case Keys.D3:
                    return "3";
                case Keys.D4:
                    return "4";
                case Keys.D5:
                    return "5";
                case Keys.D6:
                    return "6";
                case Keys.D7:
                    return "7";
                case Keys.D8:
                    return "8";
                case Keys.D9:
                    return "9";
                default:
                    return key.KeyCode.ToString().ToUpper();
            }
        }

        /// <summary>
        /// 傳入欄位名稱,標題,是否可以修改,不予以排序,AutoSize
        /// 等資訊來建立Column,並回傳建立Index
        /// </summary>
        /// <param name="columnName">Column名稱</param>
        /// <param name="columnTitle">Column標頭顯示</param>
        /// <param name="readOnly">是否鎖定此控制項</param>
        /// <param name="SortMode">是否以NotSortable進行排序</param>
        /// <param name="mode">AutoSizeMode</param>
        /// <returns></returns>
        public static DataGridViewColumn SetColumn(DataGridView dgv, string columnName, string columnTitle, bool readOnly, bool SortMode, DataGridViewAutoSizeColumnMode mode, Color bgColor)
        {
            DataGridViewTextBoxColumn dgvC = new DataGridViewTextBoxColumn();
            dgvC.Name = columnName;
            dgvC.HeaderText = columnTitle;
            if (SortMode)
                dgvC.SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvC.AutoSizeMode = mode;
            dgvC.ReadOnly = readOnly;
            dgvC.DefaultCellStyle.BackColor = bgColor;

            return dgvC;
        }
    }

    class helper
    {
        public List<AttendanceRecord> InsertHelper { get; set; }
        public List<AttendanceRecord> updateHelper { get; set; }
        public List<string> deleteList { get; set; }
        public helper()
        {
            InsertHelper = new List<AttendanceRecord>();
            updateHelper = new List<AttendanceRecord>();
            deleteList = new List<string>();
        }
    }

    class async
    {
        public DateTime dt1 { get; set; }
        public DateTime dt2 { get; set; }
        public bool locked { get; set; }
        public string def { get; set; }
    }

    class asyncMuti
    {
        public DateTime dt1 { get; set; }
        public bool locked { get; set; }
        public string def { get; set; }
    }
}
