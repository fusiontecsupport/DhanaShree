using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CHITERP.Models
{
    [Table("EMPTY_LEASING_REPAIRCODEMASTER")]
    public class EmptyLeasingRepairCodeMaster
    {
        [Key]
        public int LRPRID { get; set; }

        public string LRPRDESC { get; set; }

        public string LRPRCODE { get; set; }

        public string CUSRID { get; set; }

        public string LMUSRID { get; set; }

        public Int16 DISPSTATUS { get; set; }

        public DateTime PRCSDATE { get; set; }
    }
}