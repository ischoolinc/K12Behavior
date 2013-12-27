using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA;
using FISCA.Presentation;
using FISCA.Permission;

namespace K12.學生獎勵懲戒明細
{
    public static class Program
    {
        [MainMethod()]
        static public void Main()
        {
            string URL學生獎勵懲戒明細 = "ischool/高中系統/共用/學務/學生/報表/獎勵懲戒明細";

            FISCA.Features.Register(URL學生獎勵懲戒明細, arg =>
            {
                new Report().Print();
            });


            RibbonBarItem StudentReports = K12.Presentation.NLDPanels.Student.RibbonBarItems["資料統計"];
            StudentReports["報表"]["學務相關報表"]["學生獎勵懲戒明細"].Enable = false;
            StudentReports["報表"]["學務相關報表"]["學生獎勵懲戒明細"].Click += delegate
            {
                Features.Invoke(URL學生獎勵懲戒明細);
            };

            K12.Presentation.NLDPanels.Student.SelectedSourceChanged += delegate
            {
                if (K12.Presentation.NLDPanels.Student.SelectedSource.Count <= 0)
                {
                    StudentReports["報表"]["學務相關報表"]["學生獎勵懲戒明細"].Enable = false;
                }
                else
                {
                    StudentReports["報表"]["學務相關報表"]["學生獎勵懲戒明細"].Enable = Permissions.學生獎勵懲戒明細權限;
                }
            };

            Catalog ribbon = RoleAclSource.Instance["學生"]["報表"];
            ribbon.Add(new RibbonFeature(Permissions.學生獎勵懲戒明細, "學生獎勵懲戒明細"));
        }
    }
}
