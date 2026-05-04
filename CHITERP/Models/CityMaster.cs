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
    [Table("contactaddresscity")]
    public class CityMaster
    {
        [Key]
        public int CityID { get; set; }

        public int CountryID { get; set; }
        
        public string CityName { get; set; }
    }
}