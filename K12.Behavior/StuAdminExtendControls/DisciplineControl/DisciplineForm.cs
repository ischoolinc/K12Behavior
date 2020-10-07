using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Aspose.Cells;
using FISCA.DSAUtil;
using Framework.Feature;
using FISCA.Presentation.Controls;
using FISCA.LogAgent;

namespace K12.Behavior.StuAdminExtendControls
{
    public partial class DisciplineForm : FISCA.Presentation.Controls.BaseForm
    {
        private int _currentComboxIndex = 2;

        private Dictionary<string, string> _origMerit = new Dictionary<string, string>();
        private Dictionary<string, string> _origDemerit = new Dictionary<string, string>();
        private Dictionary<string, string> _meritList = new Dictionary<string, string>();
        private Dictionary<string, string> _demeritList = new Dictionary<string, string>();

        private bool _meritIsSave = true;
        private bool _demeritIsSave = true;
        private bool _meritCanSave = true;
        private bool _demeritCanSave = true;

        private bool _isInititaled = false;

        public DisciplineForm()
        {
            InitializeComponent();
            InitialList();
        }

        private void InitialList()
        {
            List<string> cols = new List<string>() { "�ƥѥN�X" };
            Campus.Windows.DataGridViewImeDecorator dec = new Campus.Windows.DataGridViewImeDecorator(this.dataGridViewX1, cols);


            _isInititaled = false;

            DSResponse dsrsp = Config.GetDisciplineReasonList();
            foreach (XmlElement var in dsrsp.GetContent().GetElements("Reason"))
            {
                string type = var.GetAttribute("Type");
                string code = var.GetAttribute("Code");
                string desc = var.GetAttribute("Description");

                if (type == "���y")
                {
                    if (!_origMerit.ContainsKey(code))
                    {
                        _origMerit.Add(code, desc);
                        _meritList.Add(code, desc);
                    }
                }
                else if (type == "�g��" || type == "�g�|")
                {
                    if (!_origDemerit.ContainsKey(code))
                    {
                        _origDemerit.Add(code, desc);
                        _demeritList.Add(code, desc);
                    }
                }
            }

            comboBoxEx1.Items.Add("���y�ƥѥN�X��");
            comboBoxEx1.Items.Add("�g�٨ƥѥN�X��");

            comboBoxEx1.SelectedIndex = 0;
            //ValidateList();

            _isInititaled = true;
        }

        private void FillGrid(Dictionary<string, string> list)
        {
            dataGridViewX1.Rows.Clear();
            foreach (string code in list.Keys)
            {
                int index = dataGridViewX1.Rows.Add();
                DataGridViewRow row = dataGridViewX1.Rows[index];
                row.Cells[Code.Name].Value = code;
                row.Cells[Reason.Name].Value = list[code];
            }
        }

        private void SaveCurrentList(int index)
        {
            if (!_isInititaled)
                return;

            Dictionary<string, string> list = new Dictionary<string, string>();
            foreach (DataGridViewRow row in dataGridViewX1.Rows)
            {
                if (row.IsNewRow) continue;
                string code = "" + row.Cells[Code.Name].Value;
                string reason = (row.Cells[Reason.Name].Value != null) ? row.Cells[Reason.Name].Value.ToString() : "";

                if (!string.IsNullOrEmpty(code))
                {
                    if (!list.ContainsKey(code))
                        list.Add(code, reason);
                }
            }

            if (index == 0)
                _meritList = list;
            else
                _demeritList = list;
        }

        private void comboBoxEx1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isInititaled && _currentComboxIndex == comboBoxEx1.SelectedIndex) return;

            if (comboBoxEx1.SelectedIndex == 0)
            {
                SaveCurrentList(1);
                FillGrid(_meritList);
            }
            else if (comboBoxEx1.SelectedIndex == 1)
            {
                SaveCurrentList(0);
                FillGrid(_demeritList);
            }
            ValidateList();

            _currentComboxIndex = comboBoxEx1.SelectedIndex;
        }

        private bool ValidateList()
        {
            dataGridViewX1.EndEdit();
            bool valid = true;
            bool isSave = true;

            List<string> codeList = new List<string>();
            Dictionary<string, string> origList;

            if (comboBoxEx1.SelectedIndex == 0)
                origList = _origMerit;
            else
                origList = _origDemerit;

            foreach (DataGridViewRow row in dataGridViewX1.Rows)
            {
                if (row.IsNewRow) continue;
                if (row.Cells[Code.Name].Value == null)
                {
                    row.Cells[Code.Name].ErrorText = "���ର�ť�";
                    valid = false;
                    break;
                }
                else
                    row.Cells[Code.Name].ErrorText = "";

                string codeValue = row.Cells[Code.Name].Value.ToString();

                if (!codeList.Contains(codeValue))
                {
                    codeList.Add(codeValue);
                    row.Cells[Code.Name].ErrorText = "";
                }
                else
                {
                    row.Cells[Code.Name].ErrorText = "�W�٭���";
                    valid = false;
                    break;
                }

                //�ˬd��ƬO�_�ܰ�
                if (isSave)
                {
                    if (origList.ContainsKey(codeValue))
                    {
                        if (origList[codeValue] != ((row.Cells[Reason.Name].Value != null) ? row.Cells[Reason.Name].Value.ToString() : ""))
                            isSave = false;
                    }
                    else
                        isSave = false;
                }
            }

            if (isSave)
            {
                if ((dataGridViewX1.Rows.Count - 1) != origList.Keys.Count)
                    isSave = false;
            }

            if (comboBoxEx1.SelectedIndex == 0)
            {
                _meritIsSave = isSave;
                _meritCanSave = valid;
            }
            else
            {
                _demeritIsSave = isSave;
                _demeritCanSave = valid;
            }

            return valid;
        }

        private void dataGridViewX1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            ValidateList();
        }

        #region Log
        private StringBuilder _log_desc = new StringBuilder("");

        private void GenerateLogDescription()
        {
            StringBuilder merit = new StringBuilder("");
            StringBuilder demerit = new StringBuilder("");

            foreach (string code in _meritList.Keys)
                if (!_origMerit.ContainsKey(code))
                    merit.AppendLine("�s�W�u" + code + "�G" + _meritList[code] + "�v");
            foreach (string code in _origMerit.Keys)
                if (_meritList.ContainsKey(code) && _origMerit[code] != _meritList[code])
                    merit.AppendLine("�N�X�u" + code + "�v���ƥѥѡu" + _origMerit[code] + "�v�ܧ󬰡u" + _meritList[code] + "�v");
            foreach (string code in _origMerit.Keys)
                if (!_meritList.ContainsKey(code))
                    merit.AppendLine("�R���u" + code + "�G" + _origMerit[code] + "�v");

            foreach (string code in _demeritList.Keys)
                if (!_origDemerit.ContainsKey(code))
                    demerit.AppendLine("�s�W�u" + code + "�G" + _demeritList[code] + "�v");
            foreach (string code in _origDemerit.Keys)
                if (_demeritList.ContainsKey(code) && _origDemerit[code] != _demeritList[code])
                    demerit.AppendLine("�N�X�u" + code + "�v���ƥѥѡu" + _origDemerit[code] + "�v�ܧ󬰡u" + _demeritList[code] + "�v");
            foreach (string code in _origDemerit.Keys)
                if (!_demeritList.ContainsKey(code))
                    demerit.AppendLine("�R���u" + code + "�G" + _origDemerit[code] + "�v");

            if (!string.IsNullOrEmpty(merit.ToString()))
            {
                _log_desc.AppendLine("���y�ƥѪ�G");
                _log_desc.Append(merit.ToString());
            }
            if (!string.IsNullOrEmpty(demerit.ToString()))
            {
                if (!string.IsNullOrEmpty(_log_desc.ToString())) _log_desc.AppendLine("");
                _log_desc.AppendLine("�g�٨ƥѪ�G");
                _log_desc.Append(demerit.ToString());
            }
        }

        private void WriteLog()
        {
            if (!string.IsNullOrEmpty(_log_desc.ToString()))
            {
                //Log����
                //CurrentUser.Instance.AppLog.Write("�ק���g�ƥѪ�", _log_desc.ToString(), "���g�ƥѪ�", "");
            }
        }
        #endregion

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (_meritCanSave && _demeritCanSave)
            {
                XmlDocument doc = new XmlDocument();
                XmlElement content = doc.CreateElement("Content");

                SaveCurrentList(comboBoxEx1.SelectedIndex);
                GenerateLogDescription();

                _origMerit.Clear();
                _origDemerit.Clear();

                foreach (string code in _meritList.Keys)
                {
                    if (_demeritList.ContainsKey(code))
                    {
                        MsgBox.Show("�x�s����,�N�X:(" + code + ")�w����,�Э��s��J!");
                        return;
                    }

                    XmlElement reason = doc.CreateElement("Reason");
                    reason.SetAttribute("Code", code);
                    reason.SetAttribute("Description", _meritList[code]);
                    reason.SetAttribute("Type", "���y");
                    content.AppendChild(reason);
                    _origMerit.Add(code, _meritList[code]);
                }

                foreach (string code in _demeritList.Keys)
                {
                    if (_meritList.ContainsKey(code))
                    {
                        MsgBox.Show("�x�s����,�N�X:(" + code + ")�w����,�Э��s��J!");
                        return;
                    }

                    XmlElement reason = doc.CreateElement("Reason");
                    reason.SetAttribute("Code", code);
                    reason.SetAttribute("Description", _demeritList[code]);
                    reason.SetAttribute("Type", "�g��");
                    content.AppendChild(reason);
                    _origDemerit.Add(code, _demeritList[code]);
                }

                try
                {
                    Config.SetDisciplineReasonList(content);
                    WriteLog();
                    _meritIsSave = true;
                    _demeritIsSave = true;

                    FISCA.Presentation.Controls.MsgBox.Show("�x�s���\�C");
                    this.DialogResult = DialogResult.OK;
                }
                catch (Exception ex)
                {
                    //CurrentUser.ReportError(ex);
                    FISCA.Presentation.Controls.MsgBox.Show("�x�s���ѡC\n" + ex.Message);
                    return;
                }

                ApplicationLog.Log("�ǰȨt��.���g�ƥѥN�X��", "�ק���g�ƥѥN�X��", "�u���g�ƥѥN�X��v�w�Q�ק�C");
            }
            else
            {
                if (!_meritCanSave && !_demeritCanSave)
                    FISCA.Presentation.Controls.MsgBox.Show("��Ʀ��~�C");
                else if (!_meritCanSave)
                    FISCA.Presentation.Controls.MsgBox.Show("���y�ƥѪ��Ʀ��~�C");
                else if (!_demeritCanSave)
                    FISCA.Presentation.Controls.MsgBox.Show("�g�٨ƥѪ��Ʀ��~�C");
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            #region �ץX
            Workbook wb = new Workbook();
            wb.Worksheets.Clear();
            Worksheet ws = wb.Worksheets[wb.Worksheets.Add()];
            ws.Name = "���g�ƥѥN�X��";

            ws.Cells.CreateRange(0, 1, true).ColumnWidth = 10;
            ws.Cells.CreateRange(1, 1, true).ColumnWidth = 8;
            ws.Cells.CreateRange(2, 1, true).ColumnWidth = 40;

            ws.Cells[0, 0].PutValue("���g�ƥѥN�X");
            ws.Cells[0, 1].PutValue("���O");
            ws.Cells[0, 2].PutValue("�ƥ�");

            int rowIndex = 1;

            foreach (string code in _meritList.Keys)
            {
                ws.Cells[rowIndex, 0].PutValue(code);
                ws.Cells[rowIndex, 1].PutValue("���y");
                ws.Cells[rowIndex, 2].PutValue(_meritList[code]);
                rowIndex++;
            }

            foreach (string code in _demeritList.Keys)
            {
                ws.Cells[rowIndex, 0].PutValue(code);
                ws.Cells[rowIndex, 1].PutValue("�g��");
                ws.Cells[rowIndex, 2].PutValue(_demeritList[code]);
                rowIndex++;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "�t�s�s��";
            sfd.FileName = "���g�ƥѥN�X��.xls";
            sfd.Filter = "Excel�ɮ� (*.xls)|*.xls|�Ҧ��ɮ� (*.*)|*.*";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    wb.Save(sfd.FileName, FileFormatType.Excel2003);
                    FISCA.Presentation.Controls.MsgBox.Show("�ץX�����C");
                }
                catch
                {
                    FISCA.Presentation.Controls.MsgBox.Show("���w���|�L�k�s���C", "�t�s�ɮץ���", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                ApplicationLog.Log("�ǰȨt��.���g�ƥѥN�X��", "�ץX���g�ƥѥN�X", "�u�ɮv���y�N�X��v�w�Q�ץX�C");
            }
            #endregion
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            #region �פJ
            Workbook wb = new Workbook();
            Dictionary<string, string> importMeritList = new Dictionary<string, string>();
            Dictionary<string, string> importDemeritList = new Dictionary<string, string>();

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "��ܭn�פJ�����g�ƥѥN�X��";
            ofd.Filter = "Excel�ɮ� (*.xls)|*.xls";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    wb.Open(ofd.FileName);
                }
                catch
                {
                    FISCA.Presentation.Controls.MsgBox.Show("���w���|�L�k�s���C", "�}���ɮץ���", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
                return;

            List<string> requiredHeaders = new List<string>(new string[] { "���g�ƥѥN�X", "���O", "�ƥ�" });
            Dictionary<string, int> headerIndexes = new Dictionary<string, int>();
            Worksheet ws = wb.Worksheets[0];
            for (int i = 0; i <= ws.Cells.MaxDataColumn; i++)
            {
                string header = ws.Cells[0, i].StringValue;
                if (requiredHeaders.Contains(header))
                    headerIndexes.Add(header, i);
            }
            //if (wb.Worksheets[0].Cells[0, 0].StringValue != "�ƥѥN�X" ||
            //    wb.Worksheets[0].Cells[0, 1].StringValue != "���O" ||
            //    wb.Worksheets[0].Cells[0, 2].StringValue != "�ƥ�")
            if (headerIndexes.Count != requiredHeaders.Count)
            {
                StringBuilder builder = new StringBuilder("");
                builder.AppendLine("�פJ�榡���ŦX�C");
                builder.AppendLine("�פJ��Ƽ��D�����]�t�G");
                builder.AppendLine(string.Join(",", requiredHeaders.ToArray()));
                FISCA.Presentation.Controls.MsgBox.Show(builder.ToString());
                return;
            }

            ImportConfirm form = new ImportConfirm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                int rowIndex = 1;

                while (!string.IsNullOrEmpty(ws.Cells[rowIndex, 0].StringValue))
                {
                    string code = ws.Cells[rowIndex, headerIndexes["���g�ƥѥN�X"]].StringValue;
                    string type = ws.Cells[rowIndex, headerIndexes["���O"]].StringValue;
                    string reason = ws.Cells[rowIndex, headerIndexes["�ƥ�"]].StringValue;

                    rowIndex++;

                    if (!importMeritList.ContainsKey(code) && !importDemeritList.ContainsKey(code))
                    {
                        if (type == "���y")
                            importMeritList.Add(code, reason);
                        else if (type == "�g��" || type == "�g�|")
                            importDemeritList.Add(code, reason);
                    }
                    else
                    {
                        if (type == "���y")
                        {
                            importMeritList[code] = reason;
                            if (importDemeritList.ContainsKey(code))
                                importDemeritList.Remove(code);
                        }
                        else if (type == "�g��" || type == "�g�|")
                        {
                            importDemeritList[code] = reason;
                            if (importMeritList.ContainsKey(code))
                                importMeritList.Remove(code);
                        }
                    }
                }

                if (form.Overwrite)
                {
                    #region �s�W
                    _meritList = importMeritList;
                    _demeritList = importDemeritList;
                    if (comboBoxEx1.SelectedIndex == 0)
                        FillGrid(_meritList);
                    else
                        FillGrid(_demeritList);

                    ApplicationLog.Log("�ǰȨt��.���g�ƥѥN�X��", "�פJ���g�ƥѥN�X", "�u���g�ƥѥN�X��v�w�Q�פJ�÷s�W�C");
                    #endregion
                }
                else
                {
                    #region �л\
                    foreach (string code in importMeritList.Keys)
                    {
                        if (!_meritList.ContainsKey(code) && !_demeritList.ContainsKey(code))
                            _meritList.Add(code, importMeritList[code]);
                        else if (_meritList.ContainsKey(code) && !_demeritList.ContainsKey(code))
                            _meritList[code] = importMeritList[code];
                        else if (_demeritList.ContainsKey(code) && !_meritList.ContainsKey(code))
                        {
                            _demeritList.Remove(code);
                            _meritList.Add(code, importMeritList[code]);
                        }
                        else
                        {
                            _demeritList.Remove(code);
                            _meritList[code] = importMeritList[code];
                        }
                    }
                    foreach (string code in importDemeritList.Keys)
                    {
                        if (!_demeritList.ContainsKey(code) && !_meritList.ContainsKey(code))
                            _demeritList.Add(code, importDemeritList[code]);
                        else if (_demeritList.ContainsKey(code) && !_meritList.ContainsKey(code))
                            _demeritList[code] = importDemeritList[code];
                        else if (_meritList.ContainsKey(code) && !_demeritList.ContainsKey(code))
                        {
                            _meritList.Remove(code);
                            _demeritList.Add(code, importDemeritList[code]);
                        }
                        else
                        {
                            _meritList.Remove(code);
                            _demeritList[code] = importDemeritList[code];
                        }
                    }

                    if (comboBoxEx1.SelectedIndex == 0)
                        FillGrid(_meritList);
                    else
                        FillGrid(_demeritList);

                    ApplicationLog.Log("�ǰȨt��.���g�ƥѥN�X��", "�פJ���g�ƥѥN�X", "�u���g�ƥѥN�X��v�w�Q�פJ���л\�C");
                    #endregion
                }

                FISCA.Presentation.Controls.MsgBox.Show("�w�פJ����!\n���I���x�s�����}�C");
            }
            #endregion
        }

        private void dataGridViewX1_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            ValidateList();
        }

        private void dataGridViewX1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }

        private void DisciplineForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_demeritIsSave && !_meritIsSave)
            {
                if (FISCA.Presentation.Controls.MsgBox.Show("��Ʃ|���x�s�A�z�T�w�n���}�H", "", MessageBoxButtons.YesNo) == DialogResult.No)
                    e.Cancel = true;
            }
            else if (!_meritIsSave)
            {
                if (FISCA.Presentation.Controls.MsgBox.Show("���y�ƥѪ��Ʃ|���x�s�A�z�T�w�n���}�H", "", MessageBoxButtons.YesNo) == DialogResult.No)
                    e.Cancel = true;
            }
            else if (!_demeritIsSave)
            {
                if (FISCA.Presentation.Controls.MsgBox.Show("�g�٨ƥѪ��Ʃ|���x�s�A�z�T�w�n���}�H", "", MessageBoxButtons.YesNo) == DialogResult.No)
                    e.Cancel = true;
            }
        }

        private void �W�[�ƥѫe�m��ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ResonText rt = new ResonText("�妸�վ�ƥ� �e�m��");
            DialogResult dr = rt.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.Yes)
            {
                foreach (DataGridViewCell cell in dataGridViewX1.SelectedCells)
                {
                    if (cell.ColumnIndex == Reason.Index)
                    {
                        DataGridViewRow row = cell.OwningRow;
                        if (row.IsNewRow)
                            continue;

                        row.Cells[1].Value = "" + rt.Reson + row.Cells[1].Value;
                    }
                }
            }
        }

        private void �W�[�ƥѫ�m��ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResonText rt = new ResonText("�妸�վ�ƥ� ��m��");
            DialogResult dr = rt.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.Yes)
            {
                foreach (DataGridViewCell cell in dataGridViewX1.SelectedCells)
                {
                    if (cell.ColumnIndex == Reason.Index)
                    {
                        DataGridViewRow row = cell.OwningRow;
                        if (row.IsNewRow)
                            continue;

                        row.Cells[1].Value = "" + row.Cells[1].Value + rt.Reson;
                    }
                }
            }
        }

        private void �M���ƥѤ��ۦP��rToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResonText rt = new ResonText("�M���ƥѤ��ŦX���󤧤�r(�п�J����)");
            DialogResult dr = rt.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.Yes)
            {
                foreach (DataGridViewCell cell in dataGridViewX1.SelectedCells)
                {
                    if (cell.ColumnIndex == Reason.Index)
                    {
                        DataGridViewRow row = cell.OwningRow;
                        if (row.IsNewRow)
                            continue;

                        string name = "" + row.Cells[1].Value;
                        row.Cells[1].Value = name.Replace(rt.Reson, "");
                    }
                }
            }
        }
    }
}