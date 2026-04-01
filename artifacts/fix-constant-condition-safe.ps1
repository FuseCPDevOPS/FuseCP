#!/usr/bin/env pwsh
# fix-constant-condition-safe.ps1
# Safe, line-targeted simplifications for cs/constant-condition alerts.
#
# Patterns handled:
# 1) x == null || x != null && expr   ->   x == null || expr
# 2) x == C || (x != C && expr)       ->   x == C || expr
#
# Only lines referenced by CodeQL cs/constant-condition alerts are edited.

$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path $PSScriptRoot
$alertsPath = Join-Path $PSScriptRoot 'codeql-open-alerts.json'
if (-not (Test-Path $alertsPath)) {
    throw "Missing alerts file: $alertsPath"
}

$alerts = Get-Content $alertsPath | ConvertFrom-Json
$targets = $alerts | Where-Object {
    $_.rule.id -eq 'cs/constant-condition' -and
    $_.most_recent_instance.location.path -like '*.cs' -and
    $_.most_recent_instance.location.path -notlike '*/obj/*' -and
    $_.most_recent_instance.location.path -notlike '*.g.cs'
}

Write-Host "CONSTANT_TARGETS=$($targets.Count)"

$byFile = @{}
foreach ($a in $targets) {
    $loc = $a.most_recent_instance.location
    $rel = $loc.path
    if (-not $byFile.ContainsKey($rel)) {
        $byFile[$rel] = [System.Collections.Generic.List[int]]::new()
    }
    $byFile[$rel].Add([int]$loc.start_line)
}

function Simplify-NullGuardLine {
    param([string]$line)

    $pattern = '(?<pre>.*?)(?<v>[A-Za-z_]\w*)\s*==\s*null\s*\|\|\s*(?<v2>[A-Za-z_]\w*)\s*!=\s*null\s*&&\s*(?<post>.*)'
    $m = [regex]::Match($line, $pattern)
    if (-not $m.Success) { return $line }

    if ($m.Groups['v'].Value -ne $m.Groups['v2'].Value) { return $line }

    return "$($m.Groups['pre'].Value)$($m.Groups['v'].Value) == null || $($m.Groups['post'].Value)"
}

function Simplify-ValueGuardLine {
    param([string]$line)

    $pattern = '(?<pre>.*?)(?<v>[A-Za-z_]\w*)\s*==\s*(?<c>-?\d+)\s*\|\|\s*\(\s*(?<v2>[A-Za-z_]\w*)\s*!=\s*(?<c2>-?\d+)\s*&&\s*(?<post>[^\)]*)\)(?<suffix>.*)'
    $m = [regex]::Match($line, $pattern)
    if (-not $m.Success) { return $line }

    if ($m.Groups['v'].Value -ne $m.Groups['v2'].Value) { return $line }
    if ($m.Groups['c'].Value -ne $m.Groups['c2'].Value) { return $line }

    return "$($m.Groups['pre'].Value)$($m.Groups['v'].Value) == $($m.Groups['c'].Value) || $($m.Groups['post'].Value)$($m.Groups['suffix'].Value)"
}

function Simplify-BoolGateLine {
    param([string]$line)

    # !a || (a && expr)  ->  !a || (expr)
    $p1 = '(?<pre>.*?)!\s*(?<v>[A-Za-z_]\w*)\s*\|\|\s*\(\s*(?<v2>[A-Za-z_]\w*)\s*&&\s*(?<rest>.*)'
    $m1 = [regex]::Match($line, $p1)
    if ($m1.Success -and $m1.Groups['v'].Value -eq $m1.Groups['v2'].Value) {
        return "$($m1.Groups['pre'].Value)!$($m1.Groups['v'].Value) || ($($m1.Groups['rest'].Value)"
    }

    # !a || a && expr  ->  !a || expr
    $p2 = '(?<pre>.*?)!\s*(?<v>[A-Za-z_]\w*)\s*\|\|\s*(?<v2>[A-Za-z_]\w*)\s*&&\s*(?<rest>.*)'
    $m2 = [regex]::Match($line, $p2)
    if ($m2.Success -and $m2.Groups['v'].Value -eq $m2.Groups['v2'].Value) {
        return "$($m2.Groups['pre'].Value)!$($m2.Groups['v'].Value) || $($m2.Groups['rest'].Value)"
    }

    # a || (!a && expr)  ->  a || (expr)
    $p3 = '(?<pre>.*?)(?<v>[A-Za-z_]\w*)\s*\|\|\s*\(\s*!\s*(?<v2>[A-Za-z_]\w*)\s*&&\s*(?<rest>.*)'
    $m3 = [regex]::Match($line, $p3)
    if ($m3.Success -and $m3.Groups['v'].Value -eq $m3.Groups['v2'].Value) {
        return "$($m3.Groups['pre'].Value)$($m3.Groups['v'].Value) || ($($m3.Groups['rest'].Value)"
    }

    # a || !a && expr  ->  a || expr
    $p4 = '(?<pre>.*?)(?<v>[A-Za-z_]\w*)\s*\|\|\s*!\s*(?<v2>[A-Za-z_]\w*)\s*&&\s*(?<rest>.*)'
    $m4 = [regex]::Match($line, $p4)
    if ($m4.Success -and $m4.Groups['v'].Value -eq $m4.Groups['v2'].Value) {
        return "$($m4.Groups['pre'].Value)$($m4.Groups['v'].Value) || $($m4.Groups['rest'].Value)"
    }

    return $line
}

$totalFixed = 0
$filesChanged = 0

foreach ($entry in $byFile.GetEnumerator()) {
    $relPath = $entry.Key
    $absPath = Join-Path $repoRoot ($relPath.Replace('/', '\\'))
    if (-not (Test-Path $absPath)) { continue }

    $lines = [System.IO.File]::ReadAllLines($absPath)
    $lineNums = $entry.Value | Sort-Object -Unique
    $changed = $false
    $fileFixed = 0

    foreach ($ln in $lineNums) {
        $idx = $ln - 1
        if ($idx -lt 0 -or $idx -ge $lines.Length) { continue }

        $orig = $lines[$idx]
        $next = Simplify-NullGuardLine $orig
        if ($next -eq $orig) {
            $next = Simplify-ValueGuardLine $orig
        }
        if ($next -eq $orig) {
            $next = Simplify-BoolGateLine $orig
        }

        if ($next -ne $orig) {
            $lines[$idx] = $next
            $changed = $true
            $fileFixed++
            $totalFixed++
            Write-Host "FIXED: ${relPath}:$ln"
        }
    }

    if ($changed) {
        [System.IO.File]::WriteAllLines($absPath, $lines, [System.Text.UTF8Encoding]::new($false))
        $filesChanged++
        Write-Host "  File updated: $relPath ($fileFixed changes)"
    }
}

Write-Host "CONSTANT_FIXED=$totalFixed"
Write-Host "CONSTANT_FILES_CHANGED=$filesChanged"
