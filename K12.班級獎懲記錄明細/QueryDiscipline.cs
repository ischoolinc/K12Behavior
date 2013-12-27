using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.DSAUtil;
using FISCA.Authentication;

namespace K12.ClassMeritDemerit.Detail
{
    [AutoRetryOnWebException()]
    class QueryDiscipline
    {
        public static DSResponse GetDiscipline(DSRequest request)
        {
            return DSAServices.CallService("SmartSchool.Student.Discipline.GetDiscipline", request);
        }
    }
}
