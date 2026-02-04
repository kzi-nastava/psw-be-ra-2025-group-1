-- Seed test projects
INSERT INTO autopsy."AutopsyProjects" ("Id", "Name", "Description", "GitHubRepoUrl", "JiraProjectKey", "CreatedAt", "LastAnalyzedAt", "IsActive")
VALUES 
    (-1, 'Explorer Backend', 'PSW Explorer backend project', 'https://github.com/kzi-nastava/psw-be-ra-2025-group-1', 'EXPL', NOW(), NULL, true),
    (-2, 'Test Project', 'Test project for autopsy', 'https://github.com/test/project', 'TEST', NOW(), NOW() - INTERVAL '5 days', true),
    (-3, 'Inactive Project', 'Inactive test project', NULL, NULL, NOW() - INTERVAL '30 days', NULL, false);
