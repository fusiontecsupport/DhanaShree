-- exec [dbo].[Chit_Business_Report]   29
-- exec [Chit_Business_Report] 30
-- exec [Chit_Business_Report] 0

alter PROCEDURE [dbo].[Chit_Business_Report] 
@product int=0,
@FrDate Datetime,
@ToDate Datetime
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
	

	select Product, SubscriptionCode, SchemeCode, [Customer Name], Payment_Type,CommencementDt, ColnStDt, ColnEdDt, BusinessValue, LastReceiptDt, BalAmt, PrizeAmt,
		ColAgt1, ColAgt2
	from VW_Active_Subscription_Details (nolock)
	where (ProdTypeID = @product or @product = 0)
	and commencementdt between @FrDate and @ToDate
	union
	select Product, SubscriptionCode, SchemeCode, [Customer Name], Payment_Type,CommencementDt, ColnStDt, ColnEdDt, BusinessValue, LastReceiptDt, BalAmt, PrizeAmt,
		ColAgt1, ColAgt2
	from VW_InActive_Subscription_Details (nolock)
	where (ProdTypeID = @product or @product = 0)
	and commencementdt between @FrDate and @ToDate
	order by 1, 13,14,6,2
end

