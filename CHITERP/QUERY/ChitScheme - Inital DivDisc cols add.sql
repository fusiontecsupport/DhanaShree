use dsfinfusion
go
alter table chitscheme
drop column InitialDiscountPerc
go
alter table chitscheme
drop column InitialDividendPerc
go
alter table chitscheme
add InitialDiscountPerc float null
go

alter table chitscheme
add InitialDividendPerc float null
go

UPDATE chitscheme
SET InitialDiscountPerc = 5 
WHERE InitialDiscountPerc IS NULL

UPDATE chitscheme
SET InitialDividendPerc = 2
WHERE InitialDividendPerc IS NULL