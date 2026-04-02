#!/usr/bin/env pwsh
#!/usr/bin/env pwsh
# Extract specific CodeQL issues and prepare targeted fixes
# This will generate a fixable issues report grouped by file and pattern

$alerts = @(Get-Content c:\git\FuseCPDevOPS-FuseCP\artifacts\codeql-open-alerts.json | ConvertFrom-Json)

# Group by file
$byFile = $alerts | Group-Object { $_.most_recent_instance.location.path }

# Analyze and report
Write-Host "=== BATCH 1: CodeQL Issue Analysis ==="  -ForegroundColor Cyan
Write-Host ""

$batchIssues = @()
$issueCount = 0
$targetRules = @('cs/linq/missed-select', 'cs/linq/missed-where', 'cs/dereferenced-value-may-be-null', 
                   'cs/missed-ternary-operator', 'cs/constant-condition')

foreach ($group in $byFile | Sort-Object -Property Count -Descending | Select-Object -First 15) {
    $file = $group.Name
    $count = $group.Count
    $targetCount = ($group.Group | Where-Object {$_.rule.id -in $targetRules} | Measure-Object).Count
    
    if ($targetCount -gt 0) {
        Write-Host "$count issues in: $file"
        Write-Host "  └─ Batch 1 targets: $targetCount" -ForegroundColor Green
        $issueCount += $targetCount
        $batchIssues += @($group.Group | Where-Object {$_.rule.id -in $targetRules})
    }
}

Write-Host ""
Write-Host "Batch 1 Target Issues: $issueCount" -ForegroundColor Cyan
Write-Host "Remaining to 500: $(500 - $issueCount)"

# Save for next processing step
$batchIssues | ConvertTo-Json -Depth 3 | Set-Content c:\git\FuseCPDevOPS-FuseCP\artifacts\batch1-fixable-issues.json
Write-Host ""
Write-Host "Saved to: batch1-fixable-issues.json" -ForegroundColor Yellow
