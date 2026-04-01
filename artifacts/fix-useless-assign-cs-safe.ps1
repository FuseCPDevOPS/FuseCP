#!/usr/bin/env pwsh
# fix-useless-assign-cs-safe.ps1
# Conservative fixer for cs/useless-assignment-to-local
# - Only .cs files
# - Excludes obj/bin/generated files
# - Removes literal assignments (null/false/true/0/""/-1)
# - Converts unused declaration/assignment with method call RHS into expression statements

$ErrorActionPreference = 'Stop'

$alertsPath = Join-Path $PSScriptRoot 'codeql-open-alerts.json'
if (-not (Test-Path $alertsPath)) {
    throw "Missing alerts file: $alertsPath"
}

$repoRoot = Split-Path $PSScriptRoot
$j = Get-Content $alertsPath | ConvertFrom-Json
$alerts = $j | Where-Object {
    $_.rule.id -eq 'cs/useless-assignment-to-local' -and
    $_.most_recent_instance.location.path -like '*.cs' -and
    $_.most_recent_instance.location.path -notlike '*/obj/*' -and
    $_.most_recent_instance.location.path -notlike '*/bin/*' -and
    $_.most_recent_instance.location.path -notlike '*.g.cs'
}

Write-Host "USELESS_ASSIGN_TARGETS=$($alerts.Count)"

$script:replacements = @{}
$byFile = @{}

foreach ($alert in $alerts) {
    $loc = $alert.most_recent_instance.location
    $absPath = Join-Path $repoRoot ($loc.path.Replace('/', '\\'))
    if (-not (Test-Path $absPath)) { continue }

    $lines = [System.IO.File]::ReadAllLines($absPath)
    $ln = [int]$loc.start_line - 1
    if ($ln -lt 0 -or $ln -ge $lines.Length) { continue }

    $msg = $alert.most_recent_instance.message.text
    $varName = if ($msg -match 'assignment to (\w+) is useless') { $Matches[1] } else { continue }

    $line = $lines[$ln]
    $trim = $line.Trim()
    $indent = $line.Substring(0, $line.Length - $line.TrimStart().Length)
    $replacement = $null

    # 1) direct assignment to literal constants => remove line
    $constAssign = '^' + [regex]::Escape($varName) + '\s*=\s*(null|false|true|0|""|-1)\s*;?\s*(//.*)?$'
    if ($trim -match $constAssign) {
        $replacement = ''
    }

    # 2) declaration with assignment
    if ($null -eq $replacement) {
        $declRe = '^(?:var|[A-Za-z_][A-Za-z0-9_<>,\.\[\]\?\s]*)\s+' + [regex]::Escape($varName) + '\s*=\s*(.+);\s*(//.*)?$'
        if ($trim -match $declRe) {
            $rhs = $Matches[1].Trim()

            if ($rhs -match '^(await\s+)?[A-Za-z_][A-Za-z0-9_\.]*\s*\(.*\)$') {
                $replacement = $indent + $rhs + ';'
            }
            elseif ($rhs -match '^\([^)]+\)\s*((?:await\s+)?[A-Za-z_][A-Za-z0-9_\.]*\s*\(.*\))$') {
                $replacement = $indent + $Matches[1] + ';'
            }
            elseif ($rhs -notmatch '\(') {
                $replacement = ''
            }
        }
    }

    # 3) assignment with expression
    if ($null -eq $replacement) {
        $assignRe = '^' + [regex]::Escape($varName) + '\s*=\s*(.+);\s*(//.*)?$'
        if ($trim -match $assignRe) {
            $rhs = $Matches[1].Trim()
            if ($rhs -match '^(await\s+)?[A-Za-z_][A-Za-z0-9_\.]*\s*\(.*\)$') {
                $replacement = $indent + $rhs + ';'
            }
            elseif ($rhs -notmatch '\(') {
                $replacement = ''
            }
        }
    }

    if ($null -eq $replacement) { continue }

    if (-not $byFile.ContainsKey($absPath)) { $byFile[$absPath] = [System.Collections.Generic.List[int]]::new() }
    $byFile[$absPath].Add($ln)
    $script:replacements["$absPath::$ln"] = $replacement
}

$linesPlanned = ($byFile.Values | ForEach-Object { $_.Count } | Measure-Object -Sum).Sum
if ($null -eq $linesPlanned) { $linesPlanned = 0 }
Write-Host "USELESS_ASSIGN_LINES_PLANNED=$linesPlanned"

$totalFixed = 0
$filesChanged = 0

foreach ($kvp in $byFile.GetEnumerator()) {
    $absPath = $kvp.Key
    $lineNumbers = $kvp.Value | Sort-Object -Descending -Unique

    $lines = [System.Collections.Generic.List[string]]([System.IO.File]::ReadAllLines($absPath))
    $fileChanged = $false

    foreach ($ln in $lineNumbers) {
        if ($ln -lt $lines.Count) {
            $key = "$absPath::$ln"
            $replacement = $script:replacements[$key]
            if ([string]::IsNullOrEmpty($replacement)) {
                $lines.RemoveAt($ln)
            }
            else {
                $lines[$ln] = $replacement
            }
            $totalFixed++
            $fileChanged = $true
        }
    }

    if ($fileChanged) {
        [System.IO.File]::WriteAllLines($absPath, $lines, [System.Text.UTF8Encoding]::new($false))
        $filesChanged++
        Write-Host "UPDATED: $($absPath.Replace($repoRoot + '\\', ''))"
    }
}

Write-Host "USELESS_ASSIGN_FIXED=$totalFixed"
Write-Host "USELESS_ASSIGN_FILES_CHANGED=$filesChanged"
