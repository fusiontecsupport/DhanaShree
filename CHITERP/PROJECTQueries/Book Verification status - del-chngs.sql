use FinFusion
go

use dsFinFusion
go

select * from mastertype where type like '%more%pai%'
select * from mastertype where TypeCategoryID = 207



select * 
delete From LuminaRoleStatusMap where categoryid = 207
and  statustypeid in (1101, 1102)

update mastertype
set Type = 'Less Paid (+)'
where Type = 'Less Paid (-)'

select * into mastertype_bkup030523
from mastertype
where typeid in (1101, 1102)

delete from mastertype
where typeid in (1101, 1102)

update mastertype 
where TypeCategoryID = 207