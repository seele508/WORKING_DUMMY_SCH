using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper.Models
{
    public class WebServiceResponds
    {
        public String statusCode ="";
        public String statusDesc = "";
        public String totaldata = "";
        //public String transactiondata = "";

        public List<TransactionData> transactiondata = new List<TransactionData>();

    }
}
