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
    [Table("LoanSubscription")]
    public class LoanSubscription
    {
        [Key]
        public int LoanSubscriptionID { get; set; }

        public int LoanRequestID { get; set; }

        public int LoanSchemeID { get; set; }

        public int LoanSecuredTypeId { get; set; }

        public int DisbursementModeTypeID { get; set; }

        public int RepaymentModeTypeID { get; set; }

        public double Principal { get; set; }

        public double InterestRate { get; set; }

        public Nullable<Int32> DeductionTypeID { get; set; }

        public Nullable<Double> Term { get; set; }

        public Nullable<Int32> TermTypeID { get; set; }

        public int LinkedOfficeId { get; set; }

        public Nullable<DateTime> DisbursementDate { get; set; }

        public Nullable<DateTime> ProcessedOn { get; set; }
        public Nullable<Int32> RepaymentIntervalTypeID { get; set; }
        public Nullable<Int32> RouteID { get; set; }
        public Nullable<Int32> InterestDeductionTypeID { get; set; }
        public Nullable<Int32> LinkedAddressID { get; set; }
        public Nullable<Int32> DepositAccountID { get; set; }
        public Nullable<DateTime> ClosureDate { get; set; }
        public Nullable<Double> AgentCommission { get; set; }

        public Nullable<Int32> BookPickupDay { get; set; }
        public int StatusTypeID { get; set; }
        public string LoanSubscriptionCode { get; set; }
        public string TallyCode { get; set; }
        public Nullable<Int32> CollectionType { get; set; }
        public Nullable<Int32> AssociatedOrganizationID { get; set; }
        public Nullable<Int32> Installment { get; set; }
        public Nullable<Double> Inst_Principal { get; set; }
        public Nullable<Double> Interest_Amt { get; set; }
        public double LoanAmount { get; set; }

        public Nullable<Int32> SCNO { get; set; }

    }
}