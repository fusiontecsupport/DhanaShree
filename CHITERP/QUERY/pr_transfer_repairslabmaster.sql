-- select * from REPAIRSLABMASTER
-- select * from REPAIRSLABMASTER
-- vw_repair_slab_latest_ratecard

CREATE proc [dbo].[pr_transfer_repairslabmaster]
@usrid varchar(25),
@COMPYID INT,
@frmTRFMID INT,
@toTRFMID INT,
@NewWEFDt datetime,
@FRMSTMR int,
@TOSTMR int,
@prchr numeric(18,2),
@mcostper numeric(18,2),
@reupd int=0
as
begin

	declare @tbl table
	(
	    COMPYID int,
		RSLABMDATE	smalldatetime,
		STMRID	int,
		RPRTID	int,
		RSLABHRS	numeric (18,2),
		RSLABHAMT	numeric(18,2),
		RSLABMAMT	numeric(18,2),
		RSLABTAMT	numeric(18,2),
		TARIFFMID	int,
		WRMMODE	char(10),
		CUSRID	varchar(25),
		LMUSRID	varchar(25),
		DISPSTATUS	smallint,
		PRCSDATE	datetime
	)
	DECLARE @CURDT DATETIME
	SET @CURDT= GETDATE()

	if (@reupd = 0 and not exists (select '*' from REPAIRSLABMASTER (nolock) 
	where TARIFFMID = @toTRFMID and (STMRID = @tostmr OR @tostmr = 0) and RSLABMDATE = @NewWEFDt)) or @reupd = 1
	begin

		if @reupd = 1
		begin
			delete from REPAIRSLABMASTER 
			WHERE (STMRID = @tostmr OR @tostmr = 0 )
			AND TARIFFMID = @toTRFMID 
			and RSLABMDATE = @NewWEFDt
		end

		INSERT INTO @tbl(COMPYID,RSLABMDATE, STMRID, RPRTID, RSLABHRS, RSLABHAMT, RSLABMAMT, RSLABTAMT, 
		TARIFFMID, WRMMODE, CUSRID, LMUSRID, DISPSTATUS, PRCSDATE)
		SELECT @COMPYID, @NewWEFDt, @tostmr,RPRTID, RSLABHRS, @prchr, RSLABMAMT + (RSLABMAMT* @mcostper/100), 
		@prchr+ RSLABMAMT + (RSLABMAMT* @mcostper/100),
		@toTRFMID, WRMMODE, @usrid,NULL,0, @CURDT
		FROM vw_repair_slab_latest_ratecard(NOLOCK)
		WHERE (STMRID = @frmstmr OR @frmstmr = 0 OR @tostmr = 0)
		AND TARIFFMID = @frmTRFMID 

		insert into REPAIRSLABMASTER(COMPYID, RSLABMDATE, STMRID, RPRTID, RSLABHRS, RSLABHAMT, RSLABMAMT, RSLABTAMT, 
		TARIFFMID, WRMMODE, CUSRID, LMUSRID, DISPSTATUS, PRCSDATE)
		select compyid, RSLABMDATE, STMRID, RPRTID, RSLABHRS, RSLABHAMT, RSLABMAMT, RSLABTAMT, 
		TARIFFMID, WRMMODE, CUSRID, LMUSRID, DISPSTATUS, PRCSDATE from @tbl

		Select 'Transferred'
	end
	else
		select 'Not Transferred'

end


