using System;
using FISCA;

namespace K12.Behavior
{
    /// <summary>
    /// 選擇日期區間
    /// </summary>
    public partial class SelectDateRange : WizardForm
    {
        public SelectDateRange()
        {
            InitializeComponent();
        }

         public SelectDateRange(ArgDictionary args)
            : base(args)
        {
            InitializeComponent();
            InitializeTitle();
            StartDateTime = DateTime.Today;
            EndDateTime = DateTime.Today;
        }

        protected DateTime StartDateTime
        {
            get { return dtStartDateTime.Value; }
            set { dtStartDateTime.Value = value; }
        }

        protected DateTime EndDateTime
        {
            get { return dtEndDateTime.Value; }
            set { dtEndDateTime.Value = value; }
        }

        protected override void OnNextButtonClick()
        {
            Arguments.SetValue("開始日期", dtStartDateTime.Value);
            Arguments.SetValue("結束日期", dtEndDateTime.Value);
        }
    }
}
