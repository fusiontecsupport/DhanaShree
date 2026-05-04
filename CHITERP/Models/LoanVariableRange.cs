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

    [Table("LoanVariableRange")]
    public class LoanVariableRange
    {
        [Key]

        public int LoanVariableRangeID { get; set; }

        public int LoanSchemeID { get; set; }

        public Nullable<Int32> LoanSecuredTypeId { get; set; }

        public Nullable<Int32> DisbursementModeTypeID { get; set; }

        public Nullable<Int32> VariableTypeId { get; set; }

        public double TopLimit { get; set; }

        public double BottomLimit { get; set; }

        public Nullable<Int32> TermTypeID { get; set; }

    }
}