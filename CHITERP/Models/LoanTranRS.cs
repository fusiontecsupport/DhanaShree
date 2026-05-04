using CHITERP.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CHITERP.Models
{
    public class LoanTranRS
    {
        public List<LoanRequest> loanrequests { get; set; }

        public List<LoanSubscription> loansubs { get; set; }
        public List<pr_Get_Loan_Schedule_n_Collection_Details_Result> loanschedules { get; set; }

    }
}