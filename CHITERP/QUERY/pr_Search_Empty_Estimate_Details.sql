USE [MCT_ERP]
GO
/****** Object:  StoredProcedure [dbo].[pr_Search_Empty_Estimate_Details]    Script Date: 06/05/2022 4:12:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Yamuna J>
-- Create date: <26-04-2022>
-- Description:	<Empty Estimate Details>
-- =============================================
/*
DECLARE @total INT,
        @filtered INT
EXEC [dbo].[pr_Search_Empty_Estimate_Details] NULL, 1, 'ASC', 1, 10000,  @total OUTPUT, @filtered OUTPUT, '2022-04-23','2022-04-26',3,4

SELECT @total, @filtered
*/
ALTER PROCEDURE [dbo].[pr_Search_Empty_Estimate_Details]
    @FilterTerm nvarchar(250) = NULL --parameter to search all columns by
  , @SortIndex INT = 1 -- a one based index to indicate which column to order by
  , @SortDirection CHAR(4) = 'ASC' --the direction to sort in, either ASC or DESC
  , @StartRowNum INT = 1 --the first row to return
  , @EndRowNum INT = 10 --the last row to return
  , @TotalRowsCount INT OUTPUT
  , @FilteredRowsCount INT OUTPUT  
  , @PSDate Smalldatetime
  , @PEDate Smalldatetime
  ,	@PSTMRID INT		
  ,	@PESTTID INT		
  
AS
BEGIN
	 SET @FilterTerm = '%' + @FilterTerm + '%' 
	 set nocount on
	declare @LESTTID INT	,@LSTMRID INT

		set @LESTTID=@PESTTID		
		set @LSTMRID=@PSTMRID

	 Declare @TableMaster Table 
	 (
	    TRANDATE smalldatetime   
	  , TRANNO int
      , TRANDNO Varchar(50)
      , CONTNRNO varchar(50)	
	  , CATENAME Varchar(250)	  
	  , CONTNRSDESC varchar(250)		 
	  , TRANNAMT numeric(18,2)	 
	  , TRANMID int 
      , RowNum INT
	 )

	  INSERT INTO @TableMaster(TRANDATE,TRANNO,TRANDNO,CONTNRNO,CATENAME,CONTNRSDESC,TRANNAMT,TRANMID, RowNum)
                 SELECT        TRANDATE,TRANNO,TRANDNO,CONTNRNO,CATENAME,CONTNRSDESC,TRANNAMT,TRANMID, Row_Number() OVER (
            ORDER BY
            
            /*DATETIME ORDER BY*/
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
                  WHEN 0 THEN TRANDNO
                 
	 
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN TRANDNO
                 
               
                END
            END DESC, 
            
            /*VARCHAR ORDER BY*/
              CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex                
                  
				  WHEN 1 THEN CATENAME	              
				  WHEN 2 THEN CONTNRSDESC 
				  WHEN 3 THEN CONTNRNO  
				  
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex                 
                  WHEN 1 THEN CATENAME	              
				  WHEN 2 THEN CONTNRSDESC 
				  WHEN 3 THEN CONTNRNO  
                END
            END DESC,

			    CASE @SortDirection
              WHEN 'ASC'  THEN
            CASE @SortIndex
                          
	              WHEN 4 THEN TRANNAMT
				 
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                                   
	              WHEN 4 THEN TRANNAMT
				               
                END
            END DESC
            
            ) AS RowNum
    FROM   REPAIRTRANSACTIONMASTER Inner Join 
	       EMPTY_GATEINDETAIL (NOLOCK) ON REPAIRTRANSACTIONMASTER.GIDID = EMPTY_GATEINDETAIL.GIDID Inner Join 
		   CONTAINERSIZEMASTER (NOLOCK) ON CONTAINERSIZEMASTER.CONTNRSID = EMPTY_GATEINDETAIL.CONTNRSID Inner Join 
		   GRADEMASTER  (NOLOCK) ON GRADEMASTER.GRADEID = EMPTY_GATEINDETAIL.GRADEID Inner Join 
		   CATEGORYMASTER (NOLOCK) ON CATEGORYMASTER.CATEID = REPAIRTRANSACTIONMASTER.TRANREFID 

 /* CHANGE  TABLE NAME */
     WHERE  (REPAIRTRANSACTIONMASTER.TRANREFID=@LSTMRID) and  (REPAIRTRANSACTIONMASTER.ESTTID = @LESTTID) 
	        and (REPAIRTRANSACTIONMASTER.TRANDATE BETWEEN @PSDate AND @PEDate)  
	         and (@FilterTerm IS NULL 
              OR CONTAINERSIZEMASTER.CONTNRSDESC LIKE @FilterTerm              
			  OR GRADEMASTER.GRADEDESC LIKE @FilterTerm  			  
              OR CATEGORYMASTER.CATENAME LIKE @FilterTerm         
			  OR TRANNAMT LIKE @FilterTerm  			  
              OR TRANDNO LIKE @FilterTerm                			  
			  )
                      				   
                      
       SELECT Convert(varchar(10),  TRANDATE,103 ) as TRANDATE,TRANNO,TRANDNO,CONTNRNO,CATENAME,CONTNRSDESC,TRANNAMT,TRANMID  
	          	          
	      FROM    @TableMaster
	   
   SELECT @TotalRowsCount = COUNT(*)
   FROM  REPAIRTRANSACTIONMASTER Inner Join 
	       EMPTY_GATEINDETAIL (NOLOCK) ON REPAIRTRANSACTIONMASTER.GIDID = EMPTY_GATEINDETAIL.GIDID Inner Join 
		   CONTAINERSIZEMASTER (NOLOCK) ON CONTAINERSIZEMASTER.CONTNRSID = EMPTY_GATEINDETAIL.CONTNRSID Inner Join 
		   GRADEMASTER  (NOLOCK) ON GRADEMASTER.GRADEID = EMPTY_GATEINDETAIL.GRADEID Inner Join 
		   CATEGORYMASTER (NOLOCK) ON CATEGORYMASTER.CATEID = REPAIRTRANSACTIONMASTER.TRANREFID 
   WHERE (REPAIRTRANSACTIONMASTER.TRANREFID=@LSTMRID) and  (REPAIRTRANSACTIONMASTER.ESTTID = @LESTTID) 
	        and (REPAIRTRANSACTIONMASTER.TRANDATE BETWEEN @PSDate AND @PEDate)  

   SELECT @FilteredRowsCount = COUNT(*)
   FROM   @TableMaster
        


END

