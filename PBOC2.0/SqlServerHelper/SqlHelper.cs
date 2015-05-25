using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace SqlServerHelper
{
    public class SqlHelper : InterfaceSqlOperator
    {
        private SqlConnection m_Conn = null;

        public bool OpenSqlServerConnection(string strServerName, string strDbName, string strUser, string strPwd)
        {
            string strConnection= "Persist Security Info=False;Integrated Security=sspi;server=" + strServerName +
                                                ";Initial Catalog=" + strDbName + ";User ID=" + strUser + ";Password=" + strPwd;
            m_Conn = new SqlConnection(strConnection);
            m_Conn.Open();
            return m_Conn == null ? false: true;
        }

        /// <summary>
        /// ʹ��SQL���ִ��
        /// </summary>
        /// <param name="strSql">SQL���</param>
        /// <returns>ִ�н��</returns>
        public int ExecuteCommand(string strSql)
        {
            if (m_Conn == null)
                return 0;
            SqlCommand cmd = new SqlCommand(strSql, m_Conn);
            cmd.CommandType = CommandType.Text;
            SqlParameter retValParam = new SqlParameter("ReturnValue", SqlDbType.Int, 4, ParameterDirection.ReturnValue, true, 
                                                                                        0, 0, string.Empty, DataRowVersion.Default, null);
            // ���뷵�ز��� 
            cmd.Parameters.Add(retValParam);
            cmd.ExecuteNonQuery();
            if (cmd.Parameters["ReturnValue"].Value != null)
                return (int)cmd.Parameters["ReturnValue"].Value;
            else
                return 1;
        }

        public int ExecuteCommand(string strSql, out SqlDataReader dataReader)
        {
            dataReader = null;
            if (m_Conn == null)
                return 0;
            SqlCommand cmd = new SqlCommand(strSql, m_Conn);
            cmd.CommandType = CommandType.Text;
            SqlParameter retValParam = new SqlParameter("ReturnValue", SqlDbType.Int, 4, ParameterDirection.ReturnValue, true,
                                                                                        0, 0, string.Empty, DataRowVersion.Default, null);
            // ���뷵�ز��� 
            cmd.Parameters.Add(retValParam);
            
            dataReader = cmd.ExecuteReader();
            if (cmd.Parameters["ReturnValue"].Value != null)
                return (int)cmd.Parameters["ReturnValue"].Value;
            else
                return 1;
        }

        /// <summary>
        /// ������SQL��ѯ
        /// </summary>
        /// <param name="strProcName">SQL���</param>
        /// <param name="procParam">SQL���Ĳ���</param>
        /// <returns>ִ�н��</returns>
        public int ExecuteCommand(string strSql, SqlParameter[] procParam)
        {
            if (m_Conn == null)
                return 0;
            SqlCommand cmd = new SqlCommand(strSql, m_Conn);
            cmd.CommandType = CommandType.Text;

            // ���ΰѲ�������SQL���
            if (procParam != null)
            {
                foreach (SqlParameter param in procParam)
                    cmd.Parameters.Add(param);
            }
            SqlParameter retValParam = new SqlParameter("ReturnValue", SqlDbType.Int, 4, ParameterDirection.ReturnValue, true,
                                                                               0, 0, string.Empty, DataRowVersion.Default, null);
            // ���뷵�ز��� 
            cmd.Parameters.Add(retValParam);
            cmd.ExecuteNonQuery();
            if (cmd.Parameters["ReturnValue"].Value != null)
                return (int)cmd.Parameters["ReturnValue"].Value;
            else
                return 1;
        }

        /// <summary>
        /// ������SQL��ѯ
        /// </summary>
        /// <param name="strProcName">SQL���</param>
        /// <param name="procParam">SQL���Ĳ���</param>        
        /// <param name="dataReader">���</param>
        /// <returns>ִ�н��</returns>
        public int ExecuteCommand(string strSql, SqlParameter[] procParam, out SqlDataReader dataReader)
        {
            dataReader = null;
            if (m_Conn == null)
                return 0;
            SqlCommand cmd = new SqlCommand(strSql, m_Conn);
            cmd.CommandType = CommandType.Text;

            // ���ΰѲ�������SQL��� 
            if (procParam != null)
            {
                foreach (SqlParameter param in procParam)
                    cmd.Parameters.Add(param);
            }
            SqlParameter retValParam = new SqlParameter("ReturnValue", SqlDbType.Int, 4, ParameterDirection.ReturnValue, true,
                                                                               0, 0, string.Empty, DataRowVersion.Default, null);
            // ���뷵�ز��� 
            cmd.Parameters.Add(retValParam);
            dataReader = cmd.ExecuteReader();
            if (cmd.Parameters["ReturnValue"].Value != null)
                return (int)cmd.Parameters["ReturnValue"].Value;
            else
                return 1;
        }


        /// <summary>
        /// ʹ���޲���������Ĵ洢����ִ��
        /// </summary>
        /// <param name="strProcName">�洢��������</param>
        /// <returns>ִ�н��</returns>
        public int ExecuteProc(string strProcName)
        {
            if (m_Conn == null)
                return 0;
            SqlCommand cmd = new SqlCommand(strProcName, m_Conn);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlParameter retValParam = new SqlParameter("ReturnValue", SqlDbType.Int, 4, ParameterDirection.ReturnValue, 
                                                                                        true, 0, 0, string.Empty, DataRowVersion.Default, null);
            // ���뷵�ز��� 
            cmd.Parameters.Add(retValParam);            
            cmd.ExecuteNonQuery();
            if(cmd.Parameters["ReturnValue"].Value != null)
                return (int)cmd.Parameters["ReturnValue"].Value;
            else
                return 1;
        }

       /// <summary>
        /// ʹ�ô洢����ִ��
       /// </summary>
        /// <param name="strProcName">�洢��������</param>
       /// <param name="dataReader">���</param>
        /// <returns>ִ�н��</returns>
        public int ExecuteProc(string strProcName, out SqlDataReader dataReader)
        {
            dataReader = null;
            if (m_Conn == null)
                return 0;
            SqlCommand cmd = new SqlCommand(strProcName, m_Conn);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlParameter retValParam = new SqlParameter("ReturnValue", SqlDbType.Int, 4, ParameterDirection.ReturnValue, true,
                                                                                        0, 0, string.Empty, DataRowVersion.Default, null);
            // ���뷵�ز��� 
            cmd.Parameters.Add(retValParam);
            dataReader = cmd.ExecuteReader();
            if (cmd.Parameters["ReturnValue"].Value != null)
                return (int)cmd.Parameters["ReturnValue"].Value;
            else
                return 1;
        }

        /// <summary>
        /// �洢���̲�����װ
        /// </summary>
        /// <param name="ParamName">��������</param>
        /// <param name="DbType">����</param>
        /// <param name="Size">�ֽ���</param>
        /// <param name="Direction">��������(����/���)</param>
        /// <param name="Value">����ֵ</param>
        /// <returns>ִ�н��</returns>
        public SqlParameter MakeParam(string ParamName, SqlDbType DbType, Int32 Size, ParameterDirection Direction, object Value)
        {
            SqlParameter param;
            if (Size > 0)
                param = new SqlParameter(ParamName, DbType, Size);
            else
                param = new SqlParameter(ParamName, DbType);

            param.Direction = Direction;
            if (!(Direction == ParameterDirection.Output && Value == null))
                param.Value = Value;
            return param; 
        }

       /// <summary>
        /// ʹ�ô洢����ִ��
       /// </summary>
        /// <param name="strProcName">�洢��������</param>
        /// <param name="procParam">�洢���̵Ĳ���</param>
        /// <returns>ִ�н��</returns>
        public int ExecuteProc(string strProcName, SqlParameter[] procParam)
        {
            if (m_Conn == null)
                return 0;
            SqlCommand cmd = new SqlCommand(strProcName, m_Conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // ���ΰѲ�������洢���� 
            if (procParam != null)
            {
                foreach (SqlParameter param in procParam)
                    cmd.Parameters.Add(param);
            }
            SqlParameter retValParam = new SqlParameter("ReturnValue", SqlDbType.Int, 4, ParameterDirection.ReturnValue, true,
                                                                               0, 0, string.Empty, DataRowVersion.Default, null);
            // ���뷵�ز��� 
            cmd.Parameters.Add(retValParam);
            cmd.ExecuteNonQuery();
            if (cmd.Parameters["ReturnValue"].Value != null)
                return (int)cmd.Parameters["ReturnValue"].Value;
            else
                return 1;
        }

        /// <summary>
        /// ʹ�ô洢����ִ��
        /// </summary>
        /// <param name="strProcName">�洢��������</param>
        /// <param name="procParam">�洢���̵Ĳ���</param>
        /// <param name="dataReader">���</param>
        /// <returns>ִ�н��</returns>
        public int ExecuteProc(string strProcName, SqlParameter[] procParam, out SqlDataReader dataReader)
        {
            dataReader = null;
            if (m_Conn == null)
                return 0;
            SqlCommand cmd = new SqlCommand(strProcName, m_Conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // ���ΰѲ�������洢���� 
            if (procParam != null)
            {
                foreach (SqlParameter param in procParam)
                    cmd.Parameters.Add(param);
            }
            SqlParameter retValParam = new SqlParameter("ReturnValue", SqlDbType.Int, 4, ParameterDirection.ReturnValue, true,
                                                                               0, 0, string.Empty, DataRowVersion.Default, null);
            // ���뷵�ز��� 
            cmd.Parameters.Add(retValParam);
            dataReader = cmd.ExecuteReader();
            if (cmd.Parameters["ReturnValue"].Value != null)
                return (int)cmd.Parameters["ReturnValue"].Value;
            else
                return 1;
        }

        public bool CloseConnection()
        {
            if (m_Conn != null)
            {
                m_Conn.Close();
                m_Conn = null;
            }
            return m_Conn == null ? true : false;
        }
    }
}
