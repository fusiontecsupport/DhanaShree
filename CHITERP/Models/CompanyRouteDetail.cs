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
    [Table("CompanyRouteDetail")]
    public class CompanyRouteDetail
    {
        [Key]
        public int RoutelDetailID { get; set; }
        public int RouteID { get; set; }
        public int LocalityID { get; set; }
        public int LocalityPriority { get; set; }
        public int StatusTypeID { get; set; }
    }
}