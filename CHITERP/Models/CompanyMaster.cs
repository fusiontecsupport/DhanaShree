using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CHITERP.Models
{
    [Table("COMPANYMASTER")]
    public class CompanyMaster
    {
        [Key]
        public int COMPID { get; set; }

        public string COMPNAME { get; set; }

        public string COMPDNAME { get; set; }

        public string COMPADDR { get; set; }

        public string COMPPHNID { get; set; }

        public string COMPPHN1 { get; set; }

        public string COMPPHN2 { get; set; }

        public string COMPPHN3 { get; set; }

        public string COMPPHN4 { get; set; }

        public string COMPCPRSN { get; set; }

        public string COMPMAIL { get; set; }

        public string COMPCODE { get; set; }

        public string CUSRID { get; set; }

        public string LMUSRID { get; set; }

        public Int16 DISPSTATUS { get; set; }

        public DateTime PRCSDATE { get; set; }

        public Nullable<DateTime> COMPCOFFTIME { get; set; }

        public string COMPPDESC1 { get; set; }
        
        public string COMPPDESC2 { get; set; }

        public string COMPGSTNO { get; set; }

        public string COMP_SMTP { get; set; }
        public string COMP_SMTP_PORT { get; set; }
        public string COMP_POP3 { get; set; }
        public string COMP_POP3_PORT { get; set; }
        //public string COMP_POLICY_NO { get; set; }
        //public string COMP_POLICY_DATE { get; set; }
        public string COMP_LUT_NO { get; set; }

    }
}