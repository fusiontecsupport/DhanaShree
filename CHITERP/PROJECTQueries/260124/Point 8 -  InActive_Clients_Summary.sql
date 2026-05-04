use DSFinFusion
go
-- exec [dbo].[InActive_Clients_Summary]   29
-- exec [InActive_Clients_Summary] 30
-- exec [InActive_Clients_Summary] 0

create  PROCEDURE [dbo].[InActive_Clients_Summary] 
@product int=0,
@usrid varchar(50)='',
@empid int =0,
@routeid int =0,
@officeid int =0
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
	select 'Ac Status', 'In-Active'	


	if(@routeid = 0)
	begin
		insert into @filtertbl
		select 'Route', 'ALL'
	end
	else
	begin
		insert into @filtertbl
		select 'Route', RouteName
		from CompanyRoute where routeid = @routeid
	end
	if(@officeid = 0)
	begin
		insert into @filtertbl
		select 'Office', 'ALL'
	end
	else
	begin
		insert into @filtertbl
		select 'Office', officename
		from CompanyOffice where officeid = @officeid
	end
	

	select * from @filtertbl
	

	select distinct OfficeName, Product, SubscriptionCode, SchemeCode, [Customer Name], Payment_Type,RouteName,CommencementDt, ColnStDt, ColnEdDt, [Value], LastReceiptDt, BalAmt, PrizeAmt,
	ColAgt1,CmsnAgt1
	from VW_InActive_Subscription_Details a(nolock)
		left join  employeemaster e1(nolock) on a.LinkedOfficeID = e1.LinkedOfficeID 
		and (@usrid = '' or e1.cateid = @empid )
		--join clientmaster c(nolock) on a.[Customer ID] = c.clientid
	where (ProdTypeID = @product or @product = 0)	
	and (RouteID = @routeid or @routeid=0)
	and (a.LinkedOfficeID = @officeid or @officeid=0)
	order by OfficeName, Product, RouteName, ColAgt1, CmsnAgt1 --ColAgt2, 
end