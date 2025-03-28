using FISCA;
using FISCA.Permission;
using FISCA.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K12.Behavior.Setup
{
    public class Program
    {
        [MainMethod()]
        public static void Main()
        {
            RibbonBarItem Config = MotherForm.RibbonBarItems["學務作業", "基本設定"];
            Config["設定"]["功能名稱對照表"].Enable = 功能名稱對照表權限;
            Config["設定"]["功能名稱對照表"].Click += delegate
            {
                SetupBehaviorDef acf = new SetupBehaviorDef();
                acf.ShowDialog();
            };


            Catalog ribbon = RoleAclSource.Instance["學務作業"]["功能按鈕"];
            ribbon.Add(new RibbonFeature(功能名稱對照表, "功能名稱對照表"));

        }

        public static string 功能名稱對照表 { get { return "K12.Behavior.Setup.SetupBehaviorDef"; } }

        public static bool 功能名稱對照表權限
        {
            get
            {
                return FISCA.Permission.UserAcl.Current[功能名稱對照表].Executable;
            }
        }
    }
}
