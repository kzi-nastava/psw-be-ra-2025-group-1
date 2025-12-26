-- TourPurchaseTokens iz Payments modula
-- Potrebni za TourExecution testove (provera da li turista ima token)

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