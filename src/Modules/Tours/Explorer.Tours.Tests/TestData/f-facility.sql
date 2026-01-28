-- Test data for Facilities - Novi Sad locations
INSERT INTO tours."Facility"(
    "Id", "Name", "Latitude", "Longitude", "Category", "CreatorId", "IsLocalPlace", "CreatedAt", "UpdatedAt", "IsDeleted", "EstimatedPrice")
VALUES (-1, 'Javni WC Trg Slobode', 45.2551, 19.8451, 0, -1, FALSE, '2024-01-10T08:00:00Z', NULL, FALSE, 0);

INSERT INTO tours."Facility"(
    "Id", "Name", "Latitude", "Longitude", "Category", "CreatorId", "IsLocalPlace", "CreatedAt", "UpdatedAt", "IsDeleted", "EstimatedPrice")
VALUES (-2, 'Restoran Plava Frajla', 45.2671, 19.8335, 1, -11, TRUE, '2024-01-15T09:30:00Z', NULL, FALSE, 1);

INSERT INTO tours."Facility"(
    "Id", "Name", "Latitude", "Longitude", "Category", "CreatorId", "IsLocalPlace", "CreatedAt", "UpdatedAt", "IsDeleted", "EstimatedPrice")
VALUES (-3, 'Parking Limanski Park', 45.2517, 19.8369, 2, -12, TRUE, '2024-01-20T10:00:00Z', NULL, FALSE, 2);