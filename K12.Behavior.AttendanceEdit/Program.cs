using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA;
using K12.Presentation;
using FISCA.Presentation;
using FISCA.Permission;
using K12.Data;

namespace K12.Behavior.AttendanceEdit
{
    public static class Program
    {
        [MainMethod("K12.Behavior.AttendanceEdit")]
        static public void Main()
        {
            RibbonBarItem batchEdit = FISCA.Presentation.MotherForm.RibbonBarItems["學務作業", "批次作業/查詢"];// StuAdmin.Instance.RibbonBarItems["批次作業/查詢"];
            batchEdit["缺曠批次修改"].Image = Properties.Resources.tablet_write_64;
            batchEdit["缺曠批次修改"].Enable = Permissions.缺曠批次修改權限;
            batchEdit["缺曠批次修改"].Click += delegate
            {
                AttendanceEditForm AttendanceTotal = new AttendanceEditForm();
                AttendanceTotal.ShowDialog();
            };


            Catalog ribbon = RoleAclSource.Instance["學務作業"]["功能按鈕"];
            ribbon.Add(new RibbonFeature(Permissions.缺曠批次修改_國中, "缺曠批次修改"));
        }
    }
}
