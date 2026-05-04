-- =============================================
-- Author:		<Rajesh S>
-- Create date: <29/07/2022 16:00>
-- Description:	<Loan Scheme master details>
-- Exec [dbo].[pr_Search_Master_LoanScheme] @FilterTerm='',@SortIndex=1,@SortDirection='desc',@StartRowNum=1,@EndRowNum=10,@TotalRowsCount=100,@FilteredRowsCount=100,@CUSRID='admin'
-- =============================================
alter PROCEDURE [dbo].[pr_Search_Master_LoanScheme]
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
	LoanSchemeID int,
	LoanTypeID int,
	LoanSchemeCode varchar(100),
	LoanSchemeName varchar(100),
	BookVerificationIntervalTypeId int,
	AmountCodeID int,
	StatusTypeID int,
	LoanTypeDesc varchar(100),
	BookVerificationIntervalTypeDesc varchar(100),
	AmountCodeDesc varchar(100),
	StatusTypeDesc varchar(100)
	  --, DISPSTATUS smallint
	  --, CreatedBy varchar(50)
	  --, CreatedDt datetime
	  --, UpdatedBy varchar(50)
	  --, UpdatedDt DateTime
	  
      , RowNum INT
    )

	INSERT INTO @TableMaster( LoanSchemeID, LoanSchemeCode, LoanSchemeName, BookVerificationIntervalTypeId, 
	AmountCodeID, StatusTypeID , LoanTypeDesc ,	BookVerificationIntervalTypeDesc , AmountCodeDesc , StatusTypeDesc,
	--DISPSTATUS,CreatedBy,CreatedDt,UpdatedBy,UpdatedDt, CONTID, 
	RowNum)   

	SELECT  LoanSchemeID, SchemeCode, SchemeName, BookVerificationIntervalTypeId, lns.AmountCodeID, 
	lns.StatusTypeID , case when tenurebased = 1 then 'Tenure Based'
							when emibased = 1 then 'EMI Based' else '' end, 
							d.type  BookVerificationIntervalTypeDesc ,e.AmountCodeName AmountCodeDesc , f.type StatusTypeDesc,
	--, DISPSTATUS,	LoanScheme.CreatedBy,LoanScheme.CreatedDt,LoanScheme.UpdatedBy,LoanScheme.UpdatedDt, CONTID 	       
		   Row_Number() OVER (
            ORDER BY
            
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
                  WHEN 0 THEN e.AmountCodeName
				  WHEN 1 THEN SchemeCode   
				  WHEN 2 THEN SchemeName  
				  WHEN 3 THEN d.type  
				  WHEN 4 THEN f.type
				  
                  				  	
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN e.AmountCodeName
				  WHEN 1 THEN SchemeCode   
				  WHEN 2 THEN SchemeName  
				  WHEN 3 THEN d.type  
				  WHEN 4 THEN f.type
				  
                  				  
                END
            END DESC 
            
            /*DATETIME ORDER BY*/       
            ) AS RowNum
    FROM  LoanScheme  lns(nolock) 
			left join MasterType d(nolock) on lns.BookVerificationIntervalTypeId = d.TypeID
			left join MasterAmountCode e(nolock) on lns.AmountCodeID = e.AmountCodeID
			left join MasterType f(nolock) on lns.StatusTypeID = f.TypeID

   WHERE -- (CreatedBy = @CUSRID or @CUSRID ='')   and 
   (@FilterTerm IS NULL   
              OR SchemeCode LIKE @FilterTerm  
              OR SchemeName LIKE @FilterTerm  
			  OR d.type LIKE @FilterTerm  
			  OR e.AmountCodeName LIKE @FilterTerm  
			  OR f.type LIKE @FilterTerm  )
			  			  
	SELECT LoanSchemeID, LoanSchemeCode, LoanSchemeName, BookVerificationIntervalTypeId, 
	AmountCodeID, StatusTypeID ,  LoanTypeDesc ,    BookVerificationIntervalTypeDesc ,
	 AmountCodeDesc ,  StatusTypeDesc
    FROM    @TableMaster
    --WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum
    
    SELECT @TotalRowsCount = COUNT(*)
    FROM   LoanScheme  (nolock)
	--WHERE  (CreatedBy = @CUSRID or @CUSRID ='')
	       
   /* CHANGE  TABLE NAME */
    
    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster

END

