using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace K12.Behavior.AttendanceEdit
{
    public class StudentObj
    {
        /// <summary>
        /// 取得全校學生
        /// </summary>
        public List<StudentData> GetAllStudent()
        {
            //1.全校學生
            List<StudentData> studentList = new List<StudentData>();
            FISCA.Data.QueryHelper _queryHelper = new FISCA.Data.QueryHelper();
            StringBuilder sb = new StringBuilder();
            sb.Append("select student.id,student.name,student.ref_class_id,class.class_name,student.seat_no,student.student_number,student.gender ");
            sb.Append("from student left join class on student.ref_class_id=class.id");
            DataTable table = _queryHelper.Select(sb.ToString());

            foreach (DataRow row in table.Rows)
            {
                StudentData obj = new StudentData(row);
                studentList.Add(obj);
            }

            return studentList;
        }

        /// <summary>
        /// 取得全校班級名稱清單
        /// </summary>
        public Dictionary<string, string> GetAllClassName()
        {
            //1.全校學生
            Dictionary<string, string> ClassNameDic = new Dictionary<string, string>();
            FISCA.Data.QueryHelper _queryHelper = new FISCA.Data.QueryHelper();
            StringBuilder sb = new StringBuilder();
            sb.Append("select id,class_name from class ");
            DataTable table = _queryHelper.Select(sb.ToString());
            foreach (DataRow row in table.Rows)
            {
                string id = "" + row["id"];
                string class_name = "" + row["class_name"];

                if (!ClassNameDic.ContainsKey(id))
                    ClassNameDic.Add(id, class_name);
            }

            return ClassNameDic;
        }

        /// <summary>
        /// 依年級,取得學生
        /// </summary>
        public List<StudentData> GetGradeYearStudent(string grade_year)
        {
            List<StudentData> studentList = new List<StudentData>();
            FISCA.Data.QueryHelper _queryHelper = new FISCA.Data.QueryHelper();

            StringBuilder sb = new StringBuilder();
            sb.Append("select student.id,student.name,student.ref_class_id,student.seat_no,student.student_number,student.gender,class.class_name,class.grade_year ");
            sb.Append("from student,class ");
            sb.Append("where student.ref_class_id = class.id ");
            sb.Append("and class.grade_year =" + grade_year);
            DataTable table = _queryHelper.Select(sb.ToString());

            foreach (DataRow row in table.Rows)
            {
                StudentData obj = new StudentData(row);
                studentList.Add(obj);
            }
            return studentList;
        }

        /// <summary>
        /// 依班級名稱取得學生
        /// </summary>
        public List<StudentData> GetClassNameStudent(string classname)
        {
            List<StudentData> studentList = new List<StudentData>();
            FISCA.Data.QueryHelper _queryHelper = new FISCA.Data.QueryHelper();

            StringBuilder sb = new StringBuilder();
            sb.Append("select student.id,student.name,student.ref_class_id,student.seat_no,student.student_number,student.gender,class.class_name,class.grade_year ");
            sb.Append("from student,class ");
            sb.Append("where student.ref_class_id = class.id ");
            sb.Append(string.Format("and class.class_name ='{0}'", classname));
            DataTable table = _queryHelper.Select(sb.ToString());

            foreach (DataRow row in table.Rows)
            {
                StudentData obj = new StudentData(row);
                studentList.Add(obj);
            }
            return studentList;
        }

        /// <summary>
        /// 依班級名稱與座號取得學生
        /// </summary>
        public List<StudentData> GetClassNameStudent(string classname, string studentSeatno)
        {
            List<StudentData> studentList = new List<StudentData>();
            FISCA.Data.QueryHelper _queryHelper = new FISCA.Data.QueryHelper();

            StringBuilder sb = new StringBuilder();
            sb.Append("select student.id,student.name,student.ref_class_id,student.seat_no,student.student_number,student.gender,class.class_name,class.grade_year ");
            sb.Append("from student,class ");
            sb.Append("where student.ref_class_id = class.id ");
            sb.Append(string.Format("and class.class_name ='{0}'", classname));
            sb.Append(string.Format("and student.seat_no ='{0}'", studentSeatno));
            DataTable table = _queryHelper.Select(sb.ToString());

            foreach (DataRow row in table.Rows)
            {
                StudentData obj = new StudentData(row);
                studentList.Add(obj);
            }
            return studentList;
        }

        /// <summary>
        /// 傳入學號,依學號取得學生
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public List<StudentData> GetStudentNumber(string number)
        {
            List<StudentData> studentList = new List<StudentData>();
            FISCA.Data.QueryHelper _queryHelper = new FISCA.Data.QueryHelper();

            StringBuilder sb = new StringBuilder();
            sb.Append("select student.id,student.name,student.ref_class_id,student.seat_no,student.student_number,student.gender,class.class_name,class.grade_year ");
            sb.Append("from student,class ");
            sb.Append("where student.ref_class_id = class.id ");
            sb.Append(string.Format("and student_number = '{0}'", number));
            DataTable table = _queryHelper.Select(sb.ToString());

            foreach (DataRow row in table.Rows)
            {
                StudentData obj = new StudentData(row);
                studentList.Add(obj);
            }
            return studentList;
        }

        /// <summary>
        /// 取得班級名稱,並依[班級排序資料/名稱]進行排序
        /// </summary>
        /// <returns></returns>
        public List<string> GetClassNameList()
        {
            List<string> ClassNameList = new List<string>();
            FISCA.Data.QueryHelper _queryHelper = new FISCA.Data.QueryHelper();
            DataTable table = _queryHelper.Select("select DISTINCT class.class_name,class.display_order from student,class where student.ref_class_id=class.id order by class.display_order,class.class_name");
            foreach (DataRow row in table.Rows)
            {
                string classname = "" + row["class_name"];
                if (!ClassNameList.Contains(classname))
                {
                    ClassNameList.Add(classname);
                }
            }
            return ClassNameList;
        }


        /// <summary>
        /// 取得目前年級資料
        /// </summary>
        /// <returns></returns>
        public List<string> GetGradeYearList()
        {
            List<string> GradeYearList = new List<string>();
            FISCA.Data.QueryHelper _queryHelper = new FISCA.Data.QueryHelper();
            DataTable table = _queryHelper.Select("select DISTINCT grade_year from class where grade_year is not null order by grade_year");
            foreach (DataRow row in table.Rows)
            {
                string GradeYear = "" + row["grade_year"];
                if (!GradeYearList.Contains(GradeYear))
                {
                    GradeYearList.Add(GradeYear);
                }
            }
            return GradeYearList;
        }

        /// <summary>
        /// 傳入日期,學生ID
        /// 即可知道當日該名學生是否有資料
        /// True = 當日有資料　
        /// False = 當日並無資料
        /// </summary>
        public bool CheckStudentDateWhen(DateTime dt,string studentID)
        {
            FISCA.Data.QueryHelper _queryHelper = new FISCA.Data.QueryHelper();
            string TimeDisplayFormat = "yyyy/MM/dd";

            StringBuilder sb = new StringBuilder();
            sb.Append("select student.id,student.name,attendance.occur_date from student,attendance ");
            sb.Append("where student.id=attendance.ref_student_id ");
            sb.Append(string.Format("and occur_date ='{0}' ", dt.ToString(TimeDisplayFormat)));
            sb.Append(string.Format("and student.id='{0}'", studentID));
            DataTable table = _queryHelper.Select(sb.ToString());

            if (table.Rows.Count > 0)
                return true;
            else
                return false;
        }
    }

    /// <summary>
    /// 學生ID,學號,座號,姓名,年級,班級ID,班級
    /// </summary>
    public class StudentData
    {
        /// <summary>
        /// 學生ID
        /// </summary>
        public string ref_student_id { get; set; }
        /// <summary>
        /// 學號
        /// </summary>
        public string student_number { get; set; }
        /// <summary>
        /// 座號
        /// </summary>
        public string seat_no { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 年級
        /// </summary>
        public string grade_year { get; set; }
        /// <summary>
        /// 班級ID
        /// </summary>
        public string ref_class_id { get; set; }
        /// <summary>
        /// 班級
        /// </summary>
        public string class_name { get; set; }
        /// <summary>
        /// 性別
        /// </summary>
        public string gender { get; set; }

        public StudentData(DataRow row)
        {
            ref_student_id = "" + row["id"];
            student_number = "" + row["student_number"];
            seat_no = "" + row["seat_no"];
            name = "" + row["name"];
            gender = "" + row["gender"];

            if (row.Table.Columns.Contains("grade_year"))
                grade_year = "" + row["grade_year"];
            if (row.Table.Columns.Contains("class_name"))
            {
                class_name = "" + row["class_name"];
            }
            if (row.Table.Columns.Contains("ref_class_id"))
                ref_class_id = "" + row["ref_class_id"];
        }


    }
}
