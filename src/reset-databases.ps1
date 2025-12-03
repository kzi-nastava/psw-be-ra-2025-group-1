# ============================================
# RESET AND SEED DATABASE SCRIPT
# ============================================

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

Write-Host "Dropping databases explorer-v1 and explorer-v1-test..." -ForegroundColor Yellow

try {
    $sql | & "$pgExe" -h $pgHost -p $pgPort -U $pgUser -d postgres
    Write-Host "~~~~ Databases dropped successfully ~~~~" -ForegroundColor Green
} catch {
    Write-Host "!!!!! Error dropping databases !!!!!" -ForegroundColor Red
    Remove-Item Env:\PGPASSWORD
    exit 1
}

Remove-Item Env:\PGPASSWORD

# ============================================
# RESET MIGRATIONS
# ============================================

Write-Host "`n=== RESETTING MIGRATIONS ===" -ForegroundColor Cyan

try {
    & "./reset-migrations.ps1"
    Write-Host "~~~~ Migrations reset successfully ~~~~" -ForegroundColor Green
} catch {
    Write-Host "!!!!! Error resetting migrations !!!!!" -ForegroundColor Red
    exit 1
}

# ============================================
# SEED DATABASE
# ============================================

Write-Host "`n=== SEEDING DATABASE ===" -ForegroundColor Cyan

try {
    dotnet run --project ./Explorer.API/Explorer.API.csproj -- --seed
    Write-Host "~~~~ Database seeded successfully ~~~~" -ForegroundColor Green
} catch {
    Write-Host "!!!!! Error seeding database !!!!!" -ForegroundColor Red
    exit 1
}

Write-Host "`n=== ALL OPERATIONS COMPLETED SUCCESSFULLY ===" -ForegroundColor Green