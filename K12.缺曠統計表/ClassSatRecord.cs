using System.Collections.Generic;
using System.Data;
using System.Linq;
using K12.Data;
using ReportHelper;

namespace K12.缺曠統計表
{
    /// <summary>
    /// 缺曠統計記錄物件
    /// </summary>
    public class ClassSatRecord
    {
        /// <summary>
        /// 建構式，傳入班級名稱
        /// </summary>
        /// <param name="ClassName"></param>
        public ClassSatRecord(string ClassName)
        {
            this.ClassName = ClassName;
            this.次數 = new Dictionary<string, int>();
            this.人次 = new Dictionary<string, Dictionary<string, int>>();
        }

        /// <summary>
        /// 加入缺曠記錄
        /// </summary>
        /// <param name="record"></param>
        public void Add(AttendanceRecord record)
        {
            foreach (AttendancePeriod period in record.PeriodDetail)
            {
                if (!次數.ContainsKey(period.AbsenceType))
                    次數.Add(period.AbsenceType, 0);
                次數[period.AbsenceType]++;
            }

            List<string> AbsenceTypes = record
                .PeriodDetail
                .Select(x=>x.AbsenceType)
                .Distinct()
                .ToList();

            foreach(string AbsenceType in AbsenceTypes)
            {
                if (!人次.ContainsKey(AbsenceType))
                    人次.Add(AbsenceType ,new Dictionary<string,int>());

                if (!人次[AbsenceType].ContainsKey(record.RefStudentID))
                    人次[AbsenceType].Add(record.RefStudentID,0);

                人次[AbsenceType][record.RefStudentID]++;
            }
        }

        /// <summary>
        /// 班級名稱
        /// </summary>
        public string ClassName { get; private set; }

        /// <summary>
        /// 缺曠次數
        /// </summary>
        public Dictionary<string, int> 次數 { get; private set; }

        /// <summary>
        /// 缺曠人次及人數
        /// </summary>
        public Dictionary<string, Dictionary<string, int>> 人次 { get; private set; }

        public DataSet ToDataSet(List<string> Absences)
        {
            DataSet DataSet = new DataSet("DataSection");

            DataTable tblClassName = ClassName.ToDataTable("班級名稱", "班級名稱");

            DataSet.Tables.Add(tblClassName);

            DataTable tblSat = new DataTable("缺曠統計");

            foreach (string Absence in Absences)
                tblSat.Columns.Add(Absence);
    
            DataRow row = tblSat.NewRow();

            foreach (string Absence in Absences)
                if (次數.ContainsKey(Absence))
                    row.SetField<string>(Absence, ""+次數[Absence]);
            tblSat.Rows.Add(row);

            DataRow row人次 = tblSat.NewRow();

            foreach (string Absence in Absences)
                if (人次.ContainsKey(Absence))
                    row人次.SetField<string>(Absence, "" + 人次[Absence].Values.Sum());
            tblSat.Rows.Add(row人次);

            DataRow row人數 = tblSat.NewRow();

            foreach (string Absence in Absences)
                if (人次.ContainsKey(Absence))
                    row人數.SetField<string>(Absence, "" + 人次[Absence].Count);

            tblSat.Rows.Add(row人數);

            DataSet.Tables.Add(tblSat);

            return DataSet;
        }
    }
}