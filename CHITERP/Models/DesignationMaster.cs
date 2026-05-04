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
    [Table("DESIGNATIONMASTER")]
    public class DesignationMaster
    {
        [Key]
        public int DSGNID { get; set; }

        [DisplayName("Description")]
        [Required(ErrorMessage = "Please Enter Designation !!")]        
        public string DSGNDESC { get; set; }

        [DisplayName("Code")]
        [Required(ErrorMessage = "Please Enter Code !!")]        
        public string DSGNCODE { get; set; }
                
        public string CUSRID { get; set; }

        public string LMUSRID { get; set; }

        [DisplayName("Status")]
        [Required(ErrorMessage = "Please Select Status !!")]
        public Int16 DISPSTATUS { get; set; }

        [DataType(DataType.Date)]
        public DateTime PRCSDATE { get; set; }
    }
}