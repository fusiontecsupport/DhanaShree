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

    [Table("LoanAllocation")]
    public class LoanAllocation
    {
        [Key]
        public int AllocationID { get; set; }

        public int LoanSubscriptionID { get; set; }

        public Nullable<Double> AllocationPercentage { get; set; }

        public Nullable<Double> AllocationAmount { get; set; }

        public int AllocationModeTypeID { get; set; }

        public int CompanyFundAccountID { get; set; }
       
    }
}