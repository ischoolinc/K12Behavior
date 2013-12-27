using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using K12.Data;
using Framework.Feature;
using FISCA.DSAUtil;
using System.Xml;

namespace K12.Behavior.StudentExtendControls
{
    public partial class SpeedMeritDemeritInsert : BaseForm
    {
        BackgroundWorker BGW = new BackgroundWorker();

        public SpeedMeritDemeritInsert()
        {
            InitializeComponent();

            //用背景模式 - 去處理學生資料收集

        }

        private void SpeedMeritDemeritInsert_Load(object sender, EventArgs e)
        {
            intSchoolYear.Text = School.DefaultSchoolYear;
            intSemester.Text = School.DefaultSemester;
            dtOccurDate.Text = DateTime.Now.ToShortDateString();
            dtRegisterDate.Text = DateTime.Now.ToShortDateString();
            GetDisciplineReason(); //取得獎勵事由 - 下拉式清單內容

        }

        private void GetDisciplineReason()
        {
            DSResponse dsrsp = Config.GetDisciplineReasonList();
            foreach (XmlElement element in dsrsp.GetContent().GetElements("Reason"))
            {
                if (element.GetAttribute("Type") == "獎勵")
                {
                    string v = element.GetAttribute("Description");
                    cbReason.Items.Add(v);
                }
            }
        }

    }
}
