using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CHITERP.Models
{
    [Table("EMPTY_SLABTYPEMASTER")]
    public class EmptySlabTypeMaster
    {
        [Key]
        public int SLABTID { get; set; }

        public string SLABTDESC { get; set; }

        public string SLABTCODE { get; set; }

        public int DISPORDER { get; set; }

        public string CUSRID { get; set; }

        public string LMUSRID { get; set; }

        public Int16 DISPSTATUS { get; set; }

        public DateTime PRCSDATE { get; set; }
    }
}