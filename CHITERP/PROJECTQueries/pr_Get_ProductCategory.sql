-- exec pr_Get_ProductCategory @usrid='admin',@empid=4,@prod=29, @trncateg='R'
-- exec pr_Get_ProductCategory @usrid='admin',@empid=4,@prod=30, @trncateg='P'
-- exec pr_Get_ProductCategory @usrid='admin',@empid=4,@prod=29, @trncateg='P'
-- exec pr_Get_ProductCategory @usrid='admin',@empid=4,@prod=30, @trncateg='R'
-- exec pr_Get_ProductCategory @usrid='admin',@empid=4,@prod=29,@trncateg='C'
-- exec pr_Get_Master_Types 'Transaction Category Chit'
alter proc pr_Get_ProductCategory
@usrid varchar(50),
@empid int,
@prod int,
@trncateg char(2)='C'
as
begin

	declare @tbl table
	(
		TypeID int,
		[Type] varchar(150)		
	)

	insert into @tbl
	select b.TypeID, b.Type
	from MasterCategory a(nolock) join MasterType b(nolock) on a.categoryid = b.TypeCategoryID
	where 
	(a.Category = 'Transaction Category Loan' 
	and @trncateg = 'C'
	and @prod = 30
	and b.TypeID in ( 1020, 1021, 1022, 1023, 1091, 192, 1093))
	or
	(a.Category = 'Transaction Category Loan' 
	and @trncateg = 'P'
	and @prod = 30
	and b.TypeID in ( 1019, 1077, 1094))
	or 
	(a.Category = 'Transaction Category Chit' 
	and @trncateg = 'C'
	and @prod = 29
	and b.TypeID in ( 857, 1016, 1018, 1076, 1089, 1096))
	or
	(a.Category = 'Transaction Category Chit' 
	and @trncateg = 'P'
	and @prod = 29
	and b.TypeID in (856, 1014, 1078, 1090, 1017 , 1113))
	or 
	(a.Category = 'Transaction Category Deposit' 
	and @trncateg = 'C'
	and @prod = 31
	and b.TypeID in ( 791, 1025, 1053))
	or
	(a.Category = 'Transaction Category Deposit' 
	and @trncateg = 'P'
	and @prod = 31
	and b.TypeID in (1024, 1079, 1095 ))
	--or 
	--(a.Category = 'Transaction Category Employee' 
	--and @trncateg = 'C'
	--and b.TypeID in ( 791, 1025))
	--or
	--(a.Category = 'Transaction Category Employee' 
	--and @trncateg = 'P'
	--and b.TypeID in (1024))

	


	--if(@prod=0 or @prod =29)
	--begin
	--	insert into @tbl
	--	exec pr_Get_Master_Types 'Transaction Category Chit',  @trncateg
	--end

	--if(@prod=0 or @prod =30)
	--begin
	--	insert into @tbl
	--	exec pr_Get_Master_Types 'Transaction Category Loan',  @trncateg
	--end
	
	select * from @tbl
end

