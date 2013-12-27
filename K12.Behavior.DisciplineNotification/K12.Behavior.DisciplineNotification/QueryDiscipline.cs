using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.DSAUtil;

namespace K12.Behavior.DisciplineNotification
{
    [FISCA.Authentication.AutoRetryOnWebException()]
    public class QueryDiscipline
    {
        public static DSResponse GetDiscipline(DSRequest request)
        {
            return FISCA.Authentication.DSAServices.CallService("SmartSchool.Student.Discipline.GetDiscipline", request);
        }
    }
}
