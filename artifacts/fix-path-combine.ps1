$ErrorActionPreference = 'Stop'

$j = Get-Content "artifacts/codeql-open-alerts.json" | ConvertFrom-Json
$srv = $j | Where-Object { $_.most_recent_instance.category -eq '/language:csharp-server' }
$pc = $srv | Where-Object { $_.rule.id -eq 'cs/path-combine' }
$repo = (Get-Location).Path

$byFile = @{}
foreach ($a in $pc) {
    $loc = $a.most_recent_instance.location
    if (-not $byFile.ContainsKey($loc.path)) { $byFile[$loc.path] = [System.Collections.Generic.List[int]]::new() }
    $byFile[$loc.path].Add([int]$loc.start_line)
}

Write-Output "FILES_WITH_ALERTS=$($byFile.Count)"
Write-Output "TOTAL_ALERTS=$($pc.Count)"

# Regex: Path.Combine(noParensArg, simpleIdentifierOrProperty)
# arg1 cannot contain parens to avoid matching outer call contexts
# arg2 must be a plain identifier/property chain - never a string literal
$pattern = [System.Text.RegularExpressions.Regex]::new(
    '(.*?Path\.Combine\([^()]+,\s*)([A-Za-z_][A-Za-z0-9_.]*)(\s*\))',
    [System.Text.RegularExpressions.RegexOptions]::None
)

$totalFixed = 0
$filesChanged = 0
$skippedAlready = 0
$noMatch = 0

foreach ($path in $byFile.Keys) {
    $abs = Join-Path $repo ($path -replace '/', '\')
    if (-not (Test-Path $abs)) { continue }

    $lines = [System.IO.File]::ReadAllLines($abs)
    $changed = $false

    foreach ($ln in ($byFile[$path] | Sort-Object -Unique)) {
        $i = $ln - 1
        if ($i -lt 0 -or $i -ge $lines.Count) { continue }
        $line = $lines[$i]

        # Skip if TrimStart already on this line
        if ($line -match 'TrimStart\(Path\.DirectorySeparatorChar') { $skippedAlready++; continue }

        $m = $pattern.Match($line)
        if ($m.Success) {
            $arg2 = $m.Groups[2].Value
            $trimExpr = "$arg2.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)"
            $replacement = $m.Groups[1].Value + $trimExpr + $m.Groups[3].Value
            $newLine = $line.Substring(0, $m.Index) + $replacement + $line.Substring($m.Index + $m.Length)
            if ($newLine -ne $line) {
                $lines[$i] = $newLine
                $changed = $true
                $totalFixed++
            }
        } else {
            $noMatch++
        }
    }

    if ($changed) {
        [System.IO.File]::WriteAllLines($abs, $lines, [System.Text.UTF8Encoding]::new($false))
        $filesChanged++
    }
}

Write-Output "PATH_COMBINE_FIXED=$totalFixed"
Write-Output "FILES_CHANGED=$filesChanged"
Write-Output "SKIPPED_ALREADY_FIXED=$skippedAlready"
Write-Output "NO_PATTERN_MATCH=$noMatch"
