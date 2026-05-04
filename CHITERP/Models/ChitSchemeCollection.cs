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
    [Table("ChitSchemeCollection")]
    public class ChitSchemeCollection
    {
        [Key]
        public int ChitSchemeCollectionID { get; set; }

        public int ChitSchemeID { get; set; }

        public int CollectionIntervalTypeID { get; set; }

        public double SuggestedAmount { get; set; }

        public Nullable<Double> CollectionAmount { get; set; }
    }
}