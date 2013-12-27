using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar.Controls;
using System.Xml;
using FISCA.Presentation.Controls;
using K12.Data;
using FISCA.DSAUtil;
using FISCA.LogAgent;
using FISCA.Data;

namespace K12.Behavior.Keyboard
{
    //shinmin 新民
    public partial class PdMeritDemeritKBInput : BaseForm
    {
        Dictionary<string, List<KeyBoStudent>> _ClassList = new Dictionary<string, List<KeyBoStudent>>();
        Dictionary<string, KeyBoStudent> _StudentList = new Dictionary<string, KeyBoStudent>();

        List<int> PeriodDic1 = new List<int>() { 5, 6, 7 }; //獎勵Cell
        List<int> PeriodDic2 = new List<int>() { 8, 9, 10 }; //懲戒Cell
        PdPeriodDG2 _PeriodDG;
        Dictionary<string, string> MeritDemeritList = new Dictionary<string, string>(); //獎懲代碼表

        Dictionary<string, string> ClassNameDic = new Dictionary<string, string>();

        #region 以常數定義每個欄位的名稱
        private const int ClassColumnIndex = 0; //班級
        private const int SeatNoColumnIndex = 1; //座號
        private const int StudentNumberColumnIndex = 2; //學號
        private const int StudentNameColumnIndex = 3; //座號
        private const int DateColumnIndex = 4; //日期
        private const int MeritA = 5;
        private const int MeritB = 6;
        private const int MeritC = 7;
        private const int DemeritA = 8;
        private const int DemeritB = 9;
        private const int DemeritC = 10;
        private const int ReasonColumnIndex = 11; //事由
        private const int SchoolYearIndex = 12; //學年度
        private const int SemesterIndex = 13; //學期
        private const int DefInputDate = 14;  //輸入日期
        #endregion

        public PdMeritDemeritKBInput()
        {
            InitializeComponent();
        }

        private void PdMeritDemeritKBInput_Load(object sender, EventArgs e)
        {
            #region Load
            ClassNameDic = DataSort.GetClassNameDic();

            btnInputDate.Text = DateTime.Now.ToString("yyyyMMdd");
            //tbDateTime.Text = DateTime.Now.ToString("yyyyMMdd");

            int RowsAdd = Pddgv.Rows.Add();

            #region 學年度學期
            cbBoxItem1SchoolYear.Text = School.DefaultSchoolYear;
            cbBoxItem1Semester.Text = School.DefaultSemester;
            #endregion

            _PeriodDG = new PdPeriodDG2(this.Pddgv, PeriodDic1, PeriodDic2); //註冊事件

            #region 初始化資料
            BackgroundWorker bg = new BackgroundWorker();
            bg.DoWork += delegate
            {
                GetDReasonList(); //取得獎懲代碼表
                ReflashSchoolClass(); //建立學校班級清單            
            };

            bg.RunWorkerCompleted += delegate
            {
                Enabled = true;
                this.Text = "獎懲資料鍵盤化管理";
            };
            bg.RunWorkerAsync();
            Enabled = false;
            this.Text = "初始化中...";
            #endregion

            #endregion
        }

        #region Method

        private void GetDReasonList()
        {
            #region 取得並填入獎懲代碼表

            MeritDemeritList.Clear();
            DSResponse dsrsp = Config.GetDisciplineReasonList();
            foreach (XmlElement var in dsrsp.GetContent().GetElements("Reason"))
            {

                string type = var.GetAttribute("Type");
                string code = var.GetAttribute("Code");
                string desc = var.GetAttribute("Description");

                if (!MeritDemeritList.ContainsKey(code))
                {
                    MeritDemeritList.Add(code, desc);
                    string AddItems = code + " " + "(" + type + ")" + " " + desc;
                    //?
                    cbbReasonHotKey.Invoke(new AddItem(AddHotKey), AddItems);
                }
            }
            #endregion
        }

        //?
        private delegate void AddItem(string item);
        //?
        private void AddHotKey(string item)
        {
            cbbReasonHotKey.Items.Add(item);
        }

        private void ReflashSchoolClass()
        {
            QueryHelper q = new QueryHelper();
            StringBuilder sb = new StringBuilder();
            List<KeyBoStudent> Students = new List<KeyBoStudent>();
            sb.Append("select student.id,student.name,student.student_number,student.seat_no,student.ref_class_id,class.class_name from student ");
            sb.Append("join class on student.ref_class_id=class.id ");
            sb.Append("where student.seat_no is not null ");
            sb.Append("and student.status in (1,2)");

            DataTable dt = q.Select(sb.ToString());
            foreach (DataRow row in dt.Rows)
            {
                KeyBoStudent stud = new KeyBoStudent(row);
                Students.Add(stud);
            }

            _ClassList = new Dictionary<string, List<KeyBoStudent>>();

            foreach (KeyBoStudent eachstudent in Students)
            {
                if (!_ClassList.ContainsKey(eachstudent.ClassName))
                    _ClassList.Add(eachstudent.ClassName, new List<KeyBoStudent>());

                _ClassList[eachstudent.ClassName].Add(eachstudent);

                if (!_StudentList.ContainsKey(eachstudent.ID))
                {
                    _StudentList.Add(eachstudent.ID, eachstudent);
                }
            }
        }

        private bool IsDateTime(string date)
        {
            #region 時間錯誤判斷
            if (date == "")
            {
                return false;
            }

            if (date.Length == 4)
            {
                string[] bb = DateTime.Now.ToShortDateString().Split('/');
                date = date.Insert(0, bb[0]);
            }
            else if (date.Length != 8)
            {
                return false;
            }

            date = date.Insert(4, "/");
            date = date.Insert(7, "/");

            DateTime try_value;
            if (DateTime.TryParse(date, out try_value))
            {
                return true;
            }
            return false;
            #endregion
        }

        private void TextInsertError(ErrorProvider errorProvider, TextBoxX Text, string ErrorInfo)
        {
            #region 設定Text錯誤訊息之用
            Text.SelectAll();
            errorProvider.SetError(Text, ErrorInfo);
            errorProvider.SetIconPadding(Text, -20);
            #endregion
        }

        private DateTime DateInsertSlash(string TimeString)
        {
            #region 將8碼之時間,插入"\"符號
            string InsertSlash = TimeString.Insert(4, "/");
            InsertSlash = InsertSlash.Insert(7, "/");
            return DateTimeHelper.ParseDirect(InsertSlash);
            #endregion
        }

        private void SetReadOnlyAndColor(DataGridViewRow NeRow)
        {
            #region 儲存成功鎖定本行內容
            NeRow.ReadOnly = true;
            foreach (DataGridViewCell NeCell in NeRow.Cells)
            {
                NeCell.Style.BackColor = Color.LightSkyBlue;
            }
            #endregion
        }

        private bool CheckRow(DataGridViewRow _row)
        {
            #region 檢查ROW資料是否正確
            Dictionary<DataGridViewCell, bool> dic = new Dictionary<DataGridViewCell, bool>();


            if (_row.Tag is KeyBoStudent) //本CELL已找查出學生
            {
                foreach (DataGridViewCell each in _row.Cells) //檢查每一個CELL
                {
                    //略過獎懲範圍(5,6,7,8,9,10)
                    //(&& each.ColumnIndex != 2)
                    if (!PeriodDic1.Contains(each.ColumnIndex) && !PeriodDic2.Contains(each.ColumnIndex) && each.ColumnIndex != StudentNumberColumnIndex)
                    {
                        if ("" + each.Value == string.Empty) //是否為空值
                        {
                            dic.Add(each, false);
                        }
                    }
                }
            }
            else
            {
                return true; //true為資料錯誤
            }
            return dic.ContainsValue(false);
            #endregion
        }

        private int AddNewRowSetColor()
        {
            #region 新增一行,並且預設顏色
            int addNewRowIndex = Pddgv.Rows.Add();

            foreach (DataGridViewCell cell in Pddgv.Rows[addNewRowIndex].Cells)
            {
                if (PeriodDic1.Contains(cell.OwningColumn.Index) || PeriodDic2.Contains(cell.OwningColumn.Index))
                {
                    cell.Style.BackColor = Color.White;
                }
            }
            return addNewRowIndex;
            #endregion
        }

        private void InsertRow()
        {
            #region 插入新Rows
            int InsertRow = AddNewRowSetColor();

            #region 班級
            string ClassName = "";
            if (Pddgv.Rows.Count > 1)
            {
                ClassName = "" + Pddgv.Rows[InsertRow - 1].Cells[ClassColumnIndex].Value;
            }
            Pddgv.Rows[InsertRow].Cells[ClassColumnIndex].Value = ClassName.Trim();
            #endregion

            #region 日期
            string DateTimeName = "";
            if (tbDateTime.Text != string.Empty)
            {
                if (IsDateTime(tbDateTime.Text)) //如果時間欄位有值,並且是正確的
                {
                    DateTimeName = tbDateTime.Text;
                }
            }
            else
            {
                if (Pddgv.Rows.Count > 1)
                {
                    if (IsDateTime("" + Pddgv.Rows[InsertRow - 1].Cells[DateColumnIndex].Value)) //如果時間欄位有值,並且是正確的
                    {
                        DateTimeName = "" + Pddgv.Rows[InsertRow - 1].Cells[DateColumnIndex].Value;
                    }
                }
            }
            Pddgv.Rows[InsertRow].Cells[DateColumnIndex].Value = DateTimeName.Trim();
            #endregion

            #region 事由
            string ReasonName = "";
            if (tbReason.Text != string.Empty)
            {
                ReasonName = tbReason.Text;
            }
            else
            {
                if (Pddgv.Rows.Count > 1)
                {
                    ReasonName = "" + Pddgv.Rows[InsertRow - 1].Cells[ReasonColumnIndex].Value;
                }
            }
            Pddgv.Rows[InsertRow].Cells[ReasonColumnIndex].Value = ReasonName.Trim();
            #endregion

            Pddgv.Rows[InsertRow].Cells[SchoolYearIndex].Value = cbBoxItem1SchoolYear.Text.Trim();
            Pddgv.Rows[InsertRow].Cells[SemesterIndex].Value = cbBoxItem1Semester.Text.Trim();

            #region 獎懲內容

            string a = "";
            string b = "";
            string c = "";
            string d = "";
            string f = "";
            string g = "";

            if (string.IsNullOrEmpty(textBoxX1.Text.Trim()) && string.IsNullOrEmpty(textBoxX2.Text.Trim()) && string.IsNullOrEmpty(textBoxX3.Text.Trim()) && string.IsNullOrEmpty(textBoxX4.Text.Trim()) && string.IsNullOrEmpty(textBoxX5.Text.Trim()) && string.IsNullOrEmpty(textBoxX6.Text.Trim()))
            {
                if (Pddgv.Rows.Count > 1)
                {
                    if (!string.IsNullOrEmpty("" + Pddgv.Rows[InsertRow - 1].Cells[MeritA].Value) && !string.IsNullOrEmpty("" + Pddgv.Rows[InsertRow - 1].Cells[MeritB].Value) && !string.IsNullOrEmpty("" + Pddgv.Rows[InsertRow - 1].Cells[MeritC].Value))
                    {
                        a = "" + Pddgv.Rows[InsertRow - 1].Cells[MeritA].Value;
                        b = "" + Pddgv.Rows[InsertRow - 1].Cells[MeritB].Value;
                        c = "" + Pddgv.Rows[InsertRow - 1].Cells[MeritC].Value;
                    }
                    else if (!string.IsNullOrEmpty("" + Pddgv.Rows[InsertRow - 1].Cells[DemeritA].Value) && !string.IsNullOrEmpty("" + Pddgv.Rows[InsertRow - 1].Cells[DemeritB].Value) && !string.IsNullOrEmpty("" + Pddgv.Rows[InsertRow - 1].Cells[DemeritC].Value))
                    {
                        d = "" + Pddgv.Rows[InsertRow - 1].Cells[DemeritA].Value;
                        f = "" + Pddgv.Rows[InsertRow - 1].Cells[DemeritB].Value;
                        g = "" + Pddgv.Rows[InsertRow - 1].Cells[DemeritC].Value;
                    }
                    else
                    {
                        a = textBoxX1.Text.Trim();
                        b = textBoxX2.Text.Trim();
                        c = textBoxX3.Text.Trim();
                        d = textBoxX4.Text.Trim();
                        f = textBoxX5.Text.Trim();
                        g = textBoxX6.Text.Trim();
                    }
                }
                else
                {
                    a = textBoxX1.Text.Trim();
                    b = textBoxX2.Text.Trim();
                    c = textBoxX3.Text.Trim();
                    d = textBoxX4.Text.Trim();
                    f = textBoxX5.Text.Trim();
                    g = textBoxX6.Text.Trim();
                }
            }
            else
            {
                //任意有資料,則資料來源為 textBox
                a = textBoxX1.Text.Trim();
                b = textBoxX2.Text.Trim();
                c = textBoxX3.Text.Trim();
                d = textBoxX4.Text.Trim();
                f = textBoxX5.Text.Trim();
                g = textBoxX6.Text.Trim();
            }
            Pddgv.Rows[InsertRow].Cells[MeritA].Value = a.Trim();
            Pddgv.Rows[InsertRow].Cells[MeritB].Value = b.Trim();
            Pddgv.Rows[InsertRow].Cells[MeritC].Value = c.Trim();
            Pddgv.Rows[InsertRow].Cells[DemeritA].Value = d.Trim();
            Pddgv.Rows[InsertRow].Cells[DemeritB].Value = f.Trim();
            Pddgv.Rows[InsertRow].Cells[DemeritC].Value = g.Trim();

            #endregion


            #region 登錄日期
            if (IsDateTime(btnInputDate.Text) || btnInputDate.Text == string.Empty)
            {
                Pddgv.Rows[InsertRow].Cells[DefInputDate].Value = btnInputDate.Text;
            }
            else
            {
                FISCA.Presentation.Controls.MsgBox.Show("登錄日期內容輸入錯誤");
            }
            #endregion
            #endregion
        }

        #endregion

        #region 畫面按紐

        private void btnCancel_Click(object sender, EventArgs e)
        {
            #region 關閉
            this.Close();
            #endregion
        }

        #endregion

        #region 批次輸入

        private void cbBoxItem1SchoolYear_KeyUp(object sender, KeyEventArgs e)
        {
            #region 學年度
            if (e.KeyCode == Keys.Enter)
            {
                cbBoxItem1Semester.Focus();
            }
            #endregion
        }

        private void cbBoxItem1Semester_KeyUp(object sender, KeyEventArgs e)
        {
            #region 學期
            if (e.KeyCode == Keys.Enter)
            {
                tbDateTime.Focus();
            }
            #endregion
        }

        private void tbDateTime_KeyUp(object sender, KeyEventArgs e)
        {
            #region 日期
            if (e.KeyCode == Keys.Enter)
            {
                if (IsDateTime(tbDateTime.Text))
                {
                    if (tbDateTime.Text.Length == 4)
                    {
                        string[] bb = DateTime.Now.ToShortDateString().Split('/');
                        tbDateTime.Text = tbDateTime.Text.Insert(0, bb[0]);
                    }
                    errorProvider1.Clear();
                    cbbReasonHotKey.Focus();
                }
                else if (tbDateTime.Text == string.Empty)
                {
                    errorProvider1.Clear();
                    cbbReasonHotKey.Focus();
                }
                else
                {
                    TextInsertError(errorProvider1, tbDateTime, "請輸入正確 日期格式");
                }
            }
            #endregion
        }

        private void cbbReasonHotKey_KeyUp_1(object sender, KeyEventArgs e)
        {
            #region 事由代碼
            if (e.KeyCode == Keys.Enter)
            {
                if (cbbReasonHotKey.Text != string.Empty)
                {
                    string[] _Reason = cbbReasonHotKey.Text.Split(' ');
                    if (_Reason.Length == 3)
                    {
                        tbReason.Text = "" + _Reason.GetValue(2);
                    }
                    else
                    {
                        cbbReasonHotKey.Text = "";
                        tbReason.Text = "";
                    }
                }
                tbReason.Focus();
            }
            #endregion
        }

        private void tbReason_KeyUp(object sender, KeyEventArgs e)
        {
            #region 事由
            if (e.KeyCode == Keys.Enter)
            {
                btnItem1Insert_Click(null, null);
                Pddgv.Focus();
            }
            #endregion
        }

        private void btnItem1Insert_Click(object sender, EventArgs e)
        {
            #region 新增
            int InsertRow;

            //資料錯誤檢查
            if (!CheckSchoolYearSemeset())
            {
                MsgBox.Show("學年度/學期 資料錯誤");
                return;
            }

            if (Pddgv.Rows.Count <= 0)
            {
                //如果沒有新行
                InsertRow = Pddgv.Rows.Add();
            }
            else
            {
                //如果新行內容有值,就再Add新Row
                InsertRow = Pddgv.Rows.Count - 1;
                foreach (DataGridViewCell each in Pddgv.Rows[InsertRow].Cells)
                {
                    if ("" + each.Value != string.Empty)
                    {
                        InsertRow = Pddgv.Rows.Add();
                        break;
                    }
                }
            }

            Pddgv.Rows[InsertRow].Cells[SchoolYearIndex].Value = cbBoxItem1SchoolYear.Text;
            Pddgv.Rows[InsertRow].Cells[SemesterIndex].Value = cbBoxItem1Semester.Text;
            Pddgv.Rows[InsertRow].Cells[DateColumnIndex].Value = tbDateTime.Text;
            Pddgv.Rows[InsertRow].Cells[ReasonColumnIndex].Value = tbReason.Text;
            Pddgv.Rows[InsertRow].Cells[DefInputDate].Value = btnInputDate.Text;
            Pddgv.Rows[InsertRow].Cells[MeritA].Value = textBoxX1.Text;
            Pddgv.Rows[InsertRow].Cells[MeritB].Value = textBoxX2.Text;
            Pddgv.Rows[InsertRow].Cells[MeritC].Value = textBoxX3.Text;
            Pddgv.Rows[InsertRow].Cells[DemeritA].Value = textBoxX4.Text;
            Pddgv.Rows[InsertRow].Cells[DemeritB].Value = textBoxX5.Text;
            Pddgv.Rows[InsertRow].Cells[DemeritC].Value = textBoxX6.Text;

            Pddgv.Rows[InsertRow].Cells[0].Selected = true;
            #endregion
        }

        private bool CheckSchoolYearSemeset()
        {
            #region 資料錯誤檢查
            int check;
            if (!int.TryParse(cbBoxItem1SchoolYear.Text, out check))
            {
                return false;
            }

            if (cbBoxItem1Semester.Text == "1" || cbBoxItem1Semester.Text == "2")
            {

            }
            else
            {
                return false;
            }

            return true;
            #endregion
        }

        #endregion

        private void Pddgv_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            #region CellEndEdit

            PdCellHelper cell = new PdCellHelper(Pddgv.CurrentCell);
            cell.SetError(""); //重置錯誤訊息

            if (cell.GetCellIndex() == ClassColumnIndex)
            {
                #region 班級
                cell.SetupNumCell("123"); //重設座號,學號,姓名
                cell.SetRowTag(null);

                string cellGetValue = "";
                if (ClassNameDic.ContainsKey(cell.GetValue())) //取得班級名稱代碼
                {
                    cell.SetValue(cellGetValue = ClassNameDic[cell.GetValue()]);
                }
                else
                {
                    cellGetValue = cell.GetValue();
                }

                if (_ClassList.ContainsKey(cellGetValue)) //是否有此班級
                {
                    cell.SetRowTag(_ClassList[cellGetValue]);
                    cell.SetError("");
                }
                else
                {
                    cell.SetError("系統內查無此班級");
                }

                #endregion
            }
            else if (cell.GetCellIndex() == SeatNoColumnIndex)
            {
                #region 座號
                cell.SetupNumCell("23"); //重設
                cell.SetRowTag(null);

                if (_ClassList.ContainsKey(cell.GetNumCellValue(ClassColumnIndex))) //是否已填入班級
                {
                    int Cell_seat;
                    if (int.TryParse(cell.GetValue(), out Cell_seat))
                    {
                        foreach (KeyBoStudent stud in _ClassList[cell.GetNumCellValue(ClassColumnIndex)]) //直接依班級查詢
                        {
                            if (stud.SeatNo.ToString() == Cell_seat.ToString()) //如果座號相同
                            {
                                cell.SetRowTag(stud);                        //記住學生
                                cell.SetNumCellValue(StudentNumberColumnIndex, stud.StudentNumber); //填入學號
                                cell.SetNumCellValue(StudentNameColumnIndex, stud.Name);          //填入姓名
                                break;
                            }
                        }
                    }

                    if (!(cell.GetRowTag() is KeyBoStudent))
                    {
                        cell.SetError("查無此學生");
                        return;
                    }

                }
                else
                {
                    cell.SetError("您必須先輸入班級");
                    cell.SetNumError(ClassColumnIndex, "您必須先輸入班級");
                }
                #endregion
            }
            else if (cell.GetCellIndex() == DateColumnIndex)
            {
                #region 日期
                string Date = cell.GetValue();

                if (IsDateTime(Date)) //是否日期格式
                {
                    if (Date.Length == 4)
                    {
                        string[] bb = DateTime.Now.ToShortDateString().Split('/');
                        Date = Date.Insert(0, bb[0]);
                        cell.SetValue(Date);
                    }

                    cell.SetNumCellValue(SchoolYearIndex, cbBoxItem1SchoolYear.Text);
                    cell.SetNumCellValue(SemesterIndex, cbBoxItem1Semester.Text);
                    cell.SetNumCellValue(DefInputDate, btnInputDate.Text);
                }
                else
                {
                    cell.SetNumError(DateColumnIndex, "日期格式錯誤");
                }
                #endregion
            }
            else if (cell.GetCellIndex() == SchoolYearIndex)
            {
                //if (RowNowSave)
                //{
                //    if (CheckRow(Pddgv.CurrentRow))
                //    {
                //        MsgBox.Show("請確認學生資料是否輸入完整");
                //        return;
                //    }
                //    else
                //    {
                //        _RowSave(cell.GetRow());
                //        RowNowSave = false;
                //    }
                //}
            }
            #endregion
        }

        private void Pddgv_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            #region 當事由編輯過,進行儲存處理(Pddgv_CellEnter會發生於SchoolYearIndex欄位)
            if (Pddgv.CurrentCell.ColumnIndex == ReasonColumnIndex)
            {
                Pddgv.EndEdit();
                if (MeritDemeritList.ContainsKey("" + Pddgv.CurrentCell.Value))
                {
                    Pddgv.CurrentCell.Value = MeritDemeritList["" + Pddgv.CurrentCell.Value];
                }
                Pddgv.BeginEdit(false);
            }
            #endregion
        }

        private void Pddgv_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            #region 如果事由編輯過(學年度收到焦點),呼叫儲存_RowSave方法

            //if (e.ColumnIndex == SchoolYearIndex)
            //{
            //    if (!RowNowSave)
            //    {
            //        RowNowSave = true; //Pddgv_CellEnter會發生於SchoolYearIndex欄位
            //    }
            //}
            #endregion
        }

        private bool _RowSave(DataGridViewRow _row)
        {
            #region 儲存

            string CellMerit = "" + _row.Cells[MeritA].Value;
            CellMerit += "" + _row.Cells[MeritB].Value;
            CellMerit += "" + _row.Cells[MeritC].Value;

            string CellDemerit = "" + _row.Cells[DemeritA].Value;
            CellDemerit += "" + _row.Cells[DemeritB].Value;
            CellDemerit += "" + _row.Cells[DemeritC].Value;

            if (!string.IsNullOrEmpty(CellMerit) && !string.IsNullOrEmpty(CellDemerit))
            {
                MsgBox.Show("獎懲隻數資料不可同時輸入\n系統無法判斷本資料狀態!!\n請修正資料後重新進行儲存動作");
                return false;
            }

            if (CellMerit != string.Empty) //當獎懲欄位不是空的
            {
                int aa = 0;
                int bb = 0;
                int cc = 0;
                int.TryParse("" + _row.Cells[MeritA].Value, out aa);
                int.TryParse("" + _row.Cells[MeritB].Value, out bb);
                int.TryParse("" + _row.Cells[MeritC].Value, out cc);
                if (aa + bb + cc == 0)
                {
                    MsgBox.Show("獎勵欄位相加,不可為0");
                    return false;
                }


                #region 獎勵儲存
                MeritRecord InsertMerit = new MeritRecord();
                KeyBoStudent SR = (KeyBoStudent)_row.Tag;
                InsertMerit.RefStudentID = SR.ID; //學生ID

                #region 獎勵
                if ("" + _row.Cells[MeritA].Value == string.Empty)
                {
                    _row.Cells[MeritA].Value = 0;
                }
                InsertMerit.MeritA = Int.ParseAllowNull("" + _row.Cells[MeritA].Value); //大功

                if ("" + _row.Cells[MeritB].Value == string.Empty)
                {
                    _row.Cells[MeritB].Value = 0;
                }
                InsertMerit.MeritB = Int.ParseAllowNull("" + _row.Cells[MeritB].Value); //小功

                if ("" + _row.Cells[MeritC].Value == string.Empty)
                {
                    _row.Cells[MeritC].Value = 0;
                }
                InsertMerit.MeritC = Int.ParseAllowNull("" + _row.Cells[MeritC].Value); //嘉獎 
                #endregion

                //InsertMerit.MeritFlag = "1"; //MeritFlag=0 銷過,  MeritFlag=1 記功,   MeritFlag=2 記過
                InsertMerit.OccurDate = DateInsertSlash("" + _row.Cells[DateColumnIndex].Value); //日期
                InsertMerit.Reason = "" + _row.Cells[ReasonColumnIndex].Value; //事由
                InsertMerit.SchoolYear = int.Parse("" + _row.Cells[SchoolYearIndex].Value); //學年度
                InsertMerit.Semester = int.Parse("" + _row.Cells[SemesterIndex].Value); //學期
                InsertMerit.RegisterDate = DateInsertSlash("" + _row.Cells[DefInputDate].Value);

                try
                {
                    Merit.Insert(InsertMerit);
                    SetReadOnlyAndColor(_row);
                }
                catch
                {
                    FISCA.Presentation.Controls.MsgBox.Show("儲存獎勵資料,發生錯誤");
                    return false;
                }

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("詳細資料：");
                sb.AppendLine("學生「" + SR.Name + "」");
                sb.AppendLine("日期「" + InsertMerit.OccurDate.ToShortDateString() + "」");
                sb.AppendLine("獎勵內容「大功：" + InsertMerit.MeritA + " 小功：" + InsertMerit.MeritB + " 嘉獎：" + InsertMerit.MeritC + "」");
                sb.AppendLine("獎勵事由「" + InsertMerit.Reason + "」");
                ApplicationLog.Log("獎懲鍵盤登錄", "新增", "student", SR.ID, "新增獎勵資料。\n" + sb.ToString());

                this.Text = "獎懲鍵盤登錄 - 學生「" + InsertMerit.Student.Name + "」日期「" + InsertMerit.OccurDate.ToShortDateString() + "」獎勵資料新增成功";
                #endregion
            }
            else if (CellDemerit != string.Empty) //當懲戒欄位不是空的
            {
                int aa = 0;
                int bb = 0;
                int cc = 0;
                int.TryParse("" + _row.Cells[DemeritA].Value, out aa);
                int.TryParse("" + _row.Cells[DemeritB].Value, out bb);
                int.TryParse("" + _row.Cells[DemeritC].Value, out cc);
                if (aa + bb + cc == 0)
                {
                    MsgBox.Show("懲戒欄位相加,不可為0");
                    return false;
                }

                #region 懲戒儲存
                DemeritRecord InsertDemerit = new DemeritRecord();
                KeyBoStudent SR = (KeyBoStudent)_row.Tag;
                InsertDemerit.RefStudentID = SR.ID;

                #region 懲戒
                if ("" + _row.Cells[DemeritA].Value == string.Empty)
                {
                    _row.Cells[DemeritA].Value = 0;
                }
                InsertDemerit.DemeritA = Int.ParseAllowNull("" + _row.Cells[DemeritA].Value);

                if ("" + _row.Cells[DemeritB].Value == string.Empty)
                {
                    _row.Cells[DemeritB].Value = 0;
                }
                InsertDemerit.DemeritB = Int.ParseAllowNull("" + _row.Cells[DemeritB].Value);

                if ("" + _row.Cells[DemeritC].Value == string.Empty)
                {
                    _row.Cells[DemeritC].Value = 0;
                }
                InsertDemerit.DemeritC = Int.ParseAllowNull("" + _row.Cells[DemeritC].Value);
                #endregion

                //InsertDemerit.MeritFlag = "0";
                InsertDemerit.OccurDate = DateInsertSlash("" + _row.Cells[DateColumnIndex].Value);
                InsertDemerit.Reason = "" + _row.Cells[ReasonColumnIndex].Value;
                InsertDemerit.SchoolYear = int.Parse("" + _row.Cells[SchoolYearIndex].Value);
                InsertDemerit.Semester = int.Parse("" + _row.Cells[SemesterIndex].Value);
                InsertDemerit.RegisterDate = DateInsertSlash("" + _row.Cells[DefInputDate].Value);
                try
                {
                    Demerit.Insert(InsertDemerit);
                    SetReadOnlyAndColor(_row);
                }
                catch
                {
                    FISCA.Presentation.Controls.MsgBox.Show("儲存懲戒資料,發生錯誤");
                    return false;
                }
                #endregion
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("詳細資料：");
                sb.AppendLine("學生「" + SR.Name + "」");
                sb.AppendLine("日期「" + InsertDemerit.OccurDate.ToShortDateString() + "」");
                sb.AppendLine("懲戒內容「大過：" + InsertDemerit.DemeritA + " 小過：" + InsertDemerit.DemeritB + " 警告：" + InsertDemerit.DemeritC + "」");
                sb.AppendLine("懲戒事由「" + InsertDemerit.Reason + "」");
                ApplicationLog.Log("獎懲鍵盤登錄", "新增", "student", SR.ID, "新增懲戒資料。\n" + sb.ToString());

                this.Text = "獎懲鍵盤登錄 - 學生「" + InsertDemerit.Student.Name + "」日期「" + InsertDemerit.OccurDate.ToShortDateString() + "」懲戒資料新增成功";
            }
            else
            {
                FISCA.Presentation.Controls.MsgBox.Show("獎懲欄位皆無內容,請檢查資料是否正確");
                return false;
            }

            return true;

            #endregion
        }

        private void Pddgv_KeyUp(object sender, KeyEventArgs e)
        {
            if (Pddgv.Rows.Count <= 0) //必須有Rows
                return;

            if (Pddgv.CurrentRow.ReadOnly == true) //必須選擇Row,不可為已儲存資料
                return;

            Keys key = (e.KeyData & Keys.KeyCode);

            if (key == Keys.S && e.Alt) //Alt+Enter 如果是按Alt+S
            {
                if (!CheckRow(Pddgv.CurrentRow))
                {
                    if (_RowSave(Pddgv.CurrentRow))
                    {
                        InsertRow();
                        Pddgv.CurrentCell = Pddgv.Rows[Pddgv.Rows.Count - 1].Cells[0];
                    }
                }
            }
            else if (key == Keys.Down) //Alt+Enter 如果是按Down
            {
                if (!CheckRow(Pddgv.CurrentRow))
                {
                    if (_RowSave(Pddgv.CurrentRow))
                    {
                        InsertRow();
                        Pddgv.CurrentCell = Pddgv.Rows[Pddgv.Rows.Count - 1].Cells[0];
                    }
                }
            }
            else if (key == Keys.Enter) //如果是按下Enter
            {
                //登錄日期欄位
                if (Pddgv.CurrentCell.ColumnIndex == DefInputDate)
                {
                    //是否停在最後一欄
                    if (Pddgv.CurrentRow.Index == Pddgv.Rows.Count - 1)
                    {
                        //Row資料是否正確
                        if (!CheckRow(Pddgv.CurrentRow))
                        {
                            //資料是否儲存成功
                            if (_RowSave(Pddgv.CurrentRow))
                            {
                                InsertRow();
                                Pddgv.CurrentCell = Pddgv.Rows[Pddgv.Rows.Count - 1].Cells[0];
                            }
                        }
                    }
                }
            }
        }

        private void btnSetClassNameCode_Click(object sender, EventArgs e)
        {
            SetClassCode cc = new SetClassCode();
            cc.ShowDialog();

            ClassNameDic = DataSort.GetClassNameDic();
        }

        private void btnInputDate_KeyUp(object sender, KeyEventArgs e)
        {
            #region 日期
            if (e.KeyCode == Keys.Enter)
            {
                if (IsDateTime(btnInputDate.Text))
                {
                    if (btnInputDate.Text.Length == 4)
                    {
                        string[] bb = DateTime.Now.ToShortDateString().Split('/');
                        btnInputDate.Text = btnInputDate.Text.Insert(0, bb[0]);
                    }
                    errorProvider2.Clear();
                }
                else if (btnInputDate.Text == string.Empty)
                {
                    errorProvider2.Clear();
                }
                else
                {
                    TextInsertError(errorProvider2, btnInputDate, "請輸入正確 日期格式");
                }
            }
            #endregion
        }
    }
}
