using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CHITERP.Models
{
    [Table("TRANSACTION_RECEIPT_DETAIL")]
    public class TransactionReceiptDetail
    {
        [Key]
        public int TRANRID { get; set; }
        public int TRANDID { get; set; }
        public int TRANDREFID { get; set; }
        public string TRANRREFNAME { get; set; }
        public int RTID { get; set; }
        public int TRANMID { get; set; }
        public string TRANRREFNO { get; set; }
        public DateTime TRANRREFDATE { get; set; }
        public decimal TRANRAMT { get; set; }
        public decimal TRANR_BAL_AMT { get; set; }
        public int BTYPE { get; set; }
    }
}

