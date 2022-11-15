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

namespace SchTracer
{
    public partial class ParentForm : Form
    {
        private Config oConfig;
        private cData oData;
        private cDataSqlSrv oDataSqlSrv;
        private DBConnection oDB;

        //private DBConnection oDBSQLServer;
        private DBConnectionSqlSrv oDBSQLServer;

        private String timeDtart = "05:00:00";
        private String timeStop = "21:00:00";
        private int timeoutLimit = 60000;

        private bool SchChecker = false;
        public delegate void populateTextBoxDelegate(String text);
        public delegate void populateLabelDelegate(String text);

        private String msgRunSch = "";
        //ini nilai ke overwrite dari config / db

        //DUMMY_01
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
        int status = 0;
        string currChild = "";
        public ParentForm()
        {
            InitializeComponent();
            customizeDesign();
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

            
            lblSchName.Text = oDummy_01.SCH_NAME();

            //lblLastRun.Text = DateTime.ParseExact(oSwiftTracer_Mirroring.LAST_RUN_V2(), "M/d/yyyy h:mm:ss tt", CultureInfo.GetCultureInfo("en-US")).ToString("dd-MM-yyyy HH:mm:ss");
            lblLastRun.Text = oDummy_01.LAST_RUN_V2().ToString();
            timeoutLimit = int.Parse(oConfig.getParameter("TimeoutLimit"));

            oData = new cData(oDB);
            oDataSqlSrv = new cDataSqlSrv(oDBSQLServer);

            Program.oLogger = new Logger(oDB, oConfig);
        }


        #region Drag the form
        private const int WM_NCHITTEST = 0x84;
        private const int HT_CLIENT = 0x1;
        private const int HT_CAPTION = 0x2;
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_NCHITTEST)
            {
                m.Result = (IntPtr)(HT_CAPTION);
            }
        }
        #endregion

        private void customizeDesign()
        { 
            
        }

        private void hideSubMenu()
        { 
            
        }

        private void showSubMenu(Panel subMenu)
        {
            if (subMenu.Visible == false)
            {
                hideSubMenu();
                subMenu.Visible = true;
            }
            else 
            {
                subMenu.Visible = false;
            }
        }

        private void btnMainMenu_Click(object sender, EventArgs e)
        {
            

            //Program.oLogger = new Logger("ChildMain");
            if (!IsSchOpen())
            {
                currChild = "ChildMain";
                openChildForm(new ChildFormMain(), currChild);
            }
            else 
            {
                MessageBox.Show("Stop the current process first");
            }
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            
            if (!IsSchOpen())
            {
                currChild = "ChildSetting";
                openChildForm(new ChildFormSetting(), currChild);
            }
            else 
            {
                MessageBox.Show("Stop the current process first");
            }
        }

        private Form activeForm = null;
        private void openChildForm(Form childForm, string currChildDisplay)
        {
            if (activeForm == childForm)
            {
                btnMainMenu.Enabled = false;
                btnMainMenu.BackColor = Color.Gray;
                
            }
            else
            {
                
                if (!IsSchOpen())
                {
                    btnExit.Enabled = true;

                    #region show child
                    if (activeForm != null)
                    {
                        activeForm.Close();
                    }
                    activeForm = childForm;

                    childForm.TopLevel = false;
                    childForm.FormBorderStyle = FormBorderStyle.None;
                    childForm.Dock = DockStyle.Fill;

                    panelChildForm.Controls.Add(childForm);
                    panelChildForm.Tag = childForm;

                    childForm.BringToFront();
                    childForm.Show();
                    #endregion
                } 
            }
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
        private void btnExit_Click(object sender, EventArgs e)
        {
            
            if (!IsSchOpen())
            {
                System.Environment.Exit(0);
            }
            else 
            {
                MessageBox.Show("Stop the current process first");
            }
            
        }

        private void panelChildForm_Paint(object sender, PaintEventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Now;
            lblCurrTime.Text = dt.ToString("dd-MM-yyyy HH:mm:ss");
            
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnMin_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void label2_Click_1(object sender, EventArgs e)
        {

        }

        private void lblCurrTime_Click(object sender, EventArgs e)
        {

        }
    }
}
