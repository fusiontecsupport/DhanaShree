create proc pr_Get_QRCode_Loan_Subscription_Dtl
@usrid varchar(50),
@empid int,
@subsid int
as
begin
	
	declare @tbl table
	(
		prodtype int,
		proddesc varchar(50),
		subscriptionid int,
		subscriptioncode varchar(250),
		Principal numeric(18,2),
		Loanvalue numeric(18,2),
		collectionamt numeric(18,2),
		clienttype int,
		clientname varchar(200)
	)

	insert into @tbl
	select 30, 'Loan', c.LoanSubscriptionID, isnull(c.LoanSubscriptionCode ,'') ,  c.Principal, c.LoanAmount 'LoanValue', f.SuggestedAmount, CLIENTTID, CLIENTNAME
	--,LoanInstalmentNo, LoanInstalmentDate, InterestPercentage, InterestAmount, PrincipalPercentage, PrincipalAmount, WaiverPercentage, WaiverAmount, NetCollectionAmount
	from LoanRequest b(nolock)
	join LoanSubscription c(nolock) on b.loanrequestid = c.loanrequestid
	join ClientMaster d(nolock) on b.PersonID= d.CLIENTID		
	left join LoanSubscriptionCollection f(nolock) on c.LoanSubscriptionID= f.LoanSubscriptionID and c.RepaymentIntervalTypeID = f.CollectionIntervalTypeID
	--left join LoanSubscriptionPattern lp(nolock) on c.LoanSubscriptionID= lp.LoanSubscriptionID 
	where c.LoanSubscriptionID=@subsid
	
	select * from @tbl
end

