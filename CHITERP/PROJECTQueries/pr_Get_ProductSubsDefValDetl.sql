-- EXEC pr_Get_ProductSubsDefValDetl @prod=29,@subsid=12
ALTER proc pr_Get_ProductSubsDefValDetl
@prod int,
@subsid int
as
begin
	DECLARE @TableMaster TABLE  
    (   
	subsid INT,
	 Defclientid int, 
	 ClientName varchar(200),
    Deftrantypeid int ,
    Deftranmodeid int ,
	DefIntervalId int,
    DefAmt numeric(18,2)
	)
	--select * from chitsubscription
	
	if (@prod=29)
	begin
		INSERT INTO @TableMaster(subsid, Defclientid , ClientName,  Deftrantypeid , Deftranmodeid,  DefIntervalId, DefAmt)     
  
		SELECT b.SubscriptionID, b.PersonID, c.clientname , b.CollectionTypeID , b.CollectionModeTypeID, b.CollectionIntervalTypeID,
		d.CollectionAmount
		FROM chitsubscription b(nolock)  join ClientMaster c(nolock) on b.PersonID = c.CLIENTID			
		join ChitSchemeCollection d(nolock) on b.ChitSchemeID = d.ChitSchemeID and b.CollectionIntervalTypeID = d.CollectionIntervalTypeID
		WHERE SubscriptionID= @subsid
		

	end
	else
	begin
	
		INSERT INTO @TableMaster(subsid, Defclientid , ClientName,  Deftrantypeid , Deftranmodeid,  DefIntervalId, DefAmt)     
  
		SELECT b.LoanSubscriptionID, a.PersonID, c.clientname , b.CollectionType, b.RepaymentModeTypeID, b.RepaymentIntervalTypeID,
		isnull(Inst_Principal,0) + isnull(Interest_Amt,0)
		FROM LoanSubscription b(nolock)  join loanrequest a(nolock) on  b.loanrequestid = a.loanrequestid
		join ClientMaster c(nolock) on a.PersonID = c.CLIENTID			
		WHERE b.LoanSubscriptionID = @subsid

	end
 

	update  a
	set Deftrantypeid = c.typeid
	from @TableMaster   a , MasterType b (nolock) ,  MasterType c (nolock), mastercategory d(nolock)
	where a.Deftrantypeid = b.typeid
	and LTRIM(RTRIM(b.type)) = LTRIM(RTRIM(c.Type))
	and c.TypeCategoryID = d.CategoryID
	and d.Category= 'Transaction Type'

	

	update  a
	set Deftranmodeid = c.typeid
	from @TableMaster   a , MasterType b (nolock) ,  MasterType c (nolock), mastercategory d(nolock)
	where a.Deftranmodeid = b.typeid
	and LTRIM(RTRIM(b.type)) = LTRIM(RTRIM(c.Type))
	and c.TypeCategoryID = d.CategoryID
	and d.Category= 'Transaction Mode'


	SELECT subsid ,  Defclientid , ClientName ,  Deftrantypeid , Deftranmodeid ,  DefIntervalId ,    DefAmt
    FROM    @TableMaster  

	
end

