using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace CHITERP.Models
{
    [Table("CONTAINERSIZEMASTER")]
    public class ContainerSizeMaster
    {
        [Key]
        public int CONTNRSID { get; set; }

        public string CONTNRSDESC { get; set; }

        public string CONTNRSCODE { get; set; }

        public string CUSRID { get; set; }

        public string LMUSRID { get; set; }

        public Int16 DISPSTATUS { get; set; }

        public DateTime PRCSDATE { get; set; }       

    }
}