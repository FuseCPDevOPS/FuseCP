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

$targets = $alerts | Where-Object { $_.rule.id -eq 'cs/linq/missed-where' }
$locations = @()
foreach ($a in $targets) {
    $loc = $a.most_recent_instance.location
    if (-not $loc.path.EndsWith('.cs')) { continue }
    $locations += [pscustomobject]@{ Path = $loc.path; Line = [int]$loc.start_line }
}
$locations = $locations | Sort-Object Path, Line -Unique

function Get-LoopBounds {
    param([string[]]$Lines, [int]$ForeachLineIndex)

    $openLine = -1
    if ($Lines[$ForeachLineIndex] -match '\{') {
        $openLine = $ForeachLineIndex
    }
    else {
        for ($k = $ForeachLineIndex + 1; $k -lt $Lines.Length; $k++) {
            $t = $Lines[$k].Trim()
            if ($t -eq '' -or $t.StartsWith('//')) { continue }
            if ($t -eq '{') { $openLine = $k }
            break
        }
    }
    if ($openLine -lt 0) { return $null }

    $depth = 0
    $closeLine = -1
    for ($k = $openLine; $k -lt $Lines.Length; $k++) {
        $line = $Lines[$k]
        $depth += ([regex]::Matches($line, '\{')).Count
        $depth -= ([regex]::Matches($line, '\}')).Count
        if ($depth -eq 0 -and $k -gt $openLine) { $closeLine = $k; break }
    }

    if ($closeLine -lt 0) { return $null }
    return [pscustomobject]@{ OpenLine = $openLine; CloseLine = $closeLine }
}

function HasAssignmentOperator([string]$cond) {
    # reject assignment-like conditions; allow ==, !=, <=, >=
    return ($cond -match '(?<![!<>=])=(?!=)')
}

function Find-ForeachIndexNear {
    param(
        [System.Collections.Generic.List[string]]$Lines,
        [int]$HintIndex
    )

    if ($HintIndex -ge 0 -and $HintIndex -lt $Lines.Count -and $Lines[$HintIndex] -match '^\s*foreach\s*\(') {
        return $HintIndex
    }

    for ($delta = 1; $delta -le 8; $delta++) {
        $up = $HintIndex - $delta
        if ($up -ge 0 -and $Lines[$up] -match '^\s*foreach\s*\(') {
            return $up
        }

        $down = $HintIndex + $delta
        if ($down -lt $Lines.Count -and $Lines[$down] -match '^\s*foreach\s*\(') {
            return $down
        }
    }

    return -1
}

$totalTargets = $locations.Count
$totalFixed = 0
$filesChanged = 0

foreach ($group in ($locations | Group-Object Path)) {
    $abs = Join-Path $repoRoot ($group.Name -replace '/', '\\')
    if (-not (Test-Path $abs)) { continue }

    $lines = [System.Collections.Generic.List[string]]([System.IO.File]::ReadAllLines($abs))
    $changed = $false

    foreach ($loc in ($group.Group | Sort-Object Line -Descending)) {
        $hint = $loc.Line - 1
        if ($hint -lt 0 -or $hint -ge $lines.Count) { continue }

        $i = Find-ForeachIndexNear -Lines $lines -HintIndex $hint
        if ($i -lt 0) { continue }

        $foreachLine = $lines[$i]
        if ($foreachLine -notmatch '^\s*foreach\s*\(\s*(?<type>[^\s][^\)]*?)\s+(?<iter>[A-Za-z_][A-Za-z0-9_]*)\s+in\s+(?<src>.+)\)\s*$') {
            continue
        }

        $iterType = $Matches['type']
        $iter = $Matches['iter']
        $src = $Matches['src']
        $indentLen = $foreachLine.Length - $foreachLine.TrimStart().Length
        $indent = $foreachLine.Substring(0, $indentLen)

        $bounds = Get-LoopBounds -Lines $lines.ToArray() -ForeachLineIndex $i
        if ($null -eq $bounds) { continue }

        $firstLine = -1
        for ($k = $bounds.OpenLine + 1; $k -lt $bounds.CloseLine; $k++) {
            $t = $lines[$k].Trim()
            if ($t -eq '' -or $t.StartsWith('//')) { continue }
            $firstLine = $k
            break
        }
        if ($firstLine -lt 0) { continue }

        $cond = $null
        $removeStart = -1
        $removeCount = 0

        $stmt = $lines[$firstLine].Trim()

        if ($stmt -match '^if\s*\((?<cond>.+)\)\s*continue;\s*$') {
            $cond = $Matches['cond']
            $removeStart = $firstLine
            $removeCount = 1
        }
        elseif ($stmt -match '^if\s*\((?<cond>.+)\)\s*$') {
            $next = if (($firstLine + 1) -lt $lines.Count) { $lines[$firstLine + 1].Trim() } else { '' }
            if ($next -match '^continue;\s*$') {
                $cond = $Matches['cond']
                $removeStart = $firstLine
                $removeCount = 2
            }
            elseif ($next -match '^\{\s*$' -and ($firstLine + 2) -lt $lines.Count -and $lines[$firstLine + 2].Trim() -match '^continue;\s*$' -and ($firstLine + 3) -lt $lines.Count -and $lines[$firstLine + 3].Trim() -match '^\}\s*$') {
                $cond = $Matches['cond']
                $removeStart = $firstLine
                $removeCount = 4
            }
        }

        if ([string]::IsNullOrWhiteSpace($cond)) { continue }
        if ($cond -notmatch ("\\b" + [regex]::Escape($iter) + "\\b")) { continue }
        if (HasAssignmentOperator $cond) { continue }

        $newSrc = "$src.Where($iter => !($cond))"
        $newForeach = "${indent}foreach ($iterType $iter in $newSrc)"

        $lines[$i] = $newForeach
        for ($r = 0; $r -lt $removeCount; $r++) {
            $lines.RemoveAt($removeStart)
        }

        $changed = $true
        $totalFixed++
    }

    if ($changed) {
        [System.IO.File]::WriteAllLines($abs, $lines, [System.Text.UTF8Encoding]::new($false))
        $filesChanged++
    }
}

Write-Output "MISSED_WHERE_TARGETS=$totalTargets"
Write-Output "MISSED_WHERE_FIXED=$totalFixed"
Write-Output "MISSED_WHERE_FILES_CHANGED=$filesChanged"
