using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Helper
{
    public class Config
    {

        private bool debug = false;

        //[Obsolete]
        public Config()
        {
            try
            {
                //debug = Convert.ToBoolean(ConfigurationSettings.AppSettings["debug"]);
                debug = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["debug"]);
                //debug = true;
            }
            catch (FormatException fE)
            {
                Console.WriteLine(fE);
                debug = false;
            }
        }

        public bool isDebug()
        {
            return debug;
        }

        //[Obsolete]
        public String getParameter(String paramName)
        {
            String result = "";
            //result = ConfigurationSettings.AppSettings[paramName];
            result = System.Configuration.ConfigurationManager.AppSettings[paramName];
            if (result == null)
            {
                result = "";
            }
            return result;
        }

    }
}
