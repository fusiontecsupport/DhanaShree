USE [MCT_ERP]
GO


alter table [AspNetRoles]
add [Description] [nvarchar](max) NULL

alter table [AspNetRoles]
add [RMenuType] [nvarchar](128) NULL

alter table [AspNetRoles]
add [RControllerName] [nvarchar](128) NULL

alter table [AspNetRoles]
add [RMenuGroupId] [smallint] NULL

alter table [AspNetRoles]
add [RMenuGroupOrder] [smallint] NULL

alter table [AspNetRoles]
add [RMenuIndex] [nvarchar](128) NULL

alter table [AspNetRoles]
add [ModuleID] [int]  NULL

alter table [AspNetRoles]
add [RImageClassName] [varchar](50) NULL


ALTER TABLE [dbo].[AspNetRoles] ADD  CONSTRAINT [DF_AspNetRoles_ModuleID]  DEFAULT ((0)) FOR [ModuleID]
GO

ALTER TABLE [dbo].[AspNetRoles] ADD  CONSTRAINT [DF_AspNetRoles_RImageClassName]  DEFAULT (NULL) FOR [RImageClassName]
GO


select * from [AspNetRoles]