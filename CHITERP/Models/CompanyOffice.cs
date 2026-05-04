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
    [Table("CompanyOffice")]
    public class CompanyOffice
    {
        [Key]
        public int OfficeID { get; set; }
        public int OfficeTypeID { get; set; }
        public int OfficeParentID { get; set; }
        public string OfficeName { get; set; }
        public int StatusTypeID { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string PIN { get; set; }
        public int LocalityID { get; set; }
        public int CityID { get; set; }
        public int CountryID { get; set; }
    }
}