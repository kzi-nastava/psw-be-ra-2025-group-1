-- Test Tours za Payments integration testove
-- Ovi podaci omogu?avaju checkout testove koji zahtevaju postojanje tura

-- Insert test tours (POSITIVE IDs to match test expectations)
INSERT INTO tours."Tour"(
    "Id",
    "CreatorId",
    "Title",
    "Description",
    "Difficulty",
    "Tags",
    "Status",
    "Price",
    "CreatedAt",
    "UpdatedAt",
    "PublishedAt",
    "ArchivedAt"
)
OVERRIDING SYSTEM VALUE
VALUES
    (1, -11, 'Test Tour 1 - On Sale', 'Tour for sale testing', 2, ARRAY['test', 'sale'], 1, 15.0, '2024-01-01', '2024-01-01', '2024-01-01', '-infinity'),
    (2, -11, 'Test Tour 2 - On Sale', 'Another tour for sale', 3, ARRAY['test'], 1, 20.0, '2024-01-02', '2024-01-02', '2024-01-02', '-infinity'),
    (6, -11, 'Test Tour 6 - No Sale', 'Tour without sale', 1, ARRAY['test'], 1, 10.0, '2024-01-03', '2024-01-03', '2024-01-03', '-infinity');

-- Reset sequence to avoid conflicts with future inserts
SELECT setval('tours."Tour_Id_seq"', 100, false);

-- Note: Status 1 = Published (required for checkout)
-- Tours must be published to be purchased
-- Using POSITIVE IDs to match CheckoutWithSaleTests expectations
