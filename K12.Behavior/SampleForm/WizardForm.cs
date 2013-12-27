using System;
using FISCA;
using FISCA.Presentation.Controls;

namespace K12.Behavior
{
    public partial class WizardForm : BaseForm
    {
        //狀態Previous(上一步)Next(下一步)Cancel(結束)
        private ContinueDirection WizardResult { get; set; }

        //資料儲存
        protected ArgDictionary Arguments { get; set; }

        protected int curStep;

        protected int totalStep;

        public WizardForm()
        {
            InitializeComponent();
        }

        public WizardForm(ArgDictionary args)
        {
            //承接前一步之資訊流
            Arguments = args;

            InitializeComponent();

            InitializeTitle();

            if (totalStep == 1 || curStep == 1)
                btnPrevious.Visible = false;

            if (curStep == totalStep)
                btnNext.Text = "列印";
        }

        public void InitializeTitle()
        {
            curStep = Arguments.TryGetInteger("CurrentStep", 1); //目前頁數
            totalStep = Arguments.TryGetInteger("TotalStep", 1); //總頁數
            WizardFormTitle = Arguments.TryGetString("功能名稱", ""); //功能名稱
            WizardFormTitle += "(" + curStep + "/" + totalStep + ")"; //功能名稱(目前頁數/總頁數)
        }

        public ContinueDirection ShowWizardDialog()
        {
            WizardResult = ContinueDirection.Cancel;
            ShowDialog();
            return WizardResult;
        }

        protected string WizardFormTitle
        {
            get { return this.Text; }
            set { this.Text = value; }
        }

        //上一步的狀態
        protected bool PreviousButtonStatus
        {
            get { return btnPrevious.Enabled; }
            set { btnPrevious.Enabled = value; }
        }
        //文字內容
        protected string PreviousButtonTitle
        {
            get { return btnPrevious.Text; }
            set { btnPrevious.Text = value; }
        }
        //下一步之狀態
        protected bool NextButtonStatus
        {
            get { return btnNext.Enabled; }
            set { btnNext.Enabled = value; }
        }
        //文字內容
        protected string NextButtonTitle
        {
            get { return btnNext.Text; }
            set { btnNext.Text = value; }
        }
        /// <summary>
        /// 按了上一步
        /// </summary>
        protected virtual void OnPreviousButtonClick()
        {
        }
        /// <summary>
        /// 按了下一步
        /// </summary>
        protected virtual void OnNextButtonClick()
        {
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            //上一步
            WizardResult = ContinueDirection.Previous;
            //按了上一步
            OnPreviousButtonClick();
            //關閉畫面
            Close();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            //上一步
            WizardResult = ContinueDirection.Next;
            //按了上一步
            OnNextButtonClick();
            //關閉畫面
            Close();
        }

    }
}
