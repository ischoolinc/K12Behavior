using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.DSAUtil;

namespace K12.缺曠獎懲週報表
{
    public static class Get
    {
        public static DSResponse GetDiscipline(DSRequest request)
        {
            return FISCA.Authentication.DSAServices.CallService("SmartSchool.Student.Discipline.GetDiscipline", request);
        }

        public static DSResponse GetAttendance(DSRequest request)
        {
            return FISCA.Authentication.DSAServices.CallService("SmartSchool.Student.Attendance.GetAttendance", request);
        }
    }
}
