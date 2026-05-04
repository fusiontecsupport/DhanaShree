select *from AspNetRoles where Name Like '%RepairSlabMaster%'

update AspNetRoles set  
[RMenuType] = 'Repair Part',
[RMenuGroupId] = 103,
[RMenuGroupOrder] = 1,
[RMenuIndex] = 'Index',
[RControllerName] = 'RepairPartMaster',
[ModuleID] = 1,
[RImageClassName] = 'fa fa-briefcase' where Name Like '%RepairPartMaster%'


update AspNetRoles set  
[RMenuType] = 'Repair Slab',
[RMenuGroupId] = 103,
[RMenuGroupOrder] = 2,
[RMenuIndex] = 'Index',
[RControllerName] = 'RepairSlabMaster',
[ModuleID] = 1,
[RImageClassName] = 'fa fa-briefcase' where Name Like '%RepairSlabMaster%'








