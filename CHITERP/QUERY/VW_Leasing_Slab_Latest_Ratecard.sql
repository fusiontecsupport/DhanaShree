USE [MCT_ERP]
GO

/****** Object:  View [dbo].[VW_Leasing_Slab_Latest_Ratecard]    Script Date: 03/03/2022 12:57:09 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




ALTER view [dbo].[VW_Leasing_Slab_Latest_Ratecard]
as
select a.* from LEASINGSLABMASTER a(nolock) 
	join VW_Leasing_Slab_Latest_Ratecard_01 b(nolock) on a.TARIFFMID=b.TARIFFMID and a.STMRID = b.STMRID and a.LSLABMDATE = b.RecentDate


GO


