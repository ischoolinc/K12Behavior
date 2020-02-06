using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Aspose.Cells;
using FISCA.DSAUtil;
using Framework;
using K12.Data;

namespace K12.學生獎勵懲戒明細
{
    internal class Report : IReport
    {
        #region IReport 成員

        private BackgroundWorker _BGWDisciplineDetail;
        SelectMeritDemeritForm form;

        public void Print()
        {

            if (K12.Presentation.NLDPanels.Student.SelectedSource.Count == 0)
                return;

            //警告使用者別做傻事
            if (K12.Presentation.NLDPanels.Student.SelectedSource.Count > 1500)
            {
                FISCA.Presentation.Controls.MsgBox.Show("您選取的學生超過 1500 個，可能會發生意想不到的錯誤，請減少選取的學生。");
                return;
            }

            form = new SelectMeritDemeritForm();

            if (form.ShowDialog() == DialogResult.OK)
            {
                FISCA.Presentation.MotherForm.SetStatusBarMessage("正在初始化學生獎懲記錄明細...");

                //object[] args = new object[] { form.SchoolYear, form.Semester };

                _BGWDisciplineDetail = new BackgroundWorker();
                _BGWDisciplineDetail.DoWork += new DoWorkEventHandler(_BGWDisciplineDetail_DoWork);
                _BGWDisciplineDetail.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CommonMethods.ExcelReport_RunWorkerCompleted);
                _BGWDisciplineDetail.ProgressChanged += new ProgressChangedEventHandler(CommonMethods.Report_ProgressChanged);
                _BGWDisciplineDetail.WorkerReportsProgress = true;
                _BGWDisciplineDetail.RunWorkerAsync();
            }
        }

        #endregion

        void _BGWDisciplineDetail_DoWork(object sender, DoWorkEventArgs e)
        {
            string reportName = "學生獎勵懲戒記錄明細";

            #region 快取相關資料

            //選擇的學生
            List<StudentRecord> selectedStudents = Student.SelectByIDs(K12.Presentation.NLDPanels.Student.SelectedSource);

            selectedStudents.Sort(new Comparison<StudentRecord>(CommonMethods.ClassSeatNoComparer));

            //紀錄所有學生ID
            List<string> allStudentID = new List<string>();

            //每一位學生的獎懲明細(學生id<字串組合,物件>)
            Dictionary<string, Dictionary<string, DisciplineRecord>> studentDisciplineDetail = new Dictionary<string, Dictionary<string, DisciplineRecord>>();

            //每一位學生的獎懲累計資料(學生id,特殊物件)
            Dictionary<string, DisciplineStatistics> studentDisciplineStatistics = new Dictionary<string, DisciplineStatistics>();

            //紀錄每一種獎懲在報表中的 column index
            Dictionary<string, int> columnTable = new Dictionary<string, int>();

            List<DisciplineRecord> DisciplineList = new List<DisciplineRecord>();

            //取得所有學生ID
            foreach (StudentRecord var in selectedStudents)
            {
                allStudentID.Add(var.ID);
            }

            //初始化
            string[] columnString;
            if (form.checkBoxX2Bool) //使用者已勾選"排除懲戒已銷過資料"
                columnString = new string[] { "大功", "小功", "嘉獎", "大過", "小過", "警告", "留察", "登錄日期", "事由", "備註" };
            else
                columnString = new string[] { "大功", "小功", "嘉獎", "大過", "小過", "警告", "留察", "銷過", "銷過日期", "登錄日期", "事由", "備註" };

            int i = 4;
            foreach (string s in columnString)
            {
                columnTable.Add(s, i++);
            }

            #region 取得獎勵懲戒明細
            if (form.SelectDayOrSchoolYear) //依日期
            {
                if (form.SetupTime) //依發生日期
                {
                    DisciplineList = Discipline.SelectByOccurDate(allStudentID, form.dateTimeInput1.Value, form.dateTimeInput2.Value);

                }
                else //依登錄日期
                {
                    DisciplineList = Discipline.SelectByRegisterDate(allStudentID, form.dateTimeInput1.Value, form.dateTimeInput2.Value);

                }
            }
            else //依學期
            {
                if (form.checkBoxX1Bool) //全部學期列印
                {
                    #region 全部學期列印
                    DisciplineList = Discipline.SelectByStudentIDs(allStudentID);
                    #endregion
                }
                else //指定學期列印
                {
                    #region 指定學期列印
                    foreach (DisciplineRecord each in Discipline.SelectByStudentIDs(allStudentID))
                    {
                        if (each.SchoolYear.ToString() == form.cbSchoolYear.Text && each.Semester.ToString() == form.cbSemester.Text)
                        {
                            DisciplineList.Add(each);
                        }

                    }
                    #endregion
                }
            }
            #endregion

            if (form.checkBoxX2Bool) //使用者已勾選"排除懲戒已銷過資料"
            {
                IsOrRemoveData(DisciplineList);
            }

            if (DisciplineList.Count == 0)
            {
                e.Cancel = true;
                return;
            }

            foreach (DisciplineRecord each in DisciplineList)
            {
                string studentID = each.RefStudentID;
                string schoolYear = each.SchoolYear.ToString();
                string semester = each.Semester.ToString();
                string occurDate = each.OccurDate.ToShortDateString();
                string reason = each.Reason;
                string remark = each.Remark;
                string disciplineID = each.ID;
                string sso = schoolYear + "_" + semester + "_" + occurDate + "_" + disciplineID;


                //初始化累計資料
                if (!studentDisciplineStatistics.ContainsKey(studentID))
                {
                    studentDisciplineStatistics.Add(studentID, new DisciplineStatistics(studentID));
                }

                //每一位學生獎勵懲戒資料
                if (!studentDisciplineDetail.ContainsKey(studentID))
                {
                    studentDisciplineDetail.Add(studentID, new Dictionary<string, DisciplineRecord>());
                }

                if (!studentDisciplineDetail[studentID].ContainsKey(sso))
                {
                    studentDisciplineDetail[studentID].Add(sso, each);
                }

                if (each.MeritFlag == "1")
                {
                    studentDisciplineStatistics[studentID].大功 += each.MeritA.HasValue ? each.MeritA.Value : 0;
                    studentDisciplineStatistics[studentID].小功 += each.MeritB.HasValue ? each.MeritB.Value : 0;
                    studentDisciplineStatistics[studentID].嘉獎 += each.MeritC.HasValue ? each.MeritC.Value : 0;

                }
                else if (each.MeritFlag == "0")
                {
                    if (each.Cleared != "是")
                    {
                        studentDisciplineStatistics[studentID].大過 += each.DemeritA.HasValue ? each.DemeritA.Value : 0;
                        studentDisciplineStatistics[studentID].小過 += each.DemeritB.HasValue ? each.DemeritB.Value : 0;
                        studentDisciplineStatistics[studentID].警告 += each.DemeritC.HasValue ? each.DemeritC.Value : 0;
                    }
                }
                else if (each.MeritFlag == "2") //留察
                {

                }
            }

            #endregion

            #region 產生範本

            Workbook template = new Workbook();
            if (form.checkBoxX2Bool)
            {
                template.Open(new MemoryStream(K12.學生獎懲明細.Properties.Resources.學生獎懲明細_2), FileFormatType.Excel2003);
            }
            else
            {
                template.Open(new MemoryStream(K12.學生獎懲明細.Properties.Resources.學生獎懲明細), FileFormatType.Excel2003);
            }
            Workbook prototype = new Workbook();
            prototype.Copy(template);

            Worksheet ptws = prototype.Worksheets[0];

            int startPage = 1;
            int pageNumber = 1;

            int columnNumber = 16;

            if (form.checkBoxX2Bool)
            {
                columnNumber = 14;
            }

            //合併標題列
            ptws.Cells.CreateRange(0, 0, 1, columnNumber).Merge();
            ptws.Cells.CreateRange(1, 0, 1, columnNumber).Merge();

            Range ptHeader = ptws.Cells.CreateRange(0, 4, false);
            Range ptEachRow = ptws.Cells.CreateRange(4, 1, false);

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
                string TitleName1 = School.ChineseName + " 個人獎勵懲戒明細";
                string TitleName2 = "班級：" + ((studentInfo.Class == null ? "　　　" : studentInfo.Class.Name) + "　　座號：" + ((studentInfo.SeatNo == null) ? "　" : studentInfo.SeatNo.ToString()) + "　　姓名：" + studentInfo.Name + "　　學號：" + studentInfo.StudentNumber);

                //回報進度
                _BGWDisciplineDetail.ReportProgress((int)(((double)studentCount++ * 100.0) / (double)selectedStudents.Count));

                if (!studentDisciplineDetail.ContainsKey(studentInfo.ID))
                    continue;

                //如果不是第一頁，就在上一頁的資料列下邊加黑線
                if (index != 0)
                    ws.Cells.CreateRange(index - 1, 0, 1, columnNumber).SetOutlineBorder(BorderType.BottomBorder, CellBorderType.Medium, Color.Black);

                //複製 Header
                ws.Cells.CreateRange(index, 4, false).Copy(ptHeader);

                dataIndex = index + 4;
                int recordCount = 0;

                //學生Row筆數超過40筆,則添加換行符號,與標頭
                int CountRows = 0;

                Dictionary<string, DisciplineRecord> disciplineDetail = studentDisciplineDetail[studentInfo.ID];

                //取總頁數 , 資料數除以38列(70/38=2)
                int TotlePage = disciplineDetail.Count / 40;
                //目前頁數
                int pageCount = 1;
                //如果還有餘數則+1
                if (disciplineDetail.Count % 40 != 0)
                {
                    TotlePage++;
                }

                //填寫基本資料
                ws.Cells[index, 0].PutValue(TitleName1 + "(" + pageCount.ToString() + "/" + TotlePage.ToString() + ")");
                pageCount++;
                ws.Cells[index + 1, 0].PutValue(TitleName2);

                foreach (string sso in disciplineDetail.Keys)
                {
                    CountRows++;

                    string[] ssoSplit = sso.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);

                    //複製每一個 row
                    ws.Cells.CreateRange(dataIndex, 1, false).Copy(ptEachRow);

                    //填寫學生獎懲資料
                    ws.Cells[dataIndex, 0].PutValue(ssoSplit[0]);
                    ws.Cells[dataIndex, 1].PutValue(ssoSplit[1]);
                    ws.Cells[dataIndex, 2].PutValue(ssoSplit[2]);
                    ws.Cells[dataIndex, 3].PutValue(CommonMethods.GetChineseDayOfWeek(DateTime.Parse(ssoSplit[2])));

                    DisciplineRecord record = disciplineDetail[sso];

                    if (record.MeritFlag == "1")
                    {
                        ws.Cells[dataIndex, columnTable["大功"]].PutValue(record.MeritA);
                        ws.Cells[dataIndex, columnTable["小功"]].PutValue(record.MeritB);
                        ws.Cells[dataIndex, columnTable["嘉獎"]].PutValue(record.MeritC);
                    }
                    else if (record.MeritFlag == "0")
                    {
                        if (record.Cleared == "是")
                        {
                            ws.Cells[dataIndex, columnTable["大過"]].PutValue(record.DemeritA);
                            ws.Cells[dataIndex, columnTable["小過"]].PutValue(record.DemeritB);
                            ws.Cells[dataIndex, columnTable["警告"]].PutValue(record.DemeritC);
                            if (!form.checkBoxX2Bool)
                            {
                                ws.Cells[dataIndex, columnTable["銷過"]].PutValue(record.Cleared);
                                ws.Cells[dataIndex, columnTable["銷過日期"]].PutValue(record.ClearDate.HasValue ? record.ClearDate.Value.ToShortDateString() : "");
                            }
                        }
                        else
                        {
                            ws.Cells[dataIndex, columnTable["大過"]].PutValue(record.DemeritA);
                            ws.Cells[dataIndex, columnTable["小過"]].PutValue(record.DemeritB);
                            ws.Cells[dataIndex, columnTable["警告"]].PutValue(record.DemeritC);
                        }
                    }
                    else if (record.MeritFlag == "2")
                    {
                        ws.Cells[dataIndex, columnTable["留察"]].PutValue("是");
                    }

                    ws.Cells[dataIndex, columnTable["事由"]].PutValue(record.Reason);
                    ws.Cells[dataIndex, columnTable["備註"]].PutValue(record.Remark);
                    ws.Cells[dataIndex, columnTable["登錄日期"]].PutValue(record.RegisterDate.HasValue ? record.RegisterDate.Value.ToShortDateString() : "");

                    dataIndex++;
                    recordCount++;


                    if (CountRows == 40 && pageCount <= TotlePage)
                    {
                        CountRows = 0;
                        //分頁
                        ws.HPageBreaks.Add(dataIndex, columnNumber);
                        //複製 Header
                        ws.Cells.CreateRange(dataIndex, 4, false).Copy(ptHeader);
                        //填寫基本資料
                        ws.Cells[dataIndex, 0].PutValue(TitleName1 + "(" + pageCount.ToString() + "/" + TotlePage.ToString() + ")");
                        pageCount++; //下一頁使用
                        ws.Cells[dataIndex + 1, 0].PutValue(TitleName2);

                        dataIndex += 4;
                    }
                }

                //獎懲統計資訊
                Range disciplineStatisticsRange = ws.Cells.CreateRange(dataIndex, 0, 1, columnNumber);
                disciplineStatisticsRange.Copy(ptEachRow);
                disciplineStatisticsRange.Merge();
                disciplineStatisticsRange.SetOutlineBorder(BorderType.TopBorder, CellBorderType.Double, Color.Black);
                disciplineStatisticsRange.SetOutlineBorder(BorderType.BottomBorder, CellBorderType.Double, Color.Black);
                disciplineStatisticsRange.RowHeight = 20.0;
                ws.Cells[dataIndex, 0].Style.HorizontalAlignment = TextAlignmentType.Center;
                ws.Cells[dataIndex, 0].Style.VerticalAlignment = TextAlignmentType.Center;
                ws.Cells[dataIndex, 0].Style.Font.Size = 10;
                ws.Cells[dataIndex, 0].PutValue("獎勵懲戒總計");
                dataIndex++;

                //獎懲統計內容
                ws.Cells.CreateRange(dataIndex, 0, 1, columnNumber).Copy(ptEachRow);
                ws.Cells.CreateRange(dataIndex, 0, 1, columnNumber).RowHeight = 27.0;
                ws.Cells.CreateRange(dataIndex, 0, 1, columnNumber).Merge();
                ws.Cells[dataIndex, 0].Style.HorizontalAlignment = TextAlignmentType.Center;
                ws.Cells[dataIndex, 0].Style.VerticalAlignment = TextAlignmentType.Center;
                ws.Cells[dataIndex, 0].Style.Font.Size = 10;
                ws.Cells[dataIndex, 0].Style.ShrinkToFit = true;

                StringBuilder text = new StringBuilder("");
                DisciplineStatistics disciplineStatistics = studentDisciplineStatistics[studentInfo.ID];

                List<string> list = new List<string>();
                if (disciplineStatistics.大功 > 0)
                {
                    list.Add("大功：" + disciplineStatistics.大功);
                }
                if (disciplineStatistics.小功 > 0)
                {
                    list.Add("小功：" + disciplineStatistics.小功);
                }
                if (disciplineStatistics.嘉獎 > 0)
                {
                    list.Add("嘉獎：" + disciplineStatistics.嘉獎);
                }
                if (disciplineStatistics.大過 > 0)
                {
                    list.Add("大過：" + disciplineStatistics.大過);
                }
                if (disciplineStatistics.小過 > 0)
                {
                    list.Add("小過：" + disciplineStatistics.小過);
                }
                if (disciplineStatistics.警告 > 0)
                {
                    list.Add("警告：" + disciplineStatistics.警告);
                }
                text.Append(string.Join(" ", list.ToArray()));

                ws.Cells[dataIndex, 0].PutValue(text.ToString());

                ws.Cells.CreateRange(dataIndex, 0, 1, columnNumber).SetOutlineBorder(BorderType.RightBorder, CellBorderType.Thin, Color.Black);

                dataIndex++;

                //資料列上邊加上黑線
                //ws.Cells.CreateRange(index + 3, 0, 1, columnNumber).SetOutlineBorder(BorderType.BottomBorder, CellBorderType.Thin, Color.Black);

                //表格最右邊加上黑線
                //ws.Cells.CreateRange(index + 2, columnNumber - 1, recordCount + 4, 1).SetOutlineBorder(BorderType.RightBorder, CellBorderType.Thin, Color.Black);

                index = dataIndex;

                //每500頁,增加一個WorkSheet,並於下標顯示(1~500)(501~xxx)
                if (pageNumber < 500)
                {
                    ws.HPageBreaks.Add(index, columnNumber);
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
                ws.Cells.CreateRange(dataIndex - 1, 0, 1, columnNumber).SetOutlineBorder(BorderType.BottomBorder, CellBorderType.Medium, Color.Black);
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


        private List<DisciplineRecord> IsOrRemoveData(List<DisciplineRecord> disList)
        {
            List<DisciplineRecord> _disList = disList;
            List<DisciplineRecord> Remove = new List<DisciplineRecord>();
            foreach (DisciplineRecord each in _disList)
            {
                if (each.MeritFlag != "0") //
                    continue;

                if (each.Cleared == "是") //銷過記錄
                {
                    Remove.Add(each);
                }
            }

            foreach (DisciplineRecord each in Remove)
            {
                if (_disList.Contains(each))
                    _disList.Remove(each);
            }

            return _disList;
        }
    }
}
