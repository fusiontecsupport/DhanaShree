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
    [Table("ContactMaster")]
    public class ContactMaster
    {
        [Key]
        public int CONTID { get; set; }

        public int CONTTID { get; set; }

        public Nullable<int> CLIENTID { get; set; }

        public string CONTTOTH { get; set; }

        public string CONTACTPERSON { get; set; }

        public Nullable<DateTime> DATEOFBIRTH { get; set; }

        public Nullable<Int32> GENDERTYPEID { get; set; }
        public string CPMOBILENO1 { get; set; }

        public string CPMOBILENO2 { get; set; }

        public string CPLANDLINENO1 { get; set; }

        public string CPLANDLINENO2 { get; set; }

        public string CPEMAILID { get; set; }

        public string CPDESIGNATION { get; set; }

        public string CPORGANISATION { get; set; }

        public string CPADDRESS1 { get; set; }

        public string CPADDRESS2 { get; set; }

        public string CPCITY { get; set; }

        public int CPSTATEID { get; set; }

        public int CPCOUNTRYID { get; set; }

        public string CPPINCD { get; set; }

        public Int16 DISPSTATUS { get; set; }

        public string CreatedBy { get; set; }

        [DataType(DataType.Date)]
        public Nullable<DateTime> CreatedDt { get; set; }

        public string UpdatedBy { get; set; }

        [DataType(DataType.Date)]
        public Nullable<DateTime> UpdatedDt { get; set; }
    }
}