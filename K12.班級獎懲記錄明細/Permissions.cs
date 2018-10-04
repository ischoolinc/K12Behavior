using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K12.ClassMeritDemerit.Detail
{
    class Permissions
    {
        public static string 班級學生獎懲明細_新版 { get { return "Report0230"; } }

        public static bool 班級學生獎懲明細權限_新版
        {
            get
            {
                return FISCA.Permission.UserAcl.Current[班級學生獎懲明細_新版].Executable;
            }
        }

        public static string 班級學生獎懲明細 { get { return "625B8AE1-C581-4214-8E18-426DEF81D4DE"; } }

        public static bool 班級學生獎懲明細權限
        {
            get
            {
                return FISCA.Permission.UserAcl.Current[班級學生獎懲明細].Executable;
            }
        }
    }
}
