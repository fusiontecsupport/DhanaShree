/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP (1000) [Id]
      ,[FirstName]
      ,[LastName]
      ,[Email]
      ,[EmpId]
      ,[Designation]
      ,[DeptId]
      ,[DeptName]
      ,[NPassword]
      ,[EPassword]
      ,[Avatar]
      ,[Signature]
      ,[EmailConfirmed]
      ,[PasswordHash]
      ,[SecurityStamp]
      ,[PhoneNumber]
      ,[PhoneNumberConfirmed]
      ,[TwoFactorEnabled]
      ,[LockoutEndDateUtc]
      ,[LockoutEnabled]
      ,[AccessFailedCount]
      ,[UserName]
  FROM [DSFinFusion].[dbo].[AspNetUsers]


  set identity_insert EMPLOYEEMASTER on
  insert into EMPLOYEEMASTER
  (CATEID, CATETID, CATENAME, CATEADDR1,	CATEADDR2,CATEADDR3	,CATEADDR4	,CATEPHN1	,CATEPHN2	,CATEPHN3	,CATEPHN4	,
  CATECPNAME	,CATEEMAIL	,DEPTID	,DSGNID	,LOCTID	,CATEDOB,CATEDOJ,CATEDOC,
  CATEDOR,	CATESTATUS,	CATECODE,CUSRID,	LMUSRID,	DISPSTATUS,	PRCSDATE,REGNID)
  select 4, 1, 'Admin', 'test no',NULL,	NULL,	NULL	,NULL	,NULL	,'6952314515',	NULL,	NULL,
  'test@gmail.com',6,3,	0,	NULL,	NULL,	NULL	,NULL	,0,	0,	'admin', 'admin', 0,getdate(),0

  set identity_insert EMPLOYEEMASTER off