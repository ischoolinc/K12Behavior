using FISCA.Presentation.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace K12.Behavior.Setup
{
    public partial class SetupBehaviorDef : BaseForm
    {
        K12.Data.Configuration.ConfigData cd { get; set; }
        public SetupBehaviorDef()
        {
            InitializeComponent();

            cd = K12.Data.School.Configuration["名稱變更"];

            //此欄位可新增未來設定值
            //保留未來使用的彈性
            //by Dylan 2025/3/27
            List<string> NameList = new List<string>() { "留校察看" };

            foreach (string name in NameList)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridViewX1);
                row.Cells[0].Value = name;

                dataGridViewX1.Rows.Add(row);
            }


            foreach (string name in cd.ToList())
            {
                if (NameList.Contains(name))
                {
                    foreach (DataGridViewRow row in dataGridViewX1.Rows)
                    {
                        if (row.IsNewRow)
                            continue;

                        if ("" + row.Cells[0].Value == name)
                        {
                            row.Cells[1].Value = cd[name];
                        }
                    }
                }

            }
        }

        private void buttonX2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            cd = K12.Data.School.Configuration["名稱變更"];

            foreach (DataGridViewRow each in dataGridViewX1.Rows)
            {
                if (each.IsNewRow)
                    continue;

                cd["" + each.Cells[0].Value] = "" + each.Cells[1].Value;
            }

            cd.Save();

            MsgBox.Show("儲存成功!!");
            this.Close();
        }
    }
}
