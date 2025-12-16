-- Odkomentiši i izmeni
UPDATE tours."Tour" 
SET "Status" = 1, "Price" = 100.0
WHERE "Id" IN (-1, -2);

UPDATE tours."Tour"
SET "Status" = 0, "Price" = 120.0
WHERE "Id" = -3;