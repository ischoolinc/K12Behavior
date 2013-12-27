using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using System.Xml;
using Aspose.Words;
using FISCA.DSAUtil;
using System.IO;
using K12.Data;

namespace K12.Behavior.AttendanceConfirmation
{
    public partial class AttendanceListForm : BaseForm
    {
        private int _sizeIndex = 0; //預設為0
        private BackgroundWorker _BGWClassStudentAbsenceDetail; //背景模式
        private Dictionary<string, List<string>> Absence = new Dictionary<string, List<string>>();

        private Document _doc;
        private Run _run;
        private GetAbsenceDetail Data;
        private List<string> periodList;
        private Dictionary<string, int> ColumnIndex;

        private GetConfigData GetCD;

        public const string ConfigName = "班級缺曠明細確認表_Word";

        private double FontSize = 10;
        private string FontName = "標楷體";

        public AttendanceListForm()
        {
            InitializeComponent();

            string TimmeInput1 = DateTime.Now.AddMonths(-1).ToString("yyyy/MM/dd");
            dateTimeInput1.Text = TimmeInput1;

            string TimmeInput2 = DateTime.Now.ToString("yyyy/MM/dd");
            dateTimeInput2.Text = TimmeInput2;

            GetCD = new GetConfigData();


            //LoadPreference(); //驗證設定檔 及 設定內容
        }

        private void LoadPreference()
        {
            //GetCD = new GetConfigData();

            #region 讀取 Preference

            //Absence = new Dictionary<string, List<string>>();

            ////XmlElement config = CurrentUser.Instance.Preference["班級缺曠明細表"];
            //K12.Data.Configuration.ConfigData cd = K12.Data.School.Configuration[ConfigName];
            //XmlElement config = cd.GetXml("XmlData", null);

            //if (config != null)
            //{
            //    XmlElement print = (XmlElement)config.SelectSingleNode("Print");

            //    if (print != null)
            //    {
            //        if (print.HasAttribute("PaperSize"))
            //            _sizeIndex = int.Parse(print.GetAttribute("PaperSize"));
            //    }
            //    else
            //    {
            //        XmlElement newPrint = config.OwnerDocument.CreateElement("Print");
            //        newPrint.SetAttribute("PaperSize", "0");
            //        config.AppendChild(newPrint);
            //        //CurrentUser.Instance.Preference["班級缺曠明細表"] = config;
            //        cd.SetXml("XmlData", config);
            //    }

            //    XmlNodeList AttType = config.SelectNodes("Type");
            //    foreach (XmlNode each1 in AttType)
            //    {
            //        XmlElement XmlEach1 = (XmlElement)each1; //轉換

            //        if (!Absence.ContainsKey(XmlEach1.GetAttribute("Text"))) //建立盒子
            //        {
            //            Absence.Add(XmlEach1.GetAttribute("Text"), new List<string>());
            //        }

            //        foreach (XmlNode each2 in XmlEach1.SelectNodes("Absence")) //
            //        {
            //            XmlElement XmlEach2 = (XmlElement)each2; //轉換
            //            Absence[XmlEach1.GetAttribute("Text")].Add(XmlEach2.GetAttribute("Text"));
            //        }
            //    }
            //}
            //else
            //{
            //    #region 產生空白設定檔
            //    config = new XmlDocument().CreateElement("班級缺曠明細確認表_Word");
            //    XmlElement printSetup = config.OwnerDocument.CreateElement("Print");
            //    printSetup.SetAttribute("PaperSize", "0");
            //    config.AppendChild(printSetup);
            //    //CurrentUser.Instance.Preference["班級缺曠明細表"] = config;
            //    cd.SetXml("XmlData", config);
            //    #endregion
            //}

            //cd.Save();

            #endregion
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (K12.Presentation.NLDPanels.Class.SelectedSource.Count == 0)
            {
                return;
            }

            this.btnSave.Enabled = false;

            //LoadPreference();

            GetCD = new GetConfigData();
            GetCD.SaveAll();
            GetCD.Reset();


            FISCA.Presentation.MotherForm.SetStatusBarMessage("正在初始化班級學生缺曠明細(確認表)...");

            object[] args = new object[] { dateTimeInput1.Value, dateTimeInput2.Value, _sizeIndex, Absence, GetCD.ClassNoData }; //日期起始,結束,紙張大小,列印假別,是否列印無資料班級

            _BGWClassStudentAbsenceDetail = new BackgroundWorker();
            _BGWClassStudentAbsenceDetail.DoWork += new DoWorkEventHandler(_BGWClassStudentAbsenceDetail_DoWork);
            _BGWClassStudentAbsenceDetail.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_BGWClassStudentAbsenceDetail_RunWorkerCompleted);
            _BGWClassStudentAbsenceDetail.ProgressChanged += new ProgressChangedEventHandler(_BGWClassStudentAbsenceDetail_ProgressChanged);
            _BGWClassStudentAbsenceDetail.WorkerReportsProgress = true;
            _BGWClassStudentAbsenceDetail.RunWorkerAsync(args);
        }

        void _BGWClassStudentAbsenceDetail_DoWork(object sender, DoWorkEventArgs e)
        {
            //string reportName = "班級缺曠記錄明細";

            object[] args = e.Argument as object[];

            DateTime startDate = (DateTime)args[0];
            DateTime endDate = (DateTime)args[1];
            int size = (int)args[2];
            Dictionary<string, List<string>> absence = (Dictionary<string, List<string>>)args[3];
            string ClassNoData = (string)args[4];

            #region 快取資料

            //節次對照表
            periodList = new List<string>();

            //取得 Period List

            foreach (K12.Data.PeriodMappingInfo each in K12.Data.PeriodMapping.SelectAll())
            {
                if (!periodList.Contains(each.Name))
                    periodList.Add(each.Name);

            }

            //取得學生資料
            Data = new GetAbsenceDetail(startDate, endDate, periodList);

            #endregion

            Document template;

            if (GetCD.Setup_Mode == "false")
            {
                template = new Document(new MemoryStream(Properties.Resources.班級缺曠明細確認表範本));
            }
            else
            {
                if (GetCD.Setup_Temp == "")
                {
                    template = new Document(new MemoryStream(Properties.Resources.班級缺曠明細確認表範本));
                }
                else
                {
                    template = new Document(new MemoryStream(GetCD.Temp));
                }
            }

            //template.Sections[0].Body.Tables[0].Rows[2].Cells[0].Paragraphs[0].Runs[0].Font

            FontSize = template.Sections[0].Body.Tables[0].Rows[1].Cells[0].Paragraphs[0].Runs[0].Font.Size;
            FontName = template.Sections[0].Body.Tables[0].Rows[1].Cells[0].Paragraphs[0].Runs[0].Font.Name;

            _run = new Run(template);

            DocumentBuilder DB = new DocumentBuilder(template);

            ColumnIndex = new Dictionary<string, int>();
            ColumnIndex.Add("座號", 0);
            ColumnIndex.Add("姓名", 1);
            ColumnIndex.Add("日期", 2);

            DB.MoveToMergeField("缺曠假別");
            Cell AbsenceCell = DB.CurrentParagraph.ParentNode as Cell;
            CellSplit(AbsenceCell, periodList.Count);
            int x = 3;
            foreach (string periodEace in periodList)
            {
                ColumnIndex.Add(periodEace, x);
                Write(AbsenceCell, periodEace);
                AbsenceCell = GetMoveRightCell(AbsenceCell, 1);
                x++;
            }

            _doc = new Document();
            _doc.Sections.Clear();

            foreach (ClassRecord classInfo in Data.selectedClass)
            {
                #region 如果該班級的內容為0,就離開
                if (ClassNoData == "True")
                {
                    int PageContinue = 0;
                    foreach (string test1 in Data.allAbsenceDetail[classInfo.ID].Keys) //班級
                    {
                        foreach (string test2 in Data.allAbsenceDetail[classInfo.ID][test1].Keys) //學生
                        {
                            PageContinue += Data.allAbsenceDetail[classInfo.ID][test1][test2].Count;
                        }
                    }

                    if (PageContinue == 0)
                        continue;
                }
                #endregion

                Document resultdoc = template.Clone(true) as Document;

                _run = new Run(resultdoc);

                ////取得一個班級
                //Dictionary<string, Dictionary<string, Dictionary<string, string>>> classAbsenceDetail = Data.allAbsenceDetail[classInfo.ID];

                //if (classAbsenceDetail.Count <= 0)
                //    continue;

                //foreach (string student in classAbsenceDetail.Keys) //foreach學生
                //{
                //    DB.MoveToMergeField("資料");

                //    //取得一天
                //    Dictionary<string, Dictionary<string, string>> studentDetaiil = classAbsenceDetail[student];


                //    //取得Row
                //    //DB.CurrentParagraph.ParentNode.ParentNode
                //}


                #region MailMerge
                List<string> name = new List<string>();
                List<string> value = new List<string>();

                name.Add("班級");
                value.Add(classInfo.Name);

                name.Add("日期區間");
                value.Add(startDate.ToShortDateString() + "~" + endDate.ToShortDateString());

                name.Add("製表日期");
                value.Add(DateTime.Now.ToShortDateString());

                if (dateTimeInput3.Value != DateTime.MinValue)
                {
                    name.Add("繳回日期");
                    value.Add(HowManyWeek(dateTimeInput3.Value));
                }
                else
                {
                    name.Add("繳回日期");
                    value.Add("　　　年　　　月　　　日");
                }

                name.Add("Data");
                value.Add(classInfo.ID);

                resultdoc.MailMerge.MergeField += new Aspose.Words.Reporting.MergeFieldEventHandler(MailMerge_MergeField);
                resultdoc.MailMerge.Execute(name.ToArray(), value.ToArray());
                resultdoc.MailMerge.DeleteFields();
                resultdoc.MailMerge.MergeField -= new Aspose.Words.Reporting.MergeFieldEventHandler(MailMerge_MergeField);

                _doc.Sections.Add(_doc.ImportNode(resultdoc.FirstSection, true));
                #endregion

            }

        }

        public string HowManyWeek(DateTime OccurDate)
        {
            string stringDate = OccurDate.ToShortDateString();
            switch (OccurDate.DayOfWeek.ToString())
            {
                case "Monday":
                    stringDate += "(星期一)";
                    break;
                case "Tuesday":
                    stringDate += "(星期二)";
                    break;
                case "Wednesday":
                    stringDate += "(星期三)";
                    break;
                case "Thursday":
                    stringDate += "(星期四)";
                    break;
                case "Friday":
                    stringDate += "(星期五)";
                    break;
                case "Saturday":
                    stringDate += "(星期六)";
                    break;
                case "Sunday":
                    stringDate += "(星期日)";
                    break;
            }
            return stringDate;
        }

        void MailMerge_MergeField(object sender, Aspose.Words.Reporting.MergeFieldEventArgs e)
        {
            if (e.FieldName == "Data")
            {
                DocumentBuilder builder = new DocumentBuilder(e.Document);
                builder.MoveToField(e.Field, true);
                e.Field.Remove();

                //取得學生ID/日期/缺曠資料
                Dictionary<string, Dictionary<string, Dictionary<string, string>>> dic = Data.allAbsenceDetail[e.FieldValue.ToString()] as Dictionary<string, Dictionary<string, Dictionary<string, string>>>;

                List<StudentRecord> StudentSortList = new List<StudentRecord>();
                foreach (string each in dic.Keys) //取得學生ID
                {
                    StudentRecord sr = Data.studentInfoDict[each];
                    if (!StudentSortList.Contains(sr))
                    {
                        StudentSortList.Add(sr);
                    }
                }
                StudentSortList.Sort(new Comparison<StudentRecord>(StudentComparer));

                //插入於此Row之下
                Row refrow = builder.CurrentParagraph.ParentNode.ParentNode as Row;
                Cell SplieCell = GetMoveRightCell(refrow.Cells[0], 3);
                CellSplit(SplieCell, periodList.Count);

                //範本
                Row rowtemp = builder.CurrentParagraph.ParentNode.ParentNode.Clone(true) as Row;

                //此表格
                Table table = builder.CurrentParagraph.ParentNode.ParentNode.ParentNode as Table;

                foreach (StudentRecord each in StudentSortList) //取得學生ID
                {
                    StudentRecord sr = each;

                    #region 排一下日期

                    List<string> DatList = new List<string>();
                    foreach (string each2 in dic[sr.ID].Keys) //日期
                    {
                        DatList.Add(each2);
                    }

                    DatList.Sort(sortdat);

                    #endregion


                    foreach (string each2 in DatList) //日期
                    {
                        //如果該日期為0節資料,就不列印(因為被設定檔過慮掉)
                        if (dic[sr.ID][each2].Count == 0)
                            continue;

                        Write(refrow.Cells[ColumnIndex["座號"]], sr.SeatNo.HasValue ? sr.SeatNo.Value.ToString() : ""); //座號
                        Write(refrow.Cells[ColumnIndex["姓名"]], sr.Name);   //姓名
                        Write(refrow.Cells[ColumnIndex["日期"]], each2);   //日期

                        foreach (string each3 in dic[sr.ID][each2].Keys) //節次
                        {
                            if (ColumnIndex.ContainsKey(each3))
                            {
                                Write(refrow.Cells[ColumnIndex[each3]], dic[sr.ID][each2][each3]);
                            }

                        }
                        refrow = table.InsertAfter(rowtemp.Clone(true), refrow) as Row;
                    }
                }

                refrow.Remove();
            }
        }

        private int sortdat(string a, string b)
        {
            DateTime dt1 = DateTime.Now;
            string aa = a.Remove(a.IndexOf('('));
            DateTime.TryParse(aa, out dt1);

            DateTime dt2 = DateTime.Now;
            string bb = b.Remove(b.IndexOf('('));
            DateTime.TryParse(bb, out dt2);

            return dt1.CompareTo(dt2);
        }

        void _BGWClassStudentAbsenceDetail_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.btnSave.Enabled = true;
            FISCA.Presentation.MotherForm.SetStatusBarMessage("產生 班級缺曠記錄明細(確認表) 已完成");

            SaveFileDialog sd = new System.Windows.Forms.SaveFileDialog();
            sd.Title = "另存新檔";
            sd.FileName = "班級缺曠記錄明細(確認表).doc";
            sd.Filter = "Word檔案 (*.doc)|*.doc|所有檔案 (*.*)|*.*";
            if (sd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _doc.Save(sd.FileName);
                    System.Diagnostics.Process.Start(sd.FileName);

                }
                catch
                {
                    FISCA.Presentation.Controls.MsgBox.Show("指定路徑無法存取。", "建立檔案失敗", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    this.Enabled = true;
                    return;
                }
            }
        }

        void _BGWClassStudentAbsenceDetail_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        private void lblPrintSetup_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AttendanceSetup AS = new AttendanceSetup(GetCD);
            AS.ShowDialog();

            GetCD.Reset();

            //if (config.ShowDialog() == DialogResult.OK)
            //{
            //    LoadPreference();
            //}
        }

        private void lblAttendance_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SetupForm SF = new SetupForm(GetCD);
            SF.ShowDialog();

            GetCD.Reset();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public static int StudentComparer(StudentRecord x, StudentRecord y)
        {
            int xx = x.SeatNo.HasValue ? x.SeatNo.Value : 0;
            int yy = y.SeatNo.HasValue ? y.SeatNo.Value : 0;

            return xx.CompareTo(yy);
        }

        /// <summary>
        /// Cell切割器
        /// </summary>
        /// <param name="_cell">傳入分割的儲存格</param>
        /// <param name="Count">傳入分割數目</param>
        private void CellSplit(Cell _cell, int Count)
        {
            #region Cell切割器
            double MAXwidth = _cell.CellFormat.Width;
            double Cellwidth = MAXwidth / Count;

            List<Cell> list = new List<Cell>();
            list.Add(_cell);

            Row _row = _cell.ParentNode as Row;
            for (int x = 0; x < Count - 1; x++)
            {
                list.Add((_row.InsertAfter(new Cell(_cell.Document), _cell)) as Cell);
            }

            foreach (Cell each in list)
            {
                each.CellFormat.Width = Cellwidth;
            }
            #endregion
        }

        #region 阿寶友情贊助

        /// <summary>
        /// 以Cell為基準,向下移一格
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private Cell GetMoveDownCell(Cell cell, int count)
        {
            #region 以Cell為基準,向下移一格
            if (count == 0) return cell;

            Row row = cell.ParentRow;
            int col_index = row.IndexOf(cell);
            Table table = row.ParentTable;
            int row_index = table.Rows.IndexOf(row) + count;

            try
            {
                return table.Rows[row_index].Cells[col_index];
            }
            catch (Exception ex)
            {
                return null;
            }
            #endregion
        }

        /// <summary>
        /// 以Cell為基準,向右移一格
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private Cell GetMoveRightCell(Cell cell, int count)
        {
            #region 以Cell為基準,向右移一格
            if (count == 0) return cell;

            Row row = cell.ParentRow;
            int col_index = row.IndexOf(cell);
            Table table = row.ParentTable;
            int row_index = table.Rows.IndexOf(row);

            try
            {
                return table.Rows[row_index].Cells[col_index + count];
            }
            catch (Exception ex)
            {
                return null;
            }
            #endregion
        }

        /// <summary>
        /// 以Cell為基準,使用NextSibling向右移一格
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private Cell GetMoveRightCellByNextSibling(Cell cell, int count)
        {
            #region 以Cell為基準,使用NextSibling向右移一格
            if (count == 0) return cell;

            Node node = cell;
            for (int i = 0; i < count; i++)
                node = node.NextSibling;

            try
            {
                return (Cell)node;
            }
            catch (Exception ex)
            {
                return null;
            }
            #endregion
        }

        /// <summary>
        /// 寫入資料
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="text"></param>
        private void Write(Cell cell, string text)
        {
            #region 寫入資料
            if (cell.FirstParagraph == null)
                cell.Paragraphs.Add(new Paragraph(cell.Document));
            cell.FirstParagraph.Runs.Clear();
            _run.Text = text;
            _run.Font.Size = FontSize;
            _run.Font.Name = FontName;
            cell.FirstParagraph.Runs.Add(_run.Clone(true));
            #endregion
        }

        #endregion
    }
}
