USE [FinFusion]
GO

/****** Object:  View [dbo].[VW_USER_DETAILS]    Script Date: 12/05/2022 12:18:45 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[VW_USER_DETAILS]
AS
SELECT        dbo.ApplicationUserGroups.ApplicationGroupId, dbo.ApplicationGroups.Name AS GroupName, dbo.AspNetUsers.UserName
FROM            dbo.ApplicationUserGroups INNER JOIN
                         dbo.AspNetUsers ON dbo.AspNetUsers.Id = dbo.ApplicationUserGroups.ApplicationUserId INNER JOIN
                         dbo.ApplicationGroups ON dbo.ApplicationUserGroups.ApplicationGroupId = dbo.ApplicationGroups.Id



GO


