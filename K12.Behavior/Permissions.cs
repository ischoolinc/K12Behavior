using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using FISCA.Permission;

namespace K12.Behavior
{
    /// <summary>
    /// 代表目前使用者的相關權限資訊。
    /// </summary>
    public static class Permissions
    {
        //毛毛蟲權限(對應於毛毛蟲內之權限字串)
        public static string 獎勵記錄 { get { return "K12.Student.MeritItem"; } }
        public static string 懲戒記錄 { get { return "K12.Student.DemeritItem"; } }
        public static string 缺曠記錄 { get { return "K12.Student.AttendanceItem"; } }

        public static string 獎勵 { get { return "K12.Student.MeritEditForm"; } }
        public static string 懲戒 { get { return "K12.Student.DemeritEditForm"; } }

        public static string 缺曠類別管理 { get { return "K12.Student.AbsenceConfigForm"; } }
        public static string 功過換算管理 { get { return "K12.Student.ReduceForm"; } }
        public static string 獎懲事由管理 { get { return "K12.Student.DisciplineForm"; } }

        public static string 獎懲批次修改 { get { return "K12.Student.AttendanceEditForm"; } }
        //public static string 缺曠資料檢視 { get { return "K12.Student.MerDemEditForm"; } }
        public static string 銷過記錄清單 { get { return "K12.Student.StudentDemeritClear"; } }

        public static bool 獎勵權限
        {
            get { return FISCA.Permission.UserAcl.Current[獎勵].Executable; }
        }

        public static bool 懲戒權限
        {
            get { return FISCA.Permission.UserAcl.Current[懲戒].Executable; }
        }

        public static bool 缺曠類別管理權限
        {
            get { return FISCA.Permission.UserAcl.Current[缺曠類別管理].Executable; }
        }

        public static bool 功過換算管理權限
        {
            get { return FISCA.Permission.UserAcl.Current[功過換算管理].Executable; }
        }

        public static bool 獎懲事由管理權限
        {
            get { return FISCA.Permission.UserAcl.Current[獎懲事由管理].Executable; }
        }

        public static bool 獎懲批次修改權限
        {
            get { return FISCA.Permission.UserAcl.Current[獎懲批次修改].Executable; }
        }


        public static string 匯出獎懲記錄 { get { return "Button0190"; } }
        public static string 匯出缺曠記錄 { get { return "Button0180"; } }
        public static string 匯入獎懲記錄 { get { return "Button0270"; } }
        public static string 匯入缺曠記錄 { get { return "Button0260"; } }

        public static bool 匯出獎懲記錄權限
        {
            get { return FISCA.Permission.UserAcl.Current[匯出獎懲記錄].Executable; }
        }

        public static bool 匯入獎懲記錄權限
        {
            get { return FISCA.Permission.UserAcl.Current[匯入獎懲記錄].Executable; }
        }

        public static bool 匯出缺曠記錄權限
        {
            get { return FISCA.Permission.UserAcl.Current[匯出缺曠記錄].Executable; }
        }

        public static bool 匯入缺曠記錄權限
        {
            get { return FISCA.Permission.UserAcl.Current[匯入缺曠記錄].Executable; }
        }

        //public static bool 缺曠資料檢視權限
        //{
        //    get { return FISCA.Permission.UserAcl.Current[缺曠資料檢視].Executable; }
        //}

        //public static bool 銷過記錄清單權限
        //{
        //    get { return FISCA.Permission.UserAcl.Current[銷過記錄清單].Executable; }
        //}
    }
}
