USE [MCT_ERP]
GO
/****** Object:  StoredProcedure [dbo].[pr_Search_Masters_UnitDetails]    Script Date: 28/02/2022 19:34:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Yamuna J>
-- Create date: <23-02-2022>
-- Description:	<Unit Master Details>
-- =============================================
/*
declare @tcnt int, @fcnt int
exec [pr_Search_Masters_UnitDetails] null,1,'asc',1,10, @tcnt  output, @fcnt output
*/
CREATE PROCEDURE [dbo].[pr_Search_Masters_UnitDetails]
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
	   
	   UNITDESC Varchar(100),
	   UNITCODE Varchar(100),	  	  
	   DISPSTATUS Int,	   
	   UNITID Int,	  
	   RowNum Int	 
	 )

	  INSERT INTO @TableMaster(UNITDESC ,UNITCODE, DISPSTATUS, UNITID, RowNum)
                 SELECT          UNITMASTER.UNITDESC ,UNITMASTER.UNITCODE,
	                              UNITMASTER.DISPSTATUS,
								 UNITMASTER.UNITID,
								 Row_Number() OVER (
	            ORDER BY          
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/           
           
			 CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
				  
				  WHEN 0 THEN (CASE UNITMASTER.DISPSTATUS WHEN 0 THEN 'Enabled' when 1 then 'Disabled'  END)     
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN (CASE UNITMASTER.DISPSTATUS WHEN 0 THEN 'Enabled' when 1 then 'Disabled'  END)     
                END
            END DESC ,
			 
			 CASE @SortDirection
              WHEN 'ASC'  THEN  
			    CASE @SortIndex
				   WHEN 1 THEN UNITMASTER.UNITDESC
				   WHEN 2 THEN UNITMASTER.UNITCODE
				  
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
				 WHEN 1 THEN UNITMASTER.UNITDESC
				 WHEN 2 THEN UNITMASTER.UNITCODE
				 
                END
            END DESC
            /*DATETIME ORDER BY*/                        
            ) AS RowNum
    FROM   UNITMASTER
	        

 /* CHANGE  TABLE NAME */
    WHERE    (@FilterTerm IS NULL  
              
			   OR UNITMASTER.UNITDESC LIKE @FilterTerm  
			   OR UNITMASTER.UNITCODE LIKE @FilterTerm  
			   
				  )
                      				   
                      
       SELECT  UNITCODE ,UNITDESC, CASE DISPSTATUS WHEN 0 THEN 'Enabled' when 1 then 'Disabled'  END AS DISPSTATUS,
	           UNITID
	      FROM    @TableMaster
	   
   SELECT @TotalRowsCount = COUNT(*)
   FROM  UNITMASTER
	
   
   SELECT @FilteredRowsCount = COUNT(*)
   FROM   @TableMaster
        


END

