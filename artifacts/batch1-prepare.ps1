#!/usr/bin/env pwsh
$alerts = Get-Content codeql-open-alerts.json -Raw | ConvertFrom-Json
$batch1 = @()

# Collect LINQ and null issues for batch 1
$batch1 += $alerts | Where-Object { $_.rule.id -in 'cs/linq/missed-select', 'cs/linq/missed-where', 'cs/dereferenced-value-may-be-null', 'cs/missed-ternary-operator', 'cs/constant-condition' }

$batch1 | ConvertTo-Json -Depth 5 | Set-Content batch1-codeql-targets.json
Write-Host "Batch 1 target count: $($batch1.Count)"
Write-Host "Categories:"
$batch1 | Group-Object { $_.rule.id } | Sort-Object -Property Count -Descending | ForEach-Object { Write-Host "  $($_.Count) x $($_.Name)" }
