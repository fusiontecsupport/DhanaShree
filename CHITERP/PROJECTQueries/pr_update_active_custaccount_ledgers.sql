-- exec pr_update_active_custaccount_ledgers 5790
-- exec pr_update_active_custaccount_ledgers 27293
/*
select * from VW_Active_Subscription_Details
exec pr_update_active_custaccount_ledgers @custid = '32'
*/
alter PROC pr_update_active_custaccount_ledgers
@custid varchar(100)
as
begin
	Declare @actprd table
		(
		 prdtyp int,
		 pcode varchar(50)
		 )

	insert into @actprd
	select ProdTypeID, SubscriptionCode
	from VW_Active_Subscription_Details (nolock)
	where [Customer ID] = @custid

	insert into @actprd
	select ProdTypeID, SubscriptionCode
	from VW_Active_Subscription_Details a(nolock) join ClientMaster c(nolock) on a.[Customer ID]= c.clientid
	where c.refclientid = @custid

	insert into @actprd
	select ProdTypeID,SubscriptionCode
	from VW_Active_Subscription_Details a(nolock) join ClientMaster c(nolock) on a.[Customer ID]= c.refclientid
	where c.clientid = @custid

	declare @code varchar(100), @prdtyp int
	while exists(select '*' from @actprd)
	begin
		select top 1 @code = pcode, @prdtyp= prdtyp from @actprd
		if @prdtyp = 29
			exec CustomerAccountLedgerbyPcode @code
		else if @prdtyp = 30
			exec CustomerAccountLedgerbyPcode_Loan @code
		else if @prdtyp = 31
			exec CustomerAccountLedgerbyPcode_Deposit @code

		delete @actprd where pcode = @code
	end
end