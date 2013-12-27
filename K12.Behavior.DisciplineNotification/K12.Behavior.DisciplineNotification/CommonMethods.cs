using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using K12.Data;
using FISCA.Presentation.Controls;

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

                if (File.Exists(path))
                {
                    int i = 1;
                    while (true)
                    {
                        string newPath = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + (i++) + Path.GetExtension(path);
                        if (!File.Exists(newPath))
                        {
                            path = newPath;
                            break;
                        }
                    }
                }

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

                try
                {
                    if (PrintStudetnList)
                    {
                        doc.Save(path, Aspose.Words.SaveFormat.Doc);
                        wb.Save(path2);
                        FISCA.Presentation.MotherForm.SetStatusBarMessage(reportName + "產生完成");
                        System.Diagnostics.Process.Start(path);
                        System.Diagnostics.Process.Start(path2);
                    }
                    else
                    {
                        doc.Save(path, Aspose.Words.SaveFormat.Doc);
                        FISCA.Presentation.MotherForm.SetStatusBarMessage(reportName + "產生完成");
                        System.Diagnostics.Process.Start(path);
                    }
                }
                catch (IOException ex1)
                {
                    SaveFileDialog sd = new SaveFileDialog();
                    sd.Title = "另存新檔";
                    sd.FileName = reportName + ".doc";
                    sd.Filter = "Word檔案 (*.doc)|*.doc|所有檔案 (*.*)|*.*";
                    if (sd.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            doc.Save(sd.FileName, Aspose.Words.SaveFormat.Doc);
                        }
                        catch (IOException ex2)
                        {
                            MsgBox.Show("儲存錯誤,檔案可能開啟中。", "建立檔案失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        catch (Exception ex2)
                        {
                            SmartSchool.ErrorReporting.ReportingService.ReportException(ex2);
                            MsgBox.Show("列印失敗:\n" + ex2.Message);
                            return;
                        }
                    }
                }
                catch (Exception ex1)
                {
                    SmartSchool.ErrorReporting.ReportingService.ReportException(ex1);
                    MsgBox.Show("列印失敗:\n" + ex1.Message);
                    return;
                }
            }
            else
            {
                MsgBox.Show("列印失敗:\n" + e.Error.Message);
                SmartSchool.ErrorReporting.ReportingService.ReportException(e.Error);
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
