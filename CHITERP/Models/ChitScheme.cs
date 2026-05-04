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
    [Table("ChitScheme")]
    public class ChitScheme
    {
        [Key]
        public int ChitSchemeID { get; set; }

        public int ChitTypeID { get; set; }

        public string ChitSchemeCode { get; set; }

        public string ChitSchemeName { get; set; }

        public Double ChitValue { get; set; }

        public int ChitDuration { get; set; }

        public int DurationTypeId { get; set; }

        public Nullable<Double> ForemanCommision { get; set; }

        public Nullable<Int32> ForemanPrizeInstalment { get; set; }

        public Nullable<Double> BidCapAmount { get; set; }

        public Nullable<Int32> SealingPeriod { get; set; }

        public Nullable<Double> AverageDiscount { get; set; }

        public Nullable<Double> AverageDivident { get; set; }

        public Nullable<Double> ProposedIncome { get; set; }

        public int BookVerificationTypeId { get; set; }

        public int AscertainmentTypeId { get; set; }

        public int AscertainmentFrequencyTypeId { get; set; }

        public double SubscriptionAmount { get; set; }

        public Nullable<Double> EligibilityAmount { get; set; }

        public Nullable<Double> SubscriberReturns { get; set; }

        public Nullable<Double> CommissionEmp { get; set; }

        public Nullable<Double> CommissionNonEmp { get; set; }

        public int AmountCodeID { get; set; }

        public int StatusTypeID { get; set; }

        public Nullable<Double> InitialDiscountPerc { get; set; }

        public Nullable<Double> InitialDividendPerc { get; set; }
    }
}