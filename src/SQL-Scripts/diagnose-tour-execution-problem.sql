-- ?? DIJAGNOSTIKA - Tour Execution Problem
-- Provera da li su svi podaci na mestu

-- 1. Da li postoji tabela TourPurchases?
SELECT EXISTS (
    SELECT FROM information_schema.tables 
    WHERE table_schema = 'tours' 
    AND table_name = 'TourPurchases'
) as "TourPurchasesTableExists";

-- 2. Provera tura - da li su Published ili Archived?
SELECT "Id", "Title", "Status", "Price"
FROM tours."Tour"
WHERE "Id" IN (-15, -19, -29, -1, -2)
ORDER BY "Id";

-- Status: 0=Draft, 1=Published, 2=Archived
-- Ture MORAJU biti 1 ili 2 da bi se mogle pokrenuti!

-- 3. Provera kupovina - da li turista -21 ima kupljene ture?
SELECT * 
FROM tours."TourPurchases"
WHERE "TouristId" = -21;

-- 4. Provera da li postoje aktivne tour executions
SELECT * 
FROM tours."TourExecutions"
WHERE "TouristId" = -21 
AND "Status" = 0;  -- 0 = InProgress

-- 5. DIJAGNOZA PROBLEMA:
-- Ako je broj redova u koraku 3 = 0, problem je što NEMA KUPOVINA
-- Ako je status u koraku 2 = 0 (Draft), problem je što TURE NISU OBJAVLJENE
-- Ako je broj redova u koraku 4 > 0, problem je što VE? POSTOJI AKTIVNA TURA
