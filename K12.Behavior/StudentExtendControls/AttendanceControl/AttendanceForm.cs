using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using DevComponents.DotNetBar;
using FISCA.LogAgent;
using K12.Data;
using K12.Behavior.Feature;
using FISCA.Presentation.Controls;
using FISCA.Permission;

namespace K12.Behavior.StudentExtendControls
{
    partial class AttendanceForm : BaseForm
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


        public AttendanceForm(EditorStatus status, AttendanceRecord editor, List<PeriodMappingInfo> periodList, FeatureAce permission)
        {
            InitializeComponent();

            #region 初始化學年度學期

            //學年度
            intSchoolYear.Value = int.Parse(K12.Data.School.DefaultSchoolYear);
            intSemester.Value = int.Parse(K12.Data.School.DefaultSemester);

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
                rb.CheckedChanged += delegate (object sender, EventArgs e)
                {
                    if (rb.Checked)
                    {
                        foreach (DataGridViewCell cell in dataGridViewX1.SelectedCells)
                        {
                            cell.Value = (rb.Tag as AbsenceMappingInfo).Abbreviation;
                        }
                    }
                };
                flpAbsence.Controls.Add(rb);
            }

            //把第一個缺曠類別設為預設值
            RadioButton fouse = flpAbsence.Controls[0] as RadioButton;
            fouse.Checked = true;

            #endregion

            #region 初始化節次表

            List<string> plist = new List<string>();
            foreach (PeriodMappingInfo info in periodList)
            {
                //Log使用
                DicBeforeLog.Add(info.Name, "");
                DicAfterLog.Add(info.Name, "");

                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                column.Width = 40;
                column.HeaderText = info.Name;
                plist.Add(info.Name);
                column.Tag = info;
                dataGridViewX1.Columns.Add(column);
            }
            Campus.Windows.DataGridViewImeDecorator dec = new Campus.Windows.DataGridViewImeDecorator(this.dataGridViewX1, plist);

            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(dataGridViewX1);
            dataGridViewX1.Rows.Add(row);
            dataGridViewX1.AutoResizeColumns();
            #endregion

            dateTimeInput1.Value = DateTime.Today;

            btnSave.Visible = permission.Editable;
            intSchoolYear.Enabled = permission.Editable;
            intSemester.Enabled = permission.Editable;
            panelAbsence.Enabled = permission.Editable;
            //pancelAttendence.Enabled = permission.Editable;

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

                    foreach (DataGridViewCell cell in dataGridViewX1.Rows[0].Cells)
                    {
                        PeriodMappingInfo info = cell.OwningColumn.Tag as PeriodMappingInfo;

                        if (info == null) continue;
                        if (period.Period != info.Name) continue;
                        if (period.AbsenceType == null) continue;

                        foreach (AbsenceMappingInfo ai in absenceList)
                        {
                            if (ai.Name != period.AbsenceType) continue;

                            cell.Value = ai.Abbreviation;
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            //2023/3/14 - 增加驗證使用者是否未輸入時間
            if (dateTimeInput1.Text == "0001/01/01 00:00:00" || dateTimeInput1.Text == "")
            {
                _errorProvider.SetError(dateTimeInput1, "請輸入時間日期");
                return;
            }
            else
            {
                _errorProvider.SetError(dateTimeInput1, "");
            }


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
            _editor.SchoolYear = intSchoolYear.Value;
            _editor.Semester = intSemester.Value;

            List<K12.Data.AttendancePeriod> periodDetail = new List<K12.Data.AttendancePeriod>();

            foreach (DataGridViewCell cell in dataGridViewX1.Rows[0].Cells)
            {
                if (!string.IsNullOrEmpty(("" + cell.Value).Trim()))
                {
                    K12.Data.AttendancePeriod ap = new K12.Data.AttendancePeriod();

                    foreach (AbsenceMappingInfo ai in _absenceList)
                    {
                        if (ai.Abbreviation == ("" + cell.Value).Trim())
                        {
                            ap.Period = (cell.OwningColumn.Tag as PeriodMappingInfo).Name;
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
                MsgBox.Show("不可儲存無缺曠內容之資料!");
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
                    if (DicAfterLog[each.Name] != DicBeforeLog[each.Name])
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


        private void dataGridViewX1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            dataGridViewX1.EndEdit();
        }

        private void dataGridViewX1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dataGridViewX1.EndEdit();
        }

        private void dataGridViewX1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            foreach (RadioButton var in flpAbsence.Controls)
            {
                if (var.Checked)
                {
                    _checkedAbsence = (AbsenceMappingInfo)var.Tag;
                    DataGridViewCell dgvCell = dataGridViewX1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    //如果有值,就清空
                    if ("" + dgvCell.Value == _checkedAbsence.Abbreviation)
                    {
                        dgvCell.Value = "";
                        return;
                    }
                    dgvCell.Value = _checkedAbsence.Abbreviation;
                }
            }
        }

        private void dataGridViewX1_KeyUp(object sender, KeyEventArgs e)
        {
            string CurrentCellValue = ("" + dataGridViewX1.CurrentCell.Value).Trim();

            if (CheckKeys(e) && "" + CurrentCellValue == "")
            {
                return;
            }


            //如果未輸入資料(如果是按空白鍵)
            if (string.IsNullOrEmpty(CurrentCellValue))
            {
                foreach (DataGridViewCell cell in dataGridViewX1.SelectedCells)
                {
                    cell.Value = ""; //清空資料
                }
                dataGridViewX1.GoToNEXTCell();
            }
            else
            {
                //如果不是空白資料,就比對對照表
                string Abbreviation = "";
                foreach (AbsenceMappingInfo absenceInfo in absenceList)
                {
                    if (absenceInfo.HotKey.ToUpper() == CurrentCellValue.ToUpper())
                    {
                        Abbreviation = absenceInfo.Abbreviation;
                        break;
                    }
                }

                if (Abbreviation != "") //對到資料,就替換為縮寫
                {
                    foreach (DataGridViewCell cell in dataGridViewX1.SelectedCells)
                    {
                        cell.Value = Abbreviation;
                    }
                }
                else //沒有比對到資料,就清空資料
                {
                    foreach (DataGridViewCell cell in dataGridViewX1.SelectedCells)
                    {
                        cell.Value = "";
                    }
                }
                dataGridViewX1.GoToNEXTCell();
            }
        }

        private bool CheckKeys(KeyEventArgs e)
        {
            if (e.Alt || e.Control || e.Shift ||
                e.KeyCode == Keys.Up ||
                e.KeyCode == Keys.Down ||
                e.KeyCode == Keys.Left ||
                e.KeyCode == Keys.Right ||
                e.KeyCode == Keys.Tab ||
                e.KeyCode == Keys.ShiftKey ||
                e.KeyCode == (Keys.ShiftKey | Keys.LButton) ||
                e.KeyCode == (Keys.ShiftKey | Keys.RButton) ||
                e.KeyCode == (Keys.ControlKey | Keys.LButton) ||
                e.KeyCode == (Keys.ControlKey | Keys.RButton))
            {
                return true;
            }
            else
            {
                return false;
            }

            //if (e.KeyValue < 123 && e.KeyValue > 64)
        }

        private void dataGridViewX1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            foreach (DataGridViewRow row in dataGridViewX1.Rows)
            {
                row.Cells[e.ColumnIndex].Selected = true;
            }
        }

        #region 此效果是取消DataGridView一開始會選取一格的效果

        bool SelectedFalse = true;

        private void dataGridViewX1_SelectionChanged(object sender, EventArgs e)
        {
            if (SelectedFalse)
            {
                dataGridViewX1.Rows[0].Cells[0].Selected = false;
                SelectedFalse = false;
            }
        }
        #endregion
    }
}