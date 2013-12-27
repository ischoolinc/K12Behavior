using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K12.Student.SpeedAddToTemp
{
    class Permissions
    {
        public static string 快速待處理_學生 { get { return "K12.Student.SpeedAddToTemp.0412"; } }

        public static bool 快速待處理_學生權限
        {
            get
            {
                return FISCA.Permission.UserAcl.Current[快速待處理_學生].Executable;
            }
        }
    }
}
