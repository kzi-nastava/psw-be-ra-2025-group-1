-- Clean up payments schema data first (FK dependencies)
DELETE FROM payments."TourPurchaseTokens";

-- Clean up tours schema data
DELETE FROM tours."TourRatingReactions";
DELETE FROM tours."TourRatings";
DELETE FROM tours."TourExecutions";
DELETE FROM tours."PersonEquipment";
DELETE FROM tours."Tour";
DELETE FROM tours."Equipment";
DELETE FROM tours."Facility";
DELETE FROM tours."MeetUp";
DELETE FROM tours."Monuments";DELETE FROM tours."Monuments";DELETE FROM tours."Monuments";