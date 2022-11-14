using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace Helper
{
    public class DBSchParameterV2
    {
        private DBConnection oDB;

        //valid sch param
        private bool isValidParam;
        private string message;

        //sch param
        private string _SCH_NAME;
        private string _SCH_FUNCTION;

        private int _REPEAT_DAILY_LIMIT;
        private string _START_TIME;
        private string _STOP_TIME;

        private int _WEEKEND_CHECK;
        private int _MAX_THREAD;
        private int _MAX_QUERY_RESULT;
        private int _MAX_RETRY;


        List<String> listSchName;
        List<String> listSchFunc;
        List<int> listRepeatDL;
        List<string> listStartTime;
        List<string> listStopTime;
        List<int> listWeekend;
        List<int> listMaxThread;
        List<int> listMaxQueryR;
        List<int> listMaxRetry;

        private string[] AR_SCH_FUNC;

        public DBSchParameterV2(DBConnection oDB, string schName)
        {
            this.oDB = oDB;
            isValidParam = true;
            try
            {
                string sql = "SELECT *, DATE(LAST_RUN) as TGL FROM md_scheduler_activity WHERE SCH_NAME = @PARAM_0 ";

                var list = new List<JObject>();
                list.Add(new JObject(new JProperty("@PARAM_0", schName)));

                DataTable dataParam = oDB.querySelectExtendedWithParam(sql, list);

                listSchName = new List<string>();
                listSchFunc = new List<string>();
                listRepeatDL = new List<int>();
                listStartTime = new List<string>();
                listStopTime = new List<string>();
                listWeekend = new List<int>();
                listMaxThread = new List<int>();
                listMaxQueryR = new List<int>();
                listMaxRetry = new List<int>();


                if (dataParam != null)
                {
                    /*
                    int total = dataParam.Rows.Count;
                    for (int i = 0; i < total; i++)
                    {
                    //var stringArr = dataParam.Rows[i].ItemArray.Select(x => x.ToString().ToArray());
                    var stringArr = dataParam.Rows[i].ItemArray.Select(x => x.ToString());
        
                        //var schfunction = stringArr.Select(l =>l.SCH_FUNCTION).
                    }
                    */

                    int total = dataParam.Rows.Count;
                    if (total > 0)
                    {
                        foreach (DataRow drParam in dataParam.Rows)
                        {

                            _SCH_NAME = drParam["SCH_NAME"].ToString().Trim();
                            if (string.IsNullOrEmpty(_SCH_NAME))
                            {
                                isValidParam = false;
                                message = "Invalid SCH_NAME " + _SCH_NAME;
                            }
                            else
                            {
                                listSchName.Add(_SCH_NAME);
                            }


                            _SCH_FUNCTION = drParam["SCH_FUNCTION"].ToString().Trim();
                            if (string.IsNullOrEmpty(_SCH_FUNCTION))
                            {
                                isValidParam = false;
                                message = "Invalid SCH_FUNCTION " + _SCH_FUNCTION;
                            }
                            else
                            {
                                listSchFunc.Add(_SCH_FUNCTION);
                            }

                            if (!Int32.TryParse(drParam["REPEAT_DAILY_LIMIT"].ToString(), out _REPEAT_DAILY_LIMIT))
                            {
                                isValidParam = false;
                                message = "Invalid REPEAT_DAILY_LIMIT Value : " + _REPEAT_DAILY_LIMIT;
                            }
                            else
                            {
                                listRepeatDL.Add(_REPEAT_DAILY_LIMIT);
                            }

                            var regex = new Regex(@"(?:[01]\d|2[0123]):(?:[012345]\d):(?:[012345]\d)");

                            //start time
                            _START_TIME = drParam["START_TIME"].ToString().Trim();
                            Match matchStart = regex.Match(_START_TIME);
                            if (!matchStart.Success)
                            {
                                isValidParam = false;
                                message = "Invalid START_TIME Value : " + _START_TIME;
                            }
                            else
                            {
                                listStartTime.Add(_START_TIME);
                            }

                            //stop time
                            _STOP_TIME = drParam["STOP_TIME"].ToString().Trim();
                            Match matchStop = regex.Match(_STOP_TIME);
                            if (!matchStop.Success)
                            {
                                isValidParam = false;
                                message = "Invalid STOP_TIME Value : " + _STOP_TIME;
                            }
                            else
                            {
                                listStopTime.Add(_STOP_TIME);
                            }

                            //weekend
                            if (Int32.TryParse(drParam["WEEKEND_CHECK"].ToString(), out _WEEKEND_CHECK))
                            {
                                if (_WEEKEND_CHECK != 0 && _WEEKEND_CHECK != 1)
                                {
                                    isValidParam = false;
                                    message = "Invalid WEEKEND_CHECK Value : " + _WEEKEND_CHECK;
                                }
                                else
                                {
                                    listWeekend.Add(_WEEKEND_CHECK);
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
                            else
                            {
                                listMaxThread.Add(_MAX_THREAD);
                            }

                            //max query                         
                            if (!Int32.TryParse(drParam["MAX_QUERY_RESULT"].ToString(), out _MAX_QUERY_RESULT))
                            {
                                isValidParam = false;
                                message = "Invalid MAX_QUERY_RESULT Value : " + _MAX_QUERY_RESULT;
                            }
                            else
                            {
                                listMaxQueryR.Add(_MAX_QUERY_RESULT);
                            }

                            //max retry                         
                            if (!Int32.TryParse(drParam["MAX_RETRY"].ToString(), out _MAX_RETRY))
                            {
                                isValidParam = false;
                                message = "Invalid MAX_RETRY Value : " + _MAX_RETRY;
                            }
                            else
                            {
                                listMaxRetry.Add(_MAX_RETRY);
                            }
                        }
                    }
                    else
                    {
                        isValidParam = false;
                        message = "No parameter data found for " + schName;
                    }

                }
                else
                {
                    isValidParam = false;
                    message = "No parameter data found for " + schName;
                }
            }
            catch (Exception e)
            {
                isValidParam = false;
                message = "Failed parse parameter value SCH_NAME" + schName;
            }
        }

        public List<String> getSchName()
        {
            return listSchName;
        }

        public List<String> getSchFunc()
        {
            return listSchFunc;
        }

        public List<int> getRepeatDL()
        {
            return listRepeatDL;
        }
        public List<string> getStartTime()
        {
            return listStartTime;
        }
        public List<string> getStopTime()
        {
            return listStopTime;
        }
        public List<int> getWeekend()
        {
            return listWeekend;
        }
        public List<int> getMaxThread()
        {
            return listMaxThread;
        }
        public List<int> getMaxQR()
        {
            return listMaxQueryR;
        }
        public List<int> getMaxRetry()
        {
            return listMaxRetry;
        }
    }
}
