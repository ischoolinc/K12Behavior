using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.Presentation;
using FISCA.Permission;
using System.Xml;

namespace K12.缺曠統計表
{
    public class Program
    {
        [FISCA.MainMethod()]
        public static void Main()
        {
            //XmlElement xml = FISCA.DSAUtil.DSXmlHelper.LoadXml("<LeaveInfo ClassName='資三甲' Department='資料處理科' Reason='畢業' SchoolYear='99'/>");
            //xml.GetAttribute("ClassName"); //畢業班級
            //xml.GetAttribute("Department"); //畢業科別
            //xml.GetAttribute("Reason"); //畢業原因
            //xml.GetAttribute("SchoolYear"); //畢業年

            MotherForm.RibbonBarItems["學務作業", "資料統計"]["報表"]["缺曠統計報表"].Enable = FISCA.Permission.UserAcl.Current["K12.缺曠統計表"].Executable;
            MotherForm.RibbonBarItems["學務作業", "資料統計"]["報表"]["缺曠統計報表"].Click += (sender, e) => new frmHome_new().ShowDialog();

            FISCA.Permission.Catalog AdminCatalog = FISCA.Permission.RoleAclSource.Instance["學務作業"]["功能按鈕"];
            AdminCatalog.Add(new RibbonFeature("K12.缺曠統計表", "缺曠統計表"));
        }
    }
}