use DSFinfusion
go
create proc pr_Get_Account_Status
as
begin
	select 1 TypeID, 'Active' [Type]
	union 
	select 2, 'In-Active'

end