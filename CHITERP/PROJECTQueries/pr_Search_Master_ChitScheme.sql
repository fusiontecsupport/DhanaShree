-- =============================================
-- Author:		<Rajesh S>
-- Create date: <16/06/2022 04:30>
-- Description:	<Chit Scheme master details>
-- Exec [dbo].[pr_Search_Master_ChitScheme] @FilterTerm='',@SortIndex=1,@SortDirection='desc',@StartRowNum=1,@EndRowNum=10,@TotalRowsCount=100,@FilteredRowsCount=100,@CUSRID='admin'
-- =============================================
alter PROCEDURE [dbo].[pr_Search_Master_ChitScheme]
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
	ChitSchemeID int,
	ChitTypeID int,
	ChitSchemeCode varchar(100),
	ChitSchemeName varchar(100),
	ChitValue float,
	ChitDuration int,
	DurationTypeId int,
	ForemanCommision float,
	ForemanPrizeInstalment int,
	BidCapAmount float,
	SealingPeriod int,
	AverageDiscount float,
	AverageDivident float,
	ProposedIncome float,
	BookVerificationTypeId int,
	AscertainmentTypeId int,
	AscertainmentFrequencyTypeId int,
	SubscriptionAmount float,
	EligibilityAmount float,
	SubscriberReturns float,
	CommissionEmp float,
	CommissionNonEmp float,
	AmountCodeID int,
	StatusTypeID int,
	ChitTypeDesc varchar(100),
	DurationTypeDesc varchar(100),
	AscertainmentTypeDesc varchar(100),
	AscertainmentFrequencyTypeDesc varchar(100),
	AmountCodeDesc varchar(100),
	StatusTypeDesc varchar(100)
	  --, DISPSTATUS smallint
	  --, CreatedBy varchar(50)
	  --, CreatedDt datetime
	  --, UpdatedBy varchar(50)
	  --, UpdatedDt DateTime
	  
      , RowNum INT
    )

	INSERT INTO @TableMaster( ChitSchemeID, ChitTypeID, ChitSchemeCode, ChitSchemeName, ChitValue, ChitDuration, DurationTypeId, ForemanCommision,
	ForemanPrizeInstalment, BidCapAmount, SealingPeriod, AverageDiscount, AverageDivident, ProposedIncome, BookVerificationTypeId, AscertainmentTypeId,
	AscertainmentFrequencyTypeId, SubscriptionAmount, EligibilityAmount, SubscriberReturns, CommissionEmp, CommissionNonEmp, AmountCodeID, 
	StatusTypeID , ChitTypeDesc ,	DurationTypeDesc ,	AscertainmentTypeDesc , AscertainmentFrequencyTypeDesc , AmountCodeDesc , StatusTypeDesc,
	--DISPSTATUS,CreatedBy,CreatedDt,UpdatedBy,UpdatedDt, CONTID, 
	RowNum)   

	SELECT  ChitSchemeID, ChitTypeID, ChitSchemeCode, ChitSchemeName, ChitValue, ChitDuration, DurationTypeId, ForemanCommision,
	ForemanPrizeInstalment, BidCapAmount, SealingPeriod, AverageDiscount, AverageDivident, ProposedIncome, BookVerificationTypeId, AscertainmentTypeId,
	AscertainmentFrequencyTypeId, SubscriptionAmount, EligibilityAmount, SubscriberReturns, CommissionEmp, CommissionNonEmp, cht.AmountCodeID, 
	cht.StatusTypeID , a.type ChitTypeDesc , b.type  DurationTypeDesc ,	c.type AscertainmentTypeDesc , d.type  AscertainmentFrequencyTypeDesc ,
	e.AmountCodeName AmountCodeDesc , f.type StatusTypeDesc,
	--, DISPSTATUS,	ChitScheme.CreatedBy,ChitScheme.CreatedDt,ChitScheme.UpdatedBy,ChitScheme.UpdatedDt, CONTID 	       
		   Row_Number() OVER (
            ORDER BY
            
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
                  WHEN 0 THEN f.type
				  WHEN 1 THEN ChitSchemeCode   
				  WHEN 2 THEN ChitSchemeName  
				  WHEN 3 THEN a.type  
				  WHEN 4 THEN b.type
				  
                  				  	
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN f.type
				  WHEN 1 THEN ChitSchemeCode   
				  WHEN 2 THEN ChitSchemeName  
				  WHEN 3 THEN a.type  
				  WHEN 4 THEN b.type
				  
                  				  
                END
            END DESC 
            
            /*DATETIME ORDER BY*/       
            ) AS RowNum
    FROM  ChitScheme  cht(nolock) 
			left join MasterType a(nolock) on cht.ChitTypeID = a.TypeID
			left join MasterType b(nolock) on cht.DurationTypeId = b.TypeID
			left join MasterType c(nolock) on cht.AscertainmentTypeId = c.TypeID
			left join MasterType d(nolock) on cht.AscertainmentFrequencyTypeId = d.TypeID
			left join MasterAmountCode e(nolock) on cht.AmountCodeID = e.AmountCodeID
			left join MasterType f(nolock) on cht.StatusTypeID = f.TypeID

   WHERE -- (CreatedBy = @CUSRID or @CUSRID ='')   and 
   (@FilterTerm IS NULL   
              OR ChitSchemeCode LIKE @FilterTerm  
              OR ChitSchemeName LIKE @FilterTerm  
			  OR a.type LIKE @FilterTerm  
			  OR b.type LIKE @FilterTerm  
			  OR f.type LIKE @FilterTerm  )
			  			  
	SELECT ChitSchemeID, ChitTypeID, ChitSchemeCode, ChitSchemeName, ChitValue, ChitDuration, DurationTypeId, ForemanCommision,
	ForemanPrizeInstalment, BidCapAmount, SealingPeriod, AverageDiscount, AverageDivident, ProposedIncome, BookVerificationTypeId, AscertainmentTypeId,
	AscertainmentFrequencyTypeId, SubscriptionAmount, EligibilityAmount, SubscriberReturns, CommissionEmp, CommissionNonEmp, AmountCodeID, 
	StatusTypeID ,  ChitTypeDesc ,    DurationTypeDesc , AscertainmentTypeDesc ,  AscertainmentFrequencyTypeDesc ,
	 AmountCodeDesc ,  StatusTypeDesc
    FROM    @TableMaster
    --WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum
    
    SELECT @TotalRowsCount = COUNT(*)
    FROM   ChitScheme  (nolock)
	--WHERE  (CreatedBy = @CUSRID or @CUSRID ='')
	       
   /* CHANGE  TABLE NAME */
    
    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster

END
