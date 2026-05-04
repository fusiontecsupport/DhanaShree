ALTER proc pr_Get_Chit_Group_Subscribers_Dtl
@chitgroupid int
as
begin
	set nocount on

	select a.TicketNumber 'TktNum', c.CLIENTNAME 'Name', d.type 'Sts'--, p.FirstName
	from ChitSubscription a(nolock)
		join ChitGroup b(nolock) on a.ChitGroupID = b.ChitGroupID
		left join CLIENTMASTER c(nolock) on a.PersonID = c.CLIENTID
		--left join Person p(nolock) on a.PersonID = p.PersonID
		join mastertype d(nolock) on a.StatusTypeID = d.typeid
	where a.ChitGroupID = @chitgroupid
end


