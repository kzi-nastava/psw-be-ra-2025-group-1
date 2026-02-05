INSERT INTO tours."MeetUp"(
	"Id", "Name", "Description", "Date", "Longitude", "Latitude", "UserId")
	VALUES (-1, 'Test1', 'Test1', '2025-11-18', 52, 52, 1);
INSERT INTO tours."MeetUp"(
	"Id", "Name", "Description", "Date", "Longitude", "Latitude", "UserId")
	VALUES (-2, 'Test2', 'Test2', '2025-11-19', 52, 52, 2);
INSERT INTO tours."MeetUp"(
	"Id", "Name", "Description", "Date", "Longitude", "Latitude", "UserId")
	VALUES (-3, 'Test3', 'Test3', '2025-11-12', 52, 52, 2);

-- Reset the sequence to avoid conflicts with auto-generated IDs
SELECT setval('tours."MeetUp_Id_seq"', 10000, false);