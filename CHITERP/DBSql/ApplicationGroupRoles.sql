USE [MCT_ERP]
GO

/****** Object:  Table [dbo].[ApplicationGroupRoles]    Script Date: 14-02-2022 10:14:56 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
DROP TABLE [ApplicationGroupRoles]
GO
CREATE TABLE [dbo].[ApplicationGroupRoles](
	[ApplicationRoleId] [nvarchar](128) NOT NULL,
	[ApplicationGroupId] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_dbo.ApplicationGroupRoles] PRIMARY KEY CLUSTERED 
(
	[ApplicationRoleId] ASC,
	[ApplicationGroupId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ApplicationGroupRoles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ApplicationGroupRoles_dbo.AspNetRoles_ApplicationRoleId] FOREIGN KEY([ApplicationRoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[ApplicationGroupRoles] CHECK CONSTRAINT [FK_dbo.ApplicationGroupRoles_dbo.AspNetRoles_ApplicationRoleId]
GO



--ALTER TABLE [dbo].[ApplicationGroupRoles]  drop CONSTRAINT [FK_dbo.ApplicationGroupRoles_dbo.ApplicationGroup_Id] 

ALTER TABLE [dbo].[ApplicationGroupRoles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ApplicationGroupRoles_dbo.ApplicationGroups_Id] FOREIGN KEY([ApplicationGroupId])
REFERENCES [dbo].[ApplicationGroups] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[ApplicationGroupRoles] CHECK CONSTRAINT [FK_dbo.ApplicationGroupRoles_dbo.ApplicationGroups_Id]
GO


