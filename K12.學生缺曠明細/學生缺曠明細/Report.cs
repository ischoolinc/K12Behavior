using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using FISCA.DSAUtil;
using K12.Data;
using Aspose.Cells;
using FISCA.Presentation.Controls;

namespace K12.學生缺曠明細
{
    internal class Report : IReport
    {
        #region IReport 成員

        private BackgroundWorker _BGWAbsenceDetail;
        SelectAttendanceForm form;

        /// <summary>
        /// 開始執行此報表
        /// </summary>
        public void Print()
        {
            if (K12.Presentation.NLDPanels.Student.SelectedSource.Count == 0)
                return;

            //警告使用者別做傻事
            if (K12.Presentation.NLDPanels.Student.SelectedSource.Count > 1500)
            {
                MsgBox.Show("您選取的學生超過 1500 個，可能會發生意想不到的錯誤，請減少選取的學生。");
                return;
            }

            form = new SelectAttendanceForm();

            if (form.ShowDialog() == DialogResult.OK)
            {
                FISCA.Presentation.MotherForm.SetStatusBarMessage("正在初始化學生個人缺曠明細...");

                //object[] args = new object[] { form.SchoolYear, form.Semester };

                _BGWAbsenceDetail = new BackgroundWorker();
                _BGWAbsenceDetail.DoWork += new DoWorkEventHandler(_BGWAbsenceDetail_DoWork);
                _BGWAbsenceDetail.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CommonMethods.ExcelReport_RunWorkerCompleted);
                _BGWAbsenceDetail.ProgressChanged += new ProgressChangedEventHandler(CommonMethods.Report_ProgressChanged);
                _BGWAbsenceDetail.WorkerReportsProgress = true;
                _BGWAbsenceDetail.RunWorkerAsync();
            }
        }

        #endregion

        void _BGWAbsenceDetail_DoWork(object sender, DoWorkEventArgs e)
        {
            string reportName = "學生個人缺曠明細";

            #region 快取相關資料

            //選擇的學生
            List<StudentRecord> selectedStudents = Student.SelectByIDs(K12.Presentation.NLDPanels.Student.SelectedSource);
            selectedStudents.Sort(new Comparison<StudentRecord>(CommonMethods.ClassSeatNoComparer));

            //紀錄所有學生ID
            List<string> allStudentID = new List<string>();

            //對照表
            Dictionary<string, string> absenceList = new Dictionary<string, string>();
            Dictionary<string, string> periodList = new Dictionary<string, string>();

            //根據節次類型統計每一種缺曠別的次數
            Dictionary<string, Dictionary<string, Dictionary<string, int>>> periodStatisticsByType = new Dictionary<string, Dictionary<string, Dictionary<string, int>>>();

            //每一位學生的缺曠明細
            Dictionary<string, Dictionary<string, Dictionary<string, string>>> studentAbsenceDetail = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();

            //紀錄每一個節次在報表中的 column index
            Dictionary<string, int> columnTable = new Dictionary<string, int>();

            List<AttendanceRecord> AttendanceList;

            //取得所有學生ID
            foreach (StudentRecord var in selectedStudents)
            {
                allStudentID.Add(var.ID);
            }

            //取得 Absence List
            List<AbsenceMappingInfo> Absencelist = K12.Data.AbsenceMapping.SelectAll();
            foreach (AbsenceMappingInfo var in Absencelist)
            {
                if (!absenceList.ContainsKey(var.Name))
                    absenceList.Add(var.Name, var.Abbreviation);
            }

            //取得 Period List
            List<PeriodMappingInfo> PerioList = K12.Data.PeriodMapping.SelectAll();
            foreach (PeriodMappingInfo var in PerioList)
            {
                if (!periodList.ContainsKey(var.Name))
                    periodList.Add(var.Name, var.Type);
            }

            //產生 DSRequest，取得缺曠明細


            if (form.SelectDayOrSchoolYear)
            {
                #region 依日期
                //取得學生,開始 & 結束之間的缺曠資料
                AttendanceList = Attendance.SelectByDate(Student.SelectByIDs(allStudentID), form.dateTimeInput1.Value, form.dateTimeInput2.Value);
                #endregion
            }
            else
            {
                if (form.checkBoxX1Bool)
                {
                    #region 全部學期列印
                    AttendanceList = Attendance.SelectByStudentIDs(allStudentID);
                    #endregion
                }
                else
                {
                    #region 指定學期列印
                    AttendanceList = Attendance.SelectBySchoolYearAndSemester(Student.SelectByIDs(allStudentID), int.Parse(form.SchoolYear), int.Parse(form.Semester));
                    #endregion
                }
            }

            foreach (AttendanceRecord var in AttendanceList)
            {
                string studentID = var.RefStudentID;
                string schoolYear = var.SchoolYear.ToString();
                string semester = var.Semester.ToString();
                string occurDate = var.OccurDate.ToShortDateString();
                string sso = schoolYear + "_" + semester + "_" + occurDate;

                //累計資料
                if (!periodStatisticsByType.ContainsKey(studentID))
                    periodStatisticsByType.Add(studentID, new Dictionary<string, Dictionary<string, int>>());
                foreach (string value in periodList.Values)
                {
                    if (!periodStatisticsByType[studentID].ContainsKey(value))
                        periodStatisticsByType[studentID].Add(value, new Dictionary<string, int>());
                    foreach (string absence in absenceList.Keys)
                    {
                        if (!periodStatisticsByType[studentID][value].ContainsKey(absence))
                            periodStatisticsByType[studentID][value].Add(absence, 0);
                    }
                }

                //每一位學生缺曠資料
                if (!studentAbsenceDetail.ContainsKey(studentID))
                    studentAbsenceDetail.Add(studentID, new Dictionary<string, Dictionary<string, string>>());
                if (!studentAbsenceDetail[studentID].ContainsKey(sso))
                    studentAbsenceDetail[studentID].Add(sso, new Dictionary<string, string>());

                foreach (AttendancePeriod ap in var.PeriodDetail)
                {
                    string absenceType = ap.AbsenceType;
                    string inner = ap.Period;
                    if (!periodList.ContainsKey(inner))
                        continue;
                    string periodType = periodList[inner];

                    if (!studentAbsenceDetail[studentID][sso].ContainsKey(inner) && absenceList.ContainsKey(absenceType))
                        studentAbsenceDetail[studentID][sso].Add(inner, absenceList[absenceType]);

                    if (periodStatisticsByType[studentID][periodType].ContainsKey(absenceType))
                        periodStatisticsByType[studentID][periodType][absenceType]++;
                }
            }

            #endregion

            if (AttendanceList.Count == 0)
            {
                e.Cancel = true;
                return;
            }

            #region 產生範本

            Workbook template = new Workbook();
            template.Open(new MemoryStream(Properties.Resources.學生缺曠明細), FileFormatType.Excel2003);

            Range tempEachColumn = template.Worksheets[0].Cells.CreateRange(4, 1, true);

            Workbook prototype = new Workbook();
            prototype.Copy(template);
            Worksheet ptws = prototype.Worksheets[0];
            int startPage = 1;
            int pageNumber = 1;

            int colIndex = 4;

            int startPeriodIndex = colIndex;
            int endPeriodIndex;

            //產生每一個節次的欄位
            foreach (string periodName in periodList.Keys)
            {
                ptws.Cells.CreateRange(colIndex, 1, true).Copy(tempEachColumn);
                ptws.Cells[4, colIndex].PutValue(periodName);
                columnTable.Add(periodName, colIndex);
                colIndex++;
            }
            endPeriodIndex = colIndex;

            ptws.Cells.CreateRange(3, startPeriodIndex, 1, endPeriodIndex - startPeriodIndex).Merge();
            ptws.Cells[3, startPeriodIndex].PutValue("節次");

            //合併標題列
            ptws.Cells.CreateRange(0, 0, 1, endPeriodIndex).Merge();
            ptws.Cells.CreateRange(1, 0, 1, endPeriodIndex).Merge();
            ptws.Cells.CreateRange(2, 0, 1, endPeriodIndex).Merge();
            Range ptHeader = ptws.Cells.CreateRange(0, 5, false);
            Range ptEachRow = ptws.Cells.CreateRange(5, 1, false);

            //current++;

            #endregion

            #region 產生報表

            Workbook wb = new Workbook();
            wb.Copy(prototype);
            Worksheet ws = wb.Worksheets[0];

            int index = 0;
            int dataIndex = 0;

            int studentCount = 1;

            foreach (StudentRecord studentInfo in selectedStudents)
            {
                string TitleName1 = School.ChineseName + " 個人缺曠明細";
                string time_2013 = "";
                if (form.SelectDayOrSchoolYear)
                {
                    time_2013 = "統計區間：" + form.StartDay.ToShortDateString() + "~" + form.EndDay.ToShortDateString();
                }
                else
                {
                    if (form.checkBoxX1Bool) //全部學期列印
                    {
                        time_2013 = "統計區間：(所有學年度)";
                    }
                    else
                    {
                        time_2013 = string.Format("統計區間：{0}學年度 第{1}學期", form.SchoolYear, form.Semester);
                    }
                }

                string TitleName2 = "班級：" + ((studentInfo.Class == null ? "　" : studentInfo.Class.Name) + "　座號：" + ((studentInfo.SeatNo == null) ? "　" : studentInfo.SeatNo.ToString()) + "　姓名：" + studentInfo.Name + "　學號：" + studentInfo.StudentNumber);
                string TitleName3 = time_2013;

                //回報進度
                _BGWAbsenceDetail.ReportProgress((int)(((double)studentCount++ * 100.0) / (double)selectedStudents.Count));

                if (!studentAbsenceDetail.ContainsKey(studentInfo.ID))
                    continue;

                //如果不是第一頁，就在上一頁的資料列下邊加黑線
                if (index != 0)
                    ws.Cells.CreateRange(index - 1, 0, 1, endPeriodIndex).SetOutlineBorder(BorderType.BottomBorder, CellBorderType.Medium, Color.Black);

                //複製 Header
                ws.Cells.CreateRange(index, 5, false).Copy(ptHeader);

                //填寫基本資料
                ws.Cells[index, 0].PutValue(School.ChineseName + " 個人缺曠明細");
                ws.Cells[index + 1, 0].PutValue("班級：" + ((studentInfo.Class == null ? "　　　" : studentInfo.Class.Name) + "　　座號：" + ((studentInfo.SeatNo == null) ? "　" : studentInfo.SeatNo.ToString()) + "　　姓名：" + studentInfo.Name + "　　學號：" + studentInfo.StudentNumber));

                dataIndex = index + 5;
                int recordCount = 0;

                //學生Row筆數超過40筆,則添加換行符號,與標頭
                int CountRows = 0;

                Dictionary<string, Dictionary<string, string>> absenceDetail = studentAbsenceDetail[studentInfo.ID];

                //取總頁數 , 資料數除以38列(70/38=2)
                int TotlePage = absenceDetail.Count / 40;
                //目前頁數
                int pageCount = 1;
                //如果還有餘數則+1
                if (absenceDetail.Count % 40 != 0)
                {
                    TotlePage++;
                }

                //填寫基本資料
                ws.Cells[index, 0].PutValue(TitleName1 + "(" + pageCount.ToString() + "/" + TotlePage.ToString() + ")");
                pageCount++;
                ws.Cells[index + 1, 0].PutValue(TitleName2);
                ws.Cells[index + 2, 0].PutValue(TitleName3);

                foreach (string sso in absenceDetail.Keys)
                {
                    CountRows++;

                    string[] ssoSplit = sso.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);

                    //複製每一個 row
                    ws.Cells.CreateRange(dataIndex, 1, false).Copy(ptEachRow);

                    //填寫學生缺曠資料
                    ws.Cells[dataIndex, 0].PutValue(ssoSplit[0]);
                    ws.Cells[dataIndex, 1].PutValue(ssoSplit[1]);
                    ws.Cells[dataIndex, 2].PutValue(ssoSplit[2]);
                    ws.Cells[dataIndex, 3].PutValue(CommonMethods.GetChineseDayOfWeek(DateTime.Parse(ssoSplit[2])));

                    Dictionary<string, string> record = absenceDetail[sso];
                    foreach (string periodName in record.Keys)
                    {
                        ws.Cells[dataIndex, columnTable[periodName]].PutValue(record[periodName]);
                    }

                    dataIndex++;
                    recordCount++;

                    if (CountRows == 40 && pageCount <= TotlePage)
                    {
                        CountRows = 0;
                        //分頁
                        ws.HPageBreaks.Add(dataIndex, endPeriodIndex);
                        //複製 Header
                        ws.Cells.CreateRange(dataIndex, 5, false).Copy(ptHeader);
                        //填寫基本資料
                        ws.Cells[dataIndex, 0].PutValue(TitleName1 + "(" + pageCount.ToString() + "/" + TotlePage.ToString() + ")");
                        pageCount++; //下一頁使用
                        ws.Cells[dataIndex + 1, 0].PutValue(TitleName2);
                        ws.Cells[dataIndex + 2, 0].PutValue(TitleName3);
                        dataIndex += 5;
                    }
                }

                //缺曠統計資訊
                Range absenceStatisticsRange = ws.Cells.CreateRange(dataIndex, 0, 1, endPeriodIndex);
                absenceStatisticsRange.Merge();
                absenceStatisticsRange.SetOutlineBorder(BorderType.TopBorder, CellBorderType.Double, Color.Black);
                absenceStatisticsRange.SetOutlineBorder(BorderType.BottomBorder, CellBorderType.Double, Color.Black);
                absenceStatisticsRange.SetOutlineBorder(BorderType.LeftBorder, CellBorderType.Thin, Color.Black);
                absenceStatisticsRange.SetOutlineBorder(BorderType.RightBorder, CellBorderType.Thin, Color.Black);
                absenceStatisticsRange.RowHeight = 20.0;
                ws.Cells[dataIndex, 0].Style.HorizontalAlignment = TextAlignmentType.Center;
                ws.Cells[dataIndex, 0].Style.VerticalAlignment = TextAlignmentType.Center;
                ws.Cells[dataIndex, 0].Style.Font.Size = 10;
                ws.Cells[dataIndex, 0].PutValue("缺曠總計");
                dataIndex++;

                int typeNumber = dataIndex;
                foreach (string periodType in periodStatisticsByType[studentInfo.ID].Keys)
                {
                    Dictionary<string, int> byType = periodStatisticsByType[studentInfo.ID][periodType];
                    int printable = 0;
                    foreach (string type in byType.Keys)
                    {
                        printable += byType[type];
                    }
                    if (printable == 0)
                        continue;

                    ws.Cells.CreateRange(dataIndex, 0, 1, 2).Merge();
                    ws.Cells.CreateRange(dataIndex, 2, 1, endPeriodIndex - 2).Merge();
                    ws.Cells[dataIndex, 0].Style.HorizontalAlignment = TextAlignmentType.Center;
                    ws.Cells[dataIndex, 0].Style.VerticalAlignment = TextAlignmentType.Center;
                    ws.Cells.CreateRange(dataIndex, 0, 1, endPeriodIndex).RowHeight = 27.0;
                    ws.Cells.CreateRange(dataIndex, 0, 1, 2).SetOutlineBorder(BorderType.RightBorder, CellBorderType.Thin, Color.Black);
                    ws.Cells.CreateRange(dataIndex, 0, 1, 2).SetOutlineBorder(BorderType.LeftBorder, CellBorderType.Thin, Color.Black);
                    ws.Cells.CreateRange(dataIndex, 0, 1, endPeriodIndex).SetOutlineBorder(BorderType.BottomBorder, CellBorderType.Thin, Color.Black);

                    ws.Cells[dataIndex, 0].Style.Font.Size = 10;
                    ws.Cells[dataIndex, 0].PutValue(periodType);

                    StringBuilder text = new StringBuilder("");

                    foreach (string type in byType.Keys)
                    {
                        if (byType[type] > 0)
                        {
                            if (text.ToString() != "")
                                text.Append("　");
                            text.Append(type + "：" + byType[type]);
                        }
                    }

                    ws.Cells[dataIndex, 2].Style.Font.Size = 10;
                    ws.Cells[dataIndex, 2].Style.ShrinkToFit = true;
                    ws.Cells[dataIndex, 2].PutValue(text.ToString());
                    ws.Cells.CreateRange(dataIndex, 0, 1, endPeriodIndex).SetOutlineBorder(BorderType.RightBorder, CellBorderType.Thin, Color.Black);

                    dataIndex++;
                }
                typeNumber = dataIndex - typeNumber;

                //資料列上邊加上黑線
                //ws.Cells.CreateRange(index + 3, 0, 1, endPeriodIndex).SetOutlineBorder(BorderType.BottomBorder, CellBorderType.Medium, Color.Black);

                //表格最右邊加上黑線
                //ws.Cells.CreateRange(index + 2, endPeriodIndex - 1, recordCount + typeNumber + 3, 1).SetOutlineBorder(BorderType.RightBorder, CellBorderType.Medium, Color.Black);

                index = dataIndex;

                //每500頁,增加一個WorkSheet,並於下標顯示(1~500)(501~xxx)
                if (pageNumber < 500)
                {
                    ws.HPageBreaks.Add(index, endPeriodIndex);
                    pageNumber++;
                }
                else
                {
                    ws.Name = startPage + " ~ " + (pageNumber + startPage - 1);
                    ws = wb.Worksheets[wb.Worksheets.Add()];
                    ws.Copy(prototype.Worksheets[0]);
                    startPage += pageNumber;
                    pageNumber = 1;
                    index = 0;
                }
            }


            if (dataIndex > 0)
            {
                //最後一頁的資料列下邊加上黑線
                ws.Cells.CreateRange(dataIndex - 1, 0, 1, endPeriodIndex).SetOutlineBorder(BorderType.BottomBorder, CellBorderType.Medium, Color.Black);
                ws.Name = startPage + " ~ " + (pageNumber + startPage - 2);
            }
            else
                wb = new Workbook();

            #endregion

            string path = Path.Combine(Application.StartupPath, "Reports");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path = Path.Combine(path, reportName + ".xlt");
            e.Result = new object[] { reportName, path, wb };
        }
    }
}
