using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CHITERP.Models
{
    [Table("COSTFACTORMASTER")]
    public class CostFactorMaster
    {
        [Key]
        public int CFID { get; set; }

        public int TRANTID { get; set; }

        public string CFDESC { get; set; }

        public string CFMODE { get; set; }

        public Int16 CFTYPE { get; set; }

        public decimal CFEXPR { get; set; }

        public Int16 CFNATR { get; set; }

        public Int16 CFOPTN { get; set; }

        public Int16 DORDRID { get; set; }

        public Byte DISPSTATUS { get; set; }

        public int LDGRID { get; set; }

        public DateTime PRCSDATE { get; set; }
    }
}