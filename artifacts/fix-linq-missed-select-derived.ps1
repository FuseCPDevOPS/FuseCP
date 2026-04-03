#!/usr/bin/env pwsh
# fix-linq-missed-select-derived.ps1
# Converts patterns like:
# foreach (T iter in source) {
#     U mapped = <expr using iter>;
#     ... // does not use iter after this
# }
# into:
# foreach (U mapped in source.Select(iter => <expr using iter>)) {
#     ...
# }

$ErrorActionPreference = 'Stop'
$repoRoot = (Get-Location).Path

function Get-LoopBounds {
    param(
        [System.Collections.Generic.List[string]]$Lines,
        [int]$ForeachLineIndex
    )

    $openLine = -1
    if ($Lines[$ForeachLineIndex] -match '\{') {
        $openLine = $ForeachLineIndex
    }
    else {
        for ($k = $ForeachLineIndex + 1; $k -lt $Lines.Count; $k++) {
            $t = $Lines[$k].Trim()
            if ($t -eq '' -or $t.StartsWith('//')) { continue }
            if ($t -eq '{') { $openLine = $k }
            break
        }
    }

    if ($openLine -lt 0) { return $null }

    $depth = 0
    $closeLine = -1
    for ($k = $openLine; $k -lt $Lines.Count; $k++) {
        $line = $Lines[$k]
        $depth += ([regex]::Matches($line, '\{')).Count
        $depth -= ([regex]::Matches($line, '\}')).Count
        if ($depth -eq 0 -and $k -gt $openLine) {
            $closeLine = $k
            break
        }
    }

    if ($closeLine -lt 0) { return $null }
    return [pscustomobject]@{ OpenLine = $openLine; CloseLine = $closeLine }
}

$totalFixed = 0
$filesChanged = 0

$csFiles = Get-ChildItem -Recurse -Path (Join-Path $repoRoot 'FuseCP/Sources') -Include '*.cs' |
    Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' }

foreach ($file in $csFiles) {
    $lines = [System.Collections.Generic.List[string]]([System.IO.File]::ReadAllLines($file.FullName))
    $changed = $false
    $fileFixed = 0

    for ($i = $lines.Count - 1; $i -ge 0; $i--) {
        $foreachLine = $lines[$i]
        if ($foreachLine -notmatch '^\s*foreach\s*\(\s*(?<iterType>[^\s][^\)]*?)\s+(?<iter>[A-Za-z_][A-Za-z0-9_]*)\s+in\s+(?<src>.+)\)\s*$') {
            continue
        }

        $iterType = $Matches['iterType']
        $iter = $Matches['iter']
        $src = $Matches['src']
        $indentLen = $foreachLine.Length - $foreachLine.TrimStart().Length
        $indent = $foreachLine.Substring(0, $indentLen)

        $bounds = Get-LoopBounds -Lines $lines -ForeachLineIndex $i
        if ($null -eq $bounds) { continue }

        $firstLine = -1
        for ($k = $bounds.OpenLine + 1; $k -lt $bounds.CloseLine; $k++) {
            $t = $lines[$k].Trim()
            if ($t -eq '' -or $t.StartsWith('//')) { continue }
            $firstLine = $k
            break
        }
        if ($firstLine -lt 0) { continue }

        $stmt = $lines[$firstLine].Trim()
        if ($stmt -notmatch '^(?<declType>var|[A-Za-z_][A-Za-z0-9_<>,\[\]\.\?\s]*)\s+(?<alias>[A-Za-z_][A-Za-z0-9_]*)\s*=\s*(?<rhs>.+);\s*$') {
            continue
        }

        $declType = $Matches['declType'].Trim()
        $alias = $Matches['alias']
        $rhs = $Matches['rhs'].Trim()

        if ($alias -eq $iter) { continue }
        if ($rhs -notmatch ('\b' + [regex]::Escape($iter) + '\b')) { continue }
        if ($rhs -match ('^' + [regex]::Escape($iter) + '\s*$')) { continue }

        $iterUsedAfter = $false
        for ($k = $firstLine + 1; $k -lt $bounds.CloseLine; $k++) {
            if ($lines[$k] -match ('\b' + [regex]::Escape($iter) + '\b')) {
                $iterUsedAfter = $true
                break
            }
        }
        if ($iterUsedAfter) { continue }

        $newSrc = "$src.Select($iter => $rhs)"
        $newForeach = "${indent}foreach ($declType $alias in $newSrc)"

        if ($newForeach -eq $foreachLine) { continue }

        $lines[$i] = $newForeach
        $lines.RemoveAt($firstLine)

        $changed = $true
        $fileFixed++
        $totalFixed++
    }

    if ($changed) {
        [System.IO.File]::WriteAllLines($file.FullName, $lines, [System.Text.UTF8Encoding]::new($false))
        $filesChanged++
        Write-Host "FIXED $fileFixed in $($file.FullName.Replace($repoRoot + '\\', ''))"
    }
}

Write-Output "MISSED_SELECT_DERIVED_FIXED=$totalFixed"
Write-Output "MISSED_SELECT_DERIVED_FILES_CHANGED=$filesChanged"
