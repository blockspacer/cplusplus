using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using SqlServerHelper;
using System.Data;
using IFuncPlugin;
using ApduParam;
using ApduInterface;
using ApduCtrl;
using System.Windows.Forms;
using System.Diagnostics;
using System.Xml;
using CardControl;

namespace LohApduCtrl
{
    public class LohPsamCardControl : LohCardCtrlBase , ISamCardControl
    {
        public event MessageOutput TextOutput = null;

        private ApduController m_ctrlApdu = null;
        private ISamApduProvider m_CmdProvider = null;

        private static byte[] m_PSE = new byte[] { 0x31, 0x50, 0x41, 0x59, 0x2E, 0x53, 0x59, 0x53, 0x2E, 0x44, 0x44, 0x46, 0x30, 0x31 };//"1PAY.SYS.DDF01"
        private static byte[] m_ADF01 = new byte[] { 0x53, 0x49, 0x4E, 0x4F, 0x50, 0x45, 0x43};//SINOPEC
        private static byte[] m_ADF02 = new byte[] { 0x4C, 0x4F, 0x59, 0x41, 0x4C, 0x54, 0x59};//LOYALTY

        //Ӧ��������Կ
        private static byte[] m_MAMK = new byte[] { 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11 };
        //Ӧ��ά����Կ
        private static byte[] m_MAMTK = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };
        //��������Կ
        private static byte[] m_MPK = new byte[] { 0xDA, 0xC2, 0x71, 0x5F, 0x15, 0xC1, 0x40, 0x6D, 0xF3, 0x2E, 0xE6, 0x9E, 0xD4, 0xF8, 0x46, 0x2E };
        //DTK 
        private static byte[] m_DTK = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };
        //��Կ��MAC���ܵȣ�
        private static byte[] m_MADK = new byte[] { 0xDA, 0xC2, 0x71, 0x5F, 0x15, 0xC1, 0x40, 0x6D, 0xF3, 0x2E, 0xE6, 0x9E, 0xD4, 0xF8, 0x46, 0x2E };


        public LohPsamCardControl(ApduController ApduCtrlObj, SqlConnectInfo DbInfo)
        {
            m_ctrlApdu = ApduCtrlObj;
            m_CmdProvider = m_ctrlApdu.GetPsamApduProvider();
            m_DBInfo = DbInfo;
        }

        protected virtual void OnTextOutput(MsgOutEvent args)
        {
            Trace.WriteLine(args.Message);
            if (TextOutput != null)
                TextOutput(args);
        }

        private bool SelectSamFile(bool bSamSlot,byte[] byteArray, byte[] prefixData)
        {
            m_CmdProvider.createSelectCmd(byteArray, prefixData);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.SAMCmdExchange(bSamSlot,data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
            {
                OnTextOutput(new MsgOutEvent(nRet, "SAM��ѡ��" + GetFileDescribe(byteArray) + "�ļ�ʧ��"));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                OnTextOutput(new MsgOutEvent(0, "SAM��ѡ��" + GetFileDescribe(byteArray) + "�ļ�Ӧ��" + strData));
                if (nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x61 && RecvData[nRecvLen - 1] > 0x00)
                {
                    int nGetLen = (int)RecvData[nRecvLen - 1];
                    return GetSamResponse(bSamSlot,nGetLen);
                }
                else if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return false;
            }
            return true;
        }

        private bool SelectFile(byte[] byteArray, byte[] prefixData)
        {
            m_CmdProvider.createSelectCmd(byteArray, prefixData);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];            
            int nRecvLen = 0;  
            int nRet = m_ctrlApdu.IccCmdExchange(data, datalen,RecvData,ref nRecvLen);
            if (nRet < 0)
            {
                OnTextOutput(new MsgOutEvent(nRet, "SAM��ѡ��" + GetFileDescribe(byteArray) + "�ļ�ʧ��"));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                OnTextOutput(new MsgOutEvent(0, "SAM��ѡ��" + GetFileDescribe(byteArray) + "�ļ�Ӧ��" + strData));
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

        private bool GetSamResponse(bool bSamSlot,int nResLen)
        {
            m_CmdProvider.createGetResponseCmd(nResLen);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.SAMCmdExchange(bSamSlot,data, datalen, RecvData, ref nRecvLen);
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

        private bool GetResponse(int nResLen)
        {
            m_CmdProvider.createGetResponseCmd(nResLen);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.IccCmdExchange(data, datalen, RecvData, ref nRecvLen);
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
            int nRet = m_ctrlApdu.IccCmdExchange(data, datalen, RecvData, ref nRecvLen);
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

        //PSAM���ƿ��������ⲿ��֤,���뿨Ƭ���غ���Խ����ⲿ��֤
        //private bool ExternalAuthWithKey(byte[] KeyVal)
        //{
        //    byte[] randByte = GetRandomValue(8);
        //    if (randByte == null || randByte.Length != 8)
        //        return false;

        //    OnTextOutput(new MsgOutEvent(0, "ʹ����Կ��" + BitConverter.ToString(KeyVal) + "�����ⲿ��֤"));

        //    return ExternalAuthenticate(randByte, KeyVal);
        //}

        //PSAM���ƿ��������ⲿ��֤,���뿨Ƭ���غ���Խ����ⲿ��֤
        //private bool ExternalAuthentication(bool bMainKey)
        //{
        //    byte[] randByte = GetRandomValue(8);
        //    if (randByte == null || randByte.Length != 8)
        //        return false;

        //    byte[] KeyVal = GetKeyVal(bMainKey, CardCategory.PsamCard);

        //    return ExternalAuthenticate(randByte, KeyVal);
        //}

        private bool ExternalAuthenticate(byte[] randByte, byte[] KeyVal)
        {
            m_CmdProvider.createExternalAuthenticationCmd(randByte, KeyVal);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.IccCmdExchange(data, datalen, RecvData, ref nRecvLen);
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

        private bool DeleteMF()
        {
            return ClearMF(null, null);
        }    

        private bool ClearMF(byte[] randByte, byte[] KeyVal)
        {
            m_CmdProvider.createClearMFcmd(randByte, KeyVal);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.IccCmdExchange(data, datalen, RecvData, ref nRecvLen);
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

        private string GetFileDescribe(byte[] byteArray)
        {
            if (PublicFunc.ByteDataEquals(byteArray, m_PSE))
                return "MF";
            else if (PublicFunc.ByteDataEquals(byteArray, m_ADF01))
                return "ADF01";
            else if (PublicFunc.ByteDataEquals(byteArray, m_ADF02))
                return "ADF02";
            else
                return "";
        }

        public int InitIccCard(bool bMainKey)
        {
            if (!DeleteMF())
                return 1;
            InitWhiteCard();
            return 0;
        }

        public void GetCosVer()
        {

        }

        public bool SelectPsamApp()
        {
            if (!SelectFile(m_PSE, null))
                return false;
            byte[] prefix = new byte[] { 0xA0, 0x00, 0x00, 0x00, 0x03 };
            if (!SelectFile(m_ADF01, prefix))
                return false;            
            return true; 
        }

        public bool SamAppSelect(bool bSamSlot)
        {
            if (!SelectSamFile(bSamSlot,m_PSE, null))
                return false;
            byte[] prefix = new byte[] { 0xA0, 0x00, 0x00, 0x00, 0x03 };
            if (!SelectSamFile(bSamSlot,m_ADF01, prefix))
                return false;
            return true; 
        }

        private bool CreateKeyFile(ushort uFileId)
        {
            m_CmdProvider.createGenerateKeyCmd(uFileId, 0, 0);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.IccCmdExchange(data, datalen,RecvData,ref nRecvLen);
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

        private bool CreateCardInfo(byte GenerateFlag, ushort FileId, byte FileType, ushort FileLen)
        {
            m_CmdProvider.createGenerateEFCmd(GenerateFlag, FileId, FileType, FileLen);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.IccCmdExchange(data, datalen,RecvData,ref nRecvLen);
            if (nRet < 0)
            {
                string strMsg = string.Format("�����ļ�{0}ʧ��", FileId.ToString("X"));
                OnTextOutput(new MsgOutEvent(nRet, strMsg));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                string strMsg = string.Format("�����ļ�{0}Ӧ��{1}", FileId.ToString("X"), strData);
                OnTextOutput(new MsgOutEvent(0, strMsg));
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return false;
            }
            return true;
        }

        private string GetFileName(ushort FileId,bool bMainKey)
        {
            string strRet = "";
            switch (FileId)
            {
                case 0x0015:
                    strRet = "��Ƭ������Ϣ�ļ�";
                    break;
                case 0x0016:
                    strRet = "��Ƭ�ն���Ϣ�ļ�";
                    break;
                case 0x0017:
                    strRet = "Ӧ�ù�����Ϣ�ļ�";
                    break;
            }
            return strRet;
        }

        private bool SelectCardInfo(ushort FileId,bool bMainKey)
        {
            m_CmdProvider.createSelectEFCmd(FileId);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.IccCmdExchange(data, datalen,RecvData,ref nRecvLen);
            if (nRet < 0)
            {
                string strMessage = string.Format("SAM��ѡ��{0}ʧ��", GetFileName(FileId, bMainKey));
                OnTextOutput(new MsgOutEvent(nRet, strMessage));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                string strMessage = string.Format("SAM��ѡ��{0}Ӧ��{1}", GetFileName(FileId, bMainKey), strData);
                OnTextOutput(new MsgOutEvent(0, strMessage));
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

        private bool StorageCardInfo(byte[] PsamId)
        {
            m_CmdProvider.createStorageCardInfoCmd(PsamId);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.IccCmdExchange(data, datalen,RecvData,ref nRecvLen);
            if (nRet < 0)
            {
                OnTextOutput(new MsgOutEvent(nRet, "д�뿨Ƭ������Ϣʧ��"));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                OnTextOutput(new MsgOutEvent(0, "д�뿨Ƭ������ϢӦ��" + strData));
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return false;
            }
            return true;
        }

        private bool StorageTermialInfo(byte[] TermialId)
        {
            m_CmdProvider.createStorageTermInfoCmd(TermialId);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.IccCmdExchange(data, datalen,RecvData,ref nRecvLen);
            if (nRet < 0)
            {
                OnTextOutput(new MsgOutEvent(nRet, "д���ն���Ϣʧ��"));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                OnTextOutput(new MsgOutEvent(0, "д���ն���ϢӦ��" + strData));
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return false;
            }
            return true;
        }

        private bool CreateAppDF()
        {
            //����ADF01
            if (!CreateDIR(m_ADF01, 0x3F01))
                return false;
            //����ADF02
            if (!CreateDIR(m_ADF02, 0x3F02))
                return false;
            return true;
        }

        public bool CreateIccInfo(byte[] PsamId, byte[] TermialId)
        {
            if (!SelectFile(m_PSE, null))
                return false;
            if (!CreateKeyFile(0x3F00))
                return false;
            StorageMasterKey(m_KeyPsamMain);
            StorageMaintainKey(m_KeyPsamMaintain);
            ////////////////////////////////////////////////////////////////////////////////////////////
            //����ADF01��ADF02
            if (!CreateAppDF())
                return false;
            /////////////////////////////////////////////////////////////////////////////////////////////
            if (!SelectFile(m_PSE, null))
                return false;
            //����0015�ļ�
            if (!CreateCardInfo(0, 0x0015, 0x28, 0x000E))
                return false;
            if (!SelectCardInfo(0x0015,false))
                return false;
            if (!StorageCardInfo(PsamId))
                return false;
            //����0016�ļ�
            if (!CreateCardInfo(0, 0x0016, 0x28, 0x0006))
                return false;
            if (!SelectCardInfo(0x0016, false))
                return false;
            if (!StorageTermialInfo(TermialId))
                return false;

            return true;
        }

        private bool GenerateADF(byte[] byteName, ushort FileId)
        {
            m_CmdProvider.createGenerateADFCmd(byteName, FileId);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.IccCmdExchange(data, datalen,RecvData,ref nRecvLen);
            if (nRet < 0)
            {
                string strMessage = string.Format("����{0}�ļ�ʧ��", GetFileDescribe(byteName));
                OnTextOutput(new MsgOutEvent(nRet, strMessage));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                string strMessage = string.Format("����{0}�ļ�Ӧ��{1}", GetFileDescribe(byteName), strData);
                OnTextOutput(new MsgOutEvent(0, strMessage));
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return false;
            }
            return true;
        }

        private bool CreateDIR(byte[] byteDIR, ushort FileId)
        {
            if (!SelectFile(m_PSE, null))
                return false;
            if (!GenerateADF(byteDIR, FileId))
                return false;
            return true;
        }        

        private bool StoragePsamInfo(IccCardInfoParam psamInfo)
        {
            m_CmdProvider.createStoragePsamInfoCmd(psamInfo);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.IccCmdExchange(data, datalen,RecvData,ref nRecvLen);
            if (nRet < 0)
            {
                OnTextOutput(new MsgOutEvent(nRet, "д��Ӧ�ù�����Ϣʧ��"));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                OnTextOutput(new MsgOutEvent(0, "д��Ӧ�ù�����ϢӦ��" + strData));
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return false;
            }
            return true;
        }

        public bool WriteApplicationInfo(IccCardInfoParam psamInfo)
        {
            byte[] prefix = new byte[] { 0xA0, 0x00, 0x00, 0x00, 0x03 };
            if (!SelectFile(m_ADF01, prefix))
                return false;
            if (!CreateCardInfo(0, 0x0017, 0x28, 0x0019))
                return false;
            if (!SelectCardInfo(0x0017, false))
                return false;
            if (!StoragePsamInfo(psamInfo))
                return false;
            return true;
        }

        private string GetKeyName(byte Usage, byte Ver)
        {
            string strRet = "";
            switch (Usage)
            {
                case 0x42:
                    strRet = "��������Կ";
                    break;
                case 0x15:
                    strRet = "TAC��Կ";
                    break;                    
                case 0x08:
                    strRet = "������Կ";
                    break;
                case 0x01:
                    strRet = "Ӧ��ά����Կ";
                    break;
                case 0x00:
                    strRet = "Ӧ��������Կ";
                    break;
            }
            return strRet;
        }

        private bool StoragePsamKey(byte[] keyApp, byte Usage, byte Ver, byte[] keyEncrypt)
        {
            byte[] randVal = GetRandomValue(4);
            if (randVal == null || randVal.Length != 4)
                return false;
            //PSAM��д����Կ���ֵ4�ֽ�+4�ֽ�0
            byte[] randomVal = new byte[8];
            Buffer.BlockCopy(randVal, 0, randomVal, 0, 4);
            m_CmdProvider.createStorageAppKeyCmd(randomVal, keyApp, Usage, Ver, keyEncrypt);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.IccCmdExchange(data, datalen,RecvData,ref nRecvLen);
            if (nRet < 0)
            {
                string strMessage = string.Format("д��{0}ʧ��", GetKeyName(Usage, Ver));
                OnTextOutput(new MsgOutEvent(nRet, strMessage));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                string strMessage = string.Format("д��{0}Ӧ��{1}", GetKeyName(Usage, Ver), strData);
                OnTextOutput(new MsgOutEvent(0, strMessage));
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return false;
            }
            return true;
        }

        //��ADF01Ӧ����д��Կ
        public bool SetupIccKey()
        {
            if (!SelectPsamApp())
                return false;
            //Ӧ��������Կ
            if (!StoragePsamKey(m_MAMK, 0x00, 0x01, m_KeyPsamMain))
                return false;
            //Ӧ��ά����Կ
            if (!StoragePsamKey(m_MAMTK, 0x01, 0x01, m_MAMK))
                return false;
            //��������Կ
            if (!StoragePsamKey(m_MPK, 0x42, 0x01, m_MAMK))//0x42Ҫ��PSAM������MAC1ʱ�������η�ɢ
                return false;
            //SAM TAC��Կ
            if (!StoragePsamKey(m_DTK, 0x15, 0x01, m_MAMK))
                return false;
            //��Կ��MAC���ܵȣ�
            if (!StoragePsamKey(m_MADK, 0x08, 0x01, m_MAMK))
                return false;
            return true;
        }

        //�ﻪ������д��Ƭ�����غ�ά����Կ
        public bool SetupMainKey()
        {
            return false;
        }

        private byte[] calcUserCardMAC1(byte[] ASN, byte[] rand, byte[] BusinessSn, byte[] TermialSn, byte[] TermialRand, byte[] srcData)
        {
            byte[] MAC1 = new byte[4];
            byte[] sespk = GetPrivateProcessKey(ASN, m_MPK, rand, BusinessSn, TermialSn, TermialRand);
            if (sespk == null)
                return MAC1;
             MAC1 = m_CmdProvider.CalcMacVal_DES(srcData, sespk);
             return MAC1;
        }

        public bool InitSamGrayLock(bool bSamSlot,byte[] TermialID, byte[] random, byte[] BusinessSn, byte[] byteBalance, byte BusinessType, byte[] ASN, byte[] outData)
        {
            byte[] SysTime = PublicFunc.GetBCDTime();
            byte[] byteData = new byte[36]; //28�ֽ�����+8�ֽڷ�ɢ����
            Buffer.BlockCopy(random, 0, byteData, 0, 4);
            Buffer.BlockCopy(BusinessSn, 0, byteData, 4, 2);
            Buffer.BlockCopy(byteBalance, 0, byteData, 6, 4);
            byteData[10] = BusinessType;
            Buffer.BlockCopy(SysTime, 0, byteData, 11, 7);
            byteData[18] = 0x01;//Key Ver
            byteData[19] = 0x00; // Key Flag
            Buffer.BlockCopy(ASN, 0, byteData, 20, 8);
            Buffer.BlockCopy(ASN, 0, byteData, 28, 8); //�û������к���Ϊ��ɢ����
            m_CmdProvider.createInitSamGrayLockCmd(byteData);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.SAMCmdExchange(bSamSlot,data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
            {
                OnTextOutput(new MsgOutEvent(nRet, "SAM��MAC1�����ʼ��ʧ��"));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                OnTextOutput(new MsgOutEvent(0, "SAM��MAC1�����ʼ��Ӧ��" + strData));
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return false;
                //������ݰ�������������������:�ն˽�����ţ��ն��������BCDʱ�䣬MAC1
                Buffer.BlockCopy(RecvData, 0, outData, 0, 4);
                Buffer.BlockCopy(RecvData, 4, outData, 4, 4);
                Buffer.BlockCopy(RecvData, 8, outData, 15, 4);                
                //PSAM�������MAC1
                byte[] PSAM_MAC1 = new byte[4];
                Buffer.BlockCopy(RecvData, 8, PSAM_MAC1, 0, 4);

                 //����ר�ù�����Կ ����
                byte[] TermialSn = new byte[4];                
                byte[] TermialRandom = new byte[4];
                Buffer.BlockCopy(RecvData, 0, TermialSn, 0, 4);
                Buffer.BlockCopy(RecvData, 4, TermialRandom, 0, 4);
                byte[] srcData = new byte[14];//���ڼ���MAC1��ԭʼ����                                
                srcData[0] = BusinessType;
                Buffer.BlockCopy(TermialID, 0, srcData, 1, 6);
                Buffer.BlockCopy(SysTime, 0, srcData, 7, 7);
                //��徿�ʹ��������Կ�������η�ɢ�����MAC1
                byte[] MAC1 = calcUserCardMAC1(ASN, random, BusinessSn, TermialSn, TermialRandom, srcData);                      
                Buffer.BlockCopy(SysTime, 0, outData, 8, 7);
                Buffer.BlockCopy(PSAM_MAC1, 0, outData, 15, 4);//MAC1
                string strInfo = string.Format("SAM��MAC1�����ʼ�� MAC: {0} PC Calc MAC: {1}", BitConverter.ToString(PSAM_MAC1), BitConverter.ToString(MAC1));
                System.Diagnostics.Trace.WriteLine(strInfo);
                if (!PublicFunc.ByteDataEquals(MAC1, PSAM_MAC1))
                {
                    string strMessage = string.Format("MAC1������֤ʧ�ܣ��ն˻����{0}���û�����{1}", BitConverter.ToString(TermialID), BitConverter.ToString(ASN));
                    OnTextOutput(new MsgOutEvent(0, strMessage));
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// ����ר�����ѽ��׹�����Կ����֤PSAM�������MAC1�Ƿ���Ч
        /// </summary>
        /// <param name="ASN">�û�������</param>
        /// <param name="MPKKey">��������Կ</param>
        /// <param name="Rand">�û��������</param>
        /// <param name="OfflineSn">�ѻ�������ţ�2�ֽڣ�</param>
        /// <param name="TermialSn">�ն���ţ�4�ֽڣ�</param>
        /// <param name="TermialRand">SAM�������</param>
        /// <returns></returns>
        private byte[] GetPrivateProcessKey(byte[] ASN, byte[] MPKKey, byte[] Rand, byte[] OfflineSn, byte[] TermialSn, byte[] TermialRand)
        {
            if (ASN.Length != 8)
                return null;
            //�м���Կ,���η�ɢ
            byte[] DPKKey = new byte[16];
            byte[] LeftDiversify = DesCryptography.TripleEncryptData(ASN, MPKKey);
            byte[] XorASN = new byte[8];
            for (int i = 0; i < 8; i++)
                XorASN[i] = (byte)(ASN[i] ^ 0xFF);
            byte[] RightDiversify = DesCryptography.TripleEncryptData(XorASN, MPKKey);
            Buffer.BlockCopy(LeftDiversify, 0, DPKKey, 0, 8);
            Buffer.BlockCopy(RightDiversify, 0, DPKKey, 8, 8);

            byte[] SecondDPKKey = new byte[16];
            byte[] LeftDPK = DesCryptography.TripleEncryptData(ASN, DPKKey);
            byte[] RightDPK = DesCryptography.TripleEncryptData(XorASN, DPKKey);
            Buffer.BlockCopy(LeftDPK, 0, SecondDPKKey, 0, 8);
            Buffer.BlockCopy(RightDPK, 0, SecondDPKKey, 8, 8);

            byte[] byteData = new byte[8];
            Buffer.BlockCopy(Rand, 0, byteData, 0, 4);
            Buffer.BlockCopy(OfflineSn, 0, byteData, 4, 2);
            Buffer.BlockCopy(TermialSn, 2, byteData, 6, 2);
            //�м���Կ
            byte[] byteTmpck = DesCryptography.TripleEncryptData(byteData, SecondDPKKey);
            //����MAC����Կ
            byte[] byteSESPK = m_CmdProvider.CalcPrivateProcessKey(TermialRand, byteTmpck);
            return byteSESPK;
        }

        public bool VerifyMAC2(bool bSamSlot,byte[] MAC2)
        {
            m_CmdProvider.createVerifyMAC2Cmd(MAC2);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.SAMCmdExchange(bSamSlot,data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
            {
                OnTextOutput(new MsgOutEvent(nRet, "SAM����֤MAC2ʧ��"));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                OnTextOutput(new MsgOutEvent(0, "SAM����֤MAC2Ӧ��" + strData));
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return false;
             }
            return true;
        }

        public bool CalcGMAC(bool bSamSlot,byte BusinessType,byte[] ASN, int nOffLineSn, int nMoney, byte[] outGMAC)
        {
            m_CmdProvider.createCalcGMACCmd(BusinessType, ASN, nOffLineSn, nMoney);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.SAMCmdExchange(bSamSlot,data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
            {
                OnTextOutput(new MsgOutEvent(nRet, "SAM������GMACʧ��"));
                return false;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                OnTextOutput(new MsgOutEvent(0, "SAM������GMACӦ��" + strData));
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return false;
                Buffer.BlockCopy(RecvData, 0, outGMAC, 0, 4);
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

            if (!GetDbPsamKeyValue(ObjSql))
            {
                ObjSql.CloseConnection();
                ObjSql = null;
                return false;
            }

            byte[] ConsumerKey = GlobalControl.GetDbConsumerKey(ObjSql, "PROC_GetCpuKey", "AppConsumerKey", 1);
            if (ConsumerKey == null || !PublicFunc.ByteDataEquals(ConsumerKey, m_MPK))
            {
                OnTextOutput(new MsgOutEvent(0, "����������Կ��һ��"));
                MessageBox.Show("����������Ҫ������Կһ�£�����ǰʹ�õ�������Կ��һ�¡�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            byte[] ConsumerKey_Ly = GlobalControl.GetDbConsumerKey(ObjSql, "PROC_GetCpuKey", "AppConsumerKey", 2);
            if (ConsumerKey_Ly != null && !PublicFunc.ByteDataEquals(ConsumerKey_Ly, m_MPK))
            {
                OnTextOutput(new MsgOutEvent(0, "����������Կ��һ��"));
                MessageBox.Show("����������Ҫ��Կһ�£�����ǰʹ�õĻ���������Կ��һ�¡�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
        private bool GetDbPsamKeyValue(SqlHelper sqlHelp)
        {
            PsamKeyData KeyVal = new PsamKeyData();
            if (!GlobalControl.GetDbPsamKeyVal(sqlHelp, KeyVal))
                return false;
            SetMainKeyValue(KeyVal.MasterKeyVal, CardCategory.PsamCard);  //��Ƭ������Կ  
            SetMaintainKeyValue(KeyVal.MasterTendingKeyVal, CardCategory.PsamCard);  //��Ƭά����Կ
            Buffer.BlockCopy(KeyVal.ApplicationMasterKey, 0, m_MAMK, 0, 16);//δ��,psam����Ӧ��������Կ��װ
            Buffer.BlockCopy(KeyVal.ApplicationTendingKey, 0, m_MAMTK, 0, 16);
            Buffer.BlockCopy(KeyVal.ConsumerMasterKey, 0, m_MPK, 0, 16);            
            Buffer.BlockCopy(KeyVal.MacEncryptKey, 0, m_MADK, 0, 16);
            return true;
        }


        //PSAM������������ݿ�
        public bool SavePsamCardInfoToDb(IccCardInfoParam PsamInfoPar)
        {
            string strDbVal = null;
            SqlHelper ObjSql = new SqlHelper();
            if (!ObjSql.OpenSqlServerConnection(m_DBInfo.strServerName, m_DBInfo.strDbName, m_DBInfo.strUser, m_DBInfo.strUserPwd))
            {
                ObjSql = null;
                return false;
            }

            bool bSuccess = false;
            SqlParameter[] sqlparams = new SqlParameter[10];
            strDbVal = BitConverter.ToString(PsamInfoPar.GetBytePsamId()).Replace("-", "");
            sqlparams[0] = ObjSql.MakeParam("PsamCardId", SqlDbType.Char, 16, ParameterDirection.Input, strDbVal);
            strDbVal = BitConverter.ToString(PsamInfoPar.GetByteTermId()).Replace("-", "");
            sqlparams[1] = ObjSql.MakeParam("TerminalId", SqlDbType.VarChar, 12, ParameterDirection.Input, strDbVal);
            sqlparams[2] = ObjSql.MakeParam("ClientId", SqlDbType.Int, 4, ParameterDirection.Input, PsamInfoPar.ClientID);            
            sqlparams[3] = ObjSql.MakeParam("UseValidateDate", SqlDbType.DateTime, 8, ParameterDirection.Input, PsamInfoPar.ValidAppForm);
            sqlparams[4] = ObjSql.MakeParam("UseInvalidateDate", SqlDbType.DateTime, 8, ParameterDirection.Input, PsamInfoPar.ValidAppTo);

            strDbVal = BitConverter.ToString(PsamInfoPar.GetByteCompanyIssue()).Replace("-", "");
            sqlparams[5] = ObjSql.MakeParam("CompanyFrom", SqlDbType.VarChar, 16, ParameterDirection.Input, strDbVal);
            strDbVal = BitConverter.ToString(PsamInfoPar.GetByteCompanyRecv()).Replace("-", "");
            sqlparams[6] = ObjSql.MakeParam("CompanyTo", SqlDbType.VarChar, 16, ParameterDirection.Input, strDbVal);
            sqlparams[7] = ObjSql.MakeParam("Remark", SqlDbType.NVarChar, 50, ParameterDirection.Input, PsamInfoPar.Remark);
            //��Կ
            strDbVal = "";
            sqlparams[8] = ObjSql.MakeParam("OrgKey", SqlDbType.Char, 32, ParameterDirection.Input, strDbVal);
            strDbVal = BitConverter.ToString(CardKeyToDb(false,CardCategory.PsamCard)).Replace("-", "");
            sqlparams[9] = ObjSql.MakeParam("PsamMasterKey", SqlDbType.Char, 32, ParameterDirection.Input, strDbVal);
            if (ObjSql.ExecuteProc("PROC_PublishPsamCard", sqlparams) == 0)
                bSuccess = true;
            ObjSql.CloseConnection();
            ObjSql = null;
            return bSuccess;
        }

        //������ݿ����Ƿ��иÿ��ķ�����¼,��徿�����
        public bool CheckPublishedCard(bool bMainKey, byte[] KeyInit)
        {
            //PSAM����ȡ���Ų��ý�ҵ��Ӧ��
            byte[] PsamAsn = GetPsamASN(false);
            if (PsamAsn == null || PsamAsn.Length != 8)
                return false;
            SqlHelper ObjSql = new SqlHelper();
            if (!ObjSql.OpenSqlServerConnection(m_DBInfo.strServerName, m_DBInfo.strDbName, m_DBInfo.strUser, m_DBInfo.strUserPwd))
            {
                ObjSql = null;
                return false;
            }
            string strDbAsn = BitConverter.ToString(PsamAsn).Replace("-", "");
            SqlDataReader dataReader = null;
            SqlParameter[] sqlparam = new SqlParameter[1];
            sqlparam[0] = ObjSql.MakeParam("PsamId", SqlDbType.Char, 16, ParameterDirection.Input, strDbAsn);            
            ObjSql.ExecuteCommand("select * from Psam_Card where PsamId = @PsamId", sqlparam, out dataReader);
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

        //��ȡ�ն˻����
        public byte[] GetTerminalId(bool bSamSlot)
        {
            if (!SelectSamFile(bSamSlot,m_PSE, null))
                return null;
            m_CmdProvider.createGetEFFileCmd(0x96, 0x06);//�ļ���ʶ(100+10110)0x16,�ն˻���ų���6
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.SAMCmdExchange(bSamSlot,data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
            {
                OnTextOutput(new MsgOutEvent(nRet, "SAM����ȡ�ն˻����ʧ��"));
                return null;
            }
            else
            {
                string strData = m_ctrlApdu.hex2asc(RecvData, nRecvLen);
                OnTextOutput(new MsgOutEvent(0, "SAM����ȡ�ն˻����Ӧ��" + strData));
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return null;
                byte[] TerminalID = new byte[6];
                Buffer.BlockCopy(RecvData, 0, TerminalID, 0, 6);
                return TerminalID;
            }
        }

        //��ȡPSAM���к�
        /// <summary>
        /// ԭʼ���޿��ţ�������ݿ��¼ʱ����ʾ����
        /// </summary>
        public byte[] GetPsamASN(bool bMessage)
        {
            m_CmdProvider.createGetEFFileCmd(0x95, 0x0E);//������Ϣ(100+10101)0x15�ļ�����0x0E
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.IccCmdExchange(data, datalen, RecvData, ref nRecvLen);
            if (nRet < 0)
            {
                if (bMessage)
                    OnTextOutput(new MsgOutEvent(nRet, "��ȡ����ʧ��"));
                return null;
            }
            else
            {
                if (!(nRecvLen >= 2 && RecvData[nRecvLen - 2] == 0x90 && RecvData[nRecvLen - 1] == 0x00))
                    return null;
                byte[] PsamAsn = new byte[8];
                Buffer.BlockCopy(RecvData, 2, PsamAsn, 0, 8);//ʵ��10���ֽڣ�ǰ�����ֽ�Ϊ0������
                if (bMessage)
                    OnTextOutput(new MsgOutEvent(0, "��ȡ�����ţ�" + BitConverter.ToString(PsamAsn)));
                return PsamAsn;
            }
        }

        private bool CreateMF()
        {
            m_CmdProvider.createNewMFcmd(m_PSE);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.IccCmdExchange(data, datalen, RecvData, ref nRecvLen);
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
            int nRet = m_ctrlApdu.IccCmdExchange(data, datalen, RecvData, ref nRecvLen);
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
            byte[] KeyParam = new byte[] { 0x39, 0xF0, 0xAA, 0x0A, 0xFF };
            m_CmdProvider.createStorageKeyCmd(MasterKey, p1p2, KeyParam);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.IccCmdExchange(data, datalen, RecvData, ref nRecvLen);
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
            byte[] KeyParam = new byte[] { 0x30, 0xF0, 0xAA, 0x00, 0x00 };
            m_CmdProvider.createStorageKeyCmd(MaintainKey, p1p2, KeyParam);
            byte[] data = m_CmdProvider.GetOutputCmd();
            int datalen = data.Length;
            byte[] RecvData = new byte[128];
            int nRecvLen = 0;
            int nRet = m_ctrlApdu.IccCmdExchange(data, datalen, RecvData, ref nRecvLen);
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
            //ClearDF();   
        }

        private bool ReadKeyFromXml()
        {
            string strXmlPath = m_ctrlApdu.m_strCardKeyPath;
            try
            {
                XmlNode node = null;
                XmlDocument xml = new XmlDocument();
                xml.Load(strXmlPath);//��·����xml�ļ�
                XmlNode root = xml.DocumentElement;//ָ����ڵ�
                node = root.SelectSingleNode("Seed");
                byte[] InitData = PublicFunc.StringToBCD(node.InnerText);
                node = root.SelectSingleNode("InitKey");
                byte[] InitKey = PublicFunc.StringToBCD(node.InnerText);

                byte[] Left = DesCryptography.TripleEncryptData(InitData, InitKey);
                byte[] Right = DesCryptography.TripleDecryptData(InitData, InitKey);
                byte[] EncryptKey = new byte[16];
                Buffer.BlockCopy(Left, 0, EncryptKey, 0, 8);
                Buffer.BlockCopy(Right, 0, EncryptKey, 8, 8);

                GetXmlPsamKeyValue(root, EncryptKey);

                byte[] ConsumerKey = GlobalControl.GetPrivateKeyFromXml(strXmlPath, "UserKeyValue_App1", "AppConsumerKey");
                if (ConsumerKey == null || !PublicFunc.ByteDataEquals(ConsumerKey, m_MPK))
                {
                    OnTextOutput(new MsgOutEvent(0, "����������Կ��һ��"));
                    MessageBox.Show("����������Ҫ������Կһ�£�����ǰʹ�õ�������Կ��һ�¡�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                byte[] ConsumerKey_Ly = GlobalControl.GetPrivateKeyFromXml(strXmlPath, "UserKeyValue_App2", "AppConsumerKey");
                if (ConsumerKey_Ly != null && !PublicFunc.ByteDataEquals(ConsumerKey_Ly, m_MPK))
                {
                    OnTextOutput(new MsgOutEvent(0, "����������Կ��һ��"));
                    MessageBox.Show("����������Ҫ��Կһ�£�����ǰʹ�õĻ���������Կ��һ�¡�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }
            catch
            {
                return false;
            }

            return true;
        }

        private bool GetXmlPsamKeyValue(XmlNode ParentNode, byte[] EncryptKey)
        {
            XmlNode node = null;
            byte[] byteKey = null; 
            XmlNode PsamKeyNode = ParentNode.SelectSingleNode("PsamKeyValue");

            node = PsamKeyNode.SelectSingleNode("MasterKey");
            byteKey = DesCryptography.TripleDecryptData(PublicFunc.StringToBCD(node.InnerText), EncryptKey);
            SetMainKeyValue(byteKey, CardCategory.PsamCard);  //��Ƭ������Կ   

            node = PsamKeyNode.SelectSingleNode("MasterTendingKey");
            byteKey = DesCryptography.TripleDecryptData(PublicFunc.StringToBCD(node.InnerText), EncryptKey);
            SetMaintainKeyValue(byteKey, CardCategory.PsamCard);  //��Ƭά����Կ

            node = PsamKeyNode.SelectSingleNode("ApplicationMasterKey");
            byteKey = DesCryptography.TripleDecryptData(PublicFunc.StringToBCD(node.InnerText), EncryptKey);
            Buffer.BlockCopy(byteKey, 0, m_MAMK, 0, 16);//δ��,psam����Ӧ��������Կ��װ

            node = PsamKeyNode.SelectSingleNode("ApplicationTendingKey");
            byteKey = DesCryptography.TripleDecryptData(PublicFunc.StringToBCD(node.InnerText), EncryptKey);
            Buffer.BlockCopy(byteKey, 0, m_MAMTK, 0, 16);

            node = PsamKeyNode.SelectSingleNode("ConsumerMasterKey");
            byteKey = DesCryptography.TripleDecryptData(PublicFunc.StringToBCD(node.InnerText), EncryptKey);
            Buffer.BlockCopy(byteKey, 0, m_MPK, 0, 16);

            node = PsamKeyNode.SelectSingleNode("MacEncryptKey");
            byteKey = DesCryptography.TripleDecryptData(PublicFunc.StringToBCD(node.InnerText), EncryptKey);
            Buffer.BlockCopy(byteKey, 0, m_MADK, 0, 16);

            return true;
        }

    }
}
