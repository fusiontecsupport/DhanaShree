use DSFinFusion
go
-- select * from Tmp_Ledger_Balance_Detail where lcode like '%10m%'
-- select * from Tmp_Ledger_Balance_Detail where lcode like '%100%'
-- exec [dbo].[CustomerAccountLedgerbyPcode_Loan]   l2
-- exec [dbo].[CustomerAccountLedgerbyPcode_Loan]   'DSC/10M/00001'
-- exec [dbo].[CustomerAccountLedgerbyPcode_Loan]   'DSC/10M/00002'
alter PROCEDURE [dbo].[CustomerAccountLedgerbyPcode_Loan] 
@Code varchar(50)
AS 
BEGIN 

Delete From Tmp_Ledger_Balance_Detail Where LCode = @Code

DECLARE @TableMaster TABLE 
        ( 
           [Date] Datetime, 
           Mode           VARCHAR(50), 
           ReceiptNo     VARCHAR(50),
           Category           VARCHAR(100), 
		   Remarks		varchar(200),
           Debit           decimal(18, 2),--VARCHAR(50), 
           Credit          decimal(18, 2),--VARCHAR(50), 
           Balance         decimal(18, 2)
        ) 

--Non Tenure

DECLARE @TableMaster_02 TABLE 
        ( 
           [Date] Datetime, 
           Mode           VARCHAR(50), 
           ReceiptNo     VARCHAR(50),
           Category           VARCHAR(50), 
		   Remarks		varchar(200),
           Debit           decimal(18, 2),--VARCHAR(50), 
           Credit          decimal(18, 2),--VARCHAR(50), 
		   Principal	   decimal(18, 2),	
		   Interest		   decimal(18, 2),
           Balance         decimal(18, 2)
        ) 

--Non Tenure

DECLARE @TableMaster_05 TABLE 
        ( 
           [Date] Datetime, 
           Mode           VARCHAR(50), 
           ReceiptNo     VARCHAR(50),
           Category           VARCHAR(50), 
		   Remarks		varchar(200),
           Debit           decimal(18, 2),--VARCHAR(50), 
           Credit          decimal(18, 2),--VARCHAR(50), 
		   Principal	   decimal(18, 2),	
		   Interest		   decimal(18, 2),
           Balance         decimal(18, 2)
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


Declare @TicketNumber numeric(18,2)
Declare @Loanamount numeric(18,2)
Declare @Amt numeric(18,2)
Declare @BalAmt numeric(18,2)
Declare @SDesc varchar(50)
Declare @tenure int


Declare @clientname varchar(250)
Declare @address varchar(1000)
Declare @loanscheme varchar(250)
Declare @BookVerifiedDate varchar(50)

select 
@clientname = cl.CLIENTNAME,
@address = isnull(CLIENTADDRESS1,'') +',
'+isnull(CLIENTADDRESS2,'')+',
'+isnull(CLIENTCITY,'') +' - ' + isnull(CLIENTPINCD,'')+'
Contact: '+ isnull(CLIENTMOBILENO1,'') +', ' + isnull(CLIENTEMAILID,'')
from LoanRequest lr(nolock)
		join clientmaster cl(nolock) on lr.PersonID = cl.clientid		
		join LoanSubscription ls (nolock) on lr.LoanRequestID = ls.LoanRequestID
where ls.LoanSubscriptionCode = @Code

select @BookVerifiedDate =  isnull(CONVERT(varchar,(max(bvl.VerifiedDate)), 106),'')
from LoanRequest lr(nolock)
		join clientmaster cl(nolock) on lr.PersonID = cl.clientid
		join LoanSubscription ls (nolock) on lr.LoanRequestID = ls.LoanRequestID
		join LoanScheme lsc(nolock) on ls.LoanSchemeID = lsc.LoanSchemeID
		left join CommonAccountBook ca(nolock) on ls.LoanSubscriptionID = ca.OwnerID
		left join MasterType mt(nolock) on ca.OwnerTypeID = mt.TypeID
		left join CompanyEmployeeProductMap tpm (nolock) on ls.LoanSubscriptionID = tpm.SchemeID
		left join Organization org (nolock) on ls.AssociatedOrganizationID = org.OrganizationID
		left join TransactionBookVerificationList bvl(nolock) on  ca.AccountBookID = bvl.BookID
	where ca.BookTerminatedDate is null
	and lr.StatusTypeID = 968
	and ca.StatusTypeID = 836
	and ca.OwnerTypeID = 187
	and ls.LoanSubscriptionCode=@Code


Insert into @StatusMaster(SDesc, Amount)
select 'Last Book Verified', @BookVerifiedDate


--Chit Status / Chit Value / Chit Ticket No.

	 --select @SDesc =  Type, @ChitValue = CAST((ChitScheme.ChitValue) AS decimal(18,2)), @TicketNumber = TicketNumber from dbo.chitsubscription 
		--	 inner join dbo.ChitGroup cg on cg.ChitGroupID = chitsubscription.ChitGroupID
  --           INNER JOIN dbo.person ON dbo.chitsubscription.personid = dbo.person.personid 
  --           INNER JOIN dbo.chitscheme ON dbo.chitsubscription.chitschemeid = dbo.chitscheme.chitschemeid 
		--	 INNER JOIN dbo.MasterType ON dbo.chitsubscription.StatusTypeID = dbo.MasterType.TypeID 
		--	 where chitsubscription.ChitSubscriptionCode=@Code

	SELECT       @Loanamount =  dbo.LoanSubscription.LoanAmount
	FROM            dbo.LoanSubscription (nolock) INNER JOIN
							 dbo.MasterType (nolock) ON dbo.LoanSubscription.TermTypeID = dbo.MasterType.TypeID INNER JOIN
							 dbo.LoanScheme (nolock) ON dbo.LoanSubscription.LoanSchemeID = dbo.LoanScheme.LoanSchemeID INNER JOIN
							 dbo.LoanRequest (nolock) ON dbo.LoanSubscription.LoanRequestID = dbo.LoanRequest.LoanRequestID INNER JOIN
							 dbo.MasterType (nolock) AS MasterType_1 ON dbo.LoanRequest.DeductionTypeId = MasterType_1.TypeID INNER JOIN
							 dbo.MasterType (nolock) AS MasterType_2 ON dbo.LoanSubscription.StatusTypeID = MasterType_2.TypeID LEFT OUTER JOIN
                         dbo.Organization (nolock) ON dbo.LoanRequest.OrganizationID = dbo.Organization.OrganizationID LEFT OUTER JOIN
                         dbo.ClientMaster cl(nolock) ON dbo.LoanRequest.PersonID = cl.clientid

	WHERE        (dbo.LoanSubscription.LoanSubscriptionCode = @Code)

--Set @Amt = 0--ISNULL(@Amt, 0)
--Set @BalAmt = 0
--Set @TicketNumber = ISNULL(@TicketNumber, 0)
Set @Loanamount = ISNULL(@Loanamount, 0)
--Select @Loanamount
-- Loan for Tenure Receipt

--tenure
	SELECT  @tenure = IsNull(dbo.LoanScheme.TenureBased,0)
	FROM            dbo.LoanSubscription (nolock) INNER JOIN
							 dbo.MasterType  (nolock) ON dbo.LoanSubscription.TermTypeID = dbo.MasterType.TypeID INNER JOIN
							 dbo.LoanScheme (nolock)  ON dbo.LoanSubscription.LoanSchemeID = dbo.LoanScheme.LoanSchemeID INNER JOIN
							 dbo.LoanRequest (nolock)  ON dbo.LoanSubscription.LoanRequestID = dbo.LoanRequest.LoanRequestID INNER JOIN
							 dbo.MasterType (nolock)  AS MasterType_1 ON dbo.LoanRequest.DeductionTypeId = MasterType_1.TypeID INNER JOIN
							 dbo.MasterType  (nolock) AS MasterType_2 ON dbo.LoanSubscription.StatusTypeID = MasterType_2.TypeID
	WHERE        (dbo.LoanSubscription.LoanSubscriptionCode = @Code)

	Set @tenure = ISNULL(@tenure, 0)

	--Select  @tenure
--tenure


Insert into @TableMaster 
select CONVERT(varchar, tcr.CollectionDateTime, 106),mt.Type, --tcr.PrintRecepitCode,
 isnull(tcr.PrintRecepitCode,'')  
  + (case when mt.Type ='Transfer' then 'Source A/c: '+ TargetProductCode else'' end) ,
mt1.Type,
substring(tcl.remarks,1,200),
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
		0.00
from LoanSubscription ls  (nolock) 
left outer join [dbo].[TransactionCollectionList] tcl (nolock)  on tcl.ProductID=ls.LoanSubscriptionID
left outer  join [dbo].[TransactionCollectionReciepts] tcr (nolock)  on tcr.CollectionListID = tcl.CollectionListID
left outer join [dbo].PaymentTransferLog pymt (nolock)  on tcl.CollectionListID=pymt.CollectionListID --and substring(TargetProductCode ,1,1) in ('C','L','D')
left outer join [dbo].[MasterType]  mt (nolock)  on tcl.TransactionModeTypeID = mt.TypeID
left outer join [dbo].[MasterType]  mt1 (nolock) on tcl.TransactionCategoryTypeID = mt1.TypeID
inner join [dbo].LoanScheme  (nolock) ON ls.LoanSchemeID = dbo.LoanScheme.LoanSchemeID
left join [dbo].MasterAmountCode  (nolock) ON [dbo].LoanScheme.AmountCodeID = [dbo].MasterAmountCode.AmountCodeID
--left outer join (TransactionBookVerification bv(nolock) join MasterType mt3(nolock) on bv.VerificationFindingTypeID = mt3.TypeID)
		--on tcr.CollectionRecieptID = bv.ReferenceCollectionReceiptID
where LoanSubscriptionCode=@Code and tcr.CollectionDateTime is not null and mt.Type is not null and tcl.StatusTypeID in (1035,1036)
AND tcl.ProductTypeID = 30
GROUP  BY tcr.CollectionDateTime, tcr.PrintRecepitCode,mt.Type,tcr.CollectedAmount,mt1.Type,  --mt3.Type ,
tcr.CollectedAmount ,dbo.MasterAmountCode.AmountCodeConstant, TargetProductCode, substring(tcl.remarks,1,200)
order by CONVERT(varchar, tcr.CollectionDateTime, 106) asc


Insert into @TableMaster 
select CONVERT(varchar, bv.BookEntryDate, 106),mt.Type, --tcr.PrintRecepitCode,
 isnull(tcr.PrintRecepitCode,'')  
  + (case when mt.Type ='Transfer' then 'Source A/c: '+ TargetProductCode else'' end) ,
mt1.Type+ case when mt3.Type is null then '' else '['+mt3.Type + ' ' + cast (CollectionReceiptOldAmount-BookAmount as varchar(20))+']' end,
substring(tcl.remarks,1,200),
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
		0.00
from LoanSubscription ls  (nolock) 
left outer join [dbo].[TransactionCollectionList] tcl (nolock)  on tcl.ProductID=ls.LoanSubscriptionID
left outer  join [dbo].[TransactionCollectionReciepts] tcr (nolock)  on tcr.CollectionListID = tcl.CollectionListID
left outer join [dbo].PaymentTransferLog pymt (nolock)  on tcl.CollectionListID=pymt.CollectionListID --and substring(TargetProductCode ,1,1) in ('C','L','D')
left outer join [dbo].[MasterType]  mt (nolock)  on tcl.TransactionModeTypeID = mt.TypeID
left outer join [dbo].[MasterType]  mt1 (nolock) on tcl.TransactionCategoryTypeID = mt1.TypeID
inner join [dbo].LoanScheme  (nolock) ON ls.LoanSchemeID = dbo.LoanScheme.LoanSchemeID
left join [dbo].MasterAmountCode  (nolock) ON [dbo].LoanScheme.AmountCodeID = [dbo].MasterAmountCode.AmountCodeID
join (TransactionBookVerification bv(nolock) join MasterType mt3(nolock) on bv.VerificationFindingTypeID = mt3.TypeID)
		on tcr.CollectionRecieptID = bv.ReferenceCollectionReceiptID
where LoanSubscriptionCode=@Code and tcr.CollectionDateTime is not null and mt.Type is not null and tcl.StatusTypeID in (1035,1036)
AND tcl.ProductTypeID = 30
GROUP  BY bv.BookEntryDate, tcr.PrintRecepitCode,mt.Type,tcr.CollectedAmount,mt1.Type,  mt3.Type ,
CollectionReceiptOldAmount-BookAmount,dbo.MasterAmountCode.AmountCodeConstant,
mt3.Type,CollectionReceiptOldAmount-BookAmount , TargetProductCode, substring(tcl.remarks,1,200)
order by CONVERT(varchar, bv.BookEntryDate, 106) asc

--select * from @TableMaster
-- Loan for Tenure Payment

Insert into @TableMaster 
select CONVERT(varchar,tp.PaymentDateTime, 106),mt.Type,'',
--(case when mt.Type ='Transfer' then 'Source A/c: '+ TargetProductCode else'' end) ,
mt1.Type+ case when mt3.Type is null then '' else '['+mt3.Type + ' ' + cast (CollectionReceiptOldAmount-BookAmount as varchar(20))+']' end,
substring(tpl.remarks,1,200),
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
		0.00
from LoanSubscription ls
--left outer join [dbo].[TransactionCollectionList] tcl on tcl.ProductID = ls.LoanSubscriptionID and ProductTypeID = 30 -- where  CollectionListID=4676
--left outer join [dbo].PaymentTransferLog pymt on tcl.CollectionListID=pymt.CollectionListID
left outer join [dbo].[TransactionPaymentList] tpl on tpl.ProductID = ls.LoanSubscriptionID
left outer join [dbo].[TransactionPayment] tp on tp.PaymentListID =tpl.PaymentListID
--left outer join [dbo].[TransactionCollectionReciepts] tcr on tcr.CollectionListID = tp.PaymentListID
left outer join [dbo].[MasterType]  mt on tpl.TransactionModeTypeID = mt.TypeID
left outer join [dbo].[MasterType]  mt1 on tpl.TransactionCategoryTypeID = mt1.TypeID
inner join [dbo].LoanScheme ON ls.LoanSchemeID = dbo.LoanScheme.LoanSchemeID
left join [dbo].MasterAmountCode ON [dbo].LoanScheme.AmountCodeID = [dbo].MasterAmountCode.AmountCodeID
left outer join (TransactionBookVerification bv(nolock) join MasterType mt3(nolock) on bv.VerificationFindingTypeID = mt3.TypeID)
		on tp.PaymentListID = bv.ReferenceCollectionReceiptID
where LoanSubscriptionCode=@Code and tp.PaymentDateTime is not null and mt.Type is not null and tp.StatusTypeID in (1035,1036)
AND tpl.ProductTypeTypeID = 30
--GROUP  BY tp.PaymentDateTime,tcr.PrintRecepitCode,mt.Type,tp.PaidAmount,mt1.Type
order by CONVERT(varchar,tp.PaymentDateTime, 106) asc



-- Loan for Non Tenure Receipt

Insert @TableMaster_02
select CONVERT(varchar, tcr.CollectionDateTime, 106),mt.Type, --tcr.PrintRecepitCode,
isnull(tcr.PrintRecepitCode,'')+ (case when mt.Type ='Transfer' then 'Source A/c: '+ TargetProductCode else'' end) ,
CASE WHEN mt1.Type='Interest' THEN 'Principal Repayment' ELSE mt1.Type END Type1,
substring(tcl.remarks,1,200),
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
left outer join [dbo].PaymentTransferLog pymt on tcl.CollectionListID=pymt.CollectionListID --and substring(TargetProductCode ,1,1) in ('C','L','D')
left outer join [dbo].[MasterType]  mt on tcl.TransactionModeTypeID = mt.TypeID
left outer join [dbo].[MasterType]  mt1 on tcl.TransactionCategoryTypeID = mt1.TypeID
where LoanSubscriptionCode=@Code and tcr.CollectionDateTime is not null and mt.Type is not null and tcl.StatusTypeID in (1035,1036)
AND tcl.ProductTypeID = 30
GROUP  BY tcr.CollectionDateTime, tcr.PrintRecepitCode,mt.Type,tcr.CollectedAmount,mt1.Type , TargetProductCode ,
substring(tcl.remarks,1,200)
order by CONVERT(varchar, tcr.CollectionDateTime, 106) asc

Insert into @TableMaster_05
Select [Date] ,Mode, ReceiptNo, Category, Remarks, Sum(Debit) Debit, Sum(Credit) as Credit, Sum(Principal) as Principal, 
Sum(Interest) as Interest, Balance From @TableMaster_02
Group by [Date] ,Mode, ReceiptNo, Category, Balance, Remarks

-- Loan for Non Tenure Payment

Insert into @TableMaster_05
select CONVERT(varchar,tp.PaymentDateTime, 106),mt.Type,'',
--(case when mt.Type ='Transfer' then 'Source A/c: '+ TargetProductCode else'' end),
mt1.Type,substring(tpl.remarks,1,200),
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
		0.00
from LoanSubscription ls
--left outer join [dbo].[TransactionCollectionList] tcl on tcl.ProductID = ls.LoanSubscriptionID  -- where  CollectionListID=4676
left outer join [dbo].[TransactionPaymentList] tpl on tpl.ProductID = ls.LoanSubscriptionID
left outer join [dbo].[TransactionPayment] tp on tp.PaymentListID =tpl.PaymentListID
--left outer join [dbo].PaymentTransferLog pymt on tpl.PaymentListID=pymt.PaymentListID
--left outer join [dbo].[TransactionCollectionReciepts] tcr on tcr.CollectionListID = tp.PaymentListID
left outer join [dbo].[MasterType]  mt on tpl.TransactionModeTypeID = mt.TypeID
left outer join [dbo].[MasterType]  mt1 on tpl.TransactionCategoryTypeID = mt1.TypeID
where LoanSubscriptionCode=@Code and tp.PaymentDateTime is not null and mt.Type is not null and tp.StatusTypeID in (1035,1036)
AND tpl.ProductTypeTypeID = 30
--GROUP  BY tp.PaymentDateTime,tcr.PrintRecepitCode,mt.Type,tp.PaidAmount,mt1.Type
order by CONVERT(varchar,tp.PaymentDateTime, 106) asc


--Loan For Tenure

select convert(varchar, date, 106) as Date, 
		   Mode,
           ReceiptNo,
           Category,
		   Remarks,
           Debit, 
           Credit, 
				--SUM((CAST(credit AS int) - CAST(debit AS int))) OVER (ORDER BY date ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW)
				SUM(Case @tenure When 1 then (Case When Category IN('Interest - Prepaid', 'Principal Transfer Delete','Principal Transfer') then (CAST(Debit AS decimal(18,2))) * 0 Else (Case When Category in('Charges') Then (CAST(Debit AS decimal(18,2))) * -1 
				Else (CAST(credit AS decimal(18,2))) 
				End) End) Else (CAST(credit AS decimal(18,2))) End) OVER (ORDER BY date ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW)
        AS Cumilative  INTO #TMP
		from @TableMaster order by CONVERT(DateTime, Date,101)


SELECT * FROM #TMP

--SELECT '','','','Total', Sum(CAST (Debit AS decimal(18,2))) AS Total ,SUM(CAST (Credit AS decimal(18,2))) AS [value], SUM(CAST (Credit AS decimal(18,2))) AS [BalanceTotal]
--            FROM  #TMP

SELECT '','','','','Total', Sum(CAST (Debit AS decimal(18,2))) AS Total ,SUM(CAST (Credit AS decimal(18,2))) AS [value], SUM(Case @tenure When 1 then (Case When Category IN('Interest - Prepaid','Principal Transfer') then 0 Else 
(Case When Category in('Principal Transfer dELETE', 'Charges') Then (CAST(credit AS decimal(18,2))) Else 0 End) End) Else (CAST(credit AS decimal(18,2))) End) AS [BalanceTotal]
            FROM  #TMP

--Loan For Non Tenure

select convert(varchar, date, 106) as Date, 
		   Mode,
           ReceiptNo,
           Category,
		   Remarks,
           Debit, 
           Credit, Principal,Interest,
				--SUM((CAST(credit AS int) - CAST(debit AS int))) OVER (ORDER BY date ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW)
				SUM((CAST(Debit AS decimal(18,2))) - (CAST(Principal AS decimal(18,2))) + (Case When Category IN('Interest - Prepaid', 'Principal Transfer') then (CAST(Credit AS decimal(18,2)))  Else 0 End) ) 
				OVER (ORDER BY date ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW)
        AS [P Balance]  INTO #TMP05
		from @TableMaster_05 order by CONVERT(DateTime, Date,101)


SELECT * FROM #TMP05

SELECT '','','','','Total', Sum(CAST (Debit AS decimal(18,2))) AS Total ,SUM(CAST (Credit AS decimal(18,2))) AS [value], SUM(CAST (Principal AS decimal(18,2))) AS [Principal], SUM(CAST (Interest AS decimal(18,2))) AS [Interest], 
SUM(CAST (Credit AS decimal(18,2))) AS [BalanceTotal]
            FROM  #TMP05
-- exec [dbo].[CustomerAccountLedgerbyPcode]   C16735



--SELECT '','','','Total', Sum(CAST (Debit AS int)) AS Total ,SUM(CAST (Credit AS int)) AS [value], SUM(CAST (Credit AS INT)) - SUM(CAST (Debit AS INT)) AS [BalanceTotal]
--            FROM  #TMP



IF EXISTS (SELECT 1 FROM LoanSubscription WHERE LoanSubscriptionCode=@Code)
BEGIN
	--select LoanAmount as [value],TallyCode from LoanSubscription where LoanSubscriptionCode=@Code
	SELECT        dbo.LoanSubscription.LoanSubscriptionCode, dbo.LoanSubscription.TallyCode, dbo.LoanSubscription.LoanAmount, dbo.LoanSubscription.InterestRate, dbo.LoanScheme.SchemeName, IsNull(dbo.LoanScheme.TenureBased,0) as TenureBased, 
							 dbo.LoanRequest.Term, dbo.MasterType.Type AS TermDesc, MasterType_1.Type AS DeductType, MasterType_2.Type AS StatusType,  @clientname 'ClientName',
							 SchemeName, @address 'ClientAddress'
	FROM            dbo.LoanSubscription INNER JOIN
							 dbo.MasterType ON dbo.LoanSubscription.TermTypeID = dbo.MasterType.TypeID INNER JOIN
							 dbo.LoanScheme ON dbo.LoanSubscription.LoanSchemeID = dbo.LoanScheme.LoanSchemeID INNER JOIN
							 dbo.LoanRequest ON dbo.LoanSubscription.LoanRequestID = dbo.LoanRequest.LoanRequestID INNER JOIN
							 dbo.MasterType AS MasterType_1 ON dbo.LoanRequest.DeductionTypeId = MasterType_1.TypeID INNER JOIN
							 dbo.MasterType AS MasterType_2 ON dbo.LoanSubscription.StatusTypeID = MasterType_2.TypeID LEFT OUTER JOIN
                         --dbo.Organization ON dbo.LoanRequest.OrganizationID = dbo.Organization.OrganizationID LEFT OUTER JOIN
                         dbo.clientmaster ON dbo.LoanRequest.PersonID = dbo.clientmaster.clientid

	WHERE        (dbo.LoanSubscription.LoanSubscriptionCode = @Code)

end



--select Category, SUM((CAST(credit AS decimal(18,2)) + CAST(debit AS decimal(18,2)))) as Totals from #TMP GROUP  BY Category

select case when category like '%Paid (%' 
--and @Code like 'l%' 
then 'Principal Repayment'
--when category like '%Paid (%' and @Code like 'D%' then 'Collection'
--when category like '%Paid (%' and @Code like 'C%' then 'Subscription'
else category end as Category, SUM((CAST(credit AS decimal(18,2)) + CAST(debit AS decimal(18,2)))) as Totals 
from #TMP 
GROUP  BY case when category like '%Paid (%' 
--and @Code like 'l%' 
then 'Principal Repayment'
--when category like '%Paid (%' and @Code like 'D%' then 'Collection'
--when category like '%Paid (%' and @Code like 'C%' then 'Subscription'
else category end


Select 
@BalAmt =  Sum(Case When category like '%Paid (%' or Category in('Principal Disbursement', 'Interest - Prepaid', 'Principal Transfer') then (CAST(credit AS decimal(18,2)) + CAST(debit AS decimal(18,2))) Else 0 End) -
Sum(Case When Category in('Principal Repayment', 'Principal Waiver','Interest TDS') then (CAST(credit AS decimal(18,2)) + CAST(debit AS decimal(18,2))) Else 0 End)
From #TMP 
--Group by Category

--Summary
/*
IF EXISTS (SELECT 1 FROM chitsubscription WHERE ChitSubscriptionCode=@Code)
BEGIN
	 Insert into @SummaryMaster(SDesc,Amount)
	 Values('Chit Value', @ChitValue)
	 --select 'Chit Value', CAST(ChitScheme.ChitValue as decimal(18,2)) as ChitValue from dbo.chitsubscription 
	 --		 inner join dbo.ChitGroup cg on cg.ChitGroupID = chitsubscription.ChitGroupID
     --        INNER JOIN dbo.person ON dbo.chitsubscription.personid = dbo.person.personid 
     --        INNER JOIN dbo.chitscheme ON dbo.chitsubscription.chitschemeid = 
     --                   dbo.chitscheme.chitschemeid where chitsubscription.ChitSubscriptionCode=@Code

	 Insert into @SummaryMaster(SDesc,Amount)
	 Values('Tickets', @TicketNumber)
	 --select 'Tickets', CAST(TicketNumber AS decimal(18,2)) AS TicketNumber from dbo.chitsubscription 
		--	 inner join dbo.ChitGroup cg on cg.ChitGroupID = chitsubscription.ChitGroupID
  --           INNER JOIN dbo.person ON dbo.chitsubscription.personid = dbo.person.personid 
  --           INNER JOIN dbo.chitscheme ON dbo.chitsubscription.chitschemeid = 
  --                      dbo.chitscheme.chitschemeid where chitsubscription.ChitSubscriptionCode=@Code

	 Insert into @SummaryMaster(SDesc,Amount)
	 Values('Total Value', @TicketNumber * @ChitValue)
	 --select 'Total Value',CAST((TicketNumber * ChitScheme.ChitValue) AS decimal(18,2)) from dbo.chitsubscription 
		--	 inner join dbo.ChitGroup cg on cg.ChitGroupID = chitsubscription.ChitGroupID
  --           INNER JOIN dbo.person ON dbo.chitsubscription.personid = dbo.person.personid 
  --           INNER JOIN dbo.chitscheme ON dbo.chitsubscription.chitschemeid = 
  --                      dbo.chitscheme.chitschemeid where chitsubscription.ChitSubscriptionCode=@Code

	 Insert into @SummaryMaster(SDesc,Amount)
	 select 'Tickets Paid', 0 from dbo.chitsubscription 
			 inner join dbo.ChitGroup cg on cg.ChitGroupID = chitsubscription.ChitGroupID
             INNER JOIN dbo.person ON dbo.chitsubscription.personid = dbo.person.personid 
             INNER JOIN dbo.chitscheme ON dbo.chitsubscription.chitschemeid = 
                        dbo.chitscheme.chitschemeid where chitsubscription.ChitSubscriptionCode=@Code


		--select  @Amt = 
		--		SUM(CASE
		--				WHEN mt1.Type='Dividend' THEN tcr.CollectedAmount
		--				WHEN mt1.Type='Subscription' THEN tcr.CollectedAmount
		--				WHEN mt1.Type='Foreman Commission' THEN tcr.CollectedAmount
		--				WHEN mt1.Type='Waiver' THEN tcr.CollectedAmount
		--				WHEN mt1.Type='Charges' THEN tcr.CollectedAmount
		--				WHEN mt1.Type='Dividend Reversal' THEN tcr.CollectedAmount
		--				ELSE 0
		--		   END)
				
		----tcr.CollectedAmount,'',
		--from ChitSubscription ls 
		--left outer join [dbo].[TransactionCollectionList] tcl on tcl.ProductID=ls.SubscriptionID
		--left outer  join [dbo].[TransactionCollectionReciepts] tcr on tcr.CollectionListID = tcl.CollectionListID
		--left outer join [dbo].[MasterType]  mt on tcl.TransactionModeTypeID = mt.TypeID
		--left outer join [dbo].[MasterType]  mt1 on tcl.TransactionCategoryTypeID = mt1.TypeID
		--where ChitSubscriptionCode=@Code and tcr.CollectionDateTime is not null and mt.Type is not null and tcl.StatusTypeID in (1035,1036)
	 --SELECT @Amt = Sum(Credit) FROM #TMP Where Category Not in('Prize')
	 --SELECT @Amt = Sum(Case When Category in('Subscription', 'Dividend') Then Credit Else 0 End) FROM #TMP

	 if (@SDesc = 'Created' Or @SDesc = 'Active Approval' Or @SDesc = 'Active' Or @SDesc = 'Payment Due' Or @SDesc = 'Modification Approval' Or @SDesc = 'Modification' Or @SDesc = 'Legal' Or @SDesc = 'Arrear')
		SELECT @Amt = Sum(Case When Category in('Subscription', 'Dividend') Then Credit Else 0 End) FROM #TMP
	 else
		SELECT @Amt = Sum(Case When Category in('Subscription', 'Dividend', 'Waiver') Then Credit Else (Case When Category in('Foreman Commission', 'Dividend Reversal', 'Charges', 'Prize Transfer', 'Prize') Then Debit * -1 Else 0 End) End) FROM #TMP
	 
	 Set @Amt = ISNULL(@Amt, 0)

	 if (@SDesc = 'Created' Or @SDesc = 'Active Approval' Or @SDesc = 'Active' Or @SDesc = 'Payment Due' Or @SDesc = 'Modification Approval' Or @SDesc = 'Modification' Or @SDesc = 'Legal' Or @SDesc = 'Arrear')
		Set @BalAmt = @Amt - (@TicketNumber * @ChitValue)
	 else
	    Set @BalAmt = @Amt

	 Insert into @SummaryMaster(SDesc,Amount)
	 Values('Balance', @BalAmt)
	 --select 'Balance', CAST((@Amt - (ChitScheme.ChitValue * TicketNumber)) AS decimal(18,2)) from dbo.chitsubscription 
		--	 inner join dbo.ChitGroup cg on cg.ChitGroupID = chitsubscription.ChitGroupID
  --           INNER JOIN dbo.person ON dbo.chitsubscription.personid = dbo.person.personid 
  --           INNER JOIN dbo.chitscheme ON dbo.chitsubscription.chitschemeid = 
  --                      dbo.chitscheme.chitschemeid where chitsubscription.ChitSubscriptionCode=@Code

	 Insert into @StatusMaster(SDesc,Amount)
	 Values('Status', @SDesc)
	 --select 'Status', Type from dbo.chitsubscription 
		--	 inner join dbo.ChitGroup cg on cg.ChitGroupID = chitsubscription.ChitGroupID
  --           INNER JOIN dbo.person ON dbo.chitsubscription.personid = dbo.person.personid 
  --           INNER JOIN dbo.chitscheme ON dbo.chitsubscription.chitschemeid = dbo.chitscheme.chitschemeid 
		--	 INNER JOIN dbo.MasterType ON dbo.chitsubscription.StatusTypeID = dbo.MasterType.TypeID 
		--	 where chitsubscription.ChitSubscriptionCode=@Code

end
*/


--Select @Amt
--Select @BalAmt

	 Insert into @SummaryMaster(SDesc,Amount)
	 Values('Principal Due', ABS(@BalAmt))

Select SDesc AS Summary, Amount as Value From @SummaryMaster

Select SDesc AS Status, Amount as Type From @StatusMaster


--Insert Balance in Temp Table
Declare @ZPaidAmt numeric(18,2), @AValue numeric(18,2), @BalValue numeric(18,2)

select @ZPaidAmt = SUM((CAST(credit AS decimal(18,2)) + CAST(debit AS decimal(18,2))))  from #TMP 
--Where Category in('Principal Repayment','Interest')
Where Category like 'Principal Repayment%' or  Category  ='Interest'

select @AValue = SUM((CAST(credit AS decimal(18,2)) + CAST(debit AS decimal(18,2))))  from #TMP 
Where Category in('Interest - Prepaid','Principal Disbursement')

Select @BalValue = Sum(Amount)  From @SummaryMaster
Where SDesc in('Principal Due')

Set @ZPaidAmt = ISNULL(@ZPaidAmt,0)
Set @AValue = ISNULL(@AValue,0)
Set @BalValue = ISNULL(@BalValue,0)

Insert into Tmp_Ledger_Balance_Detail(LCode, PRAmt, PaidAmt, BalAmt,PassbookTotal,OverDueAmt)
Values(@Code, @AValue, @ZPaidAmt, abs(@BalValue), @BalValue,0)

Select * From Tmp_Ledger_Balance_Detail where lcode = @Code


--SELECT 
--    date, s.debit, s.credit,
--    SUM(CAST(s.credit AS int) -CAST(s.debit AS int)) OVER (ORDER BY s.date
--                                  ROWS BETWEEN UNBOUNDED PRECEDING
--   AND CURRENT ROW)
-- AS balance
--FROM
--    @TableMaster AS s
--ORDER BY
--    date ;

DROP TABLE #TMP



end

