using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA;
using FISCA.Presentation;
using FISCA.Permission;

namespace K12.Behavior.DisciplineInput
{
    public class Program
    {
        [MainMethod()]
        static public void Main()
        {
            FISCA.ServerModule.AutoManaged("http://module.ischool.com.tw/module/193005/K12.Behavior.DisciplineInput/udm.xml");


            RibbonBarItem item = MotherForm.RibbonBarItems["學務作業", "基本設定"];
            item["設定"]["教師獎懲登錄設定"].Enable = Permissions.教師獎懲登錄設定權限;
            item["設定"]["教師獎懲登錄設定"].Click += delegate
            {
                new InputDateSettingForm().ShowDialog();
            };
            Catalog detail1 = RoleAclSource.Instance["學務作業"];
            detail1.Add(new RibbonFeature(Permissions.教師獎懲登錄設定, "教師獎懲登錄設定"));


            item = MotherForm.RibbonBarItems["學務作業", "批次作業/查詢"];
            item["獎懲批次修改(教師)"].Image = Properties.Resources.star_write_64;
            item["獎懲批次修改(教師)"].Size = RibbonBarButton.MenuButtonSize.Medium;
            item["獎懲批次修改(教師)"].Enable = Permissions.獎懲批次修改_教師權限;
            item["獎懲批次修改(教師)"].Click += delegate
            {
                new MerDemEditForm().ShowDialog();
            };
            detail1 = RoleAclSource.Instance["學務作業"];
            detail1.Add(new RibbonFeature(Permissions.獎懲批次修改_教師, "獎懲批次修改(教師)"));
        }
    }
}
