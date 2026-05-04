use DSFinFusion
go

alter PROCEDURE pr_Chit_Subscription_Organization_Name_Assgn
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT        CLIENTID, CLIENTNAME, CLIENTNAME+ ' - ' + CLIENTMOBILENO1 AS CNAME
	FROM            dbo.ClientMAster(nolock)
	WHERE        (DISPSTATUS = 0)
	Order by CLIENTNAME

END