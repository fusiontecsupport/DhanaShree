-- exec [dbo].[pr_Customer_Wise_Account_Detail_Assgn] 40
-- SELECT * FROM VW_All_Subscription_Details
CREATE PROCEDURE [dbo].[pr_Customer_Wise_Account_Detail_Assgn] @PCustID INT 
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
		--SheetHeading		 varchar(50),
		Heading_Description	 varchar(100),
		Detail				 VARCHAR(200) 
	  )

	  insert into @PartyDtl 
	  select --'Party Info',
	  'Name', CLIENTNAME
	  from ClientMaster (nolock)
	  where CLIENTID = @PCustID

	  insert into @PartyDtl 
	  select --'Party Info', 
	  'Occupation', Occupation
	  from ClientMaster (nolock)
	  where CLIENTID = @PCustID
	  
	  insert into @PartyDtl 
	  select --'Party Info', 
	  'Father/Husband Name', Father_or_HusbandName
	  from ClientMaster (nolock)
	  where CLIENTID = @PCustID

	  insert into @PartyDtl 
	  select --'Party Info', 
	  'Monthly Income / Salary', MonthlyIncome_or_Salary
	  from ClientMaster (nolock)
	  where CLIENTID = @PCustID

	  insert into @PartyDtl 
	  select --'Party Info', 
	  'Nominee', Nominee_Name
	  from ClientMaster (nolock)
	  where CLIENTID = @PCustID

	  insert into @PartyDtl 
	  select --'Party Info', 
	  'Address', isnull(CLIENTADDRESS1,'') + ' ' + isnull(clientaddress2,'') +' '+
	  isnull(localityname,'')+' '+ isnull(c.cityname,isnull(CLIENTCITY,''))+' '+ isnull(CLIENTPINCD,'')
	  from ClientMaster a(nolock) 
		left join ContactAddressLocality b(nolock) on a.localityid = b.localityid
		left join ContactAddressCity c(nolock) on b.cityid = c.cityid
	  where CLIENTID = @PCustID


      DECLARE @TableMaster TABLE 
        ( 
			Product varchar(25),
           SubscriptionCode VARCHAR(25), 
           PersonName           VARCHAR(100), 
           DisbursementDate     date, 
           --TallyCode           VARCHAR(100), 
           Principal            NUMERIC(18, 2), 
           PaidAmount           NUMERIC(18, 2), 
           BalanceAmount        NUMERIC(18, 2), 
		   PrizeAmount        NUMERIC(18, 2), 
           status               VARCHAR(50) 
        ) 

DECLARE @Temp_01 TABLE ( loansubscriptionid INT,Personid int) 
INSERT INTO @Temp_01 (loansubscriptionid,Personid ) 
select OwnerID,PersonID from CommonGuarantor (nolock) where PersonID=@LCustID and OwnerTypeID=187

DECLARE @Temp_02 TABLE (Chitsubscriptionid INT,Personid int)		
INSERT INTO @Temp_02 (Chitsubscriptionid,Personid) 
select OwnerID,PersonID from CommonGuarantor (nolock) where PersonID=@LCustID and OwnerTypeID=854

DECLARE @Temp_03 TABLE (depositsubscriptionid INT,Personid int) 
INSERT INTO @Temp_03(depositsubscriptionid,Personid) 
select OwnerID,PersonID from CommonGuarantor  (nolock) where PersonID=@LCustID and OwnerTypeID=182

	  --DECLARE @TableMaster_01 TABLE 
   --     ( 
   --        loansubscriptionid INT, 
   --        PaidAmount         NUMERIC(18, 2) 
   --     ) 

	  DECLARE @TableMaster_02 TABLE 
        ( 
           loansubscriptionid INT, 
		   vamount NUMERIC(18, 2),
           PaidAmount NUMERIC(18, 2) 
        ) 

	  DECLARE @TableMaster_01 TABLE 
        ( 
           loansubscriptionid INT, 
		   vamount NUMERIC(18, 2),
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

      INSERT INTO @TableMaster_02(loansubscriptionid,vamount,PaidAmount) 
      SELECT dbo.transactioncollectionlist.productid, 
	  Sum(Case When TransactionCollectionReciepts.TransactionCategoryTypeID in(857,1014,1017,1016) then 0 else Isnull(CollectedAmount, 0) end),
      Sum(Case When TransactionCollectionReciepts.TransactionCategoryTypeID in(856) then Isnull(CollectedAmount, 0) else 0 end)
      FROM   dbo.transactioncollectionlist  (nolock) 
             INNER JOIN dbo.transactioncollectionreciepts  (nolock) 
                     ON dbo.transactioncollectionlist.collectionlistid = 
                        dbo.transactioncollectionreciepts.collectionlistid 
      /*WHERE      (dbo.TransactionCollectionReciepts.TransactionCategoryTypeID = 1021) AND  
                             (dbo.TransactionCollectionList.StatusTypeID = 1035)*/ 
      GROUP  BY dbo.transactioncollectionlist.productid

	  INSERT INTO @TableMaster_02(loansubscriptionid, vamount,PaidAmount) 

	  SELECT        dbo.TransactionPaymentList.ProductID, 
	  Sum(Case When TransactionPayment.TransactionCategoryTypeID in(857,1014,1017) then 0 else Isnull(PaidAmount, 0) end),
	  Sum(Case When TransactionPayment.TransactionCategoryTypeID in(856) then Isnull(PaidAmount, 0) else 0 end)
	  FROM            dbo.TransactionPaymentList  (nolock)  INNER JOIN
								dbo.TransactionPayment  (nolock) ON dbo.TransactionPaymentList.PaymentListID = dbo.TransactionPayment.PaymentListID
	  GROUP BY dbo.TransactionPaymentList.ProductID

	  INSERT INTO @TableMaster_01(loansubscriptionid, vamount,PaidAmount) 
	  Select loansubscriptionid, sum(vamount), sum(PaidAmount) From @TableMaster_02
	  Group by loansubscriptionid


	  /*-----*/

      INSERT INTO @TableMaster 
                  (Product,SubscriptionCode, 
                   PersonName, 
                   DisbursementDate, 
                   --TallyCode, 
                   Principal, 
                   PaidAmount, 
                   BalanceAmount, 
				   PrizeAmount,
                   status) 
      SELECT 'Loan',loanSubscriptionCode, 
           cl.CLIENTNAME AS PersonName, 
             --CONVERT(VARCHAR(10), loansubscription.DisbursementDate, 105), 
			 
			 convert(varchar, loansubscription.DisbursementDate, 106),
             --TallyCode, 
             ( loansubscription.LoanAmount), -- * masteramountcode.AmountCodeConstant), 
             Isnull(PaidAmount, 0), 
             ( ( loansubscription.Principal ) - Isnull(PaidAmount, 0) ), 0,
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
             --LEFT OUTER JOIN dbo.personemployment  (nolock) 
             --             ON dbo.personemployment.personid = dbo.person.personid 
             --LEFT OUTER JOIN dbo.organization  (nolock) 
             --             ON dbo.organization.organizationid = 
             --                dbo.loanrequest.OrganizationID
							-- left outer join @Temp_01 cg on cg.loansubscriptionid=LoanSubscription.LoanSubscriptionID
      WHERE  /* (dbo.LoanSubscription.StatusTypeID = 968) And*/ 
	  dbo.loanrequest.personid = @LCustID
      --  ( dbo.loanrequest.personid = @LCustID or cg.PersonID=@LCustID) 
		order by loansubscription.DisbursementDate desc
      --ORDER  BY CONVERT(VARCHAR(10), loansubscription.DisbursementDate, 105) desc

	  IF EXISTS (SELECT 1 FROM @Temp_01 WHERE Personid=@LCustID)
	  BEGIN
		INSERT INTO @TableMaster 
                  (Product,SubscriptionCode, 
                   PersonName, 
                   DisbursementDate, 
                   --TallyCode, 
                   Principal, 
                   PaidAmount, 
                   BalanceAmount, 
				   PrizeAmount,
                   status) 
      SELECT 'Loan',loanSubscriptionCode, 
             --dbo.Organization.OrganizationName, 
           c.clientname  AS PersonName, 
             --CONVERT(VARCHAR(10), loansubscription.DisbursementDate, 105), 
			 
			 convert(varchar, loansubscription.DisbursementDate, 106),
             --TallyCode, 
             ( loansubscription.LoanAmount), -- * masteramountcode.AmountCodeConstant), 
             Isnull(PaidAmount, 0), 
             ( ( loansubscription.Principal ) - Isnull(PaidAmount, 0) ), 0,
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
             --LEFT OUTER JOIN dbo.personemployment  (nolock) 
             --             ON dbo.personemployment.personid = dbo.person.personid 
             --LEFT OUTER JOIN dbo.organization  (nolock) 
             --             ON dbo.organization.organizationid = 
             --                dbo.loanrequest.OrganizationID
							 left outer join @Temp_01 cg on cg.loansubscriptionid=LoanSubscription.LoanSubscriptionID
      WHERE  /* (dbo.LoanSubscription.StatusTypeID = 968) And*/ 
	  --dbo.loanrequest.personid = @LCustID
       cg.PersonID=@LCustID
		order by loansubscription.DisbursementDate desc
	  END

      INSERT INTO @TableMaster 
                  (Product,SubscriptionCode, 
                   PersonName, 
                   DisbursementDate, 
                   --TallyCode, 
                   Principal, 
                   PaidAmount, 
                   BalanceAmount, 
				   PrizeAmount,
                   status) 
      SELECT 'Chit',chitSubscriptionCode, 
              c.CLIENTNAME AS PersonName, 
--             CONVERT(VARCHAR(10), chitsubscription.startdate, 105), 
			 convert(varchar, cs.startdate, 106),
             --TallyCode, 
             csm.ChitValue  * cs.TicketNumber, 
             Isnull(PaidAmount, 0), 
			 Case When mastertype.Type in('Created','Active Approval','Active Approval','Active','Payment Due', 'Modification Approval','Modification','Legal','Arrear') 
			 then ( Isnull(vamount, 0) +  Isnull(PaidAmount, 0)) - (csm.ChitValue  * cs.TicketNumber) else ( Isnull(vamount, 0) -  Isnull(PaidAmount, 0)) end,0,
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
      GROUP  BY chitSubscriptionCode,                
                c.CLIENTNAME, 
                cs.startdate, 
                --TallyCode, 
                csm.ChitValue,
				cs.TicketNumber,
                 Isnull(vamount, 0), 
				  Isnull(PaidAmount, 0),
                [type]
				


		 IF EXISTS (SELECT 1 FROM @Temp_02 WHERE Personid=@LCustID)
		 BEGIN
			INSERT INTO @TableMaster 
                  (Product, SubscriptionCode, 
                   PersonName, 
                   DisbursementDate, 
                   --TallyCode, 
                   Principal, 
                   PaidAmount, 
                   BalanceAmount, 
				   PrizeAmount,
                   status) 
      SELECT 'Chit', chitSubscriptionCode, 
            C.CLIENTNAME AS PersonName, 
--             CONVERT(VARCHAR(10), chitsubscription.startdate, 105), 
			 convert(varchar, CS.startdate, 106),
             --TallyCode, 
             CSM.ChitValue * CS.TicketNumber, 
             isnull(PaidAmount,0), 
			 Case When mastertype.Type in('Created','Active Approval','Active Approval','Active','Payment Due', 'Modification Approval','Modification','Legal','Arrear') 
			 then ( Isnull(vamount, 0) -  Isnull(PaidAmount, 0)) - (CSM.ChitValue  * CS.TicketNumber) else ( Isnull(vamount, 0) -  Isnull(PaidAmount, 0)) end,
             --( (ChitScheme.ChitValue * chitsubscription.TicketNumber) - PaidAmount ), 
             0,type 
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
             ----INNER JOIN [dbo].[transactionpaymentlist]  (nolock) 
             ----        ON [dbo].[transactionpaymentlist].productid = 
             ----           chitsubscription.subscriptionid 
             --Left outer JOIN dbo.personemployment  (nolock) 
             --        ON dbo.personemployment.personid = dbo.person.personid 
             --left outer JOIN dbo.organization  (nolock) 
             --        ON dbo.organization.organizationid = 
             --           dbo.chitsubscription.OrganizationID 
						left outer join @Temp_02 cg on cg.Chitsubscriptionid=CS.SubscriptionID
      WHERE  /* (dbo.LoanSubscription.StatusTypeID = 968) And*/
	  cg.PersonID=@LCustID
        --( chitsubscription.personid = @LCustID or cg.PersonID=@LCustID) 
      GROUP  BY chitSubscriptionCode,                
                C.CLIENTNAME, 
                CS.startdate, 
                --TallyCode, 
               -- chitscheme.ChitValue, 
			   CSM.ChitValue * CS.TicketNumber,
			   isnull(tmp02.PaidAmount,0),
			   isnull(tmp02.vamount,0),
				CS.TicketNumber,
             --   (Case When tmp02.TypeID in(857,1014,1017) then Isnull(PaidAmount, 0) else 0 end), 
                [type]				
		 END
				--order by chitsubscription.startdate desc
     -- ORDER BY CONVERT(VARCHAR(10), chitsubscription.startdate, 105) asc





		INSERT INTO @TableMaster 
                  (Product,SubscriptionCode, 
                   PersonName, 
                   DisbursementDate, 
                   --TallyCode, 
                   Principal, 
                   PaidAmount, 
                   BalanceAmount, 
				   PrizeAmount,
                   status) 
      SELECT 'Deposit', depositaccountcode, 
            C.CLIENTNAME AS PersonName, 
--             CONVERT(VARCHAR(10), depositaccount.accountopeningdate, 105), 
            convert(varchar,  depositaccount.accountopeningdate, 106),
             --TallyCode, 
             (sum([transactionpaymentlist].amount)),-- * masteramountcode.AmountCodeConstant ), 
             0.00, 
             0.00, 
             0,type 
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
			----left outer join @Temp_03 cg on cg.depositsubscriptionid=depositaccount.DepositAccountID
      WHERE  /* (dbo.LoanSubscription.StatusTypeID = 968) And*/ 
	  depositaccount.personid = @LCustID
	  Group by depositaccountcode, C.CLIENTID, C.CLIENTNAME,
            dbo.mastertype.Type,
			dbo.depositaccount.AccountOpeningDate,
            convert(varchar,  depositaccount.accountopeningdate, 106)
            --, TallyCode
        --( depositaccount.personid = @LCustID or cg.PersonID=@LCustID)
		order by depositaccount.accountopeningdate desc

		IF EXISTS (SELECT 1 FROM @Temp_03 WHERE Personid=@LCustID)
		BEGIN
			INSERT INTO @TableMaster 
                  (Product,SubscriptionCode, 
                   PersonName, 
                   DisbursementDate, 
                   --TallyCode, 
                   Principal, 
                   PaidAmount, 
                   BalanceAmount, 
				   PrizeAmount,
                   status) 
      SELECT 'Deposit', depositaccountcode, 
            C.CLIENTNAME AS PersonName, 
--             CONVERT(VARCHAR(10), depositaccount.accountopeningdate, 105), 
            convert(varchar,  depositaccount.accountopeningdate, 106),
             --TallyCode, 
             (sum([transactionpaymentlist].amount)), -- * masteramountcode.AmountCodeConstant ), 
             0.00, 
             0.00, 
             0,type 
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
			left outer join @Temp_03 cg on cg.depositsubscriptionid=depositaccount.DepositAccountID
		WHERE  /* (dbo.LoanSubscription.StatusTypeID = 968) And*/ 
			cg.PersonID=@LCustID
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

	
	select * from @PartyDtl

      SELECT Product, tmp01.SubscriptionCode, 
             PersonName, 
			 convert(varchar, DisbursementDate, 106) as [S.Date],
             --TallyCode, 
			 isnull(Tmp_Ledger_Balance_Detail.PrizeAmt,0) 'PrizeAmt',
             PRAmt as 'ChitValue_or_Principal', 
             PaidAmt as PaidAmount, 
             BalAmt as [Outstanding_Amt], 
             status
			 
      FROM   @TableMaster tmp01 left join dbo.Tmp_Ledger_Balance_Detail on tmp01.SubscriptionCode = Tmp_Ledger_Balance_Detail.LCode
	  order by DisbursementDate desc


      RETURN 
  END 



