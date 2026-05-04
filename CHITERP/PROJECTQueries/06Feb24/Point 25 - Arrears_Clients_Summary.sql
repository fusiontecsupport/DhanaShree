use dsfinfusion
go

-- exec [dbo].[Arrears_Clients_Summary]   29
-- exec [Arrears_Clients_Summary] 30
-- exec [Arrears_Clients_Summary] 0

alter  PROCEDURE [dbo].[Arrears_Clients_Summary] 
@product int=0
AS 
BEGIN 

	set nocount on 
	declare @custname varchar(200), @colagtname varchar(200), @routename varchar(200)

	Declare @filtertbl table
	(
	Filter_Description varchar(100),
	Filter_Value varchar(100)
	)
	
	
	insert into @filtertbl
	select 'Product', case when @product =0 then 'ALL' when @product =29 then 'Chit' 
			when @product =30 then 'Loan' when @product =31 then 'Deposit' Else 'Others' End	
	

	select * from @filtertbl
	

	select Product, SubscriptionCode, SchemeCode, [Customer Name], Payment_Type as CollectionType,RouteName, CommencementDt, ColnStDt, ColnEdDt, [Value], LastReceiptDt, BalAmt, PrizeAmt, OverDueAmt
	from VW_Active_Subscription_Details (nolock)
	where (ProdTypeID = @product or @product = 0)
	and ColnEdDt < getdate() and BalAmt > 0 
	and (StatusTypeId = 974 or StatusTypeId = 884)
	--and OverDueAmt > 0
	union
	select Product, SubscriptionCode, SchemeCode, [Customer Name], Payment_Type,RouteName, CommencementDt, ColnStDt, ColnEdDt, [Value], LastReceiptDt, BalAmt, PrizeAmt, OverDueAmt
	from VW_InActive_Subscription_Details (nolock)
	where (ProdTypeID = @product or @product = 0)
	and ColnEdDt < getdate() and BalAmt > 0 
	and (StatusTypeId = 974 or StatusTypeId = 884)
	--and OverDueAmt > 0
end



