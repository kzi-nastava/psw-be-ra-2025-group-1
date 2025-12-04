# ============================================
# RESET DATABASE SCRIPT
# ============================================
# Usage:
#   .\reset-databases.ps1             # Full reset and seed both databases
#   .\reset-database.ps1 -NoSeed      # Runs migrations, but keeps the main DB empty
#   .\reset-database.ps1 -KeepDb      # Keep main DB, reset test DB only
# ============================================

param(
    [switch]$NoSeed,
    [switch]$KeepDb
)

Write-Host "=== RESETTING PostgreSQL DATABASES ===" -ForegroundColor Cyan

# PostgreSQL connection info
$pgHost = "localhost"
$pgPort = "5432"
$pgUser = "postgres"
$pgPassword = "root"

# Set password for psql
$env:PGPASSWORD = $pgPassword
$pgExe = "C:\Program Files\PostgreSQL\18\bin\psql.exe"    # MAKE SURE THAT THIS IS THE CORRECT PATH FOR YOU

# Determine which databases to drop based on flags
if ($KeepDb) {
    Write-Host "Mode: Keeping main database (explorer-v1), only resetting test database" -ForegroundColor Yellow
    $databasesToDrop = "'explorer-v1-test'"
    $dropCommands = 'DROP DATABASE IF EXISTS "explorer-v1-test";'
} else {
    # Confirmation prompt for main database deletion
    Write-Host "`nWARNING: This operation will delete the main (explorer-v1) database." -ForegroundColor Yellow
    $confirmation = Read-Host "Are you sure you want to continue? (y/n)"

    if ($confirmation -ne 'y' -and $confirmation -ne 'Y') {
        Write-Host "Canceled." -ForegroundColor Yellow
        Remove-Item Env:\PGPASSWORD
        exit 0
    }
    Write-Host "Mode: Resetting both databases" -ForegroundColor Yellow
    $databasesToDrop = "'explorer-v1', 'explorer-v1-test'"
    $dropCommands = @"
DROP DATABASE IF EXISTS "explorer-v1";
DROP DATABASE IF EXISTS "explorer-v1-test";
"@
}

$sql = @"
SELECT pg_terminate_backend(pid)
FROM pg_stat_activity
WHERE datname IN ($databasesToDrop)
  AND pid <> pg_backend_pid();
$dropCommands
"@

Write-Host "Dropping databases..." -ForegroundColor Yellow

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

if (-not $KeepDb) {
    Write-Host "`n=== RESETTING MIGRATIONS ===" -ForegroundColor Cyan

    try {
        & "./reset-migrations.ps1"
        Write-Host "~~~~ Migrations reset successfully ~~~~" -ForegroundColor Green
    } catch {
        Write-Host "!!!!! Error resetting migrations !!!!!" -ForegroundColor Red
        exit 1
    }
}

# ============================================
# SEED DATABASE
# ============================================

# KeepDb mode always skips seeding (can't seed if we kept the main DB)
if (-not $KeepDb -and -not $NoSeed) {
    Write-Host "`n=== SEEDING DATABASE ===" -ForegroundColor Cyan
    
    try {
        dotnet run --project ./Explorer.API/Explorer.API.csproj -- --seed
        Write-Host "~~~~ Database seeded successfully ~~~~" -ForegroundColor Green
    } catch {
        Write-Host "!!!!! Error seeding database !!!!!" -ForegroundColor Red
        exit 1
    }
}

Write-Host "`n=== ALL OPERATIONS COMPLETED SUCCESSFULLY ===" -ForegroundColor Green