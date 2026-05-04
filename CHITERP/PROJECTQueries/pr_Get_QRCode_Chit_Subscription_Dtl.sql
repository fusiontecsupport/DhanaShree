alter proc pr_Get_QRCode_Chit_Subscription_Dtl
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
		chitvalue numeric(18,2),
		collectionamt numeric(18,2),
		clienttype int,
		clientname varchar(200),
		clientid int
	)

	insert into @tbl
	select 29, 'Chit', c.SubscriptionID, isnull(c.ChitSubscriptionCode ,'') ,  ChitValue, 
	case when isnull(f.CollectionAmount,0) = 0 then isnull(f.suggestedamount,0) else f.CollectionAmount end, 
	CLIENTTID, CLIENTNAME, c.PersonID
	from ChitSubscription c(nolock)
	join ClientMaster d(nolock) on c.PersonID= d.CLIENTID	
	join ChitScheme e(nolock) on c.ChitSchemeID = e.ChitSchemeID
	join ChitSchemeCollection f(nolock) on e.ChitSchemeID= f.ChitSchemeID and c.CollectionIntervalTypeID = f.CollectionIntervalTypeID
	where c.SubscriptionID=@subsid
	
	select * from @tbl
end

