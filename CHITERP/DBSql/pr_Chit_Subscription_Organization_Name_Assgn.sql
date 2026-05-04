SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE pr_Chit_Subscription_Organization_Name_Assgn
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT        CONTID, CPORGANISATION, CPORGANISATION + ' - ' + CPMOBILENO1 AS CNAME
	FROM            dbo.ContactMaster
	WHERE        (DISPSTATUS = 0)
	Order by CPORGANISATION

END
GO
