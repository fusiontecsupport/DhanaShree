alter table Tmp_Ledger_Balance_Detail
add ChitAsonDueAmt numeric(18,2) null

alter table Tmp_Ledger_Balance_Detail
add ChitAsonDivAmt numeric(18,2) null

alter table Tmp_Ledger_Balance_Detail
add ChitAsonPaidDueAmt numeric(18,2) null

alter table Tmp_Ledger_Balance_Detail
add ChitAsonPaidDivAmt numeric(18,2) null


alter table Tmp_Ledger_Balance_Detail
add OverDueAmt numeric(18,2) null

alter table Tmp_Ledger_Balance_Detail
add PassbookTotal numeric(18,2) null


alter table Tmp_Ledger_Balance_Detail
add LastUpdatedDt datetime null


alter table Tmp_Ledger_Balance_Detail
add Tickets int null

alter table Tmp_Ledger_Balance_Detail
add BusinessValue numeric(18,2) null


alter table Tmp_Ledger_Balance_Detail
add LastPaymentDt datetime null