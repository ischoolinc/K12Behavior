using FISCA;
using K12.Behavior.StudentExtendControls;
using FISCA.Presentation;
using FISCA.Permission;
using K12.Behavior.StuAdminExtendControls;
using K12.Data;
using K12.Presentation;
using System.Collections.Generic;
using FISCA.Presentation.Controls;

namespace K12.Behavior
{
    public static class Program
    {
        [MainMethod("K12.Behavior")]
        static public void Main()
        {
            #region 毛毛蟲

            FISCA.Permission.FeatureAce UserPermission;
            //缺曠記錄
            UserPermission = FISCA.Permission.UserAcl.Current[Permissions.缺曠記錄];
            if (UserPermission.Editable || UserPermission.Viewable)
                K12.Presentation.NLDPanels.Student.AddDetailBulider(new FISCA.Presentation.DetailBulider<AttendanceItem>());

            //獎勵記錄
            UserPermission = FISCA.Permission.UserAcl.Current[Permissions.獎勵記錄];
            if (UserPermission.Editable || UserPermission.Viewable)
                K12.Presentation.NLDPanels.Student.AddDetailBulider(new FISCA.Presentation.DetailBulider<MeritItem>());

            //懲戒記錄
            UserPermission = FISCA.Permission.UserAcl.Current[Permissions.懲戒記錄];
            if (UserPermission.Editable || UserPermission.Viewable)
                K12.Presentation.NLDPanels.Student.AddDetailBulider(new FISCA.Presentation.DetailBulider<DemeritItem>());

            #endregion

            string URL獎勵 = "ischool/高中系統/共用/學務/學生/登錄獎勵資料";
            string URL獎勵快速登錄 = "ischool/高中系統/共用/學務/學生/獎勵快速登錄";
            string URL懲戒 = "ischool/高中系統/共用/學務/學生/登錄學生懲戒資料";
            string URL懲戒快速登錄 = "ischool/高中系統/共用/學務/學生/懲戒快速登錄";
            //string URL銷過 = "ischool/高中系統/共用/學務/學生/學生銷過";

            string txtMerit = "獎勵";
            string txtMeritSpeed = "獎勵快速登錄";
            string txtDemerit = "懲戒";
            string txtDemeritSpeed = "懲戒快速登錄";
            //string txtClearDemerit = "銷過";

            RibbonBarItem rbItem = MotherForm.RibbonBarItems["學生", "學務"];

            #region 獎勵

            rbItem[txtMerit].Image = Properties.Resources.achievement_64;
            rbItem[txtMeritSpeed].Image = Properties.Resources.achievement_clock_64;
            rbItem[txtMerit].Enable = false;
            rbItem[txtMeritSpeed].Enable = false;

            if (Permissions.獎勵權限)
            {
                NLDPanels.Student.ListPaneContexMenu[txtMerit].BeginGroup = true;

                NLDPanels.Student.ListPaneContexMenu[txtMerit].Image = Properties.Resources.achievement_64;
                NLDPanels.Student.ListPaneContexMenu[txtMeritSpeed].Image = Properties.Resources.achievement_clock_64;
                NLDPanels.Student.ListPaneContexMenu[txtMerit].Enable = false; ;
                NLDPanels.Student.ListPaneContexMenu[txtMeritSpeed].Enable = false;

                NLDPanels.Student.ListPaneContexMenu[txtMerit].Click += delegate
                {
                    MeritEditForm editor = new MeritEditForm(K12.Data.Student.SelectByIDs(K12.Presentation.NLDPanels.Student.SelectedSource));
                    editor.ShowDialog();
                };

                NLDPanels.Student.ListPaneContexMenu[txtMeritSpeed].Click += delegate
                {
                    MutiMeritDemerit editor = new MutiMeritDemerit("獎勵");
                    editor.ShowDialog();
                };
            }

            FISCA.Features.Register(URL獎勵, arg =>
            {
                MeritEditForm editor = new MeritEditForm(K12.Data.Student.SelectByIDs(K12.Presentation.NLDPanels.Student.SelectedSource));
                editor.ShowDialog();
            });

            FISCA.Features.Register(URL獎勵快速登錄, arg =>
            {
                MutiMeritDemerit editor = new MutiMeritDemerit("獎勵");
                editor.ShowDialog();
            });

            rbItem[txtMerit].Click += delegate
            {
                MeritEditForm editor = new MeritEditForm(K12.Data.Student.SelectByIDs(K12.Presentation.NLDPanels.Student.SelectedSource));
                editor.ShowDialog();
            };

            rbItem[txtMeritSpeed].Click += delegate
            {
                MutiMeritDemerit editor = new MutiMeritDemerit("獎勵");
                editor.ShowDialog();
            };


            #endregion

            #region 懲戒

            rbItem[txtDemerit].Image = Properties.Resources.laws_64;
            rbItem[txtDemeritSpeed].Image = Properties.Resources.laws_clock_64;
            rbItem[txtDemerit].Enable = false;
            rbItem[txtDemeritSpeed].Enable = false;

            if (Permissions.懲戒權限)
            {
                NLDPanels.Student.ListPaneContexMenu[txtDemerit].Image = Properties.Resources.laws_64;
                NLDPanels.Student.ListPaneContexMenu[txtDemeritSpeed].Image = Properties.Resources.laws_clock_64;
                NLDPanels.Student.ListPaneContexMenu[txtDemerit].Enable = false;
                NLDPanels.Student.ListPaneContexMenu[txtDemeritSpeed].Enable = false;

                NLDPanels.Student.ListPaneContexMenu[txtDemerit].Click += delegate
                {
                    DemeritEditForm editForm = new DemeritEditForm(K12.Data.Student.SelectByIDs(K12.Presentation.NLDPanels.Student.SelectedSource));
                    editForm.ShowDialog();
                };

                NLDPanels.Student.ListPaneContexMenu[txtDemeritSpeed].Click += delegate
                {
                    MutiMeritDemerit editor = new MutiMeritDemerit("懲戒");
                    editor.ShowDialog();
                };

            }

            FISCA.Features.Register(URL懲戒, arg =>
            {
                DemeritEditForm editForm = new DemeritEditForm(K12.Data.Student.SelectByIDs(K12.Presentation.NLDPanels.Student.SelectedSource));
                editForm.ShowDialog();
            });

            FISCA.Features.Register(URL懲戒快速登錄, arg =>
            {
                MutiMeritDemerit editor = new MutiMeritDemerit("懲戒");
                editor.ShowDialog();
            });

            rbItem[txtDemerit].Click += delegate
            {
                DemeritEditForm editForm = new DemeritEditForm(K12.Data.Student.SelectByIDs(K12.Presentation.NLDPanels.Student.SelectedSource));
                editForm.ShowDialog();
            };

            rbItem[txtDemeritSpeed].Click += delegate
            {
                MutiMeritDemerit editor = new MutiMeritDemerit("懲戒");
                editor.ShowDialog();
            };


            #endregion

            #region 事件

            K12.Presentation.NLDPanels.Student.SelectedSourceChanged += delegate
            {
                if (Permissions.獎勵權限)
                {
                    NLDPanels.Student.ListPaneContexMenu[txtMerit].Enable = (NLDPanels.Student.SelectedSource.Count >= 1);
                    NLDPanels.Student.ListPaneContexMenu[txtMeritSpeed].Enable = (NLDPanels.Student.SelectedSource.Count >= 1);
                }

                if (Permissions.懲戒權限)
                {
                    NLDPanels.Student.ListPaneContexMenu[txtDemerit].Enable = (NLDPanels.Student.SelectedSource.Count >= 1);
                    NLDPanels.Student.ListPaneContexMenu[txtDemeritSpeed].Enable = (NLDPanels.Student.SelectedSource.Count >= 1);
                }
                //NLDPanels.Student.ListPaneContexMenu[txtClearDemerit].Enable = (Permissions.懲戒權限 && NLDPanels.Student.SelectedSource.Count == 1);
                rbItem[txtMerit].Enable = (Permissions.獎勵權限 && NLDPanels.Student.SelectedSource.Count >= 1);
                rbItem[txtMeritSpeed].Enable = (Permissions.獎勵權限 && NLDPanels.Student.SelectedSource.Count >= 1);
                rbItem[txtDemerit].Enable = (Permissions.懲戒權限 && NLDPanels.Student.SelectedSource.Count >= 1);
                rbItem[txtDemeritSpeed].Enable = (Permissions.懲戒權限 && NLDPanels.Student.SelectedSource.Count >= 1);
                //rbItem[txtClearDemerit].Enable = (Permissions.懲戒權限 && NLDPanels.Student.SelectedSource.Count == 1);
            };

            #endregion

            #region 學務作業

            RibbonBarItem Config = MotherForm.RibbonBarItems["學務作業", "基本設定"];

            Config["管理"].Image = Properties.Resources.network_lock_64;
            Config["管理"]["缺曠類別管理"].Enable = Permissions.缺曠類別管理權限;
            Config["管理"]["缺曠類別管理"].Click += delegate
            {
                AbsenceConfigForm acf = new AbsenceConfigForm();
                acf.ShowDialog();
            };

            //Config["管理"]["每日節次管理"].Image = Properties.Resources.table_properties_64;
            Config["管理"]["每日節次管理"].Enable = Permissions.每日節次管理權限;
            Config["管理"]["每日節次管理"].Click += delegate
            {
                PeriodConfigForm pcf = new PeriodConfigForm();
                pcf.ShowDialog();
            };

            //Config["管理"]["功過換算管理"].Image = Properties.Resources.table_properties_64;
            Config["管理"]["功過換算管理"].Enable = Permissions.功過換算管理權限;
            Config["管理"]["功過換算管理"].Click += delegate
            {
                ReduceForm rdf = new ReduceForm();
                rdf.ShowDialog();
            };

            Config["對照/代碼"].Image = Properties.Resources.notepad_lock_64;
            Config["對照/代碼"]["獎懲事由代碼表"].Enable = Permissions.獎懲事由管理權限;
            Config["對照/代碼"]["獎懲事由代碼表"].Click += delegate
            {
                DisciplineForm disF = new DisciplineForm();
                disF.ShowDialog();
            };

            RibbonBarItem batchEdit = MotherForm.RibbonBarItems["學務作業", "批次作業/查詢"];
            batchEdit["獎懲批次修改"].Image = Properties.Resources.star_write_64;
            batchEdit["獎懲批次修改"].Enable = Permissions.獎懲批次修改權限;
            batchEdit["獎懲批次修改"].Click += delegate
            {
                MerDemEditForm mdef = new MerDemEditForm();
                mdef.ShowDialog();
            };

            #endregion

            #region 加入FISCA權限管理

            Catalog ribbon = RoleAclSource.Instance["學生"]["資料項目"];
            ribbon.Add(new FISCA.Permission.DetailItemFeature(Permissions.獎勵記錄, "獎勵記錄"));
            ribbon.Add(new FISCA.Permission.DetailItemFeature(Permissions.懲戒記錄, "懲戒記錄"));
            ribbon.Add(new FISCA.Permission.DetailItemFeature(Permissions.缺曠記錄, "缺曠紀錄"));

            //Catalog studentRibbon = RoleAclSource.Instance["學生"]["功能按鈕"];
            //studentRibbon.Add(new RibbonFeature(Permissions.缺曠, "缺曠"));

            ribbon = RoleAclSource.Instance["學生"]["功能按鈕"];
            ribbon.Add(new RibbonFeature(Permissions.獎勵, "獎勵"));
            ribbon.Add(new RibbonFeature(Permissions.懲戒, "懲戒"));

            ribbon = RoleAclSource.Instance["學務作業"]["功能按鈕"];
            ribbon.Add(new RibbonFeature(Permissions.缺曠類別管理, "缺曠類別管理"));
            ribbon.Add(new RibbonFeature(Permissions.每日節次管理, "每日節次管理"));
            ribbon.Add(new RibbonFeature(Permissions.功過換算管理, "功過換算管理"));
            ribbon.Add(new RibbonFeature(Permissions.獎懲事由管理, "獎懲事由管理"));
            ribbon.Add(new RibbonFeature(Permissions.獎懲批次修改, "獎懲批次修改"));
            //ribbon.Add(new RibbonFeature(Permissions.缺曠資料檢視, "缺曠資料檢視"));
            //ribbon.Add(new RibbonFeature(Permissions.銷過記錄清單, "銷過記錄清單"));

            #endregion
        }
    }
}