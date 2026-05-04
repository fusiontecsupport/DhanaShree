-- =============================================
-- Author:		<Yamuna J>
-- Create date: <18-01-2022>
-- Description:	<My Sales Taks Details>
/*
declare @tcnt int, @fcnt int
exec [pr_Search_Sales_MyTaskDetails] null,1,'asc',1,10, 1000, 1000,'2022-03-14','2022-03-25',2,'',1228
*/
-- =============================================-- =============================================
-- Author:		<Yamuna J>
-- Create date: <18-01-2022>
-- Description:	<My Sales Taks Details>
/*
declare @tcnt int, @fcnt int
exec [pr_Search_Sales_MyTaskDetails] @FilterTerm='',@SortIndex=0,@SortDirection='',@StartRowNum=0,@EndRowNum=0,@TotalRowsCount=0,@FilteredRowsCount=0,@PSDate='2021-12-01',@PEDate='2022-11-30',@PTaskStatusId=2,@usrid='vytheeswaran',@empid=86
exec [pr_Search_Sales_MyTaskDetails] null,1,'asc',1,10, 1000, 1000,'2022-03-14','2022-03-25',2,'',1228
exec [pr_Search_Sales_MyTaskDetails] @FilterTerm=NULL,@SortIndex=1,@SortDirection='ASC',@StartRowNum=1,@EndRowNum=9999999,@TotalRowsCount=9999999,@FilteredRowsCount=999999,@PSDate='2022-12-01',@PEDate='2023-11-30',@PTaskStatusId=0,@usrid='',@empid=1228
*/
-- =============================================
ALTER  PROCEDURE [dbo].[pr_Search_Sales_MyTaskDetails]
    @FilterTerm nvarchar(250) = NULL --parameter to search all columns by
  , @SortIndex INT = 1 -- a one based index to indicate which column to order by
  , @SortDirection CHAR(4) = 'ASC' --the direction to sort in, either ASC or DESC
  , @StartRowNum INT = 1 --the first row to return
  , @EndRowNum INT = 10 --the last row to return
  , @TotalRowsCount INT OUTPUT
  , @FilteredRowsCount INT OUTPUT
  , @PSDate Smalldatetime
  , @PEDate Smalldatetime
  , @PTaskStatusId int 
  , @usrid varchar(50)
  , @empid int 
  , @optteam int=1
AS
BEGIN
	 SET @FilterTerm = '%' + @FilterTerm + '%' 
	  
	 Declare @LSDate Smalldatetime,@LEDate Smalldatetime,@LTaskStatusId int  

	 Set @LSDate = @PSDate  Set @LEDate = @PEDate  Set @LTaskStatusId = @PTaskStatusId  

	 Declare @TableMaster Table 
	 (
	   LEADMID Int,
	   LEADNAME Varchar(250),	  
	   TASKSTATUS Int,
	   LEADSTATUS Int,
	   TASKMNO Int,
	   TASKMDNO VarChar(50),	 
	   TASKTYPE Int,
	   TASKDESCRIPTION Varchar(250),
	   ASSIGNEDDATETM DateTime,
	   PLANNEDDATETM DateTime,
	   ACTUALDATETM DateTime,
	   RESCHEDULEDDATETM DateTime,
	   TASKDID Int,
	   TASKMID Int ,
	   AssignedToEmpid int,
	   AssignedToEmp varchar(250),
	   RowNum Int	 
	 )

	 DECLARE @EMPTBL TABLE
	 (SUPEMPID INT,
	 USERNAME VARCHAR(100),
	  EMPID INT
	  )

	  INSERT INTO @EMPTBL
	  SELECT DISTINCT EL.CATEID, ASSIGNEDTOUSR, ASSIGNEDTOEMPID
	  FROM dbo.CRM_SALES_TASK_MASTER TM(NOLOCK) JOIN dbo.CRM_SALES_TASK_DETAIL  TD(NOLOCK) ON  TM.TASKMID = TD.TASKMID Inner Join  
			 dbo.LEADMASTER (NOLOCK) ON  TM.LEADMID = LEADMASTER.LEADMID  Left Join 
			EMPLOYEELINKMASTER EL (NOLOCK)  ON EL.CATEID = TM.ASSIGNEDTOEmpID OR EL.LCATEID = TM.ASSIGNEDTOEmpID
		WHERE (EL.CATEID = @empid OR TM.ASSIGNEDTOEmpID = @empid or @usrid = '') 
		--and ASSIGNEDDATETM is not null
		and 	td.TASKDSTATUS = @PTaskStatusId and 
			  (@FilterTerm IS NULL  
				  OR LEADNAME LIKE @FilterTerm
				   OR TD.TASKDESCRIPTION LIKE @FilterTerm
					OR TASKMDNO LIKE @FilterTerm
					 --OR ASSIGNEDDATETM LIKE @FilterTerm
					  )
                     

		INSERT INTO @EMPTBL
	  SELECT DISTINCT EL2.CATEID, TM.CREATEDBY, EL2.LCATEID
	  FROM dbo.CRM_SALES_TASK_MASTER TM(NOLOCK) JOIN dbo.CRM_SALES_TASK_DETAIL  TD(NOLOCK) ON  TM.TASKMID = TD.TASKMID Inner Join  
			 dbo.LEADMASTER (NOLOCK) ON  TM.LEADMID = LEADMASTER.LEADMID  Left Join 
			ASPNETUSERS U(NOLOCK) ON TM.CREATEDBY = U.USERNAME LEFT JOIN
			EMPLOYEELINKMASTER EL2 (NOLOCK)  ON EL2.LCATEID = U.BrnchId 	 
	 WHERE EL2.LCATEID NOT IN (SELECT ASSIGNEDTOEMPID FROM @EMPTBL)
	 AND (EL2.CATEID = @empid OR TM.CREATEDBY = U.UserName OR TM.CREATEDBY = @usrid ) 
	--and ASSIGNEDDATETM is not null
	and 	td.TASKDSTATUS = @PTaskStatusId and 
	      (@FilterTerm IS NULL  
              OR LEADNAME LIKE @FilterTerm
			   OR TD.TASKDESCRIPTION LIKE @FilterTerm
			    OR TASKMDNO LIKE @FilterTerm
				 --OR ASSIGNEDDATETM LIKE @FilterTerm
				  )
                     

	  INSERT INTO @TableMaster(LEADMID ,LEADNAME, TASKSTATUS, LEADSTATUS, TASKMNO, TASKMDNO,TASKTYPE,
	                                     TASKDESCRIPTION,ASSIGNEDDATETM,PLANNEDDATETM,ACTUALDATETM,RESCHEDULEDDATETM, TASKDID,TASKMID, AssignedToEmpid,AssignedToEmp, RowNum)
                 SELECT          distinct TM.LEADMID ,LEADMASTER.LEADNAME,isnull(TD.TASKDSTATUS,0) as TASKDSTATUS,TM.LEADSTATUS, 
	                             TM.TASKMNO,TM.TASKMDNO,
								 isnull(TD.TASKTYPE,0) as TASKTYPE,isnull(TD.TASKDESCRIPTION,'') as TASKDESCRIPTION,
								 TM.ASSIGNEDDATETM as ASSIGNEDDATETM, TD.PLANNEDDATETM as PLANNEDDATETM,
								 TD.ACTUALDATETM as ACTUALDATETM,TD.RESCHEDULEDDATETM as RESCHEDULEDDATETM,
								 isnull(TD.TASKDID,0) as TASKDID,
								 TM.TASKMID,isnull(tm.assignedtoempid,0), isnull(e.catename,''),
								 Row_Number() OVER (
	            ORDER BY          
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/           
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
                  WHEN 0 THEN TM.ASSIGNEDDATETM               	 
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN TM.ASSIGNEDDATETM                 		              
                END
            END DESC ,
			 CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
				  WHEN 1 THEN TASKMDNO
                  
				  WHEN 2 THEN (CASE TASKTYPE WHEN 1 THEN 'Call' when 2 then 'MOM' When 3 then 'Geo Location' When 4 then 'Machine Details' 
				  When 5 then 'Demo'  When 6 then 'Visit' When 7 then 'Quick Quotation' When 14 then 'Repeat Order' END)     
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                 WHEN 1 THEN TASKMDNO                  
				 WHEN 2 THEN (CASE TASKTYPE WHEN 1 THEN 'Call' when 2 then 'MOM' When 3 then 'Geo Location' When 4 then 'Machine Details'
				  When 5 then 'Demo'  When 6 then 'Visit' When 7  then 'Quick Quotation' When 14 then 'Repeat Order' END)     
                END
            END DESC ,
			 
			 CASE @SortDirection
      WHEN 'ASC'  THEN  
			    CASE @SortIndex
				   WHEN 3 THEN LEADNAME
				   WHEN 4 THEN TD.TASKDESCRIPTION  
                END       
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
				  WHEN 3 THEN LEADNAME
				  WHEN 4 THEN TD.TASKDESCRIPTION  
                END
            END DESC
            /*DATETIME ORDER BY*/                        
            ) AS RowNum
    FROM     
			dbo.CRM_SALES_TASK_MASTER TM(NOLOCK) JOIN dbo.CRM_SALES_TASK_DETAIL  TD(NOLOCK) ON  TM.TASKMID = TD.TASKMID Inner Join  
			 dbo.LEADMASTER (NOLOCK) ON  TM.LEADMID = LEADMASTER.LEADMID  Left Join 
			 employeemaster e(nolock) on tm.ASSIGNEDTOEmpID = e.CATEID left join
			--EMPLOYEELINKMASTER EL (NOLOCK)  ON  TM.ASSIGNEDTOEmpID = EL.CATEID = OR TM.ASSIGNEDTOEmpID = EL.LCATEID =  LEFT JOIN	
			@EMPTBL EL   ON TM.ASSIGNEDTOEmpID  = EL.SUPEMPID OR  TM.ASSIGNEDTOEmpID = EL.EMPID OR TM.CREATEDBY = EL.USERNAME	

 /* CHANGE  TABLE NAME */
    --WHERE (ASSIGNEDTOUsr = @usrid or @usrid = '') 
	--WHERE (EL.CATEID = @empid OR TM.ASSIGNEDTOEmpID = @empid or @usrid = '') 
	WHERE ((EL.SUPEMPID = @empid and @optteam=1) OR TM.ASSIGNEDTOEmpID = @empid OR TM.CREATEDBY = @usrid  or @usrid = '') 
	and ASSIGNEDDATETM is not null
	and 	td.TASKDSTATUS = @PTaskStatusId and 
	      (@FilterTerm IS NULL  
              OR LEADNAME LIKE @FilterTerm
			   OR TD.TASKDESCRIPTION LIKE @FilterTerm
			    OR TASKMDNO LIKE @FilterTerm
				 --OR ASSIGNEDDATETM LIKE @FilterTerm
				  )
                      				   
	group by TM.LEADMID ,LEADMASTER.LEADNAME,TD.TASKDSTATUS ,TM.LEADSTATUS, 
	                             TM.TASKMNO,TM.TASKMDNO,TD.TASKTYPE ,TD.TASKDESCRIPTION,TM.ASSIGNEDDATETM,TD.PLANNEDDATETM,
								 TD.ACTUALDATETM,TD.RESCHEDULEDDATETM,TD.TASKDID,TM.TASKMID, tm.ASSIGNEDTOEmpID, e.CATENAME
                      
       SELECT  distinct LEADMID ,LEADNAME, CASE TASKSTATUS WHEN 0 THEN 'Planned'  WHEN 1 THEN 'In Progress' WHEN 2 THEN 'Done' END  AS TASKSTATUS,
	           CASE LEADSTATUS WHEN 0 THEN 'Cold'  WHEN 1 THEN 'Warm' WHEN 2 THEN 'Hot' END  AS LEADSTATUS, TASKMNO, TASKMDNO,
			   CASE TASKTYPE WHEN 1 THEN 'Call'  WHEN 2 THEN 'MOM' WHEN 3 THEN 'Geo Location' WHEN 4 THEN 'Machine Details' 
			                 When 5 then 'Demo'  When 6 then 'Visit' When 7 then 'Quick Quotation'  When 14 then 'Repeat Order'  END  AS TASKTYPE,
			   TASKDESCRIPTION,
			   isnull(convert(varchar(11),ASSIGNEDDATETM,103),'') as ASSIGNEDDATETM,
			   isnull(convert(varchar(11),PLANNEDDATETM,103),'') as PLANNEDDATETM,
			   isnull(convert(varchar(11),ACTUALDATETM,103),getdate()) as ACTUALDATETM,
			   isnull(convert(varchar(11),RESCHEDULEDDATETM,103),'') as RESCHEDULEDDATETM,
			   TASKDID,TASKMID ,

			   DATEPART(YEAR, PLANNEDDATETM) AS PYear, RIGHT('00' + CONVERT(NVARCHAR(2), DATEPART(MONTH, PLANNEDDATETM)), 2) as  PMonth,			  
			   RIGHT('00' + CONVERT(NVARCHAR(2), DATEPART(DAY, PLANNEDDATETM)), 2) as PDay,

			   RIGHT('00' + CONVERT(NVARCHAR(2), DATEPART(hour, PLANNEDDATETM)), 2) as Phours,
			   RIGHT('00' + CONVERT(NVARCHAR(2), DATEPART(minute, PLANNEDDATETM)), 2) as Pminutes,

			   --DATEPART(hour, '2017/08/25 08:36') AS DatePartInt,

			   DATEPART(YEAR, ACTUALDATETM) AS AYear,RIGHT('00' + CONVERT(NVARCHAR(2), DATEPART(MONTH, ACTUALDATETM)), 2) as  AMonth,
			   ISNULL(RIGHT('00' + CONVERT(NVARCHAR(2), DATEPART(DAY, ACTUALDATETM)), 2),'00') as ADay,
			   ISNULL(RIGHT('00' + CONVERT(NVARCHAR(2), DATEPART(hour, ACTUALDATETM)), 2),'00') as Ahours,
			   ISNULL(RIGHT('00' + CONVERT(NVARCHAR(2), DATEPART(minute, ACTUALDATETM)), 2),'00') as Aminutes,

			  --DATEPART(YEAR, ACTUALDATETM) AS AYear,DATEPART(YEAR, ACTUALDATETM) AS AMonth,DATEPART(YEAR, ACTUALDATETM) AS ADay,
			  convert(varchar(10),PLANNEDDATETM,126) as StartDATE,
			  convert(varchar(10),ISNULL(ACTUALDATETM,GETDATE()),126) as EndDATE,
			
			   CASE TASKSTATUS WHEN 0 THEN 'bg-info'  WHEN 1 THEN 'bg-purple' WHEN 2 THEN 'bg-success' END  AS className ,
			   AssignedToEmpid, AssignedToEmp
			   
		FROM    @TableMaster
	   order by TASKDID desc

   SELECT @TotalRowsCount = COUNT(*)
   --FROM     EMPLOYEELINKMASTER EL (NOLOCK)  JOIN 
			--dbo.CRM_SALES_TASK_MASTER TM(NOLOCK) ON EL.CATEID = TM.ASSIGNEDTOEmpID OR EL.LCATEID = TM.ASSIGNEDTOEmpID	Left Join 
			--dbo.CRM_SALES_TASK_DETAIL  TD(NOLOCK) ON  TD.TASKMID = TD.TASKMID Inner Join  
			-- dbo.LEADMASTER (NOLOCK) ON  TM.LEADMID = LEADMASTER.LEADMID  
  FROM     
			dbo.CRM_SALES_TASK_MASTER TM(NOLOCK) JOIN dbo.CRM_SALES_TASK_DETAIL  TD(NOLOCK) ON  TM.TASKMID = TD.TASKMID Inner Join  
			 dbo.LEADMASTER (NOLOCK) ON  TM.LEADMID = LEADMASTER.LEADMID  Left Join 
			 employeemaster e(nolock) on tm.ASSIGNEDTOEmpID = e.CATEID left join
		--EMPLOYEELINKMASTER EL (NOLOCK)  ON  TM.ASSIGNEDTOEmpID = EL.CATEID = OR TM.ASSIGNEDTOEmpID = EL.LCATEID =  LEFT JOIN	
			@EMPTBL EL   ON TM.ASSIGNEDTOEmpID  = EL.SUPEMPID OR  TM.ASSIGNEDTOEmpID = EL.EMPID 	OR TM.CREATEDBY = EL.USERNAME 
	--WHERE (EL.CATEID = @empid OR TM.ASSIGNEDTOEmpID = @empid or @usrid = '')	
    --WHERE (ASSIGNEDTOUsr = @usrid or @usrid = '') 
	--WHERE (EL.CATEID = @empid OR TM.ASSIGNEDTOEmpID = @empid or @usrid = '') 
	WHERE ((EL.SUPEMPID = @empid and @optteam=1) OR TM.ASSIGNEDTOEmpID = @empid or @usrid = '' OR TM.CREATEDBY = @usrid  ) 
	and 	td.TASKDSTATUS = @PTaskStatusId 
	group by TM.LEADMID ,LEADMASTER.LEADNAME,TD.TASKDSTATUS ,TM.LEADSTATUS, 
	                             TM.TASKMNO,TM.TASKMDNO,TD.TASKTYPE ,TD.TASKDESCRIPTION,TM.ASSIGNEDDATETM,TD.PLANNEDDATETM,
								 TD.ACTUALDATETM,TD.RESCHEDULEDDATETM,TD.TASKDID,TM.TASKMID

   --FROM  dbo.CRM_SALES_TASK_DETAIL Left Join 
   --      dbo.CRM_SALES_TASK_MASTER (NOLOCK) ON  CRM_SALES_TASK_DETAIL.TASKMID = CRM_SALES_TASK_MASTER.TASKMID Inner Join  
		 --dbo.LEADMASTER (NOLOCK) ON  CRM_SALES_TASK_MASTER.LEADMID = LEADMASTER.LEADMID    --WHERE CRM_SALES_TASK_DETAIL.TASKDSTATUS = @PTaskStatusId 
   --WHERE (ASSIGNEDTOUsr = @usrid or @usrid = '')  and ASSIGNEDDATETM is not null
	

   SELECT @FilteredRowsCount = COUNT(*)
   FROM   @TableMaster
        


END

