using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using Framework;
using FISCA.DSAUtil;
using Framework.Feature;
using System.Xml;
using FISCA.LogAgent;
using Aspose.Cells;
using K12.Behavior.Feature;
using K12.Data;

namespace K12.Behavior.StuAdminExtendControls
{
    //匯入功能
    //缺曠節次 不得重覆

    public partial class PeriodConfigForm : BaseForm
    {

        private Dictionary<string, string> DicLogBefor = new Dictionary<string, string>();
        private Dictionary<string, string> DicLogAeft = new Dictionary<string, string>();

        //DataGridView更新檢查
        private ChangeListener DataListener { get; set; }
        private bool DataGridViewDataInChange = false;

        public PeriodConfigForm()
        {
            InitializeComponent();
        }

        private void PeriodConfigForm_Load(object sender, EventArgs e)
        {
            //資料更動檢查
            DataListener = new ChangeListener();
            DataListener.Add(new DataGridViewSource(dataGridView));
            DataListener.StatusChanged += new EventHandler<ChangeEventArgs>(DataListener_StatusChanged);

            //取得
            List<K12.Data.PeriodMappingInfo> List = K12.Data.PeriodMapping.SelectAll();

            List.Sort(SortByOrder); //排列順序

            foreach (K12.Data.PeriodMappingInfo info in List)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridView);

                row.Cells[0].Value = info.Name; //缺曠節次
                //2024/10/8 - 新增
                row.Cells[1].Value = info.CoursePeriod; //對應課程節次
                row.Cells[2].Value = info.Type; //類型
                row.Cells[3].Value = info.Sort.ToString(); //顯示順序
                row.Cells[4].Value = info.Aggregated; //統計權重
                dataGridView.Rows.Add(row);

                if (!DicLogBefor.ContainsKey(info.Name))
                {
                    DicLogBefor.Add(info.Name, "缺曠節次「" + info.Name + "」" + "對應課程節次「" + info.CoursePeriod + "」" + "類型「" + info.Type + "」" + "顯示順序「" + info.Sort.ToString() + "」" + "統計權重「" + info.Aggregated + "」");
                }
            }

            DataListener.Reset();
            DataListener.ResumeListen();
        }

        /// <summary>
        /// 儲存
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            DicLogAeft.Clear(); //Log

            DataListener.SuspendListen(); //終止變更判斷

            //資料檢查
            if (!ValidateRow())
            {
                FISCA.Presentation.Controls.MsgBox.Show("輸入資料有誤，請修正後再行儲存。", "內容錯誤", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            #region 資料收集

            List<PeriodMappingInfo> SaveList = new List<PeriodMappingInfo>();

            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.IsNewRow) continue;

                K12.Data.PeriodMappingInfo period = new Data.PeriodMappingInfo();
                period.Name = ("" + row.Cells[0].Value).Trim();
                period.CoursePeriod = ("" + row.Cells[1].Value).Trim();
                period.Type = ("" + row.Cells[2].Value).Trim();
                period.Sort = int.Parse(("" + row.Cells[3].Value).Trim());
                period.Aggregated = float.Parse(("" + row.Cells[4].Value).Trim());

                SaveList.Add(period);

                //Log
                string InsertLog = "缺曠節次「" + period.Name + "」";
                InsertLog += "對應課程節次「" + period.CoursePeriod + "」";
                InsertLog += "類型「" + period.Type + "」";
                InsertLog += "顯示順序「" + period.Sort + "」";
                InsertLog += "統計權重「" + period.Aggregated + "」";

                if (!DicLogAeft.ContainsKey("" + period.Name))
                {
                    DicLogAeft.Add("" + period.Name, InsertLog);
                }
            }
            #endregion

            #region 儲存
            string warningMsg = "說明:\n修改缺曠節次,建議使用資料合理性檢查\n〔學生缺曠資料與系統節次〕\n以確認系統內存資料的正確性！\n\n是否儲存變更?";
            if (FISCA.Presentation.Controls.MsgBox.Show(warningMsg, "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                K12.Data.PeriodMapping.Update(SaveList);
            }
            catch (Exception exception)
            {
                FISCA.Presentation.Controls.MsgBox.Show("更新失敗 :\n" + exception.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            #endregion

            #region Log
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("「每日節次」已被修改。");
            sb.AppendLine("修改前：");
            foreach (string each in DicLogBefor.Keys)
            {
                sb.AppendLine(DicLogBefor[each]);
            }
            sb.AppendLine("修改後：");
            foreach (string each in DicLogAeft.Keys)
            {
                sb.AppendLine(DicLogAeft[each]);
            }

            ApplicationLog.Log("每日節次管理", "修改", sb.ToString());
            #endregion

            DataGridViewDataInChange = false;
            FISCA.Presentation.Controls.MsgBox.Show("儲存成功!", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.Close();

        }

        /// <summary>
        /// 離開
        /// </summary>
        private void btnExit_Click(object sender, EventArgs e)
        {
            //if (DataGridViewDataInChange)
            //{
            //    DialogResult dr = FISCA.Presentation.Controls.MsgBox.Show("資料已變更,是否離開?", MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2);
            //    if (dr == DialogResult.Yes)
            //    {
            //        this.Close();
            //    }
            //}
            //else
            //{
            this.Close();
            //}
        }

        /// <summary>
        /// 輸入後,檢查是否重覆
        /// </summary>
        private void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView.Rows[e.RowIndex].IsNewRow)
                return;

            DataGridViewCell cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];

            if (e.ColumnIndex == 0) //缺曠節次(不可空白/不可重覆)
            {
                CheckNameRepeat(e.ColumnIndex, e.RowIndex);
            }
            else if (e.ColumnIndex == 1) //對應課程節次(不可重覆)
            {
                CheckNameRepeat(e.ColumnIndex, e.RowIndex);
            }
            else if (e.ColumnIndex == 2) //類型(不可空白)
            {
                if ("" + cell.Value == string.Empty)
                {
                    cell.ErrorText = "類型不得空白!"; //可重覆
                }
                else
                {
                    cell.ErrorText = ""; //可重覆
                }
            }
            else if (e.ColumnIndex == 3) //顯示順序(不可空白/不可重覆/必須是數字)
            {
                CheckNameRepeat(e.ColumnIndex, e.RowIndex);

                int CellInt;
                if (cell.ErrorText == "")
                {
                    if (!int.TryParse("" + cell.Value, out CellInt))
                    {
                        cell.ErrorText = "必須輸入數字!";
                    }
                    else
                    {
                        cell.ErrorText = "";
                    }
                }
            }
            else if (e.ColumnIndex == 4) //必須是數字
            {
                if ("" + cell.Value == string.Empty)
                {
                    cell.ErrorText = "統計權重不得空白!";
                }
                else
                {
                    double CellInt;
                    if (!double.TryParse("" + cell.Value, out CellInt))
                    {
                        cell.ErrorText = "必須輸入數字!";
                    }
                    else
                    {
                        cell.ErrorText = "";
                    }
                }
            }
        }

        /// <summary>
        /// 資料重覆檢查
        /// </summary>
        private void CheckNameRepeat(int ColumnIndex, int RowIndex)
        {
            string Name = "" + dataGridView.Rows[RowIndex].Cells[ColumnIndex].Value;
            DataGridViewRow row = dataGridView.Rows[RowIndex];

            List<string> list = new List<string>();

            foreach (DataGridViewRow TalRow in dataGridView.Rows)
            {
                if (TalRow.IsNewRow)
                    continue;

                foreach (DataGridViewCell cell in TalRow.Cells)
                {
                    if (cell.ColumnIndex == ColumnIndex) //同一行
                    {
                        if (cell.RowIndex != RowIndex) //不同列
                        {
                            list.Add("" + cell.Value);
                        }
                    }
                }
            }

            if (Name == string.Empty)
            {
                if (ColumnIndex == 0)
                {
                    row.Cells[ColumnIndex].ErrorText = "必須輸入內容!";
                }
            }
            else if (list.Contains(Name))
            {
                row.Cells[ColumnIndex].ErrorText = "資料重覆,請重新輸入!";
            }
            else
            {
                row.Cells[ColumnIndex].ErrorText = "";
            }
        }

        //資料檢查
        private bool ValidateRow()
        {
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.IsNewRow)
                    continue;

                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.ColumnIndex == 0 || cell.ColumnIndex == 2 || cell.ColumnIndex == 3)
                    {
                        if (cell.ErrorText != "")
                        {
                            return false;
                        }
                        else if ("" + cell.Value == "")
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        void DataListener_StatusChanged(object sender, ChangeEventArgs e)
        {
            DataGridViewDataInChange = true;
        }

        private static int SortByOrder(K12.Data.PeriodMappingInfo info1, K12.Data.PeriodMappingInfo info2)
        {
            return info1.Sort.CompareTo(info2.Sort);
        }

        /// <summary>
        /// BeginEdit
        /// </summary>
        private void dataGridView_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2 || e.ColumnIndex == 4 || e.ColumnIndex == 5)
            {
                dataGridView.ImeMode = ImeMode.OnHalf;
                dataGridView.ImeMode = ImeMode.Off;
            }

            if (dataGridView.SelectedCells.Count == 1)
                dataGridView.BeginEdit(true);
        }

        private void PeriodConfigForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DataGridViewDataInChange)
            {
                DialogResult dr = FISCA.Presentation.Controls.MsgBox.Show("資料已變更,是否離開?", MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2);
                if (dr == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        private void btnPrintOut_Click(object sender, EventArgs e)
        {
            #region 匯出
            if (saveFileDialog1.ShowDialog() != DialogResult.OK) return;

            DataGridViewExport export = new DataGridViewExport(dataGridView);
            export.Save(saveFileDialog1.FileName);

            if (new CompleteForm().ShowDialog() == DialogResult.Yes)
                System.Diagnostics.Process.Start(saveFileDialog1.FileName);
            #endregion
        }

        private void btnPrintIn_Click(object sender, EventArgs e)
        {

            #region 確認畫面
            DialogResult dr = FISCA.Presentation.Controls.MsgBox.Show("匯入每日節次設定值\n將完全覆蓋目前之資料狀態\n(建議可將原資料匯出備份)\n\n請確認繼續?", MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2);
            if (dr != DialogResult.Yes)
                return;


            Workbook wb = new Workbook();

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "選擇要匯入的每日節次設定值";
            ofd.Filter = "Excel檔案 (*.xlsx)|*.xlsx";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    wb.Open(ofd.FileName);
                }
                catch
                {
                    FISCA.Presentation.Controls.MsgBox.Show("指定路徑無法存取。", "開啟檔案失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
                return;

            //必要欄位
            List<string> requiredHeaders = new List<string>(new string[] { "缺曠節次", "對應課程節次", "類型", "顯示順序" });
            //欄位標題的索引
            Dictionary<string, int> headers = new Dictionary<string, int>();
            Worksheet ws = wb.Worksheets[0];
            for (int i = 0; i <= ws.Cells.MaxDataColumn; i++)
            {
                string header = ws.Cells[0, i].StringValue;
                if (requiredHeaders.Contains(header))
                    headers.Add(header, i);
            }

            //如果使用者匯入檔的欄位與必要欄位不符，則停止匯入
            if (headers.Count != requiredHeaders.Count)
            {
                StringBuilder builder = new StringBuilder(string.Empty);
                builder.AppendLine("匯入格式不符合。");
                builder.AppendLine("匯入資料標題必須包含：");
                builder.AppendLine(string.Join(",", requiredHeaders.ToArray()));
                FISCA.Presentation.Controls.MsgBox.Show(builder.ToString());
                return;
            }

            #endregion

            #region 匯入

            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("Periods");
            doc.AppendChild(root);

            #region 匯入重覆問題
            List<string> NameList1 = new List<string>();
            StringBuilder NameSb = new StringBuilder();
            for (int x = 1; x <= wb.Worksheets[0].Cells.MaxDataRow; x++) //每一Row
            {
                string name = ws.Cells[x, headers["缺曠節次"]].StringValue;

                if (string.IsNullOrEmpty(name.Trim())) //沒有缺曠名稱則跳過
                    continue;

                if (!NameList1.Contains(name.Trim()))
                {
                    NameList1.Add(name);
                }
                else
                {
                    NameSb.AppendLine("缺曠節次重覆:" + name);
                }

            }
            if (!string.IsNullOrEmpty(NameSb.ToString()))
            {
                FISCA.Presentation.Controls.MsgBox.Show("匯入每日節次發生錯誤:\n" + NameSb.ToString());
                return;
            }
            #endregion

            List<PeriodMappingInfo> SaveList = new List<PeriodMappingInfo>();
            StringBuilder sb_log = new StringBuilder();

            for (int x = 1; x <= wb.Worksheets[0].Cells.MaxDataRow; x++) //每一Row
            {
                string name = ws.Cells[x, headers["缺曠節次"]].StringValue;
                string course_period = ws.Cells[x, headers["對應課程節次"]].StringValue;
                string type = ws.Cells[x, headers["類型"]].StringValue;
                string sort = ws.Cells[x, headers["顯示順序"]].StringValue;
                //string aggregated = ws.Cells[x, headers["統計權重"]].StringValue;

                if (string.IsNullOrEmpty(name.Trim())) //沒有缺曠節次則跳過
                    continue;

                K12.Data.PeriodMappingInfo period = new Data.PeriodMappingInfo();
                period.Name = name.Trim();
                period.CoursePeriod = course_period.Trim();

                if (!string.IsNullOrEmpty(type.Trim()))
                {
                    period.Type = type.Trim();
                }
                else
                {
                    FISCA.Presentation.Controls.MsgBox.Show("匯入失敗,類型不可為空白!!\n此錯誤發生於節次名稱:[" + name + "]");
                    return;
                }

                if (CheckInt(sort.Trim()))
                {
                    period.Sort = int.Parse(sort.Trim());
                }
                else
                {
                    FISCA.Presentation.Controls.MsgBox.Show("匯入失敗,顯示順序必須是數字!!\n此錯誤發生於節次名稱:[" + name + "]");
                    return;
                }

                //統計權重目前沒有用到，預設為 1
                period.Aggregated = 1;

                SaveList.Add(period);
            }

            try
            {
                K12.Data.PeriodMapping.Update(SaveList);
            }
            catch (Exception exception)
            {
                FISCA.Presentation.Controls.MsgBox.Show("更新失敗 :\n" + exception.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }



            ApplicationLog.Log("每日節次管理", "匯入", "已將每日節次管理內容覆蓋匯入。");
            FISCA.Presentation.Controls.MsgBox.Show("每日節次管理,匯入成功!\n新設定將於畫面重新開啟時生效!", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DataGridViewDataInChange = false;
            this.Close();

            #endregion
        }

        private bool CheckInt(string j)
        {
            int HotKeyint;
            if (int.TryParse(j, out HotKeyint))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CheckDouble(string k)
        {
            Double HotKeyint;
            if (Double.TryParse(k, out HotKeyint))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
