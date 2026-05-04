create proc pr_Get_Contact_Type
@dval int=0
as
begin

	declare @tmptbl table
	(
		DValue int,
		DText varchar(150)
	)

	insert into @tmptbl
	select CONTTID, CONTTYPEDESC
	from ContactTypeMaster (nolock)

	select * from @tmptbl
	where @dval=0 or DValue=@dval
end