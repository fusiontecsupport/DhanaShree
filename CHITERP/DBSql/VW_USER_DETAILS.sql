use MCT_ERP
go 

CREATE VIEW [dbo].[VW_USER_DETAILS]
AS
SELECT        dbo.ApplicationUserGroups.ApplicationGroupId, dbo.ApplicationGroups.Name AS GroupName, dbo.AspNetUsers.UserName
FROM            dbo.ApplicationUserGroups INNER JOIN
                         dbo.AspNetUsers ON dbo.AspNetUsers.Id = dbo.ApplicationUserGroups.ApplicationUserId INNER JOIN
                         dbo.ApplicationGroups ON dbo.ApplicationUserGroups.ApplicationGroupId = dbo.ApplicationGroups.Id


