using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CHITERP.Models
{
    [Table("STEAMERYARDMAPPINGDETAIL")]
    public class SteamerYardMappingDetail
    {
        [Key]
        public int SYMDID { get; set; }

        public int YRDID { get; set; }

        public int COMPID { get; set; }

        public int STMRID { get; set; }

        public Nullable<DateTime> EFFFRMDT { get; set; }

        public Nullable<DateTime> EFFTODT { get; set; }

        public Int16 DISPSTATUS { get; set; }


        public string CreatedBy { get; set; }

        public Nullable<DateTime> CreatedDt { get; set; }

        public string UpdatedBy { get; set; }

        public Nullable<DateTime> UpdatedDt { get; set; }

    }
}