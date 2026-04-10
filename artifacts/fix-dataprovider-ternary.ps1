#!/usr/bin/env pwsh
# fix-dataprovider-ternary.ps1
# Converts cs/missed-ternary-operator patterns in DataProvider.cs:
#
#   if (!recursive) VAR = SRC.Where(EXPR);
#   else
#   {
#       VAR = SRC.Join(childPackages, EXPR, ch => ch, (ITEM, ch) => ITEM);
#   }
# →
#   VAR = !recursive
#       ? SRC.Where(EXPR)
#       : SRC.Join(childPackages, EXPR, ch => ch, (ITEM, ch) => ITEM);

$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path $PSScriptRoot
$filePath = Join-Path $repoRoot 'FuseCP\Sources\FuseCP.EnterpriseServer.Code\Data\DataProvider.cs'

if (-not (Test-Path $filePath)) { throw "File not found: $filePath" }
$lines = [System.Collections.Generic.List[string]]([System.IO.File]::ReadAllLines($filePath))

$totalFixed = 0

# Scan in reverse order so line-index changes don't affect earlier indices
for ($i = $lines.Count - 1; $i -ge 0; $i--) {
    $line = $lines[$i]

    # Match: INDENT if (!recursive) VAR = EXPR;
    if ($line -notmatch '^(?<indent>\s+)if \(!recursive\) (?<var>[A-Za-z_][A-Za-z0-9_]*) = (?<whereExpr>.+);$') { continue }
    $indent    = $Matches['indent']
    $varName   = $Matches['var']
    $whereExpr = $Matches['whereExpr']

    # Look ahead: expect "else", optional blank, "{", "VAR = EXPR;", "}"
    $j = $i + 1
    while ($j -lt $lines.Count -and $lines[$j].Trim() -eq '') { $j++ }
    if ($j -ge $lines.Count -or $lines[$j].Trim() -ne 'else') { continue }

    $k = $j + 1
    while ($k -lt $lines.Count -and $lines[$k].Trim() -eq '') { $k++ }
    if ($k -ge $lines.Count -or $lines[$k].Trim() -ne '{') { continue }

    $l = $k + 1
    while ($l -lt $lines.Count -and $lines[$l].Trim() -eq '') { $l++ }
    if ($l -ge $lines.Count) { continue }
    $assignLine = $lines[$l]
    if ($assignLine -notmatch "^\s+$([regex]::Escape($varName)) = (?<joinExpr>.+);$") { continue }
    $joinExpr = $Matches['joinExpr']

    $m = $l + 1
    while ($m -lt $lines.Count -and $lines[$m].Trim() -eq '') { $m++ }
    if ($m -ge $lines.Count -or $lines[$m].Trim() -ne '}') { continue }

    # Build replacement: ternary assignment spanning 3 lines
    $innerIndent = $indent + "`t"
    $newLines = @(
        "$indent$varName = !recursive",
        "${innerIndent}? $whereExpr",
        "${innerIndent}: $joinExpr;"
    )

    # Remove lines i..m and insert replacement
    $removeCount = $m - $i + 1
    $lines.RemoveRange($i, $removeCount)
    $lines.InsertRange($i, [string[]]$newLines)

    $totalFixed++
    Write-Host "FIXED L$($i+1): $varName = !recursive ? ... : ..."
}

if ($totalFixed -gt 0) {
    [System.IO.File]::WriteAllLines($filePath, $lines, [System.Text.UTF8Encoding]::new($false))
    Write-Host "Saved DataProvider.cs with $totalFixed ternary conversion(s)."
} else {
    Write-Host "No patterns matched."
}
Write-Host "DATAPROVIDER_TERNARY_FIXED=$totalFixed"
