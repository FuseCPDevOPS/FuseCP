#!/usr/bin/env pwsh
# fix-nested-if-regex.ps1
# Regex-based scan: find all nested-if patterns (no alert location needed).
# Pattern:
#   if (OUTER_COND) {
#       if (INNER_COND) {        ← merged into:
#   if (OUTER_COND && INNER_COND) {
#
# Safety constraints:
# - Both ifs must have single-line conditions (no multi-line conds)
# - No else clause on either if
# - Outer block must contain ONLY the inner if (nothing else before or after)

param(
    [string[]]$Roots = @(
        "FuseCP\Sources\FuseCP.WebPortal",
        "FuseCP\Sources\FuseCP.WebDavPortal",
        "FuseCP\Sources\FuseCP.EnterpriseServer",
        "FuseCP\Sources\FuseCP.Providers.HostedSolution.Exchange2013",
        "FuseCP\Sources\FuseCP.Providers.HostedSolution.Exchange2016",
        "FuseCP\Sources\FuseCP.Providers.HostedSolution.Exchange2019",
        "FuseCP\Sources\FuseCP.Server"
    )
)

$repoRoot   = Split-Path $PSScriptRoot
$totalFixed = 0
$filesEdited = 0

function Get-IfCondition([string]$trimmedLine) {
    if ($trimmedLine -notmatch '^(?:else\s+)?if\s*\(') { return $null }
    $start = $trimmedLine.IndexOf('(')
    $depth = 0; $end = -1
    for ($k = $start; $k -lt $trimmedLine.Length; $k++) {
        if ($trimmedLine[$k] -eq '(') { $depth++ }
        elseif ($trimmedLine[$k] -eq ')') { $depth--; if ($depth -eq 0) { $end = $k; break } }
    }
    if ($end -lt 0) { return $null }
    return @{
        Cond  = $trimmedLine.Substring($start + 1, $end - $start - 1).Trim()
        After = $trimmedLine.Substring($end + 1).Trim()  # should be "{" or ""
        IsElseIf = $trimmedLine -match '^else\s+if'
    }
}

function Find-MatchingClose([System.Collections.Generic.List[string]]$lines, [int]$openIdx) {
    # $openIdx is the line with the opening {
    $depth = 0; $close = -1
    for ($i = $openIdx; $i -lt $lines.Count; $i++) {
        foreach ($c in $lines[$i].ToCharArray()) {
            if ($c -eq '{') { $depth++ } elseif ($c -eq '}') { $depth-- }
        }
        if ($depth -eq 0 -and $i -gt $openIdx) { $close = $i; break }
    }
    return $close
}

function Wrap-Cond([string]$cond) {
    # Wrap in parens if needed for safe && combination
    if ($cond -match '\|\|' -and $cond -notmatch '^\(.*\)$') { return "($cond)" }
    return $cond
}

foreach ($relRoot in $Roots) {
    $absRoot = Join-Path $repoRoot $relRoot
    if (-not (Test-Path $absRoot)) { continue }

    Get-ChildItem -Recurse -Filter "*.cs" $absRoot | Where-Object {
        $_.FullName -notlike "*\obj\*" -and $_.FullName -notlike "*\bin\*"
    } | ForEach-Object {
        $file    = $_.FullName
        $lines   = [System.Collections.Generic.List[string]]([System.IO.File]::ReadAllLines($file))
        $changed = $false
        $fileFixed = 0
        $i = $lines.Count - 2  # process bottom-up to preserve indices

        while ($i -ge 0) {
            $trimOuter = $lines[$i].TrimStart()
            $outerInfo = Get-IfCondition $trimOuter
            if ($null -eq $outerInfo -or $outerInfo.IsElseIf) { $i--; continue }

            $indent = $lines[$i].Substring(0, $lines[$i].Length - $trimOuter.Length)

            # Outer { must be on same line or the very next non-empty line
            $outerOpenIdx = -1
            if ($outerInfo.After -eq '{' -or $trimOuter -match '\)\s*\{') { $outerOpenIdx = $i }
            else {
                for ($k = $i + 1; $k -le [Math]::Min($i + 3, $lines.Count - 1); $k++) {
                    $t = $lines[$k].Trim()
                    if ($t -eq '' -or $t.StartsWith('//')) { continue }
                    if ($t -eq '{') { $outerOpenIdx = $k } ; break
                }
            }
            if ($outerOpenIdx -lt 0) { $i--; continue }

            $outerCloseIdx = Find-MatchingClose $lines $outerOpenIdx
            if ($outerCloseIdx -lt 0) { $i--; continue }

            # Between outerOpen and outerClose must be ONLY the inner if (no other code)
            $innerIfIdx = -1
            $nonEmptyCount = 0
            for ($k = $outerOpenIdx + 1; $k -lt $outerCloseIdx; $k++) {
                $t = $lines[$k].Trim()
                if ($t -ne '' -and -not $t.StartsWith('//')) {
                    $nonEmptyCount++
                    if ($t -match '^if\s*\(') { $innerIfIdx = $k }
                }
            }
            # Outer block must have ONLY the inner if (1 non-empty line first appearance)
            # Allow for inner open brace and inner close brace lines
            if ($innerIfIdx -lt 0) { $i--; continue }

            $trimInner = $lines[$innerIfIdx].TrimStart()
            $innerInfo = Get-IfCondition $trimInner
            if ($null -eq $innerInfo -or $innerInfo.IsElseIf) { $i--; continue }

            # Find inner { and inner close
            $innerOpenIdx = -1
            if ($innerInfo.After -eq '{' -or $trimInner -match '\)\s*\{') { $innerOpenIdx = $innerIfIdx }
            else {
                for ($k = $innerIfIdx + 1; $k -le [Math]::Min($innerIfIdx + 3, $lines.Count - 1); $k++) {
                    $t = $lines[$k].Trim()
                    if ($t -eq '' -or $t.StartsWith('//')) { continue }
                    if ($t -eq '{') { $innerOpenIdx = $k } ; break
                }
            }
            if ($innerOpenIdx -lt 0) { $i--; continue }

            $innerCloseIdx = Find-MatchingClose $lines $innerOpenIdx
            if ($innerCloseIdx -lt 0 -or $innerCloseIdx -ne $outerCloseIdx - 1) { $i--; continue }

            # Check no else after outerClose
            $nextNonEmpty = -1
            for ($k = $outerCloseIdx + 1; $k -lt [Math]::Min($lines.Count, $outerCloseIdx + 4); $k++) {
                if ($lines[$k].Trim() -ne '') { $nextNonEmpty = $k; break }
            }
            if ($nextNonEmpty -ge 0 -and $lines[$nextNonEmpty].TrimStart() -match '^else\b') { $i--; continue }

            # Check no else after innerClose
            $nextNonEmptyInner = -1
            for ($k = $innerCloseIdx + 1; $k -lt [Math]::Min($lines.Count, $innerCloseIdx + 4); $k++) {
                $t = $lines[$k].Trim()
                if ($t -ne '' -and -not $t.StartsWith('//')) { $nextNonEmptyInner = $k; break }
            }
            if ($nextNonEmptyInner -ge 0 -and $nextNonEmptyInner -lt $outerCloseIdx -and $lines[$nextNonEmptyInner].TrimStart() -match '^else\b') { $i--; continue }

            # Count actual non-empty non-comment lines in the outer block
            # Should only be: inner if line, inner open brace (maybe), inner close brace, outer close brace
            $expectedLinesBetween = ($outerCloseIdx - $outerOpenIdx - 1)  # lines between { }
            # Simple check: nonEmptyCount should be small (just the inner if + braces)
            if ($nonEmptyCount -gt 5) { $i--; continue }  # too complex, skip

            # Build merged condition
            $oc = Wrap-Cond $outerInfo.Cond
            $ic = Wrap-Cond $innerInfo.Cond
            $merged = "$oc && $ic"

            # Extract the inner block content (between innerOpen and innerClose)
            $innerBodyLines = [System.Collections.Generic.List[string]]::new()
            for ($k = $innerOpenIdx + 1; $k -lt $innerCloseIdx; $k++) {
                $innerBodyLines.Add($lines[$k])
            }

            # Build replacement: merged if + inner body
            $mergedIfLine = "${indent}if ($merged)"
            $openBrace    = "${indent}{"
            $closeBrace   = "${indent}}"

            # Remove from i to outerCloseIdx (inclusive)
            $removeCount = $outerCloseIdx - $i + 1
            $lines.RemoveRange($i, $removeCount)

            # Insert replacement
            $insertIdx = $i
            $lines.Insert($insertIdx, $closeBrace)
            foreach ($bl in ($innerBodyLines | Select-Object -Last ($innerBodyLines.Count) | Sort-Object { $innerBodyLines.IndexOf($_) } -Descending)) {
                $lines.Insert($insertIdx, $bl)
            }
            $lines.Insert($insertIdx, $openBrace)
            $lines.Insert($insertIdx, $mergedIfLine)

            $changed = $true; $fileFixed++
            # Don't decrement i — we replaced starting at i, so recheck from i
            continue
        }
        $i--  # end while

        if ($changed) {
            [System.IO.File]::WriteAllLines($file, $lines, [System.Text.UTF8Encoding]::new($false))
            Write-Host "  Fixed $fileFixed nested-if in: $($_.Name)"
            $script:totalFixed += $fileFixed
            $script:filesEdited++
        }
    }
}

Write-Host ""
Write-Host "Nested-if: merged $totalFixed patterns in $filesEdited files"
