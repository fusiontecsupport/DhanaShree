using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace CHITERP.Models
{
    [Table("CONTAINERTHRUMASTER")]
    public class ContainerThruMaster
    {
        [Key]
        public int CONTNRFID { get; set; }

        public string CONTNRFDESC { get; set; }

        public string CONTNRFCODE { get; set; }

        public Int16 CONTNRFTYPE { get; set; }

        public string CUSRID { get; set; }

        public string LMUSRID { get; set; }

        public Int16 DISPSTATUS { get; set; }

        public DateTime PRCSDATE { get; set; }
    }
}