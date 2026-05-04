USE [MCT_ERP]
GO
/****** Object:  StoredProcedure [dbo].[pr_Search_Masters_Place]    Script Date: 18/02/2022 14:51:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Yamuna J>
-- Create date: <18-02-2022>
-- Description:	<Place Master Details>
-- =============================================
/*
declare @tcnt int, @fcnt int
exec [pr_Search_Masters_Place] null,1,'asc',1,10, @tcnt  output, @fcnt output
*/
CREATE PROCEDURE [dbo].[pr_Search_Masters_Place]  /* CHANGE */
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
	    PLCCODE VARCHAR(100)
      , PLCDESC VARCHAR(100)
	  , DISPSTATUS SMALLINT
	  , PLCID int 
      , RowNum INT
    )

    INSERT INTO @TableMaster(PLCCODE,PLCDESC,DISPSTATUS, PLCID , RowNum)
    SELECT   PLCCODE
            , PLCDESC,DISPSTATUS
            , PLCID
            , Row_Number() OVER (
            ORDER BY
            
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
                    WHEN 0 THEN PLCCODE
                  WHEN 1 THEN PLCDESC
	 
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN PLCCODE
                  WHEN 1 THEN PLCDESC				
               
                END
            END DESC 
            
            /*DATETIME ORDER BY*/
             

            
            ) AS RowNum
    FROM    dbo.PLACEMASTER  /* CHANGE  TABLE NAME */
    WHERE   @FilterTerm IS NULL 
              OR PLCCODE LIKE @FilterTerm
              OR PLCDESC LIKE @FilterTerm
            

    SELECT PLCCODE
            , PLCDESC
			, CASE DISPSTATUS WHEN 1 THEN 'Disabled' WHEN 0 THEN 'Enabled' END  AS DISPSTATUS
			,PLCID
    FROM    @TableMaster
    
    
    SELECT @TotalRowsCount = COUNT(*)
    FROM   dbo.PLACEMASTER   /* CHANGE  TABLE NAME */
    
    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster
        
END
RETURN 0
