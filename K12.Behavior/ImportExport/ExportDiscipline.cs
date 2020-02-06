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
            this.Title = "�ץX���g����";
            this.Group = "���m���g";
            foreach (string var in new string[] { "�Ǧ~��", "�Ǵ�", "���", "�j�\", "�p�\", "�ż�", "�j�L", "�p�L", "ĵ�i", "�ƥ�", "�O�_�P�L", "�P�L���", "�P�L�ƥ�", "�d�չ��", "�n�����", "�Ƶ�" })
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
                        int �j�\ = dis.MeritA.HasValue ? dis.MeritA.Value : 0;
                        int �p�\ = dis.MeritB.HasValue ? dis.MeritB.Value : 0;
                        int �ż� = dis.MeritC.HasValue ? dis.MeritC.Value : 0;
                        int �j�L = dis.DemeritA.HasValue ? dis.DemeritA.Value : 0;
                        int �p�L = dis.DemeritB.HasValue ? dis.DemeritB.Value : 0;
                        int ĵ�i = dis.DemeritC.HasValue ? dis.DemeritC.Value : 0;

                        if (�j�\ + �p�\ + �ż� != 0)
                        {
                            #region ���y
                            RowData row = new RowData();
                            row.ID = stu.ID;
                            foreach (string field in e.ExportFields)
                            {
                                if (ExportableFields.Contains(field))
                                {
                                    switch (field)
                                    {
                                        case "�Ǧ~��": row.Add(field, dis.SchoolYear.ToString()); break;
                                        case "�Ǵ�": row.Add(field, dis.Semester.ToString()); break;
                                        case "���": row.Add(field, dis.OccurDate.ToShortDateString()); break;
                                        //case "�a�I": row.Add(field, var.OccurPlace); break;
                                        case "�j�\": row.Add(field, �j�\.ToString()); break;
                                        case "�p�\": row.Add(field, �p�\.ToString()); break;
                                        case "�ż�": row.Add(field, �ż�.ToString()); break;
                                        case "�ƥ�": row.Add(field, dis.Reason); break;
                                        case "�n�����": row.Add(field, dis.RegisterDate.HasValue ? dis.RegisterDate.Value.ToShortDateString() : ""); break;
                                        case "�Ƶ�": row.Add(field, dis.Remark); break;
                                    }
                                }
                            }
                            e.Items.Add(row);
                            #endregion
                        }
                        else if (�j�L + �p�L + ĵ�i != 0)
                        {
                            #region �g��
                            RowData row = new RowData();
                            row.ID = stu.ID;
                            foreach (string field in e.ExportFields)
                            {
                                if (ExportableFields.Contains(field))
                                {
                                    switch (field)
                                    {
                                        case "�Ǧ~��": row.Add(field, dis.SchoolYear.ToString()); break;
                                        case "�Ǵ�": row.Add(field, dis.Semester.ToString()); break;
                                        case "���": row.Add(field, dis.OccurDate.ToShortDateString()); break;
                                        //case "�a�I": row.Add(field, var.OccurPlace); break;
                                        case "�j�L": row.Add(field, �j�L.ToString()); break;
                                        case "�p�L": row.Add(field, �p�L.ToString()); break;
                                        case "ĵ�i": row.Add(field, ĵ�i.ToString()); break;
                                        case "�ƥ�": row.Add(field, dis.Reason); break;
                                        case "�O�_�P�L": row.Add(field, (dis.Cleared == "�O" ? "�O" : "")); break;
                                        case "�P�L���": row.Add(field, (dis.Cleared == "�O" ? (dis.ClearDate.HasValue ? dis.ClearDate.Value.ToShortDateString() : "") : "")); break;
                                        case "�P�L�ƥ�": row.Add(field, (dis.Cleared == "�O" ? dis.ClearReason : "")); break;
                                        case "�d�չ��": row.Add(field, (dis.MeritFlag == "2" ? "�O" : "")); break;
                                        case "�n�����": row.Add(field, dis.RegisterDate.HasValue ? dis.RegisterDate.Value.ToShortDateString() : ""); break;
                                        case "�Ƶ�": row.Add(field, dis.Remark); break;
                                    }
                                }
                            }
                            e.Items.Add(row);
                            #endregion
                        }
                        else
                        {
                            #region �L�k�P�_
                            RowData row = new RowData();
                            row.ID = stu.ID;
                            foreach (string field in e.ExportFields)
                            {
                                if (ExportableFields.Contains(field))
                                {
                                    switch (field)
                                    {
                                        case "�Ǧ~��": row.Add(field, dis.SchoolYear.ToString()); break;
                                        case "�Ǵ�": row.Add(field, dis.Semester.ToString()); break;
                                        case "���": row.Add(field, dis.OccurDate.ToShortDateString()); break;
                                        //case "�a�I": row.Add(field, var.OccurPlace); break;
                                        case "�j�\": row.Add(field, �j�\.ToString()); break;
                                        case "�p�\": row.Add(field, �p�\.ToString()); break;
                                        case "�ż�": row.Add(field, �ż�.ToString()); break;
                                        case "�j�L": row.Add(field, �j�L.ToString()); break;
                                        case "�p�L": row.Add(field, �p�L.ToString()); break;
                                        case "ĵ�i": row.Add(field, ĵ�i.ToString()); break;
                                        case "�ƥ�": row.Add(field, dis.Reason); break;
                                        case "�O�_�P�L": row.Add(field, (dis.Cleared == "�O" ? "�O" : "")); break;
                                        case "�P�L���": row.Add(field, (dis.Cleared == "�O" ? (dis.ClearDate.HasValue ? dis.ClearDate.Value.ToShortDateString() : "") : "")); break;
                                        case "�P�L�ƥ�": row.Add(field, (dis.Cleared == "�O" ? dis.ClearReason : "")); break;
                                        case "�d�չ��": row.Add(field, (dis.MeritFlag == "2" ? "�O" : "")); break;
                                        case "�n�����": row.Add(field, dis.RegisterDate.HasValue ? dis.RegisterDate.Value.ToShortDateString() : ""); break;
                                        case "�Ƶ�": row.Add(field, dis.Remark); break;
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
