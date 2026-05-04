USE [MCT_ERP]
GO

/****** Object:  Table [dbo].[ApplicationUserGroups]    Script Date: 14-02-2022 10:14:48 ******/
SET ANSI_NULLS ON
GO
--ALTER TABLE [dbo].[ApplicationUserGroups]  drop  CONSTRAINT [FK_dbo.ApplicationUserGroups_dbo.AspNetUsers_AppUser_Id]
ALTER TABLE [dbo].[ApplicationUserGroups]  drop  CONSTRAINT [FK_dbo.ApplicationUserGroups_dbo.AspNetUsers_ApplicationUserId]

SET QUOTED_IDENTIFIER ON
GO
drop table [ApplicationUserGroups]
go
CREATE TABLE [dbo].[ApplicationUserGroups](
	--[AppUser_Id1] [nvarchar](128) NOT NULL ,
	[ApplicationUserId] [nvarchar](128) NOT NULL,
	[ApplicationGroupId] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_dbo.ApplicationUserGroups] PRIMARY KEY CLUSTERED 
(
	[ApplicationUserId] ASC,
	[ApplicationGroupId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


--ALTER TABLE [dbo].[ApplicationUserGroups]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ApplicationUserGroups_dbo.AspNetUsers_AppUser_Id] FOREIGN KEY(AppUser_Id)
--REFERENCES [dbo].[AspNetUsers] ([Id])
--ON DELETE CASCADE
--GO

ALTER TABLE [dbo].[ApplicationUserGroups]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ApplicationUserGroups_dbo.AspNetUsers_ApplicationUserId] FOREIGN KEY([ApplicationUserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO

--ALTER TABLE [dbo].[ApplicationUserGroups] CHECK CONSTRAINT [FK_dbo.ApplicationUserGroups_dbo.AspNetUsers_AppUser_Id]
--GO



ALTER TABLE [dbo].[ApplicationUserGroups] CHECK CONSTRAINT [FK_dbo.ApplicationUserGroups_dbo.AspNetUsers_ApplicationUserId]
GO

select * from ApplicationUserGroups

select * from [ApplicationUserGroups]

select * from ApplicationGroups