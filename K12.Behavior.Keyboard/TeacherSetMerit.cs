using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;

namespace K12.Behavior.Keyboard
{
    public class TeacherSetMerit : ActiveRecord
    {
        /// <summary>
        /// 獎勵系統ID
        /// </summary>
        [Field(Field = "MeritID", Indexed = true)]
        public string DisciplineID { get; set; }

        /// <summary>
        /// 是否為老師註記
        /// </summary>
        [Field(Field = "IsTeacherNote", Indexed = false)]
        public bool IsTeacherNote { get; set; }
    }
}
