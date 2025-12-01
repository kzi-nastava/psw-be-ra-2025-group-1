INSERT INTO tours."Tour"(
	"Id", "CreatorId", "Title", "Description", "Difficulty", "Tags", "Status", "Price")
	VALUES (-15, -1, 'Tour de Vojvodina', 'Best places in vojvodina', 1, ARRAY['Vojvodina','Serbia','Fun'], 0, 0.0);
INSERT INTO tours."Tour"(
	"Id", "CreatorId", "Title", "Description", "Difficulty", "Tags", "Status", "Price")
	VALUES (-19, -2, 'Tour de Hungary', 'Best places in Hungary', 1, ARRAY['Vojvodina','Serbia','Fun'], 0, 1.0);
INSERT INTO tours."Tour"(
	"Id", "CreatorId", "Title", "Description", "Difficulty", "Tags", "Status", "Price")
	VALUES (-29, -3, 'Tour de Banat', 'Places...', 1, ARRAY['Vojvodina','Serbia','Fun'], 0, 0.0);

INSERT INTO tours."Tour"(
	"Id", "CreatorId", "Title", "Description", "Difficulty", "Tags", "Status", "Price")
	VALUES (-1, -1, 'Tour de Fruska', 'Best places in Fruska', 1, ARRAY['Vojvodina','Serbia','Fun'], 0, 0.0);
INSERT INTO tours."Keypoints"(
	"Id", "Title", "Description", "ImageUrl", "Secret", "Latitude", "Longitude", "SequenceNumber", "TourId")
	VALUES (-1, 'KP1', 'Description', '', '', 0, 0, 1, -1);