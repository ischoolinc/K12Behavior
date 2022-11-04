﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Aspose.Cells;
using FISCA.DSAUtil;
using K12.Data;

namespace K12.缺曠獎懲週報表.獎懲週報表
{
    internal class Report : IReport
    {
        #region IReport 成員

        private BackgroundWorker _BGWDisciplineWeekList;

        public void Print()
        {
            if (K12.Presentation.NLDPanels.Class.SelectedSource.Count == 0)
                return;

            WeekDisciplineReportCount weekForm = new WeekDisciplineReportCount();
            if (weekForm.ShowDialog() == DialogResult.OK)
            {
                FISCA.Presentation.MotherForm.SetStatusBarMessage("正在初始化獎懲週報表...");

                object[] args = new object[] { weekForm.StartDate, weekForm.EndDate, weekForm.PaperSize, weekForm.ClassCix, weekForm.Weekcix, weekForm.radioButton1.Checked, weekForm.Remarkcix };

                _BGWDisciplineWeekList = new BackgroundWorker();
                _BGWDisciplineWeekList.DoWork += new DoWorkEventHandler(_BGWDisciplineWeekList_DoWork);
                _BGWDisciplineWeekList.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CommonMethods.ExcelReport_RunWorkerCompleted);
                _BGWDisciplineWeekList.ProgressChanged += new ProgressChangedEventHandler(CommonMethods.Report_ProgressChanged);
                _BGWDisciplineWeekList.WorkerReportsProgress = true;
                _BGWDisciplineWeekList.RunWorkerAsync(args);
            }
        }

        #endregion

        #region 獎懲週報表
        void _BGWDisciplineWeekList_DoWork(object sender, DoWorkEventArgs e)
        {
            string reportName = "獎懲週報表";

            object[] args = e.Argument as object[];

            DateTime startDate = (DateTime)args[0];
            DateTime endDate = (DateTime)args[1];
            int size = (int)args[2];
            bool CheckClass = (bool)args[3];
            bool CheckWeek = (bool)args[4];
            bool IsInsertDate = (bool)args[5];
            string Remark = (string)args[6];

            DateTime firstDate = startDate;

            #region 快取獎懲紀錄資料

            List<ClassRecord> selectedClass = Class.SelectByIDs(K12.Presentation.NLDPanels.Class.SelectedSource);
            selectedClass = SortClassIndex.K12Data_ClassRecord(selectedClass);
            //selectedClass.Sort(new Comparison<ClassRecord>(CommonMethods.ClassComparer));
            
            Dictionary<string, List<StudentRecord>> classStudentList = new Dictionary<string, List<StudentRecord>>();

            List<string> allStudentID = new List<string>();

            //紀錄每一個 Column 的 Index
            Dictionary<string, int> columnTable = new Dictionary<string, int>();

            //紀錄每一個學生的獎懲紀錄
            Dictionary<string, Dictionary<string, Dictionary<string, int>>> studentDisciplineList = new Dictionary<string, Dictionary<string, Dictionary<string, int>>>();

            //紀錄每一個學生本週累計的獎懲紀錄
            Dictionary<string, Dictionary<string, int>> studentWeekDisciplineList = new Dictionary<string, Dictionary<string, int>>();

            //紀錄每一個學生學期累計的獎懲紀錄
            Dictionary<string, Dictionary<string, int>> studentSemesterDisciplineList = new Dictionary<string, Dictionary<string, int>>();

            //獎勵項目
            Dictionary<string, string> meritTable = new Dictionary<string, string>();
            meritTable.Add("大功", "A");
            meritTable.Add("小功", "B");
            meritTable.Add("嘉獎", "C");

            //懲戒項目
            Dictionary<string, string> demeritTable = new Dictionary<string, string>();
            demeritTable.Add("大過", "A");
            demeritTable.Add("小過", "B");
            demeritTable.Add("警告", "C");

            int allStudentNumber = 0;

            //計算學生總數，取得所有學生ID
            foreach (ClassRecord aClass in selectedClass)
            {
                //取得班級學生
                List<StudentRecord> classStudent = new List<StudentRecord>(); //取得一般生
                foreach (StudentRecord aStudent in aClass.Students)
                {
                    if (aStudent.Status == StudentRecord.StudentStatus.一般)
                    {
                        allStudentID.Add(aStudent.ID);
                        classStudent.Add(aStudent);
                    }
                }

                classStudent.Sort(new Comparison<StudentRecord>(CommonMethods.ClassSeatNoComparer));

                foreach (StudentRecord aStudent in classStudent)
                {
                    allStudentID.Add(aStudent.ID);
                }
                if (!classStudentList.ContainsKey(aClass.ID))
                    classStudentList.Add(aClass.ID, classStudent);
                allStudentNumber += classStudent.Count;
            }

            //產生 DSRequest
            DSXmlHelper helper = new DSXmlHelper("Request");
            helper.AddElement("Field");
            helper.AddElement("Field", "All");
            helper.AddElement("Condition");
            foreach (string var in allStudentID)
            {
                helper.AddElement("Condition", "RefStudentID", var);
            }

            if (IsInsertDate)
            {
                helper.AddElement("Condition", "StartDate", startDate.ToShortDateString());

                if (CheckWeek) //new,True就是取得至星期日內
                {
                    helper.AddElement("Condition", "EndDate", endDate.ToShortDateString());
                }
                else //new,false就是取得到星期五的缺曠內容
                {
                    helper.AddElement("Condition", "EndDate", endDate.AddDays(-2).ToShortDateString());
                }
            }
            else
            {
                helper.AddElement("Condition", "StartRegisterDate", startDate.ToShortDateString());

                if (CheckWeek) //new,True就是取得至星期日內
                {
                    helper.AddElement("Condition", "EndRegisterDate", endDate.ToShortDateString());
                }
                else //new,false就是取得到星期五的缺曠內容
                {
                    helper.AddElement("Condition", "EndRegisterDate", endDate.AddDays(-2).ToShortDateString());
                }
            }
            helper.AddElement("Order");
            helper.AddElement("Order", "OccurDate", "desc");
            DSResponse dsrsp = Get.GetDiscipline(new DSRequest(helper));

            foreach (XmlElement var in dsrsp.GetContent().GetElements("Discipline"))
            {
                string studentID = var.SelectSingleNode("RefStudentID").InnerText;
                string occurDate = DateTime.Parse(var.SelectSingleNode("OccurDate").InnerText).ToShortDateString();

                if (!studentWeekDisciplineList.ContainsKey(studentID))
                    studentWeekDisciplineList.Add(studentID, new Dictionary<string, int>());

                if (!studentDisciplineList.ContainsKey(studentID))
                    studentDisciplineList.Add(studentID, new Dictionary<string, Dictionary<string, int>>());
                if (!studentDisciplineList[studentID].ContainsKey(occurDate))
                    studentDisciplineList[studentID].Add(occurDate, new Dictionary<string, int>());

                if (var.SelectSingleNode("MeritFlag").InnerText == "1")
                {
                    XmlElement meritElement = (XmlElement)var.SelectSingleNode("Detail/Discipline/Merit");

                    foreach (string merit in meritTable.Keys)
                    {
                        if (meritElement.GetAttribute(meritTable[merit]) != "0")
                        {
                            if (!studentDisciplineList[studentID][occurDate].ContainsKey(merit))
                                studentDisciplineList[studentID][occurDate].Add(merit, 0);
                            studentDisciplineList[studentID][occurDate][merit] += int.Parse(meritElement.GetAttribute(meritTable[merit]));
                            if (!studentWeekDisciplineList[studentID].ContainsKey(merit))
                                studentWeekDisciplineList[studentID].Add(merit, 0);
                            studentWeekDisciplineList[studentID][merit] += int.Parse(meritElement.GetAttribute(meritTable[merit]));
                        }
                    }
                }
                else
                {
                    XmlElement demeritElement = (XmlElement)var.SelectSingleNode("Detail/Discipline/Demerit");

                    foreach (string demerit in demeritTable.Keys)
                    {
                        if (demeritElement.GetAttribute(demeritTable[demerit]) != "0" && demeritElement.GetAttribute("Cleared") != "是")
                        {
                            if (!studentDisciplineList[studentID][occurDate].ContainsKey(demerit))
                                studentDisciplineList[studentID][occurDate].Add(demerit, 0);
                            studentDisciplineList[studentID][occurDate][demerit] += int.Parse(demeritElement.GetAttribute(demeritTable[demerit]));
                            if (!studentWeekDisciplineList[studentID].ContainsKey(demerit))
                                studentWeekDisciplineList[studentID].Add(demerit, 0);
                            studentWeekDisciplineList[studentID][demerit] += int.Parse(demeritElement.GetAttribute(demeritTable[demerit]));
                        }
                    }
                }
            }

            //列印本學期累計

            //產生 DSRequest
            helper = new DSXmlHelper("Request");
            helper.AddElement("Field");
            helper.AddElement("Field", "All");
            helper.AddElement("Condition");
            foreach (string var in allStudentID)
            {
                helper.AddElement("Condition", "RefStudentID", var);
            }
            helper.AddElement("Condition", "SchoolYear", K12.Data.School.DefaultSchoolYear);
            helper.AddElement("Condition", "Semester", K12.Data.School.DefaultSemester);

            if (CheckWeek) //new,True就是取得至星期日內
            {
                helper.AddElement("Condition", "EndDate", endDate.ToShortDateString());
            }
            else
            {
                helper.AddElement("Condition", "EndDate", endDate.AddDays(-2).ToShortDateString());
            }
            helper.AddElement("Order");
            helper.AddElement("Order", "OccurDate", "desc");
            dsrsp = Get.GetDiscipline(new DSRequest(helper));

            foreach (XmlElement var in dsrsp.GetContent().GetElements("Discipline"))
            {
                string studentID = var.SelectSingleNode("RefStudentID").InnerText;

                if (!studentSemesterDisciplineList.ContainsKey(studentID))
                    studentSemesterDisciplineList.Add(studentID, new Dictionary<string, int>());

                if (var.SelectSingleNode("MeritFlag").InnerText == "1")
                {
                    XmlElement meritElement = (XmlElement)var.SelectSingleNode("Detail/Discipline/Merit");

                    foreach (string merit in meritTable.Keys)
                    {
                        if (meritElement.GetAttribute(meritTable[merit]) != "0")
                        {
                            if (!studentSemesterDisciplineList[studentID].ContainsKey(merit))
                                studentSemesterDisciplineList[studentID].Add(merit, 0);

                            int v;
                            if (int.TryParse(meritElement.GetAttribute(meritTable[merit]), out v))
                                studentSemesterDisciplineList[studentID][merit] += v;
                        }
                    }
                }
                else
                {
                    XmlElement demeritElement = (XmlElement)var.SelectSingleNode("Detail/Discipline/Demerit");

                    foreach (string demerit in demeritTable.Keys)
                    {
                        if (demeritElement.GetAttribute(demeritTable[demerit]) != "0" && demeritElement.GetAttribute("Cleared") != "是")
                        {
                            if (!studentSemesterDisciplineList[studentID].ContainsKey(demerit))
                                studentSemesterDisciplineList[studentID].Add(demerit, 0);

                            int v;
                            if (int.TryParse(demeritElement.GetAttribute(demeritTable[demerit]), out v))
                                studentSemesterDisciplineList[studentID][demerit] += v;
                        }
                    }
                }
            }

            #endregion

            //計算使用者自訂項目
            int allDisciplineNumber = 13; //六個獎懲類型，七個Block
            int current = 1;
            int all = allDisciplineNumber + allStudentNumber;

            #region 產生範本

            Workbook template = new Workbook();

            template.Open(new MemoryStream(Properties.Resources.獎懲週報表), FileFormatType.Excel2003);

            Range tempStudent = template.Worksheets[0].Cells.CreateRange(0, 3, true);
            Range tempEachColumn = template.Worksheets[0].Cells.CreateRange(3, 1, true);

            Workbook prototype = new Workbook();
            prototype.Copy(template);

            prototype.Worksheets[0].Cells.CreateRange(0, 3, true).Copy(tempStudent);

            int titleRow = 2;
            int dayNumber;
            if (CheckWeek)
            {
                dayNumber = 7;
            }
            else
            {
                dayNumber = 5;
            }

            int colIndex = 3;

            int dayStartIndex = colIndex;
            int dayEndIndex;
            int dayColumnNumber;

            //產生獎勵部分的欄位
            foreach (string var in meritTable.Keys)
            {
                prototype.Worksheets[0].Cells.CreateRange(colIndex, 1, true).Copy(tempEachColumn);
                prototype.Worksheets[0].Cells[titleRow + 2, colIndex].PutValue(var);
                columnTable.Add(var, colIndex - 3);
                colIndex++;
                _BGWDisciplineWeekList.ReportProgress((int)(((double)current++ * 100.0) / (double)all));
            }
            prototype.Worksheets[0].Cells.CreateRange(titleRow + 1, colIndex - meritTable.Keys.Count, 1, meritTable.Keys.Count).Merge();
            prototype.Worksheets[0].Cells[titleRow + 1, colIndex - meritTable.Keys.Count].PutValue("獎勵");

            //產生懲戒部分的欄位
            foreach (string var in demeritTable.Keys)
            {
                prototype.Worksheets[0].Cells.CreateRange(colIndex, 1, true).Copy(tempEachColumn);
                prototype.Worksheets[0].Cells[titleRow + 2, colIndex].PutValue(var);
                columnTable.Add(var, colIndex - 3);
                colIndex++;
                _BGWDisciplineWeekList.ReportProgress((int)(((double)current++ * 100.0) / (double)all));
            }
            prototype.Worksheets[0].Cells.CreateRange(titleRow + 1, colIndex - demeritTable.Keys.Count, 1, demeritTable.Keys.Count).Merge();
            prototype.Worksheets[0].Cells[titleRow + 1, colIndex - demeritTable.Keys.Count].PutValue("懲戒");

            dayEndIndex = colIndex;
            dayColumnNumber = dayEndIndex - dayStartIndex;
            if (dayColumnNumber == 0)
                dayColumnNumber = 1;

            prototype.Worksheets[0].Cells.CreateRange(titleRow, dayStartIndex, 1, dayColumnNumber).Merge();

            Range dayRange = prototype.Worksheets[0].Cells.CreateRange(dayStartIndex, dayColumnNumber, true);
            prototype.Worksheets[0].Cells[titleRow, dayStartIndex].PutValue(firstDate.ToShortDateString() + " (" + CommonMethods.GetChineseDayOfWeek(firstDate) + ")");
            columnTable.Add(firstDate.ToShortDateString(), dayStartIndex);

            for (int i = 1; i < dayNumber; i++)
            {
                firstDate = firstDate.AddDays(1);

                dayStartIndex += dayColumnNumber;
                prototype.Worksheets[0].Cells.CreateRange(dayStartIndex, dayColumnNumber, true).Copy(dayRange);
                prototype.Worksheets[0].Cells[titleRow, dayStartIndex].PutValue(firstDate.ToShortDateString() + " (" + CommonMethods.GetChineseDayOfWeek(firstDate) + ")");
                columnTable.Add(firstDate.ToShortDateString(), dayStartIndex);
                _BGWDisciplineWeekList.ReportProgress((int)(((double)current++ * 100.0) / (double)all));
            }

            dayStartIndex += dayColumnNumber;
            prototype.Worksheets[0].Cells.CreateRange(dayStartIndex, dayColumnNumber, true).Copy(dayRange);
            prototype.Worksheets[0].Cells[titleRow, dayStartIndex].PutValue("本週合計");
            columnTable.Add("本週合計", dayStartIndex);
            _BGWDisciplineWeekList.ReportProgress((int)(((double)current++ * 100.0) / (double)all));

            dayStartIndex += dayColumnNumber;
            prototype.Worksheets[0].Cells.CreateRange(dayStartIndex, dayColumnNumber, true).Copy(dayRange);
            
            //2011/3/10 - 調整顯示字樣
            prototype.Worksheets[0].Cells[titleRow, dayStartIndex].PutValue("本學期累計");

            columnTable.Add("本學期累計", dayStartIndex);
            dayStartIndex += dayColumnNumber;
            _BGWDisciplineWeekList.ReportProgress((int)(((double)current++ * 100.0) / (double)all));

            prototype.Worksheets[0].Cells.CreateRange(0, 0, 1, dayStartIndex).Merge();
            prototype.Worksheets[0].Cells.CreateRange(1, 0, 1, dayStartIndex).Merge();

            Range prototypeRow = prototype.Worksheets[0].Cells.CreateRange(5, 1, false);
            Range prototypeHeader = prototype.Worksheets[0].Cells.CreateRange(0, 5, false);

            current++;

            #endregion

            #region 產生報表

            Workbook wb = new Workbook();
            wb.Copy(prototype);
            Worksheet ws = wb.Worksheets[0];

            #region 判斷紙張大小
            if (size == 0)
            {
                ws.PageSetup.PaperSize = PaperSizeType.PaperA3;
                ws.PageSetup.FitToPagesWide = 1;
                ws.PageSetup.FitToPagesTall = 0;
            }
            else if (size == 1)
            {
                ws.PageSetup.PaperSize = PaperSizeType.PaperA4;
                ws.PageSetup.FitToPagesWide = 1;
                ws.PageSetup.FitToPagesTall = 0;
            }
            else if (size == 2)
            {
                ws.PageSetup.PaperSize = PaperSizeType.PaperB4;
                ws.PageSetup.FitToPagesWide = 1;
                ws.PageSetup.FitToPagesTall = 0;
            }
            #endregion

            int index = 0;
            int dataIndex = 0;

            List<string> list = new List<string>();

            if (CheckClass) //如果需要過慮資料
            {
                foreach (ClassRecord CheckClassData in selectedClass)
                {
                    List<StudentRecord> classStudent = classStudentList[CheckClassData.ID];
                    bool jumpNext = false;

                    foreach (StudentRecord CheckStudentData in classStudent)
                    {
                        if (studentWeekDisciplineList.ContainsKey(CheckStudentData.ID)) //如果studentWeekAbsenceList內包含了該學生ID
                        {
                            if (!list.Contains(CheckClassData.ID)) //如果清單中不包含就加入
                            {
                                list.Add(CheckClassData.ID);
                                jumpNext = true;
                            }
                        }

                        if (jumpNext)
                            break;
                    }
                }
            }
            else
            {
                foreach (ClassRecord CheckClassData in selectedClass)
                {
                    list.Add(CheckClassData.ID);
                }
            }

            foreach (ClassRecord classInfo in selectedClass)
            {
                if (list.Contains(classInfo.ID))
                {
                    List<StudentRecord> classStudent = classStudentList[classInfo.ID];

                    //複製 Header
                    ws.Cells.CreateRange(index, 5, false).Copy(prototypeHeader);

                    //填寫基本資料
                    ws.Cells[index, 0].PutValue(K12.Data.School.DefaultSchoolYear + " 學年度 " + School.DefaultSemester + " 學期 " + School.ChineseName + " 獎懲週報表");

                    string tname = classInfo.Teacher == null ? "" : classInfo.Teacher.Name;
                    if (CheckWeek) //new,True就是取得至星期日內
                    {
                        ws.Cells[index + 1, 0].PutValue("班級名稱： " + classInfo.Name + "　　班導師： " + tname + " 老師　　獎懲統計區間： " + startDate.ToShortDateString() + " ~ " + endDate.ToShortDateString());
                    }
                    else
                    {
                        ws.Cells[index + 1, 0].PutValue("班級名稱： " + classInfo.Name + "　　班導師： " + tname + " 老師　　獎懲統計區間： " + startDate.ToShortDateString() + " ~ " + endDate.AddDays(-2).ToShortDateString());
                    }
                    dataIndex = index + 5;

                    int studentCount = 0;
                    while (studentCount < classStudent.Count)
                    {
                        //複製每一個 row
                        ws.Cells.CreateRange(dataIndex, 1, false).Copy(prototypeRow);
                        if (studentCount % 5 == 0 && studentCount != 0)
                        {
                            Range eachFiveRow = wb.Worksheets[0].Cells.CreateRange(dataIndex, 0, 1, dayStartIndex);
                            eachFiveRow.SetOutlineBorder(BorderType.TopBorder, CellBorderType.Double, Color.Black);
                        }

                        //填寫學生缺曠資料
                        StudentRecord student = classStudent[studentCount];
                        ws.Cells[dataIndex, 0].PutValue(student.StudentNumber);
                        ws.Cells[dataIndex, 1].PutValue(student.SeatNo);
                        ws.Cells[dataIndex, 2].PutValue(student.Name);

                        int startCol;
                        if (studentDisciplineList.ContainsKey(student.ID))
                        {
                            foreach (string date in studentDisciplineList[classStudent[studentCount].ID].Keys)
                            {
                                Dictionary<string, int> dateDiscipline = studentDisciplineList[classStudent[studentCount].ID][date];

                                //取得Column的index
                                if (columnTable.ContainsKey(date))
                                {
                                    startCol = columnTable[date];

                                    foreach (string var in dateDiscipline.Keys)
                                    {
                                        if (columnTable.ContainsKey(var))
                                        {
                                            ws.Cells[dataIndex, startCol + columnTable[var]].PutValue(dateDiscipline[var]);
                                        }
                                    }
                                }
                            }
                        }

                        if (studentWeekDisciplineList.ContainsKey(student.ID))
                        {
                            startCol = columnTable["本週合計"];

                            Dictionary<string, int> studentWeek = studentWeekDisciplineList[classStudent[studentCount].ID];

                            foreach (string var in studentWeek.Keys)
                            {
                                if (columnTable.ContainsKey(var))
                                    ws.Cells[dataIndex, startCol + columnTable[var]].PutValue(studentWeek[var]);
                            }
                        }

                        if (studentSemesterDisciplineList.ContainsKey(student.ID))
                        {
                            startCol = columnTable["本學期累計"];

                            Dictionary<string, int> studentSemester = studentSemesterDisciplineList[classStudent[studentCount].ID];

                            foreach (string var in studentSemester.Keys)
                            {
                                if (columnTable.ContainsKey(var))
                                    ws.Cells[dataIndex, startCol + columnTable[var]].PutValue(studentSemester[var]);
                            }
                        }

                        studentCount++;
                        dataIndex++;
                        _BGWDisciplineWeekList.ReportProgress((int)(((double)current++ * 100.0) / (double)all));
                    }

                    index = dataIndex;

                    //增加一說明欄位 - 2020/9/21
                    //1.備註資料 , 2.欄寬
                    if (!string.IsNullOrEmpty(Remark))
                    {
                        ws.Cells.CreateRange(index, 1, 1, dayStartIndex - 1).Merge();

                        Range RemarkRow = ws.Cells.CreateRange(dataIndex, 0, 1, dayStartIndex);
                        RemarkRow.SetOutlineBorder(BorderType.BottomBorder, CellBorderType.Thin, Color.Black);
                        RemarkRow.SetOutlineBorder(BorderType.LeftBorder, CellBorderType.Thin, Color.Black);
                        RemarkRow.SetOutlineBorder(BorderType.RightBorder, CellBorderType.Thin, Color.Black);

                        string[] JR = new string[] { "\r\n" };
                        string[] Hei = Remark.Split(JR, StringSplitOptions.None);
                        if (20 * Hei.Length > 409)
                            RemarkRow.RowHeight = 409;
                        else
                            RemarkRow.RowHeight = 20 * Hei.Length;

                        ws.Cells[dataIndex, 1].PutValue(Remark);

                        Aspose.Cells.Style style = ws.Cells[dataIndex, 1].Style;
                        style.IsTextWrapped = true;
                        ws.Cells[dataIndex, 1].Style = style;

                        index++;
                    }

                    //設定分頁
                    ws.HPageBreaks.Add(index, dayStartIndex);
                }
            }

            #endregion

            string path = Path.Combine(Application.StartupPath, "Reports");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path = Path.Combine(path, reportName + ".xlt");
            e.Result = new object[] { reportName, path, wb };
        }
        #endregion
    }
}
