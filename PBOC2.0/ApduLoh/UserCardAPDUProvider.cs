using System;
using System.Collections.Generic;
using System.Text;
using ApduParam;
using ApduInterface;

namespace ApduLoh
{
    public class UserCardAPDUProvider : APDULohBase , IUserApduProvider
    {
        public UserCardAPDUProvider()
        {

        }

        public bool createGenerateFCICmd()
        {
            m_CLA = 0x80;
            m_INS = 0xE0;
            m_P1 = 0x01;
            m_P2 = 0x00;
            int nLen = 10;
            m_Lc = (byte)nLen;//0x0A
            m_Data = new byte[nLen];
            //File Id
            m_Data[0] = 0xEF;
            m_Data[1] = 0x0A;
            //File Type
            m_Data[2] = 0x1C;//FCI
            //File Size
            m_Data[3] = 0x01;
            m_Data[4] = 0xFF;
            //Key Index
            m_Data[5] = 0x00;
            //ACr
            m_Data[6] = 0x00;
            m_Data[7] = 0x00;
            //ACw
            m_Data[8] = 0x00;
            m_Data[9] = 0x00;
            m_le = 0;
            m_nTotalLen = 15;
            return true;
        }

        public bool createStorageFCICmd(string strName, byte[] param, byte[] prefix)
        {
            if (string.IsNullOrEmpty(strName) || strName.Length < 5 || strName.Length > 16)
                return false;
            int nNameLen = strName.Length;
            m_CLA = 0x00;
            m_INS = 0xDC;
            m_P1 = 0x01;
            m_P2 = 0x54;
            int nLen = nNameLen + 6 + param.Length;
            if (prefix != null)
                nLen += prefix.Length;
            m_Lc = (byte)nLen;
            m_Data = new byte[nLen];
            //Tag
            m_Data[0] = 0x6F;
            //Len
            int nLen6F = nNameLen + 4 + param.Length; //�������г���
            if (prefix != null)
                nLen6F += prefix.Length;
            m_Data[1] = (byte)nLen6F;
            //Tag
            m_Data[2] = 0x84;
            //Len
            int nLen84 = nNameLen;
            if (prefix != null)
                nLen84 += prefix.Length;
            m_Data[3] = (byte)nLen84; //Name���ȣ�����A0 00 00 00 03��
            int nOffset = 4;
            if (prefix != null)
            {
                Buffer.BlockCopy(prefix, 0, m_Data, nOffset, prefix.Length);
                nOffset += prefix.Length;
            }
            byte[] byteName = Encoding.ASCII.GetBytes(strName);
            Buffer.BlockCopy(byteName, 0, m_Data, nOffset, nNameLen);
            nOffset += nNameLen;
            //Tag
            m_Data[nOffset] = 0xA5;
            nOffset += 1;
            //Len
            m_Data[nOffset] = (byte)param.Length;
            nOffset += 1;
            //Value{{
            Buffer.BlockCopy(param, 0, m_Data, nOffset, param.Length);
            //}}
            m_le = 0;
            m_nTotalLen = 5 + nLen; //APDU Len
            return true;
        }

        //����Ŀ¼�ļ�1(nFileIndex = 1, strFileName = "ENN ENERGY")
        //Ŀ¼�ļ�2(nFileIndex = 2, strFileName = "ENN LOYALTY"),Ŀ¼�ļ�3(nFileIndex = 3, strFileName = "ENN SV")
        public bool createUpdateEF01Cmd(byte nFileIndex, string strFileName)
        {
            if (string.IsNullOrEmpty(strFileName) || strFileName.Length < 5 || strFileName.Length > 16)
                return false;
            int nNameLen = strFileName.Length;
            m_CLA = 0x00;
            m_INS = 0xDC;
            m_P1 = nFileIndex;
            m_P2 = 0x0C;
            int nLen = 9 + nNameLen;
            m_Lc = (byte)nLen;
            m_Data = new byte[nLen];
            //Tag
            m_Data[0] = 0x61;
            //Len
            m_Data[1] = (byte)(7 + nNameLen);
            //Tag
            m_Data[2] = 0x4F;
            //Len
            m_Data[3] = (byte)(5 + nNameLen); //0x0F
            //
            m_Data[4] = 0xA0;
            m_Data[5] = 0x00;
            m_Data[6] = 0x00;
            m_Data[7] = 0x00;
            m_Data[8] = 0x03;
            byte[] byteName = Encoding.ASCII.GetBytes(strFileName);
            Buffer.BlockCopy(byteName, 0, m_Data, 9, nNameLen);
            m_le = 0;
            m_nTotalLen = 5 + nLen;
            return true;
        }

        public bool createGenerateKeyCmd()
        {
            m_CLA = 0x80;
            m_INS = 0xE0;
            m_P1 = 0x01;
            m_P2 = 0x00;
            int nLen = 11;
            m_Lc = (byte)nLen;
            m_Data = new byte[nLen];
            //File ID
            m_Data[0] = 0xEF;
            m_Data[1] = 0x00;
            //File Type
            m_Data[2] = 0x22;
            //Record Number
            m_Data[3] = 0x10;
            //Record Length
            m_Data[4] = 25;
            //Key Index
            m_Data[5] = 0x00;
            //ACr
            m_Data[6] = 0xFF;
            m_Data[7] = 0xFF;
            //ACw
            m_Data[8] = 0xFF;
            m_Data[9] = 0xFF;
            //ACu
            m_Data[10] = 0xFF;//��Կ�ļ����·�ʽ������+MAC��
            m_le = 0;
            m_nTotalLen = 16;
            return true;
        }

        //��װ������Կ
        public bool createStorageKeyCmd(byte[] RandomVal, byte[] StorageKey, byte[] EncryptKey)
        {
            if (RandomVal.Length != 8)
                return false;
            m_CLA = 0x80;
            m_INS = 0xD4;
            m_P1 = 0x00;//����Կ
            m_P2 = 0x01; //��Կ�ļ���һ����¼
            int nLen = 36;  //0x24 = ����+MAC
            m_Lc = (byte)nLen; //

            byte[] bufferData = new byte[32];
            //len
            bufferData[0] = 24;//0x18
            //key type
            bufferData[1] = 0x49;
            //key index
            bufferData[2] = 0x00;
            //key version
            bufferData[3] = 0x01;
            //Err Num
            bufferData[4] = 0xFF;
            //Algorithm
            bufferData[5] = 0x00;
            //Change AC
            bufferData[6] = 0x00;
            bufferData[7] = 0x00;
            //PriValue
            bufferData[8] = 0x01;
            Buffer.BlockCopy(StorageKey, 0, bufferData, 9, 16); //MF������Կ��Ϊ����
            //����32�ֽڼ�������
            int nAppendLen = 7;
            for (int i = 0; i < nAppendLen; i++)
            {
                if (i == 0)
                    bufferData[25 + i] = 0x80;
                else
                    bufferData[25 + i] = 0x00;
            }
            byte[] cryptData = DesCryptography.TripleEncryptData(bufferData, EncryptKey);//��ԭʼ��Կ����

            m_Data = new byte[nLen];//���� + MAC
            Buffer.BlockCopy(cryptData, 0, m_Data, 0, 32);
            byte[] srcMacData = new byte[37]; //ͷ5 +����32
            srcMacData[0] = m_CLA;//����Ҫ����ֽ�Ϊ4
            srcMacData[1] = m_INS;
            srcMacData[2] = m_P1;
            srcMacData[3] = m_P2;
            srcMacData[4] = m_Lc;
            Buffer.BlockCopy(cryptData, 0, srcMacData, 5, 32);
            byte[] byteMAC = CalcMACValue(srcMacData, EncryptKey, RandomVal);//ԭʼ��Կ����MAC
            Buffer.BlockCopy(byteMAC, 0, m_Data, 32, 4);
            m_le = 0;
            m_nTotalLen = 5 + nLen;
            return true;
        }

        public bool createGenerateADFCmd(string strName)
        {
            if (string.IsNullOrEmpty(strName) || strName.Length < 5 || strName.Length > 16)
                return false;
            int nNameLen = strName.Length;
            m_CLA = 0x80;
            m_INS = 0xE0;
            m_P1 = 0x00;
            m_P2 = 0x00;
            int nLen = 12 + nNameLen;
            m_Lc = (byte)nLen;
            m_Data = new byte[nLen];
            //File ID
            m_Data[0] = 0xDF;
            m_Data[1] = 0x01;
            //File Type
            m_Data[2] = 0x39;
            //
            m_Data[3] = 0x00;
            m_Data[4] = 0x01;
            //
            m_Data[5] = 0xAA;
            //Length
            m_Data[6] = (byte)(5 + nNameLen);//0x0F
            //
            m_Data[7] = 0xA0;
            m_Data[8] = 0x00;
            m_Data[9] = 0x00;
            m_Data[10] = 0x00;
            m_Data[11] = 0x03;

            byte[] byteName = Encoding.ASCII.GetBytes(strName);
            Buffer.BlockCopy(byteName, 0, m_Data, 12, nNameLen);
            m_le = 0;
            m_nTotalLen = 5 + nLen;
            return true;
        }

        public bool createADFStorageFCICmd(string strName)
        {
            if (string.IsNullOrEmpty(strName) || strName.Length < 5 || strName.Length > 16)
                return false;
            int nNameLen = strName.Length;
            m_CLA = 0x00;
            m_INS = 0xDC;
            m_P1 = 0x01;
            m_P2 = 0x54;
            int nLen = 20 + nNameLen;  //Data Len
            m_Lc = (byte)nLen; //0x1A
            m_Data = new byte[nLen];
            //Tag
            m_Data[0] = 0x6F;
            //Len
            m_Data[1] = 27;
            //Tag
            m_Data[2] = 0x84;
            //Len
            m_Data[3] = (byte)(5 + nNameLen); //0x0F

            m_Data[4] = 0xA0;
            m_Data[5] = 0x00;
            m_Data[6] = 0x00;
            m_Data[7] = 0x00;
            m_Data[8] = 0x03;

            byte[] byteName = Encoding.ASCII.GetBytes(strName);
            int nOffset = 9;
            Buffer.BlockCopy(byteName, 0, m_Data, nOffset, nNameLen);
            nOffset += nNameLen;
            //Tag
            m_Data[nOffset] = 0xA5;
            //Len
            m_Data[nOffset + 1] = 0x9;
            //Tag
            m_Data[nOffset + 2] = 0x9F;
            m_Data[nOffset + 3] = 0x08;
            //Ӧ�ð汾��
            m_Data[nOffset + 4] = 0x01;
            m_Data[nOffset + 5] = 0x01;
            //Tag
            m_Data[nOffset + 6] = 0xBF;
            m_Data[nOffset + 7] = 0x0C;
            //����������
            m_Data[nOffset + 8] = 0x02;
            m_Data[nOffset + 9] = 0x55;
            m_Data[nOffset + 10] = 0x66;
            //
            m_le = 0;
            m_nTotalLen = 5 + nLen; //APDU Len
            return true;
        }

        /// <summary>
        ///  ����EF�ļ�
        /// </summary>
        /// <param name="fileID">�ļ�ID</param>
        /// <param name="fileType">���ͣ�͸���ļ�0x01</param>
        /// <param name="fileLen">�ļ���С��͸���ļ���</param>
        /// <param name="keyIndex">Key Index</param>
        /// <param name="RecordNum">��¼������¼�ļ���</param>
        /// <param name="RecordLen">��¼���ȣ���¼�ļ���</param>
        /// <param name="ACr">��Ȩ��</param>
        /// <param name="ACw">дȨ��</param>
        /// <returns></returns>
        public bool createGenerateEFCmd(ushort fileID, byte fileType, ushort fileLen, byte keyIndex, byte RecordNum, byte RecordLen, ushort ACr, ushort ACw)
        {
            m_CLA = 0x80;
            m_INS = 0xE0;
            m_P1 = 0x01;
            m_P2 = 0x00;
            int nCmdLen = 10;
            if (fileID == 0xEF03 && fileType == 0x2A)//PIN�ļ���һ�ֽ�PriValue
                nCmdLen = 11;
            m_Lc = (byte)nCmdLen;
            m_Data = new byte[nCmdLen];
            m_Data[0] = (byte)((fileID >> 8) & 0xff);
            m_Data[1] = (byte)(fileID & 0xff);
            m_Data[2] = fileType;
            if (RecordNum == 0 && RecordLen == 0)
            {
                //͸���ļ�/����EF�ļ�
                m_Data[3] = (byte)((fileLen >> 8) & 0xff);
                m_Data[4] = (byte)(fileLen & 0xff);
            }
            else
            {
                //��¼�ļ�
                m_Data[3] = RecordNum;
                m_Data[4] = RecordLen;
            }
            m_Data[5] = keyIndex;
            m_Data[6] = (byte)((ACr >> 8) & 0xff);
            m_Data[7] = (byte)(ACr & 0xff);
            m_Data[8] = (byte)((ACw >> 8) & 0xff);
            m_Data[9] = (byte)(ACw & 0xff);
            m_le = 0;
            m_nTotalLen = 15;
            if (fileID == 0xEF03 && fileType == 0x2A)
            {
                m_Data[10] = 0x00;
                m_nTotalLen = 16;
            }
            return true;
        }

        public bool createStorageApplicationCmd()
        {
            m_CLA = 0x00;
            m_INS = 0xDC;
            m_P1 = 0x01;
            m_P2 = 0x2C;
            int nLen = 29;  //Data Len
            m_Lc = (byte)nLen; //0x1D
            m_Data = new byte[nLen];//����ȫ0          
            //
            m_le = 0;
            m_nTotalLen = 5 + nLen; //APDU Len
            return true;
        }

        public bool createStoragePINFileCmd(bool bDefaultPwd, byte[] customPwd)
        {
            if(!bDefaultPwd && customPwd.Length != 6)
                return false;            
            m_CLA = 0x00;
            m_INS = 0xDC;
            m_P1 = 0x01;
            m_P2 = 0x1C;
            int nLen = 15;  //Data Len
            m_Lc = (byte)nLen; //0x0F
            m_Data = new byte[nLen];//  
            m_Data[0] = 0x01; //Pin Index
            m_Data[1] = 0x55; //ErrNum
            //Change AC
            m_Data[2] = 0x00;
            m_Data[3] = 0x00;
            //PriValue
            m_Data[4] = 0x02;
            //Pin Len
            m_Data[5] = 0x03;
            //PIN
            if (bDefaultPwd)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (i < 3)
                        m_Data[6 + i] = 0x99;                    
                    else
                        m_Data[6 + i] = 0xFF;                    
                }
            }
            else
            {
                for (int i = 0; i < 8; i++)
                {
                    if (i < 3)
                            m_Data[6 + i] = (byte)((customPwd[i * 2] << 4) | customPwd[i * 2 + 1]);                   
                    else                    
                        m_Data[6 + i] = 0xFF;                    
                }
            }
            m_Data[14] = XorValue(m_Data);//���һ��m_DataΪ0����Ӱ��XOR���
            m_le = 0;
            m_nTotalLen = 5 + nLen; //APDU Len
            return true;
        }        

        private byte XorValue(byte[] data)
        {
            byte byteRetVal = 0;
            for (int i = 0; i < data.Length; i++)
            {
                byteRetVal ^= data[i];
            }
            return byteRetVal;
        }

        //��Ʊ������ԿEF01
        public bool createGenerateEFKeyCmd()
        {
            m_CLA = 0x80;
            m_INS = 0xE0;
            m_P1 = 0x01;
            m_P2 = 0x00;
            int nCmdLen = 11;
            m_Lc = (byte)nCmdLen;
            m_Data = new byte[nCmdLen];
            //File ID
            m_Data[0] = 0xEF;
            m_Data[1] = 0x01;
            //File Type
            m_Data[2] = 0x22;
            //Record Num
            m_Data[3] = 0x11;
            //Record Len
            m_Data[4] = 0x19;
            //key index
            m_Data[5] = 0x00;
            //ACr
            m_Data[6] = 0xFF;
            m_Data[7] = 0xFF;
            //ACw
            m_Data[8] = 0xFF;
            m_Data[9] = 0xFF;
            //ACu
            m_Data[10] = 0x03;
            m_le = 0;
            m_nTotalLen = 16;
            return true;
        }

        public bool createWriteUserKeyCmd(byte[] randVal, StorageKeyParam Param)
        {
            m_CLA = 0x80;
            m_INS = 0xD4;
            m_P1 = 0x00;
            m_P2 = Param.P2;
            int nCmdLen = 36;
            m_Lc = (byte)nCmdLen;
            m_Data = new byte[nCmdLen];

            byte[] encryptAsn = DesCryptography.TripleEncryptData(Param.ASN, Param.StorageKey);
            byte[] encryptXorAsn = DesCryptography.TripleEncryptData(Param.XorASN, Param.StorageKey);
            if (encryptAsn.Length != 8 || encryptXorAsn.Length != 8)
                return false;

            int nEncryptLen = 9 + 16;//ǰ�����9���ֽ�
            int nAddLen = 8 - (nEncryptLen % 8);//==7
            byte[] encryptAll = new byte[nEncryptLen + nAddLen];            //len
            encryptAll[0] = 24;//0x18 = 8 (ͷ)+ 8������3DES�� + 8���������0xFF����3DES��
            //key type
            encryptAll[1] = Param.KeyType;
            //key index
            encryptAll[2] = Param.KeyIndex;
            //key version
            encryptAll[3] = 0x01;
            //Err Num
            encryptAll[4] = Param.ErrCount;
            //Algorithm
            encryptAll[5] = 0x00;
            //Change AC
            encryptAll[6] = 0x00;
            encryptAll[7] = 0x00;
            //PriValue
            encryptAll[8] = Param.PriValue;
            Buffer.BlockCopy(encryptAsn, 0, encryptAll, 9, 8);
            Buffer.BlockCopy(encryptXorAsn, 0, encryptAll, 17, 8);
            for (int i = 0; i < nAddLen; i++)
            {
                if (i == 0)
                    encryptAll[nEncryptLen + i] = 0x80;
                else
                    encryptAll[nEncryptLen + i] = 0x00;
            }
            byte[] AllData = DesCryptography.TripleEncryptData(encryptAll, Param.EncryptKey);

            m_Data = new byte[nCmdLen];//���� + MAC
            Buffer.BlockCopy(AllData, 0, m_Data, 0, 32);
            byte[] srcMacData = new byte[37]; //ͷ5 +����32
            srcMacData[0] = m_CLA;//����Ҫ����ֽ�Ϊ4
            srcMacData[1] = m_INS;
            srcMacData[2] = m_P1;
            srcMacData[3] = m_P2;
            srcMacData[4] = m_Lc;
            Buffer.BlockCopy(AllData, 0, srcMacData, 5, 32);
            byte[] byteMAC = CalcMACValue(srcMacData, Param.EncryptKey, randVal);//ԭʼ��Կ����MAC
            Buffer.BlockCopy(byteMAC, 0, m_Data, 32, 4);
            m_le = 0;
            m_nTotalLen = 5 + nCmdLen;
            return true;
        }

        public bool createSetStatusCmd(byte[] RandomVal, byte[] keyCalc)
        {
            if (RandomVal.Length != 8)
                return false;
            m_CLA = 0x80;
            m_INS = 0xF1;
            m_P1 = 0x00;
            m_P2 = 0x00;
            m_Lc = 0x08;
            //��Կ��װ�����÷�ɢ��Կ������������ת��
            //�л���ʹ��������MF������Կ������������ת��
            byte[] cryptData = DesCryptography.TripleEncryptData(RandomVal, keyCalc);
            m_Data = new byte[8];
            Buffer.BlockCopy(cryptData, 0, m_Data, 0, 8);
            m_le = 0;
            m_nTotalLen = 13;
            return true;
        }

        public bool createUpdateEF15FileCmd(byte[] key, byte[] RandomVal, byte[] ASN, DateTime dateBegin, DateTime dateEnd)
        {
            if (RandomVal.Length != 8 || ASN.Length != 8)
                return false;
            m_CLA = 0x04;
            m_INS = 0xD6;
            m_P1 = 0x95;
            m_P2 = 0x00;
            int nLen = 32;
            m_Lc = (byte)nLen;
            m_Data = new byte[nLen];
            //��������ʶ
            m_Data[0] = 0x35;
            m_Data[1] = 0xFF;
            m_Data[2] = 0xFF;
            m_Data[3] = 0xFF;
            m_Data[4] = 0xFF;
            m_Data[5] = 0xFF;
            m_Data[6] = 0xFF;
            m_Data[7] = 0xFF;
            //Ӧ�����ͱ�ʶ
            m_Data[8] = 0x11;
            m_Data[9] = 0x01;
            Buffer.BlockCopy(ASN, 0, m_Data, 10, 8);
            byte[] bcdDateBegin = GetBCDDate(dateBegin);//��������
            byte[] bcdDateEnd = GetBCDDate(dateEnd);    //��Ч����
            Buffer.BlockCopy(bcdDateBegin, 0, m_Data, 18, 4);
            Buffer.BlockCopy(bcdDateEnd, 0, m_Data, 22, 4);
            m_Data[27] = 0x01;//ָ��汾
            m_Data[28] = 0x00;//�Զ���FCI����

            byte[] srcMacData = new byte[33]; //ͷ5 +Data28
            srcMacData[0] = m_CLA;//����Ҫ����ֽ�Ϊ4
            srcMacData[1] = m_INS;
            srcMacData[2] = m_P1;
            srcMacData[3] = m_P2;
            srcMacData[4] = m_Lc;
            Buffer.BlockCopy(m_Data, 0, srcMacData, 5, 28);
            byte[] byteMAC = CalcMACValue(srcMacData, key, RandomVal);//����MAC
            Buffer.BlockCopy(byteMAC, 0, m_Data, 28, 4);
            m_le = 0;
            m_nTotalLen = 37;
            return true;
        }

        public bool createUpdateEF16FileCmd(byte[] key, byte[] RandomVal, UserCardInfoParam cardInfo)
        {
            if (RandomVal.Length != 8)
                return false;
            m_CLA = 0x04;
            m_INS = 0xD6;
            m_P1 = 0x96;
            m_P2 = 0x00;
            int nLen = 74;
            m_Lc = (byte)nLen;
            m_Data = new byte[nLen];
            for (int i = 0; i < nLen; i++)
            {
                m_Data[i] = 0xFF;
            }

            m_Data[0] = (byte)cardInfo.UserCardType;//������
            m_Data[1] = 0x00;//ְ����ʶ
            //�ֿ�������
            int nOffset = 2;
            if (!string.IsNullOrEmpty(cardInfo.UserName))
            {
                byte[] byteName = Encoding.Unicode.GetBytes(cardInfo.UserName);
                for (int i = 0; i < byteName.Length; i++)
                {
                    m_Data[nOffset + i] = byteName[i];
                }
            }
            nOffset += 20;
            //���֤��
            byte[] byteIdentity = null;
            if (!string.IsNullOrEmpty(cardInfo.UserIdentity))
                byteIdentity = Encoding.ASCII.GetBytes(cardInfo.UserIdentity);
            else
                byteIdentity = Encoding.ASCII.GetBytes("12345678901234567X");//Ĭ��ֵ
            for (int i = 0; i < byteIdentity.Length; i++)
            {
                m_Data[nOffset + i] = byteIdentity[i];
            }            
            nOffset += 18;
            //nOffset = 40
            m_Data[nOffset] = (byte)(cardInfo.IdType);//֤������
            nOffset += 1;
            if (!string.IsNullOrEmpty(cardInfo.UserAccount))
            {
                byte[] byteAccount = Encoding.ASCII.GetBytes(cardInfo.UserAccount);
                for (int i = 0; i < byteAccount.Length; i++)
                {
                    m_Data[nOffset + i] = byteAccount[i];
                }
            }
            nOffset += 10;            
            //�ۿ���
            byte[] byteRate = StringToBCD(cardInfo.DiscountRate.ToString("D4"));
            Buffer.BlockCopy(byteRate, 0, m_Data, 51, 2);
            byte[] bcdDate = GetBCDDate(cardInfo.DiscountRateEnd);//��������
            //�ۿ���Ч��,BCD��
            Buffer.BlockCopy(bcdDate, 0, m_Data, 53, 4);

            m_Data[61] = cardInfo.PriceLevel;
            
            byte[] srcMacData = new byte[75]; //ͷ5 +Data70
            srcMacData[0] = m_CLA;//����Ҫ����ֽ�Ϊ4
            srcMacData[1] = m_INS;
            srcMacData[2] = m_P1;
            srcMacData[3] = m_P2;
            srcMacData[4] = m_Lc;
            Buffer.BlockCopy(m_Data, 0, srcMacData, 5, 70);
            byte[] byteMAC = CalcMACValue(srcMacData, key, RandomVal);//����MAC
            Buffer.BlockCopy(byteMAC, 0, m_Data, 70, 4);
            m_le = 0;
            m_nTotalLen = 79;
            return true;
        }

        public bool createVerifyPINCmd(bool bDefaultPwd, byte[] customPwd)
        {
            if (!bDefaultPwd && customPwd.Length != 6)
                return false; 
            m_CLA = 0x00;
            m_INS = 0x20;
            m_P1 = 0x00;
            m_P2 = 0x00;
            m_Lc = (byte)3;
            m_Data = new byte[3];
            //PIN
            for (int i = 0; i < 3; i++)
            {
                    if (bDefaultPwd)
                        m_Data[i] = 0x99;
                    else
                        m_Data[i] = (byte)((customPwd[i * 2] << 4) | customPwd[i * 2 + 1]);
            }
            m_le = 0;
            m_nTotalLen = 8;
            return true;
        }

        public bool createUpdateEF0BFileCmd(bool bDefaultPwd)
        {
            m_CLA = 0x04;
            m_INS = 0xD6;
            m_P1 = 0x8B;
            m_P2 = 0x00;
            m_Lc = (byte)32;
            m_Data = new byte[32];
            m_Data[0] = (byte)(bDefaultPwd ? 0x00 : 0x01);
            m_Data[1] = 0x01;//�ڲ�����Ա���ţ�����������
            //�ڲ�����Ա�����룬����������
            m_Data[2] = 0x12;
            m_Data[3] = 0x34;
            //4~31����
            m_le = 0;
            m_nTotalLen = 37;
            return true;
        }

        public bool createUpdateEF1CFileCmd(byte[] key, byte[] RandomVal, UserCardInfoParam cardInfo)
        {
            if (RandomVal.Length != 8)
                return false;
            m_CLA = 0x04;
            m_INS = 0xD6;
            m_P1 = 0x9C;
            m_P2 = 0x00;
            int nLen = 100;
            m_Lc = (byte)nLen;
            m_Data = new byte[nLen];
            for (int i = 0; i < nLen; i++)
                m_Data[i] = 0xFF;
            //������Ʒ
            byte[] byteGasType = BitConverter.GetBytes(cardInfo.LimitGasType);
            m_Data[0] = byteGasType[1];
            m_Data[1] = byteGasType[0];
            m_Data[2] = cardInfo.LimitArea;
            if (cardInfo.LimitArea != 0xFF)
            {
                byte[] byteLimitAreaCode = StringToBCD(cardInfo.LimitAreaCode);
                if (byteLimitAreaCode != null)
                    Buffer.BlockCopy(byteLimitAreaCode, 0, m_Data, 3, byteLimitAreaCode.Length);
            }
            if (cardInfo.LimitCarNo && !string.IsNullOrEmpty(cardInfo.CarNo))
            {
                byte[] carNo = Encoding.Unicode.GetBytes(cardInfo.CarNo);
                for (int i = 0; i < 16; i++)
                {
                    if (i < carNo.Length)
                        m_Data[43 + i] = carNo[i];
                    else
                        m_Data[43 + i] = 0x00;                    
                }
            }
                
            byte[] fixData = BitConverter.GetBytes(cardInfo.LimitFixDepartment);//���㵥λ��ʶ
            m_Data[59] = fixData[3];
            m_Data[60] = fixData[2];
            m_Data[61] = fixData[1];
            m_Data[62] = fixData[0];
            
            m_Data[63] = cardInfo.LimitGasFillCount;
            byte[] byteAmount = BitConverter.GetBytes(cardInfo.LimitGasFillAmount);
            m_Data[64] = byteAmount[3];
            m_Data[65] = byteAmount[2];
            m_Data[66] = byteAmount[1];
            m_Data[67] = byteAmount[0];

            byte[] srcMacData = new byte[101]; //ͷ5 +Data96
            srcMacData[0] = m_CLA;//����Ҫ����ֽ�Ϊ4
            srcMacData[1] = m_INS;
            srcMacData[2] = m_P1;
            srcMacData[3] = m_P2;
            srcMacData[4] = m_Lc;
            Buffer.BlockCopy(m_Data, 0, srcMacData, 5, 96);
            byte[] byteMAC = CalcMACValue(srcMacData, key, RandomVal);//����MAC
            Buffer.BlockCopy(byteMAC, 0, m_Data, 96, 4);  //m_Data�����4�ֽ���MACУ��
            m_le = 0;
            m_nTotalLen = 105;
            return true;
        }
        
        public bool createUpdateEF0DFileCmd(byte[] key, byte[] RandomVal, UserCardInfoParam cardInfo)
        {
            if (RandomVal.Length != 8)
                return false;
            m_CLA = 0x04;
            m_INS = 0xD6;
            m_P1 = 0x8D;
            m_P2 = 0x00;
            int nLen = 68;
            m_Lc = (byte)nLen;
            m_Data = new byte[nLen];
            for (int i = 0; i < nLen; i++)
                m_Data[i] = 0xFF;
            byte[] BoalExprie = GetBCDDate(cardInfo.BoalExprie);
            Buffer.BlockCopy(BoalExprie, 0, m_Data, 0, 4);
            if (!string.IsNullOrEmpty(cardInfo.CarNo))
            {
                byte[] CarNo = Encoding.Unicode.GetBytes(cardInfo.CarNo);
                if (CarNo.Length <= 16)
                    Buffer.BlockCopy(CarNo, 0, m_Data, 4, CarNo.Length);
            }
            if (!string.IsNullOrEmpty(cardInfo.BoalId))
            {
                byte[] BoalId = Encoding.ASCII.GetBytes(cardInfo.BoalId);
                if (BoalId.Length <= 16)
                    Buffer.BlockCopy(BoalId, 0, m_Data, 20, BoalId.Length);
            }
            m_Data[36] = (byte)cardInfo.CylinderNum; //��ƿ����
            if (!string.IsNullOrEmpty(cardInfo.BoalFactoryID))
            {
                byte[] BoalFactoryId = Encoding.ASCII.GetBytes(cardInfo.BoalFactoryID);
                if (BoalFactoryId.Length <= 7)
                    Buffer.BlockCopy(BoalFactoryId, 0, m_Data, 37, BoalFactoryId.Length);
            }            
            m_Data[44] = (byte)(cardInfo.CylinderVolume & 0xFF); //��ƿ�ݻ�
            m_Data[45] = (byte)((cardInfo.CylinderVolume>>8) & 0xFF); //��ƿ�ݻ�
            m_Data[46] = cardInfo.GetByteCarType();//������
            if (!string.IsNullOrEmpty(cardInfo.BusDistance))
            {
                byte[] BusDistance = Encoding.ASCII.GetBytes(cardInfo.BusDistance);
                if (BusDistance.Length <= 5)
                    Buffer.BlockCopy(BusDistance, 0, m_Data, 47, BusDistance.Length);
            }

            byte[] srcMacData = new byte[69]; //ͷ5 +Data64
            srcMacData[0] = m_CLA;//����Ҫ����ֽ�Ϊ4
            srcMacData[1] = m_INS;
            srcMacData[2] = m_P1;
            srcMacData[3] = m_P2;
            srcMacData[4] = m_Lc;
            Buffer.BlockCopy(m_Data, 0, srcMacData, 5, 64);
            byte[] byteMAC = CalcMACValue(srcMacData, key, RandomVal);//����MAC
            Buffer.BlockCopy(byteMAC, 0, m_Data, 64, 4);  //m_Data�����4�ֽ���MACУ��
            m_le = 0;
            m_nTotalLen = 73;
            return true;
        }

        public bool createUpdateEF10FileCmd(byte[] key, byte[] RandomVal)
        {
            if (RandomVal.Length != 8)
                return false;
            m_CLA = 0x04;
            m_INS = 0xD6;
            m_P1 = 0x90;
            m_P2 = 0x00;
            int nLen = 44;
            m_Lc = (byte)nLen;
            m_Data = new byte[nLen];
            //������ţ�2�ֽ�
            m_Data[0] = 0x00;
            m_Data[1] = 0x01;
            //��������ȫ0          
            //
            byte[] srcMacData = new byte[45]; //ͷ5 +Data40
            srcMacData[0] = m_CLA;//����Ҫ����ֽ�Ϊ4
            srcMacData[1] = m_INS;
            srcMacData[2] = m_P1;
            srcMacData[3] = m_P2;
            srcMacData[4] = m_Lc;
            Buffer.BlockCopy(m_Data, 0, srcMacData, 5, 40);
            byte[] byteMAC = CalcMACValue(srcMacData, key, RandomVal);//����MAC
            Buffer.BlockCopy(byteMAC, 0, m_Data, 40, 4);  //m_Data�����4�ֽ���MACУ��
            m_le = 0;
            m_nTotalLen = 49;
            return true;
        }

        public bool createInitializeLoadCmd(int nMoney, byte[] TermialID)
        {
            if (TermialID.Length != 6)
                return false;
            m_CLA = 0x80;
            m_INS = 0x50;
            m_P1 = 0x00;
            m_P2 = 0x01; //ED���Ӵ���0x01; EP����Ǯ��0x02
            int nLen = 11;
            m_Lc = (byte)nLen;
            m_Data = new byte[nLen];
            m_Data[0] = 1; //��Կ����
            byte[] byteMoney = BitConverter.GetBytes(nMoney);
            m_Data[1] = byteMoney[3];
            m_Data[2] = byteMoney[2];
            m_Data[3] = byteMoney[1];
            m_Data[4] = byteMoney[0];
            Buffer.BlockCopy(TermialID, 0, m_Data, 5, 6);
            m_le = 16;
            m_nTotalLen = 17;
            return true;
        }

        public bool createInitializeUnLoadCmd(int nMoney, byte[] TermialID)
        {
            if (TermialID.Length != 6)
                return false;
            m_CLA = 0x80;
            m_INS = 0x50;
            m_P1 = 0x05;
            m_P2 = 0x01; //Ȧ��
            int nLen = 11;
            m_Lc = (byte)nLen;
            m_Data = new byte[nLen];
            m_Data[0] = 1; //��Կ����
            byte[] byteMoney = BitConverter.GetBytes(nMoney);
            m_Data[1] = byteMoney[3];
            m_Data[2] = byteMoney[2];
            m_Data[3] = byteMoney[1];
            m_Data[4] = byteMoney[0];
            Buffer.BlockCopy(TermialID, 0, m_Data, 5, 6);
            m_le = 16;
            m_nTotalLen = 17;
            return true;
        }

        public bool createCreditLoadCmd(byte[] byteMAC2, byte[] TimeBcd)
        {
            m_CLA = 0x80;
            m_INS = 0x52;
            m_P1 = 0x00;
            m_P2 = 0x00;
            int nLen = 11;  //Data Len
            m_Lc = (byte)nLen; 
            m_Data = new byte[nLen];
            Buffer.BlockCopy(TimeBcd, 0, m_Data, 0, 7);
            Buffer.BlockCopy(byteMAC2, 0, m_Data, 7, 4);
            m_le = 4;
            m_nTotalLen = 17;
            return true;
        }

        public bool createDebitUnLoadCmd(byte[] byteMAC2, byte[] TimeBcd)
        {
            m_CLA = 0x80;
            m_INS = 0x54;
            m_P1 = 0x03;
            m_P2 = 0x00;
            int nLen = 11;  //Data Len
            m_Lc = (byte)nLen;
            m_Data = new byte[nLen];
            Buffer.BlockCopy(TimeBcd, 0, m_Data, 0, 7);
            Buffer.BlockCopy(byteMAC2, 0, m_Data, 7, 4);
            m_le = 4;
            m_nTotalLen = 17;
            return true;
        }

        public bool createCardBalanceCmd()
        {
            m_CLA = 0x80;
            m_INS = 0x5C;
            m_P1 = 0x00;
            m_P2 = 0x01; //ED���Ӵ���0x01; EP����Ǯ��0x02
            m_Lc = 0;
            m_Data = null;
            m_le = 4;
            m_nTotalLen = 5;
            return true;
        }

        public bool createrCardGrayCmd(bool bClearTAC)
        {
            m_CLA = 0xE0;
            m_INS = 0xCA;
            m_P1 = (byte)(bClearTAC ? 0x01 : 0x00); //0x00��ͨ��ȡ, 0x01���TACUF
            m_P2 = 0x00;
            m_Lc = 0;
            m_Data = null;
            m_le = 30; //Ӧ�𳤶�
            m_nTotalLen = 5;
            return true;
        }

        //������ʼ��
        public bool createrInitForGrayCmd(byte[] TermialID)
        {
            if (TermialID.Length != 6)
                return false;
            m_CLA = 0xE0;
            m_INS = 0x7A;
            m_P1 = 0x08;
            m_P2 = 0x01;//ED���Ӵ���0x01; EP����Ǯ��0x02
            int nLen = 7;
            m_Lc = (byte)nLen;
            m_Data = new byte[nLen];
            m_Data[0] = 0x01;
            Buffer.BlockCopy(TermialID, 0, m_Data, 1, 6);
            m_le = 0x0F; //Ӧ�𳤶�
            m_nTotalLen = 13;
            return true;
        }

        public bool createrGrayLockCmd(byte[] DataVal)
        {
            int nLen = DataVal.Length;
            if (nLen != 19)
                return false;
            m_CLA = 0xE0;
            m_INS = 0x7C;
            m_P1 = 0x08;
            m_P2 = 0x00;            
            m_Lc = (byte)nLen;
            m_Data = new byte[nLen];
            Buffer.BlockCopy(DataVal,0,m_Data,0,nLen);
            m_le = 0x08;
            m_nTotalLen = 25;
            return true;
        }

        //������۳�ʼ��
        public bool createrInitForUnlockCardCmd(byte[] TermialID)
        {
            if (TermialID.Length != 6)
                return false;
            m_CLA = 0xE0;
            m_INS = 0x7A;
            m_P1 = 0x09;
            m_P2 = 0x01;
            int nLen = 7;
            m_Lc = (byte)nLen;
            m_Data = new byte[nLen];
            m_Data[0] = 1; //��Կ����
            Buffer.BlockCopy(TermialID, 0, m_Data, 1, 6);
            m_le = 18;
            m_nTotalLen = 13;
            return true;
        }

        //�������
        public bool createGreyCardUnLockCmd(int nMoney,byte[] byteMAC2, byte[] TimeBcd)
        {
            m_CLA = 0xE0;
            m_INS = 0x7E;
            m_P1 = 0x09;
            m_P2 = 0x00;
            int nLen = 15;  //Data Len
            m_Lc = (byte)nLen; 
            m_Data = new byte[nLen];
            byte[] byteMoney = BitConverter.GetBytes(nMoney);
            m_Data[0] = byteMoney[3];
            m_Data[1] = byteMoney[2];
            m_Data[2] = byteMoney[1];
            m_Data[3] = byteMoney[0];
            Buffer.BlockCopy(TimeBcd, 0, m_Data, 4, 7);
            Buffer.BlockCopy(byteMAC2, 0, m_Data, 11, 4);
            m_le = 4;
            m_nTotalLen = 21;
            return true;
        }

        public bool createDebitForUnlockCmd(byte[] DebitData)
        {
            int nLen = DebitData.Length;
            if (nLen != 27)
                return false;
            m_CLA = 0xE0;
            m_INS = 0x7E;
            m_P1 = 0x08;
            m_P2 = 0x01;//ED���Ӵ���0x01; EP����Ǯ��0x02
            m_Lc = (byte)nLen;
            m_Data = new byte[nLen];
            Buffer.BlockCopy(DebitData, 0, m_Data, 0, 27);            
            m_le = 4;
            m_nTotalLen = 33;
            return true;
        }

        //����������ϸ��¼�ļ�
        public bool createReadRecordCmd(byte ResponseLen)
        {
            m_CLA = 0x00;
            m_INS = 0xB2;
            m_P1 = 0x98; //���ļ���ʶ�����ļ�100+11000-----��������11000��18�ļ���
            m_P2 = 0x00;
            m_Lc = 0x00;  //������
            m_Data = null; //������
            m_le = ResponseLen;   //����Ӧ�û��������ļ�EF15����
            m_nTotalLen = 5;
            return true;
        }

        public bool createPINResetCmd(byte[] key, byte[] bytePIN)
        {
            if (key.Length != 16 || bytePIN.Length != 6)
                return false;
            byte[] PinVal = new byte[3];
            for (int i = 0; i < 3; i++)
            {
                PinVal[i] = (byte)((bytePIN[i * 2] << 4) | bytePIN[i * 2 + 1]);
            }
            m_CLA = 0x80;
            m_INS = 0x5E;
            m_P1 = 0x00;
            m_P2 = 0x00;
            int nLen = 7;
            m_Lc = (byte)nLen;
            m_Data = new byte[nLen];

            Buffer.BlockCopy(PinVal, 0, m_Data, 0, 3);
            byte[] macKey = new byte[8];
            for (int i = 0; i < 8; i++)
            {
                macKey[i] = (byte)(key[i] ^ key[8 + i]);
            }
            byte[] mac = CalcMacVal(PinVal, macKey);
            Buffer.BlockCopy(mac, 0, m_Data, 3, 4);
            m_le = 0;
            m_nTotalLen = 12;
            return true;
        }

        public bool createChangePINCmd(byte[] oldPwd, byte[] newPwd)
        {
            if (oldPwd.Length != 6 || newPwd.Length != 6)
                return false;
            byte[] oldPinVal = new byte[3];
            byte[] newPinVal = new byte[3];
            for (int i = 0; i < 3; i++)
            {
                oldPinVal[i] = (byte)((oldPwd[i * 2] << 4) | oldPwd[i * 2 + 1]);
                newPinVal[i] = (byte)((newPwd[i * 2] << 4) | newPwd[i * 2 + 1]);
            }
            
            m_CLA = 0x80;
            m_INS = 0x5E;
            m_P1 = 0x01;
            m_P2 = 0x00;
            int nLen = 7;
            m_Lc = (byte)nLen;
            m_Data = new byte[nLen];
            Buffer.BlockCopy(oldPinVal, 0, m_Data, 0, 3);            
            m_Data[3] = 0xFF;
            Buffer.BlockCopy(newPinVal, 0, m_Data, 4, 3);
            m_le = 0;
            m_nTotalLen = 12;
            return true;
        }

        public bool createPINUnLockCmd(byte[] randval, byte[] key, byte[] bytePIN)
        {
            if (key.Length != 16 || bytePIN.Length != 6)
                return false;
                
            byte[] PINVal = new byte[8];
            PINVal[0] = 0x03;
            PINVal[1] = (byte)((bytePIN[0] << 4) | bytePIN[1]);
            PINVal[2] = (byte)((bytePIN[2] << 4) | bytePIN[3]);
            PINVal[3] = (byte)((bytePIN[4] << 4) | bytePIN[5]);
            PINVal[4] = 0x80;
            PINVal[5] = 0x00;
            PINVal[6] = 0x00;
            PINVal[7] = 0x00;

            
            m_CLA = 0x84;
            m_INS = 0x24;
            m_P1 = 0x00;
            m_P2 = 0x00;            
            int nLen = 12;
            m_Lc = (byte)nLen;
            m_Data = new byte[nLen];
            byte[] EncryptPinVal = DesCryptography.TripleEncryptData(PINVal, key);
            byte[] srcData = new byte[13];//���ڼ���MAC��ԭʼ����
            srcData[0] = m_CLA;
            srcData[1] = m_INS;
            srcData[2] = m_P1;
            srcData[3] = m_P2;
            srcData[4] = m_Lc;
            Buffer.BlockCopy(EncryptPinVal, 0, srcData, 5, 8);
            byte[] mac = CalcMACValue(srcData, key, randval);
            Buffer.BlockCopy(EncryptPinVal, 0, m_Data, 0, 8);
            Buffer.BlockCopy(mac, 0, m_Data, 8, 4);
            m_le = 0;
            m_nTotalLen = 17;
            return true;
        }
    }
}
