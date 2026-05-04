USE [MCT_ERP]
GO
/****** Object:  StoredProcedure [dbo].[pr_Search_Masters_Employee]    Script Date: 25/02/2022 11:06:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Yamuna J>
-- Create date: <25-02-2022>
-- Description:	<Employee Master Details>
-- =============================================
/*
declare @tcnt int, @fcnt int
exec [pr_Search_Masters_Employee] null,1,'asc',1,10, @tcnt  output, @fcnt output
*/
CREATE PROCEDURE pr_Search_Masters_Employee  /* CHANGE */
 @FilterTerm nvarchar(250) = NULL --parameter to search all columns by
        , @SortIndex INT = 1 -- a one based index to indicate which column to order by
        , @SortDirection CHAR(4) = 'ASC' --the direction to sort in, either ASC or DESC
        , @StartRowNum INT = 1 --the first row to return
        , @EndRowNum INT = 10 --the last row to return
        , @TotalRowsCount INT OUTPUT
        , @FilteredRowsCount INT OUTPUT
 
AS BEGIN
    --Wrap filter term with % to search for values that contain @FilterTerm
    SET @FilterTerm = '%' + @FilterTerm + '%'

    DECLARE @TableMaster TABLE
    ( 
	    CATECODE VARCHAR(100)
	  , CATENAME VARCHAR(150)
	  , DSGNDESC VARCHAR(150)
	  , DEPTDESC VARCHAR(150)
	  , CATECPNAME VARCHAR(150)
      , CATEPHN3 VARCHAR(50)
	  , CATEEMAIL VARCHAR(150)
	  , DISPSTATUS smallint
	  , CATEID int 
      , RowNum INT
    )

    INSERT INTO @TableMaster(CATECODE,CATENAME,DSGNDESC,DEPTDESC,CATECPNAME,CATEPHN3,CATEEMAIL,DISPSTATUS,CATEID, RowNum)
    SELECT  CATECODE, CATENAME, DSGNDESC,DEPTDESC, CATECPNAME, CATEPHN3, CATEEMAIL, EMPLOYEEMASTER.DISPSTATUS,CATEID,Row_Number() OVER (
            ORDER BY
            
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
                  WHEN 0 THEN CATECODE
                  WHEN 1 THEN CATENAME
				  WHEN 2 THEN DSGNDESC 
				  WHEN 3 THEN DEPTDESC 
				  WHEN 4 THEN CATECPNAME
				  WHEN 5 THEN CATEPHN3
	              WHEN 6 THEN CATEEMAIL
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN CATECODE
                  WHEN 1 THEN CATENAME
				  WHEN 2 THEN DSGNDESC 
				  WHEN 3 THEN DEPTDESC 
				  WHEN 4 THEN CATECPNAME
				  WHEN 5 THEN CATEPHN3
	              WHEN 6 THEN CATEEMAIL
                END
         END DESC 
	 
			
            
            /*DATETIME ORDER BY*/     
            ) AS RowNum
    FROM   dbo.EMPLOYEEMASTER LEFT OUTER JOIN  
	       dbo.DESIGNATIONMASTER ON dbo.EMPLOYEEMASTER.DSGNID = dbo.DESIGNATIONMASTER.DSGNID  Left Outer Join 
		   dbo.DEPARTMENTMASTER  ON dbo.EMPLOYEEMASTER.DEPTID = dbo.DEPARTMENTMASTER.DEPTID  
 /* CHANGE  TABLE NAME */
  WHERE  (@FilterTerm IS NULL 
   
              OR CATECODE LIKE @FilterTerm  
              OR CATENAME LIKE @FilterTerm  
			  OR DSGNDESC LIKE @FilterTerm  
			  OR DEPTDESC LIKE @FilterTerm  
			  OR CATECPNAME LIKE @FilterTerm  
			  OR CATEPHN3 LIKE @FilterTerm 
			  OR CATEEMAIL LIKE @FilterTerm )
			
			             		
    SELECT CATECODE ,CATENAME ,Isnull(DSGNDESC,'') as DSGNDESC , Isnull(DEPTDESC,'') as DEPTDESC ,
	       Isnull(CATECPNAME,'') as CATECPNAME,Isnull(CATEPHN3,'') as CATEPHN3,
		   Isnull(CATEEMAIL,'') as CATEEMAIL	        
	       ,CASE DISPSTATUS WHEN 1 THEN 'Disabled' WHEN 0 THEN 'Enabled' END  AS DISPSTATUS 
		   ,CATEID
    FROM    @TableMaster
    --WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum
    
    SELECT @TotalRowsCount = COUNT(*)
	FROM  dbo.EMPLOYEEMASTER LEFT OUTER JOIN  
	      dbo.DESIGNATIONMASTER ON dbo.EMPLOYEEMASTER.DSGNID = dbo.DESIGNATIONMASTER.DSGNID Left Outer Join 
		  dbo.DEPARTMENTMASTER  ON dbo.EMPLOYEEMASTER.DEPTID = dbo.DEPARTMENTMASTER.DEPTID  

	    /* CHANGE  TABLE NAME */
   
    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster
        
END
