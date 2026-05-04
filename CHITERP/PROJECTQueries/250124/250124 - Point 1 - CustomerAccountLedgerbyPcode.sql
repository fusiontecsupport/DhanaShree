use DSFinFusion
go

-- exec CustomerAccountLedgerbyPcode 'c2'
-- exec [dbo].[CustomerAccountLedgerbyPcode]   'DSC/DSB3/11'
-- exec [dbo].[CustomerAccountLedgerbyPcode]   'DSC/DSB2/09'

-- SELECT * FROM ChitSubscription WHERE CHITSUBSCRIPTIONCODE = 'DSC/DSE1/05'
-- SELECT * FROM Tmp_Ledger_Balance_Detail WHERE lCODE = 'stc/fusigr/01'
alter PROCEDURE [dbo].[CustomerAccountLedgerbyPcode] 
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
		Remarks			VARCHAR(200), 
		Debit           decimal(18, 2),--VARCHAR(50), 
		Credit          decimal(18, 2),--VARCHAR(50), 
		Balance        decimal(18, 2),
		BkVfcnSts		int,
		BkVfcnDt		datetime,
		BkVfcnAmt		NUMERIC(18,2),
		TranID			varchar(100)
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

DECLARE @CategoryMaster TABLE 
		( 
			ord int,
			Category           VARCHAR(100), 
			Totals          decimal(18, 2)--VARCHAR(100)--NUMERIC(18, 2) 
		) 


Declare @TicketNumber numeric(18,2)
Declare @ChitValue numeric(18,2)
Declare @Amt numeric(18,2)
Declare @BalAmt numeric(18,2)
Declare @SDesc varchar(50)
Declare @BookVerifiedDate varchar(50)
Declare @LastReceiptDate varchar(50)
Declare @LastReceiptDt datetime
Declare @LastPaymentDate varchar(50)
Declare @LastPaymentDt datetime

Declare @CommencementDt varchar(50)
Declare @EndDt varchar(50)
Declare @ClosureDt varchar(50)
Declare @clientname varchar(250)
Declare @address varchar(1000)
Declare @chitgroup varchar(250)
Declare @chitscheme varchar(250)
declare @chitsubsid int
select @BookVerifiedDate = isnull( CONVERT(varchar,(max(bvl.VerifiedDate)), 106),''),
@CommencementDt = isnull( CONVERT(varchar,(max(cs.CommencementDate)), 106),''),
@ClosureDt = isnull( CONVERT(varchar,(max(cs.ClosureDate)), 106),''),
@EndDt= isnull( CONVERT(varchar,(max(CS.ENDDATE)), 106),''),
@chitsubsid = max(SubscriptionID)
from ChitSubscription cs (nolock) 
		join ClientMaster cl(nolock) on cs.PersonID = cl.CLIENTID
		join ChitScheme csc(nolock) on cs.ChitSchemeID = csc.ChitSchemeID
		LEFT join CommonAccountBook ca(nolock) on cs.SubscriptionID = ca.OwnerID AND ca.BookTerminatedDate is null
		AND ca.StatusTypeID = 836 and ca.OwnerTypeID = 854
		LEFT join MasterType mt(nolock) on ca.OwnerTypeID = mt.TypeID
		left join CompanyEmployeeProductMap tpm (nolock) on cs.SubscriptionID = tpm.SchemeID
		--left join Organization org (nolock) on cs.OrganizationID = org.OrganizationID
		LEFT join TransactionBookVerificationList bvl(nolock) on  ca.AccountBookID = bvl.BookID
where  --cs.StatusTypeID = 881 and 
cs.ChitSubscriptionCode=@Code

Insert into @StatusMaster(SDesc, Amount)
select 'Subscription Code', @Code
Insert into @StatusMaster(SDesc, Amount)
select 'Commencement Date', @CommencementDt
Insert into @StatusMaster(SDesc, Amount)
select 'Subscription End Date', @EndDt
Insert into @StatusMaster(SDesc, Amount)
select 'Actual Closure Date', @ClosureDt

--Chit Status / Chit Value / Chit Ticket No.

select @SDesc =  Type, @ChitValue = CAST((ChitScheme.ChitValue) AS decimal(18,2)), 
@TicketNumber = case when TicketNumber='' then '1' else TicketNumber  end,
@clientname = ClientMaster.CLIENTNAME,@chitgroup =chitgroupname,@chitscheme=chitschemename,
@address = isnull(CLIENTADDRESS1,'') +',
'+isnull(CLIENTADDRESS2,'')+',
'+isnull(CLIENTCITY,'') +' - ' + isnull(CLIENTPINCD,'')+'
Contact: '+ isnull(CLIENTMOBILENO1,'') +', ' + isnull(CLIENTEMAILID,'')
from dbo.chitsubscription
inner join dbo.ChitGroup cg on cg.ChitGroupID = chitsubscription.ChitGroupID
--INNER JOIN dbo.person ON dbo.chitsubscription.personid = dbo.person.personid 
INNER JOIN dbo.ClientMaster ON dbo.chitsubscription.personid = dbo.ClientMaster.CLIENTID
INNER JOIN dbo.chitscheme ON dbo.chitsubscription.chitschemeid = dbo.chitscheme.chitschemeid 
INNER JOIN dbo.MasterType ON dbo.chitsubscription.StatusTypeID = dbo.MasterType.TypeID 
where chitsubscription.ChitSubscriptionCode=@Code

Set @Amt = 0--ISNULL(@Amt, 0)
Set @BalAmt = 0
Set @TicketNumber = ISNULL(@TicketNumber, 0)
Set @ChitValue = ISNULL(@ChitValue, 0)


---- Loan

--Insert into @TableMaster 
--select tcr.CollectionDateTime,mt.Type, --tcr.PrintRecepitCode,
--isnull(tcr.PrintRecepitCode,'') +   (case when mt.Type ='Transfer' then 'Source A/c: '+ isnull(TargetProductCode,'') else'' end) ,
--mt1.Type+ case when mt3.Type is null then '' else '['+mt3.Type + ' ' + cast (CollectionReceiptOldAmount-BookAmount as varchar(20))+']' end,
--tcr.CollectedAmount,0.00,0.00 from LoanSubscription ls 
--left outer join [dbo].[TransactionCollectionList] tcl on tcl.ProductID=ls.LoanSubscriptionID
--left outer join [dbo].PaymentTransferLog pymt on tcl.CollectionListID=pymt.CollectionListID  and substring(TargetProductCode ,1,1) in ('C','L','D')
--left outer  join [dbo].[TransactionCollectionReciepts] tcr on tcr.CollectionListID = tcl.CollectionListID
--left outer join [dbo].[MasterType]  mt on tcl.TransactionModeTypeID = mt.TypeID
--left outer join [dbo].[MasterType]  mt1 on tcl.TransactionCategoryTypeID = mt1.TypeID
--left outer join (TransactionBookVerification bv(nolock) join MasterType mt3(nolock) on bv.VerificationFindingTypeID = mt3.TypeID)
--		on tcr.CollectionRecieptID = bv.ReferenceCollectionReceiptID
--where LoanSubscriptionCode=@Code and tcr.CollectionDateTime is not null and mt.Type is not null and tcl.StatusTypeID in (1035,1036)
--GROUP  BY tcr.CollectionDateTime, tcr.PrintRecepitCode,mt.Type,tcr.CollectedAmount,mt1.Type,mt1.Type,mt3.Type,CollectionReceiptOldAmount-BookAmount, TargetProductCode

--order by CONVERT(varchar, tcr.CollectionDateTime, 106) asc



--Insert into @TableMaster 

--select tp.PaymentDateTime,mt.Type,tcr.PrintRecepitCode,
--mt1.Type+ (case when mt.Type ='Transfer' then 'Source A/c: '+ TargetProductCode else'' end) +case when mt3.Type is null then '' else '['+mt3.Type + ' ' + cast (CollectionReceiptOldAmount-BookAmount as varchar(20))+']' end,0.00,tp.PaidAmount,0.00  
--from LoanSubscription ls (nolock)
--left outer join [dbo].[TransactionCollectionList] tcl (nolock) on tcl.ProductID = ls.LoanSubscriptionID and ProductTypeID = 30 -- where  CollectionListID=4676
--left outer join [dbo].PaymentTransferLog pymt (nolock) on tcl.CollectionListID=pymt.CollectionListID  and substring(TargetProductCode ,1,1) in ('C','L','D')
--left outer join [dbo].[TransactionPaymentList] tpl (nolock) on tpl.ProductID = ls.LoanSubscriptionID
--left outer join [dbo].[TransactionPayment] tp (nolock) on tp.PaymentListID =tpl.PaymentListID
--left outer join [dbo].[TransactionCollectionReciepts] tcr (nolock) on tcr.CollectionListID = tp.PaymentListID
--left outer join [dbo].[MasterType]  mt (nolock) on tpl.TransactionModeTypeID = mt.TypeID
--left outer join [dbo].[MasterType]  mt1 (nolock) on tpl.TransactionCategoryTypeID = mt1.TypeID
--left outer join (TransactionBookVerification bv(nolock) join MasterType mt3(nolock) on bv.VerificationFindingTypeID = mt3.TypeID)
--		on tcr.CollectionRecieptID = bv.ReferenceCollectionReceiptID
--where LoanSubscriptionCode=@Code and tp.PaymentDateTime is not null and mt.Type is not null and tpl.StatusTypeID in (1035,1036)
--GROUP  BY 
--tp.PaymentDateTime,tcr.PrintRecepitCode,mt.Type,tp.PaidAmount,mt1.Type,mt1.Type,mt3.Type,CollectionReceiptOldAmount-BookAmount, TargetProductCode

--order by CONVERT(varchar,tp.PaymentDateTime, 106) asc


-- Chit
Insert into @TableMaster 
select distinct  CONVERT(varchar, tcr.CollectionDateTime, 106),mt.Type, 
--isnull(tcr.PrintRecepitCode,'')+
(case when mt.Type ='Transfer' then 'Source A/c: '+ isnull(TargetProductCode,'') else'' end) +
CASE WHEN mt1.Type in('Prize', 'Agent Commission','Prize Transfer', 'Discount', 'Company', 'Foreman Commission', 'Dividend Reversal','Cancellation', 'Charges') then '' Else isnull(tcr.PrintRecepitCode,'') End,
mt1.Type,
substring(tcl.remarks,1,200)+
case when tcl.chequeno is not null then 'Chq. No: ' + tcl.chequeno + ' Dtd: '+ convert(varchar,tcl.chequedate,106)+ ' Bnk: '+tcl.chequebankname else '' end,
CASE
	WHEN mt1.Type='Prize' THEN (Case When AmountCodeConstant > 0 Then tcr.CollectedAmount/ AmountCodeConstant Else tcr.CollectedAmount End)
	WHEN mt1.Type='Prize Transfer' THEN tcr.CollectedAmount
	WHEN mt1.Type= 'Agent Commission' THEN tcr.CollectedAmount 
	WHEN mt1.Type='Discount' THEN tcr.CollectedAmount
	WHEN mt1.Type='Company' THEN tcr.CollectedAmount
	WHEN mt1.Type='Foreman Commission' THEN tcr.CollectedAmount
	WHEN mt1.Type='Dividend Reversal' THEN tcr.CollectedAmount
	WHEN mt1.Type='Cancellation' THEN tcr.CollectedAmount
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
	0.00,
	case	when isnull(tcl.BkVfcnSts,0) = 0 then 0
			when isnull(tcl.BkVfcnSts,0) = 1 AND DATEDIFF(D,ISNULL(BkVfcnDt,GETDATE()),GETDATE())<=7 then 1 else 2 end,
			tcl.BkVfcnDt,tcl.BkVfcnAmt, 'C~'+cast(tcl.collectionlistid as varchar(50))
--tcr.CollectedAmount,'',
from ChitSubscription ls  (nolock)
left outer join [dbo].[TransactionCollectionList] tcl (nolock) on tcl.ProductID=ls.SubscriptionID
left outer  join [dbo].[TransactionCollectionReciepts] tcr (nolock) on tcr.CollectionListID = tcl.CollectionListID
left outer join [dbo].PaymentTransferLog pymt (nolock) on tcr.CollectionListID=pymt.CollectionListID and substring(TargetProductCode ,1,1) in ('C','L','D')
left outer join [dbo].[MasterType]  mt (nolock) on tcl.TransactionModeTypeID = mt.TypeID
left outer join [dbo].[MasterType]  mt1 (nolock) on tcl.TransactionCategoryTypeID = mt1.TypeID
inner join [dbo].ChitScheme  (nolock) ON ls.ChitSchemeID = dbo.ChitScheme.ChitSchemeID
left join [dbo].MasterAmountCode  (nolock)ON [dbo].ChitScheme.AmountCodeID = [dbo].MasterAmountCode.AmountCodeID
--left outer join (TransactionBookVerification bv(nolock) join MasterType mt3(nolock) on bv.VerificationFindingTypeID = mt3.TypeID)
	--	on tcr.CollectionRecieptID = bv.ReferenceCollectionReceiptID
where ChitSubscriptionCode=@Code and tcr.CollectionDateTime is not null and mt.Type is not null and tcl.StatusTypeID in (1035,1036) 
and ProductTypeID = 29
--------GROUP  BY CONVERT(varchar, tcr.CollectionDateTime, 106), tcr.PrintRecepitCode,mt.Type,tcr.CollectedAmount,mt1.Type
------order by CONVERT(varchar, tcr.CollectionDateTime, 106) asc


-- Chit
Insert into @TableMaster 
select distinct  CONVERT(varchar, bv.BookEntryDate, 106),mt.Type, 
--isnull(tcr.PrintRecepitCode,'')+
(case when mt.Type ='Transfer' then 'Source A/c: '+ isnull(TargetProductCode,'') else'' end) +
CASE WHEN mt1.Type in('Prize', 'Agent Commission','Prize Transfer', 'Discount', 'Company', 'Foreman Commission', 'Dividend Reversal','Cancellation', 'Charges') then '' Else isnull(tcr.PrintRecepitCode,'') End,
mt1.Type+ case when mt3.Type is null then '' else '['+mt3.Type + ' ' + cast (CollectionReceiptOldAmount-BookAmount as varchar(20))+']' end,
substring(tcl.remarks,1,200),
CASE
	WHEN mt1.Type='Prize' THEN (Case When AmountCodeConstant > 0 Then (CollectionReceiptOldAmount-BookAmount)*(case when bv.VerificationFindingTypeID = 1099 then -1 else 1 End)/ AmountCodeConstant Else tcr.CollectedAmount End)
	WHEN mt1.Type='Prize Transfer' THEN (CollectionReceiptOldAmount-BookAmount)*(case when bv.VerificationFindingTypeID = 1099 then -1 else 1 End)
	WHEN mt1.Type= 'Agent Commission' THEN (CollectionReceiptOldAmount-BookAmount)*(case when bv.VerificationFindingTypeID = 1099 then -1 else 1 End)
	WHEN mt1.Type='Discount' THEN (CollectionReceiptOldAmount-BookAmount)*(case when bv.VerificationFindingTypeID = 1099 then -1 else 1 End)
	WHEN mt1.Type='Company' THEN (CollectionReceiptOldAmount-BookAmount)*(case when bv.VerificationFindingTypeID = 1099 then -1 else 1 End)
	WHEN mt1.Type='Foreman Commission' THEN (CollectionReceiptOldAmount-BookAmount)*(case when bv.VerificationFindingTypeID = 1099 then -1 else 1 End)
	WHEN mt1.Type='Dividend Reversal' THEN (CollectionReceiptOldAmount-BookAmount)*(case when bv.VerificationFindingTypeID = 1099 then -1 else 1 End)
	WHEN mt1.Type='Cancellation' THEN (CollectionReceiptOldAmount-BookAmount)*(case when bv.VerificationFindingTypeID = 1099 then -1 else 1 End)
	WHEN mt1.Type='Charges' THEN (CollectionReceiptOldAmount-BookAmount)*(case when bv.VerificationFindingTypeID = 1099 then -1 else 1 End)
	WHEN mt1.Type='Interest' THEN (CollectionReceiptOldAmount-BookAmount)*(case when bv.VerificationFindingTypeID = 1099 then -1 else 1 End)
	ELSE 0
	END
	AS Debit,
CASE
	WHEN mt1.Type='Dividend' THEN (CollectionReceiptOldAmount-BookAmount)*(case when bv.VerificationFindingTypeID = 1099 then -1 else 1 End)
	--WHEN mt1.Type='Interest' THEN tcr.CollectedAmount
	--WHEN mt1.Type='Subscription' THEN tcr.CollectedAmount
	--WHEN mt1.Type='Subscription' THEN (Case WHEN mt.Type in('Transfer') then (Case WHEN AmountCodeConstant > 0 THEN tcr.CollectedAmount / AmountCodeConstant Else tcr.CollectedAmount End) Else tcr.CollectedAmount End)
	WHEN mt1.Type='Subscription' THEN (Case WHEN mt.Type in('Transfer') then (Case WHEN AmountCodeConstant > 0 THEN (CollectionReceiptOldAmount-BookAmount) *(case when bv.VerificationFindingTypeID = 1099 then -1 else 1 End)
	Else (CollectionReceiptOldAmount-BookAmount)*(case when bv.VerificationFindingTypeID = 1099 then -1 else 1 End) End) Else (CollectionReceiptOldAmount-BookAmount)*
	(case when bv.VerificationFindingTypeID = 1099 then (case when bv.VerificationFindingTypeID = 1099 then -1 else 1 End) else 1 End) end)
	WHEN mt1.Type='Waiver' THEN (CollectionReceiptOldAmount-BookAmount)*(case when bv.VerificationFindingTypeID = 1099 then (case when bv.VerificationFindingTypeID = 1099 then -1 else 1 End) else 1 End)
	ELSE 0
	END
	AS Credit,
	0.00,
	case	when isnull(tcl.BkVfcnSts,0) = 0 then 0
			when isnull(tcl.BkVfcnSts,0) = 1 AND DATEDIFF(D,ISNULL(BkVfcnDt,GETDATE()),GETDATE())<=7 then 1 else 2 end,
			tcl.BkVfcnDt,tcl.BkVfcnAmt, 'C~'+cast(tcl.collectionlistid as varchar(50))
--tcr.CollectedAmount,'',
from ChitSubscription ls  (nolock)
left outer join [dbo].[TransactionCollectionList] tcl (nolock) on tcl.ProductID=ls.SubscriptionID and ProductTypeID = 29
left outer  join [dbo].[TransactionCollectionReciepts] tcr (nolock) on tcr.CollectionListID = tcl.CollectionListID
left outer join [dbo].PaymentTransferLog pymt (nolock) on tcr.CollectionListID=pymt.CollectionListID --and substring(TargetProductCode ,1,1) in ('C','L','D')
left outer join [dbo].[MasterType]  mt (nolock) on tcl.TransactionModeTypeID = mt.TypeID
left outer join [dbo].[MasterType]  mt1 (nolock) on tcl.TransactionCategoryTypeID = mt1.TypeID
inner join [dbo].ChitScheme  (nolock) ON ls.ChitSchemeID = dbo.ChitScheme.ChitSchemeID
left join [dbo].MasterAmountCode  (nolock)ON [dbo].ChitScheme.AmountCodeID = [dbo].MasterAmountCode.AmountCodeID
join (TransactionBookVerification bv(nolock) join MasterType mt3(nolock) on bv.VerificationFindingTypeID = mt3.TypeID)
		on tcr.CollectionRecieptID = bv.ReferenceCollectionReceiptID
where ChitSubscriptionCode=@Code and
BookEntryDate is not null
--tcr.CollectionDateTime is not null and mt.Type is not null 
and tcl.StatusTypeID in (1035,1036) 

Insert into @TableMaster 
select  CONVERT(varchar,tp.PaymentDateTime, 106),mt.Type,
--tcr.PrintRecepitCode,
--(case when mt.Type ='Transfer' then 'Source A/c: '+ isnull(TargetProductCode,'') else'' end) +' '+ 
CASE WHEN mt1.Type in('Prize', 'Agent Commission', 'Prize Transfer', 'Discount', 'Company', 'Foreman Commission', 'Dividend Reversal', 'Cancellation', 'Charges', 'Interest') then '' Else isnull(tcr.PrintRecepitCode,'') End,
mt1.Type+ case when mt3.Type is null then '' else '['+mt3.Type + ' ' + cast (CollectionReceiptOldAmount-BookAmount as varchar(20))+']' end,
substring(tpl.remarks,1,200)+
case when tpl.chequeno is not null then 'Chq. No: ' + tpl.chequeno + ' Dtd: '+ convert(varchar,tpl.chequedate,106)+ ' Bnk: '+tpl.chequebankname else '' end,
CASE WHEN mt1.Type='Prize' THEN (Case When AmountCodeConstant > 0 Then tp.PaidAmount Else tp.PaidAmount End) --tp.PaidAmount
	WHEN mt1.Type='Prize Transfer' THEN tp.PaidAmount
	WHEN mt1.Type= 'Agent Commission' THEN tp.PaidAmount	
	WHEN mt1.Type='Foreman Commission' THEN tp.PaidAmount
	WHEN mt1.Type='Discount' THEN tp.PaidAmount
	WHEN mt1.Type='Dividend Reversal' THEN tp.PaidAmount
	WHEN mt1.Type='Cancellation' THEN tp.PaidAmount
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
	0.00,
	case	when isnull(tpl.BkVfcnSts,0) = 0 then 0
			when isnull(tpl.BkVfcnSts,0) = 1 AND DATEDIFF(D,ISNULL(BkVfcnDt,GETDATE()),GETDATE())<=7 then 1 else 2 end,
			tpl.BkVfcnDt,tpl.BkVfcnAmt, 'P~'+cast(tpl.PaymentListID as varchar(50))
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
where ChitSubscriptionCode=@Code and tp.PaymentDateTime is not null and mt.Type is not null and tpl.StatusTypeID in (1035,1036)
AND ProductTypeTypeID = 29
--GROUP  BY 
--CONVERT(varchar,tp.PaymentDateTime, 106),tcr.PrintRecepitCode,mt.Type,tp.PaidAmount,mt1.Type
--order by CONVERT(varchar,tp.PaymentDateTime, 106) asc

select @BookVerifiedDate = isnull( CONVERT(varchar,(max(BkVfcnDt)), 106),'')
from @TableMaster
Insert into @StatusMaster(SDesc, Amount)
select 'Last Book Verified', @BookVerifiedDate
update @TableMaster
set BkVfcnDt = getdate()
where BkVfcnDt is null

---- Deposit

--Insert into @TableMaster 
--select CONVERT(varchar, tcr.CollectionDateTime, 106),mt.Type, --tcr.PrintRecepitCode,
--isnull(tcr.PrintRecepitCode,'') + 
--   (case when mt.Type ='Transfer' then 'Source A/c: '+ TargetProductCode else'' end) ,
--mt1.Type+ case when mt3.Type is null then '' else '['+mt3.Type + ' ' + cast (CollectionReceiptOldAmount-BookAmount as varchar(20))+']' end,
--substring(tcl.remarks,1,200),
--CASE
--	WHEN mt1.Type='Payment' THEN tcr.CollectedAmount
--	WHEN mt1.Type='Payment Transfer' THEN tcr.CollectedAmount
--	WHEN mt1.Type='Charges' THEN tcr.CollectedAmount
--	ELSE 0
--	END AS Debit,
--CASE
--	WHEN mt1.Type='Collection' THEN tcr.CollectedAmount
--	ELSE 0
--	END	AS Credit,
--	0.00 
----tcr.CollectedAmount,'',0.00 
--from [DepositAccount] ls 
--left outer join [dbo].[TransactionCollectionList] tcl on tcl.ProductID=ls.DepositAccountID
--left outer join [dbo].PaymentTransferLog pymt on tcl.CollectionListID=pymt.CollectionListID  and substring(TargetProductCode ,1,1) in ('C','L','D')
--left outer  join [dbo].[TransactionCollectionReciepts] tcr on tcr.CollectionListID = tcl.CollectionListID
--left outer join [dbo].[MasterType]  mt on tcl.TransactionModeTypeID = mt.TypeID
--left outer join [dbo].[MasterType]  mt1 on tcl.TransactionCategoryTypeID = mt1.TypeID
--left outer join (TransactionBookVerification bv(nolock) join MasterType mt3(nolock) on bv.VerificationFindingTypeID = mt3.TypeID)
--		on tcr.CollectionRecieptID = bv.ReferenceCollectionReceiptID
--where ls.DepositAccountCode=@Code and  tcr.CollectionDateTime is not null and mt.Type is not null and tcl.StatusTypeID in (1035,1036)
--GROUP  BY CONVERT(varchar, tcr.CollectionDateTime, 106), tcr.PrintRecepitCode,mt.Type,tcr.CollectedAmount,mt1.Type,mt3.Type,CollectionReceiptOldAmount-BookAmount, TargetProductCode

----order by CONVERT(varchar, tcr.CollectionDateTime, 106) asc


--Insert into @TableMaster 
--select CONVERT(varchar,tp.PaymentDateTime, 106),mt.Type,tcr.PrintRecepitCode,mt1.Type+ case when mt3.Type is null then '' else '['+mt3.Type + ' ' + cast (CollectionReceiptOldAmount-BookAmount as varchar(20))+']' end,
--substring(tpl.remarks,1,200),
--CASE
--	WHEN mt1.Type='Payment' THEN tp.PaidAmount
--	WHEN mt1.Type='Payment Transfer' THEN tp.PaidAmount
--	WHEN mt1.Type='Charges' THEN tp.PaidAmount
--	ELSE 0
--	END AS Debit,
--CASE
--	WHEN mt1.Type='Collection' THEN tp.PaidAmount
--	ELSE 0
--	END AS Credit, 0.00  
--from [DepositAccount] ls
----left outer join [dbo].[TransactionCollectionList] tcl on tcl.ProductID = ls.DepositAccountID 
--left outer join [dbo].[TransactionPaymentList] tpl on tpl.ProductID = ls.DepositAccountID
--left outer join [dbo].[TransactionPayment] tp on tp.PaymentListID =tpl.PaymentListID
--left outer join [dbo].[TransactionCollectionReciepts] tcr on tcr.CollectionListID = tp.PaymentListID
--left outer join [dbo].[MasterType]  mt on tpl.TransactionModeTypeID = mt.TypeID
--left outer join [dbo].[MasterType]  mt1 on tpl.TransactionCategoryTypeID = mt1.TypeID
--left outer join (TransactionBookVerification bv(nolock) join MasterType mt3(nolock) on bv.VerificationFindingTypeID = mt3.TypeID)
--		on tcr.CollectionRecieptID = bv.ReferenceCollectionReceiptID
--where DepositAccountCode=@Code and tp.PaymentDateTime is not null and mt.Type is not null and tpl.StatusTypeID in (1035,1036)
--GROUP  BY 
--CONVERT(varchar,tp.PaymentDateTime, 106),tcr.PrintRecepitCode,mt.Type,tp.PaidAmount,mt1.Type,mt3.Type,CollectionReceiptOldAmount-BookAmount
----order by CONVERT(varchar,tp.PaymentDateTime, 106) asc

begin


declare @ChitDuration int, @ChitDurationinmonths numeric(18,2),
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
	--	cast('' as  varchar(100)) as Dividend_Sts
		
	--return

	set @StartDate = dateadd(m,-1,@StartDate)
	SET @CHITENDDT= dateadd(m,@ChitDurationinmonths,@StartDate)
	SET @NTEMPTERM = 1
	SET @TEMPINSTNO =1
	SET @TMPSTDT = @StartDate

	insert into @ChitCollectionPattern
	select @ChitSubsID,ChitInstalmentNo, @StartDate, @chitsubamt*@TicketNumber,
	DividentAmount*@TicketNumber, DiscountAmount*@TicketNumber, 0, (@ChitValue-DiscountAmount)*@TicketNumber
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
	 --IF(@chitcolinttypeid =775)
	 BEGIN
		 insert into @chitschemeduedivamtdtl
		 select subscriptioncode , ROW_NUMBER() over (order by stdt) 'InstNo', --cs2.cschemeid,
		 stdt,dateadd(m,cs1.chitinstalmentno,stdt),
		 (cs3.SuggestedAmount*@TicketNumber),(cs1.DividentAmount*@TicketNumber)	,(cs1.DiscountAmount*@TicketNumber)	, 
		 (cs3.SuggestedAmount*@TicketNumber)-(cs1.DividentAmount*@TicketNumber)	 
		 from ChitSchemePattern cs1 (nolock) 
		 join ChitSchemeCollection cs3 on cs1.ChitSchemeID = cs3.ChitSchemeID and cs3.CollectionIntervalTypeID = 775
		 join @chitschemedtl cs2 on cs1.ChitSchemeID = cs2.cschemeid
		 --where dateadd(m,cs1.chitinstalmentno,stdt) <= getdate()
		 --or (month(dateadd(m,cs1.chitinstalmentno,stdt) ) = month(getdate())
			--	and year(dateadd(m,cs1.chitinstalmentno,stdt) ) = year(getdate()))
		 --group by subscriptioncode, stdt, cs1.chitinstalmentno
	END

	----SELECT * FROM @chitschemeduedivamtdtl
	--IF(@chitcolinttypeid =773)
	--BEGIN
	--	--SELECT 'TEST', @TMPSTDT, @CHITENDDT
	--	WHILE (@TMPSTDT < @CHITENDDT)
	--	BEGIN
			
	--		IF (dateadd(m,@TEMPINSTNO,@STARTDATE) = @TMPSTDT)
	--			SELECT @TEMPINSTNO = @TEMPINSTNO + 1
	--		SELECT @TEMPINSTNO = ChitInstalmentNo, @TEMPDIVAMT = DividentAmount, @TEMPDISCAMT = DiscountAmount
	--		FROM CHITSCHEMEPATTERN (NOLOCK)
	--		WHERE CHITSCHEMEID = @chitschemeid
	--		AND ChitInstalmentNo = @TEMPINSTNO
	--		--SELECT dateadd(m,@TEMPINSTNO,@STARTDATE), @TMPSTDT, @TEMPINSTNO, @TEMPDIVAMT
	--		IF NOT EXISTS (SELECT '*' FROM @chitschemeduedivamtdtl WHERE emidt = @TMPSTDT)
	--		BEGIN
	--			insert into @chitschemeduedivamtdtl
	--			 select subscriptioncode , @TEMPINSTNO 'InstNo', --cs2.cschemeid,
	--			 stdt,@TMPSTDT,
	--			 (cs3.SuggestedAmount*@TicketNumber),((@TEMPDIVAMT/30)*@TicketNumber)	, (@TEMPDISCAMT/30 *@TicketNumber)	, 
	--			 (cs3.SuggestedAmount*@TicketNumber)-((@TEMPDIVAMT/30)*@TicketNumber)	 
	--			 from ChitSchemeCollection cs3 (NOLOCK)
	--			 join @chitschemedtl cs2 on cs3.ChitSchemeID = cs2.cschemeid
	--			 AND cs3.CollectionIntervalTypeID = @chitcolinttypeid
	--		END
	--		SET @NTEMPTERM = @NTEMPTERM + 1 
	--		SET @TMPSTDT = DATEADD(D,1,@TMPSTDT)
	--		--SELECT @TMPSTDT 
	--	END
	--END
	 
	-- declare @instcnt int
	-- set @instcnt =1
	-- --SELECT * FROM @chitschemeduedivamtdtl
	--IF(@chitcolinttypeid =774)
	--BEGIN
	--	--SELECT 'TEST', @TMPSTDT, @CHITENDDT
	--	WHILE (@TMPSTDT < @CHITENDDT)
	--	BEGIN
	--		declare @mn1 int, @yr1 int, @mn2 int, @yr2 int, @mnyrmn1 int , @mnyrmn2 int
	--		--select dateadd(m,@TEMPINSTNO,@STARTDATE),@TMPSTDT
	--		select	@mn1=month(dateadd(m,@TEMPINSTNO+1,@STARTDATE)), @mn2= month(@TMPSTDT),
	--				@yr1=month(dateadd(m,@TEMPINSTNO+1,@STARTDATE)), @yr2= month(@TMPSTDT)
	--		select @mnyrmn1 = @yr1 + @mn1
	--		select @mnyrmn2 = @yr2 + @mn2			
	--		IF (dateadd(m,@TEMPINSTNO,@STARTDATE) < @TMPSTDT)
	--			SELECT @TEMPINSTNO = @TEMPINSTNO + 1
	--		SELECT @TEMPINSTNO = ChitInstalmentNo, @TEMPDIVAMT = DividentAmount, @TEMPDISCAMT = DiscountAmount
	--		FROM CHITSCHEMEPATTERN (NOLOCK)
	--		WHERE CHITSCHEMEID = @chitschemeid
	--		AND ChitInstalmentNo = @TEMPINSTNO
	--		--SELECT dateadd(m,@TEMPINSTNO,@STARTDATE), @TMPSTDT, @TEMPINSTNO, @TEMPDIVAMT
	--		IF NOT EXISTS (SELECT '*' FROM @chitschemeduedivamtdtl WHERE emidt = @TMPSTDT)
	--		BEGIN
	--			insert into @chitschemeduedivamtdtl
	--			 select subscriptioncode , @TEMPINSTNO 'InstNo', --cs2.cschemeid,
	--			 stdt,@TMPSTDT,
	--			 (cs3.SuggestedAmount*@TicketNumber),((@TEMPDIVAMT/30*7)*@TicketNumber)	, (@TEMPDISCAMT/30*7 *@TicketNumber)	, 
	--			 (cs3.SuggestedAmount*@TicketNumber)-((@TEMPDIVAMT/30*7)*@TicketNumber)	 
	--			 from ChitSchemeCollection cs3 (NOLOCK)
	--			 join @chitschemedtl cs2 on cs3.ChitSchemeID = cs2.cschemeid
	--			 AND cs3.CollectionIntervalTypeID = @chitcolinttypeid
	--		END
	--		SET @NTEMPTERM = @NTEMPTERM + 7 
	--		SET @TMPSTDT = DATEADD(D,7,@TMPSTDT)
	--		set @instcnt = @instcnt + 1
	--		--SELECT @TMPSTDT 
	--	END
	--END
	 
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
	DIVAMT, DISCAMT, 0, (@ChitValue-DISCAMT)*@TicketNumber
	from @chitschemeduedivamtdtlfinal


	 update A
	set ToBePaid_Amt = b.balamt
	from @ChitCollectionPattern  A, @chitschemeduedivamtdtlfinal b
	where a.InstNo = b.InstNo
	AND INSTDT = B.EMIDT

--select * from @ChitCollectionPattern
--select * from @chitschemeduedivamtdtlfinal

 


select  convert(varchar(10), date, 120) as Date, convert(varchar(10), date, 103) as TDate,   Mode, (Case When Mode = 'Transfer' and ReceiptNo not like 'Source%' then '' Else ReceiptNo End) as ReceiptNo,
Category, Remarks, Debit, Credit, Date [trdt],  cast(0.0 as numeric (18,2)) as Due_Amt, Cast(0.0 as numeric(18,2)) as Div_Amt ,
--SUM((CAST(credit AS int) - CAST(debit AS int))) OVER (ORDER BY date ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW)
SUM((CAST(credit AS decimal(18,2)) - (Case When  Category like '%subscription%paid%' or  Category in('Charges',  'Dividend Reversal', 'Cancellation', 'Prize Transfer','Foreman Commission','Prize111') 
then CAST(debit AS decimal(18,2)) Else 0 End))) 
OVER (ORDER BY date,debit,credit ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW)
--sum(cast(credit as decimal(18,2)))+ (case when category in('Interest') then CAST (debit AS decimal(18,2)) Else 0 End)OVER (ORDER BY date ROWS BETWEEN UNBOUNDED 
--PRECEDING AND CURRENT ROW)
--SUM((CAST(credit AS decimal(18,2)))) OVER (ORDER BY date ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW)
AS Cumilative, BkVfcnSts, BkVfcnDt, BkVfcnAmt,TranID
INTO #TMP
from @TableMaster 	
union
select convert(varchar(10), InstDt, 120) ,convert(varchar(10), InstDt, 103) , '','Schedule','Due/Div Amt','',0,0,instdt,Subs_Amount , Div_Amt ,0,0,null,0,''
from @ChitCollectionPattern
where InstDt<=getdate()
and isnull(@CommencementDt,'') <> ''
	order by DATE, CATEGORY --,CONVERT(DateTime, Date,101)


end

--select 'rajesh', * from #TMP


-- exec [dbo].[CustomerAccountLedgerbyPcode]   C16735

SELECT TDate, Mode, --ReceiptNo,
Category, Remarks, Due_Amt-Div_Amt 'Due_Amt', Div_Amt, Debit, Credit, Cumilative, BkVfcnSts, BkVfcnDt, BkVfcnAmt,TranID FROM #TMP
where Category !='Agent Commission'

--SELECT '','','','Total', Sum(CAST (Debit AS int)) AS Total ,SUM(CAST (Credit AS int)) AS [value], SUM(CAST (Credit AS INT)) - SUM(CAST (Debit AS INT)) AS [BalanceTotal]

--            FROM  #TMP

SELECT '','',--'',
'','Total',
sum(Due_Amt-Div_Amt) as Due_Amt, Sum(Div_Amt) as Div_Amt,
Sum(CAST (Debit AS decimal(18,2))) AS Total ,SUM(CAST (Credit AS decimal(18,2))) AS [value], SUM(CAST(Credit AS decimal(18,2)) -
(Case When Category like '%subscription%paid%' or Category in('Charges',  'Dividend Reversal', 'Cancellation', 'Prize Transfer','Foreman Commission','Prize111') then CAST(debit AS int) Else 0 End)) AS [BalanceTotal]
FROM  #TMP
where Category !='Agent Commission'

IF EXISTS (SELECT 1 FROM LoanSubscription WHERE LoanSubscriptionCode=@Code)
BEGIN
	select LoanAmount as [value],TallyCode from LoanSubscription where LoanSubscriptionCode=@Code
end


IF EXISTS (SELECT 1 FROM chitsubscription WHERE ChitSubscriptionCode=@Code)
BEGIN
	select CAST(ChitScheme.ChitValue AS decimal(18,2)) AS ChitValue,TicketNumber, ChitGroupCode, @chitscheme 'chitscheme', 
	ClientName, @address 'ClientAddress'
	from dbo.chitsubscription 
	inner join dbo.ChitGroup cg on cg.ChitGroupID = chitsubscription.ChitGroupID
             INNER JOIN dbo.clientmaster ON dbo.chitsubscription.personid = dbo.clientmaster.clientid
             INNER JOIN dbo.chitscheme ON dbo.chitsubscription.chitschemeid = dbo.chitscheme.chitschemeid LEFT OUTER JOIN
                         dbo.Organization ON dbo.ChitSubscription.OrganizationID = dbo.Organization.OrganizationID
	where chitsubscription.ChitSubscriptionCode=@Code

end
IF EXISTS (SELECT 1 FROM [DepositAccount] WHERE DepositAccountCode=@Code)
BEGIN
	select 0.00,TallyCode from [DepositAccount] ls where  DepositAccountCode=@Code
end
select @LastReceiptDate = Convert(varchar,max([trdt]),103), @LastReceiptDt = max([trdt]) from #tmp
where Category = 'Subscription'

select @LastPaymentDate = Convert(varchar,max(case when  mt1.Type='Prize' then tp.paymentdatetime else null end),103), 
@LastPaymentDt = max(case when  mt1.Type='Prize' then tp.paymentdatetime else null end) 
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
where ChitSubscriptionCode=@Code and tp.PaymentDateTime is not null and mt.Type is not null and tpl.StatusTypeID in (1035,1036)
AND ProductTypeTypeID = 29


--select *from #tmp
--where Category = 'Subscription'

Insert into @StatusMaster(SDesc, Amount)
select 'Last Receipt Date', @LastReceiptDate
Insert into @StatusMaster(SDesc, Amount)
select 'Last Payment Date', @LastPaymentDate


insert into @CategoryMaster
select 1, 'Due_Amount' 'Category',isnull(sum(subs_amount-div_amt),0) 'Totals'
from @ChitCollectionPattern
where InstDt<=GETDATE()
union
select 2, 'Dividend' 'Category',isnull(sum(div_amt),0) 'Totals'
from @ChitCollectionPattern
where InstDt<=GETDATE()
union
SELECT 3, 'Dividend Paid', isnull(sum(Credit),0) FROM #TMP
where Category like '%Dividend%'


--SELECT * FROM #TMP

--SELECT 3, 'Dividend Paid', isnull(sum(Debit),0) FROM #TMP
--where Category like '%Dividend%'


insert into @CategoryMaster
select 4, case 
--when category like '%Paid (%' and @Code like 'l%' then 'Principal Repayment'
--when category like '%Paid (%' and @Code like 'D%' then 'Collection'
when category like '%Subscription%paid%' 
--and @Code like 'C%' 
then 'Subscription'
else category end as Category,  abs(SUM((CAST(credit AS decimal(18,2)) - CAST(debit AS decimal(18,2))))) as Totals 
from #TMP 
where category not like '%dividend%'
and category not like '%due/div%'
GROUP  BY case
--when category like '%Paid (%' and @Code like 'l%' then 'Principal Repayment'
--when category like '%Paid (%' and @Code like 'D%' then 'Collection'
when category like '%Subscription%paid%' 
--and @Code like 'C%' 
then 'Subscription'
else category end


insert into @CategoryMaster
select 5, 'Over-Due', isnull(a.Totals,0) - isnull(b.Totals,0)
from @CategoryMaster a, @CategoryMaster b
where a.ord =1 and b.ord = 4
and b.Category = 'Subscription'


insert into @CategoryMaster
select 6, 'Passbook Total', isnull(a.Totals,0) + isnull(b.Totals,0)
from @CategoryMaster a, @CategoryMaster b
where a.ord =3 and b.ord = 4
and b.Category = 'Subscription'

select cast (ord as varchar) + '.'+ Category Category, Totals
from @CategoryMaster
order by 1


--Summary
IF EXISTS (SELECT 1 FROM chitsubscription WHERE ChitSubscriptionCode=@Code)
BEGIN
	 Insert into @SummaryMaster(SDesc,Amount)
	 Values('Chit Value', @ChitValue*@TicketNumber)
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
  --          INNER JOIN dbo.person ON dbo.chitsubscription.personid = dbo.person.personid 
  --           INNER JOIN dbo.chitscheme ON dbo.chitsubscription.chitschemeid = 
  --                      dbo.chitscheme.chitschemeid where chitsubscription.ChitSubscriptionCode=@Code
	Insert into @SummaryMaster(SDesc,Amount)
	select 'Tickets Paid', isnull(TicketPaidNumber,0) from dbo.chitsubscription 
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
	--SELECT @Amt = Sum(Case When Category in('Subscription', 'Dividend') Then Credit Else 0 End) FROM #TMP
		SELECT @Amt = Sum(Case When Category in('Subscription', 'Dividend', 'Waiver') Then
		 Credit Else (Case When Category in('Charges', 'Interest') Then Debit * -1 Else 0 End) End) FROM #TMP
	else
		SELECT @Amt = Sum(Case When Category in('Subscription', 'Dividend', 'Waiver') Then Credit Else
		 (Case When Category in('Foreman Commission', 'Dividend Reversal', 'Cancellation', 'Charges', 'Prize Transfer', 'Prize', 'Interest') Then Debit * -1 Else 0 End) End) FROM #TMP

--select 'rajesh', @SDesc,@BalAmt , ( @TicketNumber * @ChitValue) ,@Amt  

	--Set @Amt = ISNULL(@Amt, 0)
	--if (@SDesc = 'Created' Or @SDesc = 'Active Approval' Or @SDesc = 'Active' Or @SDesc = 'Payment Due' Or @SDesc = 'Modification Approval' Or @SDesc = 'Modification' Or @SDesc = 'Legal' Or @SDesc = 'Arrear')
	--	Set @BalAmt = @Amt - (@TicketNumber * @ChitValue)
	--else
	--	Set @BalAmt = @Amt
	Set @BalAmt = ( @TicketNumber * @ChitValue) -@Amt  
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
	--  INNER JOIN dbo.person ON dbo.chitsubscription.personid = dbo.person.personid 
	--           INNER JOIN dbo.chitscheme ON dbo.chitsubscription.chitschemeid = dbo.chitscheme.chitschemeid 
	--	 INNER JOIN dbo.MasterType ON dbo.chitsubscription.StatusTypeID = dbo.MasterType.TypeID 
	--	 where chitsubscription.ChitSubscriptionCode=@Code



end

--Select @Amt
--Select @BalAmt
Select SDesc AS Summary, Amount as Value From @SummaryMaster
Select SDesc AS Status, Amount as Type From @StatusMaster

--Insert Balance in Temp Table
Declare @ZPaidAmt numeric(18,2), @AValue numeric(18,2), @BalValue numeric(18,2), @PrizeAmt numeric(18,2), @ACAmt numeric(18,2)


select @ZPaidAmt = SUM((CAST(credit AS decimal(18,2)) + CAST(debit AS decimal(18,2))))  from #TMP 
Where Category in('Company','Dividend','Subscription')

select @AValue = Sum(Amount)  from @SummaryMaster 
Where SDesc in('Chit Value')

Select @BalValue = Sum(Amount)  From @SummaryMaster
Where SDesc in('Balance')

select @PrizeAmt = Sum(Debit + Credit)  from #TMP 
Where Category in('Prize')

select @ACAmt = Sum(Debit + Credit)  from #TMP 
Where Category in('Agent Commission')

Set @ZPaidAmt = ISNULL(@ZPaidAmt,0)

Set @AValue = ISNULL(@AValue,0)

Set @BalValue = ISNULL(@BalValue,0)

Set @PrizeAmt = ISNULL(@PrizeAmt,0)

begin
	Insert into Tmp_Ledger_Balance_Detail(LCode, PRAmt, PaidAmt, BalAmt, PrizeAmt,LastReceiptDt)
	Values(@Code, @AValue, @ZPaidAmt, abs(@BalValue), @PrizeAmt, @LastReceiptDt)
	
	update a
	set ChitAsonDueAmt = isnull(b.Totals,0)
	from Tmp_Ledger_Balance_Detail a,  @CategoryMaster b
	where b.Category = 'Due_Amount'
	and LCode = @Code

	update a
	set ChitAsonDivAmt = isnull(b.Totals,0)
	from Tmp_Ledger_Balance_Detail a,  @CategoryMaster b
	where b.Category = 'Dividend'
	and LCode = @Code

	
	update a
	set ChitAsonPaidDivAmt = isnull(b.Totals,0)
	from Tmp_Ledger_Balance_Detail a,  @CategoryMaster b
	where b.Category = 'Dividend Paid'
	and LCode = @Code


	update a
	set ChitAsonPaidDueAmt = isnull(b.Totals,0)
	from Tmp_Ledger_Balance_Detail a,  @CategoryMaster b
	where b.Category = 'Subscription'
	and LCode = @Code

	update a
	set OverDueAmt = isnull(b.Totals,0) - isnull(c.Totals,0)
	from Tmp_Ledger_Balance_Detail a,  @CategoryMaster b, @CategoryMaster c
	where b.ord =1 and c.ord = 4
	and c.Category = 'Subscription'
	and LCode = @Code
		
	update a1
	set PassbookTotal = isnull(a.Totals,0) + isnull(b.Totals,0)
	from Tmp_Ledger_Balance_Detail a1,  @CategoryMaster a, @CategoryMaster b
	where a.ord =3 and b.ord = 4
	and b.Category = 'Subscription'	
	and LCode = @Code
	
	update a
	set LastUpdatedDt = getdate(), Tickets = isnull(@nooftickets,1), BusinessValue = (@ChitValue)*@TicketNumber, LastPaymentDt = @LastPaymentDt
	from Tmp_Ledger_Balance_Detail a
	where LCode = @Code



	Select * From Tmp_Ledger_Balance_Detail where LCode = @Code

end
--SELECT 
--    date, s.debit, s.credit,
--    SUM(CAST(s.credit AS int) -CAST(s.debit AS int)) OVER (ORDER BY s.date
--                                  ROWS BETWEEN UNBOUNDED PRECEDING
--                                           AND CURRENT ROW)
--        AS balance
--FROM
--    @TableMaster AS s
--ORDER BY
--    date ;

DROP TABLE #TMP


end