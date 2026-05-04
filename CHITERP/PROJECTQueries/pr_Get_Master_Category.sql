create proc pr_Get_Master_Category
@categorydesc varchar(150)
as
begin
	
	select a.* from MasterCategory a(nolock) 
	where a.Category = @categorydesc 
end