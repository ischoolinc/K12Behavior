using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using K12.Data;
using System.Data;

namespace K12.Student.SpeedAddToTemp
{
    class StudentMag
    {
        FISCA.Data.QueryHelper _queryHelper = new FISCA.Data.QueryHelper();


        Dictionary<string, Dictionary<string, studentObj>> dic = new Dictionary<string, Dictionary<string, studentObj>>();

        Dictionary<string, studentObj> StudentNumberDic = new Dictionary<string, studentObj>();


        public StudentMag()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select class.id as class_id,class.class_name,student.id as studnt_id,student.seat_no,student.name,student.student_number ");
            sb.Append("from student left join class on student.ref_class_id=class.id ");
            sb.Append("where student.status=1");

            DataTable dt = _queryHelper.Select(sb.ToString());



            foreach (DataRow each in dt.Rows)
            {
                studentObj obj = new studentObj(each);

                string ClassName = obj.class_name;

                if (!string.IsNullOrEmpty(ClassName) && !string.IsNullOrEmpty(obj.student_seat_no))
                {
                    if (!dic.ContainsKey(ClassName))
                    {
                        dic.Add(obj.class_name, new Dictionary<string, studentObj>());
                    }

                    if (!dic[ClassName].ContainsKey(obj.student_seat_no))
                    {
                        dic[ClassName].Add(obj.student_seat_no, obj);
                    }
                }

                string studentnumber = obj.student_number;

                if (!StudentNumberDic.ContainsKey(studentnumber))
                {
                    StudentNumberDic.Add(studentnumber, obj);
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
        public studentObj IsSeatNo(string className, string SeatNo)
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

        /// <summary>
        /// 是否有此學號
        /// </summary>
        public studentObj IsStudentNumber(string StudentNumber)
        {
            if (StudentNumberDic.ContainsKey(StudentNumber))
            {
                return StudentNumberDic[StudentNumber];
            }

            return null;
        }
    }

    class studentObj
    {
        public string class_name { get; set; }
        public string class_id { get; set; }
        public string student_id { get; set; }
        public string student_name { get; set; }
        public string student_seat_no { get; set; }
        public string student_number { get; set; }

        public studentObj(DataRow row)
        {
            class_name = "" + row["class_name"];
            class_id = "" + row["class_id"];
            student_id = "" + row["studnt_id"];
            student_name = "" + row["name"];
            student_seat_no = "" + row["seat_no"];
            if (student_seat_no.Length < 2)
            {
                student_seat_no = student_seat_no.PadLeft(2, '0');

            }

            student_number = "" + row["student_number"];
        }

    }
}
