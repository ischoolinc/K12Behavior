using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.Presentation;
using FISCA.Permission;

namespace K12.獎懲統計表
{
    public class Program
    {
        [FISCA.MainMethod()]
        public static void Main()
        {
            MotherForm.RibbonBarItems["學務作業", "資料統計"]["報表"]["獎懲統計報表"].Enable = FISCA.Permission.UserAcl.Current["K12.獎懲統計表"].Executable;
            MotherForm.RibbonBarItems["學務作業", "資料統計"]["報表"]["獎懲統計報表"].Click += (sender, e) => new frmHome_new().ShowDialog();

            FISCA.Permission.Catalog AdminCatalog = FISCA.Permission.RoleAclSource.Instance["學務作業"]["功能按鈕"];
            AdminCatalog.Add(new RibbonFeature("K12.獎懲統計表","獎懲統計表"));
        }
    }
}