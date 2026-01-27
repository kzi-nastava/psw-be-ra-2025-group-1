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
    [switch]$KeepDb,
	[switch]$ReseedData
)

Write-Host "=== RESETTING PostgreSQL DATABASES ===" -ForegroundColor Cyan

# Set the flag so we can use the main DB
$env:RUNNING_TESTS = "false"

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
        Remove-Item Env:\PGPASSWORD -ErrorAction SilentlyContinue
        exit 0
    }
    Write-Host "Mode: Resetting both databases" -ForegroundColor Yellow
    $databasesToDrop = "'explorer-v1', 'explorer-v1-test'"
    $dropCommands = @"
DROP DATABASE IF EXISTS "explorer-v1";
CREATE DATABASE "explorer-v1";
DROP DATABASE IF EXISTS "explorer-v1-test";
CREATE DATABASE "explorer-v1-test";
"@
}

if ($ReseedData) {
    Write-Host "Mode: Clearing and reseeding data only (schema intact)" -ForegroundColor Yellow

    # List of module schemas
    $schemas = @("Tours", "Payments", "Blogs", "Stakeholders", "Encounters")

    foreach ($schema in $schemas) {
        Write-Host "Clearing data in schema $schema..." -ForegroundColor Cyan

        $sql = @"
DO \$\$ 
DECLARE
    r RECORD;
BEGIN
    FOR r IN (SELECT tablename 
              FROM pg_tables 
              WHERE schemaname = '$schema') 
    LOOP
        EXECUTE 'TRUNCATE TABLE ' || quote_ident('$schema') || '.' || quote_ident(r.tablename) || ' CASCADE;';
    END LOOP;
END
\$\$;
"@

        try {
            $sql | & "$pgExe" -h $pgHost -p $pgPort -U $pgUser -d "explorer-v1"

            if ($LASTEXITCODE -ne 0) {
                throw "Data truncation failed for schema $schema with exit code $LASTEXITCODE"
            }

            Write-Host "~~~~ Data cleared for $schema successfully ~~~~" -ForegroundColor Green
        } catch {
            Write-Host "!!!!! Error truncating data in $schema !!!!!" -ForegroundColor Red
            Write-Host "Error details: $_" -ForegroundColor Red
            exit 1
        }
    }

    # Run seeding after clearing all data
    Write-Host "`n=== SEEDING DATABASE ===" -ForegroundColor Cyan
    try {
        dotnet run --project ./Explorer.API/Explorer.API.csproj -- --seed

        if ($LASTEXITCODE -ne 0) {
            throw "Database seeding failed with exit code $LASTEXITCODE"
        }

        Write-Host "~~~~ Database reseeded successfully ~~~~" -ForegroundColor Green
    } catch {
        Write-Host "!!!!! Error seeding database !!!!!" -ForegroundColor Red
        Write-Host "Error details: $_" -ForegroundColor Red
        exit 1
    }
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
    
    if ($LASTEXITCODE -ne 0) {
        throw "psql command failed with exit code $LASTEXITCODE"
    }
    
    Write-Host "~~~~ Databases dropped successfully ~~~~" -ForegroundColor Green
} catch {
    Write-Host "!!!!! Error dropping databases !!!!!" -ForegroundColor Red
    Write-Host "Error details: $_" -ForegroundColor Red
    Remove-Item Env:\PGPASSWORD -ErrorAction SilentlyContinue
    exit 1
}

# Clean up password environment variable
try {
    Remove-Item Env:\PGPASSWORD -ErrorAction SilentlyContinue
} catch {
    Write-Host "Warning: Could not remove PGPASSWORD environment variable" -ForegroundColor Yellow
}

# ============================================
# RESET MIGRATIONS
# ============================================

if (-not $KeepDb) {
    Write-Host "`n=== RESETTING MIGRATIONS ===" -ForegroundColor Cyan

    try {
        & "./reset-migrations.ps1"
        
        if ($LASTEXITCODE -ne 0) {
            throw "reset-migrations.ps1 failed with exit code $LASTEXITCODE"
        }
        
        Write-Host "~~~~ Migrations reset successfully ~~~~" -ForegroundColor Green
    } catch {
        Write-Host "!!!!! Error resetting migrations !!!!!" -ForegroundColor Red
        Write-Host "Error details: $_" -ForegroundColor Red
        exit 1
    }
}

# ============================================
# SEED DATABASE
# ============================================

# Set the correct database
$env:DATABASE_SCHEMA = "explorer-v1"

# KeepDb mode always skips seeding (can't seed if we kept the main DB)
if (-not $KeepDb -and -not $NoSeed) {
    Write-Host "`n=== SEEDING DATABASE ===" -ForegroundColor Cyan
    
    try {
        dotnet run --project ./Explorer.API/Explorer.API.csproj -- --seed
        
        if ($LASTEXITCODE -ne 0) {
            throw "Database seeding failed with exit code $LASTEXITCODE"
        }
        
        Write-Host "~~~~ Database seeded successfully ~~~~" -ForegroundColor Green
    } catch {
        Write-Host "!!!!! Error seeding database !!!!!" -ForegroundColor Red
        Write-Host "Error details: $_" -ForegroundColor Red
        exit 1
    }
}

Write-Host "`n=== ALL OPERATIONS COMPLETED SUCCESSFULLY ===" -ForegroundColor Green