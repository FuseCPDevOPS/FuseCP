# fix-js-superfluous-args-safe.ps1
# Fixes js/superfluous-trailing-arguments CodeQL alerts in FuseCP sources.
# Only modifies files under FuseCP/Sources/ — never touches tools/ submodule.

$root = Split-Path $PSScriptRoot -Parent
$fixed = 0

function Fix-Regex {
    param([string]$filePath, [string]$pattern, [string]$replacement, [string]$description)
    if (-not (Test-Path $filePath)) { Write-Host "  MISS: $filePath"; return }
    $content = Get-Content $filePath -Raw
    $newContent = [regex]::Replace($content, $pattern, $replacement)
    $count = ([regex]::Matches($content, $pattern)).Count
    if ($count -gt 0) {
        Set-Content -Path $filePath -Value $newContent -NoNewline
        $script:fixed += $count
        Write-Host "  Fixed $count occurrence(s) [$description] in $(Split-Path $filePath -Leaf)"
    } else {
        Write-Host "  SKIP (no match) [$description] in $(Split-Path $filePath -Leaf)"
    }
}

Write-Host "`n=== js/superfluous-trailing-arguments fixes ==="

# --- parseFloat(x, 10) → parseFloat(x) ---
# parseFloat ignores any second argument; only parseInt uses a radix
# fcp-common.js (minified-adjacent helper at line ~868)
$fcpCommon = "$root\FuseCP\Sources\FuseCP.WebPortal\JavaScript\fcp-common.js"
Fix-Regex -filePath $fcpCommon `
    -pattern 'parseFloat\(([^,)]+),\s*10\)' `
    -replacement 'parseFloat($1)' `
    -description "parseFloat(x,10) in fcp-common.js"

# fuelux.js (same pattern at line ~3050)
$fuelux = "$root\FuseCP\Sources\FuseCP.WebPortal\App_Themes\Default\addons\fuelux\js\fuelux.js"
Fix-Regex -filePath $fuelux `
    -pattern 'parseFloat\(([^,)]+),\s*10\)' `
    -replacement 'parseFloat($1)' `
    -description "parseFloat(x,10) in fuelux.js"

# --- tinymce hide(arg) → hide() ---
# Panel.js (source)
$panelSrc = "$root\FuseCP\Sources\FuseCP.WebPortal\tinymce\themes\inlite\src\main\js\ui\Panel.js"
Fix-Regex -filePath $panelSrc `
    -pattern '\.hide\([^)]+\)' `
    -replacement '.hide()' `
    -description ".hide(arg) in Panel.js"

# theme.raw.js (compiled scratch)
$themeRaw = "$root\FuseCP\Sources\FuseCP.WebPortal\tinymce\themes\inlite\scratch\inline\theme.raw.js"
Fix-Regex -filePath $themeRaw `
    -pattern '\.hide\(([a-zA-Z_$][a-zA-Z0-9_$]*)\)' `
    -replacement '.hide()' `
    -description ".hide(arg) in theme.raw.js"

# theme.js (inline compiled)
$themeInline = "$root\FuseCP\Sources\FuseCP.WebPortal\tinymce\themes\inlite\scratch\inline\theme.js"
Fix-Regex -filePath $themeInline `
    -pattern '\.hide\(([a-zA-Z_$][a-zA-Z0-9_$]*)\)' `
    -replacement '.hide()' `
    -description ".hide(arg) in theme.js (inline)"

# theme.js (compile scratch — different path)
$themeCompile = "$root\FuseCP\Sources\FuseCP.WebPortal\tinymce\themes\inlite\scratch\compile\theme.js"
Fix-Regex -filePath $themeCompile `
    -pattern '\.hide\(([a-zA-Z_$][a-zA-Z0-9_$]*)\)' `
    -replacement '.hide()' `
    -description ".hide(arg) in theme.js (compile)"

Write-Host "`nTotal superfluous-args fixes applied: $fixed"
