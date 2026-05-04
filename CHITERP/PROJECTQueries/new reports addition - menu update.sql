select * from aspnetroles where name like '%active%'
select * from aspnetroles where name like '%prize%'
select * from aspnetroles where name like '%arrears%'

update aspnetroles 
set RMenuType	= 'Chit Prize Paid/Unpaid', 
RControllerName	='PrizePaidUnpaidRpt',RMenuGroupId	= 202, RMenuGroupOrder	=5,RMenuIndex=	'Index',ModuleID	=1,RImageClassName=''
where Description = 'Chit Prize Paid Unpaid Report'

update aspnetroles 
set RMenuType	= 'Chit Business Report', 
RControllerName	='ChitBusinessReport',RMenuGroupId	= 202, RMenuGroupOrder	=6,RMenuIndex=	'Index',ModuleID	=1,RImageClassName=''
where name = 'ViewChitBusinessRptIndex'



update aspnetroles 
set RMenuType	= 'Arrears Report', 
RControllerName	='ArrearsReport',RMenuGroupId	= 202, RMenuGroupOrder	=7,RMenuIndex=	'Index',ModuleID	=1,RImageClassName=''
where name = 'ViewArrearsReportIndex'