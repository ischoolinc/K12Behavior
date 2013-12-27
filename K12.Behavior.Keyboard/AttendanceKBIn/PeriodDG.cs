using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K12.Behavior.Keyboard
{
    class PeriodDG
    {
        CustomDataGridView _DataGridPDG;
        Dictionary<string, int> _PeriodDic;
        Dictionary<string, string> _GetHotKetName;

        private bool nextRequired = false;
        public bool DontStop = true;

        public PeriodDG(CustomDataGridView datagrid, Dictionary<string, int> PeriodDic, Dictionary<string, string> GetHotKetName)
        {
            _DataGridPDG = datagrid; //DataGridView
            _GetHotKetName = GetHotKetName; //熱鍵/簡稱
            _PeriodDic = PeriodDic; //Column的名稱/位置

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
            if (_PeriodDic.ContainsValue(_DataGridPDG.CurrentCell.ColumnIndex)) //當輸入的內容是定義的Column時
            {
                nextRequired = true;
                _DataGridPDG.EndEdit();

                datagrid_CellValueChanged(null, null);
            }
        }

        //當輸入內容等於1的時候取代為abc(內容)
        void datagrid_CellFormatting(object sender, System.Windows.Forms.DataGridViewCellFormattingEventArgs e)
        {
            if (_PeriodDic.ContainsValue(e.ColumnIndex)) //如果是在指定的欄位
            {
                //e.Value = map.GetShortName(_DataGridPDG.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);

                if (_GetHotKetName.ContainsKey("" + _DataGridPDG.Rows[e.RowIndex].Cells[e.ColumnIndex].Value)) //如果是假別熱鍵
                {
                    e.Value = _GetHotKetName["" + _DataGridPDG.Rows[e.RowIndex].Cells[e.ColumnIndex].Value];
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
