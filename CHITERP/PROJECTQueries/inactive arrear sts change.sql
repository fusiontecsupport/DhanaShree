select * from mastertype where type like '%arrear%'

update mastertype 
set type = 'Dead Arrear'
where [type] like '%Inactive Arrear%'