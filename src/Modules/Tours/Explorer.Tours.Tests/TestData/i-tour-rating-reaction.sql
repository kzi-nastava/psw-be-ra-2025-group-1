-- Tour Rating Reactions 
-- Reaction 1: User -11 reacted to user -12's rating
INSERT INTO tours."TourRatingReactions"(
    "Id", "TourRatingId", "UserId", "CreatedAt")
    VALUES (-10830, -10822, -11, '2026-01-04T15:00:00Z');

-- Reaction 2: User -12 reacted to user -11's rating
INSERT INTO tours."TourRatingReactions"(
    "Id", "TourRatingId", "UserId", "CreatedAt")
    VALUES (-10831, -10821, -12, '2026-01-04T12:00:00Z');