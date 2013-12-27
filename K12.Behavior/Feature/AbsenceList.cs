//using System.Collections.Generic;
//using System.Text;
//using System.Xml;
//using FISCA.DSAUtil;
//using K12.Data;

//namespace K12.Behavior.Feature
//{
//    [FISCA.Authentication.AutoRetryOnWebException()]
//    public class QueryAbsenceMapping
//    {
//        private static string GET_ABSENCE_LIST = "SmartSchool.Others.GetAbsenceList";

//        public QueryAbsenceMapping() { }

//        public static List<AbsenceMappingInfo> Load()
//        {
//            StringBuilder req = new StringBuilder("<Request><Field><Content/><All/></Field></Request>");
//            List<AbsenceMappingInfo> result = new List<AbsenceMappingInfo>();

//            foreach (XmlElement item in K12.Data.Utility.DSAServices.CallService(GET_ABSENCE_LIST, new DSRequest(req.ToString())).GetContent().GetElements("Absence"))
//            {
//                result.Add(new AbsenceMappingInfo(item));
//            }

//            return result;
//        }
//    }

//}