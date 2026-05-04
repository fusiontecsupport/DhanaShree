use DSFinFusion
go
use FinFusion
go
alter table ChitSubscription
add CollectionAgentPersonID1	int null
go
alter table ChitSubscription
add CollectionAgentPersonID2	int null
go
update ChitSubscription
set CollectionAgentPersonID1 = CommisionAgentPersonID
where CollectionAgentPersonID1  is null

update ChitSubscription
set CommisionAgentPersonID = CommisionAgentPersonID2
