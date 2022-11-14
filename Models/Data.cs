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
using SchTracer.Models;
using System.Globalization;
using System.Configuration;

namespace SchTracer.helper
{
    public class cData
    {
        public static Regex digitsOnly = new Regex(@"[^\d]");

        private DBConnection oDB;

        public cData(DBConnection oDB)
        {
            this.oDB = oDB;

        }

        public bool updateSchRunning(string appName, string appFunction)
        {
            bool hasil = false;
            string sql = "UPDATE md_scheduler_activity SET IS_RUN = 1, LAST_RUN = now() WHERE SCH_NAME = @PARAM_0 AND SCH_FUNCTION = @PARAM_1";
            var list = new List<JObject>();
            list.Add(new JObject(new JProperty("@PARAM_0", appName)));
            list.Add(new JObject(new JProperty("@PARAM_1", appFunction)));

            int res = oDB.queryUpdateWithParam(sql, list);

            if (res > 0)
            {
                hasil = true;
            }
            return hasil;
        }

        public string getMtTypeParam(string param_type)
        {
            string mt_type = "";

            string sql = "SELECT Parameter_Desc FROM swt_parameter WHERE Parameter_Type = @PARAM_0";

            var list = new List<JObject>();
            list.Add(new JObject(new JProperty("@PARAM_0", param_type)));

            DataTable res = oDB.querySelectExtendedWithParam(sql, list);

            if (res.Rows.Count == 0)
            {
                return "";
            }
            else
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    var getMtType = res.Rows[i].Field<String>("Parameter_Desc");
                    mt_type = getMtType.ToString();
                }
            }

            return mt_type;

        }

        //////////////
        //Testing 01//
        //////////////
        public DataTable DataDummyTracer(int maxThread, int tred, int maxResult)
        {
            String sql = "";
            sql = "SELECT * FROM mt_tracer WHERE (ROWID %" + maxThread + "=" + tred + ")" +
                " and STATUS IN ('11') ";
            sql += " ORDER BY ROWID ASC LIMIT " + maxResult + " ";

            var list01 = new List<JObject>();

            DataTable dt = oDB.querySelectExtendedWithParam(sql, list01);

            return dt;
        }
        
        /*
        public DataTable DataMtTracer(string dateGet)
        {
            String sql = "";
            //revisi
            sql = "SELECT CONVERT(splitter_row_id, SIGNED) AS RowID, RefNo FROM mt_tracer WHERE ";
            sql += " loadDate > @PARAM_0 AND splitter_row_id IS NOT NULL order by RowID desc";

            var list01 = new List<JObject>();
            list01.Add(new JObject(new JProperty("@PARAM_0", dateGet)));

            DataTable dt = oDB.querySelectExtendedWithParam(sql, list01);

            return dt;
        }
        */

        public DataTable DataMtTracer(int maxThread, int tred, int maxResult, string mtType, string dateGet)
        {
            String sql = "";

            string[] listTypeValue = mtType.Split('|');
            int IndTotal = 0;

            var list = new List<JObject>();

            //revisi
            sql = "SELECT CONVERT(splitter_row_id, SIGNED) AS RowID, RefNo ";
            sql += " FROM mt_tracer ";
            sql += " WHERE (splitter_row_id %" + maxThread + "=" + tred + ") and ";
            
            sql += " Type <> '103' and (";
            for (int i = 0; i < listTypeValue.Length; i++)
            {
                var value = int.TryParse(listTypeValue[i], out _);
                if (value)
                {
                    if (IndTotal > 0)
                    {
                        sql += " or ";
                    }
                    sql += "Type like @PARAM_" + IndTotal.ToString() + " ";
                    list.Add(new JObject(new JProperty("@PARAM_" + IndTotal.ToString(), listTypeValue[i].ToString() + "%")));
                    IndTotal = IndTotal + 1;
                }
            }
            sql += " ) ";
            sql += " AND loadDate > @PARAM_" + IndTotal.ToString();
            sql += " AND splitter_row_id IS NOT NULL ";
            sql += " order by RowID desc ";

            list.Add(new JObject(new JProperty("@PARAM_" + IndTotal.ToString(), dateGet)));
            // list.Add(new JObject(new JProperty("@PARAM_0", dateGet)));

            DataTable dt = oDB.querySelectExtendedWithParam(sql, list);

            return dt;
        }


        public bool updateDummyMtTracer(int status, string id)
        {
            bool hasil = false;

            string sql = "UPDATE mt_tracer SET STATUS = @PARAM_0 WHERE RowID = @PARAM_1";

            var list = new List<JObject>();
            list.Add(new JObject(new JProperty("@PARAM_0", status)));
            list.Add(new JObject(new JProperty("@PARAM_1", id)));

            int res = oDB.queryUpdateWithParam(sql, list);

            if (res > 0)
            {
                hasil = true;
            }
            return hasil;
        }

        //////////////
        //Mirroring//
        //////////////
        public bool checkMirroringID(string MirroringID)
        {
            bool hasil = false;
            DataTable result = null;
            

            string sql = "SELECT * FROM mt_tracer WHERE splitter_row_id IN (@PARAM_0) ";
            var list = new List<JObject>();
            list.Add(new JObject(new JProperty("@PARAM_0", MirroringID)));

            

            result = oDB.querySelectExtendedWithParam(sql, list);

            if (result.Rows.Count == 0)
            {
                hasil = true;
            }
            return hasil;
        }

        public bool insertMirroringWithParam(string valRefNo, string valStrLine, string valMT, string valType, int status ,string valInOut, string valRowId, string valLoadDate)
        {
            bool hasil = false;
            string sqlValues = " INSERT INTO mt_tracer(RefNo,Tag21,STATUS,MessageType,Type,last_update,RECEIVED_ON,mt_category,splitter_row_id,loadDate,InOu) VALUES ";
            sqlValues += "(@PARAM_0,@PARAM_1,@PARAM_2,@PARAM_3,@PARAM_4,@PARAM_5,@PARAM_6,@PARAM_7,@PARAM_8,@PARAM_9,PARAM_10) ";

            var list = new List<JObject>();
            list.Add(new JObject(new JProperty("@PARAM_0", valRefNo)));
            list.Add(new JObject(new JProperty("@PARAM_1", valStrLine)));
            list.Add(new JObject(new JProperty("@PARAM_2", 0)));
            list.Add(new JObject(new JProperty("@PARAM_3", valMT)));
            list.Add(new JObject(new JProperty("@PARAM_4", valType)));
            list.Add(new JObject(new JProperty("@PARAM_5", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))));
            list.Add(new JObject(new JProperty("@PARAM_6", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))));
            list.Add(new JObject(new JProperty("@PARAM_7", 0)));
            list.Add(new JObject(new JProperty("@PARAM_8", valRowId)));
            list.Add(new JObject(new JProperty("@PARAM_9", valLoadDate)));
            list.Add(new JObject(new JProperty("@PARAM_10", valInOut)));


            int res = oDB.queryUpdateWithParam(sqlValues, list);

            if (res > 0)
            {
                hasil = true;
            }
            return hasil;
        }

        

        //Revision
        public int insertMirroring(string valRefNo, string valStrLine, string valMT, string valType, int status, string valInOut, string valRowId, string valLoadDate)
        {
            //bool hasil = false;
            int insertId = 0;
            string lastUpdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string recievedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            
            string sqlValues = " INSERT INTO mt_tracer(RefNo,Tag21,STATUS,MessageType,Type,last_update,RECEIVED_ON,mt_category,splitter_row_id,loadDate,InOu) VALUES ";
            sqlValues += "(@PARAM_0, @PARAM_1, @PARAM_2, @PARAM_3, ";
            sqlValues += "@PARAM_4, @PARAM_5, @PARAM_6, @PARAM_7,";
            sqlValues += "@PARAM_8, @PARAM_9, @PARAM_10 ) ";


            var list = new List<JObject>();
            list.Add(new JObject(new JProperty("@PARAM_0", valRefNo)));
            list.Add(new JObject(new JProperty("@PARAM_1", valStrLine)));
            list.Add(new JObject(new JProperty("@PARAM_2", 0)));
            list.Add(new JObject(new JProperty("@PARAM_3", valMT)));

            list.Add(new JObject(new JProperty("@PARAM_4", valType)));
            list.Add(new JObject(new JProperty("@PARAM_5", lastUpdate)));
            list.Add(new JObject(new JProperty("@PARAM_6", recievedOn)));
            list.Add(new JObject(new JProperty("@PARAM_7", 0)));

            list.Add(new JObject(new JProperty("@PARAM_8", valRowId)));
            list.Add(new JObject(new JProperty("@PARAM_9", valLoadDate)));
            list.Add(new JObject(new JProperty("@PARAM_10", valInOut)));




            long res = oDB.queryInsertWithLastIdWithExtendParam(sqlValues, list);

            if (res > 0)
            {
                //Long to int
                insertId = Convert.ToInt32(res);
            }
            return insertId;
        }

        //Mapping
        
        public DataTable fetchMtTracerData(int maxthread, int thread)
        {
            String sql = "";

            sql = "SELECT * FROM mt_tracer WHERE (RowID %" + maxthread + "=" + thread + ") and status IN ('0','80')";

            DataTable dt = oDB.XquerySelectExtended(sql);


            return dt;
        }

        public DataTable fetchMtMappingData(string pTag21)
        {
            String sql = "";

            sql = "select * from mt_mapping where REF_TO_CAPTURE = @PARAM_0 limit 1";
            var list = new List<JObject>();
            list.Add(new JObject(new JProperty("@PARAM_0", pTag21)));

            DataTable dt = oDB.querySelectExtendedWithParam(sql, list);

            return dt;
        }

        public List<string> getListRefToCapture(string pref_103_ori)
        {
            List<string> list_row_id = new List<string>();

            string sql = " SELECT ROWID_REF_TO_CAPTURE FROM mt_mapping WHERE ori_mt103_ref = @PARAM_0 ";

            var list = new List<JObject>();
            list.Add(new JObject(new JProperty("@PARAM_0", pref_103_ori)));

            DataTable dt = oDB.querySelectExtendedWithParam(sql, list);

            try
            {
                if (dt.Rows.Count > 0)
                {
                    list_row_id = dt.AsEnumerable().Select(r => r.Field<string>("ROWID_REF_TO_CAPTURE")).ToList();
                }
            }
            catch (Exception ex)
            { 
            
            }

            return list_row_id;
        }

        public List<string> getStatuId()
        {
            List<string> list_status = new List<string>();

            string sql = " SELECT status_id FROM swt_status WHERE auto_close = 1 ";

            var list = new List<JObject>();

            DataTable dt = oDB.querySelectExtendedWithParam(sql, list);

            try
            {
                if (dt.Rows.Count > 0)
                {
                    list_status = dt.AsEnumerable().Select(r => r.Field<string>("status_id")).ToList();
                }
            }
            catch (Exception ex)
            { 
            }
            return list_status;
        }

        
        //insert revisi
        public bool insertDBMapping(
            string capture, string prev, string ori, int isKerjasama,
            string inout, string correspondent, string currency, string tag20,
            string mt_103, float amount, string accountnum, string accountname,
            string tanggal, string rowid_ref_to_capture, string rowid_previous_rev, string rowid_ori_mt103_ref,
            string mt_ref, string mt_prev, string dateCapture, string datePrev,
            string dateOri, string typeCapture, string typePrev, string typeOri,
            string InOutPrev, string InOutOri, string InOutCapture)
        {
            bool statusdelete = false;

            bool statusinsert = false;

            string dtFormat = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            string sqlDelete = "";
            string sqlInsert = "";

            #region Delete data
            try
            {
                sqlDelete = "DELETE FROM mt_mapping where REF_TO_CAPTURE = @PARAM_0 and rowid_ref_to_capture = @PARAM_1 ";

                var list = new List<JObject>();
                list.Add(new JObject(new JProperty("@PARAM_0", capture)));
                list.Add(new JObject(new JProperty("@PARAM_1", rowid_ref_to_capture)));
                int res = oDB.queryUpdateWithParam(sqlDelete, list);

                if (res > 0)
                {
                    statusdelete = true;
                }
            }
            catch (Exception exDelete)
            {

            }
            #endregion

            #region Insert data
            try
            {
                string fixDateCapture = DateTime.ParseExact(dateCapture, "M/d/yyyy h:mm:ss tt", CultureInfo.GetCultureInfo("en-US")).ToString("yyyy-MM-dd HH:mm:ss");
                string fixDatePrev = DateTime.ParseExact(datePrev, "M/d/yyyy h:mm:ss tt", CultureInfo.GetCultureInfo("en-US")).ToString("yyyy-MM-dd HH:mm:ss");
                string fixDateOri = DateTime.ParseExact(dateOri, "M/d/yyyy h:mm:ss tt", CultureInfo.GetCultureInfo("en-US")).ToString("yyyy-MM-dd HH:mm:ss");


                sqlInsert = "insert into mt_mapping(REF_TO_CAPTURE,PREVIOUS_REV,ORI_MT103_REF,MSG_DOWNLOADED,";
                sqlInsert += "ENTRY_DATE,DOWNLOADED_DATE,CORRESPONDENT,ISKERJASAMA,INOROUT,currency,TAG20,mt_103,amount, account_num, ";
                sqlInsert += "account_name, tanggal,ROWID_REF_TO_CAPTURE,ROWID_PREVIOUS_REV,ROWID_ORI_MT103_REF,mt_ref,mt_prev,dateCapture,";
                sqlInsert += "datePrev,dateOri,typeCapture,typePrev,typeOri,InOutPrev,InOutOri,InOutCapture) ";
                sqlInsert += "VALUES ";
                sqlInsert += "(@PARAM_0, @PARAM_1, @PARAM_2, @PARAM_3, ";
                sqlInsert += "@PARAM_4, @PARAM_5, @PARAM_6, @PARAM_7, ";
                sqlInsert += "@PARAM_8, @PARAM_9, @PARAM_10, @PARAM_11, ";
                sqlInsert += "@PARAM_12, @PARAM_13, @PARAM_14, @PARAM_15, ";
                sqlInsert += "@PARAM_16, @PARAM_17, @PARAM_18, @PARAM_19, ";
                sqlInsert += "@PARAM_20, @PARAM_21, @PARAM_22, @PARAM_23, ";
                sqlInsert += "@PARAM_24, @PARAM_25, @PARAM_26, @PARAM_27, ";
                sqlInsert += "@PARAM_28, @PARAM_29) ";

                var list = new List<JObject>();

                list.Add(new JObject(new JProperty("@PARAM_0", capture))); //1
                list.Add(new JObject(new JProperty("@PARAM_1", prev))); //2
                list.Add(new JObject(new JProperty("@PARAM_2", ori))); //3
                list.Add(new JObject(new JProperty("@PARAM_3", 0))); //4 

                list.Add(new JObject(new JProperty("@PARAM_4", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")))); //5
                list.Add(new JObject(new JProperty("@PARAM_5", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")))); //6
                list.Add(new JObject(new JProperty("@PARAM_6", correspondent))); //7
                list.Add(new JObject(new JProperty("@PARAM_7", isKerjasama))); //8

                list.Add(new JObject(new JProperty("@PARAM_8", inout))); //9
                list.Add(new JObject(new JProperty("@PARAM_9", currency))); //10
                list.Add(new JObject(new JProperty("@PARAM_10", tag20))); //11
                list.Add(new JObject(new JProperty("@PARAM_11", mt_103))); //12

                list.Add(new JObject(new JProperty("@PARAM_12", amount))); //13
                list.Add(new JObject(new JProperty("@PARAM_13", accountnum))); //14
                list.Add(new JObject(new JProperty("@PARAM_14", accountname))); //15
                list.Add(new JObject(new JProperty("@PARAM_15", tanggal))); //16

                list.Add(new JObject(new JProperty("@PARAM_16", rowid_ref_to_capture))); //17
                list.Add(new JObject(new JProperty("@PARAM_17", rowid_previous_rev))); //18
                list.Add(new JObject(new JProperty("@PARAM_18", rowid_ori_mt103_ref))); //19
                list.Add(new JObject(new JProperty("@PARAM_19", mt_ref))); //20

                list.Add(new JObject(new JProperty("@PARAM_20", mt_prev))); //21
                list.Add(new JObject(new JProperty("@PARAM_21", fixDateCapture))); //22
                list.Add(new JObject(new JProperty("@PARAM_22", fixDatePrev))); //23
                list.Add(new JObject(new JProperty("@PARAM_23", fixDateOri))); //24

                list.Add(new JObject(new JProperty("@PARAM_24", typeCapture))); //25
                list.Add(new JObject(new JProperty("@PARAM_25", typePrev))); //26
                list.Add(new JObject(new JProperty("@PARAM_26", typeOri))); //27
                list.Add(new JObject(new JProperty("@PARAM_27", InOutPrev))); //28

                list.Add(new JObject(new JProperty("@PARAM_28", InOutOri))); //29
                list.Add(new JObject(new JProperty("@PARAM_29", InOutCapture ))); //30

                int res = oDB.queryUpdateWithParam(sqlInsert, list);

                if (res > 0)
                {
                    statusinsert = true;
                }
            }
            catch (Exception exInsert)
            {

            }
            #endregion

            return statusinsert;
        }

        
        public bool CheckSumberMT(string refno, string reffMT103, string rowidSplitter)
        {
            try
            {
                bool isTrue = false;
                bool finalRes = false;
                string sqlCek = "select * from swt_channel";

                

                DataTable res = oDB.XquerySelectExtended(sqlCek);

                reffMT103 = reffMT103.ToLower();
                string chn = "";

                if (res.Rows.Count > 0)
                {
                    for (int i = 0; i < res.Rows.Count; i++)
                    { //dataMtTracer.Rows[i]["Tag21"].ToString();
                        string channel = res.Rows[i]["value"].ToString();

                        if (reffMT103.Contains(channel))
                        {
                            isTrue = true;
                            chn = channel;
                        }
                    }

                    if (isTrue)
                    {
                        string valRow_id_splitter = "";
                        string sqlupdate = "";

                        
                        var listMtTracer = new List<JObject>();
                       

                        if (rowidSplitter.Substring(0, 1).Contains("O"))
                        {
                            valRow_id_splitter = rowidSplitter.Substring(1);
                            sqlupdate = "UPDATE mt_tracer SET channel = @PARAM_0 WHERE(RefNo = @PARAM_1 and RowID = @PARAM_2)";

                            listMtTracer.Add(new JObject(new JProperty("@PARAM_0", chn)));
                            listMtTracer.Add(new JObject(new JProperty("@PARAM_1", refno)));
                            listMtTracer.Add(new JObject(new JProperty("@PARAM_2", valRow_id_splitter)));
                        }

                        else 
                        {
                            valRow_id_splitter = rowidSplitter;
                            sqlupdate = "UPDATE mt_tracer SET channel = @PARAM_0 WHERE(RefNo = @PARAM_1 and splitter_row_id = @PARAM_2 )";

                            listMtTracer.Add(new JObject(new JProperty("@PARAM_0", chn)));
                            listMtTracer.Add(new JObject(new JProperty("@PARAM_1", refno)));
                            listMtTracer.Add(new JObject(new JProperty("@PARAM_2", valRow_id_splitter)));
                        }

                        int result = oDB.queryUpdateWithParam(sqlupdate, listMtTracer);

                        if (result > 0)
                        {
                            finalRes = true;
                        }
                    }
                }

                return finalRes;
            }
            catch (Exception exCek)
            {
                return false;
            }
        }

        public bool updateStatus(String refNo, int status, int isKerjasama, String inout, 
            String Corespondent, String currency, String TAG20, String mt_103, 
            float amount, String accountnum, String accountname, String tanggal, 
            String splitter_row_id)
        {
            try
            {
                bool finalstatus = false;
                string valSplitter_row_id = "";
                string sqlUpdateStatus = "";
                var list= new List<JObject>();

                if (splitter_row_id.Substring(0, 1).Contains("O"))
                {
                    valSplitter_row_id = splitter_row_id.Substring(1);
                    
                    sqlUpdateStatus = "UPDATE mt_tracer ";
                    sqlUpdateStatus += " SET status = @PARAM_0, isKerjasama = @PARAM_1, InOutSwift = @PARAM_2,correspondent = @PARAM_3, ";
                    sqlUpdateStatus += " currency = @PARAM_4, TAG20 = @PARAM_5, mt_103 = @PARAM_6, amount = @PARAM_7, ";
                    sqlUpdateStatus += " account_num = @PARAM_8, account_name = @PARAM_9, value_date = @PARAM_10";
                    sqlUpdateStatus += " WHERE ( RefNo = @PARAM_11  and RowID = @PARAM_12 ) ";
                    
                    list.Add(new JObject(new JProperty("@PARAM_0", status)));
                    list.Add(new JObject(new JProperty("@PARAM_1", isKerjasama)));
                    list.Add(new JObject(new JProperty("@PARAM_2", inout)));
                    list.Add(new JObject(new JProperty("@PARAM_3", Corespondent)));

                    list.Add(new JObject(new JProperty("@PARAM_4", currency)));
                    list.Add(new JObject(new JProperty("@PARAM_5", TAG20)));
                    list.Add(new JObject(new JProperty("@PARAM_6", mt_103)));
                    list.Add(new JObject(new JProperty("@PARAM_7", amount)));

                    list.Add(new JObject(new JProperty("@PARAM_8", accountnum)));
                    list.Add(new JObject(new JProperty("@PARAM_9", accountname)));
                    list.Add(new JObject(new JProperty("@PARAM_10", tanggal)));

                    list.Add(new JObject(new JProperty("@PARAM_11", refNo)));
                    list.Add(new JObject(new JProperty("@PARAM_12", valSplitter_row_id)));

                }
                else
                {
                    valSplitter_row_id = splitter_row_id;
                    
                    sqlUpdateStatus = "UPDATE mt_tracer ";
                    sqlUpdateStatus += " SET status = @PARAM_0, isKerjasama = @PARAM_1, InOutSwift = @PARAM_2,correspondent = @PARAM_3, ";
                    sqlUpdateStatus += " currency = @PARAM_4, TAG20 = @PARAM_5, mt_103 = @PARAM_6, amount = @PARAM_7, ";
                    sqlUpdateStatus += " account_num = @PARAM_8, account_name = @PARAM_9, value_date = @PARAM_10 ";
                    sqlUpdateStatus += " WHERE ( RefNo = @PARAM_11  and splitter_row_id = @PARAM_12 ) ";

                    list.Add(new JObject(new JProperty("@PARAM_0", status)));
                    list.Add(new JObject(new JProperty("@PARAM_1", isKerjasama)));
                    list.Add(new JObject(new JProperty("@PARAM_2", inout)));
                    list.Add(new JObject(new JProperty("@PARAM_3", Corespondent)));

                    list.Add(new JObject(new JProperty("@PARAM_4", currency)));
                    list.Add(new JObject(new JProperty("@PARAM_5", TAG20)));
                    list.Add(new JObject(new JProperty("@PARAM_6", mt_103)));
                    list.Add(new JObject(new JProperty("@PARAM_7", amount)));

                    list.Add(new JObject(new JProperty("@PARAM_8", accountnum)));
                    list.Add(new JObject(new JProperty("@PARAM_9", accountname)));
                    list.Add(new JObject(new JProperty("@PARAM_10", tanggal)));

                    list.Add(new JObject(new JProperty("@PARAM_11", refNo)));
                    list.Add(new JObject(new JProperty("@PARAM_12", valSplitter_row_id)));
                }
                
                int res = oDB.queryUpdateWithParam(sqlUpdateStatus, list);

                if (res > 0)
                {
                    finalstatus = true;
                }

                return finalstatus;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public bool isInsertDataMappingAutoclose(Dictionary<string, string> pDataMtTracer,
            List<Data_MT> pDetaildata103,
            Dictionary<string, string> pWsAdapterResult)
        {
            bool hasil = false;

            try
            {
                string Joined_row_id = pDataMtTracer["Joined_row_id"];
                string Joined_status_id = pDataMtTracer["Joined_status_id"];

                //buat gabungan tampung
                string valTag20Ori = pDataMtTracer["valTag20Ori"];
                string valSplitter_row_id = pDataMtTracer["valSplitter_row_id"];
                string valMT_ORI = pDataMtTracer["valMT_ORI"];
                string valLoadDateOri = pDataMtTracer["valLoadDateOri"];
                string valTypeOri = pDataMtTracer["valTypeOri"];
                string valInOutCapture = pDataMtTracer["valInOutCapture"];

                int valStatus = Int32.Parse(pDataMtTracer["valStatus"]);
                string valMaker = pDataMtTracer["valMaker"];

                int valTargetDate = Int32.Parse(pDataMtTracer["valTargetDate"]);
                string StatusMtTracer = pDataMtTracer["StatusMtTracer"];

                string valRowIdMtTracer = pDataMtTracer["valRowIdMtTracer"];

                #region cek AutoClose
                String sqlCekAC = "";
                // 1 cari string array dulu
                string[] list_row_id = null;
                string[] list_status_id = null;
                var list = new List<JObject>();

                try
                {
                    list_row_id = Joined_row_id.Split(',');
                    list_status_id = Joined_status_id.Split(',');
                }
                catch (Exception exListID)
                { 
                    
                }
                
                // 2 ini yg row id di mulai dari Param_0
                sqlCekAC = "SELECT * FROM mt_tracer WHERE RowID IN (";
                if (list_row_id.Length > 0)
                {
                    for (int i = 0; i < list_row_id.Length; i++)
                    {
                        if (i > 0)
                        {
                            sqlCekAC += ", ";
                        }
                        sqlCekAC += " @PARAM_"+i.ToString();
                        list.Add(new JObject(new JProperty("@PARAM_"+i.ToString(), list_row_id[i].ToString() )));
                    }
                }

                sqlCekAC += ") AND STATUS IN (";

                // 3 ini unutk yg list sttaus di mulai dari stlh jumlah banyak di row id 
                if (list_status_id.Length > 0)
                {
                    for (int j = 0; j < list_status_id.Length; j++)
                    {
                        if (j > 0)
                        {
                            sqlCekAC += ", ";
                        }
                        sqlCekAC += " @PARAM_" + (j + list_row_id.Length).ToString();

                        list.Add(new JObject(new JProperty("@PARAM_" + (j + list_row_id.Length).ToString(), list_status_id[j].ToString())));
                    }
                }

                sqlCekAC += ")";

                DataTable dtCekAC = oDB.querySelectExtendedWithParam(sqlCekAC, list);
                #endregion

                //list cekcer rowid + status 
                string sqlAutoClose = "";
                var listAutoClose = new List<JObject>();
                
                if (dtCekAC.Rows.Count > 0)
                {
                    sqlAutoClose = " UPDATE mt_tracer set STATUS = 11, Maker = 'system', REMARK = 'close by system', last_update = NOW() where RowID IN (";

                    if (list_row_id.Length > 0)
                    {
                        for (int i = 0; i < list_row_id.Length; i++)
                        {
                            if (i > 0)
                            {
                                sqlAutoClose += ", ";
                            }
                            sqlAutoClose += " @PARAM_" + i.ToString();
                            listAutoClose.Add(new JObject(new JProperty("@PARAM_" + i.ToString(), list_row_id[i].ToString())));
                        }
                    }
                    sqlAutoClose += ") AND STATUS IN (";

                    if (list_status_id.Length > 0)
                    {
                        for (int j = 0; j < list_status_id.Length; j++)
                        {
                            if (j > 0)
                            {
                                sqlAutoClose += ", ";
                            }
                            sqlAutoClose += " @PARAM_" + (j + list_row_id.Length).ToString();

                            listAutoClose.Add(new JObject(new JProperty("@PARAM_" + (j + list_row_id.Length).ToString(), list_status_id[j].ToString() )));
                        }
                    }

                    sqlAutoClose += ") ";
                }


                #region insertDBMapping
                string dtFormat = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                #region cek Existing on mt_mapping
                string sqlCekMapping= "";
                sqlCekMapping = "SELECT * FROM mt_mapping where REF_TO_CAPTURE = @PARAM_0 and rowid_ref_to_capture = @PARAM_1 ";
                var listcekMapping = new List<JObject>();
                listcekMapping.Add(new JObject(new JProperty("@PARAM_0", valTag20Ori)));
                listcekMapping.Add(new JObject(new JProperty("@PARAM_1", valSplitter_row_id)));
                DataTable dtCekMapping = oDB.querySelectExtendedWithParam(sqlCekMapping, listcekMapping);
                #endregion

                string sqlInsertDBMapping_Delete = "";
                var listDBMapping_delete = new List<JObject>();
                if (dtCekMapping.Rows.Count > 0)
                {
                    sqlInsertDBMapping_Delete = " DELETE FROM mt_mapping where REF_TO_CAPTURE = @PARAM_0 and rowid_ref_to_capture = @PARAM_1 ";

                    listDBMapping_delete.Add(new JObject(new JProperty("@PARAM_0", valTag20Ori)));
                    listDBMapping_delete.Add(new JObject(new JProperty("@PARAM_1", valSplitter_row_id)));
                }

                string fixDateCapture = DateTime.ParseExact(valLoadDateOri, "M/d/yyyy h:mm:ss tt", CultureInfo.GetCultureInfo("en-US")).ToString("yyyy-MM-dd HH:mm:ss");
                string fixDatePrev = DateTime.ParseExact(pDetaildata103[0].getLoad_date(), "M/d/yyyy h:mm:ss tt", CultureInfo.GetCultureInfo("en-US")).ToString("yyyy-MM-dd HH:mm:ss");
                string fixDateOri = DateTime.ParseExact(pDetaildata103[0].getLoad_date(), "M/d/yyyy h:mm:ss tt", CultureInfo.GetCultureInfo("en-US")).ToString("yyyy-MM-dd HH:mm:ss");

                string sqlInsertDBMapping_Insert = "";
                sqlInsertDBMapping_Insert = " insert into mt_mapping(REF_TO_CAPTURE,PREVIOUS_REV,ORI_MT103_REF,MSG_DOWNLOADED, ";
                sqlInsertDBMapping_Insert += "ENTRY_DATE,DOWNLOADED_DATE,CORRESPONDENT,ISKERJASAMA, ";
                sqlInsertDBMapping_Insert += "INOROUT,currency,TAG20,mt_103, ";
                sqlInsertDBMapping_Insert += "amount, account_num,account_name, ";
                sqlInsertDBMapping_Insert += "tanggal,ROWID_REF_TO_CAPTURE,ROWID_PREVIOUS_REV,ROWID_ORI_MT103_REF, ";
                sqlInsertDBMapping_Insert += "mt_ref,mt_prev,dateCapture,datePrev, ";
                sqlInsertDBMapping_Insert += "dateOri,typeCapture,typePrev,typeOri, ";
                sqlInsertDBMapping_Insert += "InOutPrev,InOutOri,InOutCapture) VALUES ";

                sqlInsertDBMapping_Insert += "(@PARAM_0, @PARAM_1, @PARAM_2, @PARAM_3, ";
                sqlInsertDBMapping_Insert += "@PARAM_4, @PARAM_5, @PARAM_6, @PARAM_7, ";
                sqlInsertDBMapping_Insert += "@PARAM_8, @PARAM_9, @PARAM_10, @PARAM_11, ";
                sqlInsertDBMapping_Insert += "@PARAM_12, @PARAM_13, @PARAM_14, @PARAM_15, ";
                sqlInsertDBMapping_Insert += "@PARAM_16, @PARAM_17, @PARAM_18, @PARAM_19, ";
                sqlInsertDBMapping_Insert += "@PARAM_20, @PARAM_21, @PARAM_22, @PARAM_23, ";
                sqlInsertDBMapping_Insert += "@PARAM_24, @PARAM_25, @PARAM_26, @PARAM_27, ";
                sqlInsertDBMapping_Insert += "@PARAM_28, @PARAM_29) ";

                var listDBMapping_insert = new List<JObject>();
                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_0", valTag20Ori))); //1
                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_1", pDetaildata103[0].getReff()))); //2
                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_2", pDetaildata103[0].getReff()))); //3
                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_3", 0))); //4 

                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_4", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")))); //5
                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_5", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")))); //6
                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_6", pDetaildata103[0].getCorrespondent()))); //7
                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_7", pDetaildata103[0].getIsKerjasama()))); //8

                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_8", pDetaildata103[0].getInOut()))); //9
                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_9", pDetaildata103[0].getCurrency()))); //10
                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_10", pDetaildata103[0].getTag20()))); //11
                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_11", pDetaildata103[0].getMT_103()))); //12

                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_12", pDetaildata103[0].getAmount()))); //13
                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_13", pDetaildata103[0].getAccountNum()))); //14
                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_14", pDetaildata103[0].getAccountName()))); //15
                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_15", pDetaildata103[0].getTanggal()))); //16

                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_16", valSplitter_row_id))); //17
                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_17", pDetaildata103[0].getRowid_ori_mt103_ref()))); //18
                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_18", pDetaildata103[0].getRowid_ori_mt103_ref()))); //19
                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_19", valMT_ORI))); //20

                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_20", pDetaildata103[0].getMT_103()))); //21
                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_21", fixDateCapture))); //22
                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_22", fixDatePrev))); //23
                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_23", fixDateOri))); //24

                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_24", valTypeOri))); //25
                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_25", pDetaildata103[0].getType()))); //26
                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_26", pDetaildata103[0].getType()))); //27
                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_27", pDetaildata103[0].getInOut()))); //28

                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_28", pDetaildata103[0].getInOut()))); //29
                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_29" ,valInOutCapture ))); //30
                #endregion

                #region Query mt_tracer
                string reqTargetDate = DateTime.Now.AddDays(valTargetDate).ToString("yyyy-MM-dd HH:mm:ss");
                string lastupdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                string sqlMt_tracer = "";
                sqlMt_tracer = "UPDATE mt_tracer SET STATUS = @PARAM_0, "; // ini 22 atau 6
                sqlMt_tracer += " source_base = @PARAM_1, ";
                sqlMt_tracer += " replied_on = @PARAM_2, ";
                sqlMt_tracer += " last_update = @PARAM_3 ";
                sqlMt_tracer += " WHERE ( RefNo = @PARAM_4 AND ";

                if (valSplitter_row_id.Substring(0, 1).Contains("O"))
                {
                    valSplitter_row_id = valSplitter_row_id.Substring(1);
                    sqlMt_tracer += " RowID = @PARAM_5 ) ";
                }
                else 
                {
                    sqlMt_tracer += " splitter_row_id = @PARAM_5 ) ";
                }

                var listMt_tracer = new List<JObject>();
                listMt_tracer.Add(new JObject(new JProperty("@PARAM_0", StatusMtTracer))); //1
                listMt_tracer.Add(new JObject(new JProperty("@PARAM_1", pDetaildata103[0].getSource_base()))); //2
                listMt_tracer.Add(new JObject(new JProperty("@PARAM_2", reqTargetDate))); //3
                listMt_tracer.Add(new JObject(new JProperty("@PARAM_3", lastupdate))); //4
                listMt_tracer.Add(new JObject(new JProperty("@PARAM_4", valTag20Ori))); //5
                listMt_tracer.Add(new JObject(new JProperty("@PARAM_5", valSplitter_row_id))); //6

                #endregion

                string sqlMtTracerDetail = ""; //blank
                var listMtTracerDetail = new List<JObject>();//Blank

                if ( (pDetaildata103[0].getSource_base() == "IRK" || pDetaildata103[0].getSource_base() == "IRN") && pWsAdapterResult.Count > 0)
                {
                    sqlMtTracerDetail = "INSERT INTO mt_tracer_detail ";

                    sqlMtTracerDetail += "(ROWID_TRACER,TRX_REF,BENEF_ACCOUNT,BENEF_NAME,";
                    sqlMtTracerDetail += "VALUEDATE,SWIFTADAPTER_FLAG_DESC,BRIFAST_FLAG_DESC,BRINET_NAME,";
                    sqlMtTracerDetail += "BRINET_FULL_NAME,DESCRIPTION,PROCESS_ID,ROWID_ORIGINAL,";
                    sqlMtTracerDetail += "MTTYPE_ORIGINAL,TAG20_101,SENDER_BANK,RECEIVER_BANK,";
                    sqlMtTracerDetail += "TAG53,TAG54,CANCEL_MT_TYPE,CANCEL_FEE_AMOUNT,";
                    sqlMtTracerDetail += "CANCEL_FEE_CURRENCY,SENDER_BANK_ISDEPCOR,TAG56,";
                    sqlMtTracerDetail += "STATUS_SWIFT_ADAPTER,TRX_TYPE,";
                    sqlMtTracerDetail += "ORI_CURRENCY,CURRENCY,CONVERTED_CURRENCY,";
                    sqlMtTracerDetail += "ORI_AMOUNT,AMOUNT,CONVERTED_AMOUNT,";
                    sqlMtTracerDetail += "ORI_CANCEL_FEE_AMOUNT,ORI_CANCEL_FEE_CURRENCY,TRANSACTION_ACCOUNT,";
                    sqlMtTracerDetail += "INTERMEDIARY_ACCOUNT,FEE_ACCOUNT,NOSTRO_ACCOUNT,";
                    sqlMtTracerDetail += "REFUND_ACCOUNT,MT_SENDER,MT_RECEIVER,";
                    sqlMtTracerDetail += "MT_TAG21,MT_TAG56,MT_TAG57A,MT_TAG58A) ";

                    sqlMtTracerDetail += " VALUES ";

                    sqlMtTracerDetail += "(@PARAM_0, @PARAM_1, @PARAM_2, @PARAM_3, ";
                    sqlMtTracerDetail += "@PARAM_4, @PARAM_5, @PARAM_6, @PARAM_7, ";
                    sqlMtTracerDetail += "@PARAM_8, @PARAM_9, @PARAM_10, @PARAM_11, ";
                    sqlMtTracerDetail += "@PARAM_12, @PARAM_13, @PARAM_14, @PARAM_15, ";
                    sqlMtTracerDetail += "@PARAM_16, @PARAM_17, @PARAM_18, @PARAM_19, ";
                    sqlMtTracerDetail += "@PARAM_20, @PARAM_21, @PARAM_22,";
                    sqlMtTracerDetail += "@PARAM_23, @PARAM_24,";
                    sqlMtTracerDetail += "@PARAM_25, @PARAM_26, @PARAM_27,";
                    sqlMtTracerDetail += "@PARAM_28, @PARAM_29, @PARAM_30,";
                    sqlMtTracerDetail += "@PARAM_31, @PARAM_32, @PARAM_33,";
                    sqlMtTracerDetail += "@PARAM_34, @PARAM_35, @PARAM_36,";
                    sqlMtTracerDetail += "@PARAM_37, @PARAM_38, @PARAM_39,";
                    sqlMtTracerDetail += "@PARAM_40, @PARAM_41, @PARAM_42, @PARAM_43 )";

                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_0", valRowIdMtTracer)));
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_1", pWsAdapterResult["TAG20"])));
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_2", pWsAdapterResult["BenefAccount"])));
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_3", pWsAdapterResult["BenefName"].Replace("'",""))));

                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_4", pWsAdapterResult["TrxDate"])));
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_5", "")));
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_6", "")));
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_7", pWsAdapterResult["BrinetName"].Replace("'", ""))));

                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_8", pWsAdapterResult["BrinetFullName"].Replace("'", ""))));
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_9", pWsAdapterResult["Description"])));
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_10", pWsAdapterResult["ProcessId"])));
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_11", pWsAdapterResult["RowID"])));

                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_12", pWsAdapterResult["MTtype"])));
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_13", pWsAdapterResult["TAG20_101"])));
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_14", pWsAdapterResult["SenderBIC"])));
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_15", pWsAdapterResult["ReceiverBIC"])));

                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_16", pWsAdapterResult["Tag53"])));
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_17", pWsAdapterResult["Tag54"])));

                    //CANCEL_MT_TYPE
                    string cancelMtType = "";
                    if (pDetaildata103[0].getSource_base() == "IRK")
                    {
                        cancelMtType = pWsAdapterResult["CancelMTType"];
                    }
                    else if (pDetaildata103[0].getSource_base() == "IRN")
                    {
                        if (pWsAdapterResult["SenderBicIsDepcor"].Equals("1") && string.IsNullOrEmpty(pWsAdapterResult["Tag53"]) && string.IsNullOrEmpty(pWsAdapterResult["Tag54"]))
                        {
                            cancelMtType = "299";
                        }
                        else if (pWsAdapterResult["SenderBicIsDepcor"].Equals("1") && !pWsAdapterResult["Tag54"].Equals(pWsAdapterResult["SenderBIC"]))
                        {
                            cancelMtType = "202";
                        }
                        else if (pWsAdapterResult["SenderBicIsDepcor"].Equals("0"))
                        {
                            cancelMtType = "202";
                        }
                    }
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_18", cancelMtType)));

                    //CANCEL_FEE_AMOUNT
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_19", pWsAdapterResult["CancelFeeAmount"])));

                    //CANCEL_FEE_CURRENCY
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_20", pWsAdapterResult["CancelFeeCurrency"])));

                    //SENDER_BANK_ISDEPCOR
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_21", pWsAdapterResult["SenderBicIsDepcor"])));

                    //TAG56
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_22", pWsAdapterResult["Tag56"])));

                    //STATUS_SWIFT_ADAPTER
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_23", pWsAdapterResult["SwiftAdapterStatus"])));

                    //TRX_TYPE
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_24", pWsAdapterResult["TrxType"])));

                    //ORI_CURRENCY
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_25", pWsAdapterResult["TrxCurrency"])));

                    //CURRENCY
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_26", pWsAdapterResult["TrxCurrency"])));

                    //CONVERTED_CURRENCY
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_27", pWsAdapterResult["TrxCurrency"])));

                    //ORI_AMOUNT
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_28", pWsAdapterResult["TrxAmount"])));

                    //AMOUNT
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_29", pWsAdapterResult["TrxAmount"])));

                    //CONVERTED_AMOUNT
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_30", pWsAdapterResult["TrxAmount"])));

                    //ORI_CANCEL_FEE_AMOUNT
                    string cancel_fee_amount = "";
                    if (pDetaildata103[0].getSource_base() == "IRK")
                    {
                        cancel_fee_amount = pWsAdapterResult["CancelFeeAmount"];
                    }
                    else if (pDetaildata103[0].getSource_base() == "IRN")
                    {
                        cancel_fee_amount = getParameterValue("refund_fee_amount");
                    }
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_31", cancel_fee_amount)));

                    //ORI_CANCEL_FEE_CURRENCY
                    string cancel_fee_currency = "";
                    if (pDetaildata103[0].getSource_base() == "IRK")
                    {
                        cancel_fee_currency = pWsAdapterResult["CancelFeeCurrency"];
                    }
                    else if (pDetaildata103[0].getSource_base() == "IRN")
                    {
                        cancel_fee_currency = getParameterValue("refund_fee_currency");
                    }
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_32", cancel_fee_currency)));

                    //TRANSACTION_ACCOUNT
                    string transaction_account = "";
                    string[] arrayTrxAccountTypeNostro = { "0001", "0003", "0004", "0005", "0008" };
                    if (arrayTrxAccountTypeNostro.Contains(pWsAdapterResult["TrxType"]))
                    {
                        transaction_account = pWsAdapterResult["NostroAccount"];
                    }
                    else if (pWsAdapterResult["TrxType"].Equals("0002") || pWsAdapterResult["TrxType"].Equals("0006"))
                    {
                        transaction_account = "";
                    }
                    else if (pWsAdapterResult["TrxType"].Equals("0007"))
                    {
                        transaction_account = getParameterValue("ia_titipan_kliring");
                    }
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_33", transaction_account)));

                    //INTERMEDIARY_ACCOUNT
                    string intermediary_account = "";
                    if (pDetaildata103[0].getSource_base() == "IRK")
                    {
                        intermediary_account = getParameterValue("ia_titipan_incoming_" + pWsAdapterResult["TrxCurrency"]);
                    }
                    else if (pDetaildata103[0].getSource_base() == "IRN")
                    {
                        intermediary_account = getParameterValue("ia_titipan_outgoing_" + pWsAdapterResult["TrxCurrency"]);
                    }
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_34", intermediary_account)));

                    //FEE_ACCOUNT
                    string fee_account = "";
                    string[] arrayFeeAccountTypeIA = { "0001", "0003", "0004", "0005","0006","0007", "0008" };
                    if (arrayFeeAccountTypeIA.Contains(pWsAdapterResult["TrxType"]))
                    {
                        fee_account = getParameterValue("ia_pendapatan_incoming");
                    }
                    else if (pWsAdapterResult["TrxType"].Equals("0002"))
                    {
                        fee_account = getParameterValue("gl_pendapatan_incoming");
                    }
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_35", fee_account)));

                    //NOSTRO_ACCOUNT
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_36", pWsAdapterResult["NostroAccount"])));

                    //REFUND_ACCOUNT
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_37", pWsAdapterResult["NostroAccount"])));

                    //MT_SENDER
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_38", "BRINIDJA")));

                    //MT_RECEIVER
                    string mt_receiver = "";
                    if (pDetaildata103[0].getSource_base() == "IRK")
                    {
                        mt_receiver = pWsAdapterResult["SenderBIC"];
                    }
                    else if (pDetaildata103[0].getSource_base() == "IRN")
                    {
                        mt_receiver = pWsAdapterResult["Depcor"];
                    }
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_39", mt_receiver)));

                    //MT_TAG21
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_40", valTag20Ori)));

                    //MT_TAG56
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_41", pWsAdapterResult["Tag56"])));

                    //MT_TAG57A
                    string tag57a = "";
                    if (pDetaildata103[0].getSource_base() == "IRK")
                    {
                        if (!string.IsNullOrEmpty(pWsAdapterResult["Tag53"]))
                        {
                            tag57a = pWsAdapterResult["Tag53"];
                        }
                    }
                    else if (pDetaildata103[0].getSource_base() == "IRN")
                    {
                        if (pWsAdapterResult["Tag53"] != mt_receiver)
                        {
                            tag57a = pWsAdapterResult["Tag53"];
                        }
                        else if (pWsAdapterResult["Tag53"] == mt_receiver)
                        {
                            tag57a = "";
                        }
                        else
                        {
                            tag57a = "";
                        }
                    }
                    else 
                    {
                        tag57a = "";
                    }
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_42", tag57a)));

                    //MT_TAG58A
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_43", pWsAdapterResult["SenderBIC"])));                    
                }

                //Combine all Param
                // set maximum first
                String[] listSql = new String[5];
                List<JObject>[] listParam = new List<JObject>[5];

                //autoclose & mtTracerDetail kosong 
                if ((string.IsNullOrEmpty(sqlAutoClose) && sqlAutoClose.Length == 0) && (string.IsNullOrEmpty(sqlMtTracerDetail) && sqlMtTracerDetail.Length == 0))
                {
                    if ((string.IsNullOrEmpty(sqlInsertDBMapping_Delete) && sqlInsertDBMapping_Delete.Length == 0))
                    {
                        listSql = new String[2];
                        listParam = new List<JObject>[2];

                        listSql[0] = sqlInsertDBMapping_Insert;
                        listParam[0] = listDBMapping_insert;

                        listSql[1] = sqlMt_tracer;
                        listParam[1] = listMt_tracer;
                    }
                    else if ((!string.IsNullOrEmpty(sqlInsertDBMapping_Delete) && sqlInsertDBMapping_Delete.Length > 0))
                    {
                        listSql = new String[3];
                        listParam = new List<JObject>[3];

                        listSql[0] = sqlInsertDBMapping_Delete;
                        listParam[0] = listDBMapping_delete;

                        listSql[1] = sqlInsertDBMapping_Insert;
                        listParam[1] = listDBMapping_insert;

                        listSql[2] = sqlMt_tracer;
                        listParam[2] = listMt_tracer;
                    }
                    else { }
                }
                //autoclose isi & tracer detail kosong
                else if ((!string.IsNullOrEmpty(sqlAutoClose) && sqlAutoClose.Length > 0) && (string.IsNullOrEmpty(sqlMtTracerDetail) && sqlMtTracerDetail.Length == 0))
                {
                    if ((string.IsNullOrEmpty(sqlInsertDBMapping_Delete) && sqlInsertDBMapping_Delete.Length == 0))
                    {
                        listSql = new String[3];
                        listParam = new List<JObject>[3];

                        listSql[0] = sqlAutoClose;
                        listParam[0] = listAutoClose;

                        listSql[1] = sqlInsertDBMapping_Insert;
                        listParam[1] = listDBMapping_insert;

                        listSql[2] = sqlMt_tracer;
                        listParam[2] = listMt_tracer;
                    }
                    else if ((!string.IsNullOrEmpty(sqlInsertDBMapping_Delete) && sqlInsertDBMapping_Delete.Length > 0))
                    {
                        listSql = new String[4];
                        listParam = new List<JObject>[4];

                        listSql[0] = sqlAutoClose;
                        listParam[0] = listAutoClose;

                        listSql[1] = sqlInsertDBMapping_Delete;
                        listParam[1] = listDBMapping_delete;

                        listSql[2] = sqlInsertDBMapping_Insert;
                        listParam[2] = listDBMapping_insert;

                        listSql[3] = sqlMt_tracer;
                        listParam[3] = listMt_tracer;
                    }
                    else { }
                }
                //autoclose kosong & tracer detail isi
                else if ((string.IsNullOrEmpty(sqlAutoClose) && sqlAutoClose.Length == 0) && (!string.IsNullOrEmpty(sqlMtTracerDetail) && sqlMtTracerDetail.Length > 0))
                {
                    if ((string.IsNullOrEmpty(sqlInsertDBMapping_Delete) && sqlInsertDBMapping_Delete.Length == 0))
                    {
                        listSql = new String[3];
                        listParam = new List<JObject>[3];

                        listSql[0] = sqlInsertDBMapping_Insert;
                        listParam[0] = listDBMapping_insert;

                        listSql[1] = sqlMt_tracer;
                        listParam[1] = listMt_tracer;

                        listSql[2] = sqlMtTracerDetail;
                        listParam[2] = listMtTracerDetail;
                    }
                    else if ((!string.IsNullOrEmpty(sqlInsertDBMapping_Delete) && sqlInsertDBMapping_Delete.Length > 0))
                    {
                        listSql = new String[4];
                        listParam = new List<JObject>[4];

                        listSql[0] = sqlInsertDBMapping_Delete;
                        listParam[0] = listDBMapping_delete;

                        listSql[1] = sqlInsertDBMapping_Insert;
                        listParam[1] = listDBMapping_insert;

                        listSql[2] = sqlMt_tracer;
                        listParam[2] = listMt_tracer;

                        listSql[3] = sqlMtTracerDetail;
                        listParam[3] = listMtTracerDetail;
                    }
                    else { }
                }
                else if ((!string.IsNullOrEmpty(sqlAutoClose) || sqlAutoClose.Length > 0) && (!string.IsNullOrEmpty(sqlMtTracerDetail) || sqlMtTracerDetail.Length > 0))
                {
                    if ((string.IsNullOrEmpty(sqlInsertDBMapping_Delete) && sqlInsertDBMapping_Delete.Length == 0))
                    {
                        listSql = new String[4];
                        listParam = new List<JObject>[4];

                        listSql[0] = sqlAutoClose;
                        listParam[0] = listAutoClose;

                        listSql[1] = sqlInsertDBMapping_Insert;
                        listParam[1] = listDBMapping_insert;

                        listSql[2] = sqlMt_tracer;
                        listParam[2] = listMt_tracer;

                        listSql[3] = sqlMtTracerDetail;
                        listParam[3] = listMtTracerDetail;
                    }
                    else if ((!string.IsNullOrEmpty(sqlInsertDBMapping_Delete) && sqlInsertDBMapping_Delete.Length > 0))
                    {
                        listSql[0] = sqlAutoClose;
                        listParam[0] = listAutoClose;

                        listSql[1] = sqlInsertDBMapping_Delete;
                        listParam[1] = listDBMapping_delete;

                        listSql[2] = sqlInsertDBMapping_Insert;
                        listParam[2] = listDBMapping_insert;

                        listSql[3] = sqlMt_tracer;
                        listParam[3] = listMt_tracer;

                        listSql[4] = sqlMtTracerDetail;
                        listParam[4] = listMtTracerDetail;
                    }
                    else 
                    { 
                    }
                }
                else
                {
                    
                }

                //ini list nya dynamic || min 2 max 5
                int res = oDB.queryUpdateWithParam2(listSql, listParam);

                if (res > 0)
                {
                    hasil = true;
                }

            }
            catch (Exception ex)
            {

                return hasil;
            }
            return hasil;
        }

        public bool isInsertDataMappingAutocloseNested(Dictionary<string, string> pDataMtTracer,
            List<Data_MT> pDetaildata103,
            Dictionary<string, string> pWsAdapterResult,
            List<Data_MT> pDetaildataNON103)
        {
            bool hasil = false;

            try
            {
                string Joined_row_id = pDataMtTracer["Joined_row_id"];
                string Joined_status_id = pDataMtTracer["Joined_status_id"];

                //buat gabungan tampung
                string valTag20Ori = pDataMtTracer["valTag20Ori"];
                string valSplitter_row_id = pDataMtTracer["valSplitter_row_id"];
                string valMT_ORI = pDataMtTracer["valMT_ORI"];
                string valLoadDateOri = pDataMtTracer["valLoadDateOri"];
                string valTypeOri = pDataMtTracer["valTypeOri"];
                string valInOutCapture = pDataMtTracer["valInOutCapture"];

                int valStatus = Int32.Parse(pDataMtTracer["valStatus"]);
                string valMaker = pDataMtTracer["valMaker"];

                int valTargetDate = Int32.Parse(pDataMtTracer["valTargetDate"]);
                string StatusMtTracer = pDataMtTracer["StatusMtTracer"];

                string valRowIdMtTracer = pDataMtTracer["valRowIdMtTracer"];

                #region cek AutoClose
                String sqlCekAC = "";
                // 1 cari string array dulu
                string[] list_row_id = null;
                string[] list_status_id = null;
                var list = new List<JObject>();

                try
                {
                    list_row_id = Joined_row_id.Split(',');
                    list_status_id = Joined_status_id.Split(',');
                }
                catch (Exception exListID)
                {

                }

                // 2 ini yg row id di mulai dari Param_0
                sqlCekAC = "SELECT * FROM mt_tracer WHERE RowID IN (";
                if (list_row_id.Length > 0)
                {
                    for (int i = 0; i < list_row_id.Length; i++)
                    {
                        if (i > 0)
                        {
                            sqlCekAC += ", ";
                        }
                        sqlCekAC += " @PARAM_" + i.ToString();
                        list.Add(new JObject(new JProperty("@PARAM_" + i.ToString(), list_row_id[i].ToString())));
                    }
                }

                sqlCekAC += ") AND STATUS IN (";

                // 3 ini unutk yg list sttaus di mulai dari stlh jumlah banyak di row id 
                if (list_status_id.Length > 0)
                {
                    for (int j = 0; j < list_status_id.Length; j++)
                    {
                        if (j > 0)
                        {
                            sqlCekAC += ", ";
                        }
                        sqlCekAC += " @PARAM_" + (j + list_row_id.Length).ToString();

                        list.Add(new JObject(new JProperty("@PARAM_" + (j + list_row_id.Length).ToString(), list_status_id[j].ToString())));
                    }
                }

                sqlCekAC += ")";

                

                DataTable dtCekAC = oDB.querySelectExtendedWithParam(sqlCekAC, list);
                #endregion


                //list cekcer rowid + status 
                string sqlAutoClose = "";
                var listAutoClose = new List<JObject>();
                

                if (dtCekAC.Rows.Count > 0)
                {
                    //sqlAutoClose = " UPDATE mt_tracer set status = 11 where RowID IN (";
                    sqlAutoClose = " UPDATE mt_tracer set STATUS = 11, Maker = 'system', REMARK = 'close by system', last_update = NOW() where RowID IN (";

                    if (list_row_id.Length > 0)
                    {
                        for (int i = 0; i < list_row_id.Length; i++)
                        {
                            if (i > 0)
                            {
                                sqlAutoClose += ", ";
                            }
                            sqlAutoClose += " @PARAM_" + i.ToString();
                            listAutoClose.Add(new JObject(new JProperty("@PARAM_" + i.ToString(), list_row_id[i].ToString())));
                        }
                    }
                    sqlAutoClose += ") AND STATUS IN (";

                    if (list_status_id.Length > 0)
                    {
                        for (int j = 0; j < list_status_id.Length; j++)
                        {
                            if (j > 0)
                            {
                                sqlAutoClose += ", ";
                            }
                            sqlAutoClose += " @PARAM_" + (j + list_row_id.Length).ToString();

                            listAutoClose.Add(new JObject(new JProperty("@PARAM_" + (j + list_row_id.Length).ToString(), list_status_id[j].ToString())));
                        }
                    }

                    sqlAutoClose += ") ";
                }

                #region insertDBMapping
                string dtFormat = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                #region cek Existing on mt_mapping
                string sqlCekMapping = "";
                sqlCekMapping = "SELECT * FROM mt_mapping where REF_TO_CAPTURE = @PARAM_0 and rowid_ref_to_capture = @PARAM_1 ";
                var listcekMapping = new List<JObject>();
                listcekMapping.Add(new JObject(new JProperty("@PARAM_0", valTag20Ori)));
                listcekMapping.Add(new JObject(new JProperty("@PARAM_1", valSplitter_row_id)));
                DataTable dtCekMapping = oDB.querySelectExtendedWithParam(sqlCekMapping, listcekMapping);
                #endregion

                string sqlInsertDBMapping_Delete = "";
                var listDBMapping_delete = new List<JObject>();
                if (dtCekMapping.Rows.Count > 0)
                {
                    sqlInsertDBMapping_Delete = " DELETE FROM mt_mapping where REF_TO_CAPTURE = @PARAM_0 and rowid_ref_to_capture = @PARAM_1 ";

                    listDBMapping_delete.Add(new JObject(new JProperty("@PARAM_0", valTag20Ori)));
                    listDBMapping_delete.Add(new JObject(new JProperty("@PARAM_1", valSplitter_row_id)));
                }

                string fixDateCapture = DateTime.ParseExact(valLoadDateOri, "M/d/yyyy h:mm:ss tt", CultureInfo.GetCultureInfo("en-US")).ToString("yyyy-MM-dd HH:mm:ss");

                //string fixDatePrev = DateTime.ParseExact(pDetaildata103[0].getLoad_date(), "M/d/yyyy h:mm:ss tt", CultureInfo.GetCultureInfo("en-US")).ToString("yyyy-MM-dd HH:mm:ss");
                string fixDatePrev = DateTime.ParseExact(pDetaildataNON103[0].getLoad_date(), "M/d/yyyy h:mm:ss tt", CultureInfo.GetCultureInfo("en-US")).ToString("yyyy-MM-dd HH:mm:ss");

                string fixDateOri = DateTime.ParseExact(pDetaildata103[0].getLoad_date(), "M/d/yyyy h:mm:ss tt", CultureInfo.GetCultureInfo("en-US")).ToString("yyyy-MM-dd HH:mm:ss");
                

                string sqlInsertDBMapping_Insert = "";
                sqlInsertDBMapping_Insert = " insert into mt_mapping(REF_TO_CAPTURE,PREVIOUS_REV,ORI_MT103_REF,MSG_DOWNLOADED, ";
                sqlInsertDBMapping_Insert += "ENTRY_DATE,DOWNLOADED_DATE,CORRESPONDENT,ISKERJASAMA, ";
                sqlInsertDBMapping_Insert += "INOROUT,currency,TAG20,mt_103, ";
                sqlInsertDBMapping_Insert += "amount, account_num,account_name, ";
                sqlInsertDBMapping_Insert += "tanggal,ROWID_REF_TO_CAPTURE,ROWID_PREVIOUS_REV,ROWID_ORI_MT103_REF, ";
                sqlInsertDBMapping_Insert += "mt_ref,mt_prev,dateCapture,datePrev, ";
                sqlInsertDBMapping_Insert += "dateOri,typeCapture,typePrev,typeOri, ";
                sqlInsertDBMapping_Insert += "InOutPrev,InOutOri,InOutCapture) VALUES ";

                sqlInsertDBMapping_Insert += "(@PARAM_0, @PARAM_1, @PARAM_2, @PARAM_3, ";
                sqlInsertDBMapping_Insert += "@PARAM_4, @PARAM_5, @PARAM_6, @PARAM_7, ";
                sqlInsertDBMapping_Insert += "@PARAM_8, @PARAM_9, @PARAM_10, @PARAM_11, ";
                sqlInsertDBMapping_Insert += "@PARAM_12, @PARAM_13, @PARAM_14, @PARAM_15, ";
                sqlInsertDBMapping_Insert += "@PARAM_16, @PARAM_17, @PARAM_18, @PARAM_19, ";
                sqlInsertDBMapping_Insert += "@PARAM_20, @PARAM_21, @PARAM_22, @PARAM_23, ";
                sqlInsertDBMapping_Insert += "@PARAM_24, @PARAM_25, @PARAM_26, @PARAM_27, ";
                sqlInsertDBMapping_Insert += "@PARAM_28, @PARAM_29) ";

                var listDBMapping_insert = new List<JObject>();
                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_0", valTag20Ori))); //1
                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_1", pDetaildataNON103[0].getReff() ))); //2
                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_2", pDetaildata103[0].getReff()))); //3
                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_3", 0))); //4 

                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_4", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")))); //5
                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_5", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")))); //6
                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_6", pDetaildata103[0].getCorrespondent()))); //7
                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_7", pDetaildata103[0].getIsKerjasama()))); //8

                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_8", pDetaildata103[0].getInOut()))); //9
                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_9", pDetaildata103[0].getCurrency()))); //10
                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_10", pDetaildata103[0].getTag20()))); //11
                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_11", pDetaildata103[0].getMT_103()))); //12

                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_12", pDetaildata103[0].getAmount()))); //13
                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_13", pDetaildata103[0].getAccountNum()))); //14
                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_14", pDetaildata103[0].getAccountName()))); //15
                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_15", pDetaildata103[0].getTanggal()))); //16

                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_16", valSplitter_row_id))); //17
                //listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_17", pDetaildata103[0].getRowid_ori_mt103_ref()))); //18
                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_17", pDetaildataNON103[0].getRowid_ori_mt103_ref() ))); //18
                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_18", pDetaildata103[0].getRowid_ori_mt103_ref()))); //19
                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_19", valMT_ORI))); //20

                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_20", pDetaildata103[0].getMT_103()))); //21
                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_21", fixDateCapture))); //22 - mt_tracer
                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_22", fixDatePrev))); //23 - NON103
                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_23", fixDateOri))); //24 - 103

                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_24", valTypeOri))); //25
                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_25", pDetaildataNON103[0].getType() ))); //26 - NON
                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_26", pDetaildata103[0].getType()))); //27
                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_27", pDetaildataNON103[0].getInOut() ))); //28 - NON

                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_28", pDetaildata103[0].getInOut()))); //29
                listDBMapping_insert.Add(new JObject(new JProperty("@PARAM_29", valInOutCapture))); //30
                #endregion

                #region Query mt_tracer
                string reqTargetDate = DateTime.Now.AddDays(valTargetDate).ToString("yyyy-MM-dd HH:mm:ss");
                string lastupdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                string sqlMt_tracer = "";
                sqlMt_tracer = "UPDATE mt_tracer SET STATUS = @PARAM_0, "; // ini 22 atau 6
                sqlMt_tracer += " source_base = @PARAM_1, ";
                sqlMt_tracer += " replied_on = @PARAM_2, ";
                sqlMt_tracer += " last_update = @PARAM_3 ";
                sqlMt_tracer += " WHERE ( RefNo = @PARAM_4 AND ";

                if (valSplitter_row_id.Substring(0, 1).Contains("O"))
                {
                    valSplitter_row_id = valSplitter_row_id.Substring(1);
                    sqlMt_tracer += " RowID = @PARAM_5 ) ";
                }
                else
                {
                    sqlMt_tracer += " splitter_row_id = @PARAM_5 ) ";
                }

                var listMt_tracer = new List<JObject>();
                listMt_tracer.Add(new JObject(new JProperty("@PARAM_0", StatusMtTracer))); //1
                listMt_tracer.Add(new JObject(new JProperty("@PARAM_1", pDetaildata103[0].getSource_base()))); //2
                listMt_tracer.Add(new JObject(new JProperty("@PARAM_2", reqTargetDate))); //3
                listMt_tracer.Add(new JObject(new JProperty("@PARAM_3", lastupdate))); //4
                listMt_tracer.Add(new JObject(new JProperty("@PARAM_4", valTag20Ori))); //4
                listMt_tracer.Add(new JObject(new JProperty("@PARAM_5", valSplitter_row_id))); //5

                #endregion

                string sqlMtTracerDetail = ""; //blank
                var listMtTracerDetail = new List<JObject>();//Blank

                if ((pDetaildata103[0].getSource_base() == "IRK" || pDetaildata103[0].getSource_base() == "IRN") && pWsAdapterResult.Count > 0)
                {
                    sqlMtTracerDetail = "INSERT INTO mt_tracer_detail";

                    sqlMtTracerDetail += "(ROWID_TRACER,TRX_REF,BENEF_ACCOUNT,BENEF_NAME,";
                    sqlMtTracerDetail += "VALUEDATE,SWIFTADAPTER_FLAG_DESC,BRIFAST_FLAG_DESC,BRINET_NAME,";
                    sqlMtTracerDetail += "BRINET_FULL_NAME,DESCRIPTION,PROCESS_ID,ROWID_ORIGINAL,";
                    sqlMtTracerDetail += "MTTYPE_ORIGINAL,TAG20_101,SENDER_BANK,RECEIVER_BANK,";
                    sqlMtTracerDetail += "TAG53,TAG54,CANCEL_MT_TYPE,CANCEL_FEE_AMOUNT,";
                    sqlMtTracerDetail += "CANCEL_FEE_CURRENCY,SENDER_BANK_ISDEPCOR,TAG56,";
                    sqlMtTracerDetail += "STATUS_SWIFT_ADAPTER,TRX_TYPE,";
                    sqlMtTracerDetail += "ORI_CURRENCY,CURRENCY,CONVERTED_CURRENCY,";
                    sqlMtTracerDetail += "ORI_AMOUNT,AMOUNT,CONVERTED_AMOUNT,";
                    sqlMtTracerDetail += "ORI_CANCEL_FEE_AMOUNT,ORI_CANCEL_FEE_CURRENCY,TRANSACTION_ACCOUNT,";
                    sqlMtTracerDetail += "INTERMEDIARY_ACCOUNT,FEE_ACCOUNT,NOSTRO_ACCOUNT,";
                    sqlMtTracerDetail += "REFUND_ACCOUNT,MT_SENDER,MT_RECEIVER,";
                    sqlMtTracerDetail += "MT_TAG21,MT_TAG56,MT_TAG57A,MT_TAG58A) ";

                    sqlMtTracerDetail += " VALUES ";

                    sqlMtTracerDetail += "(@PARAM_0, @PARAM_1, @PARAM_2, @PARAM_3, ";
                    sqlMtTracerDetail += "@PARAM_4, @PARAM_5, @PARAM_6, @PARAM_7, ";
                    sqlMtTracerDetail += "@PARAM_8, @PARAM_9, @PARAM_10, @PARAM_11, ";
                    sqlMtTracerDetail += "@PARAM_12, @PARAM_13, @PARAM_14, @PARAM_15, ";
                    sqlMtTracerDetail += "@PARAM_16, @PARAM_17, @PARAM_18, @PARAM_19, ";
                    sqlMtTracerDetail += "@PARAM_20, @PARAM_21, @PARAM_22,";
                    sqlMtTracerDetail += "@PARAM_23, @PARAM_24,";
                    sqlMtTracerDetail += "@PARAM_25, @PARAM_26, @PARAM_27,";
                    sqlMtTracerDetail += "@PARAM_28, @PARAM_29, @PARAM_30,";
                    sqlMtTracerDetail += "@PARAM_31, @PARAM_32, @PARAM_33,";
                    sqlMtTracerDetail += "@PARAM_34, @PARAM_35, @PARAM_36,";
                    sqlMtTracerDetail += "@PARAM_37, @PARAM_38, @PARAM_39,";
                    sqlMtTracerDetail += "@PARAM_40, @PARAM_41, @PARAM_42, @PARAM_43 )";

                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_0", valRowIdMtTracer)));
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_1", pWsAdapterResult["TAG20"])));
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_2", pWsAdapterResult["BenefAccount"])));
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_3", pWsAdapterResult["BenefName"].Replace("'", ""))));

                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_4", pWsAdapterResult["TrxDate"])));
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_5", "")));
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_6", "")));
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_7", pWsAdapterResult["BrinetName"].Replace("'", ""))));

                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_8", pWsAdapterResult["BrinetFullName"].Replace("'", ""))));
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_9", pWsAdapterResult["Description"])));
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_10", pWsAdapterResult["ProcessId"])));
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_11", pWsAdapterResult["RowID"])));

                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_12", pWsAdapterResult["MTtype"])));
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_13", pWsAdapterResult["TAG20_101"])));
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_14", pWsAdapterResult["SenderBIC"])));
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_15", pWsAdapterResult["ReceiverBIC"])));

                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_16", pWsAdapterResult["Tag53"])));
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_17", pWsAdapterResult["Tag54"])));

                    //CANCEL_MT_TYPE
                    string cancelMtType = "";
                    if (pDetaildata103[0].getSource_base() == "IRK")
                    {
                        cancelMtType = pWsAdapterResult["CancelMTType"];
                    }
                    else if (pDetaildata103[0].getSource_base() == "IRN")
                    {
                        if (pWsAdapterResult["SenderBicIsDepcor"].Equals("1") && string.IsNullOrEmpty(pWsAdapterResult["Tag53"]) && string.IsNullOrEmpty(pWsAdapterResult["Tag54"]))
                        {
                            cancelMtType = "299";
                        }
                        else if (pWsAdapterResult["SenderBicIsDepcor"].Equals("1") && !pWsAdapterResult["Tag54"].Equals(pWsAdapterResult["SenderBIC"]))
                        {
                            cancelMtType = "202";
                        }
                        else if (pWsAdapterResult["SenderBicIsDepcor"].Equals("0"))
                        {
                            cancelMtType = "202";
                        }
                    }
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_18", cancelMtType)));

                    //CANCEL_FEE_AMOUNT
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_19", pWsAdapterResult["CancelFeeAmount"])));

                    //CANCEL_FEE_CURRENCY
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_20", pWsAdapterResult["CancelFeeCurrency"])));

                    //SENDER_BANK_ISDEPCOR
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_21", pWsAdapterResult["SenderBicIsDepcor"])));

                    //TAG56
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_22", pWsAdapterResult["Tag56"])));

                    //STATUS_SWIFT_ADAPTER
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_23", pWsAdapterResult["SwiftAdapterStatus"])));

                    //TRX_TYPE
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_24", pWsAdapterResult["TrxType"])));

                    //ORI_CURRENCY
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_25", pWsAdapterResult["TrxCurrency"])));

                    //CURRENCY
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_26", pWsAdapterResult["TrxCurrency"])));

                    //CONVERTED_CURRENCY
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_27", pWsAdapterResult["TrxCurrency"])));

                    //ORI_AMOUNT
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_28", pWsAdapterResult["TrxAmount"])));

                    //AMOUNT
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_29", pWsAdapterResult["TrxAmount"])));

                    //CONVERTED_AMOUNT
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_30", pWsAdapterResult["TrxAmount"])));

                    //ORI_CANCEL_FEE_AMOUNT
                    //listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_31", pWsAdapterResult["CancelFeeAmount"])));

                    //ORI_CANCEL_FEE_CURRENCY
                    //listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_32", pWsAdapterResult["CancelFeeCurrency"])));

                    //ORI_CANCEL_FEE_AMOUNT
                    string cancel_fee_amount = "";
                    if (pDetaildata103[0].getSource_base() == "IRK")
                    {
                        cancel_fee_amount = pWsAdapterResult["CancelFeeAmount"];
                    }
                    else if (pDetaildata103[0].getSource_base() == "IRN")
                    {
                        cancel_fee_amount = getParameterValue("refund_fee_amount");
                    }
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_31", cancel_fee_amount)));

                    //ORI_CANCEL_FEE_CURRENCY
                    string cancel_fee_currency = "";
                    if (pDetaildata103[0].getSource_base() == "IRK")
                    {
                        cancel_fee_currency = pWsAdapterResult["CancelFeeCurrency"];
                    }
                    else if (pDetaildata103[0].getSource_base() == "IRN")
                    {
                        cancel_fee_currency = getParameterValue("refund_fee_currency");
                    }
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_32", cancel_fee_currency)));

                    //TRANSACTION_ACCOUNT
                    string transaction_account = "";
                    string[] arrayTrxAccountTypeNostro = { "0001", "0003", "0004", "0005", "0008" };
                    if (arrayTrxAccountTypeNostro.Contains(pWsAdapterResult["TrxType"]))
                    {
                        transaction_account = pWsAdapterResult["NostroAccount"];
                    }
                    else if (pWsAdapterResult["TrxType"].Equals("0002") || pWsAdapterResult["TrxType"].Equals("0006"))
                    {
                        transaction_account = "";
                    }
                    else if (pWsAdapterResult["TrxType"].Equals("0007"))
                    {
                        transaction_account = getParameterValue("ia_titipan_kliring");
                    }
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_33", transaction_account)));

                    //INTERMEDIARY_ACCOUNT
                    string intermediary_account = "";
                    if (pDetaildata103[0].getSource_base() == "IRK")
                    {
                        intermediary_account = getParameterValue("ia_titipan_incoming_" + pWsAdapterResult["TrxCurrency"]);
                    }
                    else if (pDetaildata103[0].getSource_base() == "IRN")
                    {
                        intermediary_account = getParameterValue("ia_titipan_outgoing_" + pWsAdapterResult["TrxCurrency"]);
                    }
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_34", intermediary_account)));

                    //FEE_ACCOUNT
                    string fee_account = "";
                    string[] arrayFeeAccountTypeIA = { "0001", "0003", "0004", "0005","0006","0007", "0008" };
                    if (arrayFeeAccountTypeIA.Contains(pWsAdapterResult["TrxType"]))
                    {
                        fee_account = getParameterValue("ia_pendapatan_incoming");
                    }
                    else if (pWsAdapterResult["TrxType"].Equals("0002"))
                    {
                        fee_account = getParameterValue("gl_pendapatan_incoming");
                    }
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_35", fee_account)));

                    //NOSTRO_ACCOUNT
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_36", pWsAdapterResult["NostroAccount"])));

                    //REFUND_ACCOUNT
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_37", pWsAdapterResult["NostroAccount"])));

                    //MT_SENDER
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_38", "BRINIDJA")));

                    //MT_RECEIVER
                    string mt_receiver = "";
                    if (pDetaildata103[0].getSource_base() == "IRK")
                    {
                        mt_receiver = pWsAdapterResult["SenderBIC"];
                    }
                    else if (pDetaildata103[0].getSource_base() == "IRN")
                    {
                        mt_receiver = pWsAdapterResult["Depcor"];
                    }
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_39", mt_receiver)));

                    //MT_TAG21
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_40", valTag20Ori)));

                    //MT_TAG56
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_41", pWsAdapterResult["Tag56"])));

                    //MT_TAG57A
                    string tag57a = "";
                    if (pDetaildata103[0].getSource_base() == "IRK")
                    {
                        if (!string.IsNullOrEmpty(pWsAdapterResult["Tag53"]))
                        {
                            tag57a = pWsAdapterResult["Tag53"];
                        }
                    }
                    else if (pDetaildata103[0].getSource_base() == "IRN")
                    {
                        if (pWsAdapterResult["Tag53"] != mt_receiver)
                        {
                            tag57a = pWsAdapterResult["Tag53"];
                        }
                        else if (pWsAdapterResult["Tag53"] == mt_receiver)
                        {
                            tag57a = "";
                        }
                        else
                        {
                            tag57a = "";
                        }
                    }
                    else
                    {
                        tag57a = "";
                    }
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_42", tag57a)));

                    //MT_TAG58A
                    listMtTracerDetail.Add(new JObject(new JProperty("@PARAM_43", pWsAdapterResult["SenderBIC"])));
                }

                //Combine all Param
                // set maximum first
                String[] listSql = new String[5];
                List<JObject>[] listParam = new List<JObject>[5];

                //autoclose & mtTracerDetail kosong 
                if ((string.IsNullOrEmpty(sqlAutoClose) && sqlAutoClose.Length == 0) && (string.IsNullOrEmpty(sqlMtTracerDetail) && sqlMtTracerDetail.Length == 0))
                {
                    if ((string.IsNullOrEmpty(sqlInsertDBMapping_Delete) && sqlInsertDBMapping_Delete.Length == 0))
                    {
                        listSql = new String[2];
                        listParam = new List<JObject>[2];

                        listSql[0] = sqlInsertDBMapping_Insert;
                        listParam[0] = listDBMapping_insert;

                        listSql[1] = sqlMt_tracer;
                        listParam[1] = listMt_tracer;
                    }
                    else if ((!string.IsNullOrEmpty(sqlInsertDBMapping_Delete) && sqlInsertDBMapping_Delete.Length > 0))
                    {
                        listSql = new String[3];
                        listParam = new List<JObject>[3];

                        listSql[0] = sqlInsertDBMapping_Delete;
                        listParam[0] = listDBMapping_delete;

                        listSql[1] = sqlInsertDBMapping_Insert;
                        listParam[1] = listDBMapping_insert;

                        listSql[2] = sqlMt_tracer;
                        listParam[2] = listMt_tracer;
                    }
                    else { }
                }
                //autoclose isi & tracer detail kosong
                else if ((!string.IsNullOrEmpty(sqlAutoClose) && sqlAutoClose.Length > 0) && (string.IsNullOrEmpty(sqlMtTracerDetail) && sqlMtTracerDetail.Length == 0))
                {
                    if ((string.IsNullOrEmpty(sqlInsertDBMapping_Delete) && sqlInsertDBMapping_Delete.Length == 0))
                    {
                        listSql = new String[3];
                        listParam = new List<JObject>[3];

                        listSql[0] = sqlAutoClose;
                        listParam[0] = listAutoClose;

                        listSql[1] = sqlInsertDBMapping_Insert;
                        listParam[1] = listDBMapping_insert;

                        listSql[2] = sqlMt_tracer;
                        listParam[2] = listMt_tracer;
                    }
                    else if ((!string.IsNullOrEmpty(sqlInsertDBMapping_Delete) && sqlInsertDBMapping_Delete.Length > 0))
                    {
                        listSql = new String[4];
                        listParam = new List<JObject>[4];

                        listSql[0] = sqlAutoClose;
                        listParam[0] = listAutoClose;

                        listSql[1] = sqlInsertDBMapping_Delete;
                        listParam[1] = listDBMapping_delete;

                        listSql[2] = sqlInsertDBMapping_Insert;
                        listParam[2] = listDBMapping_insert;

                        listSql[3] = sqlMt_tracer;
                        listParam[3] = listMt_tracer;
                    }
                    else { }
                }
                //autoclose kosong & tracer detail isi
                else if ((string.IsNullOrEmpty(sqlAutoClose) && sqlAutoClose.Length == 0) && (!string.IsNullOrEmpty(sqlMtTracerDetail) && sqlMtTracerDetail.Length > 0))
                {
                    if ((string.IsNullOrEmpty(sqlInsertDBMapping_Delete) && sqlInsertDBMapping_Delete.Length == 0))
                    {
                        listSql = new String[3];
                        listParam = new List<JObject>[3];

                        listSql[0] = sqlInsertDBMapping_Insert;
                        listParam[0] = listDBMapping_insert;

                        listSql[1] = sqlMt_tracer;
                        listParam[1] = listMt_tracer;

                        listSql[2] = sqlMtTracerDetail;
                        listParam[2] = listMtTracerDetail;
                    }
                    else if ((!string.IsNullOrEmpty(sqlInsertDBMapping_Delete) && sqlInsertDBMapping_Delete.Length > 0))
                    {
                        listSql = new String[4];
                        listParam = new List<JObject>[4];

                        listSql[0] = sqlInsertDBMapping_Delete;
                        listParam[0] = listDBMapping_delete;

                        listSql[1] = sqlInsertDBMapping_Insert;
                        listParam[1] = listDBMapping_insert;

                        listSql[2] = sqlMt_tracer;
                        listParam[2] = listMt_tracer;

                        listSql[3] = sqlMtTracerDetail;
                        listParam[3] = listMtTracerDetail;
                    }
                    else { }
                }
                else if ((!string.IsNullOrEmpty(sqlAutoClose) || sqlAutoClose.Length > 0) && (!string.IsNullOrEmpty(sqlMtTracerDetail) || sqlMtTracerDetail.Length > 0))
                {
                    if ((string.IsNullOrEmpty(sqlInsertDBMapping_Delete) && sqlInsertDBMapping_Delete.Length == 0))
                    {
                        listSql = new String[4];
                        listParam = new List<JObject>[4];

                        listSql[0] = sqlAutoClose;
                        listParam[0] = listAutoClose;

                        listSql[1] = sqlInsertDBMapping_Insert;
                        listParam[1] = listDBMapping_insert;

                        listSql[2] = sqlMt_tracer;
                        listParam[2] = listMt_tracer;

                        listSql[3] = sqlMtTracerDetail;
                        listParam[3] = listMtTracerDetail;
                    }
                    else if ((!string.IsNullOrEmpty(sqlInsertDBMapping_Delete) && sqlInsertDBMapping_Delete.Length > 0))
                    {
                        listSql[0] = sqlAutoClose;
                        listParam[0] = listAutoClose;

                        listSql[1] = sqlInsertDBMapping_Delete;
                        listParam[1] = listDBMapping_delete;

                        listSql[2] = sqlInsertDBMapping_Insert;
                        listParam[2] = listDBMapping_insert;

                        listSql[3] = sqlMt_tracer;
                        listParam[3] = listMt_tracer;

                        listSql[4] = sqlMtTracerDetail;
                        listParam[4] = listMtTracerDetail;
                    }
                    else
                    {
                    }
                }
                else
                {

                }

                //ini list nya dynamic || min 2 max 5
                int res = oDB.queryUpdateWithParam2(listSql, listParam);

                if (res > 0)
                {
                    hasil = true;
                }

            }
            catch (Exception ex)
            {

                return hasil;
            }
            return hasil;
        }


        
        public bool isProcessInsertSwiftAdapter(Dictionary<string,string> pWsAdapterResultSwift, string pStatusKerjasama, ref int pInserted)
        {
            bool hasil = false;
            int res = 0;
            int getIns = 0;

            #region Insert Tracer
            string sqlInsertTracer = "INSERT INTO mt_tracer (Tag21, Type, RECEIVED_ON, Maker, ";
            sqlInsertTracer += "Status, Checker, isKerjasama, InOutSwift, ";
            sqlInsertTracer += "mt_category, correspondent, currency, TAG20, ";
            sqlInsertTracer += "amount, mt_103, last_update, account_name, ";
            sqlInsertTracer += "account_num, value_date, loadDate, InOu, ";
            sqlInsertTracer += "source_base )";
            sqlInsertTracer += " VALUES ";

            sqlInsertTracer += "(@PARAM_0, @PARAM_1, @PARAM_2, @PARAM_3, ";
            sqlInsertTracer += "@PARAM_4, @PARAM_5, @PARAM_6, @PARAM_7, ";
            sqlInsertTracer += "@PARAM_8, @PARAM_9, @PARAM_10, @PARAM_11, ";
            sqlInsertTracer += "@PARAM_12, @PARAM_13, @PARAM_14, @PARAM_15, ";
            sqlInsertTracer += "@PARAM_16, @PARAM_17, @PARAM_18, @PARAM_19, ";
            sqlInsertTracer += "@PARAM_20)";

            var listValueTracer = new List<JObject>();

            listValueTracer.Add(new JObject(new JProperty("@PARAM_0", pWsAdapterResultSwift["TAG20"])));
            listValueTracer.Add(new JObject(new JProperty("@PARAM_1", 199)));
            listValueTracer.Add(new JObject(new JProperty("@PARAM_2", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") )));
            listValueTracer.Add(new JObject(new JProperty("@PARAM_3", "System")));

            listValueTracer.Add(new JObject(new JProperty("@PARAM_4", 80)));
            listValueTracer.Add(new JObject(new JProperty("@PARAM_5", "System")));
            listValueTracer.Add(new JObject(new JProperty("@PARAM_6", pStatusKerjasama)));
            listValueTracer.Add(new JObject(new JProperty("@PARAM_7", "I")));

            listValueTracer.Add(new JObject(new JProperty("@PARAM_8", 1)));
            listValueTracer.Add(new JObject(new JProperty("@PARAM_9", pWsAdapterResultSwift["Depcor"])));
            listValueTracer.Add(new JObject(new JProperty("@PARAM_10", pWsAdapterResultSwift["TrxCurrency"])));
            listValueTracer.Add(new JObject(new JProperty("@PARAM_11", pWsAdapterResultSwift["NewSeqno"])));

            listValueTracer.Add(new JObject(new JProperty("@PARAM_12", pWsAdapterResultSwift["TrxAmount"])));
            listValueTracer.Add(new JObject(new JProperty("@PARAM_13", pWsAdapterResultSwift["FullMT"].Replace(System.Environment.NewLine,"\r\n" ))));
            listValueTracer.Add(new JObject(new JProperty("@PARAM_14", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") )));
            listValueTracer.Add(new JObject(new JProperty("@PARAM_15", pWsAdapterResultSwift["BenefName"])));

            listValueTracer.Add(new JObject(new JProperty("@PARAM_16", pWsAdapterResultSwift["BenefAccount"])));
            listValueTracer.Add(new JObject(new JProperty("@PARAM_17", pWsAdapterResultSwift["TrxDate"])));
            listValueTracer.Add(new JObject(new JProperty("@PARAM_18", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") )));
            listValueTracer.Add(new JObject(new JProperty("@PARAM_19", "O" )));

            listValueTracer.Add(new JObject(new JProperty("@PARAM_20", pWsAdapterResultSwift["TrxSource"])));
            #endregion

            #region Insert Tracer detail
            string sqlInsertTracerDetail = "INSERT INTO mt_tracer_detail (ROWID_TRACER, TRX_REF, BENEF_ACCOUNT, BENEF_NAME, ";
            sqlInsertTracerDetail += "VALUEDATE, CURRENCY, AMOUNT, BRINET_NAME, ";
            sqlInsertTracerDetail += "BRINET_FULL_NAME, DESCRIPTION, PROCESS_ID, ROWID_ORIGINAL, ";
            sqlInsertTracerDetail += "MTTYPE_ORIGINAL, TAG20_101, SENDER_BANK, RECEIVER_BANK, ";
            sqlInsertTracerDetail += "TAG53, TAG54, CANCEL_MT_TYPE, CANCEL_FEE_AMOUNT, ";
            sqlInsertTracerDetail += "CANCEL_FEE_CURRENCY, SENDER_BANK_ISDEPCOR, TAG56, STATUS_SWIFT_ADAPTER, COVERPLACE) ";
            sqlInsertTracerDetail += "";

            sqlInsertTracerDetail += " VALUES ";

            sqlInsertTracerDetail += "(@PARAM_INSERTED_ID, @PARAM_0, @PARAM_1, @PARAM_2, @PARAM_3, ";
            sqlInsertTracerDetail += "@PARAM_4, @PARAM_5, @PARAM_6, @PARAM_7, ";
            sqlInsertTracerDetail += "@PARAM_8, @PARAM_9, @PARAM_10, @PARAM_11, ";
            sqlInsertTracerDetail += "@PARAM_12, @PARAM_13, @PARAM_14, @PARAM_15, ";
            sqlInsertTracerDetail += "@PARAM_16, @PARAM_17, @PARAM_18, @PARAM_19, ";
            sqlInsertTracerDetail += "@PARAM_20, @PARAM_21, @PARAM_22, @PARAM_23 )";

            var listValueTracerDetail = new List<JObject>();
                
            listValueTracerDetail.Add(new JObject(new JProperty("@PARAM_0", pWsAdapterResultSwift["TAG20"])));
            listValueTracerDetail.Add(new JObject(new JProperty("@PARAM_1", pWsAdapterResultSwift["BenefAccount"])));
            listValueTracerDetail.Add(new JObject(new JProperty("@PARAM_2", pWsAdapterResultSwift["BenefName"])));

            listValueTracerDetail.Add(new JObject(new JProperty("@PARAM_3", pWsAdapterResultSwift["TrxDate"])));
            listValueTracerDetail.Add(new JObject(new JProperty("@PARAM_4", pWsAdapterResultSwift["TrxCurrency"])));
            listValueTracerDetail.Add(new JObject(new JProperty("@PARAM_5", pWsAdapterResultSwift["TrxAmount"])));
            listValueTracerDetail.Add(new JObject(new JProperty("@PARAM_6", pWsAdapterResultSwift["BrinetName"])));

            listValueTracerDetail.Add(new JObject(new JProperty("@PARAM_7", pWsAdapterResultSwift["BrinetFullName"])));
            listValueTracerDetail.Add(new JObject(new JProperty("@PARAM_8", pWsAdapterResultSwift["Description"])));
            listValueTracerDetail.Add(new JObject(new JProperty("@PARAM_9", pWsAdapterResultSwift["ProcessId"])));
            listValueTracerDetail.Add(new JObject(new JProperty("@PARAM_10", pWsAdapterResultSwift["RowID"])));

            listValueTracerDetail.Add(new JObject(new JProperty("@PARAM_11", pWsAdapterResultSwift["MTtype"])));
            listValueTracerDetail.Add(new JObject(new JProperty("@PARAM_12", pWsAdapterResultSwift["TAG20_101"])));
            listValueTracerDetail.Add(new JObject(new JProperty("@PARAM_13", pWsAdapterResultSwift["SenderBIC"])));
            listValueTracerDetail.Add(new JObject(new JProperty("@PARAM_14", pWsAdapterResultSwift["ReceiverBIC"])));

            listValueTracerDetail.Add(new JObject(new JProperty("@PARAM_15", pWsAdapterResultSwift["Tag53"])));
            listValueTracerDetail.Add(new JObject(new JProperty("@PARAM_16", pWsAdapterResultSwift["Tag54"])));
            listValueTracerDetail.Add(new JObject(new JProperty("@PARAM_17", pWsAdapterResultSwift["CancelMTType"])));
            listValueTracerDetail.Add(new JObject(new JProperty("@PARAM_18", pWsAdapterResultSwift["CancelFeeAmount"])));

            listValueTracerDetail.Add(new JObject(new JProperty("@PARAM_19", pWsAdapterResultSwift["CancelFeeCurrency"])));
            listValueTracerDetail.Add(new JObject(new JProperty("@PARAM_20", pWsAdapterResultSwift["SenderBicIsDepcor"])));
            listValueTracerDetail.Add(new JObject(new JProperty("@PARAM_21", pWsAdapterResultSwift["Tag56"])));
            listValueTracerDetail.Add(new JObject(new JProperty("@PARAM_22", pWsAdapterResultSwift["SwiftAdapterStatus"])));
            listValueTracerDetail.Add(new JObject(new JProperty("@PARAM_23", pWsAdapterResultSwift["Depcor"])));
            #endregion

            
            res  = oDB.queryInsertWithPrevInsertedId(sqlInsertTracer, sqlInsertTracerDetail, listValueTracer, listValueTracerDetail, ref getIns);

            pInserted = getIns;
            if (res > 0)
            {
                hasil = true;
            }

            return hasil;
        }

        

        public bool isExistSwiftAdapter(string pTag20, string pTrxDate)
        {
            bool hasil = false;

            string sqlCekSA = "SELECT * FROM mt_tracer_detail WHERE TRX_REF = @PARAM_0 AND VALUEDATE = @PARAM_1 and PROCESS_ID in ('4','41') ";
            var listSA = new List<JObject>();
            listSA.Add(new JObject(new JProperty("@PARAM_0", pTag20)));
            listSA.Add(new JObject(new JProperty("@PARAM_1", pTrxDate)));

            DataTable dtCekMapping = oDB.querySelectExtendedWithParam(sqlCekSA, listSA);

            if (dtCekMapping.Rows.Count > 0)
            {
                hasil = true;
            }

            return hasil;
        }

        public bool insertLog(string id,string reff_no, string user, string message, string appName, string action)
        {
            bool hasil = false;
            String presentDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            /*
            string sql = "INSERT INTO swt_log (TRXLOG, SOURCE_IP, ROWID_TRX, TAG20, USER, MESSAGE, MENU, ACTION) " +
                "VALUES(@PARAM_0, @PARAM_1, @PARAM_2, @PARAM_3, @PARAM_4, @PARAM_5, @PARAM_6, @PARAM_7)";
            */
            string sql = "INSERT INTO swt_log (TRXLOG, SOURCE_IP, ROWID_TRX, TAG20, USER, MESSAGE, MENU, ACTION) " +
                "VALUES(NOW(), @PARAM_0, @PARAM_1, @PARAM_2, @PARAM_3, @PARAM_4, @PARAM_5, @PARAM_6)";
            var list = new List<JObject>();

 
            list.Add(new JObject(new JProperty("@PARAM_0", GetIPAddress())));
            list.Add(new JObject(new JProperty("@PARAM_1", id)));
            list.Add(new JObject(new JProperty("@PARAM_2", reff_no)));
            list.Add(new JObject(new JProperty("@PARAM_3", user)));
            list.Add(new JObject(new JProperty("@PARAM_4", message)));
            list.Add(new JObject(new JProperty("@PARAM_5", appName)));
            list.Add(new JObject(new JProperty("@PARAM_6", action)));

            int res = oDB.queryUpdateWithParam(sql, list);

            if (res > 0)
            { 
                hasil = true;
            }

            return hasil;
        }

        public string GetIPAddress()
        {
            try
            {
                string ipFinal = "";
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        ipFinal = ip.ToString();
                    }
                }

                return ipFinal;
            }
            catch (Exception e)
            {
                return "";
            }
        }

        public bool updateParamSch(
            string pSchFunction, string pRepeatDaily, string pStartTime, string pStopTime, 
            string pWeekEnd, string pMaxThread, string pMaxQuery, string pMaxRetry
            )
        {
            bool result = false;

            /*
            string sql = "UPDATE md_scheduler_activity ";
            sql += " SET ";
            sql += " REPEAT_DAILY_LIMIT = '" + pRepeatDaily + "', ";
            sql += " START_TIME = '"+pStartTime+"', ";
            sql += " STOP_TIME = '"+pStopTime+"', ";
            sql += " WEEKEND_CHECK = '"+ pWeekEnd + "', ";
            sql += " MAX_THREAD = '"+pMaxThread+"', ";
            sql += " MAX_QUERY_RESULT = '"+pMaxQuery+"', ";
            sql += " MAX_RETRY = '" + pMaxRetry + "' ";
            sql += " WHERE SCH_FUNCTION = '" + pSchFunction + "' ";
            */
            string sql = "UPDATE md_scheduler_activity ";
            sql += " SET ";
            sql += " REPEAT_DAILY_LIMIT = @PARAM_0, ";
            sql += " START_TIME =  @PARAM_1, ";
            sql += " STOP_TIME = @PARAM_2, ";
            sql += " WEEKEND_CHECK = @PARAM_3, ";
            sql += " MAX_THREAD = @PARAM_4, ";
            sql += " MAX_QUERY_RESULT = @PARAM_5, ";
            sql += " MAX_RETRY = @PARAM_6 ";
            sql += " WHERE SCH_FUNCTION = @PARAM_7 ";

            var list = new List<JObject>();
            list.Add(new JObject(new JProperty("@PARAM_0", pRepeatDaily)));
            list.Add(new JObject(new JProperty("@PARAM_1", pStartTime)));
            list.Add(new JObject(new JProperty("@PARAM_2", pStopTime)));
            list.Add(new JObject(new JProperty("@PARAM_3", pWeekEnd)));
            list.Add(new JObject(new JProperty("@PARAM_4", pMaxThread)));
            list.Add(new JObject(new JProperty("@PARAM_5", pMaxQuery)));
            list.Add(new JObject(new JProperty("@PARAM_6", pMaxRetry)));
            list.Add(new JObject(new JProperty("@PARAM_7", pSchFunction)));


            int res = oDB.queryUpdateWithParam(sql, list);

            if (res > 0)
            {
                result = true;
            }

            return result;
            
        }


        #region Bagian De-Encryppt
        public string getUsername(string pUsername)
        {
            string result = "";
            string sql = "SELECT Parameter_desc FROM swt_parameter WHERE Parameter_Type like '" + pUsername + "' ";

            var listdummy = new List<JObject>();
            DataTable dt = oDB.querySelectExtendedWithParam(sql, listdummy);

            result = dt.Rows[0]["Parameter_desc"].ToString();

            return result;
        }
        public string getWSKatakunci(string pKatakunci)
        {
            string result = "";
            string sql = "SELECT Parameter_desc FROM swt_parameter WHERE Parameter_Type like '" + pKatakunci + "' ";

            var listdummy = new List<JObject>();
            DataTable dt = oDB.querySelectExtendedWithParam(sql, listdummy);

            result = dt.Rows[0]["Parameter_desc"].ToString();

            return result;
        }

        public string getWSAddress(string pWSAddress)
        {
            string result = "";
            string sql = "SELECT Parameter_desc FROM swt_parameter WHERE Parameter_Type like '" + pWSAddress + "' ";

            var listdummy = new List<JObject>();
            DataTable dt = oDB.querySelectExtendedWithParam(sql, listdummy);

            result = dt.Rows[0]["Parameter_desc"].ToString();

            return result;
        }

        public string getTDate(string pTargetDate)
        {
            string result = "";
            string sql = "SELECT Parameter_desc FROM swt_parameter WHERE Parameter_Type like '" + pTargetDate + "' ";

            var listdummy = new List<JObject>();
            DataTable dt = oDB.querySelectExtendedWithParam(sql, listdummy);

            result = dt.Rows[0]["Parameter_desc"].ToString();

            return result;
        }

        public string getParameterValue(string pParam)
        {
            string result = "";
            string sql = "SELECT Parameter_desc FROM swt_parameter WHERE Parameter_Type = @PARAM_0 ";

            var list = new List<JObject>();
            list.Add(new JObject(new JProperty("@PARAM_0", pParam)));

            DataTable dt = oDB.querySelectExtendedWithParam(sql, list);

            if (dt.Rows.Count > 0)
            {
                result = dt.Rows[0]["Parameter_desc"].ToString();
            }

            return result;
        }

        #endregion
    }
}
