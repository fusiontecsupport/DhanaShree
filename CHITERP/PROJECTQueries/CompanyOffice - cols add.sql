use FinFusion
go

alter table companyoffice
add [AddressLine1] [nvarchar](200) NOT NULL,
constraint DF_companyoffice_AddressLine1 default '' for [AddressLine1]
go

alter table companyoffice
add [AddressLine2] [nvarchar](200) NULL
go

alter table companyoffice
add [LocalityID] [int]  NULL
constraint FK_companyoffice_LocalityID FOREIGN KEY([LocalityID]) references CONTACTADDRESSLOCALITY (localityid) 

go

alter table companyoffice
add [CityID] int  NULL
constraint FK_companyoffice_CityID FOREIGN KEY([CityID]) references CONTACTADDRESSCITY (CityID) 
	
go

alter table companyoffice
add [CountryID] int  NULL
constraint FK_companyoffice_CountryID FOREIGN KEY([CountryID]) references CONTACTADDRESSCountry (CountryID) 
go

alter table companyoffice
add [PIN] [nvarchar](20) NULL
go

alter table companyoffice
add [StatusTypeID] [int]  NULL

go

