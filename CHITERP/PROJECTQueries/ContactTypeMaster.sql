USE [FinFusion]
GO

/****** Object:  Table [dbo].[ContactTypeMaster]    Script Date: 19-06-2022 12:20:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
drop table [ContactTypeMaster]
go

CREATE TABLE [dbo].[ContactTypeMaster](
	[CONTTID] [int] IDENTITY(1,1) NOT NULL,
	[CONTTYPESHRTDESC] [varchar](15) NULL,
	[CONTTYPEDESC] [varchar](250) NULL,
	[DISPSTATUS] [smallint] NULL,
	[CreatedBy] [varchar](50) NULL,
	[CreatedDt] [datetime] NULL,
	[UpdatedBy] [varchar](50) NULL,
	[UpdatedDt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[CONTTID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ContactTypeMaster] ADD  DEFAULT ('') FOR [CONTTYPESHRTDESC]
GO

ALTER TABLE [dbo].[ContactTypeMaster] ADD  DEFAULT ('') FOR [CONTTYPEDESC]
GO

ALTER TABLE [dbo].[ContactTypeMaster] ADD  DEFAULT ((0)) FOR [DISPSTATUS]
GO


