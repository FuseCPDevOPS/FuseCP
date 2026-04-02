#!/usr/bin/env pwsh
$ErrorActionPreference = 'SilentlyContinue'
Write-Host 'Fetching latest CodeQL alerts from GitHub...'

$page = 1
$alerts = @()

while ($page -le 15) {
    $query = "/repos/FuseCPDevOPS/FuseCP/code-scanning/alerts?state=open&per_page=100&page=$page"
    $result = gh api $query -H 'Accept: application/vnd.github+json' 2>$null | ConvertFrom-Json
    
    if (-not $result -or $result.Count -eq 0) { break }
    $alerts += $result
    if ($result.Count -lt 100) { break }
    $page++
}

Write-Host "Total alerts: $($alerts.Count)"
$alerts | ConvertTo-Json -Depth 3 | Set-Content codeql-open-alerts-fresh.json

Write-Host "`nTop 15 issue types:"
$alerts | Group-Object { $_.rule.id } | Sort-Object -Property Count -Descending | Select-Object -First 15 | ForEach-Object { Write-Host "$($_.Count) x $($_.Name)" }
