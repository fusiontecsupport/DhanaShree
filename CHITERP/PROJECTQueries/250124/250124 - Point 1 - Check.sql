use DSFinFusion
go
-- Active custmer balance check

	select Product, SubscriptionCode, SchemeCode, [Customer Name], Payment_Type,CommencementDt, ColnStDt, ColnEdDt, [Value], LastReceiptDt, BalAmt, PrizeAmt
	from VW_Active_Subscription_Details (nolock)
	where (ProdTypeID = 29)
	and ColnEdDt < getdate() and BalAmt > 0 


--Case STudy for one customer
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
and ChitSubscriptionCode = 'DSC/DSB2/09'

Select * From Tmp_Ledger_Balance_Detail where LCode = 'DSC/DSB2/09'