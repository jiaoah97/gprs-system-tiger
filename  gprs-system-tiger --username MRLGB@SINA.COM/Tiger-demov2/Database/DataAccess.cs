using System;
using System.Data;
using System.Data.Common;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace Tiger.Database
{
    class DataAccess
    {
        //private string _confirString = "ConnectionString";
        private string _confirString = "ConnectionMysql";
        public DataAccess()
        {
        }

        /// <param name="configString">app.config �ؼ���</param>
        public DataAccess(string configString)
        {
            ConfigString = configString;
        }

        /**/
        /// <summary>
        /// ����,�������ݿ������ַ���
        /// </summary>
        public string ConfigString
        {
            get
            {
                return _confirString;
            }
            set
            {
                _confirString = value;
            }
        }

        //=====================================================��������ַ���====================================
        #region ��������ַ���
        /**/
        /// <summary>
        /// ��������ַ���
        /// </summary>
        /// <returns></returns>
        public string GetConnectionString(string ConfigString)
        {
            ConnectionStringSettingsCollection configStringCollention = ConfigurationManager.ConnectionStrings;
            if (configStringCollention == null || configStringCollention.Count <= 0)
            {
                throw new Exception("app.config ���������ַ���!");
            }
            ConnectionStringSettings stringSettings = null;
            if (ConfigString == string.Empty)
            {
                stringSettings = ConfigurationManager.ConnectionStrings["ConnectionString"];
            }
            else
            {
                stringSettings = ConfigurationManager.ConnectionStrings[ConfigString];
            }
            return stringSettings.ConnectionString;

        }
        public string GetConnectionString()
        {
            return GetConnectionString(ConfigString);
        }

        #endregion
        //==============================================CreateConnection=============================

        #region �������ݿ����� public DbConnection CreateConnection()
        /**/
        /// <summary>
        /// �������ݿ�����
        /// </summary>
        /// <returns></returns>
        public MySqlConnection CreateConnection()
        {
            MySqlConnection con = new MySqlConnection();
            con.ConnectionString = GetConnectionString();
            return con;
        }
        #endregion

        //==============================================CreateCommand================================

        #region ����ִ���������
        /**/
        /// <summary>
        /// ����ִ���������
        /// </summary>
        /// <param name="sql">SQL���</param>
        /// <returns>ִ���������ʵ��</returns>
        public MySqlCommand CreateCommand(string sql)
        {
            MySqlCommand _command = new MySqlCommand();
            _command.Connection = CreateConnection();
            _command.CommandText = sql;
            return _command;
        }

        #endregion
        //=========================================CreateAdapter()==============================================

        #region �������������� CreateAdapter(string sql)
        /**/
        /// <summary>
        /// ��������������
        /// </summary>
        /// <param name="sql">SQL,���</param>
        /// <returns>����������ʵ��</returns>
        public DbDataAdapter CreateAdapter(string sql)
        {
            var connection = CreateConnection();
            var command = new MySqlCommand {Connection = connection, CommandText = sql};
            var da = new MySqlDataAdapter {SelectCommand = command};

            return da;
        }
        #endregion

        //======================================================ExecuteCommand()============================================

        #region ִ�зǲ�ѯ���,��������Ӱ��ļ�¼���� ExecuteCommand(string sql)
        /**/
        /// <summary>
        /// ִ�зǲ�ѯ���,��������Ӱ��ļ�¼����
        /// </summary>
        /// <param name="sql">SQL���</param>
        /// <returns>��Ӱ���¼����</returns>
        public int ExecuteCommand(string sql)
        {
            int result = 0;
            var command = CreateCommand(sql);
            try
            {
                command.Connection.Open();
                result = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
                //  return 0;
            }
            finally
            {
                command.Connection.Close();
            }
            return result;
        }
        #endregion

        //=====================================================ExecuteScalar()=============================================

        #region ִ�зǲ�ѯ���,�������������е�ֵ ExecuteScalar(string sql)

        /**/
        /// <summary>
        /// ִ�зǲ�ѯ���,�������������е�ֵ
        /// </summary>
        /// <param name="sql">SQL���</param>
        /// <returns>Object</returns>
        public object ExecuteScalar(string sql)
        {
            object result = null;
            DbCommand command = CreateCommand(sql);
            try
            {
                command.Connection.Open();
                result = command.ExecuteScalar();
            }
            catch
            {
                throw;
            }
            finally
            {
                command.Connection.Close();
            }
            return result;
        }
        #endregion

        //=====================================================ExecuteReader()=============================================

        #region ִ�в�ѯ������DataReader���ؽ����  ExecuteReader(string sql)
        /**/
        /// <summary>
        /// ִ�в�ѯ������DataReader���ؽ����
        /// </summary>
        /// <param name="sql">SQL���</param>
        /// <returns>IDataReader</returns>
        public DbDataReader ExecuteReader(string sql)
        {
            DbDataReader result;
            DbCommand command = CreateCommand(sql);
            try
            {
                command.Connection.Open();
                result = command.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch
            {
                throw;
            }
            finally
            {

            }
            return result;
        }
        #endregion

        //=====================================================GetDataSet()================================================

        #region ִ�в�ѯ������DataSet���ؽ���� GetDataSet(string sql)
        /**/
        /// <summary>
        /// ִ�в�ѯ������DataSet���ؽ����
        /// </summary>
        /// <param name="sql">SQL���</param>
        /// <returns>DataSet</returns>
        public DataSet GetDataSet(string sql)
        {
            var result = new DataSet();
            IDataAdapter dataAdapter = CreateAdapter(sql);
            try
            {
                dataAdapter.Fill(result);
            }
            catch
            {
                throw;
            }
            finally
            {
            }
            return result;

        }
        #endregion

        //=====================================================GetDataView()===============================================

        #region ִ�в�ѯ������DataView���ؽ����   GetDataView(string sql)

        /**/
        /// <summary>
        /// ִ�в�ѯ������DataView���ؽ����
        /// </summary>
        /// <param name="sql">SQL���</param>
        /// <returns>DataView</returns>
        public DataView GetDataView(string sql)
        {
            DataView dv = GetDataSet(sql).Tables[0].DefaultView;
            return dv;
        }
        #endregion

        //=====================================================GetDataTable()==============================================

        #region ִ�в�ѯ������DataTable���ؽ����   GetDataTable(string sql)

        /**/
        /// <summary>
        /// ִ�в�ѯ������DataTable���ؽ����
        /// </summary>
        /// <param name="sql">SQL���</param>
        /// <returns>DataTable</returns>
        public DataTable GetDataTable(string sql)
        {
            var dt = GetDataSet(sql).Tables[0];
            return dt;
        }
        #endregion

    }
}
