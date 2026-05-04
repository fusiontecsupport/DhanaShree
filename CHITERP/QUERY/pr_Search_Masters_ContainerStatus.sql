USE [MCT_ERP]
GO
/****** Object:  StoredProcedure [dbo].[pr_Search_Masters_ContainerStatus]    Script Date: 15/02/2022 18:37:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Yamuna J>
-- Create date: <14-02-2022>
-- Description:	<Container Status Master Details>
-- =============================================
/*
declare @tcnt int, @fcnt int
exec [pr_Search_Masters_ContainerStatus] null,1,'asc',1,10, @tcnt  output, @fcnt output
*/
CREATE PROCEDURE [dbo].[pr_Search_Masters_ContainerStatus]
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
	   
	   CNTNRSDESC Varchar(100),
	   CNTNRSCODE Varchar(100),	  	  
	   DISPSTATUS Int,	   
	   CNTNRSID Int,	  
	   RowNum Int	 
	 )

	  INSERT INTO @TableMaster(CNTNRSDESC ,CNTNRSCODE, DISPSTATUS, CNTNRSID, RowNum)
                 SELECT          CONTAINERSTATUSMASTER.CNTNRSDESC ,CONTAINERSTATUSMASTER.CNTNRSCODE,
	                              CONTAINERSTATUSMASTER.DISPSTATUS,
								 CONTAINERSTATUSMASTER.CNTNRSID,
								 Row_Number() OVER (
	            ORDER BY          
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/           
           
			 CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
				  
				  WHEN 0 THEN (CASE CONTAINERSTATUSMASTER.DISPSTATUS WHEN 0 THEN 'Enabled' when 1 then 'Disabled'  END)     
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN (CASE CONTAINERSTATUSMASTER.DISPSTATUS WHEN 0 THEN 'Enabled' when 1 then 'Disabled'  END)     
                END
            END DESC ,
			 
			 CASE @SortDirection
              WHEN 'ASC'  THEN  
			    CASE @SortIndex
				   WHEN 1 THEN CONTAINERSTATUSMASTER.CNTNRSDESC
				   WHEN 2 THEN CONTAINERSTATUSMASTER.CNTNRSCODE
				  
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
				WHEN 1 THEN CONTAINERSTATUSMASTER.CNTNRSDESC
				WHEN 2 THEN CONTAINERSTATUSMASTER.CNTNRSCODE
				 
                END
            END DESC
            /*DATETIME ORDER BY*/                        
            ) AS RowNum
    FROM   CONTAINERSTATUSMASTER
	        

 /* CHANGE  TABLE NAME */
    WHERE    (@FilterTerm IS NULL  
              
			   OR CONTAINERSTATUSMASTER.CNTNRSDESC  LIKE @FilterTerm  
			   OR CONTAINERSTATUSMASTER.CNTNRSCODE LIKE @FilterTerm   
			   
				  )
                      				   
                      
       SELECT  CNTNRSCODE ,CNTNRSDESC, CASE DISPSTATUS WHEN 0 THEN 'Enabled' when 1 then 'Disabled'  END AS DISPSTATUS,
	           CNTNRSID
	      FROM    @TableMaster
	   
   SELECT @TotalRowsCount = COUNT(*)
   FROM  CONTAINERSTATUSMASTER
	
   
   SELECT @FilteredRowsCount = COUNT(*)
   FROM   @TableMaster
        


END

