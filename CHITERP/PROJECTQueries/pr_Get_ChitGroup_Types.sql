create  proc pr_Get_ChitGroup_Types
as
begin
	declare @tbl table
	(TypeID int,
	[Type] varchar(100))

	insert into @tbl
	select 1, 'GENERAL GROUP'
	UNION
	select 2, 'CHIT ACTUAL GROUP'

	select * From @tbl
end