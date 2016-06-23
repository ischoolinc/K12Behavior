using System;
using System.Collections.Generic;
using System.Xml;
using FISCA.DSAUtil;
using SmartSchool.Customization.Data;
using SmartSchool.Customization.PlugIn.ImportExport;
using SmartSchool.AccessControl;
using System.Text;

namespace K12.Behavior
{
    [FeatureCode("Button0260")]
    class ImportAbsence : ImportProcess
    {

        List<string> periodList = new List<string>();
        Dictionary<string, string> periodType = new Dictionary<string, string>();
        List<string> absenceList = new List<string>();
        private AccessHelper _AccessHelper = new AccessHelper();
        List<string> importStudents = new List<string>();

        public ImportAbsence()
        {
            this.Image = null;
            this.Title = "匯入缺曠紀錄";
            this.Group = "缺曠獎懲";
            this.PackageLimit = 300;
            foreach (string field in new string[] { "學年度", "學期", "日期", "缺曠假別", "缺曠節次" })
            {
                this.ImportableFields.Add(field);
            }
            foreach (string field in new string[] { "學年度", "學期", "日期", "缺曠假別", "缺曠節次" })
            {
                this.RequiredFields.Add(field);
            }
            this.BeginValidate += new EventHandler<BeginValidateEventArgs>(ImportAbsence_BeginValidate);
            this.RowDataValidated += new EventHandler<RowDataValidatedEventArgs>(ImportAbsence_RowDataValidated);
            this.DataImport += new EventHandler<DataImportEventArgs>(ImportAbsence_DataImport);
            this.EndImport += new EventHandler(ImportAbsence_EndImport);
        }

        void ImportAbsence_EndImport(object sender, EventArgs e)
        {
            SmartSchool.StudentRelated.Student.Instance.InvokAttendanceChanged(importStudents.ToArray());
        }

        void ImportAbsence_BeginValidate(object sender, BeginValidateEventArgs e)
        {
            _AccessHelper = new AccessHelper();
            importStudents.Clear();
            periodList.Clear();
            absenceList.Clear();
            periodType.Clear();

            foreach (K12.Data.PeriodMappingInfo info in K12.Data.PeriodMapping.SelectAll())
            {
                if (!periodList.Contains(info.Name))
                    periodList.Add(info.Name);
                if (!periodType.ContainsKey(info.Type))
                    periodType.Add(info.Name, info.Type);
            }

            foreach (K12.Data.AbsenceMappingInfo info in K12.Data.AbsenceMapping.SelectAll())
            {
                if (!absenceList.Contains(info.Name))
                    absenceList.Add(info.Name);
            }

            //取得節次對照表
            //foreach (XmlElement var in SmartSchool.Feature.Basic.Config.GetPeriodList().GetContent().GetElements("Period"))
            //{
            //    string name = var.GetAttribute("Name");
            //    if (!periodList.Contains(name))
            //        periodList.Add(name);
            //    string type = var.GetAttribute("Type");
            //    if ( !periodType.ContainsKey(name) )
            //        periodType.Add(name, type);
            //}

            //取得假別對照表
            //foreach (XmlElement var in SmartSchool.Feature.Basic.Config.GetAbsenceList().GetContent().GetElements("Absence"))
            //{
            //    string name = var.GetAttribute("Name");
            //    if (!absenceList.Contains(name))
            //        absenceList.Add(name);
            //}
        }

        void ImportAbsence_DataImport(object sender, DataImportEventArgs e)
        {
            List<string> keyList = new List<string>();
            Dictionary<string, int> schoolYearMapping = new Dictionary<string, int>();
            Dictionary<string, int> semesterMapping = new Dictionary<string, int>();
            Dictionary<string, DateTime> dateMapping = new Dictionary<string, DateTime>();
            Dictionary<string, string> studentIDMapping = new Dictionary<string, string>();
            Dictionary<string, List<RowData>> rowsMapping = new Dictionary<string, List<RowData>>();

            Dictionary<string, string> oldPeriodTypes = new Dictionary<string, string>();

            //1000512 - 新增檢查匯入日期,是否存在於不同學年度學期
            //ID / 日期 / 學年度學期
            //一名學生於不同學年度學期,不會有相同日期之資料
            Dictionary<string, Dictionary<string, List<string>>> testDic = new Dictionary<string, Dictionary<string, List<string>>>();

            foreach (RowData var in e.Items)
            {
                int schoolYear = int.Parse(var["學年度"]);
                int semester = int.Parse(var["學期"]);
                DateTime date = DateTime.Parse(var["日期"]);
                string studentID = var.ID;

                #region 1000512 - 新增檢查匯入日期,是否存在於不同學年度學期
                if (!testDic.ContainsKey(studentID))
                {
                    testDic.Add(studentID, new Dictionary<string, List<string>>());
                }
                if (!testDic[studentID].ContainsKey(date.ToShortDateString()))
                {
                    testDic[studentID].Add(date.ToShortDateString(), new List<string>());
                }
                if (!testDic[studentID][date.ToShortDateString()].Contains(schoolYear + "_" + semester))
                {
                    testDic[studentID][date.ToShortDateString()].Add(schoolYear + "_" + semester);
                }
                #endregion

                string key = schoolYear + "^_^" + semester + "^_^" + date.ToShortDateString() + "^_^" + studentID;
                if (!keyList.Contains(key))
                {
                    keyList.Add(key);
                    schoolYearMapping.Add(key, schoolYear);
                    semesterMapping.Add(key, semester);
                    dateMapping.Add(key, date);
                    studentIDMapping.Add(key, studentID);
                    rowsMapping.Add(key, new List<RowData>());
                }
                rowsMapping[key].Add(var);
            }

            List<StudentRecord> StudentList = _AccessHelper.StudentHelper.GetStudents(studentIDMapping.Values); ;
            Dictionary<string, StudentRecord> Students = new Dictionary<string, StudentRecord>();
            foreach (StudentRecord stu in StudentList)
            {
                if (!Students.ContainsKey(stu.StudentID))
                    Students.Add(stu.StudentID, stu);
            }

            //_AccessHelper.StudentHelper.FillAttendance(Students.Values);
            #region 抓學生現有的缺曠記錄
            //分批次處理
            foreach (List<SmartSchool.Customization.Data.StudentRecord> studentList in SplitPackage<SmartSchool.Customization.Data.StudentRecord>(GetList<SmartSchool.Customization.Data.StudentRecord>(Students.Values)))
            {
                Dictionary<string, List<SmartSchool.Customization.Data.StudentExtension.AttendanceInfo>> studentAttendanceInfo = new Dictionary<string, List<SmartSchool.Customization.Data.StudentExtension.AttendanceInfo>>();

                #region 取得編號
                string[] idList = new string[studentList.Count];
                for (int i = 0; i < idList.Length; i++)
                {
                    idList[i] = studentList[i].StudentID;
                }
                if (idList.Length == 0)
                    continue;
                #endregion
                #region 取得缺曠資料
                DSXmlHelper helper = new DSXmlHelper("Request");
                helper.AddElement("Field");
                helper.AddElement("Field", "All");
                helper.AddElement("Condition");
                foreach (string id in idList)
                {
                    helper.AddElement("Condition", "RefStudentID", id);
                }
                //if ( schoolYear > 0 )
                //    helper.AddElement("Condition", "SchoolYear", schoolYear.ToString());
                //if ( semester > 0 )
                //    helper.AddElement("Condition", "Semester", semester.ToString());
                helper.AddElement("Order");
                helper.AddElement("Order", "OccurDate", "desc");

                foreach (XmlElement var in QueryAttendance.GetAttendance(new DSRequest(helper)).GetContent().GetElements("Attendance"))
                {
                    int schoolyear = 0;
                    int.TryParse(var.SelectSingleNode("SchoolYear").InnerText, out schoolyear);
                    int semester2 = 0;
                    int.TryParse(var.SelectSingleNode("Semester").InnerText, out semester2);
                    DateTime occurdate;
                    DateTime.TryParse(var.SelectSingleNode("OccurDate").InnerText, out occurdate);
                    string studentID = var.SelectSingleNode("RefStudentID").InnerText;

                    #region 新增檢查匯入日期,是否存在於不同學年度學期(2)
                    int j_schoolYear = schoolyear;
                    int j_semester = semester2;
                    DateTime j_date = occurdate;
                    string j_studentID = studentID;

                    if (!testDic.ContainsKey(studentID))
                    {
                        testDic.Add(studentID, new Dictionary<string, List<string>>());
                    }
                    if (!testDic[studentID].ContainsKey(j_date.ToShortDateString()))
                    {
                        testDic[studentID].Add(j_date.ToShortDateString(), new List<string>());
                    }
                    if (!testDic[studentID][j_date.ToShortDateString()].Contains(j_schoolYear + "_" + j_semester))
                    {
                        testDic[studentID][j_date.ToShortDateString()].Add(j_schoolYear + "_" + j_semester);
                    }
                    #endregion

                    foreach (XmlElement element in var.SelectNodes("Detail/Attendance/Period"))
                    {
                        string period = element.InnerText;
                        string periodtype = element.GetAttribute("AttendanceType");
                        string attendance = element.GetAttribute("AbsenceType");

                        //if ( !periodList.Contains(period) || !absenceList.Contains(attendance) )
                        //    continue;

                        Attendance_islo attendanceInfo = new Attendance_islo(schoolyear, semester2, occurdate, period, periodtype, attendance, var);

                        if (!studentAttendanceInfo.ContainsKey(studentID))
                            studentAttendanceInfo.Add(studentID, new List<SmartSchool.Customization.Data.StudentExtension.AttendanceInfo>());
                        studentAttendanceInfo[studentID].Add(attendanceInfo);
                    }
                }
                #endregion
                #region 填入缺曠資料
                foreach (SmartSchool.Customization.Data.StudentRecord student in studentList)
                {
                    student.AttendanceList.Clear();
                    if (studentAttendanceInfo.ContainsKey(student.StudentID))
                        student.AttendanceList.AddRange(studentAttendanceInfo[student.StudentID]);
                }
                #endregion
            }
            #endregion

            #region 1000512 - 新增檢查匯入日期,是否存在於不同學年度學期
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("匯入失敗!!您的匯入資料有以下狀況!!");
            bool sbbool = false;
            foreach (string each1 in testDic.Keys)
            {
                Dictionary<string, List<string>> dic = testDic[each1];
                StudentRecord sr = Students[each1]; //取得學生Record
                foreach (string each2 in dic.Keys)
                {
                    List<string> list = dic[each2];
                    if (list.Count > 1)
                    {
                        sb.Append("學生:「" + sr.StudentName + "」");
                        sb.Append("於不同學期");
                        foreach (string each3 in list)
                        {
                            sb.Append("「" + each3 + "」");
                        }
                        sb.AppendLine("有相同日期「" + each2 + "」資料!!");
                        sbbool = true;
                    }
                }
            }
            if (sbbool)
            {
                System.Windows.Forms.MessageBox.Show(sb.ToString());
                return;
            }
            #endregion


            bool hasInsert = false, hasUpdate = false, hasDelete = false;
            DSXmlHelper InsertHelper = new DSXmlHelper("InsertRequest");
            DSXmlHelper updateHelper = new DSXmlHelper("UpdateRequest");
            DSXmlHelper deleteHelper = new DSXmlHelper("DeleteRequest");

            deleteHelper.AddElement("Attendance");
            foreach (string key in keyList)
            {
                int schoolYear = schoolYearMapping[key];
                int semester = semesterMapping[key];
                DateTime date = dateMapping[key];
                string studentID = studentIDMapping[key];
                if (!importStudents.Contains(studentID))
                    importStudents.Add(studentID);
                StudentRecord studentRec = Students[studentID];
                bool match = false;
                foreach (SmartSchool.Customization.Data.StudentExtension.AttendanceInfo attendInfo in studentRec.AttendanceList)
                {
                    if (attendInfo.SchoolYear == schoolYear && attendInfo.Semester == semester && attendInfo.OccurDate.ToShortDateString() == date.ToShortDateString())
                    {
                        #region 處理修改或刪除
                        XmlElement detail = attendInfo.Detail;
                        string attendInfoId = detail.GetAttribute("ID");
                        Dictionary<string, string> items = new Dictionary<string, string>();
                        foreach (XmlElement attendItem in detail.SelectNodes("Detail/Attendance/Period"))
                        {
                            string period = attendItem.InnerText;
                            string attendance = attendItem.GetAttribute("AbsenceType");
                            if (!items.ContainsKey(period))
                            {
                                items.Add(period, attendance);
                                if (!periodType.ContainsKey(period) && !oldPeriodTypes.ContainsKey(period))
                                {
                                    oldPeriodTypes.Add(period, attendItem.GetAttribute("AttendanceType"));
                                }
                            }
                        }
                        foreach (RowData row in rowsMapping[key])
                        {
                            //消除記錄
                            if (row["缺曠假別"] == "")
                            {
                                if (items.ContainsKey(row["缺曠節次"]))
                                    items.Remove(row["缺曠節次"]);
                                continue;
                            }
                            if (items.ContainsKey(row["缺曠節次"]))//更新原有的
                            {
                                items[row["缺曠節次"]] = row["缺曠假別"];
                            }
                            else//新加入
                            {
                                items.Add(row["缺曠節次"], row["缺曠假別"]);
                            }
                        }
                        if (items.Count > 0)
                        {
                            DSXmlHelper h2 = new DSXmlHelper("Attendance");
                            foreach (string p in items.Keys)
                            {
                                XmlElement element = h2.AddElement("Period");
                                element.InnerText = p;
                                element.SetAttribute("AbsenceType", items[p]);
                                element.SetAttribute("AttendanceType", periodType.ContainsKey(p) ? periodType[p] : oldPeriodTypes[p]);
                            }
                            updateHelper.AddElement("Attendance");
                            updateHelper.AddElement("Attendance", "Field");
                            updateHelper.AddElement("Attendance/Field", "RefStudentID", studentIDMapping[key]);
                            updateHelper.AddElement("Attendance/Field", "SchoolYear", "" + schoolYearMapping[key]);
                            updateHelper.AddElement("Attendance/Field", "Semester", "" + semesterMapping[key]);
                            updateHelper.AddElement("Attendance/Field", "OccurDate", dateMapping[key].ToShortDateString());
                            updateHelper.AddElement("Attendance/Field", "Detail", h2.GetRawXml(), true);
                            updateHelper.AddElement("Attendance", "Condition");
                            updateHelper.AddElement("Attendance/Condition", "ID", attendInfoId);
                            hasUpdate = true;
                        }
                        else
                        {
                            deleteHelper.AddElement("Attendance", "ID", attendInfoId);
                            hasDelete = true;
                        }
                        #endregion
                        match = true;
                        break;
                    }
                }
                if (!match)
                {
                    #region 新增
                    Dictionary<string, string> items = new Dictionary<string, string>();
                    foreach (RowData row in rowsMapping[key])
                    {
                        //消除記錄
                        if (row["缺曠假別"] == "")
                        {
                            if (items.ContainsKey(row["缺曠節次"]))
                                items.Remove(row["缺曠節次"]);
                            continue;
                        }
                        if (items.ContainsKey(row["缺曠節次"]))//更新原有的
                        {
                            items[row["缺曠節次"]] = row["缺曠假別"];
                        }
                        else//新加入
                        {
                            items.Add(row["缺曠節次"], row["缺曠假別"]);
                        }
                    }
                    DSXmlHelper h2 = new DSXmlHelper("Attendance");
                    foreach (string p in items.Keys)
                    {
                        XmlElement element = h2.AddElement("Period");
                        element.InnerText = p;
                        element.SetAttribute("AbsenceType", items[p]);
                        element.SetAttribute("AttendanceType", periodType.ContainsKey(p) ? periodType[p] : oldPeriodTypes[p]);
                    }
                    InsertHelper.AddElement("Attendance");
                    InsertHelper.AddElement("Attendance", "Field");
                    InsertHelper.AddElement("Attendance/Field", "RefStudentID", studentIDMapping[key]);
                    InsertHelper.AddElement("Attendance/Field", "SchoolYear", "" + schoolYearMapping[key]);
                    InsertHelper.AddElement("Attendance/Field", "Semester", "" + semesterMapping[key]);
                    InsertHelper.AddElement("Attendance/Field", "OccurDate", dateMapping[key].ToShortDateString());
                    InsertHelper.AddElement("Attendance/Field", "Detail", h2.GetRawXml(), true);
                    hasInsert = true;
                    #endregion
                }
            }

            //2014/3/10 新增Log記錄
            StringBuilder Log_sb = new StringBuilder();

            if (hasInsert)
            {
                Log_sb.AppendLine(GetString(InsertHelper, "新增"));
                SmartSchool.Feature.Student.EditAttendance.Insert(new DSRequest(InsertHelper));
            }
            if (hasUpdate)
            {
                Log_sb.AppendLine(GetString(updateHelper, "更新"));
                SmartSchool.Feature.Student.EditAttendance.Update(new DSRequest(updateHelper));
            }
            if (hasDelete)
            {
                Log_sb.AppendLine(GetString(deleteHelper, "刪除"));
                SmartSchool.Feature.Student.EditAttendance.Delete(new DSRequest(deleteHelper));
            }

            if (hasUpdate || hasInsert || hasDelete)
            {
                FISCA.LogAgent.ApplicationLog.Log("匯入缺曠記錄", "新增或更新", Log_sb.ToString());
            }
        }


        string GetString(DSXmlHelper helper, string state)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(state + "「" + helper.GetElements("Attendance/Field").Length + "」筆資料");
            foreach (XmlElement xml in helper.GetElements("Attendance/Field"))
            {
                K12.Data.StudentRecord sr = K12.Data.Student.SelectByID(xml.SelectSingleNode("RefStudentID").InnerText);
                if (sr != null)
                {
                    string Name = sr.Name;
                    string Class_Name = sr.Class != null ? sr.Class.Name : "";
                    string Seat_No = sr.SeatNo.HasValue ? sr.SeatNo.Value.ToString() : "";
                    string OccurDate = xml.SelectSingleNode("OccurDate").InnerText;
                    sb.Append("班級「" + Class_Name + "」");
                    sb.Append("座號「" + Seat_No + "」");
                    sb.Append("姓名「" + Name + "」");
                    sb.AppendLine("日期「" + OccurDate + "」");
                }
            }

            return sb.ToString();
        }

        void ImportAbsence_RowDataValidated(object sender, RowDataValidatedEventArgs e)
        {//"學年度", "學期", "日期", "缺曠假別", "缺曠節次"
            #region 驗各欄位填寫格式
            //if ( !periodList.Contains(period) || !absenceList.Contains(attendance) )
            //    continue;
            int t;
            foreach (string field in e.SelectFields)
            {
                string value = e.Data[field];
                switch (field)
                {
                    default:
                        break;
                    case "學年度":
                        if (value == "" || !int.TryParse(value, out t))
                        {
                            //inputFormatPass &= false;
                            e.ErrorFields.Add(field, "此欄為必填欄位，必須填入整數。");
                        }
                        break;
                    case "學期":
                        if (value == "" || !int.TryParse(value, out t))
                        {
                            //inputFormatPass &= false;
                            e.ErrorFields.Add(field, "此欄為必填欄位，必須填入整數。");
                        }
                        else if (t != 1 && t != 2)
                        {
                            //inputFormatPass &= false;
                            e.ErrorFields.Add(field, "必須填入1或2");
                        }
                        break;

                    // 2016/6/21 穎驊修改，因有學校反映在匯入獎懲資料時，會將日期打成民國年計算(應該要西元)，此舉會造成系統後續錯誤，
                    // 經過討論，決定新增提示訊息，以後只要輸入民國年、空白年，會在錯誤報告Excel中標示提醒，
                    // 下面運作的邏輯是:
                    //1. 假如資料為null、型別錯誤 使得TryParse失敗 會中止
                    //2. 假如TryParse成功，轉換輸出到occurdate，但其來源明顯是民國年(EX:105/6/6)，按它原本的邏輯會把他當成西元105/6/6
                    //所以在此就另外加一個判斷，假如此年份比1911小，就視為使用者輸入的是民國年，後續程序會擋住他，放入錯誤報告Excel中，
                    // 能夠如此大膽設條件，建立在兩個前提之下:1. 我們的正常資料不會有西元1911年前的資料 2.我們的資料也不會有民國1911後的資料
                    // 如果能等到民國1911年 還要再來處理這個Bug，那我也覺得心滿意足了哈哈

                    case "日期":
                        DateTime date = DateTime.Now;
                        if (value == "" || !DateTime.TryParse(value, out date)|| date.Year<1911)
                        {
                            //inputFormatPass &= false;
                            e.ErrorFields.Add(field, "此欄為必填欄位，\n請依照\"西元年/月/日\"格式輸入。");
                        }
                        break;
                    case "缺曠假別":
                        if (value == "")
                        {
                            e.WarningFields.Add(field, "將會消除學生此筆缺曠記錄。");
                        }
                        else if (!absenceList.Contains(value))
                        {
                            //inputFormatPass &= false;
                            e.ErrorFields.Add(field, "輸入值" + value + "不在假別清單中。");
                        }
                        break;
                    case "缺曠節次":
                        if (!periodList.Contains(value))
                        {
                            //inputFormatPass &= false;
                            e.ErrorFields.Add(field, "輸入值" + value + "不在節次清單中。");
                        }
                        break;
                }
            }
            #endregion
        }

        private static List<T>[] SplitPackage<T>(List<T> list)
        {
            if (list.Count > 0)
            {
                int packageCount = (list.Count / 300 + 1);
                int packageSize = list.Count / packageCount + list.Count % packageCount;
                packageCount = 0;
                List<List<T>> packages = new List<List<T>>();
                List<T> packageCurrent = new List<T>();
                foreach (T var in list)
                {
                    packageCurrent.Add(var);
                    packageCount++;
                    if (packageCount == packageSize)
                    {
                        packageCount = 0;
                        packages.Add(packageCurrent);
                    }
                }
                return packages.ToArray();
            }
            else
                return new List<T>[0];
        }

        private static List<T> GetList<T>(IEnumerable<T> items)
        {
            List<T> list = new List<T>();
            list.AddRange(items);
            return list;
        }
    }
}
