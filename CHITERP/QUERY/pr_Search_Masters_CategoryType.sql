USE [MCT_ERP]
GO
/****** Object:  StoredProcedure [dbo].[pr_Search_Masters_CategoryType]    Script Date: 23/02/2022 11:51:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Yamuna J>
-- Create date: <23-02-2022>
-- Description:	<Category Type Master Details>
-- =============================================
/*
declare @tcnt int, @fcnt int
exec [pr_Search_Masters_CategoryType] null,1,'asc',1,10, @tcnt  output, @fcnt output
*/
CREATE PROCEDURE [dbo].[pr_Search_Masters_CategoryType]
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
	   
	   CATETDESC Varchar(100),
	   CATETCODE Varchar(100),	  	  
	   DISPSTATUS Int,	   
	   CATETID Int,	  
	   RowNum Int	 
	 )

	  INSERT INTO @TableMaster(CATETDESC ,CATETCODE, DISPSTATUS, CATETID, RowNum)
                 SELECT          CATEGORYTYPEMASTER.CATETDESC ,CATEGORYTYPEMASTER.CATETCODE,
	                              CATEGORYTYPEMASTER.DISPSTATUS,
								 CATEGORYTYPEMASTER.CATETID,
								 Row_Number() OVER (
	            ORDER BY          
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/           
           
			 CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
				  
				  WHEN 0 THEN (CASE CATEGORYTYPEMASTER.DISPSTATUS WHEN 0 THEN 'Enabled' when 1 then 'Disabled'  END)     
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN (CASE CATEGORYTYPEMASTER.DISPSTATUS WHEN 0 THEN 'Enabled' when 1 then 'Disabled'  END)     
                END
            END DESC ,
			 
			 CASE @SortDirection
              WHEN 'ASC'  THEN  
			    CASE @SortIndex
				   WHEN 1 THEN CATEGORYTYPEMASTER.CATETDESC
				   WHEN 2 THEN CATEGORYTYPEMASTER.CATETCODE
				  
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
				 WHEN 1 THEN CATEGORYTYPEMASTER.CATETDESC
				 WHEN 2 THEN CATEGORYTYPEMASTER.CATETCODE
				 
                END
            END DESC
            /*DATETIME ORDER BY*/                        
            ) AS RowNum
    FROM   CATEGORYTYPEMASTER
	        

 /* CHANGE  TABLE NAME */
    WHERE    (@FilterTerm IS NULL  
              
			   OR CATEGORYTYPEMASTER.CATETDESC LIKE @FilterTerm  
			   OR CATEGORYTYPEMASTER.CATETCODE LIKE @FilterTerm  
			   
				  )
                      				   
                      
       SELECT  CATETCODE ,CATETDESC, CASE DISPSTATUS WHEN 0 THEN 'Enabled' when 1 then 'Disabled'  END AS DISPSTATUS,
	           CATETID
	      FROM    @TableMaster
	   
   SELECT @TotalRowsCount = COUNT(*)
   FROM  CATEGORYTYPEMASTER
	
   
   SELECT @FilteredRowsCount = COUNT(*)
   FROM   @TableMaster
        


END

