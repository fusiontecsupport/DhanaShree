select * from MasterType where type='online'

update MasterType 
set type ='GPay/UPI'
where type='online' or type='GPay'
