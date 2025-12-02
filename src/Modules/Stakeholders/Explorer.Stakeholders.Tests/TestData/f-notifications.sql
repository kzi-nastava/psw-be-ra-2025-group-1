INSERT INTO stakeholders."Notifications"(
    "Id", "UserId", "Message", "Type", "LinkId", "Timestamp", "IsRead")
VALUES (-1, -11, 'New problem reported on your tour', 0, -1, '2024-01-15T10:35:00Z', false);

INSERT INTO stakeholders."Notifications"(
    "Id", "UserId", "Message", "Type", "LinkId", "Timestamp", "IsRead")
VALUES (-2, -21, 'New message from tour author on problem #-1', 0, -1, '2024-01-15T11:00:00Z', false);

INSERT INTO stakeholders."Notifications"(
    "Id", "UserId", "Message", "Type", "LinkId", "Timestamp", "IsRead")
VALUES (-3, -11, 'Problem report #-4 marked as resolved', 0, -4, '2024-04-05T12:00:00Z', true);

INSERT INTO stakeholders."Notifications"(
    "Id", "UserId", "Message", "Type", "Timestamp", "IsRead")
VALUES (-4, -22, 'General notification without link', 2, '2024-05-01T09:00:00Z', false);
