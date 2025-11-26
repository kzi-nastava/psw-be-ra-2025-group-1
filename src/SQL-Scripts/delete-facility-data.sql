-- ================================================
-- DELETE EXISTING FACILITY DATA
-- ================================================
-- Script for removing existing facility test data
-- Run this in pgAdmin before inserting new data
-- ================================================

-- Delete all facilities from the tours schema
DELETE FROM tours."Facility" WHERE "Id" IN (-1, -2, -3);

-- Optional: Delete ALL facilities (use with caution!)
-- DELETE FROM tours."Facility";

-- Reset the sequence if needed (optional)
-- SELECT setval('tours."Facility_Id_seq"', 1, false);

-- Verify deletion
SELECT COUNT(*) as "Remaining Facilities" FROM tours."Facility";
