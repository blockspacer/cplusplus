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

        private readonly byte[] m_FixedTermialId = new byte[] { 0x20, 0x15, 0x01, 0x01, 0x00, 0x01 };  //�̶����ն˻��豸���

        private static byte[] m_TermialId = new byte[6];      //�ն˻��豸���
        private static byte[] m_ASN = new byte[] { 0x06, 0x71, 0x02, 0x01, 0x00, 0x00, 0x00, 0x01 };//�û�������
        private static string m_strPIN = "999999";//���û�����
                
        private bool m_bGray = false;   //���ѻң����ܿۿ����        

        private int m_nBusinessSn;  //�ѻ��������
        private int m_nTermialSn;  //�ն˽������ 

        private SqlConnectInfo m_DBInfo = new SqlConnectInfo();

        private ApduController m_DevControl = null;
        private bool m_bContactCard = false;

        public CardApplicationTest()
        {
            InitializeComponent();
            textPIN.Text = m_strPIN;
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
            }
            else
            {
                SamSlot.Checked = false;
                SamSlot.Enabled = true;
            }

            ContactCard.Checked = m_bContactCard;
        }

        private bool OpenSAMCard(bool bSamSlot)
        {
            if (m_DevControl == null || !m_DevControl.IsDeviceOpen())
                return false;
            m_SamCardCtrl = m_DevControl.SamCardConstructor(m_DBInfo);
            m_SamCardCtrl.TextOutput += new MessageOutput(OnMessageOutput);
            if (!m_SamCardCtrl.ReadKeyValueFormDb())
                OnMessageOutput(new MsgOutEvent(0, "δ������Կ���������ݿ��Ƿ�������"));
                        
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
            if (!m_UserCardCtrl.ReadKeyValueFormDb())
            {
                MessageBox.Show("δ����Ĭ����Կ���������ݿ��Ƿ�������");
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

        //Ȧ��
        private void btnCardLoad_Click(object sender, EventArgs e)
        {
            if (!OpenUserCard() || !ReadUserCardAsn())
                return;
            decimal MoneyLoad = decimal.Parse(textMoney.Text, System.Globalization.NumberStyles.Number);
            double dbMoneyLoad = decimal.ToDouble(MoneyLoad);
            //Ȧ��
            string strInfo = string.Format("�Կ���{0}Ȧ��{1}Ԫ", BitConverter.ToString(m_ASN), dbMoneyLoad.ToString("F2"));
            OnMessageOutput( new MsgOutEvent(0, strInfo) );
            if(m_UserCardCtrl.VerifyUserPin(m_strPIN) == 1)
            {
                byte[] TerminalId = new byte[6];
                if (PublicFunc.ByteDataEquals(TerminalId, m_TermialId))//δ�����ն˻���ţ�ʹ�ù̶����
                    Buffer.BlockCopy(m_FixedTermialId, 0, TerminalId, 0, 6);
                else
                    Buffer.BlockCopy(m_TermialId, 0, TerminalId, 0, 6);
                m_UserCardCtrl.UserCardLoad(m_ASN, TerminalId, (int)(dbMoneyLoad * 100.0),false);
            }
            CloseUserCard();            
        }

        private void btnBalance_Click(object sender, EventArgs e)
        {
            if (!OpenUserCard() || !ReadUserCardAsn())
                return;
            string strInfo = string.Format("��ȡ����{0}����������Ƿ������", BitConverter.ToString(m_ASN));
            OnMessageOutput(new MsgOutEvent(0, strInfo));
            if (m_UserCardCtrl.VerifyUserPin(m_strPIN) == 1)
            {
                m_bGray = false;                
                //δ����ʱ�ն˻�������Ϊ0
                int nCardStatus = 0;
                if (m_UserCardCtrl.UserCardGray(ref nCardStatus, m_TermialId))
                {
                    if (nCardStatus == 2)
                    {
                        //��ǰTACδ������Ҫ��պ��ض�
                        m_UserCardCtrl.ClearTACUF();
                        nCardStatus = 0;
                        m_UserCardCtrl.UserCardGray(ref nCardStatus, m_TermialId);
                        m_bGray = nCardStatus == 1 ? true : false;
                    }
                    else
                    {
                        m_bGray = nCardStatus == 1 ? true : false;
                    }
                    GrayFlag.CheckState = m_bGray ? CheckState.Checked : CheckState.Unchecked;
                    GrayFlag.Checked = m_bGray;
                }
                else
                {
                    GrayFlag.CheckState = CheckState.Indeterminate;
                    GrayFlag.Checked = false;
                }

                double dbBalance = 0.0f;
                if (m_UserCardCtrl.UserCardBalance(ref dbBalance))
                    textBalance.Text = dbBalance.ToString("F2");
                else
                    textBalance.Text = "0.00";
            }            
            CloseUserCard();
        }

        //ǿ�ƽ��
        private void btnUnlockGrayCard_Click(object sender, EventArgs e)
        {
            //δ��״̬����ǿ�ƽ��
            if (!m_bGray || !OpenUserCard() || !ReadUserCardAsn())
                return;
            if (m_UserCardCtrl.VerifyUserPin(m_strPIN) == 1)
            {
                const float BusinessMoney = 0.0F;//ǿ��������� 0 �ۿ�
                byte[] TerminalId = new byte[6];
                if (PublicFunc.ByteDataEquals(TerminalId, m_TermialId))//δ�����ն˻���ţ�ʹ�ù̶����
                    Buffer.BlockCopy(m_FixedTermialId, 0, TerminalId, 0, 6);
                else
                    Buffer.BlockCopy(m_TermialId, 0, TerminalId, 0, 6);
                if(m_UserCardCtrl.UnLockGrayCard(m_ASN, TerminalId, (int)(BusinessMoney * 100.0), false))
                    m_bGray = false;
            }
            CloseUserCard();            
        }

        private void btnLockCard_Click(object sender, EventArgs e)
        {
            if (m_bGray)
                return;
            if (!OpenUserCard() || !ReadUserCardAsn())
                return;
            if (m_UserCardCtrl.VerifyUserPin(m_strPIN) != 1)
                return;            
            //������ʼ��
            byte[] outData = new byte[15];
            m_UserCardCtrl.InitForGray(m_TermialId,outData);
            byte[] byteBalance = new byte[4];
            Buffer.BlockCopy(outData, 0, byteBalance, 0, 4);//ET���
            byte[] OfflineSn = new byte[2];//ET�ѻ��������
            Buffer.BlockCopy(outData, 4, OfflineSn, 0, 2);
            byte keyVer = outData[9];
            byte keyFlag = outData[10];
            byte[] rand = new byte[4];
            Buffer.BlockCopy(outData, 11, rand, 0, 4);
            //����
            const byte BusinessType = 0x91;//��������
            byte[] GrayLockData = new byte[19]; //��PSAM�����˳��Ϊ�ն˽�����ţ��ն��������BCDʱ�䣬MAC1

            bool bSamSlot = SamSlot.Checked;//ʹ��SAM����

            if (!OpenSAMCard(bSamSlot) || !m_SamCardCtrl.SamAppSelect(bSamSlot))
                return;
            if (!m_SamCardCtrl.InitSamGrayLock(bSamSlot, m_TermialId, rand, OfflineSn, byteBalance, BusinessType, m_ASN, GrayLockData))
                return;
            byte[] GTAC = new byte[4];
            byte[] MAC2 =new byte[4];
            if (!m_UserCardCtrl.GrayLock(GrayLockData, GTAC, MAC2))
                return;
            if (!m_SamCardCtrl.VerifyMAC2(bSamSlot, MAC2))//��֤MAC2
                return;
            m_nBusinessSn = (int)((OfflineSn[0] << 8) | OfflineSn[1]);
            m_nTermialSn = (int)((GrayLockData[0] << 24) | (GrayLockData[1] << 16) | (GrayLockData[2] << 8) | GrayLockData[3]);
            if(!m_bContactCard)
                SamSlot.Enabled = false;
        }

        private byte[] GetDebitforUnlockData(bool bSamSlot)
        {
            byte[] DebitData = new byte[27];
             //����GMAC
            const byte BusinessType = 0x93;//��������: ��0��
            decimal Amount = decimal.Parse(textPurchase.Text, System.Globalization.NumberStyles.Number);
            double dbAmount = decimal.ToDouble(Amount);
            int nMoneyAmount = (int)(dbAmount * 100.0); ////��Ʊ���ѽ��
            byte[] GMAC = new byte[4];
            if (!m_SamCardCtrl.CalcGMAC(bSamSlot, BusinessType, m_ASN, m_nBusinessSn, nMoneyAmount, GMAC))
                return null;
            byte[] byteMoney = BitConverter.GetBytes(nMoneyAmount); //��Ʊ���ѽ��
            DebitData[0] = byteMoney[3];
            DebitData[1] = byteMoney[2];
            DebitData[2] = byteMoney[1];
            DebitData[3] = byteMoney[0];
            m_nBusinessSn += 1;
            DebitData[4] = (byte)((m_nBusinessSn >> 8) & 0xFF);
            DebitData[5] = (byte)(m_nBusinessSn & 0xFF);
            Buffer.BlockCopy(m_TermialId, 0, DebitData, 6, 6);
            m_nTermialSn += 1;
            DebitData[12] = (byte)((m_nTermialSn >> 24) & 0xFF);//�ն˽������
            DebitData[13] = (byte)((m_nTermialSn >> 16) & 0xFF);
            DebitData[14] = (byte)((m_nTermialSn >> 8) & 0xFF);
            DebitData[15] = (byte)(m_nTermialSn & 0xFF);
            byte[] SysTime = PublicFunc.GetBCDTime();
            Buffer.BlockCopy(SysTime, 0, DebitData, 16, 7);//BCDʱ��
            Buffer.BlockCopy(GMAC, 0, DebitData, 23, 4);//GMAC
            return DebitData;
        }

        private void btnUnlockCard_Click(object sender, EventArgs e)
        {
            if (m_bGray || m_UserCardCtrl == null)
                return;
            bool bSamSlot = SamSlot.Checked;
            byte[] UnlockData = null;
            if (m_SamCardCtrl != null)
            {
                UnlockData = GetDebitforUnlockData(bSamSlot);
                CloseSAMCard(bSamSlot);
            }
            if(!m_bContactCard)
                SamSlot.Enabled = true;

            if (UnlockData != null)
            {
                //���debit for unlock
                if (m_UserCardCtrl.DebitForUnlock(UnlockData))
                {
                    //��TACUF ���� ������״̬��������P1 == 0x01��
                    m_UserCardCtrl.ClearTACUF();
                }
            }
            CloseUserCard();
        }

        private bool ReadUserCardAsn()
        {
            if (!m_UserCardCtrl.SelectCardApp())
                return false;
            DateTime cardStart = DateTime.MinValue;
            DateTime cardEnd = DateTime.MinValue;
            byte[] ASN = m_UserCardCtrl.GetUserCardASN(true, ref cardStart, ref cardEnd);
            if (ASN == null)
                return false;
            Buffer.BlockCopy(ASN, 0, m_ASN, 0, 8);
            return true;
        }

        private void textPIN_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != Backspace)
                e.Handled = true;//�����ܷ�����ֵ
        }

        private void textPIN_Validated(object sender, EventArgs e)
        {
            m_strPIN = textPIN.Text;
        }

        private void btnReadRecord_Click(object sender, EventArgs e)
        {
            if (!OpenUserCard())
                return;
            if (!m_UserCardCtrl.SelectCardApp())
                return;
            if (m_UserCardCtrl.VerifyUserPin(m_strPIN) == 1)
            {                 
                List<CardRecord> lstRecord = m_UserCardCtrl.ReadRecord();
                if (lstRecord.Count > 0)
                {
                    FillListView(lstRecord);
                }
            }
            CloseUserCard();            
        }

        public void FillListView(List<CardRecord> lstRecord)
        {
            RecordInCard.Items.Clear();
            foreach (CardRecord record in lstRecord)
            {
                ListViewItem item = new ListViewItem();
                item.Text = record.BusinessSn.ToString();
                item.SubItems.Add(record.OverdraftMoney.ToString("F2"));
                item.SubItems.Add(record.Amount.ToString("F2"));
                item.SubItems.Add(RecordType(record.BusinessType));
                item.SubItems.Add(record.TerminalID);
                item.SubItems.Add(record.BusinessTime);
                RecordInCard.Items.Add(item);
            }
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
                    strT = "Ȧ�����";
                    break;
                case 0x93:
                    strT = "���ѽ���";
                    break;
                case 0x95:
                    strT = "�������";
                    break;
            }
            return strT;
        }

        private void btnUnload_Click(object sender, EventArgs e)
        {
            if (m_bGray)
                return;
            if (!OpenUserCard() || !ReadUserCardAsn())
                return;
            decimal MoneyUnLoad = decimal.Parse(textMoney.Text, System.Globalization.NumberStyles.Number);
            double dbMoneyUnLoad = decimal.ToDouble(MoneyUnLoad);
            //Ȧ��
            string strInfo = string.Format("�Կ���{0}Ȧ��{1}Ԫ", BitConverter.ToString(m_ASN), dbMoneyUnLoad.ToString("F2"));
            OnMessageOutput(new MsgOutEvent(0, strInfo));
            if (m_UserCardCtrl.VerifyUserPin(m_strPIN) == 1)
            {
                byte[] TerminalId = new byte[6];
                if (PublicFunc.ByteDataEquals(TerminalId, m_TermialId))//δ�����ն˻���ţ�ʹ�ù̶����
                    Buffer.BlockCopy(m_FixedTermialId, 0, TerminalId, 0, 6);
                else
                    Buffer.BlockCopy(m_TermialId, 0, TerminalId, 0, 6);
                m_UserCardCtrl.UserCardUnLoad(m_ASN, TerminalId, (int)(dbMoneyUnLoad * 100.0), false);
            }
            CloseUserCard();            
        }

        private void ContactCard_CheckedChanged(object sender, EventArgs e)
        {
            m_bContactCard = ContactCard.Checked;
            //�Ӵ�ʽ�û���ֻ��ʹ��SAM��������
            if (m_bContactCard)
            {
                SamSlot.Checked = true;
                SamSlot.Enabled = false;
            }
            else
            {
                SamSlot.Checked = false;
                SamSlot.Enabled = true;
            }
        }

    }
}