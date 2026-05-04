using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CHITERP.Models
{
    [Table("EMPTY_TARIFFMASTER")]
    public class EmptyTariffMaster
    {
        [Key]
        public int TARIFFMID { get; set; }

        public string TARIFFMDESC { get; set; }

        public string TARIFFMCODE { get; set; }

        public int SDPTID { get; set; }

        public string CUSRID { get; set; }

        public string LMUSRID { get; set; }

        public Int16 DISPSTATUS { get; set; }

        public DateTime PRCSDATE { get; set; }
    }
}