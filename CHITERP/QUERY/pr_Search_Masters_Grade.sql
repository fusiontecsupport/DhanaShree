USE [MCT_ERP]
GO
/****** Object:  StoredProcedure [dbo].[pr_Search_Masters_Grade]    Script Date: 18/02/2022 13:09:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Yamuna J>
-- Create date: <17-02-2022>
-- Description:	<Grade Master Details>
-- =============================================
/*
declare @tcnt int, @fcnt int
exec [pr_Search_Masters_Grade] null,1,'asc',1,10, @tcnt  output, @fcnt output
*/
CREATE PROCEDURE [dbo].[pr_Search_Masters_Grade]  /* CHANGE */
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
	    GRADECODE VARCHAR(100)
      , GRADEDESC VARCHAR(100)
	  , DISPSTATUS SMALLINT
	  , GRADEID int 
      , RowNum INT
    )

    INSERT INTO @TableMaster(GRADECODE,GRADEDESC,DISPSTATUS, GRADEID , RowNum)
    SELECT   GRADECODE
            , GRADEDESC,DISPSTATUS
            , GRADEID
            , Row_Number() OVER (
            ORDER BY
            
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
                   WHEN 0 THEN GRADECODE
                  WHEN 1 THEN GRADEDESC	
	 
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN GRADECODE
                  WHEN 1 THEN GRADEDESC				
               
                END
            END DESC 
            
            /*DATETIME ORDER BY*/
             

            
            ) AS RowNum
    FROM    dbo.GRADEMASTER  /* CHANGE  TABLE NAME */
    WHERE   @FilterTerm IS NULL 
              OR GRADECODE LIKE @FilterTerm
              OR GRADEDESC LIKE @FilterTerm
            

    SELECT GRADECODE
            , GRADEDESC
			, CASE DISPSTATUS WHEN 1 THEN 'Disabled' WHEN 0 THEN 'Enabled' END  AS DISPSTATUS
			,GRADEID
    FROM    @TableMaster
    
    
    SELECT @TotalRowsCount = COUNT(*)
    FROM   dbo.GRADEMASTER   /* CHANGE  TABLE NAME */
    
    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster
        
END
RETURN 0
