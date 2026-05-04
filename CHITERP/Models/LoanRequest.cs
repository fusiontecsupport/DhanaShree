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
    [Table("LoanRequest")]
    public class LoanRequest
    {
        [Key]
        public int LoanRequestID { get; set; }

        public int OrganizationID { get; set; }

        public int PersonID { get; set; }

        //public byte SubscriberIsPerson { get; set; }

        public int LoanSchemeID { get; set; }

        public int LoanSecuredTypeId { get; set; }

        public int DisbursementModeTypeID { get; set; }

        public int RepaymentModeTypeID { get; set; }

        public double Principal { get; set; }

        public double InterestRate { get; set; }

        public Nullable<Int32> DeductionTypeId { get; set; }

        public Nullable<Double> Term { get; set; }

        public Nullable<Int32> TermTypeID { get; set; }

        public Nullable<Int32> LinkedOfficeID { get; set; }

        public Nullable<DateTime> DisbursementDate { get; set; }

        public Nullable<DateTime> RequestedOn { get; set; }

        public Nullable<DateTime> ProcessedOn { get; set; }

        public Nullable<Int32> CommissionAgentPersonID { get; set; }

        public Nullable<Double> AgentCommission { get; set; }

        public Nullable<Int32> AccountEmployeeID { get; set; }

        public int StatusTypeID { get; set; }

        public Nullable<Double> LoanAmount { get; set; }
    }
}