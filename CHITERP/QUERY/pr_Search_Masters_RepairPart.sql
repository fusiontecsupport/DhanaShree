USE [MCT_ERP]
GO
/****** Object:  StoredProcedure [dbo].[pr_Search_Masters_RepairPart]    Script Date: 02/03/2022 15:17:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Yamuna J>
-- Create date: <23-02-2022>
-- Description:	<Repair Part Master Details>
-- =============================================
/*
declare @tcnt int, @fcnt int
exec [pr_Search_Masters_RepairPart] null,1,'asc',1,1500, @tcnt  output, @fcnt output
*/
ALTER PROCEDURE [dbo].[pr_Search_Masters_RepairPart]
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
	   
	    RPRTCODE VARCHAR(100)
      , RPRTDESC VARCHAR(250)	  
	  , DISPSTATUS Int  
	  , RPRTID Int	  
	  , RowNum Int	 
	 )

	  INSERT INTO @TableMaster(RPRTCODE ,RPRTDESC, DISPSTATUS, RPRTID, RowNum)
                 SELECT          REPAIRPARTMASTER.RPRTCODE ,REPAIRPARTMASTER.RPRTDESC,
	                              REPAIRPARTMASTER.DISPSTATUS,
								 REPAIRPARTMASTER.RPRTID,
								 Row_Number() OVER (
	            ORDER BY          
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/           
           
			 CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
				  
				  WHEN 0 THEN (CASE REPAIRPARTMASTER.DISPSTATUS WHEN 0 THEN 'Enabled' when 1 then 'Disabled'  END)     
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN (CASE REPAIRPARTMASTER.DISPSTATUS WHEN 0 THEN 'Enabled' when 1 then 'Disabled'  END)     
                END
            END DESC ,
			 
			 CASE @SortDirection
              WHEN 'ASC'  THEN  
			    CASE @SortIndex
				    WHEN 0 THEN RPRTCODE
                  WHEN 1 THEN RPRTDESC	
				  
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
				  WHEN 0 THEN RPRTCODE
                  WHEN 1 THEN RPRTDESC	
				 
                END
            END DESC
            /*DATETIME ORDER BY*/                        
            ) AS RowNum
    FROM   REPAIRPARTMASTER
	        

 /* CHANGE  TABLE NAME */
    WHERE    (@FilterTerm IS NULL  
              
			   OR REPAIRPARTMASTER.RPRTCODE LIKE @FilterTerm  
			   OR REPAIRPARTMASTER.RPRTDESC LIKE @FilterTerm  
			   
				  )
                      				   
                      
       SELECT  RPRTCODE ,RPRTDESC, CASE DISPSTATUS WHEN 0 THEN 'Enabled' when 1 then 'Disabled'  END AS DISPSTATUS,
	           RPRTID
	      FROM    @TableMaster
	   
   SELECT @TotalRowsCount = COUNT(*)
   FROM  REPAIRPARTMASTER
	
   
   SELECT @FilteredRowsCount = COUNT(*)
   FROM   @TableMaster
        


END

