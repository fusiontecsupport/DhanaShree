using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace CHITERP.Models
{
    [Table("TransactionBookVerification")]
    public class TransBookVerification
    {
        [Key]
        public int BookVerificationID { get; set; }
        public int BookVerificationListID { get; set; }
        public DateTime BookEntryDate { get; set; }
        public Nullable<double> BookAmount { get; set; }
        public Nullable<int> ReferenceCollectionReceiptID { get; set; }
        public Nullable<double> CollectionReceiptOldAmount { get; set; }
        public int VerificationFindingTypeID { get; set; }
        public Nullable<int> NewCollectionReceiptID { get; set; }

        public Nullable<int> OwnerTypeId { get; set; }

        public string Remarks { get; set; }
        public int StatusTypeID { get; set; }





    }
}