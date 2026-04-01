#!/usr/bin/env pwsh
# fix-cookie-hardening-safe.ps1
# Safe fixer for:
#  - cs/web/cookie-httponly-not-set
#  - cs/web/cookie-secure-not-set
#
# Strategy:
# - Use only CodeQL alert locations.
# - Touch only HttpCookie variable flows with Cookies.Add(varCookie).
# - Insert missing assignments immediately before Add():
#     varCookie.HttpOnly = true;
#     varCookie.Secure = System.Web.Security.FormsAuthentication.RequireSSL;

$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path $PSScriptRoot
$alertsPath = Join-Path $PSScriptRoot 'codeql-open-alerts.json'
if (-not (Test-Path $alertsPath)) {
    throw "Missing alerts file: $alertsPath"
}

$alerts = Get-Content $alertsPath | ConvertFrom-Json
$targets = $alerts | Where-Object {
    ($_.rule.id -eq 'cs/web/cookie-httponly-not-set' -or $_.rule.id -eq 'cs/web/cookie-secure-not-set') -and
    $_.most_recent_instance.location.path -like '*.cs' -and
    $_.most_recent_instance.location.path -like 'FuseCP/Sources/FuseCP.WebPortal/*' -and
    $_.most_recent_instance.location.path -notlike '*/obj/*' -and
    $_.most_recent_instance.location.path -notlike '*.g.cs'
}

Write-Host "COOKIE_TARGETS=$($targets.Count)"

$byFile = @{}
foreach ($a in $targets) {
    $loc = $a.most_recent_instance.location
    $item = [pscustomobject]@{
        Rule = $a.rule.id
        Line = [int]$loc.start_line
    }

    if (-not $byFile.ContainsKey($loc.path)) {
        $byFile[$loc.path] = [System.Collections.Generic.List[object]]::new()
    }
    $byFile[$loc.path].Add($item)
}

$totalInserted = 0
$filesChanged = 0

foreach ($entry in $byFile.GetEnumerator()) {
    $relPath = $entry.Key
    $absPath = Join-Path $repoRoot ($relPath.Replace('/', '\\'))
    if (-not (Test-Path $absPath)) { continue }

    $lines = [System.Collections.Generic.List[string]]([System.IO.File]::ReadAllLines($absPath))
    $alertsForFile = $entry.Value

    # planned inserts: key = insertion line index, value = set of literal lines to insert
    $planned = @{}

    foreach ($a in $alertsForFile) {
        $hint = [Math]::Max(0, $a.Line - 1)

        # find closest Cookies.Add(varName) near hint line
        $addIndex = -1
        $cookieVar = $null
        for ($d = 0; $d -le 8; $d++) {
            $up = $hint - $d
            if ($up -ge 0 -and $up -lt $lines.Count) {
                $mAdd = [regex]::Match($lines[$up], 'Cookies\.Add\(\s*(?<v>[A-Za-z_]\w*)\s*\)')
                if ($mAdd.Success) {
                    $addIndex = $up
                    $cookieVar = $mAdd.Groups['v'].Value
                    break
                }
            }

            if ($d -gt 0) {
                $down = $hint + $d
                if ($down -ge 0 -and $down -lt $lines.Count) {
                    $mAdd = [regex]::Match($lines[$down], 'Cookies\.Add\(\s*(?<v>[A-Za-z_]\w*)\s*\)')
                    if ($mAdd.Success) {
                        $addIndex = $down
                        $cookieVar = $mAdd.Groups['v'].Value
                        break
                    }
                }
            }
            if ($addIndex -ge 0) { break }
        }

        if ($addIndex -lt 0 -or [string]::IsNullOrEmpty($cookieVar)) { continue }

        # find declaration for that cookie variable before Add
        $declIndex = -1
        for ($i = $addIndex; $i -ge [Math]::Max(0, $addIndex - 40); $i--) {
            if ($lines[$i] -match ('\bHttpCookie\s+' + [regex]::Escape($cookieVar) + '\s*=\s*new\s+HttpCookie\s*\(')) {
                $declIndex = $i
                break
            }
        }
        if ($declIndex -lt 0) { continue }

        # check existing assignments in declaration-to-add window
        $hasHttpOnly = $false
        $hasSecure = $false
        for ($i = $declIndex; $i -le $addIndex; $i++) {
            if ($lines[$i] -match ('\b' + [regex]::Escape($cookieVar) + '\s*\.\s*HttpOnly\s*=\s*true\s*;')) { $hasHttpOnly = $true }
            if ($lines[$i] -match ('\b' + [regex]::Escape($cookieVar) + '\s*\.\s*Secure\s*=')) { $hasSecure = $true }
        }

        $indent = ($lines[$addIndex] -replace '^(\s*).*', '$1')
        $httpOnlyLine = "$indent$cookieVar.HttpOnly = true;"
        $secureLine = "$indent$cookieVar.Secure = System.Web.Security.FormsAuthentication.RequireSSL;"

        if (-not $planned.ContainsKey($addIndex)) {
            $planned[$addIndex] = [System.Collections.Generic.List[string]]::new()
        }

        if ($a.Rule -eq 'cs/web/cookie-httponly-not-set' -and -not $hasHttpOnly -and -not $planned[$addIndex].Contains($httpOnlyLine)) {
            $planned[$addIndex].Add($httpOnlyLine)
        }

        if ($a.Rule -eq 'cs/web/cookie-secure-not-set' -and -not $hasSecure -and -not $planned[$addIndex].Contains($secureLine)) {
            $planned[$addIndex].Add($secureLine)
        }
    }

    if ($planned.Keys.Count -eq 0) { continue }

    foreach ($insertAt in ($planned.Keys | Sort-Object -Descending)) {
        $toInsert = $planned[$insertAt]
        foreach ($line in $toInsert) {
            $lines.Insert($insertAt, $line)
            $totalInserted++
            Write-Host "INSERTED: ${relPath}:$($insertAt+1) :: $line"
        }
    }

    [System.IO.File]::WriteAllLines($absPath, $lines, [System.Text.UTF8Encoding]::new($false))
    $filesChanged++
}

Write-Host "COOKIE_INSERTED=$totalInserted"
Write-Host "COOKIE_FILES_CHANGED=$filesChanged"
