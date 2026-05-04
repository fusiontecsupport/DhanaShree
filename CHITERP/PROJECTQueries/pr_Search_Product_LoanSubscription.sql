-- =============================================
-- Author:		<Rajesh S>
-- Create date: <02/08/2022 12:15>
-- Description:	<Loan Subscription details>
-- Exec [dbo].[pr_Search_Product_LoanSubscription] @FilterTerm='',@SortIndex=1,@SortDirection='desc',@StartRowNum=1,@EndRowNum=10,@TotalRowsCount=100,@FilteredRowsCount=100,@CUSRID='admin'
-- =============================================
alter  PROCEDURE [dbo].[pr_Search_Product_LoanSubscription]
	@FilterTerm nvarchar(250) = NULL --parameter to search all columns by  
  , @SortIndex INT = 1 -- a one based index to indicate which column to order by
  , @SortDirection CHAR(4) = 'ASC' --the direction to sort in, either ASC or DESC
  , @StartRowNum INT = 1 --the first row to return
  , @EndRowNum INT = 10 --the last row to return
  , @TotalRowsCount INT OUTPUT
  , @FilteredRowsCount INT OUTPUT
  , @CUSRID varchar(100)
  , @FrDt datetime=null
  , @ToDt datetime=null
AS
BEGIN
	SET @FilterTerm = '%' + @FilterTerm + '%'

	declare @empid int
	select @empid = EmpId from AspNetUsers(nolock) where UserName = @CUSRID

	if @FrDt=null
	set @FrDt = convert(varchar(10),getdate()-1,120)

	if @ToDt=null
	set @ToDt = convert(varchar(10),getdate(),120)

	--if @EndRowNum=0
	--set @EndRowNum=10

    DECLARE @TableMaster TABLE
    ( 
	LoanSubscriptionID int,
	ClientID int,
	ClientName varchar(150),
	ClientMobileNo varchar(100),
	LoanSchemeCode varchar(100),
	RepaymentIntervalTypeID int,
	InterestDeductionTypeID int,
	StatusTypeID int,
	LoanTypeDesc varchar(100),
	RepaymentIntervalTypeDesc varchar(100),
	InterestDeductionTypeDesc varchar(100),
	StatusTypeDesc varchar(100),
	LoanDuration varchar(50),
	Principal numeric(18,2),
	InterestRate numeric(18,2),
	SubsDate varchar(12),
	  --, DISPSTATUS smallint
	  --, CreatedBy varchar(50)
	  --, CreatedDt datetime
	  --, UpdatedBy varchar(50)
	  --, UpdatedDt DateTime
	  
      RowNum INT
    )

	INSERT INTO @TableMaster( LoanSubscriptionID, ClientID, ClientName, ClientMobileNo,LoanDuration , Principal , InterestRate, 
	StatusTypeID , LoanSchemeCode ,	RepaymentIntervalTypeDesc ,  InterestDeductionTypeDesc, StatusTypeDesc, SubsDate,
	--DISPSTATUS,CreatedBy,CreatedDt,UpdatedBy,UpdatedDt, CONTID, 
	RowNum)   

	SELECT  LoanSubscriptionID, CLIENTID, CLIENTNAME, cl.CLIENTMOBILENO1, cast(lr.Term as varchar(20))+ ' ' + g.Type, lns.LoanAmount, lns.InterestRate,
	lns.StatusTypeID , ls.SchemeCode, d.type  RepaymentIntervalTypeDesc ,e.type  , f.type StatusTypeDesc, convert(varchar(11),lns.DisbursementDate,106),
	--, DISPSTATUS,	LoanSubscription.CreatedBy,LoanSubscription.CreatedDt,LoanSubscription.UpdatedBy,LoanSubscription.UpdatedDt, CONTID 	       
		   Row_Number() OVER (
            ORDER BY
            
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
                  WHEN 0 THEN CLIENTNAME
				  WHEN 1 THEN CLIENTMOBILENO1
				  WHEN 2 THEN SCHEMECODE 
				  WHEN 3 THEN d.type  
				  WHEN 4 THEN E.type
				  WHEN 5 THEN f.type
				  WHEN 6 THEN G.type
				  
                  				  	
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN CLIENTNAME
				  WHEN 1 THEN CLIENTMOBILENO1
				  WHEN 2 THEN SCHEMECODE 
				  WHEN 3 THEN d.type  
				  WHEN 4 THEN E.type
				  WHEN 5 THEN f.type
				  WHEN 6 THEN G.type
				  
                  				  
                END
            END DESC 
            
            /*DATETIME ORDER BY*/       
            ) AS RowNum
    FROM  LoanSubscription  lns(nolock) join LoanScheme ls(nolock) on lns.LoanSchemeID = ls.LoanSchemeID
			join LoanRequest lr(nolock) on lns.LoanRequestID = lr.LoanRequestID
			join clientmaster cl(nolock) on lr.PersonID = cl.clientid
			left join MasterType d(nolock) on lns.RepaymentIntervalTypeID = d.TypeID
			left join MasterType e(nolock) on lns.InterestDeductionTypeID = e.TypeID
			left join MasterType f(nolock) on lns.StatusTypeID = f.TypeID
			left join MasterType g(nolock) on lr.TermTypeID= g.TypeID

   WHERE  (@CUSRID ='' or AccountEmployeeID = @empid)   and 
   lns.DisbursementDate between @FrDt and @ToDt and
   (@FilterTerm IS NULL   
              OR CLIENTNAME LIKE @FilterTerm  
              OR CLIENTMOBILENO1 LIKE @FilterTerm  
			  OR SCHEMECODE LIKE @FilterTerm  
			  OR d.type LIKE @FilterTerm  
			  OR e.type LIKE @FilterTerm  
			  OR f.type LIKE @FilterTerm  
			  OR g.type LIKE @FilterTerm  )
			  			  
	SELECT LoanSubscriptionID, ClientID, ClientName, ClientMobileNo,LoanDuration , Principal , InterestRate, 
	StatusTypeID , LoanSchemeCode ,	RepaymentIntervalTypeDesc ,  InterestDeductionTypeDesc, StatusTypeDesc, SubsDate
    FROM    @TableMaster
    --WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum
    
    SELECT @TotalRowsCount = COUNT(*)
    FROM   LoanSubscription  lns(nolock)
	join LoanRequest lr(nolock) on lns.LoanRequestID = lr.LoanRequestID
	WHERE  (@CUSRID ='' or AccountEmployeeID = @empid)   and 
   lns.DisbursementDate between @FrDt and @ToDt 
	--WHERE  (CreatedBy = @CUSRID or @CUSRID ='')
	       
   /* CHANGE  TABLE NAME */
    
    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster

END