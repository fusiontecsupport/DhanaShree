use KGK_ERP


-- DROP TABLE SRFMASTEREDTTEST220922
-- DROP TABLE SRFDETAILEDTTEST220922
select * INTO SRFMASTEREDTTEST220922 From SRFMASTER (nolock) where srfmid = 2298
select * INTO SRFDETAILEDTTEST220922 From SRFDETAIL (nolock) where srfmid = 2298


select *  From SRFMASTEREDTTEST220922 (nolock) where srfmid = 2298
select * From SRFMASTER (nolock) where srfmid = 2298
update SRFMASTER 
set ITEMGID=66, itemid=3914,OPFNSID=2,REGNID=1,DPIMID=5--SRFSTYPE0/1
where srfmid = 2298
select *  From SRFDETAILEDTTEST220922 (nolock) where srfmid = 2298
select * From SRFDETAIL (nolock) where srfmid = 2298