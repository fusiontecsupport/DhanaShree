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

    [Table("DEPARTMENTMASTER")]
    public class DepartmentMaster
    {
        [Key]
        public int DEPTID { get; set; }

        [DisplayName("Description")]
        [Required(ErrorMessage = "Please Enter Description !!")]       
        public string DEPTDESC { get; set; }

        [DisplayName("Code")]
        [Required(ErrorMessage = "Please Enter Code !!")]       
        public string DEPTCODE { get; set; }

        public string CUSRID { get; set; }

        public string LMUSRID { get; set; }

        [DisplayName("Status")]
        [Required(ErrorMessage = "Please Select Status !!")]
        public Int16 DISPSTATUS { get; set; }

        [DataType(DataType.Date)]
        public DateTime PRCSDATE { get; set; }
    }
}