using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K12.學生獎勵懲戒明細
{
    //每一筆獎懲資料,相關資訊
    class DisciplineStatistics
    {
        public string RefStudentID;
        public int 大功 { get; set; }
        public int 小功 { get; set; }
        public int 嘉獎 { get; set; }

        public int 大過 { get; set; }
        public int 小過 { get; set; }
        public int 警告 { get; set; }

        //初始化
        public DisciplineStatistics(string studentID)
        {
            RefStudentID = studentID;
            大功 = 0;
            小功 = 0;
            嘉獎 = 0;

            大過 = 0;
            小過 = 0;
            警告 = 0;
        }
    }
}
