using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CHITERP.Models
{
    [Table("VW_YARD_EMPLOYEE_MAPPING_DETAIL_ASSIGN")]
    public class VW_YARD_EMPLOYEE_MAPPING_DETAIL_ASSIGN
    {
        [Key]
        public int EMPID { get; set; }
        public int YRDID { get; set; }
        public string YRDDESC { get; set; }        
        public string CATENAME { get; set; }
        public string UserName { get; set; }
    }
}