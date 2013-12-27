using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K12.Behavior.BatchClearDemerit
{
    class Permissions
    {
        public static string 批次銷過 { get { return "K12.Behavior.BatchClearDemerit.010"; } }

        public static bool 批次銷過權限
        {
            get { return FISCA.Permission.UserAcl.Current[批次銷過].Executable; }
        }
    }
}
