using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K12.缺曠獎懲週報表
{
    public static class SortClassIndex
    {
        #region K12ClassRecord

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

        static public List<K12.Data.StudentRecord> K12Data_StudentRecord(List<K12.Data.StudentRecord> StudentList)
        {
            //整理出學生&班級資料清單
            List<string> classIDList = new List<string>();
            foreach (K12.Data.StudentRecord student in StudentList)
            {
                if (!string.IsNullOrEmpty(student.RefClassID))
                {
                    if (!classIDList.Contains(student.RefClassID))
                    {
                        classIDList.Add(student.RefClassID);
                    }
                }
            }
            //一次取得班級清單
            List<K12.Data.ClassRecord> classList = K12.Data.Class.SelectByIDs(classIDList);
            //班級ID對照清單
            Dictionary<string, K12.Data.ClassRecord> classDic = new Dictionary<string, K12.Data.ClassRecord>();
            foreach (K12.Data.ClassRecord classRecord in classList)
            {
                if (!classDic.ContainsKey(classRecord.ID))
                {
                    classDic.Add(classRecord.ID, classRecord);
                }
            }

            List<StudentSortObj_K12Data> list = new List<StudentSortObj_K12Data>();
            foreach (K12.Data.StudentRecord student in StudentList)
            {
                if (!string.IsNullOrEmpty(student.RefClassID))
                {
                    StudentSortObj_K12Data obj = new StudentSortObj_K12Data(classDic[student.RefClassID], student);
                    list.Add(obj);
                }
                else
                {
                    StudentSortObj_K12Data obj = new StudentSortObj_K12Data(null, student);
                    list.Add(obj);
                }
            }
            list.Sort(SortK12Data_StudentRecord);

            return list.Select(x => x._StudentRecord).ToList();

        }

        static private int SortK12Data_StudentRecord(StudentSortObj_K12Data obj1, StudentSortObj_K12Data obj2)
        {
            return obj1._SortString.CompareTo(obj2._SortString);
        }
        #endregion
    }

    class StudentSortObj_K12Data
    {
        public K12.Data.StudentRecord _StudentRecord { get; set; }
        public K12.Data.ClassRecord _ClassRecord { get; set; }
        public string _SortString { get; set; }

        public StudentSortObj_K12Data(K12.Data.ClassRecord classRecord, K12.Data.StudentRecord student)
        {
            string ClassYear = ""; //年級
            string ClassIndex = ""; //班級序號
            string ClassName = ""; //班級名稱
            string StudentSeatNo = ""; //學生座號
            string StudentName = ""; //學生姓名

            _StudentRecord = student;

            #region ClassIndex & ClassName
            if (classRecord != null) //如果有班級
            {
                _ClassRecord = classRecord;
                ClassYear = Year(_ClassRecord);
                ClassIndex = Index(_ClassRecord);
                ClassName = _ClassRecord.Name.PadLeft(10, '0');
            }
            else //如果沒有班級
            {
                ClassYear = ClassYear.PadLeft(10, '9');
                ClassIndex = ClassIndex.PadLeft(10, '9');
                ClassName = ClassName.PadLeft(10, '9');
            }
            #endregion

            StudentName = _StudentRecord.Name.PadLeft(10, '0');

            StudentSeatNo = _StudentRecord.SeatNo.HasValue ?
                _StudentRecord.SeatNo.Value.ToString().PadLeft(10, '0') :
                StudentSeatNo.PadLeft(10, '9');

            _SortString = ClassYear + ClassIndex + ClassName + StudentSeatNo + StudentName;
        }

        public string Year(K12.Data.ClassRecord _class)
        {
            string classYear = "";

            if (_class.GradeYear.HasValue)
            {
                classYear = _class.GradeYear.Value.ToString().PadLeft(10, '0');
            }
            else
            {
                classYear = classYear.PadLeft(10, '9');
            }

            return classYear;
        }

        public string Index(K12.Data.ClassRecord _class)
        {
            string classIndex = "";

            if (string.IsNullOrEmpty(_class.DisplayOrder))
            {
                classIndex = _class.DisplayOrder.PadLeft(10, '9');
            }
            else
            {
                classIndex = _class.DisplayOrder.PadLeft(10, '0');
            }

            return classIndex;
        }
    }
}
