using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;

namespace Helper
{
    public class DBConnectionSqlSrv
    {
        private bool debug = false;
        private bool connected = false;

        private SqlConnectionStringBuilder sqlCSB = new SqlConnectionStringBuilder();

        private String dbDataSource = null;
        private String dbCatalog = null;
        private String dbUsername = null;
        //private String dbPassword = null;
        private String dbGembok = null;

        public DBConnectionSqlSrv(bool debug)
        {
            this.debug = debug;
        }
        public void initConnection()
        {
            if (dbDataSource != null)
            {
                sqlCSB.DataSource = dbDataSource;
                sqlCSB.InitialCatalog = dbCatalog;
                sqlCSB.UserID = dbUsername;
                //sqlCSB.Password = dbPassword;
                sqlCSB.Password = dbGembok;
                sqlCSB.MaxPoolSize = 100;
                //sqlCSB.ConnectionString += "Column Encryption Setting=enable;";
                sqlCSB.IntegratedSecurity = false;
                sqlCSB.ColumnEncryptionSetting = SqlConnectionColumnEncryptionSetting.Enabled;

                try
                {
                    SqlConnection sqlConnResources = new SqlConnection(sqlCSB.ConnectionString);
                    sqlConnResources.Open();
                    sqlConnResources.Close();
                    sqlConnResources.Dispose();
                    connected = true;
                }
                catch (SqlException sqlE)
                {
                    if (debug) Console.WriteLine(sqlE);
                    connected = false;
                }
            }
            else
            {
                connected = false;
            }
        }

        public DataTable querySelectSqlSrv(String sql)
        {
            SqlConnection sqlConnResources = null;
            DataTable dataTable = new DataTable();
            SqlDataAdapter sqlDataAdapter = null;
            SqlCommand sqlCommand;

            sqlCSB.IntegratedSecurity = false;
            sqlCSB.ColumnEncryptionSetting = SqlConnectionColumnEncryptionSetting.Disabled;

            try
            {
                sqlConnResources = new SqlConnection(sqlCSB.ConnectionString);
                sqlConnResources.Open();
                if (sqlConnResources != null)
                {
                    sqlCommand = new SqlCommand(sql, sqlConnResources);
                    sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                    sqlDataAdapter.Fill(dataTable);
                    sqlDataAdapter.Dispose();
                }
                sqlConnResources.Close();
            }
            catch (SqlException sqlE)
            {
                if (debug) Console.WriteLine(sqlE);
                dataTable = null;
            }
            finally
            {
            }
            return dataTable;
        }


        /*
        public DataTable querySelectExtended(String sql, List<JObject> param)
        {
            SqlConnection sqlConnResources = null;
            DataTable dataTable = new DataTable();
            SqlDataAdapter sqlDataAdapter = null;
            SqlCommand sqlCommand;
            try
            {
                sqlConnResources = new SqlConnection(sqlCSB.ConnectionString);
                sqlConnResources.Open();
                if (sqlConnResources != null)
                {
                    sqlCommand = new SqlCommand(sql, sqlConnResources);

                    foreach (var item in param.Select((value, i) => new { i, value }))
                    {
                        string paramKey = "@PARAM_" + item.i;

                        JObject obj = null;
                        obj = JObject.Parse(item.value.ToString());

                        JToken paramValue;
                        if (obj.TryGetValue(paramKey, out paramValue))
                        {
                            sqlCommand.Parameters.AddWithValue(paramKey, paramValue.ToString());
                        }
                    }
                    sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                    sqlDataAdapter.Fill(dataTable);
                    sqlDataAdapter.Dispose();
                }
                sqlConnResources.Close();
            }
            catch (SqlException sqlE)
            {
                if (debug) Console.WriteLine(sqlE);
                dataTable = null;
            }
            finally
            {
            }
            return dataTable;
        }
        */

        public DataTable querySelectExtended(String sql, List<JObject> param)
        {
            SqlConnection sqlConnResources = null;
            DataTable dataTable = new DataTable();
            SqlDataAdapter sqlDataAdapter = null;
            SqlCommand sqlCommand;

            // testing
            sqlCSB.IntegratedSecurity = false;
            sqlCSB.ColumnEncryptionSetting = SqlConnectionColumnEncryptionSetting.Disabled;
            //
            try
            {
                sqlConnResources = new SqlConnection(sqlCSB.ConnectionString);
                sqlConnResources.Open();
                if (sqlConnResources != null)
                {
                    sqlCommand = new SqlCommand(sql, sqlConnResources);

                    foreach (var item in param.Select((value, i) => new { i, value }))
                    {
                        string paramKey = "@PARAM_" + item.i;

                        JObject obj = null;
                        obj = JObject.Parse(item.value.ToString());

                        JToken paramValue;
                        if (obj.TryGetValue(paramKey, out paramValue))
                        {
                            sqlCommand.Parameters.AddWithValue(paramKey, paramValue.ToString());

                        }
                    }
                    //sqlCommand.Prepare();

                    sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                    sqlDataAdapter.Fill(dataTable);
                    sqlDataAdapter.Dispose();

                }
                sqlConnResources.Close();
            }
            catch (SqlException sqlE)
            {
                if (debug) Console.WriteLine(sqlE);
                dataTable = null;
            }
            finally
            {
            }
            return dataTable;
        }

        /*
        public int queryUpdate(String sql)
        {
            SqlConnection sqlConnResources = null;
            SqlDataReader sqlReader = null;

            int result = 0;
            try
            {
                sqlConnResources = new SqlConnection(sqlCSB.ConnectionString);
                sqlConnResources.Open();
                if (sqlConnResources != null)
                {
                    SqlCommand sqlCommand = sqlConnResources.CreateCommand();
                    sqlCommand.CommandText = sql;

                    result = sqlCommand.ExecuteNonQuery();
                }
            }
            catch (SqlException sqlE)
            {
                if (debug) Console.WriteLine(sqlE);
            }
            finally
            {
                if (sqlReader != null)
                {
                    sqlReader.Close();
                }
                sqlConnResources.Close();
                sqlConnResources.Dispose();
            }

            return result;
        }
        */
        //public int queryUpdate(String sql)
        public int queryUpdate(String sql, List<JObject> param)
        {
            SqlConnection sqlConnResources = null;
            SqlDataReader sqlReader = null;

            sqlCSB.IntegratedSecurity = false;
            sqlCSB.ColumnEncryptionSetting = SqlConnectionColumnEncryptionSetting.Disabled;

            int result = 0;
            try
            {
                sqlConnResources = new SqlConnection(sqlCSB.ConnectionString);
                sqlConnResources.Open();
                if (sqlConnResources != null)
                {
                    SqlCommand sqlCommand = sqlConnResources.CreateCommand();
                    sqlCommand.CommandText = sql;
                    foreach (var item in param.Select((value, i) => new { i, value }))
                    {
                        string paramKey = "@PARAM_" + item.i;

                        JObject obj = null;
                        obj = JObject.Parse(item.value.ToString());

                        JToken paramValue;
                        if (obj.TryGetValue(paramKey, out paramValue))
                        {
                            sqlCommand.Parameters.AddWithValue(paramKey, paramValue.ToString());
                        }
                    }

                    result = sqlCommand.ExecuteNonQuery();
                }
            }
            catch (SqlException sqlE)
            {
                if (debug) Console.WriteLine(sqlE);
            }
            finally
            {
                if (sqlReader != null)
                {
                    sqlReader.Close();
                }
                sqlConnResources.Close();
                sqlConnResources.Dispose();
            }

            return result;
        }

        public bool isConnected()
        {
            return connected;
        }

        public void setDBDataSource(String dbDataSource)
        {
            this.dbDataSource = dbDataSource;
        }

        public void setDBCatalog(String dbCatalog)
        {
            this.dbCatalog = dbCatalog;
        }

        public void setDBUsername(String dbUsername)
        {
            this.dbUsername = dbUsername;
        }

        /*
        public void setDBPassword(String dbPassword)
        {
            this.dbPassword = dbPassword;
        }
        */
        public void setDBGembok(String dbGembok)
        {
            this.dbGembok = dbGembok;
        }
    }
}
