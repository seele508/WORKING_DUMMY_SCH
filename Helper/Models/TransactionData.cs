using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper.Models
{
    public class TransactionData
    {
        
        public string SenderBIC { get; set; }
        public string ReceiverBIC { get; set; }
        public string NewSeqno { get; set; }
        public string RowID { get; set; }
        public string TrxAmount { get; set; }
        public string TrxCurrency { get; set; }
        public string TrxDate { get; set; }
        public string BenefAccount { get; set; }
        public string BenefName { get; set; }
        public string ProcessId { get; set; }
        public string MTtype { get; set; }
        public string TAG20_101 { get; set; }
        public string TAG20 { get; set; }
        public string Description { get; set; }
        public string BrinetFullName { get; set; }
        public string BrinetName { get; set; }
        public string FullMT { get; set; }
        public string Depcor { get; set; }
        public string Tag53 { get; set; }
        public string Tag54 { get; set; }
        public string TrxSource { get; set; }
        public string CancelMTType { get; set; }
        public string CancelFeeAmount { get; set; }
        public string CancelFeeCurrency { get; set; }
        public string SenderBicIsDepcor { get; set; }
        public string Tag56 { get; set; }
        public string SwiftAdapterStatus { get; set; }
        public string TrxType { get; set; }
        public string NostroAccount { get; set; }
        

        public TransactionData(String SenderBIC, String ReceiverBIC, String NewSeqno, String RowID, 
            String TrxAmount, String TrxCurrency, String TrxDate, String BenefAccount, 
            String BenefName, String ProcessId, String MTtype, String TAG20_101, 
            String TAG20, String Description, String BrinetFullName, String BrinetName, 
            String FullMT, String Depcor, String Tag53, String Tag54, 
            String TrxSource, String CancelMTType, String CancelFeeAmount, String CancelFeeCurrency, 
            String SenderBicIsDepcor, String Tag56, String SwiftAdapterStatus, String TrxType, String NostroAccount)
        {
            this.SenderBIC = SenderBIC;
            this.ReceiverBIC = ReceiverBIC;
            this.NewSeqno = NewSeqno;
            this.RowID = RowID;

            this.TrxAmount = TrxAmount;
            this.TrxCurrency = TrxCurrency;
            this.TrxDate = TrxDate;
            this.BenefAccount = BenefAccount;

            this.BenefName = BenefName;
            this.ProcessId = ProcessId;
            this.MTtype = MTtype;
            this.TAG20_101 = TAG20_101;

            this.TAG20 = TAG20;
            this.Description = Description;
            this.BrinetFullName = BrinetFullName;
            this.BrinetName = BrinetName;

            this.FullMT = FullMT;
            this.Depcor = Depcor;
            this.Tag53 = Tag53;
            this.Tag54 = Tag54;

            this.TrxSource = TrxSource;
            this.CancelMTType = CancelMTType;
            this.CancelFeeAmount = CancelFeeAmount;
            this.CancelFeeCurrency = CancelFeeCurrency;

            this.SenderBicIsDepcor = SenderBicIsDepcor;
            this.Tag56 = Tag56;

            this.SwiftAdapterStatus = SwiftAdapterStatus;
            this.TrxType = TrxType;
            this.NostroAccount = NostroAccount;
        }

        public String getSenderBIC()
        {
            return SenderBIC;
        }

        public String getReceiverBIC()
        {
            return ReceiverBIC;
        }
        public String getNewSeqno()
        {
            return NewSeqno;
        }
        public String getRowID()
        {
            return RowID;
        }
        public String getTrxAmount()
        {
            return TrxAmount;
        }
        public String getTrxCurrency()
        { 
            return TrxCurrency;
        }
        public String getTrxDate()
        {
            return TrxDate;
        }
        public String getBenefAccount()
        {
            return BenefAccount;
        }
        public String getBenefName()
        {
            return BenefName;
        }
        public String getProcessId()
        {
            return ProcessId;
        }
        public String getMTtype()
        {
            return MTtype;
        }
        public String getTAG20_101()
        {
            return TAG20_101;
        }
        public String getTAG20()
        {
            return TAG20;
        }
        public String getDescription()
        {
            return Description;
        }
        public String getBrinetFullName()
        {
            return BrinetFullName;
        }
        public String getBrinetName()
        {
            return BrinetName;
        }
        public String getFullMT()
        {
            return FullMT;
        }
        public String getDepcor()
        {
            return Depcor;
        }
        public String getTag53()
        {
            return Tag53;
        }
        public String getTag54()
        {
            return Tag54;
        }
        public String getTrxSource()
        {
            return TrxSource;
        }
        public String getCancelMTType()
        {
            return CancelMTType;
        }
        public String getCancelFeeAmount()
        {
            return CancelFeeAmount;
        }
        public String getCancelFeeCurrency()
        {
            return CancelFeeCurrency;
        }
        public String getSenderBicIsDepcor()
        {
            return SenderBicIsDepcor;
        }
        public String getTag56()
        {
            return Tag56;
        }
        public String getSwiftAdapterStatus()
        {
            return SwiftAdapterStatus;
        }
        public String getTrxType()
        {
            return TrxType;
        }
        public String getNostroAccount()
        {
            return NostroAccount;
        }

    }
}
