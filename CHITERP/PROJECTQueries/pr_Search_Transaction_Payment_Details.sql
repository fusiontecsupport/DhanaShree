-- =============================================  
-- Author:  <Rajesh S>  
-- Create date: <02/07/2022>  
-- Description: <Transaction Entry collection details>  
-- Exec [dbo].[pr_Search_Transaction_Payment_Details] @FilterTerm='',@SortIndex=1,@SortDirection='asc',@StartRowNum=1,@EndRowNum=10,@TotalRowsCount=100,@FilteredRowsCount=100,@CUSRID='admin'  
-- =============================================  
alter  PROCEDURE [dbo].[pr_Search_Transaction_Payment_Details]  
 @FilterTerm nvarchar(250) = NULL --parameter to search all columns by    
  , @SortIndex INT = 1 -- a one based index to indicate which column to order by  
  , @SortDirection CHAR(4) = 'ASC' --the direction to sort in, either ASC or DESC  
  , @StartRowNum INT = 1 --the first row to return  
  , @EndRowNum INT = 10 --the last row to return  
  , @TotalRowsCount INT OUTPUT  
  , @FilteredRowsCount INT OUTPUT  
  , @CUSRID varchar(100) 
  , @empID int
  , @ProdType int
  , @FrDt datetime
  , @ToDt datetime    
AS  
BEGIN  
 SET @FilterTerm = '%' + @FilterTerm + '%'  
  
    DECLARE @TableMaster TABLE  
    (   
	TRANID INT,
	PaymentDt varchar(12),
	 CLIENTNAME VARCHAR(250) 
   , SUBSCRIPTIONID INT
   , PRODUCTTYPE VARCHAR(25)
   , TRANCATEGORY VARCHAR(250)  
   , TRANSUBCATEGORY VARCHAR(250)    
   , TRANMODE VARCHAR(250)    
   , STATUDESC VARCHAR(250)  
   , AMOUNT numeric(18,2)
   , SUBSCRIPTIONCODE varchar(100)
   , RowNum INT  
    )  
  
 --INSERT INTO @TableMaster(TRANID, CLIENTNAME ,  SUBSCRIPTIONID , PRODUCTTYPE , TRANCATEGORY , TRANSUBCATEGORY , TRANMODE, statudesc, Amount, RowNum)     
  
 --SELECT CollectionListID, c.clientname ,ProductID, case when ProductTypeID =29 then 'Chit' when ProductTypeID =30 then 'Loan' when ProductTypeID =31 then 'Deposit' End,
 --    'Collection', d.type, e.type, f.type, isnull(GeneratedAmount,0),
	-- Row_Number() OVER (  
 --           ORDER BY
 --           /*VARCHAR, NVARCHAR, CHAR ORDER BY*/  
              
 --           CASE @SortDirection  
 --             WHEN 'ASC'  THEN  
 --               CASE @SortIndex  
 --                 WHEN 0 THEN c.clientname     
	--			  WHEN 1 THEN (case when ProductTypeID =29 then 'Chit' when ProductTypeID =30 then 'Loan' when ProductTypeID =31 then 'Deposit' End)
	--			  WHEN 2 THEN d.type    
	--			  WHEN 3 THEN e.type
	--			  WHEN 4 THEN f.type  
 --               END               
 --           END ASC,  
 --           CASE @SortDirection  
 --             WHEN 'DESC' THEN   
 --               CASE @SortIndex  
 --                 WHEN 0 THEN c.clientname     
	--			  WHEN 1 THEN (case when ProductTypeID =29 then 'Chit' when ProductTypeID =30 then 'Loan' when ProductTypeID =31 then 'Deposit' End)
	--			  WHEN 2 THEN d.type    
	--			  WHEN 3 THEN e.type
	--			  WHEN 4 THEN f.type  
 --               END  
 --           END DESC   
              
 --           /*DATETIME ORDER BY*/         
 --           ) AS RowNum  
	--FROM dbo.TransactionCollectionList a (nolock) join chitsubscription b(nolock) on a.ProductID = b.SubscriptionID
	--	join ClientMaster c(nolock) on b.PersonID = c.CLIENTID
	--	join MasterType d(nolock) on a.TransactionCategoryTypeID = d.typeid
	--	join MasterType e(nolock) on a.TransactionModeTypeID = e.typeid
	--	join MasterType f(nolock) on a.StatusTypeID = f.typeid
 --  WHERE  (c.CreatedBy = @CUSRID or @CUSRID ='')  
 --  and (@FilterTerm IS NULL     
 --       OR CLIENTNAME LIKE @FilterTerm    
 --       OR (case when ProductTypeID =29 then 'Chit' when ProductTypeID =30 then 'Loan' when ProductTypeID =31 then 'Deposit' End) LIKE @FilterTerm    
	--	OR d.type LIKE @FilterTerm    
	--	OR e.type  LIKE @FilterTerm    
	--	OR f.type  LIKE @FilterTerm  )  

	INSERT INTO @TableMaster(TRANID, PaymentDt, CLIENTNAME ,  SUBSCRIPTIONID , PRODUCTTYPE , TRANCATEGORY , TRANSUBCATEGORY , TRANMODE, 
	statudesc, Amount, SUBSCRIPTIONCODE, RowNum)     
  
 SELECT PaymentListID, convert(varchar,GenerratedDate,103),c.clientname ,ProductID, case when ProductTypeTypeID =29 then 'Chit' when ProductTypeTypeID =30 then 'Loan' when ProductTypeTypeID =31 then 'Deposit' End,
     'Payment', d.type, e.type, f.type, isnull(GeneratedAmount,0), ISNULL(b1.ChitSubscriptionCode,ISNULL(b2.LoanSubscriptionCode,'')) SUBSCRIPTIONCODE,
	 Row_Number() OVER (  
            ORDER BY
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/  
              
            CASE @SortDirection  
              WHEN 'ASC'  THEN  
                CASE @SortIndex  
                  WHEN 0 THEN c.clientname     
				  WHEN 1 THEN (case when ProductTypeTypeID =29 then 'Chit' when ProductTypeTypeID =30 then 'Loan' when ProductTypeTypeID =31 then 'Deposit' End)
				  WHEN 2 THEN d.type    
				  WHEN 3 THEN e.type
				  WHEN 4 THEN f.type  
                END               
            END ASC,  
            CASE @SortDirection  
              WHEN 'DESC' THEN   
                CASE @SortIndex  
                  WHEN 0 THEN c.clientname     
				  WHEN 1 THEN (case when ProductTypeTypeID =29 then 'Chit' when ProductTypeTypeID =30 then 'Loan' when ProductTypeTypeID =31 then 'Deposit' End)
				  WHEN 2 THEN d.type    
				  WHEN 3 THEN e.type
				  WHEN 4 THEN f.type  
                END  
            END DESC   
              
            /*DATETIME ORDER BY*/         
            ) AS RowNum  
	FROM dbo.TransactionPaymentList a (nolock) 
		left join chitsubscription b1(nolock) on a.ProductID = b1.SubscriptionID and ProductTypeTypeID=29		
		left join loansubscription b2(nolock) on a.ProductID = b2.LoanSubscriptionID and ProductTypeTypeID=30
		left join LoanRequest b3(nolock) on b2.LoanRequestID= b3.LoanRequestID
		join ClientMaster c(nolock) on (b1.PersonID = c.CLIENTID or b3.PersonID = c.clientid)
		join MasterType d(nolock) on a.TransactionCategoryTypeID = d.typeid
		join MasterType e(nolock) on a.TransactionModeTypeID = e.typeid
		join MasterType f(nolock) on a.StatusTypeID = f.typeid
   WHERE  (c.CreatedBy = @CUSRID or @CUSRID ='' or @empid = CollectionAgentPersonID1 or @empid = CollectionAgentPersonID2 or b3.AccountEmployeeID = @empID)  
   and (ProductTypeTypeID = @ProdType or isnull(@ProdType ,0)=0)   
   and GenerratedDate between @FrDt and @ToDt
   and (@FilterTerm IS NULL     
        OR CLIENTNAME LIKE @FilterTerm    
        OR (case when ProductTypeTypeID =29 then 'Chit' when ProductTypeTypeID =30 then 'Loan' when ProductTypeTypeID =31 then 'Deposit' End) LIKE @FilterTerm    
		OR d.type LIKE @FilterTerm    
		OR e.type  LIKE @FilterTerm    
		OR f.type  LIKE @FilterTerm  ) 

	SELECT TRANID , CLIENTNAME , SUBSCRIPTIONID , PRODUCTTYPE ,    TRANCATEGORY , TRANSUBCATEGORY , TRANMODE, STATUDESC ,
	AMOUNT, PaymentDt, SUBSCRIPTIONCODE
    FROM    @TableMaster  
    --WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum  
      
    SELECT @TotalRowsCount = COUNT(*)  
	FROM dbo.TransactionPaymentList a (nolock) 
	
		left join chitsubscription b1(nolock) on a.ProductID = b1.SubscriptionID and ProductTypeTypeID=29		
		left join loansubscription b2(nolock) on a.ProductID = b2.LoanSubscriptionID and ProductTypeTypeID=30
		left join LoanRequest b3(nolock) on b2.LoanRequestID= b3.LoanRequestID
		join ClientMaster c(nolock) on (b1.PersonID = c.CLIENTID or b3.PersonID = c.clientid)
		join MasterType d(nolock) on a.TransactionCategoryTypeID = d.typeid
		join MasterType e(nolock) on a.TransactionTypeTypeID = e.typeid
		join MasterType f(nolock) on a.StatusTypeID = f.typeid
	WHERE  (c.CreatedBy = @CUSRID or @CUSRID ='' or @empid = CollectionAgentPersonID1 or @empid = CollectionAgentPersonID2 or b3.AccountEmployeeID = @empID)  
   and (ProductTypeTypeID  = @ProdType or isnull(@ProdType ,0)=0)   
   and GenerratedDate between @FrDt and @ToDt
          
   /* CHANGE  TABLE NAME */  
      
    SELECT @FilteredRowsCount = COUNT(*)  
    FROM   @TableMaster  
  
END 


