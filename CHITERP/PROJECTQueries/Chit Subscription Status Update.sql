select * from chitsubscription where subscriptionid = 43
update chitsubscription 
set StatusTypeID=881
where subscriptionid = 43
and StatusTypeID=883

select * from mastertype where typeid = 883
select * from mastertype where TypeCategoryID = 191