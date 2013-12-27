using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;

namespace K12.Behavior.Keyboard
{
    public partial class ClassInsertForm : BaseForm
    {
        DataGridViewRow Row = new DataGridViewRow();

        public ClassInsertForm()
        {
            InitializeComponent();
        }

        private void ClassInsertForm_Load(object sender, EventArgs e)
        {
            //int addRowIndex = dgvBaseView.Rows.Add();

            //dgvBaseView.Rows[addRowIndex].Cells[0].Value = "" + Row.Cells[0].Value;



        }
    }
}
