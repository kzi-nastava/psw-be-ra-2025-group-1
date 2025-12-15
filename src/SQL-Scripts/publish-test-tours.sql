-- Update test tours to Published status so they appear in the frontend
-- Status: 0 = Draft, 1 = Published, 2 = Archived

UPDATE tours."Tour"
SET "Status" = 1
WHERE "Id" IN (-15, -19, -29);

-- Verify the update
SELECT "Id", "Title", "Status", "Price" FROM tours."Tour";
