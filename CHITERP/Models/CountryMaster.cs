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
    [Table("contactaddresscountry")]
    public class CountryMaster
    {
        [Key]        
        public int CountryID { get; set; }

        public string CountryName { get; set; }
    }
}