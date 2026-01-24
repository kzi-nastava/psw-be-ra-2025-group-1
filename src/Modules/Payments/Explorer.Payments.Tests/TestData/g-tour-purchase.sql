-- TourPurchaseTokens za Tours integration testove
-- Omogu?avaju turistima -21, -22, -23 da pokrenu ture -1 i -2

INSERT INTO payments."TourPurchaseTokens"(
    "Id", 
    "TourId", 
    "UserId", 
    "PurchaseDate", 
    "Status"
)
OVERRIDING SYSTEM VALUE
VALUES 
    (-1, -1, -21, '2024-01-15', 'Active'),
    (-2, -1, -22, '2024-01-15', 'Active'),
    (-3, -1, -23, '2024-01-15', 'Active'),
    (-4, -2, -21, '2024-02-10', 'Active'),
    (-5, -2, -22, '2024-02-10', 'Active');