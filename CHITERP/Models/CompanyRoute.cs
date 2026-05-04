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
    [Table("CompanyRoute")]
    public class CompanyRoute

    {
        [Key]
        public int RouteID { get; set; }

        public string RouteCode { get; set; }
        public string RouteName { get; set; }
        public int RouteCityID { get; set; }
        public int LinkedOfficeID { get; set; }
        public int SupervisorEmployeeID { get; set; }
        public int StatusTypeID { get; set; }
    }
}