using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;

namespace K12.Behavior.Keyboard
{
    public class TeacherSetDemerit : ActiveRecord
    {
        /// <summary>
        /// 懲戒系統ID
        /// </summary>
        [Field(Field = "DemeritID", Indexed = true)]
        public string DemeritID { get; set; }

        /// <summary>
        /// 是否為老師註記
        /// </summary>
        [Field(Field = "IsTeacherNote", Indexed = false)]
        public bool IsTeacherNote { get; set; }
    }
}
