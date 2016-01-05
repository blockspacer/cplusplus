using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using IFuncPlugin;
using SqlServerHelper;
using System.Data.SqlClient;

namespace RechargeManage
{
    public partial class RechargeRecord : Form, IPlugin
    {
        private SqlHelper m_ObjSql = new SqlHelper();
        private SqlConnectInfo m_DBInfo = new SqlConnectInfo();

        private bool m_bFilterByDate = false;
        private DateTime m_FilterBegin;
        private DateTime m_FilterEnd;

        private int m_nCurPage = 0; //��ǰ��ʾҳ
        private int m_nRowsPerPage = 50;  //ÿҳ��ʾ��¼��
        private int m_nTotalPage = 1;  //��ҳ��


        public RechargeRecord()
        {
            InitializeComponent();
            //RechargeInfoPos();
            dtStart.Enabled = false;
            dtEnd.Enabled = false;
        }

        public MenuType GetMenuType()
        {
            return MenuType.eRechargeList;
        }

        public string PluginName()
        {
            return "RechargeRecord";
        }

        public Guid PluginGuid()
        {
            return new Guid("5315D784-78EC-4bf7-AE8B-E639BE54B784");
        }

        public string PluginMenu()
        {
            return "��ֵ��Ϣ";
        }

        public void ShowPluginForm(Panel parent, SqlConnectInfo DbInfo)
        {
            m_DBInfo = DbInfo;
            //���룬��������Ϊ�Ӵ�����ʾ
            this.TopLevel = false;
            this.Parent = parent;
            this.Show();
            this.BringToFront();
        }

        public void SetAuthority(int nLoginUserId, int nAuthority)
        {
            //empty            
        }

        /*
        private void RechargeInfoPos()
        {
            foreach (Control ctrl in Controls)
            {
                ControlPos pos = new ControlPos();
                pos.x = ctrl.Left;
                pos.dbRateH = (double)ctrl.Right / this.Width;
                pos.y = ctrl.Top;
                pos.dbRateV = (double)ctrl.Bottom / this.Height;
                ctrl.Tag = pos;
            }
        }

        private void RechargeInfo_Resize(object sender, EventArgs e)
        {
            foreach (Control ctrl in Controls)
            {
                ControlPos pos = (ControlPos)ctrl.Tag;
                pos.x = (int)((this.Width * pos.dbRateH) - ctrl.Width);
                pos.y = (int)((this.Height * pos.dbRateV) - ctrl.Height);
                ctrl.Left = pos.x;
                ctrl.Top = pos.y;
                ctrl.Tag = pos;
            }       
        }
        */

        private void RechargeRecord_Load(object sender, EventArgs e)
        {
            if (!m_ObjSql.OpenSqlServerConnection(m_DBInfo.strServerName, m_DBInfo.strDbName, m_DBInfo.strUser, m_DBInfo.strUserPwd))
            {
                m_ObjSql = null;
                return;
            }

            RechargeView.Columns.Clear();
            RechargeView.Columns.Add("Index", "���");
            RechargeView.Columns.Add("CardId", "����");
            RechargeView.Columns.Add("Type", "����");
            RechargeView.Columns.Add("ForwardBalance", "����ǰ���");
            RechargeView.Columns.Add("RechargeVal", "���");
            RechargeView.Columns.Add("CurrentBalance", "��������");
            RechargeView.Columns.Add("Time", "����ʱ��");
            RechargeView.Columns.Add("Operator", "����Ա");
            int[] ColumnWidth = new int[]{50,150,80,100,100,100,150,100};
            for (int i = 0; i < RechargeView.Columns.Count; i++)
            {
                RechargeView.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                RechargeView.Columns[i].Width = ColumnWidth[i];
            }

            GetRechargeDataTotalPage();
            //����Ȧ���¼
            ReadRechargeData();
        }

        private void GetRechargeDataTotalPage()
        {
            int nTotal = 0;
            SqlDataReader dataReader = null;
            m_ObjSql.ExecuteCommand("select count(RunningNum) Total from Data_RechargeCardRecord", out dataReader);
            if (dataReader != null)
            {
                if (dataReader.HasRows && dataReader.Read())
                {
                    nTotal = (int)dataReader["Total"];
                }
                dataReader.Close();
            }

            if (nTotal == 0 || nTotal % m_nRowsPerPage != 0)
                m_nTotalPage = nTotal / m_nRowsPerPage + 1;
            else
                m_nTotalPage = nTotal / m_nRowsPerPage;

            if (m_nTotalPage > 1)
            {
                btnPrevPage.Visible = true;
                btnNextPage.Visible = true;
            }
            else
            {
                btnPrevPage.Visible = false;
                btnNextPage.Visible = false;
            }
        }

        private void RechargeRecord_FormClosed(object sender, FormClosedEventArgs e)
        {
            RechargeView.Dispose();
            if (m_ObjSql != null)
            {
                m_ObjSql.CloseConnection();
                m_ObjSql = null;
            }
        }

        private void ReadRechargeData()
        {
            SqlHelper ObjSql = new SqlHelper();
            if (!ObjSql.OpenSqlServerConnection(m_DBInfo.strServerName, m_DBInfo.strDbName, m_DBInfo.strUser, m_DBInfo.strUserPwd))
            {
                ObjSql = null;
                return;
            }
            RechargeView.Rows.Clear();
            SqlDataReader dataReader = null;
            SqlParameter[] sqlparams = new SqlParameter[2];
            int nRowTotalIndex = 0;
            if (m_bFilterByDate)
            {
                sqlparams[0] = ObjSql.MakeParam("Start", SqlDbType.DateTime, 8, ParameterDirection.Input, m_FilterBegin);
                sqlparams[1] = ObjSql.MakeParam("End", SqlDbType.DateTime, 8, ParameterDirection.Input, m_FilterEnd);
                ObjSql.ExecuteCommand("select top 500 * from Data_RechargeCardRecord where RechargeDateTime > @Start and RechargeDateTime < @End", sqlparams, out dataReader);
                nRowTotalIndex = 0;
            }
            else
            {
                sqlparams[0] = ObjSql.MakeParam("Start", SqlDbType.Int, 4, ParameterDirection.Input, m_nCurPage * m_nRowsPerPage);
                sqlparams[1] = ObjSql.MakeParam("End", SqlDbType.Int, 4, ParameterDirection.Input, (m_nCurPage + 1) * m_nRowsPerPage);
                ObjSql.ExecuteCommand("select * from Data_RechargeCardRecord where RunningNum > @Start and RunningNum <= @End", sqlparams, out dataReader);
                nRowTotalIndex = m_nCurPage * m_nRowsPerPage;
            }
            if (dataReader != null)
            {
                if (dataReader.HasRows)
                {
                    int nCount = 0;
                    string strValue = "";
                    while (dataReader.Read())
                    {
                        int index = RechargeView.Rows.Add();
                        RechargeView.Rows[index].Cells[0].Value = nRowTotalIndex + nCount + 1;
                        strValue = (string)dataReader["CardNum"];
                        RechargeView.Rows[index].Cells[1].Value = strValue;
                        strValue = (string)dataReader["OperateType"];
                        RechargeView.Rows[index].Cells[2].Value = strValue;
                        decimal ForwardBal = (decimal)dataReader["ForwardBalance"];
                        RechargeView.Rows[index].Cells[3].Value = ForwardBal.ToString();
                        decimal RechargeVal = (decimal)dataReader["RechargeValue"];
                        RechargeView.Rows[index].Cells[4].Value = RechargeVal.ToString();
                        decimal CurrentBal = (decimal)dataReader["CurrentBalance"];
                        RechargeView.Rows[index].Cells[5].Value = CurrentBal.ToString();
                        DateTime RechargeTime = (DateTime)dataReader["RechargeDateTime"];
                        RechargeView.Rows[index].Cells[6].Value = RechargeTime.ToString("yyyy-MM-dd HH:mm:ss");
                        int OperatorId = (int)dataReader["OperatorId"];
                        RechargeView.Rows[index].Cells[7].Value = GetOperatorName(OperatorId);
                        nCount++;
                    }
                }
                dataReader.Close();
            }
            ObjSql.CloseConnection();
            ObjSql = null;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnPrevPage_Click(object sender, EventArgs e)
        {
            if (m_nTotalPage <= 1)
                return;
            if (m_nCurPage > 0)
            {
                m_nCurPage--;
                ReadRechargeData();
            }
        }

        private void btnNextPage_Click(object sender, EventArgs e)
        {
            if (m_nTotalPage <= 1)
                return;
            if (m_nCurPage < m_nTotalPage - 1)
            {
                m_nCurPage++;
                ReadRechargeData();
            }
        }

        private string GetOperatorName(int nOperatorID)
        {
            string strOperator = "";
            SqlParameter[] sqlparams = new SqlParameter[1];
            sqlparams[0] = m_ObjSql.MakeParam("UserId", SqlDbType.Int, 4, ParameterDirection.Input, nOperatorID);
            SqlDataReader dataReader = null;
            m_ObjSql.ExecuteCommand("select UserName from UserDb where UserId = @UserId", sqlparams, out dataReader);
            if (dataReader != null)
            {
                if (dataReader.HasRows && dataReader.Read())
                {
                    strOperator = (string)dataReader["UserName"];
                }
                dataReader.Close();
            }
            return strOperator;
        }

        private void chkFilter_CheckedChanged(object sender, EventArgs e)
        {
            m_bFilterByDate = chkFilter.Checked;
            if (m_bFilterByDate)
            {
                dtStart.Enabled = true;
                dtEnd.Enabled = true;
                dtStart.Value = DateTime.Now.Date.AddDays(-30);
                dtEnd.Value = DateTime.Now;
            }
            else
            {
                dtStart.Enabled = false;
                dtEnd.Enabled = false;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            m_FilterBegin = dtStart.Value.Date;
            m_FilterEnd = dtEnd.Value.Date.AddDays(1);
            if (m_FilterBegin > m_FilterEnd)
            {
                MessageBox.Show("ʱ�䷶Χ��Ч");
                return;
            }
            m_nCurPage = 0;
            ReadRechargeData();
        }

    }
}