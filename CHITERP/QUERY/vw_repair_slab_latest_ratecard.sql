USE [MCT_ERP]
GO

/****** Object:  View [dbo].[vw_repair_slab_latest_ratecard]    Script Date: 02/03/2022 11:05:58 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER view [dbo].[vw_repair_slab_latest_ratecard]
as
select a.* from REPAIRSLABMASTER a(nolock) 
	join vw_repair_slab_latest_ratecard_01 b(nolock) on a.TARIFFMID=b.TARIFFMID and a.STMRID = b.STMRID and a.RSLABMDATE = b.RecentDate
GO


