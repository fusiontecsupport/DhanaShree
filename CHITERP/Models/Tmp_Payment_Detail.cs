using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CHITERP.Models
{
    [Table("TMP_PAYMENT_DETAIL")]
    public class Tmp_Payment_Detail
    {
        [Key]
        public string KUSRID { get; set; }
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