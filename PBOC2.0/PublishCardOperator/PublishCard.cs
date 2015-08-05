using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Data.SqlClient;
using System.Data;
using IFuncPlugin;
using System.Diagnostics;
using SqlServerHelper;

namespace PublishCardOperator
{
    public class  OrgKeyValue
    {
        public int nKeyId = 0;  //��ʼ��ԿID        
        public byte[] OrgKey = new byte[16]; //��Կ
        public int nKeyType = 0;  //��Կ���ࣨ0����CPU����1����PSAM�� ��
        public string KeyDetail;  //��Կ��Ϣ����
        public bool bValid = false; //�Ƿ�ʹ��
        public DbStateFlag eDbFlag = DbStateFlag.eDbOK;  //�Ƿ����޸�
    }

    public class PsamKeyValue
    {
        public int nKeyId = 0;  //��ԿID        
        public byte[] MasterKey = new byte[16]; //������Կ
        public byte[] MasterTendingKey = new byte[16]; //��Ƭά����Կ
        public byte[] AppMasterKey = new byte[16]; //Ӧ��������Կ
        public byte[] AppTendingKey = new byte[16]; //Ӧ��ά����Կ
        public byte[] ConsumerMasterKey = new byte[16]; //��������Կ
        public byte[] GrayCardKey = new byte[16]; //������Կ
        public byte[] MacEncryptKey = new byte[16]; //MAC������Կ
        public string KeyDetail;  //��Կ��Ϣ����
        public bool bValid = false; //�Ƿ�ʹ��
        public DbStateFlag eDbFlag = DbStateFlag.eDbOK;  //�Ƿ����޸�

    }

    public class AppKeyValueGroup
    {
        public int AppIndex = 0;   //Ӧ�ú�
        public byte[] AppMasterKey = new byte[16]; //Ӧ��������Կ
        public byte[] AppTendingKey = new byte[16]; //Ӧ��ά����Կ
        public byte[] AppInternalAuthKey = new byte[16]; //Ӧ���ڲ���֤
        public byte[] PINResetKey = new byte[16];  //PIN������װ��Կ
        public byte[] PINUnlockKey = new byte[16]; //PIN������Կ
        public byte[] ConsumerMasterKey = new byte[16]; //��������Կ
        public byte[] LoadKey = new byte[16];//Ȧ����Կ
        public byte[] TacMasterKey = new byte[16];//TAC��Կ
        public byte[] UnGrayKey = new byte[16];//���������Կ
        public byte[] UnLoadKey = new byte[16];//Ȧ����Կ
        public byte[] OverdraftKey = new byte[16];//�޸�͸֧�޶�����Կ
        public DbStateFlag eDbFlag = DbStateFlag.eDbOK;  //�Ƿ����޸� 
    }

    public class CpuKeyValue
    {
        public int nKeyId = 0;  //��ԿID
        public byte[] MasterKey = new byte[16]; //������Կ
        public byte[] MasterTendingKey = new byte[16]; //��Ƭά����Կ
        public byte[] InternalAuthKey = new byte[16];  //�ڲ���֤��Կ
        public string KeyDetail;  //��Կ��Ϣ����
        public bool bValid = false; //�Ƿ�ʹ��
        public DbStateFlag eDbFlag = DbStateFlag.eDbOK;  //�Ƿ����޸� 
        public List<AppKeyValueGroup> LstAppKeyGroup = new List<AppKeyValueGroup>();
    }


    public enum DbStateFlag
    {
        eDbOK,  //����
        eDbDirty,  //db�����
        eDbAdd,   //����
        eDbDelete  //ɾ��
    } 

    public class RelatedKeyInDb
    {
        public static byte[] GetCpuConsumerKey(SqlHelper sqlHelp)
        {
            SqlDataReader dataReader = null;
                SqlParameter[] sqlparam = new SqlParameter[1];
                sqlparam[0] = sqlHelp.MakeParam("ApplicationIndex", SqlDbType.Int, 4, ParameterDirection.Input, 1);
                sqlHelp.ExecuteProc("PROC_GetCpuKey", sqlparam, out dataReader);
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
                    byte[] BcdKey = PublicFunc.StringToBCD(strKey);
                    Trace.Assert(BcdKey.Length == 16);
                    Buffer.BlockCopy(BcdKey, 0, ConsumerKey, 0, 16);                    
                }
                dataReader.Close();
                return ConsumerKey;
            }
        }

        public static byte[] GetPsamConsumerKey(SqlHelper sqlHelp)
        {
            SqlDataReader dataReader = null;
            sqlHelp.ExecuteProc("PROC_GetPsamKey", out dataReader);
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
                    byte[] BcdKey = PublicFunc.StringToBCD(strKey);
                    Trace.Assert(BcdKey.Length == 16);
                    Buffer.BlockCopy(BcdKey, 0, ConsumerKey, 0, 16);
                }
                dataReader.Close();
                return ConsumerKey;
            }
        }
    }
    
}
