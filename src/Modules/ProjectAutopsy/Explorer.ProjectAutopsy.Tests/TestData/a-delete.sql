-- Brišemo postoje?e podatke iz autopsy šeme
TRUNCATE TABLE autopsy."AIReports" CASCADE;
TRUNCATE TABLE autopsy."RiskSnapshots" CASCADE;
TRUNCATE TABLE autopsy."AutopsyProjects" CASCADE;

-- Resetujemo sekvence
ALTER SEQUENCE autopsy."AIReports_Id_seq" RESTART WITH 1;
ALTER SEQUENCE autopsy."RiskSnapshots_Id_seq" RESTART WITH 1;
ALTER SEQUENCE autopsy."AutopsyProjects_Id_seq" RESTART WITH 1;
