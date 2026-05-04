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
    [Table("ChitGroupRegistered")]
    public class ChitGroupRegistered
    {
        [Key]
        public int ChitGroupRegisteredId { get; set; }

        public int ChitGroupId { get; set; }

        public string PriorSanctionNo { get; set; }

        public string ChitAgreementNo { get; set; }


        public string ChitRegistrationNo { get; set; }

        public Nullable<DateTime> ChitRegistrationYear { get; set; }        

        public Nullable<Int32> TotalTicket { get; set; }

        public Nullable<Int32> RegistrarOfficeTypeId { get; set; }

    }
}