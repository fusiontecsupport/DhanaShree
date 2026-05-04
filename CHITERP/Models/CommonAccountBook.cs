using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHITERP.Models
{
    [Table("CommonAccountBook")]
    public class CommonAccountBook
    {
        [Key]
        public int AccountBookID { get; set; }
        public int OwnerID { get; set; }
        public int OwnerTypeID { get; set; }
        public Nullable<DateTime> BookIssuedDate { get; set; }
        public Nullable<DateTime> BookTerminatedDate { get; set; }        
        public string BookNumber { get; set; }
        public int EmployeeId { get; set; }
        public int ReviewByEmployeeId { get; set; }
        public int ApprovedByEmployeeId { get; set; }
        public int StatusTypeID { get; set; }


        public Nullable<int> BookNo { get; set; }
    }
}