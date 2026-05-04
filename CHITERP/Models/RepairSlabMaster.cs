using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CHITERP.Models
{
    [Table("REPAIRSLABMASTER")]
    public class RepairSlabMaster
    {

        [Key]
        public int RSLABMID { get; set; }

        public int COMPYID { get; set; }

        public DateTime RSLABMDATE { get; set; }

        public int STMRID { get; set; }

        public int RPRTID { get; set; }

        public decimal RSLABHRS { get; set; }

        public decimal RSLABHAMT { get; set; }

        public decimal RSLABMAMT { get; set; }

        public decimal RSLABTAMT { get; set; }

        public int TARIFFMID { get; set; }
              
        public string WRMMODE { get; set; }

        public string CUSRID { get; set; }

        public string LMUSRID { get; set; }

        public Int16 DISPSTATUS { get; set; }

        public DateTime PRCSDATE { get; set; }
    }
}