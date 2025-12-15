-- Vrv bi bilo pametno smisleno napraviti datume za CreatedAt, UpdatedAt itd i premestiti dodavanje keypoint-ova u drugu sql skriptu
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
VALUES (
    -15,
    -1,
    'Tour de Vojvodina',
    'Best places in vojvodina',
    1,
    ARRAY['Vojvodina','Serbia','Fun'],
    0,
    0.0,
    '2024-01-10T10:00:00Z',
    '2024-01-10T10:00:00Z',
    '2024-01-10T10:00:00Z',
    '2024-01-10T10:00:00Z'
);

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
VALUES (
    -19,
    -2,
    'Tour de Hungary',
    'Best places in Hungary',
    1,
    ARRAY['Vojvodina','Serbia','Fun'],
    0,
    1.0,
    '2024-02-05T09:30:00Z',
    '2024-02-05T09:30:00Z',
    '2024-01-10T10:00:00Z',
    '2024-01-10T10:00:00Z'
);

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
VALUES (
    -29,
    -3,
    'Tour de Banat',
    'Places...',
    1,
    ARRAY['Vojvodina','Serbia','Fun'],
    0,
    0.0,
    '2024-03-01T14:15:00Z',
    '2024-03-01T14:15:00Z',
    '2024-01-10T10:00:00Z',
    '2024-01-10T10:00:00Z'
);

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
VALUES (
    -1, 
    -1, 
    'Tour de Fruska', 
    'Best places in Fruska', 
    1, 
    ARRAY['Vojvodina','Serbia','Fun'], 
    0, 
    0.0, 
    '2024-03-01T14:15:00Z', 
    '2024-03-01T14:15:00Z', 
    '2024-03-01T14:15:00Z', 
    '2024-03-01T14:15:00Z'
);

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
VALUES (
    -2, 
    -1, 
    'Tour de Srem', 
    'Beautiful Srem region', 
    2, 
    ARRAY['Srem','Serbia','Nature'], 
    1, 
    50.0, 
    '2024-02-01T10:00:00Z', 
    '2024-02-01T10:00:00Z', 
    '2024-02-05T10:00:00Z', 
    '2024-01-01T00:00:00Z'
);

-- Keypoint for Tour -1 (Published tour - used in TourExecution tests)
INSERT INTO tours."Keypoints"(
    "Id", 
    "Title", 
    "Description", 
    "ImageUrl", 
    "Secret", 
    "Latitude", 
    "Longitude", 
    "SequenceNumber", 
    "TourId"
)
VALUES (
    -1, 
    'KP1', 
    'Description', 
    '', 
    '', 
    0, 
    0, 
    1, 
    -1
);

-- Keypoint for Tour -15 (Draft tour - used in keypoint update/delete tests)
INSERT INTO tours."Keypoints"(
    "Id", 
    "Title", 
    "Description", 
    "ImageUrl", 
    "Secret", 
    "Latitude", 
    "Longitude", 
    "SequenceNumber", 
    "TourId"
)
VALUES (
    -2, 
    'KP2', 
    'Draft Tour Keypoint', 
    '', 
    '', 
    0, 
    0, 
    1, 
    -15
);

-- Keypoint for Tour -2
INSERT INTO tours."Keypoints"(
    "Id", 
    "Title", 
    "Description", 
    "ImageUrl", 
    "Secret", 
    "Latitude", 
    "Longitude", 
    "SequenceNumber", 
    "TourId"
)
VALUES (
    -3, 
    'Srem Start', 
    'Starting point', 
    '', 
    '', 
    45.0, 
    19.5, 
    1, 
    -2
);

-- Update Tour -1 to be Published so it can be started in tests
UPDATE tours."Tour" 
SET "Status" = 1, "PublishedAt" = '2024-01-15T10:00:00Z'
WHERE "Id" = -1;

-- Insert TourPurchaseTokens for test tourists
-- These allow tourists -21, -22, -23 to start tour -1
INSERT INTO tours."TourPurchaseTokens"(
    "Id", 
    "TourId", 
    "UserId", 
    "PurchaseDate", 
    "Status"
)
VALUES 
    (-1, -1, -21, '2024-01-15', 'Active'),
    (-2, -1, -22, '2024-01-15', 'Active'),
    (-3, -1, -23, '2024-01-15', 'Active'),
    (-4, -2, -21, '2024-02-10', 'Active'),
    (-5, -2, -22, '2024-02-10', 'Active');

-- TourExecutions for testing
-- Status: 0 = InProgress, 1 = Completed, 2 = Abandoned
INSERT INTO tours."TourExecutions"(
    "Id",
    "TouristId", 
    "TourId", 
    "Status", 
    "StartTime", 
    "EndTime", 
    "LastActivity", 
    "PercentageCompleted"
)
VALUES 
    -- Completed tour for tourist -22 (can leave review - 100% completed)
    (-1, -22, -1, 1, '2024-01-18T09:00:00Z', '2024-01-18T15:00:00Z', '2024-12-10T15:00:00Z', 100.0),
    -- Abandoned tour for tourist -23
    (-2, -23, -1, 2, '2024-01-16T08:00:00Z', '2024-01-16T10:00:00Z', '2024-01-16T10:00:00Z', 20.0);