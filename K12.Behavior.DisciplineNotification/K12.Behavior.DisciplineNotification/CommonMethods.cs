using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using K12.Data;
using FISCA.Presentation.Controls;
using Campus.ePaperCloud;

namespace K12.Behavior.DisciplineNotification
{
    internal static class CommonMethods
    {
        //Word報表
        public static void WordReport_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
                return;

            if (e.Error == null)
            {
                object[] result = (object[])e.Result;

                string reportName = (string)result[0];
                string path = (string)result[1];
                Aspose.Words.Document doc = (Aspose.Words.Document)result[2];
                string path2 = (string)result[3];
                bool PrintStudetnList = (bool)result[4];
                Aspose.Cells.Workbook wb = (Aspose.Cells.Workbook)result[5];
                string Message = "" + result[6];

                if (File.Exists(path2))
                {
                    int i = 1;
                    while (true)
                    {
                        string newPath = Path.GetDirectoryName(path2) + "\\" + Path.GetFileNameWithoutExtension(path2) + (i++) + Path.GetExtension(path2);
                        if (!File.Exists(newPath))
                        {
                            path2 = newPath;
                            break;
                        }
                    }
                }

                if (PrintStudetnList)
                {
                    MemoryStream memoryStream = new MemoryStream();
                    doc.Save(memoryStream, Aspose.Words.SaveFormat.Doc);
                    ePaperCloud ePaperCloud = new ePaperCloud();
                    ePaperCloud.upload_ePaper(Convert.ToInt32(School.DefaultSchoolYear), Convert.ToInt32(School.DefaultSemester)
                        , reportName, "", memoryStream, ePaperCloud.ViewerType.Student, ePaperCloud.FormatType.Docx, Message);
                    
                    wb.Save(path2);
                    FISCA.Presentation.MotherForm.SetStatusBarMessage(reportName + "產生完成");
                    System.Diagnostics.Process.Start(path2);
                }
                else
                {
                    int schoolYear = Convert.ToInt32(School.DefaultSchoolYear);
                    int semester = Convert.ToInt32(School.DefaultSemester);
                    MemoryStream memoryStream = new MemoryStream();
                    doc.Save(memoryStream, Aspose.Words.SaveFormat.Doc);
                    ePaperCloud ePaperCloud = new ePaperCloud();
                    ePaperCloud.upload_ePaper(schoolYear, semester, reportName, "", memoryStream, ePaperCloud.ViewerType.Student, ePaperCloud.FormatType.Docx, Message);

                    FISCA.Presentation.MotherForm.SetStatusBarMessage(reportName + "產生完成");
                }
            }
            else
            {
                MsgBox.Show("列印失敗:\n" + e.Error.Message);
                return;
            }
        }

        //回報進度
        public static void Report_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            FISCA.Presentation.MotherForm.SetStatusBarMessage("" + e.UserState + "產生中...", e.ProgressPercentage);
        }

        internal static string GetChineseDayOfWeek(DateTime date)
        {
            string dayOfWeek = "";

            switch (date.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    dayOfWeek = "一";
                    break;
                case DayOfWeek.Tuesday:
                    dayOfWeek = "二";
                    break;
                case DayOfWeek.Wednesday:
                    dayOfWeek = "三";
                    break;
                case DayOfWeek.Thursday:
                    dayOfWeek = "四";
                    break;
                case DayOfWeek.Friday:
                    dayOfWeek = "五";
                    break;
                case DayOfWeek.Saturday:
                    dayOfWeek = "六";
                    break;
                case DayOfWeek.Sunday:
                    dayOfWeek = "日";
                    break;
            }

            return dayOfWeek;
        }
    }
}
