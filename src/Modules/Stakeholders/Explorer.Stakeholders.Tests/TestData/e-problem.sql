INSERT INTO stakeholders."Problems"(
    "Id", "TourId", "CreatorId", "AuthorId", "Priority", "Description", "CreationTime", "Category", "Status")
VALUES (-1, -1, -21, -11, 3, 'Bad organization', '2024-01-15T10:30:00Z', 1, 0);

INSERT INTO stakeholders."Problems"(
    "Id", "TourId", "CreatorId", "AuthorId", "Priority", "Description", "CreationTime", "Category", "Status")
VALUES (-2, -2, -22, -12, 5, 'The tour was late', '2024-02-20T14:15:00Z', 2, 0);

INSERT INTO stakeholders."Problems"(
    "Id", "TourId", "CreatorId", "AuthorId", "Priority", "Description", "CreationTime", "Category", "Status")
VALUES (-3, -1, -23, -11, 2, 'Guide was not polite', '2024-03-10T09:45:00Z', 0, 0);

-- Problem with resolved status
INSERT INTO stakeholders."Problems"(
    "Id", "TourId", "CreatorId", "AuthorId", "Priority", "Description", "CreationTime", "Category", "Status", "ResolvedAt", "TouristComment")
VALUES (-4, -1, -21, -11, 1, 'Resolved issue', '2024-04-01T10:00:00Z', 0, 1, '2024-04-05T12:00:00Z', 'Issue was fixed quickly');
