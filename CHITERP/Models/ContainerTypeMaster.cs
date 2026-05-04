using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace CHITERP.Models
{
    [Table("CONTAINERTYPEMASTER")]
    public class ContainerTypeMaster
    {
        [Key] 
        public int CONTNRTID { get; set; }

        public string CONTNRTDESC { get; set; }

        public string CONTNRTCODE { get; set; }

        public int CSID { get; set; }

        public string CONTNRLDESC { get; set; }

        public string CONTNRACODE { get; set; }

        public string CUSRID { get; set; }

        public string LMUSRID { get; set; }

        public Int16 DISPSTATUS { get; set; }

        public DateTime PRCSDATE { get; set; }
    }
}