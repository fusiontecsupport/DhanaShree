alter proc pr_Get_Book_List
@prodtype int,
@subsid int
as 
begin
	Select b.*,
	a.type 'BookStatus',c.type 'ProdTypeDesc',isnull(e1.catename,'') 'ReviewBy',isnull(e2.catename,'') 'ApprovedBy'
	from CommonAccountBook b(Nolock)  
	join MasterType a on StatusTypeID = a.typeid
	join MasterType c on OwnerTypeID = c.typeid
	left join EMPLOYEEMASTER e1(nolock) on b.[ReviewByEmployeeId] = e1.cateid
	left join EMPLOYEEMASTER e2(nolock) on b.[ApprovedByEmployeeId] = e2.cateid
	where OwnerID= @subsid
	and OwnerTypeID= @prodtype
end