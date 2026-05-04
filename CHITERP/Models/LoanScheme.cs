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

    [Table("LoanScheme")]
    public class LoanScheme
    {
        [Key]
        public int LoanSchemeID { get; set; }             

        public string SchemeCode { get; set; }

        public string SchemeName { get; set; }

        public Nullable<Int32> BookVerificationIntervalTypeID { get; set; }

        public string Description { get; set; }

        public int AmountCodeID { get; set; }

        public int StatusTypeID { get; set; }

        public Nullable<Boolean> TenureBased { get; set; }

        public Nullable<Boolean> EMIBased { get; set; }

       
    }
}