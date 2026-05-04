using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CHITERP.Models
{
    [Table("TransactionCollectionReciepts")]
    public class TransactionCollectionReciepts
    {
        [Key]

        public int CollectionRecieptID { get; set; }
        public int CollectionListID { get; set; }
        public int TransactionTypeTypeID { get; set; }
        public int TransactionModeTypeID { get; set; }
        public decimal CollectedAmount { get; set; }
        public DateTime CollectionDateTime { get; set; }
        public Nullable<int> RouteID { get; set; }
        public string Remarks { get; set; }

        public string PrintRecepitCode { get; set; }
        public int TransactionCategoryTypeID { get; set; }

        public Nullable<int> CollectedByEmployeeID { get; set; }

        public int StatusTypeID { get; set; }
        public string ChequeNo { get; set; }
        public Nullable<DateTime> ChequeDate { get; set; }

        public string ChequeBankName { get; set; }
        public Nullable<DateTime> ChequeRealizedDate { get; set; }
        public Nullable<int> FundAccountID { get; set; }
    }
}











