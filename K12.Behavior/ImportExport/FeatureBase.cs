using System;
using System.Collections.Generic;
using System.Text;
using SmartSchool.Common;
using FISCA.DSAUtil;

namespace K12.Behavior
{
    public class FeatureBase
    {
        public static DSResponse CallService(string serviceName, DSRequest request)
        {
            return FISCA.Authentication.DSAServices.CallService(serviceName, request);            
        }
    }
}
