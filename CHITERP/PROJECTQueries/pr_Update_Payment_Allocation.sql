-- exec pr_Update_Payment_Allocation @paymtlstid = 2,@status =1034
alter proc pr_Update_Payment_Allocation
@paymtlstid int,
@status int
as
begin
	declare @statusdesc varchar(25), @fundac int, @prodtypid int, @prodsubsid int, @pymtid int, @updby int

	select @statusdesc = type
	from MasterType (nolock)
	where TypeID = @status

	select @prodtypid = ProductTypeTypeID, @prodsubsid=ProductID
	from TransactionPaymentList (nolock)
	where PaymentListID = @paymtlstid

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
	if(@statusdesc = 'Approved' or @statusdesc = 'Paid')
	begin
		update TransactionPaymentList
		set ApprovedDate = getdate()
		where PaymentListID = @paymtlstid
		and ApprovedDate is null


		select @updby = b.EmpId
		from TransactionPaymentList(nolock) join AspNetUsers b(nolock) on collectionagent = UserName
		where PaymentListID = @paymtlstid

		update TransactionPaymentList
		set  ApprovedBy = @updby
		where PaymentListID = @paymtlstid
	end

	if(@statusdesc = 'Approved' or @statusdesc = 'Paid')
	and not exists(select '*' from TransactionPayment(nolock) where PaymentListID = @paymtlstid)
	begin
		

		insert into TransactionPayment(PaymentListID, TransactionTypeTypeID, TransactionModeTypeID, PaidAmount, 
						PaymentDateTime, TransactionCategoryTypeID, StatusTypeID, PaidByEmployeeID, ChequeNo, ChequeDate, ChequeBankName, ChequeRealizedDate, 
						FundAccountID)
		select PaymentListID, TransactionTypeTypeID, TransactionModeTypeID, GeneratedAmount, 
		GenerratedDate,TransactionCategoryTypeID, StatusTypeID, 1,ChequeNo, ChequeDate, ChequeBankName, ChequeRealizedDate, 
						FundAccountID
		from TransactionPaymentList (nolock)
		where PaymentListID = @paymtlstid

		select @pymtid = SCOPE_IDENTITY()
	end

	else if exists(select '*' from TransactionPayment(nolock) where PaymentListID = @paymtlstid)
	begin
		select @pymtid = PaymentID 
		from TransactionPayment(nolock) 
		where PaymentListID = @paymtlstid
		
		update TransactionPayment
		set TransactionTypeTypeID = a.TransactionTypeTypeID,
		 TransactionModeTypeID=a.TransactionModeTypeID,
		 PaidAmount=a.GeneratedAmount,
		 PaymentDateTime= a.GenerratedDate,
		 TransactionCategoryTypeID =a.TransactionCategoryTypeID,
		 StatusTypeID=a.StatusTypeID,
		 ChequeNo=a.ChequeNo, 
		 ChequeDate=a.ChequeDate, 
		 ChequeBankName=a.ChequeBankName, 
		 ChequeRealizedDate=a.ChequeRealizedDate, 
		 FundAccountID=a.FundAccountID
		from TransactionPaymentList a(nolock) join TransactionPayment b on a.PaymentListID = b.PaymentListID
		where a.PaymentListID = @paymtlstid
		
	end


	if(@fundac>0 and @pymtid>0)
	and not exists(select '*' from TransactionPaymentAllocation(nolock) where PaymentID = @pymtid)
	begin
		insert into TransactionPaymentAllocation(PaymentID, FundAccountID, Amount, StatusTypeID)
		select @pymtid, @fundac, PaidAmount, StatusTypeID
		from TransactionPayment (nolock)
		where PaymentID = @pymtid
	end

	else if exists(select '*' from TransactionPaymentAllocation(nolock) where PaymentID = @pymtid)
	begin
		update TransactionPaymentAllocation
		set FundAccountID = @fundac, Amount = PaidAmount, StatusTypeID=a.StatusTypeID
		from TransactionPayment  a(nolock) join TransactionPaymentAllocation b on a.PaymentID = b.PaymentID
		where a.PaymentID = @paymtlstid
		
	end
	
end