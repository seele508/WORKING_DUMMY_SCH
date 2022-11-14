using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper.Models
{
    public class WebServiceRespondsSwiftAd
    {
        public String statusCode = "";
        public String statusDesc = "";
        public String totaldata = "";
        //public String transactiondata = "";

        public List<TransactionDataSwiftAdapter> transactiondataSwift = new List<TransactionDataSwiftAdapter>();
    }
}
