using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K12.Behavior.BatchClearDemerit
{
    class DataConfig
    {
        public DataGetMode _Mode { get; set; }

        public DateTime _StartDate { get; set; }

        public DateTime _EndDate { get; set; }

        //用來提供設定資料
        public DataConfig()
        {

        }
    }
}
