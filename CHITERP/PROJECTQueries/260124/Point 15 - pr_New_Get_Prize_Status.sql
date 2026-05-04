use DSFinFusion
go

CREATE proc pr_New_Get_Prize_Status  
as  
begin  
 select 0 'DValue', 'Both' 'DText'  
 union  
 select 1, 'Paid'  
 union  
 select 2, 'UnPaid'  
end
