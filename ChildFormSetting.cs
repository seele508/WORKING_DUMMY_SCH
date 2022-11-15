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
using System.Collections;

namespace SchTracer
{
    public partial class ChildFormSetting : Form
    {
        private Config oConfig;
        private cData oData;

        private DBConnection oDB;
        private DBSchParameter oSch_PortalTesting01;

        private DBSchParameterV2 oSch_PortalTesting;
        private DBSchParameterV2 oSCh_ParamV2;

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

        private int totalfunc = 0;

        public List<MenuSchParam> Parameter { get; set; }
        public ChildFormSetting()
        {
            InitializeComponent();
            oConfig = new Config();

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
                MessageBox.Show("Failed To Connect MySQL Databse");
                System.Environment.Exit(0);
            }
            
            oSCh_ParamV2 = new DBSchParameterV2(oDB, oConfig.getParameter("APPName"));

            //get 1 data yg name nya sama
            
            oData = new cData(oDB);
            totalfunc = oSCh_ParamV2.getSchFunc().Count();
            Parameter = getParam(oSch_PortalTesting);


            setComboBox();
            setDefaultValue();
        }
    
        public void setComboBox()
        {
            comboBoxWeekEnd.Items.Clear();
            Dictionary<string, string> ComboItem = new Dictionary<string, string>();

            ComboItem.Add("1", "Yes");
            ComboItem.Add("0", "No");


            comboBoxWeekEnd.DataSource = new BindingSource(ComboItem, null);
            comboBoxWeekEnd.DisplayMember = "Value";
            comboBoxWeekEnd.ValueMember = "Key";

            string value = ((KeyValuePair<string, string>)comboBoxWeekEnd.SelectedItem).Value;
        }

        public void setDefaultValue()
        {
            txtSchFunc.Text = "";
            numUDRepeat.Value = 0;
            dtStartTime.Value = DateTime.Parse("00:00:00");
            dtStopTime.Value = DateTime.Parse("00:00:00");

            comboBoxWeekEnd.SelectedIndex = 0;
            numUDMaxThread.Value = 0;
            numUDMaxQuery.Value = 0;
            numUDRepeat.Value = 0;
        }
        
        private List<MenuSchParam> getParam(DBSchParameterV2 dBSchParameter)
        {

            var list = new List<MenuSchParam>();
           
            for (int i = 0; i < totalfunc; i++)
            {
                string SchFunc = oSCh_ParamV2.getSchFunc()[i].ToString();
                int RepeatDL = oSCh_ParamV2.getRepeatDL()[i];
                string StartT = oSCh_ParamV2.getStartTime()[i].ToString();
                string StopT = oSCh_ParamV2.getStopTime()[i].ToString();
                int CheckWeekend = oSCh_ParamV2.getWeekend()[i];
                int MaxThread = oSCh_ParamV2.getMaxThread()[i];
                int MaxQR = oSCh_ParamV2.getMaxQR()[i];
                int MaxRetry = oSCh_ParamV2.getMaxRetry()[i];

                
                list.Add(new MenuSchParam()
                {
                    sch_function = SchFunc,

                    repeat_daily_limit = RepeatDL.ToString(),
                    start_time = StartT,
                    stop_time = StopT,
                    weekend_check = CheckWeekend.ToString(),
                    maxthread = MaxThread.ToString(),
                    max_query_result = MaxQR.ToString(),
                    max_retry = MaxRetry.ToString()

                });
                
            }


            return list;
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void ChildFormSetting_Load(object sender, EventArgs e)
        {
            var param = this.Parameter;
            dataGridView1.DataSource = param;

            
            dataGridView1.Columns[0].HeaderText = "Function Name";
            dataGridView1.Columns[0].Width = 115;

            dataGridView1.Columns[1].HeaderText = "Repeat Daily Limit";
            dataGridView1.Columns[1].Width = 150;
                
            dataGridView1.Columns[2].HeaderText = "Start Time";
            dataGridView1.Columns[3].HeaderText = "Stop Time";
            dataGridView1.Columns[4].HeaderText = "Check Weekend";
            dataGridView1.Columns[4].Width = 115;

            dataGridView1.Columns[5].HeaderText = "Max Thread";
            dataGridView1.Columns[6].HeaderText = "Max Query Result";
            dataGridView1.Columns[6].Width = 150;

            dataGridView1.Columns[7].HeaderText = "Max Retry";
        }

        private void loadform()
        {
            var param = this.Parameter;
            dataGridView1.DataSource = param;
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                //var selectedParam = dataGridView1.SelectedRows[0].DataBoundItem as MenuSchParam;
                var selectedParam = dataGridView1.SelectedRows[0].DataBoundItem as MenuSchParam;
                txtSchFunc.Text = selectedParam.sch_function;
                
                
                numUDRepeat.Value = Int32.Parse(selectedParam.repeat_daily_limit.ToString());

                dtStartTime.Value = DateTime.Parse(selectedParam.start_time.ToString());

                dtStopTime.Value = DateTime.Parse(selectedParam.stop_time.ToString());

                if (selectedParam.weekend_check.ToString() == "1")
                {
                    
                    comboBoxWeekEnd.SelectedValue = "1";
                }
                else 
                {
                    
                    comboBoxWeekEnd.SelectedValue = "0";
                }
                
                numUDMaxThread.Value = Int32.Parse(selectedParam.maxthread.ToString());

                
                numUDMaxQuery.Value = Int32.Parse(selectedParam.max_query_result.ToString());

                
                numUDMaxRetry.Value = Int32.Parse(selectedParam.max_retry.ToString());
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }

        private void btnSaveParam_Click(object sender, EventArgs e)
        {
            
            bool statusUpdate = false;
            string valSchFunction = txtSchFunc.Text;
            string valRepeatDaily = numUDRepeat.Value.ToString();
            DateTime tempStart = dtStartTime.Value;

            string valStartTime = tempStart.ToString("HH:mm:ss");

            DateTime tempStop = dtStopTime.Value;
            string valStopTime = tempStop.ToString("HH:mm:ss");

            string valWeekEnd = comboBoxWeekEnd.SelectedValue.ToString();
            string valMaxThread = numUDMaxThread.Value.ToString();
            string valMaxQuery = numUDMaxQuery.Value.ToString();
            string valMaxRetry = numUDMaxRetry.Value.ToString();

            statusUpdate = oData.updateParamSch(valSchFunction, valRepeatDaily, valStartTime, valStopTime, 
                valWeekEnd, valMaxThread, valMaxQuery, valMaxRetry);

            if (statusUpdate == true)
            {
                MessageBox.Show("Success Update Scheduler");
            }
            else
            {
                MessageBox.Show("Failed Update Scheduler");
            }
            
            this.Close();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            
            this.Controls.Clear();
            this.InitializeComponent();
            this.ChildFormSetting_Load(null, EventArgs.Empty);
            
            this.setComboBox();
            this.setDefaultValue();

        }

        private void comboBoxWeekEnd_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            
        }

        private void txtRepeat_TextChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }
    }
}
