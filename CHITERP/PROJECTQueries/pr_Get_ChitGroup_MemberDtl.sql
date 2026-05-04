-- exec pr_Get_ChitGroup_MemberDtl 5
-- select * from ChitGroup
alter  proc pr_Get_ChitGroup_MemberDtl
@ChitGrpid int
as

begin

	Select A.ChitGroupId, isnull(a.MaxGrpMembers,0) 'MaxGrpMembers', Isnull(Count(b.ChitGroupID),0) 'GrpMembersCnt', 
	isnull(MaxGrpMembers,0) - Isnull(Count(b.ChitGroupID),0) 'BalanceGrpMembersCnt',
	ChitGroupTypeID, ChitGroupTypeDesc = case when ChitGroupTypeID = 1 then 'GENERAL GROUP' else 'CHIT ACTUAL GROUP' end
	from ChitGroup a(nolock) left join ChitSubscription b(nolock) on a.ChitGroupId = B.ChitGroupId 
	where a.ChitGroupId = @ChitGrpid
	group by A.ChitGroupId, isnull(a.MaxGrpMembers,0), ChitGroupTypeID

end
