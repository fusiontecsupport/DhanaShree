-- exec [dbo].[CustomerAccountLedgerbyPcode_Deposit]   D2747
alter PROCEDURE [dbo].[CustomerAccountLedgerbyPcode_Deposit] 
@Code varchar(50)
AS 
BEGIN 

DECLARE @TableMaster TABLE 
        ( 
           [Date] Datetime, 
           Mode           VARCHAR(50), 
           ReceiptNo     VARCHAR(50),
           Category           VARCHAR(50), 
		   Remarks           VARCHAR(200), 
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
		   Remarks           VARCHAR(200), 
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
		   Remarks           VARCHAR(200), 
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


Declare @OPDate smalldatetime
Declare @Openingamount numeric(18,2)
Declare @Amt numeric(18,2)
Declare @BalAmt numeric(18,2)
Declare @LastPaymentAmt numeric(18,2)
Declare @PaymentDate varchar(50)
Declare @BookVerifiedDate varchar(50)

select @BookVerifiedDate =  isnull(CONVERT(varchar,(max(bvl.VerifiedDate)), 106),'')
from DepositAccount da (nolock) 
		join clientmaster pl(nolock) on da.PersonID = pl.clientid
		join DepositScheme ds(nolock) on da.DepositSchemeID = ds.DepositSchemeID
		left join CommonAccountBook ca(nolock) on da.DepositAccountID = ca.OwnerID
		join MasterType mt(nolock) on ca.OwnerTypeID = mt.TypeID
		left join CompanyEmployeeProductMap tpm (nolock) on ds.DepositSchemeID = tpm.SchemeID
		left join Organization org (nolock) on da.OrganizationID = org.OrganizationID
		left join TransactionBookVerificationList bvl(nolock) on  ca.AccountBookID = bvl.BookID
	where ca.BookTerminatedDate is null
	and da.StatusTypeID = 758
	and ca.StatusTypeID = 836
	and ca.OwnerTypeID = 182
	and da.DepositAccountCode=@Code


--
select Top 1 @PaymentDate = CONVERT(Varchar(11), PaymentDateTime, 106)
from [DepositAccount] ls  (nolock)
--left outer join [dbo].[TransactionCollectionList] tcl (nolock) on tcl.ProductID = ls.DepositAccountID 
left outer join [dbo].[TransactionPaymentList] tpl  (nolock) on tpl.ProductID = ls.DepositAccountID
left outer join [dbo].[TransactionPayment] tp  (nolock) on tp.PaymentListID =tpl.PaymentListID
left outer join [dbo].[TransactionCollectionReciepts] tcr  (nolock) on tcr.CollectionListID = tp.PaymentListID
left outer join [dbo].[MasterType]  mt  (nolock) on tpl.TransactionModeTypeID = mt.TypeID
left outer join [dbo].[MasterType]  mt1  (nolock) on tpl.TransactionCategoryTypeID = mt1.TypeID
where DepositAccountCode=@Code and tp.PaymentDateTime is not null and mt.Type is not null and tpl.StatusTypeID in (1035,1036)
and tpl.ProductTypeTypeID = 31
Order by PaymentDateTime Desc-- Limit 1

Insert into @StatusMaster(SDesc, Amount)
select 'Last Book Verified', @BookVerifiedDate

	SELECT        @Openingamount = dbo.DepositAccount.DepositOpeningAmount
	FROM            dbo.DepositAccount  (nolock) INNER JOIN
							 dbo.DepositScheme  (nolock) ON dbo.DepositAccount.DepositSchemeID = dbo.DepositScheme.DepositSchemeID INNER JOIN
							 dbo.MasterType AS MasterType_2 ON dbo.DepositAccount.StatusTypeID = MasterType_2.TypeID
	WHERE        (dbo.DepositAccount.DepositAccountCode = @Code)


select @OPDate = CONVERT(varchar,mIN(tp.PaymentDateTime), 106)
from [DepositAccount] ls  (nolock)
--left outer join [dbo].[TransactionCollectionList] tcl  (nolock) on tcl.ProductID = ls.DepositAccountID 
left outer join [dbo].[TransactionPaymentList] tpl (nolock) on tpl.ProductID = ls.DepositAccountID
left outer join [dbo].[TransactionPayment] tp (nolock) on tp.PaymentListID =tpl.PaymentListID
left outer join [dbo].[TransactionCollectionReciepts] tcr (nolock) on tcr.CollectionListID = tp.PaymentListID
left outer join [dbo].[MasterType]  mt (nolock) on tpl.TransactionModeTypeID = mt.TypeID
left outer join [dbo].[MasterType]  mt1 (nolock)  on tpl.TransactionCategoryTypeID = mt1.TypeID
where DepositAccountCode=@Code and tp.PaymentDateTime is not null and mt.Type is not null and tpl.StatusTypeID in (1035,1036)
and tpl.ProductTypeTypeID = 31

Set @OPDate = ISNULL(@OPDate,CONVERT(varchar,GETDATE(), 106))

--Set @Amt = 0--ISNULL(@Amt, 0)
--Set @BalAmt = 0
--Set @TicketNumber = ISNULL(@TicketNumber, 0)
Set @Openingamount = ISNULL(@Openingamount, 0)
--Select @Loanamount

-- Deposit Receipt

if (@Openingamount > 0)
	Insert into @TableMaster ([Date],Mode,ReceiptNo,Category,Debit,Credit,Balance)
	Values(@OPDate, '', '', 'Opening Balance', 0, @Openingamount,0)


Insert into @TableMaster 
select distinct CONVERT(varchar, tcr.CollectionDateTime, 106),mt.Type, --tcr.PrintRecepitCode,
 isnull(tcr.PrintRecepitCode,'') +   (case when mt.Type ='Transfer' then 'Source A/c: '+ TargetProductCode else'' end) ,
mt1.Type+ case when mt3.Type is null then '' else '['+mt3.Type + ' ' + cast (CollectionReceiptOldAmount-BookAmount as varchar(20))+']' end,
substring(tcl.remarks,1,200),
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
		0.00
from [DepositAccount] ls  (nolock) 
left outer join [dbo].[TransactionCollectionList] tcl (nolock)  on tcl.ProductID=ls.DepositAccountID
left outer  join [dbo].[TransactionCollectionReciepts] tcr (nolock)  on tcr.CollectionListID = tcl.CollectionListID
left outer join [dbo].PaymentTransferLog pymt  (nolock) on tcl.CollectionListID=pymt.CollectionListID -- and substring(TargetProductCode ,1,1) in ('C','L','D')
left outer join [dbo].[MasterType]  mt  (nolock) on tcl.TransactionModeTypeID = mt.TypeID
left outer join [dbo].[MasterType]  mt1 (nolock) on tcl.TransactionCategoryTypeID = mt1.TypeID
left outer join (TransactionBookVerification bv(nolock) join MasterType mt3(nolock) on bv.VerificationFindingTypeID = mt3.TypeID)
		on tcr.CollectionRecieptID = bv.ReferenceCollectionReceiptID
where ls.DepositAccountCode=@Code and  tcr.CollectionDateTime is not null and mt.Type is not null and tcl.StatusTypeID in (1035,1036)
and ProductTypeID = 31
--GROUP  BY CONVERT(varchar, tcr.CollectionDateTime, 106), tcr.PrintRecepitCode,mt.Type,tcr.CollectedAmount,mt1.Type

--select * from @TableMaster
-- Deposit Payment

Insert into @TableMaster 
select CONVERT(varchar,tp.PaymentDateTime, 106),mt.Type, '',--(case when mt.Type ='Transfer' then 'Source A/c: '+ TargetProductCode else'' end) ,
mt1.Type+ case when mt3.Type is null then '' else '['+mt3.Type + ' ' + cast (CollectionReceiptOldAmount-BookAmount as varchar(20))+']' end,
substring(tpl.remarks,1,200),
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
		0.00
from [DepositAccount] ls  (nolock) 
--left outer join [dbo].[TransactionCollectionList] tcl (nolock)  on tcl.ProductID = ls.DepositAccountID  and ProductTypeID = 31
left outer join [dbo].[TransactionPaymentList] tpl (nolock)  on tpl.ProductID = ls.DepositAccountID
left outer join [dbo].[TransactionPayment] tp (nolock)  on tp.PaymentListID =tpl.PaymentListID
--left outer join [dbo].PaymentTransferLog pymt (nolock)  on tcl.CollectionListID=pymt.CollectionListID
left outer join [dbo].[TransactionCollectionReciepts] tcr (nolock)  on tcr.CollectionListID = tp.PaymentListID
left outer join [dbo].[MasterType]  mt (nolock)  on tpl.TransactionModeTypeID = mt.TypeID
left outer join [dbo].[MasterType]  mt1 (nolock)  on tpl.TransactionCategoryTypeID = mt1.TypeID
left outer join (TransactionBookVerification bv(nolock) join MasterType mt3(nolock) on bv.VerificationFindingTypeID = mt3.TypeID)
		on tcr.CollectionRecieptID = bv.ReferenceCollectionReceiptID
where DepositAccountCode=@Code and tp.PaymentDateTime is not null and mt.Type is not null and tpl.StatusTypeID in (1035,1036)
and tpl.ProductTypeTypeID = 31
--GROUP  BY 
--CONVERT(varchar,tp.PaymentDateTime, 106),tcr.PrintRecepitCode,mt.Type,tp.PaidAmount,mt1.Type


--Loan For Tenure

select convert(varchar, date, 106) as Date, 
		   Mode,
           ReceiptNo,
           Category, 
		   Remarks
           Debit, 
           Credit, 
				--SUM((CAST(credit AS int) - CAST(debit AS int))) OVER (ORDER BY date ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW)
				SUM((CAST(credit AS decimal(18,2)) - (Case When Category in('Payment','Payment Transfer') then CAST(debit AS decimal(18,2)) Else 0 End))) OVER (ORDER BY date ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW)
        AS Cumilative  INTO #TMP
		from @TableMaster  order by CONVERT(DateTime, Date,101)


SELECT * FROM #TMP

select @LastPaymentAmt  = isnull(sum(debit),0) from #tmp where [Date] =   CONVERT(varchar,@PaymentDate, 106) and Category = 'Payment'

SELECT '','','','','Total', Sum(CAST (Debit AS decimal(18,2))) AS Total ,SUM(CAST (Credit AS decimal(18,2))) AS [value], 
SUM((CAST(credit AS decimal(18,2)) - (Case When Category in('Payment','Payment Transfer') then 
CAST(debit AS decimal(18,2)) Else 0 End)))
 AS [BalanceTotal]
            FROM  #TMP 


--SELECT '','','','Total', Sum(CAST (Debit AS int)) AS Total ,SUM(CAST (Credit AS int)) AS [value], SUM(CAST (Credit AS INT)) - SUM(CAST (Debit AS INT)) AS [BalanceTotal]
--            FROM  #TMP



IF EXISTS (SELECT 1 FROM DepositAccount  (nolock)  WHERE DepositAccountCode=@Code)
BEGIN
	SELECT        dbo.DepositAccount.DepositAccountCode, dbo.DepositAccount.TallyCode, dbo.DepositAccount.DepositOpeningAmount, 
	Isnull(dbo.DepositAccount.InterestRate,0) as InterestRate, dbo.DepositScheme.SchemeName, MasterType_2.Type AS StatusType,
	CONVERT(varchar(10), DepositAccount.AccountOpeningDate, 105) as AccountOpeningDate
	FROM            dbo.DepositAccount  (nolock) INNER JOIN
							 dbo.DepositScheme  (nolock) ON dbo.DepositAccount.DepositSchemeID = dbo.DepositScheme.DepositSchemeID INNER JOIN
							 dbo.MasterType  (nolock)  AS MasterType_2 ON dbo.DepositAccount.StatusTypeID = MasterType_2.TypeID
	WHERE        (dbo.DepositAccount.DepositAccountCode = @Code)

end



--select Category, SUM((CAST(credit AS decimal(18,2)) + CAST(debit AS decimal(18,2)))) as Totals from #TMP Where Category <>'Opening Balance' GROUP  BY Category

select case when category like '%Paid (%' 
--and @Code like 'l%' then 'Principal Repayment'
--when category like '%Paid (%' and @Code like 'D%' 
then 'Collection'
--when category like '%Paid (%' and @Code like 'C%' then 'Subscription'
else category end as Category, SUM((CAST(credit AS decimal(18,2)) + CAST(debit AS decimal(18,2)))) as Totals 
from #TMP 
where [Date] =   CONVERT(varchar,@PaymentDate, 106) 
GROUP  BY case 
--when category like '%Paid (%' and @Code like 'l%' then 'Principal Repayment'
when category like '%Paid (%' then 'Collection' --and @Code like 'D%' 
--when category like '%Paid (%' and @Code like 'C%' then 'Subscription'
else category end

SELECT @Amt = Sum(Case When Category in('Collection', 'Interest', 'Charges') or category like '%Paid (%' Then Credit Else (Case When Category in('Payment') Then Debit * -1 Else 0 End) End) FROM #TMP

/*
Select 
@Amt =  Sum(Case When Category in('Collection', 'Interest', 'Charges') then (CAST(credit AS decimal(18,2))) Else CAST(debit AS decimal(18,2)) * -1 End) /*-
Sum(Case When Category in('Payment') then (CAST(credit AS decimal(18,2)) + CAST(debit AS decimal(18,2))) Else 0 End)*/
From #TMP 
Group by Category
*/

Set @Amt = ISNULL(@Amt,0)
Set @BalAmt = @Openingamount + @Amt

--Select @Openingamount
--Select @Amt
--Select @BalAmt

	 Insert into @SummaryMaster(SDesc,Amount)
	 Values('Opening Balance', ABS(@Openingamount))

	 Insert into @SummaryMaster(SDesc,Amount)
	 Values('Balance', ABS(@BalAmt))


--
select Top 1 @PaymentDate = CONVERT(Varchar(10), PaymentDateTime, 105)
from [DepositAccount] ls (nolock)
--left outer join [dbo].[TransactionCollectionList] tcl (nolock) on tcl.ProductID = ls.DepositAccountID 
left outer join [dbo].[TransactionPaymentList] tpl (nolock) on tpl.ProductID = ls.DepositAccountID
left outer join [dbo].[TransactionPayment]  (nolock)tp on tp.PaymentListID =tpl.PaymentListID
left outer join [dbo].[TransactionCollectionReciepts] tcr  (nolock)on tcr.CollectionListID = tp.PaymentListID
left outer join [dbo].[MasterType]  mt  (nolock) on tpl.TransactionModeTypeID = mt.TypeID
left outer join [dbo].[MasterType]  mt1 (nolock) on tpl.TransactionCategoryTypeID = mt1.TypeID
where DepositAccountCode=@Code and tp.PaymentDateTime is not null and mt.Type is not null and tpl.StatusTypeID in (1035,1036)
and tpl.ProductTypeTypeID = 31
Order by PaymentDateTime Desc-- Limit 1
--GROUP  BY 
--CONVERT(varchar,tp.PaymentDateTime, 106),tcr.PrintRecepitCode,mt.Type,tp.PaidAmount,mt1.Type

--

Insert into @StatusMaster(SDesc, Amount)
Values('Last Payment Date', @PaymentDate)


delete Tmp_Ledger_Balance_Detail where LCode = @Code

Insert into Tmp_Ledger_Balance_Detail(LCode, PRAmt, PaidAmt, BalAmt, PrizeAmt)
Values(@Code , 0, @LastPaymentAmt, abs(@BalAmt), 0)

Select SDesc AS Summary, Amount as Value From @SummaryMaster

Select SDesc AS Status, Amount as Type From @StatusMaster



DROP TABLE #TMP

end


