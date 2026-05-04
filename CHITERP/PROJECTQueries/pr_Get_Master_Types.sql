/*
exec pr_Get_Master_Types 'Duration', ''
exec pr_Get_Master_Types 'Ascertainment', ''
*/
alter proc pr_Get_Master_Types
@categorydesc varchar(150),
@classification varchar(100)
as
begin
	
	
	select b.TypeID, b.Type from MasterCategory a(nolock) join MasterType b(nolock) on a.categoryid = b.TypeCategoryID
	where a.Category = @categorydesc and isnull(a.Classification ,'') = isnull(@classification,'')
	and b.TypeID !=799 and b.TypeID != 800


end
