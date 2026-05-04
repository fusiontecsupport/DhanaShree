-- =============================================
-- Author:		<Yamuna J>
-- Create date: <14-02-2022>
-- Description:	<Company Master Details>
-- =============================================
/*
declare @tcnt int, @fcnt int
exec [pr_Search_Masters_CompanyDetails] null,1,'asc',1,10, @tcnt  output, @fcnt output
*/
alter PROCEDURE [dbo].[pr_Search_Masters_CompanyDetails]
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
	   
	   COMPCODE Varchar(100),
	   COMPNAME Varchar(100),	  
	   COMPDNAME Varchar(100),	
	   DISPSTATUS Int,	   
	   COMPID Int,	  
	   RowNum Int	 
	 )

	  INSERT INTO @TableMaster(COMPCODE ,COMPNAME, COMPDNAME, DISPSTATUS, COMPID, RowNum)
                 SELECT          COMPANYMASTER.COMPCODE ,COMPANYMASTER.COMPNAME,
	                             COMPANYMASTER.COMPDNAME,COMPANYMASTER.DISPSTATUS,
								 COMPANYMASTER.COMPID,
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
				   WHEN 1 THEN COMPCODE
				   WHEN 2 THEN COMPNAME 
				   WHEN 3 THEN COMPDNAME  
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
				 WHEN 1 THEN COMPCODE
				   WHEN 2 THEN COMPNAME 
				   WHEN 3 THEN COMPDNAME  
                END
            END DESC
            /*DATETIME ORDER BY*/                        
            ) AS RowNum
    FROM     dbo.COMPANYMASTER
	        

 /* CHANGE  TABLE NAME */
    WHERE    (@FilterTerm IS NULL  
              OR COMPCODE LIKE @FilterTerm
			   OR COMPNAME LIKE @FilterTerm
			    OR COMPDNAME LIKE @FilterTerm
				 --OR ASSIGNEDDATETM LIKE @FilterTerm
				  )
                      				   
                      
       SELECT  COMPCODE ,COMPNAME, COMPDNAME,CASE DISPSTATUS WHEN 0 THEN 'Enabled' when 1 then 'Disabled'  END AS DISPSTATUS,
	           COMPID
	      FROM    @TableMaster
	   
   SELECT @TotalRowsCount = COUNT(*)
   FROM  COMPANYMASTER
	
   
   SELECT @FilteredRowsCount = COUNT(*)
   FROM   @TableMaster
        


END

