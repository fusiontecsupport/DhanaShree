use finfusion
go
exec pr_Get_Receipt_Details_BV_Assign @bvlistid=15,@bvgendt='2023-04-20 00:00:00',@subsid=406, @prodtypid=29,@bvid=0
SELECT  COUNT(distinct CollectionListID)  
	FROM dbo.TransactionCollectionList a (nolock) 
		--join chitsubscription b(nolock) on a.ProductID = b.SubscriptionID
		--join ClientMaster c(nolock) on b.PersonID = c.CLIENTID
		left join chitsubscription b1(nolock) on a.ProductID = b1.SubscriptionID and ProductTypeID=29		
		left join loansubscription b2(nolock) on a.ProductID = b2.LoanSubscriptionID and ProductTypeID=30
		left join LoanRequest b3(nolock) on b2.LoanRequestID= b3.LoanRequestID
		join ClientMaster c(nolock) on (b1.PersonID = c.CLIENTID or b3.PersonID = c.clientid)
		LEFT JOIN CompanyRoute R1(NOLOCK) ON B1.RouteID = R1.ROUTEID		
		LEFT JOIN CompanyRoute R2(NOLOCK) ON B2.RouteID = R2.ROUTEID
		join MasterType d(nolock) on a.TransactionCategoryTypeID = d.typeid
		join MasterType e(nolock) on a.TransactionTypeTypeID = e.typeid
		join MasterType f(nolock) on a.StatusTypeID = f.typeid	
		left join companyoffice e1(nolock) on b1.LinkedOfficeID = e1.OfficeID or b2.LinkedOfficeId = e1.OfficeID		