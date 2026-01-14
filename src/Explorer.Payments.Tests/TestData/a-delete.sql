-- Cleanup script for Payments integration tests
-- Deletes test data to ensure clean state before each test run
-- Using CASCADE or explicit ordering to avoid FK violations

-- Disable triggers temporarily to avoid FK constraint issues
SET session_replication_role = 'replica';

-- Delete from payments schema in any order (FK constraints disabled)
DELETE FROM payments."OrderItems" WHERE "Id" < 0 OR "ShoppingCartId" IN (SELECT "Id" FROM payments."ShoppingCarts" WHERE "Id" < 0 OR "TouristId" >= 100);
DELETE FROM payments."CouponRedemptions" WHERE "CouponId" IN (SELECT "Id" FROM payments."Coupons" WHERE "Id" < 0);
DELETE FROM payments."Coupons" WHERE "Id" < 0;
DELETE FROM payments."TourPurchases" WHERE "Id" < 0 OR "TouristId" >= 100;
DELETE FROM payments."TourPurchaseTokens" WHERE "Id" < 0 OR "UserId" >= 100;
DELETE FROM payments."ShoppingCarts" WHERE "Id" < 0 OR "TouristId" >= 100;
DELETE FROM payments."Sales" WHERE "Id" < 0;
DELETE FROM payments."Wallets" WHERE "Id" < 0;

-- Delete test tours from tours schema
DELETE FROM tours."Keypoints" WHERE "TourId" IN (1, 2, 6);
DELETE FROM tours."TourEquipment" WHERE "TourId" IN (1, 2, 6);
DELETE FROM tours."TransportTime" WHERE "TourId" IN (1, 2, 6);
DELETE FROM tours."Tour" WHERE "Id" IN (1, 2, 6);

-- Re-enable triggers
SET session_replication_role = 'origin';
