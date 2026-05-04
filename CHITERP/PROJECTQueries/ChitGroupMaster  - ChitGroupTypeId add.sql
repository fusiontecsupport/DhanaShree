ALTER TABLE CHITGROUP
ADD ChitGroupTypeID int null
go

update CHITGROUP
set ChitGroupTypeID  = 2
where ChitGroupTypeID is null


update CHITGROUP
set ChitGroupTypeID  = 1
where MaxGrpMembers > 100