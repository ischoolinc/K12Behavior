using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using Aspose.Cells;
using Framework;
using K12.Data;

namespace K12.學生獎勵懲戒明細
{
    internal static class CommonMethods
    {
        //Excel報表
        public static void ExcelReport_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                MsgBox.Show("未取得獎勵懲戒資料");
                return;
            }

            string reportName;
            string path;
            Workbook wb;

            object[] result = (object[])e.Result;
            reportName = (string)result[0];
            path = (string)result[1];
            wb = (Workbook)result[2];

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

            try
            {
                wb.Save(path, FileFormatType.Excel2003);
                FISCA.Presentation.MotherForm.SetStatusBarMessage(reportName + "產生完成");
                System.Diagnostics.Process.Start(path);
            }
            catch
            {
                SaveFileDialog sd = new SaveFileDialog();
                sd.Title = "另存新檔";
                sd.FileName = reportName + ".xls";
                sd.Filter = "Excel檔案 (*.xls)|*.xls|所有檔案 (*.*)|*.*";
                if (sd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        wb.Save(sd.FileName, FileFormatType.Excel2003);
                    }
                    catch
                    {
                        MsgBox.Show("指定路徑無法存取。", "建立檔案失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
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

        public static int ClassSeatNoComparer(StudentRecord x, StudentRecord y)
        {
            string PadLeft1 = x.SeatNo.HasValue ? x.SeatNo.Value.ToString() : "";
            string PadLeft2 = y.SeatNo.HasValue ? y.SeatNo.Value.ToString() : "";

            string xx = (x.Class == null ? "" : x.Class.Name) + "::" + PadLeft1.PadLeft(2, '0');
            string yy = (y.Class == null ? "" : y.Class.Name) + "::" + PadLeft2.PadLeft(2, '0');

            return xx.CompareTo(yy);
        }

        public static int ClassComparer(ClassRecord x, ClassRecord y)
        {
            string xx = x.Name;
            string yy = y.Name;

            return xx.CompareTo(yy);
        }
    }
}
