using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.Permission;

namespace K12.DemeritClearList
{
    /// <summary>
    /// 代表目前使用者的相關權限資訊。
    /// </summary>
    public static class Permissions
    {
        public static string 銷過記錄清單_高中 { get { return "K12.Student.StudentDemeritClear"; } }

        public static bool 銷過記錄清單權限_高中
        {
            get { return FISCA.Permission.UserAcl.Current[銷過記錄清單_高中].Executable; }
        }

        public static string 銷過記錄清單_國中 { get { return "JHSchool.StuAdmin.Ribbon0100"; } }

        public static bool 銷過記錄清單權限_國中
        {
            get { return FISCA.Permission.UserAcl.Current[銷過記錄清單_國中].Executable; }
        }
    }
}
