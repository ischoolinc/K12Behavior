using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K12.Behavior.Keyboard
{
    class PdPeriodDG2
    {
        Behavior.Keyboard.CustomDataGridView _DataGridPDG;
        List<int> _PeriodDic1;
        List<int> _PeriodDic2;

        private bool nextRequired = false;
        public bool DontStop = true;

        public PdPeriodDG2(Behavior.Keyboard.CustomDataGridView datagrid, List<int> PeriodDic1, List<int> PeriodDic2)
        {
            _DataGridPDG = datagrid; //DataGridView
            _PeriodDic1 = PeriodDic1; //獎勵的cell
            _PeriodDic2 = PeriodDic2; //懲戒的cell

            _DataGridPDG.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(datagrid_CellValueChanged);

            _DataGridPDG.CurrentCellDirtyStateChanged += new EventHandler(datagrid_CurrentCellDirtyStateChanged);
            //_DataGridPDG.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(_DataGridPDG_CellBeginEdit);

            _DataGridPDG.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(datagrid_CellFormatting);
        }

        void _DataGridPDG_CellBeginEdit(object sender, System.Windows.Forms.DataGridViewCellCancelEventArgs e)
        {
            //if (_PeriodDic.ContainsValue(e.ColumnIndex)) //當輸入的內容是定義的Column時
            //{
            //    nextRequired = true;
            //    _DataGridPDG.EndEdit();
            //}
        }

        //當Cell的狀態變更時,在Column是2的時候(範圍)
        void datagrid_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (_PeriodDic1.Contains(_DataGridPDG.CurrentCell.ColumnIndex)) //當輸入的內容是定義的Column時
            {
                //獎勵的動作
                nextRequired = true;
                _DataGridPDG.EndEdit();

                foreach (int each in _PeriodDic2)
                {
                    _DataGridPDG.CurrentRow.Cells[each].Value = "";
                }
                datagrid_CellValueChanged(null, null);
            }
            else if (_PeriodDic2.Contains(_DataGridPDG.CurrentCell.ColumnIndex))
            {
                //懲戒的動作
                nextRequired = true;
                _DataGridPDG.EndEdit();
                foreach (int each in _PeriodDic1)
                {
                    _DataGridPDG.CurrentRow.Cells[each].Value = "";
                }
                datagrid_CellValueChanged(null, null);
            }
        }

        //當輸入內容等於1的時候取代為abc(內容)
        void datagrid_CellFormatting(object sender, System.Windows.Forms.DataGridViewCellFormattingEventArgs e)
        {
            if (_PeriodDic1.Contains(e.ColumnIndex) || _PeriodDic2.Contains(e.ColumnIndex)) //如果是在指定的欄位
            {
                //e.Value = map.GetShortName(_DataGridPDG.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
                int MeritNum = 0;

                if (int.TryParse("" + _DataGridPDG.Rows[e.RowIndex].Cells[e.ColumnIndex].Value,out MeritNum))
                {
                    _DataGridPDG.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = MeritNum;
                }
                else
                {
                    _DataGridPDG.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "";
                }
            }
        }

        //(當Column是2的時候,nextRequired為true)
        void datagrid_CellValueChanged(object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
        {
            if (nextRequired)
            {
                _DataGridPDG.GoToNEXTCell();
                nextRequired = false;
            }
        }
    }
}
