# fix-js-asi-safe.ps1
# Fixes js/automatic-semicolon-insertion CodeQL alerts in FuseCP sources.
# Only modifies files under FuseCP/Sources/ — never touches tools/ submodule.

$root = Split-Path $PSScriptRoot -Parent
$fixed = 0

function Fix-Line {
    param([string]$filePath, [int]$lineNum, [string]$oldText, [string]$newText)
    if (-not (Test-Path $filePath)) { return }
    $lines = Get-Content $filePath
    if ($lineNum -lt 1 -or $lineNum -gt $lines.Count) { return }
    $line = $lines[$lineNum - 1]
    if ($line -eq $oldText) {
        $lines[$lineNum - 1] = $newText
        Set-Content -Path $filePath -Value $lines -NoNewline:$false
        $script:fixed++
        Write-Host "  Fixed line $lineNum in $filePath"
    } else {
        Write-Host "  SKIP (no match) line $lineNum in $filePath"
        Write-Host "    Expected: $($oldText | ForEach-Object { $_ -replace '\t','→' })"
        Write-Host "    Got:      $($line | ForEach-Object { $_ -replace '\t','→' })"
    }
}

# Helper: append semicolon to a line (replaces trailing semicolon-less statement)
function Fix-Append-Semi {
    param([string]$filePath, [int]$lineNum, [string]$expectedSubstring)
    if (-not (Test-Path $filePath)) { return }
    $lines = Get-Content $filePath
    if ($lineNum -lt 1 -or $lineNum -gt $lines.Count) { return }
    $line = $lines[$lineNum - 1]
    if ($line -match [regex]::Escape($expectedSubstring) -and $line -notmatch ';\s*$') {
        $lines[$lineNum - 1] = $line + ";"
        Set-Content -Path $filePath -Value $lines -NoNewline:$false
        $script:fixed++
        Write-Host "  Fixed (append ;) line $lineNum in $filePath"
    } elseif ($line -match ';\s*$') {
        Write-Host "  SKIP (already has ;) line $lineNum in $filePath"
    } else {
        Write-Host "  SKIP (no match) line $lineNum in $filePath"
        Write-Host "    Expected substring: $expectedSubstring"
        Write-Host "    Got: $line"
    }
}

Write-Host "`n=== js/automatic-semicolon-insertion fixes ==="

# --- jquery.window.js (JavaScript source) ---
# Line 68: closes $.fn.window = function(options) { ... }
# Expected: `}` with specific indentation
$jqw = "$root\FuseCP\Sources\FuseCP.WebPortal\JavaScript\jquery.window.js"
Fix-Line -filePath $jqw -lineNum 68 `
    -oldText "`t`t`t}" `
    -newText "`t`t`t};"

# Line 572: closes window.onbeforeunload = function() { ... }
Fix-Line -filePath $jqw -lineNum 572 `
    -oldText "`t`t`t}" `
    -newText "`t`t`t};"

# Line 1531: closes inner function in IIFE
Fix-Line -filePath $jqw -lineNum 1531 `
    -oldText "`t}" `
    -newText "`t};"

# --- dataTables.autoFill.js (WebDavPortal) ---
# Line 607: do { ... } while (...) missing semicolon
$dtAF = "$root\FuseCP\Sources\FuseCP.WebDavPortal\Scripts\DataTables\dataTables.autoFill.js"
Fix-Append-Semi -filePath $dtAF -lineNum 607 `
    -expectedSubstring "while ( currOffsetParent"

# --- fcp-maps.js (JavaScript source) ---
# Line 348: closes var data = { ... }  (no semicolon after })
$fcpMapsJs = "$root\FuseCP\Sources\FuseCP.WebPortal\JavaScript\fcp-maps.js"
Fix-Line -filePath $fcpMapsJs -lineNum 348 `
    -oldText "} // end data source" `
    -newText "}; // end data source"

# --- fcp-maps.js (App_Themes copy) ---
$fcpMapsTheme = "$root\FuseCP\Sources\FuseCP.WebPortal\App_Themes\Default\js\fcp-maps.js"
Fix-Line -filePath $fcpMapsTheme -lineNum 348 `
    -oldText "} // end data source" `
    -newText "}; // end data source"

# --- fcp-elements.js (JavaScript source) ---
# Line 166: closes var sliderChanged = function() { ... }
$fcpElemJs = "$root\FuseCP\Sources\FuseCP.WebPortal\JavaScript\fcp-elements.js"
Fix-Line -filePath $fcpElemJs -lineNum 166 `
    -oldText "}" `
    -newText "};"

# --- fcp-elements.js (App_Themes copy) ---
$fcpElemTheme = "$root\FuseCP\Sources\FuseCP.WebPortal\App_Themes\Default\js\fcp-elements.js"
Fix-Line -filePath $fcpElemTheme -lineNum 166 `
    -oldText "}" `
    -newText "};"

# --- fuelux.js ---
# Line 18: throw new Error( 'Fuel UX's JavaScript requires jQuery' )  -- missing ;
# Line 22: throw new Error( 'Fuel UX's JavaScript requires Bootstrap' ) -- missing ;
$fuelux = "$root\FuseCP\Sources\FuseCP.WebPortal\App_Themes\Default\addons\fuelux\js\fuelux.js"
Fix-Append-Semi -filePath $fuelux -lineNum 18 `
    -expectedSubstring "requires jQuery"
Fix-Append-Semi -filePath $fuelux -lineNum 22 `
    -expectedSubstring "requires Bootstrap"

# --- bootstrap.js (WebDavPortal) ---
# Line 8: throw new Error('Bootstrap's JavaScript requires jQuery') -- missing ;
$bootstrapJs = "$root\FuseCP\Sources\FuseCP.WebDavPortal\Scripts\bootstrap.js"
Fix-Append-Semi -filePath $bootstrapJs -lineNum 8 `
    -expectedSubstring "requires jQuery"

Write-Host "`nTotal ASI fixes applied: $fixed"
