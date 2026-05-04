using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CHITERP.Models
{
    [Table("ESTIMATIONTYPEMASTER")]
    public class EstimationTypeMaster
    {
        [Key]

        public int ESTTID { get; set; }

        public string ESTTDESC { get; set; }

        public Int16 DISPSTATUS { get; set; }
    }
}