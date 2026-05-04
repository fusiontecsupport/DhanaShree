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

    [Table("LoanFormula")]
    public class LoanFormula
    {
        [Key]

        public int LoanFormulaID { get; set; }

        public int LoanSchemeID { get; set; }

        public Nullable<Int32> InterestDeductionTypeID { get; set; }

        public Nullable<Int32> LoanTransactionTypeID { get; set; }

        public Nullable<Int32> RepaymentIntervalTypeID { get; set; }

        public string Formula { get; set; }

    }
}