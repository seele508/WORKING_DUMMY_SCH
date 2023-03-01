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
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using ConfigurationBuilder = Microsoft.Extensions.Configuration.ConfigurationBuilder;
using System.Diagnostics;
using SchTracer.Properties;
using static Confluent.Kafka.ConfigPropertyNames;
using static System.Windows.Forms.AxHost;

namespace SchTracer
{
    public partial class ChildFormMain : Form
    {
        private Helper.Config oConfig;
        private cData oData;
        private cDataSqlSrv oDataSqlSrv;
        private DBConnection oDB;

        private cProducer oProducer;
        //private DBConnection oDBSQLServer;
        private DBConnectionSqlSrv oDBSQLServer;

        private WebServicesMapping WSRequest;

        private String timeDtart = "05:00:00";
        private String timeStop = "21:00:00";
        private int timeoutLimit = 60000;

        private bool SchChecker = false;
        public delegate void populateTextBoxDelegate(String text);
        public delegate void populateLabelDelegate(String text);

        //private String msgRunSchSwiftAdapter = "";
        //private string treatFunctionSwiftAdapter = "";

        private String msgRunSchDummy_01 = "";
        private string treatFunctionDummy_01 = "";

        //ini buat log
        private AppSettingsReader vAppSetting;
        private string tellerId;
        private string branchCodeParam;

        private string appName;
        

        //Dummy WS Swift
        //private DBSchParameter oSwiftTracer_SwiftAdapter;
        //private string schName_SwiftAdapter;
        //private int maxThread_SwiftAdapter = 1;
        //private int maxResult_SwiftAdapter = 1;
        //private int runningThread_SwiftAdapter = 0;
        //private int cancelThread_SwiftAdapter = 0;
        //private int interval_SwiftAdapter = 0;
        //private String timeStart_SwiftAdapter = "06:00:00";
        //private String timeStop_SwiftAdapter = "20:00:00";
        //private int checkWeekend_SwiftAdapter = 0;
        //private System.Windows.Forms.Timer timer_SwiftAdapter;
        //List<BackgroundWorker> bgws_SwiftAdapter = new List<BackgroundWorker>();


        private DBSchParameter oDummy_01;
        private string schName_Dummy_01;
        private int maxThread_Dummy_01 = 1;
        private int maxResult_Dummy_01 = 1;
        private int runningThread_Dummy_01 = 0;
        private int cancelThread_Dummy_01 = 0;
        private int interval_Dummy_01 = 0;
        private String timeStart_Dummy_01 = "06:00:00";
        private String timeStop_Dummy_01 = "20:00:00";
        private int checkWeekend_Dummy_01 = 0;
        private System.Windows.Forms.Timer timer_Dummy_01;
        List<BackgroundWorker> bgws_Dummy_01 = new List<BackgroundWorker>();


        public Logger oLogger;

        public string WebServiceUrl = "";
        public ChildFormMain()
        {
            InitializeComponent();
            oConfig = new Helper.Config();

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
            /*
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
            */
            #endregion

            if (IsSchOpen())
            {
                
                //if (msgRunSch != null)
                if (msgRunSchDummy_01 != null)
                {
                    //MessageBox.Show("Scheduler Function " + msgRunSch + " Already Open!!!");
                    MessageBox.Show("Scheduler Function " + msgRunSchDummy_01 + " Already Open!!!");
                    System.Environment.Exit(0);
                }
                else
                {
                    MessageBox.Show("Scheduller Already Open!!!");
                    System.Environment.Exit(0);
                }


            }

            //SwiftAdapter
            //oSwiftTracer_SwiftAdapter = new DBSchParameter(oDB, oConfig.getParameter("APPName"), oConfig.getParameter("APPFunction_SwiftAdapter"));
            //if (!oSwiftTracer_SwiftAdapter.isValidParameterValue())
            //{
            //    MessageBox.Show("Error " + oSwiftTracer_SwiftAdapter.getErrorMessage());
            //    System.Environment.Exit(0);
            //}
            //else
            //{
            //    schName_SwiftAdapter = oSwiftTracer_SwiftAdapter.SCH_NAME();
            //    maxThread_SwiftAdapter = oSwiftTracer_SwiftAdapter.MAX_THREAD();
            //    maxResult_SwiftAdapter = oSwiftTracer_SwiftAdapter.MAX_QUERY_RESULT();
            //    interval_SwiftAdapter = oSwiftTracer_SwiftAdapter.TIMER_INTERVAL_MILISECOND();
            //    timeStart_SwiftAdapter = oSwiftTracer_SwiftAdapter.START_TIME();
            //    timeStop_SwiftAdapter = oSwiftTracer_SwiftAdapter.STOP_TIME();
            //    checkWeekend_SwiftAdapter = oSwiftTracer_SwiftAdapter.WEEKEND_CHECK();
            //}

            #region Dummy_01
            oDummy_01 = new DBSchParameter(oDB, oConfig.getParameter("APPName"), oConfig.getParameter("APPFunction_Dummy_01"));
            if (!oDummy_01.isValidParameterValue())
            {
                MessageBox.Show("Error " + oDummy_01.getErrorMessage());
                System.Environment.Exit(0);
            }
            else
            {
                schName_Dummy_01 = oDummy_01.SCH_NAME();
                maxThread_Dummy_01 = oDummy_01.MAX_THREAD();
                maxResult_Dummy_01 = oDummy_01.MAX_QUERY_RESULT();
                interval_Dummy_01 = oDummy_01.TIMER_INTERVAL_MILISECOND();
                timeStart_Dummy_01 = oDummy_01.START_TIME();
                timeStop_Dummy_01 = oDummy_01.STOP_TIME();
                checkWeekend_Dummy_01 = oDummy_01.WEEKEND_CHECK();
            }
            #endregion


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
                            msgRunSchDummy_01 = dr["SCH_FUNCTION"].ToString();
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


                //SwiftAdapter
                //timer_SwiftAdapter = new System.Windows.Forms.Timer();
                //timer_SwiftAdapter.Interval = interval_SwiftAdapter;
                //timer_SwiftAdapter.Tick += new EventHandler(onTick_SwiftAdapter);
                //timer_SwiftAdapter.Enabled = true;
                //timer_SwiftAdapter.Start();

                //Dummy_01
                #region Dummy_01
                timer_Dummy_01 = new System.Windows.Forms.Timer();
                timer_Dummy_01.Interval = interval_Dummy_01;
                timer_Dummy_01.Tick += new EventHandler(onTick_Dummy_01);
                timer_Dummy_01.Enabled = true;
                timer_Dummy_01.Start();
                #endregion
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
                //DateTime dtLastRun = DateTime.ParseExact(param.LAST_RUN(), "M/d/yyyy", null);
                DateTime dtLastRun = DateTime.ParseExact(param.LAST_RUN(), "dd/MM/yyyy", null);
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

        #region SwiftAdapter
        //public void onTick_SwiftAdapter(Object s, EventArgs ae)
        //{
        //    if(bgws_SwiftAdapter.Count == 0)
        //    {
        //        for (int i = 0; i < maxThread_SwiftAdapter; i++)
        //        {
        //            bgws_SwiftAdapter.Add(new BackgroundWorker());
        //            bgws_SwiftAdapter[i].DoWork += DoWork_SwiftAdapter;
        //            bgws_SwiftAdapter[i].RunWorkerCompleted += bgws_Completed_SwiftAdapter;
        //            bgws_SwiftAdapter[i].WorkerSupportsCancellation = true;
        //            bgws_SwiftAdapter[i].RunWorkerAsync(i);
        //        }
        //    }
        //}
        //public void bgws_Completed_SwiftAdapter(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    String message = "";
        //    int i = bgws_SwiftAdapter.IndexOf((BackgroundWorker)sender);

        //    if (e.Cancelled == true)
        //    {
        //        cancelThread_SwiftAdapter += 1;
        //        bgws_SwiftAdapter.RemoveAt(1);
        //        if (cancelThread_SwiftAdapter + runningThread_SwiftAdapter + 1 == maxThread_SwiftAdapter)
        //        {
        //            message += "Thread Cancelled";
        //        }

        //    }
        //    else if (!(e.Error == null))
        //    {
        //        message = "Thread Error: " + e.Error.Message;
        //    }
        //    else
        //    {
        //        bgws_SwiftAdapter.RemoveAt(i);
        //        message = "Worker " + treatFunctionSwiftAdapter + " [" + runningThread_SwiftAdapter + "] Done";
        //        runningThread_SwiftAdapter += 1;
        //        if (bgws_SwiftAdapter.Count == 0)
        //        {
        //            message += (Environment.NewLine + "***   Thread Success!!!   ***" + Environment.NewLine);
        //            runningThread_SwiftAdapter = 0;
        //        }
        //    }

        //    if (message != "")
        //    {
        //        Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { message });
        //    }
        //}
        //public void DoWork_SwiftAdapter(Object sender, DoWorkEventArgs e)
        //{
        //    BackgroundWorker workerThread = sender as BackgroundWorker;
        //    int tred = (int)e.Argument;

        //    if (workerThread.CancellationPending == true)
        //    {
        //        e.Cancel = true;
        //    }
        //    else
        //    {

        //        string appName = oConfig.getParameter("APPName");
        //        string appFunction = oConfig.getParameter("APPFunction_SwiftAdapter");
        //        treatFunctionSwiftAdapter = appFunction;
        //        string tellerID = oConfig.getParameter("TellerID");
        //        string branch = oConfig.getParameter("Branch");
        //        DateTime currdate = DateTime.Now;
        //        int configRD = Int16.Parse(oConfig.getParameter("DateRange"));

        //        bool hasilSwift = false;
        //        bool hasilSwiftIRN = false;
        //        bool Checker_status = false;
        //        bool Checker_statusIRN = false;

        //        if (validTime(checkWeekend_SwiftAdapter, timeStart_SwiftAdapter, timeStop_SwiftAdapter) && isValidRepeatLimit(appName, appFunction, ref oSwiftTracer_SwiftAdapter))
        //        {
        //            //show console log
        //            Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { appFunction + " Start..." });

        //            oData.updateSchRunning(appName, appFunction);

        //            try
        //            {
        //                #region Bagian IRK
        //                WebServicesSwiftAdapter WSSwiftAdapter = new WebServicesSwiftAdapter();
        //                WebServiceRequestSwiftAd WSReqSwiftAdapter = new WebServiceRequestSwiftAd();
        //                WebServiceRespondsSwiftAd WSRespondSwiftAdapter = new WebServiceRespondsSwiftAd();



                        
        //                string tempUsername = oData.getUsername("adapterws_user");
        //                string tempKataKunci = oData.getWSKatakunci("adapterws_password");
        //                string tempTypeTransSource = "IRK";
        //                string statusKerjasama = "1";
        //                string WSAddress = oData.getWSAddress("adapterws_url_GetDataPendingMt199");

        //                string decUser = Enkripsi.StringCipher.Decrypt(tempUsername);
        //                string decKataKunci = Enkripsi.StringCipher.Decrypt(tempKataKunci);

        //                int InsIrkId = 0;
        //                WSReqSwiftAdapter.username = decUser;
        //                WSReqSwiftAdapter.gembok = decKataKunci;
        //                WSReqSwiftAdapter.transactionSource = tempTypeTransSource;

        //                WSRespondSwiftAdapter = WSSwiftAdapter.SendDataToSwiftAdapter(WSReqSwiftAdapter, WSAddress);

        //                if (WSRespondSwiftAdapter.statusCode == "0001")
        //                {
        //                    #region Dictionary wsAdapterResult


        //                    List<string> listTag20 = new List<string>();
        //                    List<string> listTrxdate = new List<string>();
        //                    for (int i = 0; i < Int32.Parse(WSRespondSwiftAdapter.totaldata); i++)
        //                    {
        //                        Checker_status = oData.isExistSwiftAdapter(WSRespondSwiftAdapter.transactiondataSwift[i].TAG20, WSRespondSwiftAdapter.transactiondataSwift[i].TrxDate);

        //                        if (Checker_status == false)
        //                        {
        //                            Dictionary<string, string> wsAdapterResultSwift = new Dictionary<string, string>();
        //                            wsAdapterResultSwift.Clear();//reset

        //                            wsAdapterResultSwift.Add("SenderBIC", WSRespondSwiftAdapter.transactiondataSwift[i].SenderBIC);
        //                            wsAdapterResultSwift.Add("ReceiverBIC", WSRespondSwiftAdapter.transactiondataSwift[i].ReceiverBIC);
        //                            wsAdapterResultSwift.Add("NewSeqno", WSRespondSwiftAdapter.transactiondataSwift[i].NewSeqno);
        //                            wsAdapterResultSwift.Add("RowID", WSRespondSwiftAdapter.transactiondataSwift[i].RowID);

        //                            wsAdapterResultSwift.Add("TrxAmount", WSRespondSwiftAdapter.transactiondataSwift[i].TrxAmount);
        //                            wsAdapterResultSwift.Add("TrxCurrency", WSRespondSwiftAdapter.transactiondataSwift[i].TrxCurrency);
        //                            wsAdapterResultSwift.Add("TrxDate", WSRespondSwiftAdapter.transactiondataSwift[i].TrxDate);
        //                            wsAdapterResultSwift.Add("BenefAccount", WSRespondSwiftAdapter.transactiondataSwift[i].BenefAccount);

        //                            wsAdapterResultSwift.Add("BenefName", WSRespondSwiftAdapter.transactiondataSwift[i].BenefName);
        //                            wsAdapterResultSwift.Add("ProcessId", WSRespondSwiftAdapter.transactiondataSwift[i].ProcessId);
        //                            wsAdapterResultSwift.Add("MTtype", WSRespondSwiftAdapter.transactiondataSwift[i].MTtype);
        //                            wsAdapterResultSwift.Add("TAG20_101", WSRespondSwiftAdapter.transactiondataSwift[i].TAG20_101);

        //                            wsAdapterResultSwift.Add("TAG20", WSRespondSwiftAdapter.transactiondataSwift[i].TAG20);
        //                            wsAdapterResultSwift.Add("Description", WSRespondSwiftAdapter.transactiondataSwift[i].Description);
        //                            wsAdapterResultSwift.Add("BrinetFullName", WSRespondSwiftAdapter.transactiondataSwift[i].BrinetFullName);
        //                            wsAdapterResultSwift.Add("BrinetName", WSRespondSwiftAdapter.transactiondataSwift[i].BrinetName);

        //                            wsAdapterResultSwift.Add("FullMT", WSRespondSwiftAdapter.transactiondataSwift[i].FullMT);
        //                            wsAdapterResultSwift.Add("Depcor", WSRespondSwiftAdapter.transactiondataSwift[i].Depcor);
        //                            wsAdapterResultSwift.Add("Tag53", WSRespondSwiftAdapter.transactiondataSwift[i].Tag53);
        //                            wsAdapterResultSwift.Add("Tag54", WSRespondSwiftAdapter.transactiondataSwift[i].Tag54);

        //                            wsAdapterResultSwift.Add("TrxSource", WSRespondSwiftAdapter.transactiondataSwift[i].TrxSource);
        //                            wsAdapterResultSwift.Add("CancelMTType", WSRespondSwiftAdapter.transactiondataSwift[i].CancelMTType);
        //                            wsAdapterResultSwift.Add("CancelFeeAmount", WSRespondSwiftAdapter.transactiondataSwift[i].CancelFeeAmount);
        //                            wsAdapterResultSwift.Add("CancelFeeCurrency", WSRespondSwiftAdapter.transactiondataSwift[i].CancelFeeCurrency);

        //                            wsAdapterResultSwift.Add("SenderBicIsDepcor", WSRespondSwiftAdapter.transactiondataSwift[i].SenderBicIsDepcor);
        //                            wsAdapterResultSwift.Add("Tag56", WSRespondSwiftAdapter.transactiondataSwift[i].Tag56);
        //                            wsAdapterResultSwift.Add("SwiftAdapterStatus", WSRespondSwiftAdapter.transactiondataSwift[i].SwiftAdapterStatus);

        //                            hasilSwift = oData.isProcessInsertSwiftAdapter(wsAdapterResultSwift, statusKerjasama, ref InsIrkId);


        //                            if (hasilSwift == true)
        //                            {
        //                                //Log
        //                                vAppSetting = new AppSettingsReader();

        //                                string valUser = "Scheduler Swift tracer";
        //                                branchCodeParam = vAppSetting.GetValue("Branch", Type.GetType("System.String")).ToString();

        //                                appName = vAppSetting.GetValue("APPName", Type.GetType("System.String")).ToString();

        //                                String pMessage = "Process IRK Tag20: " + WSRespondSwiftAdapter.transactiondataSwift[i].TAG20 + " Success!!! ";
        //                                String pAppName = appName;
        //                                String pAction = "Get Data IRK Swift Adapter";

        //                                oData.insertLog(InsIrkId.ToString(), WSRespondSwiftAdapter.transactiondataSwift[i].TAG20, valUser, pMessage, pAppName, pAction);
        //                                Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { "Process IRK Tag20: " + WSRespondSwiftAdapter.transactiondataSwift[i].TAG20 + " Success!!! " });
        //                            }
        //                            else
        //                            {
        //                                Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { "Process IRK Tag20: " + WSRespondSwiftAdapter.transactiondataSwift[i].TAG20 + " Failed!!! " });
        //                            }

        //                        }
        //                    }

        //                    #endregion
        //                }
        //                else 
        //                {
        //                    Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { "Process Get Swift Adapter IRK Failed!!! " });
        //                }
        //                #endregion

        //                #region Bagian IRN
        //                WebServicesSwiftAdapter WSSwiftAdapterIRN = new WebServicesSwiftAdapter();
        //                WebServiceRequestSwiftAd WSReqSwiftAdapterIRN = new WebServiceRequestSwiftAd();
        //                WebServiceRespondsSwiftAd WSRespondSwiftAdapterIRN = new WebServiceRespondsSwiftAd();

        //                string tempUsernameIRN = oData.getUsername("adapterws_user");
        //                string tempKataKunciIRN = oData.getWSKatakunci("adapterws_password");
        //                string tempTypeTransSourceIRN = "IRN";
        //                string statusKerjasamaIRN = "0";
        //                string WSAddressIRN = oData.getWSAddress("adapterws_url_GetDataPendingMt199");

        //                string decUserIRN = Enkripsi.StringCipher.Decrypt(tempUsernameIRN);
        //                string decKataKunciIRN = Enkripsi.StringCipher.Decrypt(tempKataKunciIRN);

        //                int InsIrnId = 0;
        //                WSReqSwiftAdapterIRN.username = decUserIRN;
        //                WSReqSwiftAdapterIRN.gembok = decKataKunciIRN;
        //                WSReqSwiftAdapterIRN.transactionSource = tempTypeTransSourceIRN;

        //                WSRespondSwiftAdapterIRN = WSSwiftAdapterIRN.SendDataToSwiftAdapter(WSReqSwiftAdapterIRN, WSAddressIRN);

        //                if (WSRespondSwiftAdapterIRN.statusCode == "0001")
        //                {
        //                    #region Dictionary wsAdapterResult IRN
        //                    for (int i = 0; i < Int32.Parse(WSRespondSwiftAdapterIRN.totaldata); i++)
        //                    {
        //                        Checker_statusIRN = oData.isExistSwiftAdapter(WSRespondSwiftAdapterIRN.transactiondataSwift[i].TAG20, WSRespondSwiftAdapterIRN.transactiondataSwift[i].TrxDate);

        //                        if (Checker_statusIRN == false)
        //                        {
        //                            Dictionary<string, string> wsAdapterResultSwiftIRN = new Dictionary<string, string>();
        //                            wsAdapterResultSwiftIRN.Clear();//reset

        //                            wsAdapterResultSwiftIRN.Add("SenderBIC", WSRespondSwiftAdapterIRN.transactiondataSwift[i].SenderBIC);
        //                            wsAdapterResultSwiftIRN.Add("ReceiverBIC", WSRespondSwiftAdapterIRN.transactiondataSwift[i].ReceiverBIC);
        //                            wsAdapterResultSwiftIRN.Add("NewSeqno", WSRespondSwiftAdapterIRN.transactiondataSwift[i].NewSeqno);
        //                            wsAdapterResultSwiftIRN.Add("RowID", WSRespondSwiftAdapterIRN.transactiondataSwift[i].RowID);

        //                            wsAdapterResultSwiftIRN.Add("TrxAmount", WSRespondSwiftAdapterIRN.transactiondataSwift[i].TrxAmount);
        //                            wsAdapterResultSwiftIRN.Add("TrxCurrency", WSRespondSwiftAdapterIRN.transactiondataSwift[i].TrxCurrency);
        //                            wsAdapterResultSwiftIRN.Add("TrxDate", WSRespondSwiftAdapterIRN.transactiondataSwift[i].TrxDate);
        //                            wsAdapterResultSwiftIRN.Add("BenefAccount", WSRespondSwiftAdapterIRN.transactiondataSwift[i].BenefAccount);

        //                            wsAdapterResultSwiftIRN.Add("BenefName", WSRespondSwiftAdapterIRN.transactiondataSwift[i].BenefName);
        //                            wsAdapterResultSwiftIRN.Add("ProcessId", WSRespondSwiftAdapterIRN.transactiondataSwift[i].ProcessId);
        //                            wsAdapterResultSwiftIRN.Add("MTtype", WSRespondSwiftAdapterIRN.transactiondataSwift[i].MTtype);
        //                            wsAdapterResultSwiftIRN.Add("TAG20_101", WSRespondSwiftAdapterIRN.transactiondataSwift[i].TAG20_101);

        //                            wsAdapterResultSwiftIRN.Add("TAG20", WSRespondSwiftAdapterIRN.transactiondataSwift[i].TAG20);
        //                            wsAdapterResultSwiftIRN.Add("Description", WSRespondSwiftAdapterIRN.transactiondataSwift[i].Description);
        //                            wsAdapterResultSwiftIRN.Add("BrinetFullName", WSRespondSwiftAdapterIRN.transactiondataSwift[i].BrinetFullName);
        //                            wsAdapterResultSwiftIRN.Add("BrinetName", WSRespondSwiftAdapterIRN.transactiondataSwift[i].BrinetName);

        //                            wsAdapterResultSwiftIRN.Add("FullMT", WSRespondSwiftAdapterIRN.transactiondataSwift[i].FullMT);
        //                            wsAdapterResultSwiftIRN.Add("Depcor", WSRespondSwiftAdapterIRN.transactiondataSwift[i].Depcor);
        //                            wsAdapterResultSwiftIRN.Add("Tag53", WSRespondSwiftAdapterIRN.transactiondataSwift[i].Tag53);
        //                            wsAdapterResultSwiftIRN.Add("Tag54", WSRespondSwiftAdapterIRN.transactiondataSwift[i].Tag54);

        //                            wsAdapterResultSwiftIRN.Add("TrxSource", WSRespondSwiftAdapterIRN.transactiondataSwift[i].TrxSource);
        //                            wsAdapterResultSwiftIRN.Add("CancelMTType", WSRespondSwiftAdapterIRN.transactiondataSwift[i].CancelMTType);
        //                            wsAdapterResultSwiftIRN.Add("CancelFeeAmount", WSRespondSwiftAdapterIRN.transactiondataSwift[i].CancelFeeAmount);
        //                            wsAdapterResultSwiftIRN.Add("CancelFeeCurrency", WSRespondSwiftAdapterIRN.transactiondataSwift[i].CancelFeeCurrency);

        //                            wsAdapterResultSwiftIRN.Add("SenderBicIsDepcor", WSRespondSwiftAdapterIRN.transactiondataSwift[i].SenderBicIsDepcor);
        //                            wsAdapterResultSwiftIRN.Add("Tag56", WSRespondSwiftAdapterIRN.transactiondataSwift[i].Tag56);
        //                            wsAdapterResultSwiftIRN.Add("SwiftAdapterStatus", WSRespondSwiftAdapterIRN.transactiondataSwift[i].SwiftAdapterStatus);


        //                            hasilSwiftIRN = oData.isProcessInsertSwiftAdapter(wsAdapterResultSwiftIRN, statusKerjasamaIRN, ref InsIrnId);


        //                            if (hasilSwiftIRN == true)
        //                            {
        //                                //Log
        //                                vAppSetting = new AppSettingsReader();

        //                                string valUser = "Scheduler Swift tracer";
        //                                branchCodeParam = vAppSetting.GetValue("Branch", Type.GetType("System.String")).ToString();

        //                                appName = vAppSetting.GetValue("APPName", Type.GetType("System.String")).ToString();

        //                                String pMessage = "Process IRN Tag20: " + WSRespondSwiftAdapterIRN.transactiondataSwift[i].TAG20 + " Success!!! ";
        //                                String pAppName = appName;
        //                                String pAction = "Get Data IRN Swift Adapter";

        //                                oData.insertLog(InsIrnId.ToString(), WSRespondSwiftAdapterIRN.transactiondataSwift[i].TAG20, valUser, pMessage, pAppName, pAction);
        //                                Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { "Process IRN Tag20: " + WSRespondSwiftAdapterIRN.transactiondataSwift[i].TAG20 + " Success!!! " });
        //                            }
        //                            else
        //                            {
        //                                Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { "Process IRN Tag20: " + WSRespondSwiftAdapterIRN.transactiondataSwift[i].TAG20 + " Failed!!! " });
        //                            }

        //                        }
        //                    }
        //                    #endregion
        //                }
        //                else 
        //                {
        //                    Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { "Process Get Swift Adapter IRN Failed!!! " });
        //                }
        //                #endregion

        //            }
        //            catch (Exception ex)
        //            {
                        
        //                Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { "Process Get Swift Adapter Error : " + ex.Message + " " + ex.StackTrace });
        //            }
        //            addRunTodayRepeateValue(appName, appFunction, ref oSwiftTracer_SwiftAdapter);

        //        }
        //        else if (validTime(checkWeekend_SwiftAdapter, timeStart_SwiftAdapter, timeStop_SwiftAdapter) == false)
        //        {
        //            Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { appFunction + " Cannot start at current days & time..." });
        //        }
        //        else if (isValidRepeatLimit(appName, appFunction, ref oSwiftTracer_SwiftAdapter) == false)
        //        {
        //            Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { appFunction + " Maximum Run Repeat Reached..." });
        //        }
        //        else
        //        {
        //            Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { appFunction + " Out of Time..." });
        //        }
        //    }
        //}


        #endregion

        #region Dummy_01
        public void onTick_Dummy_01(Object s, EventArgs ae)
        {
            if (bgws_Dummy_01.Count == 0)
            {
                for (int i = 0; i < maxThread_Dummy_01; i++)
                {
                    bgws_Dummy_01.Add(new BackgroundWorker());
                    bgws_Dummy_01[i].DoWork += DoWork_Dummy_01;
                    bgws_Dummy_01[i].RunWorkerCompleted += bgws_Completed_Dummy_01;
                    bgws_Dummy_01[i].WorkerSupportsCancellation = true;
                    bgws_Dummy_01[i].RunWorkerAsync(i);
                }
            }
        }
        public void bgws_Completed_Dummy_01(object sender, RunWorkerCompletedEventArgs e)
        {
            String message = "";
            int i = bgws_Dummy_01.IndexOf((BackgroundWorker)sender);

            if (e.Cancelled == true)
            {
                cancelThread_Dummy_01 += 1;
                bgws_Dummy_01.RemoveAt(1);
                if (cancelThread_Dummy_01 + runningThread_Dummy_01 + 1 == maxThread_Dummy_01)
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
                bgws_Dummy_01.RemoveAt(i);
                message = "Worker " + treatFunctionDummy_01 + " [" + runningThread_Dummy_01 + "] Done";
                runningThread_Dummy_01 += 1;
                if (bgws_Dummy_01.Count == 0)
                {
                    message += (Environment.NewLine + "***   Thread Success!!!   ***" + Environment.NewLine);
                    runningThread_Dummy_01 = 0;
                }
            }

            if (message != "")
            {
                Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { message });
            }
        }
        public async void DoWork_Dummy_01(Object sender, DoWorkEventArgs e)
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
                string appFunction = oConfig.getParameter("APPFunction_Dummy_01");
                treatFunctionDummy_01 = appFunction;
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



                if (validTime(checkWeekend_Dummy_01, timeStart_Dummy_01, timeStop_Dummy_01) && isValidRepeatLimit(appName, appFunction, ref oDummy_01))
                {
                    //show console log
                    Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { appFunction + " Start..." });

                    oData.updateSchRunning(appName, appFunction);


                    try
                    {
                        /*
                        var proc = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = @"C:\Program Files\Microsoft Visual Studio 14.0\Common7\IDE\tf.exe",
                                //FileName = @"D:\2022\kafka-dotnet-getting-started\producer\producer_04\bin\Debug\net6.0\config.txt",
                                Arguments = " ping google.com ",
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                CreateNoWindow = true,
                                WorkingDirectory = @"D:\2022\kafka-dotnet-getting-started\producer\producer_04\"
                            }
                        };

                        proc.Start();
                        */

                        //System.Diagnostics.Process.Start("CMD.exe", "/K cd D:/2022/kafka-dotnet-getting-started/producer/producer_04 & dotnet run $(pwd)/../config.txt");
                        //System.Diagnostics.Process.Start("CMD.exe", "/C cd D:/2022/kafka-dotnet-getting-started/producer/producer_04 & dotnet run $(pwd)/../config.txt");


                        //System.Diagnostics.Process.Start("CMD.exe", "/C cd D:/2022/kafka-dotnet-getting-started/producer/producer_04 & dotnet run $(pwd)/../config.txt");

                        
                        Process cmd = new Process();
                        cmd.StartInfo.FileName = "cmd.exe";
                        //cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        cmd.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                        //cmd.StartInfo.Arguments = "/C cd D:/2022/kafka-dotnet-getting-started/producer/producer_04 & dotnet run $(pwd)/../config.txt";

                        cmd.StartInfo.Arguments = "/C cd D:/ & copy D:/Budiman.png D:/Dummy/ ";

                        //cmd.StartInfo.Arguments = "/C copy c:/Budiman.png d:";
                        cmd.Start();
                        
                    }
                    catch (Exception ex)
                    {
                        Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { ex.Message});
                    }

                    Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { appFunction + " Jalan Bois..." });

                    addRunTodayRepeateValue(appName, appFunction, ref oDummy_01);
                }
                else if (validTime(checkWeekend_Dummy_01, timeStart_Dummy_01, timeStop_Dummy_01) == false)
                {
                    Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { appFunction + " Cannot start at current days & time..." });
                }
                else if (isValidRepeatLimit(appName, appFunction, ref oDummy_01) == false)
                {
                    Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { appFunction + " Maximum Run Repeat Reached..." });
                }
                else
                {
                    Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { appFunction + " Out of Time..." });
                }
            }
        }

        public static async Task Wolololo(string[] args)
        {
            var config = new ProducerConfig { BootstrapServers = "https://kafka5.dev.bri.co.id:9021" };

            // If serializers are not specified, default serializers from
            // `Confluent.Kafka.Serializers` will be automatically used where
            // available. Note: by default strings are encoded as UTF8.
            using (var p = new ProducerBuilder<Null, string>(config).Build())
            {
                try
                {
                    //var dr = await p.ProduceAsync("esb_swift_mx_transaction", new Message<Null, string> { Value = "test" });
                    //var dr = await p.Produce("esb_swift_mx_transaction", new Message<string, string> { Value = "Budi" ,Value = "test" });
                    //Console.WriteLine($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
                    //var mesg = dr.TopicPartitionOffset.ToString();
                    //Invoke(new populateTextBoxDelegate(populateTextBoxWithTimeStamp), new object[] { "Delivered to" + mesg });
                    //MessageBox.Show(mesg);

                    IConfiguration configuration = new ConfigurationBuilder()
            .AddIniFile(args[0])
            .Build();

                    const string topic = "purchases";

                    string[] users = { "eabara", "jsmith", "sgarcia", "jbernard", "htanaka", "awalther" };
                    string[] items = { "book", "alarm clock", "t-shirts", "gift card", "batteries" };

                    using (var producer = new ProducerBuilder<string, string>(
                        configuration.AsEnumerable()).Build())
                    {
                        var numProduced = 0;
                        Random rnd = new Random();
                        const int numMessages = 10;
                        for (int i = 0; i < numMessages; ++i)
                        {
                            var user = users[rnd.Next(users.Length)];
                            var item = items[rnd.Next(items.Length)];

                            producer.Produce(topic, new Message<string, string> { Key = user, Value = item },
                                (deliveryReport) =>
                                {
                                    if (deliveryReport.Error.Code != ErrorCode.NoError)
                                    {
                                        Console.WriteLine($"Failed to deliver message: {deliveryReport.Error.Reason}");
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Produced event to topic {topic}: key = {user,-10} value = {item}");
                                        numProduced += 1;
                                    }
                                });
                        }

                        producer.Flush(TimeSpan.FromSeconds(10));
                    }
                }

                catch (ProduceException<Null, string> e)
                {
                    Console.WriteLine($"Delivery failed: {e.Error.Reason}");
                }
            }
        }
        #endregion
        private void btnStop_Click(object sender, EventArgs e)
        {
            //SwiftAdapter
            //if (timer_SwiftAdapter != null)
            //{
            //    timer_SwiftAdapter.Stop();
            //    timer_SwiftAdapter.Enabled = false;
            //}
            //for (int k = 0; k < bgws_SwiftAdapter.Count; k++)
            //{
            //    bgws_SwiftAdapter[k].CancelAsync();
            //}

            //Dummy_01
            #region Dummy_01
            if (timer_Dummy_01 != null)
            {
                timer_Dummy_01.Stop();
                timer_Dummy_01.Enabled = false;
            }
            for (int k = 0; k < bgws_Dummy_01.Count; k++)
            {
                bgws_Dummy_01[k].CancelAsync();
            }
            #endregion

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
