INSERT INTO stakeholders."People"(
    "Id", "UserId", "Name", "Surname", "Email")
VALUES (-1, -1, 'Maja', 'Majic', 'admin@gmail.com');
INSERT INTO stakeholders."People"(
    "Id", "UserId", "Name", "Surname", "Email")
VALUES (-11, -11, 'Ana', 'Anić', 'autor1@gmail.com');
INSERT INTO stakeholders."People"(
    "Id", "UserId", "Name", "Surname", "Email")
VALUES (-12, -12, 'Lena', 'Lenić', 'autor2@gmail.com');
INSERT INTO stakeholders."People"(
    "Id", "UserId", "Name", "Surname", "Email")
VALUES (-13, -13, 'Sara', 'Sarić', 'autor3@gmail.com');

INSERT INTO stakeholders."People"(
    "Id", "UserId", "Name", "Surname", "Email")
VALUES (-21, -21, 'Pera', 'Perić', 'turista1@gmail.com');
INSERT INTO stakeholders."People"(
    "Id", "UserId", "Name", "Surname", "Email")
VALUES (-22, -22, 'Mika', 'Mikić', 'turista2@gmail.com');
INSERT INTO stakeholders."People"(
    "Id", "UserId", "Name", "Surname", "Email")
VALUES (-23, -23, 'Steva', 'Stević', 'turista3@gmail.com');

-- Reset the sequence to avoid conflicts with auto-generated IDs
SELECT setval('stakeholders."People_Id_seq"', 100, true);