USE [MCT_ERP]
GO

/****** Object:  View [dbo].[vw_repair_slab_latest_ratecard_01]    Script Date: 03/03/2022 12:43:24 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE view [dbo].[VW_Leasing_Slab_Latest_Ratecard_01]
as
select STMRID,TARIFFMID, max(LSLABMDATE ) 'RecentDate' from LEASINGSLABMASTER (nolock)
group by STMRID,TARIFFMID


GO


