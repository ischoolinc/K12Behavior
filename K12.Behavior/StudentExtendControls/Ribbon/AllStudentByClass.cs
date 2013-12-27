using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using K12.Data;

namespace K12.Behavior.StudentExtendControls.Ribbon
{
    class AllStudentByClass
    {
        Dictionary<string, Dictionary<string, StudentRecord>> ClassNameByStudent = new Dictionary<string, Dictionary<string, StudentRecord>>();

        public AllStudentByClass()
        {
            foreach (StudentRecord student in Student.SelectAll())
            {
                if (student.Class != null && student.SeatNo.HasValue)
                {
                    if (!ClassNameByStudent.ContainsKey(student.Class.Name))
                    {
                        ClassNameByStudent.Add(student.Class.Name, new Dictionary<string, StudentRecord>());
                    }

                    ClassNameByStudent[student.Class.Name].Add(student.SeatNo.Value.ToString(), student);
                }

            }
        }

        public bool GetClassName(string className)
        {
            if (ClassNameByStudent.ContainsKey(className))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool GetClassAndSeatNo(string className, string SeatNo)
        {
            if (ClassNameByStudent.ContainsKey(className))
            {
                if (ClassNameByStudent[className].ContainsKey(SeatNo))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
    }
}
