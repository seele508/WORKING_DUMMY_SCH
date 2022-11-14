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
    public class WebServicesSwiftAdapter
    {
        public WebServiceRespondsSwiftAd SendDataToSwiftAdapter(WebServiceRequestSwiftAd dataReq, string url)
        {
            //WebServiceResponds responds = new WebServiceResponds();
            WebServiceRespondsSwiftAd responds = new WebServiceRespondsSwiftAd();
            
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

            try {

                JObject hasilSendReq = new JObject();

                string dataRequest = "username=" + dataReq.username + "&" +
                    //"password=" +dataReq.password + "&" +
                    "password=" + dataReq.gembok + "&" +
                    "transactionSource=" + dataReq.transactionSource
                    ;
                hasilSendReq = postToWebservice(dataRequest, url);

                if (hasilSendReq != null)
                {
                    string respCode = hasilSendReq.GetValue("statusCode").ToString();
                    string respDesc = hasilSendReq.GetValue("statusDesc").ToString();
                    string respTotaldata = hasilSendReq.GetValue("totalData").ToString();

                    //Insert dulu ke Respond
                    responds.statusCode = respCode;
                    responds.statusDesc = respDesc;
                    responds.totaldata = respTotaldata;

                    string tempTransData = hasilSendReq.GetValue("transactionData").ToString();
                    //var resTransData = JsonConvert.DeserializeObject<List<TransactionData>>(tempTransData);
                    var resTransData = JsonConvert.DeserializeObject<List<TransactionDataSwiftAdapter>>(tempTransData);

                    for (int i = 0; i < resTransData.Count; i++)
                    {
                        SenderBIC = resTransData[i].SenderBIC;
                        ReceiverBIC = resTransData[i].ReceiverBIC;
                        NewSeqno = resTransData[i].NewSeqno;
                        RowID = resTransData[i].RowID;

                        TrxAmount = resTransData[i].TrxAmount;
                        TrxCurrency = resTransData[i].TrxCurrency;
                        TrxDate = resTransData[i].TrxDate;
                        BenefAccount = resTransData[i].BenefAccount;

                        BenefName = resTransData[i].BenefName;
                        ProcessId = resTransData[i].ProcessId;
                        MTtype = resTransData[i].MTtype;
                        TAG20_101 = resTransData[i].TAG20_101;

                        TAG20 = resTransData[i].TAG20;
                        Description = resTransData[i].Description;
                        BrinetFullName = resTransData[i].BrinetFullName;
                        BrinetName = resTransData[i].BrinetName;

                        FullMT = resTransData[i].FullMT;
                        Depcor = resTransData[i].Depcor;
                        Tag53 = resTransData[i].Tag53;
                        Tag54 = resTransData[i].Tag54;

                        TrxSource = resTransData[i].TrxSource;
                        CancelMTType = resTransData[i].CancelMTType;
                        CancelFeeAmount = resTransData[i].CancelFeeAmount;
                        CancelFeeCurrency = resTransData[i].CancelFeeCurrency;

                        SenderBicIsDepcor = resTransData[i].SenderBicIsDepcor;
                        Tag56 = resTransData[i].Tag56;
                        SwiftAdapterStatus = resTransData[i].SwiftAdapterStatus;

                        TransactionDataSwiftAdapter finalTD = new TransactionDataSwiftAdapter(SenderBIC, ReceiverBIC, NewSeqno, RowID,
                           TrxAmount, TrxCurrency, TrxDate, BenefAccount,
                           BenefName, ProcessId, MTtype, TAG20_101,
                           TAG20, Description, BrinetFullName, BrinetName,
                           FullMT, Depcor, Tag53, Tag54,
                           TrxSource, CancelMTType, CancelFeeAmount, CancelFeeCurrency,
                           SenderBicIsDepcor, Tag56, SwiftAdapterStatus);
                        
                        //responds.transactiondata.Add(finalTD);
                        responds.transactiondataSwift.Add(finalTD);
                    }

                }
                else
                {
                    responds.statusCode = "404";
                    responds.statusDesc = "Exception";
                    responds.totaldata = "0";
                    TransactionDataSwiftAdapter finalTD = new TransactionDataSwiftAdapter(SenderBIC, ReceiverBIC, NewSeqno, RowID,
                        TrxAmount, TrxCurrency, TrxDate, BenefAccount,
                        BenefName, ProcessId, MTtype, TAG20_101,
                        TAG20, Description, BrinetFullName, BrinetName,
                        FullMT, Depcor, Tag53, Tag54,
                        TrxSource, CancelMTType, CancelFeeAmount, CancelFeeCurrency,
                        SenderBicIsDepcor, Tag56, SwiftAdapterStatus);
                    //responds.transactiondata.Add(finalTD);
                    responds.transactiondataSwift.Add(finalTD);
                }
            }
            catch (Exception ex)
            {
                responds.statusCode = "404";
                //responds.statusDesc = "Not Found";
                responds.statusDesc = "Exception";
                responds.totaldata = "0";
                TransactionDataSwiftAdapter finalTD = new TransactionDataSwiftAdapter(SenderBIC, ReceiverBIC, NewSeqno, RowID,
                        TrxAmount, TrxCurrency, TrxDate, BenefAccount,
                        BenefName, ProcessId, MTtype, TAG20_101,
                        TAG20, Description, BrinetFullName, BrinetName,
                        FullMT, Depcor, Tag53, Tag54,
                        TrxSource, CancelMTType, CancelFeeAmount, CancelFeeCurrency,
                        SenderBicIsDepcor, Tag56, SwiftAdapterStatus);
                //responds.transactiondata.Add(finalTD);
                responds.transactiondataSwift.Add(finalTD);
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
