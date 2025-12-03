Write-Host "=== RESETTING PostgreSQL DATABASES ==="

# PostgreSQL connection info
$pgHost = "localhost"
$pgPort = "5432"
$pgUser = "postgres"
$pgPassword = "root"

# Set password for psql
$env:PGPASSWORD = $pgPassword

$pgExe = "C:\Program Files\PostgreSQL\18\bin\psql.exe" # MAKE SURE THAT THIS IS THE CORRECT PATH FOR YOU

$sql = @"
SELECT pg_terminate_backend(pid)
FROM pg_stat_activity
WHERE datname IN ('explorer-v1', 'explorer-v1-test')
  AND pid <> pg_backend_pid();

DROP DATABASE IF EXISTS "explorer-v1";
DROP DATABASE IF EXISTS "explorer-v1-test";
"@

Write-Host "Dropping databases explorer-v1 and explorer-v1-test..."

try {
    $sql | & "$pgExe" -h $pgHost -p $pgPort -U $pgUser -d postgres
    Write-Host "~~~~ Databases dropped successfully ~~~~"
} catch {
    Write-Host "!!!!! Error dropping databases !!!!!"
}

Remove-Item Env:\PGPASSWORD