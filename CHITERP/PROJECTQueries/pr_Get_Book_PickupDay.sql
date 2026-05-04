alter proc pr_Get_Book_PickupDay
@dval int=0
as
begin

	declare @tmptbl table
	(
		DValue int,
		DText varchar(150)
	)
	declare @i int
	set @i=1
	while(@i<26)
	begin
		insert into @tmptbl
		select @i, cast(@i as varchar(10))
		set @i=@i+1
	end
	--insert into @tmptbl
	--select 1, 'Monday'

	--insert into @tmptbl
	--select 2, 'Tuesday'
	--insert into @tmptbl
	--select 3, 'Wednesday'

	--insert into @tmptbl
	--select 4, 'Thursday'
	--insert into @tmptbl
	--select 5, 'Friday'

	--insert into @tmptbl
	--select 6, 'Saturday'
	--insert into @tmptbl
	--select 7, 'Sunday'

	select * from @tmptbl
	where @dval=0 or DValue=@dval
end