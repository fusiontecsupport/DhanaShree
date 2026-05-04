
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

alter PROCEDURE pr_Chit_Subscription_Scheme_Detail_Assgn
@PChitSchemeID int
AS
BEGIN

	SET NOCOUNT ON;

	SELECT ChitSchemeID, ChitSchemeCode, ChitSchemeName, ChitValue, ChitDuration
	FROM dbo.ChitScheme
	Where ChitSchemeID = @PChitSchemeID

END
GO
