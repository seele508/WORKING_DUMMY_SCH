using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Text.RegularExpressions;
using System.IO;
using Helper;
using System.Net;
using Newtonsoft.Json.Linq;

namespace SchTracer.helper
{
    class cDataSqlSrv
    {
        private bool debug = false;

        private DBConnectionSqlSrv oDBSQLSrv;
        private Config oConfig;

        
        public cDataSqlSrv(DBConnectionSqlSrv oDBSQLSrv)
        {
            this.oDBSQLSrv = oDBSQLSrv;
            //this.oConfig = oConfig;

            
        }
        
        public DataTable fetchDataDummySwiftdat(int maxThread, int tred, int maxResult)
        {
            String sql = "";
            sql = "SELECT TOP " + maxResult + " * FROM GudangMT WHERE (RowID %" + maxThread + "=" + tred + ")" +
                " and STATUS IN ('1') ORDER BY RowID ASC";

            var list01 = new List<JObject>();
 
            DataTable dt = oDBSQLSrv.querySelectExtended(sql,list01);

            return dt;
        }

        
        public DataTable fetchDataGudangMT(int maxThread, int tred, int maxResult, string mtType, DateTime dateGet)
        {
            String sql = "";
            //sql = "SELECT TOP " + maxResult + " * FROM GudangMT with(nolock) WHERE (RowID %" + maxThread + "=" + tred + ")";
            //sql += " and "+ mtType + " and InOut = 'I' and loadDate > '" + dateGet + "' order by RowID desc";

            /*
            sql = "SELECT TOP @MaxResult * FROM GudangMT with(nolock) WHERE (RowID %@MaxThread=@Tred)";
            sql += " and @MtType and InOut = 'I' and loadDate > @DateGet order by RowID desc";

            sql = sql.Replace("@MaxResult", maxResult.ToString());
            sql = sql.Replace("@MaxThread", maxThread.ToString());
            sql = sql.Replace("@Tred", tred.ToString());
            sql = sql.Replace("@MtType", mtType);
            sql = sql.Replace("@DateGet", "'" + dateGet.ToString() + "'");
            */


            sql = "SELECT * FROM GudangMT with(nolock) WHERE ";
            sql += " @PARAM_0 and InOut = 'I' and loadDate > '@PARAM_1' order by RowID desc";

            //sql = "SELECT * FROM GudangMT with(nolock) WHERE ";
            //sql += " InOut like @PARAM_0 order by RowID desc";

            var list = new List<JObject>();
            //list.Add(new JObject(new JProperty("@PARAM_0", maxResult)));
            //list.Add(new JObject(new JProperty("@PARAM_0", "'X'")));
            //list.Add(new JObject(new JProperty("@PARAM_1", maxThread)));
            //list.Add(new JObject(new JProperty("@PARAM_2", tred)));

            list.Add(new JObject(new JProperty("@PARAM_0", mtType.ToString())));
            list.Add(new JObject(new JProperty("@PARAM_1", dateGet.ToString())));

            DataTable dt = oDBSQLSrv.querySelectExtended(sql, list);
            //DataTable dt = oDBSQLSrv.querySelectSqlSrv(sql);

            return dt;
        }


        public DataTable fetchDataMirroring(int maxThread, int tred, int maxResult, string mtType, string dateGet)
        {
            String sql = "";
            
            sql = "SELECT * FROM GudangMT with(nolock) WHERE (RowID %" + maxThread + "=" + tred + ")";
            sql += " and " + mtType + " and InOut = 'I' and loadDate > @PARAM_0 order by RowID desc";

            var list = new List<JObject>();
            list.Add(new JObject(new JProperty("@PARAM_0", dateGet)));

            DataTable dt = oDBSQLSrv.querySelectExtended(sql, list);
            //DataTable dt = oDBSQLSrv.querySelectSqlSrv(sql);

            return dt;
        }

        public DataTable dataGudangMT_ori(int maxThread, int tred, int maxResult, string mtType, string dateGet)
        {
            String sql = "";
            var list = new List<JObject>();

            //sql = "SELECT RowID, RefNo FROM GudangMT with(nolock) WHERE (RowID %" + maxThread + "=" + tred + ")";
            //sql += " and " + mtType.Replace("''","'") + " and InOut = 'I' and loadDate > @PARAM_0 order by RowID desc";

            sql = string.Format("SELECT RowID, RefNo FROM GudangMT with(nolock) WHERE (RowID %" + maxThread + "=" + tred + ") and {0} and InOut = 'I' and loadDate > @PARAM_0 order by RowID desc", mtType);

            list.Add(new JObject(new JProperty("@PARAM_0", dateGet)));


            DataTable dt = oDBSQLSrv.querySelectExtended(sql, list);

            return dt;

        }

        public DataTable dataGudangMT(int maxThread, int tred, int maxResult, string mtType, string dateGet)
        {
            String sql = "";
            var list = new List<JObject>();

            string[] listTypeValue = mtType.Split('|');
            int IndTotal = 0;
            //sql = "SELECT RowID, RefNo FROM GudangMT with(nolock) WHERE (RowID %" + maxThread + "=" + tred + ")";
            //sql += " and " + mtType.Replace("''","'") + " and InOut = 'I' and loadDate > @PARAM_0 order by RowID desc";

            sql = " SELECT RowID, RefNo FROM GudangMT with(nolock) ";
            sql += " WHERE (RowID %" + maxThread + "=" + tred + ") and ";
            sql += " type <> '103' and (";
            for (int i = 0; i < listTypeValue.Length; i++)
            {
                var value = int.TryParse(listTypeValue[i], out _);
                if (value)
                {
                    if (IndTotal > 0)
                    {
                        sql += " or ";
                    }
                    sql += "type like @PARAM_" + IndTotal.ToString() + " ";
                    list.Add(new JObject(new JProperty("@PARAM_" + IndTotal.ToString(), listTypeValue[i].ToString() + "%" )));
                    IndTotal = IndTotal + 1;
                }
            }
            sql += " ) and InOut = 'I' and loadDate > ";

            sql += "@PARAM_" + IndTotal.ToString();
            list.Add(new JObject(new JProperty("@PARAM_" + IndTotal.ToString(), dateGet)));

            sql += " order by RowID desc ";

            DataTable dt = oDBSQLSrv.querySelectExtended(sql, list);

            return dt;
        }

        
        public DataTable fetchDataMirroringFinal(List<String> ListMirroringID)
        {
            String sql = "";
            
            var list = new List<JObject>();
            
            sql = "SELECT * FROM GudangMT with(nolock) WHERE RowID IN (";

            if (ListMirroringID.Count > 0)
            {
                for (int i = 0; i < ListMirroringID.Count; i++)
                {
                    if (i > 0)
                    { 
                        sql += ", ";
                    }
                    sql += "@PARAM_" + i.ToString();
                    
                    list.Add(new JObject(new JProperty("@PARAM_"+ i.ToString(), ListMirroringID[i])));
                }
            }
                
            sql += ") order by RowID desc";

            DataTable dt = oDBSQLSrv.querySelectExtended(sql, list);

            return dt;
        }

        public bool updateDummyGudangMT(int status, string id)
        {
            bool hasil = false;

            string sql = "UPDATE GudangMT SET STATUS = @PARAM_0 WHERE RowID = @PARAM_1";

            var list = new List<JObject>();
            list.Add(new JObject(new JProperty("@PARAM_0", status)));
            list.Add(new JObject(new JProperty("@PARAM_1", id)));

            //int res = oDB.queryUpdateWithParam(sql, list);
            int res = oDBSQLSrv.queryUpdate(sql, list);

            if (res > 0)
            {
                hasil = true;
            }
            return hasil;
        }


        //Mapping
        #region Mapping

        public DataTable FetchJoinData(string pTag21)
        {
            String sql = "";

            

            sql = "select gm.*, mt.ISKERJASAMA from GudangMT gm LEFT JOIN MT103Track mt ON gm.RefNo=mt.TAG20 ";
            sql += " where gm.RefNo = @PARAM_0 and  gm.type = '103'";

            

            var list = new List<JObject>();
            list.Add(new JObject(new JProperty("@PARAM_0", pTag21)));

            DataTable dt = oDBSQLSrv.querySelectExtended(sql,list);

            return dt;
        }

        public DataTable fetchCekMTGudangMT(string pTag21)
        {
            String sql = "";
            //revisi
            
            sql = "select top (1) * from GudangMT with (nolock) where RefNo = @PARAM_0 and Type not in ('202','103') order by RowID";


            var list = new List<JObject>();
            list.Add(new JObject(new JProperty("@PARAM_0", pTag21)));

            
            DataTable dt = oDBSQLSrv.querySelectExtended(sql, list);

            return dt;
        }
        #endregion
    }
}
