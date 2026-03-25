#!/usr/bin/env pwsh
# fix-missed-ternary.ps1 — convert simple if/else return/assign to ternary

# Helper: extract if-condition and remainder using paren-balancing
function Split-IfCondition([string]$line) {
    $trimmed = $line.TrimStart()
    if ($trimmed -notmatch '^if\s*\(') { return $null }
    $start = $trimmed.IndexOf('(')
    $depth = 0
    $end = -1
    for ($k = $start; $k -lt $trimmed.Length; $k++) {
        if ($trimmed[$k] -eq '(') { $depth++ }
        elseif ($trimmed[$k] -eq ')') {
            $depth--
            if ($depth -eq 0) { $end = $k; break }
        }
    }
    if ($end -lt 0) { return $null }
    return @($trimmed.Substring($start+1, $end-$start-1).Trim(), $trimmed.Substring($end+1).Trim())
}

function Get-SingleStatementFromBlock([string[]]$lines, [int]$openLine0) {
    if ($openLine0 -lt 0 -or $openLine0 -ge $lines.Count) { return $null }
    if ($lines[$openLine0].Trim() -notmatch '^\{\s*$') { return $null }

    $depth = 0
    $closeLine0 = -1
    for ($i = $openLine0; $i -lt $lines.Count; $i++) {
        foreach ($c in $lines[$i].ToCharArray()) {
            if ($c -eq '{') { $depth++ }
            elseif ($c -eq '}') { $depth-- }
        }
        if ($depth -eq 0) { $closeLine0 = $i; break }
    }
    if ($closeLine0 -lt 0) { return $null }

    $codeLines = @()
    for ($i = $openLine0 + 1; $i -lt $closeLine0; $i++) {
        $trimmed = $lines[$i].Trim()
        if ($trimmed -eq '' -or $trimmed.StartsWith('//')) { continue }
        $codeLines += [pscustomobject]@{ Line = $i; Text = $trimmed }
    }

    if ($codeLines.Count -ne 1) { return $null }

    return [pscustomobject]@{
        Statement = $codeLines[0].Text
        OpenLine = $openLine0
        CloseLine = $closeLine0
        StatementLine = $codeLines[0].Line
    }
}

$alertsJson = Join-Path $PSScriptRoot "codeql-open-alerts.json"
$repoRoot   = Split-Path $PSScriptRoot

$alerts  = Get-Content $alertsJson | ConvertFrom-Json
$targets = $alerts | Where-Object { $_.rule.id -eq "cs/missed-ternary-operator" }
Write-Host "Total alerts: $($targets.Count)"

$totalFixed = 0
$byFile = @{}
foreach ($a in $targets) {
    $loc  = $a.most_recent_instance.location
    $path = $loc.path
    if (-not $byFile.ContainsKey($path)) { $byFile[$path] = [System.Collections.Generic.List[object]]::new() }
    $byFile[$path].Add([pscustomobject]@{ Line = $loc.start_line })
}

foreach ($filePath in $byFile.Keys) {
    $absPath = Join-Path $repoRoot $filePath
    if (-not (Test-Path $absPath)) { continue }

    $lines   = [System.IO.File]::ReadAllLines($absPath)
    $changed = $false

    $fileAlerts = $byFile[$filePath] | Sort-Object Line -Descending

    foreach ($alert in $fileAlerts) {
        $ln0 = $alert.Line - 1
        if ($ln0 -lt 0 -or $ln0 -ge $lines.Count) { continue }

        $ifLine    = $lines[$ln0].TrimEnd()
        $indent    = $ifLine.Length - $ifLine.TrimStart().Length
        $spaces    = $ifLine.Substring(0, $indent)
        $trimmedIf = $ifLine.Trim()

        if (-not ($trimmedIf -match '^if\s*\(')) { continue }
        if ($trimmedIf -match '\{') { continue }

        $ifSplit = Split-IfCondition $trimmedIf
        if ($ifSplit -eq $null) { continue }
        $cond       = $ifSplit[0]
        $inlineBody = $ifSplit[1]

        # Case A: inline body after if (cond)
        if ($inlineBody -ne "") {
            $elseLn = $ln0 + 1
            while ($elseLn -lt $lines.Count -and $lines[$elseLn].Trim() -eq "") { $elseLn++ }
            if ($elseLn -ge $lines.Count) { continue }
            $trimmedElse = $lines[$elseLn].Trim()
            if (-not $trimmedElse.StartsWith("else")) { continue }
            if ($trimmedElse.StartsWith("else if")) { continue }
            if ($trimmedElse -match '\{') { continue }

            $ifRetM   = [regex]::Match($inlineBody,  '^return\s+(.+);\s*$')
            $elseRetM = [regex]::Match($trimmedElse, '^else\s+return\s+(.+);\s*$')
            if ($ifRetM.Success -and $elseRetM.Success) {
                $trueV  = $ifRetM.Groups[1].Value
                $falseV = $elseRetM.Groups[1].Value
                if ($trueV -notmatch '\?' -and $falseV -notmatch '\?') {
                    $lines[$ln0]    = "${spaces}return $cond ? $trueV : $falseV;"
                    $lines[$elseLn] = $null
                    $changed = $true; $totalFixed++
                    Write-Host "TERNARY(A-ret): $([System.IO.Path]::GetFileName($filePath)) L$($alert.Line)"
                    continue
                }
            }

            $ifAssM   = [regex]::Match($inlineBody,  '^(\w[\w.]*)\s*=\s*(.+);\s*$')
            $elseAssM = [regex]::Match($trimmedElse, '^else\s+(\w[\w.]*)\s*=\s*(.+);\s*$')
            if ($ifAssM.Success -and $elseAssM.Success) {
                $varIf = $ifAssM.Groups[1].Value;   $trueV  = $ifAssM.Groups[2].Value
                $varEl = $elseAssM.Groups[1].Value; $falseV = $elseAssM.Groups[2].Value
                if ($varIf -eq $varEl -and $trueV -notmatch '\?' -and $falseV -notmatch '\?') {
                    $lines[$ln0]    = "${spaces}$varIf = $cond ? $trueV : $falseV;"
                    $lines[$elseLn] = $null
                    $changed = $true; $totalFixed++
                    Write-Host "TERNARY(A-ass): $([System.IO.Path]::GetFileName($filePath)) L$($alert.Line)"
                    continue
                }
            }
            continue
        }

        # Case B: multi-line body
        $bodyLn = $ln0 + 1
        while ($bodyLn -lt $lines.Count -and $lines[$bodyLn].Trim() -eq "") { $bodyLn++ }
        if ($bodyLn -ge $lines.Count) { continue }
        $bodyLine = $lines[$bodyLn].Trim()
        if ($bodyLine.StartsWith("else")) { continue }

        $ifLinesToRemove = @($bodyLn)
        if ($bodyLine -match '^\{\s*$') {
            $ifBlock = Get-SingleStatementFromBlock $lines $bodyLn
            if ($ifBlock -eq $null) { continue }
            $bodyLine = $ifBlock.Statement
            $ifLinesToRemove = @($ifBlock.OpenLine..$ifBlock.CloseLine)
            $elseLn = $ifBlock.CloseLine + 1
        } else {
            $elseLn = $bodyLn + 1
        }

        while ($elseLn -lt $lines.Count -and $lines[$elseLn].Trim() -eq "") { $elseLn++ }
        if ($elseLn -ge $lines.Count) { continue }
        $trimmedElse = $lines[$elseLn].Trim()
        if (-not $trimmedElse.StartsWith("else")) { continue }
        if ($trimmedElse.StartsWith("else if")) { continue }

        $elseBody    = ""
        $elseBodyLn  = -1
        $elseLinesToRemove = @($elseLn)
        if ($trimmedElse -match '^else\s+(.+)$') {
            $candidate = $Matches[1].Trim()
            if ($candidate -match '^\{\s*$') {
                $elseBlock = Get-SingleStatementFromBlock $lines $elseLn
                if ($elseBlock -eq $null) { continue }
                $elseBody = $elseBlock.Statement
                $elseBodyLn = $elseBlock.StatementLine
                $elseLinesToRemove = @($elseBlock.OpenLine..$elseBlock.CloseLine)
            } else {
                $elseBody   = $candidate
                $elseBodyLn = $elseLn
            }
        } else {
            $elseBodyLn = $elseLn + 1
            while ($elseBodyLn -lt $lines.Count -and $lines[$elseBodyLn].Trim() -eq "") { $elseBodyLn++ }
            if ($elseBodyLn -ge $lines.Count) { continue }
            if ($lines[$elseBodyLn].Trim() -match '^\{\s*$') {
                $elseBlock = Get-SingleStatementFromBlock $lines $elseBodyLn
                if ($elseBlock -eq $null) { continue }
                $elseBody = $elseBlock.Statement
                $elseBodyLn = $elseBlock.StatementLine
                $elseLinesToRemove = @($elseLn) + @($elseBlock.OpenLine..$elseBlock.CloseLine)
            } else {
                $elseBody = $lines[$elseBodyLn].Trim()
                $elseLinesToRemove = @($elseLn, $elseBodyLn)
            }
        }

        $ifRetM   = [regex]::Match($bodyLine, '^return\s+(.+);\s*$')
        $elseRetM = [regex]::Match($elseBody, '^return\s+(.+);\s*$')
        if ($ifRetM.Success -and $elseRetM.Success) {
            $trueV  = $ifRetM.Groups[1].Value
            $falseV = $elseRetM.Groups[1].Value
            if ($trueV -notmatch '\?' -and $falseV -notmatch '\?') {
                $lines[$ln0]    = "${spaces}return $cond ? $trueV : $falseV;"
                foreach ($idx in ($ifLinesToRemove + $elseLinesToRemove | Sort-Object -Unique)) { $lines[$idx] = $null }
                $changed = $true; $totalFixed++
                Write-Host "TERNARY(B-ret): $([System.IO.Path]::GetFileName($filePath)) L$($alert.Line)"
                continue
            }
        }

        $ifAssM   = [regex]::Match($bodyLine, '^(\w[\w.]*)\s*=\s*(.+);\s*$')
        $elseAssM = [regex]::Match($elseBody, '^(\w[\w.]*)\s*=\s*(.+);\s*$')
        if ($ifAssM.Success -and $elseAssM.Success) {
            $varIf = $ifAssM.Groups[1].Value;   $trueV  = $ifAssM.Groups[2].Value
            $varEl = $elseAssM.Groups[1].Value; $falseV = $elseAssM.Groups[2].Value
            if ($varIf -eq $varEl -and $trueV -notmatch '\?' -and $falseV -notmatch '\?') {
                $lines[$ln0]    = "${spaces}$varIf = $cond ? $trueV : $falseV;"
                foreach ($idx in ($ifLinesToRemove + $elseLinesToRemove | Sort-Object -Unique)) { $lines[$idx] = $null }
                $changed = $true; $totalFixed++
                Write-Host "TERNARY(B-ass): $([System.IO.Path]::GetFileName($filePath)) L$($alert.Line)"
                continue
            }
        }
    }

    if ($changed) {
        $filtered = $lines | Where-Object { $_ -ne $null }
        [System.IO.File]::WriteAllLines($absPath, $filtered, [System.Text.UTF8Encoding]::new($false))
    }
}

Write-Host "Total fixed: $totalFixed"