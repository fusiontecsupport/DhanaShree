SELECT *
--DELETE FROM TransactionCollectionList WHERE ProductID =8
SELECT DISTINCT StatusTypeID FROM TransactionPAYMENTList WHERE StatusTypeID =1034

SELECT * fROM ChitSubscription WHERE SubscriptionID=26

SELECT * fROM TransactionCollectionList A LEFT JOIN ChitSubscription B ON A.ProductID = B.SubscriptionID 
WHERE B.SubscriptionID IS NULL
AND ProductTypeID=29
SELECT * fROM TransactionCollectionList A LEFT JOIN LoanSubscription B ON A.ProductID = B.LoanSubscriptionID 
WHERE B.LoanSubscriptionID IS NULL
AND ProductTypeID=30


SELECT * fROM TransactionPaymentList A LEFT JOIN ChitSubscription B ON A.ProductID = B.SubscriptionID 
WHERE B.SubscriptionID IS NULL
AND ProductTypeTypeID=29
SELECT * fROM TransactionPaymentList A LEFT JOIN LoanSubscription B ON A.ProductID = B.LoanSubscriptionID 
WHERE B.LoanSubscriptionID IS NULL
AND ProductTypeTypeID=30

DELETE A
FROM TransactionPaymentList A LEFT JOIN ChitSubscription B ON A.ProductID = B.SubscriptionID 
WHERE B.SubscriptionID IS NULL
AND ProductTypeTypeID=29

DELETE A
fROM TransactionPaymentList A LEFT JOIN LoanSubscription B ON A.ProductID = B.LoanSubscriptionID 
WHERE B.LoanSubscriptionID IS NULL
AND ProductTypeTypeID=30