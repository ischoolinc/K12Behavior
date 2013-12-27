using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K12.學生獎勵懲戒明細
{
    class Permissions
    {
        public static string 學生獎勵懲戒明細 { get { return "K12.Student.SelectMeritDemeritForm"; } }

        public static bool 學生獎勵懲戒明細權限
        {
            get { return FISCA.Permission.UserAcl.Current[學生獎勵懲戒明細].Executable; }
        }
    }
}
