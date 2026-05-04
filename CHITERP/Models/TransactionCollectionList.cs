using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CHITERP.Models
{
    [Table("TransactionCollectionList")]
    public class TransactionCollectionList
    {
        [Key]
        public int CollectionListID { get; set; }
        public Nullable<int> TokenNumber { get; set; }
        public int ProductTypeID { get; set; }
        public int ProductID { get; set; }
        public int TransactionCategoryTypeID { get; set; }
        public int TransactionTypeTypeID { get; set; }
        public int TransactionModeTypeID { get; set; }
        public double AmountDue { get; set; }
        public Nullable<double> PendingAmount { get; set; }
        public Nullable<double> GeneratedAmount { get; set; }
        public DateTime GeneratedDate { get; set; }
        public Nullable<int> ApprovedByEmployeeID { get; set; }
        public Nullable<DateTime> ApprovedDateTime { get; set; }
        public string Remarks { get; set; }
        public Nullable<int> RouteID { get; set; }
        public Nullable<int> AssignedToEmployeeID { get; set; }
        public Nullable<byte> BookCollectionRequired { get; set; }
        public Nullable<int> Installment { get; set; }

        public int StatusTypeID { get; set; }
        public string TallyCode { get; set; }
        public Nullable<int> OfficeID { get; set; }

        public string collectionagent { get; set; }
        public string ChequeNo { get; set; }
        public Nullable<DateTime> ChequeDate { get; set; }

        public string ChequeBankName { get; set; }
        public Nullable<int> BkVfcnSts { get; set; }
        public Nullable<DateTime> BkVfcnDt { get; set; }
        public Nullable<decimal> BkVfcnAmt { get; set; }

        public string BkVfcnRemks { get; set; }
        public Nullable<int> GSTReq { get; set; }
        public Nullable<decimal> GSTPerc { get; set; }
        public Nullable<decimal> GSTAmt { get; set; }
        public Nullable<decimal> GrossAmt { get; set; }

    }
}