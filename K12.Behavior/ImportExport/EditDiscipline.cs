using System;
using System.Collections.Generic;
using System.Text;
using FISCA.DSAUtil;

namespace K12.Behavior
{
    public class EditDiscipline : FeatureBase
    {
        [QueryRequest()]
        public static void Delete(DSRequest dSRequest)
        {
            CallService("SmartSchool.Student.Discipline.Delete", dSRequest);
        }

        [QueryRequest()]
        public static void Update(DSRequest dSRequest)
        {
            CallService("SmartSchool.Student.Discipline.Update", dSRequest);
        }

        public static void Insert(DSRequest dSRequest)
        {
            CallService("SmartSchool.Student.Discipline.Insert", dSRequest);
        }
    }
}
