
CREATE PROCEDURE [dbo].[PR_ACCOUNTINGYEAR_ID_CHK] @PFYear INT, @PTYear INT AS

SET NOCOUNT ON

SELECT AccountingYear.YRID, AccountingYear.YRDESC, * FROM AccountingYear
WHERE Year(FDate)=@PFYear AND Year(TDate)=@PTYear