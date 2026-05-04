use DSFinFusion
go
--use FinFusion
--go
alter table clientmaster
add localityid int null

alter table clientmaster
add Father_or_HusbandName varchar(150) null

alter table clientmaster
add Occupation  varchar(150) null

alter table clientmaster
add OccupationAddress  varchar(1500) null

alter table clientmaster
add MonthlyIncome_or_Salary  numeric(20,2) null

alter table clientmaster
add Nominee_Name  varchar(150) null

alter table clientmaster
add Nominee_Address  varchar(1500) null

alter table clientmaster
add Nominee_RelationshipId  int null

alter table clientmaster
add IntroByType varchar(50) null

--select * from MasterType where type='mother'
--select * from mastercategory where categoryid= 33

--select * From ContactAddressLocality