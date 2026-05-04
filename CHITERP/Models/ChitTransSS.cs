using CHITERP.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CHITERP.Models
{
    public class ChitTransSS
    {
        public List<ChitSubscription> chitsubs { get; set; }
        public List<pr_Get_Chit_Collection_Pattern_n_Actuals_Details_Result> chitschedules { get; set; }
        public List<pr_Get_Book_List_Result> chitbooklists { get; set; }
        public List<pr_Get_Chit_Dividend_Schedule_n_Actuals_Details_Result> chitdividendschedules { get; set; }

    }
}