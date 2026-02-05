-- Cleanup script for Payments integration tests
-- Deletes test data to ensure clean state before each test run
-- Using CASCADE or explicit ordering to avoid FK violations

-- Disable triggers temporarily to avoid FK constraint issues
SET session_replication_role = 'replica';

-- Delete from payments schema in any order (FK constraints disabled)
DELETE FROM payments."Bundles";
DELETE FROM payments."OrderItems";
DELETE FROM payments."CouponRedemptions";
DELETE FROM payments."Coupons";
DELETE FROM payments."TourPurchases";
DELETE FROM payments."TourPurchaseTokens";
DELETE FROM payments."ShoppingCarts";
DELETE FROM payments."Sales";
DELETE FROM payments."Wallets";

-- Delete test tours from tours schema
DELETE FROM tours."Keypoints" WHERE "TourId" IN (1, 2, 6);
DELETE FROM tours."TourEquipment" WHERE "TourId" IN (1, 2, 6);
DELETE FROM tours."TransportTime" WHERE "TourId" IN (1, 2, 6);
DELETE FROM tours."Tour" WHERE "Id" IN (1, 2, 6);

-- Re-enable triggers
SET session_replication_role = 'origin';
