
CREATE PROCEDURE [dbo].[PR_COMPANYACCOUNTINGDETAIL_ID_CHK] @PCompId Int,@PYrId Int AS

SET NOCOUNT ON

SELECT CompanyAccountingDetail.COMPYID, CompanyAccountingDetail.COMPID, CompanyAccountingDetail.YRID
FROM CompanyAccountingDetail
WHERE CompanyAccountingDetail.COMPID=@PCompId AND CompanyAccountingDetail.YRID=@PYrId