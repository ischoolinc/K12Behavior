using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using K12.Data;

namespace K12.Behavior.AttendanceEdit
{
    class EditBot
    {

        /// <summary>
        /// 先
        /// </summary>
        AttendanceRecord _beforear2;

        /// <summary>
        /// 後
        /// </summary>
        AttendanceRecord _afterar1;

        /// <summary>
        /// 差異字串
        /// </summary>
        public StringBuilder sb = new StringBuilder();

        StudentData _student;


        //傳入2缺曠資料,以比對是否修改資料內容
        public EditBot(AttendanceRecord ar1, AttendanceRecord ar2, StudentData student)
        {
            //_beforear2 = new AttendanceRecord();
            //_beforear2.OccurDate = ar1.OccurDate;
            //_beforear2.SchoolYear = ar1.SchoolYear;
            //_beforear2.Semester = ar1.Semester;
            //_beforear2.PeriodDetail = ar1.PeriodDetail;
            _beforear2 = ar1;
            _afterar1 = ar2;
            _student = student;
        }

        public void CheckChange()
        {
            sb.AppendLine("已進行「修改缺曠」動作");
            sb.AppendLine("班級「" + _student.class_name + "」座號「" + _student.seat_no + "」學號「" + _student.student_number + "」姓名「" + _student.name + "」");

            bool edit = false;
            //檢查日期是否修改
            if (_beforear2.OccurDate != _afterar1.OccurDate)
            {
                edit = true;
                sb.Append("缺曠系統編號「" + _afterar1.ID + "」之記錄");
                sb.Append("日期由「" + _beforear2.OccurDate.ToShortDateString() + "」");
                sb.AppendLine("修改為「" + _afterar1.OccurDate.ToShortDateString() + "」");
            }
            else
            {
                sb.AppendLine("日期「" + _beforear2.OccurDate.ToShortDateString() + "」");
            }

            //檢查學年度學期是否修改
            if (_beforear2.SchoolYear != _afterar1.SchoolYear)
            {
                edit = true;
                sb.Append("學年度由「" + _beforear2.SchoolYear + "」");
                sb.AppendLine("修改為「" + _afterar1.SchoolYear + "」");
            }
            if (_beforear2.Semester != _afterar1.Semester)
            {
                edit = true;
                sb.Append("學期由「" + _beforear2.Semester + "」");
                sb.AppendLine("修改為「" + _afterar1.Semester + "」");
            }

            //檢查缺曠內容是否修改 或刪除
            foreach (AttendancePeriod beforear in _beforear2.PeriodDetail)
            {
                string action = "delete";
                foreach (AttendancePeriod afterar in _afterar1.PeriodDetail)
                {
                    //當找到該節次
                    if (beforear.Period == afterar.Period)
                    {
                        action = "update";
                        //缺曠別不一致
                        if (beforear.AbsenceType != afterar.AbsenceType)
                        {
                            edit = true;
                            sb.Append("節次「" + beforear.Period + "」缺曠別由「" + beforear.AbsenceType + "」");
                            sb.AppendLine("修改為「" + afterar.AbsenceType + "」");
                        }
                        break;
                    }
                }
                if (action == "delete")
                {
                    edit = true;
                    sb.Append("節次「" + beforear.Period + "」缺曠別由「" + beforear.AbsenceType + "」");
                    sb.AppendLine("修改為「" +" "+ "」");
                }
            }

            //反查是否有由空值修改為特定缺曠的狀況
            foreach (AttendancePeriod afterar in _afterar1.PeriodDetail)
            {
                bool test1 = false;

                foreach (AttendancePeriod beforear in _beforear2.PeriodDetail)
                {
                    //當找到該節次
                    if (beforear.Period == afterar.Period)
                    {
                        test1 = true;
                        break;
                    }
                }

                if (!test1)
                {
                    edit = true;
                    sb.Append("節次「" + afterar.Period + "」缺曠別由「」");
                    sb.AppendLine("修改為「" + afterar.AbsenceType + "」");
                }
            }

            if (!edit)
            {
                sb.Clear();
            }
        }
    }
}
