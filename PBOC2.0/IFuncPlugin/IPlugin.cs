using System;
using System.Collections.Generic;
using System.Text;
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
    }

    public class GrobalVariable
    {
        public static readonly string[] strAuthority = new string[] { "�˻�����", "��λ����", "վ�����", "��ֵ��¼", "�ƿ���Կ����", "������Կ����", "���ƴ������", "���ݿ����" };

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
        eSystemAccount, //�˻�����
        eClientInfo,       //��λ��Ϣ
        eStationInfo,       //վ����Ϣ
        eRechargeList,   //��ֵ��¼        
        eCardOperating,      //�ƿ�����
        eCardPublish,      //����Ϣ��д
        eOrgKeyManage, //��ʼ����Կ
        ePsamKeyManage,  //PSAM����Կ
        eUserKeysManage, //��Կ����
        eExportKeyXml,   //��Կ������XML
        eProvinceCode,   //ʡ�����
        eCityCode,      //���д����
        eCompanyCode,   //��˾�����
        eImportKeyXml, //��ԿXML�ļ�����
        eDbManage       //���ݿⱸ�ݻ�ԭ
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
