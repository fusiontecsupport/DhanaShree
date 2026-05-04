using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CHITERP.Models
{
    [Table("COMPANYACCOUNTINGDETAIL")]
    public class CompanyAccountingDetails
    {
        [Key]
        public int COMPYID { get; set; }

        public int COMPID { get; set; }

        public int YRID { get; set; }

        public string CUSRID { get; set; }

        public int DISPSTATUS { get; set; }

        public DateTime PRCSDATE { get; set; }

    }
}