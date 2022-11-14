using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using Helper;
using System.IO;
using Newtonsoft.Json.Linq;
using SchTracer.helper;
using System.Data.SqlClient;
using System.Globalization;
using System.Configuration;
using SchTracer.Models;
using SchTracer.WebServices;
using Helper.Models;

namespace SchTracer
{
    public partial class ChildFormMain : Form
    {
        private Config oConfig;
        private cData oData;
        private cDataSqlSrv oDataSqlSrv;
        private DBConnection oDB;

        //private DBConnection oDBSQLServer;
        private DBConnectionSqlSrv oDBSQLServer;

        private WebServicesMapping WSRequest;

        private String timeDtart = "05:00:00";
        private String timeStop = "21:00:00";
        private int timeoutLimit = 60000;

        private bool SchChecker = false;
        public delegate void populateTextBoxDelegate(String text);
        public delegate void populateLabelDelegate(String text);

        private String msgRunSch = "";
        private string treatFunction = "";

        private String msgRunSchMapping = "";
        private string treatFunctionMapping = "";

        private String msgRunSchSwiftAdapter = "";
        private string treatFunctionSwiftAdapter = "";

        //ini buat log
        private AppSettingsReader vAppSetting;
        private string tellerId;
        private string branchCodeParam;

        private string appName;
        //

        //Mirroring
        private DBSchParameter oSwiftTracer_Mirroring;
        private string schName_Mirroring;
        private int maxThread_Mirroring = 1;
        private int maxResult_Mirroring = 1;
        private int runningThread_Mirroring = 0;
        private int cancelThread_Mirroring = 0;
        private int interval_Mirroring = 0;
        private String timeStart_Mirroring = "06:00:00";
        private String timeStop_Mirroring = "20:00:00";
        private int checkWeekend_Mirroring = 0;
        private System.Windows.Forms.Timer timer_Mirroring;
        List<BackgroundWorker> bgws_Mirroring = new List<BackgroundWorker>();

        //Mapping
        private DBSchParameter oSwiftTracer_Mapping;
        private string schName_Mapping;
        private int maxThread_Mapping = 1;
        private int maxResult_Mapping = 1;
        private int runningThread_Mapping = 0;
        private int cancelThread_Mapping = 0;
        private int interval_Mapping = 0;
        private String timeStart_Mapping = "06:00:00";
        private String timeStop_Mapping = "20:00:00";
        private int checkWeekend_Mapping = 0;
        private System.Windows.Forms.Timer timer_Mapping;
        List<BackgroundWorker> bgws_Mapping = new List<BackgroundWorker>();

        //Mirroring
        private DBSchParameter oSwiftTracer_SwiftAdapter;
        private string schName_SwiftAdapter;
        private int maxThread_SwiftAdapter = 1;
        private int maxResult_SwiftAdapter = 1;
        private int runningThread_SwiftAdapter = 0;
        private int cancelThread_SwiftAdapter = 0;
        private int interval_SwiftAdapter = 0;
        private String timeStart_SwiftAdapter = "06:00:00";
        private String timeStop_SwiftAdapter = "20:00:00";
        private int checkWeekend_SwiftAdapter = 0;
        private System.Windows.Forms.Timer timer_SwiftAdapter;
        List<BackgroundWorker> bgws_SwiftAdapter = new List<BackgroundWorker>();

        public Logger oLogger;

        public string WebServiceUrl = "";
        public ChildFormMain()
        {
            InitializeComponent();
            oConfig = new Config();

            #region Connection Mysql DB
            // Cek koneksi
            string tDbUser = "";
            string tDbKunci = "";

            try
            {
                tDbUser = oConfig.getParameter("DBUsername");
                tDbKunci = oConfig.getParameter("DBPassword");
            }
            catch (Exception e) { }


            oDB = new DBConnection(oConfig.isDebug());
            oDB.setDBDataSource(oConfig.getParameter("DBDataSource"));
            oDB.setDBCatalog(oConfig.getParameter("DBInitialCatalog"));
            oDB.setDBUsername(tDbUser);
            oDB.setDBPassword(tDbKunci);

            oDB.initConnection();

            if (!oDB.isConnected())
            {
                MessageBox.Show("Failed To Connect MySQL Database");
                System.Environment.Exit(0);
            }
            #endregion

            #region Connection SQlServer DB
            string tDbUserSqlServer = "";
            string tDbKunciSqlServer = "";

            try
            {
                tDbUserSqlServer = oConfig.getParameter("DBUsernameSQLServer");

                tDbKunciSqlServer = oConfig.getParameter("DBPasswordSQLServer");
            }
            catch (Exception e) { }


            oDBSQLServer = new DBConnectionSqlSrv(oConfig.isDebug());
            oDBSQLServer.setDBDataSource(oConfig.getParameter("DBDataSourceSQLServer"));
            oDBSQLServer.setDBCatalog(oConfig.getParameter("DBInitialCatalogSQLServer"));
            oDBSQLServer.setDBUsername(tDbUserSqlServer);

            oDBSQLServer.setDBGembok(tDbKunciSqlServer);

            oDBSQLServer.initConnection();

            if (!oDBSQLServer.isConnected())
            {
                MessageBox.Show("Failed To Connect SqlServer Database");
                System.Environment.Exit(0);
            }
            #endregion

            if (IsSchOpen())
            {
                
                if (msgRunSch != null)
                {
                    MessageBox.Show("Scheduler Function " + msgRunSch + " Already Open!!!");
                    System.Environment.Exit(0);
                }
                else
                {
                    MessageBox.Show("Scheduller Already Open!!!");
                    System.Environment.Exit(0);
                }


            }

            //Mirroring
            oSwiftTracer_Mirroring = new DBSchParameter(oDB, oConfig.getParameter("APPName"), oConfig.getParameter("APPFunction_Mirroring"));
            if (!oSwiftTracer_Mirroring.isValidParameterValue())
            {
                MessageBox.Show("Error " + oSwiftTracer_Mirroring.getErrorMessage());
                System.Environment.Exit(0);
            }
            else 
            {
                schName_Mirroring = oSwiftTracer_Mirroring.SCH_NAME();
                maxThread_Mirroring = oSwiftTracer_Mirroring.MAX_THREAD();
                maxResult_Mirroring = oSwiftTracer_Mirroring.MAX_QUERY_RESULT();
                interval_Mirroring = oSwiftTracer_Mirroring.TIMER_INTERVAL_MILISECOND();
                timeStart_Mirroring = oSwiftTracer_Mirroring.START_TIME();
                timeStop_Mirroring = oSwiftTracer_Mirroring.STOP_TIME();
                checkWeekend_Mirroring = oSwiftTracer_Mirroring.WEEKEND_CHECK();
            }

            //Mapping
            oSwiftTracer_Mapping = new DBSchParameter(oDB, oConfig.getParameter("APPName"), oConfig.getParameter("APPFunction_Mapping"));
            if (!oSwiftTracer_Mapping.isValidParameterValue())
            {
                MessageBox.Show("Error " + oSwiftTracer_Mapping.getErrorMessage());
                System.Environment.Exit(0);
            }
            else 
            {
                schName_Mapping = oSwiftTracer_Mapping.SCH_NAME();
                maxThread_Mapping = oSwiftTracer_Mapping.MAX_THREAD();
                maxResult_Mapping = oSwiftTracer_Mapping.MAX_QUERY_RESULT();
                interval_Mapping = oSwiftTracer_Mapping.TIMER_INTERVAL_MILISECOND();
                timeStart_Mapping = oSwiftTracer_Mapping.START_TIME();
                timeStop_Mapping = oSwiftTracer_Mapping.STOP_TIME();
                checkWeekend_Mapping = oSwiftTracer_Mapping.WEEKEND_CHECK();
            }

            //SwiftAdapter
            oSwiftTracer_SwiftAdapter = new DBSchParameter(oDB, oConfig.getParameter("APPName"), oConfig.getParameter("APPFunction_SwiftAdapter"));
            if (!oSwiftTracer_SwiftAdapter.isValidParameterValue())
            {
                MessageBox.Show("Error " + oSwiftTracer_SwiftAdapter.getErrorMessage());
                System.Environment.Exit(0);
            }
            else
            {
                schName_SwiftAdapter = oSwiftTracer_SwiftAdapter.SCH_NAME();
                maxThread_SwiftAdapter = oSwiftTracer_SwiftAdapter.MAX_THREAD();
                maxResult_SwiftAdapter = oSwiftTracer_SwiftAdapter.MAX_QUERY_RESULT();
                interval_SwiftAdapter = oSwiftTracer_SwiftAdapter.TIMER_INTERVAL_MILISECOND();
                timeStart_SwiftAdapter = oSwiftTracer_SwiftAdapter.START_TIME();
                timeStop_SwiftAdapter = oSwiftTracer_SwiftAdapter.STOP_TIME();
                checkWeekend_SwiftAdapter = oSwiftTracer_SwiftAdapter.WEEKEND_CHECK();
            }


            timeoutLimit = int.Parse(oConfig.getParameter("TimeoutLimit"));

            oData = new cData(oDB);
            oDataSqlSrv = new cDataSqlSrv(oDBSQLServer);

            Program.oLogger = new Logger(oDB, oConfig);
            btnStop.Enabled = false;
            btnStop.BackColor = Color.Gray;

            WebServiceUrl = oConfig.getParameter("urlGetDetailTransaction");
        }    

        private bool IsSchOpen()
        {
            try
            {
                String sql = "SELECT IS_RUN, SCH_FUNCTION FROM md_scheduler_activity WHERE SCH_NAME = @PARAM_0 ";

                var list = new List<JObject>();
                list.Add(new JObject(new JProperty("@PARAM_0", oConfig.getParameter("APPName"))));

                DataTable dt = oDB.querySelectExtendedWithParam(sql, list);

                bool status = true;

                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr["IS_RUN"].ToString() == "0")
                        {
                            status = false;
                        }
                        else //kalau Sch_name ada bbrp function
                        {
                            msgRunSch = dr["SCH_FUNCTION"].ToString();
                            return true;
                        }
                    }
                }
                else
                {
                    return true;
                }
                return status;
            }
            catch (Exception ex)
            {
                return true;
            }
        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            if (!IsSchOpen())
            {


                String sql = "UPDATE md_scheduler_activity SET IS_RUN = 1 WHERE SCH_NAME = @PARAM_0 ";

                var list = new List<JObject>();
                list.Add(new JObject(new JProperty("@PARAM_0", oConfig.getParameter("APPName"))));

                oDB.queryUpdateWithParam(sql, list);

       
                btnStart.Enabled = false;
                btnStart.BackColor = Color.Gray;

                btnStop.Enabled = true;
                btnStop.BackColor = Color.Red;

                //buttonExit.Enabled = false; // button parent
                Program.oLogger = new Logger(1); // pengganti btn ke status
                //Program.oLogger = new Logger("ChildMain");
                Program.oLogger = new Logger(1, "ChildMain");

                //Mirroring
                timer_Mirroring = new System.Windows.Forms.Timer();
                timer_Mirroring.Interval = interval_Mirroring;
                timer_Mirroring.Tick += new EventHandler(onTick_Mirroring);
                timer_Mirroring.Enabled = true;
                timer_Mirroring.Start();

                //Mapping
                timer_Mapping = new System.Windows.Forms.Timer();
                timer_Mapping.Interval = interval_Mapping;
                timer_Mapping.Tick += new EventHandler(onTick_Mapping);
                timer_Mapping.Enabled = true;
                timer_Mapping.Start();

                //SwiftAdapter
                timer_SwiftAdapter = new System.Windows.Forms.Timer();
                timer_SwiftAdapter.Interval = interval_SwiftAdapter;
                timer_SwiftAdapter.Tick += new EventHandler(onTick_SwiftAdapter);
                timer_SwiftAdapter.Enabled = true;
                timer_SwiftAdapter.Start();
            }
        }

        #region Texbox1 prop
        private void populateTextBoxWithTimeStamp(String text)
        {
            if (textBox1.TextLength >= 100000)
            {
                textBox1.Clear();
            }
            String now = DateTime.Now.ToString();

            this.populateTextBox(now + "    " + text); // 4 spaces
        }

        private void populateTextBox(String text)
        {
            textBox1.AppendText(text + Environment.NewLine);
        }

        private void populateTextBoxclear(string text)
        {
            textBox1.Clear();
        }
        
        private bool isValidRepeatLimit(string appName, string appFunction, ref DBSchParameter param)
        {
            try
            {
                int prevRepeatToday = param.REPEAT_TODAY();
                DateTime dtLastRun = DateTime.ParseExact(param.LAST_RUN(), "M/d/yyyy", null);
                DateTime dtNow = DateTime.Now;


                if (param.REPEAT_DAILY_LIMIT() == 0) // ga ada limit
                {
                    return true;
                }
                else if ((dtNow.Date == dtLastRun.Date && param.REPEAT_TODAY() < param.REPEAT_DAILY_LIMIT()) || (dtNow.Date > dtLastRun.Date))
                {
                    String sql = "select CASE WHEN CURDATE() = DATE(LAST_RUN) THEN REPEAT_TODAY ELSE 0 END as TODAY, DATE(LAST_RUN) as TGL from md_scheduler_activity where SCH_NAME = @PARAM_0 and SCH_FUNCTION = @PARAM_1 and REPEAT_DAILY_LIMIT != 0 and ((CURDATE() = DATE(LAST_RUN) and REPEAT_TODAY < REPEAT_DAILY_LIMIT) or CURDATE() > DATE(LAST_RUN)) ";

                    var list = new List<JObject>();
                    list.Add(new JObject(new JProperty("@PARAM_0", appName)));
                    list.Add(new JObject(new JProperty("@PARAM_1", appFunction)));

                    DataTable dt = oDB.querySelectExtendedWithParam(sql, list);

                    if (dt != null)
                    {
                        int total = dt.Rows.Count;

                        if (total > 0)
                        {
                            foreach (DataRow drParam in dt.Rows)
                            {
                                param.SET_REPEAT_TODAY(Int32.Parse(drParam["TODAY"].ToString()));
                                param.SET_LAST_RUN(drParam["TGL"].ToString());
                            }

                            if (prevRepeatToday != param.REPEAT_TODAY())
                            {
                                //update db
                                string sql2 = "UPDATE md_scheduler_activity SET REPEAT_TODAY = 0 WHERE SCH_NAME = @PARAM_0 AND SCH_FUNCTION = @PARAM_1 ";

                                var list2 = new List<JObject>();
                                list2.Add(new JObject(new JProperty("@PARAM_0", appName)));
                                list2.Add(new JObject(new JProperty("@PARAM_1", appFunction)));

                                oDB.queryUpdateWithParam(sql2, list2);
                            }

                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception sqlE)
            {
                return false;
            }
        }

        private void addRunTodayRepeateValue(string appName, string appFunction, ref DBSchParameter param)
        {
            // ga ada limit, ga usah update
            if (param.REPEAT_DAILY_LIMIT() == 0)
            {

            }
            else
            {
                //increment
                string sql = "UPDATE md_scheduler_activity SET REPEAT_TODAY = REPEAT_TODAY + 1 WHERE SCH_NAME = @PARAM_0 AND SCH_FUNCTION = @PARAM_1 ";

                var list = new List<JObject>();
                list.Add(new JObject(new JProperty("@PARAM_0", appName)));
                list.Add(new JObject(new JProperty("@PARAM_1", appFunction)));

                oDB.queryUpdateWithParam(sql, list);



                //update lokal variable
                sql = "select REPEAT_TODAY as TODAY, DATE(LAST_RUN) as TGL from md_scheduler_activity where SCH_NAME = @PARAM_0 and SCH_FUNCTION = @PARAM_1 ";

                DataTable dt = oDB.querySelectExtendedWithParam(sql, list);

                if (dt != null)
                {
                    int total = dt.Rows.Count;

                    if (total > 0)
                    {
                        foreach (DataRow drParam in dt.Rows)
                        {
                            param.SET_REPEAT_TODAY(Int32.Parse(drParam["TODAY"].ToString()));
                            param.SET_LAST_RUN(drParam["TGL"].ToString());
                        }
                    }
                }
            }
        }

        private bool validTime(int checkweekend, String timeStart, String timeStop)
        {
            bool result = false;

            try
            {
                if (checkweekend == 0)
                {
                    if (DateTime.Now.DayOfWeek != DayOfWeek.Saturday && DateTime.Now.DayOfWeek != DayOfWeek.Sunday)
                    {
                        String[] start = timeStart.Split(':');
                        String[] stop = timeStop.Split(':');

                        TimeSpan tsStart = new TimeSpan(Int32.Parse(start[0]), Int32.Parse(start[1]), Int32.Parse(start[2]));
                        TimeSpan tsStop = new TimeSpan(Int32.Parse(stop[0]), Int32.Parse(stop[1]), Int32.Parse(stop[2]));

                        TimeSpan tsNow = DateTime.Now.TimeOfDay;

                        if ((tsNow > tsStart) && (tsNow < tsStop))
                        {
                            result = true;
                        }
                    }
                    else
                    {
                        result = false;
                    }
                }
                else
                {
                    String[] start = timeStart.Split(':');
                    String[] stop = timeStop.Split(':');

                    TimeSpan tsStart = new TimeSpan(Int32.Parse(start[0]), Int32.Parse(start[1]), Int32.Parse(start[2]));
                    TimeSpan tsStop = new TimeSpan(Int32.Parse(stop[0]), Int32.Parse(stop[1]), Int32.Parse(stop[2]));
                    TimeSpan tsNow = DateTime.Now.TimeOfDay;

                    if ((tsNow > tsStart) && (tsNow < tsStop))
                    {
                        result = true;
                    }
                }
            }
            catch (Exception e)
            {
                if (oConfig.isDebug())
                {
                    Console.WriteLine(e);
                }
                result = false;
            }

            return result;
        }
        #endregion


        #region Mirroring
        public void onTick_Mirroring(Object s, EventArgs ae)
        {
            if (bgws_Mirroring.Count == 0)
            {
                for (int i = 0; i < maxThread_Mirroring; i++)
                {
                    bgws_Mirroring.Add(new BackgroundWorker());
                    bgws_Mirroring[i].DoWork += DoWork_Mirroring;
                    bgws_Mirroring[i].RunWorkerCompleted += bgws_Completed_Mirroring;
                    bgws_Mirroring[i].WorkerSupportsCancellation = true;
                    bgws_Mirroring[i].RunWorkerAsync(i);
                }
            }
        }
        public void bgws_Completed_Mirroring(object sender, RunWorkerCompletedEventArgs e)
        {
            String message = "";
            int i = bgws_Mirroring.IndexOf((BackgroundWorker)sender);

            if (e.Cancelled == true)
            {
                cancelThread_Mirroring += 1;
                bgws_Mirroring.RemoveAt(1);
                if (cancelThread_Mirroring + runningThread_Mirroring + 1 == maxThread_Mirroring)
                {
                    message += "Thread Cancelled";
                }

            }
            else if (!(e.Error == null))
            {
                message = "Thread Error: " + e.Error.Message;
            }
            else
            {
                bgws_Mirroring.RemoveAt(i);
                message = "Worker "+ treatFunction + " [" + runningThread_Mirroring + "] Done";
                runningThread_Mirroring += 1;
                if (bgws_Mirroring.Count == 0)
                {
                    message += (Environment.NewLine + "***   Thread Success!!!   ***" + Environment.NewLine);
                    runningThread_Mirroring = 0;
                }
            }

            if (message != "")
            {
                Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { message });
            }
        }
        public void DoWork_Mirroring(Object sender, DoWorkEventArgs e)
        {
            BackgroundWorker workerThread = sender as BackgroundWorker;
            int tred = (int)e.Argument;

            if (workerThread.CancellationPending == true)
            {
                e.Cancel = true;
            }
            else
            {
 
                string appName = oConfig.getParameter("APPName");
                string appFunction = oConfig.getParameter("APPFunction_Mirroring");
                treatFunction = appFunction;
                string tellerID = oConfig.getParameter("TellerID");
                string branch = oConfig.getParameter("Branch");
                DateTime currdate = DateTime.Now;
                int configRD = Int16.Parse(oConfig.getParameter("DateRange"));

                bool updated = false;

  
                string rangeDate = currdate.Subtract(TimeSpan.FromDays(configRD)).ToString("yyyy-MM-dd");
                string rangeDatefix = rangeDate + " 00:00:00";

                string mtType = "";

 
                List<String> ListMirrorID = new List<string>();
                bool checkMirrorID = false;
                int insertedID = 0;



                if (validTime(checkWeekend_Mirroring, timeStart_Mirroring, timeStop_Mirroring) && isValidRepeatLimit(appName, appFunction, ref oSwiftTracer_Mirroring))
                {
                    //show console log
                    Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { appFunction + " Start..." });

                    oData.updateSchRunning(appName, appFunction);

                    mtType = oData.getMtTypeParam("mt_type");

 

                    try
                    {
                        //enhance
                        DataTable DataGudangMT = oDataSqlSrv.dataGudangMT(maxThread_Mirroring, tred, maxResult_Mirroring, mtType, rangeDatefix);

                        //DataTable DataMtTracer = oData.DataMtTracer(rangeDatefix);
                        DataTable DataMtTracer = oData.DataMtTracer(maxThread_Mirroring, tred, maxResult_Mirroring, mtType, rangeDatefix);

                        if (DataGudangMT == null)
                        {
                            Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { "No data to process" });
                        }
                        else if (DataGudangMT.Rows.Count < DataMtTracer.Rows.Count)
                        {
                            Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { "Data already exist at table destination" });
                        }
                        else
                        {
                            try
                            {
                                DataTable dtFinal = DataGudangMT.Rows.OfType<DataRow>().Where(
                                                                    a => DataGudangMT.Rows.OfType<DataRow>().Select(
                                                                        k => Convert.ToInt32(k["RowID"])).Except(DataMtTracer.Rows.OfType<DataRow>().Select(
                                                                            k => Convert.ToInt32(k["RowID"])).ToList()).Contains(Convert.ToInt32(a["RowID"]))).CopyToDataTable();

                                if (dtFinal.Rows.Count > 0)
                                {
                                    List<String> ListMirrorExceptID = new List<string>();

                                    foreach (DataRow row in dtFinal.Rows)
                                    {
                                        ListMirrorExceptID.Add(row["RowID"].ToString());
                                    }
                                    try
                                    {
                                        DataTable dataMirroring = oDataSqlSrv.fetchDataMirroringFinal(ListMirrorExceptID);

                                        int totalDataMirror = dataMirroring.Rows.Count;
                                        int iterationX = 0;
                                        if (totalDataMirror > 0)
                                        {
                                            for (int i = 0; i < dataMirroring.Rows.Count; i++)
                                            {
                                                iterationX = iterationX + 1; // to start at 1 not 0
                                                string valRefNo = dataMirroring.Rows[i]["RefNo"].ToString();
                                                string valMT = dataMirroring.Rows[i]["MT"].ToString();
                                                string valType = dataMirroring.Rows[i]["Type"].ToString();
                                                string valInOut = dataMirroring.Rows[i]["InOut"].ToString();
                                                string valRowId = dataMirroring.Rows[i]["RowID"].ToString();

                                                string tempLoadDate = dataMirroring.Rows[i]["LoadDate"].ToString();
                                                string valLoadDate = DateTime.ParseExact(tempLoadDate, "M/d/yyyy h:mm:ss tt", CultureInfo.GetCultureInfo("en-US")).ToString("yyyy-MM-dd HH:mm:ss");

                                                string valStrLine = printNames(valMT);

                                                checkMirrorID = oData.checkMirroringID(dataMirroring.Rows[i]["RowID"].ToString());

                                                if (checkMirrorID == true && valStrLine.Length > 0) //Blm ada
                                                {
                                                    insertedID = oData.insertMirroring(valRefNo, valStrLine, valMT, valType, 0, valInOut, valRowId, valLoadDate);

                                                    //WARNING long/int
                                                    
                                                    if (insertedID > 0)
                                                    {
                                                        //Log
                                                        vAppSetting = new AppSettingsReader();

                                                        string valUser = "Scheduler Swift tracer";
                                                        branchCodeParam = vAppSetting.GetValue("Branch", Type.GetType("System.String")).ToString();

                                                        appName = vAppSetting.GetValue("APPName", Type.GetType("System.String")).ToString();
                                                        String pInsertedID = insertedID.ToString(); //RowId From MtTracer Not SplitterRowID
                                                                                               
                                                        
                                                        String pMessage = "Worker " + treatFunction + " [" + runningThread_Mirroring + "] process [" + iterationX.ToString() + "] of [" + totalDataMirror.ToString() + "] data splitter row id [" + valRowId.ToString() + "] : Insert Success";
                                                        String pAppName = appName;
                                                        String pAction = "Mirroring";

                                                        oData.insertLog(pInsertedID, valRefNo, valUser, pMessage, pAppName, pAction);
        
                                                        Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { pMessage });
                                                    }
                                                    else
                                                    {
                                                        //Log
                                                        vAppSetting = new AppSettingsReader();

                                                        string valUser = "Scheduler Swift tracer";
                                                        branchCodeParam = vAppSetting.GetValue("Branch", Type.GetType("System.String")).ToString();

                                                        appName = vAppSetting.GetValue("APPName", Type.GetType("System.String")).ToString();
                                                        String pInsertedID = "0"; // o coz cannot insert
                                                        
                                                        String pMessage = "Worker " + treatFunction + " [" + runningThread_Mirroring + "] process [" + iterationX.ToString() + "] of [" + totalDataMirror.ToString() + "] data splitter row id [" + valRowId.ToString() + "] : Insert Failed";
                                                        String pAppName = appName;
                                                        String pAction = "Mirroring";

                                                        oData.insertLog(pInsertedID, valRefNo, valUser, pMessage, pAppName, pAction);
                                                        
                                                        //pupolatebox
                                                        Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { pMessage });
                                                    }
                                                }
                                                else
                                                {
                                                    vAppSetting = new AppSettingsReader();

                                                    string valUser = "Scheduler Swift tracer";

                                                    appName = vAppSetting.GetValue("APPName", Type.GetType("System.String")).ToString();
                                                    String pInsertedID = "0"; //cannot get insert result so 0
                                                    
                                                    String pMessage = "Worker " + treatFunction + " [" + runningThread_Mirroring + "] process [" + iterationX.ToString() + "] of [" + totalDataMirror.ToString() + "] data splitter row id [" + valRowId.ToString() + "] : Data already exist OR not contain tag21";
                                                    String pAppName = appName;
                                                    String pAction = "Mirroring";

                                                    oData.insertLog(pInsertedID, valRefNo, valUser, pMessage, pAppName, pAction);
                                                    //pupolatebox
                                                    Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { pMessage });
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { "Error get List ID Mirroring" });
                                        }
                                    }
                                    catch (Exception exMirrorID)
                                    {
                                        Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { "Error get List ID Mirroring :" + exMirrorID.Message + " | " + exMirrorID.StackTrace });
                                    }
                                }
                                else
                                {
                                    //pupolatebox -> no data to process
                                    Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { "No data to process" });
                                }
                            }
                            catch (Exception exc)
                            {
                                Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { "Error : " + exc.Message + " "+ exc.StackTrace  });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        
                        Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { "Error : " + ex.Message + " " + ex.StackTrace });
                    }
                    addRunTodayRepeateValue(appName, appFunction, ref oSwiftTracer_Mirroring);
                }
                else if (validTime(checkWeekend_Mirroring, timeStart_Mirroring, timeStop_Mirroring) == false)
                {
                    Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { appFunction + " Cannot start at current days & time..." });
                }
                else if (isValidRepeatLimit(appName, appFunction, ref oSwiftTracer_Mirroring) == false)
                {
                    Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { appFunction + " Maximum Run Repeat Reached..." });
                }
                else
                {
                    Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { appFunction + " Out of Time..." });
                }
            }
        }

        public string printNames(string paramMT)
        {
            string tag21 = "";
            string tempTag21 = "";
            int i = 0;

            while (true)
            {
                int found = paramMT.IndexOf(":21:", i);
                
                if (found == -1)
                {
                    break;
                }
                int start = found + 4;// start actual name
                int end = paramMT.IndexOf(":", start); // tag21 not always 16 digit
                int range = end - start;

                tempTag21 = paramMT.Substring(start, range);
                
                tag21 = tempTag21.Replace("\r\n", "");
                i = end + 1; // advance 1 to next iteration
            }
            return tag21;
        }
        #endregion


        #region Mapping
        public void onTick_Mapping(Object s, EventArgs ae)
        {
            if (bgws_Mapping.Count == 0)
            {
                for (int i = 0; i < maxThread_Mapping; i++)
                {
                    bgws_Mapping.Add(new BackgroundWorker());
                    bgws_Mapping[i].DoWork += DoWork_Mapping;
                    bgws_Mapping[i].RunWorkerCompleted += bgws_Completed_Mapping;
                    bgws_Mapping[i].WorkerSupportsCancellation = true;
                    bgws_Mapping[i].RunWorkerAsync(i);
                }
            }
        }
        public void bgws_Completed_Mapping(object sender, RunWorkerCompletedEventArgs e)
        {
            String message = "";
            int i = bgws_Mapping.IndexOf((BackgroundWorker)sender);

            if (e.Cancelled == true)
            {
                cancelThread_Mapping += 1;
                bgws_Mapping.RemoveAt(1);
                if (cancelThread_Mapping + runningThread_Mapping + 1 == maxThread_Mapping)
                {
                    message += "Thread Cancelled";
                }

            }
            else if (!(e.Error == null))
            {
                message = "Thread Error: " + e.Error.Message;
            }
            else
            {
                bgws_Mapping.RemoveAt(i);
                message = "Worker " + treatFunctionMapping + " [" + runningThread_Mapping + "] Done";
                runningThread_Mapping += 1;
                if (bgws_Mapping.Count == 0)
                {
                    message += (Environment.NewLine + "***   Thread Success!!!   ***" + Environment.NewLine);
                    runningThread_Mapping = 0;
                }
            }

            if (message != "")
            {
                Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { message });
            }
        }

        public void DoWork_Mapping(Object sender, DoWorkEventArgs e)
        {
            BackgroundWorker workerThread = sender as BackgroundWorker;
            int tred = (int)e.Argument;

            if (workerThread.CancellationPending == true)
            {
                e.Cancel = true;
            }
            else
            {
                string appName = oConfig.getParameter("APPName");
                string appFunction = oConfig.getParameter("APPFunction_Mapping");
                treatFunctionMapping = appFunction;
                string tellerID = oConfig.getParameter("TellerID");
                string branch = oConfig.getParameter("Branch");
                DateTime currdate = DateTime.Now;
                int configRD = Int16.Parse(oConfig.getParameter("DateRange"));

                bool updated = false;
                
                if (validTime(checkWeekend_Mapping, timeStart_Mapping, timeStop_Mapping) && isValidRepeatLimit(appName, appFunction, ref oSwiftTracer_Mapping))
                {
                    //show console log
                    Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { appFunction + " Start..." });

                    oData.updateSchRunning(appName, appFunction);

                    //int pstatus = 0;
                    bool insertStatus = false;
                        
                    try
                    {
                        
                        DataTable dataMtTracer = oData.fetchMtTracerData(maxThread_Mapping, tred);

                        for (int i = 0; i < dataMtTracer.Rows.Count; i++)
                        {
                            string valTag21 = dataMtTracer.Rows[i]["Tag21"].ToString();
                            string valTag20Ori = dataMtTracer.Rows[i]["RefNo"].ToString();
                            string valOri_103 = "";
                            string valMaker = dataMtTracer.Rows[i]["Maker"].ToString();

                            string valSplitter_row_id = "";
                            string valMT_ORI = dataMtTracer.Rows[i]["MessageType"].ToString();
                            string valLoadDateOri = dataMtTracer.Rows[i]["loadDate"].ToString();
                            string valTypeOri = dataMtTracer.Rows[i]["Type"].ToString();

                            string valInOutCapture = dataMtTracer.Rows[i]["InOu"].ToString();

                            //int valStatus = Int32.Parse(dataMtTracer.Rows[i]["STATUS"].ToString());
                            string valStatus = dataMtTracer.Rows[i]["STATUS"].ToString();

                            string valRowIdMtTracer = dataMtTracer.Rows[i]["RowID"].ToString();

                            try
                            {
                                valSplitter_row_id = dataMtTracer.Rows[i]["splitter_row_id"].ToString();
                            }
                            catch (Exception xx)
                            {
                                valSplitter_row_id = "";
                            }

                            if (valSplitter_row_id == "" || valSplitter_row_id == null)
                            {
                                valSplitter_row_id = "O" + dataMtTracer.Rows[i]["RowID"].ToString();
                            }

                            if (!string.IsNullOrEmpty(valTag21) || valTag21 != null)
                            {
                                List<Data_MT> detail_data_103 = new List<Data_MT>();
                                detail_data_103 = Cek103(valTag21);

                                if (detail_data_103.Count > 0)
                                {
                                    Dictionary<string, string> wsAdapterResult = new Dictionary<string, string>();
                                    wsAdapterResult.Clear(); //reset

                                    if (detail_data_103[0].getSource_base() == "IRN" || detail_data_103[0].getSource_base() == "IRK")
                                    {
                                        wsAdapterResult = this.GetWSRespond(detail_data_103[0].getReff(), detail_data_103[0].getSource_base());
                                    }

                                    #region Dictionary data_mt_tracer
                                    Dictionary<string, string> data_mt_tracer = new Dictionary<string, string>();
                                    data_mt_tracer.Clear();
                                    

                                    //ini var di luar tampung buat insert
                                    data_mt_tracer.Add("valTag20Ori", valTag20Ori);
                                    data_mt_tracer.Add("valSplitter_row_id", valSplitter_row_id);
                                    data_mt_tracer.Add("valMT_ORI", valMT_ORI);
                                    data_mt_tracer.Add("valLoadDateOri", valLoadDateOri);
                                    data_mt_tracer.Add("valTypeOri", valTypeOri);
                                    data_mt_tracer.Add("valInOutCapture", valInOutCapture);

                                    //status awal saat di fecth data
                                    data_mt_tracer.Add("valStatus", valStatus);
                                    data_mt_tracer.Add("valMaker", valMaker);

                                    //GetTargetDate 
                                    string valTargetDate = oData.getTDate("target_date");
                                    data_mt_tracer.Add("valTargetDate", valTargetDate);
                                    data_mt_tracer.Add("valRowIdMtTracer", valRowIdMtTracer);
                                    #endregion

                                    insertStatus = this.IsMainProcess(data_mt_tracer, detail_data_103, wsAdapterResult);
                                    //invoke di dalam main process

                                }
                                else
                                {
                                    List<Data_MT> tampung_non103 = new List<Data_MT>();
                                    tampung_non103 = CekMT(valTag21);

                                    if (tampung_non103.Count > 0)
                                    {
                                        //ntar krn ngak dapet data
                                        string message = tampung_non103[0].getMT();
                                        string tag = printNames(tampung_non103[0].getMT()).Replace("\\r|\\n", "");

                                        if (tag != "")
                                        {
                                            Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { " Bukan MT103" });

                                            List<Data_MT> tampung_non = new List<Data_MT>();
                                            tampung_non = Cek103(tag);

                                            if (tampung_non.Count > 0)
                                            {
                                                Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { " Parent MT103" });

                                                Dictionary<string, string> wsAdapterResult = new Dictionary<string, string>();
                                                wsAdapterResult.Clear();
                                                if (tampung_non[0].getSource_base() == "IRN" || tampung_non[0].getSource_base() == "IRK")
                                                {
                                                    wsAdapterResult = this.GetWSRespond(tampung_non[0].getReff(), tampung_non[0].getSource_base());
                                                }

                                                #region Dictionary data_mt_tracer
                                                Dictionary<string, string> data_mt_tracer = new Dictionary<string, string>();
                                                data_mt_tracer.Clear();
                                                //ini var di luar tampung buat insert
                                                data_mt_tracer.Add("valTag20Ori", valTag20Ori);
                                                data_mt_tracer.Add("valSplitter_row_id", valSplitter_row_id);
                                                data_mt_tracer.Add("valMT_ORI", valMT_ORI);
                                                data_mt_tracer.Add("valLoadDateOri", valLoadDateOri);
                                                data_mt_tracer.Add("valTypeOri", valTypeOri);
                                                data_mt_tracer.Add("valInOutCapture", valInOutCapture);

                                                //status awal saat di fecth data
                                                data_mt_tracer.Add("valStatus", valStatus);
                                                data_mt_tracer.Add("valMaker", valMaker);

                                                //GetTargetDate 
                                                string valTargetDate = oData.getTDate("target_date");
                                                data_mt_tracer.Add("valTargetDate", valTargetDate);
                                                data_mt_tracer.Add("valRowIdMtTracer", valRowIdMtTracer);
                                                #endregion


                                                insertStatus = this.IsMainProcessNested(data_mt_tracer, tampung_non, wsAdapterResult, tampung_non103);
                                                
                                            }

                                            else
                                            {
                                                Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { " Parent Bukan 103" });

                                                List<Data_MT> tampungcari103 = new List<Data_MT>();

                                                tampungcari103 = FindMT103(tag.Replace("\\r|\\n", ""), 0);

                                                insertStatus = false;

                                                
                                                if (tampungcari103 != null)
                                                {
                                                    if (tampungcari103.Count > 0)
                                                    {
                                                        #region enhance after cek103
                                                        Dictionary<string, string> wsAdapterResult = new Dictionary<string, string>();
                                                        wsAdapterResult.Clear(); //reset

                                                        if (tampungcari103[0].getSource_base() == "IRN" || tampungcari103[0].getSource_base() == "IRK")
                                                        {
                                                            wsAdapterResult = this.GetWSRespond(tampungcari103[0].getReff(), tampungcari103[0].getSource_base());
                                                        }

                                                        Dictionary<string, string> data_mt_tracer = new Dictionary<string, string>();
                                                        data_mt_tracer.Clear();

                                                        //ini var di luar tampung buat insert
                                                        data_mt_tracer.Add("valTag20Ori", valTag20Ori);
                                                        data_mt_tracer.Add("valSplitter_row_id", valSplitter_row_id);
                                                        data_mt_tracer.Add("valMT_ORI", valMT_ORI);
                                                        data_mt_tracer.Add("valLoadDateOri", valLoadDateOri);
                                                        data_mt_tracer.Add("valTypeOri", valTypeOri);
                                                        data_mt_tracer.Add("valInOutCapture", valInOutCapture);

                                                        //status awal saat di fecth data
                                                        data_mt_tracer.Add("valStatus", valStatus);
                                                        data_mt_tracer.Add("valMaker", valMaker);

                                                        //GetTargetDate 
                                                        string valTargetDate = oData.getTDate("target_date");
                                                        data_mt_tracer.Add("valTargetDate", valTargetDate);
                                                        data_mt_tracer.Add("valRowIdMtTracer", valRowIdMtTracer);
                                                        #endregion

                                                        insertStatus = this.IsMainProcess(data_mt_tracer, tampungcari103, wsAdapterResult);

                                                    }
                                                    else 
                                                    {
                                                        oData.updateStatus(valTag20Ori, 2, 0, "", "", "", "", "", 0, "", "", "", valSplitter_row_id);
                                                    }
                                                }
                                                else
                                                {
                                                    oData.updateStatus(valTag20Ori, 2, 0, "", "", "", "", "", 0, "", "", "", valSplitter_row_id);
                                                }
                                                
                                            }
                                        }
                                        else
                                        {
                                            oData.updateStatus(valTag20Ori, 2, 0, "", "", "", "", "", 0, "", "", "", valSplitter_row_id);
                                        }


                                    }
                                    else
                                    {
                                        List<Data_Map> tampung_map = new List<Data_Map>();
                                        tampung_map = Cek_Mapping(valTag21);

                                        if (tampung_map.Count > 0)
                                        {
                                            insertStatus = oData.insertDBMapping(
                                                valTag20Ori, tampung_map[0].getREF_TO_CAPTURE(), tampung_map[0].getORI_MT103_REF(),
                                                0, tampung_map[0].getInOut(), tampung_map[0].getCorrespondent(),
                                                tampung_map[0].getCurrency(), tampung_map[0].getTag20(), tampung_map[0].getMt_103(),
                                                tampung_map[0].getAmount(), "", "",
                                                "", valSplitter_row_id, "0",
                                                "0", valMT_ORI, "",
                                                valLoadDateOri, tampung_map[0].getLoadDateToCapture(), tampung_map[0].getLoadDateOri(),
                                                valTypeOri, tampung_map[0].getTypeCapture(), tampung_map[0].getTypeOri(),
                                                tampung_map[0].getInOutPrev(), tampung_map[0].getInOutOri(), valInOutCapture
                                                );

                                            if (insertStatus == true)
                                            {
                                                oData.CheckSumberMT(valTag20Ori, tampung_map[0].getTag20(), valSplitter_row_id);
                                                if (string.IsNullOrEmpty(valMaker))
                                                {
                                                    oData.updateStatus(
                                                        valTag20Ori, 22, tampung_map[0].getIsKerjasama(),
                                                        tampung_map[0].getInOut(), tampung_map[0].getCorrespondent(), tampung_map[0].getCurrency(),
                                                        tampung_map[0].getTag20(), tampung_map[0].getMt_103(), tampung_map[0].getAmount(),
                                                        tampung_map[0].getAccountnum(), tampung_map[0].getAccountname(), tampung_map[0].getTanggal(),
                                                        valSplitter_row_id
                                                        );
                                                }
                                                else
                                                {
                                                    oData.updateStatus(
                                                        valTag20Ori, 6, tampung_map[0].getIsKerjasama(),
                                                        tampung_map[0].getInOut(), tampung_map[0].getCorrespondent(), tampung_map[0].getCurrency(),
                                                        tampung_map[0].getTag20(), tampung_map[0].getMt_103(), tampung_map[0].getAmount(),
                                                        tampung_map[0].getAccountnum(), tampung_map[0].getAccountname(), tampung_map[0].getTanggal(),
                                                        valSplitter_row_id
                                                        );
                                                }
                                            }
                                            else
                                            {
                                                oData.updateStatus(valTag20Ori, 2, 0, "", "", "", "", "", 0, "", "", "", valSplitter_row_id);
                                            }
                                        }
                                        else
                                        {
                                            oData.updateStatus(valTag20Ori, 2, 0, "", "", "", "", "", 0, "", "", "", valSplitter_row_id);
                                        }
                                    }
                                }
                            }

                            else
                            {
                                oData.updateStatus(valTag20Ori, 2, 0, "", "", "", "", "", 0, "", "", "", valSplitter_row_id);
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { " Find 103 error : " + ex.Message + ex.StackTrace });
                        oData.insertLog("0","", "Scheduler Swift tracer", " Find 103 error : " + ex.Message + ex.StackTrace, appFunction, "Mapping");
                    }
                }
                else if (validTime(checkWeekend_Mapping, timeStart_Mapping, timeStop_Mapping) == false)
                {
                    Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { appFunction + " Cannot start at current days & time..." });
                }
                else if (isValidRepeatLimit(appName, appFunction, ref oSwiftTracer_Mapping) == false)
                {
                    Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { appFunction + " Maximum Run Repeat Reached..." });
                }
                else
                {
                    Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { appFunction + " Out of Time..." });
                }
            }
        }

        public Dictionary<string, string> GetWSRespond(string pRefno, string pSource_base)
        {
            WebServicesMapping WSMapping = new WebServicesMapping();
            WebServiceRequests WSRequest = new WebServiceRequests();
            WebServiceResponds WSRespond = new WebServiceResponds();
            Dictionary<string, string> wsAdapterResult = new Dictionary<string, string>();
            wsAdapterResult.Clear();//reset

            string tempUsername = oData.getUsername("adapterws_user");
            string tempKataKunci = oData.getWSKatakunci("adapterws_password");
            string WSAddress = oData.getWSAddress("adapterws_url_GetDetailTransaction");

            string decUser = Enkripsi.StringCipher.Decrypt(tempUsername);
            string decKataKunci = Enkripsi.StringCipher.Decrypt(tempKataKunci);

            WSRequest.username = decUser;

            WSRequest.gembok = decKataKunci;
            WSRequest.refno = pRefno; //param = tag21 dari atas
            WSRequest.transactionSource = pSource_base;


            WSRespond = WSMapping.SendDataDetail(WSRequest, WSAddress);
            if (WSRespond.statusCode == "0001") // Success
            {
                #region Dictionary wsAdapterResult

                wsAdapterResult.Add("statusCode", WSRespond.statusCode);
                wsAdapterResult.Add("statusDesc", WSRespond.statusDesc);
                wsAdapterResult.Add("totalData", WSRespond.totaldata);


                wsAdapterResult.Add("SenderBIC", WSRespond.transactiondata[0].SenderBIC);
                wsAdapterResult.Add("ReceiverBIC", WSRespond.transactiondata[0].ReceiverBIC);
                wsAdapterResult.Add("NewSeqno", WSRespond.transactiondata[0].NewSeqno);
                wsAdapterResult.Add("RowID", WSRespond.transactiondata[0].RowID);

                wsAdapterResult.Add("TrxAmount", WSRespond.transactiondata[0].TrxAmount);
                wsAdapterResult.Add("TrxCurrency", WSRespond.transactiondata[0].TrxCurrency);
                wsAdapterResult.Add("TrxDate", WSRespond.transactiondata[0].TrxDate);
                wsAdapterResult.Add("BenefAccount", WSRespond.transactiondata[0].BenefAccount);

                wsAdapterResult.Add("BenefName", WSRespond.transactiondata[0].BenefName);
                wsAdapterResult.Add("ProcessId", WSRespond.transactiondata[0].ProcessId);
                wsAdapterResult.Add("MTtype", WSRespond.transactiondata[0].MTtype);
                wsAdapterResult.Add("TAG20_101", WSRespond.transactiondata[0].TAG20_101);

                wsAdapterResult.Add("TAG20", WSRespond.transactiondata[0].TAG20);
                wsAdapterResult.Add("Description", WSRespond.transactiondata[0].Description);
                wsAdapterResult.Add("BrinetFullName", WSRespond.transactiondata[0].BrinetFullName);
                wsAdapterResult.Add("BrinetName", WSRespond.transactiondata[0].BrinetName);

                wsAdapterResult.Add("FullMT", WSRespond.transactiondata[0].FullMT);
                wsAdapterResult.Add("Depcor", WSRespond.transactiondata[0].Depcor);
                wsAdapterResult.Add("Tag53", WSRespond.transactiondata[0].Tag53);
                wsAdapterResult.Add("Tag54", WSRespond.transactiondata[0].Tag54);

                wsAdapterResult.Add("TrxSource", WSRespond.transactiondata[0].TrxSource);
                wsAdapterResult.Add("CancelMTType", WSRespond.transactiondata[0].CancelMTType);
                wsAdapterResult.Add("CancelFeeAmount", WSRespond.transactiondata[0].CancelFeeAmount);
                wsAdapterResult.Add("CancelFeeCurrency", WSRespond.transactiondata[0].CancelFeeCurrency);

                wsAdapterResult.Add("SenderBicIsDepcor", WSRespond.transactiondata[0].SenderBicIsDepcor);
                wsAdapterResult.Add("Tag56", WSRespond.transactiondata[0].Tag56);

                wsAdapterResult.Add("SwiftAdapterStatus", WSRespond.transactiondata[0].SwiftAdapterStatus);
                wsAdapterResult.Add("TrxType", WSRespond.transactiondata[0].TrxType);
                wsAdapterResult.Add("NostroAccount", WSRespond.transactiondata[0].NostroAccount);
                #endregion
            }
            else if (WSRespond.statusCode == "404") //NotFound 
            {
                Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { "Cannot find detail IRK/IRN reff " + pRefno + " with result" + WSRespond.statusDesc + " " });
            }
            else //Failed etc
            {
                Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { "Cannot find detail IRK/IRN reff " + pRefno + " with result" + WSRespond.statusDesc + " " });
            }
            return wsAdapterResult;
        }

        public bool IsMainProcess(Dictionary<string,string> pDataMt_tracer,List<Data_MT> pDetail_data_103, Dictionary<string,string> pWSAdapterResult)
        {
            bool result = false;

            #region Auto Close
            List<string> list_row_id = new List<string>();

            list_row_id = oData.getListRefToCapture(pDetail_data_103[0].getTag20());
            string Joined_row_id = string.Join(",", list_row_id);

            List<string> list_status_id = new List<string>();
            list_status_id = oData.getStatuId();
            string Joined_status_id = string.Join(",", list_status_id);
            #endregion

            #region Dictionary data_mt_tracer
            Dictionary<string, string> data_mt_tracer = new Dictionary<string, string>();
            data_mt_tracer.Add("Joined_row_id", Joined_row_id);
            data_mt_tracer.Add("Joined_status_id", Joined_status_id);

            //ini var di luar tampung buat insert
            data_mt_tracer.Add("valTag20Ori", pDataMt_tracer["valTag20Ori"]);
            data_mt_tracer.Add("valSplitter_row_id", pDataMt_tracer["valSplitter_row_id"]);
            data_mt_tracer.Add("valMT_ORI", pDataMt_tracer["valMT_ORI"]);
            data_mt_tracer.Add("valLoadDateOri", pDataMt_tracer["valLoadDateOri"]);
            data_mt_tracer.Add("valTypeOri", pDataMt_tracer["valTypeOri"]);
            data_mt_tracer.Add("valInOutCapture", pDataMt_tracer["valInOutCapture"]);

            //status awal saat di fecth data
            data_mt_tracer.Add("valStatus", pDataMt_tracer["valStatus"]);
            data_mt_tracer.Add("valMaker", pDataMt_tracer["valMaker"]);

            //GetTargetDate 
            string valTargetDate = oData.getTDate("target_date");
            data_mt_tracer.Add("valTargetDate", valTargetDate);
            data_mt_tracer.Add("valRowIdMtTracer", pDataMt_tracer["valRowIdMtTracer"]);
            #endregion

            if (Int32.Parse(pDataMt_tracer["valStatus"]) == 0)
            {
                if (string.IsNullOrEmpty(pDataMt_tracer["valMaker"]))
                {
                    data_mt_tracer.Add("StatusMtTracer", "4");
                    result = oData.isInsertDataMappingAutoclose(data_mt_tracer, pDetail_data_103, pWSAdapterResult);//insert data mapping auto close

                    if (result == false)
                    {
                        Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { " Process " + pDataMt_tracer["valTag20Ori"] + " Failed Execute Query!!! " });
                    }
                    else
                    {
                        Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { " Process " + pDataMt_tracer["valTag20Ori"] + " Success Execute Query!!! " });
                    }
                }
                else
                {
                    data_mt_tracer.Add("StatusMtTracer", "6");
                    result = oData.isInsertDataMappingAutoclose(data_mt_tracer, pDetail_data_103, pWSAdapterResult);//insert data mapping auto close
                    if (result == false)
                    {
                        Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { " Process " + pDataMt_tracer["valTag20Ori"] + " Failed Execute Query!!! " });
                    }
                    else
                    {
                        Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { " Process " + pDataMt_tracer["valTag20Ori"] + " Success Execute Query!!! " });
                    }
                }
            }
            else if (Int32.Parse(pDataMt_tracer["valStatus"]) == 80)
            {
                data_mt_tracer.Add("StatusMtTracer", "90");
                result = oData.isInsertDataMappingAutoclose(data_mt_tracer, pDetail_data_103, pWSAdapterResult);//insert data mapping auto close
                if (result == false)
                {
                    Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { " Process " + pDataMt_tracer["valTag20Ori"] + " Failed Execute Query!!! " });
                }
                else
                {
                    Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { " Process " + pDataMt_tracer["valTag20Ori"] + " Success Execute Query!!! " });
                }
            }
            else
            {

            }
            return result;
        }

        public bool IsMainProcessNested(Dictionary<string, string> pDataMt_tracer, 
            List<Data_MT> pDetail_data_103, 
            Dictionary<string, string> pWSAdapterResult, 
            List<Data_MT> pDetail_data_non_103)
        {
            bool result = false;

            #region Auto Close
            List<string> list_row_id = new List<string>();

            list_row_id = oData.getListRefToCapture(pDetail_data_103[0].getTag20());
            string Joined_row_id = string.Join(",", list_row_id);

            List<string> list_status_id = new List<string>();
            list_status_id = oData.getStatuId();
            string Joined_status_id = string.Join(",", list_status_id);
            #endregion

            #region Dictionary data_mt_tracer
            Dictionary<string, string> data_mt_tracer = new Dictionary<string, string>();
            data_mt_tracer.Add("Joined_row_id", Joined_row_id);
            data_mt_tracer.Add("Joined_status_id", Joined_status_id);

            //ini var di luar tampung buat insert
            data_mt_tracer.Add("valTag20Ori", pDataMt_tracer["valTag20Ori"]);
            data_mt_tracer.Add("valSplitter_row_id", pDataMt_tracer["valSplitter_row_id"]);
            data_mt_tracer.Add("valMT_ORI", pDataMt_tracer["valMT_ORI"]);
            data_mt_tracer.Add("valLoadDateOri", pDataMt_tracer["valLoadDateOri"]);
            data_mt_tracer.Add("valTypeOri", pDataMt_tracer["valTypeOri"]);
            data_mt_tracer.Add("valInOutCapture", pDataMt_tracer["valInOutCapture"]);

            //status awal saat di fecth data
            data_mt_tracer.Add("valStatus", pDataMt_tracer["valStatus"]);
            data_mt_tracer.Add("valMaker", pDataMt_tracer["valMaker"]);

            //GetTargetDate 
            string valTargetDate = oData.getTDate("target_date");
            data_mt_tracer.Add("valTargetDate", valTargetDate);
            data_mt_tracer.Add("valRowIdMtTracer", pDataMt_tracer["valRowIdMtTracer"]);
            #endregion

            if (Int32.Parse(pDataMt_tracer["valStatus"]) == 0)
            {
                if (string.IsNullOrEmpty(pDataMt_tracer["valMaker"]))
                {
                    data_mt_tracer.Add("StatusMtTracer", "4");
 
                    result = oData.isInsertDataMappingAutocloseNested(data_mt_tracer, pDetail_data_103, pWSAdapterResult, pDetail_data_non_103);

                    if (result == false)
                    {
                        Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { " Process " + pDataMt_tracer["valTag20Ori"] + " Failed Execute Query!!! " });
                    }
                    else
                    {
                        Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { " Process " + pDataMt_tracer["valTag20Ori"] + " Success Execute Query!!! " });
                    }
                }
                else
                {
                    data_mt_tracer.Add("StatusMtTracer", "6");

                    result = oData.isInsertDataMappingAutocloseNested(data_mt_tracer, pDetail_data_103, pWSAdapterResult, pDetail_data_non_103);
                    if (result == false)
                    {

                        Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { " Process " + pDataMt_tracer["valTag20Ori"] + " Failed Execute Query!!! " });
                    }
                    else
                    {
                        Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { " Process " + pDataMt_tracer["valTag20Ori"] + " Success Execute Query!!! " });
                    }
                }
            }
            else if (Int32.Parse(pDataMt_tracer["valStatus"]) == 80)
            {
                data_mt_tracer.Add("StatusMtTracer", "90");
                
                result = oData.isInsertDataMappingAutocloseNested(data_mt_tracer, pDetail_data_103, pWSAdapterResult, pDetail_data_non_103);
                if (result == false)
                {
                    Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { " Process " + pDataMt_tracer["valTag20Ori"] + " Failed Execute Query!!! " });
                }
                else
                {
                    Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { " Process " + pDataMt_tracer["valTag20Ori"] + " Success Execute Query!!! " });
                }
            }
            else
            {

            }
            return result;
        }

        public List<Data_MT> Cek103(string pTag21)
        {
            //List<String> tampung = new List<string>();
            List<Data_MT> tampung = new List<Data_MT>();

            string RefNo = "";

            string source_base = "";

            try
            {
                string param = pTag21.Replace("\r|\n", "");
                DataTable DataJoined =  oDataSqlSrv.FetchJoinData(param);

                for (int i = 0; i < DataJoined.Rows.Count; i++)
                {
                    RefNo = DataJoined.Rows[i]["RefNo"].ToString();

                    if (RefNo != "")
                    {
                        string valType = "103";
                        string valMT = DataJoined.Rows[i]["MT"].ToString();

                        int valIsKerjasama = Convert.ToInt32(DataJoined.Rows[i]["ISKERJASAMA"]);

                        string valInOut = DataJoined.Rows[i]["InOut"].ToString();
                        string valCorrespondent = DataJoined.Rows[i]["Correspondent"].ToString();
                        string valCurrency = "";
                        string valtag20 = DataJoined.Rows[i]["RefNo"].ToString();
                        string valMT_103 = DataJoined.Rows[i]["MT"].ToString();
                        string valRow_id = DataJoined.Rows[i]["RowID"].ToString();
                        string valLoadDate = DataJoined.Rows[i]["loadDate"].ToString();
                        string type = DataJoined.Rows[i]["Type"].ToString();

                        string separator = "\r\n";
                        //string newLine = System.getProperty("line.spearator");

                        string[] lines;
                        string accountnum = "";
                        string accountname = "";
                        string tanggal = "";
                        string amount_103 = "0";

                        try
                        {
                            lines = valMT_103.Split(new string[] { "\r\n"}, StringSplitOptions.None);

                            for (int j = 0; j < lines.Length; j++)
                            {
                                if (lines[j].StartsWith(":59:"))
                                {
                                    accountnum = lines[j].Substring(4).Replace("/", "").Replace(" ", "").Replace("-", "");
                                    accountname = lines[j + 1];
                                }

                                if (lines[j].StartsWith(":32A:"))
                                {

                                    tanggal = lines[j].Substring(5, 6);

                                    valCurrency = lines[j].Substring(11, 3);
                                    amount_103 = lines[j].Substring(14);
                                }


                            }
                        }
                        catch (Exception exmt103)
                        {
                            Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { "Error : " + exmt103.Message + " " + exmt103.StackTrace });
                        }

                        float amount = float.Parse(amount_103.Replace(",","."));

                        //update 20210201
                        if (valInOut == "O")
                        {
                            source_base = "OUT";
                        }
                        else if (valInOut == "I" && valIsKerjasama == 1)
                        {
                            source_base = "IRK";
                        }
                        else if (valInOut == "I" && valIsKerjasama == 0)
                        {
                            source_base = "IRN";
                        }
                        else
                        {
                            source_base = "OTH";
                        }

                        try
                        {
                            Data_MT dd;
                            Data_MT data = new Data_MT(valType, valMT, RefNo, valIsKerjasama, valInOut,
                                valCorrespondent, valCurrency, valtag20, valMT_103, amount,
                                accountnum, accountname, tanggal, "0", "0",
                                valRow_id, valLoadDate, valType, source_base);
                            tampung.Add(data);
                        }
                        catch (Exception exFloat)
                        {
                            Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { "Error : " + exFloat.Message + " | " +exFloat.StackTrace });
                        
                        }

                    }
                }

            }
            catch (Exception X01)
            { 
            
            }

            return tampung;
        }

        
        public List<Data_MT> CekMT(string pTag21)
        {
            List<Data_MT> tampung = new List<Data_MT>();

            string RefNo = "";
            try
            {
                DataTable dt = oDataSqlSrv.fetchCekMTGudangMT(pTag21);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    RefNo = dt.Rows[i]["RefNo"].ToString();

                    if (RefNo != "")
                    {
                        string Type = dt.Rows[i]["Type"].ToString();
                        string MT = dt.Rows[i]["MT"].ToString();
                        string inout = dt.Rows[i]["InOut"].ToString();
                        string correspondent = dt.Rows[i]["Correspondent"].ToString();

                        string currency = "";
                        string tag20 = dt.Rows[i]["RefNo"].ToString();
                        string mt_103 = dt.Rows[i]["MT"].ToString();
                        string row_id = dt.Rows[i]["RowID"].ToString();
                        string loadDate = dt.Rows[i]["loadDate"].ToString();

                        string type = dt.Rows[i]["Type"].ToString();
                        String accountnum = "";
                        String accountname = "";
                        String tanggal = "";
                        float amount = 0;

                        Data_MT datafinal = new Data_MT(Type, MT, RefNo, 0, inout, correspondent, currency, tag20, mt_103, amount, accountnum, accountname, tanggal, "0", row_id, "0", loadDate, type);
                        tampung.Add(datafinal);

                    }

                }

                
            }
            catch (Exception exCekMT)
            { 
            
            }
            return tampung;
        }

        public List<Data_Map> Cek_Mapping(string pTag21)
        {
            List<Data_Map> tampung = new List<Data_Map>();

            string REF_TO_CAPTURE = "";

            try
            {

                DataTable dataMap = oData.fetchMtMappingData(pTag21);

                for (int i = 0; i < dataMap.Rows.Count; i++)
                {
                    REF_TO_CAPTURE = dataMap.Rows[i]["REF_TO_CAPTURE"].ToString();

                    if (REF_TO_CAPTURE != "")
                    {
                        string PREVIOUS_REV = dataMap.Rows[i]["PREVIOUS_REV"].ToString();
                        string ORI_MT103_REF = dataMap.Rows[i]["ORI_MT103_REF"].ToString();
                        string inout = dataMap.Rows[i]["INOROUT"].ToString();
                        string correspondent = dataMap.Rows[i]["Correspondent"].ToString();
                        int isKerjasama = Int32.Parse(dataMap.Rows[i]["ISKERJASAMA"].ToString());

                        string currency = dataMap.Rows[i]["currency"].ToString();

                        string tag20 = dataMap.Rows[i]["TAG20"].ToString();
                        string mt_103 = dataMap.Rows[i]["mt_103"].ToString();
                        float amount = float.Parse(dataMap.Rows[i]["amount"].ToString());
                        string loadDateCapture = dataMap.Rows[i]["dateCapture"].ToString();
                        string loadDateOri = dataMap.Rows[i]["dateOri"].ToString();
                        string typeCapture = dataMap.Rows[i]["typeCapture"].ToString();

                        string typeOri = dataMap.Rows[i]["typeOri"].ToString();
                        string inOutPrev = dataMap.Rows[i]["InOutPrev"].ToString();
                        string inOutOri = dataMap.Rows[i]["InOutOri"].ToString();

                        string accountnum = "";
                        string accountname = "";
                        string tanggal = "";

                        Data_Map DataMapFinal = new Data_Map(REF_TO_CAPTURE, PREVIOUS_REV, ORI_MT103_REF
                            , isKerjasama, inout, correspondent, currency, tag20
                            , mt_103, amount, accountnum, accountname, tanggal,
                            "0", "0", "0", loadDateCapture, loadDateOri, typeCapture, typeOri, inOutPrev, inOutOri);
                        
                        tampung.Add(DataMapFinal);

                    }
                
                }


            }
            catch (Exception exMap)
            { 
            
            }

            return tampung;
        }

        public List<Data_MT> FindMT103(string codereff, int doLoop)
        {
            try
            {
                if (doLoop < 30)
                {
                    Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { " Find 103 : " + codereff + " Loop : " + doLoop });

                    //insert log

                    string msglog = "Find 103 "+ codereff +" ";

                    try
                    {

                    }
                    catch (Exception ex)
                    {

                    }

                    string TAG21 = codereff;

                    if (!string.IsNullOrEmpty(TAG21) || TAG21 != null)
                    {
                        List<Data_MT> tampung = new List<Data_MT>();

                        tampung = Cek103(TAG21);

                        if (tampung.Count > 0)
                        {
                            Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { " Find 103 : " + codereff + " Merupakan 103 " });
                            //insert log
                            msglog += " Find 103 : " + codereff + " Merupakan 103 ";
                            oData.insertLog("0",codereff,"Scheduler Swift Tracer", msglog, "Swift Tracer Scheduler", "Mapping");

                            return tampung;
                        }
                        else
                        {
                            Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { " Find 103 : " + codereff + " Bukan 103 " });

                            //insert_log
                            msglog += " Find 103 : " + codereff + " Bukan 103 ";

                            List<Data_MT> tampung_non103 = new List<Data_MT>();

                            tampung_non103 = CekMT(TAG21);

                            if (tampung_non103.Count > 0)
                            {
                                Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { " Find 103 : " + codereff + " Bukan 103 cekmt " });

                                //insert_log
                                msglog += " Find 103 : " + codereff + " Bukan 103 cekmt ";

                                string tag1 = printNames(tampung_non103[0].getMT());

                                if (tag1 != "")
                                {
                                    oData.insertLog("0", tag1, "Scheduler Swift Tracer", msglog, "Swift Tracer Scheduler", "Mapping");
                                    return FindMT103(tag1.Replace("\\r|\\n", ""), doLoop + 1);
                                }
                                else
                                {
                                    return null;
                                }

                            }
                            else
                            {
                                List<Data_Map> tampung_map = new List<Data_Map>();

                                tampung_map = Cek_Mapping(TAG21);

                                if (tampung_map.Count > 0)
                                {
                                    return FindMT103(tampung_map[0].getORI_MT103_REF(), doLoop + 1);
                                }
                                else
                                {
                                    return null;
                                }
                            }
                        }

                    }
                    else
                    {
                        return null;
                    }


                }
                else 
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion

        #region SwiftAdapter
        public void onTick_SwiftAdapter(Object s, EventArgs ae)
        {
            if(bgws_SwiftAdapter.Count == 0)
            {
                for (int i = 0; i < maxThread_SwiftAdapter; i++)
                {
                    bgws_SwiftAdapter.Add(new BackgroundWorker());
                    bgws_SwiftAdapter[i].DoWork += DoWork_SwiftAdapter;
                    bgws_SwiftAdapter[i].RunWorkerCompleted += bgws_Completed_SwiftAdapter;
                    bgws_SwiftAdapter[i].WorkerSupportsCancellation = true;
                    bgws_SwiftAdapter[i].RunWorkerAsync(i);
                }
            }
        }
        public void bgws_Completed_SwiftAdapter(object sender, RunWorkerCompletedEventArgs e)
        {
            String message = "";
            int i = bgws_SwiftAdapter.IndexOf((BackgroundWorker)sender);

            if (e.Cancelled == true)
            {
                cancelThread_SwiftAdapter += 1;
                bgws_SwiftAdapter.RemoveAt(1);
                if (cancelThread_SwiftAdapter + runningThread_SwiftAdapter + 1 == maxThread_SwiftAdapter)
                {
                    message += "Thread Cancelled";
                }

            }
            else if (!(e.Error == null))
            {
                message = "Thread Error: " + e.Error.Message;
            }
            else
            {
                bgws_SwiftAdapter.RemoveAt(i);
                message = "Worker " + treatFunctionSwiftAdapter + " [" + runningThread_SwiftAdapter + "] Done";
                runningThread_SwiftAdapter += 1;
                if (bgws_SwiftAdapter.Count == 0)
                {
                    message += (Environment.NewLine + "***   Thread Success!!!   ***" + Environment.NewLine);
                    runningThread_SwiftAdapter = 0;
                }
            }

            if (message != "")
            {
                Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { message });
            }
        }
        public void DoWork_SwiftAdapter(Object sender, DoWorkEventArgs e)
        {
            BackgroundWorker workerThread = sender as BackgroundWorker;
            int tred = (int)e.Argument;

            if (workerThread.CancellationPending == true)
            {
                e.Cancel = true;
            }
            else
            {

                string appName = oConfig.getParameter("APPName");
                string appFunction = oConfig.getParameter("APPFunction_SwiftAdapter");
                treatFunctionSwiftAdapter = appFunction;
                string tellerID = oConfig.getParameter("TellerID");
                string branch = oConfig.getParameter("Branch");
                DateTime currdate = DateTime.Now;
                int configRD = Int16.Parse(oConfig.getParameter("DateRange"));

                bool hasilSwift = false;
                bool hasilSwiftIRN = false;
                bool Checker_status = false;
                bool Checker_statusIRN = false;

                if (validTime(checkWeekend_SwiftAdapter, timeStart_SwiftAdapter, timeStop_SwiftAdapter) && isValidRepeatLimit(appName, appFunction, ref oSwiftTracer_SwiftAdapter))
                {
                    //show console log
                    Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { appFunction + " Start..." });

                    oData.updateSchRunning(appName, appFunction);

                    try
                    {
                        #region Bagian IRK
                        WebServicesSwiftAdapter WSSwiftAdapter = new WebServicesSwiftAdapter();
                        WebServiceRequestSwiftAd WSReqSwiftAdapter = new WebServiceRequestSwiftAd();
                        WebServiceRespondsSwiftAd WSRespondSwiftAdapter = new WebServiceRespondsSwiftAd();



                        
                        string tempUsername = oData.getUsername("adapterws_user");
                        string tempKataKunci = oData.getWSKatakunci("adapterws_password");
                        string tempTypeTransSource = "IRK";
                        string statusKerjasama = "1";
                        string WSAddress = oData.getWSAddress("adapterws_url_GetDataPendingMt199");

                        string decUser = Enkripsi.StringCipher.Decrypt(tempUsername);
                        string decKataKunci = Enkripsi.StringCipher.Decrypt(tempKataKunci);

                        int InsIrkId = 0;
                        WSReqSwiftAdapter.username = decUser;
                        WSReqSwiftAdapter.gembok = decKataKunci;
                        WSReqSwiftAdapter.transactionSource = tempTypeTransSource;

                        WSRespondSwiftAdapter = WSSwiftAdapter.SendDataToSwiftAdapter(WSReqSwiftAdapter, WSAddress);

                        if (WSRespondSwiftAdapter.statusCode == "0001")
                        {
                            #region Dictionary wsAdapterResult


                            List<string> listTag20 = new List<string>();
                            List<string> listTrxdate = new List<string>();
                            for (int i = 0; i < Int32.Parse(WSRespondSwiftAdapter.totaldata); i++)
                            {
                                Checker_status = oData.isExistSwiftAdapter(WSRespondSwiftAdapter.transactiondataSwift[i].TAG20, WSRespondSwiftAdapter.transactiondataSwift[i].TrxDate);

                                if (Checker_status == false)
                                {
                                    Dictionary<string, string> wsAdapterResultSwift = new Dictionary<string, string>();
                                    wsAdapterResultSwift.Clear();//reset

                                    wsAdapterResultSwift.Add("SenderBIC", WSRespondSwiftAdapter.transactiondataSwift[i].SenderBIC);
                                    wsAdapterResultSwift.Add("ReceiverBIC", WSRespondSwiftAdapter.transactiondataSwift[i].ReceiverBIC);
                                    wsAdapterResultSwift.Add("NewSeqno", WSRespondSwiftAdapter.transactiondataSwift[i].NewSeqno);
                                    wsAdapterResultSwift.Add("RowID", WSRespondSwiftAdapter.transactiondataSwift[i].RowID);

                                    wsAdapterResultSwift.Add("TrxAmount", WSRespondSwiftAdapter.transactiondataSwift[i].TrxAmount);
                                    wsAdapterResultSwift.Add("TrxCurrency", WSRespondSwiftAdapter.transactiondataSwift[i].TrxCurrency);
                                    wsAdapterResultSwift.Add("TrxDate", WSRespondSwiftAdapter.transactiondataSwift[i].TrxDate);
                                    wsAdapterResultSwift.Add("BenefAccount", WSRespondSwiftAdapter.transactiondataSwift[i].BenefAccount);

                                    wsAdapterResultSwift.Add("BenefName", WSRespondSwiftAdapter.transactiondataSwift[i].BenefName);
                                    wsAdapterResultSwift.Add("ProcessId", WSRespondSwiftAdapter.transactiondataSwift[i].ProcessId);
                                    wsAdapterResultSwift.Add("MTtype", WSRespondSwiftAdapter.transactiondataSwift[i].MTtype);
                                    wsAdapterResultSwift.Add("TAG20_101", WSRespondSwiftAdapter.transactiondataSwift[i].TAG20_101);

                                    wsAdapterResultSwift.Add("TAG20", WSRespondSwiftAdapter.transactiondataSwift[i].TAG20);
                                    wsAdapterResultSwift.Add("Description", WSRespondSwiftAdapter.transactiondataSwift[i].Description);
                                    wsAdapterResultSwift.Add("BrinetFullName", WSRespondSwiftAdapter.transactiondataSwift[i].BrinetFullName);
                                    wsAdapterResultSwift.Add("BrinetName", WSRespondSwiftAdapter.transactiondataSwift[i].BrinetName);

                                    wsAdapterResultSwift.Add("FullMT", WSRespondSwiftAdapter.transactiondataSwift[i].FullMT);
                                    wsAdapterResultSwift.Add("Depcor", WSRespondSwiftAdapter.transactiondataSwift[i].Depcor);
                                    wsAdapterResultSwift.Add("Tag53", WSRespondSwiftAdapter.transactiondataSwift[i].Tag53);
                                    wsAdapterResultSwift.Add("Tag54", WSRespondSwiftAdapter.transactiondataSwift[i].Tag54);

                                    wsAdapterResultSwift.Add("TrxSource", WSRespondSwiftAdapter.transactiondataSwift[i].TrxSource);
                                    wsAdapterResultSwift.Add("CancelMTType", WSRespondSwiftAdapter.transactiondataSwift[i].CancelMTType);
                                    wsAdapterResultSwift.Add("CancelFeeAmount", WSRespondSwiftAdapter.transactiondataSwift[i].CancelFeeAmount);
                                    wsAdapterResultSwift.Add("CancelFeeCurrency", WSRespondSwiftAdapter.transactiondataSwift[i].CancelFeeCurrency);

                                    wsAdapterResultSwift.Add("SenderBicIsDepcor", WSRespondSwiftAdapter.transactiondataSwift[i].SenderBicIsDepcor);
                                    wsAdapterResultSwift.Add("Tag56", WSRespondSwiftAdapter.transactiondataSwift[i].Tag56);
                                    wsAdapterResultSwift.Add("SwiftAdapterStatus", WSRespondSwiftAdapter.transactiondataSwift[i].SwiftAdapterStatus);

                                    hasilSwift = oData.isProcessInsertSwiftAdapter(wsAdapterResultSwift, statusKerjasama, ref InsIrkId);


                                    if (hasilSwift == true)
                                    {
                                        //Log
                                        vAppSetting = new AppSettingsReader();

                                        string valUser = "Scheduler Swift tracer";
                                        branchCodeParam = vAppSetting.GetValue("Branch", Type.GetType("System.String")).ToString();

                                        appName = vAppSetting.GetValue("APPName", Type.GetType("System.String")).ToString();

                                        String pMessage = "Process IRK Tag20: " + WSRespondSwiftAdapter.transactiondataSwift[i].TAG20 + " Success!!! ";
                                        String pAppName = appName;
                                        String pAction = "Get Data IRK Swift Adapter";

                                        oData.insertLog(InsIrkId.ToString(), WSRespondSwiftAdapter.transactiondataSwift[i].TAG20, valUser, pMessage, pAppName, pAction);
                                        Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { "Process IRK Tag20: " + WSRespondSwiftAdapter.transactiondataSwift[i].TAG20 + " Success!!! " });
                                    }
                                    else
                                    {
                                        Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { "Process IRK Tag20: " + WSRespondSwiftAdapter.transactiondataSwift[i].TAG20 + " Failed!!! " });
                                    }

                                }
                            }

                            #endregion
                        }
                        else 
                        {
                            Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { "Process Get Swift Adapter IRK Failed!!! " });
                        }
                        #endregion

                        #region Bagian IRN
                        WebServicesSwiftAdapter WSSwiftAdapterIRN = new WebServicesSwiftAdapter();
                        WebServiceRequestSwiftAd WSReqSwiftAdapterIRN = new WebServiceRequestSwiftAd();
                        WebServiceRespondsSwiftAd WSRespondSwiftAdapterIRN = new WebServiceRespondsSwiftAd();

                        string tempUsernameIRN = oData.getUsername("adapterws_user");
                        string tempKataKunciIRN = oData.getWSKatakunci("adapterws_password");
                        string tempTypeTransSourceIRN = "IRN";
                        string statusKerjasamaIRN = "0";
                        string WSAddressIRN = oData.getWSAddress("adapterws_url_GetDataPendingMt199");

                        string decUserIRN = Enkripsi.StringCipher.Decrypt(tempUsernameIRN);
                        string decKataKunciIRN = Enkripsi.StringCipher.Decrypt(tempKataKunciIRN);

                        int InsIrnId = 0;
                        WSReqSwiftAdapterIRN.username = decUserIRN;
                        WSReqSwiftAdapterIRN.gembok = decKataKunciIRN;
                        WSReqSwiftAdapterIRN.transactionSource = tempTypeTransSourceIRN;

                        WSRespondSwiftAdapterIRN = WSSwiftAdapterIRN.SendDataToSwiftAdapter(WSReqSwiftAdapterIRN, WSAddressIRN);

                        if (WSRespondSwiftAdapterIRN.statusCode == "0001")
                        {
                            #region Dictionary wsAdapterResult IRN
                            for (int i = 0; i < Int32.Parse(WSRespondSwiftAdapterIRN.totaldata); i++)
                            {
                                Checker_statusIRN = oData.isExistSwiftAdapter(WSRespondSwiftAdapterIRN.transactiondataSwift[i].TAG20, WSRespondSwiftAdapterIRN.transactiondataSwift[i].TrxDate);

                                if (Checker_statusIRN == false)
                                {
                                    Dictionary<string, string> wsAdapterResultSwiftIRN = new Dictionary<string, string>();
                                    wsAdapterResultSwiftIRN.Clear();//reset

                                    wsAdapterResultSwiftIRN.Add("SenderBIC", WSRespondSwiftAdapterIRN.transactiondataSwift[i].SenderBIC);
                                    wsAdapterResultSwiftIRN.Add("ReceiverBIC", WSRespondSwiftAdapterIRN.transactiondataSwift[i].ReceiverBIC);
                                    wsAdapterResultSwiftIRN.Add("NewSeqno", WSRespondSwiftAdapterIRN.transactiondataSwift[i].NewSeqno);
                                    wsAdapterResultSwiftIRN.Add("RowID", WSRespondSwiftAdapterIRN.transactiondataSwift[i].RowID);

                                    wsAdapterResultSwiftIRN.Add("TrxAmount", WSRespondSwiftAdapterIRN.transactiondataSwift[i].TrxAmount);
                                    wsAdapterResultSwiftIRN.Add("TrxCurrency", WSRespondSwiftAdapterIRN.transactiondataSwift[i].TrxCurrency);
                                    wsAdapterResultSwiftIRN.Add("TrxDate", WSRespondSwiftAdapterIRN.transactiondataSwift[i].TrxDate);
                                    wsAdapterResultSwiftIRN.Add("BenefAccount", WSRespondSwiftAdapterIRN.transactiondataSwift[i].BenefAccount);

                                    wsAdapterResultSwiftIRN.Add("BenefName", WSRespondSwiftAdapterIRN.transactiondataSwift[i].BenefName);
                                    wsAdapterResultSwiftIRN.Add("ProcessId", WSRespondSwiftAdapterIRN.transactiondataSwift[i].ProcessId);
                                    wsAdapterResultSwiftIRN.Add("MTtype", WSRespondSwiftAdapterIRN.transactiondataSwift[i].MTtype);
                                    wsAdapterResultSwiftIRN.Add("TAG20_101", WSRespondSwiftAdapterIRN.transactiondataSwift[i].TAG20_101);

                                    wsAdapterResultSwiftIRN.Add("TAG20", WSRespondSwiftAdapterIRN.transactiondataSwift[i].TAG20);
                                    wsAdapterResultSwiftIRN.Add("Description", WSRespondSwiftAdapterIRN.transactiondataSwift[i].Description);
                                    wsAdapterResultSwiftIRN.Add("BrinetFullName", WSRespondSwiftAdapterIRN.transactiondataSwift[i].BrinetFullName);
                                    wsAdapterResultSwiftIRN.Add("BrinetName", WSRespondSwiftAdapterIRN.transactiondataSwift[i].BrinetName);

                                    wsAdapterResultSwiftIRN.Add("FullMT", WSRespondSwiftAdapterIRN.transactiondataSwift[i].FullMT);
                                    wsAdapterResultSwiftIRN.Add("Depcor", WSRespondSwiftAdapterIRN.transactiondataSwift[i].Depcor);
                                    wsAdapterResultSwiftIRN.Add("Tag53", WSRespondSwiftAdapterIRN.transactiondataSwift[i].Tag53);
                                    wsAdapterResultSwiftIRN.Add("Tag54", WSRespondSwiftAdapterIRN.transactiondataSwift[i].Tag54);

                                    wsAdapterResultSwiftIRN.Add("TrxSource", WSRespondSwiftAdapterIRN.transactiondataSwift[i].TrxSource);
                                    wsAdapterResultSwiftIRN.Add("CancelMTType", WSRespondSwiftAdapterIRN.transactiondataSwift[i].CancelMTType);
                                    wsAdapterResultSwiftIRN.Add("CancelFeeAmount", WSRespondSwiftAdapterIRN.transactiondataSwift[i].CancelFeeAmount);
                                    wsAdapterResultSwiftIRN.Add("CancelFeeCurrency", WSRespondSwiftAdapterIRN.transactiondataSwift[i].CancelFeeCurrency);

                                    wsAdapterResultSwiftIRN.Add("SenderBicIsDepcor", WSRespondSwiftAdapterIRN.transactiondataSwift[i].SenderBicIsDepcor);
                                    wsAdapterResultSwiftIRN.Add("Tag56", WSRespondSwiftAdapterIRN.transactiondataSwift[i].Tag56);
                                    wsAdapterResultSwiftIRN.Add("SwiftAdapterStatus", WSRespondSwiftAdapterIRN.transactiondataSwift[i].SwiftAdapterStatus);


                                    hasilSwiftIRN = oData.isProcessInsertSwiftAdapter(wsAdapterResultSwiftIRN, statusKerjasamaIRN, ref InsIrnId);


                                    if (hasilSwiftIRN == true)
                                    {
                                        //Log
                                        vAppSetting = new AppSettingsReader();

                                        string valUser = "Scheduler Swift tracer";
                                        branchCodeParam = vAppSetting.GetValue("Branch", Type.GetType("System.String")).ToString();

                                        appName = vAppSetting.GetValue("APPName", Type.GetType("System.String")).ToString();

                                        String pMessage = "Process IRN Tag20: " + WSRespondSwiftAdapterIRN.transactiondataSwift[i].TAG20 + " Success!!! ";
                                        String pAppName = appName;
                                        String pAction = "Get Data IRN Swift Adapter";

                                        oData.insertLog(InsIrnId.ToString(), WSRespondSwiftAdapterIRN.transactiondataSwift[i].TAG20, valUser, pMessage, pAppName, pAction);
                                        Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { "Process IRN Tag20: " + WSRespondSwiftAdapterIRN.transactiondataSwift[i].TAG20 + " Success!!! " });
                                    }
                                    else
                                    {
                                        Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { "Process IRN Tag20: " + WSRespondSwiftAdapterIRN.transactiondataSwift[i].TAG20 + " Failed!!! " });
                                    }

                                }
                            }
                            #endregion
                        }
                        else 
                        {
                            Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { "Process Get Swift Adapter IRN Failed!!! " });
                        }
                        #endregion

                    }
                    catch (Exception ex)
                    {
                        
                        Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { "Process Get Swift Adapter Error : " + ex.Message + " " + ex.StackTrace });
                    }
                    addRunTodayRepeateValue(appName, appFunction, ref oSwiftTracer_SwiftAdapter);

                }
                else if (validTime(checkWeekend_SwiftAdapter, timeStart_SwiftAdapter, timeStop_SwiftAdapter) == false)
                {
                    Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { appFunction + " Cannot start at current days & time..." });
                }
                else if (isValidRepeatLimit(appName, appFunction, ref oSwiftTracer_SwiftAdapter) == false)
                {
                    Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { appFunction + " Maximum Run Repeat Reached..." });
                }
                else
                {
                    Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { appFunction + " Out of Time..." });
                }
            }
        }


        #endregion
        private void btnStop_Click(object sender, EventArgs e)
        {
 
            //Mirroring
            if (timer_Mirroring != null)
            {
                timer_Mirroring.Stop();
                timer_Mirroring.Enabled = false;
            }
            for (int i = 0; i < bgws_Mirroring.Count; i++)
            {
                bgws_Mirroring[i].CancelAsync();
            }

            //Mapping
            if (timer_Mapping != null)
            {
                timer_Mapping.Stop();
                timer_Mapping.Enabled = false;
            }
            for (int j = 0; j < bgws_Mapping.Count; j++)
            {
                bgws_Mapping[j].CancelAsync();
            }

            //SwiftAdapter
            if (timer_SwiftAdapter != null)
            {
                timer_SwiftAdapter.Stop();
                timer_SwiftAdapter.Enabled = false;
            }
            for (int k = 0; k < bgws_SwiftAdapter.Count; k++)
            {
                bgws_SwiftAdapter[k].CancelAsync();
            }

            btnStart.Enabled = true;
            btnStart.BackColor = Color.LimeGreen;

            btnStop.Enabled = false;
            btnStop.BackColor = Color.Gray;

            //buttonExit.Enabled = true;
            Program.oLogger = new Logger(0);

            //Update Stop Scheduler
            string sql = "UPDATE md_scheduler_activity SET IS_RUN = 0 WHERE SCH_NAME = @PARAM_0";

            var list = new List<JObject>();
            list.Add(new JObject(new JProperty("@PARAM_0", oConfig.getParameter("APPName"))));

            oDB.queryUpdateWithParam(sql, list);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!IsSchOpen())
            {
                this.Close();
            }
            else 
            {
                MessageBox.Show("Stop the current process first");
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
