-- =============================================
-- Author:		<Yamuna J>
-- Create date: <30/05/2022>
-- Description:	<Call master details>
-- Exec [dbo].[pr_Search_Master_ContactMaster] @FilterTerm='',@SortIndex=1,@SortDirection='asc',@StartRowNum=1,@EndRowNum=10,@TotalRowsCount=100,@FilteredRowsCount=100,@CUSRID=''
-- =============================================
alter PROCEDURE [dbo].[pr_Search_Master_ContactMaster]
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
	    ClientName varchar(250)
	  , ContactType varchar(250)
	  ,  CONTACTPERSON VARCHAR(250)	      	  
      , CPMOBILENO1 VARCHAR(25) 
	  , CPEMAILID VARCHAR(150) 
	  , CPDESIGNATION VARCHAR(150) 
	  , CPORGANISATION VARCHAR(250) 
	  , DISPSTATUS smallint
	  , CreatedBy varchar(50)
	  , CreatedDt datetime
	  , UpdatedBy varchar(50)
	  , UpdatedDt DateTime
	  , CONTID int 
      , RowNum INT
    )

	INSERT INTO @TableMaster( ClientName, CONTACTPERSON, ContactType, CPMOBILENO1,CPEMAILID,CPDESIGNATION,CPORGANISATION, DISPSTATUS,CreatedBy,CreatedDt,UpdatedBy,UpdatedDt, CONTID, RowNum)   

	SELECT b.ClientName, CONTACTPERSON, c.CONTTYPEDESC, CPMOBILENO1, CPEMAILID, CPDESIGNATION,CPORGANISATION, a.DISPSTATUS,
	a.CreatedBy,a.CreatedDt,a.UpdatedBy,a.UpdatedDt, CONTID 	       
		   , Row_Number() OVER (
            ORDER BY
            
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
                  WHEN 0 THEN ClientName   
				  WHEN 1 THEN CONTACTPERSON  
				  WHEN 2 THEN c.CONTTYPEDESC  
				  WHEN 3 THEN CPDESIGNATION  
				  WHEN 4 THEN CPORGANISATION
                  				  	
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                   WHEN 0 THEN ClientName   
				  WHEN 1 THEN CONTACTPERSON  
				  WHEN 2 THEN c.CONTTYPEDESC  
				  WHEN 3 THEN CPDESIGNATION  
				  WHEN 4 THEN CPORGANISATION
                  				  
                END
            END DESC 
            
            /*DATETIME ORDER BY*/       
            ) AS RowNum
    FROM  CONTACTMASTER a(nolock) join clientmaster b(nolock) on a.clientid = b.clientid
		left join ContactTypeMaster c(nolock) on a.CONTTID= c.CONTTID 

   WHERE  (b.CreatedBy = @CUSRID or @CUSRID ='')
   and (@FilterTerm IS NULL   
              OR CONTACTPERSON LIKE @FilterTerm  
              OR CPMOBILENO1 LIKE @FilterTerm  
			  OR CPEMAILID LIKE @FilterTerm  
			  OR CPDESIGNATION LIKE @FilterTerm  
			  OR CPORGANISATION LIKE @FilterTerm  )
			  			  
	SELECT ClientName, CONTACTPERSON, ContactType, CPMOBILENO1,CPEMAILID,CPDESIGNATION,CPORGANISATION,
	       CASE DISPSTATUS WHEN 1 THEN 'Disabled' WHEN 0 THEN 'Enabled' END  AS DISPSTATUS ,isnull(CreatedBy,'') as CreatedBy ,
		   isnull(Convert(varchar(10),CreatedDt,103),'') as CreatedDt,isnull(UpdatedBy,'') as UpdatedBy, isnull(Convert(varchar(10),UpdatedDt,103),'') as UpdatedDt  , CONTID 
    FROM    @TableMaster
    --WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum
    
    SELECT @TotalRowsCount = COUNT(*)
    FROM   CONTACTMASTER  (nolock)
	WHERE  (CreatedBy = @CUSRID or @CUSRID ='')
	       
   /* CHANGE  TABLE NAME */
    
    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster

END

