--alter table ChitSubscription
--drop constraint FK_ChitSubscription_Person

--ALTER TABLE [dbo].ChitSubscription  WITH CHECK ADD  CONSTRAINT [FK_ChitSubscription_ContactMaster] FOREIGN KEY(PersonId)
--REFERENCES [dbo].[ContactMaster] (ContId)
--ON DELETE CASCADE
--GO
--ALTER TABLE [dbo].ChitSubscription CHECK CONSTRAINT [FK_ChitSubscription_ContactMaster]
--GO

--alter table ChitSubscription
--drop constraint [FK_ChitSubscription_ContactMaster]

alter table ChitSubscription
drop constraint [FK_ChitSubscription_ClientMaster]

ALTER TABLE [dbo].ChitSubscription  WITH CHECK ADD  CONSTRAINT [FK_ChitSubscription_ClientMaster] FOREIGN KEY(PersonId)
REFERENCES [dbo].[ClientMaster] (ClientId)
ON DELETE CASCADE
GO
ALTER TABLE [dbo].ChitSubscription CHECK CONSTRAINT [FK_ChitSubscription_ClientMaster]
GO

alter table ChitSubscription
drop constraint FK_ChitSubscription_ComissionAgent

--ALTER TABLE [dbo].ChitSubscription  WITH CHECK ADD  CONSTRAINT FK_ChitSubscription_ComissionAgent FOREIGN KEY(CommisionAgentPersonID)
--REFERENCES [dbo].[ContactMaster] (ContID)
--ON DELETE CASCADE
--GO
--ALTER TABLE [dbo].ChitSubscription CHECK CONSTRAINT FK_ChitSubscription_ComissionAgent
--GO


alter table ChitSubscription
drop constraint FK_ChitSubscription_NomineePerson
go

ALTER TABLE [dbo].ChitSubscription  WITH CHECK ADD  CONSTRAINT FK_ChitSubscription_NomineePerson FOREIGN KEY(NomineePersonID)
REFERENCES [dbo].[ContactMaster] (ContID)
--ON DELETE CASCADE
GO
ALTER TABLE [dbo].ChitSubscription CHECK CONSTRAINT FK_ChitSubscription_NomineePerson
GO

alter table ChitSubscription
drop constraint FK_ChitSubscription_WitnessPerson1
go
ALTER TABLE [dbo].ChitSubscription  WITH CHECK ADD  CONSTRAINT FK_ChitSubscription_WitnessPerson1 FOREIGN KEY(Witness1PersonID)
REFERENCES [dbo].[ContactMaster] (ContID)
--ON DELETE CASCADE
GO
ALTER TABLE [dbo].ChitSubscription CHECK CONSTRAINT FK_ChitSubscription_WitnessPerson1
GO


alter table ChitSubscription
drop constraint FK_ChitSubscription_WitnessPerson2
go
ALTER TABLE [dbo].ChitSubscription  WITH CHECK ADD  CONSTRAINT FK_ChitSubscription_WitnessPerson2 FOREIGN KEY(Witness2PresonID)
REFERENCES [dbo].[ContactMaster] (ContID)
--ON DELETE CASCADE
GO
ALTER TABLE [dbo].ChitSubscription CHECK CONSTRAINT FK_ChitSubscription_WitnessPerson2
GO

alter table chitsubscription
drop constraint [UNIQUE_CHITSUBSCRIPTION_CODE]

--alter table ChitSubscription
--drop constraint FK_ChitSubscription_Organization