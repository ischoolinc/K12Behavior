using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using K12.Data;

namespace K12.Student.SpeedAddToTemp
{
    public static class DataSort
    {
        /// <summary>
        /// 取得班級名稱代碼表
        /// </summary>
        static public Dictionary<string, string> GetClassNameDic(string Code)
        {
            //班級代碼/班級名稱
            Dictionary<string, string> Dic = new Dictionary<string, string>();
            List<ClassRecord> classList = Class.SelectAll();
            DataSort.K12Data_ClassRecord(classList);

            K12.Data.Configuration.ConfigData cd = School.Configuration[Code];
            if (cd.Count != 0) //已設定
            {
                foreach (string each in cd) //取出班級名稱
                {
                    if (!Dic.ContainsKey(cd[each]))
                    {
                        Dic[cd[each]] = each;
                    }
                }
            }

            return Dic;
        }

        static public List<K12.Data.ClassRecord> K12Data_ClassRecord(List<K12.Data.ClassRecord> ClassList)
        {
            ClassList.Sort(SortK12Data_ClassRecord);
            return ClassList;
        }

        static private int SortK12Data_ClassRecord(K12.Data.ClassRecord class1, K12.Data.ClassRecord class2)
        {
            string ClassYear1 = class1.GradeYear.HasValue ? class1.GradeYear.Value.ToString().PadLeft(10, '0') : string.Empty.PadLeft(10, '9');
            string ClassYear2 = class2.GradeYear.HasValue ? class2.GradeYear.Value.ToString().PadLeft(10, '0') : string.Empty.PadLeft(10, '9');

            string DisplayOrder1 = "";
            if (string.IsNullOrEmpty(class1.DisplayOrder))
            {
                DisplayOrder1 = class1.DisplayOrder.PadLeft(10, '9');
            }
            else
            {
                DisplayOrder1 = class1.DisplayOrder.PadLeft(10, '0');
            }
            string DisplayOrder2 = "";
            if (string.IsNullOrEmpty(class2.DisplayOrder))
            {
                DisplayOrder2 = class2.DisplayOrder.PadLeft(10, '9');
            }
            else
            {
                DisplayOrder2 = class2.DisplayOrder.PadLeft(10, '0');
            }

            string ClassName1 = class1.Name.PadLeft(10, '0');
            string ClassName2 = class2.Name.PadLeft(10, '0');

            string Compareto1 = ClassYear1 + DisplayOrder1 + ClassName1;
            string Compareto2 = ClassYear2 + DisplayOrder2 + ClassName2;

            return Compareto1.CompareTo(Compareto2);
        }

    }
}
