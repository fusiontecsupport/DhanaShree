alter proc pr_Update_Collection_Receipt_Allocation
@collectionlstid int,
@status int
as
begin
	declare @statusdesc varchar(25), @fundac int, @prodtypid int, @prodsubsid int, @collectionrecptid int, @aprvdstsid int, @updby int

	select @statusdesc = type
	from MasterType (nolock)
	where TypeID = @status

	select @prodtypid = ProductTypeID, @prodsubsid=ProductID
	from TransactionCollectionList (nolock)
	where CollectionListID = @collectionlstid

	if (@prodtypid =29)
	begin
		select @fundac = FundAccountID
		From ChitSubscription (nolock) 
		where SubscriptionID = @prodsubsid
	end

	--if (@prodtypid =30)
	--begin
	--	select @fundac = FundAccountID
	--	From LoanSubscription (nolock) 
	--	where LoanSubscriptionID = @prodsubsid
	--end
	if(@statusdesc = 'Approved' or @statusdesc = 'Collected')
	begin
		update TransactionCollectionList
		set ApprovedDateTime = getdate()
		where CollectionListID = @collectionlstid
		and ApprovedDateTime is null

		select @updby = b.EmpId
		from TransactionCollectionList(nolock) join AspNetUsers b(nolock) on collectionagent = UserName
		where CollectionListID = @collectionlstid

		update TransactionCollectionList
		set  ApprovedByEmployeeID = @updby
		where CollectionListID = @collectionlstid
		and ApprovedByEmployeeID is null
	end

	if(@statusdesc = 'Approved' or @statusdesc = 'Collected')
	and not exists(select '*' from TransactionCollectionReciepts(nolock) where CollectionListID = @collectionlstid)
	begin
		
		insert into TransactionCollectionReciepts(CollectionListID, TransactionTypeTypeID, TransactionModeTypeID, CollectedAmount, 
						CollectionDateTime, TransactionCategoryTypeID, StatusTypeID, ChequeNo, ChequeDate, ChequeBankName )
		select CollectionListID, TransactionTypeTypeID, TransactionModeTypeID, GeneratedAmount, GeneratedDate,TransactionCategoryTypeID, StatusTypeID,
			ChequeNo, ChequeDate, ChequeBankName
		from TransactionCollectionList (nolock)
		where CollectionListID = @collectionlstid

		select @collectionrecptid = SCOPE_IDENTITY()
	end

	else if exists(select '*' from TransactionCollectionReciepts(nolock) where CollectionListID = @collectionlstid)
	begin
		select @collectionrecptid = CollectionRecieptID 
		from TransactionCollectionReciepts(nolock) 
		where CollectionListID = @collectionlstid
		
		update TransactionCollectionReciepts
		set TransactionTypeTypeID = a.TransactionTypeTypeID,
		 TransactionModeTypeID=a.TransactionModeTypeID,
		 CollectedAmount=a.GeneratedAmount,
		 CollectionDateTime= a.GeneratedDate,
		 TransactionCategoryTypeID =a.TransactionCategoryTypeID,
		 StatusTypeID=a.StatusTypeID,
		 ChequeDate= a.ChequeDate,
		 ChequeBankName=a.ChequeBankName,
		 ChequeNo=a.ChequeNo
		from TransactionCollectionList a(nolock) join TransactionCollectionReciepts b on a.CollectionListID = b.CollectionListID
		where a.CollectionListID = @collectionlstid		
	end


	if(@fundac>0 and @collectionrecptid>0)
	and not exists(select '*' from TransactionCollectionAllocation(nolock) where CollectionReceiptID = @collectionrecptid)
	begin
		insert into TransactionCollectionAllocation(CollectionReceiptID, FundAccountID, Amount, StatusTypeID)
		select @collectionrecptid, @fundac, CollectedAmount, StatusTypeID
		from TransactionCollectionReciepts (nolock)
		where CollectionRecieptID = @collectionrecptid
	end

	else if exists(select '*' from TransactionCollectionAllocation(nolock) where CollectionReceiptID = @collectionrecptid)
	begin
		update TransactionCollectionAllocation
		set FundAccountID = @fundac, Amount = CollectedAmount, StatusTypeID=a.StatusTypeID
		from TransactionCollectionReciepts  a(nolock) join TransactionCollectionAllocation b on a.CollectionRecieptID = b.CollectionReceiptID
		where a.CollectionRecieptID = @collectionlstid
		
	end
	
end
