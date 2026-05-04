-- =============================================  
-- Author:  <Yamuna J>  
-- Create date: <30/05/2022>  
-- Description: <Call master details>  
-- Exec [dbo].[pr_Search_Company_OfficeMaster] @FilterTerm='',@SortIndex=1,@SortDirection='asc',@StartRowNum=1,@EndRowNum=10,@TotalRowsCount=100,@FilteredRowsCount=100,@CUSRID='admin'  
-- =============================================  
alter PROCEDURE [dbo].[pr_Search_Company_OfficeMaster]  
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
	 OfficeName VARCHAR(100) 
   , TypeDesc varchar(100)  
   , CityName VARCHAR(100)   
   , LocalityName VARCHAR(100)          
   , StatusTypeID smallint  
   , OfficeID int   
      , RowNum INT  
    )  
  
 INSERT INTO @TableMaster(OfficeName,TypeDesc,CityName,LocalityName,StatusTypeID, OfficeID,RowNum)     
  
 SELECT OfficeName,Type,CityName,LocalityName,StatusTypeID, OfficeID,           
     Row_Number() OVER (  
            ORDER BY  
              
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/  
              
            CASE @SortDirection  
              WHEN 'ASC'  THEN  
                CASE @SortIndex  
                  WHEN 0 THEN OfficeName     
				  WHEN 1 THEN Type  
				  WHEN 2 THEN CityName    
				  WHEN 3 THEN LocalityName    
                END               
            END ASC,  
            CASE @SortDirection  
              WHEN 'DESC' THEN   
                CASE @SortIndex  
                  WHEN 0 THEN OfficeName     
				  WHEN 1 THEN Type 
				  WHEN 2 THEN CityName    
				  WHEN 3 THEN LocalityName    
                END  
            END DESC   
              
            /*DATETIME ORDER BY*/         
            ) AS RowNum  
	FROM dbo.CompanyOffice 
	INNER JOIN dbo.MasterType ON dbo.CompanyOffice.OfficeTypeID = dbo.MasterType.TypeID 
	INNER JOIN dbo.ContactAddressLocality ON dbo.CompanyOffice.LocalityID = dbo.ContactAddressLocality.LocalityID 
	INNER JOIN dbo.ContactAddressCity ON dbo.CompanyOffice.CityID = dbo.ContactAddressCity.CityID
   WHERE  (@FilterTerm IS NULL     
        OR OfficeName LIKE @FilterTerm    
        OR Type LIKE @FilterTerm    
		OR CityName LIKE @FilterTerm    
		OR LocalityName LIKE @FilterTerm)  
            
 SELECT OfficeName,TypeDesc,CityName,LocalityName,StatusTypeID, 
        CASE StatusTypeID WHEN 302 THEN 'Disabled' WHEN 304 THEN 'Enabled' END  AS DISPSTATUS,  OfficeID   
    FROM    @TableMaster  
    --WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum  
      
    SELECT @TotalRowsCount = COUNT(*)  
	FROM dbo.CompanyOffice 
	INNER JOIN dbo.MasterType ON dbo.CompanyOffice.OfficeTypeID = dbo.MasterType.TypeID 
	INNER JOIN dbo.ContactAddressLocality ON dbo.CompanyOffice.LocalityID = dbo.ContactAddressLocality.LocalityID 
	INNER JOIN dbo.ContactAddressCity ON dbo.CompanyOffice.CityID = dbo.ContactAddressCity.CityID
          
   /* CHANGE  TABLE NAME */  
      
    SELECT @FilteredRowsCount = COUNT(*)  
    FROM   @TableMaster  
  
END  

