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
    [Table("ChitSubscription")]
    public class ChitSubscription
    {
        [Key]
        public int SubscriptionID { get; set; }

        public string TicketNumber { get; set; }

        public int PersonID { get; set; }

        public int ChitGroupID { get; set; }

        public int ChitSchemeID { get; set; }

        public int CollectionTypeID { get; set; }

        public int CollectionModeTypeID { get; set; }

        public int CollectionIntervalTypeID { get; set; }

        public Nullable<Int32> RouteID { get; set; }

        public Nullable<Int32> NomineePersonID { get; set; }

        public Nullable<Int32> NomineeRelationshipTypeID { get; set; }

        public Nullable<Int32> Witness1PersonID { get; set; }

        public Nullable<Int32> Witness2PresonID { get; set; }

        public Nullable<Int32> CommisionAgentPersonID { get; set; }

        public Nullable<Int32> OrganizationID { get; set; }        
        
        public int SubscriberIsPerson { get; set; }

        public Nullable<Double> CommisionAmount { get; set; }

        public Nullable<DateTime> CommencementDate { get; set; }        

        public Nullable<DateTime> ClosureDate { get; set; }

        public Nullable<DateTime> FirstInstalmentDate { get; set; }

        public Nullable<Int32> BookPickupDay { get; set; }

        public Nullable<Int32> FundAccountID { get; set; }

        public int StatusTypeID { get; set; }

        public Nullable<Int32> AssociatedOrganizationID { get; set; }

        public Nullable<Int32> LinkedAddressTypeID { get; set; }

        public Nullable<DateTime> StartDate { get; set; }

        public Nullable<DateTime> EndDate { get; set; }

        public string ChitSubscriptionCode { get; set; }

        public string TallyCode { get; set; }

        public Nullable<Int32> AccountEmployeeID { get; set; }

        public Nullable<Int32> DepositAccountID { get; set; }

        public Nullable<Int32> TicketPaidNumber { get; set; }

        public Nullable<Int32> CommisionAgentPersonID2 { get; set; }

        public string CommencementTime { get; set; }
        public int LinkedOfficeID { get; set; }
        public Nullable<Int32> SCNO { get; set; }

        public Nullable<Int32> CollectionAgentPersonID1 { get; set; }

        public Nullable<Int32> CollectionAgentPersonID2 { get; set; }

    }
}