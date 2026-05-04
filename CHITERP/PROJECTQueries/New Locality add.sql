select * from ContactAddressCity where cityname like 'coimb%'
select * from ContactAddressLocality where cityid=61
localityname like 'coimb%'
update ContactAddressLocality 
set LocalityName='Sundarapuram,CBE'
where LocalityName='Sundarapuram'
insert into ContactAddressLocality (CityID,LocalityName)
select 61, 'Sundarapuram,CBE'
