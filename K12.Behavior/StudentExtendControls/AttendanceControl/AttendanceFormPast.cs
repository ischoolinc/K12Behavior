using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using FISCA.LogAgent;
using FISCA.Presentation.Controls;
using K12.Behavior.Feature;
using K12.Data;

namespace K12.Behavior.StudentExtendControls
{
    //缺曠登錄主畫面
    partial class AttendanceFormPast : BaseForm
    {
        private ErrorProvider _errorProvider = new ErrorProvider();
        private EditorStatus _status;

        private AttendanceRecord _editor;
        private List<AbsenceMappingInfo> _absenceList;
        private List<PeriodMappingInfo> _periodList;
        private List<AbsenceMappingInfo> absenceList;
        private AbsenceMappingInfo _checkedAbsence;

        //Log使用
        //之前
        private Dictionary<string, string> DicBeforeLog = new Dictionary<string, string>();
        //之後
        private Dictionary<string, string> DicAfterLog = new Dictionary<string, string>();

        public AttendanceFormPast(EditorStatus status, AttendanceRecord editor, List<PeriodMappingInfo> periodList, FISCA.Permission.FeatureAce permission, int SchoolYear, int Semester)
        {
            InitializeComponent();

            #region 初始化學年度學期

            //學年度
            intSchoolYear.Value = SchoolYear;
            intSemester.Value = Semester;

            //intSchoolYear.Items.Add(SchoolYear - 4);
            //intSchoolYear.Items.Add(SchoolYear - 3);
            //intSchoolYear.Items.Add(SchoolYear - 2);
            //intSchoolYear.Items.Add(SchoolYear - 1);
            //int SchoolYearSelectIndex = intSchoolYear.Items.Add(SchoolYear);
            //intSchoolYear.Items.Add(SchoolYear + 1);
            //intSchoolYear.Items.Add(SchoolYear + 2);
            //intSchoolYear.Items.Add(SchoolYear + 3);
            //intSchoolYear.SelectedIndex = SchoolYearSelectIndex;
            //學期
            //intSemester.Items.Add(1);
            //intSemester.Items.Add(2);
            //intSemester.SelectedIndex = Semester == 1 ? 0 : 1;

            #endregion

            #region 初始化缺曠類別

            //初始化時,即取得最新缺曠資料
            absenceList = K12.Data.AbsenceMapping.SelectAll();

            foreach (AbsenceMappingInfo info in absenceList)
            {
                RadioButton rb = new RadioButton();
                rb.Text = info.Name + "(" + info.HotKey.ToUpper() + ")";
                rb.AutoSize = true;
                //rb.Font = new Font(FontStyles.GeneralFontFamily, 9.25f);
                rb.Tag = info;

                flpAbsence.Controls.Add(rb);
            }

            //把第一個缺曠類別設為預設值
            RadioButton fouse = flpAbsence.Controls[0] as RadioButton;
            fouse.Checked = true;

            #endregion

            #region 初始化節次表

            foreach (PeriodMappingInfo info in periodList)
            {
                //Log使用
                DicBeforeLog.Add(info.Name, "");
                DicAfterLog.Add(info.Name, "");
                PeriodControl pc = new PeriodControl();
                pc.Label.Text = info.Name;
                pc.Tag = info;
                pc.TextBox.KeyUp += delegate(object sender, KeyEventArgs e)
                {
                    var txtBox = sender as DevComponents.DotNetBar.Controls.TextBoxX;

                    foreach (AbsenceMappingInfo absenceInfo in absenceList)
                    {
                        if (KeyConverter.GetKeyMapping(e) == absenceInfo.HotKey || KeyConverter.GetKeyMapping(e) == absenceInfo.HotKey.ToUpper())
                        {
                            txtBox.Text = absenceInfo.Abbreviation;

                            if (flpPeriod.GetNextControl(pc, true) != null)
                                (flpPeriod.GetNextControl(pc, true) as PeriodControl).TextBox.Focus();

                            return;
                        }
                    }

                    txtBox.SelectAll();
                };
                pc.TextBox.MouseDoubleClick += new MouseEventHandler(TextBox_MouseDoubleClick);
                flpPeriod.Controls.Add(pc);
            }

            #endregion

            dateTimeInput1.Value = DateTime.Today;

            btnSave.Visible = permission.Editable;
            intSchoolYear.Enabled = permission.Editable;
            intSemester.Enabled = permission.Editable;
            panelAbsence.Enabled = permission.Editable;
            pancelAttendence.Enabled = permission.Editable;

            if (status == EditorStatus.Insert)
            {
                Text = "管理學生缺曠紀錄【新增模式】";
            }

            _status = status;
            _editor = editor;
            _absenceList = absenceList;
            _periodList = periodList;
        }

        public AttendanceFormPast(EditorStatus status, AttendanceRecord editor, List<PeriodMappingInfo> periodList, FISCA.Permission.FeatureAce permission)
        {
            InitializeComponent();

            #region 初始化學年度學期

            //學年度
            intSchoolYear.Value = int.Parse(School.DefaultSchoolYear);
            intSemester.Value = int.Parse(School.DefaultSemester);

            //intSchoolYear.Items.Add(int.Parse(School.DefaultSchoolYear) - 3);
            //intSchoolYear.Items.Add(int.Parse(School.DefaultSchoolYear) - 2);
            //intSchoolYear.Items.Add(int.Parse(School.DefaultSchoolYear) - 1);
            //int SchoolYearSelectIndex = intSchoolYear.Items.Add(int.Parse(School.DefaultSchoolYear));
            //intSchoolYear.Items.Add(int.Parse(School.DefaultSchoolYear) + 1);
            //intSchoolYear.Items.Add(int.Parse(School.DefaultSchoolYear) + 2);
            //intSchoolYear.SelectedIndex = SchoolYearSelectIndex;
            //學期
            //intSemester.Items.Add(1);
            //intSemester.Items.Add(2);
            //intSemester.SelectedIndex = School.DefaultSemester == "1" ? 0 : 1;

            #endregion

            #region 初始化缺曠類別

            //初始化時,即取得最新缺曠資料
            absenceList = K12.Data.AbsenceMapping.SelectAll();

            foreach (AbsenceMappingInfo info in absenceList)
            {
                RadioButton rb = new RadioButton();
                rb.Text = info.Name + "(" + info.HotKey.ToUpper() + ")";
                rb.AutoSize = true;
                //rb.Font = new Font(FontStyles.GeneralFontFamily, 9.25f);
                rb.Tag = info;

                flpAbsence.Controls.Add(rb);
            }

            //把第一個缺曠類別設為預設值
            RadioButton fouse = flpAbsence.Controls[0] as RadioButton;
            fouse.Checked = true;

            #endregion

            #region 初始化節次表

            foreach (PeriodMappingInfo info in periodList)
            {
                //Log使用
                DicBeforeLog.Add(info.Name, "");
                DicAfterLog.Add(info.Name, "");
                PeriodControl pc = new PeriodControl();
                pc.Label.Text = info.Name;
                pc.Tag = info;
                pc.TextBox.KeyUp += delegate(object sender, KeyEventArgs e)
                {
                    var txtBox = sender as DevComponents.DotNetBar.Controls.TextBoxX;

                    foreach (AbsenceMappingInfo absenceInfo in absenceList)
                    {
                        if (KeyConverter.GetKeyMapping(e) == absenceInfo.HotKey || KeyConverter.GetKeyMapping(e) == absenceInfo.HotKey.ToUpper())
                        {
                            txtBox.Text = absenceInfo.Abbreviation;

                            if (flpPeriod.GetNextControl(pc, true) != null)
                                (flpPeriod.GetNextControl(pc, true) as PeriodControl).TextBox.Focus();

                            return;
                        }
                    }

                    txtBox.SelectAll();
                };
                pc.TextBox.MouseDoubleClick += new MouseEventHandler(TextBox_MouseDoubleClick);
                flpPeriod.Controls.Add(pc);
            }

            #endregion

            dateTimeInput1.Value = DateTime.Today;

            btnSave.Visible = permission.Editable;
            intSchoolYear.Enabled = permission.Editable;
            intSemester.Enabled = permission.Editable;
            panelAbsence.Enabled = permission.Editable;
            pancelAttendence.Enabled = permission.Editable;

            if (status == EditorStatus.Insert)
            {
                Text = "管理學生缺曠紀錄【新增模式】";
            }

            if (status == EditorStatus.Update)
            {
                Text = "管理學生缺曠紀錄【修改模式】";

                dateTimeInput1.Value = editor.OccurDate;
                dateTimeInput1.Enabled = false;
                intSchoolYear.Text = editor.SchoolYear.ToString();
                intSemester.Text = editor.Semester.ToString();

                foreach (K12.Data.AttendancePeriod period in editor.PeriodDetail)
                {
                    #region Log

                    if (DicBeforeLog.ContainsKey(period.Period))
                    {
                        DicBeforeLog[period.Period] = period.AbsenceType;
                    }

                    #endregion

                    foreach (PeriodControl pc in flpPeriod.Controls)
                    {
                        PeriodMappingInfo info = pc.Tag as PeriodMappingInfo;

                        if (info == null) continue;
                        if (period.Period != info.Name) continue;
                        if (period.AbsenceType == null) continue;

                        foreach (AbsenceMappingInfo ai in absenceList)
                        {
                            if (ai.Name != period.AbsenceType) continue;

                            pc.TextBox.Text = ai.Abbreviation;
                            break;
                        }
                    }
                }
            }

            if (!permission.Editable)
                Text = "管理學生缺曠紀錄【檢視模式】";

            _status = status;
            _editor = editor;
            _absenceList = absenceList;
            _periodList = periodList;
        }

        //private string chengDateTime(DateTime x)
        //{
        //    string time = x.ToString();
        //    int y = time.IndexOf(' ');
        //    return time.Remove(y);
        //}

        void TextBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            foreach (RadioButton var in flpAbsence.Controls)
            {
                if (var.Checked)
                {
                    _checkedAbsence = (AbsenceMappingInfo)var.Tag;

                    TextBox textBox = sender as TextBox;
                    if (textBox.Text == _checkedAbsence.Abbreviation)
                    {
                        textBox.Text = "";
                        textBox.Tag = null;
                        return;
                    }
                    textBox.Text = _checkedAbsence.Abbreviation;
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (_status == EditorStatus.Insert)
            {
                foreach (AttendanceRecord each in Attendance.SelectByStudentIDs(new string[] { _editor.RefStudentID }))
                {
                    if (each.OccurDate == dateTimeInput1.Value)
                    {
                        _errorProvider.SetError(dateTimeInput1, "此日期已有紀錄存在，請改用修改模式");
                        return;
                    }
                }
            }

            _editor.OccurDate = dateTimeInput1.Value;
            _editor.SchoolYear = int.Parse(intSchoolYear.Text);
            _editor.Semester = int.Parse(intSemester.Text);

            List<K12.Data.AttendancePeriod> periodDetail = new List<K12.Data.AttendancePeriod>();

            foreach (PeriodControl pc in flpPeriod.Controls)
            {
                if (pc.TextBox.Text.Trim() != "")
                {
                    K12.Data.AttendancePeriod ap = new K12.Data.AttendancePeriod();

                    foreach (AbsenceMappingInfo ai in _absenceList)
                    {
                        if (ai.Abbreviation == pc.TextBox.Text)
                        {
                            ap.Period = (pc.Tag as PeriodMappingInfo).Name;
                            ap.AbsenceType = ai.Name;
                            //ap.AttendanceType = "一般";
                        }
                    }

                    periodDetail.Add(ap);
                }
            }

            _editor.PeriodDetail = periodDetail;

            if (periodDetail.Count == 0)
            {
                MsgBox.Show("未輸入正確資料!儲存失敗!");
                return;
            }

            if (_status == EditorStatus.Insert)
            {
                #region Insert
                try
                {
                    Attendance.Insert(_editor);
                }
                catch (Exception ex)
                {
                    MsgBox.Show("新增缺曠資料失敗!" + ex.Message);
                    return;
                }

                StringBuilder sb = new StringBuilder();
                sb.Append("學生「" + _editor.Student.Name + "」");
                sb.Append("日期「" + _editor.OccurDate.ToShortDateString() + "」");
                sb.AppendLine("新增一筆缺曠資料。");
                sb.AppendLine("詳細資料：");
                foreach (K12.Data.AttendancePeriod each in _editor.PeriodDetail)
                {
                    sb.AppendLine("節次「" + each.Period + "」設為「" + each.AbsenceType + "」");
                }

                ApplicationLog.Log("學務系統.缺曠資料", "新增學生缺曠資料", "student", _editor.Student.ID, sb.ToString());
                MsgBox.Show("新增學生缺曠資料成功!"); 
                #endregion
            }
            else if (_status == EditorStatus.Update)
            {
                #region Update
                try
                {
                    Attendance.Update(_editor);
                }
                catch (Exception ex)
                {
                    MsgBox.Show("修改缺曠資料失敗!" + ex.Message);
                    return;
                }

                StringBuilder sb = new StringBuilder();
                sb.Append("學生「" + _editor.Student.Name + "」");
                sb.Append("日期「" + _editor.OccurDate.ToShortDateString() + "」");
                sb.AppendLine("缺曠資料已修改。");
                sb.AppendLine("詳細資料：");

                foreach (K12.Data.AttendancePeriod each in _editor.PeriodDetail)
                {
                    if (DicAfterLog.ContainsKey(each.Period))
                    {
                        DicAfterLog[each.Period] = each.AbsenceType;
                    }
                }

                bool LogMode = false;
                foreach (PeriodMappingInfo each in _periodList)
                {
                    if (DicAfterLog[each.Name] != "" && DicBeforeLog[each.Name] != "" && DicAfterLog[each.Name] != DicBeforeLog[each.Name])
                    {
                        sb.AppendLine("節次「" + each.Name + "」由「" + DicBeforeLog[each.Name] + "」變更為「" + DicAfterLog[each.Name] + "」");
                        LogMode = true;
                    }
                }

                if (LogMode)
                {
                    ApplicationLog.Log("學務系統.缺曠資料", "修改學生缺曠資料", "student", _editor.Student.ID, sb.ToString());
                }

                MsgBox.Show("修改學生缺曠資料成功!"); 
                #endregion
            }
            this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}