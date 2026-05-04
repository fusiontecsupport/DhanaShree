use DSFinFusion
go

	  Declare @PartyDtl table
	  (
		personid int,
		PartyName			varchar(100),
		Occupation			VARCHAR(200),
		Father_Husband		VARCHAR(200),
		Month_Inc_Sal		numeric(18,2),
		Nominee_Name		varchar(200),
		PartyAddress		varchar(500),
		Locality			varchar(100),
		City				varchar(100),
		Pincode				varchar(15)
	  )

	  insert into @PartyDtl 
	  select a.clientid, CLIENTNAME, Occupation, Father_or_HusbandName,MonthlyIncome_or_Salary,
		Nominee_Name,  isnull(CLIENTADDRESS1,'') + ' ' + isnull(clientaddress2,'') ,
	  isnull(localityname,''), isnull(c.cityname,isnull(CLIENTCITY,'')), isnull(CLIENTPINCD,'')
	  from ClientMaster a(nolock) 
		left join ContactAddressLocality b(nolock) on a.localityid = b.localityid
		left join ContactAddressCity c(nolock) on b.cityid = c.cityid
	  where CLIENTID = 141

      DECLARE @TableMaster TABLE 
        ( 
			Product varchar(25),
           SubscriptionCode VARCHAR(25), 
		   PersonID				int,
           PersonName           VARCHAR(100), 		   
           DisbursementDate     date, 
		   EndDate				date, 
		   AgentName			varchar(150),
		   schemeid				int,
		   ticketnumber			int,
		   ProductDesc			nvarchar(150),
           --TallyCode           VARCHAR(100), 
           Principal            NUMERIC(18, 2), 
           PaidAmount           NUMERIC(18, 2), 
           BalanceAmount        NUMERIC(18, 2), 
		   PrizeAmount        NUMERIC(18, 2), 
		   CollectedAmount      NUMERIC(18, 2),
           status               VARCHAR(50) 
        ) 

DECLARE @Temp_01 TABLE ( subscriptionid INT,Personid int,prodtypeid int) 
DECLARE @Temp_02 TABLE ( subscriptionid INT,Personid int,prodtypeid int) 
DECLARE @Temp_03 TABLE ( subscriptionid INT,Personid int,prodtypeid int) 
DECLARE @Temp_00 TABLE ( subscriptionid INT,Personid int,prodtypeid int) 

select * from LoanSubscription where loansubscriptioncode ='DSC/10M/00073'
select * from Tmp_Ledger_Balance_Detail where LCode ='DSC/10M/00073'
select * from loanrequest where loanrequestid = 3335
select * from loanrequest where loanrequestid = 3335
select * from clientmaster where clientid = 3424
select * from clientmaster where clientid = 141
INSERT INTO @Temp_01 (subscriptionid,Personid,prodtypeid ) 
select LoanSubscriptionID ,b.PersonID, 30
from Loansubscription a(nolock) 
join loanrequest b(nolock) on a.LoanRequestID = b.LoanRequestID
join clientmaster c(nolock) on b.personid = c.CLIENTID
where  c.REFCLIENTID=141 
or c.CLIENTID=141 

select * from @Temp_01

INSERT INTO @Temp_02 (subscriptionid,Personid,prodtypeid) 
select a.SubscriptionID, b.CLIENTID,29 from Chitsubscription a(nolock)  join clientmaster b(nolock) on a.personid = b.CLIENTID
where  b.REFCLIENTID=141 
or b.CLIENTID=141 


INSERT INTO @Temp_03 (subscriptionid,Personid,prodtypeid) 
select DepositAccountID ,a.PersonID, 31
from DepositAccount a(nolock) join clientmaster b(nolock) on a.personid = b.CLIENTID
where  b.REFCLIENTID=141 
or b.CLIENTID=141 

insert into @Temp_00
select subscriptionid,Personid,prodtypeid from @Temp_01
union
select subscriptionid,Personid,prodtypeid from @Temp_02
union
select subscriptionid,Personid,prodtypeid from @Temp_03

	  DECLARE @TableMaster_02 TABLE 
        ( 
		   producttypeid int,
           loansubscriptionid INT, 
		   colnamount NUMERIC(18, 2),
           PaidAmount NUMERIC(18, 2) 
        ) 

	  DECLARE @TableMaster_01 TABLE 
        ( 
			producttypeid int,
           loansubscriptionid INT, 
		   colnamount NUMERIC(18, 2),
           PaidAmount NUMERIC(18, 2) 
        ) 


      --INSERT INTO @TableMaster_01 (loansubscriptionid, PaidAmount) 
      --SELECT dbo.transactioncollectionlist.productid, Isnull(Sum(dbo.transactioncollectionreciepts.CollectedAmount), 0) 
      --FROM   dbo.transactioncollectionlist 
      --       INNER JOIN dbo.transactioncollectionreciepts 
      --               ON dbo.transactioncollectionlist.collectionlistid = 
      --                  dbo.transactioncollectionreciepts.collectionlistid 
      --/*WHERE      (dbo.TransactionCollectionReciepts.TransactionCategoryTypeID = 1021) AND  
      --                       (dbo.TransactionCollectionList.StatusTypeID = 1035)*/ 
      --GROUP  BY dbo.transactioncollectionlist.productid 

	  /*-----*/

      INSERT INTO @TableMaster_02(producttypeid, loansubscriptionid, colnamount,PaidAmount) 
      SELECT ProductTypeID, dbo.transactioncollectionlist.productid, 
	  Sum(Case When TransactionCollectionReciepts.TransactionCategoryTypeID in (857,1020,1021) then Isnull(CollectedAmount, 0) else 0 end),
      --Sum(Case When TransactionCollectionReciepts.TransactionCategoryTypeID in (856,1014,1019,1017,1113) then Isnull(CollectedAmount, 0) else 0 end)
	  0
      FROM   dbo.transactioncollectionlist  (nolock) 
			join @Temp_00 t on transactioncollectionlist.ProductTypeID = t.prodtypeid and transactioncollectionlist.ProductID = t.subscriptionid
             INNER JOIN dbo.transactioncollectionreciepts  (nolock) 
                     ON dbo.transactioncollectionlist.collectionlistid = 
                        dbo.transactioncollectionreciepts.collectionlistid 
	  --where dbo.TransactionCollectionList.StatusTypeID in( 1035,  1036)
      /*WHERE      (dbo.TransactionCollectionReciepts.TransactionCategoryTypeID = 1021) AND  
                             (dbo.TransactionCollectionList.StatusTypeID = 1035)*/ 
      GROUP  BY ProductTypeID, dbo.transactioncollectionlist.productid

	  SELECT ProductTypeID, dbo.transactioncollectionlist.productid, transactioncollectionreciepts.TransactionCategoryTypeID,
	  Sum(Isnull(CollectedAmount, 0) ),
      --Sum(Case When TransactionCollectionReciepts.TransactionCategoryTypeID in (856,1014,1019,1017,1113) then Isnull(CollectedAmount, 0) else 0 end)
	  0
      FROM   dbo.transactioncollectionlist  (nolock) 
			join @Temp_00 t on transactioncollectionlist.ProductTypeID = t.prodtypeid and transactioncollectionlist.ProductID = t.subscriptionid
             INNER JOIN dbo.transactioncollectionreciepts  (nolock) 
                     ON dbo.transactioncollectionlist.collectionlistid = 
                        dbo.transactioncollectionreciepts.collectionlistid 
	  --where dbo.TransactionCollectionList.StatusTypeID in( 1035,  1036)
      /*WHERE      (dbo.TransactionCollectionReciepts.TransactionCategoryTypeID = 1021) AND  
                             (dbo.TransactionCollectionList.StatusTypeID = 1035)*/ 
      GROUP  BY ProductTypeID, dbo.transactioncollectionlist.productid,transactioncollectionreciepts.TransactionCategoryTypeID
	  
	  select * from @TableMaster_02

	  INSERT INTO @TableMaster_02(producttypeid,loansubscriptionid, colnamount,PaidAmount) 	  
	  SELECT       ProductTypeTypeID, dbo.TransactionPaymentList.ProductID, 
	  0,--Sum(Case When TransactionPayment.TransactionCategoryTypeID in (857,1020,1021)  then Isnull(PaidAmount, 0) else 0 end),
	  Sum(Case When TransactionPayment.TransactionCategoryTypeID in (856,1014,1019,1017,1113) then Isnull(PaidAmount, 0) else 0 end)
	  FROM            dbo.TransactionPaymentList  (nolock)  
				join @Temp_00 t on TransactionPaymentList.ProductTypeTypeID = t.prodtypeid and TransactionPaymentList.ProductID = t.subscriptionid
				INNER JOIN dbo.TransactionPayment  (nolock) ON dbo.TransactionPaymentList.PaymentListID = dbo.TransactionPayment.PaymentListID
	  --where dbo.TransactionCollectionList.StatusTypeID in( 1035,  1036)
	  GROUP BY ProductTypeTypeID,dbo.TransactionPaymentList.ProductID

	  --select * from @TableMaster_02

	  INSERT INTO @TableMaster_01(producttypeid,  loansubscriptionid, colnamount,PaidAmount) 
	  Select producttypeid, a.loansubscriptionid, sum(colnamount), sum(PaidAmount) From @TableMaster_02 a
	  Group by producttypeid, a.loansubscriptionid

 SELECT 'Loan',loanSubscriptionCode, cl.clientid,
           cl.CLIENTNAME AS PersonName, 
             --CONVERT(VARCHAR(10), loansubscription.DisbursementDate, 105), 
			 
			 convert(varchar, loansubscription.DisbursementDate, 106),
			 ClosureDate,
			 isnull(catename,''),
			 loansubscription.LoanSchemeID,1, N'₹' + cast( loansubscription.LoanAmount as varchar(25)),
             --TallyCode, 
             ( loansubscription.LoanAmount), -- * masteramountcode.AmountCodeConstant),              
             ( ( loansubscription.Principal ) - Isnull(colnamount, 0) ) 'balamt',  0, Isnull(colnamount, 0) colamt,Isnull(PaidAmount, 0) 'paidamt', 
   type 
      FROM   dbo.loansubscription  (nolock) 
             INNER JOIN dbo.loanrequest  (nolock) 
                     ON dbo.loansubscription.loanrequestid = 
                        dbo.loanrequest.loanrequestid 
             INNER JOIN dbo.clientmaster  cl(nolock) 
                     ON dbo.loanrequest.personid = cl.clientid			
			INNER JOIN dbo.loanscheme  (nolock) 
                     ON dbo.loansubscription.loanschemeid = 
                        dbo.loanscheme.loanschemeid 
             LEFT OUTER JOIN dbo.ClientMaster Person_1  (nolock) 
                          ON cl.REFCLIENTID = Person_1.CLIENTID 
             left JOIN dbo.masteramountcode  (nolock) 
                     ON dbo.loanscheme.amountcodeid = 
                        dbo.masteramountcode.amountcodeid 
             LEFT OUTER JOIN @TableMaster_01 tmp02 
                          ON loansubscription.loansubscriptionid = 
                             tmp02.loansubscriptionid 
             INNER JOIN [dbo].[mastertype]  (nolock) 
                     ON [dbo].[mastertype].typeid = 
                        dbo.loansubscription.statustypeid 	
			left JOIN dbo.EMPLOYEEMASTER  ca1(nolock) 
                     ON loanrequest.AccountEmployeeID = ca1.CATEID
             --LEFT OUTER JOIN dbo.personemployment  (nolock) 
             --   ON dbo.personemployment.personid = dbo.person.personid 
             --LEFT OUTER JOIN dbo.organization  (nolock) 
             --             ON dbo.organization.organizationid = 
             --                dbo.loanrequest.OrganizationID
							-- left outer join @Temp_01 cg on cg.subscriptionid=LoanSubscription.LoanSubscriptionID
      WHERE  /* (dbo.LoanSubscription.StatusTypeID = 968) And*/ 
	  dbo.loanrequest.personid = 141
      --  ( dbo.loanrequest.personid = 141 or cg.PersonID=141) 
		order by loansubscription.DisbursementDate desc
      --ORDER  BY CONVERT(VARCHAR(10), loansubscription.DisbursementDate, 105) desc