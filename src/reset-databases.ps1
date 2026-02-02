# ============================================
# RESET DATABASE SCRIPT
# ============================================
# Usage:
#   .\reset-databases.ps1                       # Full reset and seed both databases
#   .\reset-database.ps1 -NoSeed                # Resets both databases, runs migrations, but keeps the main DB empty
#   .\reset-database.ps1 -Tests                 # Keep main DB, reset test DB only
#   .\reset-database.ps1 -KeepDb                # (Alias for -Tests) Keep main DB, reset test DB only
#   .\reset-database.ps1 -Main                  # Delete main DB and recreates it [You need to manually run migrations]
#   .\reset-database.ps1 -Migrate               # Run migrations for all modules 
#   .\reset-database.ps1 -Migrate <ModuleName>  # Run migrations for specific module
#   .\reset-database.ps1 -Module <ModuleName>   # (Alias for -Migrate <ModuleName>)
#   .\reset-database.ps1 -Seed                  # Seed the main database
#   .\reset-database.ps1 -Clear                 # Clear all data from main DB (keep structure), then seed [No need for migrations]
# ============================================

param(
    [switch]$NoSeed,
    [switch]$Tests,
    [switch]$KeepDb,  # Alias for -Tests (backwards compatibility)
    [switch]$Main,
    [switch]$Migrate,
    [string]$Module = "",
    [switch]$Seed,
    [switch]$Clear
)

Write-Host "=== RESETTING PostgreSQL DATABASES ===" -ForegroundColor Cyan

# Backwards compatibility: -KeepDb is an alias for -Tests
if ($KeepDb) {
    $Tests = $true
}

# Alias: -Module <Name> is a shorthand for -Migrate <Name>
if ($Module -ne "" -and -not $Migrate) {
    $Migrate = $true
}

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

# Handle -Migrate command
if ($Migrate) {
    Write-Host "`n=== RUNNING MIGRATIONS ===" -ForegroundColor Cyan
    
    # Set the correct database
    $env:DATABASE_SCHEMA = "explorer-v1"
    
    if ($Module -ne "") {
        # Run migrations for specific module
        Write-Host "Running migrations for module: $Module" -ForegroundColor Yellow
        
        # Map module names to their schemas
        $moduleSchemas = @{
            "Stakeholders" = "stakeholders"
            "Tours" = "tours"
            "Blog" = "blog"
            "Encounters" = "encounters"
            "Payments" = "payments"
            "ProjectAutopsy" = "autopsy"
        }
        
        if (-not $moduleSchemas.ContainsKey($Module)) {
            Write-Host "!!!!! Unknown module: $Module !!!!!" -ForegroundColor Red
            Write-Host "Available modules: $($moduleSchemas.Keys -join ', ')" -ForegroundColor Yellow
            Remove-Item Env:\PGPASSWORD -ErrorAction SilentlyContinue
            exit 1
        }
        
        $schema = $moduleSchemas[$Module]
        
        # Drop and recreate the schema for the specific module
        Write-Host "Dropping and recreating schema: $schema" -ForegroundColor Yellow
        
        $dropSchemaSQL = @"
DROP SCHEMA IF EXISTS "$schema" CASCADE;
CREATE SCHEMA "$schema";
"@
        
        try {
            $dropSchemaSQL | & "$pgExe" -h $pgHost -p $pgPort -U $pgUser -d "explorer-v1"
            
            if ($LASTEXITCODE -ne 0) {
                throw "Failed to drop and recreate schema $schema"
            }
            
            Write-Host "~~~~ Schema $schema dropped and recreated successfully ~~~~" -ForegroundColor Green
        } catch {
            Write-Host "!!!!! Error dropping and recreating schema $schema !!!!!" -ForegroundColor Red
            Write-Host "Error details: $_" -ForegroundColor Red
            Remove-Item Env:\PGPASSWORD -ErrorAction SilentlyContinue
            exit 1
        }
        
        # Run migrations for the specific module
        try {
            & "./reset-migrations.ps1" -Module $Module
            
            if ($LASTEXITCODE -ne 0) {
                throw "Migration failed for module $Module with exit code $LASTEXITCODE"
            }
            
            Write-Host "~~~~ Migrations completed successfully for $Module ~~~~" -ForegroundColor Green
        } catch {
            Write-Host "!!!!! Error running migrations for $Module !!!!!" -ForegroundColor Red
            Write-Host "Error details: $_" -ForegroundColor Red
            Write-Host "NOTE: Did you forget to run the -Main command first?" -ForegroundColor Yellow
            Remove-Item Env:\PGPASSWORD -ErrorAction SilentlyContinue
            exit 1
        }
    } else {
        # Run migrations for all modules
        Write-Host "Running migrations for all modules" -ForegroundColor Yellow
        
        try {
            & "./reset-migrations.ps1"
            
            if ($LASTEXITCODE -ne 0) {
                throw "Migrations failed with exit code $LASTEXITCODE"
            }
            
            Write-Host "~~~~ All migrations completed successfully ~~~~" -ForegroundColor Green
        } catch {
            Write-Host "!!!!! Error running migrations !!!!!" -ForegroundColor Red
            Write-Host "Error details: $_" -ForegroundColor Red
            Write-Host "NOTE: Did you forget to run the -Main command first?" -ForegroundColor Yellow
            Remove-Item Env:\PGPASSWORD -ErrorAction SilentlyContinue
            exit 1
        }
    }
    
    Remove-Item Env:\PGPASSWORD -ErrorAction SilentlyContinue
    Write-Host "`n=== MIGRATION OPERATIONS COMPLETED ===" -ForegroundColor Green
    exit 0
}

# Handle -Seed command
if ($Seed) {
    Write-Host "Mode: Seeding main database" -ForegroundColor Yellow
    
    # Clean up password environment variable
    try {
        Remove-Item Env:\PGPASSWORD -ErrorAction SilentlyContinue
    } catch {
        Write-Host "Warning: Could not remove PGPASSWORD environment variable" -ForegroundColor Yellow
    }
    
    # Set the correct database
    $env:DATABASE_SCHEMA = "explorer-v1"

    Write-Host "`n=== SEEDING DATABASE ===" -ForegroundColor Cyan
    
    try {
        dotnet run --project ./Explorer.API/Explorer.API.csproj --no-launch-profile -- --seed
        
        if ($LASTEXITCODE -ne 0) {
            throw "Database seeding failed with exit code $LASTEXITCODE"
        }
        
        Write-Host "~~~~ Database seeded successfully ~~~~" -ForegroundColor Green
    } catch {
        Write-Host "!!!!! Error seeding database !!!!!" -ForegroundColor Red
        Write-Host "Error details: $_" -ForegroundColor Red
        exit 1
    }

    Write-Host "`n=== SEED OPERATION COMPLETED ===" -ForegroundColor Green
    exit 0
}

# Handle -Clear command
if ($Clear) {
    Write-Host "Mode: Clearing all data from main database (keeping structure)" -ForegroundColor Yellow
    
    # SQL to truncate all tables and reset sequences in all schemas
    $truncateSQL = @"
DO `$`$
DECLARE
    r RECORD;
BEGIN
    -- Truncate all tables
    FOR r IN (SELECT tablename, schemaname FROM pg_tables WHERE schemaname IN ('stakeholders', 'tours', 'blog', 'encounters', 'payments', 'autopsy')) LOOP
        EXECUTE 'TRUNCATE TABLE ' || quote_ident(r.schemaname) || '.' || quote_ident(r.tablename) || ' RESTART IDENTITY CASCADE';
    END LOOP;
END `$`$;
"@

    Write-Host "Clearing all data from tables and resetting sequences..." -ForegroundColor Yellow

    try {
        $truncateSQL | & "$pgExe" -h $pgHost -p $pgPort -U $pgUser -d "explorer-v1"
        
        if ($LASTEXITCODE -ne 0) {
            throw "psql command failed with exit code $LASTEXITCODE"
        }
        
        Write-Host "~~~~ All data cleared and sequences reset successfully ~~~~" -ForegroundColor Green
    } catch {
        Write-Host "!!!!! Error clearing data !!!!!" -ForegroundColor Red
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

    # Set the correct database
    $env:DATABASE_SCHEMA = "explorer-v1"

    # Run seeder
    Write-Host "`n=== SEEDING DATABASE ===" -ForegroundColor Cyan
    
    try {
        dotnet run --project ./Explorer.API/Explorer.API.csproj --no-launch-profile -- --seed
        
        if ($LASTEXITCODE -ne 0) {
            throw "Database seeding failed with exit code $LASTEXITCODE"
        }
        
        Write-Host "~~~~ Database seeded successfully ~~~~" -ForegroundColor Green
    } catch {
        Write-Host "!!!!! Error seeding database !!!!!" -ForegroundColor Red
        Write-Host "Error details: $_" -ForegroundColor Red
        exit 1
    }

    Write-Host "`n=== CLEAR OPERATION COMPLETED ===" -ForegroundColor Green
    exit 0
}

# Handle -Main command (delete and recreate main DB only - no migrations, no seeder)
if ($Main) {
    Write-Host "Mode: Deleting and recreating main database" -ForegroundColor Yellow
    
    # Confirmation prompt for main database deletion
    Write-Host "`nWARNING: This operation will delete the main (explorer-v1) database." -ForegroundColor Yellow
    $confirmation = Read-Host "Are you sure you want to continue? (y/n)"

    if ($confirmation -ne 'y' -and $confirmation -ne 'Y') {
        Write-Host "Canceled." -ForegroundColor Yellow
        Remove-Item Env:\PGPASSWORD -ErrorAction SilentlyContinue
        exit 0
    }
    
    $databasesToDrop = "'explorer-v1'"
    $dropCommands = @"
DROP DATABASE IF EXISTS "explorer-v1";
CREATE DATABASE "explorer-v1";
"@

    $sql = @"
SELECT pg_terminate_backend(pid)
FROM pg_stat_activity
WHERE datname IN ($databasesToDrop)
  AND pid <> pg_backend_pid();
$dropCommands
"@

    Write-Host "Deleting and recreating main database..." -ForegroundColor Yellow

    try {
        $sql | & "$pgExe" -h $pgHost -p $pgPort -U $pgUser -d postgres
        
        if ($LASTEXITCODE -ne 0) {
            throw "psql command failed with exit code $LASTEXITCODE"
        }
        
        Write-Host "~~~~ Main database deleted and recreated successfully ~~~~" -ForegroundColor Green
        Write-Host "REMINDER: You need to manually run migrations using -Migrate command" -ForegroundColor Yellow
    } catch {
        Write-Host "!!!!! Error deleting and recreating main database !!!!!" -ForegroundColor Red
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

    Write-Host "`n=== MAIN DATABASE RESET COMPLETED ===" -ForegroundColor Green
    exit 0
}

# Determine which databases to drop based on flags
if ($Tests) {
    Write-Host "Mode: Keeping main database (explorer-v1), only resetting test database" -ForegroundColor Yellow
    $databasesToDrop = "'explorer-v1-test'"
    $dropCommands = @"
DROP DATABASE IF EXISTS "explorer-v1-test";
CREATE DATABASE "explorer-v1-test";
"@
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

if (-not $Tests) {
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

# Tests mode always skips seeding (can't seed if we kept the main DB)
if (-not $Tests -and -not $NoSeed) {
    Write-Host "`n=== SEEDING DATABASE ===" -ForegroundColor Cyan
    
    try {
        dotnet run --project ./Explorer.API/Explorer.API.csproj --no-launch-profile -- --seed
        
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