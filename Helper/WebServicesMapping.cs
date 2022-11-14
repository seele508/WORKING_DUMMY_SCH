using Helper.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public class WebServicesMapping
    {
        public WebServiceResponds SendDataDetail(WebServiceRequests dataReq, string url)
        {
            WebServiceResponds responds = new WebServiceResponds();
            //TransactionData setTransaData = new TransactionData();
            string SenderBIC = "";
            string ReceiverBIC = "";
            string NewSeqno = "";
            string RowID = "";
            string TrxAmount = "";
            string TrxCurrency = "";
            string TrxDate = "";
            string BenefAccount = "";
            string BenefName = "";
            string ProcessId = "";
            string MTtype = "";
            string TAG20_101 = "";
            string TAG20 = "";
            string Description = "";
            string BrinetFullName = "";
            string BrinetName = "";
            string FullMT = "";
            string Depcor = "";
            string Tag53 = "";
            string Tag54 = "";
            string TrxSource = "";
            string CancelMTType = "";
            string CancelFeeAmount = "";
            string CancelFeeCurrency = "";
            string SenderBicIsDepcor = "";
            string Tag56 = "";
            string SwiftAdapterStatus = "";
            string TrxType = "";
            string NostroAccount = "";

            try
            {
                

                JObject hasilSendReq = new JObject();

                string dataRequest = "username=" + dataReq.username + "&"+                    
                    "password=" + dataReq.gembok + "&" +
                    "refno=" +dataReq.refno + "&"+
                    "transactionSource=" +dataReq.transactionSource
                    ;
                hasilSendReq = postToWebservice(dataRequest, url);

                if (hasilSendReq != null)
                {
                    string respCode = hasilSendReq.GetValue("statusCode").ToString();
                    string respDesc = hasilSendReq.GetValue("statusDesc").ToString();
                    string respTotaldata = hasilSendReq.GetValue("totalData").ToString();
                    string tempTransData = hasilSendReq.GetValue("transactionData").ToString();

                    var resTransData = JsonConvert.DeserializeObject<List<TransactionData>>(tempTransData);

                    foreach (var TD in resTransData)
                    {
                        SenderBIC = TD.SenderBIC;
                        //setTransaData.SenderBIC = TD.SenderBIC;
                        ReceiverBIC = TD.ReceiverBIC;
                        NewSeqno = TD.NewSeqno;
                        RowID = TD.RowID;

                        TrxAmount = TD.TrxAmount;
                        TrxCurrency = TD.TrxCurrency;
                        TrxDate = TD.TrxDate;
                        BenefAccount = TD.BenefAccount;

                        BenefName = TD.BenefName;
                        ProcessId = TD.ProcessId;
                        MTtype = TD.MTtype;
                        TAG20_101 = TD.TAG20_101;

                        TAG20 = TD.TAG20;
                        Description = TD.Description;
                        BrinetFullName = TD.BrinetFullName;
                        BrinetName = TD.BrinetName;

                        FullMT = TD.FullMT;
                        Depcor = TD.Depcor;
                        Tag53 = TD.Tag53;
                        Tag54 = TD.Tag54;

                        TrxSource = TD.TrxSource;
                        CancelMTType = TD.CancelMTType;
                        CancelFeeAmount = TD.CancelFeeAmount;
                        CancelFeeCurrency = TD.CancelFeeCurrency;

                        SenderBicIsDepcor = TD.SenderBicIsDepcor;
                        Tag56 = TD.Tag56;

                        SwiftAdapterStatus = TD.SwiftAdapterStatus;
                        TrxType = TD.TrxType;
                        NostroAccount = TD.NostroAccount;
                    }

                    responds.statusCode = respCode;
                    responds.statusDesc = respDesc;
                    responds.totaldata = respTotaldata;
                    TransactionData finalTD = new TransactionData(SenderBIC, ReceiverBIC, NewSeqno, RowID, 
                        TrxAmount, TrxCurrency, TrxDate, BenefAccount, 
                        BenefName, ProcessId, MTtype, TAG20_101, 
                        TAG20, Description, BrinetFullName, BrinetName,
                        FullMT, Depcor, Tag53, Tag54,
                        TrxSource, CancelMTType, CancelFeeAmount, CancelFeeCurrency,
                        SenderBicIsDepcor, Tag56, SwiftAdapterStatus, TrxType, NostroAccount);
                    responds.transactiondata.Add(finalTD);
                }
                else 
                {
                    responds.statusCode = "404";
                    responds.statusDesc = "Exception";
                    responds.totaldata = "0";
                    TransactionData finalTD = new TransactionData(SenderBIC, ReceiverBIC, NewSeqno, RowID,
                        TrxAmount, TrxCurrency, TrxDate, BenefAccount,
                        BenefName, ProcessId, MTtype, TAG20_101,
                        TAG20, Description, BrinetFullName, BrinetName,
                        FullMT, Depcor, Tag53, Tag54,
                        TrxSource, CancelMTType, CancelFeeAmount, CancelFeeCurrency,
                        SenderBicIsDepcor, Tag56, SwiftAdapterStatus, TrxType, NostroAccount);
                    responds.transactiondata.Add(finalTD);
                }

            }
            catch (Exception ex)
            {
                responds.statusCode = "404";
                //responds.statusDesc = "Not Found";
                responds.statusDesc = "Exception";
                responds.totaldata = "0";
                TransactionData finalTD = new TransactionData(SenderBIC, ReceiverBIC, NewSeqno, RowID,
                        TrxAmount, TrxCurrency, TrxDate, BenefAccount,
                        BenefName, ProcessId, MTtype, TAG20_101,
                        TAG20, Description, BrinetFullName, BrinetName,
                        FullMT, Depcor, Tag53, Tag54,
                        TrxSource, CancelMTType, CancelFeeAmount, CancelFeeCurrency,
                        SenderBicIsDepcor, Tag56, SwiftAdapterStatus, TrxType, NostroAccount);
                responds.transactiondata.Add(finalTD);
            }
            return responds;
        }

        public static JObject postToWebservice(string data, string url)
        {
            JObject obj = null;

            try
            {
                String sUrl = "";

                if (data != null)
                {
                    System.Net.ServicePointManager.DefaultConnectionLimit = 100;

                    sUrl = url;

                    String sParam = data;

                    using (WebClient wc = new WebClient())
                    {
                        wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                        string HTMLResult = wc.UploadString(sUrl, sParam);
                        obj = JObject.Parse(HTMLResult);
                        wc.Dispose();
                    }

                }
            }
            catch (Exception ex)
            { 

            }
            return obj;
        }
    }
}
