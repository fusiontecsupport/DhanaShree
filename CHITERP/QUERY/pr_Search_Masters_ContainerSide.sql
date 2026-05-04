USE [MCT_ERP]
GO
/****** Object:  StoredProcedure [dbo].[pr_Search_Masters_ContainerSide]    Script Date: 15/02/2022 18:09:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Yamuna J>
-- Create date: <14-02-2022>
-- Description:	<Container Side Master Details>
-- =============================================
/*
declare @tcnt int, @fcnt int
exec [pr_Search_Masters_ContainerSide] null,1,'asc',1,10, @tcnt  output, @fcnt output
*/
CREATE PROCEDURE [dbo].[pr_Search_Masters_ContainerSide]
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
	   
	   CSDESC Varchar(100),
	   CSCODE Varchar(100),	  	  
	   DISPSTATUS Int,	   
	   CSID Int,	  
	   RowNum Int	 
	 )

	  INSERT INTO @TableMaster(CSDESC ,CSCODE, DISPSTATUS, CSID, RowNum)
                 SELECT          CONTAINERSIDEMASTER.CSDESC ,CONTAINERSIDEMASTER.CSCODE,
	                              CONTAINERSIDEMASTER.DISPSTATUS,
								 CONTAINERSIDEMASTER.CSID,
								 Row_Number() OVER (
	            ORDER BY          
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/           
           
			 CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
				  
				  WHEN 0 THEN (CASE CONTAINERSIDEMASTER.DISPSTATUS WHEN 0 THEN 'Enabled' when 1 then 'Disabled'  END)     
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN (CASE CONTAINERSIDEMASTER.DISPSTATUS WHEN 0 THEN 'Enabled' when 1 then 'Disabled'  END)     
                END
            END DESC ,
			 
			 CASE @SortDirection
              WHEN 'ASC'  THEN  
			    CASE @SortIndex
				   WHEN 1 THEN CONTAINERSIDEMASTER.CSDESC
				   WHEN 2 THEN CONTAINERSIDEMASTER.CSCODE
				  
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
				 WHEN 1 THEN CONTAINERSIDEMASTER.CSDESC
				 WHEN 2 THEN CONTAINERSIDEMASTER.CSCODE
				 
                END
            END DESC
            /*DATETIME ORDER BY*/                        
            ) AS RowNum
    FROM   CONTAINERSIDEMASTER
	        

 /* CHANGE  TABLE NAME */
    WHERE    (@FilterTerm IS NULL  
              
			   OR CONTAINERSIDEMASTER.CSDESC LIKE @FilterTerm  
			   OR CONTAINERSIDEMASTER.CSCODE LIKE @FilterTerm  
			   
				  )
                      				   
                      
       SELECT  CSCODE ,CSDESC, CASE DISPSTATUS WHEN 0 THEN 'Enabled' when 1 then 'Disabled'  END AS DISPSTATUS,
	           CSID
	      FROM    @TableMaster
	   
   SELECT @TotalRowsCount = COUNT(*)
   FROM  CONTAINERSIDEMASTER
	
   
   SELECT @FilteredRowsCount = COUNT(*)
   FROM   @TableMaster
        


END

