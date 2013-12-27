using System.Windows.Forms;
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.Controls;

namespace K12.Behavior.StudentExtendControls
{
    //缺曠登錄(特有元件)
    public partial class PeriodControl : UserControl
    {
        public PeriodControl()
        {
            InitializeComponent();            

            //this.Font = DevComponents.DotNetBar.FontStyles.General;
            this.Width = 45;
        }

        public LabelX Label
        {
            get { return label; }
        }

        public TextBoxX TextBox
        {
            get { return textBox; }
        }
    }
}