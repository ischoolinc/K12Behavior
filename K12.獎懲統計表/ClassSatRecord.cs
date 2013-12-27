using System.Collections.Generic;
using System.Linq;
using ReportHelper;
using K12.Data;
using System.Data;

namespace K12.獎懲統計表
{
    /// <summary>
    /// 班級獎懲統計
    /// </summary>
    public class ClassSatRecord
    {
        private Dictionary<string, int> MeritA人次統計 = new Dictionary<string, int>();
        private Dictionary<string, int> MeritB人次統計 = new Dictionary<string, int>();
        private Dictionary<string, int> MeritC人次統計 = new Dictionary<string, int>();

        private Dictionary<string, int> DemeritA人次統計 = new Dictionary<string, int>();
        private Dictionary<string, int> DemeritB人次統計 = new Dictionary<string, int>();
        private Dictionary<string, int> DemeritC人次統計 = new Dictionary<string, int>();

        /// <summary>
        /// 建構式，傳入班級名稱
        /// </summary>
        /// <param name="ClassName"></param>
        public ClassSatRecord(string ClassName)
        {
            this.ClassName = ClassName;
            MeritA = 0;
            MeritB = 0;
            MeritC = 0;
            DemeritA = 0;
            DemeritB = 0;
            DemeritC = 0;
        }

        /// <summary>
        /// 加入獎懲記錄
        /// </summary>
        /// <param name="record"></param>
        public void Add(DisciplineRecord record)
        {
            //判斷是否銷過
            if (!string.IsNullOrEmpty(record.Cleared) &&
                record.Cleared.Equals("是"))
                return;

            if (record.MeritA.HasValue && record.MeritA.Value>0)
            {
                MeritA += record.MeritA.Value;
                if (!MeritA人次統計.ContainsKey(record.RefStudentID))
                    MeritA人次統計.Add(record.RefStudentID,0);
                MeritA人次統計[record.RefStudentID]++;
            }

            if (record.MeritB.HasValue && record.MeritB.Value>0)
            {
                MeritB += record.MeritB.Value;
                if (!MeritB人次統計.ContainsKey(record.RefStudentID))
                    MeritB人次統計.Add(record.RefStudentID, 0);
                MeritB人次統計[record.RefStudentID]++;
            }

            if (record.MeritC.HasValue && record.MeritC.Value>0)
            {
                MeritC += record.MeritC.Value;
                if (!MeritC人次統計.ContainsKey(record.RefStudentID))
                    MeritC人次統計.Add(record.RefStudentID, 0);
                MeritC人次統計[record.RefStudentID]++;
            }

            if (record.DemeritA.HasValue && record.DemeritA.Value>0)
            {
                DemeritA += record.DemeritA.Value;
                if (!DemeritA人次統計.ContainsKey(record.RefStudentID))
                    DemeritA人次統計.Add(record.RefStudentID, 0);
                DemeritA人次統計[record.RefStudentID]++;
            }

            if (record.DemeritB.HasValue && record.DemeritB.Value>0)
            {
                DemeritB += record.DemeritB.Value;
                if (!DemeritB人次統計.ContainsKey(record.RefStudentID))
                    DemeritB人次統計.Add(record.RefStudentID, 0);
                DemeritB人次統計[record.RefStudentID]++;
            }

            if (record.DemeritC.HasValue && record.DemeritC.Value>0)
            {
                DemeritC += record.DemeritC.HasValue ? record.DemeritC.Value : 0;
                if (!DemeritC人次統計.ContainsKey(record.RefStudentID))
                    DemeritC人次統計.Add(record.RefStudentID, 0);
                DemeritC人次統計[record.RefStudentID]++;
            }
        }

        /// <summary>
        /// 班級系統編號
        /// </summary>
        public string ClassName { get; private set; }

        /// <summary>
        /// 大功次數
        /// </summary>
        public int MeritA { get; private set; }

        /// <summary>
        /// 小功次數
        /// </summary>
        public int MeritB { get; private set; }

        /// <summary>
        /// 獎勵次數
        /// </summary>
        public int MeritC { get; private set; }

        /// <summary>
        /// 大過次數
        /// </summary>
        public int DemeritA { get; private set; }

        /// <summary>
        /// 小過次數
        /// </summary>
        public int DemeritB { get; private set; }

        /// <summary>
        /// 警告次數
        /// </summary>
        public int DemeritC { get; private set; }

        /// <summary>
        /// 大過人次
        /// </summary>
        public int MeritA人次 { get{ return MeritA人次統計.Values.Sum(); } }

        /// <summary>
        /// 小過人次
        /// </summary>
        public int MeritB人次 { get { return MeritB人次統計.Values.Sum(); } }

        /// <summary>
        /// 警告人次
        /// </summary>
        public int MeritC人次 { get { return MeritC人次統計.Values.Sum(); } }

        /// <summary>
        /// 大過次數
        /// </summary>
        public int DemeritA人次 { get { return DemeritA人次統計.Values.Sum(); } }

        /// <summary>
        /// 小過次數
        /// </summary>
        public int DemeritB人次 { get { return DemeritB人次統計.Values.Sum(); } }

        /// <summary>
        /// 警告次數
        /// </summary>
        public int DemeritC人次 { get { return DemeritC人次統計.Values.Sum(); } }

        /// <summary>
        /// 大功人數
        /// </summary>
        public int MeritA人數 { get { return MeritA人次統計.Count; } }

        /// <summary>
        /// 小功人數
        /// </summary>
        public int MeritB人數 { get { return MeritB人次統計.Count; } }

        /// <summary>
        /// 嘉獎人數
        /// </summary>
        public int MeritC人數 { get { return MeritC人次統計.Count; } }

        /// <summary>
        /// 大過人數
        /// </summary>
        public int DemeritA人數 { get { return DemeritA人次統計.Count; } }

        /// <summary>
        /// 小過人數
        /// </summary>
        public int DemeritB人數 { get { return DemeritB人次統計.Count; } }

        /// <summary>
        /// 警告人數
        ///< /summary>
        public int DemeritC人數 { get { return DemeritC人次統計.Count; } }

        public DataSet ToDataSet()
        {
            DataSet DataSet = new DataSet("DataSection");

            DataTable tblClassName = ClassName.ToDataTable("班級名稱", "班級名稱");

            DataSet.Tables.Add(tblClassName);

            DataTable tblSat = new DataTable("獎懲統計");
            tblSat.Columns.Add("大功");
            tblSat.Columns.Add("小功");
            tblSat.Columns.Add("嘉獎");
            tblSat.Columns.Add("大過");
            tblSat.Columns.Add("小過");
            tblSat.Columns.Add("警告");

            DataRow row = tblSat.NewRow();
            row.SetField<string>("大功", MeritA.ToStr());
            row.SetField<string>("小功", MeritB.ToStr());
            row.SetField<string>("嘉獎", MeritC.ToStr());

            row.SetField<string>("大過", DemeritA.ToStr());
            row.SetField<string>("小過", DemeritB.ToStr());
            row.SetField<string>("警告", DemeritC.ToStr());

            tblSat.Rows.Add(row);

            DataRow row人次 = tblSat.NewRow();
            row人次.SetField<string>("大功", MeritA人次.ToStr());
            row人次.SetField<string>("小功", MeritB人次.ToStr());
            row人次.SetField<string>("嘉獎", MeritC人次.ToStr());

            row人次.SetField<string>("大過", DemeritA人次.ToStr());
            row人次.SetField<string>("小過", DemeritB人次.ToStr());
            row人次.SetField<string>("警告", DemeritC人次.ToStr());

            tblSat.Rows.Add(row人次);

            DataRow row人數 = tblSat.NewRow();
            row人數.SetField<string>("大功", MeritA人數.ToStr());
            row人數.SetField<string>("小功", MeritB人數.ToStr());
            row人數.SetField<string>("嘉獎", MeritC人數.ToStr());

            row人數.SetField<string>("大過", DemeritA人數.ToStr());
            row人數.SetField<string>("小過", DemeritB人數.ToStr());
            row人數.SetField<string>("警告", DemeritC人數.ToStr());

            tblSat.Rows.Add(row人數);

            DataSet.Tables.Add(tblSat); 

            return DataSet;
        }
    }
}