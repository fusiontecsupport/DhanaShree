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
    [Table("MasterAmountCode")]
    public class MasterAmountCode
    {
        [Key]
        public int AmountCodeID { get; set; }

        public string AmountCode { get; set; }

        public string AmountCodeName { get; set; }

        public double AmountCodeConstant { get; set; }

        public int CustomerViewTypeID { get; set; }

        public int StatusTypeID { get; set; }
    }
}