#!/usr/bin/env pwsh
# Batch 1 Fix Runner: LINQ, Ternary, Constant Conditions, Null Dereferences
# Target: ~500 fixes across multiple categories

$ErrorActionPreference = 'Continue'
$repoRoot = 'c:\git\FuseCPDevOPS-FuseCP'
cd $repoRoot

Write-Host "=== BATCH 1: CodeQL Fixes ===" -ForegroundColor Cyan
Write-Host "Target categories: LINQ (Select/Where), Missed Ternary, Constant Conditions"
Write-Host "Estimated fixes: ~500"
Write-Host ""

$scripts = @(
    @{Name = 'LINQ Miss-Select (Simple)'; Script = './artifacts/fix-linq-missed-select-simple.ps1'},
    @{Name = 'LINQ Missed-Select (Map/Filter)'; Script = './artifacts/fix-linq-missed-select-map-filter.ps1'},
    @{Name = 'LINQ Missed-Where (Simple)'; Script = './artifacts/fix-linq-missed-where-simple.ps1'},
    @{Name = 'LINQ Missed-Where (v2)'; Script = './artifacts/fix-linq-missed-where-v2.ps1'},
    @{Name = 'Missed Ternary Operator'; Script = './artifacts/fix-missed-ternary.ps1'},
    @{Name = 'Constant Condition (Safe)'; Script = './artifacts/fix-constant-condition-safe.ps1'}
)

$totalFixed = 0
$results = @()

foreach ($item in $scripts) {
    Write-Host "Running: $($item.Name)..." -ForegroundColor Yellow
    
    if (Test-Path $item.Script) {
        $output = & pwsh $item.Script 2>&1
        $results += @{
            Name = $item.Name
            Output = $output
        }
        
        # Extract fixes count if available
        $fixLine = $output | Select-String '_FIXED=(\d+)' 
        if ($fixLine) {
            $match = $fixLine | Select-Object -First 1
            if ($match -match '_FIXED=(\d+)') {
                $fixed = [int]$Matches[1]
                $totalFixed += $fixed
                Write-Host "  ✓ Fixed: $fixed" -ForegroundColor Green
            }
        }
    } else {
        Write-Host "  ✗ Script not found: $($item.Script)" -ForegroundColor Red
    }
    Write-Host ""
}

Write-Host "=== BATCH 1 SUMMARY ===" -ForegroundColor Cyan
Write-Host "Total Fixed: $totalFixed"
Write-Host ""
Write-Host "Next steps:"
Write-Host "  1. Review changes with: git status"
Write-Host "  2. Build with: build-debug.bat"
Write-Host "  3. Validate with: run-local-validation.ps1"
Write-Host ""

# Save summary
$results | ConvertTo-Json -Depth 3 | Set-Content artifacts/batch1-results.json
Write-Host "Results saved to: artifacts/batch1-results.json"
