-- Tour 1: Used by problems -1, -3, -4, -5, -7 (CreatorId = -11 = AuthorId in problems)
INSERT INTO tours."Tour"(
	"Id", "CreatorId", "Title", "Description", "Difficulty", "Tags", "Status", "Price", "CreatedAt", "UpdatedAt", "PublishedAt", "ArchivedAt")
	VALUES (-1, -11, 'Mountain Adventure Tour', 'Explore the beautiful mountains', 3, ARRAY['nature','adventure','hiking'], 1, 150.0, '2024-03-01T14:15:00Z', '2024-03-01T14:15:00Z', '2024-03-01T14:15:00Z', '2024-03-01T14:15:00Z');

-- Tour 2: Used by problems -2, -6 (CreatorId = -12 = AuthorId in problems)
INSERT INTO tours."Tour"(
	"Id", "CreatorId", "Title", "Description", "Difficulty", "Tags", "Status", "Price", "CreatedAt", "UpdatedAt", "PublishedAt", "ArchivedAt")
	VALUES (-2, -12, 'City Walking Tour', 'Discover the city on foot', 1, ARRAY['city','culture','walking'], 1, 50.0, '2024-03-01T14:15:00Z', '2024-03-01T14:15:00Z', '2024-03-01T14:15:00Z', '2024-03-01T14:15:00Z');

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

-- Used to be tour id -1, changed to -5
INSERT INTO tours."Tour"(
	"Id", "CreatorId", "Title", "Description", "Difficulty", "Tags", "Status", "Price", "CreatedAt", "UpdatedAt", "PublishedAt", "ArchivedAt")
	VALUES (-5, -1, 'Tour de Fruska', 'Best places in Fruska', 1, ARRAY['Vojvodina','Serbia','Fun'], 0, 0.0, '2024-03-01T14:15:00Z', '2024-03-01T14:15:00Z', '2024-03-01T14:15:00Z', '2024-03-01T14:15:00Z');

-- Insert keypoint into tour
INSERT INTO tours."Keypoints"(
	"Id", "Title", "Description", "ImageUrl", "Secret", "Latitude", "Longitude", "SequenceNumber", "TourId")
	VALUES (-1, 'KP1', 'Description', '', '', 0, 0, 1, -5);

-- Insert equipment into tour
INSERT INTO tours."TourEquipment"(
	"EquipmentId", "TourId")
	VALUES (-1, -5);

INSERT INTO tours."TourEquipment"(
	"EquipmentId", "TourId")
	VALUES (-4, -5);