using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA;
using K12.Presentation;
using FISCA.Permission;

namespace K12.Behavior.DisciplineNotification
{
    public class Program
    {
        [MainMethod()]
        public static void Main()
        {
            NLDPanels.Student.RibbonBarItems["資料統計"]["報表"]["學務相關報表"]["學生獎勵懲戒通知單"].Enable = false;
            NLDPanels.Student.RibbonBarItems["資料統計"]["報表"]["學務相關報表"]["學生獎勵懲戒通知單"].Click += delegate
            {
                new Report("student").Print();
            };

            NLDPanels.Class.RibbonBarItems["資料統計"]["報表"]["學務相關報表"]["班級獎勵懲戒通知單"].Enable = false;
            NLDPanels.Class.RibbonBarItems["資料統計"]["報表"]["學務相關報表"]["班級獎勵懲戒通知單"].Click += delegate
            {
                new Report("class").Print();
            };

            NLDPanels.Student.SelectedSourceChanged += delegate
            {
                if (NLDPanels.Student.SelectedSource.Count <= 0)
                {
                    NLDPanels.Student.RibbonBarItems["資料統計"]["報表"]["學務相關報表"]["學生獎勵懲戒通知單"].Enable = false;
                }
                else
                {
                    NLDPanels.Student.RibbonBarItems["資料統計"]["報表"]["學務相關報表"]["學生獎勵懲戒通知單"].Enable = Permissions.學生獎勵懲戒通知單權限 && NLDPanels.Student.SelectedSource.Count > 0;
                }
            };

            NLDPanels.Class.SelectedSourceChanged += delegate
            {
                if (NLDPanels.Class.SelectedSource.Count <= 0)
                {
                    NLDPanels.Class.RibbonBarItems["資料統計"]["報表"]["學務相關報表"]["班級獎勵懲戒通知單"].Enable = false;
                }
                else
                {
                    NLDPanels.Class.RibbonBarItems["資料統計"]["報表"]["學務相關報表"]["班級獎勵懲戒通知單"].Enable = Permissions.班級獎勵懲戒通知單權限 && NLDPanels.Class.SelectedSource.Count > 0;
                }
            };
            Catalog ribbon = RoleAclSource.Instance["學生"]["報表"];
            ribbon.Add(new RibbonFeature(Permissions.學生獎勵懲戒通知單, "學生獎勵懲戒通知單"));
            
            ribbon = RoleAclSource.Instance["班級"]["報表"];
            ribbon.Add(new RibbonFeature(Permissions.班級獎勵懲戒通知單, "班級獎勵懲戒通知單"));
        }
    }
}
