using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CHITERP.Models
{

    [Table("CONTAINERSIDEMASTER")]
    
    public class ContainerSideMaster
    {

        [Key]
        public int CSID { get; set; }

        public string CSDESC { get; set; }

        public string CSCODE { get; set; }

        public string CUSRID { get; set; }

        public string LMUSRID { get; set; }

        public Int16 DISPSTATUS { get; set; }

        public DateTime PRCSDATE { get; set; }

        public int DISPORDER { get; set; }

    }
}