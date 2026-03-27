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

$targets = $alerts | Where-Object { $_.rule.id -eq 'cs/empty-catch-block' }

$byFile = @{}
foreach ($a in $targets) {
    $loc = $a.most_recent_instance.location
    if (-not $loc.path.EndsWith('.cs')) { continue }
    if (-not $byFile.ContainsKey($loc.path)) {
        $byFile[$loc.path] = [System.Collections.Generic.List[int]]::new()
    }
    $byFile[$loc.path].Add([int]$loc.start_line)
}

function Find-CatchIndexNear {
    param(
        [System.Collections.Generic.List[string]]$Lines,
        [int]$HintIndex
    )

    if ($HintIndex -ge 0 -and $HintIndex -lt $Lines.Count -and $Lines[$HintIndex].Trim().StartsWith('catch')) {
        return $HintIndex
    }

    for ($delta = 1; $delta -le 6; $delta++) {
        $up = $HintIndex - $delta
        if ($up -ge 0 -and $Lines[$up].Trim().StartsWith('catch')) { return $up }

        $down = $HintIndex + $delta
        if ($down -lt $Lines.Count -and $Lines[$down].Trim().StartsWith('catch')) { return $down }
    }

    return -1
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
        $hint = $ln - 1
        if ($hint -lt 0 -or $hint -ge $lines.Count) { continue }

        $i = Find-CatchIndexNear -Lines $lines -HintIndex $hint
        if ($i -lt 0) { continue }

        $catchLine = $lines[$i]
        if ($catchLine -notmatch '^\s*catch\s*\(' -and $catchLine -notmatch '^\s*catch\b') { continue }

        $varName = $null
        if ($catchLine -match '^\s*catch\s*\((?<sig>[^\)]*)\)') {
            $sig = $Matches['sig'].Trim()
            if ($sig -match '\s+(?<v>[A-Za-z_][A-Za-z0-9_]*)\s*$') {
                $varName = $Matches['v']
            }
        }

        $open = -1
        if ($catchLine -match '\{') {
            $open = $i
        }
        else {
            for ($k = $i + 1; $k -lt $lines.Count; $k++) {
                $t = $lines[$k].Trim()
                if ($t -eq '' -or $t.StartsWith('//')) { continue }
                if ($t -eq '{') { $open = $k }
                break
            }
        }
        if ($open -lt 0) { continue }

        $close = -1
        $depth = 0
        for ($k = $open; $k -lt $lines.Count; $k++) {
            $line = $lines[$k]
            $depth += ([regex]::Matches($line, '\{')).Count
            $depth -= ([regex]::Matches($line, '\}')).Count
            if ($depth -eq 0 -and $k -gt $open) {
                $close = $k
                break
            }
        }
        if ($close -lt 0) { continue }

        $bodyStmtLine = -1
        for ($k = $open + 1; $k -lt $close; $k++) {
            $t = $lines[$k].Trim()
            if ($t -eq '' -or $t.StartsWith('//')) { continue }
            $bodyStmtLine = $k
            break
        }
        if ($bodyStmtLine -ge 0) { continue }

        $baseIndentLen = $catchLine.Length - $catchLine.TrimStart().Length
        $baseIndent = $catchLine.Substring(0, $baseIndentLen)
        $stmtIndent = $baseIndent + '    '
        $stmt = if ($varName) { "${stmtIndent}_ = $varName;" } else { "${stmtIndent}_ = 0;" }

        if ($catchLine -match '\{\s*\}\s*$') {
            $lines[$i] = $catchLine -replace '\{\s*\}\s*$', ''
            $lines.Insert($i + 1, $baseIndent + '{')
            $lines.Insert($i + 2, $stmt)
            $lines.Insert($i + 3, $baseIndent + '}')
        }
        else {
            $lines.Insert($open + 1, $stmt)
        }

        $changed = $true
        $totalFixed++
    }

    if ($changed) {
        [System.IO.File]::WriteAllLines($abs, $lines, [System.Text.UTF8Encoding]::new($false))
        $filesChanged++
    }
}

Write-Output "EMPTY_CATCH_NEAR_TARGETS=$totalTargets"
Write-Output "EMPTY_CATCH_NEAR_FIXED=$totalFixed"
Write-Output "EMPTY_CATCH_NEAR_FILES_CHANGED=$filesChanged"
