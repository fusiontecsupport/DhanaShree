USE [MCT_ERP]
GO
/****** Object:  StoredProcedure [dbo].[pr_Search_Masters_LeasingContainerStatus]    Script Date: 18/02/2022 13:09:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Yamuna J>
-- Create date: <18-02-2022>
-- Description:	<Leasing Container Status Master Details>
-- =============================================
/*
declare @tcnt int, @fcnt int
exec [pr_Search_Masters_LeasingContainerStatus] null,1,'asc',1,10, @tcnt  output, @fcnt output
*/
CREATE PROCEDURE [dbo].[pr_Search_Masters_LeasingContainerStatus]  /* CHANGE */
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
	    LCNTNRSCODE VARCHAR(100)
      , LCNTNRSDESC VARCHAR(100)
	  , DISPSTATUS SMALLINT
	  , LCNTNRSID int 
      , RowNum INT
    )

    INSERT INTO @TableMaster(LCNTNRSCODE,LCNTNRSDESC,DISPSTATUS, LCNTNRSID , RowNum)
    SELECT   LCNTNRSCODE
            , LCNTNRSDESC,DISPSTATUS
            , LCNTNRSID
            , Row_Number() OVER (
            ORDER BY
            
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
                    WHEN 0 THEN LCNTNRSCODE
                  WHEN 1 THEN LCNTNRSDESC
	 
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN LCNTNRSCODE
                  WHEN 1 THEN LCNTNRSDESC				
               
                END
            END DESC 
            
            /*DATETIME ORDER BY*/
             

            
            ) AS RowNum
    FROM    dbo.LEASING_CONTAINERSTATUSMASTER  /* CHANGE  TABLE NAME */
    WHERE   @FilterTerm IS NULL 
              OR LCNTNRSCODE LIKE @FilterTerm
              OR LCNTNRSDESC LIKE @FilterTerm
            

    SELECT LCNTNRSCODE
            , LCNTNRSDESC
			, CASE DISPSTATUS WHEN 1 THEN 'Disabled' WHEN 0 THEN 'Enabled' END  AS DISPSTATUS
			,LCNTNRSID
    FROM    @TableMaster
    
    
    SELECT @TotalRowsCount = COUNT(*)
    FROM   dbo.LEASING_CONTAINERSTATUSMASTER   /* CHANGE  TABLE NAME */
    
    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster
        
END
RETURN 0
