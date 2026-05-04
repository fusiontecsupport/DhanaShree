use dsfinfusion
go
-- exec pr_Get_Chit_Dividend_Schedule_n_Actuals_Details 23
-- exec pr_Get_Chit_Dividend_Schedule_n_Actuals_Details 6485
alter proc pr_Get_Chit_Dividend_Schedule_n_Actuals_Details
@ChitSubsID int,
@usrid varchar(100)=''
as
begin
	declare @ChitValue numeric(18,2), @ChitDuration int, @ChitDurationinmonths numeric(18,2),
	@ChitDurationtype int, @ChitDurationtypedesc varchar(100),  @orgChitValue numeric(18,2), @StartDate datetime, 
	@ChitDurationgap varchar(10), @ChitCode varchar(50), @chitschemeid int, @chitsubamt numeric(18,2), @actchitsubamt numeric(18,2),
	@chitcolinttypeid int, @term int, @colltypedesc varchar(100), @nooftickets int, @NTEMPTERM INT, @TEMPINSTNO INT, 
	@TEMPDIVAMT NUMERIC(18,2), @TMPSTDT DATETIME, @TEMPDISCAMT NUMERIC(18,2), @CHITENDDT DATETIME

	select @ChitValue = ChitValue ,  @ChitDuration = ChitDuration, @ChitDurationtype = DurationTypeId,
	 @StartDate = isnull(CommencementDate,StartDate), @ChitCode = ChitSubscriptionCode, @chitschemeid = cs.chitschemeid,
	 @chitsubamt = csm.SubscriptionAmount, @chitcolinttypeid = CollectionIntervalTypeID, @nooftickets = TicketNumber
	from ChitSubscription cs (nolock) join ChitScheme csm(nolock) on cs.chitschemeid = csm.chitschemeid
	where SubscriptionID = @ChitSubsID

	select @ChitDurationtypedesc = type from MasterType (nolock) where typeid = @ChitDurationtype	

	select @actchitsubamt = SuggestedAmount
	--select * 
	from ChitSchemeCollection (nolock)
	where chitschemeid = @chitschemeid
	and CollectionIntervalTypeID = @chitcolinttypeid
	
	select @colltypedesc = type from MasterType (nolock) where typeid = @chitcolinttypeid	

	select @ChitDurationinmonths = case when @ChitDurationtypedesc = 'Days' then @ChitDuration/30.0
								when @ChitDurationtypedesc = 'Weeks' then @ChitDuration*7/30.0
								when @ChitDurationtypedesc = 'Months' then @ChitDuration
								when @ChitDurationtypedesc = 'Years' then @ChitDuration/12 end
	
	select @term = case when @colltypedesc = 'Daily' then @ChitDuration*30.0
								when @colltypedesc = 'Weekly' then @ChitDuration*30.0/7
								when @colltypedesc = 'Monthly' then @ChitDuration
								when @colltypedesc = 'Querterly' then @ChitDuration*4
								when @colltypedesc = 'Yearly' then @ChitDuration*12 end
	

	set @orgChitValue = @ChitValue
	

	--select @orgChitValue 'ChitValue' , @ChitDuration 'ChitDuration', @ChitDurationtype 'DurationTypeId', @ChitDurationtypedesc 'ChitDurationtypedesc',
	--@ChitDurationinmonths 'ChitDurationinmonths', @actchitsubamt 'actchitsubamt', @chitsubamt 'chitsubamt', @colltypedesc 'colltypedesc',
	--@term

--	select cast(0 as int) SubsID, cast(0 as int) InstNo , cast (getdate() as datetime) InstDt ,
--		cast (0 as numeric(18,2)) Subs_Amount , cast(0.0 as  numeric(18,2)) Div_Amt, 
--		cast(0.0 as  numeric(18,2)) Disc_Amt, cast(0.0 as  numeric(18,2)) ToBePaid_Amt, 
--		cast(0 as numeric(18,2)) Prize_Amt, CONVERT(varchar, getdate(), 103) 'InsttDt',@orgChitValue 'ChitValue' ,  @ChitDuration 'ChitDuration', @ChitDurationtype 'DurationTypeId', 
--		@ChitDurationtypedesc 'ChitDurationtypedesc',@ChitDurationinmonths 'ChitDurationinmonths', 
--		@actchitsubamt 'actchitsubamt', @chitsubamt 'chitsubamt',cast(getdate() as datetime) collectiondt, 
--		cast(0 as numeric(18,2)) Credit, cast(0.0 as  numeric(18,2)) 'colbalance', 
--		cast('' as  varchar(100)) as CollectionSts , cast(0.0 as  numeric(18,2)) Dividend, 
--		cast('' as  varchar(100)) as Dividend_Sts
		
----	return
	declare @ChitCollectionPattern Table
	(
		SubsID int,		
		InstNo int,
		InstDt datetime,
		Subs_Amount numeric(18,2),		
		Div_Amt numeric(18,2),
		Disc_Amt numeric(18,2),
		ToBePaid_Amt numeric(18,2),
		Prize_Amt numeric(18,2)--,
		--CollectionDt datetime,
		--Collected_Amt numeric(18,2),
		--Collected_Cumulative_Amt numeric(18,2)
		
	)
	--select cast(0 as int) SubsID, cast(0 as int) InstNo , cast (getdate() as datetime) InstDt ,
	--	cast (0 as numeric(18,2)) Subs_Amount , cast(0.0 as  numeric(18,2)) Div_Amt, 
	--	cast(0.0 as  numeric(18,2)) Disc_Amt, cast(0.0 as  numeric(18,2)) ToBePaid_Amt, 
	--	cast(0 as numeric(18,2)) Prize_Amt, CONVERT(varchar, getdate(), 103) 'InsttDt',@orgChitValue 'ChitValue' ,  @ChitDuration 'ChitDuration', @ChitDurationtype 'DurationTypeId', 
	--	@ChitDurationtypedesc 'ChitDurationtypedesc',@ChitDurationinmonths 'ChitDurationinmonths', 
	--	@actchitsubamt 'actchitsubamt', @chitsubamt 'chitsubamt',cast(getdate() as datetime) collectiondt, 
	--	cast(0 as numeric(18,2)) Credit, cast(0.0 as  numeric(18,2)) 'colbalance', 
	--	cast('' as  varchar(100)) as CollectionSts , cast(0.0 as  numeric(18,2)) Dividend, 
	--	cast('' as  varchar(100)) as DividendSts
		
	--return

	set @StartDate = dateadd(m,-1,@StartDate)
	SET @CHITENDDT= dateadd(m,@ChitDurationinmonths,@StartDate)
	SET @NTEMPTERM = 1
	SET @TEMPINSTNO =1
	SET @TMPSTDT = @StartDate

	insert into @ChitCollectionPattern
	select @ChitSubsID,ChitInstalmentNo, @StartDate, @chitsubamt*@nooftickets,
	DividentAmount*@nooftickets, DiscountAmount*@nooftickets, 0, (@ChitValue-DiscountAmount)*@nooftickets
	from ChitSchemePattern (nolock)
	where chitschemeid = @chitschemeid

	declare @chitschemedtl table
	(subscriptioncode varchar(50),cschemeid int, stdt datetime)
	insert into @chitschemedtl 
	select distinct @ChitCode, @chitschemeid, @StartDate
	
	--select distinct ChitSubscriptionCode,  ChitSchemeID,  CommencementDate
	--from ChitSubscription where PersonID = 26

	declare @chitschemeduedivamtdtl table
	(csubscriptioncode varchar(50), --cschemeid int,
	InstNo int,
	stdt datetime,
	emidt datetime,
	 dueamt numeric(18,2),
	 divamt numeric(18,2),
	 DISCAMT NUMERIC(18,2),
	 balamt numeric(18,2)
	 )
	 declare @chitschemeduedivamtdtlfinal table
	(csubscriptioncode varchar(50), --cschemeid int,
	InstNo int,
	stdt datetime,
	emidt datetime,
	 dueamt numeric(18,2),
	 divamt numeric(18,2),
	 DISCAMT NUMERIC(18,2),
	 balamt numeric(18,2)
	 )
	 set @chitcolinttypeid = 775
--	 select * From ChitSchemeCollection
	 IF(@chitcolinttypeid =775)
	 BEGIN
		 insert into @chitschemeduedivamtdtl
		 select subscriptioncode , ROW_NUMBER() over (order by stdt) 'InstNo', --cs2.cschemeid,
		 stdt,dateadd(m,cs1.chitinstalmentno,stdt),
		 (cs3.SuggestedAmount*@nooftickets),(cs1.DividentAmount*@nooftickets)	,(cs1.DiscountAmount*@nooftickets)	, 
		 (cs3.SuggestedAmount*@nooftickets)-(cs1.DividentAmount*@nooftickets)	 
		 from ChitSchemePattern cs1 (nolock) 
		 join ChitSchemeCollection cs3 on cs1.ChitSchemeID = cs3.ChitSchemeID and cs3.CollectionIntervalTypeID = 775
		 join @chitschemedtl cs2 on cs1.ChitSchemeID = cs2.cschemeid
		 --where dateadd(m,cs1.chitinstalmentno,stdt) <= getdate()
		 --or (month(dateadd(m,cs1.chitinstalmentno,stdt) ) = month(getdate())
			--	and year(dateadd(m,cs1.chitinstalmentno,stdt) ) = year(getdate()))
		 --group by subscriptioncode, stdt, cs1.chitinstalmentno
	END

	--SELECT * FROM @chitschemeduedivamtdtl
	IF(@chitcolinttypeid =773)
	BEGIN
		--SELECT 'TEST', @TMPSTDT, @CHITENDDT
		WHILE (@TMPSTDT < @CHITENDDT)
		BEGIN
			
			IF (dateadd(m,@TEMPINSTNO,@STARTDATE) = @TMPSTDT)
				SELECT @TEMPINSTNO = @TEMPINSTNO + 1
			SELECT @TEMPINSTNO = ChitInstalmentNo, @TEMPDIVAMT = DividentAmount, @TEMPDISCAMT = DiscountAmount
			FROM CHITSCHEMEPATTERN (NOLOCK)
			WHERE CHITSCHEMEID = @chitschemeid
			AND ChitInstalmentNo = @TEMPINSTNO
			--SELECT dateadd(m,@TEMPINSTNO,@STARTDATE), @TMPSTDT, @TEMPINSTNO, @TEMPDIVAMT
			IF NOT EXISTS (SELECT '*' FROM @chitschemeduedivamtdtl WHERE emidt = @TMPSTDT)
			BEGIN
				insert into @chitschemeduedivamtdtl
				 select subscriptioncode , @TEMPINSTNO 'InstNo', --cs2.cschemeid,
				 stdt,@TMPSTDT,
				 (cs3.SuggestedAmount*@nooftickets),((@TEMPDIVAMT/30)*@nooftickets)	, (@TEMPDISCAMT/30 *@nooftickets)	, 
				 (cs3.SuggestedAmount*@nooftickets)-((@TEMPDIVAMT/30)*@nooftickets)	 
				 from ChitSchemeCollection cs3 (NOLOCK)
				 join @chitschemedtl cs2 on cs3.ChitSchemeID = cs2.cschemeid
				 AND cs3.CollectionIntervalTypeID = @chitcolinttypeid
			END
			SET @NTEMPTERM = @NTEMPTERM + 1 
			SET @TMPSTDT = DATEADD(D,1,@TMPSTDT)
			--SELECT @TMPSTDT 
		END
	END
	 
	 declare @instcnt int
	 set @instcnt =1
	 --SELECT * FROM @chitschemeduedivamtdtl
	IF(@chitcolinttypeid =774)
	BEGIN
		--SELECT 'TEST', @TMPSTDT, @CHITENDDT
		WHILE (@TMPSTDT < @CHITENDDT)
		BEGIN
			declare @mn1 int, @yr1 int, @mn2 int, @yr2 int, @mnyrmn1 int , @mnyrmn2 int
			--select dateadd(m,@TEMPINSTNO,@STARTDATE),@TMPSTDT
			select	@mn1=month(dateadd(m,@TEMPINSTNO+1,@STARTDATE)), @mn2= month(@TMPSTDT),
					@yr1=month(dateadd(m,@TEMPINSTNO+1,@STARTDATE)), @yr2= month(@TMPSTDT)
			select @mnyrmn1 = @yr1 + @mn1
			select @mnyrmn2 = @yr2 + @mn2			
			IF (dateadd(m,@TEMPINSTNO,@STARTDATE) < @TMPSTDT)
				SELECT @TEMPINSTNO = @TEMPINSTNO + 1
			SELECT @TEMPINSTNO = ChitInstalmentNo, @TEMPDIVAMT = DividentAmount, @TEMPDISCAMT = DiscountAmount
			FROM CHITSCHEMEPATTERN (NOLOCK)
			WHERE CHITSCHEMEID = @chitschemeid
			AND ChitInstalmentNo = @TEMPINSTNO
			--SELECT dateadd(m,@TEMPINSTNO,@STARTDATE), @TMPSTDT, @TEMPINSTNO, @TEMPDIVAMT
			IF NOT EXISTS (SELECT '*' FROM @chitschemeduedivamtdtl WHERE emidt = @TMPSTDT)
			BEGIN
				insert into @chitschemeduedivamtdtl
				 select subscriptioncode , @TEMPINSTNO 'InstNo', --cs2.cschemeid,
				 stdt,@TMPSTDT,
				 (cs3.SuggestedAmount*@nooftickets),((@TEMPDIVAMT/30*7)*@nooftickets)	, (@TEMPDISCAMT/30*7 *@nooftickets)	, 
				 (cs3.SuggestedAmount*@nooftickets)-((@TEMPDIVAMT/30*7)*@nooftickets)	 
				 from ChitSchemeCollection cs3 (NOLOCK)
				 join @chitschemedtl cs2 on cs3.ChitSchemeID = cs2.cschemeid
				 AND cs3.CollectionIntervalTypeID = @chitcolinttypeid
			END
			SET @NTEMPTERM = @NTEMPTERM + 7 
			SET @TMPSTDT = DATEADD(D,7,@TMPSTDT)
			set @instcnt = @instcnt + 1
			--SELECT @TMPSTDT 
		END
	END
	 
	 --where dateadd(m,cs1.chitinstalmentno,stdt) <= getdate()
	 --or (month(dateadd(m,cs1.chitinstalmentno,stdt) ) = month(getdate())
		--	and year(dateadd(m,cs1.chitinstalmentno,stdt) ) = year(getdate()))
	 --group by subscriptioncode, stdt, cs1.chitinstalmentno
	 
	
	insert into @chitschemeduedivamtdtlfinal
	 select csubscriptioncode,InstNo,stdt,emidt,
	 dueamt, divamt, DISCAMT,SUM(CAST(dueamt AS decimal(18,2)) - CAST(divamt AS decimal(18,2))) 
	OVER (ORDER BY emidt ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW)
		 AS Cumilative
	 from @chitschemeduedivamtdtl  order by 1,2,3

	--select * From @chitschemeduedivamtdtl
	--Select * From @chitschemeduedivamtdtlFINAL
	
	DELETE @ChitCollectionPattern
	insert into @ChitCollectionPattern
	select @ChitSubsID,InstNo, emidt, DUEAMT,
	DIVAMT, DISCAMT, 0, (@ChitValue-DISCAMT)*@nooftickets
	from @chitschemeduedivamtdtlfinal


	 update A
	set ToBePaid_Amt = b.balamt
	from @ChitCollectionPattern  A, @chitschemeduedivamtdtlfinal b
	where a.InstNo = b.InstNo
	AND INSTDT = B.EMIDT
		

	--SELECT * fROM @ChitCollectionPattern
	

	DECLARE @CollDtl1 TABLE 
        ( 
           [Date] Datetime, 
           Mode           VARCHAR(50), 
           ReceiptNo     VARCHAR(50),
           Category           VARCHAR(50), 
           Debit           decimal(18, 2),--VARCHAR(50), 
           Credit          decimal(18, 2),--VARCHAR(50), 		 
           Balance         decimal(18, 2),
		   Dividend        decimal(18, 2)--VARCHAR(50), 
        ) 
	

	--select a.*, CONVERT(varchar, a.InstDt, 103) 'InsttDt', @orgChitValue 'ChitValue' , @ChitDuration 'ChitDuration', @ChitDurationtype 'DurationTypeId', 
	--	@ChitDurationtypedesc 'ChitDurationtypedesc',@ChitDurationinmonths 'ChitDurationinmonths', 
	--	case when a.InstDt <= getdate() then (Subs_Amount-div_amt) else Subs_Amount end 'actchitsubamt', @chitsubamt 'chitsubamt', [date] collectiondt, Credit, 
	--	Balance 'colbalance', 'Over Due' CollectionSts
	--from @CollDtl1 b left join @ChitCollectionPattern a on month(a.instdt) = month([date]) and year(a.instdt) = year([date])
	--where instdt is null
	--ORDER BY instno,INSTDT, collectiondt
	--return -- EDMX FIX
	
	insert @CollDtl1
	select  CONVERT(varchar,tp.PaymentDateTime, 106),mt.Type,		
	CASE WHEN mt1.Type in('Prize', 'Agent Commission', 'Prize Transfer', 'Discount', 'Company', 'Foreman Commission', 'Dividend Reversal', 'Charges', 'Interest') then '' Else isnull(tcr.PrintRecepitCode,'') End,
	mt1.Type+ case when mt3.Type is null then '' else '['+mt3.Type + ' ' + cast (CollectionReceiptOldAmount-BookAmount as varchar(20))+']' end,
	--substring(tpl.remarks,1,200),
	CASE
		WHEN mt1.Type='Dividend' THEN tp.PaidAmount
		--WHEN mt1.Type='Interest' THEN tp.PaidAmount
		WHEN mt1.Type='Subscription' THEN (Case WHEN mt.Type in('Transfer') then (Case WHEN AmountCodeConstant > 0 THEN tp.PaidAmount / AmountCodeConstant Else tp.PaidAmount End) Else tp.PaidAmount End)
		WHEN mt1.Type='Waiver' THEN tp.PaidAmount
		ELSE 0
		END AS Debit,
	CASE WHEN mt1.Type='Prize' THEN (Case When AmountCodeConstant > 0 Then tp.PaidAmount Else tp.PaidAmount End) --tp.PaidAmount
		WHEN mt1.Type='Prize Transfer' THEN tp.PaidAmount
		WHEN mt1.Type= 'Agent Commission' THEN tp.PaidAmount	
		WHEN mt1.Type='Foreman Commission' THEN tp.PaidAmount
		WHEN mt1.Type='Discount' THEN tp.PaidAmount
		WHEN mt1.Type='Dividend Reversal' THEN tp.PaidAmount
		WHEN mt1.Type='Charges' THEN tp.PaidAmount
		WHEN mt1.Type='Interest' THEN tp.PaidAmount
		ELSE 0
		END AS Credit,
		0,
		CASE
		WHEN mt1.Type='Dividend' THEN tp.PaidAmount
		--WHEN mt1.Type='Interest' THEN tp.PaidAmount
		WHEN mt1.Type='Subscription' THEN (Case WHEN mt.Type in('Transfer') then (Case WHEN AmountCodeConstant > 0 THEN tp.PaidAmount / AmountCodeConstant Else tp.PaidAmount End) Else tp.PaidAmount End)
		WHEN mt1.Type='Waiver' THEN tp.PaidAmount
		ELSE 0
		END AS Dividend
	--'',tp.PaidAmount,
	from ChitSubscription ls (nolock)
	--left outer join [dbo].[TransactionCollectionList] tcl on tcl.ProductID = ls.SubscriptionID  -- where  CollectionListID=4676
	left outer join [dbo].[TransactionPaymentList] tpl (nolock) on tpl.ProductID = ls.SubscriptionID
	left outer join [dbo].[TransactionPayment] tp (nolock) on tp.PaymentListID =tpl.PaymentListID
	left outer join [dbo].[TransactionCollectionReciepts] tcr (nolock) on tcr.CollectionListID = tp.PaymentListID
	--left outer join [dbo].PaymentTransferLog pymt on tcr.CollectionListID=pymt.CollectionListID  and substring(TargetProductCode ,1,1) in ('C','L','D')
	left outer join [dbo].[MasterType]  mt (nolock) on tpl.TransactionModeTypeID = mt.TypeID
	left outer join [dbo].[MasterType]  mt1 (nolock) on tpl.TransactionCategoryTypeID = mt1.TypeID
	inner join [dbo].ChitScheme (nolock) ON ls.ChitSchemeID = dbo.ChitScheme.ChitSchemeID
	left join [dbo].MasterAmountCode (nolock) ON [dbo].ChitScheme.AmountCodeID = [dbo].MasterAmountCode.AmountCodeID
	left outer join (TransactionBookVerification bv(nolock) join MasterType mt3(nolock) on bv.VerificationFindingTypeID = mt3.TypeID)
			on tcr.CollectionRecieptID = bv.ReferenceCollectionReceiptID
	where SubscriptionID=@ChitSubsID and tp.PaymentDateTime is not null and mt.Type is not null and tpl.StatusTypeID in (1035,1036)
	AND ProductTypeTypeID = 29
	and mt1.Type='Dividend'


	--Insert @CollDtl1	
	--select distinct  CONVERT(varchar, tcr.CollectionDateTime, 106),mt.Type, 
	----isnull(tcr.PrintRecepitCode,'')+
	--(case when mt.Type ='Transfer' then 'Source A/c: '+ isnull(TargetProductCode,'') else'' end) +
	--CASE WHEN mt1.Type in('Prize', 'Agent Commission','Prize Transfer', 'Discount', 'Company', 'Foreman Commission', 'Dividend Reversal', 'Charges') then '' Else isnull(tcr.PrintRecepitCode,'') End,
	--mt1.Type+ case when mt3.Type is null then '' else '['+mt3.Type + ' ' + cast (CollectionReceiptOldAmount-BookAmount as varchar(20))+']' end,
	--CASE
	--	WHEN mt1.Type='Prize' THEN (Case When AmountCodeConstant > 0 Then tcr.CollectedAmount/ AmountCodeConstant Else tcr.CollectedAmount End)
	--	WHEN mt1.Type='Prize Transfer' THEN tcr.CollectedAmount
	--	WHEN mt1.Type= 'Agent Commission' THEN tcr.CollectedAmount 
	--	WHEN mt1.Type='Discount' THEN tcr.CollectedAmount
	--	WHEN mt1.Type='Company' THEN tcr.CollectedAmount
	--	WHEN mt1.Type='Foreman Commission' THEN tcr.CollectedAmount
	--	WHEN mt1.Type='Dividend Reversal' THEN tcr.CollectedAmount
	--	WHEN mt1.Type='Charges' THEN tcr.CollectedAmount
	--	WHEN mt1.Type='Interest' THEN tcr.CollectedAmount
	--	ELSE 0
	--	END
	--	AS Debit,
	--CASE
	--	WHEN mt1.Type='Dividend' THEN tcr.CollectedAmount
	--	--WHEN mt1.Type='Interest' THEN tcr.CollectedAmount
	--	--WHEN mt1.Type='Subscription' THEN tcr.CollectedAmount
	--	--WHEN mt1.Type='Subscription' THEN (Case WHEN mt.Type in('Transfer') then (Case WHEN AmountCodeConstant > 0 THEN tcr.CollectedAmount / AmountCodeConstant Else tcr.CollectedAmount End) Else tcr.CollectedAmount End)
	--	WHEN mt1.Type='Subscription' THEN (Case WHEN mt.Type in('Transfer') then (Case WHEN AmountCodeConstant > 0 THEN tcr.CollectedAmount  Else tcr.CollectedAmount End) Else tcr.CollectedAmount End)
	--	WHEN mt1.Type='Waiver' THEN tcr.CollectedAmount
	--	ELSE 0
	--	END
	--	AS Credit,
	--	0 as balance,0 
	--from ChitSubscription ls 
	--left outer join [dbo].[TransactionCollectionList] tcl on tcl.ProductID=ls.SubscriptionID
	--left outer  join [dbo].[TransactionCollectionReciepts] tcr on tcr.CollectionListID = tcl.CollectionListID
	--left outer join [dbo].PaymentTransferLog pymt on tcr.CollectionListID=pymt.CollectionListID and substring(TargetProductCode ,1,1) in ('C','L','D')
	--left outer join [dbo].[MasterType]  mt on tcl.TransactionModeTypeID = mt.TypeID
	--left outer join [dbo].[MasterType]  mt1 on tcl.TransactionCategoryTypeID = mt1.TypeID
	--inner join [dbo].ChitScheme ON ls.ChitSchemeID = dbo.ChitScheme.ChitSchemeID
	--left join [dbo].MasterAmountCode ON [dbo].ChitScheme.AmountCodeID = [dbo].MasterAmountCode.AmountCodeID
	--left outer join (TransactionBookVerification bv(nolock) join MasterType mt3(nolock) on bv.VerificationFindingTypeID = mt3.TypeID)
	--		on tcr.CollectionRecieptID = bv.ReferenceCollectionReceiptID
	--where ChitSubscriptionCode=@ChitCode and tcr.CollectionDateTime is not null and mt.Type is not null and tcl.StatusTypeID in (1035,1036) 
	--and ProductTypeID = 29

	
	select convert(varchar, date, 106) as Date,  --Mode, (Case When Mode = 'Transfer' and ReceiptNo not like 'Source%' then '' Else ReceiptNo End) as ReceiptNo,
	--Category, 
	Debit, Credit, 
	--SUM((CAST(credit AS int) - CAST(debit AS int))) OVER (ORDER BY date ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW)
	SUM((CAST(credit AS decimal(18,2)) - (Case When Category in('Charges',  'Dividend Reversal','Prize Transfer','Foreman Commission Delete','Prize111')
	then CAST(debit AS decimal(18,2)) Else 0 End))) 
	OVER (ORDER BY date ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW)
	--sum(cast(credit as decimal(18,2)))+ (case when category in('Interest') then CAST (debit AS decimal(18,2)) Else 0 End)OVER (ORDER BY date ROWS BETWEEN UNBOUNDED 
	--PRECEDING AND CURRENT ROW)
	--SUM((CAST(credit AS decimal(18,2)))) OVER (ORDER BY date ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW)
	AS Cumilative  INTO #BalCalcTMP
	from @CollDtl1 	
		order by CONVERT(DateTime, Date,101)

	update a
	set balance = Cumilative
	from @CollDtl1 a, #BalCalcTMP b
	where convert(varchar, a.[date], 106) = convert(varchar, b.[date], 106)

	drop table #BalCalcTMP

	select a.*, CONVERT(varchar,  isnull(a.InstDt,[date]), 103) 'InsttDt', @orgChitValue 'ChitValue' ,  @ChitDuration 'ChitDuration', @ChitDurationtype 'DurationTypeId', 
		@ChitDurationtypedesc 'ChitDurationtypedesc',@ChitDurationinmonths 'ChitDurationinmonths', 
		case when a.InstDt <= getdate() then (Subs_Amount-div_amt) else Subs_Amount end 'actchitsubamt', @chitsubamt 'chitsubamt', [date] collectiondt, isnull(Credit,0) 'Credit', 
		case when instno =1 then isnull(isnull(Balance,ToBePaid_Amt),0) else isnull(Balance,0) end 'colbalance',
		CollectionSts = case 
		when Dividend >0 then 'Dividend Paid'
		when ToBePaid_Amt = ToBePaid_Amt and Credit >0 and [date]<=InstDt then 'On Time'
		 when ToBePaid_Amt = ToBePaid_Amt and Credit >0 and [date]>InstDt then 'Over Due' 
		 when isnull(Credit,0) = 0 and InstDt<getdate() then 'Pending - Over Due'
		 else 'Pending' end,
		 isnull(Dividend,0) 'Dividend',
		 Dividend_Sts = case
		 when Div_Amt = 0 then 'No Dividend'
		 when  Debit >0 and  -- Debit = Dividend and Div_Amt = Dividend and
		 month([date]) = month(InstDt) and year([date])  = year(instdt) then 'Paid' Else 'Not Paid' End
	from @ChitCollectionPattern a left join @CollDtl1 b on 
	((month(a.instdt) = month([date]) and year(a.instdt) = year([date]) AND @chitcolinttypeid =775)
	OR (A.INSTDT = [DATE] AND @chitcolinttypeid in (773,774)))
	union 
	select a.*, CONVERT(varchar, isnull(a.InstDt,[date]), 103) 'InsttDt', @orgChitValue 'ChitValue' , @ChitDuration 'ChitDuration', @ChitDurationtype 'DurationTypeId', 
		@ChitDurationtypedesc 'ChitDurationtypedesc',@ChitDurationinmonths 'ChitDurationinmonths', 
		case when a.InstDt <= getdate() then (Subs_Amount-div_amt) else Subs_Amount end 'actchitsubamt', @chitsubamt 'chitsubamt', [date] collectiondt, Credit, 
		Balance 'colbalance', 'Over Due' CollectionSts, 0,''
	from @CollDtl1 b left join @ChitCollectionPattern a on 
	((month(a.instdt) = month([date]) and year(a.instdt) = year([date]) AND @chitcolinttypeid =775)
	OR (A.INSTDT = [DATE] AND @chitcolinttypeid in (773,774)))
	where instdt is null
	ORDER BY instno, a.instdt,collectiondt
end

