using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using K12.Data;

namespace K12.Behavior.AttendanceEdit
{
    class EditLog
    {
        /// <summary>
        /// 缺曠資料原始檔 - 缺曠ID,缺曠
        /// </summary>
        public Dictionary<string, AttendanceRecord> beforeData = new Dictionary<string, AttendanceRecord>();

        /// <summary>
        /// 缺曠資料修改後 - 缺曠ID,缺曠
        /// </summary>
        public Dictionary<string, AttendanceRecord> afterData = new Dictionary<string, AttendanceRecord>();

        /// <summary>
        /// 刪除資料
        /// </summary>
        public Dictionary<string, AttendanceRecord> deleteData = new Dictionary<string, AttendanceRecord>();

        /// <summary>
        /// 每一筆缺曠記錄,修改Log
        /// </summary>
        public EditLog()
        {

        }

    }
}
