create proc pr_Get_Prod_Types
@prodtype int=0
as
begin

	select Typeid 'DVal', Type 'DTxt'
	from MasterType(nolock)
	where TypeID in (187,854) --, 182
	and (@prodtype = 0 or TypeID = @prodtype)
end