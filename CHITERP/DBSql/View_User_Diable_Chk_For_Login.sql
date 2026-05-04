alter VIEW [dbo].[View_User_Diable_Chk_For_Login]
AS
SELECT        dbo.ApplicationUserGroups.ApplicationGroupId, dbo.ApplicationGroups.Name AS GroupName, dbo.AspNetUsers.UserName, dbo.AspNetUsers.EmpId,
                         dbo.EMPLOYEEMASTER.DISPSTATUS, dbo.EMPLOYEEMASTER.CATEID
FROM            dbo.AspNetUsers INNER JOIN dbo.EMPLOYEEMASTER ON dbo.AspNetUsers.EmpId = dbo.EMPLOYEEMASTER.CATEID left JOIN
				dbo.ApplicationUserGroups ON dbo.AspNetUsers.Id = dbo.ApplicationUserGroups.ApplicationUserId left join
				 dbo.ApplicationGroups ON dbo.ApplicationUserGroups.ApplicationGroupId = dbo.ApplicationGroups.Id 
						 
GO