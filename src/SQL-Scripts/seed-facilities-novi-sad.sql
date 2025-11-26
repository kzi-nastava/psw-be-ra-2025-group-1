-- ================================================
-- SEED FACILITY DATA - NOVI SAD LOCATIONS
-- ================================================
-- Script for inserting facility data with real Novi Sad locations
-- Run this in pgAdmin after deleting old data
-- ================================================

-- Make sure you're connected to the correct database
-- SET search_path TO tours;

-- Insert Facilities in Novi Sad
INSERT INTO tours."Facility"(
    "Id", "Name", "Latitude", "Longitude", "Category", "CreatedAt", "UpdatedAt", "IsDeleted")
VALUES 
    (-1, 'Javni WC Trg Slobode', 45.2551, 19.8451, 0, NOW(), NULL, FALSE),
    (-2, 'Restoran Plava Frajla', 45.2671, 19.8335, 1, NOW(), NULL, FALSE),
    (-3, 'Parking Limanski Park', 45.2517, 19.8369, 2, NOW(), NULL, FALSE);

-- Additional realistic Novi Sad facilities (optional - remove comment to use)
/*
INSERT INTO tours."Facility"(
    "Name", "Latitude", "Longitude", "Category", "CreatedAt", "UpdatedAt", "IsDeleted")
VALUES 
    ('WC Petrovaradinska Tvr?ava', 45.2522, 19.8625, 0, NOW(), NULL, FALSE),
    ('Restoran Lazin Salaš', 45.2396, 19.8494, 1, NOW(), NULL, FALSE),
    ('Restoran Project 72', 45.2547, 19.8428, 1, NOW(), NULL, FALSE),
    ('Parking Univerzitet', 45.2472, 19.8517, 2, NOW(), NULL, FALSE),
    ('Parking BIG Shopping Centar', 45.2398, 19.8307, 2, NOW(), NULL, FALSE),
    ('Parking Promenada', 45.2529, 19.8414, 2, NOW(), NULL, FALSE),
    ('WC Strand', 45.2504, 19.8658, 0, NOW(), NULL, FALSE),
    ('Turisti?ki Info Centar', 45.2551, 19.8433, 3, NOW(), NULL, FALSE),
    ('Apoteka Bulevar', 45.2537, 19.8415, 3, NOW(), NULL, FALSE);
*/

-- Verify insertion
SELECT 
    "Id",
    "Name",
    "Latitude",
    "Longitude",
    CASE "Category"
        WHEN 0 THEN 'WC'
        WHEN 1 THEN 'Restaurant'
        WHEN 2 THEN 'Parking'
        WHEN 3 THEN 'Other'
    END as "CategoryName",
    "CreatedAt",
    "IsDeleted"
FROM tours."Facility"
ORDER BY "Id";

-- ================================================
-- CATEGORY LEGEND:
-- 0 = WC (Toaleti)
-- 1 = Restaurant (Restorani)
-- 2 = Parking (Parking)
-- 3 = Other (Ostalo - Info centri, apoteke, itd.)
-- ================================================
