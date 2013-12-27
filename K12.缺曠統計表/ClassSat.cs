using System;
using System.Collections.Generic;
using System.Data;
using K12.Data;
using ReportHelper;

namespace K12.缺曠統計表
{
    public class ClassSat
    {
        private Dictionary<string, ClassSatRecord> mRecords;
        private ClassSatRecord mTotalRecord;

        /// <summary>
        /// 建構式
        /// </summary>
        public ClassSat()
        {
            mRecords = new Dictionary<string, ClassSatRecord>();
            mTotalRecord = new ClassSatRecord("全校");
        }

        /// <summary>
        /// 缺曠假別
        /// </summary>
        public List<string> AbsenceTypes { get; private set; }

        /// <summary>
        /// 加入獎懲物件，以班級來統計
        /// </summary>
        /// <param name="ClassName"></param>
        /// <param name="vRecord"></param>
        public void Add(string ClassName, AttendanceRecord vRecord)
        {
            if (!mRecords.ContainsKey(ClassName))
            {
                ClassSatRecord vSatRecord = new ClassSatRecord(ClassName);
                mRecords.Add(ClassName, vSatRecord);
            }

            mRecords[ClassName].Add(vRecord);
            mTotalRecord.Add(vRecord);
        }

        public Dictionary<string,List<DataSet>> ToReportData(
            DateTime StartDate,
            DateTime EndDate,
            List<string> Absences)
        {
            Dictionary<string, List<DataSet>> result = new Dictionary<string, List<DataSet>>();

            DataSet PageHeader = new DataSet("PageHeader");

            PageHeader.Tables.Add(StartDate.ToShortDateString().ToDataTable("開始日期", "開始日期"));
            PageHeader.Tables.Add(EndDate.ToShortDateString().ToDataTable("結束日期", "結束日期"));
            PageHeader.Tables.Add(K12.Data.School.ChineseName.ToDataTable("學校名稱", "學校名稱"));

            result.Add("缺曠統計", new List<DataSet>());
            result["缺曠統計"].Add(PageHeader);

            foreach (ClassSatRecord record in mRecords.Values)
                result["缺曠統計"].Add(record.ToDataSet(Absences));

            result["缺曠統計"].Add(mTotalRecord.ToDataSet(Absences));

            return result;
        }
    }
}