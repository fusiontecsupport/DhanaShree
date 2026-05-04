-- exec pr_Get_Today_Collection '2022-08-05','',29
-- EXEC pr_Get_Today_Collection @indt ='2022-08-08', @usrid = '', @prod = 0

alter proc pr_Get_Today_Collection
@indt datetime,
@usrid varchar(100),
@empid int,
@prod int
as
begin
	DECLARE @CollDtl1 TABLE 
        ( 
           EnteredBy varchar(100),
		   Client varchar(250),
		   SubsCode varchar(25),
		   CollectedAmt numeric(18,2)
        ) 

	--declare @empid int
	--select @empid = EmpId from aspnetusers (nolock) where username = @usrid

	Insert @CollDtl1
	select  tcl.collectionagent, LoanSubscriptionCode+'-'+ cl.CLIENTNAME,	loansubscriptioncode,
	CASE
					WHEN mt1.Type in('Principal Repayment', 'Interest') THEN tcr.CollectedAmount
					WHEN mt1.Type='Interest - Prepaid' THEN tcr.CollectedAmount
					WHEN mt1.Type='Interest TDS' THEN tcr.CollectedAmount
					WHEN mt1.Type='Interest Waiver' THEN tcr.CollectedAmount
					WHEN mt1.Type='Principal Waiver' THEN tcr.CollectedAmount

					ELSE 0
			   END
	from LoanSubscription ls  (nolock)
	join LoanRequest lr(nolock) on ls.LoanRequestID = lr.LoanRequestID
	join ClientMaster cl (nolock) on lr.PersonID = cl.CLIENTID
	left outer join [dbo].[TransactionCollectionList] tcl (nolock) on tcl.ProductID=ls.LoanSubscriptionID and tcl.ProductTypeID = 30
	left outer join [dbo].[TransactionCollectionReciepts] tcr (nolock) on tcr.CollectionListID = tcl.CollectionListID
	left outer join [dbo].PaymentTransferLog pymt (nolock) on tcl.CollectionListID=pymt.CollectionListID and substring(TargetProductCode ,1,1) in ('C','L','D')
	left outer join [dbo].[MasterType]  mt (nolock) on tcl.TransactionModeTypeID = mt.TypeID
	left outer join [dbo].[MasterType]  mt1 (nolock) on tcl.TransactionCategoryTypeID = mt1.TypeID
	where CONVERT(varchar(10), tcr.CollectionDateTime, 120)=@indt
	and (collectionagent =@usrid or @usrid='' or AccountEmployeeID= @empid)
	and tcr.CollectionDateTime is not null and mt.Type is not null and tcl.StatusTypeID in (1035,1036)	
	and (tcl.ProductTypeID = @prod or (tcl.ProductTypeID = 30 and @prod=0))	
	order by CONVERT(varchar, tcr.CollectionDateTime, 103) asc

	
	Insert @CollDtl1
	
	select  tcl.collectionagent, ChitSubscriptionCode+'-'+ cl.CLIENTNAME,chitsubscriptioncode,
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
	from ChitSubscription ls 
	join ClientMaster cl (nolock) on ls.PersonID = cl.CLIENTID
	left outer join [dbo].[TransactionCollectionList] tcl (nolock) on tcl.ProductID=ls.SubscriptionID
	left outer  join [dbo].[TransactionCollectionReciepts] tcr (nolock) on tcr.CollectionListID = tcl.CollectionListID
	left outer join [dbo].PaymentTransferLog pymt (nolock) on tcr.CollectionListID=pymt.CollectionListID and substring(TargetProductCode ,1,1) in ('C','L','D')
	left outer join [dbo].[MasterType]  mt (nolock) on tcl.TransactionModeTypeID = mt.TypeID
	left outer join [dbo].[MasterType]  mt1 (nolock) on tcl.TransactionCategoryTypeID = mt1.TypeID
	left join [dbo].ChitScheme  (nolock) ON ls.ChitSchemeID = dbo.ChitScheme.ChitSchemeID
	left join [dbo].MasterAmountCode  (nolock) ON [dbo].ChitScheme.AmountCodeID = [dbo].MasterAmountCode.AmountCodeID
	--left outer join (TransactionBookVerification bv(nolock) join MasterType mt3(nolock) on bv.VerificationFindingTypeID = mt3.TypeID)
	--		on tcr.CollectionRecieptID = bv.ReferenceCollectionReceiptID
	where CONVERT(varchar(10), tcr.CollectionDateTime, 120)=@indt
	and (collectionagent =@usrid or @usrid='' or @empid = CollectionAgentPersonID1 or @empid = CollectionAgentPersonID2)
	and tcr.CollectionDateTime is not null and mt.Type is not null and tcl.StatusTypeID in (1035,1036) 
	and (ProductTypeID = @prod or (ProductTypeID = 29 and @prod=0))

	select * from @CollDtl1
end


