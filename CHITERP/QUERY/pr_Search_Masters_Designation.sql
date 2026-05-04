USE [MCT_ERP]
GO
/****** Object:  StoredProcedure [dbo].[pr_Search_Masters_Designation]    Script Date: 23/02/2022 11:51:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Yamuna J>
-- Create date: <23-02-2022>
-- Description:	<Designation Master Details>
-- =============================================
/*
declare @tcnt int, @fcnt int
exec [pr_Search_Masters_Designation] null,1,'asc',1,10, @tcnt  output, @fcnt output
*/
CREATE PROCEDURE [dbo].[pr_Search_Masters_Designation]
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
	   
	   DSGNDESC Varchar(100),
	   DSGNCODE Varchar(100),	  	  
	   DISPSTATUS Int,	   
	   DSGNID Int,	  
	   RowNum Int	 
	 )

	  INSERT INTO @TableMaster(DSGNDESC ,DSGNCODE, DISPSTATUS, DSGNID, RowNum)
                 SELECT          DESIGNATIONMASTER.DSGNDESC ,DESIGNATIONMASTER.DSGNCODE,
	                              DESIGNATIONMASTER.DISPSTATUS,
								 DESIGNATIONMASTER.DSGNID,
								 Row_Number() OVER (
	            ORDER BY          
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/           
           
			 CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
				  
				  WHEN 0 THEN (CASE DESIGNATIONMASTER.DISPSTATUS WHEN 0 THEN 'Enabled' when 1 then 'Disabled'  END)     
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN (CASE DESIGNATIONMASTER.DISPSTATUS WHEN 0 THEN 'Enabled' when 1 then 'Disabled'  END)     
                END
            END DESC ,
			 
			 CASE @SortDirection
              WHEN 'ASC'  THEN  
			    CASE @SortIndex
				   WHEN 1 THEN DESIGNATIONMASTER.DSGNDESC
				   WHEN 2 THEN DESIGNATIONMASTER.DSGNCODE
				  
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
				 WHEN 1 THEN DESIGNATIONMASTER.DSGNDESC
				 WHEN 2 THEN DESIGNATIONMASTER.DSGNCODE
				 
                END
            END DESC
            /*DATETIME ORDER BY*/                        
            ) AS RowNum
    FROM   DESIGNATIONMASTER
	        

 /* CHANGE  TABLE NAME */
    WHERE    (@FilterTerm IS NULL  
              
			   OR DESIGNATIONMASTER.DSGNDESC LIKE @FilterTerm  
			   OR DESIGNATIONMASTER.DSGNCODE LIKE @FilterTerm  
			   
				  )
                      				   
                      
       SELECT  DSGNCODE ,DSGNDESC, CASE DISPSTATUS WHEN 0 THEN 'Enabled' when 1 then 'Disabled'  END AS DISPSTATUS,
	           DSGNID
	      FROM    @TableMaster
	   
   SELECT @TotalRowsCount = COUNT(*)
   FROM  DESIGNATIONMASTER
	
   
   SELECT @FilteredRowsCount = COUNT(*)
   FROM   @TableMaster
        


END

