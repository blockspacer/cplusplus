using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Diagnostics;

namespace CardOperating
{
    public class StorageKeyParam
    {
        private string m_strStorageName; //��Ϣ        
        private byte m_StorageIndex; //��Կ���
        private byte m_KeyType; //��Կ����
        private byte m_KeyIndex; //��Կ������
        private byte m_ErrorCount; //���������
        private byte m_PriValue;  //��ȷӦ�ú�ĺ��Ȩ��
        private byte[] m_ASN = null;  //����
        private byte[] m_XorASN = null; //����Ŀ���
        private byte[] m_StorageKey = null; //��װ��Key,λ��������
        private byte[] m_EncryptKey = null;  //���ڼ��ܵ�Key

        public string PromptInfo
        {
            get { return m_strStorageName; }
        }
        public byte P2
        {
            get { return m_StorageIndex; }
        }
        public byte KeyType
        {
            get { return m_KeyType; }
        }
        public byte KeyIndex
        {
            get { return m_KeyIndex; }
        }
        public byte ErrCount
        {
            get { return m_ErrorCount; }
        }
        public byte PriValue
        {
            get { return m_PriValue; }
        }
        public byte[] ASN
        {
            get { return m_ASN; }
        }
        public byte[] XorASN
        {
            get { return m_XorASN; }
        }
        public byte[] StorageKey
        {
            get { return m_StorageKey; }
        }
        public byte[] EncryptKey
        {
            get { return m_EncryptKey; }
        }

        public StorageKeyParam(string strStorageName,byte StorageIndex,byte KeyType, byte KeyIndex,byte ErrCount, byte PriValue)
        {
            m_strStorageName = strStorageName;
            m_StorageIndex = StorageIndex;
            m_KeyType = KeyType;
            m_KeyIndex = KeyIndex;
            m_ErrorCount = ErrCount;
            m_PriValue = PriValue;
        }

        public void SetParam(byte[] ASN, byte[] StorageKey)
        {
            if (ASN.Length < 8 || ASN.Length > 16)
                return;            
            m_ASN = new byte[8];
            m_XorASN = new byte[8];
            int nOffset = ASN.Length - 8;
            Buffer.BlockCopy(ASN, nOffset, m_ASN, 0, 8);
            for (int i = 0; i < 8; i++)
                m_XorASN[i] = (byte)(m_ASN[i] ^ 0xFF);
            m_StorageKey = StorageKey;
            byte[] KeyOrg = APDUBase.GetEncryptKey(false);//������Կ
            m_EncryptKey = new byte[16];
            Buffer.BlockCopy(KeyOrg, 0, m_EncryptKey, 0, 16);
        }

        public void SetDiversify(byte[] byteKey)
        {
            if (byteKey.Length != 16)
                return;
            m_EncryptKey = new byte[16];
            Buffer.BlockCopy(byteKey, 0, m_EncryptKey, 0, 16);
        }

        public static byte[] GetDiversify(byte[] byteASN)
        {
            if (byteASN.Length < 8 || byteASN.Length > 16)
                return null;   
            byte[] asn = new byte[8];
            byte[] xorasn = new byte[8];
            int nOffset = byteASN.Length - 8;
            Buffer.BlockCopy(byteASN, nOffset, asn, 0, 8);
            for (int i = 0; i < 8; i++)
                xorasn[i] = (byte)(asn[i] ^ 0xFF);
            //���ɷ�ɢ��Կ
            byte[] keyDiversify = new byte[16];
            byte[] KeyApp = APDUBase.GetEncryptKey(true);
            byte[] KeyLeft = APDUBase.TripleEncryptData(asn, KeyApp);
            byte[] KeyRight = APDUBase.TripleEncryptData(xorasn, KeyApp);
            Buffer.BlockCopy(KeyLeft, 0, keyDiversify, 0, 8);
            Buffer.BlockCopy(KeyRight, 0, keyDiversify, 8, 8);
            return keyDiversify;
        }

        public static byte[] GetUpdateEFKey(byte[] keyMAMK, byte[] byteASN)
        {
            if (byteASN.Length < 8 || byteASN.Length > 16)
                return null;
            byte[] asn = new byte[8];
            byte[] xorasn = new byte[8];
            int nOffset = byteASN.Length - 8;
            Buffer.BlockCopy(byteASN, nOffset, asn, 0, 8);
            for (int i = 0; i < 8; i++)
                xorasn[i] = (byte)(asn[i] ^ 0xFF);
            //���ɷ�ɢ��Կ
            byte[] keyUpdate = new byte[16];
            byte[] KeyLeft = APDUBase.TripleEncryptData(asn, keyMAMK);
            byte[] KeyRight = APDUBase.TripleEncryptData(xorasn, keyMAMK);
            Buffer.BlockCopy(KeyLeft, 0, keyUpdate, 0, 8);
            Buffer.BlockCopy(KeyRight, 0, keyUpdate, 8, 8);
            return keyUpdate;
        }

    }
    public class APDUBase
    {
        private static byte[] DEFAULT_MF_NAME = new byte[] { 0x31, 0x50, 0x41, 0x59, 0x2E, 0x53, 0x59, 0x53, 0x2E, 0x44, 0x44, 0x46, 0x30, 0x31 };//"1PAY.SYS.DDF01"        
        public APDUBase()
        {
             
        }

        //��Ƭ�г�ʼ��Կ
       protected static byte[] m_KeyOrg = new byte[] { 0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4a, 0x4b, 0x4c, 0x4d, 0x4e, 0x4f };
             
        //MF�¿�Ƭ������Կ
        protected static byte[] m_KeyMain = new byte[] { 0xF2, 0x1B, 0x12, 0x34, 0x04, 0x38, 0x30, 0xD4, 0x48, 0x29, 0x3E, 0x66, 0x36, 0x88, 0x33, 0x78 };
        
        //��ƬӦ��������Կ
        protected static byte[] m_KeyAppMain = new byte[] { 0xF2, 0x1B, 0x12, 0x34, 0x04, 0x38, 0x30, 0xD4, 0x48, 0x29, 0x3E, 0x66, 0x36, 0x88, 0x33, 0xCC };

        //////////////////////////////////////////////////////////////////////////
        //PSAM�� ��ʼ��Կ
        protected static byte[] m_PsamKeyOrg = new byte[] { 0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4a, 0x4b, 0x4c, 0x4d, 0x4e, 0x4f };

        //PSAM����MF�¿�Ƭ������Կ
        protected static byte[] m_KeyPsamMain = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f };

        protected int m_nTotalLen = 0;        
        /// <summary>
        /// APDU����
        /// </summary>
        protected byte m_CLA = 0;
        protected byte m_INS = 0;
        protected byte m_P1 = 0;
        protected byte m_P2 = 0;
        protected byte m_Lc = 0;
        protected byte[] m_Data = null;
        protected byte m_le = 0;

        //��Ƭ����
        public enum CardCategory
        {
            CpuCard,  //CPU��
            PsamCard  //PSAM��
        }

        public byte[] GetOutputCmd()
        {
            byte[] outByte = new byte[m_nTotalLen];
            outByte[0] = m_CLA;
            outByte[1] = m_INS;
            outByte[2] = m_P1;
            outByte[3] = m_P2;
            int nOffset = 4;
            if (m_Lc > 0)
            {
                outByte[nOffset] = m_Lc;//m_Data������ʱ��m_LcҲ������
                nOffset++;
                Buffer.BlockCopy(m_Data, 0, outByte, nOffset, m_Lc);
                nOffset += m_Lc;
            }
            if (m_nTotalLen > nOffset)
                outByte[nOffset] = m_le;
            return outByte;
        }

        public void SetOrgKeyValue(byte[] byteKey,CardCategory eCategory)
        {
            if (byteKey.Length != 16)
                return;           
            if(eCategory == CardCategory.CpuCard)
                Buffer.BlockCopy(byteKey, 0, m_KeyOrg, 0, 16);
            else if(eCategory == CardCategory.PsamCard)
                Buffer.BlockCopy(byteKey, 0, m_PsamKeyOrg, 0, 16);
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
            if (bOrg)
            {
                if (eCategory == CardCategory.CpuCard)
                    return m_KeyOrg;
                else if (eCategory == CardCategory.PsamCard)
                    return m_PsamKeyOrg;
                else
                    return null;
            }
            else
            {
                if (eCategory == CardCategory.CpuCard)
                    return m_KeyMain;
                else if (eCategory == CardCategory.PsamCard)
                    return m_KeyPsamMain;
                else
                    return null;
            }
        }

        public static byte[] GetEncryptKey(bool bAppMainKey)
        {
            if (bAppMainKey)
            {
                //�����ɢ��Կʱʹ��
                return m_KeyAppMain;//Ӧ��������Կ
            }
            else
            {

                //��װӦ��������Կʱʹ�ã���װ��������Կ��װ��Ҫ��ɢ
                return m_KeyOrg;//��Ƭ�г�ʼ��Կ
            }
        }

        //���ܷ����� 8�ֽ�Ϊ1�顣��Կ��벿�ֶ����ݽ��м��ܣ�Ȼ�����Ұ벿�ֽ��ܣ�������벿�ּ��ܵõ����
        public static byte[] TripleEncryptData(byte[] byteSrc, byte[] TripleKey)
        {
            if (byteSrc.Length % 8 != 0)
                return byteSrc;

            int nSrcLen = byteSrc.Length;

            byte[] KeyLeft = new byte[8];
            byte[] KeyRight = new byte[8];
            Buffer.BlockCopy(TripleKey, 0, KeyLeft, 0, 8);
            Buffer.BlockCopy(TripleKey, 8, KeyRight, 0, 8);

            byte[] DataReturn = new byte[nSrcLen];
            byte[] calcData = new byte[8];
            
            int nCalcOffset = 0;
            int nCalcCount = nSrcLen / 8;//��Ȼ������
            for (int nCalcIndex = 0; nCalcIndex < nCalcCount; nCalcIndex++)
            {
                Buffer.BlockCopy(byteSrc, nCalcOffset, calcData, 0, 8);
                EncryptData(calcData, KeyLeft);//����Կ����벿�����м��ܲ���
                DecryptData(calcData, KeyRight);//�Ұ벿����
                EncryptData(calcData, KeyLeft); //��벿����
                Buffer.BlockCopy(calcData, 0, DataReturn, nCalcOffset, 8);
                nCalcOffset += 8;
            }

            return DataReturn;
        }


        //���ܷ����� 8�ֽ�Ϊ1�顣��Կ��벿�ֶ����ݽ��н��ܣ�Ȼ�����Ұ벿�ּ��ܣ�������벿�ֽ��ܵõ����
        public static byte[] TripleDecryptData(byte[] byteSrc, byte[] TripleKey)
        {
            if (byteSrc.Length % 8 != 0)
                return byteSrc;

            int nSrcLen = byteSrc.Length;

            byte[] KeyLeft = new byte[8];
            byte[] KeyRight = new byte[8];
            Buffer.BlockCopy(TripleKey, 0, KeyLeft, 0, 8);
            Buffer.BlockCopy(TripleKey, 8, KeyRight, 0, 8);

            byte[] DataReturn = new byte[nSrcLen];
            byte[] calcData = new byte[8];

            int nCalcOffset = 0;
            int nCalcCount = nSrcLen / 8;//��Ȼ������
            for (int nCalcIndex = 0; nCalcIndex < nCalcCount; nCalcIndex++)
            {
                Buffer.BlockCopy(byteSrc, nCalcOffset, calcData, 0, 8);
                DecryptData(calcData, KeyLeft);//����Կ����벿�����н��ܲ���
                EncryptData(calcData, KeyRight);//�Ұ벿����
                DecryptData(calcData, KeyLeft);//�����߽���
                Buffer.BlockCopy(calcData, 0, DataReturn, nCalcOffset, 8);
                nCalcOffset += 8;
            }

            return DataReturn;
        }

        public static bool ByteDataEquals(byte[] byteL, byte[] byteR)
        {
            if(byteL.Length != byteR.Length)
                return false;
            for (int i = 0; i < byteL.Length; i++)
            {
                if (byteL[i] != byteR[i])
                    return false;
            }
            return true;
        }

        //ѡ��
        public bool createSelectCmd(byte[] byteName, byte[] prefixData)
        {
            if (byteName == null || byteName.Length < 5 || byteName.Length > 16)
                return false;
            bool bMF = false;
            if (prefixData == null && ByteDataEquals(byteName,DEFAULT_MF_NAME) )
                bMF = true;                
            m_CLA = 0x00;
            m_INS = 0xA4;
            m_P1 = 0x04;
            m_P2 = 0x00;
            int nLen = byteName.Length;
            if (!bMF)
            {
                nLen += prefixData.Length;
            }
            m_Lc = (byte)nLen; //1. MF 0x0e   / 2. ADF name.length + prefixData.length        
            m_Data = new byte[nLen];
            if (bMF)
            {
                Buffer.BlockCopy(byteName, 0, m_Data, 0, byteName.Length);
            }
            else
            {
                for (int i = 0; i < prefixData.Length; i++)
                {
                    m_Data[i] = prefixData[i];
                }
                Buffer.BlockCopy(byteName, 0, m_Data, prefixData.Length, byteName.Length);
            }
            m_le = 0;
            m_nTotalLen = 5 + nLen;
            return true;
        }

        //ȡ�����
        public bool createGetChallengeCmd(int nRandLen)
        {
            if (nRandLen != 4 && nRandLen != 8)
                return false;
            m_CLA = 0x00;
            m_INS = 0x84;
            m_P1 = 0x00;
            m_P2 = 0x00;            
            m_Lc = 0x00;  //������
            m_Data = null; //������
            m_le = (byte)nRandLen;   //ָ��������ֽ�
            m_nTotalLen = 5;
            return true;
        }

        //COS�汾
        public bool createCosVersionCmd()
        {
            m_CLA = 0x00;
            m_INS = 0xCA;
            m_P1 = 0x9F;
            m_P2 = 0x80;
            m_Lc = 0x00;  //������
            m_Data = null; //������
            m_le = 3;
            m_nTotalLen = 5;
            return true;
        }

        //�ⲿ��֤
        /// <summary>
        /// 
        /// </summary>
        /// <param name="byteRandom">�����,ÿ�����������ֻʹ��1��</param>        
        /// <param name="KeyValue">ʹ�õ���Կ:��ʼ��Կ/������Կ</param>
        /// <returns></returns>
        public bool createExternalAuthenticationCmd(byte[] byteRandom, byte[] KeyValue)
        {
            if (byteRandom.Length != 4 && byteRandom.Length != 8)
                return false;

            m_CLA = 0x00;
            m_INS = 0x82;
            m_P1 = 0x00;   //�ް�ȫ��Ϣ
            m_P2 = 0x00;
            m_Lc = 0x08;

            //byteRandom.Length==4ʱ��4�ֽ�Ϊ0x00�����ܺ�õ���֤����
            byte[] baseData = new byte[8];
            Buffer.BlockCopy(byteRandom, 0, baseData, 0, byteRandom.Length);
            byte[] cryptData = TripleEncryptData(baseData, KeyValue);
            
            m_Data = new byte[8];//����
            Buffer.BlockCopy(cryptData, 0, m_Data, 0, 8);            
            m_le = 0;
            m_nTotalLen = 13;
            return true;
        }

        /// <summary>
        /// ɾ��MF
        /// </summary>
        /// <param name="byteRandom">�����,ÿ�����������ֻʹ��1��</param>
        /// <param name="KeyValue">ʹ�õ���Կ:��ʼ��Կ/������Կ</param>
        /// <returns></returns>
        public bool createClearMFcmd(byte[] byteRandom, byte[] KeyValue)
        {
            if (byteRandom.Length != 4 && byteRandom.Length != 8)
                return false;

            m_CLA = 0x80;
            m_INS = 0xE4;
            m_P1 = 0x00;
            m_P2 = 0x00;
            m_Lc = 0x08;

            //byteRandom.Length==4ʱ��4�ֽ�Ϊ0x00�����ܺ�õ���֤����
            byte[] baseData = new byte[8];
            Buffer.BlockCopy(byteRandom, 0, baseData, 0, byteRandom.Length);
            byte[] cryptData = TripleEncryptData(baseData, KeyValue);

            m_Data = new byte[8];//����
            Buffer.BlockCopy(cryptData, 0, m_Data, 0, 8);
            m_le = 0;
            m_nTotalLen = 13;
            return true;
        }

        public byte[] GetKeyVal(bool bMainKey, CardCategory eCategory)
        {
            byte[] key = null;
            if (bMainKey)
            {
                if (eCategory == CardCategory.CpuCard)
                    key = m_KeyMain;
                else if (eCategory == CardCategory.PsamCard)
                    key = m_KeyPsamMain;
            }
            else
            {
                if (eCategory == CardCategory.CpuCard)
                    key = m_KeyOrg;
                else if (eCategory == CardCategory.PsamCard)
                    key = m_PsamKeyOrg;
            }
            return key;
        }

        public bool createGetEFFileCmd(byte fileFlag, byte ReponseLen)
        {
            m_CLA = 0x00;
            m_INS = 0xB0;
            m_P1 = fileFlag; //���ļ���ʶ�����ļ�
            m_P2 = 0x00;
            m_Lc = 0x00;  //������
            m_Data = null; //������
            m_le = ReponseLen;   //���س���
            m_nTotalLen = 5;
            return true;
        }

        protected byte XorValue(byte[] data)
        {
            byte byteRetVal = 0;
            for (int i = 0; i < data.Length; i++)
            {
                byteRetVal ^= data[i];
            }
            return byteRetVal;
        }

        //�����������ݵ�MACֵ��Ϊ������ȷ����֤
        protected static byte[] CalcMACValue(byte[] srcData, byte[] KeyValue, byte[] InitVal)
        {
            byte[] MacResult = new byte[8];//EncryptData��DecryptData�����л��޸ĸ�ֵ

            Buffer.BlockCopy(InitVal, 0, MacResult, 0, 8);

            int nAppendLen =  8 - (srcData.Length % 8);
            int nSrcLen = srcData.Length + nAppendLen;
            byte[] calcData = new byte[nSrcLen];
            Buffer.BlockCopy(srcData, 0, calcData, 0, srcData.Length);
            for (int i = 0; i < nAppendLen; i++)
            {                
                if(i == 0)                    
                    calcData[srcData.Length + i] = 0x80;
                else
                    calcData[srcData.Length + i] = 0x00;
            }

            byte[] MacData = new byte[8];

            byte[] KeyLeft = new byte[8];
            byte[] KeyRight = new byte[8];
            Buffer.BlockCopy(KeyValue,0,KeyLeft,0,8);
            Buffer.BlockCopy(KeyValue,8,KeyRight,0,8);

            int nCalcOffset = 0;
            int nCalcCount = nSrcLen / 8;//��Ȼ������
            for(int nCalcIndex = 0; nCalcIndex < nCalcCount; nCalcIndex++)
            {
                Buffer.BlockCopy(calcData, nCalcOffset, MacData, 0, 8);
                for (int i = 0; i < 8; i++)
                {
                    MacResult[i] ^= MacData[i];//�������                
                }
                EncryptData(MacResult, KeyLeft);//����Կ��ǰ�벿�����м��ܲ���
                nCalcOffset += 8;                
            }
            //ʹ�õ���8�ֽ���Կ�����һ�����ݿ�Ĵ���
            DecryptData(MacResult, KeyRight);//����Կ�ĺ�벿�����н��ܲ���
            EncryptData(MacResult, KeyLeft);//��ǰ�벿����
            
            byte[] MACData = new byte[4];
            Buffer.BlockCopy(MacResult, 0, MACData, 0, 4);
            return MACData;
            
        }

        //�ù�����Կ����MAC1��MAC2ֵ
        public byte[] CalcMacVal(byte[] srcData, byte[] keytoMac)
        {
            byte[] CryptData = SingleDesCalc(srcData, keytoMac);

            byte[] MACData = new byte[4];
            Buffer.BlockCopy(CryptData, 0, MACData, 0, 4);
            return MACData;
        }

        public byte[] CalcPrivateProcessKey(byte[] srcData, byte[] tmpck)
        {
            if (srcData.Length != 4)
                return null;
            //srcData��4�ֽڣ�������Ҫ����0x80 0x00 0x00 0x00��
            return SingleDesCalc(srcData, tmpck);
        }

        protected static byte[] SingleDesCalc(byte[] srcData, byte[] KeyValue)
        {
            byte[] MacResult = new byte[8];//��ʼֵȫ0
            int nAppendLen = 8 - (srcData.Length % 8);
            int nSrcLen = srcData.Length + nAppendLen;
            byte[] calcData = new byte[nSrcLen];
            Buffer.BlockCopy(srcData, 0, calcData, 0, srcData.Length);
            for (int i = 0; i < nAppendLen; i++)
            {
                if (i == 0)
                    calcData[srcData.Length + i] = 0x80;
                else
                    calcData[srcData.Length + i] = 0x00;
            }

            byte[] MacData = new byte[8];

            int nCalcOffset = 0;
            int nCalcCount = nSrcLen / 8;//��Ȼ������
            for (int nCalcIndex = 0; nCalcIndex < nCalcCount; nCalcIndex++)
            {
                Buffer.BlockCopy(calcData, nCalcOffset, MacData, 0, 8);
                for (int i = 0; i < 8; i++)
                {
                    MacResult[i] ^= MacData[i];//�������                
                }
                EncryptData(MacResult, KeyValue);//���м��ܲ���
                nCalcOffset += 8;
            }

            return MacResult;
        }

        //DES����
        private static void EncryptData(byte[] byteData, byte[] Key)
        {
            if (byteData.Length != 8 || Key.Length != 8)
            {
                Trace.WriteLine("DES���ܵ����ݳ��Ȼ���Կ���Ȳ���ȷ");
                return;
            }

            DESCryptoServiceProvider alg = new DESCryptoServiceProvider();

            alg.Padding = PaddingMode.Zeros; //ȫ0��䣬ʹ�����������һ��
            alg.Key = Key;//��Կ
            alg.IV = new byte[8];//��ʼ��������ȫ0

            ICryptoTransform ITrans = alg.CreateEncryptor(alg.Key, alg.IV);
            byte[] outData = ITrans.TransformFinalBlock(byteData, 0, byteData.Length);
            Buffer.BlockCopy(outData, 0, byteData, 0, 8);
            ITrans.Dispose();
            alg.Clear();
        }

        //DES����
        private static void DecryptData(byte[] byteData, byte[] Key)
        {
            if (byteData.Length != 8 || Key.Length != 8)
            {
                Trace.WriteLine("DES���ܵ����ݳ��Ȼ���Կ���Ȳ���ȷ");
                return;
            }

            DESCryptoServiceProvider alg = new DESCryptoServiceProvider();
            alg.Padding = PaddingMode.Zeros; //ȫ0��䣬ʹ�����������һ��
            alg.Key = Key;//��Կ
            alg.IV = new byte[8];//��ʼ��������ȫ0

            ICryptoTransform ITrans = alg.CreateDecryptor(alg.Key, alg.IV);
            byte[] outData = ITrans.TransformFinalBlock(byteData, 0, byteData.Length);
            Buffer.BlockCopy(outData, 0, byteData, 0, 8);
            ITrans.Dispose();
            alg.Clear();            
        }

        protected byte[] GetBCDDate(DateTime dateData)
        {
            byte[] byteReturn = new byte[4];
            string strDate = dateData.ToString("yyyyMMdd");
            for (int i = 0; i < 4; i++)
            {
                byteReturn[i] = Convert.ToByte(strDate.Substring(i * 2, 2), 16);
            }
            return byteReturn;        
        }

        public static byte[] StringToBCD(string strData)
        {
            if (string.IsNullOrEmpty(strData) || strData.Length % 2 != 0)
                return null;
            int nByteSize = strData.Length / 2;
            byte[] byteBCD = new byte[nByteSize];
            for (int i = 0; i < nByteSize; i++)
            {
                byte bcdbyte = 0;
                byte.TryParse(strData.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber, null, out bcdbyte);
                byteBCD[i] = bcdbyte;
            }
            return byteBCD;   
        }

    }
}
