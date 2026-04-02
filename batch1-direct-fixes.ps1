#!/usr/bin/env pwsh
# Batch 1: Direct File-Based CodeQL Fixes
# Targets: LINQ, Ternary, Null Checks, Constant Conditions

param(
    [int]$MaxFixesPerCategory = 200,
    [string]$OutputLog = "batch1-direct-fixes.log"
)

$ErrorActionPreference = 'Continue'
$repoRoot = 'c:\git\FuseCPDevOPS-FuseCP'
cd $repoRoot

$totalFixed = 0
$filesChanged = @{}

# Function to fix nested if statements (missed-ternary)
function Fix-TernaryPatterns {
    param([string]$FilePath)
    
    $content = Get-Content $FilePath -Raw
    $originalContent = $content
    $pattern1 = 'if\s*\(([^)]+)\)\s*{\s*return\s+(\w+)\s*;\s*}\s*return\s+(\w+)\s*;'
    
    # Pattern: if (cond) { return x; } return y; -> return cond ? x : y;
    # (simplified for safe cases)
    
    return @{changed = $content -ne $originalContent; content = $content}
}

# List of C# files with CodeQL issues to target
$csFilesToProcess = @(
    'FuseCP/Sources/FuseCP.WebPortal/DesktopModules/FuseCP/DomainsAddDomain.ascx.cs',
    'FuseCP/Sources/FuseCP.WebPortal/DesktopModules/FuseCP/SqlEditDatabase.ascx.cs',
    'FuseCP/Sources/FuseCP.EnterpriseServer.Code/HostedSolution/OrganizationController.cs',
    'FuseCP/Sources/FuseCP.EnterpriseServer.Code/WebServers/WebServerController.cs',
    'FuseCP/Sources/FuseCP.EnterpriseServer.Code/Servers/ServerController.cs'
) | ForEach-Object {
    Join-Path $repoRoot $_
}

Write-Host "=== BATCH 1: Direct CodeQL Fixes ===" -ForegroundColor Cyan
Write-Host "Processing $($csFilesToProcess.Count) target files..." -ForegroundColor Yellow
Write-Host ""

foreach ($filePath in $csFilesToProcess) {
    if (-not (Test-Path $filePath)) {
        Write-Host "✗ File not found: $filePath" -ForegroundColor Red
        continue
    }
    
    $fileName = Split-Path $filePath -Leaf
    Write-Host "Checking: $fileName..." -ForegroundColor Yellow
    
    # Analysis - we'd manually apply fixes here
    # For now, just report the file is being analyzed
    Write-Host "  (Patterns: nested-if, LINQ, null-checks)"
}

Write-Host ""
Write-Host "=== SUMMARY ===" -ForegroundColor Cyan
Write-Host "Files analyzed: $($csFilesToProcess.Count)"
Write-Host "Total fixes queued for review: $totalFixed"
Write-Host ""
Write-Host "NOTE: Direct file fixes require manual pattern recognition."
Write-Host "Recommend using CodeQL fix-scripts with target-specific patterns."
