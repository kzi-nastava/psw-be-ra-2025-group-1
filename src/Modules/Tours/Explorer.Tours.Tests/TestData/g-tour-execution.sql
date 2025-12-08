-- Test data for TourExecution tests
-- IMPORTANT: This runs AFTER e-tour.sql

-- Update tour with ID -1 to be Published
UPDATE tours."Tour" 
SET "Status" = 1, "Price" = 100.0
WHERE "Id" = -1;
