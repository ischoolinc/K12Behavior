using FISCA;
using K12.Data;

namespace K12.Behavior
{
    public partial class SelectSchoolYear : WizardForm
    {
        public SelectSchoolYear()
        {
            InitializeComponent();
        }

        public SelectSchoolYear(ArgDictionary args)
            : base(args)
        {
            InitializeComponent();

            InitializeTitle();

            //學年度
            intSchoolYear.Value = int.Parse(School.DefaultSchoolYear);

            //學期
            intSemester.Value = int.Parse(School.DefaultSemester);
        }


        protected override void OnNextButtonClick()
        {
            Arguments.SetValue("學年度", intSchoolYear.Value);
            Arguments.SetValue("學期", intSemester.Value);
        }
    }
}
