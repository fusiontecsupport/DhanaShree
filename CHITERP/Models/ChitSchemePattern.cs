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
    [Table("ChitSchemePattern")]
    public class ChitSchemePattern
    {
        [Key]
        public int PatternID { get; set; }

        public int ChitSchemeID { get; set; }

        public int ChitInstalmentNo { get; set; }

        public Nullable<Double> DiscountPercentage { get; set; }

        public Nullable<Double> DiscountAmount { get; set; }

        public Nullable<Double> DividentPercentage { get; set; }

        public Nullable<Double> DividentAmount { get; set; }
    }
}