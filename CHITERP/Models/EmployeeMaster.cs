using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CHITERP.Models
{
    [Table("EMPLOYEEMASTER")]
    public class EmployeeMaster
    {
        [Key]
        public int CATEID { get; set; }
        [Required(ErrorMessage = "Please Enter numeric or Alphanumeric string")]
        //[Remote("ValidateEMPCATENAME", "Common", AdditionalFields = "i_CATENAME", ErrorMessage = "This is already used.")]
        public string CATENAME { get; set; }
        [Required(ErrorMessage = "Please Enter numeric or Alphanumeric string")]
        //[Remote("ValidateEMPCATECODE", "Common", AdditionalFields = "i_CATECODE", ErrorMessage = "This is already used.")]
        public string CATECODE { get; set; }
        public int CATETID { get; set; }
        public string CATEADDR1 { get; set; }
        public string CATEADDR2 { get; set; }
        public string CATEADDR3 { get; set; }
        public string CATEADDR4 { get; set; }
        public string CATEPHN1 { get; set; }
        public string CATEPHN2 { get; set; }
        public string CATEPHN3 { get; set; }
        public string CATEPHN4 { get; set; }
        public string CATECPNAME { get; set; }
        public string CATEEMAIL { get; set; }
        public int DEPTID { get; set; }
        public int DSGNID { get; set; }
        public int LOCTID { get; set; }
        public Nullable<DateTime> CATEDOB { get; set; }
        public Nullable<DateTime> CATEDOJ { get; set; }
        public Nullable<DateTime> CATEDOC { get; set; }
        public Nullable<DateTime> CATEDOR { get; set; }
        public short CATESTATUS { get; set; }
        public string CUSRID { get; set; }
        public string LMUSRID { get; set; }
        public short DISPSTATUS { get; set; }
        public DateTime PRCSDATE { get; set; }
        public int REGNID { get; set; }
        public IEnumerable<string> emplfile { get; set; }
    }
}