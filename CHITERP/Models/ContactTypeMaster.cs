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
    [Table("ContactTypeMaster")]
    public class ContactTypeMaster
    {
        [Key]
        public int CONTTID { get; set; }

        public string CONTTYPESHRTDESC { get; set; }

        public string CONTTYPEDESC { get; set; }

        public Nullable<Int16> DISPSTATUS { get; set; }

        public string CreatedBy { get; set; }     

        public Nullable<DateTime> CreatedDt { get; set; }

        public string UpdatedBy { get; set; }

        public Nullable<DateTime> UpdatedDt { get; set; }

    }
}