using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using CHITERP.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System.Web.Mvc;
using CHITERP.Data;

namespace CHITERP.Models
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, string, AppUserLogin, AppUserRole, AppUserClaim>
    {
        //public virtual IDbSet<AspNetRoles> Roles { get; set; }
        public virtual IDbSet<VW_ACCOUNTING_YEAR_DETAIL_ASSGN> VW_ACCOUNTING_YEAR_DETAIL_ASSGN { get; set; }

        public virtual IDbSet<ApplicationGroups> ApplicationGroups { get; set; }

        public virtual IDbSet<MenuRoleMaster> MenuRoleMasters { get; set; }

        public DbSet<CompanyMaster> companymasters { get; set; }

        public DbSet<CompanyAccountingDetails> companyaccountingdetail { get; set; }

        public DbSet<DesignationMaster> designationmasters { get; set; }

        public DbSet<DepartmentMaster> departmentmasters { get; set; }

        public DbSet<EmployeeMaster> employeemasters { get; set; }

        public DbSet<StateMaster> statemasters { get; set; }

        public DbSet<MasterAmountCode> masteramountcodes { get; set; }

        public DbSet<CompanyFundAccount> fundaccount { get; set; }

        public DbSet<ContactTypeMaster> contacttypemasters { get; set; }
        public DbSet<ContactMaster> contactmasters { get; set; }

        public DbSet<ClientMaster> clientmasters { get; set; }

        public DbSet<ChitScheme> chitscheme { get; set; }

        public DbSet<ChitSchemeCollection> chitschemecolletion { get; set; }

        public DbSet<ChitSchemePattern> chitschemepattern { get; set; }

        public DbSet<ChitGroup> chitgroups { get; set; }

        public DbSet<ChitGroupRegistered> chitgroupregistereds { get; set; }

        public DbSet<ChitSubscription> chitsubscriptions { get; set; }

       // public DbSet<ChitSubscriptionMaster> chitsubscriptionsmaster { get; set; }

        public DbSet<LoanScheme> loanscheme { get; set; }
        public DbSet<LoanSubscription> loansubscription { get; set; }
        
        public DbSet<LoanVariableRange> loanvariablerange { get; set; }

        public DbSet<LoanFormula> loanformula { get; set; }

        public DbSet<LoanRequest> loanrequest { get; set; }

        public DbSet<CategoryMaster> categorymasters { get; set; }

        public DbSet<TypeMaster> Typemasters { get; set; }
        public DbSet<CategoryTypeMaster> categorytypematers { get; set; }

        public DbSet<Category_Address_Details> categoryaddressdetail { get; set; }

        public DbSet<EmptyComponentCodeMaster> emptycomponentcodemasters { get; set; }

        public DbSet<EmptyDamageCodeMaster> emptydamagecodemasters { get; set; }

        public DbSet<EmptyLeasingRepairCodeMaster> emptyleasingrepaircodemasters { get; set; }

        public DbSet<EmptyRepairCodeMaster> emptyrepaircodemasters { get; set; }

        public DbSet<EmptyResponseCodeMaster> emptyresponsecodemasters { get; set; }

        public DbSet<EmptyResponsibilityCodeMaster> emptyresponsibilitycodemasters { get; set; }

        public DbSet<EmptySlabTypeMaster> emptyslabtypemasters { get; set; }

        public DbSet<EmptySlabMaster> emptyslabmasters { get; set; }

        public DbSet<RepairSlabMaster> repairslabmasters { get; set; }

        public DbSet<LeasingSlabMaster> leasingslabmasters { get; set; }

        public DbSet<EmptyTariffMaster> emptytariffmasters { get; set; }

        public DbSet<GradeMaster> grademasters { get; set; }

        public DbSet<PlaceMaster> placemasters { get; set; }

        public DbSet<RepairPartMaster> repairpartmasters { get; set; }
               
        public DbSet<EmployeeYardMappingDetail> empyardmappingdetails { get; set; }

        public DbSet<SteamerYardMappingDetail> stmyardmappingdetails { get; set; }

        public DbSet<Vw_Employee_Details> vw_employee_details { get; set; }

        public DbSet<EmptyGateinDetail> emptygateindetails { get; set; }

        public DbSet<ProductGroupMaster> productgroupmasters { get; set; }

        public DbSet<EmptyGateOutDetail> emptygateoutdetails { get; set; }

        public DbSet<Vw_Empty_GateIn_Container_Detail> Vw_Empty_GateIn_Container_Details { get; set; }

        public DbSet<VW_YARD_EMPLOYEE_MAPPING_DETAIL_ASSIGN> VW_YARD_EMPLOYEE_MAPPING_DETAIL_ASSIGNs { get; set; }

        public DbSet<VW_YARD_STEAMER_MAPPING_DETAIL_ASSIGN> VW_YARD_STEAMER_MAPPING_DETAIL_ASSIGNs { get; set; }

        public DbSet<EstimationTypeMaster> estimationtypemasters { get; set; }

        public DbSet<RepairTransactionMaster> repairtranscationmasters { get; set; }

        public DbSet<RepairTransactionDetail> repairtranscationdetails { get; set; }

        public DbSet<RepairTransactionMasterFactor> repairtranscationmasterfactors { get; set; }
        public DbSet<ContactAddressCountry> contactaddresscountrys { get; set; }
        public DbSet<ContactAddressCity> contactaddresscitys { get; set; }
        public DbSet<ContactAddressLocality> contactaddresslocalitys { get; set; }
        public DbSet<CompanyOffice> companyoffices { get; set; }
        public DbSet<CompanyRoute> companyroutes { get; set; }
        public DbSet<CompanyRouteDetail> companyroutedetails { get; set; }
        public DbSet<CompanyRouteEmployee> companyrouteemployees { get; set; }
        public DbSet<TransactionCollectionList> trncolnlst { get; set; }
        public DbSet<TransactionCollectionReciepts> trncolnrecpts { get; set; }
        public DbSet<TransactionCollectionAllocation> trncolnalcn { get; set; }
        public DbSet<TransactionPaymentList> trnpymtlst { get; set; }
        public DbSet<TransactionPayment> trnpymtrecpts { get; set; }
        public DbSet<TransactionPaymentAllocation> trnpymtalcn { get; set; }

        public DbSet<CountryMaster> countryMasters { get; set; }
        public DbSet<CityMaster> cityMasters { get; set; }
        public DbSet<LocalityMaster> localityMasters { get; set; }        
        public DbSet<CommonAccountBook> commonacbooks { get; set; }

        public DbSet<TransactionBookVerificationList> trnbkvfcnlsts { get; set; }
        public DbSet<TransBookVerification> trnbkvfcns { get; set; }

        public DbSet<AccountGroupMaster> accountgroupmasters { get; set; }
        public DbSet<AccountHeadMaster> accountheadmasters { get; set; }
        public DbSet<TransactionMaster> transactionmasters { get; set; }
        public DbSet<TransactionDetail> transactiondetails { get; set; }
        public DbSet<TransactionReceiptDetail> transactionreceiptdetails { get; set; }

        public DbSet<FIN_PAYMENT_MODE> Fin_PaymentModes { get; set; }

        public DbSet<Journal_Entry_Type_Master> journal_entry_type_masters { get; set; }
        public DbSet<Receipt_Type_Detail> receipt_type_details { get; set; }
        public DbSet<Tmp_Receipt_Detail> tmp_receipt_details { get; set; }
        public DbSet<Tmp_Payment_Detail> tmp_payment_details { get; set; }
        public DbSet<TMPRPT_IDS> TMPRPT_IDS { get; set; }
        public virtual IDbSet<BankMaster> bankmasters { get; set; }
        public AppDbContext() : base("IdentityCon") { } //, throwIfV1Schema: false
       
        public static AppDbContext Create()
        {
            return new AppDbContext();
        }

        // Override OnModelsCreating:
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Make sure to call the base method first:
            base.OnModelCreating(modelBuilder);

            // Map Users to Groups:
            modelBuilder.Entity<ApplicationGroups>()
                .HasMany<ApplicationUserGroup>((ApplicationGroups g) => g.ApplicationUsers)
                .WithRequired()                
                .HasForeignKey<string>((ApplicationUserGroup ag) => ag.ApplicationGroupId);

            var augrp = modelBuilder.Entity<ApplicationUserGroup>()
                .HasKey((ApplicationUserGroup r) =>
                    new
                    {   
                        ApplicationUserId = r.ApplicationUserId,
                        ApplicationGroupId = r.ApplicationGroupId
                    }).ToTable("ApplicationUserGroups");

            //augrp.Ignore(a => a.AppUser_Id);
            //augrp.Ignore(a => a.AppUser_Id1);

            // Map Roles to Groups:
            modelBuilder.Entity<ApplicationGroups>()
                .HasMany<ApplicationGroupRole>((ApplicationGroups g) => g.ApplicationRoles)
                .WithRequired()
                .HasForeignKey<string>((ApplicationGroupRole ap) => ap.ApplicationGroupId);
            modelBuilder.Entity<ApplicationGroupRole>().HasKey((ApplicationGroupRole gr) =>
                new
                {
                    ApplicationRoleId = gr.ApplicationRoleId,
                    ApplicationGroupId = gr.ApplicationGroupId
                }).ToTable("ApplicationGroupRoles");
        }

    }
}