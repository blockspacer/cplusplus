using System;
using System.Collections.Generic;
using System.Text;
using SqlServerHelper;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using IFuncPlugin;
using ApduParam;
using ApduInterface;
using ApduCtrl;
using System.Windows.Forms;
using System.Xml;
using CardControl;

namespace LohApduCtrl
{
    public class LohUserCardControl : LohCardCtrlBase, IUserCardControl
    {
        public event MessageOutput TextOutput = null;

        private ApduController m_ctrlApdu = null;
        private IUserApduProvider m_CmdProvider = null;
        private bool m_bContactCard = false;

        private static byte[] m_PSE = new byte[] { 0x31, 0x50, 0x41, 0x59, 0x2E, 0x53, 0x59, 0x53, 0x2E, 0x44, 0x44, 0x46, 0x30, 0x31 };//"1PAY.SYS.DDF01"
        private static byte[] m_ADF01 = new byte[] { 0x53, 0x49, 0x4E, 0x4F, 0x50, 0x45, 0x43 };//SINOPEC
        private static byte[] m_ADF02 = new byte[] { 0x4C, 0x4F, 0x59, 0x41, 0x4C, 0x54, 0x59 };//LOYALTY
        //����Ӧ��������ԿMAMK
        private static byte[] m_MAMK = new byte[] { 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11 };
        //����������ԿMPK1
        private static byte[] m_MPK = new byte[] { 0xDA, 0xC2, 0x71, 0x5F, 0x15, 0xC1, 0x40, 0x6D, 0xF3, 0x2E, 0xE6, 0x9E, 0xD4, 0xF8, 0x46, 0x2E };
        //Ȧ����ԿMLK
        private static byte[] m_MLK = new byte[] { 0x44, 0x44, 0x44, 0x44, 0x44, 0x44, 0x44, 0x44, 0x44, 0x44, 0x44, 0x44, 0x44, 0x44, 0x44, 0x44 };
        //Ȧ����ԿMULK
        private static byte[] m_MULK = new byte[] { 0x55, 0x55, 0x55, 0x55, 0x55, 0x55, 0x55, 0x55, 0x55, 0x55, 0x55, 0x55, 0x55, 0x55, 0x55, 0x55 };
        //TAC����ԿMTK
        private static byte[] m_MTK = new byte[] { 0x66, 0x66, 0x66, 0x66, 0x66, 0x66, 0x66, 0x66, 0x66, 0x66, 0x66, 0x66, 0x66, 0x66, 0x66, 0x66 };
        //PIN������ԿMPUK
        private static byte[] m_MPUK = new byte[] { 0x77, 0x77, 0x77, 0x77, 0x77, 0x77, 0x77, 0x77, 0x77, 0x77, 0x77, 0x77, 0x77, 0x77, 0x77, 0x77 };
        //������װ��ԿMRPK
        private static byte[] m_MRPK = new byte[] { 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88 };
        //͸֧�޶���ԿMUK
        private static byte[] m_MUK = new byte[] { 0x99, 0x99, 0x99, 0x99, 0x99, 0x99, 0x99, 0x99, 0x99, 0x99, 0x99, 0x99, 0x99, 0x99, 0x99, 0x99 };
        //���������Կ
        private static byte[] m_MUGK = new byte[] { 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA };
        //Ӧ��ά����ԿMAMTK
        private static byte[] m_MAMTK = new byte[] { 0x33, 0x33, 0x33, 0x33, 0x33, 0x33, 0x33, 0x33, 0x33, 0x33, 0x33, 0x33, 0x33, 0x33, 0x33, 0x33 };
        //�ڲ���֤��ԿMIAK
        private static byte[] m_MIAK = new byte[] { 0xBB, 0xBB, 0xBB, 0xBB, 0xBB, 0xBB, 0xBB, 0xBB, 0xBB, 0xBB, 0xBB, 0xBB, 0xBB, 0xBB, 0xBB, 0xBB };


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //����Ӧ��������ԿMAMK
        private static byte[] m_MAMK_Ly = new byte[] { 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11 };
        //����������ԿMPK
        private static byte[] m_MPK_Ly = new byte[] { 0xDA, 0xC2, 0x71, 0x5F, 0x15, 0xC1, 0x40, 0x6D, 0xF3, 0x2E, 0xE6, 0x9E, 0xD4, 0xF8, 0x46, 0x2E };
        //����Ȧ����ԿMLK
        private static byte[] m_MLK_Ly = new byte[] { 0x44, 0x44, 0x44, 0x44, 0x44, 0x44, 0x44, 0x44, 0x44, 0x44, 0x44, 0x44, 0x44, 0x44, 0x44, 0x44 };
        //TAC����ԿMTK
        private static byte[] m_MTK_Ly = new byte[] { 0x66, 0x66, 0x66, 0x66, 0x66, 0x66, 0x66, 0x66, 0x66, 0x66, 0x66, 0x66, 0x66, 0x66, 0x66, 0x66 };
        //PIN��������ԿMPUK
        private static byte[] m_MPUK_Ly = new byte[] { 0x77, 0x77, 0x77, 0x77, 0x77, 0x77, 0x77, 0x77, 0x77, 0x77, 0x77, 0x77, 0x77, 0x77, 0x77, 0x77 };
        //������װ����ԿMRPK
        private static byte[] m_MRPK_Ly = new byte[] { 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88 };
        //���������Կ
        private static byte[] m_MUGK_Ly = new byte[] { 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA };
        //����ά������ԿMAMTK
        private static byte[] m_MAMTK_Ly = new byte[] { 0x33, 0x33, 0x33, 0x33, 0x33, 0x33, 0x33, 0x33, 0x33, 0x33, 0x33, 0x33, 0x33, 0x33, 0x33, 0x33 };
        //�ڲ���֤��ԿMIAK
        private static byte[] m_MIAK_Ly = new byte[] { 0xBB, 0xBB, 0xBB, 0xBB, 0xBB, 0xBB, 0xBB, 0xBB, 0xBB, 0xBB, 0xBB, 0xBB, 0xBB, 0xBB, 0xBB, 0xBB };

        public LohUserCardControl(ApduController ApduCtrlObj, bool bContactCard, SqlConnectInfo DbInfo)
        {
            m_ctrlApdu = ApduCtrlObj;
            m_bContactCard = bContactCard;
            m_CmdProvider = m_ctrlApdu.GetUserApduProvider();
            m_DBInfo = DbInfo;
        }

        protected virtual void OnTextOutput(MsgOutEvent args)
        {
            Trace.WriteLine(args.Message);
            if (TextOutput != null)
                TextOutput(args);
        }

        private bool SelectFile(byte[] AIDName, byte[] prefixData)
        {
            m_CmdProvider.createSelectCmd(AIDName, prefixData);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.CmdExchange(m_bContactCard, data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
            {
                OnTextOutput(new MsgOutEvent(nRet, "ѡ��" + GetFileDescribe(AIDName) + "�ļ�ʧ��"));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                OnTextOutput(new MsgOutEvent(0, "ѡ��" + GetFileDescribe(AIDName) + "�ļ�Ӧ��" + strData));
                if (nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x61 && RecvData[nRecvLen - 1] > 0x00)
                {
                    int nGetLen = (int)RecvData[nRecvLen - 1];
                    return GetResponse(nGetLen);
                }
                else if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return false;
            }
            return true;
        }

        private bool GetResponse(int nResLen)
        {
            m_CmdProvider.createGetResponseCmd(nResLen);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.CmdExchange(m_bContactCard, data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
            {
                OnTextOutput(new MsgOutEvent(nRet, "��ȡ��������ʧ��"));
                return false;
            }
            else
            {
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return false;
            }

            OnTextOutput(new MsgOutEvent(0, "��ȡ��������Ӧ��" + m_ctrlApdu.hex2asc(RecvData, nRecvLen)));
            return true;
        }

        private byte[] GetRandomValue(int nRandomLen)
        {
            m_CmdProvider.createGetChallengeCmd(nRandomLen);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.CmdExchange(m_bContactCard, data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
            {
                OnTextOutput(new MsgOutEvent(nRet, "��ȡ���ֵʧ��"));
                return null;
            }
            else
            {
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return null;
            }
            byte[] RandomValue = new byte[nRandomLen];
            Buffer.BlockCopy(RecvData, 0, RandomValue, 0, nRandomLen);
            return RandomValue;
        }

        private bool ExternalAuthenticate(byte[] randByte, byte[] KeyVal)
        {
            m_CmdProvider.createExternalAuthenticationCmd(randByte, KeyVal);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.CmdExchange(m_bContactCard, data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
            {
                OnTextOutput(new MsgOutEvent(nRet, "�ⲿ��֤ʧ��"));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                if (nRecvLen >= 2 && (RecvData[nRecvLen - 2] == 0x6A && RecvData[nRecvLen - 1] == 0x82) || (RecvData[nRecvLen - 2] == 0x94 && RecvData[nRecvLen - 1] == 0x03))
                {
                    //�ļ�δ�ҵ�/��Կ������֧��
                    OnTextOutput(new MsgOutEvent(0, "�ⲿ��֤������"));
                }
                else if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                {
                    string strErr = GlobalControl.GetErrString(RecvData[nRecvLen - 2], RecvData[nRecvLen - 1], strData);
                    OnTextOutput(new MsgOutEvent(0, " �ⲿ��֤����" + strErr));
                    return false;
                }
                else
                {
                    OnTextOutput(new MsgOutEvent(0, "�ⲿ��֤Ӧ��" + strData));
                }
            }
            return true;
        }

        //private bool ExternalAuthentication(bool bMainKey)
        //{
        //    byte[] randByte = GetRandomValue(8);
        //    if (randByte == null || randByte.Length != 8)
        //        return false;

        //    byte[] KeyVal = GetKeyVal(bMainKey, CardCategory.CpuCard);

        //    return ExternalAuthenticate(randByte, KeyVal);
        //}

        private bool ExternalAuthWithKey(byte[] KeyVal)
        {
            byte[] randByte = GetRandomValue(8);
            if (randByte == null || randByte.Length != 8)
                return false;

            OnTextOutput(new MsgOutEvent(0, "ʹ����Կ��" + BitConverter.ToString(KeyVal) + "�����ⲿ��֤"));

            return ExternalAuthenticate(randByte, KeyVal);
        }

        private bool ClearMF(byte[] randByte, byte[] KeyVal)
        {
            m_CmdProvider.createClearMFcmd(randByte, KeyVal);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.CmdExchange(m_bContactCard, data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
            {
                OnTextOutput(new MsgOutEvent(nRet, "��ʼ��ʧ��"));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                OnTextOutput(new MsgOutEvent(0, "��ʼ��Ӧ��" + strData));
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return false;
            }
            return true;
        }

        private bool DeleteMF(bool bContact)
        {
            if (!bContact)
            {
                //�ǽӴ�ʽ������PIN�������ƿ���Ȼ��PIN��������Ƭ��������
                ClearCardFile(0x01);
                ClearCardFile(0x02);
            }
            return ClearMF(null, null);
        }

        private bool ClearCardFile(byte fileId)
        {
            m_CmdProvider.createClearCardFileCmd(fileId);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.CmdExchange(m_bContactCard, data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
            {
                OnTextOutput(new MsgOutEvent(nRet, "��տ�Ƭʧ��"));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                OnTextOutput(new MsgOutEvent(0, "��տ�ƬӦ��" + strData));
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return false;
            }
            return true;
        }

        private string GetFileDescribe(byte[] AIDName)
        {
            if (PublicFunc.ByteDataEquals(AIDName, m_PSE))
                return "MF";
            else if (PublicFunc.ByteDataEquals(AIDName, m_ADF01))
                return "3F01";
            else if (PublicFunc.ByteDataEquals(AIDName, m_ADF02))
                return "3F02";
            return "";
        }

        public int InitCard(bool bMainKey)
        {
            if (!DeleteMF(m_bContactCard))
                return 1;
            InitWhiteCard();
            return 0;
        }

        public void GetCosVer()
        {

        }

        private bool CreateFCI()
        {
            m_CmdProvider.createGenerateFCICmd();
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.CmdExchange(m_bContactCard, data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
            {
                OnTextOutput(new MsgOutEvent(nRet, "����DF�ļ�ʧ��"));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                OnTextOutput(new MsgOutEvent(0, "����DF�ļ�Ӧ��" + strData));
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return false;
            }
            return true;
        }

        public bool CreateEFInMF()
        {
            if (!SelectFile(m_PSE, null))
                return false;
            return true;
        }

        private bool UpdateDir(int nIndex, byte[] AidName)
        {
            m_CmdProvider.createUpdateEF01Cmd((byte)nIndex, AidName);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.CmdExchange(m_bContactCard, data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
            {
                string strErr = string.Format("����{0}�ļ�ʧ��", GetFileDescribe(AidName));
                OnTextOutput(new MsgOutEvent(nRet, strErr));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                string strMsg = string.Format("����{0}�ļ�Ӧ��{1}", GetFileDescribe(AidName), strData);
                OnTextOutput(new MsgOutEvent(0, strMsg));
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return false;
            }
            return true;
        }

        //����Key�ļ�
        private bool CreateKeyFile()
        {
            m_CmdProvider.createGenerateKeyCmd();
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.CmdExchange(m_bContactCard, data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
            {
                OnTextOutput(new MsgOutEvent(nRet, "����Key�ļ�ʧ��"));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                OnTextOutput(new MsgOutEvent(0, "����Key�ļ�Ӧ��" + strData));
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return false;
            }
            return true;
        }

        private bool CreateRecordFile(ushort fileID, byte fileType, byte RecordNum, byte RecordLen, ushort ACr, ushort ACw)
        {
            m_CmdProvider.createGenerateEFCmd(fileID, fileType, 0, 0, RecordNum, RecordLen, ACr, ACw);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.CmdExchange(m_bContactCard, data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
            {
                string strMessage = string.Format("����{0}��¼�ļ�ʧ��", fileID.ToString("X4"));
                OnTextOutput(new MsgOutEvent(nRet, strMessage));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                string strMessage = string.Format("����{0}��¼�ļ�Ӧ��{1}", fileID.ToString("X4"), strData);
                OnTextOutput(new MsgOutEvent(0, strMessage));
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return false;
            }
            return true;
        }

        private bool CreateEFFile(ushort fileID, byte fileType, ushort fileLen, byte keyIndex, ushort ACr, ushort ACw)
        {
            m_CmdProvider.createGenerateEFCmd(fileID, fileType, fileLen, keyIndex, 0, 0, ACr, ACw);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.CmdExchange(m_bContactCard, data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
            {
                string strMessage = string.Format("����{0}�ļ�ʧ��", fileID.ToString("X4"));
                OnTextOutput(new MsgOutEvent(nRet, strMessage));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                string strMessage = string.Format("����{0}�ļ�Ӧ��{1}", fileID.ToString("X4"), strData);
                OnTextOutput(new MsgOutEvent(0, strMessage));
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return false;
            }
            return true;
        }

        private void CreateDF()
        {
            if (!CreateFCI())
                return;
            UpdateDir(1, m_ADF01);
            UpdateDir(2, m_ADF02);
        }

        //��װ��Կ
        public void CreateKey()
        {
            if (!CreateKeyFile())
                return;
            StorageMasterKey(m_KeyMain);
            StorageMaintainKey(m_KeyMaintain);

            CreateDF();
        }

        public bool CreateADFApp(int nAppIndex)
        {
            if (!SelectFile(m_PSE, null))
                return false;
            return true;
        }

        public bool CreateApplication(byte[] byteASN, bool bDefaultPwd, string strCustomPwd)
        {
            byte[] prefix = new byte[] { 0xA0, 0x00, 0x00, 0x00, 0x03 };
            if (!SelectFile(m_ADF01, prefix))
                return false;
            //��Ʊ������Կ
            if (!CreateEFKeyFile())
                return false;

            StorageEncryptyKey(byteASN);
            StoragePINFile(bDefaultPwd, strCustomPwd);//PIN��װ

            //����Ӧ�û��������ļ�
            if (!CreateEFFile(0x0015, 0xA8, 0x001E, 0, 0xF0F0, 0xFFFF))
                return false;
            //�ֿ��˻��������ļ�
            if (!CreateEFFile(0x0016, 0xA8, 0x0029, 0, 0xF0F0, 0xFFFF))
                return false;
            //��ƱӦ����ͨ��Ϣ�����ļ�
            if (!CreateEFFile(0x001B, 0x28, 0x0027, 0, 0xF0F0, 0xFFFF))
                return false;
            //��ƱӦ��������Ϣ�����ļ�
            if (!CreateEFFile(0x001C, 0xA8, 0x0060, 0, 0xF0F0, 0xFFFF))
                return false;
            //��Ʊר�������ļ�
            if (!CreateEFFile(0x000D, 0xA8, 0x0040, 0, 0xF0F0, 0xFFFF))
                return false;
            //������ϸ�ļ�EF18 ѭ����¼�ļ�
            if (!CreateRecordFile(0x0018, 0x2E, 0x0B, 0x17, 0xF1EF, 0xFFFF))
                return false;
            return true;
        }

        private bool CreateEFKeyFile()
        {
            m_CmdProvider.createGenerateEFKeyCmd();
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.CmdExchange(m_bContactCard, data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
            {
                OnTextOutput(new MsgOutEvent(nRet, "����������Կʧ��"));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                OnTextOutput(new MsgOutEvent(0, "����������ԿӦ��" + strData));
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return false;
            }
            return true;
        }

        //��װ������Կ���û���������Կ�������η�ɢ����������һ�η�ɢ
        //�û�����PSAM��������Կ��Ӧ�����PSAM��ʹ��������Կ����MAC1ʱ�����η�ɢ
        private bool StorageEncryptyKey(byte[] byteASN)
        {
            StorageKeyParam KeyInfo = null;
            //����Ӧ��������ԿMCMK
            KeyInfo = new StorageKeyParam("��װӦ��������Կ", 0x00, 0xF9, 0xAA, 0x0A, 0x33);
            KeyInfo.SetParam(byteASN, m_MAMK, m_KeyMain);
            if (!storageUserKey(KeyInfo))
                return false;

            //����������ԿMPK
            byte[] LeftDiversify = DesCryptography.TripleEncryptData(KeyInfo.ASN, m_MPK);
            byte[] RightDiversify = DesCryptography.TripleEncryptData(KeyInfo.XorASN, m_MPK);
            byte[] TempMPK = new byte[16];           //����������Կ��ɢ
            Buffer.BlockCopy(LeftDiversify, 0, TempMPK, 0, 8);
            Buffer.BlockCopy(RightDiversify, 0, TempMPK, 8, 8);
            KeyInfo = new StorageKeyParam("��װ����������Կ", 0x01, 0xFE, 0xAA, 0x01, 0x00);
            KeyInfo.SetParam(byteASN, TempMPK, m_KeyMain);
            if (!storageUserKey(KeyInfo))
                return false;
            //Ȧ����ԿMLK
            KeyInfo = new StorageKeyParam("��װȦ����Կ", 0x01, 0xFF, 0xAA, 0x01, 0x00);
            KeyInfo.SetParam(byteASN, m_MLK, m_KeyMain);
            if (!storageUserKey(KeyInfo))
                return false;
            //Ȧ����ԿMULK
            KeyInfo = new StorageKeyParam("��װȦ����Կ", 0x01, 0xFD, 0xAA, 0x01, 0x00);
            KeyInfo.SetParam(byteASN, m_MULK, m_KeyMain);
            if (!storageUserKey(KeyInfo))
                return false;
            //TAC��ԿMTK
            KeyInfo = new StorageKeyParam("��װTAC��Կ", 0x00, 0xF4, 0xAA, 0x01, 0x00);
            KeyInfo.SetParam(byteASN, m_MTK, m_KeyMain);
            if (!storageUserKey(KeyInfo))
                return false;
            //PIN������ԿMPUK
            KeyInfo = new StorageKeyParam("��װPIN������Կ", 0x00, 0xF7, 0xAA, 0xFF, 0x33);
            KeyInfo.SetParam(byteASN, m_MPUK, m_KeyMain);
            if (!storageUserKey(KeyInfo))
                return false;
            //������װ��ԿMRPK
            KeyInfo = new StorageKeyParam("��װ������װ��Կ", 0x00, 0xF8, 0xAA, 0xFF, 0x33);
            KeyInfo.SetParam(byteASN, m_MRPK, m_KeyMain);
            if (!storageUserKey(KeyInfo))
                return false;
            //͸֧�޶���ԿMUK
            KeyInfo = new StorageKeyParam("��װ͸֧�޶���Կ", 0x01, 0xFC, 0xAA, 0x01, 0x00);
            KeyInfo.SetParam(byteASN, m_MUK, m_KeyMain);
            if (!storageUserKey(KeyInfo))
                return false;
            //���������ԿMUGK
            KeyInfo = new StorageKeyParam("��װ���������Կ", 0x01, 0xD9, 0xAA, 0x01, 0x00);
            KeyInfo.SetParam(byteASN, m_MUGK, m_KeyMain);
            if (!storageUserKey(KeyInfo))
                return false;
            //Ӧ��ά����ԿMAMK
            KeyInfo = new StorageKeyParam("��װӦ��ά����Կ", 0x00, 0xF6, 0xAA, 0xFF, 0x33);
            KeyInfo.SetParam(byteASN, m_MAMTK, m_KeyMain);
            if (!storageUserKey(KeyInfo))
                return false;
            //�ڲ���֤��ԿMIAK
            KeyInfo = new StorageKeyParam("��װ�ڲ���֤��Կ", 0x01, 0xF9, 0xAA, 0x0A, 0x33);//ͬ������Կ
            KeyInfo.SetParam(byteASN, m_MIAK, m_KeyMain);
            if (!storageUserKey(KeyInfo))
                return false;
            return true;
        }

        private bool StoragePINFile(bool bDefaultPwd, string strCustomPwd)
        {
            byte[] PwdData = new byte[6];
            if (strCustomPwd.Length == 6)
            {
                for (int i = 0; i < 6; i++)
                    PwdData[i] = Convert.ToByte(strCustomPwd.Substring(i, 1), 10);
            }
            else
            {
                for (int i = 0; i < 6; i++)
                    PwdData[i] = 0x09;
            }
            m_CmdProvider.createStoragePINFileCmd(bDefaultPwd, PwdData);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.CmdExchange(m_bContactCard, data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
            {
                OnTextOutput(new MsgOutEvent(nRet, "��װPIN�ļ�ʧ��"));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                OnTextOutput(new MsgOutEvent(0, "��װPIN�ļ�Ӧ��" + strData));
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return false;
            }
            return true;
        }

        private bool storageUserKey(StorageKeyParam Param)
        {
            m_CmdProvider.createWriteUserKeyCmd(null, Param);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.CmdExchange(m_bContactCard, data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
            {
                OnTextOutput(new MsgOutEvent(nRet, Param.PromptInfo + "ʧ��"));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                OnTextOutput(new MsgOutEvent(0, Param.PromptInfo + "Ӧ��" + strData));
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return false;
            }
            return true;
        }

        private bool UpdateEF15File(byte[] key, byte[] ASN, DateTime dateBegin, DateTime dateEnd)
        {
            byte[] randomVal = GetRandomValue(8);
            if (randomVal == null || randomVal.Length != 8)
                return false;
            m_CmdProvider.createUpdateEF15FileCmd(key, randomVal, ASN, dateBegin, dateEnd);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.CmdExchange(m_bContactCard, data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
            {
                OnTextOutput(new MsgOutEvent(nRet, "���¹���Ӧ�û��������ļ�EF15ʧ��"));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                OnTextOutput(new MsgOutEvent(0, "���¹���Ӧ�û��������ļ�EF15Ӧ��" + strData));
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return false;
            }
            return true;
        }

        private bool UpdateEF16File(byte[] key, UserCardInfoParam cardInfo)
        {
            byte[] randomVal = GetRandomValue(8);
            if (randomVal == null || randomVal.Length != 8)
                return false;
            m_CmdProvider.createUpdateEF16FileCmd(key, randomVal, cardInfo);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.CmdExchange(m_bContactCard, data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
            {
                OnTextOutput(new MsgOutEvent(nRet, "���³ֿ��˻��������ļ�EF16ʧ��"));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                OnTextOutput(new MsgOutEvent(0, "���³ֿ��˻��������ļ�EF16Ӧ��" + strData));
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return false;
            }
            return true;
        }

        private int VerifyPIN(bool bDefaultPwd, string strCustomPwd)
        {
            byte[] PwdData = new byte[6];
            if (!bDefaultPwd && strCustomPwd.Length == 6)
            {
                for (int i = 0; i < 6; i++)
                    PwdData[i] = Convert.ToByte(strCustomPwd.Substring(i, 1), 10);
            }
            else
            {
                for (int i = 0; i < 6; i++)
                    PwdData[i] = 0x09;
            }
            m_CmdProvider.createVerifyPINCmd(bDefaultPwd, PwdData);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.CmdExchange(m_bContactCard, data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
            {
                OnTextOutput(new MsgOutEvent(nRet, "��֤PINʧ��"));
                return 0;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                OnTextOutput(new MsgOutEvent(0, "��֤PINӦ��" + strData));
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                {
                    if (nRecvLen == 2 && RecvData[nRecvLen - 2] == 0x69 && RecvData[nRecvLen - 1] == 0x83)
                        return 2;//PIN����
                    else
                        return 0;
                }
            }
            return 1;
        }

        private bool UpdateEF0BFile(bool bDefaultPwd)
        {
            m_CmdProvider.createUpdateEF0BFileCmd(bDefaultPwd);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.CmdExchange(m_bContactCard, data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
            {
                OnTextOutput(new MsgOutEvent(nRet, "������ͨ��Ϣ�����ļ�EF1Bʧ��"));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                OnTextOutput(new MsgOutEvent(0, "������ͨ��Ϣ�����ļ�EF1BӦ��" + strData));
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return false;
            }
            return true;
        }

        private bool UpdateEF1CFile(byte[] key, UserCardInfoParam cardInfo)
        {
            byte[] randomVal = GetRandomValue(8);
            if (randomVal == null || randomVal.Length != 8)
                return false;
            m_CmdProvider.createUpdateEF1CFileCmd(key, randomVal, cardInfo);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.CmdExchange(m_bContactCard, data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
            {
                OnTextOutput(new MsgOutEvent(nRet, "����������Ϣ�����ļ�EF1Cʧ��"));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                OnTextOutput(new MsgOutEvent(0, "����������Ϣ�����ļ�EF1CӦ��" + strData));
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return false;
            }
            return true;
        }

        private bool UpdateEF0DFile(byte[] key, UserCardInfoParam cardInfo)
        {
            byte[] randomVal = GetRandomValue(8);
            if (randomVal == null || randomVal.Length != 8)
                return false;
            m_CmdProvider.createUpdateEF0DFileCmd(key, randomVal, cardInfo);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.CmdExchange(m_bContactCard, data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
            {
                OnTextOutput(new MsgOutEvent(nRet, "������Ʊר�������ļ�EF0Dʧ��"));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                OnTextOutput(new MsgOutEvent(0, "������Ʊר�������ļ�EF0DӦ��" + strData));
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return false;
            }
            return true;
        }

        //���¼���Ӧ���ļ�
        //���ļ�ʱ����Ӧ���ļ�����Ҫ��Կ��Ҳ����ҪMAC
        public bool UpdateApplicationFile(UserCardInfoParam UserCardInfoPar, byte[] AppTendingKey)
        {
            //ѡ��ADF01
            byte[] prefix = new byte[] { 0xA0, 0x00, 0x00, 0x00, 0x03 };
            if (!SelectFile(m_ADF01, prefix))
                return false;
            byte[] byteCardId = UserCardInfoPar.GetUserCardID();
            byte[] keyUpdate = null;
            if (AppTendingKey != null)
            {
                //ά����Կ�����һ�η�ɢ
                keyUpdate = StorageKeyParam.GetUpdateEFKey(AppTendingKey, byteCardId);
                if (keyUpdate == null)
                    return false;
            }
            //���¹���Ӧ�û��������ļ�EF15
            if (!UpdateEF15File(keyUpdate, byteCardId, UserCardInfoPar.ValidCardBegin, UserCardInfoPar.ValidCardEnd))
                return false;
            //���³ֿ��˻��������ļ�EF16
            if (!UpdateEF16File(keyUpdate, UserCardInfoPar))
                return false;
            //������ͨ��Ϣ�����ļ�EF0B
            if (!UpdateEF0BFile(UserCardInfoPar.DefaultPwdFlag))
                return false;
            //������Ϣ�ļ�EF1C
            if (!UpdateEF1CFile(keyUpdate, UserCardInfoPar))
                return false;
            //��Ʊר���ļ�EF0D
            if (!UpdateEF0DFile(keyUpdate, UserCardInfoPar))
                return false;
            return true;
        }

        private bool InitializeForLoad(int nMoney, byte[] TermId, byte[] outData, BalanceType eType)
        {
            m_CmdProvider.createInitializeLoadCmd(nMoney, TermId, eType);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.CmdExchange(m_bContactCard, data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
            {
                OnTextOutput(new MsgOutEvent(nRet, "Ȧ���ʼ��ʧ��"));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                OnTextOutput(new MsgOutEvent(0, "Ȧ���ʼ��Ӧ��" + strData));
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return false;
                Buffer.BlockCopy(RecvData, 0, outData, 0, 16);
            }
            return true;
        }

        private bool InitializeForUnLoad(int nMoney, byte[] TermId, byte[] outData)
        {
            m_CmdProvider.createInitializeUnLoadCmd(nMoney, TermId);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.CmdExchange(m_bContactCard, data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
            {
                OnTextOutput(new MsgOutEvent(nRet, "Ȧ���ʼ��ʧ��"));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                OnTextOutput(new MsgOutEvent(0, "Ȧ���ʼ��Ӧ��" + strData));
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return false;
                Buffer.BlockCopy(RecvData, 0, outData, 0, 16);
            }
            return true;
        }

        //����MAC2
        private byte[] CalcMAC2(byte BusinessType, int nMoneyValue, byte[] TermialID, byte[] TimeBcd, byte[] byteKey)
        {
            byte[] srcData = new byte[18];
            byte[] byteMoney = BitConverter.GetBytes(nMoneyValue);
            srcData[0] = byteMoney[3];
            srcData[1] = byteMoney[2];
            srcData[2] = byteMoney[1];
            srcData[3] = byteMoney[0];
            srcData[4] = BusinessType;
            Buffer.BlockCopy(TermialID, 0, srcData, 5, 6);
            Buffer.BlockCopy(TimeBcd, 0, srcData, 11, 7);
            byte[] MAC2 = m_CmdProvider.CalcMacVal_DES(srcData, byteKey);
            return MAC2;
        }

        private bool CreditForLoad(byte[] byteMAC2, byte[] TimeBcd)
        {
            m_CmdProvider.createCreditLoadCmd(byteMAC2, TimeBcd);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.CmdExchange(m_bContactCard, data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
            {
                OnTextOutput(new MsgOutEvent(nRet, "Ȧ�潻��ʧ��"));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                OnTextOutput(new MsgOutEvent(0, "Ȧ�潻��Ӧ��" + strData));
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return false;
                OnTextOutput(new MsgOutEvent(0, "Ȧ��TAC:" + BitConverter.ToString(RecvData, 0, 4)));//ǰ4�ֽ�ΪTAC
            }
            return true;
        }

        public bool DebitFoUnLoad(byte[] byteMAC2, byte[] TimeBcd)
        {
            m_CmdProvider.createDebitUnLoadCmd(byteMAC2, TimeBcd);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.CmdExchange(m_bContactCard, data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
            {
                OnTextOutput(new MsgOutEvent(nRet, "Ȧ�ύ��ʧ��"));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                OnTextOutput(new MsgOutEvent(0, "Ȧ�ύ��Ӧ��" + strData));
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return false;
                OnTextOutput(new MsgOutEvent(0, "Ȧ��MAC3:" + BitConverter.ToString(RecvData, 0, 4)));//ǰ4�ֽ�ΪMAC
            }
            return true;
        }

        public bool SelectCardApp(int nAppIndex)
        {
            if (!SelectFile(m_PSE, null))
                return false;
            byte[] prefix = new byte[] { 0xA0, 0x00, 0x00, 0x00, 0x03 };

            byte[] Adf = null;
            if (nAppIndex == 1)
                Adf = m_ADF01;
            else if (nAppIndex == 2)
                Adf = m_ADF02;
            if (!SelectFile(Adf, prefix))
                return false;
            return true;
        }

        public int VerifyUserPin(string strPIN)
        {
            return VerifyPIN(false, strPIN);
        }

        public bool LoyaltyLoad(byte[] ASN, byte[] TermId, int nLoyaltyValue, bool bReadKey)
        {
            //��ȡ�ѷ�����Ȧ����Կ
            if (bReadKey)
            {
                byte[] keyLoad = GetApplicationKeyVal(ASN, "AppLoadKey", 2);
                if (keyLoad == null)
                {
                    MessageBox.Show("�޻���Ȧ����Կ������Ȧ����֡�");
                    return false;
                }
                Buffer.BlockCopy(keyLoad, 0, m_MLK_Ly, 0, 16);
            }
            const byte BusinessType = 0x02; //�������ͱ�ʶ��Ȧ�����0x01 Ȧ��Ǯ��0x02
            byte[] outData = new byte[16];
            byte[] SysTime = PublicFunc.GetBCDTime();
            if (!InitializeForLoad(nLoyaltyValue, TermId, outData, BalanceType.Balance_EP))
                return false;
            byte[] byteBalance = new byte[4];
            Buffer.BlockCopy(outData, 0, byteBalance, 0, 4);
            byte[] OnlineSn = new byte[2];//�������
            Buffer.BlockCopy(outData, 4, OnlineSn, 0, 2);
            byte keyVer = (byte)outData[6];
            byte keyFlag = (byte)outData[7];
            byte[] rand = new byte[4];
            Buffer.BlockCopy(outData, 8, rand, 0, 4);
            byte[] MAC1 = new byte[4];
            Buffer.BlockCopy(outData, 12, MAC1, 0, 4);
            //�ж�MAC1�Ƿ���ȷ
            byte[] seslk = GlobalControl.GetProcessKey(ASN, m_MLK_Ly, rand, OnlineSn);//m_MLK_LyȦ������Կ1��DLK��
            if (seslk == null)
                return false;
            byte[] srcData = new byte[15];//���ڼ���MAC1��ԭʼ����
            Buffer.BlockCopy(byteBalance, 0, srcData, 0, 4);
            byte[] byteMoney = BitConverter.GetBytes(nLoyaltyValue);
            srcData[4] = byteMoney[3];
            srcData[5] = byteMoney[2];
            srcData[6] = byteMoney[1];
            srcData[7] = byteMoney[0];
            srcData[8] = BusinessType;
            Buffer.BlockCopy(TermId, 0, srcData, 9, 6);
            byte[] MAC1Compare = m_CmdProvider.CalcMacVal_DES(srcData, seslk);
            if (!PublicFunc.ByteDataEquals(MAC1, MAC1Compare))//MAC1���
            {
                string strInfo = string.Format("Ȧ����� Output MAC: {0} PC Calc MAC: {1}", BitConverter.ToString(MAC1), BitConverter.ToString(MAC1Compare));
                System.Diagnostics.Trace.WriteLine(strInfo);
                return false;
            }
            byte[] MAC2 = CalcMAC2(BusinessType, nLoyaltyValue, TermId, SysTime, seslk);
            CreditForLoad(MAC2, SysTime);
            return true;
        }

        //Ȧ�湦��
        public bool UserCardLoad(byte[] ASN, byte[] TermId, int nMoneyValue, bool bReadKey)
        {
            //��ȡ�ѷ�����Ȧ����Կ
            if (bReadKey)
            {
                byte[] keyLoad = GetApplicationKeyVal(ASN, "AppLoadKey", 1);
                if (keyLoad == null)
                {
                    MessageBox.Show("��Ȧ����Կ������Ȧ�档");
                    return false;
                }
                Buffer.BlockCopy(keyLoad, 0, m_MLK, 0, 16);
            }
            const byte BusinessType = 0x01; //�������ͱ�ʶ��Ȧ�����0x01 Ȧ��Ǯ��0x02
            byte[] outData = new byte[16];
            byte[] SysTime = PublicFunc.GetBCDTime();
            if (!InitializeForLoad(nMoneyValue, TermId, outData, BalanceType.Balance_ED))
                return false;
            byte[] byteBalance = new byte[4];
            Buffer.BlockCopy(outData, 0, byteBalance, 0, 4);
            byte[] OnlineSn = new byte[2];//�������
            Buffer.BlockCopy(outData, 4, OnlineSn, 0, 2);
            byte keyVer = (byte)outData[6];
            byte keyFlag = (byte)outData[7];
            byte[] rand = new byte[4];
            Buffer.BlockCopy(outData, 8, rand, 0, 4);
            byte[] MAC1 = new byte[4];
            Buffer.BlockCopy(outData, 12, MAC1, 0, 4);
            //�ж�MAC1�Ƿ���ȷ
            byte[] seslk = GlobalControl.GetProcessKey(ASN, m_MLK, rand, OnlineSn);//m_MLK1Ȧ������Կ1��DLK��
            if (seslk == null)
                return false;
            byte[] srcData = new byte[15];//���ڼ���MAC1��ԭʼ����
            Buffer.BlockCopy(byteBalance, 0, srcData, 0, 4);
            byte[] byteMoney = BitConverter.GetBytes(nMoneyValue);
            srcData[4] = byteMoney[3];
            srcData[5] = byteMoney[2];
            srcData[6] = byteMoney[1];
            srcData[7] = byteMoney[0];
            srcData[8] = BusinessType;
            Buffer.BlockCopy(TermId, 0, srcData, 9, 6);
            byte[] MAC1Compare = m_CmdProvider.CalcMacVal_DES(srcData, seslk);
            if (!PublicFunc.ByteDataEquals(MAC1, MAC1Compare))//MAC1���
            {
                string strInfo = string.Format("Ȧ�湦�� Output MAC: {0} PC Calc MAC: {1}", BitConverter.ToString(MAC1), BitConverter.ToString(MAC1Compare));
                System.Diagnostics.Trace.WriteLine(strInfo);
                return false;
            }
            byte[] MAC2 = CalcMAC2(BusinessType, nMoneyValue, TermId, SysTime, seslk);
            CreditForLoad(MAC2, SysTime);
            return true;
        }

        //Ȧ��
        public bool UserCardUnLoad(byte[] ASN, byte[] TermId, int nMoneyValue, bool bReadKey)
        {
            if (bReadKey)
            {
                byte[] keyUnLoad = GetApplicationKeyVal(ASN, "AppUnLoadKey", 1);//Ȧ����Կ
                if (keyUnLoad == null)
                {
                    MessageBox.Show("��Ȧ����Կ������Ȧ�ᡣ");
                    return false;
                }
                Buffer.BlockCopy(keyUnLoad, 0, m_MULK, 0, 16);
            }
            const byte BusinessType = 0x03;//Ȧ��
            byte[] outData = new byte[16];
            byte[] SysTime = PublicFunc.GetBCDTime();
            if (!InitializeForUnLoad(nMoneyValue, TermId, outData))
                return false;
            byte[] byteBalance = new byte[4];
            Buffer.BlockCopy(outData, 0, byteBalance, 0, 4);
            byte[] OnlineSn = new byte[2];//�������
            Buffer.BlockCopy(outData, 4, OnlineSn, 0, 2);
            byte keyVer = (byte)outData[6];
            byte keyFlag = (byte)outData[7];
            byte[] rand = new byte[4];
            Buffer.BlockCopy(outData, 8, rand, 0, 4);
            byte[] MAC1 = new byte[4];
            Buffer.BlockCopy(outData, 12, MAC1, 0, 4);
            //�ж�MAC1�Ƿ���ȷ
            byte[] sesulk = GlobalControl.GetProcessKey(ASN, m_MULK, rand, OnlineSn);//m_MULK Ȧ����Կ����Կ
            if (sesulk == null)
                return false;
            byte[] srcData = new byte[15];//���ڼ���MAC1��ԭʼ����
            Buffer.BlockCopy(byteBalance, 0, srcData, 0, 4);
            byte[] byteMoney = BitConverter.GetBytes(nMoneyValue);
            srcData[4] = byteMoney[3];
            srcData[5] = byteMoney[2];
            srcData[6] = byteMoney[1];
            srcData[7] = byteMoney[0];
            srcData[8] = BusinessType;
            Buffer.BlockCopy(TermId, 0, srcData, 9, 6);
            byte[] MAC1Compare = m_CmdProvider.CalcMacVal_DES(srcData, sesulk);
            if (!PublicFunc.ByteDataEquals(MAC1, MAC1Compare))//MAC1���
            {
                string strInfo = string.Format("Ȧ�Ṧ�� Output MAC: {0} PC Calc MAC: {1}", BitConverter.ToString(MAC1), BitConverter.ToString(MAC1Compare));
                System.Diagnostics.Trace.WriteLine(strInfo);
                return false;
            }
            byte[] MAC2 = CalcMAC2(BusinessType, nMoneyValue, TermId, SysTime, sesulk);
            DebitFoUnLoad(MAC2, SysTime);
            return true;
        }


        public bool UserCardBalance(ref int nBalance, BalanceType eType)
        {
            m_CmdProvider.createCardBalanceCmd(eType);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.CmdExchange(m_bContactCard, data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
            {
                OnTextOutput(new MsgOutEvent(nRet, "��ȡ���ʧ��"));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                OnTextOutput(new MsgOutEvent(0, "��ȡ���Ӧ��" + strData));
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return false;
                byte[] byteBalance = new byte[4];
                byteBalance[0] = RecvData[3];
                byteBalance[1] = RecvData[2];
                byteBalance[2] = RecvData[1];
                byteBalance[3] = RecvData[0];
                nBalance = BitConverter.ToInt32(byteBalance, 0);
            }
            return true;
        }

        public bool UserCardGray(ref int nStatus, byte[] PSAM_TID, byte[] GTAC)
        {
            m_CmdProvider.createrCardGrayCmd(false);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.CmdExchange(m_bContactCard, data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
            {
                OnTextOutput(new MsgOutEvent(nRet, "��ȡ����״̬ʧ��"));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                OnTextOutput(new MsgOutEvent(0, "��ȡ����״̬Ӧ��" + strData));
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return false;
                if (RecvData[0] == 0x10)
                    nStatus = 2;
                else if (RecvData[0] == 0x01)
                    nStatus = 1;
                else
                    nStatus = 0;
                //δ����ʱ�ն˻���š�GTAC��Ϊ0
                Buffer.BlockCopy(RecvData, 9, PSAM_TID, 0, 6);
                //GTAC
                Buffer.BlockCopy(RecvData, 26, GTAC, 0, 4);
            }
            return true;
        }



        public bool InitForGray(byte[] TermialID, byte[] outData)
        {
            m_CmdProvider.createrInitForGrayCmd(TermialID);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.CmdExchange(m_bContactCard, data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
            {
                OnTextOutput(new MsgOutEvent(nRet, "������ʼ��ʧ��"));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                OnTextOutput(new MsgOutEvent(0, "������ʼ��Ӧ��" + strData));
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return false;
                Buffer.BlockCopy(RecvData, 0, outData, 0, 15);
            }
            return true;
        }

        //����Ӧ�����ѣ�ֻ���ڵ���Ǯ�� EP
        public bool InitForPurchase(byte[] TerminalID, int nLyAmount, byte[] outData)
        {
            m_CmdProvider.createrInitForPurchaseCmd(TerminalID, nLyAmount);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.CmdExchange(m_bContactCard, data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
            {
                OnTextOutput(new MsgOutEvent(nRet, "�������ѳ�ʼ��ʧ��"));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                OnTextOutput(new MsgOutEvent(0, "�������ѳ�ʼ��Ӧ��" + strData));
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return false;
                Buffer.BlockCopy(RecvData, 0, outData, 0, 15);
            }
            return true;
        }

        //��������
        public bool GrayLock(byte[] Data, byte[] outGTAC, byte[] outMAC2)
        {
            m_CmdProvider.createrGrayLockCmd(Data);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.CmdExchange(m_bContactCard, data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
            {
                OnTextOutput(new MsgOutEvent(nRet, "����ʧ��"));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                OnTextOutput(new MsgOutEvent(0, "����Ӧ��" + strData));
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return false;
                Buffer.BlockCopy(RecvData, 0, outGTAC, 0, 4);
                Buffer.BlockCopy(RecvData, 4, outMAC2, 0, 4);
            }
            return true;
        }

        //��������
        public bool LyPurchase(byte[] Data, byte[] outTAC, byte[] outMAC2)
        {
            m_CmdProvider.createrLyPurchaseCmd(Data);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.CmdExchange(m_bContactCard, data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
            {
                OnTextOutput(new MsgOutEvent(nRet, "��������ʧ��"));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                OnTextOutput(new MsgOutEvent(0, "��������Ӧ��" + strData));
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return false;
                Buffer.BlockCopy(RecvData, 0, outTAC, 0, 4);
                Buffer.BlockCopy(RecvData, 4, outMAC2, 0, 4);
            }
            return true;
        }

        //������۳�ʼ��
        public bool InitForUnlockGreyCard(byte[] TermialID, byte[] outData)
        {
            m_CmdProvider.createrInitForUnlockCardCmd(TermialID);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.CmdExchange(m_bContactCard, data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
            {
                OnTextOutput(new MsgOutEvent(nRet, "������۳�ʼ��ʧ��"));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                OnTextOutput(new MsgOutEvent(0, "������۳�ʼ��Ӧ��" + strData));//0
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return false;
                Buffer.BlockCopy(RecvData, 0, outData, 0, 18);
            }
            return true;
        }

        //�������
        public bool UnLockGrayCard(byte[] ASN, byte[] TermialID, int nUnlockMoney, bool bReadKey, int nAppIndex)
        {
            byte[] byteMUGK = new byte[16];
            if (nAppIndex == 1)
                Buffer.BlockCopy(m_MUGK, 0, byteMUGK, 0, 16);
            else if (nAppIndex == 2)
                Buffer.BlockCopy(m_MUGK_Ly, 0, byteMUGK, 0, 16);
            else
                return false;
            //��ȡ�ѷ�����Ȧ����Կ,����ʱ�����ݿ�洢��Ĭ����Կ
            if (bReadKey)
            {
                byte[] keyUnlockGray = GetApplicationKeyVal(ASN, "AppUnGrayKey", nAppIndex);
                if (keyUnlockGray == null)
                {
                    MessageBox.Show("�޽����Կ�����ܽ�ҡ�");
                    return false;
                }
                Buffer.BlockCopy(keyUnlockGray, 0, byteMUGK, 0, 16);
            }
            const byte BusinessType = 0x95; //�������ͱ�ʶ��������0��
            byte[] outData = new byte[18];
            if (!InitForUnlockGreyCard(TermialID, outData))
                return false;
            byte[] SysTime = PublicFunc.GetBCDTime();
            byte[] byteBalance = new byte[4];
            Buffer.BlockCopy(outData, 0, byteBalance, 0, 4);
            byte[] OfflineSn = new byte[2];//�ѻ��������
            Buffer.BlockCopy(outData, 4, OfflineSn, 0, 2);
            byte[] OnlineSn = new byte[2];//�����������
            Buffer.BlockCopy(outData, 6, OnlineSn, 0, 2);
            byte keyVer = outData[8];
            byte keyFlag = outData[9];
            byte[] rand = new byte[4];
            Buffer.BlockCopy(outData, 10, rand, 0, 4);
            byte[] MAC1 = new byte[4];
            Buffer.BlockCopy(outData, 14, MAC1, 0, 4);
            //�ж�MAC1�Ƿ���ȷ
            byte[] sesukk = GlobalControl.GetProcessKey(ASN, byteMUGK, rand, OnlineSn);//���������Կ
            if (sesukk == null)
                return false;
            byte[] srcData = new byte[13];//���ڼ���MAC1��ԭʼ����
            Buffer.BlockCopy(byteBalance, 0, srcData, 0, 4);
            Buffer.BlockCopy(OfflineSn, 0, srcData, 4, 2);
            srcData[6] = BusinessType;
            Buffer.BlockCopy(TermialID, 0, srcData, 7, 6);
            byte[] MAC1Compare = m_CmdProvider.CalcMacVal_DES(srcData, sesukk);
            if (!PublicFunc.ByteDataEquals(MAC1, MAC1Compare))//MAC1���
            {
                string strInfo = string.Format("������� Output MAC: {0} PC Calc MAC: {1}", BitConverter.ToString(MAC1), BitConverter.ToString(MAC1Compare));
                System.Diagnostics.Trace.WriteLine(strInfo);
                return false;
            }
            byte[] MAC2 = CalcMAC2(BusinessType, nUnlockMoney, TermialID, SysTime, sesukk);
            UnLockForGreyCard(nUnlockMoney, MAC2, SysTime);
            return true;
        }

        private bool UnLockForGreyCard(int nUnlockMoney, byte[] byteMAC2, byte[] TimeBcd)
        {
            m_CmdProvider.createGreyCardUnLockCmd(nUnlockMoney, byteMAC2, TimeBcd);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.CmdExchange(m_bContactCard, data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
            {
                OnTextOutput(new MsgOutEvent(nRet, "�������ʧ��"));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                OnTextOutput(new MsgOutEvent(0, "�������Ӧ��" + strData));//��ʾ0������Ϣ
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return false;
                OnTextOutput(new MsgOutEvent(0, "�������MAC3:" + BitConverter.ToString(RecvData, 0, 4)));
            }
            return true;
        }

        public bool DebitForUnlock(byte[] byteData)
        {
            m_CmdProvider.createDebitForUnlockCmd(byteData);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.CmdExchange(m_bContactCard, data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
            {
                OnTextOutput(new MsgOutEvent(nRet, "�����ʧ��"));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                OnTextOutput(new MsgOutEvent(0, "�����Ӧ��" + strData));//��ʾ0������Ϣ
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return false;
                OnTextOutput(new MsgOutEvent(0, "���TAC:" + BitConverter.ToString(RecvData, 0, 4)));
            }
            return true;
        }

        public bool ClearTACUF()
        {
            m_CmdProvider.createrCardGrayCmd(true);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.CmdExchange(m_bContactCard, data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
            {
                OnTextOutput(new MsgOutEvent(nRet, "���TACUFʧ��"));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                OnTextOutput(new MsgOutEvent(0, "���TACUFӦ��" + strData));
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return false;
            }
            return true;
        }

        private bool ReadKeyFromDb()
        {
            SqlHelper ObjSql = new SqlHelper();
            if (!ObjSql.OpenSqlServerConnection(m_DBInfo.strServerName, m_DBInfo.strDbName, m_DBInfo.strUser, m_DBInfo.strUserPwd))
            {
                ObjSql = null;
                return false;
            }

            if (!GetDbUserKeyValue(ObjSql, 1))
            {
                ObjSql.CloseConnection();
                ObjSql = null;
                return false;
            }
            bool bCompare = false;
            if (GetDbUserKeyValue(ObjSql, 2))
                bCompare = true;

            byte[] ConsumerKey = GlobalControl.GetDbConsumerKey(ObjSql, "PROC_GetPsamKey", "ConsumerMasterKey", 0);
            if (ConsumerKey == null || !PublicFunc.ByteDataEquals(ConsumerKey, m_MPK) || (bCompare && !PublicFunc.ByteDataEquals(ConsumerKey, m_MPK_Ly)))
            {
                OnTextOutput(new MsgOutEvent(0, "��Ƭ������Կ��һ��"));
                MessageBox.Show("���������������Ҫ������Կһ�£�����ǰʹ�õ�������Կ��һ�¡�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            ObjSql.CloseConnection();
            ObjSql = null;
            return true;
        }

        public int ReadKeyValueFromSource()
        {
            int nRet = 0;
            if (m_ctrlApdu.m_CardKeyFrom == CardKeySource.CardKeyFromXml)
            {
                if (!ReadKeyFromXml())
                    nRet = 2;
            }
            else
            {
                if (!ReadKeyFromDb())
                    nRet = 1;
            }
            return nRet;
        }

        //���ò��ϵ���Կû�ж���
        private bool GetDbUserKeyValue(SqlHelper sqlHelp, int nAppIndex)
        {
            CpuKeyData KeyVal = new CpuKeyData();
            KeyVal.nAppIndex = nAppIndex;
            if (!GlobalControl.GetDbCpuKeyVal(sqlHelp, KeyVal))
                return false;
            SetMainKeyValue(KeyVal.MasterKeyVal, CardCategory.CpuCard);//��Ƭ������Կ
            SetMaintainKeyValue(KeyVal.MasterTendingKeyVal, CardCategory.CpuCard);  //��Ƭά����Կ   
            if (nAppIndex == 1)
            {
                Buffer.BlockCopy(KeyVal.AppMasterKey, 0, m_MAMK, 0, 16);
                Buffer.BlockCopy(KeyVal.AppTendingKey, 0, m_MAMTK, 0, 16);
                Buffer.BlockCopy(KeyVal.AppInternalAuthKey, 0, m_MIAK, 0, 16);
                Buffer.BlockCopy(KeyVal.AppPinResetKey, 0, m_MRPK, 0, 16);
                Buffer.BlockCopy(KeyVal.AppPinUnlockKey, 0, m_MPUK, 0, 16);
                Buffer.BlockCopy(KeyVal.AppConsumerKey, 0, m_MPK, 0, 16);
                Buffer.BlockCopy(KeyVal.AppLoadKey, 0, m_MLK, 0, 16);
                Buffer.BlockCopy(KeyVal.AppTacKey, 0, m_MTK, 0, 16);
                Buffer.BlockCopy(KeyVal.AppUnGrayKey, 0, m_MUGK, 0, 16);
                Buffer.BlockCopy(KeyVal.AppUnLoadKey, 0, m_MULK, 0, 16);
                Buffer.BlockCopy(KeyVal.AppOverdraftKey, 0, m_MUK, 0, 16);
            }
            else if (nAppIndex == 2)
            {
                Buffer.BlockCopy(KeyVal.AppMasterKey, 0, m_MAMK_Ly, 0, 16);
                Buffer.BlockCopy(KeyVal.AppTendingKey, 0, m_MAMTK_Ly, 0, 16);
                Buffer.BlockCopy(KeyVal.AppInternalAuthKey, 0, m_MIAK_Ly, 0, 16);
                Buffer.BlockCopy(KeyVal.AppPinResetKey, 0, m_MRPK_Ly, 0, 16);
                Buffer.BlockCopy(KeyVal.AppPinUnlockKey, 0, m_MPUK_Ly, 0, 16);
                Buffer.BlockCopy(KeyVal.AppConsumerKey, 0, m_MPK_Ly, 0, 16);
                Buffer.BlockCopy(KeyVal.AppLoadKey, 0, m_MLK_Ly, 0, 16);
                Buffer.BlockCopy(KeyVal.AppTacKey, 0, m_MTK_Ly, 0, 16);
                Buffer.BlockCopy(KeyVal.AppUnGrayKey, 0, m_MUGK_Ly, 0, 16);
            }
            return true;
        }

        private void GetSqlParam(SqlHelper ObjSql, SqlParameter[] sqlparams, UserCardInfoParam UserCardInfoPar)
        {
            string strDbVal = null;

            strDbVal = BitConverter.ToString(UserCardInfoPar.GetUserCardID()).Replace("-", "");
            sqlparams[0] = ObjSql.MakeParam("CardId", SqlDbType.Char, 16, ParameterDirection.Input, strDbVal);
            byte nCardType = (byte)UserCardInfoPar.UserCardType;
            sqlparams[1] = ObjSql.MakeParam("CardType", SqlDbType.VarChar, 2, ParameterDirection.Input, nCardType.ToString("X2"));
            sqlparams[2] = ObjSql.MakeParam("ClientId", SqlDbType.Int, 4, ParameterDirection.Input, UserCardInfoPar.ClientID);
            sqlparams[3] = ObjSql.MakeParam("UseValidateDate", SqlDbType.DateTime, 8, ParameterDirection.Input, UserCardInfoPar.ValidCardBegin);
            sqlparams[4] = ObjSql.MakeParam("UseInvalidateDate", SqlDbType.DateTime, 8, ParameterDirection.Input, UserCardInfoPar.ValidCardEnd);

            sqlparams[5] = ObjSql.MakeParam("Plate", SqlDbType.NVarChar, 16, ParameterDirection.Input, UserCardInfoPar.CarNo);
            sqlparams[6] = ObjSql.MakeParam("SelfId", SqlDbType.VarChar, 50, ParameterDirection.Input, UserCardInfoPar.SelfId);
            byte nCertificatesType = (byte)UserCardInfoPar.IdType;
            sqlparams[7] = ObjSql.MakeParam("CertificatesType", SqlDbType.VarChar, 2, ParameterDirection.Input, nCertificatesType.ToString("X2"));//֤������:���֤
            sqlparams[8] = ObjSql.MakeParam("PersonalId", SqlDbType.VarChar, 32, ParameterDirection.Input, UserCardInfoPar.UserIdentity);
            sqlparams[9] = ObjSql.MakeParam("DriverName", SqlDbType.NVarChar, 50, ParameterDirection.Input, UserCardInfoPar.UserName);
            sqlparams[10] = ObjSql.MakeParam("DriverTel", SqlDbType.VarChar, 32, ParameterDirection.Input, UserCardInfoPar.TelePhone);
            sqlparams[11] = ObjSql.MakeParam("VechileCategory", SqlDbType.NVarChar, 8, ParameterDirection.Input, UserCardInfoPar.CarType);

            sqlparams[12] = ObjSql.MakeParam("SteelCylinderId", SqlDbType.VarChar, 32, ParameterDirection.Input, UserCardInfoPar.BoalId);
            sqlparams[13] = ObjSql.MakeParam("CylinderTestDate", SqlDbType.DateTime, 8, ParameterDirection.Input, UserCardInfoPar.BoalExprie);
            sqlparams[14] = ObjSql.MakeParam("Remark", SqlDbType.NVarChar, 50, ParameterDirection.Input, UserCardInfoPar.Remark);

            if (UserCardInfoPar.LimitGasFillCount > 0)
            {
                sqlparams[15] = ObjSql.MakeParam("R_OilTimesADay", SqlDbType.Int, 4, ParameterDirection.Input, UserCardInfoPar.LimitGasFillCount);//
                sqlparams[16] = ObjSql.MakeParam("R_OilVolATime", SqlDbType.Decimal, 16, ParameterDirection.Input, UserCardInfoPar.LimitGasFillAmount / UserCardInfoPar.LimitGasFillCount);//
            }
            else
            {
                sqlparams[15] = ObjSql.MakeParam("R_OilTimesADay", SqlDbType.Int, 4, ParameterDirection.Input, 0);//
                sqlparams[16] = ObjSql.MakeParam("R_OilVolATime", SqlDbType.Decimal, 16, ParameterDirection.Input, 0);//
            }
            sqlparams[17] = ObjSql.MakeParam("R_OilVolTotal", SqlDbType.Decimal, 16, ParameterDirection.Input, UserCardInfoPar.LimitGasFillAmount);//
            sqlparams[18] = ObjSql.MakeParam("R_OilEndDate", SqlDbType.DateTime, 8, ParameterDirection.Input, UserCardInfoPar.ValidCardEnd);//
            sqlparams[19] = ObjSql.MakeParam("R_Plate", SqlDbType.Bit, 1, ParameterDirection.Input, UserCardInfoPar.LimitCarNo);//
            sqlparams[20] = ObjSql.MakeParam("R_Oil", SqlDbType.VarChar, 4, ParameterDirection.Input, UserCardInfoPar.LimitGasType.ToString("X4"));//
            sqlparams[21] = ObjSql.MakeParam("R_RFID", SqlDbType.Bit, 1, ParameterDirection.Input, false);//
            sqlparams[22] = ObjSql.MakeParam("CylinderNum", SqlDbType.Int, 4, ParameterDirection.Input, UserCardInfoPar.CylinderNum);//
            sqlparams[23] = ObjSql.MakeParam("FactoryNum", SqlDbType.Char, 7, ParameterDirection.Input, UserCardInfoPar.BoalFactoryID);//
            sqlparams[24] = ObjSql.MakeParam("CylinderVolume", SqlDbType.Int, 4, ParameterDirection.Input, UserCardInfoPar.CylinderVolume);//
            sqlparams[25] = ObjSql.MakeParam("BusDistance", SqlDbType.VarChar, 10, ParameterDirection.Input, UserCardInfoPar.BusDistance);//
        }

        public bool SaveCpuCardInfoToDb(UserCardInfoParam UserCardInfoPar, bool bUpdate)
        {
            bool bSuccess = false;
            SqlHelper ObjSql = new SqlHelper();
            if (!ObjSql.OpenSqlServerConnection(m_DBInfo.strServerName, m_DBInfo.strDbName, m_DBInfo.strUser, m_DBInfo.strUserPwd))
            {
                ObjSql = null;
                return false;
            }
            SqlParameter[] sqlparams = new SqlParameter[28];
            GetSqlParam(ObjSql, sqlparams, UserCardInfoPar);

            Guid CardGuid = Guid.NewGuid();
            sqlparams[26] = ObjSql.MakeParam("UserKeyGuid", SqlDbType.UniqueIdentifier, 16, ParameterDirection.Input, CardGuid);//
            byte[] MotherCard = UserCardInfoPar.GetRelatedMotherCardID();
            if (UserCardInfoPar.UserCardType == CardType.CompanySubCard && MotherCard != null)
            {
                string strVal = BitConverter.ToString(MotherCard).Replace("-", "");
                sqlparams[27] = ObjSql.MakeParam("RelatedMotherCard", SqlDbType.Char, 16, ParameterDirection.Input, strVal);//
            }
            else
            {
                sqlparams[27] = ObjSql.MakeParam("RelatedMotherCard", SqlDbType.Char, 16, ParameterDirection.Input, "");//
            }
            if (ObjSql.ExecuteProc("PROC_PublishCpuCard", sqlparams) == 0)
            {
                SaveCpuCardKey(ObjSql, CardGuid, UserCardInfoPar.GetUserCardID());
                bSuccess = true;
            }
            ObjSql.CloseConnection();
            ObjSql = null;
            return bSuccess;
        }

        public bool UpdateCardInfoToDb(UserCardInfoParam UserCardInfoPar)
        {
            bool bSuccess = false;
            SqlHelper ObjSql = new SqlHelper();
            if (!ObjSql.OpenSqlServerConnection(m_DBInfo.strServerName, m_DBInfo.strDbName, m_DBInfo.strUser, m_DBInfo.strUserPwd))
            {
                ObjSql = null;
                return false;
            }
            SqlParameter[] sqlparams = new SqlParameter[27];
            GetSqlParam(ObjSql, sqlparams, UserCardInfoPar);

            byte[] MotherCard = UserCardInfoPar.GetRelatedMotherCardID();
            if (UserCardInfoPar.UserCardType == CardType.CompanySubCard && MotherCard != null)
            {
                string strVal = BitConverter.ToString(MotherCard).Replace("-", "");
                sqlparams[26] = ObjSql.MakeParam("RelatedMotherCard", SqlDbType.Char, 16, ParameterDirection.Input, strVal);//
            }
            else
            {
                sqlparams[26] = ObjSql.MakeParam("RelatedMotherCard", SqlDbType.Char, 16, ParameterDirection.Input, "");//
            }
            if (ObjSql.ExecuteProc("PROC_RewriteCpuCard", sqlparams) == 0)
                bSuccess = true;
            ObjSql.CloseConnection();
            ObjSql = null;
            return bSuccess;
        }

        public void GetUserCardInfo(UserCardInfoParam CardInfo)
        {
            DateTime cardStart = DateTime.MinValue;
            DateTime cardEnd = DateTime.MinValue;
            byte[] byteAsn = GetUserCardASN(false, ref cardStart, ref cardEnd);
            if (byteAsn == null)
                return;
            CardInfo.CardOrderNo = BitConverter.ToString(byteAsn, 5, 3).Replace("-", "");
            CardInfo.UserCardType = (CardType)byteAsn[3];
            Trace.Assert(byteAsn[2] == 0x02);
            CardInfo.SetCardId(BitConverter.ToString(byteAsn, 0, 2).Replace("-", ""));
            CardInfo.ValidCardBegin = cardStart;
            CardInfo.ValidCardEnd = cardEnd;

            GetBaseInfo(CardInfo);
            GetLimitInfo(CardInfo);
            GetCylinderInfo(CardInfo);

        }

        /// <summary>
        /// ԭʼ���޿��ţ�������ݿ��¼ʱ����ʾ����
        /// </summary>
        public byte[] GetUserCardASN(bool bMessage, ref DateTime cardStart, ref DateTime cardEnd)
        {
            m_CmdProvider.createGetEFFileCmd(0x95, 0x1C);//����Ӧ�û�������(100+10101)0x15�ļ�����0x1C
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.CmdExchange(m_bContactCard, data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
            {
                if (bMessage)
                    OnTextOutput(new MsgOutEvent(nRet, "��ȡ����ʧ��"));
                return null;
            }
            else
            {
                Trace.WriteLine(string.Format("��ȡEF15�ļ�Ӧ��:" + m_ctrlApdu.hex2asc(RecvData, nRecvLen)));
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return null;
                byte[] UserCardASN = new byte[8];
                try
                {
                    Buffer.BlockCopy(RecvData, 12, UserCardASN, 0, 8);//��徿�����10�ֽ�����ǰ���ֽ�
                    string strAsn = BitConverter.ToString(UserCardASN).Replace("-", "");
                    if (bMessage)
                        OnTextOutput(new MsgOutEvent(0, "��ȡ�����ţ�" + strAsn));
                    for (int i = 0; i < strAsn.Length; i++)
                    {
                        if (!Char.IsDigit(strAsn[i]))
                            return null;
                    }
                    cardStart = DateTime.ParseExact(BitConverter.ToString(RecvData, 20, 4).Replace("-", ""), "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                    cardEnd = DateTime.ParseExact(BitConverter.ToString(RecvData, 24, 4).Replace("-", ""), "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                }
                catch
                {

                }
                return UserCardASN;
            }
        }

        private void GetBaseInfo(UserCardInfoParam CardInfo)
        {
            m_CmdProvider.createGetEFFileCmd(0x96, 0x29);//��������(100+10110)0x16�ļ�����41
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.CmdExchange(m_bContactCard, data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
                return;
            Trace.WriteLine(string.Format("��ȡEF16�ļ�Ӧ��:" + m_ctrlApdu.hex2asc(RecvData, nRecvLen)));
            if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                return;
            try
            {
                Trace.Assert(CardInfo.UserCardType == (CardType)RecvData[0]);
                int nCount = 0;
                for (int i = 0; i < 20; i++)
                {
                    if (RecvData[2 + i] != 0xFF)
                        nCount++;
                    else
                        break;
                }
                if (nCount > 0)
                    CardInfo.UserName = Encoding.Unicode.GetString(RecvData, 2, nCount);
                string strIdentity = BitConverter.ToString(RecvData, 22, 9).Replace("-", "");
                CardInfo.UserIdentity = strIdentity.Replace('F', 'X');
                CardInfo.IdType = (UserCardInfoParam.IdentityType)(RecvData[31]);//֤������
                string strValue = BitConverter.ToString(RecvData, 32, 2).Replace("-", "");
                int nDiscountRate = Convert.ToInt32(strValue);
                DateTime RateExprieValid = DateTime.ParseExact(BitConverter.ToString(RecvData, 34, 4).Replace("-", ""), "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                CardInfo.setDiscountRate(nDiscountRate * 1.0 / 100.0, RateExprieValid);
                CardInfo.PriceLevel = RecvData[38];  //�۸�ȼ�
            }
            catch
            {

            }
        }

        private void GetLimitInfo(UserCardInfoParam CardInfo)
        {
            m_CmdProvider.createGetEFFileCmd(0x9C, 0x60);//��������(100+11100)0x01C�ļ�����96
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.CmdExchange(m_bContactCard, data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
                return;
            Trace.WriteLine(string.Format("��ȡEF1C�ļ�Ӧ��:" + m_ctrlApdu.hex2asc(RecvData, nRecvLen)));
            if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                return;
            try
            {
                CardInfo.LimitGasType = (ushort)((RecvData[0] << 8) + RecvData[1]);
                CardInfo.setLimitArea(RecvData[2], BitConverter.ToString(RecvData, 3, 40).Replace("-", ""));
                int nCount = 0;
                for (int i = 0; i < 16; i++)
                {
                    if (RecvData[43 + i] != 0xFF)
                        nCount++;
                    else
                        break;
                }
                if (nCount > 0)
                {
                    CardInfo.LimitCarNo = true;
                    CardInfo.CarNo = Encoding.Unicode.GetString(RecvData, 43, nCount);
                }
                else
                {
                    CardInfo.LimitCarNo = false;
                    CardInfo.CarNo = "";
                }
                CardInfo.LimitGasFillCount = RecvData[63];
                CardInfo.LimitGasFillAmount = (uint)((RecvData[64] << 24) + (RecvData[65] << 16) + (RecvData[66] << 8) + RecvData[67]);
            }
            catch
            {

            }
        }

        private void GetCylinderInfo(UserCardInfoParam CardInfo)
        {
            m_CmdProvider.createGetEFFileCmd(0x8D, 0x40);//��������(100+01101)0x0D�ļ�����64
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.CmdExchange(m_bContactCard, data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
                return;
            if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                return;
            try
            {
                CardInfo.BoalExprie = DateTime.ParseExact(BitConverter.ToString(RecvData, 0, 4).Replace("-", ""), "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                int nCarNoCount = 0;
                for (int i = 0; i < 16; i++)
                {
                    if (RecvData[4 + i] != 0xFF)
                        nCarNoCount++;
                    else
                        break;
                }
                if (nCarNoCount > 0)
                {
                    CardInfo.CarNo = Encoding.Unicode.GetString(RecvData, 4, nCarNoCount); //װ����ƿ�ĳ��ƺ�
                }
                int nCount = 0;
                for (int i = 0; i < 16; i++)
                {
                    if (RecvData[20 + i] != 0xFF)
                        nCount++;
                    else
                        break;
                }
                if (nCount > 0)
                    CardInfo.BoalId = Encoding.ASCII.GetString(RecvData, 20, nCount);
                CardInfo.CylinderNum = (int)RecvData[36];
                nCount = 0;
                for (int i = 0; i < 7; i++)
                {
                    if (RecvData[37 + i] != 0xFF)
                        nCount++;
                    else
                        break;
                }
                if (nCount > 0)
                    CardInfo.BoalFactoryID = Encoding.ASCII.GetString(RecvData, 37, nCount);
                CardInfo.CylinderVolume = (ushort)((RecvData[45] << 8) + RecvData[44]);
                CardInfo.CarType = GetCarCateGorybyByte(RecvData[46]);
                nCount = 0;
                for (int i = 0; i < 5; i++)
                {
                    if (RecvData[47 + i] != 0xFF)
                        nCount++;
                    else
                        break;
                }
                if (nCount > 0)
                    CardInfo.BusDistance = Encoding.ASCII.GetString(RecvData, 47, nCount);
            }
            catch
            {

            }
        }

        private string GetCarCateGorybyByte(byte carType)
        {
            string strRet = "����";
            switch (carType)
            {
                case 0xFF:
                    strRet = "����";
                    break;
                case 0x01:
                    strRet = "˽�ҳ�";
                    break;
                case 0x02:
                    strRet = "��λ��";
                    break;
                case 0x03:
                    strRet = "���⳵";
                    break;
                case 0x04:
                    strRet = "������";
                    break;
            }
            return strRet;
        }

        private bool ReadSingleRecord(int nRecordId, out CardRecord record)
        {
            record = null;
            m_CmdProvider.createReadRecordCmd(0x17, nRecordId);//��ȡ1��������¼
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.CmdExchange(m_bContactCard, data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
            {
                OnTextOutput(new MsgOutEvent(nRet, "��ȡ������¼ʧ��"));
                return false;
            }
            string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
            OnTextOutput(new MsgOutEvent(0, "��ȡ������¼Ӧ��" + strData));
            if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                return false;

            const int LenPerRecord = 23;
            int nCount = (nRecvLen - 2) / LenPerRecord;
            if (nCount == 1)
            {
                record = new CardRecord();
                record.BusinessSn = (RecvData[0] << 8) + RecvData[1];
                record.OverdraftMoney = ((RecvData[2] << 16) + (RecvData[3] << 8) + RecvData[4]) / 100.0f;
                record.Amount = ((RecvData[5] << 24) + (RecvData[6] << 16) + (RecvData[7] << 8) + RecvData[8]) / 100.0f;
                record.BusinessType = RecvData[9];
                record.TerminalID = BitConverter.ToString(RecvData, 10, 6).Replace("-", "");
                record.BusinessTime = BitConverter.ToString(RecvData, 16, 7).Replace("-", "");
            }
            return true;
        }

        //����Ƭ�еļ�����¼
        public List<CardRecord> ReadRecord()
        {
            List<CardRecord> lstRet = new List<CardRecord>();
            CardRecord newRecord = null;
            for (int i = 0; i < 10; i++)
            {
                bool bRet = ReadSingleRecord(i + 1, out newRecord);//�����10����¼
                if (!bRet)
                    break;
                lstRet.Add(newRecord);
            }
            return lstRet;

        }

        //������ݿ����Ƿ��иÿ��ķ�����¼,��徿�����
        public bool CheckPublishedCard(bool bMainKey, byte[] KeyInit)
        {
            if (!SelectCardApp(1))//�û�����Ҫ��Ӧ�ú���ܻ�ȡ����
                return false;
            DateTime cardStart = DateTime.MinValue;
            DateTime cardEnd = DateTime.MinValue;
            byte[] CardAsn = GetUserCardASN(false, ref cardStart, ref cardEnd);
            if (CardAsn == null || CardAsn.Length != 8)
                return false;
            SqlHelper ObjSql = new SqlHelper();
            if (!ObjSql.OpenSqlServerConnection(m_DBInfo.strServerName, m_DBInfo.strDbName, m_DBInfo.strUser, m_DBInfo.strUserPwd))
            {
                ObjSql = null;
                return false;
            }
            string strDbAsn = BitConverter.ToString(CardAsn).Replace("-", "");

            SqlDataReader dataReader = null;
            SqlParameter[] sqlparam = new SqlParameter[2];
            sqlparam[0] = ObjSql.MakeParam("CardNum", SqlDbType.Char, 16, ParameterDirection.Input, strDbAsn);
            sqlparam[1] = ObjSql.MakeParam("ApplicationIndex", SqlDbType.Int, 4, ParameterDirection.Input, 1);
            ObjSql.ExecuteProc("PROC_GetPublishedCard", sqlparam, out dataReader);
            bool bRet = false;
            if (dataReader != null)
            {
                if (!dataReader.HasRows)
                    dataReader.Close();
                else
                {
                    if (dataReader.Read())
                    {
                        bRet = true;
                    }
                    dataReader.Close();
                }
            }
            ObjSql.CloseConnection();
            ObjSql = null;
            return bRet;
        }

        public bool UpdateCardInfo(UserCardInfoParam CardInfo)
        {
            if (!SelectFile(m_PSE, null))
                return false;
            byte[] ExternalAuthKey = GetApplicationKeyVal(CardInfo.GetUserCardID(), "MasterKey", 1);
            byte[] AppTendingKey = GetApplicationKeyVal(CardInfo.GetUserCardID(), "AppTendingKey", 1);
            if (AppTendingKey == null || ExternalAuthKey == null)
            {
                MessageBox.Show("��������Կ��ά����Կ�������޸Ŀ���Ϣ��");
                return false;
            }
            if (!ExternalAuthWithKey(ExternalAuthKey))
                return false;
            return UpdateApplicationFile(CardInfo, AppTendingKey);
        }

        private byte[] GetApplicationKeyVal(byte[] CardId, string strKeyName, int nAppIndex)
        {
            if (m_ctrlApdu.m_CardKeyFrom == CardKeySource.CardKeyFromXml)
            {
                string strName = string.Format("UserKeyValue_App{0}", nAppIndex);
                return GlobalControl.GetPrivateKeyFromXml(m_ctrlApdu.m_strCardKeyPath, strName, strKeyName);
            }
            else
            {
                string strKeyVal = GlobalControl.GetPublishedCardInfoFormDb(m_DBInfo, CardId, strKeyName, nAppIndex);
                return PublicFunc.StringToBCD(strKeyVal);
            }
        }

        public bool ChangePIN(string strOldPin, string strNewPin)
        {
            byte[] OldPwdData = new byte[6];
            if (strOldPin.Length == 6)
            {
                for (int i = 0; i < 6; i++)
                    OldPwdData[i] = Convert.ToByte(strOldPin.Substring(i, 1), 10);
            }
            else
            {
                return false;
            }
            byte[] NewPwdData = new byte[6];
            if (strNewPin.Length == 6)
            {
                for (int i = 0; i < 6; i++)
                    NewPwdData[i] = Convert.ToByte(strNewPin.Substring(i, 1), 10);
            }
            else
            {
                return false;
            }

            m_CmdProvider.createChangePINCmd(OldPwdData, NewPwdData);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.CmdExchange(m_bContactCard, data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
            {
                OnTextOutput(new MsgOutEvent(nRet, "�޸�PIN��ʧ��"));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                OnTextOutput(new MsgOutEvent(0, "�޸�PIN��Ӧ��" + strData));
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return false;
            }
            return true;
        }

        public bool PINReset(byte[] ASN, string strPin, int nAppIndex)
        {
            if (ASN.Length != 8 || strPin.Length != 6)
                return false;

            byte[] PwdData = new byte[6];
            for (int i = 0; i < 6; i++)
                PwdData[i] = Convert.ToByte(strPin.Substring(i, 1), 10);
            //��ȡPIN��װ��Կ
            byte[] keyReset = GetApplicationKeyVal(ASN, "AppPinResetKey", nAppIndex);
            if (keyReset == null)
            {
                MessageBox.Show("��PIN��װ��Կ��������װPIN��");
                return false;
            }
            Buffer.BlockCopy(keyReset, 0, m_MRPK, 0, 16);

            byte[] SubKey = new byte[16];
            byte[] LeftDiversify = DesCryptography.TripleEncryptData(ASN, m_MRPK);
            byte[] XorASN = new byte[8];
            for (int i = 0; i < 8; i++)
                XorASN[i] = (byte)(ASN[i] ^ 0xFF);
            byte[] RightDiversify = DesCryptography.TripleEncryptData(XorASN, m_MRPK);
            Buffer.BlockCopy(LeftDiversify, 0, SubKey, 0, 8);
            Buffer.BlockCopy(RightDiversify, 0, SubKey, 8, 8);
            //������
            m_CmdProvider.createPINResetCmd(SubKey, PwdData);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.CmdExchange(m_bContactCard, data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
            {
                OnTextOutput(new MsgOutEvent(nRet, "��װPIN��ʧ��"));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                OnTextOutput(new MsgOutEvent(0, "��װPIN��Ӧ��" + strData));//0
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return false;
            }
            return true;
        }

        public bool PINUnLock(byte[] ASN, string strPIN, int nAppIndex)
        {
            if (ASN.Length != 8 || strPIN.Length != 6)
                return false;
            byte[] randomVal = GetRandomValue(4);
            if (randomVal == null || randomVal.Length != 4)
                return false;

            byte[] MacInit = new byte[8];
            Buffer.BlockCopy(randomVal, 0, MacInit, 0, 4);//���ֽ����ֵ�����ֽڣ�

            byte[] PwdData = new byte[6];
            for (int i = 0; i < 6; i++)
                PwdData[i] = Convert.ToByte(strPIN.Substring(i, 1), 10);
            //��ȡPIN������Կ
            byte[] keyUnlock = GetApplicationKeyVal(ASN, "AppPinUnlockKey", nAppIndex);
            if (keyUnlock == null)
            {
                MessageBox.Show("��PIN������Կ�����ܽ���PIN��");
                return false;
            }
            Buffer.BlockCopy(keyUnlock, 0, m_MPUK, 0, 16);

            byte[] SubKey = new byte[16];
            byte[] LeftDiversify = DesCryptography.TripleEncryptData(ASN, m_MPUK);
            byte[] XorASN = new byte[8];
            for (int i = 0; i < 8; i++)
                XorASN[i] = (byte)(ASN[i] ^ 0xFF);
            byte[] RightDiversify = DesCryptography.TripleEncryptData(XorASN, m_MPUK);
            Buffer.BlockCopy(LeftDiversify, 0, SubKey, 0, 8);
            Buffer.BlockCopy(RightDiversify, 0, SubKey, 8, 8);
            //������
            m_CmdProvider.createPINUnLockCmd(MacInit, SubKey, PwdData);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.CmdExchange(m_bContactCard, data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
            {
                OnTextOutput(new MsgOutEvent(nRet, "����PIN��ʧ��"));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                OnTextOutput(new MsgOutEvent(0, "����PIN��Ӧ��" + strData));//0
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return false;
            }
            return true;

        }

        private bool CreateMF()
        {
            m_CmdProvider.createNewMFcmd(m_PSE);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.CmdExchange(m_bContactCard, data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
            {
                OnTextOutput(new MsgOutEvent(nRet, "����" + GetFileDescribe(m_PSE) + "�ļ�ʧ��"));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                OnTextOutput(new MsgOutEvent(0, "����" + GetFileDescribe(m_PSE) + "�ļ�Ӧ��" + strData));
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return false;
            }
            return true;
        }

        private bool ClearDF()
        {
            m_CmdProvider.createClearDFcmd();
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.CmdExchange(m_bContactCard, data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
            {
                OnTextOutput(new MsgOutEvent(nRet, "����DF�µ��ļ�ʧ��"));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                OnTextOutput(new MsgOutEvent(0, "����DF�µ��ļ�Ӧ��" + strData));
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return false;
            }
            return true;
        }

        private bool StorageMasterKey(byte[] MasterKey)
        {
            byte[] p1p2 = new byte[] { 0x01, 0x00 };
            byte[] KeyParam = new byte[] { 0xF9, 0xF0, 0xAA, 0x0A, 0x33 };
            m_CmdProvider.createStorageKeyCmd(MasterKey, p1p2, KeyParam);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.CmdExchange(m_bContactCard, data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
            {
                OnTextOutput(new MsgOutEvent(nRet, "��װKey�ļ�ʧ��"));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                OnTextOutput(new MsgOutEvent(0, "��װKey�ļ�Ӧ��" + strData));
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return false;
            }
            return true;
        }

        private bool StorageMaintainKey(byte[] MaintainKey)
        {
            byte[] p1p2 = new byte[] { 0x01, 0x00 };
            byte[] KeyParam = new byte[] { 0xF6, 0xF0, 0xAA, 0xFF, 0x33 };
            m_CmdProvider.createStorageKeyCmd(MaintainKey, p1p2, KeyParam);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.CmdExchange(m_bContactCard, data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
            {
                OnTextOutput(new MsgOutEvent(nRet, "��װKey�ļ�ʧ��"));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                OnTextOutput(new MsgOutEvent(0, "��װKey�ļ�Ӧ��" + strData));
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return false;
            }
            return true;
        }

        private void InitWhiteCard()
        {
            CreateMF();
            //ClearDF(); //MF����գ�����Ҫ�ٲ���DF���ļ�           
        }

        public bool CreateLoyaltyApp(byte[] byteASN, bool bDefaultPwd, string strCustomPwd)
        {
            byte[] prefix = new byte[] { 0xA0, 0x00, 0x00, 0x00, 0x03 };
            if (!SelectFile(m_ADF02, prefix))
                return false;
            //��Ʊ������Կ
            if (!CreateEFKeyFile())
                return false;

            StorageLoyaltyKey(byteASN);
            StoragePINFile(bDefaultPwd, strCustomPwd);//PIN��װ

            //����Ӧ�û��������ļ�
            if (!CreateEFFile(0x0015, 0xA8, 0x001E, 0, 0xF0F0, 0xFFFF))
                return false;
            //�ֿ��˻��������ļ�
            if (!CreateEFFile(0x0016, 0xA8, 0x0029, 0, 0xF0F0, 0xFFFF))
                return false;
            //��ƱӦ����ͨ��Ϣ�����ļ�
            if (!CreateEFFile(0x001B, 0x28, 0x0027, 0, 0xF0F0, 0xFFFF))
                return false;
            //������ϸ�ļ�EF18 ѭ����¼�ļ�
            if (!CreateRecordFile(0x0018, 0x2E, 0x0B, 0x17, 0xF1EF, 0xFFFF))
                return false;
            return true;
        }

        public bool UpdateLoyaltyApp(UserCardInfoParam UserCardInfoPar, byte[] AppTendingKey)
        {
            //ѡ��ADF01
            byte[] prefix = new byte[] { 0xA0, 0x00, 0x00, 0x00, 0x03 };
            if (!SelectFile(m_ADF02, prefix))
                return false;
            byte[] byteCardId = UserCardInfoPar.GetUserCardID();
            byte[] keyUpdate = null;
            if (AppTendingKey != null)
            {
                //ά����Կ�����һ�η�ɢ
                keyUpdate = StorageKeyParam.GetUpdateEFKey(AppTendingKey, byteCardId);
                if (keyUpdate == null)
                    return false;
            }
            //���¹���Ӧ�û��������ļ�EF15
            if (!UpdateEF15File(keyUpdate, byteCardId, UserCardInfoPar.ValidCardBegin, UserCardInfoPar.ValidCardEnd))
                return false;
            //���³ֿ��˻��������ļ�EF16
            if (!UpdateEF16File(keyUpdate, UserCardInfoPar))
                return false;
            //������ͨ��Ϣ�����ļ�EF0B
            if (!UpdateEF0BFile(UserCardInfoPar.DefaultPwdFlag))
                return false;
            return true;
        }

        //��װ����Ӧ����Կ
        private bool StorageLoyaltyKey(byte[] byteASN)
        {
            StorageKeyParam KeyInfo = null;
            //����Ӧ��������ԿMCMK
            KeyInfo = new StorageKeyParam("��װ����������Կ", 0x00, 0xF9, 0xAA, 0x0A, 0x33);
            KeyInfo.SetParam(byteASN, m_MAMK_Ly, m_KeyMain);
            if (!storageUserKey(KeyInfo))
                return false;

            //����������ԿMPK
            byte[] LeftDiversify = DesCryptography.TripleEncryptData(KeyInfo.ASN, m_MPK_Ly);
            byte[] RightDiversify = DesCryptography.TripleEncryptData(KeyInfo.XorASN, m_MPK_Ly);
            byte[] TempMPK_Ly = new byte[16];           //����������Կ��ɢ
            Buffer.BlockCopy(LeftDiversify, 0, TempMPK_Ly, 0, 8);
            Buffer.BlockCopy(RightDiversify, 0, TempMPK_Ly, 8, 8);
            KeyInfo = new StorageKeyParam("��װ����������Կ", 0x01, 0xFE, 0xAA, 0x01, 0x00);
            KeyInfo.SetParam(byteASN, TempMPK_Ly, m_KeyMain);
            if (!storageUserKey(KeyInfo))
                return false;
            //Ȧ����ԿMLK
            KeyInfo = new StorageKeyParam("��װ����Ȧ����Կ", 0x01, 0xFF, 0xAA, 0x01, 0x00);
            KeyInfo.SetParam(byteASN, m_MLK_Ly, m_KeyMain);
            if (!storageUserKey(KeyInfo))
                return false;
            //TAC��ԿMTK
            KeyInfo = new StorageKeyParam("��װ����TAC��Կ", 0x00, 0xF4, 0xAA, 0x01, 0x00);
            KeyInfo.SetParam(byteASN, m_MTK_Ly, m_KeyMain);
            if (!storageUserKey(KeyInfo))
                return false;
            //PIN������ԿMPUK
            KeyInfo = new StorageKeyParam("��װ����PIN������Կ", 0x00, 0xF7, 0xAA, 0xFF, 0x33);
            KeyInfo.SetParam(byteASN, m_MPUK_Ly, m_KeyMain);
            if (!storageUserKey(KeyInfo))
                return false;
            //������װ��ԿMRPK
            KeyInfo = new StorageKeyParam("��װ����PIN��װ��Կ", 0x00, 0xF8, 0xAA, 0xFF, 0x33);
            KeyInfo.SetParam(byteASN, m_MRPK_Ly, m_KeyMain);
            if (!storageUserKey(KeyInfo))
                return false;
            //���������ԿMUGK
            KeyInfo = new StorageKeyParam("��װ�������������Կ", 0x01, 0xD9, 0xAA, 0x01, 0x00);
            KeyInfo.SetParam(byteASN, m_MUGK_Ly, m_KeyMain);
            if (!storageUserKey(KeyInfo))
                return false;
            //Ӧ��ά����ԿMAMK
            KeyInfo = new StorageKeyParam("��װ����Ӧ��ά����Կ", 0x00, 0xF6, 0xAA, 0xFF, 0x33);
            KeyInfo.SetParam(byteASN, m_MAMTK_Ly, m_KeyMain);
            if (!storageUserKey(KeyInfo))
                return false;
            //�ڲ���֤��ԿMIAK
            KeyInfo = new StorageKeyParam("��װ�����ڲ���֤��Կ", 0x01, 0xF9, 0xAA, 0x0A, 0x33);//ͬ������Կ
            KeyInfo.SetParam(byteASN, m_MIAK_Ly, m_KeyMain);
            if (!storageUserKey(KeyInfo))
                return false;
            return true;
        }

        private bool ReadKeyFromXml()
        {
            CpuKeyData CpuKey = new CpuKeyData();
            CpuKey.nAppIndex = 1;
            if (!GlobalControl.GetXmlCpuKeyVal(m_ctrlApdu.m_strCardKeyPath, CpuKey))
                return false;
            CpuKeyData CpuKey_Ly = new CpuKeyData();
            CpuKey_Ly.nAppIndex = 2;
            if (!GlobalControl.GetXmlCpuKeyVal(m_ctrlApdu.m_strCardKeyPath, CpuKey))
                CpuKey_Ly = null;

            SetMainKeyValue(CpuKey.MasterKeyVal, CardCategory.CpuCard);//��Ƭ������Կ
            SetMaintainKeyValue(CpuKey.MasterTendingKeyVal, CardCategory.CpuCard);  //��Ƭά����Կ   
            Buffer.BlockCopy(CpuKey.AppMasterKey, 0, m_MAMK, 0, 16);
            Buffer.BlockCopy(CpuKey.AppTendingKey, 0, m_MAMTK, 0, 16);
            Buffer.BlockCopy(CpuKey.AppInternalAuthKey, 0, m_MIAK, 0, 16);
            Buffer.BlockCopy(CpuKey.AppPinResetKey, 0, m_MRPK, 0, 16);
            Buffer.BlockCopy(CpuKey.AppPinUnlockKey, 0, m_MPUK, 0, 16);
            Buffer.BlockCopy(CpuKey.AppConsumerKey, 0, m_MPK, 0, 16);
            Buffer.BlockCopy(CpuKey.AppLoadKey, 0, m_MLK, 0, 16);
            Buffer.BlockCopy(CpuKey.AppTacKey, 0, m_MTK, 0, 16);
            Buffer.BlockCopy(CpuKey.AppUnGrayKey, 0, m_MUGK, 0, 16);
            Buffer.BlockCopy(CpuKey.AppUnLoadKey, 0, m_MULK, 0, 16);
            Buffer.BlockCopy(CpuKey.AppOverdraftKey, 0, m_MUK, 0, 16);

            if (CpuKey_Ly != null)
            {
                Buffer.BlockCopy(CpuKey.AppMasterKey, 0, m_MAMK_Ly, 0, 16);
                Buffer.BlockCopy(CpuKey.AppTendingKey, 0, m_MAMTK_Ly, 0, 16);
                Buffer.BlockCopy(CpuKey.AppInternalAuthKey, 0, m_MIAK_Ly, 0, 16);
                Buffer.BlockCopy(CpuKey.AppPinResetKey, 0, m_MRPK_Ly, 0, 16);
                Buffer.BlockCopy(CpuKey.AppPinUnlockKey, 0, m_MPUK_Ly, 0, 16);
                Buffer.BlockCopy(CpuKey.AppConsumerKey, 0, m_MPK_Ly, 0, 16);
                Buffer.BlockCopy(CpuKey.AppLoadKey, 0, m_MLK_Ly, 0, 16);
                Buffer.BlockCopy(CpuKey.AppTacKey, 0, m_MTK_Ly, 0, 16);
                Buffer.BlockCopy(CpuKey.AppUnGrayKey, 0, m_MUGK_Ly, 0, 16);
            }

            byte[] ConsumerKey = GlobalControl.GetPrivateKeyFromXml(m_ctrlApdu.m_strCardKeyPath, "PsamKeyValue", "ConsumerMasterKey");
            if (ConsumerKey == null || !PublicFunc.ByteDataEquals(ConsumerKey, m_MPK) || !PublicFunc.ByteDataEquals(ConsumerKey, m_MPK_Ly))
            {
                OnTextOutput(new MsgOutEvent(0, "��Ƭ������Կ��һ��"));
                MessageBox.Show("���������������Ҫ������Կһ�£�����ǰʹ�õ�������Կ��һ�¡�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return true;
        }

        private void SaveCpuCardKey(SqlHelper ObjSql, Guid keyGuid, byte[] ASN)
        {
            //����Կ�洢��Base_Card_Key��
            if (m_ctrlApdu.m_CardKeyFrom == CardKeySource.CardKeyFromXml)
            {
                GlobalControl.InsertCardKeyFromXml(ObjSql, keyGuid, ASN, m_ctrlApdu.m_strCardKeyPath, 1);
                GlobalControl.InsertCardKeyFromXml(ObjSql, keyGuid, ASN, m_ctrlApdu.m_strCardKeyPath, 2);
            }
            else
            {
                GlobalControl.InsertCardKeyFromDb(ObjSql, keyGuid, ASN, 1);
                GlobalControl.InsertCardKeyFromDb(ObjSql, keyGuid, ASN, 2);
            }
        }

    }
}
