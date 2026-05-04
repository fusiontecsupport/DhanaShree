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
    [Table("MasterCategory")]
    public class CategoryMaster
    {
        [Key]
        public int CategoryId { get; set; }


        [DisplayName("Name")]
        [Required(ErrorMessage = "Please Enter Category !!")]
        //[Remote("ValidateCategory", "Common", AdditionalFields = "i_CATENAME", ErrorMessage = "This is already used.")]
        public string Category { get; set; }



        [DisplayName("Classification")]
        [Required(ErrorMessage = "Please Enter Classification !!")]
        //[Remote("ValidateClassification", "Common", AdditionalFields = "i_Classification", ErrorMessage = "This is already used.")]
        public string Classification { get; set; }

        [DisplayName("ControlName")]
        [Required(ErrorMessage = "Please Enter ControlName !!")]
        public string ControlName { get; set; }


    }

    
}