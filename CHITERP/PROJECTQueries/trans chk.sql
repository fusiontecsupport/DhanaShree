select * from TransactionCollectionList order by CollectionListID desc
select * from TransactionPaymentAllocation
select * from TransactionCollectionAllocation
select * from TransactionPaymentList
select * from TransactionPaymentAllocation
select * from TransactionCollectionList (nolock) 
where CollectionListID =3273245
select * from TransactionCollectionReciepts (nolock) 
where CollectionListID =3273245
select * from person
select * from ContactAddress

select * from TransactionCollectionAllocation (nolock) 
where CollectionReceiptID =3273260

select * from TransactionCollectionReciepts (nolock) 
where CollectionListID =3273245

order by CollectionRecieptID desc


select *from MasterType where TypeID in (1034,1035,1036)