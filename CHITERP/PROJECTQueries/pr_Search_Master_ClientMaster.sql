-- =============================================  
-- Author:  <Yamuna J>  
-- Create date: <30/05/2022>  
-- Description: <Call master details>  
-- Exec [dbo].[pr_Search_Master_ClientMaster] @FilterTerm='',@SortIndex=1,@SortDirection='asc',@StartRowNum=1,@EndRowNum=10,@TotalRowsCount=100,@FilteredRowsCount=100,@CUSRID='admin'  
-- =============================================  
ALTER PROCEDURE [dbo].[pr_Search_Master_ClientMaster]  
 @FilterTerm nvarchar(250) = NULL --parameter to search all columns by    
  , @SortIndex INT = 1 -- a one based index to indicate which column to order by  
  , @SortDirection CHAR(4) = 'ASC' --the direction to sort in, either ASC or DESC  
  , @StartRowNum INT = 1 --the first row to return  
  , @EndRowNum INT = 10 --the last row to return  
  , @TotalRowsCount INT OUTPUT  
  , @FilteredRowsCount INT OUTPUT  
  , @CUSRID varchar(100)   
AS  
BEGIN  
 SET @FilterTerm = '%' + @FilterTerm + '%'  
  
    DECLARE @TableMaster TABLE  
    (   
	 CLIENTNAME VARCHAR(250) 
   , CLIENTTYPEDESC varchar(50)  
   , CLIENTMOBILENO1 VARCHAR(25)   
   , CLIENTEMAILID VARCHAR(150)   
   , CLIENTCONTACTPERSON VARCHAR(250)            
   , DISPSTATUS smallint  
   , CreatedBy varchar(50)  
   , CreatedDt datetime  
   , UpdatedBy varchar(50)  
   , UpdatedDt DateTime  
   , CLIENTID int   
      , RowNum INT  
    )  
  
 INSERT INTO @TableMaster(CLIENTNAME,CLIENTTYPEDESC,CLIENTMOBILENO1,CLIENTEMAILID,CLIENTCONTACTPERSON, DISPSTATUS,CreatedBy,CreatedDt,UpdatedBy,UpdatedDt, CLIENTID, RowNum)     
  
 SELECT CLIENTNAME,case when CLIENTTID=1 then 'Individual' Else 'Company' end as CLIENTTYPEDESC,CLIENTMOBILENO1,CLIENTEMAILID,CLIENTCONTACTPERSON, CLIENTMASTER.DISPSTATUS,  
 CLIENTMASTER.CreatedBy,CLIENTMASTER.CreatedDt,CLIENTMASTER.UpdatedBy,CLIENTMASTER.UpdatedDt, CLIENTID           
     , Row_Number() OVER (  
            ORDER BY  
              
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/  
              
            CASE @SortDirection  
              WHEN 'ASC'  THEN  
                CASE @SortIndex  
                  WHEN 0 THEN CLIENTNAME     
				  WHEN 1 THEN (case when CLIENTTID=1 then 'Individual' Else 'Company' end)
				  WHEN 2 THEN CLIENTMOBILENO1    
				  WHEN 3 THEN CLIENTEMAILID    
				  WHEN 4 THEN CLIENTCONTACTPERSON  
                END               
            END ASC,  
            CASE @SortDirection  
              WHEN 'DESC' THEN   
                CASE @SortIndex  
                  WHEN 0 THEN CLIENTNAME     
				  WHEN 1 THEN  (case when CLIENTTID=1 then 'Individual' Else 'Company' end) 
				  WHEN 2 THEN CLIENTMOBILENO1    
				  WHEN 3 THEN CLIENTEMAILID    
				  WHEN 4 THEN CLIENTCONTACTPERSON 
                END  
            END DESC   
              
            /*DATETIME ORDER BY*/         
            ) AS RowNum  
	FROM dbo.ClientMaster 
   WHERE  (ClientMaster.CreatedBy = @CUSRID or @CUSRID ='')  
   and (@FilterTerm IS NULL     
        OR CLIENTNAME LIKE @FilterTerm    
        OR (case when CLIENTTID=1 then 'Individual' Else 'Company' end) LIKE @FilterTerm    
		OR CLIENTMOBILENO1 LIKE @FilterTerm    
		OR CLIENTEMAILID LIKE @FilterTerm    
		OR CLIENTCONTACTPERSON LIKE @FilterTerm  )  
            
 SELECT CLIENTNAME,CLIENTTYPEDESC,CLIENTMOBILENO1,CLIENTEMAILID,CLIENTCONTACTPERSON,  
        CASE DISPSTATUS WHEN 1 THEN 'Disabled' WHEN 0 THEN 'Enabled' END  AS DISPSTATUS ,isnull(CreatedBy,'') as CreatedBy ,  
     isnull(Convert(varchar(10),CreatedDt,103),'') as CreatedDt,isnull(UpdatedBy,'') as UpdatedBy, isnull(Convert(varchar(10),UpdatedDt,103),'') as UpdatedDt  , CLIENTID   
    FROM    @TableMaster  
    --WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum  
      
    SELECT @TotalRowsCount = COUNT(*)  
	FROM dbo.ClientMaster (nolock) 
	WHERE  (ClientMaster.CreatedBy = @CUSRID or @CUSRID ='')  
          
   /* CHANGE  TABLE NAME */  
      
    SELECT @FilteredRowsCount = COUNT(*)  
    FROM   @TableMaster  
  
END  