#!/usr/bin/env pwsh
# fix-linq-missed-where-v2.ps1
# Handles the "entire body inside if" pattern:
#   foreach (T x in src) { if (cond) { body } }
# →  foreach (T x in src.Where(x => cond)) { body }
#
# Run refresh-alerts.ps1 first if you want up-to-date alert counts in the output.

$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path $PSScriptRoot

# Fetch open alerts from GitHub API
Write-Host "Fetching cs/linq/missed-where alerts from GitHub API..."
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
Write-Host "Total cs/linq/missed-where alerts: $($targets.Count)"

$locations = @()
foreach ($a in $targets) {
    $loc = $a.most_recent_instance.location
    if (-not $loc.path.EndsWith('.cs')) { continue }
    $locations += [pscustomobject]@{ Path = $loc.path; Line = [int]$loc.start_line }
}
$locations = $locations | Sort-Object Path, Line -Unique

# ── helpers ──────────────────────────────────────────────────────────────────

function Get-LoopBounds {
    param([string[]]$Lines, [int]$ForeachIdx)
    $openLine = -1
    if ($Lines[$ForeachIdx] -match '\{') { $openLine = $ForeachIdx }
    else {
        for ($k = $ForeachIdx + 1; $k -lt $Lines.Length; $k++) {
            $t = $Lines[$k].Trim()
            if ($t -eq '' -or $t.StartsWith('//')) { continue }
            if ($t -eq '{') { $openLine = $k }
            break
        }
    }
    if ($openLine -lt 0) { return $null }
    $depth = 0; $closeLine = -1
    for ($k = $openLine; $k -lt $Lines.Length; $k++) {
        $depth += ([regex]::Matches($Lines[$k], '\{')).Count
        $depth -= ([regex]::Matches($Lines[$k], '\}')).Count
        if ($depth -eq 0 -and $k -gt $openLine) { $closeLine = $k; break }
    }
    if ($closeLine -lt 0) { return $null }
    return [pscustomobject]@{ OpenLine = $openLine; CloseLine = $closeLine }
}

function Find-ForeachIndexNear {
    param($Lines, [int]$HintIndex)
    if ($HintIndex -ge 0 -and $HintIndex -lt $Lines.Count -and $Lines[$HintIndex] -match '^\s*foreach\s*\(') {
        return $HintIndex
    }
    for ($delta = 1; $delta -le 8; $delta++) {
        $up   = $HintIndex - $delta
        $down = $HintIndex + $delta
        if ($up -ge 0 -and $Lines[$up] -match '^\s*foreach\s*\(') { return $up }
        if ($down -lt $Lines.Count -and $Lines[$down] -match '^\s*foreach\s*\(') { return $down }
    }
    return -1
}

function HasAssignmentOperator([string]$cond) {
    return ($cond -match '(?<![!<>=])=(?!=)')
}

# ── main ─────────────────────────────────────────────────────────────────────

$totalTargets  = $locations.Count
$totalFixed    = 0
$filesChanged  = 0
$skippedReason = @{}

foreach ($group in ($locations | Group-Object Path)) {
    $abs = Join-Path $repoRoot ($group.Name -replace '/', '\\')
    if (-not (Test-Path $abs)) { continue }

    $lines   = [System.Collections.Generic.List[string]]([System.IO.File]::ReadAllLines($abs))
    $changed = $false

    foreach ($loc in ($group.Group | Sort-Object Line -Descending)) {
        $hint = $loc.Line - 1
        if ($hint -lt 0 -or $hint -ge $lines.Count) { continue }

        # ── 1. locate foreach ──────────────────────────────────────────────
        $i = Find-ForeachIndexNear -Lines $lines -HintIndex $hint
        if ($i -lt 0) { $skippedReason['no-foreach']++; continue }

        $foreachLine = $lines[$i]
        if ($foreachLine -notmatch '^\s*foreach\s*\(\s*(?<type>[^\s][^\)]*?)\s+(?<iter>[A-Za-z_][A-Za-z0-9_]*)\s+in\s+(?<src>.+)\)\s*$') {
            $skippedReason['foreach-parse']++; continue
        }
        $iterType = $Matches['type']
        $iter     = $Matches['iter']
        $src      = $Matches['src']
        $indentLen = $foreachLine.Length - $foreachLine.TrimStart().Length
        $indent    = $foreachLine.Substring(0, $indentLen)

        # Guard: don't double-apply
        if ($src -match '\.Where\s*\(') { $skippedReason['already-where']++; continue }

        # ── 2. loop bounds ─────────────────────────────────────────────────
        $bounds = Get-LoopBounds -Lines $lines.ToArray() -ForeachIdx $i
        if ($null -eq $bounds) { $skippedReason['no-bounds']++; continue }

        # ── 3. first non-trivial statement inside loop ─────────────────────
        $firstLine = -1
        for ($k = $bounds.OpenLine + 1; $k -lt $bounds.CloseLine; $k++) {
            $t = $lines[$k].Trim()
            if ($t -eq '' -or $t.StartsWith('//')) { continue }
            $firstLine = $k; break
        }
        if ($firstLine -lt 0) { $skippedReason['empty-body']++; continue }

        # Must start with 'if ('
        $ifTrimmed = $lines[$firstLine].TrimStart()
        if ($ifTrimmed -notmatch '^if\s*\(') { $skippedReason['no-if']++; continue }

        # ── 4. extract condition via paren-balancing ───────────────────────
        $ifStart = $ifTrimmed.IndexOf('(')
        $depth = 0; $condEnd = -1
        for ($k = $ifStart; $k -lt $ifTrimmed.Length; $k++) {
            if ($ifTrimmed[$k] -eq '(') { $depth++ }
            elseif ($ifTrimmed[$k] -eq ')') { $depth--; if ($depth -eq 0) { $condEnd = $k; break } }
        }
        if ($condEnd -lt 0) { $skippedReason['multiline-cond']++; continue }  # multi-line condition – skip safely

        $cond = $ifTrimmed.Substring($ifStart + 1, $condEnd - $ifStart - 1).Trim()

        # Condition must reference the iterator variable
        if ($cond -notmatch ("\b" + [regex]::Escape($iter) + "\b")) { $skippedReason['no-iter-ref']++; continue }
        # No assignment in condition
        if (HasAssignmentOperator $cond) { $skippedReason['assignment-in-cond']++; continue }

        # ── 5. locate the if-body open brace ──────────────────────────────
        $remainder  = $ifTrimmed.Substring($condEnd + 1).Trim()
        $ifOpenLine = -1
        if ($remainder -match '^\{') {
            $ifOpenLine = $firstLine
        } else {
            for ($k = $firstLine + 1; $k -le [Math]::Min($firstLine + 3, $bounds.CloseLine - 1); $k++) {
                $t2 = $lines[$k].Trim()
                if ($t2 -eq '' -or $t2.StartsWith('//')) { continue }
                if ($t2 -eq '{') { $ifOpenLine = $k }
                break
            }
        }
        if ($ifOpenLine -lt 0) {
            # Braceless if: find single-statement body
            $bodyIdx = -1
            for ($k = $firstLine + 1; $k -lt $bounds.CloseLine; $k++) {
                $bt = $lines[$k].Trim()
                if ($bt -eq '' -or $bt.StartsWith('//')) { continue }
                $bodyIdx = $k; break
            }
            if ($bodyIdx -lt 0) { $skippedReason['braceless-nobody']++; continue }
            # No else after body
            for ($k = $bodyIdx + 1; $k -lt $bounds.CloseLine; $k++) {
                $bt = $lines[$k].Trim()
                if ($bt -eq '' -or $bt.StartsWith('//')) { continue }
                if ($bt -match '^else\b') { $skippedReason['braceless-has-else']++; $bodyIdx = -1 }
                break
            }
            if ($bodyIdx -lt 0) { continue }
            # No extra code before the if
            $extraBefore = $false
            for ($k = $bounds.OpenLine + 1; $k -lt $firstLine; $k++) {
                $bt = $lines[$k].Trim()
                if ($bt -ne '' -and -not $bt.StartsWith('//')) { $extraBefore = $true; break }
            }
            if ($extraBefore) { $skippedReason['braceless-extra-before']++; continue }
            # No extra code after body (to loop close)
            $extraAfter = $false
            for ($k = $bodyIdx + 1; $k -lt $bounds.CloseLine; $k++) {
                $bt = $lines[$k].Trim()
                if ($bt -ne '' -and -not $bt.StartsWith('//')) { $extraAfter = $true; break }
            }
            if ($extraAfter) { $skippedReason['braceless-extra-after']++; continue }
            # De-indent the body by one level (remove leading 4 spaces or 1 tab)
            $ifIndent = $lines[$firstLine].Substring(0, $lines[$firstLine].Length - $lines[$firstLine].TrimStart().Length)
            $bodyLine = $lines[$bodyIdx]
            if ($bodyLine.StartsWith($ifIndent + "    ")) {
                $lines[$bodyIdx] = $ifIndent + $bodyLine.Substring($ifIndent.Length + 4)
            } elseif ($bodyLine.StartsWith($ifIndent + "`t")) {
                $lines[$bodyIdx] = $ifIndent + $bodyLine.Substring($ifIndent.Length + 1)
            }
            # Update foreach, remove the if line
            $newForeach = "${indent}foreach ($iterType $iter in $src.Where($iter => $cond))"
            $lines[$i] = $newForeach
            $lines.RemoveAt($firstLine)
            $changed = $true; $totalFixed++
            Write-Host "FIXED(braceless): $($group.Name.Split('/')[-1]):$($loc.Line) [$cond]"
            continue
        }

        # ── 6. find if-body close brace ────────────────────────────────────
        $ifDepth = 0; $ifCloseLine = -1
        for ($k = $ifOpenLine; $k -lt $lines.Count; $k++) {
            $ifDepth += ([regex]::Matches($lines[$k], '\{')).Count
            $ifDepth -= ([regex]::Matches($lines[$k], '\}')).Count
            if ($ifDepth -eq 0 -and $k -gt $ifOpenLine) { $ifCloseLine = $k; break }
        }
        if ($ifCloseLine -lt 0 -or $ifCloseLine -ge $bounds.CloseLine) { $skippedReason['no-if-close']++; continue }

        # ── 7. verify no other code before/after the if within the loop ────
        # No code after if-close and before loop-close
        $extraAfter = $false
        for ($k = $ifCloseLine + 1; $k -lt $bounds.CloseLine; $k++) {
            $t3 = $lines[$k].Trim()
            if ($t3 -ne '' -and -not $t3.StartsWith('//')) { $extraAfter = $true; break }
        }
        if ($extraAfter) { $skippedReason['extra-after-if']++; continue }

        # No meaningful code between loop-open and the if line
        $extraBefore = $false
        for ($k = $bounds.OpenLine + 1; $k -lt $firstLine; $k++) {
            $t4 = $lines[$k].Trim()
            if ($t4 -ne '' -and -not $t4.StartsWith('//')) { $extraBefore = $true; break }
        }
        if ($extraBefore) { $skippedReason['extra-before-if']++; continue }

        # ── 8. transform ───────────────────────────────────────────────────
        $newForeach = "${indent}foreach ($iterType $iter in $src.Where($iter => $cond))"
        $lines[$i] = $newForeach

        # Remove from bottom-up to preserve indices: ifCloseLine, then ifOpenLine or firstLine
        $lines.RemoveAt($ifCloseLine)

        if ($ifOpenLine -ne $firstLine) {
            # Brace was on a separate line from the if
            $lines.RemoveAt($ifOpenLine)
            $lines.RemoveAt($firstLine)
        } else {
            # Brace was on same line as the if header
            $lines.RemoveAt($firstLine)
        }

        $changed = $true
        $totalFixed++

        Write-Host "FIXED: $($group.Name.Split('/')[-1]):$($loc.Line) [$cond]"
    }

    if ($changed) {
        [System.IO.File]::WriteAllLines($abs, $lines, [System.Text.UTF8Encoding]::new($false))
        $filesChanged++
    }
}

Write-Output ""
Write-Output "MISSED_WHERE_V2_TARGETS=$totalTargets"
Write-Output "MISSED_WHERE_V2_FIXED=$totalFixed"
Write-Output "MISSED_WHERE_V2_FILES_CHANGED=$filesChanged"

if ($skippedReason.Count -gt 0) {
    Write-Output "Skipped reasons:"
    $skippedReason.GetEnumerator() | Sort-Object Value -Descending | ForEach-Object {
        Write-Output "  $($_.Key): $($_.Value)"
    }
}
