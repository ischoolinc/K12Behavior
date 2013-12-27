using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K12.學生缺曠明細
{
    class Permissions
    {
        public static string 學生缺曠明細 { get { return "K12.Student.SelectAttendanceForm"; } }

        public static bool 學生缺曠明細權限
        {
            get { return FISCA.Permission.UserAcl.Current[學生缺曠明細].Executable; }
        }
    }
}
