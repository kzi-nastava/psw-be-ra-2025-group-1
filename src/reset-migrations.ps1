param(
    [string]$Module = ""
)

Write-Host "=== RUNNING MIGRATION RESET SCRIPT ===" -ForegroundColor Cyan

# SET THE DATABASE ENVIRONMENT VARIABLE
$env:DATABASE_SCHEMA = "explorer-v1"

# Define all modules and their configurations
$modules = @{
    "Stakeholders" = @{
        Folder = "Modules\Stakeholders\Explorer.Stakeholders.Infrastructure\Migrations"
        Context = "StakeholdersContext"
        Project = "Modules/Stakeholders/Explorer.Stakeholders.Infrastructure"
        MigrationName = "Init"
    }
    "Tours" = @{
        Folder = "Modules\Tours\Explorer.Tours.Infrastructure\Migrations"
        Context = "ToursContext"
        Project = "Modules/Tours/Explorer.Tours.Infrastructure"
        MigrationName = "Init_Tours"
    }
    "Blog" = @{
        Folder = "Modules\Blog\Explorer.Blog.Infrastructure\Migrations"
        Context = "BlogContext"
        Project = "Modules/Blog/Explorer.Blog.Infrastructure"
        MigrationName = "Init_Blog"
    }
    "Encounters" = @{
        Folder = "Modules\Encounters\Explorer.Encounters.Infrastructure\Migrations"
        Context = "EncounterContext"
        Project = "Modules/Encounters/Explorer.Encounters.Infrastructure/Explorer.Encounters.Infrastructure.csproj"
        MigrationName = "Init_Encounters"
    }
    "Payments" = @{
        Folder = "Modules\Payments\Explorer.Payments.Infrastructure\Migrations"
        Context = "PaymentsContext"
        Project = "Modules/Payments/Explorer.Payments.Infrastructure"
        MigrationName = "Init_Payments"
    }
}

function Remove-MigrationFolder {
    param([string]$folder)
    
    if (Test-Path $folder) {
        Remove-Item $folder -Recurse -Force
        Write-Host "Deleted $folder" -ForegroundColor Yellow
    } else {
        Write-Host "Folder not found: $folder" -ForegroundColor Gray
    }
}

function Run-ModuleMigration {
    param(
        [string]$moduleName,
        [hashtable]$config
    )
    
    Write-Host ""
    Write-Host "=== Migrating $($config.Context) ===" -ForegroundColor Cyan
    
    # Add migration
    dotnet ef migrations add $config.MigrationName `
        --context $config.Context `
        --project $config.Project `
        --startup-project Explorer.API
    
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to add migration for $moduleName"
    }
    
    # Update database
    dotnet ef database update `
        --context $config.Context `
        --project $config.Project `
        --startup-project Explorer.API
    
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to update database for $moduleName"
    }
    
    Write-Host "~~~~ $moduleName migration completed ~~~~" -ForegroundColor Green
}

# If a specific module is requested
if ($Module -ne "") {
    if ($modules.ContainsKey($Module)) {
        Write-Host "Running migration for module: $Module" -ForegroundColor Yellow
        
        $config = $modules[$Module]
        
        # Delete migration folder
        Remove-MigrationFolder -folder $config.Folder
        
        # Run migration
        try {
            Run-ModuleMigration -moduleName $Module -config $config
        } catch {
            Write-Host "!!!!! Error migrating $Module !!!!!" -ForegroundColor Red
            Write-Host "Error details: $_" -ForegroundColor Red
            exit 1
        }
    } else {
        Write-Host "!!!!! Unknown module: $Module !!!!!" -ForegroundColor Red
        Write-Host "Available modules: $($modules.Keys -join ', ')" -ForegroundColor Yellow
        exit 1
    }
} else {
    # Run migrations for all modules
    Write-Host "Running migrations for all modules" -ForegroundColor Yellow
    Write-Host ""
    
    # Delete all migration folders
    Write-Host "=== Deleting Migration Folders ===" -ForegroundColor Cyan
    foreach ($moduleName in $modules.Keys) {
        Remove-MigrationFolder -folder $modules[$moduleName].Folder
    }
    
    # Run all migrations
    foreach ($moduleName in $modules.Keys) {
        try {
            Run-ModuleMigration -moduleName $moduleName -config $modules[$moduleName]
        } catch {
            Write-Host "!!!!! Error migrating $moduleName !!!!!" -ForegroundColor Red
            Write-Host "Error details: $_" -ForegroundColor Red
            exit 1
        }
    }
}

Write-Host ""
Write-Host "=== MIGRATION OPERATIONS COMPLETED SUCCESSFULLY ===" -ForegroundColor Green