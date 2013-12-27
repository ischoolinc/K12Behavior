using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.Presentation;
using FISCA.Permission;
using FISCA;

namespace K12.Behavior.BatchClearDemerit
{
    public class Program
    {
        [MainMethod("K12.Behavior.BatchClearDemerit")]
        static public void Main()
        {
            RibbonBarItem batchEdit = FISCA.Presentation.MotherForm.RibbonBarItems["學生", "學務"];// StuAdmin.Instance.RibbonBarItems["批次作業/查詢"];
            batchEdit["銷過"].Image = Properties.Resources.draw_pen_ok_64;
            batchEdit["銷過"].Enable = false;
            batchEdit["銷過"].Click += delegate
            {
                BatchClearDemeritFrom batchClear = new BatchClearDemeritFrom(K12.Presentation.NLDPanels.Student.SelectedSource);
                batchClear.ShowDialog();
            };

            K12.Presentation.NLDPanels.Student.SelectedSourceChanged += delegate
            {
                if (Permissions.批次銷過權限)
                {
                    batchEdit["銷過"].Enable = Permissions.批次銷過權限 && K12.Presentation.NLDPanels.Student.SelectedSource.Count > 0;
                    K12.Presentation.NLDPanels.Student.ListPaneContexMenu["銷過"].Enable = Permissions.批次銷過權限 && K12.Presentation.NLDPanels.Student.SelectedSource.Count > 0;
                }
            };

            if (Permissions.批次銷過權限)
            {
                K12.Presentation.NLDPanels.Student.ListPaneContexMenu["銷過"].Image = Properties.Resources.draw_pen_ok_64;
                K12.Presentation.NLDPanels.Student.ListPaneContexMenu["銷過"].Enable = false;
                K12.Presentation.NLDPanels.Student.ListPaneContexMenu["銷過"].Click += delegate
                {
                    BatchClearDemeritFrom batchClear = new BatchClearDemeritFrom(K12.Presentation.NLDPanels.Student.SelectedSource);
                    batchClear.ShowDialog();
                };
            }

            Catalog ribbon = RoleAclSource.Instance["學生"]["功能按鈕"];
            ribbon.Add(new RibbonFeature(Permissions.批次銷過, "銷過"));

        }
    }
}
