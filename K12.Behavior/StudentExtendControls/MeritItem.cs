using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using DevComponents.DotNetBar;
using Framework;
using Framework.Security;
using FISCA.LogAgent;
using K12.Data;

namespace K12.Behavior.StudentExtendControls
{
    [FISCA.Permission.FeatureCode("K12.Student.MeritItem", "���y�O��")]
    internal partial class MeritItem : DetailContentBase
    {

        //public static string FeatureCode = (Attribute.GetCustomAttribute(typeof(MeritItem), typeof(FeatureCodeAttribute)) as FeatureCodeAttribute).FeatureCode;
        //private FeatureAce _permission;

        internal static FISCA.Permission.FeatureAce UserPermission;
        private List<MeritRecord> _records = new List<MeritRecord>();

        BackgroundWorker BGW = new BackgroundWorker();
        bool BkWBool = false;

        public MeritItem()
        {
            InitializeComponent();

            BGW.DoWork += new DoWorkEventHandler(BkW_DoWork);
            BGW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BkW_RunWorkerCompleted);

            Merit.AfterInsert += new EventHandler<K12.Data.DataChangedEventArgs>(Merit_Changed);
            Merit.AfterDelete += new EventHandler<K12.Data.DataChangedEventArgs>(Merit_Changed);
            Merit.AfterUpdate += new EventHandler<K12.Data.DataChangedEventArgs>(Merit_Changed);

            //�ȸѪk
            //Merit.Instance.ItemUpdated += new EventHandler<ItemUpdatedEventArgs>(Instance_ItemUpdated);

            //Merit.Instance.ItemUpdated += new EventHandler<ItemUpdatedEventArgs>(this.refreshUIData); //�� Cache Manager ������Ʀ��Q��s�ɴN���s��s�e���C

            this.Group = "���y�O��";    //�]�w�������ܪ� Title �A�p���]�w�|�� Exception�C

            UserPermission = FISCA.Permission.UserAcl.Current[FISCA.Permission.FeatureCodeAttribute.GetCode(GetType())];
            //UserPermission = User.Acl[FCode.GetCode(GetType())];

            btnInsert.Visible = UserPermission.Editable;
            btnUpdate.Visible = UserPermission.Editable;
            btnDelete.Visible = UserPermission.Editable;
            btnView.Visible = UserPermission.Viewable & !UserPermission.Editable;
        }

        //�ȸѪk
        //void Instance_ItemUpdated(object sender, ItemUpdatedEventArgs e)
        //{
        //    if (!BgW.IsBusy)
        //        BgW.RunWorkerAsync();
        //}

        void Merit_Changed(object sender, K12.Data.DataChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, K12.Data.DataChangedEventArgs>(Merit_Changed), sender, e);
            }
            else
            {
                if (this.PrimaryKey != "")
                {
                    this.Loading = true;

                    if (BGW.IsBusy)
                    {
                        BkWBool = true;
                    }
                    else
                    {
                        BGW.RunWorkerAsync();
                    }
                }
            }
        }

        /// <summary>
        /// ��D�e���˵����P�ǥͮɡA������δN�Q���w�s���ǥ�ID�A�N�|�I�s����k�C
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPrimaryKeyChanged(EventArgs e)
        {
            //this.Loading = true;    //�e���|�e�{��ƤU���������A(���@����)
            //Merit.Instance.SyncDataBackground(this.PrimaryKey);   //SyncDataBackground ��k��|Ĳ�o Demerit.Instance.ItemUpdateed �ƥ�

            this.Loading = true;

            if (BGW.IsBusy)
            {
                BkWBool = true;
            }
            else
            {
                BGW.RunWorkerAsync();
            }
        }

        void BkW_DoWork(object sender, DoWorkEventArgs e)
        {
            _records.Clear();
            _records = Merit.SelectByStudentIDs(new string[] { this.PrimaryKey });
        }

        void BkW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            if (BkWBool)
            {
                BkWBool = false;
                BGW.RunWorkerAsync();
                return;
            }

            refreshUIData();

            this.Loading = false;
        }

        private void refreshUIData()
        {
            if (!this.SaveButtonVisible && !this.CancelButtonVisible && this.PrimaryKey.Contains(PrimaryKey))
            {
                //�qCache Manager ���Ӿǥͪ��g�ٰO���A�ç�s��e���W�C
                this.listView.Items.Clear();

                _records.Sort(new Comparison<MeritRecord>(SchoolYearComparer));

                foreach (MeritRecord item in _records)
                {
                    #region ���


                    ListViewItem itm = new ListViewItem(item.SchoolYear.ToString());
                    itm.SubItems.Add(item.Semester.ToString());
                    itm.SubItems.Add(item.OccurDate.ToShortDateString());
                    itm.SubItems.Add(item.MeritA.ToString());
                    itm.SubItems.Add(item.MeritB.ToString());
                    itm.SubItems.Add(item.MeritC.ToString());
                    itm.SubItems.Add(item.Reason);
                    itm.SubItems.Add(item.RegisterDate.HasValue ? item.RegisterDate.Value.ToShortDateString() : "");
                    itm.SubItems.Add(item.Remark);

                    //�N��ƥ[�JListView
                    itm.Tag = item;
                    listView.Items.Add(itm);
                    #endregion

                }
                //this.Loading = false;
                this.CancelButtonVisible = false;
                this.SaveButtonVisible = false;
                this.ContentValidated = true;
            }

            this.Loading = false;   //�e���N�^�и�Ƥw�U���������e��
        }

        //private string chengDateTime(DateTime x)
        //{
        //    if (x == null)
        //        return "";
        //    string time = x.ToString();
        //    int y = time.IndexOf(' ');
        //    return time.Remove(y);
        //}

        private void btnInsert_Click(object sender, EventArgs e)
        {
            List<StudentRecord> studs = new List<StudentRecord>();
            studs.Add(Student.SelectByID(this.PrimaryKey));
            MeritEditForm editForm = new MeritEditForm(studs);  //���s����b�s�W�Ҧ��U���\�@����h��ǥͷs�W�ۦP���g�ٰO���A�ҥH Constructor �n�ǤJ�ǥͪ����X�C
            editForm.ShowDialog();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count == 0)
            {
                MsgBox.Show("�Х���ܤ@���z�n�ק諸���");
                return;
            }
            if (listView.SelectedItems.Count > 1)
            {
                MsgBox.Show("��ܸ�Ƶ��ƹL�h�A�@���u��ק�@�����");
                return;
            }

            MeritRecord record = (MeritRecord)this.listView.SelectedItems[0].Tag;
            MeritEditForm editForm = new MeritEditForm(record, UserPermission); 
            //���s����b�ק�Ҧ��U�A�@���u���@��ǥͪ��Y�@���g�ٰO���i��ק�A�ҥH Constructor �N�ǤJ�@�� Editor ����C
            editForm.ShowDialog();
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            btnUpdate_Click(sender, e);
            //if (listView.SelectedItems.Count == 0)
            //{
            //    MsgBox.Show("�Х���ܤ@���z�n�˵������");
            //    return;
            //}
            //if (listView.SelectedItems.Count > 1)
            //{
            //    MsgBox.Show("��ܸ�Ƶ��ƹL�h�A�@���u���˵��@�����");
            //    return;
            //}

            //MeritRecord record = (MeritRecord)this.listView.SelectedItems[0].Tag;
            //MeritEditForm editForm = new MeritEditForm(record, UserPermission); 
            ////���s����b�ק�Ҧ��U�A�@���u���@��ǥͪ��Y�@���g�ٰO���i��ק�A�ҥH Constructor �N�ǤJ�@�� Editor ����C
            //editForm.ShowDialog();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count == 0)
            {
                MsgBox.Show("������ܤ@���H�W���!!");
                return;
            }

            List<MeritRecord> MeritList = new List<MeritRecord>();

            if (MsgBox.Show("�T�w�N�R���ҿ�ܤ����y���?", "�T�{", MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            foreach (ListViewItem item in listView.SelectedItems)
            {
                MeritRecord record = item.Tag as MeritRecord;
                MeritList.Add(record);
            }     

            try
            {
                Merit.Delete(MeritList);
            }
            catch (Exception ex)
            {
                MsgBox.Show("�R�����y��ƥ���!" + ex.Message);
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("�ǥ͡u" + K12.Data.Student.SelectByID(this.PrimaryKey).Name + "�v");
            foreach (MeritRecord merit in MeritList)
            {
                sb.AppendLine("����u" + merit.OccurDate.ToShortDateString() + "�v");
            }
            sb.AppendLine("���y��Ƥw�Q�R���C");

            ApplicationLog.Log("�ǰȨt��.���y���", "�R���ǥͼ��y���", "student", this.PrimaryKey, sb.ToString());
            
            MsgBox.Show("�R�����y��Ʀ��\");
        }

        private int SchoolYearComparer(MeritRecord x, MeritRecord y)
        {
            return y.OccurDate.CompareTo(x.OccurDate);

            //string DataXX = x.OccurDate.ToShortDateString();



            //string xx = x.SchoolYear.ToString() + x.Semester.ToString();
            //string yy = y.SchoolYear.ToString() + y.Semester.ToString();



            //return xx.CompareTo(yy);
        }

        private void listView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView.SelectedItems.Count == 1)
            {
                MeritEditForm editor = new MeritEditForm(listView.SelectedItems[0].Tag as MeritRecord, UserPermission);
                editor.ShowDialog();
            }
        }
    }
}