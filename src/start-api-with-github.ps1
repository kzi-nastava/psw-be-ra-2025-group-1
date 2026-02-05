# Helper script to start the API with GitHub token
# Token is read from environment variable - never hardcode tokens!

Write-Host "Starting Explorer API with GitHub integration..." -ForegroundColor Cyan
Write-Host ""

# Check if token exists
if (-not $env:GITHUB_TOKEN) {
    Write-Host "ERROR: GITHUB_TOKEN environment variable is not set!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Set it using one of these methods:" -ForegroundColor Yellow
    Write-Host "  1. Temporary (this session only):" -ForegroundColor Gray
    Write-Host '     $env:GITHUB_TOKEN = "ghp_your_token_here"' -ForegroundColor White
    Write-Host ""
    Write-Host "  2. Permanent (survives restarts):" -ForegroundColor Gray
    Write-Host '     [Environment]::SetEnvironmentVariable("GITHUB_TOKEN", "ghp_your_token_here", "User")' -ForegroundColor White
    Write-Host "     # Then restart your terminal" -ForegroundColor Gray
    Write-Host ""
    Write-Host "  3. Using .NET User Secrets:" -ForegroundColor Gray
    Write-Host '     cd Explorer.API' -ForegroundColor White
    Write-Host '     dotnet user-secrets init' -ForegroundColor White
    Write-Host '     dotnet user-secrets set "GITHUB_TOKEN" "ghp_your_token_here"' -ForegroundColor White
    Write-Host ""
    exit 1
}

Write-Host "GITHUB_TOKEN found (length: $($env:GITHUB_TOKEN.Length))" -ForegroundColor Green
Write-Host ""
Write-Host "Starting API on http://localhost:5000..." -ForegroundColor Yellow
Write-Host "Press Ctrl+C to stop the server" -ForegroundColor Gray
Write-Host ""

# Start the API
Set-Location -Path $PSScriptRoot
dotnet run --project Explorer.API/Explorer.API.csproj
