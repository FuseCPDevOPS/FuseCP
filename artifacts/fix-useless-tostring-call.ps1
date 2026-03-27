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

$targets = $alerts | Where-Object { $_.rule.id -eq 'cs/useless-tostring-call' }

$byFile = @{}
foreach ($a in $targets) {
    $loc = $a.most_recent_instance.location
    if (-not $loc.path.EndsWith('.cs')) { continue }

    if (-not $byFile.ContainsKey($loc.path)) {
        $byFile[$loc.path] = [System.Collections.Generic.List[int]]::new()
    }
    $byFile[$loc.path].Add([int]$loc.start_line)
}

function ReplaceFirstToString([string]$line) {
    return [regex]::Replace($line, '\s*\.\s*ToString\s*\(\s*\)', '', 1)
}

$totalTargets = $targets.Count
$totalFixed = 0
$filesChanged = 0

foreach ($path in $byFile.Keys) {
    $abs = Join-Path $repoRoot ($path -replace '/', '\\')
    if (-not (Test-Path $abs)) { continue }

    $lines = [System.Collections.Generic.List[string]]([System.IO.File]::ReadAllLines($abs))
    $changed = $false

    foreach ($ln in ($byFile[$path] | Sort-Object -Descending -Unique)) {
        $idx = [int]$ln - 1
        if ($idx -lt 0 -or $idx -ge $lines.Count) { continue }

        $done = $false
        foreach ($probe in @($idx, ($idx - 1), ($idx + 1), ($idx - 2), ($idx + 2))) {
            if ($probe -lt 0 -or $probe -ge $lines.Count) { continue }
            $line = $lines[$probe]
            if ($line -notmatch '\.\s*ToString\s*\(\s*\)') { continue }

            $newLine = ReplaceFirstToString $line
            if ($newLine -ne $line) {
                $lines[$probe] = $newLine
                $changed = $true
                $totalFixed++
                $done = $true
                break
            }
        }

        if (-not $done) {
            continue
        }
    }

    if ($changed) {
        [System.IO.File]::WriteAllLines($abs, $lines, [System.Text.UTF8Encoding]::new($false))
        $filesChanged++
    }
}

Write-Output "USELESS_TOSTRING_TARGETS=$totalTargets"
Write-Output "USELESS_TOSTRING_FIXED=$totalFixed"
Write-Output "USELESS_TOSTRING_FILES_CHANGED=$filesChanged"
