Write-Host "=== RUNNING PAYMENTS MIGRATION SCRIPT ===" -ForegroundColor Cyan

# SET THE DATABASE ENVIRONMENT VARIABLE
$env:DATABASE_SCHEMA = "explorer-v1"

Write-Host ""

Write-Host "=== Migrating PaymentsContext ===" -ForegroundColor Cyan
dotnet ef migrations add InitPayments `
    --context PaymentsContext `
    --project Explorer.Payments.Infrastructure `
    --startup-project Explorer.API

dotnet ef database update `
    --context PaymentsContext `
    --project Explorer.Payments.Infrastructure `
    --startup-project Explorer.API

Write-Host ""
Write-Host "=== PAYMENTS MIGRATION COMPLETE ===" -ForegroundColor Green
