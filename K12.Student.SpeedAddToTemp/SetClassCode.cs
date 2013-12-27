using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using K12.Data;
using DevComponents.DotNetBar.Controls;
using System.Xml;
using Aspose.Cells;

namespace K12.Student.SpeedAddToTemp
{
    public partial class SetClassCode : BaseForm
    {

        private string _Code { get; set; }

        private List<string> ClassNameList = new List<string>();

        private K12.Data.Configuration.ConfigData cd;

        public SetClassCode(string Code)
        {
            InitializeComponent();

            _Code = Code;
        }

        private void SetClassCode_Load(object sender, EventArgs e)
        {
            SetForm();
        }

        private void SetForm()
        {
            dataGridViewX1.Rows.Clear();

            //建立班級名稱清單(依年級/班級序號/班級名稱排序
            List<ClassRecord> classList = Class.SelectAll();
            classList = SortClassIndex.K12Data_ClassRecord(classList);
            foreach (ClassRecord each in classList)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridViewX1);
                row.Cells[0].Value = each.Name;
                row.Tag = each;
                dataGridViewX1.Rows.Add(row);

                if (!ClassNameList.Contains(each.Name))
                {
                    ClassNameList.Add(each.Name);
                }
            }

            //取得各班的代碼

            cd = School.Configuration[_Code];

            if (cd.Count != 0) //已設定
            {
                foreach (string each in cd)
                {
                    foreach (DataGridViewRow row in dataGridViewX1.Rows)
                    {
                        if (row.IsNewRow)
                            continue;

                        if ("" + row.Cells[0].Value == each)
                        {
                            row.Cells[1].Value = cd[each];
                        }
                    }
                }
            }
            else if (cd.Count == 0)
            {
                foreach (DataGridViewRow row in dataGridViewX1.Rows)
                {
                    if (row.IsNewRow)
                        continue;

                    row.Cells[1].Value = "" + row.Cells[0].Value;
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            School.Configuration.Remove(cd);
            cd = School.Configuration[_Code];

            foreach (DataGridViewRow row in dataGridViewX1.Rows)
            {
                if (row.IsNewRow)
                    continue;

                cd["" + row.Cells[0].Value] = "" + row.Cells[1].Value;

            }

            cd.Save();

            this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonX3_Click(object sender, EventArgs e)
        {
            cd = School.Configuration[_Code];
            School.Configuration.Remove(cd);
            MsgBox.Show("已刪除代碼!!");
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            #region 匯出

            if (saveFileDialog1.ShowDialog() != DialogResult.OK) return;

            DataGridViewExport export = new DataGridViewExport(dataGridViewX1);
            export.Save(saveFileDialog1.FileName);

            if (new CompleteForm().ShowDialog() == DialogResult.Yes)
                System.Diagnostics.Process.Start(saveFileDialog1.FileName);
            #endregion
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            #region 確認畫面
            DialogResult dr = FISCA.Presentation.Controls.MsgBox.Show("匯入班級名稱代碼表\n將完全覆蓋目前之資料狀態\n(建議可將原資料匯出備份)\n\n請確認繼續?", MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2);
            if (dr != DialogResult.Yes)
                return;

            Workbook wb = new Workbook();

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "選擇要匯入的班級名稱代碼表";
            ofd.Filter = "Excel檔案 (*.xls)|*.xls";
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
            List<string> requiredHeaders = new List<string>(new string[] { "班級名稱", "班級名稱代碼" });
            //欄位標題的索引
            Dictionary<string, int> headers = new Dictionary<string, int>();
            Worksheet ws = wb.Worksheets[0];
            for (int i = 0; i <= 1; i++)
            {
                string header = ws.Cells[0, i].StringValue;
                if (requiredHeaders.Contains(header))
                    headers.Add(header, i);
            }

            //如果使用者匯入檔的欄位與必要欄位不符，則停止匯入
            if (headers.Count != requiredHeaders.Count)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("匯入格式不符合。");
                builder.AppendLine("匯入資料標題必須包含：");
                builder.AppendLine(string.Join(",", requiredHeaders.ToArray()));
                FISCA.Presentation.Controls.MsgBox.Show(builder.ToString());
                return;
            }
            #endregion

            #region 匯入

            //檢查班級名稱是否正確
            for (int x = 1; x <= wb.Worksheets[0].Cells.MaxDataRow; x++) //每一Row
            {
                if (!string.IsNullOrEmpty(ws.Cells[x, headers["班級名稱"]].StringValue))
                {
                    if (!ClassNameList.Contains(ws.Cells[x, headers["班級名稱"]].StringValue))
                    {
                        FISCA.Presentation.Controls.MsgBox.Show("匯入資料內有不存在的班級名稱!!");
                        return;
                    }
                }
            }


            School.Configuration.Remove(cd);
            cd = School.Configuration[_Code];
            for (int x = 1; x <= wb.Worksheets[0].Cells.MaxDataRow; x++) //每一Row
            {
                cd[ws.Cells[x, headers["班級名稱"]].StringValue] = ws.Cells[x, headers["班級名稱代碼"]].StringValue;
            }

            //儲存
            try
            {
                cd.Save();
            }
            catch (Exception exception)
            {
                FISCA.Presentation.Controls.MsgBox.Show("更新失敗 :" + exception.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            FISCA.Presentation.Controls.MsgBox.Show("匯入成功!", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
            SetForm();
            #endregion
        }
    }
}
