using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace K12.Behavior.AttendanceEdit
{
    /// <summary>
    /// 
    /// </summary>
    public class DataGridViewSource : ChangeSource
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        public DataGridViewSource(DataGridView control)
        {
            Grid = control;
            OriginValues = new Dictionary<Point, string>();
            SubscribeControlEvents();
        }

        private void SubscribeControlEvents()
        {
            Grid.RowsAdded += new DataGridViewRowsAddedEventHandler(Grid_RowsAdded);
            Grid.RowsRemoved += new DataGridViewRowsRemovedEventHandler(Grid_RowsRemoved);
            Grid.CurrentCellDirtyStateChanged += new EventHandler(Grid_CurrentCellDirtyStateChanged);
            Grid.CellValueChanged += new DataGridViewCellEventHandler(Grid_CellValueChanged);
        }

        void Grid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            CompareValues();
        }

        private void Grid_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            CompareValues();
        }

        private void Grid_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            CompareValues();
        }

        private void Grid_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            CompareValues();
        }

        private void CompareValues()
        {
            if (Suspend) return;

            bool changed = false;
            foreach (DataGridViewRow row in Grid.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    //取得座標
                    Point location = new Point(cell.ColumnIndex, cell.RowIndex);
                    //originValue - 原本的值
                    //newValue - 新的值
                    string originValue = string.Empty, newValue = string.Empty;

                    //是否包含此座標,取得值
                    if (OriginValues.ContainsKey(location))
                        originValue = OriginValues[location];
                    //取得新值
                    newValue = cell.Value + "";
                    //新舊值是否相同
                    if (originValue != newValue)
                    {
                        changed = true; //已被修改
                        break;
                    }
                }

                if (changed) break;
            }

            if (changed)
                RaiseStatusChanged(ValueStatus.Dirty);
            else
                RaiseStatusChanged(ValueStatus.Clean);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Reset()
        {
            OriginValues = new Dictionary<Point, string>();

            foreach (DataGridViewRow row in Grid.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                    OriginValues.Add(new Point(cell.ColumnIndex, cell.RowIndex), cell.Value + "");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected Dictionary<Point, string> OriginValues { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected DataGridView Grid { get; set; }

    }
}
