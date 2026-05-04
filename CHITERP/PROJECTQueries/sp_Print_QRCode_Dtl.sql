-- sp_Print_QRCode_Dtl 'localhost',29,26
-- sp_Print_QRCode_Dtl 'localhost',29,412
alter proc sp_Print_QRCode_Dtl
@baseurl varchar(150),
@prodtype int,
@subsid int
as
begin
	--select	@baseurl+'/Content/images/logo.png' 'logourl', 
	--		'subsqrcodeimgurl' = case 
	--		when @prodtype = 29 then @baseurl + '/CSubsQRCode/'
	--		when @prodtype = 30 then @baseurl + '/LSubsQRCode/'
	--		when @prodtype = 31 then @baseurl + '/DSubsQRCode/' else '/' end+ 
	--		cast(@subsid as varchar(15)) +'.PNG' ,
	--		* from  [COMPANYMASTER](nolock)
	declare @db varchar(50), @path varchar(50), @subscode varchar(100), @subsclientname varchar(150)
	select @subscode = cs.ChitSubscriptionCode, @subsclientname = c.CLIENTNAME
	from ChitSubscription cs (nolock) join clientmaster c(nolock) on	cs.personid = c.clientid
	where SubscriptionID =  @subsid

	Select @db = DB_NAME()
	if @@servername ='ftec009'
		set @path = 'E:\Rajesh\GitRepo\'+@db+'_Works\CHITERP\'
	else
		set @path = 'd:\fusiontec\'+@db+'\'
	select	@path+'Content\images\logo.png' 'logourl', 
			'subsqrcodeimgurl' = case 
			when @prodtype = 29 then @path+ 'CSubsQRCode\'
			when @prodtype = 30 then @path+ 'LSubsQRCode\'
			when @prodtype = 31 then @path+ 'DSubsQRCode\' else '\' end+ 
			cast(@subsid as varchar(15)) +'.PNG' , @subscode 'subscode', @subsclientname 'subsclientname',
			* from  [COMPANYMASTER](nolock)
end

