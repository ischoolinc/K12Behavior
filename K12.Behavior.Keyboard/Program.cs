using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA;
using FISCA.Presentation;
using FISCA.Permission;

namespace K12.Behavior.Keyboard
{
    public static class Program
    {
        [MainMethod()]
        public static void Main()
        {
            //string URL缺曠鍵盤登錄 = "ischool/高中系統/共用/學務/學務作業/鍵盤輸入學生缺曠資料";
            //string URL獎懲鍵盤登錄 = "ischool/高中系統/共用/學務/學務作業/鍵盤輸入學生獎懲資料";

            string txt缺曠鍵盤登錄 = "缺曠鍵盤登錄";
            string txt獎懲鍵盤登錄 = "獎懲鍵盤登錄";

            RibbonBarItem KBKeyIn = FISCA.Presentation.MotherForm.RibbonBarItems["學務作業", "鍵盤作業"];

            KBKeyIn[txt缺曠鍵盤登錄].Enable = FISCA.Permission.UserAcl.Current["JHSchool.StuAdmin.Ribbon0070"].Executable;
            KBKeyIn[txt缺曠鍵盤登錄].Image = Properties.Resources.polygon_64;
            KBKeyIn[txt缺曠鍵盤登錄].Click += delegate
            {
                new RtAttendanceKBInput().ShowDialog();
            };


            KBKeyIn[txt獎懲鍵盤登錄].Enable = FISCA.Permission.UserAcl.Current["JHSchool.StuAdmin.Ribbon0080"].Executable;
            KBKeyIn[txt獎懲鍵盤登錄].Image = Properties.Resources.star_64;
            KBKeyIn[txt獎懲鍵盤登錄].Click += delegate
            {
                new PdMeritDemeritKBInput().ShowDialog();
            };

            Catalog ribbon = RoleAclSource.Instance["學務作業"];
            ribbon.Add(new RibbonFeature("JHSchool.StuAdmin.Ribbon0070", txt缺曠鍵盤登錄));
            ribbon.Add(new RibbonFeature("JHSchool.StuAdmin.Ribbon0080", txt獎懲鍵盤登錄));
        }
    }
}
