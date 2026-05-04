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
    [Table("ClientMaster")]
    public class ClientMaster
    {
        [Key]
        public int CLIENTID { get; set; }

        public int CLIENTTID { get; set; }

       public Nullable<int> REFCLIENTID { get; set; }

        public string CLIENTNAME { get; set; }

       public string CLIENTCONTACTPERSON { get; set; }

        public Nullable<DateTime> DATEOFBIRTH { get; set; }

        public Nullable<Int32> GENDERTYPEID { get; set; }
        public string CLIENTMOBILENO1 { get; set; }

        public string CLIENTMOBILENO2 { get; set; }

        public string CLIENTLANDLINENO1 { get; set; }

        public string CLIENTLANDLINENO2 { get; set; }

        public string CLIENTEMAILID { get; set; }

        public string CLIENTCONTACTDESIGNATION { get; set; }

       // public string CLIENTORGANISATION { get; set; }

        public string CLIENTADDRESS1 { get; set; }

        public string CLIENTADDRESS2 { get; set; }

        public string CLIENTCITY { get; set; }

        public int CLIENTSTATEID { get; set; }

        public int CLIENTCOUNTRYID { get; set; }

        public string CLIENTPINCD { get; set; }

        public Int16 DISPSTATUS { get; set; }

        public string CreatedBy { get; set; }

        [DataType(DataType.Date)]
        public Nullable<DateTime> CreatedDt { get; set; }

        public string UpdatedBy { get; set; }

        [DataType(DataType.Date)]
        public Nullable<DateTime> UpdatedDt { get; set; }

                
        public Nullable<int> localityid { get; set; }
        
        public string Father_or_HusbandName { get; set; }
        
        public string Occupation { get; set; }

        public string OccupationAddress { get; set; }

        public Nullable<decimal> MonthlyIncome_or_Salary { get; set; }
        public string Nominee_Name { get; set; }
        public string Nominee_Address { get; set; }
        public Nullable<int> Nominee_RelationshipId { get; set; }
    }
}