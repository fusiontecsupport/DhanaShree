-- exec pr_Get_Chit_Daily_Collection_Plan 0, 0 , 0, '2022-12-01', '2022-12-29'
alter proc pr_Get_Chit_Daily_Collection_Plan
@ProductTypeID int=29,
@RouteID int,
@CollectionAgentID int,
@FrDt Datetime,
@ToDt datetime
as
begin

	declare @ChitInfoTbl table
	(
	 ChitSubsId int, 
	 ChitSubsCode varchar(100),
	 ClientName varchar(150),
	 ChitValue numeric(18,2), 
	 ChitDuration int, 
	 ChitDurationinmonths numeric(18,2),
	 ChitDurationtype int, 
	 ChitDurationtypedesc varchar(100),  	 
	 ColnStartDate datetime, 
	 ColnEndDate datetime,
	 NextColnDate datetime,
	 ChitDurationgap varchar(10), 
	 ChitSchemeid int, 
	 ChitSchemeCode varchar(100),
	 Chitsubamt numeric(18,2), 
	 ActChitSubamt numeric(18,2),
	 ChitColIntTypeid int, 
	 CollIntTypeDesc varchar(100),	 
	 NoofTickets int, 
	 Term int,  	 
	 CollectionSts varchar(15),
	 ColnAmt  numeric(18,2),
	 BookPickupDate int,
	 RouteName varchar(150),
	 CollectionAgent varchar(250),
	 ProdTypId int,
	 ColnActualAmt  numeric(18,2)
	)

	declare @ChitInfoDtTbl table
	(
	 ChitSubsId int, 
	 ChitSubsCode varchar(100),
	 ClientName varchar(150),
	 ChitValue numeric(18,2), 
	 ChitDuration int, 
	 ChitDurationinmonths numeric(18,2),
	 ChitDurationtype int, 
	 ChitDurationtypedesc varchar(100),  	 
	 ColnStartDate datetime, 
	 ColnEndDate datetime,
	 NextColnDate datetime,
	 ChitDurationgap varchar(10), 
	 ChitSchemeid int, 
	 ChitSchemeCode varchar(100),
	 Chitsubamt numeric(18,2), 
	 ActChitSubamt numeric(18,2),
	 ChitColIntTypeid int, 
	 CollIntTypeDesc varchar(100),	 
	 NoofTickets int, 
	 Term int,  	 
	 CollectionSts varchar(15),
	 ColnAmt  numeric(18,2),
	 BookPickupDate int,
	 RouteName varchar(150),
	 CollectionAgent varchar(250),
	 ProdTypId int,
	 ColnActualAmt  numeric(18,2)
	)

	
	DECLARE @CollDtl1 TABLE 
        ( 
			prodtypeid int, 
			subsid int,
           [Date] Datetime, 
           Credit          decimal(18, 2)
        ) 

	declare @dys table
	(cdate datetime)

	declare @tdt datetime
	set @tdt = @frdt
	while (@tdt<=@todt)
	begin
		insert into @dys
		select @tdt
		set @tdt = @tdt  + 1
	end

	insert into @ChitInfoTbl
	select distinct SubscriptionID, SubscriptionCode , [Customer Name], Value, Duration, datediff(m,ColnStDt,ColnEdDt), DurationTypeId, DurationTypeDesc, ColnStDt, ColnEdDt, 
	case 
	when CollectionIntervalDesc = 'Daily' then convert(varchar(10),getdate(),120)
	when CollectionIntervalDesc = 'Weekly' then convert(varchar(10),dateadd(d,1,dateadd(wk,datediff(wk,colnstdt,getdate()),colnstdt)),120)
	when CollectionIntervalDesc = 'Monthly' then convert(varchar(8),getdate(),120)+cast(datepart(d,colnstdt) as varchar) end,
	'',	SchemeID, SchemeCode, SuggestedAmount, CollectionAmount,  CollectionIntervalTypeID, CollectionIntervalDesc, tickets, Duration, '',
	(case when Tickets ='' then 1 else Tickets end) * CollectionAmount, BookPickupDay, isnull(RouteName,''), isnull(ColAgt1,'')+', '+isnull(ColAgt2,''),  ProdTypeID, 0
	From VW_Active_Subscription_Details (nolock), @dys
	where (ProdTypeID = @ProductTypeID or @ProductTypeID = 0)
	and (CollectionAgentPersonID1 = @CollectionAgentID or CollectionAgentPersonID2 = @CollectionAgentID or @CollectionAgentID = 0)
	and (RouteID = @RouteID or @RouteID = 0)

	insert into @ChitInfoDtTbl
	select distinct SubscriptionID, SubscriptionCode , [Customer Name], Value, Duration, datediff(m,ColnStDt,ColnEdDt), DurationTypeId, DurationTypeDesc, ColnStDt, ColnEdDt, 
	case 
	when CollectionIntervalDesc = 'Daily' then convert(varchar(10),cdate,120)
	when CollectionIntervalDesc = 'Weekly' then convert(varchar(10),dateadd(d,1,dateadd(wk,datediff(wk,colnstdt,cdate),colnstdt)),120)
	when CollectionIntervalDesc = 'Monthly' then convert(varchar(8),cdate,120)+cast(datepart(d,colnstdt) as varchar) end,
	'',	SchemeID, SchemeCode, SuggestedAmount, CollectionAmount,  CollectionIntervalTypeID, CollectionIntervalDesc, tickets, Duration, '',
	(case when Tickets ='' then 1 else Tickets end) * CollectionAmount, BookPickupDay, isnull(RouteName,''), isnull(ColAgt1,'')+', '+isnull(ColAgt2,''),  ProdTypeID, 0
	From VW_Active_Subscription_Details (nolock), @dys
	where (ProdTypeID = @ProductTypeID or @ProductTypeID = 0)
	and (CollectionAgentPersonID1 = @CollectionAgentID or CollectionAgentPersonID2 = @CollectionAgentID or @CollectionAgentID = 0)
	and (RouteID = @RouteID or @RouteID = 0)

	Insert @CollDtl1	
	select distinct ci.ProdTypId, ls.SubscriptionID , CONVERT(varchar, tcr.CollectionDateTime, 106),
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
	from ChitSubscription ls join @ChitInfoTbl ci on ls.SubscriptionID = ci.ChitSubsId  
	left outer join [dbo].[TransactionCollectionList] tcl on tcl.ProductID=ls.SubscriptionID and tcl.ProductTypeID = ci.ProdTypId
	left outer  join [dbo].[TransactionCollectionReciepts] tcr on tcr.CollectionListID = tcl.CollectionListID
	left outer join [dbo].PaymentTransferLog pymt on tcr.CollectionListID=pymt.CollectionListID 
	left outer join [dbo].[MasterType]  mt on tcl.TransactionModeTypeID = mt.TypeID
	left outer join [dbo].[MasterType]  mt1 on tcl.TransactionCategoryTypeID = mt1.TypeID
	inner join [dbo].ChitScheme ON ls.ChitSchemeID = dbo.ChitScheme.ChitSchemeID
	left join [dbo].MasterAmountCode ON [dbo].ChitScheme.AmountCodeID = [dbo].MasterAmountCode.AmountCodeID
	left outer join (TransactionBookVerification bv(nolock) join MasterType mt3(nolock) on bv.VerificationFindingTypeID = mt3.TypeID)
			on tcr.CollectionRecieptID = bv.ReferenceCollectionReceiptID
	where tcr.CollectionDateTime is not null and mt.Type is not null and tcl.StatusTypeID in (1035,1036) 
	and tcr.CollectionDateTime between @FrDt and @ToDt
	and ProductTypeID = 29
	and mt1.type = 'Subscription'

	insert into @CollDtl1
	select ci.ProdTypId, ls.LoanSubscriptionID , CONVERT(varchar, tcr.CollectionDateTime, 106),
		sum(CASE
				
				--WHEN mt1.Type='Principal Repayment' THEN (Case When AmountCodeConstant > 0 Then tcr.CollectedAmount/ AmountCodeConstant Else tcr.CollectedAmount End)--tcr.CollectedAmount
				WHEN mt1.Type='Principal Repayment' THEN tcr.CollectedAmount --tcr.CollectedAmount
				WHEN mt1.Type='Interest - Prepaid' THEN tcr.CollectedAmount
				--WHEN mt1.Type='Interest' THEN (Case When AmountCodeConstant > 0 Then tcr.CollectedAmount/ AmountCodeConstant Else tcr.CollectedAmount End)--tcr.CollectedAmount
				WHEN mt1.Type='Interest' THEN  tcr.CollectedAmount --tcr.CollectedAmount
				WHEN mt1.Type='Interest TDS' THEN tcr.CollectedAmount
				WHEN mt1.Type='Interest Waiver' THEN tcr.CollectedAmount
				WHEN mt1.Type='Principal Waiver' THEN tcr.CollectedAmount
				ELSE 0
		   END)
        AS Credit
	from LoanSubscription ls  (nolock) join @ChitInfoTbl  ci on ls.LoanSubscriptionID = ci.ChitSubsId 	
		left outer join [dbo].[TransactionCollectionList] tcl (nolock)  on tcl.ProductID=ls.LoanSubscriptionID and tcl.ProductTypeID = ci.ProdTypId
		left outer  join [dbo].[TransactionCollectionReciepts] tcr (nolock)  on tcr.CollectionListID = tcl.CollectionListID
		left outer join [dbo].PaymentTransferLog pymt (nolock)  on tcl.CollectionListID=pymt.CollectionListID --and substring(TargetProductCode ,1,1) in ('C','L','D')
		left outer join [dbo].[MasterType]  mt (nolock)  on tcl.TransactionModeTypeID = mt.TypeID
		left outer join [dbo].[MasterType]  mt1 (nolock) on tcl.TransactionCategoryTypeID = mt1.TypeID
		inner join [dbo].LoanScheme  (nolock) ON ls.LoanSchemeID = dbo.LoanScheme.LoanSchemeID
		left join [dbo].MasterAmountCode  (nolock) ON [dbo].LoanScheme.AmountCodeID = [dbo].MasterAmountCode.AmountCodeID
		left outer join (TransactionBookVerification bv(nolock) join MasterType mt3(nolock) on bv.VerificationFindingTypeID = mt3.TypeID)
		on tcr.CollectionRecieptID = bv.ReferenceCollectionReceiptID
	where tcr.CollectionDateTime is not null and mt.Type is not null and tcl.StatusTypeID in (1035,1036)
		and mt1.Type='Principal Repayment'
		AND tcl.ProductTypeID = 30
	and tcr.CollectionDateTime between @FrDt and @ToDt
	GROUP  BY ci.ProdTypId, ls.LoanSubscriptionID , CONVERT(varchar, tcr.CollectionDateTime, 106)

	update a
	set ColnActualAmt   = isnull(Credit,0)
	from @ChitInfoDtTbl a, @CollDtl1 b
	where a.ProdTypId = b.prodtypeid
	and a.ChitSubsId = b.subsid
	and a.NextColnDate = b.Date

	select ChitSubsCode, ClientName, NextColnDate 'CollectionDate', ColnAmt 'DueAmt',CollIntTypeDesc 'IntervalType',RouteName, CollectionAgent,
	ColnActualAmt, ColnAmt - ColnActualAmt 'Bal_To_Be_Collected', ChitValue
	from @ChitInfoDtTbl
	where NextColnDate between @FrDt and @ToDt
	order by routename,CollectionAgent
	
end