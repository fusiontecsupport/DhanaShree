USE [MCT_ERP]
GO
/****** Object:  StoredProcedure [dbo].[pr_Search_Masters_EmptyResponseCode]    Script Date: 17/02/2022 11:19:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Yamuna J>
-- Create date: <17-02-2022>
-- Description:	<Empty Response Code Master Details>
-- =============================================
/*
declare @tcnt int, @fcnt int
exec [pr_Search_Masters_EmptyResponseCode] null,1,'asc',1,10, @tcnt  output, @fcnt output
*/
CREATE PROCEDURE [dbo].[pr_Search_Masters_EmptyResponseCode]
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
	   RESPCODE Varchar(100),	  
	   RESPDESC Varchar(100),	
	   DISPSTATUS Int,	   
	   RESPID Int,	  
	   RowNum Int	 
	 )

	  INSERT INTO @TableMaster(RESPCODE ,RESPDESC, DISPSTATUS, RESPID, RowNum)
                 SELECT          EMPTY_RESPONSECODEMASTER.RESPCODE ,EMPTY_RESPONSECODEMASTER.RESPDESC,
	                             EMPTY_RESPONSECODEMASTER.DISPSTATUS,
								 EMPTY_RESPONSECODEMASTER.RESPID,
								 Row_Number() OVER (
	            ORDER BY          
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/           
           
			 CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
				  
				  WHEN 0 THEN (CASE DISPSTATUS WHEN 0 THEN 'Enabled' when 1 then 'Disabled'  END)     
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN (CASE DISPSTATUS WHEN 0 THEN 'Enabled' when 1 then 'Disabled'  END)     
                END
            END DESC ,
			 
			 CASE @SortDirection
              WHEN 'ASC'  THEN  
			    CASE @SortIndex
				   WHEN 1 THEN RESPCODE
				   WHEN 2 THEN RESPDESC 
				  
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
				  WHEN 1 THEN RESPCODE  
				   WHEN 2 THEN RESPDESC   
                END
            END DESC
            /*DATETIME ORDER BY*/                        
            ) AS RowNum
    FROM     dbo.EMPTY_RESPONSECODEMASTER
	        

 /* CHANGE  TABLE NAME */
    WHERE    (@FilterTerm IS NULL  
              OR RESPCODE LIKE @FilterTerm  
			   OR RESPDESC LIKE @FilterTerm  			   
				  )
                      				   
                      
       SELECT  RESPCODE ,RESPDESC, CASE DISPSTATUS WHEN 0 THEN 'Enabled' when 1 then 'Disabled'  END AS DISPSTATUS,
	           RESPID
	      FROM    @TableMaster
	   
   SELECT @TotalRowsCount = COUNT(*)
   FROM  EMPTY_RESPONSECODEMASTER
	
   
   SELECT @FilteredRowsCount = COUNT(*)
   FROM   @TableMaster
        


END

