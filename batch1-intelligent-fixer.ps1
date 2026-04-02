#!/usr/bin/env pwsh
# Batch 1 Intelligent Fixer
# Uses CodeQL JSON data to apply targeted, safe transformations

param([int]$TargetFixes = 150)

$repoRoot = 'c:\git\FuseCPDevOPS-FuseCP'
cd $repoRoot

$issues = Get-Content artifacts/batch1-fixable-issues.json | ConvertFrom-Json
$allAlerts = Get-Content artifacts/codeql-open-alerts.json | ConvertFrom-Json

$fileGroups = $issues | Group-Object { $_.most_recent_instance.location.path }
$totalFixed = 0
$filesChanged = @()

Write-Host "=== BATCH 1: Intelligent CodeQL Fixes ===" -ForegroundColor Cyan
Write-Host "Target: $TargetFixes fixes across ~150-200 issues"
Write-Host ""

# Strategy: Fix the safest patterns first
# 1. Removed nested if with &&
# 2. Constant conditions simplifications
# 3. LINQ missed-where → .Where()

$safePatterns = @(
    @{
        Name = 'nested-if-combination'
        RuleId = 'cs/nested-if-statements'
        Priority = 1
    },
    @{
        Name = 'missed-ternary'
        RuleId = 'cs/missed-ternary-operator'
        Priority = 2
    },
    @{
        Name = 'constant-condition'
        RuleId = 'cs/constant-condition'
        Priority = 3
    },
    @{
        Name = 'missed-where-linq'
        RuleId = 'cs/linq/missed-where'
        Priority = 4
    }
)

Write-Host "Processing by safety priority:" -ForegroundColor Yellow
foreach ($pattern in $safePatterns | Sort-Object Priority) {
    $matching = $issues | Where-Object { $_.rule.id -eq $pattern.RuleId }
    if ($matching) {
        Write-Host "  $($pattern.Name): $($matching.Count) issues (priority $($pattern.Priority))"
    }
}

Write-Host ""
Write-Host "Top files to target:"
$fileGroups | Sort-Object -Property Count -Descending | Select-Object -First 8 | ForEach-Object {
    $file = Split-Path $_.Name -Leaf
    Write-Host "  $($_.Count) issues: $file"
}

Write-Host ""
Write-Host "Status: Ready for systematic fixing"
Write-Host "Next: Apply rule-specific transformations with validation"
