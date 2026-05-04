USE [DSFinFusion]
GO

/****** Object:  View [dbo].[VW_Active_Subscription_Details]    Script Date: 20-02-2023 13:12:08 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- select * from VW_Active_Subscription_Details where SuggestedAmount = CollectionAmount
ALTER view [dbo].[VW_Active_Subscription_Details]
as 
SELECT  'Chit' as 'Product', SubscriptionID, ChitSubscriptionCode as SubscriptionCode,  cs.ChitSchemeCode as 'SchemeCode',
		b.Type as 'Status',
		a.personid AS [Customer ID],  p.CLIENTNAME AS [Customer Name], 29 ProdTypeID,
		TicketPaidNumber,  coltyp.Type as 'Payment_Type', mdtyp.Type as 'Payment_Mode', 
		a.CommencementDate 'CommencementDt', StartDate 'ColnStDt', EndDate 'ColnEdDt', cs.chitvalue 'Value' , LastReceiptDt,
		L.BalAmt, L.PrizeAmt, a.ChitGroupID 'GroupID', cg.ChitGroupName 'GroupName',	a.FundAccountID 'FAcID', fa.AccountName 'FAcName',
		a.RouteID 'RouteID', RouteName, BookPickupDay, a.CollectionIntervalTypeID, c.type 'CollectionIntervalDesc',
		ChitDuration 'Duration', DurationTypeId 'DurationTypeId', d.type 'DurationTypeDesc', cp.SuggestedAmount,
		cp.CollectionAmount, a.TicketNumber 'Tickets', a.ChitSchemeId 'SchemeID', ca1.catename 'ColAgt1', ca2.catename 'ColAgt2', a.CollectionAgentPersonID1,
		a.CollectionAgentPersonID2, case when isnull(a.TicketNumber,'') =''  then 1 else a.TicketNumber* cs.chitvalue end 'BusinessValue'
FROM [ChitSubscription] a(nolock) 
					join MasterType b(nolock) on StatusTypeID = b.TypeID
					INNER JOIN dbo.ChitScheme cs(nolock) ON a.ChitSchemeID = cs.ChitSchemeID	
					inner join ChitSchemeCollection cp on a.chitschemeid = cp.chitschemeid
						and a.CollectionIntervalTypeID = cp.CollectionIntervalTypeID
					LEFT OUTER JOIN dbo.clientmaster p(nolock)
                          ON a.personid = p.CLIENTID 
					left join MasterType mdtyp on CollectionModeTypeID = mdtyp.TypeID
					left join MasterType coltyp on CollectionTypeID = coltyp.TypeID
					left join Tmp_Ledger_Balance_Detail l(nolock) on a.ChitSubscriptionCode= l.LCode
					left join mastertype c(nolock) on a.CollectionIntervalTypeID = c.TypeID
					left join CompanyFundAccount fa(nolock) on a.FundAccountID = fa.FundAccountID
					left join ChitGroup cg(nolock) on a.ChitGroupID = cg.ChitGroupID
					left join CompanyRoute cr(nolock) on a.RouteID = cr.RouteID
					left join mastertype d(nolock) on DurationTypeId = d.TypeID
					left join employeemaster ca1 on a.CollectionAgentPersonID1 = ca1.cateid
					left join employeemaster ca2 on a.CollectionAgentPersonID2 = ca2.cateid
where a.StatusTypeID = 881 --879 Created
union all

SELECT  'Deposit' as 'Product', DepositAccountID as SubscriptionID, DepositAccountCode as SubscriptionCode, ds.SchemeCode, b.Type, 
			a.personid AS [Customer ID],  p.CLIENTNAME AS [Customer Name], 31 ProdTypeID,
			1,  coltyp.Type as 'Payment_Type', mdtyp.Type as 'Payment_Mode', 
		AccountOpeningDate 'CommencementDt', AccountOpeningDate 'ColnStDt', AccountOpeningDate 'ColnEdDt', a.DepositOpeningAmount 'Value', LastReceiptDt,
		L.BalAmt, L.PrizeAmt,  -1 'GroupID', '' 'GroupName',	a.FundAccountID 'FAcID', fa.AccountName 'FAcName',
		a.RouteID 'RouteID', RouteName, BookPickupDay, CollectionIntervalTypeID, c.type 'CollectionIntervalDesc',
		0 'Duration', 0 'DurationTypeId', ''  'DurationTypeDesc', InstallmentAmount, InstallmentAmount, 0 'Tkts', a.depositschemeid , 
		ca1.catename 'ColAgt1', '' 'ColAgt2', a.AccountEmployeeID , 0,a.DepositOpeningAmount 
FROM [DepositAccount] a(nolock) 
					join MasterType b(nolock) on StatusTypeID = b.TypeID
					INNER JOIN dbo.DepositScheme ds(nolock) ON a.DepositSchemeID = ds.DepositSchemeID
					LEFT OUTER JOIN dbo.clientmaster p(nolock)
                          ON a.personid = p.CLIENTID 			
					left join MasterType mdtyp on CollectionModeTypeID = mdtyp.TypeID
					left join MasterType coltyp on CollectionTypeTypeID = coltyp.TypeID
					left join Tmp_Ledger_Balance_Detail l(nolock) on a.DepositAccountCode= l.LCode
					left join mastertype c(nolock) on CollectionIntervalTypeID = c.TypeID
					left join CompanyFundAccount fa(nolock) on a.FundAccountID = fa.FundAccountID
					left join CompanyRoute cr(nolock) on a.RouteID = cr.RouteID
					left join employeemaster ca1 on a.AccountEmployeeID = ca1.cateid
where a.StatusTypeID = 758 -- 756 --created
union all

SELECT 'Loan' as 'Product', LoanSubscriptionID as SubscriptionID, LoanSubscriptionCode as SubscriptionCode, ls.SchemeCode, b.Type, 
			lr.personid AS [Customer ID],  p.CLIENTNAME AS [Customer Name], 30 ProdTypeID,
			1,  coltyp.Type as 'Payment_Type', mdtyp.Type as 'Payment_Mode', 
		a.DisbursementDate 'CommencementDt', lr.DisbursementDate 'ColnStDt', 
		case when a.TermTypeID =61 then dateadd(d, a.term,lr.DisbursementDate)
			when a.TermTypeID =64 then dateadd(m, a.term,lr.DisbursementDate) else dateadd(m, a.term,lr.DisbursementDate) end 'ColnEdDt'	, a.LoanAmount , LastReceiptDt,
		L.BalAmt, L.PrizeAmt,  -1 'GroupID', '' 'GroupName',	a.DepositAccountID 'FAcID', fa.AccountName 'FAcName',
		a.RouteID 'RouteID', RouteName, BookPickupDay, RepaymentIntervalTypeID, c.type 'CollectionIntervalDesc',
		a.Term 'Duration', a.TermTypeID 'DurationTypeId', d.type 'DurationTypeDesc', a.LoanAmount/a.Term, a.LoanAmount/a.Term, 0 'Tkts', ls.loanschemeid,
		ca1.catename 'ColAgt1', '' 'ColAgt2', lr.AccountEmployeeID 'ColAgt1ID', 0'ColAgt2ID', a.LoanAmount
FROM LoanSubscription a(nolock) 
					join MasterType b(nolock) on StatusTypeID = b.TypeID
					LEFT OUTER JOIN LoanScheme ls(nolock)
						  ON ls.LoanSchemeID = a.LoanSchemeID
             LEFT OUTER JOIN loanrequest lr (nolock)
                          ON a.loanrequestid = 
                             lr.loanrequestid 
             LEFT OUTER JOIN dbo.ClientMaster as p
                          ON lr.personid = p.CLIENTID             
			left join MasterType mdtyp on a.RepaymentModeTypeID = mdtyp.TypeID
			left join MasterType coltyp on a.DeductionTypeID = coltyp.TypeID
			left join Tmp_Ledger_Balance_Detail l(nolock) on a.LoanSubscriptionCode= l.LCode
			left join mastertype c(nolock) on RepaymentIntervalTypeID = c.TypeID
			left join CompanyFundAccount fa(nolock) on a.DepositAccountID = fa.FundAccountID
			left join CompanyRoute cr(nolock) on a.RouteID = cr.RouteID
			left join mastertype d(nolock) on a.TermTypeID = d.TypeID
			left join employeemaster ca1 on lr.AccountEmployeeID = ca1.cateid
where a.StatusTypeID = 968 -- 


GO


