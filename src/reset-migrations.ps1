Write-Host "=== RUNNING MIGRATION RESET SCRIPT ===" -ForegroundColor Cyan

# SET THE DATABASE ENVIRONMENT VARIABLE
$env:DATABASE_SCHEMA = "explorer-v1"

# Delete old migration folders
$migrationFolders = @(
    "Modules\Stakeholders\Explorer.Stakeholders.Infrastructure\Migrations",
    "Modules\Tours\Explorer.Tours.Infrastructure\Migrations",
    "Modules\Blog\Explorer.Blog.Infrastructure\Migrations"
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