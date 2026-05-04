create proc pr_Get_Transaction_Dtl
@categ varchar(5),
@id int
as
begin
	DECLARE @TableMaster TABLE  
    (   
	TRANID INT,
	 CLIENTNAME VARCHAR(250) 
   , SUBSCRIPTIONID INT
   , PRODUCTTYPE VARCHAR(25)
   , TRANCATEGORY VARCHAR(250)  
   , TRANSUBCATEGORY VARCHAR(250)    
   , TRANMODE VARCHAR(250)    
   , STATUDESC VARCHAR(250)  
   , AMOUNT numeric(18,2)
	)
	
	if (@categ='C')
	begin
		INSERT INTO @TableMaster(TRANID, CLIENTNAME ,  SUBSCRIPTIONID , PRODUCTTYPE , TRANCATEGORY , TRANSUBCATEGORY , TRANMODE, statudesc, Amount)     
  
		SELECT CollectionListID, c.clientname ,ProductID, case when ProductTypeID =29 then 'Chit' when ProductTypeID =30 then 'Loan' when ProductTypeID =31 then 'Deposit' End,
		 'Collection', d.type, e.type, f.type, isnull(GeneratedAmount,0)
		FROM dbo.TransactionCollectionList a (nolock) join chitsubscription b(nolock) on a.ProductID = b.SubscriptionID
			join ClientMaster c(nolock) on b.PersonID = c.CLIENTID
			join MasterType d(nolock) on a.TransactionCategoryTypeID = d.typeid
			join MasterType e(nolock) on a.TransactionModeTypeID = e.typeid
			join MasterType f(nolock) on a.StatusTypeID = f.typeid
		WHERE CollectionListID= @id
		

	end
	else
	begin
	
		INSERT INTO @TableMaster(TRANID, CLIENTNAME ,  SUBSCRIPTIONID , PRODUCTTYPE , TRANCATEGORY , TRANSUBCATEGORY , TRANMODE, statudesc, Amount)     
  
		SELECT PaymentListID, c.clientname ,ProductID, case when ProductTypeTypeID =29 then 'Chit' when ProductTypeTypeID =30 then 'Loan' when ProductTypeTypeID =31 then 'Deposit' End,
		'Payment', d.type, e.type, f.type, isnull(GeneratedAmount,0)
		FROM dbo.TransactionPaymentList a (nolock) join chitsubscription b(nolock) on a.ProductID = b.SubscriptionID
			join ClientMaster c(nolock) on b.PersonID = c.CLIENTID
			join MasterType d(nolock) on a.TransactionCategoryTypeID = d.typeid
			join MasterType e(nolock) on a.TransactionModeTypeID = e.typeid
			join MasterType f(nolock) on a.StatusTypeID = f.typeid
		WHERE  PaymentListID = @id

	end
 
	SELECT TRANID , CLIENTNAME , SUBSCRIPTIONID , PRODUCTTYPE ,    TRANCATEGORY , TRANSUBCATEGORY , TRANMODE, STATUDESC , AMOUNT
    FROM    @TableMaster  
end