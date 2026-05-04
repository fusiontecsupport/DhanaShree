using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CHITERP.Models
{
    [Table("EMPTY_REPAIRCODEMASTER")]
    public class EmptyRepairCodeMaster
    {
        [Key]
        public int RPRID { get; set; }

        public string RPRDESC { get; set; }

        public string RPRCODE { get; set; }

        public string CUSRID { get; set; }

        public string LMUSRID { get; set; }

        public Int16 DISPSTATUS { get; set; }

        public DateTime PRCSDATE { get; set; }

    }
}