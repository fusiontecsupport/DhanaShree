-- =============================================
-- Author:		<Yamuna J>
-- Create date: <26-05-2022>
-- Description:	<Masters Fund Account Details>
-- =============================================
/*
exec pr_Get_Master_Types 'Company Fund Account', 'Status'
declare @tcnt int, @fcnt int
exec [pr_Search_Masters_FundAccount] null,1,'asc',1,10, @tcnt  output, @fcnt output
*/
alter PROCEDURE [dbo].[pr_Search_Masters_FundAccount]
	 @FilterTerm nvarchar(250) = NULL --parameter to search all columns by
  , @SortIndex INT = 1 -- a one based index to indicate which column to order by
  , @SortDirection CHAR(4) = 'ASC' --the direction to sort in, either ASC or DESC
  , @StartRowNum INT = 1 --the first row to return
  , @EndRowNum INT = 10 --the last row to return
  , @TotalRowsCount INT OUTPUT
  , @FilteredRowsCount INT OUTPUT
AS
BEGIN
	 SET @FilterTerm = '%' + @FilterTerm + '%' 	  
	set nocount on
	 Declare @TableMaster Table 
	 (
	   
	   AccountCode Varchar(100),
	   AccountName Varchar(100),	
	   AccountOpeningBalance float , 
	   AccountBank 	 Varchar(150) ,
	   AccountBranch Varchar(150),
	   AccountNumber Varchar(150),	
	   ADescription Varchar(250),
	   StatusTypeID varchar(50),	   	     
	   FundAccountID Int,	
	   RowNum Int	 
	 )

	 INSERT INTO @TableMaster(AccountCode ,AccountName,AccountOpeningBalance, AccountBank,AccountBranch,AccountNumber,ADescription,StatusTypeID, FundAccountID, RowNum)
                 SELECT       AccountCode ,AccountName,isnull(AccountOpeningBalance,0.00) as AccountOpeningBalance, AccountBank,AccountBranch,AccountNumber,
				              isnull([Description],'') as [Description],st.type, FundAccountID,
								 Row_Number() OVER (
	            ORDER BY          
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/           
           
			 CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex			  
				  when 0 then st.type
				   --WHEN 0 THEN (CASE StatusTypeID WHEN 0 THEN 'Active' when 1 then 'InActive'  END)     
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
					when 0 then st.type
				  -- WHEN 0 THEN (CASE StatusTypeID WHEN 0 THEN 'Active' when 1 then 'InActive'  END)    
                END
            END DESC ,
			 
			 CASE @SortDirection
              WHEN 'ASC'  THEN  
			    CASE @SortIndex
				   WHEN 1 THEN AccountCode
				   WHEN 2 THEN AccountName 
				   WHEN 3 THEN AccountBank  
				   WHEN 4 THEN AccountBranch  
				   WHEN 5 THEN AccountNumber   
				   WHEN 6 THEN [Description] 
				  
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
				WHEN 1 THEN AccountCode
				   WHEN 2 THEN AccountName 
				   WHEN 3 THEN AccountBank  
				   WHEN 4 THEN AccountBranch  
				   WHEN 5 THEN AccountNumber   
				   WHEN 6 THEN [Description] 
                END
            END DESC,
			CASE @SortDirection
              WHEN 'ASC'  THEN  
			    CASE @SortIndex
				   WHEN 7 THEN AccountOpeningBalance
				   
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
				 WHEN 8 THEN AccountOpeningBalance
				
                END
            END DESC
            /*DATETIME ORDER BY*/                        
            ) AS RowNum
    FROM CompanyFundAccount (nolock) left join MasterType st(nolock) on CompanyFundAccount.statustypeid = st.typeid
	        

 /* CHANGE  TABLE NAME */
    WHERE   ( @FilterTerm IS NULL                
			  OR AccountName LIKE @FilterTerm   
			  OR AccountBank LIKE @FilterTerm  			
			  OR AccountBranch LIKE @FilterTerm  			
			  OR AccountNumber LIKE @FilterTerm  			
			  OR [Description] LIKE @FilterTerm  			   
			  OR AccountOpeningBalance LIKE @FilterTerm  		
			  --OR StatusTypeID LIKE @FilterTerm  		
			  or st.type LIKE @FilterTerm  		
			)                      				   
                      
       SELECT  AccountCode ,AccountName,AccountOpeningBalance, AccountBank,AccountBranch,AccountNumber,ADescription,
				StatusTypeID,
	           --CASE StatusTypeID WHEN 0 THEN 'Active' WHEN 1 THEN 'InActive' END  AS StatusTypeID,
	           FundAccountID
	      FROM    @TableMaster
	   
   SELECT @TotalRowsCount = COUNT(*)
   FROM  CompanyFundAccount
	
   
   SELECT @FilteredRowsCount = COUNT(*)
   FROM   @TableMaster
END

