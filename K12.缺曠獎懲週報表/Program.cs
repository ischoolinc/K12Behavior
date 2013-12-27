using FISCA;
using System.Collections.Generic;
using K12.Data;
using FISCA.Permission;
using FISCA.Presentation;
using K12.Presentation;

namespace JHSchool.Behavior.Report
{
    public static class Program
    {
        [MainMethod()]
        public static void Main()
        {
            string MeritDemeritName = "獎懲週報表";
            string CountByPeriodName = "缺曠週報表(依節次)";
            string CountByAbsenceName = "缺曠週報表(依假別)";

            MenuButton item = NLDPanels.Class.RibbonBarItems["資料統計"]["報表"]["學務相關報表"];
            item[MeritDemeritName].Enable = false;
            item[CountByPeriodName].Enable = false;
            item[CountByAbsenceName].Enable = false;

            item[MeritDemeritName].Click += delegate
            {
                new K12.缺曠獎懲週報表.獎懲週報表.Report().Print();
            };

            item[CountByPeriodName].Click += delegate
            {
                new K12.缺曠獎懲週報表.缺曠週報表_依節次.Report().Print();
            };

            item[CountByAbsenceName].Click += delegate
            {
                new K12.缺曠獎懲週報表.缺曠週報表_依假別.Report().Print();
            };

            //事件
            K12.Presentation.NLDPanels.Class.SelectedSourceChanged += delegate
            {
                if (K12.Presentation.NLDPanels.Class.SelectedSource.Count <= 0)
                {
                    item[MeritDemeritName].Enable = false;
                    item[CountByPeriodName].Enable = false;
                    item[CountByAbsenceName].Enable = false;
                }
                else
                {
                    item[MeritDemeritName].Enable = Permissions.獎懲週報表權限;
                    item[CountByPeriodName].Enable = Permissions.缺曠週報表_依節次權限;
                    item[CountByAbsenceName].Enable = Permissions.缺曠週報表_依假別權限;
                }
            };

            #region 註冊權限(目前依附ischool高中的xml檔案)
            RoleAclSource.Instance["班級"]["報表"].Add(new ReportFeature(Permissions.獎懲週報表, MeritDemeritName));
            RoleAclSource.Instance["班級"]["報表"].Add(new ReportFeature(Permissions.缺曠週報表_依節次, CountByPeriodName));
            RoleAclSource.Instance["班級"]["報表"].Add(new ReportFeature(Permissions.缺曠週報表_依假別, CountByAbsenceName));
            #endregion
        }
    }
}
