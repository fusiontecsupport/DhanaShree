-- exec pr_Get_Employee_Dtl 'test'
alter proc pr_Get_Employee_Dtl
@usrid varchar(100)=''
as
begin

	select a.cateid DValue, a.catename + ', '+isnull(DSGNDESC,'')+', '+  isnull(DEPTDESC,'') DText
	from EMPLOYEEMASTER a (nolock)
		left join DEPARTMENTMASTER b(nolock) on a.deptid = b.deptid
		left join designationmaster c(nolock) on a.dsgnid = c.dsgnid 
	union
	select distinct  0 DValue, 'ALL' DText
	from EMPLOYEEMASTER a (nolock)
	where @usrid =''
end