#!/usr/bin/env pwsh
# fix-js-useless-expression-safe.ps1
# Conservative fixer for js/useless-expression.
# Only rewrites alert lines that are obviously side-effect-free standalone expressions.

$ErrorActionPreference = 'Stop'

$alertsPath = Join-Path $PSScriptRoot 'codeql-open-alerts.json'
if (-not (Test-Path $alertsPath)) {
    throw "Missing alerts file: $alertsPath"
}

$repoRoot = Split-Path $PSScriptRoot
$j = Get-Content $alertsPath | ConvertFrom-Json
$targets = $j | Where-Object {
    $_.rule.id -eq 'js/useless-expression' -and
    $_.most_recent_instance.location.path -match '\.(js|mjs|cjs)$' -and
    $_.most_recent_instance.location.path -notlike '*/node_modules/*'
}

Write-Host "JS_USELESS_TARGETS=$($targets.Count)"

$byFile = @{}
foreach ($a in $targets) {
    $loc = $a.most_recent_instance.location
    if (-not $byFile.ContainsKey($loc.path)) {
        $byFile[$loc.path] = [System.Collections.Generic.List[int]]::new()
    }
    $byFile[$loc.path].Add([int]$loc.start_line)
}

function Is-SafeUselessExpressionLine([string]$trimmed) {
    if ([string]::IsNullOrWhiteSpace($trimmed)) { return $false }
    if ($trimmed.StartsWith('//') -or $trimmed.StartsWith('/*') -or $trimmed.StartsWith('*')) { return $false }

    # Must be a single statement ending with semicolon and without assignment/call keywords.
    if ($trimmed -notmatch ';\s*$') { return $false }
    if ($trimmed -match '\b(return|throw|break|continue|if|for|while|switch|new|delete|await|yield)\b') { return $false }
    if ($trimmed -match '(?<![=!<>])=(?!=)') { return $false }

    # Allow only literal/identifier/property chain / bracket access / unary ! wrappers.
    $expr = $trimmed.TrimEnd(';').Trim()

    if ($expr -match '^!?[A-Za-z_$][A-Za-z0-9_$]*(\.[A-Za-z_$][A-Za-z0-9_$]*|\[[^\]]+\])*$') { return $true }
    if ($expr -match "^`"[^`"]*`"$|^'[^']*'$|^\d+(\.\d+)?$|^(true|false|null|undefined)$") { return $true }

    return $false
}

$totalFixed = 0
$filesChanged = 0

foreach ($entry in $byFile.GetEnumerator()) {
    $relPath = $entry.Key
    $absPath = Join-Path $repoRoot ($relPath.Replace('/', '\\'))
    if (-not (Test-Path $absPath)) { continue }

    $lines = [System.Collections.Generic.List[string]]([System.IO.File]::ReadAllLines($absPath))
    $lineNums = $entry.Value | Sort-Object -Descending -Unique
    $changed = $false

    foreach ($ln in $lineNums) {
        $idx = $ln - 1
        if ($idx -lt 0 -or $idx -ge $lines.Count) { continue }

        $line = $lines[$idx]
        $trim = $line.Trim()
        if (-not (Is-SafeUselessExpressionLine $trim)) { continue }

        $indent = $line.Substring(0, $line.Length - $line.TrimStart().Length)
        $lines[$idx] = "$indent// removed useless expression (CodeQL): $trim"
        $totalFixed++
        $changed = $true
        Write-Host "FIXED: ${relPath}:$ln"
    }

    if ($changed) {
        [System.IO.File]::WriteAllLines($absPath, $lines, [System.Text.UTF8Encoding]::new($false))
        $filesChanged++
    }
}

Write-Host "JS_USELESS_FIXED=$totalFixed"
Write-Host "JS_USELESS_FILES_CHANGED=$filesChanged"
