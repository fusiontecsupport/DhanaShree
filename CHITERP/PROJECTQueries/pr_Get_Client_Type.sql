alter proc pr_Get_Client_Type
@dval int=0
as
begin

	declare @tmptbl table
	(
		DValue int,
		DText varchar(150)
	)

	insert into @tmptbl
	select 1, 'Individual'

	insert into @tmptbl
	select 2, 'Company'

	select * from @tmptbl
	where @dval=0 or DValue=@dval
end