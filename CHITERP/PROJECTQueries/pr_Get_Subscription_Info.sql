alter proc pr_Get_Subscription_Info
@usrid varchar(50),
@empid int,
@prod int
as
begin

	declare @tbl table
	(
		subscriptionid int,
		subscriptioncode varchar(250)		
	)


	if(@prod=0 or @prod =29)
	begin
		insert into @tbl
		select c.SubscriptionID, isnull(c.ChitSubscriptionCode ,'') +' - '+ CLIENTNAME
		from ChitSubscription c(nolock)
			left join ClientMaster d(nolock) on c.PersonID= d.CLIENTID
		where @usrid ='' or @empid = CollectionAgentPersonID1 or @empid = CollectionAgentPersonID2
	end

	if(@prod=0 or @prod =30)
	begin
		insert into @tbl
		select c.LoanSubscriptionID, isnull(c.LoanSubscriptionCode ,'') +' - '+ CLIENTNAME
		from LoanRequest a (nolock) 
			join LoanSubscription c(nolock) on a.LoanRequestID = c.LoanRequestID
			left join ClientMaster d(nolock) on a.PersonID= d.CLIENTID
		where @usrid ='' or a.AccountEmployeeID = @empid
	end
	
	select * from @tbl
end


