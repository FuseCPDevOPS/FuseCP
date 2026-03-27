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

function Get-FirstStatements {
    param([System.Collections.Generic.List[string]]$Lines, [int]$StartLine, [int]$EndLine)

    $stmts = @()
    for ($k = $StartLine; $k -lt $EndLine; $k++) {
        $t = $Lines[$k].Trim()
        if ($t -eq '' -or $t.StartsWith('//')) { continue }
        $stmts += [pscustomobject]@{ Line = $k; Text = $t }
        if ($stmts.Count -ge 4) { break }
    }
    return $stmts
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
        $i = $loc.Line - 1
        if ($i -lt 0 -or $i -ge $lines.Count) { continue }

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

        $stmts = Get-FirstStatements -Lines $lines -StartLine ($bounds.OpenLine + 1) -EndLine $bounds.CloseLine
        if ($stmts.Count -lt 2) { continue }

        $mapStmt = $stmts[0]
        $ifStmt = $stmts[1]

        if ($mapStmt.Text -notmatch '^(?<declType>var|[A-Za-z_][A-Za-z0-9_<>\[\]\.,\?\s]*)\s+(?<mapped>[A-Za-z_][A-Za-z0-9_]*)\s*=\s*(?<expr>.+)\s*;\s*$') {
            continue
        }

        $mapped = $Matches['mapped']
        $expr = $Matches['expr']
        if ($expr -notmatch ("\\b" + [regex]::Escape($iter) + "\\b")) { continue }

        $ifMatches = $false
        if ($ifStmt.Text -match ('^if\s*\(\s*' + [regex]::Escape($mapped) + '\s*==\s*null\s*\)\s*continue;\s*$')) {
            $ifMatches = $true
        }
        elseif ($ifStmt.Text -match ('^if\s*\(\s*null\s*==\s*' + [regex]::Escape($mapped) + '\s*\)\s*continue;\s*$')) {
            $ifMatches = $true
        }
        elseif ($ifStmt.Text -match ('^if\s*\(\s*String\.IsNullOrEmpty\(\s*' + [regex]::Escape($mapped) + '\s*\)\s*\)\s*continue;\s*$')) {
            $ifMatches = $true
        }
        elseif ($ifStmt.Text -match ('^if\s*\(\s*string\.IsNullOrEmpty\(\s*' + [regex]::Escape($mapped) + '\s*\)\s*\)\s*continue;\s*$')) {
            $ifMatches = $true
        }

        if (-not $ifMatches) { continue }

        $iterUsedAfter = $false
        for ($k = $ifStmt.Line + 1; $k -lt $bounds.CloseLine; $k++) {
            if ($lines[$k] -match ("\\b" + [regex]::Escape($iter) + "\\b")) {
                $iterUsedAfter = $true
                break
            }
        }
        if ($iterUsedAfter) { continue }

        $wherePredicate = "$mapped != null"
        if ($ifStmt.Text -match 'IsNullOrEmpty') {
            $wherePredicate = "!string.IsNullOrEmpty($mapped)"
        }

        $newSrc = "$src.Select($iter => $expr).Where($mapped => $wherePredicate)"
        $newForeach = "$indentforeach ($iterType $mapped in $newSrc)"

        $lines[$i] = $newForeach
        $lines.RemoveAt($ifStmt.Line)
        $lines.RemoveAt($mapStmt.Line)

        $changed = $true
        $totalFixed++
    }

    if ($changed) {
        [System.IO.File]::WriteAllLines($abs, $lines, [System.Text.UTF8Encoding]::new($false))
        $filesChanged++
    }
}

Write-Output "MISSED_SELECT_MAP_FILTER_TARGETS=$totalTargets"
Write-Output "MISSED_SELECT_MAP_FILTER_FIXED=$totalFixed"
Write-Output "MISSED_SELECT_MAP_FILTER_FILES_CHANGED=$filesChanged"
