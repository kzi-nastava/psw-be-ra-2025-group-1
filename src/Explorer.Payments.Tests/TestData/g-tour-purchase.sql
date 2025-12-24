-- Test data for TourPurchases
-- Tourists -21, -22, -23 purchase tours -1 and -2

DELETE FROM tours."TourPurchases";

INSERT INTO tours."TourPurchases" ("Id", "TouristId", "TourId", "PricePaid", "PurchaseDate")
VALUES 
    (-100, -21, -1, 99.99, NOW() - INTERVAL '5 days'),
    (-101, -21, -2, 99.99, NOW() - INTERVAL '3 days'),
    (-102, -22, -1, 99.99, NOW() - INTERVAL '1 day'),
    (-103, -22, -2, 99.99, NOW() - INTERVAL '1 day'),
    (-104, -23, -1, 99.99, NOW() - INTERVAL '2 days'),
    (-105, -23, -2, 99.99, NOW() - INTERVAL '2 days');

-- Reset sequence to start from 1 for future inserts (not from negative test IDs)
SELECT setval('tours."TourPurchases_Id_seq"', 1, false);
