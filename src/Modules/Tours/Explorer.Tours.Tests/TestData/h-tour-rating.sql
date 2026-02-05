-- Tour Ratings 
-- Rating 1: Existing rating for testing Get operations
INSERT INTO tours."TourRatings"(
    "Id", "UserId", "TourExecutionId", "Stars", "Comment", "CompletedPercentage", "ThumbsUpCount", "CreatedAt") 
    VALUES (-10820, -11, -10811, 4, 'Great completed tour!', '100.0', '0', '2025-01-14T13:00:00Z');

-- Rating 2: Another existing rating for testing pagination
INSERT INTO tours."TourRatings"(
    "Id", "UserId", "TourExecutionId", "Stars", "Comment", "CompletedPercentage", "ThumbsUpCount", "CreatedAt") 
    VALUES (-10821, -11, -10810, 3, 'Good tour so far', '0.0', '0', '2025-01-15T11:00:00Z');

-- Rating 3: Rating from another user for testing reactions
INSERT INTO tours."TourRatings"(
    "Id", "UserId", "TourExecutionId", "Stars", "Comment", "CompletedPercentage", "ThumbsUpCount", "CreatedAt") 
    VALUES (-10822, -12, -10811, 5, 'Amazing experience!', '100.0', '1', '2025-01-14T14:00:00Z');
