using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using DevComponents.DotNetBar;
using FISCA.DSAUtil;
using FISCA.LogAgent;
using FISCA.Presentation.Controls;
using K12.Data;
using Framework.Feature;
//using SmartSchool.ApplicationLog;

namespace K12.Behavior.StudentExtendControls
{
    /// <summary>
    /// 新增或修改懲戒資料的畫面。
    /// 修改時，因為只能修改一個學生的某一筆獎懲資料，所以只要傳入一個 DemeritRecordEditor 物件即可
    /// 新增時，可以同時對多位學生增加相同的懲戒紀錄，所以要傳入多位學生的資料，但不傳入DemeritRecordEditor 物件，此物件會在儲存時由每位學生記錄取得。
    /// </summary>
    public partial class DemeritEditForm : BaseForm
    {
        private List<StudentRecord> _students;
        private ErrorProvider _errorProvider;
        private DemeritRecord _demeritRecordEditor;
        private Dictionary<string, string> dicReason = new Dictionary<string, string>();

        private Dictionary<string, string> ResonDic = new Dictionary<string, string>();

        bool Check_ischool_isSeniorOrJunior;

        //Log
        private Dictionary<string, string> DicBeforeData = new Dictionary<string, string>();

        /// <summary>
        /// Constructor，新增時使用。
        /// </summary>
        /// <param name="students"></param>
        public DemeritEditForm(List<StudentRecord> students)
        {
            #region 新增
            this._students = students;
            Initialize();
            dateTimeInput1.Value = DateTime.Today;
            dateTimeInput2.Value = DateTime.Today;
            if (this._students.Count > 1)
            {
                Text = string.Format("懲戒管理 【 新增：{0} ... 等共 {1} 位 】", this._students[0].Name, this._students.Count.ToString()); ;
            }
            else if (this._students.Count == 1)
            {
                Text = string.Format("懲戒管理 【 新增：{0} 】", this._students[0].Name); ;
            } 
            #endregion
        }

        public DemeritEditForm(List<StudentRecord> students, string SchoolYear, string Semester)
        {
            #region 新增
            this._students = students;

            #region 建構子
            InitializeComponent();

            _errorProvider = new ErrorProvider();

            //學年度

            intSchoolYear.Text = School.DefaultSchoolYear;
            intSemester.Text = School.DefaultSemester;

            //cboSchoolYear.Items.Add(int.Parse(School.DefaultSchoolYear) - 4);
            //cboSchoolYear.Items.Add(int.Parse(School.DefaultSchoolYear) - 3);
            //cboSchoolYear.Items.Add(int.Parse(School.DefaultSchoolYear) - 2);
            //cboSchoolYear.Items.Add(int.Parse(School.DefaultSchoolYear) - 1);
            //int SchoolYearSelectIndex = cboSchoolYear.Items.Add(SchoolYear);
            //cboSchoolYear.Items.Add(int.Parse(School.DefaultSchoolYear) + 1);
            //cboSchoolYear.Items.Add(int.Parse(School.DefaultSchoolYear) + 2);
            //cboSchoolYear.Items.Add(int.Parse(School.DefaultSchoolYear) + 3);
            //cboSchoolYear.SelectedIndex = SchoolYearSelectIndex;
            ////學期
            //cboSemester.Items.Add(1);
            //cboSemester.Items.Add(2);
            //cboSemester.SelectedIndex = Semester == "1" ? 0 : 1;

            #endregion

            dateTimeInput1.Value = DateTime.Today;
            dateTimeInput2.Value = DateTime.Today;
            if (this._students.Count > 1)
            {
                Text = string.Format("懲戒管理 【 新增：{0} ... 等共 {1} 位 】", this._students[0].Name, this._students.Count.ToString()); ;
            }
            else if (this._students.Count == 1)
            {
                Text = string.Format("懲戒管理 【 新增：{0} 】", this._students[0].Name); ;
            }
            #endregion
        }

        /// <summary>
        /// Constructor，修改時使用
        /// </summary>
        /// <param name="_demeritRecordEditor"></param>
        public DemeritEditForm(DemeritRecord demeritRecordEditor, FISCA.Permission.FeatureAce permission)
        {
            #region 修改
            this._demeritRecordEditor = demeritRecordEditor;

            #region Log用

            if (demeritRecordEditor.DemeritA.HasValue)
            {
                DicBeforeData.Add("大過", demeritRecordEditor.DemeritA.Value.ToString());
            }
            if (demeritRecordEditor.DemeritB.HasValue)
            {
                DicBeforeData.Add("小過", demeritRecordEditor.DemeritB.Value.ToString());
            }
            if (demeritRecordEditor.DemeritC.HasValue)
            {
                DicBeforeData.Add("警告", demeritRecordEditor.DemeritC.Value.ToString());
            }
            DicBeforeData.Add("事由", demeritRecordEditor.Reason);
            #endregion

            this._students = new List<StudentRecord>();
            this._students.Add(Student.SelectByID(demeritRecordEditor.RefStudentID));

            Initialize();

            if (permission.Editable)
                Text = string.Format("懲戒管理 【 修改：{0}，{1} 】", Student.SelectByID(demeritRecordEditor.RefStudentID).Name, demeritRecordEditor.OccurDate.ToShortDateString());
            else
                Text = string.Format("懲戒管理 【 檢視：{0}，{1} 】", Student.SelectByID(demeritRecordEditor.RefStudentID).Name, demeritRecordEditor.OccurDate.ToShortDateString());

            btnSave.Visible = permission.Editable;
            cboReasonRef.Enabled = permission.Editable;
            intSchoolYear.Enabled = permission.Editable;
            intSemester.Enabled = permission.Editable;
            dateTimeInput1.Enabled = permission.Editable;
            dateTimeInput2.Enabled = permission.Editable;

            foreach (Control each in Controls)
            {
                if (each is DevComponents.DotNetBar.Controls.TextBoxX)
                    (each as DevComponents.DotNetBar.Controls.TextBoxX).ReadOnly = !permission.Editable;
            } 
            #endregion
        }

        private void Initialize()
        {
            #region 建構子
            InitializeComponent();

            _errorProvider = new ErrorProvider();

            intSchoolYear.Text = School.DefaultSchoolYear;
            intSemester.Text = School.DefaultSemester;

            ////學年度
            //cboSchoolYear.Items.Add(int.Parse(School.DefaultSchoolYear) - 3);
            //cboSchoolYear.Items.Add(int.Parse(School.DefaultSchoolYear) - 2);
            //cboSchoolYear.Items.Add(int.Parse(School.DefaultSchoolYear) - 1);
            //int SchoolYearSelectIndex = cboSchoolYear.Items.Add(int.Parse(School.DefaultSchoolYear));
            //cboSchoolYear.Items.Add(int.Parse(School.DefaultSchoolYear) + 1);
            //cboSchoolYear.Items.Add(int.Parse(School.DefaultSchoolYear) + 2);
            //cboSchoolYear.SelectedIndex = SchoolYearSelectIndex;
            ////學期
            //cboSemester.Items.Add(1);
            //cboSemester.Items.Add(2);
            //cboSemester.SelectedIndex = School.DefaultSemester == "1" ? 0 : 1;

            //判斷是國中還是高中資料庫
            //判斷是國中系統還是高中系統，若是國中系統則用Framework的權限
            //若是高中系統則用FISCA權限。
            K12.Data.Configuration.ConfigData cd = K12.Data.School.Configuration["ischool_Metadata"];
            if (cd["EducationalSystem"] == "SeniorHighSchool") //如果是高中
            {
                Check_ischool_isSeniorOrJunior = true;
                cbMeritFlag.Visible = true; //高中模式就將留察顯示
            }
            else if (cd["EducationalSystem"] == "JuniorHighSchool") //如果是國中
            {
                Check_ischool_isSeniorOrJunior = false;
                cbMeritFlag.Visible = false; //國中模式就將留察隱藏
            }
            else //預設為高中模式
            {
                Check_ischool_isSeniorOrJunior = true;
                cbMeritFlag.Visible = true; //高中模式就將留察顯示
            }
            #endregion
        }

        private void DemeritEditor_Load(object sender, EventArgs e)
        {
            #region Load
            //取得懲戒的代碼和原因清單，並放到 事由代碼 的下拉式方塊中。
            DSResponse dsrsp = Config.GetDisciplineReasonList();
            cboReasonRef.SelectedItem = null;
            cboReasonRef.Items.Clear();
            dicReason.Clear();
            DSXmlHelper helper = dsrsp.GetContent();
            KeyValuePair<string, string> fkvp = new KeyValuePair<string, string>("", "");
            cboReasonRef.Items.Add(fkvp);

            foreach (XmlElement element in helper.GetElements("Reason"))
            {
                if (element.GetAttribute("Type") == "懲戒" || element.GetAttribute("Type") == "懲誡")
                {
                    dicReason.Add(element.GetAttribute("Code"), element.GetAttribute("Description"));
                    string k = element.GetAttribute("Code") + "-" + element.GetAttribute("Description");
                    string v = element.GetAttribute("Description");
                    KeyValuePair<string, string> kvp = new KeyValuePair<string, string>(k, v);
                    cboReasonRef.Items.Add(kvp);

                    if (!ResonDic.ContainsKey("" + element.GetAttribute("Code")))
                    {
                        ResonDic.Add("" + element.GetAttribute("Code"), "" + element.GetAttribute("Description"));
                    }
                }
            }
            cboReasonRef.DisplayMember = "Key";
            cboReasonRef.ValueMember = "Value";
            cboReasonRef.SelectedIndex = 0;

            //如果是修改模式，則把資料填到畫面上。
            if (this._demeritRecordEditor != null)
            {
                txtReason.Text = _demeritRecordEditor.Reason;
                txt1.Text = _demeritRecordEditor.DemeritA.ToString();
                txt2.Text = _demeritRecordEditor.DemeritB.ToString();
                txt3.Text = _demeritRecordEditor.DemeritC.ToString();
                dateTimeInput1.Value = _demeritRecordEditor.OccurDate;
                intSchoolYear.Text = _demeritRecordEditor.SchoolYear.ToString();
                intSemester.Text = _demeritRecordEditor.Semester.ToString();

                //如果是高中模式,判斷留察狀態
                if (Check_ischool_isSeniorOrJunior)
                {
                    cbMeritFlag.Checked = _demeritRecordEditor.MeritFlag == "2" ? true : false;
                }

                if (_demeritRecordEditor.RegisterDate.HasValue)
                {
                    int SetIndex = _demeritRecordEditor.RegisterDate.ToString().IndexOf(' ');
                    string SetRegisterDate = _demeritRecordEditor.RegisterDate.ToString().Remove(SetIndex);
                    dateTimeInput2.Value = _demeritRecordEditor.RegisterDate.Value;
                }
                else
                {
                    dateTimeInput2.Text = "";
                }
            }
            #endregion
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            #region Save
            bool valid = true;
            foreach (Control control in this.Controls)
                if (!string.IsNullOrEmpty(_errorProvider.GetError(control)))
                    valid = false;

            if (!valid)
            {
                MsgBox.Show("資料驗證錯誤，請先修正後再行儲存");
                return;
            }

            //檢查使用者是否忘記輸入功過次數。

            int sum = int.Parse(GetTextValue(txt1.Text)) + int.Parse(GetTextValue(txt2.Text)) + int.Parse(GetTextValue(txt3.Text));
            //留察資料 
            if (cbMeritFlag.Checked == false)
            {
                //未輸入值
                if (sum <= 0)
                {
                    MsgBox.Show("請別忘了輸入懲戒次數。");
                    return;
                }
            }

            if (txtReason.Text.Trim() == "")
            {
                DialogResult dr = MsgBox.Show("事由未輸入，是否繼續進行儲存操作？", MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2);
                if (dr == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }
            }

            if (this._demeritRecordEditor == null) //新增懲戒
            {
                List<DemeritRecord> LogDemeritList = new List<DemeritRecord>();

                try
                {
                    LogDemeritList = Insert();
                    Demerit.Insert(LogDemeritList);
                }
                catch (Exception ex)
                {
                    MsgBox.Show("新增懲戒記錄時發生錯誤: \n" + ex.Message);
                }

                if (_students.Count == 1)
                {
                    #region 單筆新增Log
                    StringBuilder sb = new StringBuilder();
                    sb.Append("學生「" + LogDemeritList[0].Student.Name + "」");
                    sb.Append("日期「" + LogDemeritList[0].OccurDate.ToShortDateString() + "」");
                    sb.AppendLine("新增一筆懲戒資料。");
                    sb.AppendLine("詳細資料：");
                    if (LogDemeritList[0].DemeritA.HasValue)
                    {
                        sb.Append("大過「" + LogDemeritList[0].DemeritA.Value.ToString() + "」");
                    }
                    if (LogDemeritList[0].DemeritB.HasValue)
                    {
                        sb.Append("小過「" + LogDemeritList[0].DemeritB.Value.ToString() + "」");
                    }
                    if (LogDemeritList[0].DemeritC.HasValue)
                    {
                        sb.Append("警告「" + LogDemeritList[0].DemeritC.Value.ToString() + "」");
                    }
                    sb.Append("懲戒事由「" + LogDemeritList[0].Reason + "」");

                    ApplicationLog.Log("學務系統.懲戒資料", "新增學生懲戒資料", "student", _students[0].ID, sb.ToString());
                    #endregion
                    MsgBox.Show("新增懲戒資料成功!");
                }
                else if (_students.Count > 1)
                {
                    #region 批次新增Log
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("批次新增懲戒資料");
                    sb.Append("日期「" + LogDemeritList[0].OccurDate.ToShortDateString() + "」");
                    sb.AppendLine("共「" + LogDemeritList.Count + "」名學生，");
                    sb.AppendLine("詳細資料：");
                    if (LogDemeritList[0].DemeritA.HasValue)
                    {
                        sb.Append("大過「" + LogDemeritList[0].DemeritB.Value.ToString() + "」");
                    }
                    if (LogDemeritList[0].DemeritB.HasValue)
                    {
                        sb.Append("小過「" + LogDemeritList[0].DemeritB.Value.ToString() + "」");
                    }
                    if (LogDemeritList[0].DemeritC.HasValue)
                    {
                        sb.Append("警告「" + LogDemeritList[0].DemeritC.Value.ToString() + "」");
                    }
                    sb.AppendLine("懲戒事由「" + LogDemeritList[0].Reason + "」");

                    sb.AppendLine("學生詳細資料：");
                    foreach (DemeritRecord each in LogDemeritList)
                    {
                        sb.Append("學生「" + each.Student.Name + "」");

                        if (each.Student.Class != null)
                        {
                            sb.Append("班級「" + each.Student.Class.Name + "」");
                        }
                        else
                        {
                            sb.Append("班級「」");
                        }

                        if (each.Student.SeatNo.HasValue)
                        {
                            sb.AppendLine("座號「" + each.Student.SeatNo.Value.ToString() + "」");
                        }
                        else
                        {
                            sb.AppendLine("座號「」");
                        }
                    }

                    ApplicationLog.Log("學務系統.懲戒資料", "批次新增學生懲戒資料", sb.ToString());
                    #endregion
                    MsgBox.Show("批次新增懲戒資料成功!");
                }
            }
            else
            {
                try
                {
                    Modify();
                    Demerit.Update(this._demeritRecordEditor);
                }
                catch (Exception ex)
                {
                    MsgBox.Show("修改懲戒記錄時發生錯誤: \n" + ex.Message);
                }

                #region 修改Log
                StringBuilder sb = new StringBuilder();
                sb.Append("學生「" + this._demeritRecordEditor.Student.Name + "」");
                sb.AppendLine("日期「" + this._demeritRecordEditor.OccurDate.ToShortDateString() + "」懲戒資料已修改。");
                sb.AppendLine("詳細資料：");
                sb.AppendLine("大過「" + DicBeforeData["大過"] + "」變更為「" + this._demeritRecordEditor.DemeritA.Value + "」");
                sb.AppendLine("小過「" + DicBeforeData["小過"] + "」變更為「" + this._demeritRecordEditor.DemeritB.Value + "」");
                sb.AppendLine("警告「" + DicBeforeData["警告"] + "」變更為「" + this._demeritRecordEditor.DemeritC.Value + "」");
                sb.AppendLine("懲戒事由「" + DicBeforeData["事由"] + "」變更為「" + this._demeritRecordEditor.Reason + "」");
                ApplicationLog.Log("學務系統.懲戒資料", "修改學生懲戒資料", "student", this._demeritRecordEditor.Student.ID, sb.ToString());
                #endregion
                MsgBox.Show("修改懲戒資料成功!");
            }

            this.Close();
            #endregion
        }

        private void cboSchoolYear_Validated(object sender, EventArgs e)
        {
            //_errorProvider.SetError(cboSchoolYear, null);
            //int i;
            //if (!int.TryParse(cboSchoolYear.Text, out i))
            //    _errorProvider.SetError(cboSchoolYear, "學年度必須為整數數字");
        }

        private void cboSemester_Validated(object sender, EventArgs e)
        {
            //_errorProvider.SetError(cboSemester, null);
            //if (cboSemester.Text != "1" && cboSemester.Text != "2")
            //    _errorProvider.SetError(cboSemester, "學期必須填入1或2");
        }

        private void txt1_Validated(object sender, EventArgs e)
        {
            this.Text_Validate(this.txt1);
        }

        private void txt2_Validated(object sender, EventArgs e)
        {
            this.Text_Validate(this.txt2);
        }

        private void txt3_Validated(object sender, EventArgs e)
        {
            this.Text_Validate(this.txt3);
        }

        //將TextBox的除錯動作合為單一處理程序
        private void Text_Validate(DevComponents.DotNetBar.Controls.TextBoxX txt)
        {
            _errorProvider.SetError(txt, null);
            if (string.IsNullOrEmpty(txt.Text))
                return;
            int i = 0;
            if (!int.TryParse(txt.Text, out i))
                _errorProvider.SetError(txt, "必須為整數數字");
            else
            {
                if (i < 0)
                    _errorProvider.SetError(txt, "不能輸入負數");
            }
        }

        //畫面離開
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //事由控制項在被改變時
        private void cboReasonRef_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboReasonRef.SelectedItem == null) return;
            KeyValuePair<string, string> kvp = (KeyValuePair<string, string>)cboReasonRef.SelectedItem;
            txtReason.Text = kvp.Value;
        }

        private string GetTextValue(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "0";
            return text;
        }

        private void Modify()
        {
            //把畫面的資料填回 DemeritRecordEditor 物件中
            this.FillDataToEditor(this._demeritRecordEditor);
        }

        private List<DemeritRecord> Insert()
        {
            List<DemeritRecord> newEditors = new List<DemeritRecord>();

            //對所有學生，都準備好相關的 DemeritRecordEditor物件
            foreach (StudentRecord sr in this._students)
            {
                DemeritRecord dre = new DemeritRecord();
                dre.RefStudentID = sr.ID;
                this.FillDataToEditor(dre);
                newEditors.Add(dre);
            }

            return newEditors;

        }

        //把畫面資料填到 Editor 中。
        private void FillDataToEditor(DemeritRecord editor)
        {
            #region 把畫面資料填到 Editor 中
            //把畫面的資料填回 DemeritRecordEditor 物件中

            if (Check_ischool_isSeniorOrJunior) //高中模式
            {
                if (cbMeritFlag.Checked) //如果留察
                {
                    editor.SchoolYear = intSchoolYear.Value;
                    editor.Semester = intSemester.Value;
                    editor.DemeritA = 0;
                    editor.DemeritB = 0;
                    editor.DemeritC = 0;
                    editor.Reason = txtReason.Text;
                    editor.MeritFlag = "2";

                    editor.ClearDate = null;
                    editor.Cleared = "";
                    editor.ClearReason = "";

                    editor.OccurDate = dateTimeInput1.Value;
                    if (dateTimeInput2.Text != "")
                    {
                        editor.RegisterDate = dateTimeInput2.Value;
                    }
                }
                else //不是留查
                {
                    editor.SchoolYear = intSchoolYear.Value;
                    editor.Semester = intSemester.Value;
                    editor.DemeritA = ChangeInt(txt1.Text);
                    editor.DemeritB = ChangeInt(txt2.Text);
                    editor.DemeritC = ChangeInt(txt3.Text);
                    editor.MeritFlag = "0";
                    editor.Reason = txtReason.Text;
                    editor.OccurDate = dateTimeInput1.Value;
                    if (dateTimeInput2.Text != "")
                    {
                        editor.RegisterDate = dateTimeInput2.Value;
                    }
                }

            }
            else //國中模式
            {
                editor.SchoolYear = intSchoolYear.Value;
                editor.Semester = intSemester.Value;
                editor.DemeritA = ChangeInt(txt1.Text);
                editor.DemeritB = ChangeInt(txt2.Text);
                editor.DemeritC = ChangeInt(txt3.Text);
                editor.Reason = txtReason.Text;
                editor.OccurDate = dateTimeInput1.Value;
                if (dateTimeInput2.Text != "")
                {
                    editor.RegisterDate = dateTimeInput2.Value;
                }
            }

            #endregion
        }

        private int ChangeInt(string txt)
        {
            int xParse;
            if (int.TryParse(txt, out xParse))
            {
                return xParse;
            }
            else
            {
                return 0;
            }
        }

        private void cboReasonRef_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtReason.Focus();
                txtReason.Select(txtReason.Text.Length + 1, 0);
            }
        }

        private void cbMeritFlag_CheckedChanged(object sender, EventArgs e)
        {
            if (cbMeritFlag.Checked == true)
            {
                txt1.Enabled = false;
                txt2.Enabled = false;
                txt3.Enabled = false;
                txt1.Text = "";
                txt2.Text = "";
                txt3.Text = "";
            }
            else
            {
                txt1.Enabled = true;
                txt2.Enabled = true;
                txt3.Enabled = true;

            }
        }

        private void txtReason_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                string reasonValue = "";
                List<string> list = new List<string>();
                string[] reasonList = txtReason.Text.Split(',');
                foreach (string each in reasonList)
                {
                    string each1 = each.Replace("\r\n", "");
                    if (ResonDic.ContainsKey(each1))
                    {
                        list.Add(ResonDic[each1]);
                    }
                    else
                    {
                        list.Add(each1);
                    }
                }

                reasonValue = string.Join(",", list);

                txtReason.Text = reasonValue;
            }
        }
    }
}