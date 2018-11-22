using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using FISCA.Data;

namespace K12.Behavior.BatchClearDemerit
{
    public partial class frmSelectReason : BaseForm
    {
        public string _reason = "";

        public frmSelectReason()
        {
            InitializeComponent();
        }

        private void frmSelectReason_Load(object sender, EventArgs e)
        {
            #region SQL
            string sql = @"
WITH data_row AS(
	SELECT 
		CASE WHEN merit_flag = 0 THEN (unnest(xpath('/Discipline/Demerit/@ClearDate', xmlparse(content detail)))::text) ELSE null END as clear_date 
		, CASE WHEN merit_flag = 0 THEN unnest(xpath('/Discipline/Demerit/@ClearReason', xmlparse(content detail)))::text ELSE null END as clear_reason 
	FROM 
		discipline 
)
SELECT
	clear_reason
	, count(clear_reason)
FROM
	data_row
WHERE
	clear_reason IS NOT NULL
	AND clear_reason <> ''
GROUP BY 
	clear_reason
ORDER BY count DESC
LIMIT 10"; 
            #endregion

            QueryHelper qh = new QueryHelper();

            try
            {
                DataTable dt = qh.Select(sql);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        DataGridViewRow dgvrow = new DataGridViewRow();
                        dgvrow.CreateCells(dataGridViewX1);

                        dgvrow.Cells[0].Value = "" + row["clear_reason"];
                        dgvrow.Cells[0].ToolTipText = "點擊兩下選取事由";


                        dataGridViewX1.Rows.Add(dgvrow);
                    }
                }
            }
            catch(Exception ex)
            {
                MsgBox.Show(ex.Message);
            }

        }

        private void dataGridViewX1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                this._reason = "" + dataGridViewX1.Rows[e.RowIndex].Cells[0].Value;

                this.DialogResult = DialogResult.Yes;
                this.Close();
            }
        }
    }
}
