using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using FISCA.LogAgent;
using FISCA.Presentation.Controls;
using K12.Behavior.Feature;
using K12.Data;
using SHSchool.Behavior.StudentExtendControls;
using SHSchool.Data;

namespace K12.Behavior.StudentExtendControls
{
    [FISCA.Permission.FeatureCode("K12.Student.AttendanceItem", "缺曠記錄")]
    public partial class AttendanceItem : DetailContentBase
    {
        internal static FISCA.Permission.FeatureAce UserPermission;

        private List<AttendanceRecord> _records = new List<AttendanceRecord>();
        private List<PeriodMappingInfo> _periodList = new List<PeriodMappingInfo>();
        private List<AbsenceMappingInfo> _absenceList = new List<AbsenceMappingInfo>();

        private BackgroundWorker BGW = new BackgroundWorker();
        private bool BkWBool = false;

        public AttendanceItem()
        {
            InitializeComponent();
            Group = "缺曠紀錄";

            Attendance.AfterDelete += new EventHandler<K12.Data.DataChangedEventArgs>(Attendance_AfterDelete);
            Attendance.AfterInsert += new EventHandler<K12.Data.DataChangedEventArgs>(Attendance_AfterDelete);
            Attendance.AfterUpdate += new EventHandler<K12.Data.DataChangedEventArgs>(Attendance_AfterDelete);

            //這是暫解法
            //Attendance.Instance.ItemUpdated += new EventHandler<ItemUpdatedEventArgs>(Instance_ItemUpdated);

            //Initialize();

            //Attendance.Instance.ItemUpdated += new EventHandler<ItemUpdatedEventArgs>(Instance_ItemUpdated);

            BGW.DoWork += new DoWorkEventHandler(BkW_DoWork);
            BGW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BkW_RunWorkerCompleted);

            UserPermission = FISCA.Permission.UserAcl.Current[FISCA.Permission.FeatureCodeAttribute.GetCode(GetType())];

            btnAdd.Visible = UserPermission.Editable;
            btnUpdate.Visible = UserPermission.Editable;
            btnDelete.Visible = UserPermission.Editable;
            btnView.Visible = UserPermission.Viewable & !UserPermission.Editable;
        }

        void Attendance_AfterDelete(object sender, K12.Data.DataChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, K12.Data.DataChangedEventArgs>(Attendance_AfterDelete), sender, e);
            }
            else
            {
                if (this.PrimaryKey != "")
                {
                    this.Loading = true;

                    if (BGW.IsBusy)
                    {
                        BkWBool = true;
                    }
                    else
                    {
                        BGW.RunWorkerAsync();
                    }
                }
            }
        }

        //這是暫解法
        //void Instance_ItemUpdated(object sender, ItemUpdatedEventArgs e)
        //{
        //    if (!BGW.IsBusy)
        //        BGW.RunWorkerAsync();
        //}

        //建立預設畫面
        private void Initialize()
        {
            //取得此 Class 定義的 FeatureCode。
            //FeatureCodeAttribute code = Attribute.GetCustomAttribute(this.GetType(), typeof(FeatureCodeAttribute)) as FeatureCodeAttribute;
            //_permission = Framework.Legacy.GlobalOld.Acl[code.FeatureCode];
            _periodList.Clear();
            _periodList = K12.Data.PeriodMapping.SelectAll();
            _absenceList.Clear();
            _absenceList = K12.Data.AbsenceMapping.SelectAll();

            listView.Columns.Clear();

            listView.Columns.Add("SchoolYear", "學年度");
            listView.Columns.Add("Semester", "學期");
            listView.Columns.Add("OccurDate", "缺曠日期");
            listView.Columns.Add("DayOfWeek", "星期");

            foreach (PeriodMappingInfo info in _periodList)
            {
                ColumnHeader column = listView.Columns.Add(info.Name, info.Name);
                column.Tag = info;
            }
        }

        protected override void OnPrimaryKeyChanged(EventArgs e)
        {
            //Attendance.Instance.SyncDataBackground(PrimaryKey);
            //base.OnPrimaryKeyChanged(e);

            this.Loading = true;

            if (BGW.IsBusy)
            {
                BkWBool = true;
            }
            else
            {
                BGW.RunWorkerAsync();
            }
        }

        void BkW_DoWork(object sender, DoWorkEventArgs e)
        {
            if (string.IsNullOrEmpty(this.PrimaryKey)) return;

            _records.Clear();
            _records = Attendance.SelectByStudentIDs(new string[] { this.PrimaryKey });
        }

        void BkW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (BkWBool)
            {
                BkWBool = false;
                BGW.RunWorkerAsync();
                return;
            }
            BindData();
            this.Loading = false;
        }

        private void BindData()
        {
            Initialize();

            listView.Items.Clear();

            _records.Sort(new Comparison<AttendanceRecord>(SchoolYearComparer));

            foreach (AttendanceRecord record in _records)
            {
                ListViewItem lvItem = listView.Items.Add(record.SchoolYear.ToString());

                lvItem.Tag = record;
                lvItem.SubItems.Add(record.Semester.ToString());
                lvItem.SubItems.Add(record.OccurDate.ToShortDateString());
                lvItem.SubItems.Add(record.DayOfWeek);
                if (listView.Columns.Count != 0)
                {
                    listView.Columns[0].Width = 70;
                    listView.Columns[1].Width = 50;
                    listView.Columns[2].Width = 110;
                }

                for (int i = 4; i < listView.Columns.Count; i++)
                    lvItem.SubItems.Add("");

                for (int i = 4; i < listView.Columns.Count; i++)
                {
                    ColumnHeader column = listView.Columns[i];
                    PeriodMappingInfo info = column.Tag as PeriodMappingInfo;

                    //if (record.PeriodDetail == null) continue;

                    foreach (AttendancePeriod period in record.PeriodDetail)
                    {
                        if (info == null) continue;
                        if (period.Period != info.Name) continue;
                        if (period.AbsenceType == null) continue;

                        System.Windows.Forms.ListViewItem.ListViewSubItem subitem = lvItem.SubItems[i];

                        foreach (AbsenceMappingInfo ai in _absenceList)
                        {
                            if (ai.Name != period.AbsenceType) continue;

                            subitem.Text = ai.Abbreviation;
                            break;
                        }
                    }
                }
            }
        }

        //private string chengDateTime(DateTime x)
        //{
        //    if (x == null)
        //        return "";
        //    string time = x.ToShortDateString();
        //    int y = time.IndexOf(' ');
        //    return time.Remove(y);
        //}

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AttendanceRecord record = new AttendanceRecord();
            record.RefStudentID = this.PrimaryKey;

            //SingleEditor singleEditor = new SingleEditor(SHStudent.SelectByID(Presentation.NLDPanels.Student.SelectedSource[0]));
            //singleEditor.ShowDialog();
            AttendanceForm editor = new AttendanceForm(Feature.EditorStatus.Insert, record, _periodList, UserPermission);
            editor.ShowDialog();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count == 0)
            {
                MsgBox.Show("請先選擇一筆您要修改的資料");
                return;
            }
            else if (listView.SelectedItems.Count > 1)
            {
                MsgBox.Show("選擇資料筆數過多，一次只能修改一筆資料");
                return;
            }

            //SingleEditor singleEditor = new SingleEditor(SHStudent.SelectByID(Presentation.NLDPanels.Student.SelectedSource[0]), (listView.SelectedItems[0].Tag as AttendanceRecord).OccurDate);
            //singleEditor.ShowDialog();
            AttendanceForm editor = new AttendanceForm(Feature.EditorStatus.Update, listView.SelectedItems[0].Tag as AttendanceRecord, _periodList, UserPermission);
            editor.ShowDialog();
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            btnUpdate_Click(sender, e);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count == 0)
            {
                MsgBox.Show("必須選擇一筆以上資料!!");
                return;
            }

            List<AttendanceRecord> AttendanceList = new List<AttendanceRecord>();

            if (MsgBox.Show("確定將刪除所選擇之缺曠資料?", "確認", MessageBoxButtons.YesNo) == DialogResult.No) return;

            foreach (ListViewItem item in listView.SelectedItems)
            {
                AttendanceRecord editor = item.Tag as AttendanceRecord;
                AttendanceList.Add(editor);
            }

            try
            {
                Attendance.Delete(AttendanceList);
            }
            catch (Exception ex)
            {
                MsgBox.Show("刪除缺曠資料失敗" + ex.Message);
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("學生「" + K12.Data.Student.SelectByID(this.PrimaryKey).Name + "」");
            foreach (AttendanceRecord att in AttendanceList)
            {
                sb.AppendLine("日期「" + att.OccurDate.ToShortDateString() + "」");
            }
            sb.AppendLine("缺曠資料已被刪除。");

            //DSXmlHelper LogDescription = new DSXmlHelper("Log");
            //LogDescription.AddElement("Description");
            //LogDescription.AddText("Description", sb.ToString());
            //LogDescription.AddElement("SubLogs");
            //LogDescription.AddElement("SubLogs", "Log");
            //LogDescription.SetAttribute("SubLogs/Log", "ID", editor.Student.ID);

            ApplicationLog.Log("學務系統.缺曠資料", "刪除學生缺曠資料", "student", this.PrimaryKey, sb.ToString());

            MsgBox.Show("刪除缺曠資料成功");
        }
        private int SchoolYearComparer(AttendanceRecord x, AttendanceRecord y)
        {
            return y.OccurDate.CompareTo(x.OccurDate);
        }

        private void listView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView.SelectedItems.Count == 1)
            {
                //SingleEditor singleEditor = new SingleEditor(SHStudent.SelectByID(Presentation.NLDPanels.Student.SelectedSource[0]), (listView.SelectedItems[0].Tag as AttendanceRecord).OccurDate);
                //singleEditor.ShowDialog();
                AttendanceForm editor = new AttendanceForm(Feature.EditorStatus.Update, listView.SelectedItems[0].Tag as AttendanceRecord, _periodList, UserPermission);
                editor.ShowDialog();
            }
        }

        private void listView_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}