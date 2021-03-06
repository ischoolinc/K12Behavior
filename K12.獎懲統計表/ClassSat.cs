﻿using System;
using System.Collections.Generic;
using System.Data;
using K12.Data;
using ReportHelper;

namespace K12.獎懲統計表
{
    /// <summary>
    /// 班級獎懲統計
    /// </summary>
    public class ClassSat
    {
        private Dictionary<string, ClassSatRecord> mRecords;
        private ClassSatRecord mTotalRecord;
        private Dictionary<string, DisPlayOrder> ClassDisPlayOrderDic;
        /// <summary>
        /// 建構式
        /// </summary>
        public ClassSat()
        {
            mRecords = new Dictionary<string, ClassSatRecord>();
            mTotalRecord = new ClassSatRecord("全校");
        }

        /// <summary>
        /// 加入獎懲物件，以班級來統計
        /// </summary>
        /// <param name="ClassName"></param>
        /// <param name="vRecord"></param>
        public void Add(string ClassName, DisciplineRecord vRecord)
        {
            if (!mRecords.ContainsKey(ClassName))
            {
                ClassSatRecord vSatRecord = new ClassSatRecord(ClassName);
                mRecords.Add(ClassName, vSatRecord);
            }

            mRecords[ClassName].Add(vRecord);
            mTotalRecord.Add(vRecord);
        }

        private int SortClassOrder(string className1, string className2)
        {
            string name1 = "";
            string name2 = "";

            if (ClassDisPlayOrderDic.ContainsKey(className1))
            {
                if (!string.IsNullOrEmpty(ClassDisPlayOrderDic[className1].display_order))
                {
                    name1 = ClassDisPlayOrderDic[className1].grade_year.PadLeft(10, '0') + ClassDisPlayOrderDic[className1].display_order.PadLeft(10, '0') + className1;
                }
                else
                {
                    name1 = "龘龘龘龘龘龘龘龘龘龘龘" + className1;
                }
            }
            else
            {
                name1 = "龘龘龘龘龘龘龘龘龘龘龘" + className1;
            }

            if (ClassDisPlayOrderDic.ContainsKey(className2))
            {
                if (!string.IsNullOrEmpty(ClassDisPlayOrderDic[className2].display_order))
                {
                    name2 = ClassDisPlayOrderDic[className2].grade_year.PadLeft(10, '0') + ClassDisPlayOrderDic[className2].display_order.PadLeft(10, '0') + className2;
                }
                else
                {
                    name2 = "龘龘龘龘龘龘龘龘龘龘龘" + className2;
                }
            }
            else
            {
                name2 = "龘龘龘龘龘龘龘龘龘龘龘" + className2;
            }

            return name1.CompareTo(name2);
        }

        public Dictionary<string, List<DataSet>> ToReportData(DateTime StartDate, DateTime EndDate, Dictionary<string, DisPlayOrder> classDisPlayOrderDic)
        {
            ClassDisPlayOrderDic = classDisPlayOrderDic;
            List<string> sName = new List<string>();
            foreach (string each in mRecords.Keys)
            {
                sName.Add(each);
            }
            sName.Sort(SortClassOrder);

            Dictionary<string, List<DataSet>> result = new Dictionary<string, List<DataSet>>();

            DataSet PageHeader = new DataSet("PageHeader");

            PageHeader.Tables.Add(StartDate.ToShortDateString().ToDataTable("開始日期", "開始日期"));
            PageHeader.Tables.Add(EndDate.ToShortDateString().ToDataTable("結束日期", "結束日期"));
            PageHeader.Tables.Add(K12.Data.School.ChineseName.ToDataTable("學校名稱", "學校名稱"));

            result.Add("獎懲統計", new List<DataSet>());
            result["獎懲統計"].Add(PageHeader);

            foreach (string name in sName)
            {
                if (mRecords.ContainsKey(name))
                {
                    result["獎懲統計"].Add(mRecords[name].ToDataSet());

                }
            }

            //2017/1/10 - 調整為依字串排序
            //foreach (ClassSatRecord record in mRecords.Values)
            //    result["獎懲統計"].Add(record.ToDataSet());

            result["獎懲統計"].Add(mTotalRecord.ToDataSet());

            return result;
        }
    }
}