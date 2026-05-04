use DSFinFusion
go

-- exec [dbo].[pr_Customer_Wise_Account_Detail_Report] 6
-- exec [dbo].[pr_Customer_Wise_Account_Detail_Report] 141
-- exec [dbo].[pr_Customer_Wise_Account_Detail_Report] 3296
-- SELECT * FROM VW_All_Subscription_Details
alter PROCEDURE [dbo].[pr_Customer_Wise_Account_Detail_Report] @PCustID INT 
--@PSDate Datetime, @PEDate Datetime 
AS 
  BEGIN 
      SET nocount ON; 

      DECLARE @ATypeID INT, 
              @TypeID  INT 
      DECLARE @LCustID INT 

      SET @LCustID = @PCustID 

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
	  where CLIENTID = @PCustID

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

INSERT INTO @Temp_01 (subscriptionid,Personid,prodtypeid ) 
select LoanSubscriptionID ,b.PersonID, 30
from Loansubscription a(nolock) 
join loanrequest b(nolock) on a.LoanRequestID = b.LoanRequestID
join clientmaster c(nolock) on b.personid = c.CLIENTID
where  c.REFCLIENTID=@LCustID 
or c.CLIENTID=@LCustID 

INSERT INTO @Temp_02 (subscriptionid,Personid,prodtypeid) 
select a.SubscriptionID, b.CLIENTID,29 from Chitsubscription a(nolock)  join clientmaster b(nolock) on a.personid = b.CLIENTID
where  b.REFCLIENTID=@LCustID 
or b.CLIENTID=@LCustID 


INSERT INTO @Temp_03 (subscriptionid,Personid,prodtypeid) 
select DepositAccountID ,a.PersonID, 31
from DepositAccount a(nolock) join clientmaster b(nolock) on a.personid = b.CLIENTID
where  b.REFCLIENTID=@LCustID 
or b.CLIENTID=@LCustID 

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
	  
	  --select 'rajesh',* from @TableMaster_02

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

	  --if @PCustID=76
	  --select 'rajesh2',* from @TableMaster_01

	  /*-----*/

      INSERT INTO @TableMaster 
                  (Product,SubscriptionCode, 
				  PersonID,
                   PersonName, 
                   DisbursementDate, 
				   EndDate, 
				   AgentName,
				   schemeid,
				   ticketnumber,
				   ProductDesc,
                   --TallyCode, 
                   Principal,                    
                   BalanceAmount, 
				   PrizeAmount,
				   CollectedAmount, 
				   PaidAmount,
                   status) 
      SELECT 'Loan',loanSubscriptionCode, cl.clientid,
           cl.CLIENTNAME AS PersonName, 
             --CONVERT(VARCHAR(10), loansubscription.DisbursementDate, 105), 
			 
			 convert(varchar, loansubscription.DisbursementDate, 106),
			 ClosureDate,
			 isnull(catename,''),
			 loansubscription.LoanSchemeID,1, N'₹' + cast( loansubscription.LoanAmount as varchar(25)),
             --TallyCode, 
             ( loansubscription.LoanAmount), -- * masteramountcode.AmountCodeConstant),              
             ( ( loansubscription.Principal ) - Isnull(colnamount, 0) ),  0, Isnull(colnamount, 0),Isnull(PaidAmount, 0), 
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
	  dbo.loanrequest.personid = @LCustID
      --  ( dbo.loanrequest.personid = @LCustID or cg.PersonID=@LCustID) 
		order by loansubscription.DisbursementDate desc
      --ORDER  BY CONVERT(VARCHAR(10), loansubscription.DisbursementDate, 105) desc


	  --select 'rajesh3',* from @TableMaster

	  IF EXISTS (SELECT 1 FROM @Temp_01  WHERE Personid <>@LCustID) 
	  BEGIN
		INSERT INTO @TableMaster 
                  (Product,SubscriptionCode, PersonID,
                   PersonName, 
                   DisbursementDate, 
                   --TallyCode, 
				    EndDate, 
				   AgentName,
				   schemeid,
				   ticketnumber,
				   ProductDesc,
                   Principal,                     
                   BalanceAmount, 
				   PrizeAmount,
				   CollectedAmount,
				   PaidAmount,
                   status) 
      SELECT 'Loan',loanSubscriptionCode, c.clientid,
             --dbo.Organization.OrganizationName, 
           c.clientname  AS PersonName, 
             --CONVERT(VARCHAR(10), loansubscription.DisbursementDate, 105), 
			 
			 convert(varchar, loansubscription.DisbursementDate, 106),
			  ClosureDate,
			 isnull(catename,''),
			 LoanSubscription.LoanSchemeID,1, N'₹' + cast( loansubscription.LoanAmount as varchar(25)),
             --TallyCode, 
             ( loansubscription.LoanAmount), -- * masteramountcode.AmountCodeConstant),              
             ( ( loansubscription.Principal ) - Isnull(PaidAmount, 0) ), 0,Isnull(colnamount, 0),Isnull(PaidAmount, 0), 
             type 
      FROM   dbo.loansubscription  (nolock) 
             INNER JOIN dbo.loanrequest lr (nolock) 
                     ON dbo.loansubscription.loanrequestid = 
                        lr.loanrequestid 
             INNER JOIN dbo.clientmaster c  (nolock) 
                     ON lr.personid = c.clientid 
             INNER JOIN dbo.loanscheme  (nolock) 
                     ON dbo.loansubscription.loanschemeid = 
                        dbo.loanscheme.loanschemeid 
             LEFT OUTER JOIN dbo.ClientMaster AS Person_1  (nolock) 
                          ON c.REFCLIENTID = Person_1.CLIENTID
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
                     ON lr.AccountEmployeeID = ca1.CATEID
             --LEFT OUTER JOIN dbo.personemployment  (nolock) 
             --             ON dbo.personemployment.personid = dbo.person.personid 
             --LEFT OUTER JOIN dbo.organization  (nolock) 
             --             ON dbo.organization.organizationid = 
             --                dbo.loanrequest.OrganizationID
							 left outer join @Temp_01 cg on cg.subscriptionid=LoanSubscription.LoanSubscriptionID
      WHERE  /* (dbo.LoanSubscription.StatusTypeID = 968) And*/ 
	  --dbo.loanrequest.personid = @LCustID
       C.REFCLIENTID=@LCustID
		order by loansubscription.DisbursementDate desc
	  END

      INSERT INTO @TableMaster 
                  (Product,SubscriptionCode, PersonID,
                   PersonName, 
                   DisbursementDate, 
				     EndDate, 
				   AgentName,
				   schemeid,
				   ticketnumber,
				   ProductDesc,
                --TallyCode, 
                   Principal,                     
                   BalanceAmount, 
				   PrizeAmount,
				   CollectedAmount,
				   PaidAmount,
                   status) 
      SELECT 'Chit',chitSubscriptionCode, c.clientid,
     c.CLIENTNAME AS PersonName, 
--             CONVERT(VARCHAR(10), chitsubscription.startdate, 105), 
			 convert(varchar, isnull(cs.CommencementDate, cs.startdate), 106),
			  convert(varchar, isnull(cs.ClosureDate, cs.EndDate), 106),
			 isnull(ca1.catename,'')+','+isnull(ca2.catename,''),
			 csm.ChitSchemeID ,cast(isnull(cs.ticketnumber,1) as int), N'₹' + LTRIM(Str(csm.ChitValue, 38, 0))+' * '+cast(cast(isnull(cs.ticketnumber,1) as int) as varchar(25)),
             --TallyCode, 
             csm.ChitValue  * cast(isnull(cs.ticketnumber,1) as int),              
			 Isnull(colnamount, 0)  - (csm.ChitValue  * cast(cs.ticketnumber as int)),
             0,Isnull(colnamount, 0),Isnull(PaidAmount, 0), 
			 --( (ChitScheme.ChitValue  * chitsubscription.TicketNumber) - Isnull(PaidAmount, 0) ), 
             type 
      FROM   dbo.chitsubscription  cs(nolock) 
             INNER JOIN 
             --dbo.LoanRequest ON dbo.LoanSubscription.LoanRequestID = dbo.LoanRequest.LoanRequestID INNER JOIN
             dbo.clientmaster  c(nolock) 
                     ON cs.personid = c.clientid
             INNER JOIN dbo.chitscheme csm (nolock) 
                     ON cs.chitschemeid = 
                        csm.chitschemeid 
             LEFT OUTER JOIN dbo.ClientMaster AS Person_1  (nolock) 
                          ON c.REFCLIENTID = Person_1.CLIENTID
             left JOIN dbo.masteramountcode  (nolock) 
                     ON csm.amountcodeid = 
                        dbo.masteramountcode.amountcodeid 
             LEFT OUTER JOIN @TableMaster_01 tmp02 
                          ON cs.subscriptionid = 
                             tmp02.loansubscriptionid 
             INNER JOIN [dbo].[mastertype]  (nolock) 
                     ON [dbo].[mastertype].typeid = 
                        cs.statustypeid 
			left JOIN dbo.EMPLOYEEMASTER  ca1(nolock) 
                     ON cs.CommisionAgentPersonID = ca1.CATEID
			left JOIN dbo.EMPLOYEEMASTER  ca2(nolock) 
                     ON cs.CommisionAgentPersonID2 = ca2.CATEID
             ----INNER JOIN [dbo].[transactionpaymentlist] 
             ----        ON [dbo].[transactionpaymentlist].productid = 
             ----           chitsubscription.subscriptionid 
             --Left outer JOIN dbo.personemployment  (nolock) 
             --        ON dbo.personemployment.personid = dbo.person.personid 
             --left outer JOIN dbo.organization  (nolock) 
             --        ON dbo.organization.organizationid = 
             --           dbo.chitsubscription.OrganizationID 
						--left outer join @Temp_02 cg on cg.Chitsubscriptionid=chitsubscription.SubscriptionID
      WHERE  /* (dbo.LoanSubscription.StatusTypeID = 968) And*/
	  cs.personid = @LCustID
        --( chitsubscription.personid = @LCustID or cg.PersonID=@LCustID) 
      GROUP  BY chitSubscriptionCode,    c.clientid,            
                c.CLIENTNAME, isnull(cs.ticketnumber,1),
                isnull(cs.CommencementDate, cs.startdate),  cs.ClosureDate, cs.EndDate,ca1.catename,ca2.catename,csm.ChitSchemeID,
                --TallyCode, 
                csm.ChitValue,
				cs.TicketNumber,
                 Isnull(colnamount, 0), 
				  Isnull(PaidAmount, 0),
                [type]
				


		 IF EXISTS (SELECT 1 FROM @Temp_02  WHERE Personid <>@LCustID) 
		 BEGIN
			INSERT INTO @TableMaster 
                  (Product, SubscriptionCode, PersonID,
                   PersonName, 
				   DisbursementDate, 
				 EndDate, 
				   AgentName,
				   schemeid,
				   ticketnumber,
				   ProductDesc,
                   --TallyCode, 
                   Principal,                     
                   BalanceAmount, 
				   PrizeAmount,
				   CollectedAmount,
				   PaidAmount,
                   status) 
      SELECT 'Chit', chitSubscriptionCode, c.clientid,
            C.CLIENTNAME AS PersonName, 
--             CONVERT(VARCHAR(10), chitsubscription.startdate, 105), 
			 convert(varchar, isnull(cs.CommencementDate, cs.startdate), 106),
             --TallyCode, 
			  convert(varchar, isnull(cs.ClosureDate, cs.EndDate), 106),
			 isnull(ca1.catename,'')+','+isnull(ca2.catename,''),
			 csm.ChitSchemeID ,cast(isnull(cs.ticketnumber,1) as int), N'₹' +  LTRIM(Str(csm.ChitValue, 38, 0))+' * '+cast(cast(isnull(cs.ticketnumber,1) as int) as varchar(25)),            
             CSM.ChitValue * cast(isnull(cs.ticketnumber,1) as int), 
             --isnull(PaidAmount,0), 
			 --Case When mastertype.Type in('Created','Active Approval','Active Approval','Active','Payment Due', 'Modification Approval','Modification','Legal','Arrear') 
			 --then ( Isnull(colnamount, 0) -  Isnull(PaidAmount, 0)) - (CSM.ChitValue  * cast(cs.ticketnumber as int)) else ( Isnull(colnamount, 0) -  Isnull(PaidAmount, 0)) end,
    --         --( (ChitScheme.ChitValue * chitsubscription.TicketNumber) - PaidAmount ), 
    --         0,isnull(colnamount,0), type			
			 Isnull(colnamount, 0)  - (csm.ChitValue  * cast(cs.ticketnumber as int)),
             0,Isnull(colnamount, 0),Isnull(PaidAmount, 0),type
      FROM   dbo.chitsubscription CS (nolock) 
             INNER JOIN 
             --dbo.LoanRequest ON dbo.LoanSubscription.LoanRequestID = dbo.LoanRequest.LoanRequestID INNER JOIN
             dbo.ClientMaster C(NOLOCK) 
                     ON CS.personid = C.CLIENTID
             INNER JOIN dbo.chitscheme  CSM(nolock) 
                ON CS.chitschemeid = 
                        CSM.chitschemeid 
             LEFT OUTER JOIN dbo.ClientMaster AS Person_1  (nolock) 
                          ON C.REFCLIENTID= Person_1.CLIENTID
             LEFT JOIN dbo.masteramountcode  (nolock) 
                     ON CSM.amountcodeid = 
                        dbo.masteramountcode.amountcodeid 
             LEFT OUTER JOIN @TableMaster_01 tmp02 
                          ON CS.subscriptionid = 
                             tmp02.loansubscriptionid 
             INNER JOIN [dbo].[mastertype]  (nolock) 
                     ON [dbo].[mastertype].typeid = 
                        CS.statustypeid 
             left JOIN dbo.EMPLOYEEMASTER  ca1(nolock) 
                     ON cs.CommisionAgentPersonID = ca1.CATEID
			left JOIN dbo.EMPLOYEEMASTER  ca2(nolock) 
                     ON cs.CommisionAgentPersonID2 = ca2.CATEID
			 ----INNER JOIN [dbo].[transactionpaymentlist]  (nolock) 
             ----        ON [dbo].[transactionpaymentlist].productid = 
             ----           chitsubscription.subscriptionid 
             --Left outer JOIN dbo.personemployment  (nolock) 
             --        ON dbo.personemployment.personid = dbo.person.personid 
             --left outer JOIN dbo.organization  (nolock) 
             --        ON dbo.organization.organizationid = 
             --           dbo.chitsubscription.OrganizationID 
						left outer join @Temp_02 cg on cg.Subscriptionid=CS.SubscriptionID
      WHERE  /* (dbo.LoanSubscription.StatusTypeID = 968) And*/
	  C.REFCLIENTID=@LCustID
        --( chitsubscription.personid = @LCustID or cg.PersonID=@LCustID) 
      GROUP  BY chitSubscriptionCode,                
                C.CLIENTNAME, c.clientid,cs.ticketnumber,
                isnull(cs.CommencementDate, cs.startdate), cs.ClosureDate, cs.EndDate,ca1.catename,ca2.catename,csm.ChitSchemeID,
                --TallyCode, 
               csm.ChitValue, 			   
			   isnull(tmp02.PaidAmount,0),
			   isnull(tmp02.colnamount,0),				
             --   (Case When tmp02.TypeID in(857,1014,1017) then Isnull(PaidAmount, 0) else 0 end), 
                [type]				
		 END
				--order by chitsubscription.startdate desc
     -- ORDER BY CONVERT(VARCHAR(10), chitsubscription.startdate, 105) asc





		INSERT INTO @TableMaster 
                  (Product,SubscriptionCode, PersonID,
                   PersonName, 
                   DisbursementDate, 
                   --TallyCode, 
                   Principal, 
                   PaidAmount, 
                   BalanceAmount, 
				   PrizeAmount,
				   CollectedAmount,
                   status) 
      SELECT 'Deposit', depositaccountcode, c.clientid,
            C.CLIENTNAME AS PersonName, 
--             CONVERT(VARCHAR(10), depositaccount.accountopeningdate, 105), 
            convert(varchar,  depositaccount.accountopeningdate, 106),
             --TallyCode, 
             (sum([transactionpaymentlist].amount)),-- * masteramountcode.AmountCodeConstant ), 
             0.00, 
             0.00, 
             0,0,type 
      FROM   dbo.depositaccount  (nolock) 
             INNER JOIN 
             --dbo.LoanRequest ON dbo.LoanSubscription.LoanRequestID = dbo.LoanRequest.LoanRequestID INNER JOIN
             dbo.ClientMaster C(NOLOCK) 
                     ON dbo.depositaccount.personid = C.CLIENTID
             INNER JOIN dbo.depositscheme  (nolock) 
                     ON dbo.depositscheme.depositschemeid = 
                        dbo.depositaccount.depositschemeid 
             LEFT OUTER JOIN dbo.CLIENTMASTER AS Person_1  (nolock) 
                          ON C.REFCLIENTID = Person_1.CLIENTID
             LEFT JOIN dbo.masteramountcode  (nolock) 
                     ON dbo.depositscheme.amountcodeid = 
                        dbo.masteramountcode.amountcodeid 
             LEFT OUTER JOIN @TableMaster_01 tmp02 
                          ON depositaccount.depositaccountid = 
                             tmp02.loansubscriptionid 
             INNER JOIN [dbo].[mastertype]  (nolock) 
                     ON [dbo].[mastertype].typeid = 
                        dbo.depositaccount.statustypeid 
             LEFT OUTER JOIN [dbo].[transactionpaymentlist]  (nolock) 
                          ON [dbo].[transactionpaymentlist].productid = 
                             depositaccount.depositaccountid 
             --LEFT OUTER JOIN dbo.personemployment  (nolock) 
             --             ON dbo.personemployment.personid = dbo.person.personid 
             --LEFT OUTER JOIN dbo.organization  (nolock) 
             --             ON dbo.organization.organizationid = 
             --                dbo.depositaccount.OrganizationID 
			----left outer join @Temp_03 cg on cg.subscriptionid=depositaccount.DepositAccountID
      WHERE  /* (dbo.LoanSubscription.StatusTypeID = 968) And*/ 
	  depositaccount.personid = @LCustID
	  Group by depositaccountcode, C.CLIENTID, C.CLIENTNAME,
            dbo.mastertype.Type,
			dbo.depositaccount.AccountOpeningDate,
            convert(varchar,  depositaccount.accountopeningdate, 106)
            --, TallyCode
        --( depositaccount.personid = @LCustID or cg.PersonID=@LCustID)
		order by depositaccount.accountopeningdate desc

		IF EXISTS (SELECT 1 FROM @Temp_03  WHERE Personid <>@LCustID) 
		BEGIN
			INSERT INTO @TableMaster 
                  (Product,SubscriptionCode, PersonID,
                   PersonName, 
                   DisbursementDate, 
                   --TallyCode, 
                   Principal, 
                   PaidAmount, 
                   BalanceAmount, 
				   PrizeAmount,
				   CollectedAmount,
                   status) 
      SELECT 'Deposit', depositaccountcode, c.clientid,
            C.CLIENTNAME AS PersonName, 
--             CONVERT(VARCHAR(10), depositaccount.accountopeningdate, 105), 
            convert(varchar,  depositaccount.accountopeningdate, 106),
             --TallyCode, 
             (sum([transactionpaymentlist].amount)), -- * masteramountcode.AmountCodeConstant ), 
             0.00, 
             0.00, 
             0,0,type 
      FROM   dbo.depositaccount  (nolock) 
   INNER JOIN 
             --dbo.LoanRequest ON dbo.LoanSubscription.LoanRequestID = dbo.LoanRequest.LoanRequestID INNER JOIN
             dbo.ClientMaster C(NOLOCK) 
                     ON dbo.depositaccount.personid = C.CLIENTID
             INNER JOIN dbo.depositscheme  (nolock) 
                     ON dbo.depositscheme.depositschemeid = 
                        dbo.depositaccount.depositschemeid 
             LEFT OUTER JOIN dbo.ClientMaster AS Person_1  (nolock) 
                       ON C.REFCLIENTID = Person_1.CLIENTID
             LEFT JOIN dbo.masteramountcode  (nolock) 
                     ON dbo.depositscheme.amountcodeid = 
                        dbo.masteramountcode.amountcodeid 
             LEFT OUTER JOIN @TableMaster_01 tmp02 
                          ON depositaccount.depositaccountid = 
                             tmp02.loansubscriptionid 
             INNER JOIN [dbo].[mastertype]  (nolock) 
                     ON [dbo].[mastertype].typeid = 
                        dbo.depositaccount.statustypeid 
             LEFT OUTER JOIN [dbo].[transactionpaymentlist]  (nolock) 
                          ON [dbo].[transactionpaymentlist].productid = 
                             depositaccount.depositaccountid 
             --LEFT OUTER JOIN dbo.personemployment  (nolock) 
             --             ON dbo.personemployment.personid = dbo.person.personid 
             --LEFT OUTER JOIN dbo.organization  (nolock) 
             --             ON dbo.organization.organizationid = 
             --                dbo.depositaccount.OrganizationID 
			left outer join @Temp_03 cg on cg.subscriptionid=depositaccount.DepositAccountID
		WHERE  /* (dbo.LoanSubscription.StatusTypeID = 968) And*/ 
			C.REFCLIENTID=@LCustID
	  Group by depositaccountcode, C.CLIENTID, C.CLIENTNAME,
            dbo.mastertype.Type,
			dbo.depositaccount.AccountOpeningDate,
            convert(varchar,  depositaccount.accountopeningdate, 106)
             -- , TallyCode
        --( depositaccount.personid = @LCustID or cg.PersonID=@LCustID)
		order by depositaccount.accountopeningdate desc
		END
      --ORDER  BY CONVERT(VARCHAR(10), depositaccount.accountopeningdate, 105) asc

   --   SELECT loanSubscriptionCode, 
   --          PersonName, 
   --          --convert(varchar, DisbursementDate, 106) as DisbursementDate,
			-- --DisbursementDate,
			-- convert(varchar, DisbursementDate, 106) Ddate,
			-- --FORMAT (DisbursementDate, 'dd-MMM-yyyy') as DisbursementDate,
			----FORMAT (DisbursementDate, 'dd-mm-yyyy') as DisbursementDate,
   --          TallyCode, 
   --          Principal, 
   --          PaidAmount, 
   --          BalanceAmount, 
   --          status 
   --   FROM   @TableMaster order by DisbursementDate desc
	update A
	set EndDate = case when trm.Type like 'Day%' then dateadd(d,term, a.DisbursementDate)
	when trm.Type like 'Month%' then dateadd(m,term,a.DisbursementDate)
	when trm.Type like 'Year%' then dateadd(yy,term,a.DisbursementDate) end
	from @TableMaster A join LoanSubscription ls (nolock) on a.SubscriptionCode = ls.LoanSubscriptionCode
				left JOIN [dbo].[mastertype]  trm(nolock) ON ls.TermTypeID = trm.typeid 
	where a.Product = 'Loan'
	and EndDate is null

	--select * from @PartyDtl
	declare @noofacs int, @chqcolbouncests varchar(15), @defltrsts varchar(15)
	set @chqcolbouncests =''
	set @defltrsts ='' 

	select @noofacs = isnull(count (distinct SubscriptionCode),0)
	from @TableMaster

	declare @chitschemedtl table
	(subscriptioncode varchar(50),cschemeid int, stdt datetime)
	insert into @chitschemedtl 
	select distinct SubscriptionCode, schemeid, DisbursementDate
	from @TableMaster
	where Product = 'Chit'
	--select distinct ChitSubscriptionCode,  ChitSchemeID,  CommencementDate
	--from ChitSubscription where PersonID = 26

	declare @chitschemeduedivamtdtl table
	(csubscriptioncode varchar(50), --cschemeid int,
	--stdt datetime,
	--emidt datetime,
	 dueamt numeric(18,2),
	 divamt numeric(18,2)
	 )
--	 select * From ChitSchemeCollection
	 insert into @chitschemeduedivamtdtl
	 select subscriptioncode , --cs2.cschemeid,stdt,dateadd(m,cs1.chitinstalmentno,stdt),
	 sum(cs3.SuggestedAmount),sum(cs1.DividentAmount)
	 from ChitSchemePattern cs1 (nolock) 
	 join ChitSchemeCollection cs3 on cs1.ChitSchemeID = cs3.ChitSchemeID and cs3.CollectionIntervalTypeID = 775
	 join @chitschemedtl cs2 on cs1.ChitSchemeID = cs2.cschemeid
	 where dateadd(m,cs1.chitinstalmentno,stdt) <= getdate()
	 or (month(dateadd(m,cs1.chitinstalmentno,stdt) ) = month(getdate())
			and year(dateadd(m,cs1.chitinstalmentno,stdt) ) = year(getdate()))
	 group by subscriptioncode
	 --select * from @chitschemeduedivamtdtl  order by 1,2,3
	
	update @TableMaster
	set ticketnumber = 1 where isnull(ticketnumber,0) = 0 

      SELECT tmp01.Product, tmp01.SubscriptionCode, 
			tmp01.PersonID,
             PersonName, 
			 convert(varchar, DisbursementDate, 106) as [S.Date],
			 convert(varchar, EndDate, 106) as [E.Date],
			 datediff(mm,DisbursementDate, Enddate)+1 Duration ,
			 AgentName,
			 tmp01.schemeid,
			 ticketnumber,
			 ProductDesc,
             --TallyCode, 
			 isnull(Tmp_Ledger_Balance_Detail.PrizeAmt,0) 'PrizeAmt',
             isnull(PRAmt,Principal) as 'ChitValue_or_Principal', 
             --PaidAmt -isnull(Tmp_Ledger_Balance_Detail.PrizeAmt,0) ---isnull((b.divamt*ticketnumber),0) 
			 CollectedAmount = case when tmp01.Product='Loan' then tmp01.CollectedAmount else 
			 Tmp_Ledger_Balance_Detail.ChitAsonPaidDueAmt end,
             Tmp_Ledger_Balance_Detail.BalAmt as [Outstanding_Amt], 
             tmp01.status,
			 p.*, Convert(varchar,getdate(),103) 'AsonDt', @noofacs 'NoofAcs',
			 @chqcolbouncests 'chqcolbncsts', @defltrsts 'dfltrsts',
			 MainGrp = case when tmp01.personid = @lcustid then 'Party Accounts'
			 else 'Linked Party Accounts' end,			 
			 ((b.dueamt-b.divamt)*ticketnumber) 'DueAmt',
			 (b.divamt*ticketnumber) 'DivAmt',
			 'ActualOutstanding'= case when tmp01.product = 'Loan' then tmp01.BalanceAmount when tmp01.product = 'Chit' then ((b.dueamt-b.divamt)*ticketnumber)-CollectedAmount  else Tmp_Ledger_Balance_Detail.BalAmt end,
			 Tmp_Ledger_Balance_Detail.LastReceiptDt 'LastReceiptDt',
			 tmp01.PaidAmount PaidAmount,
			 --tmp01.PaidAmount-isnull(Tmp_Ledger_Balance_Detail.PrizeAmt,0)
			 Tmp_Ledger_Balance_Detail.ChitAsonPaidDivAmt 'DividendPaid',
			 Tmp_Ledger_Balance_Detail.PassbookTotal 'PassbookTotal',
			 --PassbookTotal = case when tmp01.product = 'Loan' then tmp01.BalanceAmount else Tmp_Ledger_Balance_Detail.PassbookTotal end ,
			 cast(InstNo as varchar)+'/' +cast(Duration as varchar) 'InstNo',
			 Tmp_Ledger_Balance_Detail.ChitAsonDueAmt 'ChitAsonDueAmt',
			 --Tmp_Ledger_Balance_Detail.OverDueAmt 'OverDueAmt'
			 OverDueAmt = case when tmp01.product = 'Loan' then Tmp_Ledger_Balance_Detail.BalAmt else Tmp_Ledger_Balance_Detail.OverDueAmt end 
      FROM   @TableMaster tmp01 
				left join @chitschemeduedivamtdtl b on tmp01.SubscriptionCode = b.csubscriptioncode
				left join dbo.Tmp_Ledger_Balance_Detail on tmp01.SubscriptionCode = Tmp_Ledger_Balance_Detail.LCode
				left join dbo.vw_all_subscription_details v(nolock) on tmp01.SubscriptionCode = v.SubscriptionCode,				
				@PartyDtl p 
	  order by DisbursementDate desc

	  --select * From @TableMaster
	  --select * From @TableMaster_01
	  --select * From @Temp_00
      RETURN 
  END 



