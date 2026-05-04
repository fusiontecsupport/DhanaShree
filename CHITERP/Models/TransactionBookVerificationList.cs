using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace CHITERP.Models
{
    [Table("TransactionBookVerificationList")]
    public class TransactionBookVerificationList
    {
        [Key]
        public int BookVerificationListID { get; set; }
        public int BookID { get; set; }
        public DateTime GeneratedDate { get; set; }
        public int ProductTypeID { get; set; }
        public int ProductID { get; set; }
        public Nullable<DateTime> CollectedDate { get; set; }
        public Nullable<int> AssignedBy { get; set; }
        public Nullable<DateTime> AssignedDate { get; set; }

        public Nullable<int> VerifiedBy { get; set; }
        public Nullable<DateTime> VerifiedDate { get; set; }
        public Nullable<int> ApprovedBy { get; set; }
        public Nullable<DateTime> ApprovedDate { get; set; }
        public Nullable<DateTime> ReturnedDate { get; set; }
        public string Remarks { get; set; }
        public int StatusTypeID { get; set; }


        public Nullable<int> VerificationAssignedTo { get; set; }
        public Nullable<int> ApprovalAssignedTo { get; set; }


    }
}