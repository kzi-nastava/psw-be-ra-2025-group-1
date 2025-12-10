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
	"Id", "CreatorId", "Title", "Description", "Difficulty", "Tags", "Status", "Price", "CreatedAt", "UpdatedAt", "PublishedAt", "ArchivedAt")
	VALUES (-1, -1, 'Tour de Fruska', 'Best places in Fruska', 1, ARRAY['Vojvodina','Serbia','Fun'], 0, 0.0, '2024-03-01T14:15:00Z', '2024-03-01T14:15:00Z', '2024-03-01T14:15:00Z', '2024-03-01T14:15:00Z');
INSERT INTO tours."Keypoints"(
	"Id", "Title", "Description", "ImageUrl", "Secret", "Latitude", "Longitude", "SequenceNumber", "TourId")
	VALUES (-1, 'KP1', 'Description', '', '', 0, 0, 1, -1);