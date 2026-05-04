use DSFinFusion
go
use FinFusion
go

/*
select * from SOFT_TABLE_DELETE_DETAIL where OPTNSTR= 'ChitSubscription'
select * from finfusion..SOFT_TABLE_DELETE_DETAIL where OPTNSTR= 'ChitSubscription'
select * From SOFT_TABLE_DELETE_DETAIL
  insert into [SOFT_TABLE_DELETE_DETAIL] (TABDID,[OPTNSTR], [TABNAME], [PFLDNAME],[DCONDTNSTR],[DISPDESC])
  select 1,'ChitScheme','ChitGroup','ChitSchemeId','ChitSchemeId=', 'Selected Chit Scheme is used in Chit Group Master!'
  union
  select 2,'ChitGroup','ChitSubscription','ChitGroupId','ChitGroupId=', 'Selected Chit Group is used in Chit Subscription!'
  union
  select 3,'LoanScheme','LoanRequest','LoanSchemeID','LoanSchemeID=', 'Selected Loan Scheme is used in Loan Application/Subscription!'
  union
  select 4,'EmployeeMaster','ChitSubscription','CATEID','CommissionAgentPersonID=', 'Selected Employee is used as Collection Agent in Chit Subscription!'
  union
  select 5,'EmployeeMaster','ChitSubscription','CATEID','CommissionAgentPersonID2=', 'Selected Employee is used as Commission Agent in Chit Subscription!'
  union
  select 6,'EmployeeMaster','LoanRequest','CATEID','AccountEmployeeID=', 'Selected Employee is used as Collection Agent in Loan Subscription!'
  union
  select 7,'ChitSubscription','TransactionCollectionList','ProductID','ProductTypeID=29 and SubscriptionID=', 'Selected Chit Subscription is having Collection Transactions!'
  union
  select 8,'ChitSubscription','TransactionPaymentList','ProductID','ProductTypeTypeID=29 and SubscriptionID=', 'Selected Chit Subscription is having Collection Transactions!'
  union
  select 9,'LoanSubscription','TransactionCollectionList','ProductID','ProductTypeID=30 and LoanSubscriptionID=', 'Selected Loan Subscription is having Collection Transactions!'
  union
  select 10,'LoanSubscription','TransactionPaymentList','ProductID','ProductTypeTypeID=30 and LoanSubscriptionID=', 'Selected Loan Subscription is having Collection Transactions!'
  
  update SOFT_TABLE_DELETE_DETAIL
  set PFLDNAME = 'ProductID' --,DCONDTNSTR=REPLACE(DCONDTNSTR,'ProductID','SubscriptionID')
  where  OPTNSTR in ('ChitSubscription', 'LoanSubscription')

  update SOFT_TABLE_DELETE_DETAIL
  set DCONDTNSTR=REPLACE(DCONDTNSTR,'LoanSubscriptionID','ProductID')
  where  OPTNSTR in ('LoanSubscription')

  update SOFT_TABLE_DELETE_DETAIL
  set DCONDTNSTR=REPLACE(DCONDTNSTR,'SubscriptionID','ProductID')
  where  OPTNSTR in ('ChitSubscription')
  */
