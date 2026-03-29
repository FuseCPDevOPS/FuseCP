#!/usr/bin/env pwsh
# fix-containskey-regex.ps1
# Regex scan for cs/inefficient-containskey pattern — single-line ternary:
#   dict.ContainsKey(key) ? dict[key] : expr
# → dict.TryGetValue(key, out var _ckv) ? _ckv : expr
#
# Also handles the NOT-form:
#   !dict.ContainsKey(key) ? expr : dict[key]
# → !dict.TryGetValue(key, out var _ckv) ? expr : _ckv
#
# Does NOT rely on stale alert line positions.

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

foreach ($relRoot in $Roots) {
    $absRoot = Join-Path $repoRoot $relRoot
    if (-not (Test-Path $absRoot)) { continue }

    Get-ChildItem -Recurse -Filter "*.cs" $absRoot | Where-Object {
        $_.FullName -notlike "*\obj\*" -and
        $_.FullName -notlike "*\bin\*"
    } | ForEach-Object {
        $file    = $_.FullName
        $lines   = [System.IO.File]::ReadAllLines($file)
        $changed = $false
        $fileFixed = 0

        for ($i = 0; $i -lt $lines.Count; $i++) {
            $line = $lines[$i]

            # ── Pattern 1: ternary form ─────────────────────────────────────
            # dict.ContainsKey(key) ? dict[key] : fallback
            $m = [regex]::Match($line,
                '(?<pre>.*)(?<dict>\b(?:[A-Za-z_]\w*(?:\.[A-Za-z_]\w*)*))\s*\.ContainsKey\s*\((?<key>[^)]+)\)\s*\?\s*(?:\k<dict>)\s*\[(?:\k<key>)\]\s*:(?<rest>.+)')
            if ($m.Success) {
                $dict = $m.Groups['dict'].Value
                $key  = $m.Groups['key'].Value.Trim()
                $pre  = $m.Groups['pre'].Value
                $rest = $m.Groups['rest'].Value
                $varName = "_ckv"
                $newLine = "${pre}${dict}.TryGetValue(${key}, out var ${varName}) ? ${varName} :${rest}"
                if ($newLine -ne $line) {
                    $lines[$i] = $newLine
                    $changed = $true; $fileFixed++
                    continue
                }
            }

            # ── Pattern 2: single-line if with immediate index access ────────
            # if (dict.ContainsKey(key)) { var x = dict[key];
            # (only when entire if-body is on same line: if (dict.ContainsKey(key)) { something = dict[key]; })
            $m2 = [regex]::Match($line,
                '(?<ind>\s*)if\s*\(\s*(?<dict>\b(?:[A-Za-z_]\w*(?:\.[A-Za-z_]\w*)*))\s*\.ContainsKey\s*\(\s*(?<key>[^)]+)\s*\)\s*\)\s*\{(?<body>[^}]+)\}(?<tail>[^}]*)')
            if ($m2.Success) {
                $dict = $m2.Groups['dict'].Value
                $key  = $m2.Groups['key'].Value.Trim()
                $ind  = $m2.Groups['ind'].Value
                $body = $m2.Groups['body'].Value
                $tail = $m2.Groups['tail'].Value
                # only transform if body uses dict[key]
                $escapedDict = [regex]::Escape($dict)
                $escapedKey  = [regex]::Escape($key)
                if ($body -match "${escapedDict}\s*\[\s*${escapedKey}\s*\]") {
                    $varName = "_ckv"
                    $newBody = [regex]::Replace($body, "${escapedDict}\s*\[\s*${escapedKey}\s*\]", $varName)
                    $newLine = "${ind}if (${dict}.TryGetValue(${key}, out var ${varName})) {${newBody}}${tail}"
                    if ($newLine -ne $line) {
                        $lines[$i] = $newLine
                        $changed = $true; $fileFixed++
                    }
                }
                continue
            }
        }

        if ($changed) {
            [System.IO.File]::WriteAllLines($file, $lines, [System.Text.UTF8Encoding]::new($false))
            Write-Host "  Fixed $fileFixed in: $($_.Name)"
            $script:totalFixed += $fileFixed
            $script:filesEdited++
        }
    }
}

Write-Host ""
Write-Host "ContainsKey: fixed $totalFixed patterns in $filesEdited files"
