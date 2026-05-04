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
    [Table("CATEGORYMASTER")]
    public class CategoryMaster
    {
        [Key]
        public int CATEID { get; set; }

        public int CATETID { get; set; }

        [DisplayName("Name")]
        //[Required(ErrorMessage = "Field is required")]
        [Remote("ValidateCATENAME", "Common", AdditionalFields = "i_CATENAME", ErrorMessage = "This is already used.")]
        public string CATENAME { get; set; }

        [DisplayName("Printable Name")]
        public string CATEDNAME { get; set; }

        [DisplayName("Address")]
        //[Required(ErrorMessage = "Field is required")]
        public string CATEADDR { get; set; }

        [DisplayName("PHN Code")]
        public string CATEPHNID { get; set; }

        [DisplayName("Landline1")]
        [DataType(DataType.PhoneNumber)]
        public string CATEPHN1 { get; set; }

        [DisplayName("Landline2")]
        [DataType(DataType.PhoneNumber)]
        public string CATEPHN2 { get; set; }

        [DisplayName("Mobile1")]
        [DataType(DataType.PhoneNumber)]
        public string CATEPHN3 { get; set; }

        [DisplayName("Mobile2")]
        [DataType(DataType.PhoneNumber)]
        // [RegularExpression(@"([0-9]{10,11})", ErrorMessage = "Number Invalid..Enter 10 digit")]
        public string CATEPHN4 { get; set; }

        [DataType(DataType.EmailAddress)]
        [DisplayName("Email")]
        [MaxLength(50)]
        public string CATEMAIL { get; set; }

        [DisplayName("Contact Person")]
        public string CATECPRSN { get; set; }

        [DisplayName("Code")]
        //[Remote("ValidateCATECODE", "Common", AdditionalFields = "i_CATECODE", ErrorMessage = "This is already used.")]
        public string CATECODE { get; set; }

        public string CATEACODE { get; set; }

        public int ACHEADID { get; set; }

        public int TARIFFMID { get; set; }

        public Nullable<Int16> CATEBTYPE { get; set; }         
        
        public string CATEGSTNO { get; set; }
       
        public string CATEADDR1 { get; set; }

        public string CATEADDR2 { get; set; }
      
        public string CATEADDR3 { get; set; }
      
        public string CATEADDR4 { get; set; }
       
        public string CATELICNO { get; set; }

        public decimal CATETDSEXPRN { get; set; }       

        public Int16 CATELTYPE { get; set; }        

        public string CUSRID { get; set; }

        public string LMUSRID { get; set; }

        [DisplayName("Status")]
        public Nullable<Int16> DISPSTATUS { get; set; }

        [DataType(DataType.Date)]
        public Nullable<DateTime> PRCSDATE { get; set; }
        
    }
}