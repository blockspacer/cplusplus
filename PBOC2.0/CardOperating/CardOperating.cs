using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using IFuncPlugin;
using SqlServerHelper;
using System.Data.SqlClient;
using ApduDaHua;

namespace CardOperating
{
    public partial class CardOperating : Form, IPlugin
    {
        private int m_MTDevHandler = 0;  //������
        private short m_nRetValue = 0;  //����ֵ

        private IccCardControl m_IccCardCtrl = null;
        private UserCardControl m_UserCardCtrl = null;

        private byte[] m_IccCardId = null;
        private byte[] m_UserCardId = null;

        private ICC_Status m_curIccStatus = ICC_Status.ICC_PowerOff;
        private bool m_bShowPanel = false;
        private IccCardInfo m_CardPSAM = new IccCardInfo();
        private UserCardInfo m_CardUser = new UserCardInfo();
        private CardApplicationTest m_CardMethod = new CardApplicationTest();

        private SqlConnectInfo m_DBInfo = new SqlConnectInfo();
        private int m_nCardOperatorAuthority = 0;

        public CardOperating()
        {
            InitializeComponent();
            //CardOperatingCtrlPos();

            m_CardUser.TopLevel = false;
            m_CardUser.Parent = this;
            CardInfoPanel.Controls.Add(m_CardUser);
            m_CardPSAM.TopLevel = false;
            m_CardPSAM.Parent = this;
            CardInfoPanel.Controls.Add(m_CardPSAM);

            m_CardMethod.TextOutput += new MessageOutput(OnMessageOutput);
            m_CardMethod.TopLevel = false;
            m_CardMethod.Parent = this;
            CardInfoPanel.Controls.Add(m_CardMethod);

        }

        public MenuType GetMenuType()
        {
            return MenuType.eCardOperating;
        }

        public string PluginName()
        {
            return "CardOperating";
        }

        public Guid PluginGuid()
        {
            return new Guid("1AFEA8C6-5026-4bf7-9C77-573D8C10E4A8");
        }

        public string PluginMenu()
        {
            return "�Ʒ�������";
        }

        public void ShowPluginForm(Panel parent, SqlConnectInfo DbInfo)
        {
            m_DBInfo = DbInfo;
            m_CardPSAM.SetDbInfo(m_DBInfo);
            m_CardUser.SetDbInfo(m_DBInfo);
            m_CardMethod.SetDbInfo(m_DBInfo);
            //���룬��������Ϊ�Ӵ�����ʾ
            this.TopLevel = false;
            this.Parent = parent;
            this.Show();
            this.BringToFront();
            if (m_nCardOperatorAuthority != GrobalVariable.CardOperating_Authority)
            {
                Card.Enabled = false;
                ICC_Card.Enabled = false;
                MessageBox.Show("��ǰ�û����ƿ�Ȩ��");
            }
        }

        public void SetAuthority(int nLoginUserId, int nAuthority)
        {
            m_nCardOperatorAuthority = nAuthority;
        }

        private void CardOperating_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_MTDevHandler <= 0)
                return;
            m_nRetValue = DllExportMT.close_device(m_MTDevHandler);
            m_MTDevHandler = 0;
        }

        private void CardOprQuit_Click(object sender, EventArgs e)
        {
            Close();
        }

        /*
        private void CardOperatingCtrlPos()
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

        private void CardOperating_Resize(object sender, EventArgs e)
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

        private void WriteMsg(short nErr, string strMsg)
        {
            string strTextOut = "";
            if (nErr != 0)
                strTextOut = strMsg + " �����룺" + nErr.ToString("X4") + "\r\n";
            else
                strTextOut = strMsg + "\r\n";

            OutputText.AppendText(strTextOut);
            OutputText.Refresh();
            OutputText.ScrollToCaret();
        }

        private void btnCleanInfo_Click(object sender, EventArgs e)
        {
            OutputText.Text = "";
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (m_MTDevHandler > 0)
                return;
            m_MTDevHandler = DllExportMT.open_device(0, 9600);
            if (m_MTDevHandler <= 0)
            {
                WriteMsg((short)m_MTDevHandler, "��������ʧ��");
                btnDisconnect.Enabled = false;
            }
            else
            {
                WriteMsg(0, "�������ӳɹ�");
                btnDisconnect.Enabled = true;
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            if (m_MTDevHandler <= 0)
                return;
            m_nRetValue = DllExportMT.close_device(m_MTDevHandler);
            if (m_nRetValue != 0)
                WriteMsg(m_nRetValue, "�Ͽ�����ʧ��");
            else
                WriteMsg(0, "�Ͽ����ӳɹ�");
            btnDisconnect.Enabled = false;
            m_MTDevHandler = 0;
        }

        private void btnOpenCard_Click(object sender, EventArgs e)
        {
            if (m_MTDevHandler <= 0)
                return;
            byte[] cardUid = new byte[4];
            byte[] cardInfo = new byte[64];
            byte[] cardInfolen = new byte[4];
            uint infoLen = 0;
            m_nRetValue = DllExportMT.OpenCard(m_MTDevHandler, 1, cardUid, cardInfo, cardInfolen);
            if (m_nRetValue != 0)
            {
                WriteMsg(m_nRetValue, "�ǽӴ�ʽ����ʧ��");
            }
            else
            {
                WriteMsg(0, "�ǽӴ�ʽ���򿪳ɹ�");
                byte[] cardUidAsc = new byte[8];
                DllExportMT.hex_asc(cardUid, cardUidAsc, 4);
                WriteMsg(0, "Uid��" + Encoding.ASCII.GetString(cardUidAsc));
                infoLen = BitConverter.ToUInt32(cardInfolen, 0);
                byte[] cardInfoAsc = new byte[infoLen * 2];
                DllExportMT.hex_asc(cardInfo, cardInfoAsc, infoLen);
                WriteMsg(0, "����Ϣ��" + Encoding.ASCII.GetString(cardInfoAsc));
            }

            m_UserCardCtrl = new UserCardControl(m_MTDevHandler, m_DBInfo);
            m_UserCardCtrl.TextOutput += new MessageOutput(OnMessageOutput);
            if (!m_UserCardCtrl.ReadKeyValueFormDb())
                WriteMsg(0, "δ������Կ���������ݿ��Ƿ�������");
            m_UserCardCtrl.GetCardCosVersion();
        }

        private void btnCloseCard_Click(object sender, EventArgs e)
        {
            if (m_MTDevHandler <= 0)
                return;
            m_nRetValue = DllExportMT.CloseCard(m_MTDevHandler);
            if (m_nRetValue != 0)
                WriteMsg(m_nRetValue, "�رտ�Ƭʧ��");
            else
                WriteMsg(0, "�رտ�Ƭ�ɹ�");            
            m_UserCardCtrl = null;
        }

        private void OnMessageOutput(MsgOutEvent args)
        {
            WriteMsg((short)args.ErrCode, args.Message);
        }

        //ɾ���׿�MF�е��ļ���ֻ����MF
        private void btnInitCard_Click(object sender, EventArgs e)
        {
            if (m_MTDevHandler <= 0 || m_UserCardCtrl == null)
                return;
            //����
            if (m_UserCardCtrl.InitCard(false) != 0)
                MessageBox.Show("��ǰ��Ƭ��������Կ��ƥ�䣬��ʼ��ʧ�ܡ�\r\n��ȷ�Ͽ���Ȼ�����á�", "����", MessageBoxButtons.OK);
        }

        private void btnUserCardReset_Click(object sender, EventArgs e)
        {
            if (m_MTDevHandler <= 0 || m_UserCardCtrl == null)
                return;
            //����
            if (m_UserCardCtrl.InitCard(true) != 0)
                MessageBox.Show("��ǰ��Ƭ��������Կ��ƥ�䣬����ʧ�ܡ�\r\n��ȷ�Ͽ���Ȼ���ʼ����", "����", MessageBoxButtons.OK);
        }

        //����Ϣ����
        private void UserCardSetting_Click(object sender, EventArgs e)
        {
            if (m_MTDevHandler <= 0)
                return;
            if (!m_CardUser.Visible && m_bShowPanel)
            {
                m_CardPSAM.Hide();
                m_CardMethod.Hide();
                m_CardUser.Show();
            }
            else
            {
                m_bShowPanel = !m_bShowPanel;
                if (m_bShowPanel)
                {
                    CardInfoPanel.Visible = true;
                    this.Width += CardInfoPanel.Width;
                    m_CardUser.Show();
                }
                else
                {
                    this.Width -= CardInfoPanel.Width;
                    CardInfoPanel.Visible = false;
                    m_CardUser.Hide();
                }
            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            if (m_MTDevHandler <= 0 || m_UserCardCtrl == null)
                return;
            if (!m_UserCardCtrl.CreateDIR())
                return;
            m_UserCardCtrl.CreateKey();
        }


        private void btnApplication_Click(object sender, EventArgs e)
        {
            if (m_MTDevHandler <= 0 || m_UserCardCtrl == null)
                return;
            UserCardInfoParam cardInfo = m_CardUser.GetUserCardParam();
            m_UserCardId = cardInfo.GetUserCardID();
            if (m_UserCardId == null)
            {
                WriteMsg(0, "�û�����Ϊ�գ����Ƚ�����Ϣ���á�");
                return;
            }
            WriteMsg(0, "�û����ţ�" + BitConverter.ToString(m_UserCardId));
            //��������Ӧ��ADF01
            if (!m_UserCardCtrl.CreateADFApp())
                return;
            //���ɼ��������ļ�
            if (!m_UserCardCtrl.CreateApplication(m_UserCardId, cardInfo.DefaultPwdFlag, cardInfo.CustomPassword))
                return;
            m_UserCardCtrl.UpdateApplicationFile(cardInfo,null);

            //���������ݿ�            
            string strSuccess = m_UserCardCtrl.SaveCpuCardInfoToDb(cardInfo) ? "�ɹ�" : "ʧ��";
            WriteMsg(0, "����Ϣд�����ݿ⣬�����" + strSuccess);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////PSAM���ƿ�///////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void btnOpenIccCard_Click(object sender, EventArgs e)
        {
            if (m_MTDevHandler <= 0)
                return;
            byte[] sInfo = new byte[64];
            byte[] sInfolen = new byte[4];
            uint infoLen = 0;
            if (m_curIccStatus == ICC_Status.ICC_PowerOn)
            {
                m_nRetValue = DllExportMT.ICC_Reset(m_MTDevHandler, 0x00, sInfo, sInfolen);
            }
            else
            {
                m_nRetValue = DllExportMT.ICC_PowerOn(m_MTDevHandler, 0x00, sInfo, sInfolen);
            }

            if (m_nRetValue != 0)
            {
                WriteMsg(m_nRetValue, "�Ӵ�ʽ����λʧ��");
                m_curIccStatus = ICC_Status.ICC_PowerOff;
                return;
            }
            else
            {
                if (m_curIccStatus == ICC_Status.ICC_PowerOff)
                    WriteMsg(0, "�Ӵ�ʽ���ϵ縴λ�ɹ�");
                else
                    WriteMsg(0, "�Ӵ�ʽ�����¸�λ�ɹ�");
                m_curIccStatus = ICC_Status.ICC_PowerOn;
                infoLen = BitConverter.ToUInt32(sInfolen, 0);
                byte[] infoAsc = new byte[infoLen * 2];
                DllExportMT.hex_asc(sInfo, infoAsc, infoLen);
                WriteMsg(0, "��λ��Ϣ��" + Encoding.ASCII.GetString(infoAsc));
            }
            m_IccCardId = new byte[infoLen];
            Buffer.BlockCopy(sInfo, 0, m_IccCardId, 0, (int)infoLen);

            m_IccCardCtrl = new IccCardControl(m_MTDevHandler, m_DBInfo);
            m_IccCardCtrl.TextOutput += new MessageOutput(OnMessageOutput);

            if (!m_IccCardCtrl.ReadKeyValueFormDb())
                WriteMsg(0, "δ������Կ���������ݿ��Ƿ�������");
            m_IccCardCtrl.GetCardCosVersion();
        }

        private void btnCloseIccCard_Click(object sender, EventArgs e)
        {
            if (m_MTDevHandler <= 0)
                return;
            m_nRetValue = DllExportMT.ICC_PowerOff(m_MTDevHandler, 0x00);
            if (m_nRetValue != 0)
                WriteMsg(m_nRetValue, "��Ƭ�ر�ʧ��");
            else
                WriteMsg(m_nRetValue, "��Ƭ�رճɹ�");
            m_curIccStatus = ICC_Status.ICC_PowerOff;
            m_IccCardCtrl = null;
        }

        //ɾ���׿�MF�е��ļ���ֻ����MF
        private void btnInitIccCard_Click(object sender, EventArgs e)
        {
            if (m_MTDevHandler <= 0 || m_IccCardCtrl == null)
                return;
            //����            
            if (m_IccCardCtrl.InitIccCard(false) != 0)
                MessageBox.Show("��ǰPSAM����������Կ��ƥ�䣬��ʼ��ʧ�ܡ�\r\n��ȷ�Ͽ���Ȼ�����á�", "����", MessageBoxButtons.OK);
        }

        private void btnIccCardReset_Click(object sender, EventArgs e)
        {
            if (m_MTDevHandler <= 0 || m_IccCardCtrl == null)
                return;
            //����            
            if (m_IccCardCtrl.InitIccCard(true) != 0)
                MessageBox.Show("��ǰPSAM����������Կ��ƥ�䣬����ʧ�ܡ�\r\n��ȷ�Ͽ���Ȼ���ʼ����", "����", MessageBoxButtons.OK);
        }

        private void IccCardSetting_Click(object sender, EventArgs e)
        {
            if (m_MTDevHandler <= 0)
                return;
            if (!m_CardPSAM.Visible && m_bShowPanel)
            {
                m_CardUser.Hide();
                m_CardMethod.Hide();
                m_CardPSAM.Show();
            }
            else
            {
                m_bShowPanel = !m_bShowPanel;
                if (m_bShowPanel)
                {
                    CardInfoPanel.Visible = true;
                    this.Width += CardInfoPanel.Width;
                    m_CardPSAM.Show();
                }
                else
                {
                    this.Width -= CardInfoPanel.Width;
                    CardInfoPanel.Visible = false;
                    m_CardPSAM.Hide();
                }                
            }

        }

        private void btnIccCreate_Click(object sender, EventArgs e)
        {
            if (m_MTDevHandler <= 0 || m_IccCardCtrl == null)
                return;
            IccCardInfoParam PSAMInfo = m_CardPSAM.GetPSAMCardParam();
            m_IccCardId = PSAMInfo.GetBytePsamId();
            if (m_IccCardId == null)
            {
                WriteMsg(0, "PSAM����Ϊ�գ����Ƚ�����Ϣ���á�");
                return;
            }

            if (IsExistPsamId(m_IccCardId))
            {
                if (MessageBox.Show("��PSAM�����Ѵ��ڣ��Ƿ�Ҫ����������PSAM����", "��ʾ", MessageBoxButtons.YesNo) == DialogResult.No)
                    return;                
            }

            byte[] TermialId = PSAMInfo.GetByteTermId();
            if (TermialId == null)
            {
                WriteMsg(0, "�ն˻����Ϊ�գ����Ƚ�����Ϣ���á�");
                return;
            }

            WriteMsg(0, "PSAM���ţ�" + BitConverter.ToString(m_IccCardId));
            WriteMsg(0, "�ն˻���ţ�" + BitConverter.ToString(TermialId));
            if (!m_IccCardCtrl.CreateIccInfo(m_IccCardId, TermialId))
                return;
            m_IccCardCtrl.WriteApplicationInfo(PSAMInfo);
        }

        private void btnIccAppKey_Click(object sender, EventArgs e)
        {
            if (m_MTDevHandler <= 0 || m_IccCardCtrl == null)
                return;
            //��װ������Կ
            if (!m_IccCardCtrl.SetupIccKey())
                return;
            m_IccCardCtrl.SetupMainKey();
            //���������ݿ�
            string strSuccess = m_IccCardCtrl.SavePsamCardInfoToDb(m_CardPSAM.GetPSAMCardParam()) ? "�ɹ�" : "ʧ��";
            WriteMsg(0, "����Ϣд�����ݿ⣬�����" + strSuccess);

        }

        private void btnMethod_Click(object sender, EventArgs e)
        {
            if (m_MTDevHandler <= 0)
                return;
            if (!m_CardMethod.Visible && m_bShowPanel)
            {
                m_CardPSAM.Hide();
                m_CardUser.Hide();
                m_CardMethod.Show();
                m_CardMethod.SetDeviceHandler(m_MTDevHandler);
            }
            else
            {
                m_bShowPanel = !m_bShowPanel;
                if (m_bShowPanel)
                {
                    CardInfoPanel.Visible = true;
                    m_CardMethod.Show();
                    m_CardMethod.SetDeviceHandler(m_MTDevHandler);
                    this.Width += CardInfoPanel.Width;                                        
                }
                else
                {                    
                    CardInfoPanel.Visible = false;
                    m_CardMethod.Hide();
                    this.Width -= CardInfoPanel.Width;
                }                
            }
        }

        private bool IsExistPsamId(byte[] psamID)
        {            
            SqlHelper ObjSql = new SqlHelper();
            if (!ObjSql.OpenSqlServerConnection(m_DBInfo.strServerName, m_DBInfo.strDbName, m_DBInfo.strUser, m_DBInfo.strUserPwd))
            {
                ObjSql = null;
                return false;
            }
            bool bExist = false;
            string strPsamId = BitConverter.ToString(psamID).Replace("-", "");


            SqlParameter[] sqlparams = new SqlParameter[1];
            sqlparams[0] = ObjSql.MakeParam("PsamId",SqlDbType.Char,16,ParameterDirection.Input,strPsamId);

            SqlDataReader dataReader = null;
            ObjSql.ExecuteCommand("select * from Psam_Card where PsamId=@PsamId", sqlparams, out dataReader);
            if (dataReader != null)
            {
                if (dataReader.HasRows && dataReader.Read())
                {
                    bExist = true;
                }
                dataReader.Close();
            }
            ObjSql.CloseConnection();
            ObjSql = null;
            return bExist;
        }

    }
}