-- tourist -21 markers
INSERT INTO tours."MapMarkers"(
	"Id", "ImageUrl", "IsStandalone")
	VALUES (-81010, 'haha', False);
INSERT INTO tours."MapMarkers"(
	"Id", "ImageUrl", "IsStandalone")
	VALUES (-81011, 'hehe', False);
INSERT INTO tours."MapMarkers"(
	"Id", "ImageUrl", "IsStandalone")
	VALUES (-81012, 'hihi', False);
INSERT INTO tours."MapMarkers"(
	"Id", "ImageUrl", "IsStandalone")
	VALUES (-81013, 'hoho', False);

INSERT INTO tours."TouristMapMarkers"(
	"Id", "TouristId", "MapMarkerId", "IsActive")
	VALUES (-81010, -22, -81010, False);
INSERT INTO tours."TouristMapMarkers"(
	"Id", "TouristId", "MapMarkerId", "IsActive")
	VALUES (-81011, -22, -81011, True); -- active marker
INSERT INTO tours."TouristMapMarkers"(
	"Id", "TouristId", "MapMarkerId", "IsActive")
	VALUES (-81012, -22, -81012, False);

