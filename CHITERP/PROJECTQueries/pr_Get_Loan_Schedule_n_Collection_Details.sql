-- exec pr_Get_Loan_Schedule_n_Collection_Details 11
alter proc pr_Get_Loan_Schedule_n_Collection_Details
@LoanSubsID int,
@usrid varchar(100)=''
as
begin
	declare @principal numeric(18,2), @interestrate numeric(18,2), @term int, @terminmonths numeric(18,2),
	@monthlyinterestrate numeric(18,2), @termtype int, @termtypedesc varchar(100), @interestratepermonth numeric(18,2),
	@colamt numeric(18,2),@colamtinmonth numeric(18,2), @dedtypid int, @dedtype varchar(100), @inst int, 
	@amttobepaid numeric(18,2), @orgprincipal numeric(18,2), @dispstdt datetime, @termgap varchar(10), @LoanCode varchar(50), 
	@loanschemeid int, @tenure int
	

	select @principal = principal , @interestrate = InterestRate/100, @term = Term, @termtype = TermTypeID,
	@dedtypid =DeductionTypeID, @dispstdt = DisbursementDate, @LoanCode = LoanSubscriptionCode, @loanschemeid= LoanSchemeID
	from LoanSubscription (nolock)
	where LoanSubscriptionID = @LoanSubsID

	
	select @tenure = IsNull(dbo.LoanScheme.TenureBased,0)
	from LoanScheme (nolock) where LoanSchemeID = @loanschemeid

	select @termtypedesc = type from MasterType (nolock) where typeid = @termtype	
	select @dedtype  = type from MasterType (nolock) where typeid = @dedtypid		
	set @dedtype = isnull(@dedtype,'Prepaid')

	select @terminmonths = case when @termtypedesc = 'Days' then @term/30.0
								when @termtypedesc = 'Weeks' then @term*7/30.0
								when @termtypedesc = 'Months' then @term
								when @termtypedesc = 'Years' then @term/12 end
	--select @termgap = 
	select @interestratepermonth = case when @termtypedesc = 'Days' then @interestrate*30.0
								when @termtypedesc = 'Weeks' then @interestrate*@term*7/30.0
								when @termtypedesc = 'Months' then @interestrate
								when @termtypedesc = 'Years' then @interestrate/12 end

	select @colamt= case when @dedtype = 'Prepaid' then @principal/ @term
	else 
	(@principal*@interestrate*power((1+@interestrate),@term))/((power((1+@interestrate),@term))-1) end,
	@colamtinmonth= case when @dedtype = 'Prepaid' then @principal/
	( case when @termtypedesc = 'Days' then @term*30.0
								when @termtypedesc = 'Weeks' then @term*7/30.0
								when @termtypedesc = 'Months' then @term
								when @termtypedesc = 'Years' then @term/12 end)
	else 
	(@principal*@interestratepermonth*power((1+@interestratepermonth),@terminmonths))/((power((1+@interestratepermonth),@terminmonths))-1)
	end
	set @orgprincipal = @principal

	--select @orgprincipal 'principal' , @interestrate 'InterestRate', @term 'Term', @termtype 'TermTypeID', @termtypedesc 'termtypedesc',
	--@terminmonths 'terminmonths', @interestratepermonth 'interestratepermonth', @dedtype 'dedtype', @colamtinmonth 'colamtinmonth'


	--select cast(0 as int) SubsID, cast(0 as int) InstNo , cast (getdate() as datetime) InstDt ,
	--	cast(0.0 as  numeric(18,2)) Loan_Balance, cast (0 as numeric(18,2)) EMI_Amount ,
	--	cast(0 as numeric(18,2)) Interest_Amt , cast(0 as numeric(18,2)) Principal_Amt ,
	--	cast(0 as numeric(18,2)) ToBePaid_Amt , CONVERT(varchar, getdate(), 103) 'InsttDt',
	--	@orgprincipal 'principal' , @interestrate 'InterestRate', @term 'Term', @termtype 'TermTypeID', 
	--	@termtypedesc 'termtypedesc',@terminmonths 'terminmonths', @interestratepermonth 'interestratepermonth', 
	--	@dedtype 'dedtype', @colamtinmonth 'colamtinmonth', cast(getdate() as datetime) collectiondt, 
	--	cast(0 as numeric(18,2)) Credit, cast(0.0 as  numeric(18,2)) 'colprincipal',
	--	cast(0.0 as  numeric(18,2))'colinterest', cast(0.0 as  numeric(18,2)) 'colbalance', 
	--	cast('' as  varchar(100)) as CollectionSts 
		
	--return
	declare @EMICalc Table
	(
		SubsID int,		
		InstNo int,
		InstDt datetime,
		Loan_Balance numeric(18,2),
		EMI_Amount numeric(18,2),
		Interest_Amt numeric(18,2),
		Principal_Amt numeric(18,2),
		ToBePaid_Amt numeric(18,2)--,
		--CollectionDt datetime,
		--Collected_Amt numeric(18,2),
		--Collected_Cumulative_Amt numeric(18,2)
		
	)
	set @inst=1
	if @dedtype ='Prepaid'
	begin
		while(@inst<= @term)
		begin
			select @amttobepaid = sum(EMI_Amount) from @EMICalc where instno<=@inst

			set @amttobepaid= isnull(@amttobepaid,0)+ @colamt
			set @dispstdt = case when @termtypedesc = 'Days' then dateadd(d,1,@dispstdt)
								when @termtypedesc = 'Weeks' then  dateadd(wk,1,@dispstdt)
								when @termtypedesc = 'Months' then  dateadd(mm,1,@dispstdt)
								when @termtypedesc = 'Years' then  dateadd(yy,1,@dispstdt) end
			insert into @EMICalc (SubsID, InstNo , InstDt, Loan_Balance , EMI_Amount , Interest_Amt , Principal_Amt , ToBePaid_Amt)
			select @LoanSubsID, @inst, @dispstdt, @principal, @colamt,0,
			@colamt,@amttobepaid

			set @principal = @principal-@colamt

			set @inst = @inst+1
		end
	end
	else
	begin
		while(@inst<= @terminmonths)
		begin
			select @amttobepaid = sum(EMI_Amount) from @EMICalc where instno<=@inst

			set @amttobepaid= isnull(@amttobepaid,0)+ @colamtinmonth
			set @dispstdt = case when @termtypedesc = 'Days' then dateadd(d,1,@dispstdt)
								when @termtypedesc = 'Weeks' then  dateadd(wk,1,@dispstdt)
								when @termtypedesc = 'Months' then  dateadd(mm,1,@dispstdt)
								when @termtypedesc = 'Years' then  dateadd(yy,1,@dispstdt) end
			insert into @EMICalc (SubsID, InstNo , InstDt, Loan_Balance , EMI_Amount , Interest_Amt , Principal_Amt , ToBePaid_Amt)
			select @LoanSubsID, @inst, @dispstdt, @principal, @colamtinmonth, @principal*@interestratepermonth,
			@colamtinmonth-@principal*@interestratepermonth,@amttobepaid

			set @principal = @principal-@colamtinmonth
			set @inst = @inst+1
		end
	end

	
	--DECLARE @LoanCollection TABLE 
 --       ( 
 --          [Date] Datetime, 
 --          Mode           VARCHAR(50), 
 --          ReceiptNo     VARCHAR(50),
 --          Category           VARCHAR(50), 
 --          Debit           decimal(18, 2),--VARCHAR(50), 
 --          Credit          decimal(18, 2),--VARCHAR(50), 
	--	   Principal	   decimal(18, 2),	
	--	   Interest		   decimal(18, 2),
 --          Balance         decimal(18, 2)
 --       ) 
		
	DECLARE @CollDtl1 TABLE 
        ( 
           trandt Datetime, 
           Mode           VARCHAR(50), 
           ReceiptNo     VARCHAR(50),
           Category           VARCHAR(50), 
           Debit           decimal(18, 2),--VARCHAR(50), 
           Credit          decimal(18, 2),--VARCHAR(50), 
		   Principal	   decimal(18, 2),	
		   Interest		   decimal(18, 2),
           Balance         decimal(18, 2)
        ) 

	
	Insert @CollDtl1
	select tcr.CollectionDateTime,mt.Type, --tcr.PrintRecepitCode,
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

			0.00
	from LoanSubscription ls 
	left outer join [dbo].[TransactionCollectionList] tcl on tcl.ProductID=ls.LoanSubscriptionID and tcl.ProductTypeID = 30
	left outer  join [dbo].[TransactionCollectionReciepts] tcr on tcr.CollectionListID = tcl.CollectionListID
	left outer join [dbo].PaymentTransferLog pymt on tcl.CollectionListID=pymt.CollectionListID and substring(TargetProductCode ,1,1) in ('C','L','D')
	left outer join [dbo].[MasterType]  mt on tcl.TransactionModeTypeID = mt.TypeID
	left outer join [dbo].[MasterType]  mt1 on tcl.TransactionCategoryTypeID = mt1.TypeID
	where LoanSubscriptionCode=@LoanCode and tcr.CollectionDateTime is not null and mt.Type is not null and tcl.StatusTypeID in (1035,1036)
	AND tcl.ProductTypeID = 30
	GROUP  BY tcr.CollectionDateTime, tcr.PrintRecepitCode,mt.Type,tcr.CollectedAmount,mt1.Type , TargetProductCode 
	order by CONVERT(varchar, tcr.CollectionDateTime, 103) asc

	
	select trandt,
		  -- Mode,
           --ReceiptNo,
           --Category, 
           Debit, 
           Credit, 
				--SUM((CAST(credit AS int) - CAST(debit AS int))) OVER (ORDER BY date ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW)
				SUM(Case @tenure When 1 then (Case When Category IN('Interest - Prepaid', 'Principal Transfer Delete','Principal Transfer') then (CAST(Debit AS decimal(18,2))) * 0 Else (Case When Category in('Charges') Then (CAST(Debit AS decimal(18,2))) * -1 
				Else (CAST(credit AS decimal(18,2))) 
				End) End) Else (CAST(credit AS decimal(18,2))) End) OVER (ORDER BY trandt ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW)
        AS Cumilative  INTO #BalCalcTMP
		from @CollDtl1 order by CONVERT(DateTime, trandt,101)

		
	update a
	set balance = Cumilative
	from @CollDtl1 a, #BalCalcTMP b
	where convert(varchar, a.trandt, 106) = convert(varchar, b.trandt, 106)

	drop table #BalCalcTMP
	
	select a.*, CONVERT(varchar, a.InstDt, 103) 'InsttDt', @orgprincipal 'principal' , @interestrate 'InterestRate', @term 'Term', @termtype 'TermTypeID', 
		@termtypedesc 'termtypedesc',@terminmonths 'terminmonths', @interestratepermonth 'interestratepermonth', 
		@dedtype 'dedtype', @colamtinmonth 'colamtinmonth', trandt collectiondt, Credit, b.Principal 'colprincipal',
		b.interest 'colinterest', case when instno =1 then isnull(Balance,Loan_Balance) else Balance end 'colbalance',
		CollectionSts = case when ToBePaid_Amt = isnull(Balance,Loan_Balance) and trandt<=InstDt then 'On Time'
		 when ToBePaid_Amt > isnull(Balance,Loan_Balance) and InstDt<getdate()  then 'Over Due' 
		 when ToBePaid_Amt <= isnull(Balance,Loan_Balance)  and isnull(Credit,0)>0  then 'Paid in Advance' 
		 when isnull(Credit,0) = 0 and InstDt<getdate() then 'Pending - Over Due'
		 else 'Pending' end
	from @EMICalc a  join @CollDtl1 b on a.instdt = trandt
	union 
	select a.*, CONVERT(varchar, a.InstDt, 103) 'InsttDt', @orgprincipal 'principal' , @interestrate 'InterestRate', @term 'Term', @termtype 'TermTypeID', 
		@termtypedesc 'termtypedesc',@terminmonths 'terminmonths', @interestratepermonth 'interestratepermonth', 
		@dedtype 'dedtype', @colamtinmonth 'colamtinmonth', trandt collectiondt, Credit, b.Principal 'colprincipal',
		b.interest 'colinterest', case when instno =1 then isnull(Balance,Loan_Balance) else Balance end 'colbalance',
		CollectionSts = case when ToBePaid_Amt = isnull(Balance,Loan_Balance) and trandt<=InstDt then 'On Time'
		 when ToBePaid_Amt > isnull(Balance,Loan_Balance) and InstDt<getdate()  then 'Over Due' 
		 when ToBePaid_Amt <= isnull(Balance,Loan_Balance)  and isnull(Credit,0)>0  then 'Paid in Advance' 
		 when isnull(Credit,0) = 0 and InstDt<getdate() then 'Pending - Over Due'
		 else 'Pending' end
	from @EMICalc a  left join @CollDtl1 b on a.instdt = trandt
	where trandt is null
	union 
	select a.*, CONVERT(varchar, a.InstDt, 103) 'InsttDt', @orgprincipal 'principal' , @interestrate 'InterestRate', @term 'Term', @termtype 'TermTypeID', 
		@termtypedesc 'termtypedesc',@terminmonths 'terminmonths', @interestratepermonth 'interestratepermonth', 
		@dedtype 'dedtype', @colamtinmonth 'colamtinmonth', trandt collectiondt, Credit, b.Principal 'colprincipal',
		b.interest 'colinterest', Balance 'colbalance',
		CollectionSts = case when ToBePaid_Amt = isnull(Balance,Loan_Balance) and trandt<=InstDt then 'On Time'
		 when ToBePaid_Amt > isnull(Balance,Loan_Balance)  and InstDt<getdate()   then 'Over Due' 
		 when ToBePaid_Amt <= isnull(Balance,Loan_Balance) and isnull(Credit,0)>0 then 'Paid in Advance' 
		 when isnull(Credit,0) = 0 and InstDt<getdate() then 'Pending - Over Due'
		 else 'Pending' end
	from @CollDtl1 b left join @EMICalc a on trandt = a.instdt 
	where instdt is null
	order by instdt, trandt
end


