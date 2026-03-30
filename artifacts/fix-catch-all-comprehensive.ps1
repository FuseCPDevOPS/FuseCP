#!/usr/bin/env pwsh
# fix-catch-all-comprehensive.ps1
# Comprehensive scan of ALL .cs files to add when (fatal-exclusion) to bare catch-all blocks.
# Does NOT rely on stale API alert line numbers.

param(
    [string[]]$Roots = @("FuseCP\Sources"),
    [switch]$WhatIf
)

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path $PSScriptRoot

$totalFixed   = 0
$filesChanged = 0

$whenTemplate = "when ({0} is not (OutOfMemoryException or StackOverflowException or AccessViolationException))"

foreach ($root in $Roots) {
    $absRoot = Join-Path $repoRoot $root
    if (-not (Test-Path $absRoot)) { continue }

    $csFiles = Get-ChildItem -Recurse -Path $absRoot -Include "*.cs" |
        Where-Object { $_.FullName -notmatch "\\(bin|obj|\.git)\\" }

    foreach ($file in $csFiles) {
        $lines    = [System.Collections.Generic.List[string]]([System.IO.File]::ReadAllLines($file.FullName))
        $changed  = $false
        $fileFixed = 0

        for ($i = 0; $i -lt $lines.Count; $i++) {
            $line    = $lines[$i]
            $trimmed = $line.TrimStart()

            # Skip any catch that already has when(
            if ($trimmed -match "^catch\b" -and $trimmed -match "\bwhen\s*\(") { continue }

            $indentLen = $line.Length - $trimmed.Length
            $indent    = $line.Substring(0, $indentLen)

            # catch (System.Exception varname)  or  catch (Exception varname)
            if ($trimmed -match "^catch\s*\(\s*(System\.)?Exception\s+([A-Za-z_]\w*)\s*\)\s*$") {
                $sysPrefix = $Matches[1]; $var = $Matches[2]
                $when = $whenTemplate -f $var
                $lines[$i] = "${indent}catch (${sysPrefix}Exception $var) $when"
                $changed = $true; $fileFixed++; continue
            }
            # catch (System.Exception varname) {
            if ($trimmed -match "^catch\s*\(\s*(System\.)?Exception\s+([A-Za-z_]\w*)\s*\)\s*\{") {
                $sysPrefix = $Matches[1]; $var = $Matches[2]
                $when = $whenTemplate -f $var
                $lines[$i] = "${indent}catch (${sysPrefix}Exception $var) $when {"
                $changed = $true; $fileFixed++; continue
            }
            # catch (System.Exception)  or  catch (Exception)  no var
            if ($trimmed -match "^catch\s*\(\s*(System\.)?Exception\s*\)\s*$") {
                $sysPrefix = $Matches[1]
                $when = $whenTemplate -f "ex"
                $lines[$i] = "${indent}catch (${sysPrefix}Exception ex) $when"
                $changed = $true; $fileFixed++; continue
            }
            # catch (System.Exception) {  or  catch (Exception) {
            if ($trimmed -match "^catch\s*\(\s*(System\.)?Exception\s*\)\s*\{") {
                $sysPrefix = $Matches[1]
                $when = $whenTemplate -f "ex"
                $lines[$i] = "${indent}catch (${sysPrefix}Exception ex) $when {"
                $changed = $true; $fileFixed++; continue
            }
            # bare catch
            if ($trimmed -match "^catch\s*$") {
                $when = $whenTemplate -f "ex"
                $lines[$i] = "${indent}catch (Exception ex) $when"
                $changed = $true; $fileFixed++; continue
            }
            # bare catch {
            if ($trimmed -match "^catch\s*\{") {
                $when = $whenTemplate -f "ex"
                $lines[$i] = "${indent}catch (Exception ex) $when {"
                $changed = $true; $fileFixed++; continue
            }
        }

        if ($changed) {
            if (-not $WhatIf) {
                [System.IO.File]::WriteAllLines($file.FullName, $lines, [System.Text.UTF8Encoding]::new($false))
            }
            $filesChanged++
            $totalFixed += $fileFixed
            Write-Host "  Fixed $fileFixed in $($file.Name)"
        }
    }
}

Write-Host ""
Write-Host "Total catch-of-all fixed: $totalFixed across $filesChanged files"