INSERT INTO payments."Wallets"(
    "Id", 
    "TouristId", 
    "Balance",
    "CreatedAt"
)
OVERRIDING SYSTEM VALUE
VALUES 
    (-1, -21, 100.0, '2024-01-15 00:00:00'),
    (-2, -22, 200.0, '2024-01-15 00:00:00'),
    (-3, -23, 0.0, '2024-01-15 00:00:00'),
    
    -- Test wallets for integration tests
    (-4, 100, 1000.0, '2024-01-15 00:00:00'),
    (-5, 101, 1000.0, '2024-01-15 00:00:00'),
    (-6, 102, 1000.0, '2024-01-15 00:00:00');