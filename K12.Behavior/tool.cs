using K12.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace K12.Behavior
{
    public class tool
    {
        public static FISCA.UDT.AccessHelper _A = new FISCA.UDT.AccessHelper();

        public static FISCA.Data.QueryHelper _Q = new FISCA.Data.QueryHelper();

        /// <summary>
        /// 取得常用備註欄位資料
        /// 0是懲戒 1 是獎勵
        /// </summary>
        public static List<string> GerRemarkTitle(string type)
        {
            List<string> remarkList = new List<string>();
            remarkList.Add("");
            DataTable dt = _Q.Select(string.Format(@"select DISTINCT(remark) from discipline 
where merit_flag={0} and remark is not null order by remark", type));

            foreach (DataRow row in dt.Rows)
            {
                string remark = "" + row["remark"];
                remarkList.Add(remark);
            }
            return remarkList;
        }

        static public int SortComparer(ClassRecord xClass, ClassRecord yClass)
        {

            string XGradeYear = xClass.GradeYear.HasValue ? xClass.GradeYear.Value.ToString().PadLeft(8, '0') : "99999999";
            string YGradeYear = yClass.GradeYear.HasValue ? yClass.GradeYear.Value.ToString().PadLeft(8, '0') : "99999999";

            XGradeYear += !string.IsNullOrEmpty(xClass.DisplayOrder) ? xClass.DisplayOrder.PadLeft(8, '0') : "99999999";
            YGradeYear += !string.IsNullOrEmpty(yClass.DisplayOrder) ? yClass.DisplayOrder.PadLeft(8, '0') : "99999999";

            XGradeYear += xClass.Name.PadLeft(8, '0');
            YGradeYear += yClass.Name.PadLeft(8, '0');

            Console.WriteLine(XGradeYear);
            Console.WriteLine(YGradeYear);
            Console.WriteLine(XGradeYear.CompareTo(YGradeYear));

            return XGradeYear.CompareTo(YGradeYear);
        }

        static public int SortComparerInStudent(StRecord xStudent, StRecord yStudent)
        {
            #region 排序學生


            string XGradeYear = xStudent.GradeYear.PadLeft(8, '0');
            string YGradeYear = yStudent.GradeYear.PadLeft(8, '0');

            XGradeYear += xStudent.DisplayOrder.PadLeft(8, '0');
            YGradeYear += yStudent.DisplayOrder.PadLeft(8, '0');

            XGradeYear += xStudent.ClassName.PadLeft(8, '0');
            YGradeYear += yStudent.ClassName.PadLeft(8, '0');

            XGradeYear += xStudent.SeatNo.PadLeft(8, '0');
            YGradeYear += yStudent.SeatNo.PadLeft(8, '0');

            XGradeYear += xStudent.StudentName.PadLeft(8, '0');
            YGradeYear += yStudent.StudentName.PadLeft(8, '0');

            return XGradeYear.CompareTo(YGradeYear);

            #endregion
        }

    }

    public class StRecord
    {
        public StRecord(DataRow row)
        {
            StudentID = "" + row["student_id"];
            StudentName = "" + row["student_name"];
            StudentNumber = "" + row["student_number"];
            if ("" + row["gender"] == "0")
                Gender = "女";
            else if ("" + row["gender"] == "1")
                Gender = "男";
            else
                Gender = "";

            SeatNo = "" + row["seat_no"];
            ClassName = "" + row["class_name"];
            DisplayOrder = "" + row["display_order"];
            GradeYear = "" + row["grade_year"];
            RefClassID = "" + row["class_id"];
        }

        public string StudentID { get; set; }
        public string StudentName { get; set; }
        public string StudentNumber { get; set; }
        public string Gender { get; set; }
        public string SeatNo { get; set; }
        public string ClassName { get; set; }

        public string GradeYear { get; set; }
        public string DisplayOrder { get; set; }
        public string RefClassID { get; set; }


    }
}
