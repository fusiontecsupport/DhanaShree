select *from AspNetRoles where Name Like '%EmptyTariffMaster%'
select *from AspNetRoles order by RMenuType desc

update AspNetRoles set  
[RMenuType] = 'Empty Component Code',
[RMenuGroupId] = 101,
[RMenuGroupOrder] = 1,
[RMenuIndex] = 'Index',
[RControllerName] = 'EmptyComponentCodeMaster',
[ModuleID] = 2,
[RImageClassName] = 'fa fa-briefcase' where Name Like '%EmptyComponentCode%'

update AspNetRoles set 
[RMenuType] = 'Empty Damage Code',
[RMenuGroupId] = 101,
[RMenuGroupOrder] = 2,
[RMenuIndex] = 'Index',
[RControllerName] = 'EmptyDamageCodeMaster',
[ModuleID] = 2,
[RImageClassName] = 'fa fa-briefcase' where Name Like '%EmptyDamageCodeMaster%'

update AspNetRoles set  
[RMenuType] = 'Empty Leasing Repair Code',
[RMenuGroupId] = 101,
[RMenuGroupOrder] = 3,
[RMenuIndex] = 'Index',
[RControllerName] = 'EmptyLeasingRepairCodeMaster',
[ModuleID] = 2,
[RImageClassName] = 'fa fa-briefcase' where Name Like '%EmptyLeasingRepairCode%'

update AspNetRoles set  
[RMenuType] = 'Empty Repair Code',
[RMenuGroupId] = 101,
[RMenuGroupOrder] = 4,
[RMenuIndex] = 'Index',
[RControllerName] = 'EmptyRepairCodeMaster',
[ModuleID] = 2,
[RImageClassName] = 'fa fa-briefcase' where Name Like '%EmptyRepairCodeMaster%'

update AspNetRoles set  
[RMenuType] = 'Empty Response Code',
[RMenuGroupId] = 101,
[RMenuGroupOrder] = 5,
[RMenuIndex] = 'Index',
[RControllerName] = 'EmptyResponseCodeMaster',
[ModuleID] = 2,
[RImageClassName] = 'fa fa-briefcase' where Name Like '%EmptyResponseCode%'

update AspNetRoles set  
[RMenuType] = 'Empty Responsibility Code',
[RMenuGroupId] = 101,
[RMenuGroupOrder] = 6,
[RMenuIndex] = 'Index',
[RControllerName] = 'EmptyResponsibilityCodeMaster',
[ModuleID] = 2,
[RImageClassName] = 'fa fa-briefcase' where Name Like '%EmptyResponsibilityCode%'

update AspNetRoles set  
[RMenuType] = 'Empty Tariff',
[RMenuGroupId] = 101,
[RMenuGroupOrder] = 7,
[RMenuIndex] = 'Index',
[RControllerName] = 'EmptyTariffMaster',
[ModuleID] = 2,
[RImageClassName] = 'fa fa-briefcase' where Name Like '%EmptyTariffMaster%'


update AspNetRoles set  
[RMenuType] = 'Empty Slab Type',
[RMenuGroupId] = 101,
[RMenuGroupOrder] = 8,
[RMenuIndex] = 'Index',
[RControllerName] = 'EmptySlabTypeMaster',
[ModuleID] = 2,
[RImageClassName] = 'fa fa-briefcase' where Name Like '%EmptySlabTypeMaster%'


update AspNetRoles set  
[RMenuType] = 'Empty Slab',
[RMenuGroupId] = 101,
[RMenuGroupOrder] = 9,
[RMenuIndex] = 'Index',
[RControllerName] = 'EmptySlabMaster',
[ModuleID] = 2,
[RImageClassName] = 'fa fa-briefcase' where Name Like '%EmptySlabMaster%'







