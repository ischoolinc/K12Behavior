using System;
using System.Collections.Generic;
using System.Text;
using SmartSchool.Customization.PlugIn.ImportExport;
using SmartSchool.Customization.Data;
using SmartSchool.Customization.Data.StudentExtension;
using SmartSchool.AccessControl;

namespace K12.Behavior
{
    [FeatureCode("Button0190")]
    class ExportDiscipline : ExportProcess
    {
        private AccessHelper _access_helper;

        public ExportDiscipline()
        {
            this.Title = "匯出獎懲紀錄";
            this.Group = "缺曠獎懲";
            foreach (string var in new string[] { "學年度", "學期", "日期", "大功", "小功", "嘉獎", "大過", "小過", "警告", "事由", "是否銷過", "銷過日期", "銷過事由", "留校察看" })
            {
                this.ExportableFields.Add(var);
            }
            this.ExportPackage += new EventHandler<ExportPackageEventArgs>(ExportDiscipline_ExportPackage);
            _access_helper = new AccessHelper();
        }

        private void ExportDiscipline_ExportPackage(object sender, ExportPackageEventArgs e)
        {
            List<SmartSchool.Customization.Data.StudentRecord> students = _access_helper.StudentHelper.GetStudents(e.List);
            _access_helper.StudentHelper.FillReward(students);

            foreach (SmartSchool.Customization.Data.StudentRecord stu in students)
            {
                foreach (RewardInfo var in stu.RewardList)
                {
                    int 大功 = 0;
                    int 小功 = 0;
                    int 嘉獎 = 0;
                    int 大過 = 0;
                    int 小過 = 0;
                    int 警告 = 0;

                    int.TryParse(var.AwardA.ToString(), out 大功);
                    int.TryParse(var.AwardB.ToString(), out 小功);
                    int.TryParse(var.AwardC.ToString(), out 嘉獎);
                    int.TryParse(var.FaultA.ToString(), out 大過);
                    int.TryParse(var.FaultB.ToString(), out 小過);
                    int.TryParse(var.FaultC.ToString(), out 警告);

                    if (大功 + 小功 + 嘉獎 != 0)
                    {
                        #region 獎勵
                        RowData row = new RowData();
                        row.ID = stu.StudentID;
                        foreach (string field in e.ExportFields)
                        {
                            if (ExportableFields.Contains(field))
                            {
                                switch (field)
                                {
                                    case "學年度": row.Add(field, var.SchoolYear.ToString()); break;
                                    case "學期": row.Add(field, var.Semester.ToString()); break;
                                    case "日期": row.Add(field, var.OccurDate.ToShortDateString()); break;
                                    //case "地點": row.Add(field, var.OccurPlace); break;
                                    case "大功": row.Add(field, 大功.ToString()); break;
                                    case "小功": row.Add(field, 小功.ToString()); break;
                                    case "嘉獎": row.Add(field, 嘉獎.ToString()); break;
                                    case "事由": row.Add(field, var.OccurReason); break;
                                }
                            }
                        }
                        e.Items.Add(row);
                        #endregion
                    }
                    else if (大過 + 小過 + 警告 != 0)
                    {
                        #region 懲戒
                        RowData row = new RowData();
                        row.ID = stu.StudentID;
                        foreach (string field in e.ExportFields)
                        {
                            if (ExportableFields.Contains(field))
                            {
                                switch (field)
                                {
                                    case "學年度": row.Add(field, var.SchoolYear.ToString()); break;
                                    case "學期": row.Add(field, var.Semester.ToString()); break;
                                    case "日期": row.Add(field, var.OccurDate.ToShortDateString()); break;
                                    //case "地點": row.Add(field, var.OccurPlace); break;
                                    case "大過": row.Add(field, 大過.ToString()); break;
                                    case "小過": row.Add(field, 小過.ToString()); break;
                                    case "警告": row.Add(field, 警告.ToString()); break;
                                    case "事由": row.Add(field, var.OccurReason); break;
                                    case "是否銷過": row.Add(field, (var.Cleared ? "是" : "")); break;
                                    case "銷過日期": row.Add(field, (var.Cleared ? var.ClearDate.ToShortDateString() : "")); break;
                                    case "銷過事由": row.Add(field, (var.Cleared ? var.ClearReason : "")); break;
                                    case "留校察看": row.Add(field, (var.UltimateAdmonition ? "是" : "")); break;
                                }
                            }
                        }
                        e.Items.Add(row);
                        #endregion
                    }
                    else
                    {
                        #region 無法判斷
                        RowData row = new RowData();
                        row.ID = stu.StudentID;
                        foreach (string field in e.ExportFields)
                        {
                            if (ExportableFields.Contains(field))
                            {
                                switch (field)
                                {
                                    case "學年度": row.Add(field, var.SchoolYear.ToString()); break;
                                    case "學期": row.Add(field, var.Semester.ToString()); break;
                                    case "日期": row.Add(field, var.OccurDate.ToShortDateString()); break;
                                    //case "地點": row.Add(field, var.OccurPlace); break;
                                    case "大功": row.Add(field, 大功.ToString()); break;
                                    case "小功": row.Add(field, 小功.ToString()); break;
                                    case "嘉獎": row.Add(field, 嘉獎.ToString()); break;
                                    case "大過": row.Add(field, 大過.ToString()); break;
                                    case "小過": row.Add(field, 小過.ToString()); break;
                                    case "警告": row.Add(field, 警告.ToString()); break;
                                    case "事由": row.Add(field, var.OccurReason); break;
                                    case "是否銷過": row.Add(field, (var.Cleared ? "是" : "")); break;
                                    case "銷過日期": row.Add(field, (var.Cleared ? var.ClearDate.ToShortDateString() : "")); break;
                                    case "銷過事由": row.Add(field, (var.Cleared ? var.ClearReason : "")); break;
                                    case "留校察看": row.Add(field, (var.UltimateAdmonition ? "是" : "")); break;
                                }
                            }
                        }
                        e.Items.Add(row);
                        #endregion
                    }
                }
            }
        }
    }
}
