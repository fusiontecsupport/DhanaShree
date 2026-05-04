create proc pr_get_attachment_type
as 
begin
	declare @tbl table
	(
		DValue int,
		DText varchar(50)
	)

	insert into @tbl
	select 1, 'Client Photo'
	insert into @tbl
	select 2, 'Client ID Proof'
	insert into @tbl
	select 3, 'Client Address Proof'

	select * From @tbl
end