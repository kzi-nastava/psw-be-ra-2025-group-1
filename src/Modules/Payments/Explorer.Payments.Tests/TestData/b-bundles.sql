-- Test Bundles za Payments integration testove

INSERT INTO payments."Bundles"(
    "Id",
    "AuthorId",
    "Name",
    "Price",
    "Status",
    "TourIds"
)
OVERRIDING SYSTEM VALUE
VALUES
    (-1, -11, 'Draft Bundle', 25.0, 'Draft', ARRAY[1, 2]::bigint[]),
    (-2, -11, 'Published Bundle', 30.0, 'Published', ARRAY[1, 6]::bigint[]),
    (-3, -11, 'Archived Bundle', 20.0, 'Archived', ARRAY[2, 6]::bigint[]);

-- Reset sequence
SELECT setval('payments."Bundles_Id_seq"', 10000, false);
