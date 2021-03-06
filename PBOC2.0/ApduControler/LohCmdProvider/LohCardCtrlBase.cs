using System;
using System.Collections.Generic;
using System.Text;
using IFuncPlugin;
using System.Data.SqlClient;
using System.Data;
using SqlServerHelper;
using ApduParam;
using ApduCtrl;
using System.Xml;
using ApduInterface;

namespace LohApduCtrl
{
    public class LohCardCtrlBase
    {
        //MF�¿�Ƭ������Կ
        protected byte[] m_KeyMain = new byte[16];
        //MF�¿�Ƭά����Կ
        protected byte[] m_KeyMaintain = new byte[16];

        //////////////////////////////////////////////////////////////////////////
        //PSAM����MF�¿�Ƭ������Կ
        protected byte[] m_KeyPsamMain = new byte[16];
        //PSAM����MF�¿�Ƭά����Կ
        protected byte[] m_KeyPsamMaintain = new byte[16];

        protected SqlConnectInfo m_DBInfo = new SqlConnectInfo();

        public LohCardCtrlBase()
        {

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

        public void SetMaintainKeyValue(byte[] byteKey, CardCategory eCategory)
        {
            if (byteKey.Length != 16)
                return;
            if (eCategory == CardCategory.CpuCard)
                Buffer.BlockCopy(byteKey, 0, m_KeyMaintain, 0, 16);
            else if (eCategory == CardCategory.PsamCard)
                Buffer.BlockCopy(byteKey, 0, m_KeyPsamMaintain, 0, 16);
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
    }
}
