using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace IFuncPlugin
{
    public class ConvertBCD
    {
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

    public class GrobalVariable
    {
        public static readonly string[] strAuthority = new string[] { "�˻�����", "��λ����", "վ�����", "��ֵ��¼", "�ƿ�����", "����Ϣά��", "��Կ����", "���ƴ������" };

        public static readonly int Authority_Config_Count = 8;
        //Ȩ��ֵ
        public static readonly int Account_Authority = 1;
        public static readonly int ClientInfo_Authority = 1 << 1;
        public static readonly int StationInfo_Authority = 1 << 2;
        public static readonly int RechargeList_Authority = 1 << 3;
        public static readonly int CardOperating_Authority = 1 << 4;
        public static readonly int CardPublish_Authority = 1 << 5;
        public static readonly int KeyManage_Authority = 1 << 6;
        public static readonly int CodeTable_Authority = 1 << 7;
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
        eProvinceCode,   //ʡ�����
        eCityCode,      //���д����
        eCompanyCode,   //��˾�����
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
        void ShowPluginForm(Form parent); //��ʾ�������  
    }
}
