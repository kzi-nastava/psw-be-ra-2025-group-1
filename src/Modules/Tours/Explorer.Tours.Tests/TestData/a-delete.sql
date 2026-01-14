-- Clean up payments schema data first (FK dependencies)
DELETE FROM payments."TourPurchaseTokens";

-- Clean up tours schema data in correct order (respecting FK constraints)
DELETE FROM tours."TourRatingReactions";
DELETE FROM tours."TourRatings";
DELETE FROM tours."KeypointProgress";
DELETE FROM tours."TourExecutions";
DELETE FROM tours."PersonEquipment";
DELETE FROM tours."TourEquipment"; -- Equipment-Tour many-to-many relationship
DELETE FROM tours."TransportTime"; -- Transport times are owned by tours
DELETE FROM tours."Keypoints"; -- Keypoints are owned by tours
DELETE FROM tours."Tour";
DELETE FROM tours."Equipment";
DELETE FROM tours."Facility";
DELETE FROM tours."MeetUp";
DELETE FROM tours."Monuments";
DELETE FROM tours."Restaurants";