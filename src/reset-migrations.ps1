Write-Host "=== RUNNING MIGRATION RESET SCRIPT ===" -ForegroundColor Cyan

# SET THE DATABASE ENVIRONMENT VARIABLE
$env:DATABASE_SCHEMA = "explorer-v1"

# Delete old migration folders
$migrationFolders = @(
    "Modules\Stakeholders\Explorer.Stakeholders.Infrastructure\Migrations",
    "Modules\Tours\Explorer.Tours.Infrastructure\Migrations",
    "Modules\Blog\Explorer.Blog.Infrastructure\Migrations", 
    "Modules\Encounters\Explorer.Encounters.Infrastructure\Migrations"
)

foreach ($folder in $migrationFolders) {
    if (Test-Path $folder) {
        Remove-Item $folder -Recurse -Force
        Write-Host "Deleted $folder" -ForegroundColor Yellow
    } else {
        Write-Host "Folder not found: $folder" -ForegroundColor Gray
    }
}

Write-Host ""

Write-Host "=== Migrating StakeholdersContext ===" -ForegroundColor Cyan
dotnet ef migrations add Init `
    --context StakeholdersContext `
    --project Modules/Stakeholders/Explorer.Stakeholders.Infrastructure `
    --startup-project Explorer.API 

dotnet ef database update `
    --context StakeholdersContext `
    --project Modules/Stakeholders/Explorer.Stakeholders.Infrastructure `
    --startup-project Explorer.API 

Write-Host ""

Write-Host "=== Migrating ToursContext ===" -ForegroundColor Cyan
dotnet ef migrations add Init_Tours `
    --context ToursContext `
    --project Modules/Tours/Explorer.Tours.Infrastructure `
    --startup-project Explorer.API

dotnet ef database update `
    --context ToursContext `
    --project Modules/Tours/Explorer.Tours.Infrastructure `
    --startup-project Explorer.API

Write-Host ""

Write-Host "=== Migrating BlogContext ===" -ForegroundColor Cyan
dotnet ef migrations add Init_Blog `
    --context BlogContext `
    --project Modules/Blog/Explorer.Blog.Infrastructure `
    --startup-project Explorer.API

dotnet ef database update `
    --context BlogContext `
    --project Modules/Blog/Explorer.Blog.Infrastructure `
    --startup-project Explorer.API

Write-Host ""

Write-Host "=== Migrating EncounterContext ===" -ForegroundColor Cyan
dotnet ef migrations add Init_Encounters `
    --context EncounterContext `
    --project Modules/Encounters/Explorer.Encounters.Infrastructure/Explorer.Encounters.Infrastructure.csproj `
    --startup-project Explorer.API/Explorer.API.csproj

dotnet ef database update `
    --context EncounterContext `
    --project Modules/Encounters/Explorer.Encounters.Infrastructure/Explorer.Encounters.Infrastructure.csproj `
    --startup-project Explorer.API/Explorer.API.csproj


Write-Host "=== Migrating PaymentsContext ===" -ForegroundColor Cyan
dotnet ef migrations add Init_Payments `
    --context PaymentsContext `
    --project Modules/Payments/Explorer.Payments.Infrastructure `
    --startup-project Explorer.API

dotnet ef database update `
    --context PaymentsContext `
    --project Modules/Payments/Explorer.Payments.Infrastructure `
    --startup-project Explorer.API