using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace K12.Behavior.Keyboard
{
    class CellHelper
    {
        private DataGridViewCell _Cell;

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="row"></param>
        public CellHelper(DataGridViewCell cell)
        {
            _Cell = cell;
        }

        #region Is

        /// <summary>
        /// 比較Cell的ColumnName
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <returns></returns>
        public bool CompareColumnName(string ColumnName)
        {
            return (_Cell.OwningColumn.Name == ColumnName) ? true : false;
        }

        /// <summary>
        /// 比較Cell的值是否相等
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsCompare(string value)
        {
            return ("" + _Cell.Value == value) ? true : false;
        }

        /// <summary>
        /// 取得cell是否為空值
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return ("" + _Cell.Value == string.Empty) ? true : false;
        }

        /// <summary>
        /// 如果cell為空字串,就設定錯誤訊息
        /// </summary>
        public void IsEmptySetErrod()
        {
            if ("" + _Cell.Value == string.Empty)
            {
                _Cell.ErrorText = "請填入內容";
            }
            else
            {
                _Cell.ErrorText ="";
            }
        }

        #endregion

        #region Get

        /// <summary>
        /// 取得Cell的值
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public string GetValue()
        {
            return "" + _Cell.Value;
        }

        /// <summary>
        /// 取得Row的Tag
        /// </summary>
        /// <returns></returns>
        public Object GetRowTag()
        {
            return _Cell.OwningRow.Tag;
        }

        public int GetCellIndex()
        {
            return _Cell.OwningColumn.Index;
        }

        /// <summary>
        /// 取得Cell所在Row其他Cell的值
        /// </summary>
        /// <param name="index"></param>
        public string GetNumCellValue(int index)
        {
            return "" + _Cell.OwningRow.Cells[index].Value;
        }

        /// <summary>
        /// 取得Cell所在其他Cell的錯誤訊息
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetNumCellError(int index)
        {
            return _Cell.OwningRow.Cells[index].ErrorText;
        }

        /// <summary>
        /// 取得Row
        /// </summary>
        /// <returns></returns>
        public DataGridViewRow GetRow()
        {
            return _Cell.OwningRow;
        }

        #endregion

        #region Set

        /// <summary>
        /// 設定NowValue的值
        /// </summary>
        /// <param name="Value"></param>
        public void SetValue(string value)
        {
            _Cell.Value = value;
        }

        /// <summary>
        /// 設定Row的Tag
        /// </summary>
        /// <param name="Obj"></param>
        public void SetRowTag(Object Obj)
        {
            _Cell.OwningRow.Tag = Obj;
        }

        /// <summary>
        /// 設定Cell所在Row"指定欄位"的"背景色彩","文字顏色",是否"鎖定"
        /// </summary>
        /// <param name="locked"></param>
        public void SetCellStyle(string locked, Color ForeColor, bool Locked)
        {
            foreach (DataGridViewCell each in _Cell.OwningRow.Cells)
            {
                if (each.ColumnIndex > 4)
                    continue;

                if (locked.Contains(Char.Parse(each.OwningColumn.Index.ToString())))
                {
                    //each.Style.BackColor = BackColor;
                    each.Style.ForeColor = ForeColor;
                    each.ReadOnly = Locked;
                }
            }
        }

        /// <summary>
        /// 設定Cell所在Row其他Cell的值
        /// </summary>
        /// <param name="numCell"></param>
        /// <param name="value"></param>
        public void SetNumCellValue(int numCell, string value)
        {
            _Cell.OwningRow.Cells[numCell].Value = value;

        }

        /// <summary>
        /// 設定錯誤訊息
        /// </summary>
        /// <param name="ErrorText"></param>
        public void SetError(string ErrorText)
        {
            _Cell.ErrorText = ErrorText;
        }

        /// <summary>
        /// 設定Cell所在Row其他Cell的錯誤訊息
        /// </summary>
        /// <param name="num"></param>
        /// <param name="errorMessage"></param>
        public void SetNumError(int num,string errorMessage)
        {
            if (num <= 6 && num >= 0)
            {
                _Cell.OwningRow.Cells[num].ErrorText = errorMessage;
            }
        }

        /// <summary>
        /// 設定Cell的ReadOnly
        /// </summary>
        /// <param name="Only"></param>
        public void SetReadOnly(bool Only)
        {
            _Cell.ReadOnly = Only;
        }

        #endregion

        #region Clear

        /// <summary>
        /// 清空所有Cell的錯誤訊息
        /// </summary>
        public void ClearCellAllError()
        {
            DataGridViewRow NowRow = _Cell.OwningRow;
            foreach (DataGridViewCell each in NowRow.Cells)
            {
                each.ErrorText = "";
            }
        }

        /// <summary>
        /// 清空所有Cell的值
        /// </summary>
        public void ClearCellAllValue()
        {
            DataGridViewRow NowRow = _Cell.OwningRow;
            foreach (DataGridViewCell each in NowRow.Cells)
            {
                each.Value = "";
            }
        }

        /// <summary>
        /// 清空指定Cell的錯誤訊息
        /// </summary>
        /// <param name="num"></param>
        public void ClearNumCellError(string num)
        {
            DataGridViewRow NowRow = _Cell.OwningRow;
            foreach (DataGridViewCell each in NowRow.Cells)
            {
                if (each.ColumnIndex > 4)
                    continue;

                if (num.Contains(Char.Parse(each.OwningColumn.Index.ToString())))
                {
                    each.ErrorText = "";
                }
            }
        }

        /// <summary>
        /// 清空指定Cell的值(string小於9)
        /// </summary>
        /// <param name="ClearValue"></param>
        public void ClearNumCellValue(string ClearValue)
        {
            DataGridViewRow NowRow = _Cell.OwningRow;
            foreach (DataGridViewCell each in NowRow.Cells)
            {
                if (each.ColumnIndex > 4)
                    continue;

                if (ClearValue.Contains(Char.Parse(each.OwningColumn.Index.ToString())))
                {
                    each.Value = "";
                }
            }
        }

        /// <summary>
        /// 清空指定Cell(Int)的值
        /// </summary>
        /// <param name="ClearValue"></param>
        public void ClearIntNumCellValue(int ClearValue)
        {
            DataGridViewRow NowRow = _Cell.OwningRow;

            NowRow.Cells[ClearValue].Value = "";
        }

        #endregion

        /// <summary>
        /// 重設指定Cell的值(清空資料,清空錯誤)
        /// </summary>
        /// <param name="num"></param>
        public void SetupNumCell(string num)
        {
            DataGridViewRow NowRow = _Cell.OwningRow;
            foreach (DataGridViewCell each in NowRow.Cells)
            {
                if (each.ColumnIndex > 4)
                    continue;

                if (num.Contains(Char.Parse(each.OwningColumn.Index.ToString())))
                {
                    each.Value = "";
                    each.ErrorText = "";
                }
            }
        }
    }
}
