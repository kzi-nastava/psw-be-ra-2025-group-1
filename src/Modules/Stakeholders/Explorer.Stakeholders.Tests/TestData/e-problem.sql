-- Problem 1: Open problem without deadline
INSERT INTO stakeholders."Problems"(
    "Id", "TourId", "CreatorId", "AuthorId", "Priority", "Description", "CreationTime", "Category", "Status")
VALUES (-1, -1, -21, -11, 3, 'Bad organization', '2024-01-15T10:30:00Z', 1, 0);

-- Problem 2: Open problem with future deadline
INSERT INTO stakeholders."Problems"(
    "Id", "TourId", "CreatorId", "AuthorId", "Priority", "Description", "CreationTime", "Category", "Status", "AdminDeadline")
VALUES (-2, -2, -22, -12, 5, 'The tour was late', '2024-02-20T14:15:00Z', 2, 0, '2025-12-31T23:59:59Z');

-- Problem 3: Open problem (for deletion test)
INSERT INTO stakeholders."Problems"(
    "Id", "TourId", "CreatorId", "AuthorId", "Priority", "Description", "CreationTime", "Category", "Status")
VALUES (-3, -1, -23, -11, 2, 'Guide was not polite', '2024-03-10T09:45:00Z', 0, 0);

-- Problem 4: Resolved by tourist with comment
INSERT INTO stakeholders."Problems"(
    "Id", "TourId", "CreatorId", "AuthorId", "Priority", "Description", "CreationTime", "Category", "Status", "ResolvedAt", "TouristComment")
VALUES (-4, -1, -21, -11, 1, 'Resolved issue', '2024-04-01T10:00:00Z', 0, 1, '2024-04-05T12:00:00Z', 'Issue was fixed quickly');

-- Problem 5: Unresolved problem with comment
INSERT INTO stakeholders."Problems"(
    "Id", "TourId", "CreatorId", "AuthorId", "Priority", "Description", "CreationTime", "Category", "Status", "ResolvedAt", "TouristComment")
VALUES (-5, -1, -21, -11, 4, 'Still waiting for response', '2024-03-15T08:00:00Z', 1, 2, '2024-04-10T14:00:00Z', 'No response from author');

-- Problem 6: Old open problem (overdue - older than 5 days)
INSERT INTO stakeholders."Problems"(
    "Id", "TourId", "CreatorId", "AuthorId", "Priority", "Description", "CreationTime", "Category", "Status")
VALUES (-6, -2, -22, -12, 5, 'Safety hazard not addressed', '2024-01-01T10:00:00Z', 0, 0);

-- Problem 7: Open problem with expired admin deadline
INSERT INTO stakeholders."Problems"(
    "Id", "TourId", "CreatorId", "AuthorId", "Priority", "Description", "CreationTime", "Category", "Status", "AdminDeadline")
VALUES (-7, -1, -21, -11, 5, 'Critical issue with missed deadline', '2024-03-01T10:00:00Z', 0, 0, '2024-03-15T23:59:59Z');
