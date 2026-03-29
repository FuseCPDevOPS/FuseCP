#!/usr/bin/env pwsh
# fix-ternary-regex.ps1
# cs/missed-ternary-operator: convert simple 2-line if/else assign/return to ternary.
#
# Patterns handled (braceless and braced single-statement forms):
#
# A)  if (cond)
#         var = expr1;
#     else
#         var = expr2;
# →       var = cond ? expr1 : expr2;
#
# B)  if (cond)
#         return expr1;
#     else
#         return expr2;
# →       return cond ? expr1 : expr2;
#
# C)  if (cond) { var = expr1; }
#     else { var = expr2; }   (single-statement braced)
#
# Safety: only handles simple single-line conditions, single-statement bodies.

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

function Get-BalancedCondition([string]$trimmedLine) {
    # Extract condition from if (...)  returning (cond, after_paren_text)
    if ($trimmedLine -notmatch '^if\s*\(') { return $null }
    $start = $trimmedLine.IndexOf('(')
    $depth = 0; $end = -1
    for ($k = $start; $k -lt $trimmedLine.Length; $k++) {
        if ($trimmedLine[$k] -eq '(') { $depth++ }
        elseif ($trimmedLine[$k] -eq ')') { $depth--; if ($depth -eq 0) { $end = $k; break } }
    }
    if ($end -lt 0) { return $null }
    return @{ Cond = $trimmedLine.Substring($start+1, $end-$start-1).Trim()
              Rest = $trimmedLine.Substring($end+1).Trim() }
}

function Extract-Statement([string]$stmtLine) {
    # Returns the statement text without trailing semicolon if present, plus the verb
    $t = $stmtLine.Trim()
    if ($t -match '^return\s+(.+);?\s*$') { return @{ Verb='return'; Expr=$Matches[1].TrimEnd(';',' ') } }
    # assignment: optionally prefixed with type+var (decl) or just var
    if ($t -match '^(?:(?:var|[A-Za-z_][A-Za-z0-9_<>,\.\[\]\?\s]*)\s+)?(\w[\w.]*(?:\[[^\]]*\])?)\s*=\s*(.+);?\s*$') {
        return @{ Verb='assign'; LHS=$Matches[1]; Expr=$Matches[2].TrimEnd(';',' ') }
    }
    return $null
}

function Next-NonEmpty([System.Collections.Generic.List[string]]$lines, [int]$from) {
    for ($k = $from; $k -lt $lines.Count; $k++) {
        if ($lines[$k].Trim() -ne '') { return $k }
    }
    return -1
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
        $i = $lines.Count - 1

        while ($i -ge 0) {
            $t = $lines[$i].TrimStart()
            if ($t -notmatch '^if\s*\(') { $i--; continue }

            $ifInfo = Get-BalancedCondition $t
            if ($null -eq $ifInfo) { $i--; continue }

            $cond   = $ifInfo.Cond
            $rest   = $ifInfo.Rest  # either "" "{" or "stmt;"
            $indent = $lines[$i].Substring(0, $lines[$i].Length - $t.Length)

            # Determine the then-body
            $thenStmt = $null
            $thenEnd  = -1  # last line of then-block

            if ($rest -eq '' -or $rest -eq $null) {
                # braceless: body is next non-empty line
                $thenIdx = Next-NonEmpty $lines ($i + 1)
                if ($thenIdx -lt 0) { $i--; continue }
                $thenT = $lines[$thenIdx].Trim()
                if ($thenT -eq '{') { $i--; continue }  # skip braced form here
                if ($thenT -match '\{' -or $thenT -match '\}') { $i--; continue }
                $thenStmt = Extract-Statement $thenT
                if ($null -eq $thenStmt) { $i--; continue }
                $thenEnd = $thenIdx
            } elseif ($rest -match '^\{$' -or $rest -match '^\{\s*\}') {
                # then-block starts on same line with {
                $openIdx = $i
                # Find the single statement inside { }
                $depth = 0; $closeIdx = -1
                for ($k = $openIdx; $k -lt $lines.Count; $k++) {
                    foreach ($c in $lines[$k].ToCharArray()) {
                        if ($c -eq '{') { $depth++ } elseif ($c -eq '}') { $depth-- }
                    }
                    if ($depth -eq 0 -and $k -gt $openIdx) { $closeIdx = $k; break }
                }
                if ($closeIdx -lt 0) { $i--; continue }
                # Only one non-empty line inside
                $innerLines = @()
                for ($k = $openIdx + 1; $k -lt $closeIdx; $k++) {
                    $kt = $lines[$k].Trim()
                    if ($kt -ne '' -and -not $kt.StartsWith('//')) { $innerLines += $kt }
                }
                if ($innerLines.Count -ne 1) { $i--; continue }
                $thenStmt = Extract-Statement $innerLines[0]
                if ($null -eq $thenStmt) { $i--; continue }
                $thenEnd = $closeIdx
            } else {
                $i--; continue
            }

            # Now look for else
            $elseIdx = Next-NonEmpty $lines ($thenEnd + 1)
            if ($elseIdx -lt 0 -or $lines[$elseIdx].TrimStart() -notmatch '^else\b') { $i--; continue }

            $elseT = $lines[$elseIdx].TrimStart()
            # Must be plain "else" not "else if"
            if ($elseT -match '^else\s+if\b') { $i--; continue }

            # Determine else-body
            $elseStmt = $null
            $elseEnd  = -1

            $elseRest = ($elseT -replace '^else\s*', '').Trim()
            if ($elseRest -eq '' -or $elseRest -eq $null) {
                $elseBodyIdx = Next-NonEmpty $lines ($elseIdx + 1)
                if ($elseBodyIdx -lt 0) { $i--; continue }
                $elseBT = $lines[$elseBodyIdx].Trim()
                if ($elseBT -match '\{') { $i--; continue }
                $elseStmt = Extract-Statement $elseBT
                if ($null -eq $elseStmt) { $i--; continue }
                $elseEnd = $elseBodyIdx
            } elseif ($elseRest -match '^\{') {
                $openIdx = $elseIdx
                $depth = 0; $closeIdx = -1
                for ($k = $openIdx; $k -lt $lines.Count; $k++) {
                    foreach ($c in $lines[$k].ToCharArray()) {
                        if ($c -eq '{') { $depth++ } elseif ($c -eq '}') { $depth-- }
                    }
                    if ($depth -eq 0 -and $k -gt $openIdx) { $closeIdx = $k; break }
                }
                if ($closeIdx -lt 0) { $i--; continue }
                $innerLines = @()
                for ($k = $openIdx + 1; $k -lt $closeIdx; $k++) {
                    $kt = $lines[$k].Trim()
                    if ($kt -ne '' -and -not $kt.StartsWith('//')) { $innerLines += $kt }
                }
                if ($innerLines.Count -ne 1) { $i--; continue }
                $elseStmt = Extract-Statement $innerLines[0]
                if ($null -eq $elseStmt) { $i--; continue }
                $elseEnd = $closeIdx
            } else {
                # else stmt; on same line
                $elseStmt = Extract-Statement $elseRest
                if ($null -eq $elseStmt) { $i--; continue }
                $elseEnd = $elseIdx
            }

            # Both must have same verb and LHS
            if ($thenStmt.Verb -ne $elseStmt.Verb) { $i--; continue }
            if ($thenStmt.Verb -eq 'assign' -and $thenStmt.LHS -ne $elseStmt.LHS) { $i--; continue }

            # Build ternary
            $ternary = $null
            if ($thenStmt.Verb -eq 'return') {
                $ternary = "${indent}return $cond ? $($thenStmt.Expr) : $($elseStmt.Expr);"
            } else {
                $ternary = "${indent}$($thenStmt.LHS) = $cond ? $($thenStmt.Expr) : $($elseStmt.Expr);"
            }

            # Don't produce very long lines
            if ($ternary.Length -gt 200) { $i--; continue }

            # Replace i..elseEnd with ternary line
            $removeCount = $elseEnd - $i + 1
            $lines.RemoveRange($i, $removeCount)
            $lines.Insert($i, $ternary)

            $changed = $true; $fileFixed++
            # Stay at $i to check if we created another opportunity above
            $i--
        }

        if ($changed) {
            [System.IO.File]::WriteAllLines($file, $lines, [System.Text.UTF8Encoding]::new($false))
            Write-Host "  Fixed $fileFixed ternaries in: $($_.Name)"
            $script:totalFixed += $fileFixed
            $script:filesEdited++
        }
    }
}

Write-Host ""
Write-Host "Missed-ternary: converted $totalFixed if/else pairs in $filesEdited files"
