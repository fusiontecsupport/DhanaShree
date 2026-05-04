use dsfinfusion
go
select term,c.type,disbursementdate,case when c.type = 'days' then dateadd(d,term,disbursementdate)
		when c.type = 'months' then dateadd(m,term,disbursementdate)end,
		case when getdate()>(case when c.type = 'days' then dateadd(d,term,disbursementdate)
		when c.type = 'months' then dateadd(m,term,disbursementdate)end) then 'Arrear' else '' end,
		a.statustypeid , d.type,
		getdate(),* 
from LoanSubscription a(nolock ) join Tmp_Ledger_Balance_Detail b(nolock) on a.LoanSubscriptionCode = b.LCode
join mastertype c(nolock) on a.termtypeid = c.typeid
join mastertype d(nolock) on a.statustypeid = d.typeid
where balamt>0
and statustypeid = 968 -- 974 Arrear


select StartDate,EndDate,
		case when getdate()>EndDate then 'Arrear' else '' end,
		a.statustypeid , d.type,
		getdate(),* 
from ChitSubscription a(nolock ) join Tmp_Ledger_Balance_Detail b(nolock) on a.chitsubscriptioncode = b.LCode
--join ChitSchemePattern s(nolock) on a.chitschemeid = s.chitschemeid
join mastertype d(nolock) on a.statustypeid = d.typeid
join mastertype e(nolock) on a.collectionintervaltypeid= e.typeid
where balamt>0
and EndDate is not null
and statustypeid = 881 --884 Arrear

