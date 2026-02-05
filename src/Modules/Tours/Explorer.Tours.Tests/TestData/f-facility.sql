-- Test data for Facilities - Novi Sad locations
INSERT INTO tours."Facility"(
    "Id", "Name", "Latitude", "Longitude", "Category", "CreatorId", "IsLocalPlace", "CreatedAt", "UpdatedAt", "IsDeleted", "EstimatedPrice")
VALUES (-1, 'Javni WC Trg Slobode', 45.2551, 19.8451, 0, -1, FALSE, '2024-01-10T08:00:00Z', NULL, FALSE, 0);

INSERT INTO tours."Facility"(
    "Id", "Name", "Latitude", "Longitude", "Category", "CreatorId", "IsLocalPlace", "CreatedAt", "UpdatedAt", "IsDeleted", "EstimatedPrice")
VALUES (-2, 'Restoran Plava Frajla', 45.2671, 19.8335, 3, -11, TRUE, '2024-01-15T09:30:00Z', NULL, FALSE, 1);

INSERT INTO tours."Facility"(
    "Id", "Name", "Latitude", "Longitude", "Category", "CreatorId", "IsLocalPlace", "CreatedAt", "UpdatedAt", "IsDeleted", "EstimatedPrice")
VALUES (-3, 'Parking Limanski Park', 45.2517, 19.8369, 2, -12, TRUE, '2024-01-20T10:00:00Z', NULL, FALSE, 2);

INSERT INTO tours."Facility"(
    "Id", "Name", "Latitude", "Longitude", "Category", "CreatorId", "IsLocalPlace", "CreatedAt", "UpdatedAt", "IsDeleted", "EstimatedPrice")
VALUES (-4, 'Prodavnica Merkator', 45.2540, 19.8420, 1, -1, FALSE, '2024-01-25T11:00:00Z', NULL, FALSE, 1);

-- Reset the sequence to avoid conflicts with auto-generated IDs
SELECT setval('tours."Facility_Id_seq"', 10000, false);