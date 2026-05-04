using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CHITERP.Models
{

    [Table("REPAIRTRANSACTIONMASTERFACTOR")]
    public class RepairTransactionMasterFactor
    {
        [Key]
        public int TRANMFID { get; set; }

        public int TRANMID { get; set; }

        public int CFID { get; set; }

        public decimal DEDEXPRN { get; set; }

        public decimal DEDNOS { get; set; }

        public string DEDMODE { get; set; }

        public int DEDTYPE { get; set; }

        public int DEDORDR { get; set; }

        public Int16 CFOPTN { get; set; }

        public Int16 DORDRID { get; set; }

        public decimal DEDVALUE { get; set; }

        public string DEDCFDESC { get; set; }

    }
}