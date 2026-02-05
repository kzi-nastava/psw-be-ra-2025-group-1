SET session_replication_role = 'replica';

DELETE FROM tours."TourRatingReactions";
DELETE FROM tours."TourRatings";
DELETE FROM tours."KeypointProgress";
DELETE FROM tours."TourExecutions";
DELETE FROM tours."PersonEquipment";
DELETE FROM tours."TourEquipment";
DELETE FROM tours."TransportTime";
DELETE FROM tours."Keypoints";
DELETE FROM tours."TouristMapMarkers";
-- Preserve tours seeded by Payments module (IDs 1, 2, 6) to avoid
-- cross-assembly interference when test projects run in parallel
DELETE FROM tours."Tour" WHERE "Id" NOT IN (1, 2, 6);
DELETE FROM tours."MapMarkers";
DELETE FROM tours."Equipment";
DELETE FROM tours."Facility";
DELETE FROM tours."MeetUp";
DELETE FROM tours."Monuments";
DELETE FROM tours."Restaurants";

SET session_replication_role = 'origin';