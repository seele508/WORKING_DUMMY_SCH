using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace Helper
{
    public class DBSchParameter
    {
        //db con
        private DBConnection oDB;

        //valid sch param
        private bool isValidParam;
        private string message;

        //sch param
        private string _SCH_NAME;
        private string _SCH_VERSION;
        private int _IS_RUN;
        private int _REPEAT_DAILY_LIMIT;
        private int _REPEAT_TODAY;
        private string _START_TIME;
        private string _STOP_TIME;
        private int _TIMER_INTERVAL_MILISECOND;
        private int _WEEKEND_CHECK;
        private int _MAX_THREAD;
        private int _MAX_QUERY_RESULT;
        private int _MAX_RETRY;
        private string _LAST_RUN;
        private string _LAST_RUN_V2;


        public DBSchParameter(DBConnection oDB, string schName, string schFunction)
        {
            this.oDB = oDB;
            isValidParam = true;

            try
            {
                string sql = "SELECT *, DATE(LAST_RUN) as TGL FROM md_scheduler_activity WHERE SCH_NAME = @PARAM_0 and SCH_FUNCTION = @PARAM_1 ";

                var list = new List<JObject>();
                list.Add(new JObject(new JProperty("@PARAM_0", schName)));
                list.Add(new JObject(new JProperty("@PARAM_1", schFunction)));

                DataTable dataParam = oDB.querySelectExtendedWithParam(sql, list);

                if (dataParam != null)
                {
                    int total = dataParam.Rows.Count;

                    if (total > 0)
                    {
                        foreach (DataRow drParam in dataParam.Rows)
                        {
                            //sch name
                            _SCH_NAME = drParam["SCH_NAME"].ToString().Trim();
                            if (string.IsNullOrEmpty(_SCH_NAME))
                            {
                                isValidParam = false;
                                message = "Invalid SCH_NAME " + _SCH_NAME;
                            }

                            //sch version
                            _SCH_VERSION = drParam["SCH_VERSION"].ToString().Trim();
                            if (string.IsNullOrEmpty(_SCH_VERSION))
                            {
                                isValidParam = false;
                                message = "Invalid SCH_VERSION " + _SCH_VERSION;
                            }

                            //is run
                            if (Int32.TryParse(drParam["IS_RUN"].ToString(), out _IS_RUN))
                            {
                                if (_IS_RUN != 0 && _IS_RUN != 1)
                                {
                                    isValidParam = false;
                                    message = "Invalid IS_RUN Value : " + _IS_RUN;
                                }
                            }
                            else
                            {
                                isValidParam = false;
                                message = "Invalid IS_RUN Value : " + _IS_RUN;
                            }

                            //daily limit                         
                            if (!Int32.TryParse(drParam["REPEAT_DAILY_LIMIT"].ToString(), out _REPEAT_DAILY_LIMIT))
                            {
                                isValidParam = false;
                                message = "Invalid REPEAT_DAILY_LIMIT Value : " + _REPEAT_DAILY_LIMIT;
                            }

                            //repeate today                         
                            if (!Int32.TryParse(drParam["REPEAT_TODAY"].ToString(), out _REPEAT_TODAY))
                            {
                                isValidParam = false;
                                message = "Invalid REPEAT_TODAY Value : " + _REPEAT_TODAY;
                            }

                            //regex time
                            var regex = new Regex(@"(?:[01]\d|2[0123]):(?:[012345]\d):(?:[012345]\d)");

                            //start time
                            _START_TIME = drParam["START_TIME"].ToString().Trim();
                            Match matchStart = regex.Match(_START_TIME);
                            if (!matchStart.Success)
                            {
                                isValidParam = false;
                                message = "Invalid START_TIME Value : " + _START_TIME;
                            }

                            //stop time
                            _STOP_TIME = drParam["STOP_TIME"].ToString().Trim();
                            Match matchStop = regex.Match(_STOP_TIME);
                            if (!matchStop.Success)
                            {
                                isValidParam = false;
                                message = "Invalid STOP_TIME Value : " + _STOP_TIME;
                            }

                            //timer interval                         
                            if (!Int32.TryParse(drParam["TIMER_INTERVAL_MILISECOND"].ToString(), out _TIMER_INTERVAL_MILISECOND))
                            {
                                isValidParam = false;
                                message = "Invalid TIMER_INTERVAL_MILISECOND Value : " + _TIMER_INTERVAL_MILISECOND;
                            }

                            //weekend check                         
                            if (Int32.TryParse(drParam["WEEKEND_CHECK"].ToString(), out _WEEKEND_CHECK))
                            {
                                if (_WEEKEND_CHECK != 0 && _WEEKEND_CHECK != 1)
                                {
                                    isValidParam = false;
                                    message = "Invalid WEEKEND_CHECK Value : " + _WEEKEND_CHECK;
                                }
                            }
                            else
                            {
                                isValidParam = false;
                                message = "Invalid WEEKEND_CHECK Value : " + _WEEKEND_CHECK;
                            }

                            //max thread                         
                            if (!Int32.TryParse(drParam["MAX_THREAD"].ToString(), out _MAX_THREAD))
                            {
                                isValidParam = false;
                                message = "Invalid MAX_THREAD Value : " + _MAX_THREAD;
                            }

                            //max query                         
                            if (!Int32.TryParse(drParam["MAX_QUERY_RESULT"].ToString(), out _MAX_QUERY_RESULT))
                            {
                                isValidParam = false;
                                message = "Invalid MAX_QUERY_RESULT Value : " + _MAX_QUERY_RESULT;
                            }

                            //max retry                         
                            if (!Int32.TryParse(drParam["MAX_RETRY"].ToString(), out _MAX_RETRY))
                            {
                                isValidParam = false;
                                message = "Invalid MAX_RETRY Value : " + _MAX_RETRY;
                            }

                            //last run
                            _LAST_RUN = drParam["TGL"].ToString().Trim();
                            if (string.IsNullOrEmpty(_LAST_RUN))
                            {
                                isValidParam = false;
                                message = "Invalid LAST_RUN " + _LAST_RUN;
                            }

                            //last run v2
                            _LAST_RUN_V2 = drParam["LAST_RUN"].ToString().Trim();
                            if (string.IsNullOrEmpty(_LAST_RUN_V2))
                            {
                                isValidParam = false;
                                message = "Invalid LAST_RUN " + _LAST_RUN_V2;
                            }

                        }
                    }
                    else
                    {
                        isValidParam = false;
                        message = "No parameter data found for " + schFunction;
                    }
                }
                else
                {
                    isValidParam = false;
                    message = "No parameter data found for " + schFunction;
                }
            }
            catch (Exception e)
            {
                isValidParam = false;
                message = "Failed parse parameter value " + schFunction;
            }
        }

        public bool isValidParameterValue()
        {
            return isValidParam;
        }

        public string getErrorMessage()
        {
            return message;
        }

        public string SCH_NAME()
        {
            return _SCH_NAME;
        }
        public string SCH_VERSION()
        {
            return _SCH_VERSION;
        }

        public int MAX_THREAD()
        {
            return _MAX_THREAD;
        }

        public int MAX_QUERY_RESULT()
        {
            return _MAX_QUERY_RESULT;
        }

        public int TIMER_INTERVAL_MILISECOND()
        {
            return _TIMER_INTERVAL_MILISECOND;
        }

        public int REPEAT_DAILY_LIMIT()
        {
            return _REPEAT_DAILY_LIMIT;
        }

        public int REPEAT_TODAY()
        {
            return _REPEAT_TODAY;
        }

        public void SET_REPEAT_TODAY(int hariIni)
        {
            _REPEAT_TODAY = hariIni;
        }

        public string LAST_RUN()
        {
            return _LAST_RUN;
        }

        public string LAST_RUN_V2()
        {
            return _LAST_RUN_V2;
        }

        public void SET_LAST_RUN(string hariIni)
        {
            _LAST_RUN = hariIni;
        }

        public string START_TIME()
        {
            return _START_TIME;
        }
        public string STOP_TIME()
        {
            return _STOP_TIME;
        }
        public int WEEKEND_CHECK()
        {
            return _WEEKEND_CHECK;
        }

        public int MAX_RETRY()
        {
            return _MAX_RETRY;
        }
    }
}
