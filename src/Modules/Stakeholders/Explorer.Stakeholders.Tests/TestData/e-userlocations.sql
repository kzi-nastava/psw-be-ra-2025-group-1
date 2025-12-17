INSERT INTO stakeholders."UserLocations"(
    "Id", "UserId", "Latitude", "Longitude", "Timestamp")
VALUES (-1, -21, 45, 20, NOW());

INSERT INTO stakeholders."UserLocations"(
    "Id", "UserId", "Latitude", "Longitude", "Timestamp")
VALUES (-2, -22, 44.7866, 20.4489, NOW());

INSERT INTO stakeholders."UserLocations"(
    "Id", "UserId", "Latitude", "Longitude", "Timestamp")
VALUES (-3, -23, 48.8566, 2.3522, NOW());

-- ============================================================================
-- TEST DATA FOR KEYPOINT PROGRESS
-- ============================================================================

-- Initial location for tourist (ID: -10830)
INSERT INTO stakeholders."UserLocations"(
    "Id", "UserId", "Latitude", "Longitude", "Timestamp")
    VALUES (-10830, -11, 0.0, 0.0, '2024-01-15T09:00:00Z');

-- Location at first keypoint (ID: -10831)
INSERT INTO stakeholders."UserLocations"(
    "Id", "UserId", "Latitude", "Longitude", "Timestamp")
    VALUES (-10831, -11, 1.0, 1.0, '2024-01-15T10:00:00Z');

-- Location at second keypoint (ID: -10832)
INSERT INTO stakeholders."UserLocations"(
    "Id", "UserId", "Latitude", "Longitude", "Timestamp")
    VALUES (-10832, -11, 5.0, 5.0, '2024-01-15T11:00:00Z');

-- Location at third keypoint (ID: -10833)
INSERT INTO stakeholders."UserLocations"(
    "Id", "UserId", "Latitude", "Longitude", "Timestamp")
    VALUES (-10833, -11, 18.0, 18.0, '2024-01-15T12:00:00Z');

-- Location between keypoints (ID: -10834)
INSERT INTO stakeholders."UserLocations"(
    "Id", "UserId", "Latitude", "Longitude", "Timestamp")
    VALUES (-10834, -11, 3.0, 3.0, '2024-01-16T10:30:00Z');

-- Location for percentage test (ID: -10835)
INSERT INTO stakeholders."UserLocations"(
    "Id", "UserId", "Latitude", "Longitude", "Timestamp")
    VALUES (-10835, -11, 2.0, 2.0, '2024-01-16T11:00:00Z');

-- Location for multi-keypoint test (ID: -10836)
INSERT INTO stakeholders."UserLocations"(
    "Id", "UserId", "Latitude", "Longitude", "Timestamp")
    VALUES (-10836, -11, 7.0, 7.0, '2024-01-17T09:00:00Z');
