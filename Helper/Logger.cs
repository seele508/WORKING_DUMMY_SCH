using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Helper
{
    public class Logger
    {
        private DBConnection oDB;
        private Config oConfig;

        private int _status = 0;
        private string _currChild = ""; // ini set default open main menu

        public Logger()
        {

        }

        public Logger(int status)
        {
            _status = status;
        }

        public Logger(int status, String currChild)
        {
            _status = status;
            _currChild = currChild;
        }

        public int getLogger()
        {
            //return _status;
            return this._status;
        }

        public string getLoggerCurrChild()
        {
            //return _currChild;
            return this._currChild;
        }

        public Logger(DBConnection oDB, Config oConfig)
        {
            this.oDB = oDB;
            this.oConfig = oConfig;
        }

        public void logBooking(String start, String end, String counterpart, String state, String message, String status, String refno, String sequence)
        {
            String sql = "INSERT INTO reff_log_payment_booking (start, stop, counterpart, trx, description, status, refno, seqno) VALUES (@PARAM_0, @PARAM_1, @PARAM_2, @PARAM_3, @PARAM_4, @PARAM_5, @PARAM_6, @PARAM_7)";

            var list = new List<JObject>();
            list.Add(new JObject(new JProperty("@PARAM_0", start)));
            list.Add(new JObject(new JProperty("@PARAM_1", end)));
            list.Add(new JObject(new JProperty("@PARAM_2", counterpart)));
            list.Add(new JObject(new JProperty("@PARAM_3", state)));
            list.Add(new JObject(new JProperty("@PARAM_4", message)));
            list.Add(new JObject(new JProperty("@PARAM_5", status)));
            list.Add(new JObject(new JProperty("@PARAM_6", refno)));
            list.Add(new JObject(new JProperty("@PARAM_7", sequence)));

            int eff = oDB.queryUpdateWithParam(sql, list);
        }
    }
}
