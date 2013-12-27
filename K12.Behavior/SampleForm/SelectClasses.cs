using System.Collections.Generic;
using System.Windows.Forms;
using FISCA;
using K12.Data;

namespace K12.Behavior
{
    public partial class SelectClasses : WizardForm
    {
        public SelectClasses()
        {
            InitializeComponent();
        }

        public SelectClasses(ArgDictionary args)
            : base(args)
        {
            InitializeComponent();
            InitializeTitle();
            Arguments = args;

            List<ClassRecord> Classes = K12.Data.Class.SelectByIDs(Arguments.TryGetList<string>("所選班級"));
            Classes.Sort(SortClassByName);
            foreach (ClassRecord classRecord in Classes)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgvStudentList);
                row.Cells[Column1.Index].Value = classRecord.ID;
                row.Cells[Column2.Index].Value = classRecord.Name;
                row.Cells[Column3.Index].Value = classRecord.Students.Count.ToString();
                row.Cells[Column4.Index].Value = classRecord.Teacher != null ? classRecord.Teacher.Name : "";
                dgvStudentList.Rows.Add(row);
            }

        }

        protected override void OnNextButtonClick()
        {
            List<string> Classes = new List<string>();
            foreach (DataGridViewRow row in dgvStudentList.SelectedRows)
                Classes.Add("" + row.Cells[Column1.Index].Value);

            Arguments.SetValue("DataGridView所選班級", Classes);
        }

        /// <summary>
        /// 依班級名稱排序
        /// </summary>
        private int SortClassByName(ClassRecord cr1, ClassRecord cr2)
        {
            return cr1.Name.CompareTo(cr2.Name);
        }
    }
}
