using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FISCA.DSAUtil;
using System.Xml;
using System.Globalization;
using DevComponents.DotNetBar;
using FISCA.LogAgent;
using K12.Data;

namespace K12.Behavior.AttendanceEdit
{
    public partial class AttendanceEditForm : FISCA.Presentation.Controls.BaseForm
    {
        //初始化
        BackgroundWorker InitializeBGW = new BackgroundWorker();
        //重新整理
        BackgroundWorker RefreshBGW = new BackgroundWorker();
        //儲存背景模式
        BackgroundWorker SaveBGW = new BackgroundWorker();

        //缺曠資料的可輸入起始定位
        int _startIndex;

        ChangeListen _ChangeListen = new ChangeListen();

        List<AttendanceRecord> _AttendanceRecordList = new List<AttendanceRecord>();

        List<StudentData> _StudentDataList = new List<StudentData>();

        Dictionary<string, StudentData> _StudentDataDic = new Dictionary<string, StudentData>();

        //log 需要用到的
        //<學生,<日期<節次,缺曠別>>
        //<學生ID,每一個Log記錄>
        private Dictionary<string, EditLog> LOG = new Dictionary<string, EditLog>();

        /// <summary>
        /// 班級名稱清單
        /// </summary>
        Dictionary<string, string> AllClassName = new Dictionary<string, string>();
        /// <summary>
        /// 學生相關資料取得
        /// </summary>
        StudentObj _studentObj = new StudentObj();

        //學生人數
        int studentCount = 0;

        /// <summary>
        /// 班級名稱資料
        /// </summary>
        List<string> classNameList = new List<string>();
        /// <summary>
        /// 年級名稱資料
        /// </summary>
        List<string> GradeYearList = new List<string>();

        bool 資料是否修改過 = false;

        /// <summary>
        /// 缺曠類別
        /// </summary>
        List<AbsenceMappingInfo> AbsenceList { get; set; }

        /// <summary>
        /// 缺曠類別HotKey->MappingInfo
        /// </summary>
        Dictionary<string, AbsenceMappingInfo> AbsenceHotKeyDic = new Dictionary<string, AbsenceMappingInfo>();

        /// <summary>
        /// 缺曠類別HotKey->MappingInfo
        /// </summary>
        Dictionary<string, AbsenceMappingInfo> AbsenceDic = new Dictionary<string, AbsenceMappingInfo>();

        /// <summary>
        /// 缺曠類別 - 熱鍵比對縮寫
        /// </summary>
        Dictionary<string, string> AbsenceByHotKey_Abb = new Dictionary<string, string>();

        /// <summary>
        /// 缺曠類別 - 名稱比對縮寫
        /// </summary>
        Dictionary<string, string> AbsenceByName_Abb = new Dictionary<string, string>();

        /// <summary>
        /// 缺曠類別 - 縮寫比對名稱
        /// </summary>
        Dictionary<string, string> AbsenceByAbb_Name = new Dictionary<string, string>();

        /// <summary>
        /// 取得每日節次
        /// </summary>
        List<PeriodMappingInfo> PeriodList { get; set; }

        DateTime _StartDate;
        DateTime _EndDate;

        /// <summary>
        /// 節次清單
        /// </summary>
        List<string> _PeriodNameList = new List<string>();

        private Dictionary<string, AttendanceRecord> AttendanceID = new Dictionary<string, AttendanceRecord>();

        Dictionary<string, int> P_index = new Dictionary<string, int>();

        List<StudentData> studentData = new List<StudentData>();

        //建構子
        public AttendanceEditForm()
        {
            InitializeComponent();
        }

        private void AttendanceEditForm_Load(object sender, EventArgs e)
        {
            List<string> cols = new List<string>() { "日期", "學年度", "學期" };
            Campus.Windows.DataGridViewImeDecorator dec = new Campus.Windows.DataGridViewImeDecorator(this.dataGridViewX1, cols);

            //起始 - 背景模式
            InitializeBGW.DoWork += new DoWorkEventHandler(InitializeBGW_DoWork);
            InitializeBGW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(InitializeBGW_RunWorkerCompleted);
            //更新 - 背景模式
            RefreshBGW.DoWork += new DoWorkEventHandler(RefreshBGW_DoWork);
            RefreshBGW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RefreshBGW_RunWorkerCompleted);
            //儲存背景模式
            SaveBGW.DoWork += new DoWorkEventHandler(SaveBGW_DoWork);
            SaveBGW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(SaveBGW_RunWorkerCompleted);

            _ChangeListen = new ChangeListen();
            _ChangeListen.Add(new DataGridViewSource(dataGridViewX1));
            _ChangeListen.StatusChanged += new EventHandler<ChangeEventArgs>(_ChangeListen_StatusChanged);

            //開始畫面初始化
            InitializeDoWork();
        }

        /// <summary>
        /// 開始更新畫面資料
        /// 這個背景模式理論上來說,只會執行一次
        /// </summary>
        private void InitializeDoWork()
        {
            FormPause = false;
            if (!InitializeBGW.IsBusy) //如果不是忙碌中...
            {
                InitializeBGW.RunWorkerAsync();
            }
            else
            {
                FormPause = true;
                FISCA.Presentation.Controls.MsgBox.Show("系統忙碌中,請稍後再試!!");
            }
        }

        void InitializeBGW_DoWork(object sender, DoWorkEventArgs e)
        {
            //取得缺曠類別
            AbsenceByHotKey_Abb.Clear();
            AbsenceByName_Abb.Clear();
            AbsenceByAbb_Name.Clear();
            AbsenceHotKeyDic.Clear();
            AbsenceDic.Clear();
            AbsenceList = AbsenceMapping.SelectAll();
            foreach (AbsenceMappingInfo each in AbsenceList)
            {
                if (!AbsenceByHotKey_Abb.ContainsKey(each.HotKey))
                    AbsenceByHotKey_Abb.Add(each.HotKey, each.Abbreviation);

                if (!AbsenceByName_Abb.ContainsKey(each.Name))
                    AbsenceByName_Abb.Add(each.Name, each.Abbreviation);
                //通常只有儲存時會使用
                if (!AbsenceByAbb_Name.ContainsKey(each.Abbreviation))
                    AbsenceByAbb_Name.Add(each.Abbreviation, each.Name);

                if (!AbsenceHotKeyDic.ContainsKey(each.HotKey))
                    AbsenceHotKeyDic.Add(each.HotKey.ToUpper(), each);

                if (!AbsenceDic.ContainsKey(each.Abbreviation))
                    AbsenceDic.Add(each.Abbreviation, each);
            }


            //取得節次對照表
            PeriodList = PeriodMapping.SelectAll();
            PeriodList.Sort(SingleEditorMethod.SortByOrder);
            _PeriodNameList.Clear();
            foreach (K12.Data.PeriodMappingInfo each in PeriodList)
            {
                if (!_PeriodNameList.Contains(each.Name))
                {
                    _PeriodNameList.Add(each.Name);
                }

            }
            //取得班級清單
            classNameList = _studentObj.GetClassNameList();
            //取得年級清單
            GradeYearList = _studentObj.GetGradeYearList();
            //取得班級ID比對名稱字典
            AllClassName = _studentObj.GetAllClassName();
        }

        void InitializeBGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //當沒有錯誤時
            if (e.Error == null)
            {
                //設定假別選項
                SetupAbsenceCheckBox();
                //設定Column
                SetupPeriodColumn();
                //設定ComboBox內容
                SetupDateAndComboBox();
            }
            else //發生錯誤時
            {
                FISCA.Presentation.Controls.MsgBox.Show("背景模式取得資料,發生錯誤!!\n" + e.Error.Message);
                SmartSchool.ErrorReporting.ReportingService.ReportException(e.Error);
            }
            FormPause = true;
            cbRange.SelectedIndex = 0;
        }

        /// <summary>
        /// 填入假別CheckBox
        /// </summary>
        private void SetupAbsenceCheckBox()
        {
            foreach (AbsenceMappingInfo each in AbsenceList)
            {
                CheckBox cbBox1 = new CheckBox();
                cbBox1.Text = each.Abbreviation + "(" + each.HotKey + ")";
                cbBox1.ForeColor = Color.FromArgb(22, 66, 113);
                cbBox1.Checked = true;
                cbBox1.Size = new Size(80, 23);
                cbBox1.Tag = each;
                cpAtt.Controls.Add(cbBox1);
            }
        }

        /// <summary>
        /// 於畫面上建立Column
        /// </summary>
        private void SetupPeriodColumn()
        {
            int columnIndex = dataGridViewX1.Columns.Add(SingleEditorMethod.SetColumn(dataGridViewX1, "colAttendanceID", "AttendanceID", true, true, DataGridViewAutoSizeColumnMode.DisplayedCells, Color.LightCyan));
            P_index.Add("AttendanceID", columnIndex);
            dataGridViewX1.Columns[columnIndex].Visible = false; //隱藏

            columnIndex = dataGridViewX1.Columns.Add(SingleEditorMethod.SetColumn(dataGridViewX1, "colClassName", "班級", true, true, DataGridViewAutoSizeColumnMode.AllCells, Color.LightCyan));
            P_index.Add("班級", columnIndex);

            columnIndex = dataGridViewX1.Columns.Add(SingleEditorMethod.SetColumn(dataGridViewX1, "colSeatNo", "座號", true, true, DataGridViewAutoSizeColumnMode.DisplayedCells, Color.LightCyan));
            P_index.Add("座號", columnIndex);

            columnIndex = dataGridViewX1.Columns.Add(SingleEditorMethod.SetColumn(dataGridViewX1, "colStudentName", "姓名", true, true, DataGridViewAutoSizeColumnMode.DisplayedCells, Color.LightCyan));
            P_index.Add("姓名", columnIndex);

            columnIndex = dataGridViewX1.Columns.Add(SingleEditorMethod.SetColumn(dataGridViewX1, "colDateTime", "日期", false, false, DataGridViewAutoSizeColumnMode.AllCells, Color.White));
            P_index.Add("日期", columnIndex);
            dataGridViewX1.Columns[columnIndex].Frozen = true; //凍結欄位

            columnIndex = dataGridViewX1.Columns.Add(SingleEditorMethod.SetColumn(dataGridViewX1, "colWeekInChinese", "星期", true, true, DataGridViewAutoSizeColumnMode.DisplayedCells, Color.LightCyan));
            P_index.Add("星期", columnIndex);

            columnIndex = dataGridViewX1.Columns.Add(SingleEditorMethod.SetColumn(dataGridViewX1, "colStudentNumber", "學號", true, true, DataGridViewAutoSizeColumnMode.DisplayedCells, Color.LightCyan));
            P_index.Add("學號", columnIndex);

            columnIndex = dataGridViewX1.Columns.Add(SingleEditorMethod.SetColumn(dataGridViewX1, "colGender", "性別", true, true, DataGridViewAutoSizeColumnMode.DisplayedCells, Color.LightCyan));
            P_index.Add("性別", columnIndex);

            columnIndex = dataGridViewX1.Columns.Add(SingleEditorMethod.SetColumn(dataGridViewX1, "SchoolYear", "學年度", false, true, DataGridViewAutoSizeColumnMode.DisplayedCells, Color.White));
            P_index.Add("學年度", columnIndex);

            columnIndex = dataGridViewX1.Columns.Add(SingleEditorMethod.SetColumn(dataGridViewX1, "Semester", "學期", true, true, DataGridViewAutoSizeColumnMode.DisplayedCells, Color.White));
            P_index.Add("學期", columnIndex);
            _startIndex = P_index["學期"] + 1;

            foreach (PeriodMappingInfo each in PeriodList)
            {
                columnIndex = dataGridViewX1.Columns.Add(SingleEditorMethod.SetColumn(dataGridViewX1, each.Name, each.Name, true, true, DataGridViewAutoSizeColumnMode.DisplayedCells, Color.White));
                dataGridViewX1.Columns[columnIndex].Tag = each;
                P_index.Add(each.Name, columnIndex);
            }
        }

        /// <summary>
        /// 設定ComboBox/開始日期/結束日期
        /// </summary>
        private void SetupDateAndComboBox()
        {
            //開始日期 
            _StartDate = dateTimeInput1.Value = DateTime.Today.AddDays(-7);
            //結束日期
            _EndDate = dateTimeInput2.Value = DateTime.Today;
            //班級
            cbClass.Items.AddRange(classNameList.ToArray());
            //年級
            cbGradeYear.Items.AddRange(GradeYearList.ToArray());
        }

        /// <summary>
        /// 重新整理鈕
        /// </summary>
        private void btnRefresh_Click_1(object sender, EventArgs e)
        {
            if (資料是否修改過)
            {
                if (FISCA.Presentation.Controls.MsgBox.Show("資料已變更尚未儲存，確認重新整理?", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return;
                }
            }

            RefreshDoWork();
        }

        /// <summary>
        /// 開始取得畫面資料
        /// 條件:開始日期/結束日期/學生範圍/缺曠別
        /// </summary>
        public void RefreshDoWork()
        {
            FormPause = false;
            if (!RefreshBGW.IsBusy)
            {
                #region 背景模式

                _StartDate = dateTimeInput1.Value;
                _EndDate = dateTimeInput2.Value;

                Dictionary<string, string> dic = new Dictionary<string, string>();

                //===取得學生之條件式===
                //0是全校 - 僅使用日期條件
                //1是年級 - 使用年級條件(cbGradeYear)
                //2是班級 - 使用班級條件(cbClass)
                //3是學號 - 使用學號條件(txtSeatNo)
                //4是班座 - 使用班級座號條件(cbClass,txtSeatNo)
                if (cbRange.SelectedIndex == 0)
                {
                    dic.Add("Mode", "0");
                }
                else if (cbRange.SelectedIndex == 1)
                {
                    dic.Add("Mode", "1");
                    dic.Add("年級", cbGradeYear.Text);
                }
                else if (cbRange.SelectedIndex == 2)
                {
                    dic.Add("Mode", "2");
                    dic.Add("班級", cbClass.Text);
                }
                else if (cbRange.SelectedIndex == 3)
                {
                    if (string.IsNullOrEmpty(txtClass.Text))
                    {
                        FormPause = true;
                        FISCA.Presentation.Controls.MsgBox.Show("請輸入學號!!");
                        return;
                    }

                    dic.Add("Mode", "3");
                    dic.Add("學號", txtClass.Text);
                }
                else if (cbRange.SelectedIndex == 4)
                {
                    if (string.IsNullOrEmpty(txtSeatNo.Text))
                    {
                        FormPause = true;
                        FISCA.Presentation.Controls.MsgBox.Show("請輸入座號!!");
                        return;
                    }

                    dic.Add("Mode", "4");
                    dic.Add("班級", cbClass.Text);
                    dic.Add("座號", txtSeatNo.Text);
                }
                RefreshBGW.RunWorkerAsync(dic);

                #endregion
            }
            else
            {
                FormPause = true;
                FISCA.Presentation.Controls.MsgBox.Show("系統忙碌中,請稍後再試!!");
            }
        }

        void RefreshBGW_DoWork(object sender, DoWorkEventArgs e)
        {
            _ChangeListen.SuspendListen(); //終止變更判斷
            _AttendanceRecordList.Clear();
            _StudentDataList.Clear();
            _StudentDataDic.Clear();
            Dictionary<string, string> Dic = (Dictionary<string, string>)e.Argument;

            #region 條件式

            if (Dic["Mode"] == "0") //0是全校 - 僅使用日期條件
            {
                _StudentDataList = _studentObj.GetAllStudent();
                _AttendanceRecordList = K12.Data.Attendance.SelectByDate(_StartDate, _EndDate);
            }
            else if (Dic["Mode"] == "1") //1是年級 - 使用年級條件(cbGradeYear)
            {
                _StudentDataList = _studentObj.GetGradeYearStudent(Dic["年級"]);
                List<string> studentIDList = new List<string>();
                foreach (StudentData each in _StudentDataList)
                {
                    if (!studentIDList.Contains(each.ref_student_id))
                        studentIDList.Add(each.ref_student_id);
                }
                List<K12.Data.StudentRecord> studentList = Student.SelectByIDs(studentIDList);
                if (studentList.Count > 0)
                    _AttendanceRecordList = K12.Data.Attendance.SelectByDate(studentList, _StartDate, _EndDate);


            }
            else if (Dic["Mode"] == "2") //2是班級 - 使用班級條件(cbClass)
            {
                _StudentDataList = _studentObj.GetClassNameStudent(Dic["班級"]);
                List<string> studentIDList = new List<string>();
                foreach (StudentData each in _StudentDataList)
                {
                    if (!studentIDList.Contains(each.ref_student_id))
                        studentIDList.Add(each.ref_student_id);
                }
                List<K12.Data.StudentRecord> studentList = Student.SelectByIDs(studentIDList);
                if (studentList.Count > 0)
                    _AttendanceRecordList = K12.Data.Attendance.SelectByDate(studentList, _StartDate, _EndDate);
            }
            else if (Dic["Mode"] == "3") //3是學號 - 使用學號條件(txtSeatNo)
            {
                _StudentDataList = _studentObj.GetStudentNumber(Dic["學號"]);
                List<string> studentIDList = new List<string>();
                foreach (StudentData each in _StudentDataList)
                {
                    if (!studentIDList.Contains(each.ref_student_id))
                        studentIDList.Add(each.ref_student_id);
                }
                List<K12.Data.StudentRecord> studentList = Student.SelectByIDs(studentIDList);
                if (studentList.Count > 0)
                    _AttendanceRecordList = K12.Data.Attendance.SelectByDate(studentList, _StartDate, _EndDate);


            }
            else if (Dic["Mode"] == "4") //4是班座 - 使用班級座號條件(cbClass,txtSeatNo)
            {
                _StudentDataList = _studentObj.GetClassNameStudent(Dic["班級"], Dic["座號"]);
                List<string> studentIDList = new List<string>();
                foreach (StudentData each in _StudentDataList)
                {
                    if (!studentIDList.Contains(each.ref_student_id))
                        studentIDList.Add(each.ref_student_id);
                }
                List<K12.Data.StudentRecord> studentList = Student.SelectByIDs(studentIDList);
                if (studentList.Count > 0)
                    _AttendanceRecordList = K12.Data.Attendance.SelectByDate(studentList, _StartDate, _EndDate);
            }

            foreach (StudentData each in _StudentDataList)
            {
                if (!_StudentDataDic.ContainsKey(each.ref_student_id))
                {
                    _StudentDataDic.Add(each.ref_student_id, each);
                }
            }

            #endregion
        }

        void RefreshBGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                if (_AttendanceRecordList.Count == 0)
                {
                    dataGridViewX1.Rows.Clear();
                    FormPause = true;
                    FISCA.Presentation.Controls.MsgBox.Show("查無缺曠資料!");
                    return;
                }
                FillDataGridView();
            }
            else
            {
                FISCA.Presentation.Controls.MsgBox.Show("背景模式取得資料,發生錯誤!!\n" + e.Error.Message);
                SmartSchool.ErrorReporting.ReportingService.ReportException(e.Error);
            }
            _ChangeListen.Reset();
            _ChangeListen.ResumeListen();
            資料是否修改過 = false;
            FormPause = true;
            this.Text = "缺曠批次修改(學生人數：" + studentCount + ")";
        }

        /// <summary>
        /// 更新畫面資料
        /// </summary>
        private void FillDataGridView()
        {
            dataGridViewX1.Rows.Clear();
            dataGridViewX1.SuspendLayout();

            List<string> jone = new List<string>();

            foreach (CheckBox each in cpAtt.Controls)
            {
                if (each.Checked)
                {
                    AbsenceMappingInfo amf = (AbsenceMappingInfo)each.Tag;
                    jone.Add(amf.Abbreviation); //清除"("括號之後的內容
                }
            }

            _AttendanceRecordList.Sort(SortByClassAndSeatNo);

            List<string> StudentCount = new List<string>();

            //Log
            LOG.Clear();
            foreach (AttendanceRecord each in _AttendanceRecordList)
            {
                if (!LOG.ContainsKey(each.RefStudentID))
                {
                    LOG.Add(each.RefStudentID, new EditLog());
                }
            }

            foreach (AttendanceRecord att in _AttendanceRecordList)
            {
                bool InPort = true;
                foreach (K12.Data.AttendancePeriod AttP in att.PeriodDetail)
                {
                    if (AbsenceByName_Abb.ContainsKey(AttP.AbsenceType))
                    {
                        if (jone.Contains(AbsenceByName_Abb[AttP.AbsenceType])) //只要有一個內容包含於清單中
                        {
                            InPort = false;
                            break;
                        }
                    }
                }

                if (InPort)
                    continue;



                //log 紀錄修改前的資料
                if (!LOG[att.RefStudentID].beforeData.ContainsKey(att.ID))
                {
                    AttendanceRecord ar = new AttendanceRecord();
                    ar.OccurDate = att.OccurDate;
                    ar.SchoolYear = att.SchoolYear;
                    ar.Semester = att.Semester;
                    foreach (AttendancePeriod each in att.PeriodDetail)
                    {
                        ar.PeriodDetail.Add(each);
                    }
                    LOG[att.RefStudentID].beforeData.Add(att.ID, ar);
                }

                List<string> values = new List<string>();
                StudentData _student = _StudentDataDic[att.RefStudentID];

                if (!StudentCount.Contains(att.RefStudentID))
                {
                    StudentCount.Add(att.RefStudentID); //學生人數統計
                }

                DataGridViewRow _row = new DataGridViewRow();
                _row.CreateCells(dataGridViewX1);
                _row.Tag = att;

                _row.Cells[P_index["AttendanceID"]].Value = att.RefStudentID;

                if (_student.ref_class_id != "")
                {
                    _row.Cells[P_index["班級"]].Value = AllClassName[_student.ref_class_id];
                }
                _row.Cells[P_index["座號"]].Value = _student.seat_no;
                _row.Cells[P_index["姓名"]].Value = _student.name;

                _row.Cells[P_index["日期"]].Value = att.OccurDate.ToShortDateString();

                _row.Cells[P_index["星期"]].Value = SingleEditorMethod.GetDayOfWeekInChinese(att.OccurDate.DayOfWeek);


                _row.Cells[P_index["學號"]].Value = _student.student_number;

                _row.Cells[P_index["性別"]].Value = _student.gender == "1" ? "男" : "女";

                _row.Cells[P_index["學年度"]].Value = att.SchoolYear;
                _row.Cells[P_index["學期"]].Value = att.Semester;

                foreach (K12.Data.AttendancePeriod AttPeriod in att.PeriodDetail)
                {
                    if (_PeriodNameList.Contains(AttPeriod.Period))
                    {
                        if (AbsenceByName_Abb.ContainsKey(AttPeriod.AbsenceType))
                        {
                            //判斷此假別為使用者所選假別?
                            //if(jone.Contains(AbsenceByName_Abb[AttPeriod.AbsenceType]))
                            _row.Cells[P_index[AttPeriod.Period]].Value = AbsenceByName_Abb[AttPeriod.AbsenceType];
                            _row.Cells[P_index[AttPeriod.Period]].Tag = AbsenceDic[AbsenceByName_Abb[AttPeriod.AbsenceType]];
                        }
                    }
                }
                dataGridViewX1.Rows.Add(_row);
            }

            studentCount = StudentCount.Count;
            //cbRange.SelectedIndex = 0;
            dataGridViewX1.ResumeLayout();
        }

        #region Save

        private void btnSave_Click(object sender, EventArgs e)
        {
            FormPause = false; //就把畫面鎖定
            if (!IsValid())
            {
                FormPause = true;
                FISCA.Presentation.Controls.MsgBox.Show("資料驗證失敗，請修正後再行儲存", "驗證失敗", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            helper helper = new helper();

            foreach (DataGridViewRow row in dataGridViewX1.Rows)
            {
                AttendanceRecord attRecord = (AttendanceRecord)row.Tag;
                //如果該日期資料不等於原日期內容時

                #region 一般修改資料
                if (attRecord.OccurDate != DateTime.Parse("" + row.Cells[P_index["日期"]].Value))
                {
                    attRecord.OccurDate = DateTime.Parse("" + row.Cells[P_index["日期"]].Value);
                }

                attRecord.PeriodDetail.Clear();

                bool hasContent = false;
                for (int i = _startIndex; i < dataGridViewX1.Columns.Count; i++)
                {
                    DataGridViewCell cell = row.Cells[i];
                    if (string.IsNullOrEmpty(("" + cell.Value).Trim())) continue;

                    PeriodMappingInfo pinfo = dataGridViewX1.Columns[i].Tag as PeriodMappingInfo;
                    AbsenceMappingInfo acInfo = cell.Tag as AbsenceMappingInfo;

                    K12.Data.AttendancePeriod ap = new K12.Data.AttendancePeriod();
                    ap.Period = pinfo.Name;
                    ap.AbsenceType = acInfo.Name;
                    attRecord.PeriodDetail.Add(ap);

                    hasContent = true;
                }

                if (!LOG[attRecord.RefStudentID].afterData.ContainsKey(attRecord.ID))
                {
                    LOG[attRecord.RefStudentID].afterData.Add(attRecord.ID, attRecord);
                }

                if (hasContent)
                {
                    attRecord.SchoolYear = int.Parse("" + row.Cells[P_index["學年度"]].Value);
                    attRecord.Semester = int.Parse("" + row.Cells[P_index["學期"]].Value);
                    helper.updateHelper.Add(attRecord);
                }
                else
                {
                    helper.deleteList.Add(attRecord.ID);

                    //log 紀錄被刪除的資料
                    LOG[attRecord.RefStudentID].afterData.Remove(attRecord.ID);
                    LOG[attRecord.RefStudentID].deleteData.Add(attRecord.ID, attRecord);
                }
                #endregion

            }

            //開始儲存資料
            SaveBGW.RunWorkerAsync(helper);
        }


        /// <summary>
        /// DataGridView資料驗證(如果ErrorText內容為空)
        /// </summary>
        private bool IsValid()
        {
            foreach (DataGridViewRow row in dataGridViewX1.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.ErrorText != string.Empty)
                        return false;
                }
            }
            return true;
        }

        void SaveBGW_DoWork(object sender, DoWorkEventArgs e)
        {
            helper obj = (helper)e.Argument;

            #region InsertHelper(註解)
            //if (obj.InsertHelper.Count > 0)
            //{
            //    try
            //    {
            //        K12.Data.Attendance.Insert(obj.InsertHelper);
            //    }
            //    catch (Exception ex)
            //    {
            //        FISCA.Presentation.Controls.MsgBox.Show("缺曠紀錄新增失敗 : " + ex.Message, "新增失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        SmartSchool.ErrorReporting.ReportingService.ReportException(ex);
            //    }

            //    #region log 寫入log
            //    foreach (string each in LOG.Keys)
            //    {
            //        foreach (string date in LOG[each].afterData.Keys)
            //        {
            //            if (!LOG[each].beforeData.ContainsKey(date) && LOG[each].afterData[date].Count > 0)
            //            {
            //                StringBuilder desc = new StringBuilder("");
            //                desc.AppendLine("已進行「新增缺曠」動作!");
            //                desc.AppendLine("學生「" + K12.Data.Student.SelectByID(each).Name + "」");
            //                desc.AppendLine("日期「" + date + "」");
            //                foreach (string period in LOG[each].afterData[date].Keys)
            //                {
            //                    desc.AppendLine("節次「" + period + "」設為「" + LOG[each].afterData[date][period] + "」");
            //                }
            //                ApplicationLog.Log("缺曠批次修改", "新增", "student", each, desc.ToString());
            //                //Log部份
            //                //CurrentUser.Instance.AppLog.Write(EntityType.Student, EntityAction.Insert, _student.ID, desc.ToString(), this.Text, "");
            //            }
            //        }
            //    }
            //    #endregion
            //}
            #endregion

            #region updateHelper
            if (obj.updateHelper.Count > 0)
            {
                try
                {
                    //為何缺曠更新會發生錯誤訊息
                    K12.Data.Attendance.Update(obj.updateHelper);
                }
                catch (Exception ex)
                {
                    FISCA.Presentation.Controls.MsgBox.Show("缺曠紀錄更新失敗 : " + ex.Message, "更新失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    SmartSchool.ErrorReporting.ReportingService.ReportException(ex);
                    return;
                }

                #region log 寫入log
                foreach (string each in LOG.Keys) //each = 取出每一名學生
                {
                    foreach (string date in LOG[each].afterData.Keys) //date = 缺曠ID
                    {
                        if (LOG[each].beforeData.ContainsKey(date))
                        {
                            AttendanceRecord ar1 = LOG[each].beforeData[date];
                            AttendanceRecord ar2 = LOG[each].afterData[date];

                            EditBot bot = new EditBot(ar1, ar2, _StudentDataDic[ar2.RefStudentID]);
                            bot.CheckChange(); //開始判斷
                            if (!string.IsNullOrEmpty(bot.sb.ToString())) //如果資料不是空的,表示有資料更新
                            {
                                ApplicationLog.Log("缺曠批次修改", "修改", "student", ar2.RefStudentID, bot.sb.ToString());
                            }
                        }
                    }
                }
                #endregion
            }
            #endregion

            #region deleteList
            if (obj.deleteList.Count > 0)
            {
                try
                {
                    K12.Data.Attendance.Delete(obj.deleteList);
                }
                catch (Exception ex)
                {
                    FISCA.Presentation.Controls.MsgBox.Show("缺曠紀錄刪除失敗 : " + ex.Message, "刪除失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    SmartSchool.ErrorReporting.ReportingService.ReportException(ex);
                }

                #region log 寫入被刪除的資料的log
                foreach (string each in LOG.Keys)
                {
                    if (LOG[each].deleteData.Count == 0)
                        continue;

                    StringBuilder desc = new StringBuilder("");
                    desc.AppendLine("已進行「刪除缺曠」動作!");
                    desc.Append("班級「" + _StudentDataDic[each].class_name + "」");
                    desc.Append("座號「" + _StudentDataDic[each].seat_no + "」");
                    desc.Append("學號「" + _StudentDataDic[each].student_number + "」");
                    desc.AppendLine("姓名「" + _StudentDataDic[each].name + "」");
                    foreach (string date in LOG[each].deleteData.Keys)
                    {
                        AttendanceRecord ar1 = LOG[each].deleteData[date];
                        desc.AppendLine("刪除「" + ar1.OccurDate.ToShortDateString() + "」缺曠紀錄 ");
                    }

                    //Log部份
                    ApplicationLog.Log("缺曠批次修改", "刪除", "student", each, desc.ToString());
                    //CurrentUser.Instance.AppLog.Write(EntityType.Student, EntityAction.Delete, _student.ID, desc.ToString(), this.Text, "");
                }
                #endregion
            }
            #endregion


        }


        void SaveBGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                FISCA.Presentation.Controls.MsgBox.Show("儲存缺曠資料成功!", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshDoWork();
            }
            else
            {
                FISCA.Presentation.Controls.MsgBox.Show("缺曠紀錄儲存失敗 : " + e.Error.Message, "儲存失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            _ChangeListen.Reset();

        }

        #endregion

        /// <summary>
        /// 系統被修改事件
        /// </summary>
        void _ChangeListen_StatusChanged(object sender, ChangeEventArgs e)
        {
            資料是否修改過 = (e.Status == ValueStatus.Dirty);

            if (資料是否修改過)
            {
                pictureBox1.Image = Properties.Resources.clipboard_info_64;
            }
            else
            {
                pictureBox1.Image = Properties.Resources.clipboard_64;
            }
        }

        /// <summary>
        /// 使用者選擇之範圍條件變更時...
        /// </summary>
        private void cbRange_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbRange.SelectedIndex == 0) //全校
            {
                cbGradeYear.Visible = false;

                txtClass.Visible = false;
                txtClass.Text = "";
                cbClass.Visible = false;
                lbClass.Visible = false;

                lbSeatNo.Visible = false;
                txtSeatNo.Visible = false;
                txtSeatNo.Text = "";
            }
            else if (cbRange.SelectedIndex == 1) //年級
            {
                lbClass.Text = "年級";
                lbClass.Visible = true;
                cbGradeYear.Visible = true;

                if (cbGradeYear.Items.Count != 0)
                    cbGradeYear.SelectedIndex = 0;

                txtClass.Visible = false;
                txtClass.Text = "";
                cbClass.Visible = false;

                lbSeatNo.Visible = false;
                txtSeatNo.Visible = false;
                txtSeatNo.Text = "";

            }
            else if (cbRange.SelectedIndex == 2) //班級
            {
                lbClass.Text = "班級";
                lbClass.Visible = true;
                cbClass.Visible = true; //顯示班級控制
                if (cbClass.Items.Count != 0)
                    cbClass.SelectedIndex = 0;

                cbGradeYear.Visible = false; //隱藏年級

                txtClass.Visible = false; //隱藏班級TXT
                txtClass.Text = ""; //清空班級TXT

                lbSeatNo.Visible = false; //隱藏座號lb
                txtSeatNo.Visible = false; //隱藏座號控制
                txtSeatNo.Text = ""; //清空座號內容
            }
            else if (cbRange.SelectedIndex == 3) //學號
            {
                lbClass.Text = "學號";
                lbClass.Visible = true;

                txtClass.Visible = true;
                txtClass.Text = "";

                cbClass.Visible = false;

                lbSeatNo.Visible = false;
                txtSeatNo.Visible = false;
                txtSeatNo.Text = "";
            }
            else if (cbRange.SelectedIndex == 4) //班級座號
            {
                lbClass.Text = "班級"; //更改班級lb
                lbClass.Visible = true; //顯示班級lb
                cbClass.Visible = true; //顯示班級控制

                txtSeatNo.Text = "";
                lbSeatNo.Visible = true; //顯示座號文字
                txtSeatNo.Visible = true; //顯示控制

                cbGradeYear.Visible = false;

                txtClass.Visible = false;
                txtClass.Text = "";

            }
        }

        /// <summary>
        /// 全選CheckBox
        /// </summary>
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            foreach (CheckBox each in cpAtt.Controls)
            {
                each.Checked = checkBox1.Checked;
            }
        }

        #region 加入待處理
        private void btnAdd_Click(object sender, EventArgs e)
        {
            List<string> TemporalStud = GetSelectedStudentList();

            K12.Presentation.NLDPanels.Student.AddToTemp(TemporalStud);

            FISCA.Presentation.Controls.MsgBox.Show("新增 " + TemporalStud.Count + " 名學生於待處理");
            labelX3.Text = "待處理共 " + K12.Presentation.NLDPanels.Student.TempSource.Count + " 名學生";
            labelX3.Visible = true;
            btnClear.Visible = true;
        }

        /// <summary>
        /// 取得目前選擇的Row,內存的學生
        /// </summary>
        /// <returns></returns>
        private List<string> GetSelectedStudentList()
        {
            List<string> _temporallist = new List<string>();
            foreach (DataGridViewCell cell in dataGridViewX1.SelectedCells)
            {
                string studentID = "" + dataGridViewX1.Rows[cell.RowIndex].Cells[0].Value;

                if (!_temporallist.Contains(studentID))
                {
                    _temporallist.Add(studentID);
                }
            }
            return _temporallist;
        }
        #endregion

        /// <summary>
        /// 清空待處理
        /// </summary>
        private void btnClear_Click(object sender, EventArgs e)
        {
            K12.Presentation.NLDPanels.Student.RemoveFromTemp(K12.Presentation.NLDPanels.Student.TempSource);
            FISCA.Presentation.Controls.MsgBox.Show("已清除 待處理 所有學生");

            labelX3.Visible = false;
            btnClear.Visible = false;
        }

        #region 右鍵刪除功能
        private void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult DR = new DialogResult();
            DR = FISCA.Presentation.Controls.MsgBox.Show("是否刪除選擇之缺曠資料?\n共" + AttendanceID.Count + "筆\n\n(當日缺曠資料將全數刪除\n因假別未勾選而未顯示之內容\n亦將被同時刪除)", MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2);
            if (DR == DialogResult.Yes)
            {
                try
                {
                    K12.Data.Attendance.Delete(AttendanceID.Keys);
                }
                catch
                {
                    FISCA.Presentation.Controls.MsgBox.Show("刪除發生錯誤");
                    return;
                }

                //整理
                Dictionary<string, List<AttendanceRecord>> dic = new Dictionary<string, List<AttendanceRecord>>();

                foreach (AttendanceRecord Record in AttendanceID.Values)
                {
                    if (!dic.ContainsKey(Record.RefStudentID))
                    {
                        dic.Add(Record.RefStudentID, new List<AttendanceRecord>());
                    }
                    dic[Record.RefStudentID].Add(Record);
                }
                //收集
                foreach (string value in dic.Keys)
                {
                    StringBuilder desc = new StringBuilder("");
                    desc.AppendLine("已進行「刪除缺曠」動作!");
                    desc.Append("班級「" + _StudentDataDic[value].class_name + "」");
                    desc.Append("座號「" + _StudentDataDic[value].seat_no + "」");
                    desc.Append("學號「" + _StudentDataDic[value].student_number + "」");
                    desc.AppendLine("姓名「" + _StudentDataDic[value].name + "」");

                    foreach (AttendanceRecord Record in dic[value])
                    {
                        desc.AppendLine("刪除「" + Record.OccurDate.ToShortDateString() + "」缺曠紀錄 ");
                    }
                    //Log
                    ApplicationLog.Log("缺曠批次修改", "刪除", "student", value, desc.ToString());
                }

                //ApplicationLog.Log("缺曠批次修改", "刪除", "已將選取的「" + AttendanceID.Count + "」筆缺曠資料刪除。");
                FISCA.Presentation.Controls.MsgBox.Show("刪除成功!!");
                資料是否修改過 = false;
                btnRefresh_Click_1(null, null);
            }
        }

        //處理選擇多少資料
        private void dataGridViewX1_SelectionChanged(object sender, EventArgs e)
        {
            AttendanceID.Clear();

            foreach (DataGridViewCell cell in dataGridViewX1.SelectedCells)
            {
                DataGridViewRow row = cell.OwningRow;
                if (row.Tag != null)
                {
                    AttendanceRecord att = (AttendanceRecord)row.Tag;
                    if (!AttendanceID.ContainsKey(att.ID))
                    {
                        AttendanceID.Add(att.ID, att);
                    }
                }
            }
        }
        #endregion

        private void 加入待處理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnAdd_Click(null, null);
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            #region 匯出
            if (saveFileDialog1.ShowDialog() != DialogResult.OK) return;

            DataGridViewExport export = new DataGridViewExport(dataGridViewX1);
            export.Save(saveFileDialog1.FileName);
            ApplicationLog.Log("學務系統.缺曠資料檢視", "匯出缺曠內容", "缺曠資料檢視，已將缺曠查詢內容匯出。");

            if (new CompleteForm().ShowDialog() == DialogResult.Yes)
                System.Diagnostics.Process.Start(saveFileDialog1.FileName);
            #endregion
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private int SortByClassAndSeatNo(AttendanceRecord attendX, AttendanceRecord attendY)
        {
            StudentRecord x = attendX.Student;
            StudentRecord y = attendY.Student;
            string 班級名稱1 = (x.Class == null ? "" : x.Class.Name) + "::";
            string 座號1 = (x.SeatNo.HasValue ? x.SeatNo.Value.ToString().PadLeft(2, '0') : "") + "::";
            string 班級名稱2 = (y.Class == null ? "" : y.Class.Name) + "::";
            string 座號2 = (y.SeatNo.HasValue ? y.SeatNo.Value.ToString().PadLeft(2, '0') : "") + "::";

            班級名稱1 += 座號1;
            班級名稱2 += 座號2;
            if (attendX.OccurDate > attendY.OccurDate)
                班級名稱1 += "3";
            else if (attendX.OccurDate == attendY.OccurDate)
                班級名稱1 += "2";
            else
                班級名稱1 += "1";

            if (attendY.OccurDate > attendX.OccurDate)
                班級名稱2 += "3";
            else if (attendY.OccurDate == attendX.OccurDate)
                班級名稱2 += "2";
            else
                班級名稱2 += "1";

            return 班級名稱1.CompareTo(班級名稱2);
        }

        /// <summary>
        /// 畫面資料更新,可[開放]或[鎖定]部份元件
        /// </summary>
        bool FormPause
        {
            set
            {
                if (value)
                    this.Text = "缺曠批次修改";
                else
                    this.Text = "資料處理中...";

                groupPanel1.Enabled = value;
                groupPanel3.Enabled = value;
                btnRefresh.Enabled = value;
                btnExport.Enabled = value;
                btnAdd.Enabled = value;
                btnClear.Enabled = value;
                btnSave.Enabled = value;
            }
        }

        private void dataGridViewX1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = dataGridViewX1.Rows[e.RowIndex].Cells[e.ColumnIndex];
            if (e.ColumnIndex == P_index["學年度"])
            {
                string errorMessage = "";
                int schoolYear;
                if (cell.Value == null)
                    errorMessage = "學年度不可為空白";
                else if (!int.TryParse(cell.Value.ToString(), out schoolYear))
                    errorMessage = "學年度必須為整數";

                if (errorMessage != "")
                {
                    cell.Style.BackColor = Color.Red;
                    cell.ErrorText = errorMessage;
                }
                else
                {
                    if ("" + cell.Value != "")
                    {
                        cell.Style.BackColor = Color.Violet;
                        cell.ErrorText = errorMessage;
                    }
                    else
                    {
                        cell.Style.BackColor = Color.White;
                        cell.ErrorText = errorMessage;
                    }
                }
            }
            else if (e.ColumnIndex == P_index["學期"])
            {
                string errorMessage = "";

                if (cell.Value == null)
                    errorMessage = "學期不可為空白";
                else if (cell.Value.ToString() != "1" && cell.Value.ToString() != "2")
                    errorMessage = "學期必須為整數『1』或『2』";

                if (errorMessage != "")
                {
                    cell.Style.BackColor = Color.Red;
                    cell.ErrorText = errorMessage;
                }
                else
                {
                    cell.Style.BackColor = Color.White; //白色
                    cell.ErrorText = string.Empty;
                }
            }
            else if (e.ColumnIndex == P_index["日期"])
            {
                //如果使用者修改日期
                //需初步提醒使用者當日是否已有日期資料
                string StudentID = "" + dataGridViewX1.Rows[e.RowIndex].Cells[P_index["AttendanceID"]].Value;
                string OccurDate = "" + dataGridViewX1.Rows[e.RowIndex].Cells[P_index["日期"]].Value;
                AttendanceRecord ar = (AttendanceRecord)dataGridViewX1.Rows[e.RowIndex].Tag;

                DateTime d;

                if (DateTime.TryParse(OccurDate, out d)) //是否日期格式
                {
                    if (ar.OccurDate != d) //是否是原本日期資料(不是)
                    {
                        if (_studentObj.CheckStudentDateWhen(d, StudentID)) //該學生是否已經有此資料
                        {
                            cell.Style.BackColor = Color.Red;
                            cell.ErrorText = "學生當日已有缺曠資料!!";
                        }
                        else if (checkDataGridView(cell)) //檢查
                        {
                            cell.Style.BackColor = Color.Red;
                            cell.ErrorText = "一名學生一天不可有兩筆缺曠!!";
                        }
                        else
                        {
                            cell.Style.BackColor = Color.Violet; //白色
                            cell.ErrorText = string.Empty;
                        }
                    }
                    else //是當日資料
                    {
                        cell.Style.BackColor = Color.White; //白色
                        cell.ErrorText = string.Empty;
                    }
                }
                else //輸入錯誤資料
                {
                    cell.Style.BackColor = Color.Red;
                    cell.ErrorText = "輸入之資料不是日期!!";
                }
            }
        }

        /// <summary>
        /// 檢查傳入之日期cell是否於DataGridView上
        /// 有重複之日期,必須排除自己
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private bool checkDataGridView(DataGridViewCell Nowcell)
        {
            //判斷此行是否為同名學生所有
            //取得傳入Cell是那個學生的
            string NowStudentID = "" + Nowcell.OwningRow.Cells[P_index["AttendanceID"]].Value;
            string NowDay = "" + Nowcell.Value;

            //取得
            foreach (DataGridViewRow each in dataGridViewX1.Rows)
            {
                DataGridViewCell cell = each.Cells[P_index["日期"]];
                if (cell != Nowcell)
                {
                    string StudentID = "" + each.Cells[P_index["AttendanceID"]].Value;
                    string Day = "" + each.Cells[P_index["日期"]].Value;
                    if (StudentID == NowStudentID)
                    {
                        if (NowDay == Day)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;

        }

        private void dataGridViewX1_KeyDown(object sender, KeyEventArgs e)
        {
            if (dataGridViewX1.CurrentCell == null)
                return;

            string key = SingleEditorMethod.GetKeyMapping(e);
            if (dataGridViewX1.CurrentCell.ColumnIndex == P_index["學期"])
            {
                if (key == "1" || key == "2")
                {
                    foreach (DataGridViewCell cell in dataGridViewX1.SelectedCells)
                    {
                        if (cell.ColumnIndex != P_index["學期"]) continue;
                        cell.Value = key;
                        cell.Style.BackColor = Color.Violet; //白色
                    }
                }
                else
                    return;
            }
            else
            {
                if (!AbsenceHotKeyDic.ContainsKey(key))
                {
                    //不是Space,不是Delete則離開
                    if (e.KeyCode != Keys.Space && e.KeyCode != Keys.Delete) return;

                    foreach (DataGridViewCell cell in dataGridViewX1.SelectedCells)
                    {
                        if (cell.ColumnIndex < _startIndex) continue;
                        cell.Value = null;
                        cell.Tag = null;
                    }
                }
                else
                {
                    AbsenceMappingInfo info = AbsenceHotKeyDic[key];
                    foreach (DataGridViewCell cell in dataGridViewX1.SelectedCells)
                    {
                        if (cell.ColumnIndex < _startIndex) continue;

                        cell.Value = info.Abbreviation;
                        cell.Tag = info;
                        cell.Style.BackColor = Color.Violet; //白色
                    }
                }
            }


        }

        private void AttendanceEditForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (資料是否修改過)
            {
                if (FISCA.Presentation.Controls.MsgBox.Show("資料已變更尚未儲存，確認離開?", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        private void 批次輸入學年期ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<DataGridViewCell> SchoolYearList = new List<DataGridViewCell>();
            List<DataGridViewCell> SemesterList = new List<DataGridViewCell>();
            foreach (DataGridViewCell cell in dataGridViewX1.SelectedCells)
            {
                if (cell.ColumnIndex == P_index["學年度"])
                {
                    if (!SchoolYearList.Contains(cell))
                    {
                        SchoolYearList.Add(cell);
                    }
                }
                else if (cell.ColumnIndex == P_index["學期"])
                {
                    if (!SemesterList.Contains(cell))
                    {
                        SemesterList.Add(cell);
                    }
                }
            }

            int schoolYear = 90;
            int semester = 1;
            if (!int.TryParse("" + dataGridViewX1.CurrentRow.Cells[P_index["學年度"]].Value, out schoolYear))
                schoolYear = int.Parse(School.DefaultSchoolYear);

            if (int.TryParse("" + dataGridViewX1.CurrentRow.Cells[P_index["學期"]].Value, out semester))
                semester = int.Parse(School.DefaultSemester);

            SetValueSchoolYearSemester SetSys = new SetValueSchoolYearSemester(schoolYear, semester);
            DialogResult dr = SetSys.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.Yes)
            {
                foreach (DataGridViewCell cell in SchoolYearList)
                {
                    cell.Value = SetSys._schoolYear.ToString();
                    cell.Style.BackColor = Color.Violet; //白色
                    cell.ErrorText = string.Empty;
                }
                foreach (DataGridViewCell cell in SemesterList)
                {
                    cell.Value = SetSys._semester.ToString();
                    cell.Style.BackColor = Color.Violet; //白色
                    cell.ErrorText = string.Empty;
                }
            }
            else if (dr == System.Windows.Forms.DialogResult.No) //無動作
            {

            }
        }
    }
}
