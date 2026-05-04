USE [FinFusion]
GO

/****** Object:  Table [dbo].[ContactMaster]    Script Date: 19-06-2022 12:03:05 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
drop table [ContactMaster]
go
CREATE TABLE [dbo].[ContactMaster](
	[CONTID] [int] IDENTITY(1,1) NOT NULL,
	[CONTTID] [int] NOT NULL,
	[CONTTOTH] [varchar](100) NULL,
	[CONTACTPERSON] [varchar](150) NULL,
	[DATEOFBIRTH] [datetime] NULL,
	[GENDERTYPEID] [int] NULL,
	[CPMOBILENO1] [varchar](25) NULL,
	[CPMOBILENO2] [varchar](25) NULL,
	[CPLANDLINENO1] [varchar](25) NULL,
	[CPLANDLINENO2] [varchar](25) NULL,
	[CPEMAILID] [varchar](150) NULL,
	[CPDESIGNATION] [varchar](150) NULL,
	[CPORGANISATION] [varchar](250) NULL,
	[CPADDRESS1] [varchar](150) NULL,
	[CPADDRESS2] [varchar](150) NULL,
	[CPCITY] [varchar](150) NULL,
	[CPSTATEID] [int] NULL,
	[CPCOUNTRYID] [int] NOT NULL,
	[CPPINCD] [varchar](150) NULL,
	[DISPSTATUS] [smallint] NULL,
	[CreatedBy] [varchar](50) NULL,
	[CreatedDt] [datetime] NULL,
	[UpdatedBy] [varchar](50) NULL,
	[UpdatedDt] [datetime] NULL,
	[CLIENTID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[CONTID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ContactMaster] ADD  DEFAULT ((0)) FOR [CONTTID]
GO

ALTER TABLE [dbo].[ContactMaster] ADD  DEFAULT ('') FOR [CONTTOTH]
GO

ALTER TABLE [dbo].[ContactMaster] ADD  DEFAULT ('') FOR [CONTACTPERSON]
GO

ALTER TABLE [dbo].[ContactMaster] ADD  DEFAULT ('') FOR [DATEOFBIRTH]
GO

ALTER TABLE [dbo].[ContactMaster] ADD  DEFAULT ((0)) FOR [GENDERTYPEID]
GO

ALTER TABLE [dbo].[ContactMaster] ADD  DEFAULT ('') FOR [CPMOBILENO1]
GO

ALTER TABLE [dbo].[ContactMaster] ADD  DEFAULT ('') FOR [CPMOBILENO2]
GO

ALTER TABLE [dbo].[ContactMaster] ADD  DEFAULT ('') FOR [CPLANDLINENO1]
GO

ALTER TABLE [dbo].[ContactMaster] ADD  DEFAULT ('') FOR [CPLANDLINENO2]
GO

ALTER TABLE [dbo].[ContactMaster] ADD  DEFAULT ('') FOR [CPEMAILID]
GO

ALTER TABLE [dbo].[ContactMaster] ADD  DEFAULT ('') FOR [CPDESIGNATION]
GO

ALTER TABLE [dbo].[ContactMaster] ADD  DEFAULT ('') FOR [CPORGANISATION]
GO

ALTER TABLE [dbo].[ContactMaster] ADD  DEFAULT ('') FOR [CPADDRESS1]
GO

ALTER TABLE [dbo].[ContactMaster] ADD  DEFAULT ('') FOR [CPADDRESS2]
GO

ALTER TABLE [dbo].[ContactMaster] ADD  DEFAULT ('') FOR [CPCITY]
GO

ALTER TABLE [dbo].[ContactMaster] ADD  DEFAULT ((0)) FOR [CPSTATEID]
GO

ALTER TABLE [dbo].[ContactMaster] ADD  DEFAULT ((0)) FOR [CPCOUNTRYID]
GO

ALTER TABLE [dbo].[ContactMaster] ADD  DEFAULT ('') FOR [CPPINCD]
GO

ALTER TABLE [dbo].[ContactMaster] ADD  DEFAULT ((0)) FOR [DISPSTATUS]
GO


