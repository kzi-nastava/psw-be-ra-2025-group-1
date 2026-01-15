-- Test Sales za Payment testove
-- Test sales za različite scenario testiranja
-- UPDATED: Using POSITIVE Tour IDs to match f-tours.sql

INSERT INTO payments."Sales"(
    "Id", 
    "Name", 
    "DiscountPercentage", 
    "StartDate", 
    "EndDate", 
    "AuthorId",
    "TourIds"
)
OVERRIDING SYSTEM VALUE
VALUES 
    -- Active sale za tour 1 (10% popust)
    (-1, 'Winter Sale 2024', 10, '2024-01-01', '2024-12-31', -1, '[1]'::jsonb),
    
    -- Active sale za tour 2 (20% popust)
    (-2, 'Spring Sale 2024', 20, '2024-01-01', '2024-12-31', -1, '[2]'::jsonb),
    
    -- Sale sa više tura (15% popust)
    (-3, 'Multi Tour Sale', 15, '2024-01-01', '2024-12-31', -1, '[1, 2]'::jsonb),
    
    -- Expired sale (ne bi trebalo da se primenjuje)
    (-4, 'Past Sale', 25, '2023-01-01', '2023-12-31', -1, '[1]'::jsonb),
    
    -- Future sale (ne bi trebalo da se primenjuje još uvek)
    (-5, 'Future Sale', 30, '2025-01-01', '2025-12-31', -1, '[2]'::jsonb),
    
    -- Sale za drugog autora (author -2)
    (-6, 'Author 2 Sale', 15, '2024-01-01', '2024-12-31', -2, '[6]'::jsonb);

