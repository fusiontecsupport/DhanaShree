-- exec [dbo].[CustomerAccountLedgerbyPcode_Daybook]   '2022-01-01',  '2022-01-31'
-- exec [CustomerAccountLedgerbyPcode_Daybook] @usrid ='' , @FromDate = '2022-01-01', @ToDate = '2022-10-08', @product=0, @customer = 0, @account = '', @collectionagent=0, @routeid = 0

alter PROCEDURE [dbo].[CustomerAccountLedgerbyPcode_Daybook] 
@usrid varchar(100),
@FromDate datetime,
@ToDate  datetime,
@product int=0,
@customer int=0,
@account varchar(100)='',
@collectionagent int=0,
@routeid int=0
AS 
BEGIN 

	set nocount on 
	declare @custname varchar(200), @colagtname varchar(200), @routename varchar(200)
	select @custname = isnull(clientname,'') from ClientMaster (nolock) where clientid = @customer
	select @colagtname = isnull(CATENAME,'') from employeemaster (nolock) where cateid = @collectionagent
	select @routename = isnull(RouteName,'') from CompanyRoute (nolock) where RouteID = @routeid

	Declare @filtertbl table
	(
	Filter_Description varchar(100),
	Filter_Value varchar(100)
	)
	
	insert into @filtertbl
	select 'From Date', Convert(varchar,@fromdate,103)
	insert into @filtertbl
	select 'To Date', Convert(varchar,@ToDate,103)
	insert into @filtertbl
	select 'Product', case when @product =0 then 'ALL' when @product =29 then 'Chit' 
			when @product =30 then 'Loan' when @product =31 then 'Deposit' Else 'Others' End	
	insert into @filtertbl
	select 'Route', case when @routeid =0 then 'ALL'  Else @routename End
	insert into @filtertbl
	select 'Collection Agent', case when @collectionagent =0 then 'ALL'  Else @colagtname End
	insert into @filtertbl
	select 'Customer', case when @customer =0 then 'ALL'  Else @custname End

	select * from @filtertbl
	

	DECLARE @TableMasterMain TABLE 
			( 
				ProductID int,
				ProductTypeid int,
			   AccountNumber varchar(50),
			   SubscriptionID int,
			   CustomerID int,
			   SchemeCode varchar(50),
			   TicketNumber numeric(18,2),
			   Loanamount numeric(18,2),
			   Amt numeric(18,2),
			   BalAmt numeric(18,2),
			   SDesc varchar(50),
			   tenure int,
			   OPDate smalldatetime,
			   Openingamount numeric(18,2),
			   LastPaymentAmt numeric(18,2),
			   PaymentDate varchar(50),
			   BookVerifiedDate varchar(50),
			   CollectionAgent  varchar(150),
			   RouteDesc varchar(100)
			) 


	DECLARE @TableMaster TABLE 
			( 
			   [Date] Datetime, 
			   Mode           VARCHAR(50), 
			   ReceiptNo     VARCHAR(50),
			   Category           VARCHAR(100), 
			   Debit           decimal(18, 2),--VARCHAR(50), 
			   Credit          decimal(18, 2),--VARCHAR(50), 
			   Balance         decimal(18, 2),
			   AccountNumber		varchar(50),
			   subscriptionid int,
			   ProductTypeid int,
			   CollectionAgent varchar(150)
			) 

	--Non Tenure

	DECLARE @TableMaster_02 TABLE 
			( 
			   [Date] Datetime, 
			   Mode           VARCHAR(50), 
			   ReceiptNo     VARCHAR(50),
			   Category           VARCHAR(50), 
			   Debit           decimal(18, 2),--VARCHAR(50), 
			   Credit          decimal(18, 2),--VARCHAR(50), 
			   Principal	   decimal(18, 2),	
			   Interest		   decimal(18, 2),
			   Balance         decimal(18, 2),
			   AccountNumber		varchar(50),
			   subscriptionid int,
			   ProductTypeid int,
			   CollectionAgent varchar(50)
			) 

	--Non Tenure

	DECLARE @TableMaster_05 TABLE 
			( 
			   [Date] Datetime, 
			   Mode           VARCHAR(50), 
			   ReceiptNo     VARCHAR(50),
			   Category           VARCHAR(50), 
			   Debit           decimal(18, 2),--VARCHAR(50), 
			   Credit          decimal(18, 2),--VARCHAR(50), 
			   Principal	   decimal(18, 2),	
			   Interest		   decimal(18, 2),
			   Balance         decimal(18, 2),
			   AccountNumber		varchar(50),
			   subscriptionid int,
			   ProductTypeid int,
			   CollectionAgent varchar(50)
			) 

	DECLARE @SummaryMaster TABLE 
			( 
			   SDesc           VARCHAR(100), 
			   Amount          decimal(18, 2)--VARCHAR(100)--NUMERIC(18, 2) 
			) 

	DECLARE @StatusMaster TABLE 
			( 
			   SDesc           VARCHAR(100), 
			   Amount          VARCHAR(100)--NUMERIC(18, 2) 
			) 

	
	-- select 'start loan collection' , getdate()
	Insert into @TableMaster 
	select CONVERT(varchar(10), tcr.CollectionDateTime, 120),mt.Type, --tcr.PrintRecepitCode,
	 isnull(tcr.PrintRecepitCode,'')  
	  + (case when mt.Type ='Transfer' then 'Source A/c: '+ TargetProductCode else'' end) ,
	mt1.Type+ case when mt3.Type is null then '' else '['+mt3.Type + ' ' + cast (CollectionReceiptOldAmount-BookAmount as varchar(20))+']' end,
	CASE
					WHEN mt1.Type='Principal Disbursement' THEN tcr.CollectedAmount
					WHEN mt1.Type='Principal Transfer' THEN (Case When AmountCodeConstant > 0 Then tcr.CollectedAmount/ AmountCodeConstant Else tcr.CollectedAmount End)--tcr.CollectedAmount
					WHEN mt1.Type='Interest Reversal' THEN tcr.CollectedAmount
					WHEN mt1.Type='Charges' THEN tcr.CollectedAmount
					ELSE 0
			   END
			AS Debit,
			CASE
					--WHEN mt1.Type='Principal Repayment' THEN (Case When AmountCodeConstant > 0 Then tcr.CollectedAmount/ AmountCodeConstant Else tcr.CollectedAmount End)--tcr.CollectedAmount
					WHEN mt1.Type='Principal Repayment' THEN tcr.CollectedAmount --tcr.CollectedAmount
					WHEN mt1.Type='Interest - Prepaid' THEN tcr.CollectedAmount
					--WHEN mt1.Type='Interest' THEN (Case When AmountCodeConstant > 0 Then tcr.CollectedAmount/ AmountCodeConstant Else tcr.CollectedAmount End)--tcr.CollectedAmount
					WHEN mt1.Type='Interest' THEN  tcr.CollectedAmount --tcr.CollectedAmount
					WHEN mt1.Type='Interest TDS' THEN tcr.CollectedAmount
					WHEN mt1.Type='Interest Waiver' THEN tcr.CollectedAmount
					WHEN mt1.Type='Principal Waiver' THEN tcr.CollectedAmount
					ELSE 0
			   END
			AS Credit,
			0.00, LoanSubscriptionCode, LoanSubscriptionID, 30, colagt.CATENAME
	from LoanSubscription ls (nolock) 
	join LoanRequest lr(nolock) on ls.loanrequestid = lr.loanrequestid
	join employeemaster colagt(nolock) on lr.AccountEmployeeID= colagt.cateid
	left outer join [dbo].[TransactionCollectionList] tcl (nolock)  on tcl.ProductID=ls.LoanSubscriptionID
	left outer  join [dbo].[TransactionCollectionReciepts] tcr  (nolock) on tcr.CollectionListID = tcl.CollectionListID
	left outer join [dbo].PaymentTransferLog pymt  (nolock) on tcl.CollectionListID=pymt.CollectionListID 
	--and substring(TargetProductCode ,1,1) in ('C','L','D')
	left outer join [dbo].[MasterType]  mt  (nolock) on tcl.TransactionModeTypeID = mt.TypeID
	left outer join [dbo].[MasterType]  mt1  (nolock) on tcl.TransactionCategoryTypeID = mt1.TypeID
	inner join [dbo].LoanScheme  (nolock) ON ls.LoanSchemeID = dbo.LoanScheme.LoanSchemeID
	inner join [dbo].MasterAmountCode  (nolock)  ON [dbo].LoanScheme.AmountCodeID = [dbo].MasterAmountCode.AmountCodeID
	left outer join (TransactionBookVerification bv(nolock) join MasterType mt3(nolock) on bv.VerificationFindingTypeID = mt3.TypeID)
			on tcr.CollectionRecieptID = bv.ReferenceCollectionReceiptID
	where tcr.CollectionDateTime between @FromDate and @ToDate 
	and tcr.CollectionDateTime is not null and mt.Type is not null and tcl.StatusTypeID in (1035,1036)
	AND tcl.ProductTypeID = 30
	and (@product = 30 or @product =0)
	and (LoanSubscriptionCode = @account  or @account ='')
	and (lr.PersonID = @customer or @customer = 0)
	and (ls.RouteID = @routeid or @routeid=0)
	and (colagt.cateid = @collectionagent or (@collectionagent=0 and @usrid =''))
	GROUP  BY tcr.CollectionDateTime, tcr.PrintRecepitCode,mt.Type,tcr.CollectedAmount,mt1.Type,  mt3.Type ,CollectionReceiptOldAmount-BookAmount,dbo.MasterAmountCode.AmountCodeConstant,
	mt3.Type,CollectionReceiptOldAmount-BookAmount , TargetProductCode, LoanSubscriptionCode, LoanSubscriptionID,  colagt.CATENAME
	order by CONVERT(varchar(10), tcr.CollectionDateTime, 120) asc

	-- select 'start loan payment' , getdate()
	Insert into @TableMaster 
	select CONVERT(varchar(10),tp.PaymentDateTime, 120),mt.Type,'',
	mt1.Type+ case when mt3.Type is null then '' else '['+mt3.Type + ' ' + cast (CollectionReceiptOldAmount-BookAmount as varchar(20))+']' end,
	CASE
					WHEN mt1.Type='Principal Disbursement' THEN tp.PaidAmount
					--WHEN mt1.Type='Principal Transfer' THEN (Case When AmountCodeConstant > 0 Then tp.PaidAmount/ AmountCodeConstant Else tp.PaidAmount End)--tp.PaidAmount
					WHEN mt1.Type='Principal Transfer' THEN  tp.PaidAmount
					WHEN mt1.Type='Interest Reversal' THEN tp.PaidAmount
					WHEN mt1.Type='Charges' THEN tp.PaidAmount
					ELSE 0
			   END
			AS Debit,
			CASE
					WHEN mt1.Type='Principal Repayment' THEN (Case When AmountCodeConstant > 0 Then tp.PaidAmount/ AmountCodeConstant Else tp.PaidAmount End)--THEN tp.PaidAmount
					WHEN mt1.Type='Interest - Prepaid' THEN tp.PaidAmount
					WHEN mt1.Type='Interest' THEN (Case When AmountCodeConstant > 0 Then tp.PaidAmount/ AmountCodeConstant Else tp.PaidAmount End) --tp.PaidAmount
					WHEN mt1.Type='Interest TDS' THEN tp.PaidAmount
					WHEN mt1.Type='Interest Waiver' THEN tp.PaidAmount
					WHEN mt1.Type='Principal Waiver' THEN tp.PaidAmount
					ELSE 0
			   END
			AS Credit,
			0.00, LoanSubscriptionCode, LoanSubscriptionID, 30,  colagt.CATENAME
	from LoanSubscription ls (nolock) 
	join LoanRequest lr(nolock) on ls.loanrequestid = lr.loanrequestid
	join employeemaster colagt(nolock) on lr.AccountEmployeeID= colagt.cateid
	left outer join [dbo].[TransactionPaymentList] tpl (nolock)  on tpl.ProductID = ls.LoanSubscriptionID
	left outer join [dbo].[TransactionPayment] tp (nolock)  on tp.PaymentListID =tpl.PaymentListID
	left outer join [dbo].[MasterType]  mt (nolock)  on tpl.TransactionModeTypeID = mt.TypeID
	left outer join [dbo].[MasterType]  mt1  (nolock) on tpl.TransactionCategoryTypeID = mt1.TypeID
	inner join [dbo].LoanScheme  (nolock) ON ls.LoanSchemeID = dbo.LoanScheme.LoanSchemeID
	inner join [dbo].MasterAmountCode  (nolock)  ON [dbo].LoanScheme.AmountCodeID = [dbo].MasterAmountCode.AmountCodeID
	left outer join (TransactionBookVerification bv(nolock) join MasterType mt3(nolock) on bv.VerificationFindingTypeID = mt3.TypeID)
			on tp.PaymentListID = bv.ReferenceCollectionReceiptID
	where tp.PaymentDateTime between @FromDate and @ToDate 
	and tp.PaymentDateTime is not null and mt.Type is not null and tp.StatusTypeID in (1035,1036)
	AND tpl.ProductTypeTypeID = 30
	and (@product = 30 or @product =0)
	and (LoanSubscriptionCode = @account  or @account ='')
	and (lr.PersonID = @customer or @customer = 0)
	and (ls.RouteID = @routeid or @routeid=0)
	and (colagt.cateid = @collectionagent or (@collectionagent=0 and @usrid =''))

	-- Loan for Non Tenure Receipt
	-- select 'start loan collection2' , getdate()
	Insert @TableMaster_02
	select CONVERT(varchar(10), tcr.CollectionDateTime, 120),mt.Type, --tcr.PrintRecepitCode,
	isnull(tcr.PrintRecepitCode,'')+ (case when mt.Type ='Transfer' then 'Source A/c: '+ TargetProductCode else'' end) ,
	CASE WHEN mt1.Type='Interest' THEN 'Principal Repayment' ELSE mt1.Type END Type1,
	CASE
					WHEN mt1.Type='Principal Disbursement' THEN tcr.CollectedAmount
					WHEN mt1.Type='Principal Transfer' THEN tcr.CollectedAmount
					WHEN mt1.Type='Interest Reversal' THEN tcr.CollectedAmount
					WHEN mt1.Type='Charges' THEN tcr.CollectedAmount
					ELSE 0
			   END
			AS Debit,
			CASE
					WHEN mt1.Type in('Principal Repayment', 'Interest') THEN tcr.CollectedAmount
					WHEN mt1.Type='Interest - Prepaid' THEN tcr.CollectedAmount
					WHEN mt1.Type='Interest TDS' THEN tcr.CollectedAmount
					WHEN mt1.Type='Interest Waiver' THEN tcr.CollectedAmount
					WHEN mt1.Type='Principal Waiver' THEN tcr.CollectedAmount

					ELSE 0
			   END
			AS Credit,
	CASE WHEN mt1.Type in ('Principal Repayment','Principal Waiver','Interest TDS') THEN tcr.CollectedAmount ELSE 0 END AS Principal,
	CASE WHEN mt1.Type='Interest' THEN tcr.CollectedAmount ELSE 0 END AS Interest,

			0.00, LoanSubscriptionCode, LoanSubscriptionID, 30, colagt.CATENAME
	from LoanSubscription ls (nolock) 
	join LoanRequest lr(nolock) on ls.loanrequestid = lr.loanrequestid
	join employeemaster colagt(nolock) on lr.AccountEmployeeID= colagt.cateid
	left outer join [dbo].[TransactionCollectionList] tcl (nolock)  on tcl.ProductID=ls.LoanSubscriptionID and tcl.ProductTypeID = 30
	left outer  join [dbo].[TransactionCollectionReciepts] tcr  (nolock) on tcr.CollectionListID = tcl.CollectionListID
	left outer join [dbo].PaymentTransferLog pymt (nolock)  on tcl.CollectionListID=pymt.CollectionListID 
	--and substring(TargetProductCode ,1,1) in ('C','L','D')
	left outer join [dbo].[MasterType]  mt  (nolock) on tcl.TransactionModeTypeID = mt.TypeID
	left outer join [dbo].[MasterType]  mt1  (nolock) on tcl.TransactionCategoryTypeID = mt1.TypeID
	where  tcr.CollectionDateTime between @FromDate and @ToDate and tcr.CollectionDateTime is not null and mt.Type is not null 
	and tcl.StatusTypeID in (1035,1036)
	AND tcl.ProductTypeID = 30
	and (@product = 30 or @product =0)
	and (LoanSubscriptionCode = @account  or @account ='')
	and (lr.PersonID = @customer or @customer = 0)
	and (ls.RouteID = @routeid or @routeid=0)	
	and (colagt.cateid = @collectionagent or (@collectionagent=0 and @usrid =''))
	GROUP  BY tcr.CollectionDateTime, tcr.PrintRecepitCode,mt.Type,tcr.CollectedAmount,mt1.Type , TargetProductCode , LoanSubscriptionCode, 
	LoanSubscriptionID,colagt.CATENAME
	order by CONVERT(varchar(10), tcr.CollectionDateTime, 120) asc

	-- select 'start loan collection5' , getdate()
	Insert into @TableMaster_05
	Select [Date] ,Mode, ReceiptNo, Category, Sum(Debit) Debit, Sum(Credit) as Credit, Sum(Principal) as Principal, 
	Sum(Interest) as Interest, Balance, AccountNumber, subscriptionid, ProductTypeid , CollectionAgent From @TableMaster_02
	Group by [Date] ,Mode, ReceiptNo, Category, Balance, AccountNumber, subscriptionid, ProductTypeid, CollectionAgent

	-- Loan for Non Tenure Payment
	-- select 'start loan payment 5' , getdate()
	Insert into @TableMaster_05
	select CONVERT(varchar(10),tp.PaymentDateTime, 120),mt.Type,'',
	--(case when mt.Type ='Transfer' then 'Source A/c: '+ TargetProductCode else'' end),
	mt1.Type,
	CASE
					WHEN mt1.Type='Principal Disbursement' THEN tp.PaidAmount
					WHEN mt1.Type='Principal Transfer' THEN tp.PaidAmount
					WHEN mt1.Type='Interest Reversal' THEN tp.PaidAmount
					WHEN mt1.Type='Charges' THEN tp.PaidAmount
					ELSE 0
			   END
			AS Debit,
			CASE
					WHEN mt1.Type='Principal Repayment' THEN tp.PaidAmount
					WHEN mt1.Type='Interest - Prepaid' THEN tp.PaidAmount
					WHEN mt1.Type='Interest' THEN tp.PaidAmount
					WHEN mt1.Type='Interest TDS' THEN tp.PaidAmount
					WHEN mt1.Type='Interest Waiver' THEN tp.PaidAmount
					WHEN mt1.Type='Principal Waiver' THEN tp.PaidAmount
					ELSE 0
			   END
			AS Credit,0,0,
			0.00, LoanSubscriptionCode, LoanSubscriptionID, 30, colagt.CATENAME
	from LoanSubscription ls (nolock) 
	join LoanRequest lr(nolock) on ls.loanrequestid = lr.loanrequestid
	join employeemaster colagt(nolock) on lr.AccountEmployeeID= colagt.cateid
	left outer join [dbo].[TransactionPaymentList] tpl on tpl.ProductID = ls.LoanSubscriptionID
	left outer join [dbo].[TransactionPayment] tp on tp.PaymentListID =tpl.PaymentListID
	left outer join [dbo].[MasterType]  mt on tpl.TransactionModeTypeID = mt.TypeID
	left outer join [dbo].[MasterType]  mt1 on tpl.TransactionCategoryTypeID = mt1.TypeID
	where tp.PaymentDateTime between @FromDate and @ToDate  
	and tp.PaymentDateTime is not null and mt.Type is not null and tp.StatusTypeID in (1035,1036)
	AND tpl.ProductTypeTypeID = 30
	and (@product = 30 or @product =0)
	and (LoanSubscriptionCode = @account  or @account ='')
	and (lr.PersonID = @customer or @customer = 0)
	and (ls.RouteID = @routeid or @routeid=0)
	and (colagt.cateid = @collectionagent or (@collectionagent=0 and @usrid =''))
	-- select 'start deposit collection' , getdate()

	Insert into @TableMaster 
	select distinct CONVERT(varchar(10), tcr.CollectionDateTime, 120),mt.Type, --tcr.PrintRecepitCode,
	 isnull(tcr.PrintRecepitCode,'') +   (case when mt.Type ='Transfer' then 'Source A/c: '+ TargetProductCode else'' end) ,
	mt1.Type+ case when mt3.Type is null then '' else '['+mt3.Type + ' ' + cast (CollectionReceiptOldAmount-BookAmount as varchar(20))+']' end,
	CASE
					WHEN mt1.Type='Payment' THEN tcr.CollectedAmount
					WHEN mt1.Type='Charges' THEN tcr.CollectedAmount
					WHEN mt1.Type='Payment Transfer' THEN tcr.CollectedAmount

					ELSE 0
			   END
			AS Debit,
			CASE
				
					WHEN mt1.Type='Collection' THEN tcr.CollectedAmount
					WHEN mt1.Type='Interest' THEN tcr.CollectedAmount
					WHEN mt1.Type='Payment Transfer' THEN tcr.CollectedAmount
					ELSE 0
			   END
			AS Credit,
			0.00, DepositAccountCode, DepositAccountID, 31, isnull(ca1.catename,'')
	from [DepositAccount] ls  (nolock) 
	--tcr.CollectedAmount,'',
		left join EMPLOYEEMASTER ca1 (nolock) on ls.AccountEmployeeID = ca1.CATEID
		left outer join [dbo].[TransactionCollectionList] tcl  (nolock) on tcl.ProductID=ls.DepositAccountID
	left outer  join [dbo].[TransactionCollectionReciepts] tcr  (nolock) on tcr.CollectionListID = tcl.CollectionListID
	left outer join [dbo].PaymentTransferLog pymt (nolock)  on tcl.CollectionListID=pymt.CollectionListID 
	--and substring(TargetProductCode ,1,1) in ('C','L','D')
	left outer join [dbo].[MasterType]  mt  (nolock) on tcl.TransactionModeTypeID = mt.TypeID
	left outer join [dbo].[MasterType]  mt1  (nolock) on tcl.TransactionCategoryTypeID = mt1.TypeID
	left outer join (TransactionBookVerification bv(nolock) join MasterType mt3(nolock) on bv.VerificationFindingTypeID = mt3.TypeID)
			on tcr.CollectionRecieptID = bv.ReferenceCollectionReceiptID
	where tcr.CollectionDateTime between @FromDate and @ToDate  and  tcr.CollectionDateTime is not null and mt.Type is not null 
	and tcl.StatusTypeID in (1035,1036)
	and ProductTypeID = 31
	and (@product = 31 or @product =0)
	and (DepositAccountCode = @account  or @account ='')
	and (ls.PersonID = @customer or @customer = 0)
	and (ls.RouteID = @routeid or @routeid=0)	
	and (ca1.cateid = @collectionagent or (@collectionagent=0 and @usrid =''))
	--GROUP  BY CONVERT(varchar(10), tcr.CollectionDateTime, 120), tcr.PrintRecepitCode,mt.Type,tcr.CollectedAmount,mt1.Type

	--select * from @TableMaster
	-- Deposit Payment
	-- select 'start deposit payment' , getdate()
	Insert into @TableMaster 
	select CONVERT(varchar(10),tp.PaymentDateTime, 120),mt.Type, '',--(case when mt.Type ='Transfer' then 'Source A/c: '+ TargetProductCode else'' end) ,
	mt1.Type+ case when mt3.Type is null then '' else '['+mt3.Type + ' ' + cast (CollectionReceiptOldAmount-BookAmount as varchar(20))+']' end,
	CASE
					WHEN mt1.Type='Payment' THEN tp.PaidAmount
					WHEN mt1.Type='Charges' THEN tp.PaidAmount
					WHEN mt1.Type='Payment Transfer' THEN tp.PaidAmount
					ELSE 0
			   END
			AS Debit,
			CASE
					WHEN mt1.Type='Collection' THEN tp.PaidAmount
					WHEN mt1.Type='Interest' THEN tp.PaidAmount
					--WHEN mt1.Type='Payment Transfer' THEN tp.PaidAmount
					ELSE 0
			   END
			AS Credit,
			0.00, DepositAccountCode, DepositAccountID, 31, isnull(ca1.catename,'')
	from [DepositAccount] ls  (nolock) 
	--tcr.CollectedAmount,'',
		left join EMPLOYEEMASTER ca1 (nolock) on ls.AccountEmployeeID = ca1.CATEID
		left outer join [dbo].[TransactionPaymentList] tpl (nolock)  on tpl.ProductID = ls.DepositAccountID
	left outer join [dbo].[TransactionPayment] tp  (nolock) on tp.PaymentListID =tpl.PaymentListID	
	left outer join [dbo].[TransactionCollectionReciepts] tcr (nolock)  on tcr.CollectionListID = tp.PaymentListID
	left outer join [dbo].[MasterType]  mt (nolock)  on tpl.TransactionModeTypeID = mt.TypeID
	left outer join [dbo].[MasterType]  mt1  (nolock) on tpl.TransactionCategoryTypeID = mt1.TypeID
	left outer join (TransactionBookVerification bv(nolock) join MasterType mt3(nolock) on bv.VerificationFindingTypeID = mt3.TypeID)
			on tcr.CollectionRecieptID = bv.ReferenceCollectionReceiptID
	where tp.PaymentDateTime between @FromDate and @ToDate  and tp.PaymentDateTime is not null and mt.Type is not null 
	and tpl.StatusTypeID in (1035,1036)
	and tpl.ProductTypeTypeID = 31
	and (@product = 31 or @product =0)
	and (DepositAccountCode = @account  or @account ='')
	and (ls.PersonID = @customer or @customer = 0)
	and (ls.RouteID = @routeid or @routeid=0)
	and (ca1.cateid = @collectionagent or (@collectionagent=0 and @usrid =''))

	-- Chit
	-- select 'start chit collection' , getdate()
	Insert into @TableMaster 
	select distinct  CONVERT(varchar(10), tcr.CollectionDateTime, 120) ,mt.Type, 
	--isnull(tcr.PrintRecepitCode,'')+
	(case when mt.Type ='Transfer' then 'Source A/c: '+ isnull(TargetProductCode,'') else'' end) +
	CASE WHEN mt1.Type in('Prize', 'Prize Transfer', 'Discount', 'Company', 'Foreman Commission', 'Dividend Reversal', 'Charges') then '' Else isnull(tcr.PrintRecepitCode,'') End,
	mt1.Type+ case when mt3.Type is null then '' else '['+mt3.Type + ' ' + cast (CollectionReceiptOldAmount-BookAmount as varchar(20))+']' end,
	CASE
		WHEN mt1.Type='Prize' THEN (Case When AmountCodeConstant > 0 Then tcr.CollectedAmount/ AmountCodeConstant Else tcr.CollectedAmount End)
		WHEN mt1.Type='Prize Transfer' THEN tcr.CollectedAmount
		WHEN mt1.Type='Discount' THEN tcr.CollectedAmount
		WHEN mt1.Type='Company' THEN tcr.CollectedAmount
		WHEN mt1.Type='Foreman Commission' THEN tcr.CollectedAmount
		WHEN mt1.Type='Dividend Reversal' THEN tcr.CollectedAmount
		WHEN mt1.Type='Charges' THEN tcr.CollectedAmount
		WHEN mt1.Type='Interest' THEN tcr.CollectedAmount
		ELSE 0
		END
		AS Debit,
	CASE
		WHEN mt1.Type='Dividend' THEN tcr.CollectedAmount
		--WHEN mt1.Type='Interest' THEN tcr.CollectedAmount
		--WHEN mt1.Type='Subscription' THEN tcr.CollectedAmount
		--WHEN mt1.Type='Subscription' THEN (Case WHEN mt.Type in('Transfer') then (Case WHEN AmountCodeConstant > 0 THEN tcr.CollectedAmount / AmountCodeConstant Else tcr.CollectedAmount End) Else tcr.CollectedAmount End)
		WHEN mt1.Type='Subscription' THEN (Case WHEN mt.Type in('Transfer') then (Case WHEN AmountCodeConstant > 0 THEN tcr.CollectedAmount  Else tcr.CollectedAmount End) Else tcr.CollectedAmount End)
		WHEN mt1.Type='Waiver' THEN tcr.CollectedAmount
		ELSE 0
		END
		AS Credit,
		0.00, ChitSubscriptionCode, SubscriptionID, 29, isnull(ca1.catename,'') + ', '+ isnull(ca2.catename,'')
	--tcr.CollectedAmount,'',
	from ChitSubscription ls  (nolock)  
	left join EMPLOYEEMASTER ca1 (nolock) on ls.CollectionAgentPersonID1 = ca1.CATEID
	left join EMPLOYEEMASTER ca2 (nolock) on ls.CollectionAgentPersonID2 = ca2.CATEID
	left outer join [dbo].[TransactionCollectionList] tcl (nolock)  on tcl.ProductID=ls.SubscriptionID
	left outer  join [dbo].[TransactionCollectionReciepts] tcr (nolock)  on tcr.CollectionListID = tcl.CollectionListID
	left outer join [dbo].PaymentTransferLog pymt (nolock)  on tcr.CollectionListID=pymt.CollectionListID 
	--and substring(TargetProductCode ,1,1) in ('C','L','D')
	left outer join [dbo].[MasterType]  mt (nolock)  on tcl.TransactionModeTypeID = mt.TypeID
	left outer join [dbo].[MasterType]  mt1 (nolock)  on tcl.TransactionCategoryTypeID = mt1.TypeID
	inner join [dbo].ChitScheme  (nolock) ON ls.ChitSchemeID = dbo.ChitScheme.ChitSchemeID
	inner join [dbo].MasterAmountCode (nolock)  ON [dbo].ChitScheme.AmountCodeID = [dbo].MasterAmountCode.AmountCodeID
	left outer join (TransactionBookVerification bv(nolock) join MasterType mt3(nolock) on bv.VerificationFindingTypeID = mt3.TypeID)
			on tcr.CollectionRecieptID = bv.ReferenceCollectionReceiptID
	where tcr.CollectionDateTime between @FromDate and @ToDate and tcr.CollectionDateTime is not null and mt.Type is not null 
	and tcl.StatusTypeID in (1035,1036) 
	and ProductTypeID = 29
	and (@product = 29 or @product =0)
	and (ChitSubscriptionCode = @account  or @account ='')
	and (ls.PersonID = @customer or @customer = 0)
	and (ls.RouteID = @routeid or @routeid=0)	
	and (ca1.cateid = @collectionagent or ca2.cateid = @collectionagent or (@collectionagent=0 and @usrid =''))

	--------GROUP  BY CONVERT(varchar(10), tcr.CollectionDateTime, 120), tcr.PrintRecepitCode,mt.Type,tcr.CollectedAmount,mt1.Type
	------order by CONVERT(varchar, tcr.CollectionDateTime, 103) asc

	--select 'test1', * from @TableMaster

	-- select 'start chit payment' , getdate()
	Insert into @TableMaster 
	select  CONVERT(varchar(10),tp.PaymentDateTime, 120),mt.Type,
	--tcr.PrintRecepitCode,
	--(case when mt.Type ='Transfer' then 'Source A/c: '+ isnull(TargetProductCode,'') else'' end) +' '+
	CASE WHEN mt1.Type in('Prize', 'Prize Transfer', 'Discount', 'Company', 'Foreman Commission', 'Dividend Reversal', 'Charges', 'Interest') then '' Else isnull(tcr.PrintRecepitCode,'') End,
	mt1.Type+ case when mt3.Type is null then '' else '['+mt3.Type + ' ' + cast (CollectionReceiptOldAmount-BookAmount as varchar(20))+']' end,
	CASE WHEN mt1.Type='Prize' THEN (Case When AmountCodeConstant > 0 Then tp.PaidAmount Else tp.PaidAmount End) --tp.PaidAmount
		WHEN mt1.Type='Prize Transfer' THEN tp.PaidAmount
		WHEN mt1.Type='Foreman Commission' THEN tp.PaidAmount
		WHEN mt1.Type='Discount' THEN tp.PaidAmount
		WHEN mt1.Type='Dividend Reversal' THEN tp.PaidAmount
		WHEN mt1.Type='Charges' THEN tp.PaidAmount
		WHEN mt1.Type='Interest' THEN tp.PaidAmount
		ELSE 0
		END AS Debit,
	CASE
		WHEN mt1.Type='Dividend' THEN tp.PaidAmount
		--WHEN mt1.Type='Interest' THEN tp.PaidAmount
		WHEN mt1.Type='Subscription' THEN (Case WHEN mt.Type in('Transfer') then (Case WHEN AmountCodeConstant > 0 THEN tp.PaidAmount / AmountCodeConstant Else tp.PaidAmount End) Else tp.PaidAmount End)
		WHEN mt1.Type='Waiver' THEN tp.PaidAmount
		ELSE 0
		END AS Credit,
		0.00, ChitSubscriptionCode, SubscriptionID, 29, isnull(ca1.catename,'') + ', '+ isnull(ca2.catename,'')
	--tp.Paidamount,'',
	from ChitSubscription ls  (nolock)  
	left join EMPLOYEEMASTER ca1 (nolock) on ls.CollectionAgentPersonID1 = ca1.CATEID
	left join EMPLOYEEMASTER ca2 (nolock) on ls.CollectionAgentPersonID2 = ca2.CATEID
	left outer join [dbo].[TransactionPaymentList] tpl  (nolock) on tpl.ProductID = ls.SubscriptionID
	left outer join [dbo].[TransactionPayment] tp (nolock)  on tp.PaymentListID =tpl.PaymentListID
	left outer join [dbo].[TransactionCollectionReciepts] tcr  (nolock) on tcr.CollectionListID = tp.PaymentListID	
	left outer join [dbo].[MasterType]  mt  (nolock) on tpl.TransactionModeTypeID = mt.TypeID
	left outer join [dbo].[MasterType]  mt1  (nolock) on tpl.TransactionCategoryTypeID = mt1.TypeID
	inner join [dbo].ChitScheme  (nolock) ON ls.ChitSchemeID = dbo.ChitScheme.ChitSchemeID
	inner join [dbo].MasterAmountCode  (nolock) ON [dbo].ChitScheme.AmountCodeID = [dbo].MasterAmountCode.AmountCodeID
	left outer join (TransactionBookVerification bv(nolock) join MasterType mt3(nolock) on bv.VerificationFindingTypeID = mt3.TypeID)
			on tcr.CollectionRecieptID = bv.ReferenceCollectionReceiptID
	where tp.PaymentDateTime between @FromDate and @ToDate and tp.PaymentDateTime is not null and mt.Type is not null 
	and tpl.StatusTypeID in (1035,1036)
	AND ProductTypeTypeID = 29
	and (@product = 29 or @product =0)
	and (ChitSubscriptionCode = @account  or @account ='')
	and (ls.PersonID = @customer or @customer = 0)
	and (ls.RouteID = @routeid or @routeid=0)
	and (ca1.cateid = @collectionagent or ca2.cateid = @collectionagent or (@collectionagent=0 and @usrid =''))
	--GROUP  BY 
	--CONVERT(varchar(10),tp.PaymentDateTime, 120),tcr.PrintRecepitCode,mt.Type,tp.PaidAmount,mt1.Type
	--order by CONVERT(varchar,tp.PaymentDateTime, 103) asc

	--Loan For Tenure
	-- select 'start main' , getdate()
	insert into @TableMasterMain(AccountNumber , SubscriptionID, ProductTypeid, CollectionAgent)
	select distinct  AccountNumber,  subscriptionid, ProductTypeid, CollectionAgent
	from @TableMaster
	union
	select distinct  AccountNumber,  subscriptionid, ProductTypeid, CollectionAgent
	from @TableMaster_02
	union
	select distinct  AccountNumber,  subscriptionid, ProductTypeid, CollectionAgent
	from @TableMaster_05

	
	---- select 'start loan update ' , getdate()
	--update tm
	--set		Loanamount = isnull(ls.LoanAmount,0), 
	--		tenure = IsNull(lsc.TenureBased,0)
	--from  LoanRequest lr(nolock)
	--		join person pl(nolock) on lr.PersonID = pl.PersonID
	--		join LoanSubscription ls (nolock) on lr.LoanRequestID = ls.LoanRequestID
	--		join @TableMasterMain tm on ls.LoanSubscriptionCode = tm.AccountCode and tm.AccountCode like 'L%'
	--		join LoanScheme lsc(nolock) on ls.LoanSchemeID = lsc.LoanSchemeID
	--		join CommonAccountBook ca(nolock) on ls.LoanSubscriptionID = ca.OwnerID
	--		join MasterType mt(nolock) on ca.OwnerTypeID = mt.TypeID
	--		left join CompanyEmployeeProductMap tpm (nolock) on ls.LoanSubscriptionID = tpm.SchemeID
	--		left join Organization org (nolock) on ls.AssociatedOrganizationID = org.OrganizationID			
	--	where lr.StatusTypeID = 968
	--	and ca.StatusTypeID = 836
	--	and ca.OwnerTypeID = 187

	---- select 'start book verification update' , getdate()
	--update tm
	--set BookVerifiedDate = bvdt
	--from @TableMasterMain tm join #bookvdt bv on tm.AccountCode = bv.LoanSubscriptionCode

	-- select 'start tmp' , getdate()
	select convert(varchar(10), date, 120) as Date, 
			   Mode,
			   ReceiptNo,
			   Category, 
			   Debit, 
			   Credit, 
			--		--SUM((CAST(credit AS int) - CAST(debit AS int))) OVER (ORDER BY date ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW)
			--	case when tm.accountcode like 'L%' then	SUM(Case tmn.tenure When 1 then (Case When Category IN('Interest - Prepaid', 'Principal Transfer Delete') 
			--		then 0 Else (Case When Category in('Principal Transfer', 'Charges') Then (CAST(Debit AS decimal(18,2))) * 0 Else (CAST(credit AS decimal(18,2))) 
			--		End) End) Else (CAST(credit AS decimal(18,2))) End) OVER (ORDER BY date ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW)
			--		when tm.accountcode like 'D%' then SUM((CAST(credit AS decimal(18,2)) - (Case When Category in('Payment','Payment Transfer')
			--		then CAST(debit AS decimal(18,2)) Else 0 End))) OVER (ORDER BY date ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW)
			--		else SUM((CAST(credit AS decimal(18,2)) - (Case When Category in('Charges', 'Dividend Reversal','Prize Transfer','Foreman Commission Delete','Prize111') then CAST(debit AS decimal(18,2)) Else 0 End))) 
			--		OVER (ORDER BY date ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) end
					
			--AS Cumilative  ,
			tm.AccountNumber, tm.subscriptionid, tm.ProductTypeid, BookVerifiedDate, tenure , cast('' as varchar(25)) TransferStatus, 
			tm.CollectionAgent, cast('' as varchar(100)) CustomerName
	INTO #TMP
			from @TableMaster tm join @TableMasterMain tmn on tm.AccountNumber = tmn.AccountNumber
			and tm.subscriptionid = tmn.SubscriptionID
			order by CONVERT(DateTime, Date,101)


	
	--Loan For Non Tenure
	-- select 'start tmp5' , getdate()
	select convert(varchar(10), date, 120) as Date, 
			   Mode,
			   ReceiptNo,
			   Category, 
			   Debit, 
			   Credit, Principal,Interest,
			tm.AccountNumber, tm.subscriptionid, tm.ProductTypeid, BookVerifiedDate, tenure , cast('' as varchar(25)) TransferStatus,
			tm.CollectionAgent, cast('' as varchar(100)) CustomerName
			INTO #TMP05 
			from @TableMaster_05 tm join @TableMasterMain tmn on tm.AccountNumber = tmn.AccountNumber 
			and tm.subscriptionid = tmn.SubscriptionID
			order by CONVERT(DateTime, Date,101)

	--delete #TMP where AccountNumber like 'l%' and ReceiptNo in (select ReceiptNo  from #TMP05)
	delete #TMP where ProductTypeid = 30 and ReceiptNo in (select ReceiptNo  from #TMP05)

	-- select 'start final select' , getdate()
	
	update #TMP
	set  TransferStatus = case	when ReceiptNo ='' or ReceiptNo  like 'Source A/c:%' then 'N/A'
								when b.TRANSACTIONDET_vRecieptno is null then 'Failed' Else 'Imported' end
	from #TMP a left join TRANSACTIONDET b(nolock) on a.ReceiptNo = b.TRANSACTIONDET_vRecieptno

	update #TMP05
	set  TransferStatus = case	when a.ReceiptNo =''  or ReceiptNo  like 'Source A/c:%' then 'N/A'
								when b.TRANSACTIONDET_vRecieptno is null then 'Failed' Else 'Imported' end
	from #TMP05 a left join TRANSACTIONDET b(nolock) on a.ReceiptNo = b.TRANSACTIONDET_vRecieptno

	
	update #TMP05
	set  CustomerName =  [Customer Name]
	from #TMP05 a left join VW_All_Subscription_Details b(nolock) on a.AccountNumber = b.SubscriptionCode

	update #TMP
	set  CustomerName =  [Customer Name]
	from #TMP a left join VW_All_Subscription_Details b(nolock) on a.AccountNumber = b.SubscriptionCode

	

	SELECT CustomerName, [Date],Mode,ReceiptNo,Category, Debit, Credit, 0.00 Principal, 0.00 Interest, AccountNumber, 
	subscriptionid,  CollectionAgent -- TransferStatus,
	FROM #TMP
	union
	select CustomerName, [Date],Mode,ReceiptNo,Category, Debit, Credit, Principal, Interest, AccountNumber, 
	subscriptionid,  CollectionAgent -- TransferStatus,
	from #TMP05
	order by 1,2,3

	declare @datewisesummary table
	(
		TranDate varchar(10),
		ProductType varchar(25),
		Debit numeric(18,2),
		Credit numeric(18,2),
		Principal numeric(18,2),
		Interest numeric(18,2)

	)

	insert into @datewisesummary
	SELECT [Date], case when ProductTypeid=29 then 'Chits'
	when ProductTypeid=30 then 'Loans'
	when ProductTypeid=31 then 'Deposits' Else 'Others' End,
	sum(Debit) 'Debit', Sum(Credit) 'Credit', 0.0, 0.0
	FROM #TMP
	group by [Date], case when ProductTypeid=29 then 'Chits'
	when ProductTypeid=30 then 'Loans'
	when ProductTypeid=31 then 'Deposits' Else 'Others' End

	insert into @datewisesummary
	select [Date], case when ProductTypeid=29 then 'Chits'
	when ProductTypeid=30 then 'Loans'
	when ProductTypeid=31 then 'Deposits' Else 'Others' End,
	sum(Debit), Sum(Credit), Sum(Principal), Sum(Interest)
	from #TMP05
	group by [Date], case when ProductTypeid=29 then 'Chits'
	when ProductTypeid=30 then 'Loans'
	when ProductTypeid=31 then 'Deposits' Else 'Others' End
		
	select TranDate, ProductType,
	sum(Debit) Debit, Sum(Credit)  Credit, Sum(Principal) Principal, Sum(Interest) Interest
	from @datewisesummary
	group by TranDate, ProductType
	order by 1,2,3

	declare @colagtwisesummary table
	(
		CollectionAgent varchar(150),
		ProductType varchar(25),
		Debit numeric(18,2),
		Credit numeric(18,2),
		Principal numeric(18,2),
		Interest numeric(18,2)

	)

	insert into @colagtwisesummary
	SELECT CollectionAgent, case when ProductTypeid=29 then 'Chits'
	when ProductTypeid=30 then 'Loans'
	when ProductTypeid=31 then 'Deposits' Else 'Others' End,
	sum(Debit) 'Debit', Sum(Credit) 'Credit', 0.0, 0.0
	FROM #TMP
	group by CollectionAgent, case when ProductTypeid=29 then 'Chits'
	when ProductTypeid=30 then 'Loans'
	when ProductTypeid=31 then 'Deposits' Else 'Others' End

	insert into @colagtwisesummary
	select CollectionAgent, case when ProductTypeid=29 then 'Chits'
	when ProductTypeid=30 then 'Loans'
	when ProductTypeid=31 then 'Deposits' Else 'Others' End,
	sum(Debit), Sum(Credit), Sum(Principal), Sum(Interest)
	from #TMP05
	group by CollectionAgent, case when ProductTypeid=29 then 'Chits'
	when ProductTypeid=30 then 'Loans'
	when ProductTypeid=31 then 'Deposits' Else 'Others' End
		
	select CollectionAgent, ProductType,
	sum(Debit) Debit, Sum(Credit)  Credit, Sum(Principal) Principal, Sum(Interest) Interest
	from @colagtwisesummary
	group by CollectionAgent,ProductType
	order by 1,2,3

	--drop table #bookvdt
	drop table #TMP
	drop table #TMP05
end


