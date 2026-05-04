using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CHITERP.Models
{
    [Table("VW_EMPTY_GATEIN_COTAINER_DETAILS")]
    public class Vw_Empty_GateIn_Container_Detail
    {
        [Key]

        public int GIDID { get; set; }

        public string CONTNRNO { get; set; }

        public int GILTYPE { get; set; }

        public int STMRID { get; set; }

        public string STMRNAME { get; set; }
    }
}