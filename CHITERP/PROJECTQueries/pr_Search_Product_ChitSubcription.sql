-- =============================================
-- Author:		<Rajesh S>
-- Create date: <25/06/2022 13:30>
-- Description:	<Chit Scheme master details>
-- Exec [dbo].[pr_Search_Product_ChitSubcription] @FilterTerm='',@SortIndex=1,@SortDirection='desc',@StartRowNum=1,@EndRowNum=10,@TotalRowsCount=100,@FilteredRowsCount=100,@CUSRID='admin'
-- =============================================
alter PROCEDURE [dbo].[pr_Search_Product_ChitSubcription]
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
  , @ColAgnt int=0
  , @custtype int = 0
AS
BEGIN
	SET @FilterTerm = '%' + @FilterTerm + '%'
	declare @empid int
	select @empid = EmpId from AspNetUsers(nolock) where UserName = @CUSRID
	--if @EndRowNum=0
	--set @EndRowNum=10
	set nocount on
    DECLARE @TableMaster TABLE
    ( 
	SubscriptionID int,
	SubscriptionCode varchar(100),
	ClientId int,
	ClientType varchar(15),
	ClientName varchar(200),
	ClientContactNos varchar(100),
	ChitSchemeID int,
	ChitTypeID int,	
	ChitSchemeCode varchar(100),
	ChitSchemeName varchar(100),	
	ChitGroupCode varchar(100),
	ChitGroupName varchar(100),	
	ChitValue float,
	ChitDuration int,
	StatusTypeId int,
	DurationTypeId int,
	DurationTypeDesc varchar(100),
	IntervalFreqTypeDesc varchar(100),
	StatusTypeDesc varchar(100),
	SubsDate varchar(12),
	CollectionAgent varchar(250)
	  --, DISPSTATUS smallint
	  --, CreatedBy varchar(50)
	  --, CreatedDt datetime
	  --, UpdatedBy varchar(50)
	  --, UpdatedDt DateTime
	  
      , RowNum INT
    )
	declare @custsublist table
	(subscode varchar(150))

	insert into @custsublist
	select a.ChitSubscriptionCode
	from chitsubscription a (nolock) left join Tmp_Ledger_Balance_Detail l(nolock) on a.ChitSubscriptionCode= l.LCode
	--where ((EndDate < getdate() and BalAmt > 0 and @custtype = 1)
	--or ((@custtype = 0) and (EndDate < getdate() or isnull(EndDate,'') ='' )))

	INSERT INTO @TableMaster(SubscriptionCode, ClientId, ClientType, ClientName, ClientContactNos,SubscriptionID, ChitSchemeID, ChitTypeID, ChitSchemeCode, ChitSchemeName, 
	ChitGroupCode, ChitGroupName, ChitValue, ChitDuration, DurationTypeId,  StatusTypeID , DurationTypeDesc ,	IntervalFreqTypeDesc, 
	StatusTypeDesc,SubsDate, CollectionAgent,
	--DISPSTATUS,CreatedBy,CreatedDt,UpdatedBy,UpdatedDt, CONTID, 
	RowNum)   

	SELECT chitsubscriptioncode,  chts.PersonID, case  when chts.SubscriberIsPerson = 1 then 'Individual' else 'Organization' end as ClientType, cm.CLIENTNAME, 
	isnull(cm.CLIENTMOBILENO1,'') + ','+ isnull(cm.CLIENTLANDLINENO1,''),
	chts.SubscriptionID, chts.ChitSchemeID, ChitTypeID, ChitSchemeCode, ChitSchemeName, chtg.ChitGroupCode, chtg.ChitGroupName, cht.ChitValue, 
	ChitDuration, DurationTypeId, chts.StatusTypeID , b.type as  DurationTypeDesc ,	d.type  IntervalFreqTypeDesc , f.type StatusTypeDesc, chts.startdate,
	isnull(e1.catename ,'') +',' + isnull(e2.catename,''),
	--, DISPSTATUS,	ChitScheme.CreatedBy,ChitScheme.CreatedDt,ChitScheme.UpdatedBy,ChitScheme.UpdatedDt, CONTID 	       
		   Row_Number() OVER (
            ORDER BY
            
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
                 -- WHEN 0 THEN startdate
				  WHEN 1 THEN chitsubscriptioncode 
				  WHEN 2 THEN CLIENTNAME  
				  WHEN 3 THEN case  when chts.SubscriberIsPerson = 1 then 'Individual' else 'Organization' end
				  --WHEN 3 THEN ChitSchemeCode 
				  --WHEN 4 THEN ChitSchemeName
				  --WHEN 5 THEN chtg.ChitGroupCode
				  --WHEN 6 THEN chtg.ChitGroupName
				  WHEN 4 THEN Chitvalue
				  WHEN 5 THEN isnull(cm.CLIENTMOBILENO1,'') + ','+ isnull(cm.CLIENTLANDLINENO1,'')
				  when 6 then f.type                  				  	
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
      --      WHEN 0 THEN f.type
				  --WHEN 1 THEN case  when chts.SubscriberIsPerson = 1 then 'Individual' else 'Organization' end   
				  --WHEN 2 THEN CLIENTNAME  
				  --WHEN 3 THEN ChitSchemeCode 
				  --WHEN 4 THEN ChitSchemeName
				  --WHEN 5 THEN chtg.ChitGroupCode
				  --WHEN 6 THEN chtg.ChitGroupName
				  --WHEN 7 THEN isnull(cm.CLIENTMOBILENO1,'') + ','+ isnull(cm.CLIENTLANDLINENO1,'')
				  --WHEN 0 THEN startdate
				  WHEN 1 THEN chitsubscriptioncode 
				  WHEN 2 THEN CLIENTNAME  
				  WHEN 3 THEN case  when chts.SubscriberIsPerson = 1 then 'Individual' else 'Organization' end
				  --WHEN 3 THEN ChitSchemeCode 
				  --WHEN 4 THEN ChitSchemeName
				  --WHEN 5 THEN chtg.ChitGroupCode
				  --WHEN 6 THEN chtg.ChitGroupName
				  WHEN 4 THEN Chitvalue
				  WHEN 5 THEN isnull(cm.CLIENTMOBILENO1,'') + ','+ isnull(cm.CLIENTLANDLINENO1,'')
				  when 6 then f.type                  				  	
                  				  
                END
            END DESC 
            
         /*DATETIME ORDER BY*/       
            ) AS RowNum
    FROM  ChitSubscription chts
			join @custsublist csl on chts.chitsubscriptioncode = csl.subscode
			join ChitScheme  cht(nolock) on chts.ChitSchemeID = cht.ChitSchemeID
			left join ChitGroup chtg(nolock) on chts.ChitGroupID= chtg.ChitGroupID
			join ClientMaster cm(nolock) on chts.PersonID= cm.CLIENTID			
			left join MasterType b(nolock) on cht.DurationTypeId = b.TypeID			
			left join MasterType d(nolock) on chts.CollectionIntervalTypeID = d.TypeID			
			left join MasterType f(nolock) on chts.StatusTypeID = f.TypeID
			left join EMPLOYEEMASTER e1(nolock) on chts.CollectionAgentPersonID1 = e1.CATEID
			left join EMPLOYEEMASTER e2(nolock) on chts.CollectionAgentPersonID2 = e2.CATEID
    
    WHERE  (@CUSRID ='' or @empid = CollectionAgentPersonID1 or @empid = CollectionAgentPersonID2)   and 
   (chts.startdate is null or chts.StartDate between @FrDt and @ToDt )and
   (chts.CollectionAgentPersonID1 = @ColAgnt or chts.CollectionAgentPersonID2 = @ColAgnt or @ColAgnt = 0) and
   (@FilterTerm IS NULL   
              OR ChitSchemeCode LIKE @FilterTerm  
              OR ChitSchemeName LIKE @FilterTerm  
			  OR b.type LIKE @FilterTerm  
			  OR d.type LIKE @FilterTerm  
			  OR f.type LIKE @FilterTerm 
			  or isnull(cm.CLIENTMOBILENO1,'') + ','+ isnull(cm.CLIENTLANDLINENO1,'' ) LIKE @FilterTerm 
			  or isnull(e1.catename ,'') +',' + isnull(e2.catename,'')  LIKE @FilterTerm 
			  )
			  			  
	SELECT ClientId, ClientType, ClientName, SubscriptionID, ChitSchemeID, ChitTypeID, ChitSchemeCode, ChitSchemeName, ChitValue, ChitDuration, DurationTypeId,  
	StatusTypeID , DurationTypeDesc ,	IntervalFreqTypeDesc, StatusTypeDesc, ChitGroupCode ,	ChitGroupName, 
	ClientContactNos, SubsDate, SubscriptionCode, CollectionAgent
    FROM    @TableMaster
    --WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum
    
    SELECT @TotalRowsCount = COUNT(*)
    FROM   ChitSubscription chts
			join @custsublist csl on chts.chitsubscriptioncode = csl.subscode
			join ChitScheme  cht(nolock) on chts.ChitSchemeID = cht.ChitSchemeID
			left join ChitGroup chtg(nolock) on chts.ChitGroupID= chtg.ChitGroupID
	WHERE  (@CUSRID ='' or @empid = CollectionAgentPersonID1 or @empid = CollectionAgentPersonID2)   and 
   (chts.startdate is null or chts.StartDate between @FrDt and @ToDt ) and
   (chts.CollectionAgentPersonID1 = @ColAgnt or chts.CollectionAgentPersonID2 = @ColAgnt or @ColAgnt = 0) 
	--WHERE  (CreatedBy = @CUSRID or @CUSRID ='')
	       
   /* CHANGE  TABLE NAME */
    
    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster

	select @TotalRowsCount = isnull(@TotalRowsCount,0), @FilteredRowsCount = isnull(@FilteredRowsCount,0)

END

