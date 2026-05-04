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
    [Table("MasterType")]
    public class TypeMaster
    {
        [Key]
        public int TypeId { get; set; }

        public int TypeCategoryId { get; set; }

        [DisplayName("Type")]
        [Required(ErrorMessage = "Please Enter Type !!")]
        //[Remote("ValidateCATENAME", "Common", AdditionalFields = "i_CATENAME", ErrorMessage = "This is already used.")]
        public string Type { get; set; }



        [DisplayName("TypePriority")]
        //[Required(ErrorMessage = "Please Enter TypePriority !!")]
        public Nullable<Int32> TypePriority { get; set; }



        [DisplayName("ControlName")]
        //[Required(ErrorMessage = "Please Enter ControlName !!")]
        public string ControlName { get; set; }
    }
}