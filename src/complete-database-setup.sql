-- Complete setup script for explorer-v1 database
-- Run this in pgAdmin on the explorer-v1 database

-- Create schemas if they don't exist
CREATE SCHEMA IF NOT EXISTS stakeholders;
CREATE SCHEMA IF NOT EXISTS tours;

-- Insert test users into stakeholders schema
INSERT INTO stakeholders."Users"("Id", "Username", "Password", "Role", "IsActive")
VALUES 
(-1, 'admin@gmail.com', 'admin', 0, true),
(-21, 'turista1@gmail.com', 'turista1', 2, true),
(-22, 'turista2@gmail.com', 'turista2', 2, true),
(-23, 'turista3@gmail.com', 'turista3', 2, true)
ON CONFLICT ("Id") DO UPDATE SET
"Username" = EXCLUDED."Username",
"Password" = EXCLUDED."Password", 
"Role" = EXCLUDED."Role",
"IsActive" = EXCLUDED."IsActive";

-- Insert corresponding people records
INSERT INTO stakeholders."People"("Id", "UserId", "Name", "Surname", "Email")
VALUES
(-21, -21, 'Pera', 'Peri?', 'turista1@gmail.com'),
(-22, -22, 'Mika', 'Miki?', 'turista2@gmail.com'),
(-23, -23, 'Steva', 'Stevi?', 'turista3@gmail.com')
ON CONFLICT ("Id") DO UPDATE SET
"UserId" = EXCLUDED."UserId",
"Name" = EXCLUDED."Name",
"Surname" = EXCLUDED."Surname",
"Email" = EXCLUDED."Email";

-- Insert test problems into tours schema
INSERT INTO tours."Problem"("Id", "TourId", "CreatorId", "Priority", "Description", "CreationTime", "Category")
VALUES 
(-1, -1, -21, 3, 'Bad organization', '2024-01-15T10:30:00Z', 1),
(-2, -2, -22, 5, 'The tour was late', '2024-02-20T14:15:00Z', 2),
(-3, -1, -23, 2, 'Guide was not polite', '2024-03-10T09:45:00Z', 0)
ON CONFLICT ("Id") DO UPDATE SET
"TourId" = EXCLUDED."TourId",
"CreatorId" = EXCLUDED."CreatorId",
"Priority" = EXCLUDED."Priority",
"Description" = EXCLUDED."Description",
"CreationTime" = EXCLUDED."CreationTime",
"Category" = EXCLUDED."Category";

-- Verify the data was inserted correctly
SELECT 'Users Count:' as info, COUNT(*) as count FROM stakeholders."Users"
UNION ALL
SELECT 'People Count:' as info, COUNT(*) as count FROM stakeholders."People"  
UNION ALL
SELECT 'Problems Count:' as info, COUNT(*) as count FROM tours."Problem";