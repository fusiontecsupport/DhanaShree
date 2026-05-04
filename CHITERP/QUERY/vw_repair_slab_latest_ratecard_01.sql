USE [MCT_ERP]
GO

/****** Object:  View [dbo].[vw_repair_slab_latest_ratecard_01]    Script Date: 02/03/2022 11:06:44 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER view [dbo].[vw_repair_slab_latest_ratecard_01]
as
select STMRID,TARIFFMID, max(RSLABMDATE ) 'RecentDate' from REPAIRSLABMASTER (nolock)
group by STMRID,TARIFFMID

GO


