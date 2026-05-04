-- =============================================
-- Author:		<Rajesh S>
-- Create date: <16/06/2022 04:30>
-- Description:	<Chit Scheme master details>
-- Exec [dbo].[pr_Search_Master_ChitGroup] @FilterTerm='',@SortIndex=1,@SortDirection='desc',@StartRowNum=1,@EndRowNum=10,@TotalRowsCount=100,@FilteredRowsCount=100,@CUSRID='admin'
-- =============================================
create PROCEDURE [dbo].[pr_Search_Master_ChitGroup]
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
	ChitGroupID int,
	ChitGroupCode varchar(100),
	ChitGroupName varchar(100),
	ChitSchemeID int,
	ChitSchemeCode varchar(100),
	ChitSchemeName varchar(100),
	ChitValue float,
	ChitDuration int,
	DurationTypeId int,	
	CommencementDate varchar(10),
	OfficeID int,
	OfficeDesc varchar(100),
	FirstInstallmentDate varchar(10),
	TerminationDate varchar(10),
	MaxGrpMembers int,
	StatusTypeID int,
	DurationTypeDesc varchar(100),
	StatusTypeDesc varchar(100),
	ChitSchemeStatusTypeDesc varchar(100),
	  --, DISPSTATUS smallint
	  --, CreatedBy varchar(50)
	  --, CreatedDt datetime
	  --, UpdatedBy varchar(50)
	  --, UpdatedDt DateTime
	  
      RowNum INT
    )

	INSERT INTO @TableMaster( ChitGroupID, ChitGroupCode, ChitGroupName, ChitSchemeID, ChitSchemeCode, ChitSchemeName, ChitValue, ChitDuration,
	DurationTypeId, CommencementDate, MaxGrpMembers, FirstInstallmentDate, TerminationDate, StatusTypeDesc,	DurationTypeDesc,ChitSchemeStatusTypeDesc,
	--DISPSTATUS,CreatedBy,CreatedDt,UpdatedBy,UpdatedDt, CONTID, 
	RowNum)   

	SELECT  cg.ChitGroupID, cg.ChitGroupCode, cg.ChitGroupName, cg.ChitSchemeID, ChitSchemeCode, ChitSchemeName, ChitValue, ChitDuration, 
	DurationTypeId, case when CommencementDate is null then '' else convert(varchar(10),CommencementDate,120) end , isnull(MaxGrpMembers,0), 
	case when FirstInstallmentDate is null then '' else convert(varchar(10),FirstInstallmentDate,120) end,
	case when TerminationDate is null then '' else convert(varchar(10),TerminationDate,120) end, a.Type , b.type  DurationTypeDesc ,	
	f.type StatusTypeDesc,
	--, DISPSTATUS,	ChitGroup.CreatedBy,ChitGroup.CreatedDt,ChitGroup.UpdatedBy,ChitGroup.UpdatedDt,
		   Row_Number() OVER (
            ORDER BY
            
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
                  WHEN 0 THEN ChitSchemeCode
				  WHEN 1 THEN ChitSchemeName  
				  WHEN 2 THEN ChitGroupCode   
				  WHEN 3 THEN ChitGroupName 
				  WHEN 4 THEN ChitValue
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN ChitSchemeCode
				  WHEN 1 THEN ChitSchemeName  
				  WHEN 2 THEN ChitGroupCode   
				  WHEN 3 THEN ChitGroupName 
				  WHEN 4 THEN ChitValue
                END
            END DESC 
            
            /*DATETIME ORDER BY*/       
            ) AS RowNum
    FROM  ChitGroup cg join ChitScheme  chts(nolock) on cg.ChitSchemeID = chts.ChitSchemeID
			left join MasterType a(nolock) on cg.StatusTypeID= a.TypeID
			left join MasterType b(nolock) on chts.DurationTypeId = b.TypeID
			left join MasterType f(nolock) on chts.StatusTypeID = f.TypeID

   WHERE -- (CreatedBy = @CUSRID or @CUSRID ='')   and 
   (@FilterTerm IS NULL   
              OR ChitSchemeCode LIKE @FilterTerm  
              OR ChitSchemeName LIKE @FilterTerm  
			  OR ChitGroupCode LIKE @FilterTerm  
              OR ChitGroupName LIKE @FilterTerm  
			  OR a.type LIKE @FilterTerm  
			  OR b.type LIKE @FilterTerm  
			  OR f.type LIKE @FilterTerm  )
			  			  
	SELECT ChitGroupID, ChitGroupCode, ChitGroupName, ChitSchemeID, ChitSchemeCode, ChitSchemeName, ChitValue, ChitDuration, 
	DurationTypeId, CommencementDate, MaxGrpMembers, FirstInstallmentDate, TerminationDate, StatusTypeDesc,	DurationTypeDesc,ChitSchemeStatusTypeDesc
    FROM    @TableMaster
    --WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum
    
    SELECT @TotalRowsCount = COUNT(*)
    FROM   ChitGroup  (nolock)
	--WHERE  (CreatedBy = @CUSRID or @CUSRID ='')
	       
   /* CHANGE  TABLE NAME */
    
    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster

END


