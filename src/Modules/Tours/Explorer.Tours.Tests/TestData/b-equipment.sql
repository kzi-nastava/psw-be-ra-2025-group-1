claudeINSERT INTO tours."Equipment"(
    "Id", "Name", "Description")
VALUES (-1, 'Voda', 'Količina vode varira od temperature i trajanja ture. Preporuka je da se pije pola litre vode na jedan sat umerena fizičke aktivnosti (npr. hajk u prirodi bez značajnog uspona) po umerenoj vrućini');
INSERT INTO tours."Equipment"(
    "Id", "Name", "Description")
VALUES (-2, 'Štapovi za šetanje', 'Štapovi umanjuju umor nogu, pospešuju aktivnost gornjeg dela tela i pružaju stabilnost na neravnom terenu.');
INSERT INTO tours."Equipment"(
    "Id", "Name", "Description")
VALUES (-3, 'Obična baterijska lampa', 'Baterijska lampa od 200 do 400 lumena.');
INSERT INTO tours."Equipment"(
    "Id", "Name", "Description")
VALUES (-4, 'Neobicna baterijska lampa', 'Baterijska lampa od 200000 do 40000000000 lumena.');

-- Reset the sequence to avoid conflicts with auto-generated IDs
-- Using 10000 to give plenty of room for test-generated entities
SELECT setval('tours."Equipment_Id_seq"', 10000, false);