using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CHITERP.Models
{
    [Table("REPAIRTRANSACTIONDETAIL")]
    public class RepairTransactionDetail
    {
        [Key]
        public int TRANDID { get; set; }

        public int TRANMID { get; set; }

        public int TRANDREFID { get; set; }

        public string TRANDREFNO { get; set; }

        public string TRANDREFNAME { get; set; }

        public string TRANDREFDESC { get; set; }

        public int LOCTID { get; set; }

        public int COMPTID { get; set; }

        public int DMGID { get; set; }

        public int RPRID { get; set; }

        public decimal TRANDQTY { get; set; }

        public decimal TRANDRATE { get; set; }

        public decimal TRANDWGHT { get; set; }

        public decimal TRANDHRS { get; set; }

        public decimal TRANDHAMT { get; set; }

        public decimal TRANDLAMT { get; set; }

        public decimal TRANDMAMT { get; set; }

        public decimal TRANDGAMT { get; set; }

        public decimal TRANDTEXPRN { get; set; }

        public decimal TRANDTAMT { get; set; }

        public decimal TRANDNAMT { get; set; }

        public int TRANDAID { get; set; }

        public int TRANDGIDID { get; set; }

        public int TRANDLMID { get; set; }

        public string TRANDLMNO { get; set; }

        public Nullable<DateTime> TRANDLMDATE { get; set; }

        public int RESPID { get; set; }

        public int TGRADEID { get; set; }

        public int LRPRID { get; set; }

        public int RBID { get; set; }

        public decimal TRANDLENGTH { get; set; }

        public decimal TRANDWIDTH { get; set; }

        public string TRANDUNITCODE { get; set; }

        public decimal TRANDCWCAMT { get; set; }

        public decimal TRANDLTAXAMT { get; set; }

        public decimal TRANDMTAXAMT { get; set; }

        public int CSID { get; set; }

        public Int16 DISPSTATUS { get; set; }

    }
}