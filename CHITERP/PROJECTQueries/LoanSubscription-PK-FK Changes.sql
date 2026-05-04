
alter table LoanRequest
drop constraint FK_LoanRequest_ComissionAgent

ALTER TABLE [dbo].LoanRequest  WITH CHECK ADD  CONSTRAINT FK_LoanRequest_ComissionAgent FOREIGN KEY(CommisionAgentPersonID)
REFERENCES [dbo].[ContactMaster] (ContID)
ON DELETE CASCADE
GO
ALTER TABLE [dbo].LoanRequest CHECK CONSTRAINT FK_LoanRequest_ComissionAgent
GO


alter table LoanRequest
drop constraint FK_LoanRequest_NomineePerson
go

ALTER TABLE [dbo].LoanRequest  WITH CHECK ADD  CONSTRAINT FK_LoanRequest_NomineePerson FOREIGN KEY(NomineePersonID)
REFERENCES [dbo].[ContactMaster] (ContID)
--ON DELETE CASCADE
GO
ALTER TABLE [dbo].LoanRequest CHECK CONSTRAINT FK_LoanRequest_NomineePerson
GO

alter table LoanRequest
drop constraint FK_LoanRequest_WitnessPerson1
go
ALTER TABLE [dbo].LoanRequest  WITH CHECK ADD  CONSTRAINT FK_LoanRequest_WitnessPerson1 FOREIGN KEY(Witness1PersonID)
REFERENCES [dbo].[ContactMaster] (ContID)
--ON DELETE CASCADE
GO
ALTER TABLE [dbo].LoanRequest CHECK CONSTRAINT FK_LoanRequest_WitnessPerson1
GO


alter table LoanRequest
drop constraint FK_LoanRequest_WitnessPerson2
go
ALTER TABLE [dbo].LoanRequest  WITH CHECK ADD  CONSTRAINT FK_LoanRequest_WitnessPerson2 FOREIGN KEY(Witness2PresonID)
REFERENCES [dbo].[ContactMaster] (ContID)
--ON DELETE CASCADE
GO
ALTER TABLE [dbo].LoanRequest CHECK CONSTRAINT FK_LoanRequest_WitnessPerson2
GO