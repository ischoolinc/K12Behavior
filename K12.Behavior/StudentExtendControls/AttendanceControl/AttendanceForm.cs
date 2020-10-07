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

        //Log�ϥ�
        //���e
        private Dictionary<string, string> DicBeforeLog = new Dictionary<string, string>();
        //����
        private Dictionary<string, string> DicAfterLog = new Dictionary<string, string>();

        #region �L�Ϋغc��
        /// <summary>
        /// �s�W�Ҧ�
        /// </summary>
        //public AttendanceForm(EditorStatus status, AttendanceRecord editor, List<PeriodMappingInfo> periodList, FeatureAce permission, int SchoolYear, int Semester)
        //{
        //    InitializeComponent();

        //    #region ��l�ƾǦ~�׾Ǵ�

        //    //�Ǧ~��
        //    intSchoolYear.Value = SchoolYear;
        //    intSemester.Value = Semester;

        //    #endregion

        //    #region ��l�Ư��m���O

        //    //��l�Ʈ�,�Y���o�̷s���m���
        //    absenceList = K12.Data.AbsenceMapping.SelectAll();

        //    foreach (AbsenceMappingInfo info in absenceList)
        //    {
        //        RadioButton rb = new RadioButton();
        //        //���m�O,�Y�g,����
        //        rb.Text = info.Name + "(" + info.HotKey.ToUpper() + ")";
        //        rb.AutoSize = true;
        //        //rb.Font = new Font(FontStyles.GeneralFontFamily, 9.25f);
        //        rb.Tag = info;
        //        rb.CheckedChanged += delegate(object sender, EventArgs e)
        //        {
        //            if (rb.Checked)
        //            {
        //                foreach (DataGridViewCell cell in dataGridViewX1.SelectedCells)
        //                {
        //                    cell.Value = (rb.Tag as AbsenceMappingInfo).Abbreviation;
        //                }
        //            }
        //        };
        //        flpAbsence.Controls.Add(rb);
        //    }

        //    //��Ĥ@�ӯ��m���O�]���w�]��
        //    RadioButton fouse = flpAbsence.Controls[0] as RadioButton;
        //    fouse.Checked = true;

        //    #endregion

        //    #region ��l�Ƹ`����
        //    foreach (PeriodMappingInfo info in periodList)
        //    {
        //        //Log�ϥ�
        //        DicBeforeLog.Add(info.Name, "");
        //        DicAfterLog.Add(info.Name, "");

        //        DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
        //        column.Width = 40;
        //        column.HeaderText = info.Name;
        //        column.Tag = info;
        //        dataGridViewX1.Columns.Add(column);

        //        //PeriodControl pc = new PeriodControl();
        //        //pc.Label.Text = info.Name;
        //        //pc.Tag = info;
        //        //pc.TextBox.KeyUp += delegate(object sender, KeyEventArgs e)
        //        //{
        //        //    var txtBox = sender as DevComponents.DotNetBar.Controls.TextBoxX;
        //        //    foreach (AbsenceMappingInfo absenceInfo in absenceList)
        //        //    {
        //        //        if (KeyConverter.GetKeyMapping(e) == absenceInfo.HotKey || KeyConverter.GetKeyMapping(e) == absenceInfo.HotKey.ToUpper())
        //        //        {
        //        //            txtBox.Text = absenceInfo.Abbreviation;
        //        //            if (flpPeriod.GetNextControl(pc, true) != null)
        //        //                (flpPeriod.GetNextControl(pc, true) as PeriodControl).TextBox.Focus();
        //        //            return;
        //        //        }
        //        //    }
        //        //    txtBox.SelectAll();
        //        //};
        //        //pc.TextBox.MouseDoubleClick += new MouseEventHandler(TextBox_MouseDoubleClick);
        //        //flpPeriod.Controls.Add(pc);
        //    }
        //    DataGridViewRow row = new DataGridViewRow();
        //    row.CreateCells(dataGridViewX1);
        //    dataGridViewX1.Rows.Add(row);
        //    dataGridViewX1.AutoResizeColumns();
        //    #endregion

        //    dateTimeInput1.Value = DateTime.Today;

        //    btnSave.Visible = permission.Editable;
        //    intSchoolYear.Enabled = permission.Editable;
        //    intSemester.Enabled = permission.Editable;
        //    panelAbsence.Enabled = permission.Editable;
        //    //pancelAttendence.Enabled = permission.Editable;
        //    dataGridViewX1.Enabled = permission.Editable;

        //    if (status == EditorStatus.Insert)
        //    {
        //        Text = "�޲z�ǥͯ��m�����i�s�W�Ҧ��j";
        //    }

        //    _status = status;
        //    _editor = editor;
        //    _absenceList = absenceList;
        //    _periodList = periodList;
        //} 
        #endregion

        public AttendanceForm(EditorStatus status, AttendanceRecord editor, List<PeriodMappingInfo> periodList, FeatureAce permission)
        {
            InitializeComponent();

            #region ��l�ƾǦ~�׾Ǵ�

            //�Ǧ~��
            intSchoolYear.Value = int.Parse(K12.Data.School.DefaultSchoolYear);
            intSemester.Value = int.Parse(K12.Data.School.DefaultSemester);

            #endregion

            #region ��l�Ư��m���O

            //��l�Ʈ�,�Y���o�̷s���m���
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

            //��Ĥ@�ӯ��m���O�]���w�]��
            RadioButton fouse = flpAbsence.Controls[0] as RadioButton;
            fouse.Checked = true;

            #endregion

            #region ��l�Ƹ`����

            List<string> plist = new List<string>();
            foreach (PeriodMappingInfo info in periodList)
            {
                //Log�ϥ�
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
                Text = "�޲z�ǥͯ��m�����i�s�W�Ҧ��j";
            }

            if (status == EditorStatus.Update)
            {
                Text = "�޲z�ǥͯ��m�����i�ק�Ҧ��j";

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
                Text = "�޲z�ǥͯ��m�����i�˵��Ҧ��j";

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
            if (_status == EditorStatus.Insert)
            {
                foreach (AttendanceRecord each in Attendance.SelectByStudentIDs(new string[] { _editor.RefStudentID }))
                {
                    if (each.OccurDate == dateTimeInput1.Value)
                    {
                        _errorProvider.SetError(dateTimeInput1, "������w�������s�b�A�Ч�έק�Ҧ�");
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
                            //ap.AttendanceType = "�@��";

                        }
                    }

                    periodDetail.Add(ap);
                }
            }

            _editor.PeriodDetail = periodDetail;

            if (periodDetail.Count == 0)
            {
                MsgBox.Show("���i�x�s�L���m���e�����!");
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
                    MsgBox.Show("�s�W���m��ƥ���!" + ex.Message);
                    return;
                }

                StringBuilder sb = new StringBuilder();
                sb.Append("�ǥ͡u" + _editor.Student.Name + "�v");
                sb.Append("����u" + _editor.OccurDate.ToShortDateString() + "�v");
                sb.AppendLine("�s�W�@�����m��ơC");
                sb.AppendLine("�ԲӸ�ơG");
                foreach (K12.Data.AttendancePeriod each in _editor.PeriodDetail)
                {
                    sb.AppendLine("�`���u" + each.Period + "�v�]���u" + each.AbsenceType + "�v");
                }

                ApplicationLog.Log("�ǰȨt��.���m���", "�s�W�ǥͯ��m���", "student", _editor.Student.ID, sb.ToString());
                MsgBox.Show("�s�W�ǥͯ��m��Ʀ��\!");
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
                    MsgBox.Show("�ק���m��ƥ���!" + ex.Message);
                    return;
                }

                StringBuilder sb = new StringBuilder();
                sb.Append("�ǥ͡u" + _editor.Student.Name + "�v");
                sb.Append("����u" + _editor.OccurDate.ToShortDateString() + "�v");
                sb.AppendLine("���m��Ƥw�ק�C");
                sb.AppendLine("�ԲӸ�ơG");

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
                        sb.AppendLine("�`���u" + each.Name + "�v�ѡu" + DicBeforeLog[each.Name] + "�v�ܧ󬰡u" + DicAfterLog[each.Name] + "�v");
                        LogMode = true;
                    }
                }

                if (LogMode)
                {
                    ApplicationLog.Log("�ǰȨt��.���m���", "�ק�ǥͯ��m���", "student", _editor.Student.ID, sb.ToString());
                }

                MsgBox.Show("�ק�ǥͯ��m��Ʀ��\!");
                #endregion
            }
            this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #region ���������B�z
        private void dataGridViewX1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            //DataGridViewCell dgvCell = dataGridViewX1.Rows[e.RowIndex].Cells[e.ColumnIndex];
            //string dgvString = "" + dgvCell.Value;
            //dgvCell.Value = "";
            //if (!string.IsNullOrEmpty(dgvString))
            //{
            //    foreach (AbsenceMappingInfo absenceInfo in absenceList)
            //    {
            //        if (dgvString == absenceInfo.HotKey)
            //        {
            //            dgvCell.Value = absenceInfo.Abbreviation;
            //            break;
            //        }
            //    }
            //    dataGridViewX1.GoToNEXTCell();
            //}
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
                    //�p�G����,�N�M��
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


            //�p�G����J���(�p�G�O���ť���)
            if (string.IsNullOrEmpty(CurrentCellValue))
            {
                foreach (DataGridViewCell cell in dataGridViewX1.SelectedCells)
                {
                    cell.Value = ""; //�M�Ÿ��
                }
                dataGridViewX1.GoToNEXTCell();
            }
            else
            {
                //�p�G���O�ťո��,�N����Ӫ�
                string Abbreviation = "";
                foreach (AbsenceMappingInfo absenceInfo in absenceList)
                {
                    if (absenceInfo.HotKey.ToUpper() == CurrentCellValue.ToUpper())
                    {
                        Abbreviation = absenceInfo.Abbreviation;
                        break;
                    }
                }

                if (Abbreviation != "") //�����,�N�������Y�g
                {
                    foreach (DataGridViewCell cell in dataGridViewX1.SelectedCells)
                    {
                        cell.Value = Abbreviation;
                    }
                }
                else //�S��������,�N�M�Ÿ��
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

        #endregion

        private void dataGridViewX1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            foreach (DataGridViewRow row in dataGridViewX1.Rows)
            {
                row.Cells[e.ColumnIndex].Selected = true;
            }
        }

        #region ���ĪG�O����DataGridView�@�}�l�|����@�檺�ĪG

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