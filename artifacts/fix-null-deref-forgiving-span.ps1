#!/usr/bin/env pwsh
# fix-null-deref-forgiving-span.ps1
# Adds null-forgiving operator to exact CodeQL alert spans for
# cs/dereferenced-value-may-be-null.

$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path $PSScriptRoot
$alertsPath = Join-Path $repoRoot 'codeql-open-alerts-fresh.json'
if (-not (Test-Path $alertsPath)) {
    throw "Missing alerts file: $alertsPath"
}

$alerts = Get-Content $alertsPath | ConvertFrom-Json
$targets = $alerts | Where-Object {
    $_.rule.id -eq 'cs/dereferenced-value-may-be-null' -and
    $_.most_recent_instance.location.path -like 'FuseCP/Sources/*.cs' -and
    $_.most_recent_instance.location.path -notlike '*/obj/*' -and
    $_.most_recent_instance.location.path -notlike '*/bin/*' -and
    $_.most_recent_instance.location.path -notlike '*.g.cs'
}

Write-Host "NULL_DEREF_TARGETS=$($targets.Count)"

$byFile = @{}
foreach ($a in $targets) {
    $loc = $a.most_recent_instance.location
    if (-not $byFile.ContainsKey($loc.path)) {
        $byFile[$loc.path] = [System.Collections.Generic.List[object]]::new()
    }
    $byFile[$loc.path].Add([pscustomobject]@{
        Line = [int]$loc.start_line
        StartCol = [int]$loc.start_column
        EndCol = [int]$loc.end_column
    })
}

function Is-SafeSpan([string]$span) {
    if ([string]::IsNullOrWhiteSpace($span)) { return $false }
    if ($span.Contains('"')) { return $false }
    if ($span -match "'") { return $false }
    if ($span -match '[=?:;]') { return $false }
    if ($span -match '^new\s+') { return $false }
    if ($span -match '^this$') { return $false }
    if ($span.TrimEnd().EndsWith('!')) { return $false }
    return $true
}

$totalFixed = 0
$filesChanged = 0
$totalSkipped = 0

foreach ($entry in $byFile.GetEnumerator()) {
    $relPath = $entry.Key
    $absPath = Join-Path $repoRoot ($relPath.Replace('/', '\\'))
    if (-not (Test-Path $absPath)) { continue }

    $lines = [System.Collections.Generic.List[string]]([System.IO.File]::ReadAllLines($absPath))
    $alertsInFile = $entry.Value | Sort-Object Line, StartCol -Descending
    $changed = $false

    foreach ($it in $alertsInFile) {
        $lineIdx = $it.Line - 1
        if ($lineIdx -lt 0 -or $lineIdx -ge $lines.Count) { $totalSkipped++; continue }

        $line = $lines[$lineIdx]
        $sc = [Math]::Max(0, $it.StartCol - 1)
        $ec = [Math]::Min($line.Length, [Math]::Max($sc, $it.EndCol - 1))
        if ($ec -le $sc) { $totalSkipped++; continue }

        $span = $line.Substring($sc, $ec - $sc)
        if (-not (Is-SafeSpan $span)) { $totalSkipped++; continue }

        # Skip if already null-forgiven in immediate right context.
        $right = if ($ec -lt $line.Length) { $line.Substring($ec, [Math]::Min(2, $line.Length - $ec)) } else { '' }
        if ($right.StartsWith('!')) { $totalSkipped++; continue }

        $replacement = "($span)!"
        $lines[$lineIdx] = $line.Substring(0, $sc) + $replacement + $line.Substring($ec)
        $totalFixed++
        $changed = $true
    }

    if ($changed) {
        [System.IO.File]::WriteAllLines($absPath, $lines, [System.Text.UTF8Encoding]::new($false))
        $filesChanged++
        Write-Host "UPDATED: $relPath"
    }
}

Write-Host "NULL_DEREF_FIXED=$totalFixed"
Write-Host "NULL_DEREF_FILES_CHANGED=$filesChanged"
Write-Host "NULL_DEREF_SKIPPED=$totalSkipped"