USE [MCT_ERP]
GO
/****** Object:  StoredProcedure [dbo].[pr_Search_Masters_ProductGroup]    Script Date: 04/03/2022 13:41:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Yamuna J>
-- Create date: <23-02-2022>
-- Description:	<Product Group Master Details>
-- =============================================
/*
declare @tcnt int, @fcnt int
exec [pr_Search_Masters_ProductGroup] null,1,'asc',1,10, @tcnt  output, @fcnt output
*/
CREATE PROCEDURE [dbo].[pr_Search_Masters_ProductGroup]
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
	   
	   PRDTGDESC Varchar(100),
	   PRDTGCODE Varchar(100),	  	  
	   DISPSTATUS Int,	   
	   PRDTGID Int,	  
	   RowNum Int	 
	 )

	  INSERT INTO @TableMaster(PRDTGDESC ,PRDTGCODE, DISPSTATUS, PRDTGID, RowNum)
                 SELECT          PRODUCTGROUPMASTER.PRDTGDESC ,PRODUCTGROUPMASTER.PRDTGCODE,
	                              PRODUCTGROUPMASTER.DISPSTATUS,
								 PRODUCTGROUPMASTER.PRDTGID,
								 Row_Number() OVER (
	            ORDER BY          
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/           
           
			 CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
				  
				  WHEN 0 THEN (CASE PRODUCTGROUPMASTER.DISPSTATUS WHEN 0 THEN 'Enabled' when 1 then 'Disabled'  END)     
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN (CASE PRODUCTGROUPMASTER.DISPSTATUS WHEN 0 THEN 'Enabled' when 1 then 'Disabled'  END)     
                END
            END DESC ,
			 
			 CASE @SortDirection
              WHEN 'ASC'  THEN  
			    CASE @SortIndex
				   WHEN 1 THEN PRODUCTGROUPMASTER.PRDTGDESC
				   WHEN 2 THEN PRODUCTGROUPMASTER.PRDTGCODE
				  
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
				 WHEN 1 THEN PRODUCTGROUPMASTER.PRDTGDESC
				 WHEN 2 THEN PRODUCTGROUPMASTER.PRDTGCODE
				 
                END
            END DESC
            /*DATETIME ORDER BY*/                        
            ) AS RowNum
    FROM   PRODUCTGROUPMASTER
	        

 /* CHANGE  TABLE NAME */
    WHERE    (@FilterTerm IS NULL  
              
			   OR PRODUCTGROUPMASTER.PRDTGDESC LIKE @FilterTerm  
			   OR PRODUCTGROUPMASTER.PRDTGCODE LIKE @FilterTerm  
			   
				  )
                      				   
                      
       SELECT  PRDTGCODE ,PRDTGDESC, CASE DISPSTATUS WHEN 0 THEN 'Enabled' when 1 then 'Disabled'  END AS DISPSTATUS,
	           PRDTGID
	      FROM    @TableMaster
	   
   SELECT @TotalRowsCount = COUNT(*)
   FROM  PRODUCTGROUPMASTER
	
   
   SELECT @FilteredRowsCount = COUNT(*)
   FROM   @TableMaster
        


END

