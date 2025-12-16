INSERT INTO blog."Blogs"(
	"Id", "UserId", "Title", "Description", "CreationDate", "Images", "Status", "LastModifiedDate")
	VALUES (-1, -1, 'test1', 'test1', NOW(), ARRAY['string'], 0, NULL);
INSERT INTO blog."Blogs"(
	"Id", "UserId", "Title", "Description", "CreationDate", "Images", "Status", "LastModifiedDate")
	VALUES (-2, -1, 'test2', 'test2', NOW(), ARRAY['string'], 1, NULL);
INSERT INTO blog."Blogs"(
	"Id", "UserId", "Title", "Description", "CreationDate", "Images", "Status", "LastModifiedDate")
	VALUES (-3, -1, 'test3', 'test3', NOW(), ARRAY['string'], 2, NULL);