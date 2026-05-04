use DSFinFusion
go


use FinFusion
go


alter table  transactioncollectionlist
add ChequeNo varchar(25) null

alter table  transactioncollectionlist
add ChequeDate datetime null

alter table  transactioncollectionlist
add ChequeBankName varchar(150) null 


alter table  TransactionCollectionReciepts
add ChequeNo varchar(25) null

alter table  TransactionCollectionReciepts
add ChequeDate datetime null

alter table  TransactionCollectionReciepts
add ChequeBankName varchar(150) null 


alter table  transactionpaymentlist
add ChequeNo varchar(25) null

alter table  transactionpaymentlist
add ChequeDate datetime null

alter table  transactionpaymentlist
add ChequeBankName varchar(150) null

alter table  transactionpaymentlist
add ChequeRealizedDate datetime null

alter table  transactionpaymentlist
add FundAccountID int null



alter table  transactionpayment
add ChequeNo varchar(25) null

alter table  transactionpayment
add ChequeDate datetime null

alter table  transactionpayment
add ChequeBankName varchar(150) null

alter table  transactionpayment
add ChequeRealizedDate datetime null

alter table  transactionpayment
add FundAccountID int null

