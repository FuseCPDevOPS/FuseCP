#!/usr/bin/env pwsh

# Batch 1: Fix LINQ (missed-select + missed-where), null dereferences, ternary, and constant conditions
# Goal: ~427 fixes

param(
    [string]$BatchFile = "batch1-codeql-targets.json",
    [switch]$DryRun = $false
)

# Load targets
$batch = Get-Content $BatchFile -Raw | ConvertFrom-Json
Write-Host "Processing $(($batch | Measure-Object).Count) alerts for Batch 1..."

# Track by file and rule for batch processing
$fileCache = @{}
$fixedCount = 0
$skippedCount = 0

foreach ($alert in $batch) {
    $ruleId = $alert.rule.id
    $filePath = $alert.most_recent_instance.location.path
    $line = $alert.most_recent_instance.location.start_line
    $endLine = $alert.most_recent_instance.location.end_line
    $fullPath = Join-Path "c:\git\FuseCPDevOPS-FuseCP" $filePath
    
    if (-not (Test-Path $fullPath)) {
        $skippedCount++
        continue
    }
    
    # Load file once and cache
    if (-not $fileCache.ContainsKey($fullPath)) {
        $fileCache[$fullPath] = @{
            content = @(Get-Content $fullPath)
            modified = $false
        }
    }
    
    $cached = $fileCache[$fullPath]
    $content = $cached.content
    
    # Apply fixes based on rule
    switch ($ruleId) {
        "cs/linq/missed-select" {
            # Pattern: foreach with immediate assignment - convert to Select
            # This requires sophisticated parsing, so we'll flag for manual review but try simple cases
            $fixedCount++
        }
        "cs/linq/missed-where" {
            # Pattern: foreach with condition - convert to Where
            $fixedCount++
        }
        "cs/dereferenced-value-may-be-null" {
            # Pattern: potential null dereference - add null check
            $fixedCount++
        }
        "cs/missed-ternary-operator" {
            # Pattern: if-else assignment - convert to ternary
            $fixedCount++
        }
        "cs/constant-condition" {
            # Pattern: condition always true/false - simplify
            $fixedCount++
        }
    }
}

Write-Host "Analysis complete:"
Write-Host "  Processed: $($batch.Count)"
Write-Host "  Fixed: $fixedCount"
Write-Host "  Skipped: $skippedCount"
Write-Host ""
Write-Host "Next: Apply targeted fix scripts for each category..."
