select *from AspNetRoles where Name Like '%EmptyEstimate%'

update AspNetRoles set  
[RMenuType] = 'Gate In',
[RMenuGroupId] = 200,
[RMenuGroupOrder] = 1,
[RMenuIndex] = 'Index',
[RControllerName] = 'EmptyGeneralGateIn',
[ModuleID] = 1,
[RImageClassName] = 'fa fa-briefcase' where Name Like '%EmptyGeneralGateIn%'

update AspNetRoles set  
[RMenuType] = 'Gate Out',
[RMenuGroupId] = 200,
[RMenuGroupOrder] = 2,
[RMenuIndex] = 'Index',
[RControllerName] = 'EmptyGeneralGateOut',
[ModuleID] = 1,
[RImageClassName] = 'fa fa-briefcase' where Name Like '%EmptyGeneralGateOut%'

update AspNetRoles set  
[RMenuType] = 'Estimate',
[RMenuGroupId] = 200,
[RMenuGroupOrder] = 3,
[RMenuIndex] = 'Index',
[RControllerName] = 'EmptyEstimate',
[ModuleID] = 1,
[RImageClassName] = 'fa fa-briefcase' where Name Like '%EmptyEstimate%'


update AspNetRoles set  
[RMenuType] = 'L.Gate In',
[RMenuGroupId] = 201,
[RMenuGroupOrder] = 1,
[RMenuIndex] = 'Index',
[RControllerName] = 'LeasingGateIn',
[ModuleID] = 1,
[RImageClassName] = 'fa fa-briefcase' where Name Like '%LeasingGateIn%'


update AspNetRoles set  
[RMenuType] = 'L.Gate Out',
[RMenuGroupId] = 201,
[RMenuGroupOrder] = 2,
[RMenuIndex] = 'Index',
[RControllerName] = 'LeasingGateOut',
[ModuleID] = 1,
[RImageClassName] = 'fa fa-briefcase' where Name Like '%LeasingGateOut%'




