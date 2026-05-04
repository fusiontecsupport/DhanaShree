using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CHITERP.Models
{
    [Table("TransactionCollectionAllocation")]
    public class TransactionCollectionAllocation
    {
        [Key]
        public int CollectionAllocationID { get; set; }
        public int CollectionReceiptID { get; set; }
        public int FundAccountID { get; set; }
        public decimal Amount { get; set; }
        public int StatusTypeID { get; set; }
    }
}