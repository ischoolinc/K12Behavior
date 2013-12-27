using System;
using System.Collections.Generic;
using System.Windows.Forms;
using FISCA;
using K12.Data;

namespace K12.Behavior
{
    public partial class SelectStudents : WizardForm
    {
        public SelectStudents()
        {
            InitializeComponent();
        }

        public SelectStudents(ArgDictionary args)
            : base(args)
        {
            InitializeComponent();

            InitializeTitle();

            List<StudentRecord> students = K12.Data.Student.SelectByIDs(Arguments.TryGetList<string>("所選學生"));
            students.Sort(SortStudentBySeatNo);
            foreach (StudentRecord student in students)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgvStudentList);
                row.Cells[Column1.Index].Value = student.ID;
                row.Cells[Column2.Index].Value = student.Class != null ? student.Class.Name : "";
                row.Cells[Column3.Index].Value = student.SeatNo.HasValue ? student.SeatNo.Value.ToString() : "";
                row.Cells[Column4.Index].Value = student.StudentNumber;
                row.Cells[Column5.Index].Value = student.Name;
                row.Cells[Column6.Index].Value = student.StatusStr;
                dgvStudentList.Rows.Add(row);
            }
        }

        protected override void OnNextButtonClick()
        {
            List<string> students = new List<string>();
            foreach (DataGridViewRow row in dgvStudentList.SelectedRows)
                students.Add("" + row.Cells[Column1.Index].Value);

            Arguments.SetValue("DataGridView所選學生", students);
        }

        /// <summary>
        /// 依班級座號排序
        /// </summary>
        private int SortStudentBySeatNo(StudentRecord sr1, StudentRecord sr2)
        {
            string sr1ClassName = sr1.Class != null ? sr1.Class.Name : "";
            string sr2ClassName = sr2.Class != null ? sr2.Class.Name : "";
            sr1ClassName = sr1ClassName.PadLeft(10, '0');
            sr2ClassName = sr2ClassName.PadLeft(10, '0');

            int sr1SeatNo = sr1.SeatNo.HasValue ? sr1.SeatNo.Value : 0;
            int sr2SeatNo = sr2.SeatNo.HasValue ? sr2.SeatNo.Value : 0;
            string sr3SeatNo = sr1SeatNo.ToString().PadLeft(10, '0');
            string sr4SeatNo = sr2SeatNo.ToString().PadLeft(10, '0');

            sr1ClassName += sr3SeatNo;
            sr2ClassName += sr4SeatNo;

            return sr1ClassName.CompareTo(sr2ClassName);
        }

        private void RemoveStudents_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvStudentList.SelectedRows)
            {
                dgvStudentList.Rows.Remove(row);
            }
        }
    }
}
