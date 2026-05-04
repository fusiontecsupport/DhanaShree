using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CHITERP.Models
{
    [Table("REPAIRPARTMASTER")]
    public class RepairPartMaster
    {
        [Key]
        public int RPRTID { get; set; }

        public string RPRTDESC { get; set; }

        public string RPRTCODE { get; set; }

        public int STMRID { get; set; }

        public int COMPTID { get; set; }

        public int RPRID { get; set; }

        public string CUSRID { get; set; }

        public string LMUSRID { get; set; }

        public Int16 DISPSTATUS { get; set; }

        public DateTime PRCSDATE { get; set; }
    }
}