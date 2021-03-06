﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using IFuncPlugin;
using SqlServerHelper;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace CodeTable
{
    public partial class CompanyCode : Form , IPlugin
    {
        private SqlHelper m_ObjSql = new SqlHelper();
        private List<SuperiorCodeTable> m_lstSuperiorCode = new List<SuperiorCodeTable>();
        private SqlConnectInfo m_DBInfo = new SqlConnectInfo();
        private int m_nAuthority = 0;
        private Dictionary<Guid, string> m_DirtyData = new Dictionary<Guid, string>();

        public CompanyCode()
        {
            InitializeComponent();
        }

        public MenuType GetMenuType()
        {
            return MenuType.eCompanyCode;
        }

        public string PluginName()
        {
            return "CompanyCode";
        }

        public Guid PluginGuid()
        {
            return new Guid("E37D8675-AB62-4804-A8BC-306ADEE68E58");
        }

        public string PluginMenu()
        {
            return "公司代码";
        }

        public void ShowPluginForm(Panel parent, SqlConnectInfo DbInfo)
        {
            m_DBInfo = DbInfo;
            //必须，否则不能作为子窗口显示
            this.TopLevel = false;
            this.Parent = parent;
            this.Show();
            this.BringToFront();
            if (m_nAuthority != GrobalVariable.CodeTable_Authority)
            {
                btnAdd.Enabled = false;
                btnDel.Enabled = false;
            }
        }

        public void SetAuthority(int nLoginUserId, int nAuthority)
        {
            m_nAuthority = nAuthority;
        }

        private void CompanyCode_Load(object sender, EventArgs e)
        {
            //显示所有上级单位代码
            if (!m_ObjSql.OpenSqlServerConnection(m_DBInfo.strServerName, m_DBInfo.strDbName, m_DBInfo.strUser, m_DBInfo.strUserPwd))
            {
                m_ObjSql = null;
                return;
            }
            ReadSuperiorCodeFromDb();
        }

        private void CompanyCode_FormClosed(object sender, FormClosedEventArgs e)
        {
            //关闭窗口时判断是否需要保存
            bool bSave = false;
            foreach (SuperiorCodeTable value in m_lstSuperiorCode)
            {
                if (value.eDbState != DbStateFlag.eDbOK)
                {
                    bSave = true;
                    break;
                }
            }
            if (bSave)
            {
                DialogResult result = MessageBox.Show("是否保存更改的数据？", "提示", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    SaveSuperiorCodeDataToDb();
                }
            }
            m_DirtyData.Clear();
            SuperiorView.Dispose();
            m_lstSuperiorCode.Clear();
            if (m_ObjSql != null)
            {
                m_ObjSql.CloseConnection();
                m_ObjSql = null;
            }
        }

        private void ReadSuperiorCodeFromDb()
        {
            SuperiorView.Rows.Clear();
            SqlDataReader dataReader = null;
            m_ObjSql.ExecuteCommand("select * from Data_Superior", out dataReader);
            if (dataReader != null)
            {
                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        int index = SuperiorView.Rows.Add();
                        SuperiorCodeTable SuperiorVal = new SuperiorCodeTable();
                        SuperiorVal.guidCode = Guid.NewGuid();
                        SuperiorVal.eDbState = DbStateFlag.eDbOK;
                        SuperiorVal.nDataGridViewRowIndex = index;
                        SuperiorVal.strSuperiorName = (string)dataReader["CompanyName"];
                        byte[] codeBcd = PublicFunc.StringToBCD((string)dataReader["CompanyCode"]);
                        Trace.Assert(codeBcd != null && codeBcd.Length == 2);
                        SuperiorVal.SuperiorCode[0] = codeBcd[0];
                        SuperiorVal.SuperiorCode[1] = codeBcd[1];
                        SuperiorView.Rows[index].Cells[0].Value = SuperiorVal.strSuperiorName;
                        SuperiorView.Rows[index].Cells[1].Value = BitConverter.ToString(codeBcd).Replace("-", "");
                        m_lstSuperiorCode.Add(SuperiorVal);
                    }
                }
                dataReader.Close();
            }
        }

        private int GetIndexOfList(int nRowIndex)
        {
            int nListIndex = -1;
            int nCount = 0;
            foreach (SuperiorCodeTable value in m_lstSuperiorCode)
            {
                if (value.nDataGridViewRowIndex == nRowIndex)
                {
                    nListIndex = nCount;
                    break;
                }
                nCount++;
            }
            return nListIndex;
        }

        private void SuperiorView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            if (SuperiorView.CurrentCell == null)
                return;
            string strInput = (string)SuperiorView.CurrentCell.FormattedValue;
            if (strInput == "")
                return;
            int nRowIndex = SuperiorView.CurrentCell.RowIndex;
            int nItem = SuperiorView.CurrentCell.ColumnIndex;
            if (nItem == 0)
            {
                Regex reg = new Regex(@"^[A-Za-z\u4e00-\u9fa5]+$");
                if (!reg.Match(strInput).Success)
                {
                    SuperiorView.CurrentCell.Value = "";
                    MessageBox.Show("公司名称不能包含数字和特殊字符");
                    return;
                }
                int nListIndex = GetIndexOfList(nRowIndex);
                if (nListIndex == -1)
                {
                    //新增
                    bool bAdd = true;
                    foreach (SuperiorCodeTable value in m_lstSuperiorCode)
                    {
                        if (value.strSuperiorName == strInput)
                        {
                            bAdd = false;
                            SuperiorView.CurrentCell.Value = "";
                            MessageBox.Show("公司名称已存在");
                            break;
                        }
                    }
                    if (bAdd)
                    {
                        SuperiorCodeTable newVal = new SuperiorCodeTable();
                        newVal.guidCode = Guid.NewGuid();
                        newVal.eDbState = DbStateFlag.eDbAdd;
                        newVal.nDataGridViewRowIndex = nRowIndex;
                        newVal.strSuperiorName = strInput;
                        newVal.SuperiorCode[0] = 0;
                        newVal.SuperiorCode[1] = 0;
                        m_lstSuperiorCode.Add(newVal);
                    }
                }
                else if (strInput != m_lstSuperiorCode[nListIndex].strSuperiorName)
                {
                    string strSuperiorName = m_lstSuperiorCode[nListIndex].strSuperiorName;
                    m_lstSuperiorCode[nListIndex].strSuperiorName = strInput;
                    if (m_lstSuperiorCode[nListIndex].eDbState == DbStateFlag.eDbOK)
                    {
                        m_lstSuperiorCode[nListIndex].eDbState = DbStateFlag.eDbDirty;
                        string strDirtyData = strSuperiorName + "|" + BitConverter.ToString(m_lstSuperiorCode[nListIndex].SuperiorCode).Replace("-", "");
                        if (m_DirtyData.ContainsKey(m_lstSuperiorCode[nListIndex].guidCode))
                            m_DirtyData[m_lstSuperiorCode[nListIndex].guidCode] = strDirtyData;
                        else
                            m_DirtyData.Add(m_lstSuperiorCode[nListIndex].guidCode, strDirtyData);
                    }
                }
            }
            else if (nItem == 1)
            {
                Regex reg = new Regex(@"^[0-9]+$");
                if (!reg.Match(strInput).Success)
                {
                    SuperiorView.CurrentCell.Value = "";
                    MessageBox.Show("公司代码只能是数字");
                    return;
                }
                byte[] codebyte = PublicFunc.StringToBCD(strInput);
                if (codebyte == null || codebyte.Length != 2)
                {
                    SuperiorView.CurrentCell.Value = "";
                    MessageBox.Show("公司代码无效");
                    return;
                }
                int nListIndex = GetIndexOfList(nRowIndex);
                if (nListIndex == -1)
                {
                    //新增
                    bool bAdd = true;
                    foreach (SuperiorCodeTable value in m_lstSuperiorCode)
                    {
                        if (value.SuperiorCode[0] == codebyte[0] && value.SuperiorCode[1] == codebyte[1])
                        {
                            bAdd = false;
                            SuperiorView.CurrentCell.Value = "";
                            MessageBox.Show("公司代码已存在");
                            break;
                        }
                    }
                    if (bAdd)
                    {
                        SuperiorCodeTable newVal = new SuperiorCodeTable();
                        newVal.guidCode = Guid.NewGuid();
                        newVal.eDbState = DbStateFlag.eDbAdd;
                        newVal.nDataGridViewRowIndex = nRowIndex;
                        newVal.strSuperiorName = "";
                        newVal.SuperiorCode[0] = codebyte[0];
                        newVal.SuperiorCode[1] = codebyte[1];
                        m_lstSuperiorCode.Add(newVal);
                    }
                }
                else if ((codebyte[0] != m_lstSuperiorCode[nListIndex].SuperiorCode[0]) || (codebyte[1] != m_lstSuperiorCode[nListIndex].SuperiorCode[1]))
                {
                    string strSuperiorCode = BitConverter.ToString(m_lstSuperiorCode[nListIndex].SuperiorCode).Replace("-", "");
                    m_lstSuperiorCode[nListIndex].SuperiorCode[0] = codebyte[0];
                    m_lstSuperiorCode[nListIndex].SuperiorCode[1] = codebyte[1];
                    if (m_lstSuperiorCode[nListIndex].eDbState == DbStateFlag.eDbOK)
                    {
                        m_lstSuperiorCode[nListIndex].eDbState = DbStateFlag.eDbDirty;
                        string strDirtyData = m_lstSuperiorCode[nListIndex].strSuperiorName + "|" + strSuperiorCode;
                        if (m_DirtyData.ContainsKey(m_lstSuperiorCode[nListIndex].guidCode))
                            m_DirtyData[m_lstSuperiorCode[nListIndex].guidCode] = strDirtyData;
                        else
                            m_DirtyData.Add(m_lstSuperiorCode[nListIndex].guidCode, strDirtyData);
                    }
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (SuperiorView.Rows.Count > m_lstSuperiorCode.Count || !CodeTable.IsSuperiorListCompleted(m_lstSuperiorCode))
            {
                MessageBox.Show("请先将空行填完整");
                return;
            }
            int nIndex = SuperiorView.Rows.Add();
            SuperiorView.Rows[nIndex].Cells[0].Selected = true;
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (SuperiorView.CurrentCell == null)
                return;
            int nRowIndex = SuperiorView.CurrentCell.RowIndex;
            int nListIndex = GetIndexOfList(nRowIndex);
            if (nListIndex != -1)
            {
                //未保存过的直接删除
                if (m_lstSuperiorCode[nListIndex].eDbState == DbStateFlag.eDbAdd)
                    m_lstSuperiorCode.RemoveAt(nListIndex);
                else
                    m_lstSuperiorCode[nListIndex].eDbState = DbStateFlag.eDbDelete;
            }
            SuperiorView.Rows.RemoveAt(nRowIndex);
            if (nRowIndex > 0)
                SuperiorView.Rows[nRowIndex - 1].Cells[0].Selected = true;
        }

        private void SaveSuperiorCodeDataToDb()
        {
            SqlParameter[] sqlparams = new SqlParameter[2];
            List<SuperiorCodeTable> deleteLst = new List<SuperiorCodeTable>();
            int nCount = m_lstSuperiorCode.Count;
            for (int i = 0; i < nCount; i++)
            {
                SuperiorCodeTable value = m_lstSuperiorCode[i];
                sqlparams[0] = m_ObjSql.MakeParam("Code", SqlDbType.VarChar, 4, ParameterDirection.Input, BitConverter.ToString(value.SuperiorCode).Replace("-", ""));
                sqlparams[1] = m_ObjSql.MakeParam("Name", SqlDbType.NVarChar, 50, ParameterDirection.Input, value.strSuperiorName);
                if (value.eDbState == DbStateFlag.eDbAdd)
                {
                    m_ObjSql.ExecuteCommand("insert into Data_Superior values(@Code,@Name)", sqlparams);
                    value.eDbState = DbStateFlag.eDbOK;
                }
                else if (value.eDbState == DbStateFlag.eDbDelete)
                {
                    m_ObjSql.ExecuteCommand("delete from Data_Superior where CompanyCode = @Code and CompanyName = @Name", sqlparams);
                    deleteLst.Add(value);
                }
                else if (value.eDbState == DbStateFlag.eDbDirty)
                {                    
                    SqlParameter[] sqlDirtyParams = new SqlParameter[2];
                    string strDirty = m_DirtyData[value.guidCode];
                    sqlDirtyParams[0] = m_ObjSql.MakeParam("DirtyCode", SqlDbType.VarChar, 4, ParameterDirection.Input, strDirty.Substring(strDirty.IndexOf("|") + 1));
                    sqlDirtyParams[1] = m_ObjSql.MakeParam("DirtyName", SqlDbType.NVarChar, 50, ParameterDirection.Input, strDirty.Substring(0, strDirty.IndexOf("|")));
                    m_ObjSql.ExecuteCommand("delete from Data_Superior where CompanyCode = @DirtyCode and CompanyName = @DirtyName", sqlDirtyParams);
                    m_ObjSql.ExecuteCommand("insert into Data_Superior values(@Code,@Name)", sqlparams);
                    value.eDbState = DbStateFlag.eDbOK;
                }
                m_lstSuperiorCode[i] = value;
            }

            foreach (SuperiorCodeTable temp in deleteLst)
            {
                m_lstSuperiorCode.Remove(temp);
            }
            deleteLst.Clear();
        }

        private void SuperiorView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (m_nAuthority != GrobalVariable.CodeTable_Authority)
                e.Cancel = true;
        }

    }
}
