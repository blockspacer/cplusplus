using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using IFuncPlugin;
using ApduParam;
using ApduCtrl;
using ApduInterface;

namespace CardOperating
{
    public partial class CardApplicationTest : Form
    {
        private const Char Backspace = (Char)8;
        public event MessageOutput TextOutput = null;

        private ISamCardControl m_SamCardCtrl = null;
        private IUserCardControl m_UserCardCtrl = null;

        private readonly byte[] m_FixedTermialId = new byte[] { 0x14, 0x32, 0x00, 0x00, 0x00, 0x01 };  //�̶����ն˻��豸���
        private static byte[] m_TermialId = new byte[6];      //�ն˻��豸���
        private static byte[] m_GTAC = new byte[4];

        private static byte[] m_ASN = new byte[] { 0x06, 0x71, 0x02, 0x01, 0x00, 0x00, 0x00, 0x01 };//�û�������

        private SqlConnectInfo m_DBInfo = new SqlConnectInfo();

        private ApduController m_DevControl = null;
        private bool m_bContactCard = false;

        private int m_nAppIndex = 1;

        public CardApplicationTest()
        {
            InitializeComponent();

            Page1Init();
            Page2Init();            
        }

        public void SetDbInfo(SqlConnectInfo DbInfo)
        {
            m_DBInfo = DbInfo;
        }

        public void SetDeviceHandler(ApduController ApduCtrlObj,bool bContactCard)
        {
            m_DevControl = ApduCtrlObj;
            m_bContactCard = bContactCard;

            //�Ӵ�ʽ�û���ֻ��ʹ��SAM��������
            if (m_bContactCard)
            {
                SamSlot.Checked = true;
                SamSlot.Enabled = false;

                LySamSlot.Checked = true;
                LySamSlot.Enabled = false;


            }
            else
            {
                SamSlot.Checked = false;
                SamSlot.Enabled = true;

                LySamSlot.Checked = false;
                LySamSlot.Enabled = true;
            }

            ContactCard.Checked = m_bContactCard;
        }

        private bool OpenSAMCard(bool bSamSlot, int nAppIndex)
        {
            if (m_DevControl == null || !m_DevControl.IsDeviceOpen())
                return false;
            m_SamCardCtrl = m_DevControl.SamCardConstructor(m_DBInfo);
            m_SamCardCtrl.TextOutput += new MessageOutput(OnMessageOutput);
            int nResult = m_SamCardCtrl.ReadKeyValueFromSource();
            if (nResult == 1)
            {
                MessageBox.Show("�����ݿ��ȡPSAM����Կʧ�ܣ����顣");
                return false;
            }
            else if(nResult == 2)
            {
                MessageBox.Show("��XML�ļ���ȡPSAM����Կʧ�ܣ����顣");
                return false;
            }
                        
            string strCardInfo = "";
            bool bRet = m_DevControl.SAMPowerOn(bSamSlot, ref strCardInfo);
            if (!bRet)
            {
                OnMessageOutput(new MsgOutEvent(0, "SAM����λʧ��"));                
                return false;
            }
            else
            {
                OnMessageOutput(new MsgOutEvent(0, "SAM����λ�ɹ�"));
                OnMessageOutput(new MsgOutEvent(0, "��λ��Ϣ��" + strCardInfo));
            }

            byte[] TermialId = m_SamCardCtrl.GetTerminalId(bSamSlot);
            if(TermialId != null)
                Buffer.BlockCopy(TermialId, 0, m_TermialId, 0, 6);
            return true;
        }

        private bool CloseSAMCard(bool bSamSlot)
        {
            if (m_DevControl == null || !m_DevControl.IsDeviceOpen())
                return false;
            m_DevControl.SAMPowerOff(bSamSlot);
            OnMessageOutput(new MsgOutEvent(0, "�رտ�Ƭ�ɹ�"));
            m_SamCardCtrl = null;
            return true;
        }

        private bool OpenUserCard()
        {
            if (m_DevControl == null || !m_DevControl.IsDeviceOpen())
                return false;
            m_UserCardCtrl = m_DevControl.UserCardConstructor(m_bContactCard, m_DBInfo);
            m_UserCardCtrl.TextOutput += new MessageOutput(OnMessageOutput);
            int nResult = m_UserCardCtrl.ReadKeyValueFromSource();
            if (nResult == 1)
            {
                MessageBox.Show("�����ݿ��ȡ�û�����Կʧ�ܣ����顣");
                return false;
            }
            else if (nResult == 2)
            {
                MessageBox.Show("��XML�ļ���ȡ�û�����Կʧ�ܣ����顣");
                return false;
            }

            string cardInfo = "";
            if (m_bContactCard)
            {
                bool bRet = m_DevControl.OpenContactCard(ref cardInfo);
                if (!bRet)
                {
                    OnMessageOutput(new MsgOutEvent(0, "�Ӵ�ʽ�û�����ʧ��"));                    
                    return false;
                }
            }
            else
            {
                bool bRet = m_DevControl.OpenCard(ref cardInfo);
                if (!bRet)
                {
                    OnMessageOutput(new MsgOutEvent(0, "�ǽӴ�ʽ����ʧ��"));
                    return false;
                }
            }

            OnMessageOutput(new MsgOutEvent(0, "�û����򿪳ɹ�"));
            OnMessageOutput(new MsgOutEvent(0, "����Ϣ��" + cardInfo));
            return true;
        }

        private bool CloseUserCard()
        {
            if (m_DevControl == null || !m_DevControl.IsDeviceOpen())
                return false;
            if (m_bContactCard)
                m_DevControl.CloseContactCard();
            else
                m_DevControl.CloseCard();
            OnMessageOutput(new MsgOutEvent(0, "�رտ�Ƭ�ɹ�"));
            m_UserCardCtrl = null;
            return true;
       }

        private void OnMessageOutput(MsgOutEvent args)
        {
            if (this.TextOutput != null)
                this.TextOutput(args);
        }

        private bool ReadUserCardAsn(int nAppIndex)
        {
            if (!m_UserCardCtrl.SelectCardApp(nAppIndex))
                return false;
            DateTime cardStart = DateTime.MinValue;
            DateTime cardEnd = DateTime.MinValue;
            byte[] ASN = m_UserCardCtrl.GetUserCardASN(true, ref cardStart, ref cardEnd);
            if (ASN == null)
                return false;
            Buffer.BlockCopy(ASN, 0, m_ASN, 0, 8);
            return true;
        }

        private string RecordType(byte RecordType)
        {
            string strT = RecordType.ToString();
            switch (RecordType)
            {
                case 0x01:
                    strT = "Ȧ�����";
                    break;
                case 0x02:
                    strT = "Ȧ��Ǯ��";
                    break;
                case 0x03:
                    strT = "����Ȧ��";
                    break;
                case 0x93:
                    strT = "��������";
                    break;
                case 0x95:
                    strT = "�������׽��";
                    break;
                case 0xB1:
                    strT = "����Ȧ��";
                    break;
                case 0xA3:
                    strT = "��������";
                    break;
                case 0xA5:
                    strT = "�������ֽ��";
                    break;
            }
            return strT;
        }

        private void ContactCard_CheckedChanged(object sender, EventArgs e)
        {
            m_bContactCard = ContactCard.Checked;
            //�Ӵ�ʽ�û���ֻ��ʹ��SAM��������
            if (m_bContactCard)
            {
                SamSlot.Checked = true;
                SamSlot.Enabled = false;

                LySamSlot.Checked = true;
                LySamSlot.Enabled = false;

            }
            else
            {
                SamSlot.Checked = false;
                SamSlot.Enabled = true;

                LySamSlot.Checked = false;
                LySamSlot.Enabled = true;

            }
        }

        private void tabApp_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabApp.SelectedIndex == 1)
                m_nAppIndex = 2;
            else
                m_nAppIndex = 1;
        }

        //����ת���
        private void btnToED_Click(object sender, EventArgs e)
        {
            int nLyAmount = 0;
            int.TryParse(textLyPurchase.Text, out nLyAmount);//���ѻ��ֽ��
            if (nLyAmount < 1)
                return;
            decimal LyRate = 0;
            decimal.TryParse(textRate.Text, out LyRate);
            double dbLyRate = decimal.ToDouble(LyRate);
            if (dbLyRate < 1)
                return;
            int nRealMoney = Convert.ToInt32(Math.Floor(nLyAmount / dbLyRate));//��λ��Ԫ

            bool bSamSlot = LySamSlot.Checked;
            nLyAmount = (int)(nRealMoney * dbLyRate);  //�����۳�����

            if (m_bGray || m_bLyGray)
                return;
            if (!OpenUserCard())
                return;

            if (!LoyaltyPurchase(nLyAmount))
            {
                CloseUserCard();
                return;
            }
            //Ȧ�浥λΪ��
            if (!MoneyLoad((int)(nRealMoney * 100.0), m_TermialId))
            {
                string strMsg = string.Format("ת�ɽ��ʧ�ܡ��ѿ۳�{0}���֣�Ӧת����{1}Ԫ��", nLyAmount.ToString(), nRealMoney.ToString());
                MessageBox.Show(strMsg);
                CloseUserCard();
                return;
            }
            string strOkMsg = string.Format("�ɹ�ת����{0}Ԫ", nRealMoney.ToString());
            MessageBox.Show(strOkMsg);
            CloseUserCard();
        }
    }
}