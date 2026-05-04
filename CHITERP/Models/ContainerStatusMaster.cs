using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CHITERP.Models
{
    [Table("CONTAINERSTATUSMASTER")]
    public class ContainerStatusMaster
    {
        [Key]
        public int CNTNRSID { get; set; }

        public string CNTNRSDESC { get; set; }

        public string CNTNRSCODE { get; set; }

        public string CUSRID { get; set; }

        public string LMUSRID { get; set; }

        public Int16 DISPSTATUS { get; set; }

        public DateTime PRCSDATE { get; set; }

        public int DISPORDER { get; set; }
    }
}