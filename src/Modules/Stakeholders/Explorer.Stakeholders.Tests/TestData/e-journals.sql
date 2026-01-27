INSERT INTO stakeholders."Journals"(
    "Id", "Title", "CreatedAt", "Status", "Latitude", "Longitude", "LocationName", "UserId", "Content", "Images", "Videos", "PublishedBlogId")
VALUES
    (-100, 'Dnevnik1', NOW(), 0, 24, 24,'Novi Sad', 22, 'test1', ARRAY['string'], ARRAY['string'], NULL),
    (-101, 'Dnevnik2', NOW(), 0, 24, 24,'Beograd', 21, 'test2', ARRAY['string'], ARRAY['string'], NULL),
    (-102, 'Dnevnik3', NOW(), 0, 24, 24,'Novi Sad', 23, 'test3', ARRAY['string'], ARRAY['string'], NULL);