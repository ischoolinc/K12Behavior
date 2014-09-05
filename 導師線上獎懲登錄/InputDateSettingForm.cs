using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using FISCA.LogAgent;
using K12.Data;
using FISCA.Presentation.Controls;
using DevComponents.DotNetBar.Controls;
namespace K12.Behavior.DisciplineInput
{
    public partial class InputDateSettingForm : FISCA.Presentation.Controls.BaseForm
    {
        private const string myDateTimeFormat = "yyyy/MM/dd HH:mm";
        ConfigObj conf;
        public InputDateSettingForm()
        {
            InitializeComponent();

            this.Text = "教師獎懲登錄設定";
            lblSemester.Text = string.Format("{0}學年度 {1}學期", School.DefaultSchoolYear, School.DefaultSemester);
            dgvTimes.AllowUserToAddRows = false;
            DataGridViewRow dgvr = new DataGridViewRow();
            conf = new ConfigObj();

            dgvr.CreateCells(dgvTimes);
            dgvr.Cells[0].Value = conf.InputStartTime.ToString(myDateTimeFormat);
            dgvr.Cells[1].Value = conf.InputEndTime.ToString(myDateTimeFormat);
            dgvTimes.Rows.Add(dgvr);
            cbMeritA.Checked = conf.AllowMeritA;
            cbMeritB.Checked = conf.AllowMeritB;
            cbMeritC.Checked = conf.AllowMeritC;
            cbDemeritA.Checked = conf.AllowDemeritA;
            cbDemeritB.Checked = conf.AllowDemeritB;
            cbDemeritC.Checked = conf.AllowDemeritC;
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            bool valid = true;
            foreach (DataGridViewRow each in dgvTimes.Rows)
            {
                if (!string.IsNullOrEmpty(each.ErrorText))
                {
                    valid = false;
                }
                foreach (DataGridViewCell eachCell in each.Cells)
                {
                    if (!string.IsNullOrEmpty(eachCell.ErrorText))
                    {
                        valid = false;
                    }
                }
                if (!valid) break;
            }
            if (!valid)
            {
                MsgBox.Show("畫面中含有不正確資料。");
                this.DialogResult = System.Windows.Forms.DialogResult.None;
                return;
            }
            //foreach (DataGridViewRow each in dgvTimes.Rows)
            //{
            DataGridViewRow each1 = dgvTimes.Rows[0];
            bool bools, boole;
            bools = DateTime.TryParse("" + each1.Cells[chStartTime.Index].Value, out conf.InputStartTime);
            boole = DateTime.TryParse("" + each1.Cells[chEndTime.Index].Value, out conf.InputEndTime);
            if (bools && boole)
            {
                conf.AllowMeritA = cbMeritA.Checked;
                conf.AllowMeritB = cbMeritB.Checked;
                conf.AllowMeritC = cbMeritC.Checked;
                conf.AllowDemeritA = cbDemeritA.Checked;
                conf.AllowDemeritB = cbDemeritB.Checked;
                conf.AllowDemeritC = cbDemeritC.Checked;
                if (conf.Save())
                    MsgBox.Show("儲存成功!!");
                else
                    MsgBox.Show("儲存失敗");
            }
            //}
            //ApplicationLog.Log("日常生活表現輸入時間", "修改", sb1.ToString() + sb2.ToString());
            DialogResult = DialogResult.OK;
        }
        private void dgvTimes_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            DataGridViewRow row = dgvTimes.Rows[e.RowIndex];
            string startTime = row.Cells[chStartTime.Index].Value + "";
            string endTime = row.Cells[chEndTime.Index].Value + "";

            row.ErrorText = "";
            if (string.IsNullOrEmpty(startTime) && string.IsNullOrEmpty(endTime))
            {
                //這裡沒有程式。
            }
            else if (!string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime))
            {
                DateTime? objStart = DateTimeHelper.Parse(startTime);
                DateTime? objEnd = DateTimeHelper.Parse(endTime);

                if (objStart.HasValue && objEnd.HasValue)
                {
                    if (objStart.Value >= objEnd.Value)
                        row.ErrorText = "截止時間必須在開始時間之後。";
                }
            }
            else
                row.ErrorText = "請輸入正確的時間限制資料(必需同時有資料或同時沒有資料)。";
        }
        private void dgvTimes_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int columnIndex = e.ColumnIndex;
            int rowIndex = e.RowIndex;
            string value = "" + dgvTimes.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            if (columnIndex == chStartTime.Index || columnIndex == chEndTime.Index)
            {
                DataGridViewCell cell = dgvTimes.Rows[rowIndex].Cells[columnIndex];
                cell.ErrorText = "";
                if (string.IsNullOrEmpty(value)) //沒有資料就不作任何檢查。
                    return;

                DateTime dt;
                if (!DateTime.TryParse(value, out dt))
                {
                    cell.ErrorText = "日期格式錯誤。";
                }
                else
                {
                    cell.Value = dt.ToString(myDateTimeFormat);
                }
            }
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void checkBoxX1_CheckedChanged(object sender, EventArgs e)
        {
            bool tmpChecked = checkBoxX1.Checked;
            cbMeritA.Checked = tmpChecked;
            cbMeritB.Checked = tmpChecked;
            cbMeritC.Checked = tmpChecked;
            cbDemeritA.Checked = tmpChecked;
            cbDemeritB.Checked = tmpChecked;
            cbDemeritC.Checked = tmpChecked;
        }
    }
        
}
