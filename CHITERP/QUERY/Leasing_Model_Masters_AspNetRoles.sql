select *from AspNetRoles where Name Like '%LeasingSlabMaster%'
select *from AspNetRoles order by RMenuType desc

update AspNetRoles set  
[RMenuType] = 'Leasing Condition',
[RMenuGroupId] = 102,
[RMenuGroupOrder] = 1,
[RMenuIndex] = 'Index',
[RControllerName] = 'LeasingConditionMaster',
[ModuleID] = 3,
[RImageClassName] = 'fa fa-briefcase' where Name Like '%LeasingCondition%'

update AspNetRoles set  
[RMenuType] = 'Leasing Container Status',
[RMenuGroupId] = 102,
[RMenuGroupOrder] = 2,
[RMenuIndex] = 'Index',
[RControllerName] = 'LeasingContainerStatusMaster',
[ModuleID] = 3,
[RImageClassName] = 'fa fa-briefcase' where Name Like '%LeasingContainerStatus%'


update AspNetRoles set  
[RMenuType] = 'Leasing Grade',
[RMenuGroupId] = 102,
[RMenuGroupOrder] = 3,
[RMenuIndex] = 'Index',
[RControllerName] = 'LeasingGradeMaster',
[ModuleID] = 3,
[RImageClassName] = 'fa fa-briefcase' where Name Like '%LeasingGradeMaster%'

update AspNetRoles set  
[RMenuType] = 'Leasing Slab',
[RMenuGroupId] = 102,
[RMenuGroupOrder] = 4,
[RMenuIndex] = 'Index',
[RControllerName] = 'LeasingSlabMaster',
[ModuleID] = 3,
[RImageClassName] = 'fa fa-briefcase' where Name Like '%LeasingSlabMaster%'








