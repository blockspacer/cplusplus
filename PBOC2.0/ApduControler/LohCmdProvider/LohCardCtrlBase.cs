using System;
using System.Collections.Generic;
using System.Text;
using IFuncPlugin;
using System.Data.SqlClient;
using System.Data;
using SqlServerHelper;
using ApduParam;
using ApduCtrl;
using ApduInterface;

namespace LohApduCtrl
{
    public class LohCardCtrlBase : ICardCtrlBase
    {
        //MF�¿�Ƭ������Կ
        protected static byte[] m_KeyMain = new byte[] { 0x57, 0x41, 0x54, 0x43, 0x48, 0x44, 0x41, 0x54, 0x41, 0x54, 0x69, 0x6d, 0x65, 0x43, 0x4f, 0x53 };
        //MF�¿�Ƭά����Կ
        protected static byte[] m_KeyMaintain = new byte[] { 0x57, 0x41, 0x54, 0x43, 0x48, 0x44, 0x41, 0x54, 0x41, 0x54, 0x69, 0x6d, 0x65, 0x43, 0x4f, 0x53 };

        //��ƬӦ��������Կ
        protected static byte[] m_KeyAppMain = new byte[] { 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11 };

        //////////////////////////////////////////////////////////////////////////
        //PSAM����MF�¿�Ƭ������Կ
        protected static byte[] m_KeyPsamMain = new byte[] { 0x57, 0x41, 0x54, 0x43, 0x48, 0x44, 0x41, 0x54, 0x41, 0x54, 0x69, 0x6d, 0x65, 0x43, 0x4f, 0x53 };
        //PSAM����MF�¿�Ƭά����Կ
        protected static byte[] m_KeyPsamMaintain = new byte[] { 0x57, 0x41, 0x54, 0x43, 0x48, 0x44, 0x41, 0x54, 0x41, 0x54, 0x69, 0x6d, 0x65, 0x43, 0x4f, 0x53 };

        protected SqlConnectInfo m_DBInfo = new SqlConnectInfo();

        public LohCardCtrlBase()
        {

        }

        protected string GetErrString(byte SW1, byte SW2, string strErrCode)
        {
            if (SW1 == 0x63 && (byte)(SW2 & 0xF0) == 0xC0)
            {
                int nRetry = (int)(SW2&0x0F);
                return string.Format("��֤ʧ�ܣ�ʣ��{0}�λ���",nRetry);
            }
            else if (SW1 == 0x69 && SW2 == 0x83)
            {
                return "��֤��������";
            }
            else if (SW1 == 0x93 && SW2 == 0x03)
            {
                return "Ӧ����������";
            }
            return "��������" + strErrCode;
        }

        public void SetMainKeyValue(byte[] byteKey, CardCategory eCategory)
        {
            if (byteKey.Length != 16)
                return;
            if (eCategory == CardCategory.CpuCard)
                Buffer.BlockCopy(byteKey, 0, m_KeyMain, 0, 16);
            else if (eCategory == CardCategory.PsamCard)
                Buffer.BlockCopy(byteKey, 0, m_KeyPsamMain, 0, 16);
        }

        public void SetUserAppKeyValue(byte[] byteKey)
        {
            if (byteKey.Length != 16)
                return;
            Buffer.BlockCopy(byteKey, 0, m_KeyAppMain, 0, 16);
        }

        public byte[] CardKeyToDb(bool bOrg, CardCategory eCategory)
        {
            if (eCategory == CardCategory.CpuCard)
                return m_KeyMain;
            else if (eCategory == CardCategory.PsamCard)
                return m_KeyPsamMain;
            else
                return null;
        }

        public byte[] GetKeyVal(bool bMainKey, CardCategory eCategory)
        {
            byte[] key = null;

            if (eCategory == CardCategory.CpuCard)
                key = m_KeyMain;
            else if (eCategory == CardCategory.PsamCard)
                key = m_KeyPsamMain;
     
            return key;
        }

        //���������Կ
        protected byte[] GetProcessKey(byte[] ASN, byte[] MasterKey, byte[] RandVal, byte[] byteSn)
        {
            if (ASN.Length != 8)
                return null;
            //������Կ
            byte[] SubKey = new byte[16];
            byte[] encryptAsn = DesCryptography.TripleEncryptData(ASN, MasterKey);
            byte[] XorASN = new byte[8];
            for (int i = 0; i < 8; i++)
                XorASN[i] = (byte)(ASN[i] ^ 0xFF);
            byte[] encryptXorAsn = DesCryptography.TripleEncryptData(XorASN, MasterKey);
            Buffer.BlockCopy(encryptAsn, 0, SubKey, 0, 8);
            Buffer.BlockCopy(encryptXorAsn, 0, SubKey, 8, 8);
            byte[] byteData = new byte[8];
            Buffer.BlockCopy(RandVal, 0, byteData, 0, 4);
            Buffer.BlockCopy(byteSn, 0, byteData, 4, 2);
            byteData[6] = 0x80;
            byteData[7] = 0x00;
            byte[] byteRetKey = DesCryptography.TripleEncryptData(byteData, SubKey);
            return byteRetKey;
        }

        protected void StrKeyToByte(string strKey, byte[] byteKey)
        {
            byte[] BcdKey = PublicFunc.StringToBCD(strKey);
            if (BcdKey.Length == 16)
                Buffer.BlockCopy(BcdKey, 0, byteKey, 0, 16);
        }

        protected byte[] GetRelatedKey(SqlHelper sqlHelp, CardCategory eCardType)
        {
            SqlDataReader dataReader = null;
            if (eCardType == CardCategory.PsamCard)
            {
                sqlHelp.ExecuteProc("PROC_GetPsamKey", out dataReader);
            }
            else
            {
                SqlParameter[] sqlparam = new SqlParameter[1];
                sqlparam[0] = sqlHelp.MakeParam("ApplicationIndex", SqlDbType.Int, 4, ParameterDirection.Input, 1);
                sqlHelp.ExecuteProc("PROC_GetCpuKey", sqlparam, out dataReader);
            }
            if (dataReader == null)
                return null;
            if (!dataReader.HasRows)
            {
                dataReader.Close();
                return null;
            }
            else
            {
                byte[] ConsumerKey = new byte[16];
                if (dataReader.Read())
                {
                    string strKey = (string)dataReader["ConsumerMasterKey"];
                    StrKeyToByte(strKey, ConsumerKey);
                }
                dataReader.Close();
                return ConsumerKey;
            }
        }
    }
}
