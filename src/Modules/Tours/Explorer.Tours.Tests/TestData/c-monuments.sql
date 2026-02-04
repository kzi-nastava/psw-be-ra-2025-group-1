INSERT INTO tours."Monuments"(
    "Id", "Name", "Description", "CreationYear", "Status", "Longitude", "Latitude")
VALUES (-1, 'Test Monument 1', 'Description for test monument 1', 2020, 0, 20.5, 44.8);

INSERT INTO tours."Monuments"(
    "Id", "Name", "Description", "CreationYear", "Status", "Longitude", "Latitude")
VALUES (-2, 'Test Monument 2', 'Description for test monument 2', 2021, 0, 21.0, 45.0);

INSERT INTO tours."Monuments"(
    "Id", "Name", "Description", "CreationYear", "Status", "Longitude", "Latitude")
VALUES (-3, 'Test Monument 3', 'Description for test monument 3', 2022, 1, 19.8, 44.2);

-- Reset the sequence to avoid conflicts with auto-generated IDs
SELECT setval('tours."Monuments_Id_seq"', 10000, false);