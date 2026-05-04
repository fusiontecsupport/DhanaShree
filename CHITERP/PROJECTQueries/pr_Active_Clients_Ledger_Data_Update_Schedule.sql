-- exec [dbo].[pr_Active_Clients_Ledger_Data_Update_Schedule]   29
-- exec [pr_Active_Clients_Ledger_Data_Update_Schedule] 30
-- exec [pr_Active_Clients_Ledger_Data_Update_Schedule] 0

CREATE PROCEDURE [dbo].[pr_Active_Clients_Ledger_Data_Update_Schedule] 
AS 
BEGIN 

	set nocount on 
	Declare @actprd table
		(
		 prdtyp int,
		 pcode varchar(50)
		 )
	Declare @actprdfin table
		(
		 prdtyp int,
		 pcode varchar(50)
		 )

	insert into @actprd
	select distinct ProdTypeID, SubscriptionCode
	from VW_Active_Subscription_Details v(nolock) join TransactionPaymentList p(nolock) on v.SubscriptionID = p.ProductID and v.ProdTypeID =ProductTypeTypeID 
	--where DATEDIFF(d,GenerratedDate, getdate()) <= 32 

	insert into @actprd
	select distinct ProdTypeID, SubscriptionCode
	from VW_Active_Subscription_Details v(nolock) join TransactionCollectionList c(nolock) on v.SubscriptionID = c.ProductID and v.ProdTypeID =c.ProductTypeID 
	--where DATEDIFF(d,GeneratedDate, getdate()) <= 32
	
	insert into @actprdfin
	select distinct prdtyp, pcode from @actprd

	declare @code varchar(100), @prdtyp int
	while exists(select '*' from @actprdfin)
	begin
		select top 1 @code = pcode, @prdtyp= prdtyp from @actprdfin
		if @prdtyp = 29
			exec CustomerAccountLedgerbyPcode @code
		else if @prdtyp = 30
			exec CustomerAccountLedgerbyPcode_Loan @code
		else if @prdtyp = 31
			exec CustomerAccountLedgerbyPcode_Deposit @code

		delete @actprdfin where pcode = @code
	end
end



