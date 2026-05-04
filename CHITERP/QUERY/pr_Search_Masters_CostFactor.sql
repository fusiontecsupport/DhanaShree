USE [MCT_ERP]
GO
/****** Object:  StoredProcedure [dbo].[pr_Search_Masters_CostFactor]    Script Date: 16/02/2022 16:48:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Yamuna J>
-- Create date: <16-02-2022>
-- Description:	<CostFactor Master Details>
-- =============================================
/*
declare @tcnt int, @fcnt int
exec [pr_Search_Masters_CostFactor] null,1,'asc',1,10, @tcnt  output, @fcnt output
*/

CREATE PROCEDURE [dbo].[pr_Search_Masters_CostFactor]  /* CHANGE */
 @FilterTerm nvarchar(250) = NULL --parameter to search all columns by
        , @SortIndex INT = 1 -- a one based index to indicate which column to order by
        , @SortDirection CHAR(4) = 'ASC' --the direction to sort in, either ASC or DESC
        , @StartRowNum INT = 1 --the first row to return
        , @EndRowNum INT = 10 --the last row to return
        , @TotalRowsCount INT OUTPUT
        , @FilteredRowsCount INT OUTPUT
 
AS BEGIN
    --Wrap filter term with % to search for values that contain @FilterTerm
    SET @FilterTerm = '%' + @FilterTerm + '%'

    DECLARE @TableMaster TABLE
    ( 
      CFDESC VARCHAR(100)
	  ,CFEXPR NUMERIC(18,3)
	  ,DISPSTATUS SMALLINT
	  , CFID int 
      , RowNum INT
    )

    INSERT INTO @TableMaster(
CFDESC,CFEXPR,DISPSTATUS,CFID , RowNum)
    SELECT  CFDESC
			,CFEXPR
			,DISPSTATUS
            , CFID
            , Row_Number() OVER (
            ORDER BY
            
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
                  WHEN 0 THEN CFDESC
				
	 
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN CFDESC
						
               
                END
            END DESC ,
            
            /*CFEXPR ORDER BY*/
                 CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
                  
				  WHEN 1 THEN CFEXPR
	 
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
             
				  WHEN 1 THEN CFEXPR				
               
                END
            END DESC

            
            ) AS RowNum
    FROM    dbo.COSTFACTORMASTER  /* CHANGE  TABLE NAME */
    WHERE   @FilterTerm IS NULL 
              OR CFDESC LIKE @FilterTerm
			  OR CFEXPR LIKE @FilterTerm
            

    SELECT CFDESC,CFEXPR,
			  CASE DISPSTATUS WHEN 1 THEN 'Disabled' WHEN 0 THEN 'Enabled' END  AS DISPSTATUS
			  , CFID
    FROM    @TableMaster
    WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum
    
    SELECT @TotalRowsCount = COUNT(*)
    FROM   dbo.COSTFACTORMASTER   /* CHANGE  TABLE NAME */
    
    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster
        
END