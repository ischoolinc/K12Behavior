using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.DSAUtil;
using FISCA.Authentication;

namespace K12.Behavior.DisciplineNotification
{
    class QueryStudent
    {
        public static DSResponse GetMultiParentInfo(params string[] idlist)
        {
            DSRequest dsreq = new DSRequest();
            DSXmlHelper helper = new DSXmlHelper("GetParentInfoRequest");
            helper.AddElement("Field");
            helper.AddElement("Field", "All");
            helper.AddElement("Condition");
            foreach (string id in idlist)
            {
                helper.AddElement("Condition", "ID", id);
            }
            dsreq.SetContent(helper);
            return DSAServices.CallService("SmartSchool.Student.GetMultiParentInfo", dsreq);
        }
    }
}
