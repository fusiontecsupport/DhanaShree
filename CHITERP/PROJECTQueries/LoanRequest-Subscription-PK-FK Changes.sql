alter table LoanRequest
drop constraint FK_LoanRequest_Person
go

--ALTER TABLE [dbo].LoanRequest  WITH CHECK ADD  CONSTRAINT [FK_LoanRequest_ContactMaster] FOREIGN KEY(PersonId)
--REFERENCES [dbo].[ContactMaster] (ContId)
--ON DELETE CASCADE
--GO
--ALTER TABLE [dbo].LoanRequest CHECK CONSTRAINT [FK_LoanRequest_ContactMaster]
--GO

--alter table LoanRequest
--drop constraint [FK_LoanRequest_ContactMaster]

alter table LoanRequest
drop constraint [FK_LoanRequest_ClientMaster]
go

ALTER TABLE [dbo].LoanRequest  WITH CHECK ADD  CONSTRAINT [FK_LoanRequest_ClientMaster] FOREIGN KEY(PersonId)
REFERENCES [dbo].[ClientMaster] (ClientId)
ON DELETE CASCADE
GO
ALTER TABLE [dbo].LoanRequest CHECK CONSTRAINT [FK_LoanRequest_ClientMaster]
GO


alter table LoanRequest
drop constraint FK_LoanRequest_Organization
go