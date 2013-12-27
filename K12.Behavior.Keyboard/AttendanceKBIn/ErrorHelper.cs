using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using K12.Data;

namespace K12.Behavior.Keyboard
{
    class ErrorHelper
    {
        private Dictionary<string, List<StudentRecord>> _ClassAndStudentList;

        public ErrorHelper()
        {
            //全校學生資訊
            _ClassAndStudentList = new Dictionary<string, List<StudentRecord>>();
            //List<StudentRecord> list = new List<StudentRecord>();
            foreach (ClassRecord each in Class.SelectAll())
            {
                _ClassAndStudentList.Add(each.Name, each.Students);
            }
        }

        /// <summary>
        /// 檢查是否存在此班級
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool CheckInClassName(string ClassName)
        {
            return (_ClassAndStudentList.ContainsKey(ClassName)) ? true : false;

        }

                /// <summary>
        /// 檢查班級,座號是否存在
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool CheckInSeatNo(string ClassName, string Num)
        {
            if (_ClassAndStudentList.ContainsKey(ClassName))
            {
                foreach (K12.Data.StudentRecord each in _ClassAndStudentList[ClassName])
                {
                    if (each.SeatNo.ToString() == Num)
                    {
                        return true;
                    }
                }
            }
            else
            {
                return false;
            }

            return false;
        }

        /// <summary>
        /// 檢查學號是否存在
        /// </summary>
        /// <param name="StudentNumber"></param>
        /// <returns></returns>
        public bool CheckInStudentNumber(string SN)
        {
            foreach (List<StudentRecord> each in _ClassAndStudentList.Values)
            {
                foreach (StudentRecord stud in each)
                {
                    if (stud.StudentNumber == SN)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 以班級,座號取得StudentRecord
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public StudentRecord GetInSeatNo(string ClassName, string Num)
        {
            if (_ClassAndStudentList.ContainsKey(ClassName))
            {
                foreach (StudentRecord each in _ClassAndStudentList[ClassName])
                {
                    if (each.SeatNo.ToString() == Num)
                    {
                        return each;
                    }
                }
            }
            else
            {
                return null;
            }

            return null;
        }

        /// <summary>
        /// 以學號取得StudentRecord
        /// </summary>
        /// <param name="StudentNumber"></param>
        /// <returns></returns>
        public StudentRecord GetInStudentNumber(string SN)
        {
            foreach (List<StudentRecord> each in _ClassAndStudentList.Values)
            {
                foreach (StudentRecord stud in each)
                {
                    if (stud.StudentNumber == SN)
                    {
                        return stud;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 將8碼之時間,插入"\"符號
        /// </summary>
        /// <param name="TimeString"></param>
        /// <returns></returns>
        private string InsertSlash(string Time)
        {
            if (Time == "")
                return "";

            if(Time.Length != 8)
                return "";

            return Time.Insert(4, "/").Insert(7, "/");
        }

        /// <summary>
        /// 時間錯誤檢查
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private bool CheckDateTime(string date)
        {
            if (date == "")
            {
                return false;
            }

            if (date.Length != 8)
            {
                return false;
            }

            date = InsertSlash(date); //呼叫插入"/"方法

            DateTime try_value;
            if (DateTime.TryParse(date, out try_value))
            {
                return true;
            }
            return false;
        }
    }
}
