USE [MCT_ERP]
GO
/****** Object:  StoredProcedure [dbo].[pr_Search_Masters_Vessel]    Script Date: 18/02/2022 15:12:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Yamuna J>
-- Create date: <18-02-2022>
-- Description:	<Vessel Master Details>
-- =============================================
/*
declare @tcnt int, @fcnt int
exec [pr_Search_Masters_Vessel] null,1,'asc',1,10, @tcnt  output, @fcnt output
*/
CREATE PROCEDURE [dbo].[pr_Search_Masters_Vessel]  /* CHANGE */
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
	    VSLCODE VARCHAR(100)
      , VSLDESC VARCHAR(100)
	  , DISPSTATUS smallint
	  , VSLID int 
      , RowNum INT
    )

    INSERT INTO @TableMaster(VSLCODE,
VSLDESC,DISPSTATUS,VSLID, RowNum)
    SELECT  VSLCODE
            , VSLDESC,DISPSTATUS
            , VSLID
            , Row_Number() OVER (
            ORDER BY
            
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
                  WHEN 0 THEN VSLCODE
                  WHEN 1 THEN VSLDESC
	 
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN VSLCODE
                  WHEN 1 THEN VSLDESC				
               
                END
            END DESC 
            
            /*DATETIME ORDER BY*/
             

            
            ) AS RowNum
    FROM    dbo.VESSELMASTER  /* CHANGE  TABLE NAME */
    WHERE   @FilterTerm IS NULL 
              OR VSLCODE LIKE @FilterTerm
              OR VSLDESC LIKE @FilterTerm
            

    SELECT VSLCODE
            , VSLDESC
			,
			  CASE DISPSTATUS WHEN 1 THEN 'Disabled' WHEN 0 THEN 'Enabled' END  AS DISPSTATUS
			 ,VSLID
    FROM    @TableMaster
    WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum
    
    SELECT @TotalRowsCount = COUNT(*)
    FROM   dbo.VESSELMASTER   /* CHANGE  TABLE NAME */
    
    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster
        
END
RETURN 0
