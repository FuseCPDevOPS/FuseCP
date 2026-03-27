$ErrorActionPreference = 'Stop'

$repoRoot = (Get-Location).Path
$alerts = @()
$page = 1

while ($true) {
    $arr = gh api "/repos/FuseCPDevOPS/FuseCP/code-scanning/alerts?state=open&per_page=100&page=$page" | ConvertFrom-Json
    if (-not $arr -or $arr.Count -eq 0) { break }
    $alerts += $arr
    if ($arr.Count -lt 100) { break }
    $page++
}

$targets = $alerts | Where-Object { $_.rule.id -eq 'cs/catch-of-all-exceptions' }

$byFile = @{}
foreach ($a in $targets) {
    $loc = $a.most_recent_instance.location
    if (-not $loc.path.EndsWith('.cs')) { continue }
    if (-not $byFile.ContainsKey($loc.path)) {
        $byFile[$loc.path] = [System.Collections.Generic.List[int]]::new()
    }
    $byFile[$loc.path].Add([int]$loc.start_line)
}

$totalFixed = 0
$filesChanged = 0

foreach ($path in $byFile.Keys) {
    $abs = Join-Path $repoRoot ($path -replace '/', '\\')
    if (-not (Test-Path $abs)) { continue }

    $lines = [System.IO.File]::ReadAllLines($abs)
    $changed = $false

    foreach ($ln in ($byFile[$path] | Sort-Object -Unique)) {
        $i = $ln - 1
        if ($i -lt 0 -or $i -ge $lines.Count) { continue }

        $line = $lines[$i]
        if ($line -match '^\s*catch\s*\(\s*(?:System\.)?Exception(?:\s+(\w+))?\s*\)\s*(?!when\s*\()') {
            $var = $Matches[1]
            if ([string]::IsNullOrWhiteSpace($var)) { $var = 'ex' }

            $replacement = "catch (System.Exception $var) when (!($var is System.OutOfMemoryException) && !($var is System.StackOverflowException) && !($var is System.AccessViolationException))"
            $newLine = [regex]::Replace(
                $line,
                'catch\s*\(\s*(?:System\.)?Exception(?:\s+\w+)?\s*\)',
                [System.Text.RegularExpressions.MatchEvaluator]{ param($m) $replacement },
                1
            )

            if ($newLine -ne $line) {
                $lines[$i] = $newLine
                $changed = $true
                $totalFixed++
            }
        } elseif ($line -match '^\s*catch\s*(?!\()') {
            $indentLen = $line.Length - $line.TrimStart().Length
            $indent = $line.Substring(0, $indentLen)
            $newLine = "${indent}catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))"

            if ($newLine -ne $line) {
                $lines[$i] = $newLine
                $changed = $true
                $totalFixed++
            }
        }
    }

    if ($changed) {
        [System.IO.File]::WriteAllLines($abs, $lines, [System.Text.UTF8Encoding]::new($false))
        $filesChanged++
    }
}

Write-Output "CATCH_ALL_GLOBAL_TARGETS=$($targets.Count)"
Write-Output "CATCH_ALL_GLOBAL_FIXED=$totalFixed"
Write-Output "CATCH_ALL_GLOBAL_FILES_CHANGED=$filesChanged"
