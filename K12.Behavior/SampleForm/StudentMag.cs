using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using K12.Data;

namespace K12.Behavior
{
    class StudentMag
    {
        Dictionary<string,Dictionary<string,StudentRecord>> dic = new Dictionary<string,Dictionary<string,StudentRecord>>();

        public StudentMag()
        {
            foreach (StudentRecord each in Student.SelectAll())
            {
                if (string.IsNullOrEmpty(each.RefClassID)) //如果沒有班級ID
                    continue;

                if (!each.SeatNo.HasValue) //沒有座號
                    continue;

                string ClassName = each.Class.Name;

                if (!dic.ContainsKey(ClassName))
                {
                    dic.Add(ClassName, new Dictionary<string, StudentRecord>());
                }

                if (!dic[ClassName].ContainsKey(each.SeatNo.Value.ToString()))
                {
                    dic[ClassName].Add(each.SeatNo.Value.ToString(), each);
                }
            }
        }

        /// <summary>
        /// 是否有此班級
        /// </summary>
        public bool IsClassName(string className)
        {
            return dic.ContainsKey(className);
        }

        /// <summary>
        /// 是否有此學生
        /// </summary>
        /// <param name="className"></param>
        /// <param name="SeatNo"></param>
        /// <returns></returns>
        public StudentRecord IsSeatNo(string className, string SeatNo)
        {
            if (dic.ContainsKey(className))
            {
                if (dic[className].ContainsKey(SeatNo))
                {
                    return dic[className][SeatNo];
                }
            }
            return null;
        }
    }
}
