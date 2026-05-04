USE [MCT_ERP]
GO

/****** Object:  Table [dbo].[ApplicationGroupRole]    Script Date: 14-02-2022 10:14:56 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
DROP TABLE [ApplicationGroups]
GO
CREATE TABLE [dbo].[ApplicationGroups](
	[Id] [nvarchar](128) NOT NULL,
	[name] [nvarchar](500) NOT NULL,
	[Description] [nvarchar](max) NULL
 CONSTRAINT [PK_dbo.ApplicationGroups] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
