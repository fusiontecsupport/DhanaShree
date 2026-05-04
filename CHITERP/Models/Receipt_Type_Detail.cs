using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CHITERP.Models
{
    [Table("RECEIPT_TYPE_DETAIL")]
    public class Receipt_Type_Detail
    {
        [Key]
        public int RTID { get; set; }
        public string RTDESC { get; set; }
    }
}