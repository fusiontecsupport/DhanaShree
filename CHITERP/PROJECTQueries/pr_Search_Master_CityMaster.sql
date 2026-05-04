-- =============================================
-- Author:		<Rajesh S>
-- Create date: <24/09/2022 10:50>
-- Description:	<Chit Scheme master details>
-- Exec [dbo].[pr_Search_Master_CityMaster] @FilterTerm='',@SortIndex=1,@SortDirection='desc',@StartRowNum=1,@EndRowNum=10,@TotalRowsCount=100,@FilteredRowsCount=100,@CUSRID='admin'
-- =============================================
alter PROCEDURE [dbo].[pr_Search_Master_CityMaster]
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

	--if @EndRowNum=0
	--set @EndRowNum=10

    DECLARE @TableMaster TABLE
    ( 
	CityID int,	
	CityName varchar(100),
	CountryID int,	
	CountryName varchar(100),
	  --, DISPSTATUS smallint
	  --, CreatedBy varchar(50)
	  --, CreatedDt datetime
	  --, UpdatedBy varchar(50)
	  --, UpdatedDt DateTime
	  
      RowNum INT
    )

	INSERT INTO @TableMaster( CityID, CityName, CountryID, CountryName, 	
	--DISPSTATUS,CreatedBy,CreatedDt,UpdatedBy,UpdatedDt, CONTID, 
	RowNum)   

	SELECT  ct.CityID, ct.CityName, ct.CountryID, CountryName, 
			--, DISPSTATUS,	ContactAddressCity.CreatedBy,ContactAddressCity.CreatedDt,ContactAddressCity.UpdatedBy,ContactAddressCity.UpdatedDt,
		   Row_Number() OVER (
            ORDER BY
            
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
                  WHEN 0 THEN CityName
				  WHEN 1 THEN CountryName
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                   WHEN 0 THEN CityName
				  WHEN 1 THEN CountryName
                END
            END DESC 
            
            /*DATETIME ORDER BY*/       
            ) AS RowNum
    FROM  ContactAddressCity ct 
			join ContactAddressCountry  Cnt(nolock) on ct.CountryID = Cnt.CountryID
	WHERE ct.countryid =1 
	and (@FilterTerm IS NULL             
              OR CountryName LIKE @FilterTerm  			  
              OR CityName LIKE @FilterTerm )
			  			  
	SELECT	CityID, CityName, CountryID, CountryName 
    FROM    @TableMaster
    --WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum
    
    SELECT @TotalRowsCount = COUNT(*)
    FROM   ContactAddressCity  (nolock)
	WHERE countryid =1 
	--WHERE  (CreatedBy = @CUSRID or @CUSRID ='')
	       
   /* CHANGE  TABLE NAME */
    
    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster

END

