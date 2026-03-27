#!/usr/bin/env pwsh
# refresh-alerts.ps1 — Download all open CodeQL alerts from GitHub and save to codeql-open-alerts.json.
# Run this before any fix script that reads from codeql-open-alerts.json.

$ErrorActionPreference = 'Stop'
$outPath = Join-Path $PSScriptRoot "codeql-open-alerts.json"

Write-Host "Fetching open CodeQL alerts from GitHub API..."
$alerts = @()
$page = 1
while ($true) {
    Write-Host "  page $page..."
    $arr = gh api "/repos/FuseCPDevOPS/FuseCP/code-scanning/alerts?state=open&per_page=100&page=$page" | ConvertFrom-Json
    if (-not $arr -or $arr.Count -eq 0) { break }
    $alerts += $arr
    if ($arr.Count -lt 100) { break }
    $page++
}

Write-Host "Total open alerts fetched: $($alerts.Count)"

# Show distribution of top rules
$alerts | Group-Object { $_.rule.id } |
    Sort-Object Count -Descending |
    Select-Object -First 20 |
    ForEach-Object { Write-Host "  $($_.Count)  $($_.Name)" }

$alerts | ConvertTo-Json -Depth 10 | Set-Content $outPath -Encoding UTF8
Write-Host "Saved to $outPath"
