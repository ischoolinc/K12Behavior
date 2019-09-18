using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Aspose.Words;
using FISCA.DSAUtil;
using K12.Data;
using K12.Data.Configuration;

namespace K12.缺曠通知單
{
    internal class Report : IReport
    {
        private BackgroundWorker _BGWAbsenceNotification;

        private List<StudentRecord> SelectedStudents { get; set; }

        private Dictionary<string, List<string>> config;

        ConfigOBJ obj;

        string entityName;

        //轉縮寫
        Dictionary<string, string> absenceList = new Dictionary<string, string>();

        public Report(string _entityName)
        {
            entityName = _entityName;
        }

        public void Print()
        {
            #region IReport 成員

            AbsenceNotificationSelectDateRangeForm form = new AbsenceNotificationSelectDateRangeForm();

            if (form.ShowDialog() == DialogResult.OK)
            {

                #region 讀取缺曠別 Preference
                config = new Dictionary<string, List<string>>();

                //XmlElement preferenceData = CurrentUser.Instance.Preference["缺曠通知單_缺曠別設定"];
                ConfigData cd = K12.Data.School.Configuration["缺曠通知單_ForK12_缺曠別設定"];
                XmlElement preferenceData = cd.GetXml("XmlData", null);

                if (preferenceData != null)
                {
                    foreach (XmlElement type in preferenceData.SelectNodes("Type"))
                    {
                        string prefix = type.GetAttribute("Text");
                        if (!config.ContainsKey(prefix))
                            config.Add(prefix, new List<string>());

                        foreach (XmlElement absence in type.SelectNodes("Absence"))
                        {
                            if (!config[prefix].Contains(absence.GetAttribute("Text")))
                                config[prefix].Add(absence.GetAttribute("Text"));
                        }
                    }
                }
                #endregion

                FISCA.Presentation.MotherForm.SetStatusBarMessage("正在初始化缺曠通知單...");

                #region 建立設定檔

                obj = new ConfigOBJ();
                obj.StartDate = form.StartDate;
                obj.EndDate = form.EndDate;
                obj.PrintHasRecordOnly = form.PrintHasRecordOnly;
                obj.Template = form.Template;
                //obj.userDefinedConfig = config;
                obj.ReceiveName = form.ReceiveName;
                obj.ReceiveAddress = form.ReceiveAddress;
                obj.ConditionName = form.ConditionName;
                obj.ConditionNumber = form.ConditionNumber;
                obj.ConditionName2 = form.ConditionName2;
                obj.ConditionNumber2 = form.ConditionNumber2;
                obj.PrintStudentList = form.PrintStudentList;

                #endregion

                _BGWAbsenceNotification = new BackgroundWorker();
                _BGWAbsenceNotification.DoWork += new DoWorkEventHandler(_BGWAbsenceNotification_DoWork);
                _BGWAbsenceNotification.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CommonMethods.WordReport_RunWorkerCompleted);
                _BGWAbsenceNotification.ProgressChanged += new ProgressChangedEventHandler(CommonMethods.Report_ProgressChanged);
                _BGWAbsenceNotification.WorkerReportsProgress = true;
                _BGWAbsenceNotification.RunWorkerAsync();
            }

            #endregion
        }

        private void _BGWAbsenceNotification_DoWork(object sender, DoWorkEventArgs e)
        {
            #region 取得學生

            if (entityName.ToLower() == "student") //學生模式
            {
                SelectedStudents = K12.Data.Student.SelectByIDs(K12.Presentation.NLDPanels.Student.SelectedSource);
            }
            else if (entityName.ToLower() == "class") //班級模式
            {
                SelectedStudents = new List<StudentRecord>();
                foreach (StudentRecord each in Student.SelectByClassIDs(K12.Presentation.NLDPanels.Class.SelectedSource))
                {
                    if (each.Status != StudentRecord.StudentStatus.一般)
                        continue;

                    SelectedStudents.Add(each);
                }
            }
            else
                throw new NotImplementedException();

            SelectedStudents.Sort(new Comparison<StudentRecord>(CommonMethods.ClassSeatNoComparer));

            #endregion
            string reportName = "缺曠通知單" + obj.StartDate.ToString("yyyy_MM_dd") + "至" + obj.EndDate.ToString("yyyy_MM_dd");

            #region 快取資料

            //超級資訊物件
            Dictionary<string, StudentOBJ> StudentSuperOBJ = new Dictionary<string, StudentOBJ>();

            //所有學生ID
            List<string> allStudentID = new List<string>();

            //學生人數
            int currentStudentCount = 1;
            int totalStudentNumber = 0;

            //Period List
            List<string> periodList = new List<string>();

            //????使用者所選取的所有假別種類????
            List<string> userDefinedAbsenceList = new List<string>();
            foreach (string kind in config.Keys)
            {
                foreach (string type in config[kind])
                {
                    if (!userDefinedAbsenceList.Contains(type))
                        userDefinedAbsenceList.Add(type);
                }
            }

            #region 取得所有學生ID
            foreach (StudentRecord aStudent in SelectedStudents)
            {
                //建立學生資訊，班級、座號、學號、姓名、導師
                string studentID = aStudent.ID;
                if (!StudentSuperOBJ.ContainsKey(studentID))
                    StudentSuperOBJ.Add(studentID, new StudentOBJ());

                //學生ID清單
                if (!allStudentID.Contains(studentID))
                    allStudentID.Add(studentID);

                StudentSuperOBJ[studentID].student = aStudent;
                StudentSuperOBJ[studentID].TeacherName = aStudent.Class != null ? (aStudent.Class.Teacher != null ? aStudent.Class.Teacher.Name : "") : "";
                StudentSuperOBJ[studentID].ClassName = aStudent.Class != null ? aStudent.Class.Name : "";
                StudentSuperOBJ[studentID].SeatNo = aStudent.SeatNo.HasValue ? aStudent.SeatNo.Value.ToString() : "";
                StudentSuperOBJ[studentID].StudentNumber = aStudent.StudentNumber;
            }
            #endregion

            #region 取得 Period List
            Dictionary<string, string> TestPeriodList = new Dictionary<string, string>();

            foreach (K12.Data.PeriodMappingInfo each in K12.Data.PeriodMapping.SelectAll())
            {
                if (!periodList.Contains(each.Name))
                    periodList.Add(each.Name);

                if (!TestPeriodList.ContainsKey(each.Name)) //節次<-->類別
                    TestPeriodList.Add(each.Name, each.Type);
            }
            #endregion

            #region 取得 Absence List
            Dictionary<string, string> TestAbsenceList = new Dictionary<string, string>(); //代碼替換(新)
            foreach (K12.Data.AbsenceMappingInfo each in K12.Data.AbsenceMapping.SelectAll())
            {
                if (!absenceList.ContainsKey(each.Name))
                {
                    absenceList.Add(each.Name, each.Abbreviation);
                }

                if (!TestAbsenceList.ContainsKey(each.Name)) //縮寫<-->假別
                {
                    TestAbsenceList.Add(each.Abbreviation, each.Name);
                }
            }
            #endregion

            #region 取得所有學生缺曠紀錄，日期區間

            foreach (AttendanceRecord attendance in K12.Data.Attendance.SelectByDate(obj.StartDate, obj.EndDate))
            {
                if (!allStudentID.Contains(attendance.RefStudentID)) //如果是選取班級的學生
                    continue;

                string studentID = attendance.RefStudentID;
                DateTime occurDate = attendance.OccurDate;
                StudentOBJ studentOBJ = StudentSuperOBJ[studentID]; //取得這個物件

                foreach (AttendancePeriod attendancePeriod in attendance.PeriodDetail)
                {
                    string absenceType = attendancePeriod.AbsenceType; //假別
                    string periodName = attendancePeriod.Period; //節次

                    //是否為設定檔節次清單之中
                    if (!TestPeriodList.ContainsKey(periodName))
                        continue;

                    //是否為使用者選取之假別&類型
                    if (config.ContainsKey(TestPeriodList[periodName]))
                    {
                        if (config[TestPeriodList[periodName]].Contains(absenceType))
                        {
                            string PeriodAndAbsence = TestPeriodList[periodName] + "," + absenceType;
                            //區間統計
                            if (!studentOBJ.studentAbsence.ContainsKey(PeriodAndAbsence))
                            {
                                studentOBJ.studentAbsence.Add(PeriodAndAbsence, 0);
                            }

                            studentOBJ.studentAbsence[PeriodAndAbsence]++;

                            //明細記錄
                            if (!studentOBJ.studentAbsenceDetail.ContainsKey(occurDate.ToShortDateString()))
                            {
                                studentOBJ.studentAbsenceDetail.Add(occurDate.ToShortDateString(), new Dictionary<string, string>());
                            }

                            studentOBJ.studentAbsenceDetail[occurDate.ToShortDateString()].Add(attendancePeriod.Period, attendancePeriod.AbsenceType);
                        }
                    }
                }
            }

            #endregion

            List<string> DelStudent = new List<string>(); //列印的學生
            #region 條件1
            if (obj.ConditionName != "") //如果不等於空就是要判斷啦
            {
                foreach (string each1 in StudentSuperOBJ.Keys) //取出一個學生
                {
                    int AbsenceCount = 0;
                    bool AbsenceBOOL = false;
                    foreach (string each2 in StudentSuperOBJ[each1].studentAbsenceDetail.Keys) //取出一天
                    {
                        foreach (string each3 in StudentSuperOBJ[each1].studentAbsenceDetail[each2].Keys) //取出一節內容
                        {
                            string each4 = StudentSuperOBJ[each1].studentAbsenceDetail[each2][each3];

                            if (TestPeriodList.ContainsKey(each3))
                            {
                                if (config.ContainsKey(TestPeriodList[each3]))
                                {
                                    if (obj.ConditionName == each4)
                                    {
                                        AbsenceCount++;
                                    }

                                    if (AbsenceCount >= int.Parse(obj.ConditionNumber))
                                    {
                                        AbsenceBOOL = true;
                                        if (!DelStudent.Contains(each1))
                                        {
                                            DelStudent.Add(each1); //把學生ID記下
                                        }
                                    }

                                    if (AbsenceBOOL)
                                        break;
                                }
                            }
                        }
                        if (AbsenceBOOL)
                            break;
                    }
                }
            }
            #endregion

            #region 條件2
            if (obj.ConditionName2 != "") //如果等於空就是直接全部印啦!!
            {
                foreach (string each1 in StudentSuperOBJ.Keys) //取出一個學生
                {
                    int AbsenceCount = 0;
                    bool AbsenceBOOL = false;
                    foreach (string each2 in StudentSuperOBJ[each1].studentAbsenceDetail.Keys) //取出一天
                    {
                        foreach (string each3 in StudentSuperOBJ[each1].studentAbsenceDetail[each2].Keys) //取出一節內容
                        {
                            string each4 = StudentSuperOBJ[each1].studentAbsenceDetail[each2][each3];

                            if (TestPeriodList.ContainsKey(each3))
                            {
                                if (config.ContainsKey(TestPeriodList[each3]))
                                {
                                    if (obj.ConditionName2 == each4)
                                    {
                                        AbsenceCount++;
                                    }

                                    if (AbsenceCount >= int.Parse(obj.ConditionNumber2))
                                    {
                                        AbsenceBOOL = true;

                                        DelStudent.Add(each1); //把學生ID記下
                                    }

                                    if (AbsenceBOOL)
                                        break;
                                }
                                if (AbsenceBOOL)
                                    break;
                            }
                        }
                    }
                }
            }
            #endregion

            #region 無條件則全部列印
            if (obj.ConditionName == "" && obj.ConditionName2 == "")
            {
                foreach (string each1 in StudentSuperOBJ.Keys) //取出一個學生
                {
                    if (!DelStudent.Contains(each1))
                    {
                        DelStudent.Add(each1);
                    }
                }
            }
            #endregion

            #region 取得所有學生缺曠紀錄，學期累計
            foreach (AttendanceRecord attendance in K12.Data.Attendance.SelectBySchoolYearAndSemester(Student.SelectByIDs(allStudentID), int.Parse(School.DefaultSchoolYear), int.Parse(School.DefaultSemester)))
            {
                //1(大於),0(等於)-1(小於)
                if (obj.EndDate.CompareTo(attendance.OccurDate) == -1)
                    continue;

                string studentID = attendance.RefStudentID;
                DateTime occurDate = attendance.OccurDate;
                StudentOBJ studentOBJ = StudentSuperOBJ[studentID]; //取得這個物件

                foreach (AttendancePeriod attendancePeriod in attendance.PeriodDetail)
                {
                    string absenceType = attendancePeriod.AbsenceType; //假別
                    string periodName = attendancePeriod.Period; //節次
                    if (!TestPeriodList.ContainsKey(periodName))
                        continue;

                    string PeriodAndAbsence = TestPeriodList[periodName] + "," + absenceType;
                    //區間統計
                    if (!studentOBJ.studentSemesterAbsence.ContainsKey(PeriodAndAbsence))
                    {
                        studentOBJ.studentSemesterAbsence.Add(PeriodAndAbsence, 0);
                    }

                    studentOBJ.studentSemesterAbsence[PeriodAndAbsence]++;
                }
            }

            #endregion

            #region 取得學生通訊地址資料
            foreach (AddressRecord record in Address.SelectByStudentIDs(allStudentID))
            {
                if (obj.ReceiveAddress == "戶籍地址")
                {
                    if (!string.IsNullOrEmpty(record.PermanentAddress))
                        StudentSuperOBJ[record.RefStudentID].address = record.Permanent.County + record.Permanent.Town + record.Permanent.District + record.Permanent.Area + record.Permanent.Detail;

                    if (!string.IsNullOrEmpty(record.PermanentZipCode))
                    {
                        StudentSuperOBJ[record.RefStudentID].ZipCode = record.PermanentZipCode;

                        if (record.PermanentZipCode.Length >= 1)
                            StudentSuperOBJ[record.RefStudentID].ZipCode1 = record.PermanentZipCode.Substring(0, 1);
                        if (record.PermanentZipCode.Length >= 2)
                            StudentSuperOBJ[record.RefStudentID].ZipCode2 = record.PermanentZipCode.Substring(1, 1);
                        if (record.PermanentZipCode.Length >= 3)
                            StudentSuperOBJ[record.RefStudentID].ZipCode3 = record.PermanentZipCode.Substring(2, 1);
                        if (record.PermanentZipCode.Length >= 4)
                            StudentSuperOBJ[record.RefStudentID].ZipCode4 = record.PermanentZipCode.Substring(3, 1);
                        if (record.PermanentZipCode.Length >= 5)
                            StudentSuperOBJ[record.RefStudentID].ZipCode5 = record.PermanentZipCode.Substring(4, 1);
                    }

                }
                else if (obj.ReceiveAddress == "聯絡地址")
                {
                    if (!string.IsNullOrEmpty(record.MailingAddress))
                        StudentSuperOBJ[record.RefStudentID].address = record.Mailing.County + record.Mailing.Town + record.Mailing.District + record.Mailing.Area + record.Mailing.Detail; //再處理

                    if (!string.IsNullOrEmpty(record.MailingZipCode))
                    {
                        StudentSuperOBJ[record.RefStudentID].ZipCode = record.MailingZipCode;

                        if (record.MailingZipCode.Length >= 1)
                            StudentSuperOBJ[record.RefStudentID].ZipCode1 = record.MailingZipCode.Substring(0, 1);
                        if (record.MailingZipCode.Length >= 2)
                            StudentSuperOBJ[record.RefStudentID].ZipCode2 = record.MailingZipCode.Substring(1, 1);
                        if (record.MailingZipCode.Length >= 3)
                            StudentSuperOBJ[record.RefStudentID].ZipCode3 = record.MailingZipCode.Substring(2, 1);
                        if (record.MailingZipCode.Length >= 4)
                            StudentSuperOBJ[record.RefStudentID].ZipCode4 = record.MailingZipCode.Substring(3, 1);
                        if (record.MailingZipCode.Length >= 5)
                            StudentSuperOBJ[record.RefStudentID].ZipCode5 = record.MailingZipCode.Substring(4, 1);
                    }
                }
                else if (obj.ReceiveAddress == "其他地址")
                {
                    if (!string.IsNullOrEmpty(record.Address1Address))
                        StudentSuperOBJ[record.RefStudentID].address = record.Address1.County + record.Address1.Town + record.Address1.District + record.Address1.Area + record.Address1.Detail; //再處理

                    if (!string.IsNullOrEmpty(record.Address1ZipCode))
                    {
                        StudentSuperOBJ[record.RefStudentID].ZipCode = record.Address1ZipCode;

                        if (record.Address1ZipCode.Length >= 1)
                            StudentSuperOBJ[record.RefStudentID].ZipCode1 = record.Address1ZipCode.Substring(0, 1);
                        if (record.Address1ZipCode.Length >= 2)
                            StudentSuperOBJ[record.RefStudentID].ZipCode2 = record.Address1ZipCode.Substring(1, 1);
                        if (record.Address1ZipCode.Length >= 3)
                            StudentSuperOBJ[record.RefStudentID].ZipCode3 = record.Address1ZipCode.Substring(2, 1);
                        if (record.Address1ZipCode.Length >= 4)
                            StudentSuperOBJ[record.RefStudentID].ZipCode4 = record.Address1ZipCode.Substring(3, 1);
                        if (record.Address1ZipCode.Length >= 5)
                            StudentSuperOBJ[record.RefStudentID].ZipCode5 = record.Address1ZipCode.Substring(4, 1);
                    }
                }
            }
            #endregion

            #region 取得學生監護人父母親資料
            foreach (ParentRecord record in Parent.SelectByStudentIDs(allStudentID))
            {
                StudentSuperOBJ[record.RefStudentID].CustodianName = record.CustodianName;
                StudentSuperOBJ[record.RefStudentID].FatherName = record.FatherName;
                StudentSuperOBJ[record.RefStudentID].MotherName = record.MotherName;
            }
            //dsrsp = JHSchool.Compatibility.Feature.QueryStudent.GetMultiParentInfo(allStudentID.ToArray());
            //foreach (XmlElement var in dsrsp.GetContent().GetElements("ParentInfo"))
            //{
            //    string studentID = var.GetAttribute("StudentID");

            //    studentInfo[studentID].Add("CustodianName", var.SelectSingleNode("CustodianName").InnerText);
            //    studentInfo[studentID].Add("FatherName", var.SelectSingleNode("FatherName").InnerText);
            //    studentInfo[studentID].Add("MotherName", var.SelectSingleNode("MotherName").InnerText);
            //}
            #endregion

            #endregion

            Document template = new Document(obj.Template, "", LoadFormat.Doc, "");
            DocumentBuilder builder = new DocumentBuilder(template);

            //缺曠類別部份
            #region 缺曠類別部份
            builder.MoveToMergeField("缺曠類別");
            Table table = template.Sections[0].Body.Tables[0];
            Cell startCell = (Cell)builder.CurrentParagraph.ParentNode;
            Row startRow = (Row)startCell.ParentNode;

            double totalWidth = startCell.CellFormat.Width;
            int startRowIndex = table.IndexOf(startRow);
            int columnNumber = 0;

            foreach (List<string> var in config.Values)
            {
                columnNumber += var.Count;
            }
            double columnWidth = totalWidth / columnNumber;

            for (int i = startRowIndex; i < startRowIndex + 4; i++)
            {
                table.Rows[i].RowFormat.HeightRule = HeightRule.Exactly;
                table.Rows[i].RowFormat.Height = 12;
            }

            foreach (string attendanceType in config.Keys)
            {
                Cell newCell = new Cell(template);
                newCell.CellFormat.Width = config[attendanceType].Count * columnWidth;
                newCell.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;
                newCell.CellFormat.WrapText = true;
                newCell.Paragraphs.Add(new Paragraph(template));
                newCell.Paragraphs[0].ParagraphFormat.Alignment = ParagraphAlignment.Center;
                newCell.Paragraphs[0].ParagraphFormat.LineSpacingRule = LineSpacingRule.Exactly;
                newCell.Paragraphs[0].ParagraphFormat.LineSpacing = 12;
                newCell.Paragraphs[0].Runs.Add(new Run(template, attendanceType));
                newCell.Paragraphs[0].Runs[0].Font.Size = 8;
                table.Rows[startRowIndex].Cells.Add(newCell.Clone(true));
                foreach (string absenceType in config[attendanceType])
                {
                    newCell.CellFormat.Width = columnWidth;
                    newCell.Paragraphs[0].Runs[0].Text = absenceType;
                    table.Rows[startRowIndex + 1].Cells.Add(newCell.Clone(true));
                    newCell.Paragraphs[0].Runs[0].Text = "0";
                    table.Rows[startRowIndex + 2].Cells.Add(newCell.Clone(true));
                    table.Rows[startRowIndex + 3].Cells.Add(newCell.Clone(true));
                }
            }

            for (int i = startRowIndex; i < startRowIndex + 4; i++)
            {
                if (config.Count > 0)
                    table.Rows[i].Cells[1].Remove();
                table.Rows[i].LastCell.CellFormat.Borders.Right.Color = Color.Black;
                table.Rows[i].LastCell.CellFormat.Borders.Right.LineWidth = 2.25;
            }
            #endregion

            #region 產生報表

            Document doc = new Document();
            doc.Sections.Clear();

            foreach (string studentID in StudentSuperOBJ.Keys)
            {
                if (!DelStudent.Contains(studentID)) //如果不包含在內,就離開
                    continue;

                if (obj.PrintHasRecordOnly)
                {
                    //明細等於0
                    if (StudentSuperOBJ[studentID].studentAbsenceDetail.Count == 0)
                    {
                        currentStudentCount++;
                        continue;
                    }
                }

                Document eachSection = new Document();
                eachSection.Sections.Clear();
                eachSection.Sections.Add(eachSection.ImportNode(template.Sections[0], true));

                //合併列印的資料
                Dictionary<string, object> mapping = new Dictionary<string, object>();
                //Dictionary<string, string> eachStudentInfo = studentInfo[studentID];

                //學校資訊
                mapping.Add("學校名稱", School.ChineseName);
                mapping.Add("學校地址", School.Address);
                mapping.Add("學校電話", School.Telephone);

                //學生資料
                mapping.Add("學生姓名", StudentSuperOBJ[studentID].student.Name);
                mapping.Add("班級", StudentSuperOBJ[studentID].ClassName);
                mapping.Add("座號", StudentSuperOBJ[studentID].SeatNo);
                mapping.Add("學號", StudentSuperOBJ[studentID].StudentNumber);
                mapping.Add("導師", StudentSuperOBJ[studentID].TeacherName);
                mapping.Add("資料期間", obj.StartDate.ToShortDateString() + " 至 " + obj.EndDate.ToShortDateString());
                mapping.Add("系統編號", "系統編號{" + studentID + "}");

                //收件人資料
                if (obj.ReceiveName == "監護人姓名")
                    mapping.Add("收件人姓名", StudentSuperOBJ[studentID].CustodianName);
                else if (obj.ReceiveName == "父親姓名")
                    mapping.Add("收件人姓名", StudentSuperOBJ[studentID].FatherName);
                else if (obj.ReceiveName == "母親姓名")
                    mapping.Add("收件人姓名", StudentSuperOBJ[studentID].MotherName);
                else
                    mapping.Add("收件人姓名", StudentSuperOBJ[studentID].student.Name);

                //收件人地址資料
                mapping.Add("收件人地址", StudentSuperOBJ[studentID].address);
                mapping.Add("郵遞區號", StudentSuperOBJ[studentID].ZipCode);
                mapping.Add("0", StudentSuperOBJ[studentID].ZipCode1);
                mapping.Add("1", StudentSuperOBJ[studentID].ZipCode2);
                mapping.Add("2", StudentSuperOBJ[studentID].ZipCode3);
                mapping.Add("4", StudentSuperOBJ[studentID].ZipCode4);
                mapping.Add("5", StudentSuperOBJ[studentID].ZipCode5);

                mapping.Add("學年度", School.DefaultSchoolYear);
                mapping.Add("學期", School.DefaultSemester);

                if (StudentSuperOBJ[studentID].studentAbsenceDetail.Count != 0)
                {
                    object[] objectValues = new object[] { StudentSuperOBJ[studentID].studentAbsenceDetail, periodList };
                    mapping.Add("缺曠明細", objectValues);
                }
                else
                {
                    mapping.Add("缺曠明細", null);
                }

                string[] keys = new string[mapping.Count];
                object[] values = new object[mapping.Count];
                int i = 0;
                foreach (string key in mapping.Keys)
                {
                    keys[i] = key;
                    values[i++] = mapping[key];
                }

                //合併列印
                eachSection.MailMerge.MergeField += new Aspose.Words.Reporting.MergeFieldEventHandler(AbsenceNotification_MailMerge_MergeField);
                eachSection.MailMerge.RemoveEmptyParagraphs = true;
                eachSection.MailMerge.Execute(keys, values);

                //填寫缺曠記錄
                Table eachTable = eachSection.Sections[0].Body.Tables[0];
                int columnIndex = 1;
                foreach (string attendanceType in config.Keys)
                {
                    foreach (string absenceType in config[attendanceType])
                    {
                        string dataValue = "0";
                        string semesterDataValue = "0";
                        string PeriodAndAbsence = attendanceType + "," + absenceType;
                        if (StudentSuperOBJ[studentID].studentAbsence.ContainsKey(PeriodAndAbsence))
                        {
                            dataValue = StudentSuperOBJ[studentID].studentAbsence[PeriodAndAbsence].ToString();
                        }
                        if (StudentSuperOBJ[studentID].studentSemesterAbsence.ContainsKey(PeriodAndAbsence))
                        {
                            semesterDataValue = StudentSuperOBJ[studentID].studentSemesterAbsence[PeriodAndAbsence].ToString();
                        }
                        eachTable.Rows[startRowIndex + 3].Cells[columnIndex].Paragraphs[0].Runs[0].Text = dataValue;
                        eachTable.Rows[startRowIndex + 2].Cells[columnIndex].Paragraphs[0].Runs[0].Text = semesterDataValue;
                        columnIndex++;
                    }
                }

                doc.Sections.Add(doc.ImportNode(eachSection.Sections[0], true));

                //回報進度
                _BGWAbsenceNotification.ReportProgress((int)(((double)currentStudentCount++ * 100.0) / (double)totalStudentNumber));
            }

            #endregion

            #region 產生學生清單

            Aspose.Cells.Workbook wb = new Aspose.Cells.Workbook();
            if (obj.PrintStudentList)
            {
                int CountRow = 0;
                wb.Worksheets[0].Cells[CountRow, 0].PutValue("班級");
                wb.Worksheets[0].Cells[CountRow, 1].PutValue("座號");
                wb.Worksheets[0].Cells[CountRow, 2].PutValue("學號");
                wb.Worksheets[0].Cells[CountRow, 3].PutValue("學生姓名");
                wb.Worksheets[0].Cells[CountRow, 4].PutValue("收件人姓名");
                wb.Worksheets[0].Cells[CountRow, 5].PutValue("地址");
                CountRow++;
                foreach (string each in StudentSuperOBJ.Keys)
                {
                    if (!DelStudent.Contains(each)) //如果不包含在內,就離開
                        continue;

                    if (obj.PrintHasRecordOnly)
                    {
                        //明細等於0
                        if (StudentSuperOBJ[each].studentAbsenceDetail.Count == 0)
                        {
                            currentStudentCount++;
                            continue;
                        }
                    }

                    wb.Worksheets[0].Cells[CountRow, 0].PutValue(StudentSuperOBJ[each].ClassName);
                    wb.Worksheets[0].Cells[CountRow, 1].PutValue(StudentSuperOBJ[each].SeatNo);
                    wb.Worksheets[0].Cells[CountRow, 2].PutValue(StudentSuperOBJ[each].StudentNumber);
                    wb.Worksheets[0].Cells[CountRow, 3].PutValue(StudentSuperOBJ[each].student.Name);
                    //收件人資料
                    if (obj.ReceiveName == "監護人姓名")
                        wb.Worksheets[0].Cells[CountRow, 4].PutValue(StudentSuperOBJ[each].CustodianName);
                    else if (obj.ReceiveName == "父親姓名")
                        wb.Worksheets[0].Cells[CountRow, 4].PutValue(StudentSuperOBJ[each].FatherName);
                    else if (obj.ReceiveName == "母親姓名")
                        wb.Worksheets[0].Cells[CountRow, 4].PutValue(StudentSuperOBJ[each].MotherName);
                    else
                        wb.Worksheets[0].Cells[CountRow, 4].PutValue(StudentSuperOBJ[each].student.Name);

                    wb.Worksheets[0].Cells[CountRow, 5].PutValue(StudentSuperOBJ[each].ZipCode + " " + StudentSuperOBJ[each].address);
                    CountRow++;
                }
                wb.Worksheets[0].AutoFitColumns();
            }
            #endregion

            string path = Path.Combine(Application.StartupPath, "Reports");
            string path2 = Path.Combine(Application.StartupPath, "Reports");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path = Path.Combine(path, reportName + ".doc");
            path2 = Path.Combine(path2, reportName + "(學生清單).xls");
            e.Result = new object[] { reportName, path, doc, path2, obj.PrintStudentList, wb };
        }

        private void AbsenceNotification_MailMerge_MergeField(object sender, Aspose.Words.Reporting.MergeFieldEventArgs e)
        {
            #region 缺曠明細
            if (e.FieldName == "缺曠明細")
            {
                if (e.FieldValue == null)
                    return;

                object[] objectValues = (object[])e.FieldValue;
                Dictionary<string, Dictionary<string, string>> studentAbsenceDetail = (Dictionary<string, Dictionary<string, string>>)objectValues[0];
                List<string> periodList = (List<string>)objectValues[1];

                DocumentBuilder builder = new DocumentBuilder(e.Document);

                #region 缺曠明細部份
                builder.MoveToField(e.Field, false);
                Cell detailStartCell = (Cell)builder.CurrentParagraph.ParentNode;
                Row detailStartRow = (Row)detailStartCell.ParentNode;
                int detailStartRowIndex = e.Document.Sections[0].Body.Tables[0].IndexOf(detailStartRow);

                Table detailTable = builder.StartTable();
                builder.CellFormat.Borders.Left.LineWidth = 0.5;
                builder.CellFormat.Borders.Right.LineWidth = 0.5;

                builder.RowFormat.HeightRule = HeightRule.Auto;
                builder.RowFormat.Height = 12;
                builder.RowFormat.Alignment = RowAlignment.Center;

                int rowNumber = 4; //共4個Row,依缺曠天數進行調整
                if (studentAbsenceDetail.Count > rowNumber * 3)
                {
                    rowNumber = studentAbsenceDetail.Count / 3;
                    if (studentAbsenceDetail.Count % 3 > 0)
                        rowNumber++;
                }

                #region 暫解阿!!
                int TestPeriodListCount = periodList.Count;
                if (periodList.Count < 10)
                {
                    TestPeriodListCount = 10;
                }
                else
                {
                    TestPeriodListCount = periodList.Count;
                }
                #endregion

                builder.InsertCell();

                #region 填入日期 & 節次
                for (int i = 0; i < 3; i++)
                {
                    builder.CellFormat.Borders.Right.Color = Color.Black;
                    builder.CellFormat.Borders.Left.Color = Color.Black;
                    builder.CellFormat.Width = 20;
                    builder.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;
                    builder.CellFormat.Borders.LineStyle = LineStyle.Single;
                    builder.Write("日期");
                    builder.InsertCell();

                    for (int j = 0; j < TestPeriodListCount; j++)
                    {
                        builder.CellFormat.Borders.Right.Color = Color.Black;
                        builder.CellFormat.Borders.Left.Color = Color.Black;
                        builder.CellFormat.Borders.LineStyle = LineStyle.Dot;
                        builder.CellFormat.Width = 9;
                        builder.CellFormat.WrapText = true;
                        builder.CellFormat.LeftPadding = 0.5;
                        if (j < periodList.Count)
                        {
                            builder.Write(periodList[j]); //寫入節次名稱
                        }
                        builder.InsertCell();
                    }
                }
                #endregion

                builder.EndRow();

                #region 建立每日格數
                for (int x = 0; x < rowNumber; x++)
                {
                    builder.CellFormat.Borders.Right.Color = Color.Black;
                    builder.CellFormat.Borders.Left.Color = Color.Black;
                    builder.CellFormat.Borders.Left.LineWidth = 0.5;
                    builder.CellFormat.Borders.Right.LineWidth = 0.5;
                    builder.CellFormat.Borders.Top.LineWidth = 0.5;
                    builder.CellFormat.Borders.Bottom.LineWidth = 0.5;
                    builder.CellFormat.Borders.LineStyle = LineStyle.Dot;
                    builder.RowFormat.HeightRule = HeightRule.Exactly;
                    builder.RowFormat.Height = 12;
                    builder.RowFormat.Alignment = RowAlignment.Center;
                    builder.InsertCell();

                    for (int i = 0; i < 3; i++)
                    {
                        builder.CellFormat.Borders.Left.LineStyle = LineStyle.Single;
                        builder.CellFormat.Width = 20;
                        builder.Write("");
                        builder.InsertCell();

                        builder.CellFormat.Borders.LineStyle = LineStyle.Dot;

                        for (int j = 0; j < TestPeriodListCount; j++)
                        {
                            builder.CellFormat.Width = 9;
                            builder.Write("");
                            builder.InsertCell();
                        }
                    }

                    builder.EndRow();
                }
                #endregion

                builder.EndTable();

                foreach (Cell var in detailTable.Rows[0].Cells)
                {
                    var.Paragraphs[0].ParagraphFormat.LineSpacingRule = LineSpacingRule.Exactly;
                    var.Paragraphs[0].ParagraphFormat.LineSpacing = 9;
                }
                #endregion

                #region 填寫缺曠明細
                int eachDetailRowIndex = 0;
                int eachDetailColIndex = 0;

                foreach (string date in studentAbsenceDetail.Keys)
                {
                    int eachDetailPeriodColIndex = eachDetailColIndex + 1;
                    string[] splitDate = date.Split('/');
                    Paragraph dateParagraph = detailTable.Rows[eachDetailRowIndex + 1].Cells[eachDetailColIndex].Paragraphs[0];
                    dateParagraph.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                    dateParagraph.ParagraphFormat.LineSpacingRule = LineSpacingRule.Exactly;
                    dateParagraph.ParagraphFormat.LineSpacing = 9;

                    dateParagraph.Runs.Clear();
                    dateParagraph.Runs.Add(new Run(e.Document));
                    dateParagraph.Runs[0].Font.Size = 8;
                    dateParagraph.Runs[0].Text = splitDate[1] + "/" + splitDate[2];

                    foreach (string period in periodList)
                    {
                        string dataValue = "";
                        if (studentAbsenceDetail[date].ContainsKey(period))
                        {
                            dataValue = studentAbsenceDetail[date][period];
                            Cell miniCell = detailTable.Rows[eachDetailRowIndex + 1].Cells[eachDetailPeriodColIndex];
                            miniCell.Paragraphs.Clear();
                            miniCell.Paragraphs.Add(dateParagraph.Clone(true));
                            miniCell.Paragraphs[0].Runs[0].Font.Size = 14 - (int)(TestPeriodListCount / 2); //依表格多寡縮小文字
                            if (absenceList.ContainsKey(dataValue))
                            {
                                miniCell.Paragraphs[0].Runs[0].Text = absenceList[dataValue];
                            }
                            else
                            {
                                miniCell.Paragraphs[0].Runs[0].Text = "";
                            }
                        }
                        eachDetailPeriodColIndex++;
                    }
                    eachDetailRowIndex++;
                    if (eachDetailRowIndex >= rowNumber)
                    {
                        eachDetailRowIndex = 0;
                        eachDetailColIndex += (TestPeriodListCount + 1);
                    }
                }
                #endregion

                e.Text = string.Empty;
            }
            #endregion
        }
    }
}
