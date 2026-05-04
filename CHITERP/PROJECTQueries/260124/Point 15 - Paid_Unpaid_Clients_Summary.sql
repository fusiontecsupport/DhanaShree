USE DSFinFusion
GO
-- exec [dbo].[Paid_Unpaid_Clients_Summary]   29
-- exec [Paid_Unpaid_Clients_Summary] 30
-- exec [Paid_Unpaid_Clients_Summary] 0

ALTER PROCEDURE [dbo].[Paid_Unpaid_Clients_Summary] 
@product int=0 ,
@sts int=0 ,
@LinkedOfficeId int = 0
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
	
	 
	insert into @filtertbl          
	select 'Prize Status', case when @sts =0 then 'Both' when @sts = 1 then 'Paid'           
	when @sts = 2 then 'UnPaid'  Else 'Others' End           
          

	select * from @filtertbl
	

	select Product, SubscriptionCode, SchemeCode, [Customer Name], Payment_Type,RouteName, CommencementDt, ColnStDt, ColnEdDt, BusinessValue, LastReceiptDt, BalAmt, PrizeAmt,
		Prize_Paid_Sts = case when  PrizeAmt > 0 then 'Paid' Else 'Unpaid' end, ColAgt1, ColAgt2
	from VW_Active_Subscription_Details (nolock)
	where (ProdTypeID = @product or @product = 0)
	AND (@sts = 0 or (@sts =1 and PrizeAmt > 0)           
	or (@sts =2 and PrizeAmt = 0))           
	and (StatusTypeID = 881)
	--union
	--select Product, SubscriptionCode, SchemeCode, [Customer Name], Payment_Type,RouteName, CommencementDt, ColnStDt, ColnEdDt, BusinessValue, LastReceiptDt, BalAmt, PrizeAmt,
	--	Prize_Paid_Sts = case when  PrizeAmt > 0 then 'Paid' Else 'Unpaid' end, ColAgt1, ColAgt2
	--from VW_InActive_Subscription_Details (nolock)
	--where (ProdTypeID = @product or @product = 0)
	--AND (@sts = 0 or (@sts =1 and and PrizeAmt > 0)           
	--or (@sts =2 and and PrizeAmt = 0))           
	--and (StatusTypeID = 881)
	order by 1, 13,14,15,2
end


