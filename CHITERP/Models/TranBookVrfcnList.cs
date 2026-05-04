using CHITERP.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CHITERP.Models
{
    public class TranBookVrfcnList
    {
        public List<TransactionBookVerificationList> tranbookvfcnlst { get; set; }


        public List<pr_Get_Book_Verification_Entry_Details_Result> tranbookentrylst { get; set; }
    }
}