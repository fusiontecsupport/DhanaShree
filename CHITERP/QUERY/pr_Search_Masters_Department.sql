USE [MCT_ERP]
GO
/****** Object:  StoredProcedure [dbo].[pr_Search_Masters_Department]    Script Date: 23/02/2022 11:51:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Yamuna J>
-- Create date: <23-02-2022>
-- Description:	<Department Master Details>
-- =============================================
/*
declare @tcnt int, @fcnt int
exec [pr_Search_Masters_Department] null,1,'asc',1,10, @tcnt  output, @fcnt output
*/
CREATE PROCEDURE [dbo].[pr_Search_Masters_Department]
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
	   
	   DEPTDESC Varchar(100),
	   DEPTCODE Varchar(100),	  	  
	   DISPSTATUS Int,	   
	   DEPTID Int,	  
	   RowNum Int	 
	 )

	  INSERT INTO @TableMaster(DEPTDESC ,DEPTCODE, DISPSTATUS, DEPTID, RowNum)
                 SELECT          DEPARTMENTMASTER.DEPTDESC ,DEPARTMENTMASTER.DEPTCODE,
	                              DEPARTMENTMASTER.DISPSTATUS,
								 DEPARTMENTMASTER.DEPTID,
								 Row_Number() OVER (
	            ORDER BY          
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/           
           
			 CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
				  
				  WHEN 0 THEN (CASE DEPARTMENTMASTER.DISPSTATUS WHEN 0 THEN 'Enabled' when 1 then 'Disabled'  END)     
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN (CASE DEPARTMENTMASTER.DISPSTATUS WHEN 0 THEN 'Enabled' when 1 then 'Disabled'  END)     
                END
            END DESC ,
			 
			 CASE @SortDirection
              WHEN 'ASC'  THEN  
			    CASE @SortIndex
				   WHEN 1 THEN DEPARTMENTMASTER.DEPTDESC
				   WHEN 2 THEN DEPARTMENTMASTER.DEPTCODE
				  
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
				 WHEN 1 THEN DEPARTMENTMASTER.DEPTDESC
				 WHEN 2 THEN DEPARTMENTMASTER.DEPTCODE
				 
                END
            END DESC
            /*DATETIME ORDER BY*/                        
            ) AS RowNum
    FROM   DEPARTMENTMASTER
	        

 /* CHANGE  TABLE NAME */
    WHERE    (@FilterTerm IS NULL  
              
			   OR DEPARTMENTMASTER.DEPTDESC LIKE @FilterTerm  
			   OR DEPARTMENTMASTER.DEPTCODE LIKE @FilterTerm  
			   
				  )
                      				   
                      
       SELECT  DEPTCODE ,DEPTDESC, CASE DISPSTATUS WHEN 0 THEN 'Enabled' when 1 then 'Disabled'  END AS DISPSTATUS,
	           DEPTID
	      FROM    @TableMaster
	   
   SELECT @TotalRowsCount = COUNT(*)
   FROM  DEPARTMENTMASTER
	
   
   SELECT @FilteredRowsCount = COUNT(*)
   FROM   @TableMaster
        


END

