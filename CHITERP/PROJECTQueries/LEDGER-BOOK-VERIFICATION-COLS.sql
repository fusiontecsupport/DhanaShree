USE DSFINFUSION
GO

/*
select * from TransactionCollectionList

ALTER TABLE TransactionCollectionList
DROP COLUMN BkVfcnSts
ALTER TABLE TransactionCollectionList
DROP COLUMN BkVfcnDt
ALTER TABLE TransactionCollectionList
DROP COLUMN BkVfcnAmt
ALTER TABLE TransactionCollectionList
DROP COLUMN BkVfcnRemks


ALTER TABLE Transactionpaymentlist
DROP COLUMN BkVfcnSts
ALTER TABLE Transactionpaymentlist
DROP COLUMN BkVfcnDt
ALTER TABLE Transactionpaymentlist
DROP COLUMN BkVfcnAmt
ALTER TABLE Transactionpaymentlist
DROP COLUMN BkVfcnRemks

*/
alter table TransactionCollectionList
add BkVfcnSts int null

alter table TransactionCollectionList
add BkVfcnDt DATETIME null

alter table TransactionCollectionList
add BkVfcnAmt numeric(18,2) null

alter table TransactionCollectionList
add BkVfcnRemks varchar(1000) null

alter table Transactionpaymentlist
add BkVfcnSts int null

alter table Transactionpaymentlist
add BkVfcnDt DATETIME null

alter table Transactionpaymentlist
add BkVfcnAmt numeric(18,2) null

alter table Transactionpaymentlist
add BkVfcnRemks varchar(1000) null