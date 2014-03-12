using System;
using System.Collections.Generic;
using System.Text;
using FISCA.DSAUtil;

namespace K12.Behavior
{
    public class EditAttendance : FeatureBase
    {
        [QueryRequest()]
        public static void Delete(DSRequest dSRequest)
        {
            CallService("SmartSchool.Student.Attendance.Delete", dSRequest);
        }

        [QueryRequest()]
        public static void Update(DSRequest dSRequest)
        {
            CallService("SmartSchool.Student.Attendance.Update", dSRequest);
        }

        public static void Insert(DSRequest dSRequest)
        {
            CallService("SmartSchool.Student.Attendance.Insert", dSRequest);
        }
    }
}
