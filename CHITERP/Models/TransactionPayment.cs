using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CHITERP.Models
{
    [Table("TransactionPayment")]
    public class TransactionPayment
    {
        [Key]
        public int PaymentID { get; set; }
        public int PaymentListID { get; set; }
        public int ProductID { get; set; }
        public int TransactionTypeTypeID { get; set; }
        public int TransactionModeTypeID { get; set; }
        public DateTime PaymentDateTime { get; set; }

        public decimal PaidAmount { get; set; }
        public int PaidByEmployeeID { get; set; }

        public string Remarks { get; set; }
        public string VoucherCode { get; set; }
        public string VoucherBookNo { get; set; }
        public int TransactionCategoryTypeID { get; set; }
        public int StatusTypeID { get; set; }
        public string ChequeNo { get; set; }
        public Nullable<DateTime> ChequeDate { get; set; }

        public string ChequeBankName { get; set; }
        public Nullable<DateTime> ChequeRealizedDate { get; set; }
        public Nullable<int> FundAccountID { get; set; }
    }
}