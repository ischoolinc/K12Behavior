using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K12.Behavior.DisciplineNotification
{
    /// <summary>
    /// 代表目前使用者的相關權限資訊。
    /// </summary>
    public static class Permissions
    {
        public static string 學生獎勵懲戒通知單 { get { return "JHSchool.Student.Report0030"; } }
        public static bool 學生獎勵懲戒通知單權限
        {
            get { return FISCA.Permission.UserAcl.Current[學生獎勵懲戒通知單].Executable; }
        }

        public static string 班級獎勵懲戒通知單 { get { return "JHSchool.Class.Report0040"; } }

        public static bool 班級獎勵懲戒通知單權限
        {
            get { return FISCA.Permission.UserAcl.Current[班級獎勵懲戒通知單].Executable; }
        }
    }
}
