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
    public class DBConnection
    {

        private bool debug = false;
        private bool connected = false;
        //Mysql
        private MySqlConnectionStringBuilder sqlCSB = new MySqlConnectionStringBuilder();
        

        //SqlServer
        private SqlConnectionStringBuilder sqlSrv = new SqlConnectionStringBuilder();

        private String dbDataSource = null;
        private String dbCatalog = null;
        private String dbUsername = null;
        private String dbKunci = null;

        public DBConnection(bool debug)
        {
            this.debug = debug;
        }
        #region MySQl Connection
        public void initConnection()
        {
            if (dbDataSource != null)
            {
                sqlCSB.Server = dbDataSource;
                sqlCSB.Database = dbCatalog;
                sqlCSB.UserID = dbUsername;
                sqlCSB.Password = dbKunci;
                sqlCSB.MaximumPoolSize = 100;
                sqlCSB.AllowZeroDateTime = true;
                sqlCSB.ConvertZeroDateTime = true;
                //sqlCSB.IntegratedSecurity = true;

                try
                {
                    MySqlConnection sqlConnResources = new MySqlConnection(sqlCSB.ConnectionString);
                    sqlConnResources.Open();
                    sqlConnResources.Close();
                    sqlConnResources.Dispose();
                    connected = true;
                }
                catch (MySqlException sqlE)
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

        public DataTable XquerySelectExtended(String sql)
        {
            DataTable dataTable = new DataTable();
            MySqlDataAdapter sqlDataAdapter = null;
            MySqlCommand sqlCommand;
            using (MySqlConnection sqlConnResources = new MySqlConnection(sqlCSB.ConnectionString))
            {
                try
                {
                    //sqlConnResources = new MySqlConnection(sqlCSB.ConnectionString);
                    sqlConnResources.Open();
                    if (sqlConnResources != null)
                    {
                        sqlCommand = new MySqlCommand(sql, sqlConnResources);
                        sqlDataAdapter = new MySqlDataAdapter(sqlCommand);

                        sqlDataAdapter.Fill(dataTable);
                        sqlDataAdapter.Dispose();
                    }
                    sqlConnResources.Close();
                }
                catch (MySqlException sqlE)
                {
                    if (debug) Console.WriteLine(sqlE);
                    dataTable = null;
                }

            }
            return dataTable;
        }

        public int XqueryUpdate(String sql)
        {
            MySqlConnection sqlConnResources = null;
            MySqlDataReader sqlReader = null;

            int result = 0;
            try
            {
                sqlConnResources = new MySqlConnection(sqlCSB.ConnectionString);
                sqlConnResources.Open();
                if (sqlConnResources != null)
                {
                    MySqlCommand sqlCommand = sqlConnResources.CreateCommand();
                    sqlCommand.CommandText = sql;
                    if (sql != "")
                        result = sqlCommand.ExecuteNonQuery();
                }
            }
            catch (MySqlException sqlE)
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

        public long queryInsertWithLastId(String sql)
        {
            MySqlConnection sqlConnResources = null;
            MySqlDataReader sqlReader = null;

            int result = 0;
            long insertId = 0;
            try
            {
                sqlConnResources = new MySqlConnection(sqlCSB.ConnectionString);
                sqlConnResources.Open();
                if (sqlConnResources != null)
                {
                    MySqlCommand sqlCommand = sqlConnResources.CreateCommand();
                    sqlCommand.CommandText = sql;
                    if (sql != "")
                        result = sqlCommand.ExecuteNonQuery();

                    if (result > 0)
                        insertId = sqlCommand.LastInsertedId;
                }
            }
            catch (MySqlException sqlE)
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

            return insertId;
        }

        public long queryInsertWithLastIdWithExtendParam(String sql, List<JObject> param)
        {
            MySqlConnection sqlConnResources = null;
            MySqlDataReader sqlReader = null;

            int result = 0;
            long insertId = 0;
            try
            {
                sqlConnResources = new MySqlConnection(sqlCSB.ConnectionString);


                sqlConnResources.Open();
                if (sqlConnResources != null)
                {
                    MySqlCommand sqlCommand = sqlConnResources.CreateCommand();
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


                    if (sql != "")
                    {
                        result = sqlCommand.ExecuteNonQuery();
                    }
                    if (result > 0)
                    {
                        insertId = sqlCommand.LastInsertedId;
                    }
                }
            }
            catch (MySqlException sqlE)
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

            return insertId;
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

        public void setDBPassword(String dbPassword)
        {
            this.dbKunci = dbPassword;
        }

        public DataTable querySelectExtendedWithParam(String sql, List<JObject> param)
        {
            DataTable dataTable = new DataTable();
            MySqlDataAdapter sqlDataAdapter = null;
            MySqlCommand sqlCommand;

            using (MySqlConnection sqlConnResources = new MySqlConnection(sqlCSB.ConnectionString))
            {
                try
                {
                    sqlConnResources.Open();
                    if (sqlConnResources != null)
                    {
                        sqlCommand = new MySqlCommand(sql, sqlConnResources);

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

                        sqlDataAdapter = new MySqlDataAdapter(sqlCommand);

                        sqlDataAdapter.Fill(dataTable);
                        sqlDataAdapter.Dispose();
                    }
                    sqlConnResources.Close();
                }
                catch (MySqlException sqlE)
                {
                    if (debug) Console.WriteLine(sqlE);
                    dataTable = null;
                }
            }

            return dataTable;
        }

        public int queryUpdateWithParam(String sql, List<JObject> param)
        {
            int result = 0;
            using (MySqlConnection sqlConnResources = new MySqlConnection(sqlCSB.ConnectionString))
            {
                try
                {
                    sqlConnResources.Open();
                    if (sqlConnResources != null)
                    {
                        MySqlCommand sqlCommand = new MySqlCommand(sql, sqlConnResources);

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
                    sqlConnResources.Close();
                }
                catch (MySqlException sqlE)
                {
                    if (debug) Console.WriteLine(sqlE);
                }
            }

            return result;
        }

        public int queryUpdateWithParam2(String[] sqlArray, List<JObject>[] paramArray)
        {
            if (sqlArray.Length != paramArray.Length)
            {
                return 0;
            }
            else
            {
                int result = 0;
                using (MySqlConnection sqlConnResources = new MySqlConnection(sqlCSB.ConnectionString))
                {
                    try
                    {
                        MySqlTransaction transaction;
                        sqlConnResources.Open();
                        transaction = sqlConnResources.BeginTransaction();
                        bool queryStatus = true;

                        if (sqlConnResources != null)
                        {
                            MySqlCommand sqlCommand;

                            for (int x = 0; x < sqlArray.Length; x++)
                            {
                                result = 0;
                                sqlCommand = new MySqlCommand(sqlArray[x], sqlConnResources);

                                foreach (var item in paramArray[x].Select((value, i) => new { i, value }))
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

                                try
                                {
                                    result = sqlCommand.ExecuteNonQuery();
                                }
                                catch (Exception cvf) { }

                                if (result > 0)
                                {
                                    queryStatus = true;
                                }
                                else
                                {
                                    queryStatus = false;
                                    transaction.Rollback();

                                    break;
                                }
                            }

                            if (queryStatus)
                            {
                                result = 1;
                                transaction.Commit();
                            }
                            else
                            {
                                result = 0;
                            }

                        }
                        sqlConnResources.Close();
                    }
                    catch (MySqlException sqlE)
                    {
                        if (debug) Console.WriteLine(sqlE);
                    }
                }

                return result;
            }
        }

        public int queryInsertWithParamAndRollBack(String[] sqlArray, List<JObject>[] paramArray)
        {
            if (sqlArray.Length != paramArray.Length)
            {
                return 0;
            }
            else
            {
                int result = 0;
                using (MySqlConnection sqlConnResources = new MySqlConnection(sqlCSB.ConnectionString))
                {
                    try
                    {
                        MySqlTransaction transaction;
                        sqlConnResources.Open();
                        transaction = sqlConnResources.BeginTransaction();
                        bool queryStatus = true;

                        if (sqlConnResources != null)
                        {
                            MySqlCommand sqlCommand;

                            for (int x = 0; x < sqlArray.Length; x++)
                            {
                                result = 0;
                                sqlCommand = new MySqlCommand(sqlArray[x], sqlConnResources);

                                foreach (var item in paramArray[x].Select((value, i) => new { i, value }))
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

                                try
                                {
                                    result = sqlCommand.ExecuteNonQuery();
                                }
                                catch (Exception cvf) { }

                                if (result > 0)
                                {
                                    queryStatus = true;
                                }
                                else
                                {
                                    queryStatus = false;
                                    transaction.Rollback();

                                    break;
                                }
                            }

                            if (queryStatus)
                            {
                                result = 1;
                                transaction.Commit();
                            }
                            else
                            {
                                result = 0;
                            }

                        }
                        sqlConnResources.Close();
                    }
                    catch (MySqlException sqlE)
                    {
                        if (debug) Console.WriteLine(sqlE);
                    }
                }

                return result;
            }
        }

        public int queryInsertWithPrevInsertedId_ori(String sqlInsert01, String sqlInsert02, List<JObject> paramInsert01, List<JObject> paramInsert02)
        {
            int result = 0;

            using (MySqlConnection sqlConnResources = new MySqlConnection(sqlCSB.ConnectionString))
            {
                try
                {
                    MySqlTransaction transaction;
                    sqlConnResources.Open();
                    transaction = sqlConnResources.BeginTransaction();
                    bool queryStatus = true;

                    if (sqlConnResources != null)
                    {
                        MySqlCommand sqlCommand;
                        long insertedID = 0;

                        //QUERY INSERT 1
                        result = 0;
                        sqlCommand = new MySqlCommand(sqlInsert01, sqlConnResources);

                        foreach (var item in paramInsert01.Select((value, i) => new { i, value }))
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

                        try
                        {
                            result = sqlCommand.ExecuteNonQuery();
                            insertedID = sqlCommand.LastInsertedId;
                        }
                        catch (Exception cvf) { }

                        if (result > 0)
                        {
                            //QUERY INSERT 2
                            result = 0; //direset dulu resultnya
                            sqlCommand = new MySqlCommand(sqlInsert02, sqlConnResources);

                            foreach (var item in paramInsert02.Select((value, i) => new { i, value }))
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

                            //tambah id dari query pertama
                            sqlCommand.Parameters.AddWithValue("@PARAM_INSERTED_ID", insertedID.ToString());

                            try
                            {
                                result = sqlCommand.ExecuteNonQuery();
                            }
                            catch (Exception cvf) { }

                            if (result > 0)
                            {
                                result = 1;
                                transaction.Commit();
                            }
                            else
                            {
                                queryStatus = false;
                                transaction.Rollback();

                                //break;
                            }
                        }
                        else
                        {
                            queryStatus = false;
                            transaction.Rollback();

                            //break;
                        }
                    }
                    sqlConnResources.Close();
                }
                catch (MySqlException sqlE)
                {
                    if (debug) Console.WriteLine(sqlE);
                }
            }

            return result;
        }

        public int queryInsertWithPrevInsertedId(String sqlInsert01, String sqlInsert02, List<JObject> paramInsert01, List<JObject> paramInsert02, ref int pInserted)
        {
            int result = 0;

            using (MySqlConnection sqlConnResources = new MySqlConnection(sqlCSB.ConnectionString))
            {
                try
                {
                    MySqlTransaction transaction;
                    sqlConnResources.Open();
                    transaction = sqlConnResources.BeginTransaction();
                    bool queryStatus = true;

                    if (sqlConnResources != null)
                    {
                        MySqlCommand sqlCommand;
                        long insertedID = 0;

                        //QUERY INSERT 1
                        result = 0;
                        sqlCommand = new MySqlCommand(sqlInsert01, sqlConnResources);

                        foreach (var item in paramInsert01.Select((value, i) => new { i, value }))
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

                        try
                        {
                            result = sqlCommand.ExecuteNonQuery();
                            insertedID = sqlCommand.LastInsertedId;

                            pInserted = Convert.ToInt32(insertedID);
                        }
                        catch (Exception cvf) { }

                        if (result > 0)
                        {
                            //QUERY INSERT 2
                            result = 0; //direset dulu resultnya
                            sqlCommand = new MySqlCommand(sqlInsert02, sqlConnResources);

                            foreach (var item in paramInsert02.Select((value, i) => new { i, value }))
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

                            //tambah id dari query pertama
                            sqlCommand.Parameters.AddWithValue("@PARAM_INSERTED_ID", insertedID.ToString());

                            try
                            {
                                result = sqlCommand.ExecuteNonQuery();
                            }
                            catch (Exception cvf) { }

                            if (result > 0)
                            {
                                result = 1;
                                transaction.Commit();
                            }
                            else
                            {
                                queryStatus = false;
                                transaction.Rollback();

                            }
                        }
                        else
                        {
                            queryStatus = false;
                            transaction.Rollback();

                        }
                    }
                    sqlConnResources.Close();
                }
                catch (MySqlException sqlE)
                {
                    if (debug) Console.WriteLine(sqlE);
                }
            }

            return result;
        }

        #endregion

       
    }
}
