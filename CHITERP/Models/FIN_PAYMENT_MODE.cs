using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CHITERP.Models
{
    [Table("fin_PAYMENT_MODE")]
    public class FIN_PAYMENT_MODE
    {
        [Key]
        public int FinPymtModeID { get; set; }
        public string FinPymtModeDESC { get; set; }
        public string CUSRID { get; set; }
        public string LMUSRID { get; set; }
        public short DISPSTATUS { get; set; }
        public DateTime PRCSDATE { get; set; }
    }
}