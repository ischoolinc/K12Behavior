using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA;
using FISCA.Presentation;
using FISCA.Permission;

namespace K12.Student.SpeedAddToTemp
{
    public class Program
    {
        [MainMethod()]
        public static void Main()
        {
            RibbonBarItem StuItem4 = FISCA.Presentation.MotherForm.RibbonBarItems["學生", "其它"];
            StuItem4["快速待處理"].Image = Properties.Resources.trainning_add_64;
            StuItem4["快速待處理"].Enable = Permissions.快速待處理_學生權限;
            StuItem4["快速待處理"].Size = RibbonBarButton.MenuButtonSize.Medium;
            StuItem4["快速待處理"].Click += delegate
            {
                SpeedAddFormIs speed = new SpeedAddFormIs();
                speed.ShowIcon = true;
                speed.ShowInTaskbar = true;
                speed.Show();

            };

            Catalog ribbon = RoleAclSource.Instance["學生"]["功能按鈕"];
            ribbon.Add(new RibbonFeature(Permissions.快速待處理_學生, "快速待處理"));
        }
    }
}
