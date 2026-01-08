-- ============================================================================
-- TEST TOURS
-- ============================================================================

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
    -5, 
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
    -22, 
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

-- Tour 1: Main test tour with 3 keypoints (ID: -10800)
INSERT INTO tours."Tour"(
    "Id", "CreatorId", "Title", "Description", "Difficulty", "Tags", "Status", "Price", "CreatedAt", "UpdatedAt", "PublishedAt", "ArchivedAt")
    VALUES (-10800, -12, 'Test Tour', 'Description for test tour', 2, ARRAY['tag1'], 1, 50.0, '2024-01-01T10:00:00Z', '2024-01-01T10:00:00Z', '2024-01-01T10:00:00Z', '2024-01-01T12:00:00Z');

-- Tour 2: For percentage calculation tests (ID: -10801)
INSERT INTO tours."Tour"(
    "Id", "CreatorId", "Title", "Description", "Difficulty", "Tags", "Status", "Price", "CreatedAt", "UpdatedAt", "PublishedAt", "ArchivedAt")
    VALUES (-10801, -12, 'Percentage Test Tour', 'For testing progress calculations', 2, ARRAY['progress'], 1, 40.0, '2024-01-01T11:00:00Z', '2024-01-01T11:00:00Z', '2024-01-01T11:00:00Z',  '2024-01-01T12:00:00Z');

-- Tour 3: For multiple keypoint tests (ID: -10802)
INSERT INTO tours."Tour"(
    "Id", "CreatorId", "Title", "Description", "Difficulty", "Tags", "Status", "Price", "CreatedAt", "UpdatedAt", "PublishedAt", "ArchivedAt")
    VALUES (-10802, -12, 'Multi Keypoint Tour', 'For testing multiple keypoint progression', 1, ARRAY['multi'], 1, 30.0, '2024-01-01T12:00:00Z', '2024-01-01T12:00:00Z', '2024-01-01T12:00:00Z', '2024-01-01T12:00:00Z');



-- ============================================================================
-- KEYPOINTS FOR TEST TOURS
-- ============================================================================

INSERT INTO tours."Keypoints"(
    "Id", "TourId", "Title", "Description", "ImageUrl", "Secret", "Latitude", "Longitude", "SequenceNumber")
    VALUES (-10800, -10800, 'KP1', 'First keypoint', NULL, NULL, 1.0, 1.0, 1);

INSERT INTO tours."Keypoints"(
    "Id", "TourId", "Title", "Description", "ImageUrl", "Secret", "Latitude", "Longitude", "SequenceNumber")
    VALUES (-10801, -10800, 'KP2', 'Second keypoint', NULL, NULL, 5.0, 5.0, 2);

INSERT INTO tours."Keypoints"(
    "Id", "TourId", "Title", "Description", "ImageUrl", "Secret", "Latitude", "Longitude", "SequenceNumber")
    VALUES (-10802, -10800, 'KP3', 'Third keypoint', NULL, NULL, 18.0, 18.0, 3);

-- Keypoints for Tour -10801
INSERT INTO tours."Keypoints"(
    "Id", "TourId", "Title", "Description", "ImageUrl", "Secret", "Latitude", "Longitude", "SequenceNumber")
    VALUES (-10803, -10801, 'Percentage KP1', 'First keypoint', NULL, NULL, 2.0, 2.0, 1);

INSERT INTO tours."Keypoints"(
    "Id", "TourId", "Title", "Description", "ImageUrl", "Secret", "Latitude", "Longitude", "SequenceNumber")
    VALUES (-10804, -10801, 'Percentage KP2', 'Second keypoint', NULL, NULL, 6.0, 6.0, 2);

INSERT INTO tours."Keypoints"(
    "Id", "TourId", "Title", "Description", "ImageUrl", "Secret", "Latitude", "Longitude", "SequenceNumber")
    VALUES (-10805, -10801, 'Percentage KP3', 'Third keypoint', NULL, NULL, 10.0, 10.0, 3);

-- Keypoints for Tour -10802
INSERT INTO tours."Keypoints"(
    "Id", "TourId", "Title", "Description", "ImageUrl", "Secret", "Latitude", "Longitude", "SequenceNumber")
    VALUES (-10806, -10802, 'Multi KP1', 'First keypoint', NULL, NULL, 3.0, 3.0, 1);

INSERT INTO tours."Keypoints"(
    "Id", "TourId", "Title", "Description", "ImageUrl", "Secret", "Latitude", "Longitude", "SequenceNumber")
    VALUES (-10807, -10802, 'Multi KP2', 'Second keypoint', NULL, NULL, 7.0, 7.0, 2);

INSERT INTO tours."Keypoints"(
    "Id", "TourId", "Title", "Description", "ImageUrl", "Secret", "Latitude", "Longitude", "SequenceNumber")
    VALUES (-10808, -10802, 'Multi KP3', 'Third keypoint', NULL, NULL, 11.0, 11.0, 3);


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

    
-- ============================================================================
-- TOUR EXECUTIONS
-- ============================================================================

-- Status: 0 = InProgress, 1 = Completed, 2 = Abandoned
INSERT INTO tours."TourExecutions"(
    "Id",
    "TouristId", 
    "TourId", 
    "Status", 
    "StartTime", 
    "EndTime", 
    "LastActivity", 
    "PercentageCompleted",
	"CurrentKeypointSequence"
)
VALUES 
    -- Completed tour for tourist -22 (can leave review - 100% completed)
    (-1, -22, -1, 1, '2024-01-18T09:00:00Z', '2024-01-18T15:00:00Z', '2024-12-10T15:00:00Z', 100.0, 0),
    -- Abandoned tour for tourist -23
    (-2, -23, -1, 2, '2024-01-16T08:00:00Z', '2024-01-16T10:00:00Z', '2024-01-16T10:00:00Z', 20.0, 0);

INSERT INTO tours."TourEquipment"(
	"EquipmentId", "TourId")
	VALUES (-1, -5);

INSERT INTO tours."TourEquipment"(
	"EquipmentId", "TourId")
	VALUES (-4, -5);

-- Tour Execution 1: In progress on Tour -10800 (ID: -10810)
INSERT INTO tours."TourExecutions"(
    "Id", "TouristId", "TourId", "Status", "StartTime", "EndTime", "LastActivity", "PercentageCompleted", "CurrentKeypointSequence")
    VALUES (-10810, -11, -10800, 0, '2025-12-15T10:00:00Z', NULL, '2025-12-15T10:00:00Z', 0.0, 1);
    
-- Tour Execution 2: Completed tour (ID: -10811) - Most commonly used in tests
INSERT INTO tours."TourExecutions"(
    "Id", "TouristId", "TourId", "Status", "StartTime", "EndTime", "LastActivity", "PercentageCompleted", "CurrentKeypointSequence")
    VALUES (-10811, -11, -10800, 1, '2025-12-20T09:00:00Z', '2025-12-20T12:00:00Z', '2025-12-20T12:00:00Z', 100.0, 3);
    
-- Tour Execution 3: Old activity for 3-month test (ID: -10812)
INSERT INTO tours."TourExecutions"(
    "Id", "TouristId", "TourId", "Status", "StartTime", "EndTime", "LastActivity", "PercentageCompleted", "CurrentKeypointSequence")
    VALUES (-10812, -11, -10801, 1, '2025-09-01T14:00:00Z', '2025-09-01T15:00:00Z', '2025-09-01T14:20:00Z', 100.0, 1);
    
-- Tour Execution 4: Partially completed (1 keypoint reached) (ID: -10813)
INSERT INTO tours."TourExecutions"(
    "Id", "TouristId", "TourId", "Status", "StartTime", "EndTime", "LastActivity", "PercentageCompleted", "CurrentKeypointSequence")
    VALUES (-10813, -11, -10801, 0, '2025-12-16T11:00:00Z', NULL, '2025-12-16T11:15:00Z', 33.33, 2);
    
-- Tour Execution 5: For multiple keypoint progression tests (ID: -10814)
INSERT INTO tours."TourExecutions"(
    "Id", "TouristId", "TourId", "Status", "StartTime", "EndTime", "LastActivity", "PercentageCompleted", "CurrentKeypointSequence")
    VALUES (-10814, -11, -10802, 0, '2025-12-17T08:00:00Z', NULL, '2025-12-17T08:00:00Z', 0.0, 1);

-- Tour Execution 6: For TourRatings tests (ID: -10815)
INSERT INTO tours."TourExecutions"(
    "Id", "TouristId", "TourId", "Status", "StartTime", "EndTime", "LastActivity", "PercentageCompleted", "CurrentKeypointSequence")
    VALUES (-10815, -11, -10800, 1, '2027-12-17T08:00:00Z', '2027-12-20T12:00:00Z', '2027-12-20T12:00:00Z', 100.0, 3);


-- ============================================================================
-- KEYPOINT PROGRESS (Reached keypoints)
-- ============================================================================

-- Progress for completed execution -10811 (all 3 keypoints)
INSERT INTO tours."KeypointProgress"(
    "Id", "TourExecutionId", "KeypointId", "ReachedAt", "CompletedAt")
    VALUES (-10820, -10811, -10800, '2024-01-14T09:30:00Z', '2024-01-14T09:30:00Z');

INSERT INTO tours."KeypointProgress"(
    "Id", "TourExecutionId", "KeypointId", "ReachedAt", "CompletedAt")
    VALUES (-10821, -10811, -10801, '2024-01-14T10:30:00Z', '2024-01-14T10:30:00Z');

INSERT INTO tours."KeypointProgress"(
    "Id", "TourExecutionId", "KeypointId", "ReachedAt", "CompletedAt")
    VALUES (-10822, -10811, -10802, '2024-01-14T11:30:00Z', '2024-01-14T11:30:00Z');

-- Progress for abandoned execution -10812 (1 keypoint before abandoning)
INSERT INTO tours."KeypointProgress"(
    "Id", "TourExecutionId", "KeypointId", "ReachedAt", "CompletedAt")
    VALUES (-10823, -10812, -10803, '2024-01-13T14:20:00Z', '2024-01-13T14:20:00Z');

-- Progress for partially completed execution -10813 (1 keypoint)
INSERT INTO tours."KeypointProgress"(
    "Id", "TourExecutionId", "KeypointId", "ReachedAt", "CompletedAt")
    VALUES (-10824, -10813, -10803, '2024-01-16T11:15:00Z', '2024-01-16T11:15:00Z');
