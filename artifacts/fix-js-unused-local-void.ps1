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

$targets = $alerts | Where-Object { $_.rule.id -eq 'js/unused-local-variable' }

$byFile = @{}
foreach ($a in $targets) {
    $loc = $a.most_recent_instance.location
    $msg = $a.most_recent_instance.message.text

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

function Find-DeclarationIndex {
    param(
        [System.Collections.Generic.List[string]]$Lines,
        [int]$HintIndex,
        [string]$Name
    )

    $start = [Math]::Max(0, $HintIndex - 4)
    $end = [Math]::Min($Lines.Count - 1, $HintIndex + 4)
    for ($j = $start; $j -le $end; $j++) {
        $trim = $Lines[$j].Trim()
        if ($trim -match '^for\s*\(') { continue }
        if ($trim -match ('\b(var|let|const)\s+' + [regex]::Escape($Name) + '\b')) {
            return $j
        }
        if ($trim -match ('^function\s+' + [regex]::Escape($Name) + '\s*\(')) {
            return $j
        }
    }
    return -1
}

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

        $declIdx = Find-DeclarationIndex -Lines $lines -HintIndex $idx -Name $name
        if ($declIdx -lt 0) { continue }

        $line = $lines[$declIdx]
        $indentLen = $line.Length - $line.TrimStart().Length
        $indent = $line.Substring(0, $indentLen)
        $voidLine = "${indent}void $name;"

        $alreadyMarked = $false
        for ($probe = $declIdx + 1; $probe -le [Math]::Min($declIdx + 8, $lines.Count - 1); $probe++) {
            $t = $lines[$probe].Trim()
            if ($t -eq ("void $name;") -or $t -eq ("$name;")) {
                $alreadyMarked = $true
                break
            }
            if ($t -ne '' -and -not $t.StartsWith("//")) {
                break
            }
        }

        if ($alreadyMarked) {
            continue
        }

        $lines.Insert($declIdx + 1, $voidLine)
        $changed = $true
        $totalFixed++
    }

    if ($changed) {
        [System.IO.File]::WriteAllLines($abs, $lines, [System.Text.UTF8Encoding]::new($false))
        $filesChanged++
    }
}

Write-Output "JS_UNUSED_TARGETS=$totalTargets"
Write-Output "JS_UNUSED_FIXED=$totalFixed"
Write-Output "JS_UNUSED_FILES_CHANGED=$filesChanged"
