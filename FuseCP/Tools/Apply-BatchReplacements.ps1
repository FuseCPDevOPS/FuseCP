$batch = @(Get-Content "artifacts/replacements-batch.json" | ConvertFrom-Json)
Write-Host "Applying $($batch.Count) batch replacements..."
$applied = 0
$failed = 0

foreach ($replacement in $batch) {
    $absPath = Join-Path "c:\git\FuseCPDevOPS-FuseCP" $replacement.filePath
    
    if (-not (Test-Path $absPath)) {
        Write-Host "✗ Not found: $($replacement.filePath)" -ForegroundColor Yellow
        $failed++
        continue
    }
    
    $content = Get-Content $absPath -Raw
    if ($content.Contains($replacement.oldString)) {
        $newContent = $content.Replace($replacement.oldString, $replacement.newString)
        Set-Content $absPath $newContent -Encoding UTF8 -NoNewline
        $applied++
        Write-Host "✓ $($replacement.filePath)" -ForegroundColor Green
    } else {
        Write-Host "✗ Pattern not found in: $($replacement.filePath)" -ForegroundColor Yellow
        $failed++
    }
}

Write-Host ""
Write-Host "=== Results ===" -ForegroundColor Cyan
Write-Host "Applied: $applied" -ForegroundColor Green
Write-Host "Failed: $failed" -ForegroundColor $(if ($failed -gt 0) { 'Yellow' } else { 'Green' })
Write-Host ""
Write-Host "Total fixed (batch + manual): $($applied + 5)" -ForegroundColor Green
