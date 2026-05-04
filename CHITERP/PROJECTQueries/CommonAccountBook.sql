USE [FinFusion]
GO
USE [DSFinFusion]
GO

/****** Object:  Table [dbo].[CommonAccountBook]    Script Date: 08-10-2022 13:01:26 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
drop table [CommonAccountBook]
go
CREATE TABLE [dbo].[CommonAccountBook](
	[AccountBookID] [int] IDENTITY(1,1) NOT NULL,
	[OwnerID] [int] NOT NULL,
	[OwnerTypeID] [int] NOT NULL,
	[BookIssuedDate] [datetime] NULL,
	[BookTerminatedDate] [datetime] NULL,
	[BookNumber] [nvarchar](50) NOT NULL,
	[EmployeeId] [int] NOT NULL,
	[ReviewByEmployeeId] [int] NOT NULL,
	[ApprovedByEmployeeId] [int] NOT NULL,
	[StatusTypeID] [int] NOT NULL,
 CONSTRAINT [PK_ChitBook] PRIMARY KEY CLUSTERED 
(
	[AccountBookID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[CommonAccountBook]  WITH CHECK ADD  CONSTRAINT [FK_CommonAccountBook_CompanyEmployee] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].employeemaster (CATEID)
GO

ALTER TABLE [dbo].[CommonAccountBook] CHECK CONSTRAINT [FK_CommonAccountBook_CompanyEmployee]
GO
ALTER TABLE [dbo].[CommonAccountBook]  WITH CHECK ADD  CONSTRAINT [FK_CommonAccountBook_CompanyEmployee_ReviewBy] FOREIGN KEY([ReviewByEmployeeId])
REFERENCES [dbo].employeemaster (CATEID)
GO

ALTER TABLE [dbo].[CommonAccountBook] CHECK CONSTRAINT [FK_CommonAccountBook_CompanyEmployee_ReviewBy]
GO
ALTER TABLE [dbo].[CommonAccountBook]  WITH CHECK ADD  CONSTRAINT [FK_CommonAccountBook_CompanyEmployee_ApprovedBy] FOREIGN KEY([ApprovedByEmployeeId])
REFERENCES [dbo].employeemaster (CATEID)
GO

ALTER TABLE [dbo].[CommonAccountBook] CHECK CONSTRAINT [FK_CommonAccountBook_CompanyEmployee_ApprovedBy]
GO

ALTER TABLE [dbo].[CommonAccountBook]  WITH CHECK ADD  CONSTRAINT [FK_CommonAccountBook_MasterType] FOREIGN KEY([StatusTypeID])
REFERENCES [dbo].[MasterType] ([TypeID])
GO

ALTER TABLE [dbo].[CommonAccountBook] CHECK CONSTRAINT [FK_CommonAccountBook_MasterType]
GO


