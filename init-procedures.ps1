# PowerShell script to initialize stored procedures in MySQL container
# Run this after: docker compose up -d

Write-Host "Waiting for MySQL container to be healthy..." -ForegroundColor Yellow
$maxAttempts = 30
$attempt = 0

while ($attempt -lt $maxAttempts) {
    $health = docker inspect capstone-mysql --format='{{.State.Health.Status}}' 2>$null
    if ($health -eq "healthy") {
        Write-Host "MySQL container is healthy!" -ForegroundColor Green
        break
    }
    $attempt++
    Start-Sleep -Seconds 1
}

if ($attempt -eq $maxAttempts) {
    Write-Host "MySQL container did not become healthy in time." -ForegroundColor Red
    exit 1
}

Write-Host "Creating stored procedures..." -ForegroundColor Cyan
Get-Content "create-procedures-manual.sql" | docker exec -i capstone-mysql mysql -uroot -pPoofBall#1 fun_facts_db 2>&1 | Out-Null

if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Stored procedures created successfully!" -ForegroundColor Green
    
    # Verify procedures were created
    Write-Host "`nVerifying procedures..." -ForegroundColor Cyan
    docker exec capstone-mysql mysql -uroot -pPoofBall#1 fun_facts_db -e "SHOW PROCEDURE STATUS WHERE Db = 'fun_facts_db';" 2>&1 | Select-String "sp_"
    
    Write-Host "`n✓ All procedures are ready!" -ForegroundColor Green
} else {
    Write-Host "✗ Failed to create stored procedures." -ForegroundColor Red
    exit 1
}
