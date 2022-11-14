using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchTracer
{
    public class MenuSchParam
    {
        public string sch_function { get; set; }
 
        public string repeat_daily_limit { get; set; }
  
        public string start_time { get; set; }

        public string stop_time { get; set; }

     
        public string weekend_check { get; set; }
        public string maxthread { get; set; }
        public string max_query_result { get; set; }
        public string max_retry { get; set; }
        
    }
}
