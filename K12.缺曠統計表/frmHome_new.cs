using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Aspose.Cells;
using Campus.Report;
using FISCA.Data;
using K12.Data;
using K12.Data.Configuration;
using FISCA.Presentation;

namespace K12.缺曠統計表
{
     public partial class frmHome_new : FISCA.Presentation.Controls.BaseForm
    {
        private QueryHelper mHelper = new QueryHelper();
        private BackgroundWorker worker = new BackgroundWorker();

        public frmHome_new()
        {
            InitializeComponent();
        }

        private void frmHome_Load(object sender, EventArgs e)
        {
            #region 判斷是否用自訂範本，以及自訂範本是否有內容才套用
            ConfigData config = K12.Data.School.Configuration["缺曠統計表"];

            string strStartDate = config["開始日期"];
            string strEndDate = config["結束日期"];

            DateTime? StartDate = K12.Data.DateTimeHelper.Parse(strStartDate);
            DateTime? EndDate = K12.Data.DateTimeHelper.Parse(strEndDate);

            dateStart.Value = StartDate.HasValue ? StartDate.Value : DateTime.Today;
            dateEnd.Value = EndDate.HasValue ? EndDate.Value : DateTime.Today;

            #endregion
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            DateTime Start = dateStart.Value;
            DateTime End = dateEnd.Value;

            #region 儲存 Preference
            ConfigData config = K12.Data.School.Configuration["缺曠統計表"];

            config["開始日期"] = Start.ToShortDateString();
            config["結束日期"] = End.ToShortDateString();

            config.Save();
            #endregion

            worker.WorkerReportsProgress = true;
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            worker.RunWorkerAsync(new List<object>() { Start, End });
            btnPrint.Enabled = false;
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            MotherForm.SetStatusBarMessage("產生缺曠統計報表中...", e.ProgressPercentage);
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnPrint.Enabled = true;

            ClassSat vClassSat = e.Result as ClassSat;

            FISCA.Presentation.MotherForm.SetStatusBarMessage("缺曠統計報表產生完成");

            try
            {
                byte[] Byte = Properties.Resources.缺曠統計報表; //將成績單先為預設

                #region 判斷是否用自訂範本，以及自訂範本是否有內容才套用
                //ConfigData config = K12.Data.School.Configuration["調代課通知單"];
                //int _useTemplateNumber = 0;
                //int.TryParse(config["TemplateNumber"], out _useTemplateNumber);
                //string customize = config["CustomizeTemplate"];
                //if (!string.IsNullOrEmpty(customize) && _useTemplateNumber == 1)
                //    Byte = Convert.FromBase64String(customize);
                #endregion
                List<string> Absences = AbsenceMapping
                    .SelectAll()
                    .Select(x => x.Name)
                    .ToList();

                Absences.Sort();

                MemoryStream Stream = new MemoryStream(Byte);

                Workbook template = new Workbook();
                template.Open(Stream);

                for (int i = 1; i < Absences.Count; i++)
                {
                    template.Worksheets["DataSection"].Cells.CopyColumn(template.Worksheets["DataSection"].Cells, 3, 3 + i);
                    template.Worksheets["DataSection"].Cells[0, 3 + i].PutValue(string.Empty);
                    template.Worksheets["PageHeader"].Cells.CopyColumn(template.Worksheets["PageHeader"].Cells, 3, 3 + i);
                }

                for (int i = 0; i < Absences.Count; i++)
                    template.Worksheets["PageHeader"].Cells[3, 3 + i].PutValue(Absences[i]);

                template.Worksheets["PageHeader"].Cells.Merge(0, 0, 1, 3 + Absences.Count);
                template.Worksheets["PageHeader"].Cells.Merge(1, 0, 1, 3 + Absences.Count);
                template.Worksheets["PageHeader"].Cells[0, 0].Style.HorizontalAlignment = TextAlignmentType.Center;
                template.Worksheets["PageHeader"].Cells[1, 0].Style.HorizontalAlignment = TextAlignmentType.Center;
                template.Worksheets["PageHeader"].AutoFitColumns();

                MemoryStream templateStream = template.SaveToStream();

                Workbook wb = ReportHelper.Report.Produce(vClassSat.ToReportData(dateStart.Value, dateEnd.Value, Absences), templateStream, false);

                foreach (Worksheet sheet in wb.Worksheets)
                    sheet.PageSetup.CenterHorizontally = true;

                string mSaveFilePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\Reports\\缺曠統計表.xls";

                ReportSaver.SaveWorkbook(wb, mSaveFilePath);
            }
            catch (Exception ve)
            {
                MessageBox.Show(ve.Message);
                SmartSchool.ErrorReporting.ReportingService.ReportException(ve);
            }
            finally
            {
                this.Close();
            }
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<object> Argument = e.Argument as List<object>;

            DateTime Start = (DateTime)Argument[0];
            DateTime End = (DateTime)Argument[1];

            //建立獎懲統計物件
            ClassSat vClassSat = new ClassSat();

            #region 找出一般及延修生
            DataTable table = mHelper.Select("select id from student where status in (1,2)");

            List<string> GStudentIDs = new List<string>();

            foreach (DataRow row in table.Rows)
            {
                string StudentID = row.Field<string>("id");
                GStudentIDs.Add(StudentID);
            }
            #endregion

            //尋找日期區間內的獎懲記錄
            List<AttendanceRecord> records = Attendance
                .SelectByDate(Start, End);

            records = records
                .FindAll(x=> GStudentIDs.Contains(x.RefStudentID));

            //找出獎懲記錄的學生系統編號
            List<string> StudentIDs = records
                .Select(x => x.RefStudentID)
                .Distinct()
                .ToList();

            #region 找出學生系統編號對應的班級名稱
            Dictionary<string, string> StudentClassNames = new Dictionary<string, string>();

            if (StudentIDs.Count > 0)
            {
                DataTable vtable = mHelper.Select("select student.id,class.class_name from student left outer join class on student.ref_class_id=class.id where student.id in (" + string.Join(",", StudentIDs.ToArray()) + ")");

                foreach (DataRow row in vtable.Rows)
                {
                    string StudentID = row.Field<string>("id");
                    string ClassName = row.Field<string>("class_name");

                    if (!StudentClassNames.ContainsKey(StudentID))
                        StudentClassNames.Add(StudentID, ClassName);
                }
            }
            #endregion

            #region 將每筆缺曠加入至統計中
            for (int i = 0; i < records.Count; i++)
            {
                if (StudentClassNames.ContainsKey(records[i].RefStudentID))
                    vClassSat.Add(StudentClassNames[records[i].RefStudentID], records[i]);
                else
                    throw new Exception("缺曠學生對應不存在！");

                worker.ReportProgress((int)((double)i / records.Count * 100));
            }
            #endregion

            e.Result = vClassSat;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
