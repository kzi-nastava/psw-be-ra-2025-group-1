-- ======================
-- INSERT USERS
-- ======================
INSERT INTO stakeholders."Users"("Id", "Username", "Password", "Role", "IsActive")
VALUES 
    (-1, 'sender@test.com', 'sender', 0, true),
    (-2, 'receiver@test.com', 'receiver', 0, true),
    (-11, 'autor1@gmail.com', 'autor1', 1, true),
    (-12, 'autor2@gmail.com', 'autor2', 1, true),
    (-13, 'autor3@gmail.com', 'autor3', 1, true),
    (-21, 'turista1@gmail.com', 'turista1', 2, true),
    (-22, 'turista2@gmail.com', 'turista2', 2, true),
    (-23, 'turista3@gmail.com', 'turista3', 2, true);

-- ======================
-- INSERT PEOPLE
-- ======================
INSERT INTO stakeholders."People"("Id", "UserId", "Name", "Surname", "Email")
VALUES
    (-1, -1, 'Sender', 'Test', 'sender@test.com'),
    (-2, -2, 'Receiver', 'Test', 'receiver@test.com'),
    (-11, -11, 'Ana', 'Anić', 'autor1@gmail.com'),
    (-12, -12, 'Lena', 'Lenić', 'autor2@gmail.com'),
    (-13, -13, 'Sara', 'Sarić', 'autor3@gmail.com'),
    (-21, -21, 'Pera', 'Perić', 'turista1@gmail.com'),
    (-22, -22, 'Mika', 'Mikić', 'turista2@gmail.com'),
    (-23, -23, 'Steva', 'Stević', 'turista3@gmail.com');

-- ======================
-- INSERT CONVERSATION (za test poruka)
-- ======================
INSERT INTO stakeholders."Conversations"("Id", "User1Id", "User2Id", "StartedAt", "LastMessageAt")
VALUES
    (-1, -1, -2, NOW(), NOW());

-- ======================
-- INSERT MESSAGES (za test poruka)
-- ======================
INSERT INTO stakeholders."Messages"("Id", "SenderId", "ReceiverId", "ConversationId", "Content", "Timestamp", "EditedAt", "DeletedAt", "IsDeleted")
VALUES
    (-1, -1, -2, -1, 'Hello from sender!', NOW(), NULL, NULL, FALSE),
    (-2, -2, -1, -1, 'Reply from receiver!', NOW(), NULL, NULL, FALSE);
