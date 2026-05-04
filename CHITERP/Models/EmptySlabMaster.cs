using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CHITERP.Models
{
    [Table("EMPTY_SLABMASTER")]
    public class EmptySlabMaster
    {
        [Key]
        public int SLABMID { get; set; }

        public int COMPYID { get; set; }
        public DateTime SLABMDATE { get; set; }

        public int  SLABTID { get; set; }

        public int TARIFFMID { get; set; }

        public int STMRID { get; set; }

        public Int16 CHRGETYPE { get; set; }

        public int CONTNRSID { get; set; }

        public Int16 SDTYPE { get; set; }

        public Int16 YRDTYPE { get; set; }

        public Int16 HTYPE { get; set; }

        public Int16 WTYPE { get; set; }

        public int SDPTTYPEID { get; set; }

        public decimal SLABMIN { get; set; }

        public decimal SLABMAX { get; set; }

        public decimal SLABAMT { get; set; }

        public string CUSRID { get; set; }

        public string LMUSRID { get; set; }

        public Int16 DISPSTATUS { get; set; }

        public DateTime PRCSDATE { get; set; }

    }
}