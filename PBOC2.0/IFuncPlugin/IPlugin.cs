using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace IFuncPlugin
{
    public enum MenuType
    {
        eSystemAccount, //�˻�����
        eGasDataList,   //�������ݿ����
        eCommunicationUDP,  //UDPͨѶʵʱ��ʾ����
        eCardOperating      //������
    }

    //����и����ؼ���λ��
    public struct ControlPos
    {
        public int x;
        public int y;
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
