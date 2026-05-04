
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Yamuna J>
-- Create date: <13-05-2022>
-- Description:	<Masters Amount Code Details>
-- =============================================
/*
declare @tcnt int, @fcnt int
exec [pr_Search_Masters_AmountCode] null,1,'asc',1,10, @tcnt  output, @fcnt output
*/
CREATE PROCEDURE [dbo].[pr_Search_Masters_AmountCode]
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
	   
	   AmountCode Varchar(100),
	   AmountCodeName Varchar(100),	
	   AmountCodeConstant decimal , 	  
	   CustomerViewTypeID Int,	   
	   StatusTypeID Int,	  
	   AmountCodeID Int,	
	   RowNum Int	 
	 )

	 INSERT INTO @TableMaster(AmountCode ,AmountCodeName,AmountCodeConstant, CustomerViewTypeID,StatusTypeID, AmountCodeID, RowNum)
                 SELECT        AmountCode ,AmountCodeName,AmountCodeConstant, CustomerViewTypeID,StatusTypeID, AmountCodeID,
								 Row_Number() OVER (
	            ORDER BY          
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/           
           
			 CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
				  
				  WHEN 0 THEN (CASE CustomerViewTypeID WHEN 0 THEN 'Yes' when 1 then 'No'  END)    
				   WHEN 1 THEN (CASE StatusTypeID WHEN 0 THEN 'Active' when 1 then 'InActive'  END)     
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN (CASE CustomerViewTypeID WHEN 0 THEN 'Yes' when 1 then 'No'  END)     
				   WHEN 1 THEN (CASE StatusTypeID WHEN 0 THEN 'Active' when 1 then 'InActive'  END)    
                END
            END DESC ,
			 
			 CASE @SortDirection
              WHEN 'ASC'  THEN  
			    CASE @SortIndex
				   WHEN 2 THEN AmountCode
				   WHEN 3 THEN AmountCodeName
				  
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
				 WHEN 2 THEN AmountCode
				 WHEN 3 THEN AmountCodeName
				 
                END
            END DESC,
			CASE @SortDirection
              WHEN 'ASC'  THEN  
			    CASE @SortIndex
				   WHEN 4 THEN AmountCodeConstant
				   
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
				 WHEN 4 THEN AmountCodeConstant
				
                END
            END DESC
            /*DATETIME ORDER BY*/                        
            ) AS RowNum
    FROM   MasterAmountCode
	        

 /* CHANGE  TABLE NAME */
    WHERE   ( @FilterTerm IS NULL                
			  OR AmountCode LIKE @FilterTerm   
			  OR AmountCodeName LIKE @FilterTerm  			
			  OR CustomerViewTypeID LIKE @FilterTerm  			
			  OR StatusTypeID LIKE @FilterTerm  			
			  OR AmountCodeConstant LIKE @FilterTerm  			   
			)                      				   
                      
       SELECT  AmountCode ,AmountCodeName,isnull(AmountCodeConstant,0) as AmountCodeConstant,
	          CASE CustomerViewTypeID WHEN 0 THEN 'Yes' WHEN 1 THEN 'No' END  AS CustomerViewTypeID
			, CASE StatusTypeID WHEN 0 THEN 'Active' WHEN 1 THEN 'InActive' END  AS StatusTypeID,
	           AmountCodeID
	      FROM    @TableMaster
	   
   SELECT @TotalRowsCount = COUNT(*)
   FROM  MasterAmountCode
	
   
   SELECT @FilteredRowsCount = COUNT(*)
   FROM   @TableMaster
END
GO
