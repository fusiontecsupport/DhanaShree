-- exec [pr_USER_MENU_DETAIL_ASSGN] @PKUSRID ='admin'
alter  PROCEDURE [dbo].[pr_USER_MENU_DETAIL_ASSGN] @PKUSRID AS  VARCHAR(100) 

AS

SET NOCOUNT ON

Declare @username varchar(100)

Set @username = LOWER(@PKUSRID)

delete from MenuRoleMaster WHERE Roles = @username --@PKUSRID 

INSERT INTO MenuRoleMaster(LinkText, ActionName, ControllerName, Roles, MenuGId, MenuGIndex,ImageClassName)
SELECT    dbo.AspNetRoles.RMenuType, dbo.AspNetRoles.RMenuIndex, dbo.AspNetRoles.RControllerName, dbo.AspNetUsers.UserName, 
                      dbo.AspNetRoles.RMenuGroupId, dbo.AspNetRoles.RMenuGroupOrder, RImageClassName
FROM         dbo.AspNetUsers INNER JOIN
                      dbo.AspNetUserRoles ON dbo.AspNetUsers.Id = dbo.AspNetUserRoles.UserId INNER JOIN
                      dbo.AspNetRoles ON dbo.AspNetUserRoles.RoleId = dbo.AspNetRoles.Id INNER JOIN
                      dbo.ApplicationUserGroups ON dbo.AspNetUsers.Id = dbo.ApplicationUserGroups.ApplicationUserId INNER JOIN
                      dbo.ApplicationGroups ON dbo.ApplicationUserGroups.ApplicationGroupId = dbo.ApplicationGroups.Id
GROUP BY dbo.AspNetUsers.UserName, dbo.AspNetRoles.RMenuType, dbo.AspNetRoles.RMenuIndex, dbo.AspNetRoles.RControllerName, dbo.AspNetRoles.RMenuGroupId, 
                      dbo.AspNetRoles.RMenuGroupOrder, RImageClassName
HAVING      (dbo.AspNetUsers.UserName = @username) AND (dbo.AspNetRoles.RMenuType IS NOT NULL)
ORDER BY dbo.AspNetRoles.RMenuGroupId, dbo.AspNetRoles.RMenuGroupOrder

select * from MenuRoleMaster


