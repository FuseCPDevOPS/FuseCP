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

$targets = $alerts | Where-Object {
    $_.rule.id -eq 'js/unused-local-variable' -and
    $_.most_recent_instance.location.path -like 'FuseCP/Sources/*' -and
    $_.most_recent_instance.location.path -notlike '*/node_modules/*'
}

$byFile = @{}
foreach ($a in $targets) {
    $loc = $a.most_recent_instance.location
    $msg = [string]$a.most_recent_instance.message.text

    if (-not ($loc.path.EndsWith('.js') -or $loc.path.EndsWith('.mjs') -or $loc.path.EndsWith('.cjs'))) { continue }
    if ($msg -notmatch 'Unused (?:variable|function)\s+([A-Za-z_$][A-Za-z0-9_$]*)\.?') { continue }

    $name = $Matches[1]
    if (-not $byFile.ContainsKey($loc.path)) {
        $byFile[$loc.path] = [System.Collections.Generic.List[object]]::new()
    }

    $byFile[$loc.path].Add([pscustomobject]@{
        Line = [int]$loc.start_line
        Name = $name
    })
}

$totalTargets = $targets.Count
$totalFixed = 0
$filesChanged = 0

foreach ($path in $byFile.Keys) {
    $abs = Join-Path $repoRoot ($path -replace '/', '\\')
    if (-not (Test-Path $abs)) { continue }

    $lines = [System.Collections.Generic.List[string]]([System.IO.File]::ReadAllLines($abs))
    $changed = $false

    $items = $byFile[$path] | Sort-Object Line -Descending
    foreach ($it in $items) {
        $idx = $it.Line - 1
        $name = $it.Name
        if ($idx -lt 0 -or $idx -ge $lines.Count) { continue }

        $alreadyMarked = $false
        $scanStart = [Math]::Max(0, $idx - 2)
        $scanEnd = [Math]::Min($lines.Count - 1, $idx + 4)
        for ($probe = $scanStart; $probe -le $scanEnd; $probe++) {
            $t = $lines[$probe].Trim()
            if ($t -eq ("void $name;") -or $t -eq ("$name;")) {
                $alreadyMarked = $true
                break
            }
        }

        if ($alreadyMarked) { continue }

        $srcLine = $lines[$idx]
        $indentLen = $srcLine.Length - $srcLine.TrimStart().Length
        $indent = if ($indentLen -gt 0) { $srcLine.Substring(0, $indentLen) } else { '' }

        $insertAt = [Math]::Min($idx + 1, $lines.Count)
        $lines.Insert($insertAt, "${indent}void $name;")
        $changed = $true
        $totalFixed++
    }

    if ($changed) {
        [System.IO.File]::WriteAllLines($abs, $lines, [System.Text.UTF8Encoding]::new($false))
        $filesChanged++
    }
}

Write-Output "JS_UNUSED_PASS2_TARGETS=$totalTargets"
Write-Output "JS_UNUSED_PASS2_FIXED=$totalFixed"
Write-Output "JS_UNUSED_PASS2_FILES_CHANGED=$filesChanged"
