Write-Host "=== RUNNING MIGRATION RESET SCRIPT (PMC) ==="

# Delete old migration folders
$migrationFolders = @(
    "Modules\Stakeholders\Explorer.Stakeholders.Infrastructure\Migrations",
    "Modules\Tours\Explorer.Tours.Infrastructure\Migrations",
    "Modules\Blog\Explorer.Blog.Infrastructure\Migrations"
)

foreach ($folder in $migrationFolders) {
    if (Test-Path $folder) {
        Remove-Item $folder -Recurse -Force
        Write-Host "Deleted $folder"
    } else {
        Write-Host "Folder not found: $folder"
    }
}

Write-Host ""

# Reset migrations
Write-Host "=== Migratating StakeholdersContext ==="
Add-Migration Init -Context StakeholdersContext -Project Explorer.Stakeholders.Infrastructure -StartupProject Explorer.API
Update-Database -Context StakeholdersContext -Project Explorer.Stakeholders.Infrastructure -StartupProject Explorer.API

Write-Host ""
Write-Host "=== Migratating ToursContext ==="
Add-Migration Init_Tours -Context ToursContext -Project Explorer.Tours.Infrastructure -StartupProject Explorer.API
Update-Database -Context ToursContext -Project Explorer.Tours.Infrastructure -StartupProject Explorer.API

Write-Host ""
Write-Host "=== Migratating BlogContext ==="
Add-Migration Init_Blog -Context BlogContext -Project Explorer.Blog.Infrastructure -StartupProject Explorer.API
Update-Database -Context BlogContext -Project Explorer.Blog.Infrastructure -StartupProject Explorer.API

Write-Host ""
Write-Host "=== MIGRATION RESET FINISHED ==="

# Demo seeder

