USE [MCT_ERP]
GO
/****** Object:  StoredProcedure [dbo].[pr_transfer_LeasingSlabMaster]    Script Date: 03/03/2022 13:26:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER proc [dbo].[pr_Transfer_LeasingSlabMaster]
@usrid varchar(25),
@COMPYID INT,
@frmTRFMID INT,
@toTRFMID INT,
@NewWEFDt datetime,
@FRMSTMR int,
@TOSTMR int,
@pLength numeric(18,2),
@pWidth numeric(18,2),
@prchr numeric(18,2),
@mcostper numeric(18,2),
@reupd int=0
as
begin

	declare @tbl table
	(
	    COMPYID int,
		LSLABMDATE	smalldatetime,
		TARIFFMID int,
		STMRID	int,
		COMPTID	int,
		DMGID	int,
		LRPRID	int,
		UNITID	int,
		LLENGTH	numeric (18,2),
		LWIDTH	numeric(18,2),
		LSLABHRS	numeric(18,2),
		LSLABAMT	numeric(18,2),		
		PRICECODE	char(25),
		CUSRID	varchar(25),
		LMUSRID	varchar(25),
		DISPSTATUS	smallint,
		PRCSDATE	datetime
	)
	DECLARE @CURDT DATETIME
	SET @CURDT= GETDATE()

	if (@reupd = 0 and not exists (select '*' from LEASINGSLABMASTER (nolock) 
	where TARIFFMID = @toTRFMID and (STMRID = @tostmr OR @tostmr = 0) and LSLABMDATE = @NewWEFDt)) or @reupd = 1
	begin

		if @reupd = 1
		begin
			delete from LEASINGSLABMASTER 
			WHERE (STMRID = @tostmr OR @tostmr = 0 )
			AND TARIFFMID = @toTRFMID 
			and LSLABMDATE = @NewWEFDt
		end

		INSERT INTO @tbl(COMPYID,LSLABMDATE,TARIFFMID, STMRID, COMPTID,DMGID,LRPRID,UNITID,
		                 LLENGTH, LWIDTH, LSLABHRS, LSLABAMT,  PRICECODE, CUSRID, LMUSRID, DISPSTATUS, PRCSDATE)
		          SELECT @COMPYID, @NewWEFDt,@toTRFMID, @tostmr,COMPTID, DMGID,LRPRID,UNITID,
		                 LLENGTH,LWIDTH, @prchr,@prchr+ LSLABAMT + (LSLABAMT* @mcostper/100), PRICECODE, @usrid,NULL,0, getdate()
		FROM VW_Leasing_Slab_Latest_Ratecard (NOLOCK)
		WHERE (STMRID = @frmstmr OR @frmstmr = 0 OR @tostmr = 0)
		AND TARIFFMID = @frmTRFMID 

		insert into LEASINGSLABMASTER(COMPYID,LSLABMDATE,TARIFFMID, STMRID, COMPTID,DMGID,LRPRID,UNITID,
		                 LLENGTH, LWIDTH, LSLABHRS, LSLABAMT,  PRICECODE, CUSRID, LMUSRID, DISPSTATUS, PRCSDATE)
		select compyid, LSLABMDATE,TARIFFMID, STMRID,COMPTID, DMGID,LRPRID,UNITID,LLENGTH, LWIDTH, LSLABHRS, LSLABAMT, PRICECODE, 
		       CUSRID, LMUSRID, DISPSTATUS, PRCSDATE from @tbl

		Select 'Transferred'
	end
	else
		select 'Not Transferred'

end

