using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CHITERP.Models
{
    [Table("TransactionPaymentAllocation")]
    public class TransactionPaymentAllocation
    {
        [Key]
        public int PaymentAllocationID { get; set; }
        public int PaymentID { get; set; }
        public int FundAccountID { get; set; }
        public decimal Amount { get; set; }
        public int StatusTypeID { get; set; }
    }
}