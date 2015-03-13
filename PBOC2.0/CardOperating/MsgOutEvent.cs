using System;
using System.Collections.Generic;
using System.Text;

namespace CardOperating
{
    public enum ICC_Status
    {
        ICC_PowerOff = 0, //δ�ϵ�
        ICC_PowerOn     //���ϵ�
    }

    public class MsgOutEvent : EventArgs
    {
        private int m_nErrorCode = 0;
        public int ErrCode
        {
            get { return m_nErrorCode; }            
        }

        private string m_strMessage = "";
        public string Message
        {
            get { return m_strMessage; }            
        }

        public MsgOutEvent(int nErr, string strMsg)
        {
            m_nErrorCode = nErr;
            m_strMessage = strMsg;
        }
    }
    public delegate void MessageOutput(MsgOutEvent args);
}
