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
    [Table("ContactAddressLocality")]
    public class ContactAddressLocality
    {
        [Key]
        public int LocalityID { get; set; }
        public int CityID { get; set; }
        public string LocalityName { get; set; }
    }
}