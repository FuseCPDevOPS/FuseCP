#!/usr/bin/env pwsh
# fix-bool-regex.ps1
# Regex-based scan of all .cs files to fix cs/simplifiable-boolean-expression patterns.
# Does NOT rely on stale alert line/column positions.

param(
    [string[]]$Roots = @(
        "FuseCP\Sources\FuseCP.WebPortal",
        "FuseCP\Sources\FuseCP.WebDavPortal",
        "FuseCP\Sources\FuseCP.EnterpriseServer",
        "FuseCP\Sources\FuseCP.Providers.HostedSolution.Exchange2013",
        "FuseCP\Sources\FuseCP.Providers.HostedSolution.Exchange2016",
        "FuseCP\Sources\FuseCP.Providers.HostedSolution.Exchange2019",
        "FuseCP\Sources\FuseCP.Providers.Mail.SmarterMail2",
        "FuseCP\Sources\FuseCP.Server"
    )
)

$repoRoot = Split-Path $PSScriptRoot
$totalFixed = 0
$filesEdited = 0

# Valid tokens that can appear after the expression to ensure it's a boolean context
$boolContextAfter = '(?=\s*[;)\|&,]|\s*&&|\s*\|\||\s*$)'

# Simple expression: identifiers + dots + [] + () — NO operators like == != < >
$simpleExpr = '(?:!?\s*)?(?:[a-zA-Z_]\w*(?:\.[a-zA-Z_]\w*)*(?:\[\w+\])*(?:\([^()]*\))?)'

$patterns = @(
    # A == true  →  A
    @{
        Regex   = [regex]'(\b(?:[a-zA-Z_]\w*(?:\.[a-zA-Z_]\w*)*(?:\[[^\]]*\])*(?:\([^()]*\))?))(\s*==\s*true)(?=\s*[;)\|&,?:]|\s*&&|\s*\|\|)'
        Replace = { param($m) $m.Groups[1].Value }
        Name    = '== true'
    },
    # A != false  →  A
    @{
        Regex   = [regex]'(\b(?:[a-zA-Z_]\w*(?:\.[a-zA-Z_]\w*)*(?:\[[^\]]*\])*(?:\([^()]*\))?))(\s*!=\s*false)(?=\s*[;)\|&,?:]|\s*&&|\s*\|\|)'
        Replace = { param($m) $m.Groups[1].Value }
        Name    = '!= false'
    },
    # A == false  →  !A    (simple identifier/property only — no leading !)
    @{
        Regex   = [regex]'(\b(?:[a-zA-Z_]\w*(?:\.[a-zA-Z_]\w*)*(?:\[[^\]]*\])*(?:\([^()]*\))?))(\s*==\s*false)(?=\s*[;)\|&,?:]|\s*&&|\s*\|\|)'
        Replace = { param($m) '!' + $m.Groups[1].Value }
        Name    = '== false'
    },
    # A != true  →  !A
    @{
        Regex   = [regex]'(\b(?:[a-zA-Z_]\w*(?:\.[a-zA-Z_]\w*)*(?:\[[^\]]*\])*(?:\([^()]*\))?))(\s*!=\s*true)(?=\s*[;)\|&,?:]|\s*&&|\s*\|\|)'
        Replace = { param($m) '!' + $m.Groups[1].Value }
        Name    = '!= true'
    }
)

foreach ($relRoot in $Roots) {
    $absRoot = Join-Path $repoRoot $relRoot
    if (-not (Test-Path $absRoot)) { continue }

    Get-ChildItem -Recurse -Filter "*.cs" $absRoot | Where-Object {
        $_.FullName -notlike "*\obj\*" -and
        $_.FullName -notlike "*\.g.cs" -and
        $_.FullName -notlike "*\bin\*"
    } | ForEach-Object {
        $file = $_.FullName
        $content = [System.IO.File]::ReadAllText($file)
        $original = $content

        foreach ($pat in $patterns) {
            $content = $pat.Regex.Replace($content, $pat.Replace)
        }

        if ($content -ne $original) {
            [System.IO.File]::WriteAllText($file, $content, [System.Text.UTF8Encoding]::new($false))
            $changed = ($original -replace '\r','').Split("`n").Count - ($content -replace '\r','').Split("`n").Count
            Write-Host "  Fixed: $($_.Name)"
            $script:filesEdited++
            # Count replacements
            $matchesBefore = 0
            foreach ($pat in $patterns) { $matchesBefore += $pat.Regex.Matches($original).Count }
            $matchesAfter = 0
            foreach ($pat in $patterns) { $matchesAfter += $pat.Regex.Matches($content).Count }
            $script:totalFixed += ($matchesBefore - $matchesAfter)
        }
    }
}

Write-Host ""
Write-Host "Boolean simplification: fixed patterns in $filesEdited files (~$totalFixed changes)"
