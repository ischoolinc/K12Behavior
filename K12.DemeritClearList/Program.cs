using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA;
using FISCA.Permission;
using FISCA.Presentation;

namespace K12.DemeritClearList
{
    public class Program
    {
        [MainMethod()]
        static public void Main()
        {
            RibbonBarItem batchEdit1 = FISCA.Presentation.MotherForm.RibbonBarItems["學務作業", "資料統計"];
            batchEdit1["報表"].Image = Properties.Resources.paste_64;
            batchEdit1["報表"]["銷過記錄清單"].Enable = Permissions.銷過記錄清單權限_國中 || Permissions.銷過記錄清單權限_高中;
            batchEdit1["報表"]["銷過記錄清單"].Click += delegate
            {
                StudentDemeritClear sdc = new StudentDemeritClear();
                sdc.ShowDialog();
            };

            Catalog ribbon = RoleAclSource.Instance["學務作業"]["功能按鈕"];
            ribbon.Add(new RibbonFeature(Permissions.銷過記錄清單_高中, "銷過記錄清單"));

        }
    }
}
