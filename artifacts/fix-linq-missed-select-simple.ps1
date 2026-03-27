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

$targets = $alerts | Where-Object { $_.rule.id -eq 'cs/linq/missed-select' }

$locations = @()
foreach ($a in $targets) {
    $loc = $a.most_recent_instance.location
    if (-not $loc.path.EndsWith('.cs')) { continue }
    $locations += [pscustomobject]@{
        Path = $loc.path
        Line = [int]$loc.start_line
    }
}

$locations = $locations | Sort-Object Path, Line -Unique

function Get-LoopBounds {
    param(
        [string[]]$Lines,
        [int]$ForeachLineIndex
    )

    $openLine = -1
    if ($Lines[$ForeachLineIndex] -match '\{') {
        $openLine = $ForeachLineIndex
    }
    else {
        for ($k = $ForeachLineIndex + 1; $k -lt $Lines.Length; $k++) {
            $t = $Lines[$k].Trim()
            if ($t -eq '') { continue }
            if ($t.StartsWith("//")) { continue }
            if ($t -eq '{') {
                $openLine = $k
            }
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
        if ($depth -eq 0 -and $k -gt $openLine) {
            $closeLine = $k
            break
        }
    }

    if ($closeLine -lt 0) { return $null }

    return [pscustomobject]@{
        OpenLine = $openLine
        CloseLine = $closeLine
    }
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
$filesTouched = @{}

foreach ($group in ($locations | Group-Object Path)) {
    $abs = Join-Path $repoRoot ($group.Name -replace '/', '\\')
    if (-not (Test-Path $abs)) { continue }

    $lines = [System.Collections.Generic.List[string]]([System.IO.File]::ReadAllLines($abs))
    $changed = $false

    $lineTargets = $group.Group | Sort-Object Line -Descending

    foreach ($loc in $lineTargets) {
        $hint = $loc.Line - 1
        if ($hint -lt 0 -or $hint -ge $lines.Count) { continue }

        $i = Find-ForeachIndexNear -Lines $lines -HintIndex $hint
        if ($i -lt 0) { continue }

        $foreachLine = $lines[$i]
        if ($foreachLine -notmatch '^\s*foreach\s*\(\s*(?<type>[^\s][^\)]*?)\s+(?<iter>[A-Za-z_][A-Za-z0-9_]*)\s+in\s+(?<src>.+)\)\s*$') {
            continue
        }

        $iter = $Matches['iter']
        $iterType = $Matches['type']
        $src = $Matches['src']
        $indentLen = $foreachLine.Length - $foreachLine.TrimStart().Length
        $indent = $foreachLine.Substring(0, $indentLen)

        $bounds = Get-LoopBounds -Lines $lines.ToArray() -ForeachLineIndex $i
        if ($null -eq $bounds) { continue }

        $stmtLine = -1
        for ($k = $bounds.OpenLine + 1; $k -lt $bounds.CloseLine; $k++) {
            $t = $lines[$k].Trim()
            if ($t -eq '') { continue }
            if ($t.StartsWith("//")) { continue }
            $stmtLine = $k
            break
        }
        if ($stmtLine -lt 0) { continue }

        $stmt = $lines[$stmtLine].Trim()
        if ($stmt -notmatch '^(?<declType>var|[A-Za-z_][A-Za-z0-9_<>\[\]\.,\?\s]*)\s+(?<alias>[A-Za-z_][A-Za-z0-9_]*)\s*=\s*(?<rhs>[A-Za-z_][A-Za-z0-9_]*)\s*;\s*$') {
            continue
        }

        $alias = $Matches['alias']
        $rhs = $Matches['rhs']
        if ($rhs -ne $iter) { continue }
        if ($alias -eq $iter) { continue }

        $iterUseInBody = $false
        for ($k = $stmtLine + 1; $k -lt $bounds.CloseLine; $k++) {
            if ($lines[$k] -match ("\\b" + [regex]::Escape($iter) + "\\b")) {
                $iterUseInBody = $true
                break
            }
        }
        if ($iterUseInBody) { continue }

        $newForeach = "${indent}foreach ($iterType $alias in $src)"
        if ($lines[$i] -eq $newForeach) { continue }

        $lines[$i] = $newForeach
        $lines.RemoveAt($stmtLine)

        $changed = $true
        $totalFixed++
    }

    if ($changed) {
        [System.IO.File]::WriteAllLines($abs, $lines, [System.Text.UTF8Encoding]::new($false))
        $filesChanged++
        $filesTouched[$group.Name] = $true
    }
}

Write-Output "MISSED_SELECT_TARGETS=$totalTargets"
Write-Output "MISSED_SELECT_FIXED=$totalFixed"
Write-Output "MISSED_SELECT_FILES_CHANGED=$filesChanged"
