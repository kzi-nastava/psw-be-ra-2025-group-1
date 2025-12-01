-- Korisnici za messaging testove
INSERT INTO stakeholders."Users"("Id", "Username", "Password", "Role", "IsActive")
VALUES
    (-31, 'messenger1@test.com', 'messenger1', 2, true),
    (-32, 'messenger2@test.com', 'messenger2', 2, true),
    (-33, 'nomessages@test.com', 'nomessages', 2, true);

-- People zapisi (neki testovi zavise od FK)
INSERT INTO stakeholders."People"("Id", "UserId", "Name", "Surname", "Email")
VALUES
    (-31, -31, 'Mes', 'One', 'messenger1@test.com'),
    (-32, -32, 'Mes', 'Two', 'messenger2@test.com'),
    (-33, -33, 'No', 'Messages', 'nomessages@test.com');

-- Jedna konverzacija: -31 ↔ -32
INSERT INTO stakeholders."Conversations"("Id", "User1Id", "User2Id", "StartedAt", "LastMessageAt")
VALUES (-300, -31, -32, NOW(), NOW());

-- Poruke u konverzaciji -300
INSERT INTO stakeholders."Messages"(
    "Id", "SenderId", "ReceiverId", "ConversationId", "Content",
    "Timestamp", "EditedAt", "DeletedAt", "IsDeleted"
) VALUES
    (-3001, -31, -32, -300, 'Hello from -31!', NOW(), NULL, NULL, FALSE),
    (-3002, -32, -31, -300, 'Reply from -32!', NOW(), NULL, NULL, FALSE);
