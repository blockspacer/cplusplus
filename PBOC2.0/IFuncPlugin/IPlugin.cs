using System;
using System.Collections.Generic;
using System.Text;
using System.Management;
using System.Windows.Forms;

namespace IFuncPlugin
{
    public class PublicFunc
    {
        //��ȡ��ǰBCD���ʽ��ϵͳʱ��
        public static byte[] GetBCDTime()
        {
            string strTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            int nByteSize = strTime.Length / 2;
            byte[] byteBCD = new byte[nByteSize];
            for (int i = 0; i < nByteSize; i++)
            {
                byteBCD[i] = Convert.ToByte(strTime.Substring(i * 2, 2), 16);
            }
            return byteBCD;
        }

        public static bool ByteDataEquals(byte[] byteL, byte[] byteR)
        {
            if (byteL.Length != byteR.Length)
                return false;
            for (int i = 0; i < byteL.Length; i++)
            {
                if (byteL[i] != byteR[i])
                    return false;
            }
            return true;
        }

        public static byte[] StringToBCD(string strData)
        {
            if (string.IsNullOrEmpty(strData) || strData.Length % 2 != 0)
                return null;
            try
            {
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
            catch
            {
                return null;
            }
        }

        public static string GetCardTypeString(byte CardType)
        {
            string strCardType = "";
            switch (CardType)
            {
                case 0x01:
                    strCardType = "���˿�";
                    break;
                case 0x02:
                    strCardType = "����";
                    break;
                case 0x04:
                    strCardType = "Ա����";
                    break;
                case 0x06:
                    strCardType = "ά�޿�";
                    break;
                case 0x11:
                    strCardType = "��λ�ӿ�";
                    break;
                case 0x21:
                    strCardType = "��λĸ��";
                    break;
            }
            return strCardType;
        }

        public static string GetPhysicalAddress()
        {
            try
            {
                string mac = "";
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"] == true)
                    {
                        mac = mo["MacAddress"].ToString();
                        break;
                    }
                }
                moc = null;
                mc = null;
                string strSplit = ":";
                string[] macVal = mac.Split(strSplit.ToCharArray(), 6);
                byte[] byteVal = new byte[16];
                int i = 0;
                foreach (string strMac in macVal)
                {
                    byteVal[i] = Convert.ToByte(strMac, 16);
                    i++;
                }                
                byteVal[6] = 0x55;
                byteVal[7] = 0xAA;
                for (i = 0; i < 8; i++)
                    byteVal[8 + i] = (byte)(byteVal[i] ^ 0xFF);                
                return BitConverter.ToString(byteVal).Replace("-", "");
            }
            catch
            {
                return "";
            }
        }

        public static string GetStringFromEncoding(byte[] SrcData, int nIndex, int nCount)
        {
            return Encoding.Unicode.GetString(SrcData, nIndex, nCount);
        }

        public static byte[] GetBytesFormEncoding(string strData)
        {
            return Encoding.Unicode.GetBytes(strData);
        }

    }

    public class GrobalVariable
    {
        public static readonly string[] strAuthority = new string[] { "�˻�����", "��λ����", "վ�����", "��ֵ��¼", "�ƿ���Կ����", "��Ƭ����", "���ƴ������", "���ݿ����" };

        public static readonly int Authority_Config_Count = 8;
        //Ȩ��ֵ
        public static readonly int Account_Authority = 1;
        public static readonly int ClientInfo_Authority = 1 << 1;
        public static readonly int StationInfo_Authority = 1 << 2;
        public static readonly int RechargeList_Authority = 1 << 3;
        public static readonly int CardOp_KeyManage_Authority = 1 << 4;
        public static readonly int CardPublish_Authority = 1 << 5;        
        public static readonly int CodeTable_Authority = 1 << 6;
        public static readonly int DbManage_Authority = 1 << 7;
    }

    public class SqlConnectInfo
    {
        public bool m_bConfig = false;
        public string strServerName = "(local)";
        public string strDbName = "FunnettStation";
        public string strUser = "";
        public string strUserPwd = "";
    }

    public enum MenuType
    {
        eUnknown = 0,
        eSystemAccount, //�˻�����
        eClientInfo,       //��λ��Ϣ
        eStationInfo,       //վ����Ϣ
        eRechargeList,   //��ֵ��¼        
        eCardOperating,      //�ƿ�����
        eOneKeyMadeCard,  //һ���ƿ�
        eCardPublish,      //����Ϣ��д
        eOrgKeyManage, //��ʼ����Կ
        ePsamKeyManage,  //PSAM����Կ
        eUserKeysManage, //��Կ����
        eExportKeyXml,   //��Կ������XML
        eProvinceCode,   //ʡ�����
        eCityCode,      //���д����
        eCompanyCode,   //��˾�����
        eImportKeyXml, //��ԿXML�ļ�����
        eDbManage,       //���ݿⱸ�ݻ�ԭ
        eToBlackCard     //����ʧ����������
    }

    //����и����ؼ���λ��
    public struct ControlPos
    {
        public int x;
        public double dbRateH;//�������
        public int y;
        public double dbRateV; //�������
    }

    public interface IPlugin
    {
        MenuType GetMenuType();//����Ĳ˵�λ��
        string PluginName();  //�������
        Guid PluginGuid();  //���Ψһ��ʶ��GUID��
        string PluginMenu(); //�����Ӧ�˵�����
        void SetAuthority(int nLoginUserId, int nAuthority);//�ò����Ȩ��
        void ShowPluginForm(Panel parent, SqlConnectInfo DbInfo); //��ʾ�������  
    }
}
