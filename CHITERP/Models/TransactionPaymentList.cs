using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace CHITERP.Models
{
    [Table("TransactionPaymentList")]
    public class TransactionPaymentList
    {
        [Key]
        public int PaymentListID { get; set; }        
        public int ProductTypeTypeID { get; set; }
        public int ProductID { get; set; }
        public int TransactionCategoryTypeID { get; set; }
        public int TransactionTypeTypeID { get; set; }
        public int TransactionModeTypeID { get; set; }

        public DateTime GenerratedDate { get; set; }
        public Nullable<double> Amount { get; set; }
        public Nullable<double> GeneratedAmount { get; set; }
        public Nullable<DateTime> ChangedDate { get; set; }
        
        
        public Nullable<int> ApprovedBy { get; set; }
        public Nullable<double> ApprovedAmount { get; set; }
        public Nullable<DateTime> ApprovedDate { get; set; }
        public string Remarks { get; set; }
        public Nullable<int> TokenNumber { get; set; }
        public Nullable<int> Installment { get; set; }
        public int StatusTypeID { get; set; }
        public Nullable<int> OfficeID { get; set; }
        public string collectionagent { get; set; }
        //public Nullable<int> RouteID { get; set; }
        //public Nullable<int> AssignedToEmployeeID { get; set; }
        //public Nullable<byte> BookCollectionRequired { get; set; }

        public string ChequeNo { get; set; }
        public Nullable<DateTime> ChequeDate { get; set; }

        public string ChequeBankName { get; set; }
        public Nullable<DateTime> ChequeRealizedDate { get; set; }
        public Nullable<int> FundAccountID { get; set; }
        public Nullable<int> BkVfcnSts { get; set; }
        public Nullable<DateTime> BkVfcnDt { get; set; }
        public Nullable<decimal> BkVfcnAmt { get; set; }

        public string BkVfcnRemks { get; set; }
        public Nullable<int> GSTReq { get; set; }
        public Nullable<decimal> GSTPerc { get; set; }
        public Nullable<decimal> GSTAmt { get; set; }
        public Nullable<decimal> GrossAmt { get; set; }

        //public string TallyCode { get; set; }

    }
}