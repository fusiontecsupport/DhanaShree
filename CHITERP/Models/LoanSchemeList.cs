using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CHITERP.Models
{
    public class LoanSchemeList
    {
        public List<LoanScheme> loanschemes { get; set; }

        public List<LoanFormula> loanformulas { get; set; }

        public List<LoanVariableRange> loanvariablerangs { get; set; }
    }
}