using System.Data;
using ReportHelper;

namespace K12.獎懲統計表
{
    /// <summary>
    /// 班級獎懲統計
    /// </summary>
    public class TotalClassSatRecord
    {
        /// <summary>
        /// 建構式，傳入班級名稱
        /// </summary>
        /// <param name="ClassName"></param>
        public TotalClassSatRecord(string ClassName)
        {
            this.ClassName = ClassName;
            MeritA = 0;
            MeritB = 0;
            MeritC = 0;
            DemeritA = 0;
            DemeritB = 0;
            DemeritC = 0;
            MeritA人次 = 0;
            MeritB人次 = 0;
            MeritC人次 = 0;
            DemeritA人次 = 0;
            DemeritB人次 = 0;
            DemeritC人次 = 0;
            MeritA人數 = 0;
            MeritB人數 = 0;
            MeritC人數 = 0;
            DemeritA人數 = 0;
            DemeritB人數 = 0;
            DemeritC人數 = 0;
        }

        /// <summary>
        /// 加入獎懲記錄
        /// </summary>
        /// <param name="record"></param>
        public void Add(ClassSatRecord record)
        {
            MeritA += record.MeritA;
            MeritB += record.MeritB;
            MeritC += record.MeritC;
            DemeritA += record.DemeritA;
            DemeritB += record.DemeritB;
            DemeritC += record.DemeritC;
            MeritA人次 += record.MeritA人次;
            MeritB人次 += record.MeritB人次;
            MeritC人次 += record.MeritC人次;
            DemeritA人次 += record.DemeritA人次;
            DemeritB人次 += record.DemeritB人次;
            DemeritC人次 += record.DemeritC人次;
            MeritA人數 += record.MeritA人數;
            MeritB人數 += record.MeritB人數;
            MeritC人數 += record.MeritC人數;
            DemeritA人數 += record.DemeritA人數;
            DemeritB人數 += record.DemeritB人數;
            DemeritC人數 += record.DemeritC人數;
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
        public int MeritA人次 { get; private set; }

        /// <summary>
        /// 小過人次
        /// </summary>
        public int MeritB人次 { get; private set; }

        /// <summary>
        /// 警告人次
        /// </summary>
        public int MeritC人次 { get; private set; }

        /// <summary>
        /// 大過次數
        /// </summary>
        public int DemeritA人次 { get; private set; }

        /// <summary>
        /// 小過次數
        /// </summary>
        public int DemeritB人次 { get; private set; }

        /// <summary>
        /// 警告次數
        /// </summary>
        public int DemeritC人次 { get; private set; }

        /// <summary>
        /// 大功人數
        /// </summary>
        public int MeritA人數 { get; private set; }

        /// <summary>
        /// 小功人數
        /// </summary>
        public int MeritB人數 { get; private set; }

        /// <summary>
        /// 嘉獎人數
        /// </summary>
        public int MeritC人數 { get; private set; }

        /// <summary>
        /// 大過人數
        /// </summary>
        public int DemeritA人數 { get; private set; }

        /// <summary>
        /// 小過人數
        /// </summary>
        public int DemeritB人數 { get; private set; }

        /// <summary>
        /// 警告人數
        ///< /summary>
        public int DemeritC人數 { get; private set; }

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