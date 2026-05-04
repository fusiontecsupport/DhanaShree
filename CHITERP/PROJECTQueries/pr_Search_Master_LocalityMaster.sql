-- =============================================
-- Author:		<Rajesh S>
-- Create date: <24/09/2022 10:50>
-- Description:	<Chit Scheme master details>
-- Exec [dbo].[pr_Search_Master_LocalityMaster] @FilterTerm='',@SortIndex=1,@SortDirection='desc',@StartRowNum=1,@EndRowNum=10,@TotalRowsCount=100,@FilteredRowsCount=100,@CUSRID='admin'
-- =============================================
alter PROCEDURE [dbo].[pr_Search_Master_LocalityMaster]
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
	LocalityID int,	
	LocalityName varchar(100),
	  --, DISPSTATUS smallint
	  --, CreatedBy varchar(50)
	  --, CreatedDt datetime
	  --, UpdatedBy varchar(50)
	  --, UpdatedDt DateTime
	  
      RowNum INT
    )

	INSERT INTO @TableMaster( CityID, CityName, LocalityID, LocalityName, 	
	--DISPSTATUS,CreatedBy,CreatedDt,UpdatedBy,UpdatedDt, CONTID, 
	RowNum)   

	SELECT  Lty.CityID, Cty.CityName, Lty.LocalityID, LocalityName, 
			--, DISPSTATUS,	ContactAddressLocality.CreatedBy,ContactAddressLocality.CreatedDt,ContactAddressLocality.UpdatedBy,ContactAddressLocality.UpdatedDt,
		   Row_Number() OVER (
            ORDER BY
            
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
                  WHEN 0 THEN CityName
				  WHEN 1 THEN LocalityName
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                   WHEN 0 THEN CityName
				  WHEN 1 THEN LocalityName
                END
            END DESC 
            
            /*DATETIME ORDER BY*/       
            ) AS RowNum
    FROM  ContactAddressLocality Lty 
			join ContactAddressCity  Cty(nolock) on Lty.CityID = Cty.CityID
	WHERE cty.countryid =1  and
	(@FilterTerm IS NULL             
              OR LocalityName LIKE @FilterTerm  			  
              OR CityName LIKE @FilterTerm )
			  			  
	SELECT	CityID, CityName, LocalityID, LocalityName 
    FROM    @TableMaster
    --WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum
    
    SELECT @TotalRowsCount = COUNT(*)
    FROM   ContactAddressLocality  Lty(nolock) join ContactAddressCity  Cty(nolock) on Lty.CityID = Cty.CityID
	WHERE cty.countryid =1  
	--WHERE  (CreatedBy = @CUSRID or @CUSRID ='')
	       
   /* CHANGE  TABLE NAME */
    
    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster

END

