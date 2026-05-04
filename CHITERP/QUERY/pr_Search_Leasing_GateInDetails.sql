USE [MCT_ERP]
GO
/****** Object:  StoredProcedure [dbo].[pr_Search_Leasing_GateInDetails]    Script Date: 07/03/2022 16:06:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Yamuna J>
-- Create date: <04-03-2022>
-- Description:	<Empty GateIn details>
/*
DECLARE @total INT,
        @filtered INT
EXEC [dbo].[pr_Search_Leasing_GateInDetails] NULL, 1, 'ASC', 1, 10000,  @total OUTPUT, @filtered OUTPUT, '2022-03-05','2022-03-05',1

SELECT @total, @filtered
*/
-- =============================================
ALTER PROCEDURE [dbo].[pr_Search_Leasing_GateInDetails]  /* CHANGE */
 @FilterTerm nvarchar(250) = NULL --parameter to search all columns by
        , @SortIndex INT = 1 -- a one based index to indicate which column to order by
        , @SortDirection CHAR(4) = 'ASC' --the direction to sort in, either ASC or DESC
        , @StartRowNum INT = 1 --the first row to return
        , @EndRowNum INT = 10 --the last row to return
        , @TotalRowsCount INT OUTPUT
        , @FilteredRowsCount INT OUTPUT
        , @PSDate Smalldatetime
        , @PEDate Smalldatetime
		,@PCompyid int
AS
BEGIN
	 SET @FilterTerm = '%' + @FilterTerm + '%'

	declare @LCompyid int 
	set @LCompyid=@PCompyid
    DECLARE @TableMaster TABLE
    ( 
        GIDATE SMALLDATETIME
      , GINO INT
      , GIDNO VARCHAR(15)
      , CONTNRNO  VARCHAR(15)
      , CONTNRSDESC  varchar(100) 
	  , CONTNRTDESC varchar(100)
	  , VHLNO varchar(20)
	  , VSLDESC VARCHAR(100)	       
      , STMRNAME VARCHAR(150)	 
	  , GRADEDESC VARCHAR(150)
	  , DISPSTATUS int
	  , GIDID int 
      , RowNum INT
    )

	 INSERT INTO @TableMaster(GIDATE, GINO, GIDNO, CONTNRNO,CONTNRSDESC,CONTNRTDESC,
	                          VHLNO,VSLDESC ,STMRNAME,GRADEDESC,DISPSTATUS ,GIDID, RowNum)
     SELECT  GIDATE, GINO, GIDNO, CONTNRNO, CONTAINERSIZEMASTER.CONTNRSDESC,CONTNRTDESC,
	  VHLNO,VESSELMASTER.VSLDESC ,CATEGORYMASTER.CATENAME,GRADEMASTER.GRADEDESC, EMPTY_GATEINDETAIL.DISPSTATUS , GIDID , Row_Number() OVER (
            ORDER BY
            
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
				
           	       WHEN 2 then GIDNO
			  	   WHEN 3 then CONTNRNO			  	 
				   WHEN 5 then CONTAINERSIZEMASTER.CONTNRSDESC
			  	   WHEN 6 then CONTNRTDESC
			       WHEN 7 then VHLNO
				   WHEN 8 then VESSELMASTER.VSLDESC
				   WHEN 9 then CATEGORYMASTER.CATENAME
				   WHEN 10 then GRADEMASTER.GRADEDESC
			  	   
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex

           	       WHEN 2 then GIDNO
			  	   WHEN 3 then CONTNRNO			  	 
				   WHEN 5 then CONTAINERSIZEMASTER.CONTNRSDESC
			  	   WHEN 6 then CONTNRTDESC
			       WHEN 7 then VHLNO
				   WHEN 8 then VESSELMASTER.VSLDESC
				   WHEN 9 then CATEGORYMASTER.CATENAME
				   WHEN 10 then GRADEMASTER.GRADEDESC
                END
            END DESC,
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
                  WHEN 0 THEN GIDATE
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN GIDATE
                END
            END DESC,
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
				  WHEN 1 then GINO
                  
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
				  WHEN 1 then GINO
                  
                END
            END DESC                            

            ) AS RowNum
   FROM    EMPTY_GATEINDETAIL (nolock) 
           Left JOIN CONTAINERTYPEMASTER (NOLOCK) ON EMPTY_GATEINDETAIL.CONTNRTID  = CONTAINERTYPEMASTER.CONTNRTID 
		   Left JOIN CONTAINERSIZEMASTER  (NOLOCK) ON EMPTY_GATEINDETAIL.CONTNRSID = CONTAINERSIZEMASTER.CONTNRSID  
		   Left JOIN VESSELMASTER   (NOLOCK) ON EMPTY_GATEINDETAIL.VSLID = VESSELMASTER.VSLID  
		   Left JOIN CATEGORYMASTER   (NOLOCK) ON EMPTY_GATEINDETAIL.STMRID = CATEGORYMASTER.CATEID 
		   Left JOIN GRADEMASTER   (NOLOCK) ON EMPTY_GATEINDETAIL.GRADEID = GRADEMASTER.GRADEID 
    WHERE   (GIDATE BETWEEN @PSDate AND @PEDate) and (COMPYID=@LCompyid) AND SDPTID=8 AND GILTYPE=1 
	        AND EMPTY_GATEINDETAIL.CONTNRSID > 0 AND CONTNRID >= 0 AND (@FilterTerm IS NULL 
              OR  GIDATE LIKE @FilterTerm 
              OR  GINO LIKE @FilterTerm 
              OR  GIDNO  LIKE @FilterTerm 
              OR  CONTAINERSIZEMASTER.CONTNRSDESC  LIKE @FilterTerm 
              OR  CONTNRNO    LIKE @FilterTerm 
              OR  CONTNRTDESC    LIKE @FilterTerm 
			  OR  VHLNO LIKE @FilterTerm 
              OR  VESSELMASTER.VSLDESC  LIKE @FilterTerm 
              OR  CATEGORYMASTER.CATENAME    LIKE @FilterTerm   
			  OR  GRADEMASTER.GRADEDESC    LIKE @FilterTerm   
			 
		      OR  PRDTDESC    LIKE @FilterTerm )  

	  SELECT Convert(varchar(10), GIDATE,103 ) as GIDATE, GINO, GIDNO, CONTNRNO,CONTNRSDESC,CONTNRTDESC,
	         VHLNO,VSLDESC ,STMRNAME,GRADEDESC, CASE DISPSTATUS WHEN 0 THEN 'Enabled' when 1 then 'Disabled'  END AS DISPSTATUS, GIDID 
	  FROM    @TableMaster WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum
    
    SELECT @TotalRowsCount = COUNT(*)
    FROM         EMPTY_GATEINDETAIL (nolock) 
           Left JOIN CONTAINERTYPEMASTER (NOLOCK) ON EMPTY_GATEINDETAIL.CONTNRTID  = CONTAINERTYPEMASTER.CONTNRTID 
		   Left JOIN CONTAINERSIZEMASTER  (NOLOCK) ON EMPTY_GATEINDETAIL.CONTNRSID = CONTAINERSIZEMASTER.CONTNRSID  
		   Left JOIN VESSELMASTER   (NOLOCK) ON EMPTY_GATEINDETAIL.VSLID = VESSELMASTER.VSLID  
		   Left JOIN CATEGORYMASTER   (NOLOCK) ON EMPTY_GATEINDETAIL.STMRID = CATEGORYMASTER.CATEID 
		   Left JOIN GRADEMASTER   (NOLOCK) ON EMPTY_GATEINDETAIL.GRADEID = GRADEMASTER.GRADEID   /* CHANGE  TABLE NAME */
    WHERE  (GIDATE BETWEEN @PSDate AND @PEDate) and (COMPYID=@LCompyid) AND SDPTID=8 AND GILTYPE=1 
	      AND EMPTY_GATEINDETAIL.CONTNRSID > 0 AND CONTNRID >= 0
    
    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster

	 
END
