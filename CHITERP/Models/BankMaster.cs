using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CHITERP.Models
{
    [Table("BANKMASTER")]
    public class BankMaster
    {
        [Key]
        public int BANKMID { get; set; }
        [Required(ErrorMessage = "Please Enter numeric or Alphanumeric string")]
        //[Remote("ValidateBANKMDESC", "Common", AdditionalFields = "i_BANKMDESC", ErrorMessage = "This is already used.")]
        public string BANKMDESC { get; set; }
        [Required(ErrorMessage = "Please Enter numeric or Alphanumeric string")]
        //[Remote("ValidateBANKMCODE", "Common", AdditionalFields = "i_BANKMCODE", ErrorMessage = "This is already used.")]
        public string BANKMCODE { get; set; }
        public string CUSRID { get; set; }
        public string LMUSRID { get; set; }
        public short DISPSTATUS { get; set; }
        public DateTime PRCSDATE { get; set; }
    }
}