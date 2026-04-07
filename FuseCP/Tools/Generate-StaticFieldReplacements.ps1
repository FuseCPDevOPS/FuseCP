
# Generate precise replacements for all 69 cs/static-field-written-by-instance alerts
# This extracts field names from write locations and finds declarations to replace

param([switch]$ShowOnly = $false)

$wsRoot = "c:\git\FuseCPDevOPS-FuseCP"
Set-Location $wsRoot

$json = Get-Content 'artifacts/codeql-open-alerts-fresh.json' | ConvertFrom-Json  
$staticAlerts = @($json | Where-Object { $_.rule.id -eq 'cs/static-field-written-by-instance' })

Write-Host "Analyzing $($staticAlerts.Count) CodeQL static-field-written-by-instance alerts..." -ForegroundColor Green
Write-Host ""

# Build list of (filePath, targetFieldName, declarationLine) tuples
$targets = @()
$processed_files = @{}

foreach ($alert in $staticAlerts) {
    $filePath = $alert.most_recent_instance.location.path
    $writeLine = $alert.most_recent_instance.location.start_line
    
    $absPath = Join-Path $wsRoot $filePath
    if (-not (Test-Path $absPath)) { continue }
    
    # Read file content
    $content = Get-Content $absPath -Raw
    $lines = @(Get-Content $absPath)
    
    if ($writeLine -gt $lines.Count) { continue }
    
    $writeContent = $lines[$writeLine - 1]
    
    # Extract the field name being written (handles X=, X+=, X[i]=, etc.)
    $fieldMatch = $null
    if ($writeContent -match '(\w+)\s*(\[.*?\])?\s*(\+=?|=)') {
        $fieldMatch = $matches[1]
    }
    
    if (-not $fieldMatch) { continue }
    
    # Now find the static declaration of this field
    # Look for: [modifier] static [type] fieldName
    $declPattern = "(private|protected|public)?\s+static\s+\w+\s+$([System.Text.RegularExpressions.Regex]::Escape($fieldMatch))\s*="
    
    $matches = [System.Text.RegularExpressions.Regex]::Matches($content, $declPattern, 'IgnoreCase')
    
    if ($matches.Count -gt 0) {
        # Find the exact line with this declaration
        for ($i = 0; $i -lt $lines.Count; $i++) {
            if ($lines[$i] -match $declPattern) {
                $declLine = $lines[$i]
                $lineNum = $i + 1
                
                # Create the replacement
                $newDeclLine = $declLine -replace '(\s+)static(\s+)', '$1$2'
                
                if ($declLine -ne $newDeclLine) {
                    $targets += @{
                        FilePath = $filePath
                        FieldName = $fieldMatch
                        DeclLineNum = $lineNum
                        OldDecl = $declLine
                        NewDecl = $newDeclLine
                    }
                    break
                }
            }
        }
    }
}

Write-Host "Found $($targets.Count) field declarations to fix" -ForegroundColor Cyan
Write-Host ""

# Group by file for batch application
$byFile = $targets | Group-Object -Property FilePath

Write-Host "Files to modify: $($byFile.Count)" -ForegroundColor Green
Write-Host ""

if ($ShowOnly) {
    # Show first 15 replacements
    $targets | Select-Object -First 15 | ForEach-Object {
        Write-Host "$($_.FilePath):$($_.DeclLineNum) - $($_.FieldName)" -ForegroundColor Gray
        Write-Host "  -$($_.OldDecl)" -ForegroundColor Red
        Write-Host "  +$($_.NewDecl)" -ForegroundColor Green
        Write-Host ""
    }
    Write-Host "... and $($targets.Count - 15) more" -ForegroundColor Gray
    return
}

# Export for batch processing by multi_replace_string_in_file
$replacements = @()
foreach ($target in $targets) {
    $replacements += @{
        filePath = "$wsRoot/$($target.FilePath)"
        oldString = $target.OldDecl
        newString = $target.NewDecl
    }
}

# Save to JSON for reference
$replacements | ConvertTo-Json -Depth 5 | Out-File "artifacts/static-field-replacements-apply.json" -Encoding UTF8
Write-Host "Saved replacement list to artifacts/static-field-replacements-apply.json" -ForegroundColor Green

# Return the list for applying with multi_replace_string_in_file
$replacements
