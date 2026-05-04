 -- exec pr_db_today_Summary '',1,'2022-04-01', '2023-03-31',0
 -- exec pr_db_today_Summary 'maheshwaran',23,'2022-04-01', '2023-03-31',0
alter proc pr_db_today_Summary
@usrid varchar(100),
@empid int,
@fdt datetime,
@tdt datetime,
@prod int=0
as
begin

	declare @curdt datetime
	set @curdt = convert(varchar(10),getdate(),120)
	
	if exists(select '*'
	-- select u.*, g.*
	from AspNetUsers u(nolock) join ApplicationUserGroups ug(nolock) on u.Id = ug.ApplicationUserId
	join ApplicationGroups g(nolock) on ug.ApplicationGroupId = g.Id
	and g.name like '%admin%'
	where u.UserName = @usrid)
		select @usrid=''

	declare @tbl table
	(
		Dt varchar(50),
		YrMon varchar(10),
		WkPrd varchar(25),
		Product varchar(50),
		Category varchar(25),		
		Mode varchar(50),
		SubCategory Varchar(100),
		CollectionAgent varchar(100),
		DebitAmount numeric(18,2),
		CreditAmount numeric(18,2)
	)
	
	
	insert into @tbl
	select CONVERT(varchar, tcr.CollectionDateTime, 106), CONVERT(varchar(7), tcr.CollectionDateTime, 120),'', 'Loan',
	'Collection', mt.Type, 
	CASE WHEN mt1.Type='Interest' THEN 'Principal Repayment' ELSE mt1.Type END Type1,
	e.catename,
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
			AS Credit
	--, CASE WHEN mt1.Type in ('Principal Repayment','Principal Waiver','Interest TDS') THEN tcr.CollectedAmount ELSE 0 END AS Principal,
	--CASE WHEN mt1.Type='Interest' THEN tcr.CollectedAmount ELSE 0 END AS Interest,

	--		0.00
	from LoanSubscription ls (nolock)
	join LoanRequest lr(nolock) on ls.loanrequestid = lr.loanrequestid
	join [dbo].[TransactionCollectionList] tcl  (nolock) on tcl.ProductID=ls.LoanSubscriptionID and tcl.ProductTypeID = 30
	left outer  join [dbo].[TransactionCollectionReciepts] tcr (nolock) on tcr.CollectionListID = tcl.CollectionListID
	left outer join [dbo].PaymentTransferLog pymt (nolock) on tcl.CollectionListID=pymt.CollectionListID and substring(TargetProductCode ,1,1) ='L'
	left outer join [dbo].[MasterType]  mt (nolock) on tcl.TransactionModeTypeID = mt.TypeID
	left outer join [dbo].[MasterType]  mt1 (nolock) on tcl.TransactionCategoryTypeID = mt1.TypeID
	left join EMPLOYEEMASTER e(nolock) on lr.AccountEmployeeID = e.cateid
	where (@usrid = '' or @empid = AccountEmployeeID)
	and tcr.CollectionDateTime between @fdt and @tdt
	and mt.Type is not null and tcl.StatusTypeID in (1035,1036)
	AND tcl.ProductTypeID = 30 
	--and (tcl.ProductTypeID = @prod or @prod=0) 
	GROUP  BY tcr.CollectionDateTime, tcr.PrintRecepitCode,mt.Type,tcr.CollectedAmount,mt1.Type , TargetProductCode ,
	e.catename
	order by CONVERT(varchar, tcr.CollectionDateTime, 106) asc

	insert into @tbl
	select CONVERT(varchar, tp.PaymentDateTime, 106), CONVERT(varchar(7), tp.PaymentDateTime, 120),'', 'Loan',
	'Payment', mt.Type, 	mt1.Type,
	e.catename,
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
			AS Credit
	from LoanSubscription ls (nolock) join LoanRequest lr(nolock) on ls.LoanRequestID = lr.LoanRequestID
	--left outer join [dbo].[TransactionCollectionList] tcl  (nolock) on tcl.ProductID = ls.LoanSubscriptionID  -- where  CollectionListID=4676
	join [dbo].[TransactionPaymentList] tpl (nolock) on tpl.ProductID = ls.LoanSubscriptionID and tpl.ProductTypeTypeID=30
	left outer join [dbo].[TransactionPayment] tp (nolock) on tp.PaymentListID =tpl.PaymentListID
	--left outer join [dbo].PaymentTransferLog pymt (nolock) on tpl.PaymentListID=pymt.PaymentListID
	--left outer join [dbo].[TransactionCollectionReciepts] tcr (nolock) on tcr.CollectionListID = tp.PaymentListID
	left outer join [dbo].[MasterType]  mt (nolock) on tpl.TransactionModeTypeID = mt.TypeID
	left outer join [dbo].[MasterType]  mt1 (nolock) on tpl.TransactionCategoryTypeID = mt1.TypeID
	left join EMPLOYEEMASTER e(nolock) on lr.AccountEmployeeID = e.cateid
	where (@usrid = '' or @empid = AccountEmployeeID)
	and tp.PaymentDateTime between @fdt and @tdt
	and mt.Type is not null and tp.StatusTypeID in (1035,1036)
	AND tpl.ProductTypeTypeID = 30
	--GROUP  BY tp.PaymentDateTime,tcr.PrintRecepitCode,mt.Type,tp.PaidAmount,mt1.Type
	order by CONVERT(varchar,tp.PaymentDateTime, 106) asc

	insert into @tbl
	select CONVERT(varchar, tcr.CollectionDateTime, 106), CONVERT(varchar(7), tcr.CollectionDateTime, 120),'', 'Chit',
	'Collection', mt.Type, mt1.Type, catename,
	CASE
		--WHEN mt1.Type='Prize' THEN (Case When AmountCodeConstant > 0 Then tcr.CollectedAmount/ AmountCodeConstant Else tcr.CollectedAmount End)
		WHEN mt1.Type='Prize' THEN  tcr.CollectedAmount
		WHEN mt1.Type='Prize Transfer' THEN tcr.CollectedAmount
		WHEN mt1.Type= 'Agent Commission' THEN tcr.CollectedAmount 
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
		AS Credit
	--, CASE WHEN mt1.Type in ('Principal Repayment','Principal Waiver','Interest TDS') THEN tcr.CollectedAmount ELSE 0 END AS Principal,
	--CASE WHEN mt1.Type='Interest' THEN tcr.CollectedAmount ELSE 0 END AS Interest,

	--		0.00
	from ChitSubscription ls  (nolock)
	left outer join [dbo].[TransactionCollectionList] tcl (nolock) on tcl.ProductID=ls.SubscriptionID and tcl.ProductTypeID = 29
	left outer  join [dbo].[TransactionCollectionReciepts] tcr (nolock) on tcr.CollectionListID = tcl.CollectionListID
	left outer join [dbo].PaymentTransferLog pymt  (nolock) on tcl.CollectionListID=pymt.CollectionListID and substring(TargetProductCode ,1,1) in ('C','L','D')
	left outer join [dbo].[MasterType]  mt  (nolock) on tcl.TransactionModeTypeID = mt.TypeID
	left outer join [dbo].[MasterType]  mt1  (nolock)  on tcl.TransactionCategoryTypeID = mt1.TypeID
	left join EMPLOYEEMASTER e(nolock) on ls.CommisionAgentPersonID= e.cateid
	inner join [dbo].ChitScheme (nolock) ON ls.ChitSchemeID = dbo.ChitScheme.ChitSchemeID
	left join [dbo].MasterAmountCode   (nolock)  ON [dbo].ChitScheme.AmountCodeID = [dbo].MasterAmountCode.AmountCodeID
	where (@usrid = ''  or @empid = ls.CollectionAgentPersonID1 or @empid = ls.CollectionAgentPersonID2)
	and tcr.CollectionDateTime between @fdt and @tdt
	and mt.Type is not null and tcl.StatusTypeID in (1035,1036)
	AND tcl.ProductTypeID = 29 
	--and (tcl.ProductTypeID = @prod or @prod=0) 
	GROUP  BY tcr.CollectionDateTime, tcr.PrintRecepitCode,mt.Type,tcr.CollectedAmount,mt1.Type , TargetProductCode ,
	e.catename, AmountCodeConstant 
	order by CONVERT(varchar, tcr.CollectionDateTime, 106) asc


	insert into @tbl
	select CONVERT(varchar, tp.PaymentDateTime, 106), CONVERT(varchar(7), tp.PaymentDateTime, 120),'', 'Chit',
	'Payment', mt.Type, 	mt1.Type,	e.catename,
	CASE WHEN mt1.Type='Prize' THEN (Case When AmountCodeConstant > 0 Then tp.PaidAmount Else tp.PaidAmount End) --tp.PaidAmount
		WHEN mt1.Type='Prize Transfer' THEN tp.PaidAmount
		WHEN mt1.Type= 'Agent Commission' THEN tp.PaidAmount	
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
		END AS Credit
	from ChitSubscription ls (nolock) 
	--left outer join [dbo].[TransactionCollectionList] tcl on tcl.ProductID = ls.LoanSubscriptionID  -- where  CollectionListID=4676
	left outer join [dbo].[TransactionPaymentList] tpl  (nolock)  on tpl.ProductID = ls.SubscriptionID and tpl.ProductTypeTypeID=29
	left outer join [dbo].[TransactionPayment] tp  (nolock)  on tp.PaymentListID =tpl.PaymentListID
	--left outer join [dbo].PaymentTransferLog pymt  (nolock)  on tpl.PaymentListID=pymt.PaymentListID
	--left outer join [dbo].[TransactionCollectionReciepts] tcr  (nolock)  on tcr.CollectionListID = tp.PaymentListID
	left outer join [dbo].[MasterType]  mt  (nolock)  on tpl.TransactionModeTypeID = mt.TypeID
	left outer join [dbo].[MasterType]  mt1   (nolock) on tpl.TransactionCategoryTypeID = mt1.TypeID
	left join EMPLOYEEMASTER e(nolock) on ls.CommisionAgentPersonID= e.cateid
	inner join [dbo].ChitScheme  (nolock)  ON ls.ChitSchemeID = dbo.ChitScheme.ChitSchemeID
	left join [dbo].MasterAmountCode  (nolock)  ON [dbo].ChitScheme.AmountCodeID = [dbo].MasterAmountCode.AmountCodeID
	where (@usrid = ''  or @empid = ls.CollectionAgentPersonID1 or @empid = ls.CollectionAgentPersonID2)
	and tp.PaymentDateTime between @fdt and @tdt
	and mt.Type is not null and tp.StatusTypeID in (1035,1036)
	AND tpl.ProductTypeTypeID = 29
	--GROUP  BY tp.PaymentDateTime,tcr.PrintRecepitCode,mt.Type,tp.PaidAmount,mt1.Type
	order by CONVERT(varchar,tp.PaymentDateTime, 106) asc
	

	select 'Today''s Summary' 'Heading',Product,Mode,
	--isnull(sum(debitamount),0) 'Debit', isnull(sum(creditamount),0) 'Credit'
	isnull(sum(debitamount),0) + isnull(sum(creditamount),0) 'Amount'
	from @tbl where dt =  CONVERT(varchar, @curdt, 106)
	group by Product,Mode
	union all	
	select 'Today''s Summary' 'Heading','','Today Total',
	--isnull(sum(debitamount),0) 'Debit', isnull(sum(creditamount),0) 'Credit'
	isnull(sum(debitamount),0) + isnull(sum(creditamount),0) 'Amount'
	from @tbl where dt =  CONVERT(varchar, @curdt, 106)
	

	select 'Last 7 Days Summary' 'Heading',Product,Category,
	--isnull(sum(debitamount),0) 'Debit', isnull(sum(creditamount),0)  'Credit'
	isnull(sum(debitamount),0) + isnull(sum(creditamount),0) 'Amount'
	from @tbl where dt >=   CONVERT(varchar, @curdt-7, 106) and dt <=  CONVERT(varchar, @curdt, 106) 
	group by Product,Category
	union all	
	select 'Last 7 Days Summary' 'Heading','','Week Total',
	--isnull(sum(debitamount),0) 'Debit', isnull(sum(creditamount),0)  'Credit'
	isnull(sum(debitamount),0) + isnull(sum(creditamount),0) 'Amount'
	from @tbl where dt >=   CONVERT(varchar, @curdt-7, 106) and dt <=  CONVERT(varchar, @curdt, 106) 
	
	select 'Current Month Summary' 'Heading',Product,Category,Mode,
	--isnull(sum(debitamount),0) 'Debit', isnull(sum(creditamount),0)  'Credit'
	isnull(sum(debitamount),0) + isnull(sum(creditamount),0) 'Amount'
	from @tbl where yrmon = CONVERT(varchar(7),  @curdt, 120)
	group by Product,Category,Mode
	union all	
	select 'Current Month Summary' 'Heading','','','Month Total',
	--isnull(sum(debitamount),0) 'Debit', isnull(sum(creditamount),0)  'Credit'
	isnull(sum(debitamount),0) + isnull(sum(creditamount),0) 'Amount'
	from @tbl  where yrmon = CONVERT(varchar(7),  @curdt, 120)

	--select 'Month wise Summary' 'Heading',yrmon, Product+' '+ Category 'Desc',	
	--abs(isnull(sum(creditamount),0) - isnull(sum(debitamount),0))  'Amt'
	--from @tbl 
	--group by Product,Category,yrmon
	--union all	
	--select 'Month wise Summary' 'Heading','','ZNet Total',
	--abs(isnull(sum(creditamount),0) - isnull(sum(debitamount),0))  'Amt'
	--from @tbl  

	
	select 'Today''s - Agent wise Collection Summary' 'Heading',Product,CollectionAgent,Mode,
	--isnull(sum(debitamount),0) 'Debit', isnull(sum(creditamount),0) 'Credit'
	isnull(sum(debitamount),0) + isnull(sum(creditamount),0) 'Amount'
	from @tbl 
	where dt =  CONVERT(varchar, @curdt, 106) and category = 'Collection'
	group by Product,CollectionAgent,Mode
	union all
	select 'Today - Agent wise Collection Summary' 'Heading','Total','','',
	--isnull(sum(debitamount),0) 'Debit', isnull(sum(creditamount),0) 'Credit'
	isnull(sum(debitamount),0) + isnull(sum(creditamount),0) 'Amount'
	from @tbl 
	where dt =  CONVERT(varchar, @curdt, 106) and category = 'Collection'	
	
	select 'Today''s Loan Collection' 'Heading', ls.LoanSubscriptionCode 'SubsCode', cl.CLIENTNAME 'Client', 
	cl.CLIENTMOBILENO1 'Mobile No.', lr.Principal, 
	ls.LoanAmount, ls.DisbursementDate, tcl.GeneratedAmount 'Amt',
	slr.type 'ReqSts', sls.Type 'LSubsSts', mt2.type 'TranSts'
	from loanrequest lr (nolock) 
		join loansubscription ls(nolock) on lr.LoanRequestID = ls.LoanRequestID
		join [dbo].[TransactionCollectionList] tcl  (nolock) on tcl.ProductID=ls.LoanSubscriptionID and tcl.ProductTypeID = 30
		left outer  join [dbo].[TransactionCollectionReciepts] tcr (nolock) on tcr.CollectionListID = tcl.CollectionListID
		--left outer join [dbo].PaymentTransferLog pymt (nolock) on tcl.CollectionListID=pymt.CollectionListID and substring(TargetProductCode ,1,1) ='L'
		--left outer join [dbo].[MasterType]  mt (nolock) on tcl.TransactionModeTypeID = mt.TypeID
		--left outer join [dbo].[MasterType]  mt1 (nolock) on tcl.TransactionCategoryTypeID = mt1.TypeID
		--left join EMPLOYEEMASTER e(nolock) on lr.AccountEmployeeID = e.cateid
		left join MasterType slr (nolock) on lr.StatusTypeID = slr.TypeID
		left join MasterType sls (nolock) on ls.StatusTypeID = sls.TypeID
		join ClientMaster cl(nolock) on lr.PersonID = cl.clientid		
		left outer join [dbo].[MasterType]  mt2 on tcl.StatusTypeID = mt2.TypeID
	where tcr.CollectionDateTime = @curdt
	and (@usrid ='' or lr.AccountEmployeeID = @empid)
	union
	select 'Today''s Loan Collection' 'Heading', 'Total', '', '', null, null, null, 
	sum(tcl.GeneratedAmount) 'Amt',	'' 'ReqSts', '' 'LSubsSts', ''
	from loanrequest lr (nolock) 
	--	join loansubscription ls(nolock) on lr.LoanRequestID = ls.LoanRequestID
	--	left join MasterType slr (nolock) on lr.StatusTypeID = slr.TypeID
	--	left join MasterType sls (nolock) on ls.StatusTypeID = sls.TypeID
		
	--	join ClientMaster cl(nolock) on lr.PersonID = cl.clientid
	--	left join transactionpaymentlist tpl(nolock) on ls.LoanSubscriptionID = tpl.ProductID and tpl.ProductTypeTypeID = 30
	--	left outer join [dbo].[TransactionPayment] tp on tp.PaymentListID =tpl.PaymentListID
	--	left outer join [dbo].[MasterType]  mt2 on tpl.StatusTypeID = mt2.TypeID
	--where tpl.ProductID is null
	--and tp.PaymentDateTime= @curdt
	join loansubscription ls(nolock) on lr.LoanRequestID = ls.LoanRequestID
		join [dbo].[TransactionCollectionList] tcl  (nolock) on tcl.ProductID=ls.LoanSubscriptionID and tcl.ProductTypeID = 30
		left outer  join [dbo].[TransactionCollectionReciepts] tcr (nolock) on tcr.CollectionListID = tcl.CollectionListID
		--left outer join [dbo].PaymentTransferLog pymt (nolock) on tcl.CollectionListID=pymt.CollectionListID and substring(TargetProductCode ,1,1) ='L'
		--left outer join [dbo].[MasterType]  mt (nolock) on tcl.TransactionModeTypeID = mt.TypeID
		--left outer join [dbo].[MasterType]  mt1 (nolock) on tcl.TransactionCategoryTypeID = mt1.TypeID
		--left join EMPLOYEEMASTER e(nolock) on lr.AccountEmployeeID = e.cateid
		left join MasterType slr (nolock) on lr.StatusTypeID = slr.TypeID
		left join MasterType sls (nolock) on ls.StatusTypeID = sls.TypeID
		join ClientMaster cl(nolock) on lr.PersonID = cl.clientid		
		left outer join [dbo].[MasterType]  mt2 on tcl.StatusTypeID = mt2.TypeID
	where tcr.CollectionDateTime = @curdt
	and (@usrid ='' or lr.AccountEmployeeID = @empid)

	select 'Today''s Chit Collection' 'Heading' , ls.ChitSubscriptionCode 'Subs.Code', cl.CLIENTNAME 'Client', cl.CLIENTMOBILENO1 'Mobile No.', 
	e.CATENAME 'CollectionAgent', cs.ChitValue, 	mt.Type 'Mode', mt1.Type 'Category',
	TranAmt = case WHEN mt1.Type='Dividend' THEN tcr.CollectedAmount
		--WHEN mt1.Type='Interest' THEN tcr.CollectedAmount
		--WHEN mt1.Type='Subscription' THEN tcr.CollectedAmount
		--WHEN mt1.Type='Subscription' THEN (Case WHEN mt.Type in('Transfer') then (Case WHEN AmountCodeConstant > 0 THEN tcr.CollectedAmount / AmountCodeConstant Else tcr.CollectedAmount End) Else tcr.CollectedAmount End)
		WHEN mt1.Type='Subscription' THEN (Case WHEN mt.Type in('Transfer') then (Case WHEN AmountCodeConstant > 0 THEN tcr.CollectedAmount  Else tcr.CollectedAmount End) Else tcr.CollectedAmount End)
		WHEN mt1.Type='Waiver' THEN tcr.CollectedAmount
		ELSE 0
		END, mt2.Type 'TranStatus'
	from ChitSubscription ls (nolock) join ClientMaster cl(nolock) on ls.PersonID = cl.CLIENTID
	left outer join [dbo].[TransactionCollectionList] tcl on tcl.ProductID=ls.SubscriptionID and tcl.ProductTypeID = 29
	left outer  join [dbo].[TransactionCollectionReciepts] tcr on tcr.CollectionListID = tcl.CollectionListID
	left outer join [dbo].PaymentTransferLog pymt on tcl.CollectionListID=pymt.CollectionListID and substring(TargetProductCode ,1,1) in ('C','L','D')
	left outer join [dbo].[MasterType]  mt on tcl.TransactionModeTypeID = mt.TypeID
	left outer join [dbo].[MasterType]  mt1 on tcl.TransactionCategoryTypeID = mt1.TypeID
	left outer join [dbo].[MasterType]  mt2 on tcl.StatusTypeID = mt2.TypeID
	left join EMPLOYEEMASTER e(nolock) on ls.CommisionAgentPersonID= e.cateid
	inner join [dbo].ChitScheme cs(nolock) ON ls.ChitSchemeID = cs.ChitSchemeID
	left join [dbo].MasterAmountCode (nolock) ON cs.AmountCodeID = [dbo].MasterAmountCode.AmountCodeID
	where (@usrid = ''  or @empid = ls.CollectionAgentPersonID1 or @empid = ls.CollectionAgentPersonID2)
	and tcr.CollectionDateTime = @curdt
	and mt.Type is not null --and tcl.StatusTypeID in (1035,1036)
	AND tcl.ProductTypeID = 29 
	union
	select 'Todays Chit Collection', 'Total','', '', '' 'CollectionAgent', null, 
	'' 'Mode', '' 'Category',
	TranAmt = sum(case WHEN mt1.Type='Dividend' THEN tcr.CollectedAmount
		--WHEN mt1.Type='Interest' THEN tcr.CollectedAmount
		--WHEN mt1.Type='Subscription' THEN tcr.CollectedAmount
		--WHEN mt1.Type='Subscription' THEN (Case WHEN mt.Type in('Transfer') then (Case WHEN AmountCodeConstant > 0 THEN tcr.CollectedAmount / AmountCodeConstant Else tcr.CollectedAmount End) Else tcr.CollectedAmount End)
		WHEN mt1.Type='Subscription' THEN (Case WHEN mt.Type in('Transfer') then (Case WHEN AmountCodeConstant > 0 THEN tcr.CollectedAmount  Else tcr.CollectedAmount End) Else tcr.CollectedAmount End)
		WHEN mt1.Type='Waiver' THEN tcr.CollectedAmount
		ELSE 0
		END),''
	from ChitSubscription ls (nolock) join ClientMaster cl(nolock) on ls.PersonID = cl.CLIENTID
	left outer join [dbo].[TransactionCollectionList] tcl on tcl.ProductID=ls.SubscriptionID and tcl.ProductTypeID = 29
	left outer  join [dbo].[TransactionCollectionReciepts] tcr on tcr.CollectionListID = tcl.CollectionListID
	left outer join [dbo].PaymentTransferLog pymt on tcl.CollectionListID=pymt.CollectionListID and substring(TargetProductCode ,1,1) in ('C','L','D')
	left outer join [dbo].[MasterType]  mt on tcl.TransactionModeTypeID = mt.TypeID
	left outer join [dbo].[MasterType]  mt1 on tcl.TransactionCategoryTypeID = mt1.TypeID
	left outer join [dbo].[MasterType]  mt2 on tcl.StatusTypeID = mt2.TypeID
	left join EMPLOYEEMASTER e(nolock) on ls.CommisionAgentPersonID= e.cateid
	inner join [dbo].ChitScheme cs(nolock) ON ls.ChitSchemeID = cs.ChitSchemeID
	left join [dbo].MasterAmountCode (nolock) ON cs.AmountCodeID = [dbo].MasterAmountCode.AmountCodeID
	where (@usrid = ''  or @empid = ls.CollectionAgentPersonID1 or @empid = ls.CollectionAgentPersonID2)
	and tcr.CollectionDateTime = @curdt
	and mt.Type is not null --and tcl.StatusTypeID in (1035,1036)
	AND tcl.ProductTypeID = 29 
	
	-- and  slr.Type = 'Created' or sls.Type = 'Created'	
end

