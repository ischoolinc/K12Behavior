using System;
using System.Xml;
using FISCA.DSAUtil;
using SmartSchool.API.PlugIn;
using SmartSchool.Common;
using SmartSchool.Customization.Data;
using SmartSchool.Customization.Data.StudentExtension;
using SmartSchool.AccessControl;
using System.Text;
using System.Data;
using System.Collections.Generic;

namespace K12.Behavior
{
    [FeatureCode("Button0270")]
    class ImportDiscipline : SmartSchool.API.PlugIn.Import.Importer
    {
        public ImportDiscipline()
        {
            this.Text = "匯入獎懲記錄";
        }

        public override void InitializeImport(SmartSchool.API.PlugIn.Import.ImportWizard wizard)
        {
            AccessHelper accessHelper = new AccessHelper();
            VirtualRadioButton chose1 = new VirtualRadioButton("比對事由變更獎懲次數", false);
            VirtualRadioButton chose2 = new VirtualRadioButton("比對獎懲次數變更事由", false);
            chose1.CheckedChanged += delegate
            {
                if (chose1.Checked)
                {
                    wizard.RequiredFields.Clear();
                    wizard.RequiredFields.AddRange("學年度", "學期", "日期", "事由");
                }
            };
            chose2.CheckedChanged += delegate
            {
                if (chose2.Checked)
                {
                    wizard.RequiredFields.Clear();
                    wizard.RequiredFields.AddRange("學年度", "學期", "日期", "大功", "小功", "嘉獎", "大過", "小過", "警告");
                }
            };
            wizard.ImportableFields.AddRange("學年度", "學期", "日期", "大功", "小功", "嘉獎", "大過", "小過", "警告", "事由", "是否銷過", "銷過日期", "銷過事由", "留校察看", "登錄日期", "備註");
            wizard.Options.AddRange(chose1, chose2);
            chose1.Checked = true;
            wizard.PackageLimit = 400;
            bool allPass = true;
            wizard.ValidateStart += delegate (object sender, SmartSchool.API.PlugIn.Import.ValidateStartEventArgs e)
            {
                accessHelper.StudentHelper.FillReward(accessHelper.StudentHelper.GetStudents(e.List));
                allPass = true;
            };
            int insertRecords = 0;
            int updataRecords = 0;
            wizard.ValidateRow += delegate (object sender, SmartSchool.API.PlugIn.Import.ValidateRowEventArgs e)
            {
                #region 驗證資料
                bool pass = true;
                int schoolYear, semester;
                DateTime occurdate;
                bool isInsert = false;
                bool isUpdata = false;
                #region 驗共同必填欄位
                if (!int.TryParse(e.Data["學年度"], out schoolYear))
                {
                    e.ErrorFields.Add("學年度", "必需輸入數字");
                    pass = false;
                }
                if (!int.TryParse(e.Data["學期"], out semester))
                {
                    e.ErrorFields.Add("學期", "必需輸入數字");
                    pass = false;
                }
                if (!DateTime.TryParse(e.Data["日期"], out occurdate))
                {
                    e.ErrorFields.Add("日期", "輸入格式為 西元年//月//日");
                    pass = false;
                }
                #endregion
                if (!pass)
                {
                    allPass = false;
                    return;
                }
                if (chose1.Checked)
                {
                    #region 以事由為Key更新
                    string reason = e.Data["事由"];
                    int match = 0;
                    foreach (RewardInfo rewardInfo in accessHelper.StudentHelper.GetStudent(e.Data.ID).RewardList)
                    {
                        if (rewardInfo.SchoolYear == schoolYear && rewardInfo.Semester == semester && rewardInfo.OccurDate == occurdate && rewardInfo.OccurReason == reason)
                            match++;
                    }
                    if (match > 1)
                    {
                        e.ErrorMessage = "系統發現此事由在同一天中存在兩筆重複資料，無法進行更新，建議您手動處裡此筆變更。";
                        pass = false;
                    }
                    if (match == 0)
                    {
                        isInsert = true;
                    }
                    else
                    {
                        isUpdata = true;
                    }
                    #endregion
                }
                if (chose2.Checked)
                {
                    #region 以次數為Key更新
                    int awardA = 0;
                    int awardB = 0;
                    int awardC = 0;
                    int faultA = 0;
                    int faultB = 0;
                    int faultC = 0;
                    #region 驗證必填欄位
                    if (e.Data["大功"] != "" && !int.TryParse(e.Data["大功"], out awardA))
                    {
                        e.ErrorFields.Add("大功", "必需輸入數字");
                        pass = false;
                    }
                    if (e.Data["小功"] != "" && !int.TryParse(e.Data["小功"], out awardB))
                    {
                        e.ErrorFields.Add("小功", "必需輸入數字");
                        pass = false;
                    }
                    if (e.Data["嘉獎"] != "" && !int.TryParse(e.Data["嘉獎"], out awardC))
                    {
                        e.ErrorFields.Add("嘉獎", "必需輸入數字");
                        pass = false;
                    }
                    if (e.Data["大過"] != "" && !int.TryParse(e.Data["大過"], out faultA))
                    {
                        e.ErrorFields.Add("大過", "必需輸入數字");
                        pass = false;
                    }
                    if (e.Data["小過"] != "" && !int.TryParse(e.Data["小過"], out faultB))
                    {
                        e.ErrorFields.Add("小過", "必需輸入數字");
                        pass = false;
                    }
                    if (e.Data["警告"] != "" && !int.TryParse(e.Data["警告"], out faultC))
                    {
                        e.ErrorFields.Add("警告", "必需輸入數字");
                        pass = false;
                    }
                    #endregion
                    if (!pass)
                    {
                        return;
                    }
                    int match = 0;
                    #region 檢查重複
                    foreach (RewardInfo rewardInfo in accessHelper.StudentHelper.GetStudent(e.Data.ID).RewardList)
                    {
                        if (rewardInfo.SchoolYear == schoolYear &&
                            rewardInfo.Semester == semester &&
                            rewardInfo.OccurDate == occurdate &&
                            rewardInfo.AwardA == awardA &&
                            rewardInfo.AwardB == awardB &&
                            rewardInfo.AwardC == awardC &&
                            rewardInfo.FaultA == faultA &&
                            rewardInfo.FaultB == faultB &&
                            rewardInfo.FaultC == faultC)
                            match++;
                    }
                    #endregion
                    if (match > 1)
                    {
                        e.ErrorMessage = "系統發現此獎懲次數在同一天中存在兩筆重複資料，無法進行更新，建議您手動處裡此筆變更。";
                        pass = false;
                    }
                    if (match == 0)
                    {
                        isInsert = true;
                    }
                    else
                    {
                        isUpdata = true;
                    }
                    #endregion
                }
                if (!pass)
                {
                    allPass = false;
                    return;
                }
                #region 驗證可選則欄位值
                int integer;
                DateTime dateTime;
                bool hasAward = false, hasFault = false, IsErrorAward = false;

                bool IsIn_School = false; //是否"是"留查資料

                foreach (string field in e.SelectFields)
                {
                    switch (field)
                    {
                        case "大功":
                        case "小功":
                        case "嘉獎":
                            if (e.Data[field] != "")
                            {
                                if (!int.TryParse(e.Data[field], out integer))
                                {
                                    e.ErrorFields.Add(field, "必需輸入數字");
                                    pass = false;
                                }
                                else
                                {
                                    if (integer < 0)
                                    {
                                        e.ErrorFields.Add(field, "不可為負數");
                                        pass = false;

                                    }
                                    else
                                    {
                                        IsErrorAward = false;
                                        hasAward |= integer > 0;
                                    }
                                }
                            }
                            break;
                        case "大過":
                        case "小過":
                        case "警告":
                            if (e.Data[field] != "")
                            {
                                if (!int.TryParse(e.Data[field], out integer))
                                {
                                    e.ErrorFields.Add(field, "必需輸入數字");
                                    pass = false;
                                }
                                else
                                {
                                    if (integer < 0)
                                    {
                                        e.ErrorFields.Add(field, "不可為負數");
                                        pass = false;

                                    }
                                    else
                                    {
                                        IsErrorAward = false;
                                        hasFault |= integer > 0;
                                    }
                                }
                            }
                            break;
                        case "銷過日期":
                            if (e.Data[field] != "" && !DateTime.TryParse(e.Data[field], out dateTime))
                            {
                                e.ErrorFields.Add(field, "輸入格式為 西元年//月//日");
                                pass = false;
                            }
                            break;
                        case "是否銷過":
                            if (e.Data[field] != "" && e.Data[field] != "是" && e.Data[field] != "否")
                            {
                                e.ErrorFields.Add(field, "如果為是請填入\"是\"否則請保留空白或填入\"否\"");
                                pass = false;
                            }
                            break;
                        case "留校察看":
                            if (e.Data[field] != "" && e.Data[field] != "是" && e.Data[field] != "否")
                            {
                                e.ErrorFields.Add(field, "如果為是請填入\"是\"否則請保留空白或填入\"否\"");
                                pass = false;
                            }
                            else
                            {
                                if (e.Data[field] == "是") //是留查資料
                                {
                                    IsIn_School = true;
                                }
                            }
                            break;
                        case "登錄日期":
                            if (e.Data[field] != "" && !DateTime.TryParse(e.Data[field], out dateTime))
                            {
                                e.ErrorFields.Add(field, "輸入格式為 西元年//月//日");
                                pass = false;
                            }
                            break;
                    }
                }

                bool 檢查是否獎都是0 = false;
                bool 檢查是否懲都是0 = false;
                bool 檢查獎是空值 = false;
                bool 檢查懲是空值 = false;
                int 大功A = 0;
                int 小功A = 0;
                int 嘉獎A = 0;
                int 大過A = 0;
                int 小過A = 0;
                int 警告A = 0;
                if (e.SelectFields.Contains("大功") && e.SelectFields.Contains("小功") && e.SelectFields.Contains("嘉獎"))
                {
                    #region 獎
                    //,是否都是空值
                    if (!string.IsNullOrEmpty(e.Data["大功"]) || !string.IsNullOrEmpty(e.Data["小功"]) || !string.IsNullOrEmpty(e.Data["嘉獎"]))
                    {

                        int.TryParse(e.Data["大功"], out 大功A);
                        int.TryParse(e.Data["小功"], out 小功A);
                        int.TryParse(e.Data["嘉獎"], out 嘉獎A);

                        //相加是否為0
                        if (大功A == 0 && 小功A == 0 && 嘉獎A == 0)
                        {
                            檢查是否獎都是0 = true;
                        }
                    }
                    else
                    {
                        檢查獎是空值 = true;
                    }
                    #endregion
                }

                if (e.SelectFields.Contains("大過") && e.SelectFields.Contains("小過") && e.SelectFields.Contains("警告"))
                {
                    #region 懲
                    //是否都是空值
                    if (!string.IsNullOrEmpty(e.Data["大過"]) || !string.IsNullOrEmpty(e.Data["小過"]) || !string.IsNullOrEmpty(e.Data["警告"]))
                    {
                        int.TryParse(e.Data["大過"], out 大過A);
                        int.TryParse(e.Data["小過"], out 小過A);
                        int.TryParse(e.Data["警告"], out 警告A);

                        if (大過A == 0 && 小過A == 0 && 警告A == 0)
                        {
                            檢查是否懲都是0 = true;
                        }
                    }
                    else
                    {
                        檢查懲是空值 = true;
                    }
                    #endregion
                }

                if (!IsIn_School) //留查資料,不需判斷
                {
                    if (檢查是否獎都是0 && 檢查是否懲都是0)
                    {
                        e.ErrorMessage = "獎懲皆為0,系統無法判斷此類型資料。";
                        pass = false;
                    }
                    else if (檢查是否獎都是0 && 檢查懲是空值)
                    {
                        e.ErrorMessage = "獎懲皆為0,系統無法判斷此類型資料。";
                        pass = false;
                    }
                    else if (檢查是否懲都是0 && 檢查獎是空值)
                    {
                        e.ErrorMessage = "獎懲皆為0,系統無法判斷此類型資料。";
                        pass = false;
                    }

                    if (IsErrorAward)
                    {
                        e.ErrorMessage = "獎勵與懲戒不可同時未輸入內容!!";
                        pass = false;
                    }
                }

                if (hasAward && hasFault)
                {
                    e.ErrorMessage = "系統愚昧無法理解同時記功又記過的情況。";
                    pass = false;
                }
                if (!pass && isInsert && (!e.SelectFields.Contains("留校察看") || e.Data["留校察看"] != "是") && (!hasFault && !hasAward))
                {
                    e.ErrorMessage = "無法新增沒有獎懲的記錄。";
                    pass = false;
                }
                if (pass && isInsert)
                    insertRecords++;

                if (pass && isUpdata)
                    updataRecords++;

                #endregion
                if (!pass)
                {
                    allPass = false;
                }
                #endregion
            };
            wizard.ValidateComplete += delegate
            {
                StringBuilder sb = new StringBuilder();
                if (allPass && insertRecords > 0)
                {
                    sb.AppendLine("新增" + insertRecords + "筆獎懲記錄");
                }

                if (allPass && updataRecords > 0)
                {
                    sb.AppendLine("更新" + updataRecords + "筆獎懲記錄");
                }

                if (sb.ToString() != "")
                {
                    sb.AppendLine("\n如與資料筆數不符請勿繼續。");
                    MsgBox.Show(sb.ToString(), "新增與更新獎懲", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                }

                insertRecords = 0;
                updataRecords = 0;
            };
            wizard.ImportPackage += delegate (object sender, SmartSchool.API.PlugIn.Import.ImportPackageEventArgs e)
            {
                bool hasUpdate = false, hasInsert = false;
                DSXmlHelper updateHelper = new DSXmlHelper("UpdateRequest");
                DSXmlHelper insertHelper = new DSXmlHelper("InsertRequest");

                //2014/3/6 新增Log記錄「" + name + "」
                StringBuilder Log_sb = new StringBuilder();
                if (chose1.Checked)
                {
                    Log_sb.AppendLine("「以事由為鍵值匯入」");
                    Log_sb.AppendLine("");
                }
                if (chose2.Checked)
                {
                    Log_sb.AppendLine("「以支數為鍵值匯入」");
                    Log_sb.AppendLine("");
                }

                foreach (RowData row in e.Items)
                {
                    #region row

                    int schoolYear = int.Parse(row["學年度"]);
                    int semester = int.Parse(row["學期"]);

                    //20240319 將日期的時分秒Parse掉 - Dylan
                    DateTime date2 = DateTime.Parse(row["日期"]);
                    DateTime occurdate = DateTime.Parse(date2.ToString("yyyy/MM/dd"));

                    DateTime rd = new DateTime();
                    string register = e.ImportFields.Contains("登錄日期") ? row["登錄日期"] : "";
                    DateTime RegisterDate = DateTime.Today;
                    if (DateTime.TryParse(register, out rd))
                    {
                        RegisterDate = DateTime.Parse(register);
                    }


                    if (chose1.Checked)
                    {
                        #region 以事由為Key更新

                        #region Title

                        bool isAward;
                        int awardA = 0;
                        int awardB = 0;
                        int awardC = 0;
                        int faultA = 0;
                        int faultB = 0;
                        int faultC = 0;
                        bool cleared = false;
                        DateTime cleardate = DateTime.Today;
                        string clearreason = "";
                        bool ultimateAdmonition = false;

                        if (row.ContainsKey("大功"))
                            awardA = (row["大功"] == "") ? 0 : int.Parse(row["大功"]);
                        if (row.ContainsKey("小功"))
                            awardB = (row["小功"] == "") ? 0 : int.Parse(row["小功"]);
                        if (row.ContainsKey("嘉獎"))
                            awardC = (row["嘉獎"] == "") ? 0 : int.Parse(row["嘉獎"]);
                        if (row.ContainsKey("大過"))
                            faultA = (row["大過"] == "") ? 0 : int.Parse(row["大過"]);
                        if (row.ContainsKey("小過"))
                            faultB = (row["小過"] == "") ? 0 : int.Parse(row["小過"]);
                        if (row.ContainsKey("警告"))
                            faultC = (row["警告"] == "") ? 0 : int.Parse(row["警告"]);
                        cleared = e.ImportFields.Contains("是否銷過") ?
                            ((row["是否銷過"] == "是") ? true : false) : false;

                        cleardate = (e.ImportFields.Contains("銷過日期") && row["銷過日期"] != "") ?
                            DateTime.Parse(row["銷過日期"]) : DateTime.Now;

                        clearreason = e.ImportFields.Contains("銷過事由") ?
                            row["銷過事由"] : "";

                        ultimateAdmonition = e.ImportFields.Contains("留校察看") ?
                            ((row["留校察看"] == "是") ? true : false) : false;

                        string reason = row.ContainsKey("事由") ? row["事由"] : "";
                        string remark = row.ContainsKey("備註") ? row["備註"] : "";
                        bool match = false;

                        #endregion

                        foreach (RewardInfo rewardInfo in accessHelper.StudentHelper.GetStudent(row.ID).RewardList)
                        {
                            #region RewardInfo

                            if (rewardInfo.SchoolYear == schoolYear && rewardInfo.Semester == semester && rewardInfo.OccurDate == occurdate && rewardInfo.OccurReason == reason)
                            {
                                match = true;
                                #region 其他項目
                                cleared = e.ImportFields.Contains("是否銷過") ?
                                    ((row["是否銷過"] == "是") ? true : false) :
                                    rewardInfo.Cleared;

                                cleardate = (e.ImportFields.Contains("銷過日期") && row["銷過日期"] != "") ?
                                    DateTime.Parse(row["銷過日期"]) :
                                    rewardInfo.ClearDate;

                                clearreason = e.ImportFields.Contains("銷過事由") ?
                                    row["銷過事由"] :
                                    rewardInfo.ClearReason;

                                ultimateAdmonition = e.ImportFields.Contains("留校察看") ?
                                    ((row["留校察看"] == "是") ? true : false) :
                                    rewardInfo.UltimateAdmonition;
                                #endregion
                                DSXmlHelper h = new DSXmlHelper("Discipline");

                                isAward = awardA + awardB + awardC > 0;
                                if (isAward) //是否為獎懲
                                {
                                    XmlElement element = h.AddElement("Merit");
                                    element.SetAttribute("A", awardA.ToString());
                                    element.SetAttribute("B", awardB.ToString());
                                    element.SetAttribute("C", awardC.ToString());
                                }
                                else
                                {
                                    XmlElement element = h.AddElement("Demerit");

                                    if (!ultimateAdmonition) //是否為留查資料
                                    {
                                        element.SetAttribute("A", faultA.ToString());
                                        element.SetAttribute("B", faultB.ToString());
                                        element.SetAttribute("C", faultC.ToString());
                                        element.SetAttribute("Cleared", cleared ? "是" : string.Empty);
                                        element.SetAttribute("ClearDate", cleared ? cleardate.ToShortDateString() : "");
                                        element.SetAttribute("ClearReason", clearreason);

                                    }
                                    else
                                    {
                                        // "是" 留查資料
                                        element.SetAttribute("A", "0");
                                        element.SetAttribute("B", "0");
                                        element.SetAttribute("C", "0");
                                        element.SetAttribute("Cleared", string.Empty);
                                        element.SetAttribute("ClearDate", string.Empty);
                                        element.SetAttribute("ClearReason", string.Empty);
                                    }
                                }

                                updateHelper.AddElement("Discipline");
                                updateHelper.AddElement("Discipline", "Field");
                                updateHelper.AddElement("Discipline/Field", "MeritFlag", ultimateAdmonition ? "2" : isAward ? "1" : "0");
                                updateHelper.AddElement("Discipline/Field", "RegisterDate", RegisterDate.ToShortDateString());
                                updateHelper.AddElement("Discipline/Field", "Detail", h.GetRawXml(), true);
                                updateHelper.AddElement("Discipline/Field", "Remark", remark); //備註
                                updateHelper.AddElement("Discipline", "Condition");
                                updateHelper.AddElement("Discipline/Condition", "ID", rewardInfo.Detail.GetAttribute("ID"));

                                hasUpdate = true;
                                break;
                            }

                            #endregion
                        }

                        if (!match)
                        {
                            #region match

                            DSXmlHelper h = new DSXmlHelper("Discipline");
                            isAward = awardA + awardB + awardC > 0;
                            if (isAward)
                            {
                                XmlElement element = h.AddElement("Merit");
                                element.SetAttribute("A", awardA.ToString());
                                element.SetAttribute("B", awardB.ToString());
                                element.SetAttribute("C", awardC.ToString());
                            }
                            else
                            {
                                XmlElement element = h.AddElement("Demerit");
                                if (!ultimateAdmonition)
                                {
                                    element.SetAttribute("A", faultA.ToString());
                                    element.SetAttribute("B", faultB.ToString());
                                    element.SetAttribute("C", faultC.ToString());
                                    element.SetAttribute("Cleared", cleared ? "是" : string.Empty);
                                    element.SetAttribute("ClearDate", cleared ? cleardate.ToShortDateString() : "");
                                    element.SetAttribute("ClearReason", clearreason);
                                }
                                else
                                {
                                    element.SetAttribute("A", "0");
                                    element.SetAttribute("B", "0");
                                    element.SetAttribute("C", "0");
                                    element.SetAttribute("Cleared", string.Empty);
                                    element.SetAttribute("ClearDate", string.Empty);
                                    element.SetAttribute("ClearReason", string.Empty);
                                }
                            }
                            insertHelper.AddElement("Discipline");
                            insertHelper.AddElement("Discipline", "RefStudentID", row.ID);
                            insertHelper.AddElement("Discipline", "SchoolYear", schoolYear.ToString());
                            insertHelper.AddElement("Discipline", "Semester", semester.ToString());
                            insertHelper.AddElement("Discipline", "OccurDate", occurdate.ToShortDateString());
                            insertHelper.AddElement("Discipline", "RegisterDate", RegisterDate.ToShortDateString());
                            insertHelper.AddElement("Discipline", "Reason", reason);
                            insertHelper.AddElement("Discipline", "Remark", remark);
                            insertHelper.AddElement("Discipline", "MeritFlag", ultimateAdmonition ? "2" : isAward ? "1" : "0");
                            insertHelper.AddElement("Discipline", "Type", "1");
                            insertHelper.AddElement("Discipline", "Detail", h.GetRawXml(), true);

                            hasInsert = true;

                            #endregion
                        }
                        #endregion
                    }
                    if (chose2.Checked)
                    {
                        #region 以次數為Key更新

                        #region Title

                        bool isAward;
                        int awardA = 0;
                        int awardB = 0;
                        int awardC = 0;
                        int faultA = 0;
                        int faultB = 0;
                        int faultC = 0;
                        bool cleared = false;
                        DateTime cleardate = DateTime.Today;
                        string clearreason = "";
                        bool ultimateAdmonition = false;
                        string reason = row.ContainsKey("事由") ? row["事由"] : "";
                        string remark = row.ContainsKey("備註") ? row["備註"] : "";

                        if (row.ContainsKey("大功"))
                            awardA = (row["大功"] == "") ? 0 : int.Parse(row["大功"]);
                        if (row.ContainsKey("小功"))
                            awardB = (row["小功"] == "") ? 0 : int.Parse(row["小功"]);
                        if (row.ContainsKey("嘉獎"))
                            awardC = (row["嘉獎"] == "") ? 0 : int.Parse(row["嘉獎"]);
                        if (row.ContainsKey("大過"))
                            faultA = (row["大過"] == "") ? 0 : int.Parse(row["大過"]);
                        if (row.ContainsKey("小過"))
                            faultB = (row["小過"] == "") ? 0 : int.Parse(row["小過"]);
                        if (row.ContainsKey("警告"))
                            faultC = (row["警告"] == "") ? 0 : int.Parse(row["警告"]);
                        cleared = e.ImportFields.Contains("是否銷過") ?
                            ((row["是否銷過"] == "是") ? true : false) : false;

                        cleardate = (e.ImportFields.Contains("銷過日期") && row["銷過日期"] != "") ?
                            DateTime.Parse(row["銷過日期"]) : DateTime.Now;

                        clearreason = e.ImportFields.Contains("銷過事由") ?
                            row["銷過事由"] : "";

                        ultimateAdmonition = e.ImportFields.Contains("留校察看") ?
                            ((row["留校察看"] == "是") ? true : false) : false;

                        bool match = false;

                        #endregion

                        foreach (RewardInfo rewardInfo in accessHelper.StudentHelper.GetStudent(row.ID).RewardList)
                        {
                            #region RewardInfo

                            if (rewardInfo.SchoolYear == schoolYear &&
                                                    rewardInfo.Semester == semester &&
                                                    rewardInfo.OccurDate == occurdate &&
                                                    rewardInfo.AwardA == awardA &&
                                                    rewardInfo.AwardB == awardB &&
                                                    rewardInfo.AwardC == awardC &&
                                                    rewardInfo.FaultA == faultA &&
                                                    rewardInfo.FaultB == faultB &&
                                                    rewardInfo.FaultC == faultC)
                            {
                                match = true;
                                #region 其他項目
                                reason = e.ImportFields.Contains("事由") ? row["事由"] : rewardInfo.OccurReason;

                                cleared = e.ImportFields.Contains("是否銷過") ?
                                    ((row["是否銷過"] == "是") ? true : false) :
                                    rewardInfo.Cleared;

                                cleardate = (e.ImportFields.Contains("銷過日期") && row["銷過日期"] != "") ?
                                    DateTime.Parse(row["銷過日期"]) :
                                    rewardInfo.ClearDate;

                                clearreason = e.ImportFields.Contains("銷過事由") ?
                                    row["銷過事由"] :
                                    rewardInfo.ClearReason;

                                ultimateAdmonition = e.ImportFields.Contains("留校察看") ?
                                    ((row["留校察看"] == "是") ? true : false) :
                                    rewardInfo.UltimateAdmonition;
                                #endregion
                                DSXmlHelper h = new DSXmlHelper("Discipline");
                                isAward = awardA + awardB + awardC > 0;
                                if (isAward)
                                {
                                    XmlElement element = h.AddElement("Merit");
                                    element.SetAttribute("A", awardA.ToString());
                                    element.SetAttribute("B", awardB.ToString());
                                    element.SetAttribute("C", awardC.ToString());
                                }
                                else
                                {
                                    XmlElement element = h.AddElement("Demerit");
                                    if (!ultimateAdmonition)
                                    {
                                        element.SetAttribute("A", faultA.ToString());
                                        element.SetAttribute("B", faultB.ToString());
                                        element.SetAttribute("C", faultC.ToString());
                                        element.SetAttribute("Cleared", cleared ? "是" : string.Empty);
                                        element.SetAttribute("ClearDate", cleared ? cleardate.ToShortDateString() : "");
                                        element.SetAttribute("ClearReason", clearreason);
                                    }
                                    else
                                    {
                                        element.SetAttribute("A", "0");
                                        element.SetAttribute("B", "0");
                                        element.SetAttribute("C", "0");
                                        element.SetAttribute("Cleared", string.Empty);
                                        element.SetAttribute("ClearDate", string.Empty);
                                        element.SetAttribute("ClearReason", string.Empty);
                                    }

                                }
                                updateHelper.AddElement("Discipline");
                                updateHelper.AddElement("Discipline", "Field");
                                updateHelper.AddElement("Discipline/Field", "MeritFlag", ultimateAdmonition ? "2" : isAward ? "1" : "0");
                                updateHelper.AddElement("Discipline/Field", "RegisterDate", RegisterDate.ToShortDateString());
                                updateHelper.AddElement("Discipline/Field", "Detail", h.GetRawXml(), true);
                                updateHelper.AddElement("Discipline/Field", "Reason", reason);
                                updateHelper.AddElement("Discipline/Field", "Remark", remark);
                                updateHelper.AddElement("Discipline", "Condition");
                                updateHelper.AddElement("Discipline/Condition", "ID", rewardInfo.Detail.GetAttribute("ID"));

                                hasUpdate = true;
                                break;
                            }

                            #endregion
                        }

                        if (!match)
                        {
                            #region match

                            DSXmlHelper h = new DSXmlHelper("Discipline");
                            isAward = awardA + awardB + awardC > 0;
                            if (isAward)
                            {
                                XmlElement element = h.AddElement("Merit");
                                element.SetAttribute("A", awardA.ToString());
                                element.SetAttribute("B", awardB.ToString());
                                element.SetAttribute("C", awardC.ToString());
                            }
                            else
                            {
                                XmlElement element = h.AddElement("Demerit");
                                if (!ultimateAdmonition)
                                {
                                    element.SetAttribute("A", faultA.ToString());
                                    element.SetAttribute("B", faultB.ToString());
                                    element.SetAttribute("C", faultC.ToString());
                                    element.SetAttribute("Cleared", cleared ? "是" : string.Empty);
                                    element.SetAttribute("ClearDate", cleared ? cleardate.ToShortDateString() : "");
                                    element.SetAttribute("ClearReason", clearreason);
                                }
                                else
                                {
                                    element.SetAttribute("A", "0");
                                    element.SetAttribute("B", "0");
                                    element.SetAttribute("C", "0");
                                    element.SetAttribute("Cleared", string.Empty);
                                    element.SetAttribute("ClearDate", string.Empty);
                                    element.SetAttribute("ClearReason", string.Empty);
                                }
                            }
                            insertHelper.AddElement("Discipline");
                            insertHelper.AddElement("Discipline", "RefStudentID", row.ID);
                            insertHelper.AddElement("Discipline", "SchoolYear", schoolYear.ToString());
                            insertHelper.AddElement("Discipline", "Semester", semester.ToString());
                            insertHelper.AddElement("Discipline", "OccurDate", occurdate.ToShortDateString());
                            insertHelper.AddElement("Discipline", "RegisterDate", RegisterDate.ToShortDateString());
                            insertHelper.AddElement("Discipline", "Reason", reason);
                            insertHelper.AddElement("Discipline", "Remark", remark);
                            insertHelper.AddElement("Discipline", "MeritFlag", ultimateAdmonition ? "2" : isAward ? "1" : "0");
                            insertHelper.AddElement("Discipline", "Type", "1");
                            insertHelper.AddElement("Discipline", "Detail", h.GetRawXml(), true);

                            hasInsert = true;

                            #endregion
                        }
                        #endregion
                    }

                    #endregion
                }

                if (hasInsert)
                {
                    SmartSchool.Feature.Student.EditDiscipline.Insert(new DSRequest(insertHelper));
                    Log_sb.AppendLine(GetInsertString(insertHelper, "新增"));
                }

                if (hasUpdate)
                {
                    SmartSchool.Feature.Student.EditDiscipline.Update(new DSRequest(updateHelper));
                    Log_sb.AppendLine(GetUpdataString(updateHelper, "更新"));
                }

                if (hasUpdate || hasInsert)
                {
                    FISCA.LogAgent.ApplicationLog.Log("匯入獎懲記錄", "新增或更新", Log_sb.ToString());
                }
            };
        }

        /// <summary>
        /// 取得新增Log
        /// </summary>
        string GetInsertString(DSXmlHelper helper, string state)
        {
            StringBuilder sb = new StringBuilder();
            int count = 0;
            foreach (XmlElement xml in helper.GetElements("Discipline"))
            {
                K12.Data.StudentRecord sr = K12.Data.Student.SelectByID(xml.SelectSingleNode("RefStudentID").InnerText);
                if (sr != null)
                {
                    string MeritFlag = xml.SelectSingleNode("MeritFlag").InnerText;
                    string name = sr.Name;
                    string class_name = sr.Class != null ? sr.Class.Name : "";
                    string seat_no = sr.SeatNo.HasValue ? sr.SeatNo.Value.ToString() : "";

                    string OccurDate = xml.SelectSingleNode("OccurDate").InnerText;
                    DateTime dtime = DateTime.Parse(OccurDate);

                    string Reason = xml.SelectSingleNode("Reason").InnerText;
                    string Remark = xml.SelectSingleNode("Remark").InnerText;

                    string MeritA = "0";
                    string MeritB = "0";
                    string MeritC = "0";
                    string DemeritA = "0";
                    string DemeritB = "0";
                    string DemeritC = "0";

                    XmlElement Merit = (XmlElement)xml.SelectSingleNode("Detail//Discipline//Merit");
                    if (Merit != null)
                    {
                        MeritA = Merit.GetAttribute("A");
                        MeritB = Merit.GetAttribute("B");
                        MeritC = Merit.GetAttribute("C");
                    }

                    XmlElement Demerit = (XmlElement)xml.SelectSingleNode("Detail//Discipline//Demerit");
                    if (Demerit != null)
                    {
                        DemeritA = Demerit.GetAttribute("A");
                        DemeritB = Demerit.GetAttribute("B");
                        DemeritC = Demerit.GetAttribute("C");
                    }

                    if (MeritFlag == "1")
                    {
                        sb.AppendLine("獎勵：");
                        sb.AppendLine("班級「" + class_name + "」座號「" + seat_no + "」姓名「" + name + "」");
                        sb.AppendLine("日期「" + dtime.ToShortDateString() + "」");
                        sb.AppendLine("大功「" + MeritA + "」小功「" + MeritB + "」嘉獎「" + MeritC + "」");
                    }
                    else if (MeritFlag == "0")
                    {
                        sb.AppendLine("懲戒：");
                        sb.AppendLine("班級「" + class_name + "」座號「" + seat_no + "」姓名「" + name + "」");
                        sb.AppendLine("日期「" + dtime.ToShortDateString() + "」");
                        sb.AppendLine("大過「" + DemeritA + "」小過「" + DemeritB + "」警告「" + DemeritC + "」");
                    }
                    else if (MeritFlag == "2")
                    {
                        sb.AppendLine("留查：");
                        sb.AppendLine("班級「" + class_name + "」座號「" + seat_no + "」姓名「" + name + "」");
                        sb.AppendLine("日期「" + dtime.ToShortDateString() + "」");
                        sb.AppendLine("「留察資料支數為0」");
                    }

                    sb.AppendLine("事由「" + Reason + "」");
                    sb.AppendLine("備註「" + Remark + "」");
                    sb.AppendLine("");
                    count++;
                }
            }
            sb.AppendLine(state + "「" + count + "」筆資料");
            return sb.ToString();
        }

        /// <summary>
        /// 取得更新Log
        /// </summary>
        string GetUpdataString(DSXmlHelper helper, string state)
        {
            StringBuilder sb = new StringBuilder();
            int count = 0;
            FISCA.Data.QueryHelper _q = new FISCA.Data.QueryHelper();

            List<string> disList = new List<string>();
            foreach (XmlElement each in helper.GetElements("Discipline//Condition//ID"))
            {
                if (!disList.Contains(each.InnerText))
                {
                    disList.Add(each.InnerText);
                    count++;
                }
            }

            DataTable dt = _q.Select("select * from discipline where id in ('" + string.Join("','", disList) + "')");
            foreach (DataRow row in dt.Rows)
            {
                string RefStudentID = "" + row["ref_student_id"];
                string OccurDate = "" + row["occur_date"];
                DateTime dtime = DateTime.Parse(OccurDate);

                string Reason = "" + row["reason"];
                string Remark = "" + row["remark"];
                string MeritFlag = "" + row["merit_Flag"];

                XmlElement xml = XmlHelper.LoadXml("" + row["detail"]);

                string MeritA = "0";
                string MeritB = "0";
                string MeritC = "0";
                string DemeritA = "0";
                string DemeritB = "0";
                string DemeritC = "0";

                XmlElement Merit = (XmlElement)xml.SelectSingleNode("Merit");
                if (Merit != null)
                {
                    MeritA = Merit.GetAttribute("A");
                    MeritB = Merit.GetAttribute("B");
                    MeritC = Merit.GetAttribute("C");
                }

                XmlElement Demerit = (XmlElement)xml.SelectSingleNode("Demerit");
                if (Demerit != null)
                {
                    DemeritA = Demerit.GetAttribute("A");
                    DemeritB = Demerit.GetAttribute("B");
                    DemeritC = Demerit.GetAttribute("C");
                }

                K12.Data.StudentRecord sr = K12.Data.Student.SelectByID(RefStudentID);
                if (sr != null)
                {
                    string name = sr.Name;
                    string class_name = sr.Class != null ? sr.Class.Name : "";
                    string seat_no = sr.SeatNo.HasValue ? sr.SeatNo.Value.ToString() : "";
                    string date = dtime.ToShortDateString();

                    if (MeritFlag == "1")
                    {
                        sb.AppendLine("獎勵：");
                        sb.AppendLine("班級「" + class_name + "」座號「" + seat_no + "」姓名「" + name + "」");
                        sb.AppendLine("日期「" + dtime.ToShortDateString() + "」");
                        sb.AppendLine("大功「" + MeritA + "」小功「" + MeritB + "」嘉獎「" + MeritC + "」");
                    }
                    else if (MeritFlag == "0")
                    {
                        sb.AppendLine("懲戒：");
                        sb.AppendLine("班級「" + class_name + "」座號「" + seat_no + "」姓名「" + name + "」");
                        sb.AppendLine("日期「" + dtime.ToShortDateString() + "」");
                        sb.AppendLine("大過「" + DemeritA + "」小過「" + DemeritB + "」警告「" + DemeritC + "」");
                    }
                    else if (MeritFlag == "2")
                    {
                        sb.AppendLine("留查：");
                        sb.AppendLine("班級「" + class_name + "」座號「" + seat_no + "」姓名「" + name + "」");
                        sb.AppendLine("日期「" + dtime.ToShortDateString() + "」");
                        sb.AppendLine("「留察資料支數為0」");
                    }
                    sb.AppendLine("事由「" + Reason + "」");
                    sb.AppendLine("備註「" + Remark + "」");
                    sb.AppendLine("");
                }
            }
            sb.AppendLine(state + "「" + count + "」筆資料");
            return sb.ToString();
        }
    }
}
