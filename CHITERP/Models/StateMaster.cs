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
    [Table("STATEMASTER")]
    public class StateMaster
    {
        [Key]
        public int STATEID { get; set; }
        [DisplayName("Descrition")]
        [Required(ErrorMessage = "Field is required")]
        
        public string STATEDESC { get; set; }
        [DisplayName("Code")]
        [Required(ErrorMessage = "Field is required")]      
        public string STATECODE { get; set; }

        public Int16 STATETYPE { get; set; }

        public string CUSRID { get; set; }

        public string LMUSRID { get; set; }

        [DisplayName("Status")]
        [Required(ErrorMessage = "Field is required")]
        public Int16 DISPSTATUS { get; set; }


        [DataType(DataType.Date)]
        public DateTime PRCSDATE { get; set; }
    }
}