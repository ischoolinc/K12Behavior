using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using FISCA.DSAUtil;
using FISCA.LogAgent;
using K12.Data;
using Framework.Feature;
using FISCA.Presentation.Controls;

namespace K12.Behavior.StuAdminExtendControls
{
    public partial class AttendanceEditForm : FISCA.Presentation.Controls.BaseForm
    {
        private string _startDate;
        private string _endDate;

        private Dictionary<string, string> _week;

        private List<string> _periodList;

        Dictionary<string, List<StudentRecord>> classList; //班級名稱,學生List
        List<ClassRecord> Classes; //全校班級
        List<StudentRecord> Students; //全校學生

        private List<string> AttendanceID = new List<string>(); //選擇的內容

        private bool Waiting
        {
            set { picWaiting.Visible = value; }
        }
        
        private BackgroundWorker _loader;

        //建構子
        public AttendanceEditForm()
        {
            InitializeComponent();
        }

        Dictionary<string, int> PeriodIndex = new Dictionary<string, int>();

        private void AttendanceEditForm_Load(object sender, EventArgs e)
        {
            #region Load
            //註冊BackgroundWorker事件
            InitialBackgroundWorker();

            //填入假別CheckBox
            AddCheckBox();

            BackgroundWorker bg = new BackgroundWorker();
            bg.DoWork += delegate
            {
                //建立班級清單
                ClassRef();
                #region 一週的字典
                _week = new Dictionary<string, string>();
                _week.Add("Monday", "一");
                _week.Add("Tuesday", "二");
                _week.Add("Wednesday", "三");
                _week.Add("Thursday", "四");
                _week.Add("Friday", "五");
                _week.Add("Saturday", "六");
                _week.Add("Sunday", "日");
                #endregion
                periodList(); //建立節次清單
            };

            bg.RunWorkerCompleted += delegate
            {
                ControlEnabled = true;
                this.Text = "缺曠資料檢視";

                #region 動態產生節次清單
                foreach (string var in _periodList)
                {
                    int columnIndex = dataGridViewX1.Columns.Add(var, var);
                    dataGridViewX1.Columns[columnIndex].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                    dataGridViewX1.Columns[columnIndex].SortMode = DataGridViewColumnSortMode.NotSortable;
                    dataGridViewX1.Columns[columnIndex].ReadOnly = true;
                    dataGridViewX1.Columns[columnIndex].Tag = var;

                    PeriodIndex.Add(var, columnIndex);
                }

                int ontIndex = dataGridViewX1.Columns.Add("SchoolYear", "學年度");
                PeriodIndex.Add("學年度", ontIndex);

                int twoIndex = dataGridViewX1.Columns.Add("SchoolYear", "學期");
                PeriodIndex.Add("學期", twoIndex);

                #endregion

                InitialDate(); //設定日期
                //btnRefresh_Click_1(null, null); //直接重新整理

                cbRange.SelectedIndex = 0;
                btnRefresh.Pulse(5);
            };

            bg.RunWorkerAsync();
            ControlEnabled = false;
            this.Text = "資料讀取中";
            #endregion
        }

        private void AddCheckBox()
        {
            #region 填入假別CheckBox
            DSResponse dsrsp = Config.GetAbsenceList();
            DSXmlHelper helper = dsrsp.GetContent();
            foreach (XmlElement element in helper.GetElements("Absence"))
            {
                CheckBox cbBox1 = new CheckBox();
                cbBox1.Text = element.GetAttribute("Name");
                cbBox1.ForeColor = Color.FromArgb(22, 66, 113);
                cbBox1.Checked = true;
                cbBox1.Size = new Size(60, 23);
                cpAtt.Controls.Add(cbBox1);
            } 
            #endregion
        }

        /// <summary>
        /// 鎖定畫面
        /// </summary>
        private bool ControlEnabled
        {
            set
            {
                groupPanel1.Enabled = value;
                groupPanel3.Enabled = value;
                btnRefresh.Enabled = value;
                btnExport.Enabled = value;
                btnAdd.Enabled = value;
                btnClear.Enabled = value;

            }
        }

        public void ClassRef()
        {
            #region 建立班級資訊

            classList = new Dictionary<string, List<StudentRecord>>();
            classList.Clear();

            Classes = new List<ClassRecord>();
            Classes.Clear();
            Students = new List<StudentRecord>();
            Students.Clear();

            Classes = Class.SelectAll();
            Students = Student.SelectAll();

            foreach (StudentRecord eachstudent in Students)
            {
                if (!string.IsNullOrEmpty(eachstudent.RefClassID))
                {
                    if (!classList.ContainsKey(eachstudent.Class.Name))
                        classList.Add(eachstudent.Class.Name, new List<StudentRecord>());

                    classList[eachstudent.Class.Name].Add(eachstudent);
                }
            }
            #endregion
        }

        private void periodList()
        {
            #region 取得節次資料
            _periodList = new List<string>();
            DSResponse dsrsp = Config.GetPeriodList();
            DSXmlHelper helper = dsrsp.GetContent();

            List<int> listxx = new List<int>();
            Dictionary<int, string> dic = new Dictionary<int, string>();
            foreach (XmlElement element in helper.GetElements("Period"))
            {
                listxx.Add(int.Parse(element.GetAttribute("Sort")));
                dic.Add(int.Parse(element.GetAttribute("Sort")), element.GetAttribute("Name"));
            }

            listxx.Sort();

            foreach (int each in listxx)
            {
                _periodList.Add(dic[each]);
            }
            #endregion
        }

        private void InitialBackgroundWorker()
        {
            #region 註冊BackgroundWorker事件
            _loader = new BackgroundWorker();
            _loader.DoWork += new DoWorkEventHandler(_loader_DoWork);
            _loader.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_loader_RunWorkerCompleted);
            #endregion
        }

        private void InitialDate()
        {
            #region 初始化日期(預設區為隔七天)
            _startDate = dateTimeInput1.Text = DateTime.Today.AddDays(-7).ToShortDateString();
            _endDate = dateTimeInput2.Text = DateTime.Today.ToShortDateString();
            #endregion
        }

        private void cbRange_SelectedIndexChanged(object sender, EventArgs e)
        {
            #region 範圍條件變更
            if (cbRange.SelectedIndex == 4) //學校
            {

                txtClass.Visible = false;
                txtClass.Text = "";

                cbClass.Visible = false; //隱藏班級控制
                cbClass.Items.Clear(); //清空班級內容

                lbClass.Visible = false; //隱藏班級lb


                lbSeatNo.Visible = false; //隱藏座號lb
                txtSeatNo.Visible = false; //隱藏座號控制
                txtSeatNo.Text = ""; //清空座號內容

            }
            else if (cbRange.SelectedIndex == 3) //年級
            {
                ChengGr();

                txtClass.Visible = false;
                txtClass.Text = "";

                cbClass.Visible = true; //顯示班級控制

                lbClass.Text = "年級"; //更改班級lb
                lbClass.Visible = true; //顯示班級lb

                lbSeatNo.Visible = false; //隱藏座號lb

                txtSeatNo.Visible = false; //隱藏座號控制
                txtSeatNo.Text = ""; //清空座號內容

                cbClass.SelectedIndex = 0;

            }
            else if (cbRange.SelectedIndex == 2) //班級
            {
                ChengClass(); //建立班級內容

                txtClass.Visible = false;
                txtClass.Text = "";

                cbClass.Visible = true; //顯示班級控制
                lbClass.Text = "班級"; //更改班級lb
                lbClass.Visible = true; //顯示班級lb

                lbSeatNo.Visible = false; //隱藏座號lb

                txtSeatNo.Visible = false; //隱藏座號控制
                txtSeatNo.Text = ""; //清空座號內容

                cbClass.SelectedIndex = 0;

            }
            else if (cbRange.SelectedIndex == 1) //學號
            {
                cbClass.Visible = false;
                cbClass.Items.Clear();

                txtClass.Visible = true;
                txtClass.Text = "";

                lbClass.Text = "學號";
                lbClass.Visible = true;

                lbSeatNo.Visible = false;
                txtSeatNo.Visible = false;
                txtSeatNo.Text = "";
            }
            else if (cbRange.SelectedIndex == 0) //班級座號
            {
                ChengClass(); //建立班級內容

                cbClass.Visible = true; //顯示班級控制
                lbClass.Text = "班級"; //更改班級lb
                lbClass.Visible = true; //顯示班級lb

                lbSeatNo.Visible = true; //顯示座號文字
                txtSeatNo.Visible = true; //顯示控制
                txtSeatNo.Text = "";

                txtClass.Visible = false;
                txtClass.Text = "";

                cbClass.SelectedIndex = 0;

            } 
            #endregion
        }

        private void ChengGr()
        {
            #region 年級資料處理

            cbClass.Items.Clear(); //清空下拉式選單
            cbClass.DisplayMember = "Key";
            Classes.Sort(new Comparison<ClassRecord>(tool.SortComparer)); //排序

            Dictionary<string, List<ClassRecord>> dic = new Dictionary<string, List<ClassRecord>>();
            List<string> dicSort = new List<string>();

            foreach (ClassRecord DCA in Classes)
            {
                if (DCA.GradeYear == null)
                    continue;

                if (!dic.ContainsKey(DCA.GradeYear.ToString()))
                {
                    dic.Add(DCA.GradeYear.ToString(), new List<ClassRecord>());
                    dicSort.Add(DCA.GradeYear.ToString());
                }

                dic[DCA.GradeYear.ToString()].Add(DCA);
            }

            dicSort.Sort();

            foreach (string each in dicSort)
            {
                KeyValuePair<string, List<ClassRecord>> KKBOX = new KeyValuePair<string, List<ClassRecord>>(each, dic[each]);
                cbClass.Items.Add(KKBOX);
            }

            #endregion
        }

        private void ChengClass()
        {
            #region 班級資料處理

            cbClass.Items.Clear(); //清空下拉式選單
            cbClass.DisplayMember = "Key";
            Classes.Sort(new Comparison<ClassRecord>(tool.SortComparer)); //排序

            foreach (ClassRecord DCA in Classes)
            {
                if (DCA.Name == string.Empty)
                    continue;
                KeyValuePair<string, ClassRecord> KKBOX = new KeyValuePair<string, ClassRecord>(DCA.Name, DCA);
                cbClass.Items.Add(KKBOX);
             }
            #endregion           
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            #region 全選CheckBox
            foreach (CheckBox each in cpAtt.Controls)
            {
                each.Checked = checkBox1.Checked;
            }
            #endregion
        }

        private void btnRefresh_Click_1(object sender, EventArgs e)
        {
            #region 查詢

            _startDate = dateTimeInput1.Text;
            _endDate = dateTimeInput2.Text;

            btnRefresh.Enabled = false; //關掉開關

            if (cbRange.SelectedIndex == 0)
            {
                #region 班級座號
                KeyValuePair<string, ClassRecord> item = (KeyValuePair<string, ClassRecord>)cbClass.SelectedItem;
                List<StudentRecord> stulist = item.Value.Students;

                StudentRecord SeleteStud = new StudentRecord();

                foreach (StudentRecord stud in stulist)
                {
                    if (stud.SeatNo == null)
                    {
                        continue;
                    }

                    if ("" + stud.SeatNo == txtSeatNo.Text)
                    {
                        SeleteStud = stud;
                        break;
                    }
                }

                if (SeleteStud.ID == null)
                {
                    MsgBox.Show("查無此座號,請重新輸入");
                    btnRefresh.Enabled = true;
                    Waiting = false;
                    txtSeatNo.SelectAll();
                    return;
                }
                else
                {
                    List<StudentRecord> NowList = new List<StudentRecord>();
                    NowList.Add(SeleteStud);
                    _loader.RunWorkerAsync(NowList);
                }
                #endregion
            }
            else if (cbRange.SelectedIndex == 1)
            {
                #region 學號
                StudentRecord SeleteStud = new StudentRecord();

                foreach (StudentRecord stud in Students)
                {
                    if (stud.StudentNumber == txtClass.Text)
                    {
                        SeleteStud = stud;
                        break;
                    }
                }

                if (SeleteStud.ID == null)
                {
                    MsgBox.Show("查無此學號,請重新輸入");
                    btnRefresh.Enabled = true;
                    Waiting = false;
                    txtClass.SelectAll();
                }
                else
                {
                    List<StudentRecord> NowList = new List<StudentRecord>();
                    NowList.Add(SeleteStud);
                    _loader.RunWorkerAsync(NowList);
                }
                #endregion
            }
            else if (cbRange.SelectedIndex == 2)
            {
                #region 班級
                KeyValuePair<string, ClassRecord> item = (KeyValuePair<string, ClassRecord>)cbClass.SelectedItem;

                if (item.Value.Students.Count == 0)
                {
                    MsgBox.Show("本班級並無學生");
                    btnRefresh.Enabled = true;
                    Waiting = false;
                    return;
                }

                _loader.RunWorkerAsync(item.Value.Students);
                #endregion
            }
            else if (cbRange.SelectedIndex == 3)
            {
                #region 年級
                KeyValuePair<string, List<ClassRecord>> item = (KeyValuePair<string, List<ClassRecord>>)cbClass.SelectedItem;

                List<StudentRecord> stud = new List<StudentRecord>();

                foreach (ClassRecord STUD in item.Value)
                {
                    stud.AddRange(STUD.Students);
                }
                _loader.RunWorkerAsync(stud);
                #endregion
            }
            else if (cbRange.SelectedIndex == 4) //全校
            {
                _loader.RunWorkerAsync(Students);
            } 
            #endregion
            //查詢不Log
            //ApplicationLog.Log("學務系統.缺曠資料檢視", "查詢缺曠內容", "缺曠資料檢視，已使用查詢功能。");

        }

        private void _loader_DoWork(object sender, DoWorkEventArgs e)
        {
            #region 開始執行背景作業
            try
            {
                List<StudentRecord> studList = (List<StudentRecord>)e.Argument;
                List<AttendanceRecord> AttendList = Attendance.SelectByDate(studList, dateTimeInput1.Value, dateTimeInput2.Value);
                e.Result = AttendList;
            }
            catch (Exception ex)
            {
                FISCA.Presentation.Controls.MsgBox.Show("取得缺曠失敗。" + ex);
                btnRefresh.Enabled = true;
                Waiting = false; //旋轉停止
            }
            #endregion
        }

        private void _loader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region 背景作業完成

            btnRefresh.Enabled = true; //按鈕打開
            Waiting = false; //旋轉停止

            if (e.Error == null)
            {
                if (e.Result is List<AttendanceRecord> && e.Result != null)
                {
                    List<AttendanceRecord> AttResult = (List<AttendanceRecord>)e.Result;
                    if (AttResult.Count == 0)
                    {
                        dataGridViewX1.Rows.Clear();
                        MsgBox.Show("查無缺曠資料!");
                        return;
                    }
                    FillDataGridView((List<AttendanceRecord>)e.Result);
                }
            }
            #endregion
        }

        private void FillDataGridView(List<AttendanceRecord> attendList)
        {
            #region 更新畫面資料

            dataGridViewX1.Rows.Clear();
            dataGridViewX1.SuspendLayout();

            List<string> jone = new List<string>();

            foreach (CheckBox each in cpAtt.Controls)
            {
                if (each.Checked)
                {
                    jone.Add(each.Text);
                }
            }

            attendList.Sort(SortByClassAndSeatNo);

            List<string> StudentCount = new List<string>();

            foreach (AttendanceRecord att in attendList)
            {
                bool InPort = true;
                foreach (K12.Data.AttendancePeriod AttP in att.PeriodDetail)
                {
                    if (jone.Contains(AttP.AbsenceType)) //只要有一個內容包含於清單中
                    {
                        InPort = false;
                        break;
                    }
                }

                if (InPort)
                    continue;


                List<string> values = new List<string>();
                StudentRecord _student = Student.SelectByID(att.RefStudentID);

                if (!StudentCount.Contains(att.RefStudentID))
                {
                    StudentCount.Add(att.RefStudentID); //學生人數統計
                }

                DataGridViewRow _row = new DataGridViewRow();
                _row.CreateCells(dataGridViewX1);
                _row.Tag = _student.ID;
                _row.Cells[0].Value = att.ID;

                _row.Cells[1].Value = att.OccurDate.ToShortDateString();

                _row.Cells[2].Value = _week[att.OccurDate.DayOfWeek.ToString()];
                if (_student.Class != null)
                {
                    _row.Cells[3].Value = _student.Class.Name;
                }

                _row.Cells[4].Value = _student.SeatNo;
                _row.Cells[5].Value = _student.StudentNumber;
                _row.Cells[6].Value = _student.Name;
                _row.Cells[7].Value = _student.Gender;

                foreach (K12.Data.AttendancePeriod AttPeriod in att.PeriodDetail)
                {
                    if (_periodList.Contains(AttPeriod.Period) && jone.Contains(AttPeriod.AbsenceType))
                    {
                        _row.Cells[PeriodIndex[AttPeriod.Period]].Value = AttPeriod.AbsenceType;
                    }
                }

                _row.Cells[PeriodIndex["學年度"]].Value = att.SchoolYear;
                _row.Cells[PeriodIndex["學期"]].Value = att.Semester;

                dataGridViewX1.Rows.Add(_row);
            }

            txtHelpStudentCount.Text = "學生人數：" + StudentCount.Count;

            dataGridViewX1.ResumeLayout();
            #endregion
        }

        #region 時間判斷
        //private string ChengTime(DateTime dt)
        //{
        //    int x = dt.ToString().IndexOf(' ');
        //    string y = dt.ToString().Remove(x);
        //    return y;
        //}

        //private bool IsDateTime(string date)
        //{

        //    DateTime try_value;
        //    if (DateTime.TryParse(date, out try_value))
        //        return true;
        //    return false;
        //}
        #endregion

        private void rbNoDate_CheckedChanged(object sender, EventArgs e)
        {
            #region 啟用日期條件
            //if (!rbNoDate.Checked)
            //{
            //    lbStartDate.Enabled = true;
            //    lbEndDate.Enabled = true;
            //    txtStartDate.Enabled = true;
            //    txtEndDate.Enabled = true;
            //}
            //else
            //{
            //    lbStartDate.Enabled = false;
            //    lbEndDate.Enabled = false;
            //    txtStartDate.Enabled = false;
            //    txtEndDate.Enabled = false;
            //}
            #endregion
        }

        #region 加入待處理
        private void btnAdd_Click(object sender, EventArgs e)
        {
            //DataGridViewRowCollection _selectRow = (DataGridViewRowCollection)dataGridViewX1.SelectedRows;

            //List<string> _temporallist = GetSelectedStudentList(ref _count);

            List<string> TemporalStud = GetSelectedStudentList();
            K12.Presentation.NLDPanels.Student.AddToTemp(TemporalStud);

            FISCA.Presentation.Controls.MsgBox.Show("新增 " + TemporalStud.Count + " 名學生於待處理");
            labelX3.Text = "待處理共 " + K12.Presentation.NLDPanels.Student.TempSource.Count + " 名學生";
            labelX3.Visible = true;
            btnClear.Visible = true;
        }

        private List<string> GetSelectedStudentList()
        {
            List<string> _temporallist = new List<string>();
            foreach (DataGridViewRow var in dataGridViewX1.SelectedRows)
            {
                if (!_temporallist.Contains("" + (string)var.Tag))
                {
                    _temporallist.Add("" + (string)var.Tag);
                }
            }
            return _temporallist;
        }
        #endregion

        private void btnClear_Click(object sender, EventArgs e)
        {
            #region 移出待處理
            K12.Presentation.NLDPanels.Student.RemoveFromTemp(K12.Presentation.NLDPanels.Student.TempSource);
            FISCA.Presentation.Controls.MsgBox.Show("已清除 待處理 所有學生");

            labelX3.Visible = false;
            btnClear.Visible = false; 
            #endregion
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
                    Attendance.Delete(AttendanceID);
                }
                catch
                {
                    FISCA.Presentation.Controls.MsgBox.Show("刪除發生錯誤");
                    return;
                }

                ApplicationLog.Log("學務系統.缺曠資料檢視", "刪除缺曠資料", "已將選取之「" + AttendanceID.Count + "」筆缺曠資料刪除。");
                FISCA.Presentation.Controls.MsgBox.Show("刪除成功!!");
                btnRefresh_Click_1(null, null);
                //if (!_loader.IsBusy)
                //{
                //    Attendance.Instance.SyncAllBackground();
                //}
            }
        }

        //處理選擇多少資料
        private void dataGridViewX1_SelectionChanged(object sender, EventArgs e)
        {
            AttendanceID.Clear();

            foreach (DataGridViewRow row in dataGridViewX1.SelectedRows)
            {
                AttendanceID.Add("" + row.Cells[0].Value);
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


        private int SortByClassAndSeatNo(AttendanceRecord attendX, AttendanceRecord attendy)
        {
            StudentRecord x = attendX.Student;
            StudentRecord y = attendy.Student;
            string 班級名稱1 = (x.Class == null ? "" : x.Class.Name) + "::";
            string 座號1 = (x.SeatNo.HasValue ? x.SeatNo.Value.ToString().PadLeft(2, '0') : "") + "::";
            string 班級名稱2 = (y.Class == null ? "" : y.Class.Name) + "::";
            string 座號2 = (y.SeatNo.HasValue ? y.SeatNo.Value.ToString().PadLeft(2, '0') : "") + "::";
            string 日期1 = attendX.OccurDate.ToShortDateString();
            string 日期2 = attendy.OccurDate.ToShortDateString();
            班級名稱1 += 座號1;
            班級名稱1 += 日期1;

            班級名稱2 += 座號2;
            班級名稱2 += 日期2;

            return 班級名稱1.CompareTo(班級名稱2);
        }
    
    }
}