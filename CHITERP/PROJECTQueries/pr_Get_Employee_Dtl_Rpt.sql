alter proc pr_Get_Employee_Dtl_Rpt
@usrid varchar(100)=''
as
begin

	select distinct a.cateid DValue, a.catename + ', '+isnull(DSGNDESC,'')+', '+  isnull(DEPTDESC,'') DText
	from aspnetusers u(nolock) 
		left join EMPLOYEEMASTER a (nolock) on a.cateid = u.EmpId
		left join DEPARTMENTMASTER b(nolock) on a.deptid = b.deptid
		left join designationmaster c(nolock) on a.dsgnid = c.dsgnid 
	where UserName = @usrid or @usrid = ''
end


