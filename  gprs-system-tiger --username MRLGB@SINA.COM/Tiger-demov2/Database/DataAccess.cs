using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace Tiger
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
            ConnectionStringSettingsCollection ConfigStringCollention = ConfigurationManager.ConnectionStrings;
            if (ConfigStringCollention == null || ConfigStringCollention.Count <= 0)
            {
                throw new Exception("app.config ���������ַ���!");
            }
            ConnectionStringSettings StringSettings = null;
            if (ConfigString == string.Empty)
            {
                StringSettings = ConfigurationManager.ConnectionStrings["ConnectionString"];
            }
            else
            {
                StringSettings = ConfigurationManager.ConnectionStrings[ConfigString];
            }
            return StringSettings.ConnectionString;

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
            MySqlConnection _connection = CreateConnection();
            MySqlCommand _command = new MySqlCommand();
            _command.Connection = _connection;
            _command.CommandText = sql;
            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = _command;

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
            int _result = 0;
            MySqlCommand _command = CreateCommand(sql);
            try
            {
                _command.Connection.Open();
                _result = _command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
                //  return 0;
            }
            finally
            {
                _command.Connection.Close();
            }
            return _result;
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
            object _result = null;
            DbCommand _command = CreateCommand(sql);
            try
            {
                _command.Connection.Open();
                _result = _command.ExecuteScalar();
            }
            catch
            {
                throw;
            }
            finally
            {
                _command.Connection.Close();
            }
            return _result;
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
            DbDataReader _result;
            DbCommand _command = CreateCommand(sql);
            try
            {
                _command.Connection.Open();
                _result = _command.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch
            {
                throw;
            }
            finally
            {

            }
            return _result;
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
            DataSet _result = new DataSet();
            IDataAdapter _dataAdapter = CreateAdapter(sql);
            try
            {
                _dataAdapter.Fill(_result);
            }
            catch
            {
                throw;
            }
            finally
            {
            }
            return _result;

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
            DataTable dt = GetDataSet(sql).Tables[0];
            return dt;
        }
        #endregion

    }
}
