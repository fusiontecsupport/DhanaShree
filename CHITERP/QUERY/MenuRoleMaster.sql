USE [MCT_ERP]
GO
/****** Object:  Table [dbo].[MenuRoleMaster]    Script Date: 11/02/2022 16:50:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MenuRoleMaster](
	[MenuId] [int] IDENTITY(1,1) NOT NULL,
	[LinkText] [varchar](100) NOT NULL,
	[ActionName] [varchar](100) NOT NULL,
	[ControllerName] [varchar](100) NOT NULL,
	[Roles] [varchar](250) NOT NULL,
	[MenuGId] [smallint] NULL,
	[MenuGIndex] [smallint] NULL,
	[ImageClassName] [varchar](100) NULL CONSTRAINT [DF_MenuRoleMaster_ImageClassName_1]  DEFAULT (NULL),
 CONSTRAINT [PK_Table_1] PRIMARY KEY CLUSTERED 
(
	[MenuId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO