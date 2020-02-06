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
            foreach (string var in new string[] { "學年度", "學期", "日期", "大功", "小功", "嘉獎", "大過", "小過", "警告", "事由", "是否銷過", "銷過日期", "銷過事由", "留校察看", "登錄日期", "備註" })
            {
                this.ExportableFields.Add(var);
            }
            this.ExportPackage += new EventHandler<ExportPackageEventArgs>(ExportDiscipline_ExportPackage);
            _access_helper = new AccessHelper();
        }

        private void ExportDiscipline_ExportPackage(object sender, ExportPackageEventArgs e)
        {
            List<K12.Data.StudentRecord> Students = K12.Data.Student.SelectByIDs(e.List);
            Students = SortClassIndex.K12Data_StudentRecord(Students);

            Dictionary<string, List<K12.Data.DisciplineRecord>> StudDisDic = new Dictionary<string, List<Data.DisciplineRecord>>();

            List<K12.Data.DisciplineRecord> Disciplines = K12.Data.Discipline.SelectByStudentIDs(e.List);
            foreach (K12.Data.DisciplineRecord dis in Disciplines)
            {
                if (!StudDisDic.ContainsKey(dis.RefStudentID))
                {
                    StudDisDic.Add(dis.RefStudentID, new List<Data.DisciplineRecord>());
                }
                StudDisDic[dis.RefStudentID].Add(dis);
            }



            //List<SmartSchool.Customization.Data.StudentRecord> students = _access_helper.StudentHelper.GetStudents(e.List);
            //_access_helper.StudentHelper.FillReward(students);

            foreach (K12.Data.StudentRecord stu in Students)
            {
                if (StudDisDic.ContainsKey(stu.ID))
                {
                    List<K12.Data.DisciplineRecord> disList = StudDisDic[stu.ID];
                    disList.Sort(SortDic);

                    foreach (K12.Data.DisciplineRecord dis in disList)
                    {
                        int 大功 = dis.MeritA.HasValue ? dis.MeritA.Value : 0;
                        int 小功 = dis.MeritB.HasValue ? dis.MeritB.Value : 0;
                        int 嘉獎 = dis.MeritC.HasValue ? dis.MeritC.Value : 0;
                        int 大過 = dis.DemeritA.HasValue ? dis.DemeritA.Value : 0;
                        int 小過 = dis.DemeritB.HasValue ? dis.DemeritB.Value : 0;
                        int 警告 = dis.DemeritC.HasValue ? dis.DemeritC.Value : 0;

                        if (大功 + 小功 + 嘉獎 != 0)
                        {
                            #region 獎勵
                            RowData row = new RowData();
                            row.ID = stu.ID;
                            foreach (string field in e.ExportFields)
                            {
                                if (ExportableFields.Contains(field))
                                {
                                    switch (field)
                                    {
                                        case "學年度": row.Add(field, dis.SchoolYear.ToString()); break;
                                        case "學期": row.Add(field, dis.Semester.ToString()); break;
                                        case "日期": row.Add(field, dis.OccurDate.ToShortDateString()); break;
                                        //case "地點": row.Add(field, var.OccurPlace); break;
                                        case "大功": row.Add(field, 大功.ToString()); break;
                                        case "小功": row.Add(field, 小功.ToString()); break;
                                        case "嘉獎": row.Add(field, 嘉獎.ToString()); break;
                                        case "事由": row.Add(field, dis.Reason); break;
                                        case "登錄日期": row.Add(field, dis.RegisterDate.HasValue ? dis.RegisterDate.Value.ToShortDateString() : ""); break;
                                        case "備註": row.Add(field, dis.Remark); break;
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
                            row.ID = stu.ID;
                            foreach (string field in e.ExportFields)
                            {
                                if (ExportableFields.Contains(field))
                                {
                                    switch (field)
                                    {
                                        case "學年度": row.Add(field, dis.SchoolYear.ToString()); break;
                                        case "學期": row.Add(field, dis.Semester.ToString()); break;
                                        case "日期": row.Add(field, dis.OccurDate.ToShortDateString()); break;
                                        //case "地點": row.Add(field, var.OccurPlace); break;
                                        case "大過": row.Add(field, 大過.ToString()); break;
                                        case "小過": row.Add(field, 小過.ToString()); break;
                                        case "警告": row.Add(field, 警告.ToString()); break;
                                        case "事由": row.Add(field, dis.Reason); break;
                                        case "是否銷過": row.Add(field, (dis.Cleared == "是" ? "是" : "")); break;
                                        case "銷過日期": row.Add(field, (dis.Cleared == "是" ? (dis.ClearDate.HasValue ? dis.ClearDate.Value.ToShortDateString() : "") : "")); break;
                                        case "銷過事由": row.Add(field, (dis.Cleared == "是" ? dis.ClearReason : "")); break;
                                        case "留校察看": row.Add(field, (dis.MeritFlag == "2" ? "是" : "")); break;
                                        case "登錄日期": row.Add(field, dis.RegisterDate.HasValue ? dis.RegisterDate.Value.ToShortDateString() : ""); break;
                                        case "備註": row.Add(field, dis.Remark); break;
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
                            row.ID = stu.ID;
                            foreach (string field in e.ExportFields)
                            {
                                if (ExportableFields.Contains(field))
                                {
                                    switch (field)
                                    {
                                        case "學年度": row.Add(field, dis.SchoolYear.ToString()); break;
                                        case "學期": row.Add(field, dis.Semester.ToString()); break;
                                        case "日期": row.Add(field, dis.OccurDate.ToShortDateString()); break;
                                        //case "地點": row.Add(field, var.OccurPlace); break;
                                        case "大功": row.Add(field, 大功.ToString()); break;
                                        case "小功": row.Add(field, 小功.ToString()); break;
                                        case "嘉獎": row.Add(field, 嘉獎.ToString()); break;
                                        case "大過": row.Add(field, 大過.ToString()); break;
                                        case "小過": row.Add(field, 小過.ToString()); break;
                                        case "警告": row.Add(field, 警告.ToString()); break;
                                        case "事由": row.Add(field, dis.Reason); break;
                                        case "是否銷過": row.Add(field, (dis.Cleared == "是" ? "是" : "")); break;
                                        case "銷過日期": row.Add(field, (dis.Cleared == "是" ? (dis.ClearDate.HasValue ? dis.ClearDate.Value.ToShortDateString() : "") : "")); break;
                                        case "銷過事由": row.Add(field, (dis.Cleared == "是" ? dis.ClearReason : "")); break;
                                        case "留校察看": row.Add(field, (dis.MeritFlag == "2" ? "是" : "")); break;
                                        case "登錄日期": row.Add(field, dis.RegisterDate.HasValue ? dis.RegisterDate.Value.ToShortDateString() : ""); break;
                                        case "備註": row.Add(field, dis.Remark); break;
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

        public int SortDic(K12.Data.DisciplineRecord a, K12.Data.DisciplineRecord b)
        {
            return a.OccurDate.CompareTo(b.OccurDate);
        }
    }
}
