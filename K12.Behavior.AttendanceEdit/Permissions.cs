using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K12.Behavior.AttendanceEdit
{
    class Permissions
    {
        public static string 缺曠批次修改_國中 { get { return "JHSchool.StuAdmin.Ribbon0060"; } }

        public static bool 缺曠批次修改權限
        {
            get { return FISCA.Permission.UserAcl.Current[缺曠批次修改_國中].Executable; }
        }
    }
}
