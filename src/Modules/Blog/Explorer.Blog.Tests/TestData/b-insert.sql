INSERT INTO blog."Blogs" ("Id", "UserId", "Title", "Description", "CreationDate", "Images")
VALUES 
    (-1, -1, 'Test Blog 1', 'Test Description 1', '2025-01-01 10:00:00+00', ARRAY['https://example.com/image1.jpg']::text[]),
    (-2, -1, 'Test Blog 2', 'Test Description 2', '2025-01-02 11:00:00+00', ARRAY[]::text[]),
    (-3, -2, 'Different User Test BLog 3', 'Test Description 3; Blog by different user for testing filters.', '2025-01-03 12:00:00+00', ARRAY['https://example.com/image2.jpg', 'https://example.com/image3.jpg']::text[]);