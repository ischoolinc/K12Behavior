using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.Presentation;
using FISCA.Permission;

namespace K12.ClassMeritDemerit.Detail
{
    public static class Program
    {
        [FISCA.MainMethod()]
        public static void Main()
        {
            RibbonBarItem classSpecialItem = K12.Presentation.NLDPanels.Class.RibbonBarItems["資料統計"];
            classSpecialItem["報表"]["學務相關報表"]["班級學生獎懲明細(新版)"].Enable = Permissions.班級學生獎懲明細權限_新版;
            classSpecialItem["報表"]["學務相關報表"]["班級學生獎懲明細(新版)"].Click += delegate
            {
                new K12.ClassMeritDemerit.Detail.Report().Print();
            };

            Catalog ribbon = RoleAclSource.Instance["班級"]["報表"];
            // 2018.09.27 [ischoolKingdom] Vicky依據 [H成績][H學務][06] 功能沒有設定權限管理 項目，將各功能按鈕註冊時Enable設定與系統權限綁定，權限Code使用GUID。
            ribbon.Add(new RibbonFeature(Permissions.班級學生獎懲明細, "班級學生獎懲明細"));
            //ribbon.Add(new RibbonFeature(Permissions.班級學生獎懲明細, "班級學生獎懲明細"));  
            ribbon.Add(new RibbonFeature(Permissions.班級學生獎懲明細_新版, "班級學生獎懲明細(新版)")); //增加"(新版)"文字
        }
    }
}
