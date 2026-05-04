using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CHITERP.Models
{
    [Table("REPAIRTRANSACTIONMASTER")]
    public class RepairTransactionMaster
    {
        [Key]
        public int TRANMID { get; set; }

        public int COMPYID { get; set; }

        public int SDPTID { get; set; }

        public int REGSTRID { get; set; }

        public Int16 TRANBTYPE { get; set; }

        public DateTime TRANDATE { get; set; }

        public DateTime TRANTIME { get; set; }

        public int TRANNO { get; set; }

        public string TRANDNO { get; set; }

        public int TRANREFID { get; set; }

        public string TRANREFNAME { get; set; }

        public int GIDID { get; set; }

        public Nullable<DateTime> TRANMFGDATE { get; set; }
      
        public string TRANAMTWRDS { get; set; }

        public int TRANLMID { get; set; }

        public string TRANLMNO { get; set; }

        public Nullable<DateTime> TRANLMDATE { get; set; }

        public string TRANRMKS { get; set; }

        public int TRANPCOUNT { get; set; }

        public string TRANRMKS2 { get; set; }

        public string TRANRMKS3 { get; set; }

        public int TCNTNRSID { get; set; }

        public Nullable<DateTime> TAVDATE { get; set; }

        public string TAVDESC { get; set; }

        public int CCNTNRSID { get; set; }

        public Nullable<DateTime> CAVDATE { get; set; }

        public string CAVDESC { get; set; }

        public string TRANSURNAME { get; set; }

        public string TRANCSCDESC { get; set; }

        public int ESTTID { get; set; }

        public string TRANHDESC { get; set; }

        public string TRANCHANAME { get; set; }

        public string TRANIMPNAME { get; set; }

        public int CURNID { get; set; }

        public decimal TRANCUREXPRN { get; set; }

        public decimal TRANCGSTEXPRN { get; set; }

        public decimal TRANSGSTEXPRN { get; set; }

        public decimal TRANIGSTEXPRN { get; set; }

        public decimal TRANCGSTAMT { get; set; }

        public decimal TRANSGSTAMT { get; set; }

        public decimal TRANIGSTAMT { get; set; }

        public decimal TRANGAMT { get; set; }

        public decimal TRANNAMT { get; set; }

        public string CUSRID { get; set; }

        public string LMUSRID { get; set; }

        public Int16 DISPSTATUS { get; set; }

        public Nullable<DateTime> PRCSDATE { get; set; }

    }
}