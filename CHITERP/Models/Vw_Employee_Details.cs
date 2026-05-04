using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CHITERP.Models
{
    [Table("VW_EMPLOYEE_DETAILS")]
    public class Vw_Employee_Details
    {
        [Key]
        public int CATEID { get; set; }

        public string CATENAME { get; set; }

        public int DEPTID { get; set; }

        public string DEPTDESC { get; set; }

        public int DSGNID { get; set; }

        public string DSGNDESC { get; set; }
    }
}