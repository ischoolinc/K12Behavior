using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.Authentication;
using FISCA.DSAUtil;

namespace K12.Behavior.DisciplineNotification
{
    public class Config
    {
        [AutoRetryOnWebException()]
        public static DSResponse GetMDReduce()
        {
            return DSAServices.CallService("SmartSchool.Config.GetMDReduce", new DSRequest());
        }

    }
}
