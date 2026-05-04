using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CHITERP.Models
{
    [Table("LEASINGSLABMASTER")]
    public class LeasingSlabMaster
    {
        [Key]
        public int LSLABMID { get; set; }

        public int COMPYID { get; set; }

        public DateTime LSLABMDATE { get; set; }

        public int TARIFFMID { get; set; }

        public int STMRID { get; set; }

        public int COMPTID { get; set; }

        public int DMGID { get; set; }

        public int LRPRID { get; set; }

        public int UNITID { get; set; }

        public decimal LLENGTH { get; set; }

        public decimal LWIDTH { get; set; }

        public decimal LSLABHRS { get; set; }

        public decimal LSLABAMT { get; set; }

        public string PRICECODE { get; set; }

        public string CUSRID { get; set; }

        public string LMUSRID { get; set; }

        public Int16 DISPSTATUS { get; set; }

        public DateTime PRCSDATE { get; set; }

    }
}