using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CHITERP.Models
{
    [Table("CompanyFundAccount")]
    public class CompanyFundAccount
    {
        [Key]
        public int FundAccountID { get; set; }

        public string AccountCode { get; set; }

        public string AccountName { get; set; }

        public Nullable<double> AccountOpeningBalance { get; set; }

        public string AccountBank { get; set; }

        public string AccountBranch { get; set; }

        public string AccountNumber { get; set; }

        public string Description { get; set; }

        public int StatusTypeID { get; set; }
    }
}