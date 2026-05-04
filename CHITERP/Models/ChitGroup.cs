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
    [Table("ChitGroup")]
    public class ChitGroup
    {
        [Key]
        public int ChitGroupID { get; set; }
              
        public string ChitGroupCode { get; set; }
                
        public string ChitGroupName { get; set; }

        public int ChitSchemeID { get; set; }

        public Nullable<DateTime> CommencementDate { get; set; }

        public int OfficeID { get; set; }

        public Nullable<DateTime> FirstInstallmentDate { get; set; }

        public Nullable<DateTime> TerminationDate { get; set; }

        public int StatusTypeID { get; set; }

        public Nullable<Int32> MaxGrpMembers { get; set; }
        public int ChitGroupTypeID { get; set; }


    }

}