$j = Get-Content "C:\git\FuseCPDevOPS-FuseCP\artifacts\codeql-open-alerts.json" | ConvertFrom-Json
$alerts = $j | Where-Object {
    $_.rule.id -eq "cs/useless-assignment-to-local" -and
    $_.most_recent_instance.location.path -notlike "*/obj/*" -and
    $_.most_recent_instance.location.path -notlike "*.g.cs"
}
Write-Host "Total alerts: $($alerts.Count)"

# Only target standalone lines where the useless assignment can be safely removed
# or converted back to an expression statement.
$byFile = @{}
foreach ($alert in $alerts) {
    $loc = $alert.most_recent_instance.location
    $absPath = "C:\git\FuseCPDevOPS-FuseCP\" + $loc.path.Replace('/','\')
    if (-not (Test-Path $absPath)) { continue }
    $lines = [System.IO.File]::ReadAllLines($absPath)
    $ln = $loc.start_line - 1
    if ($ln -ge $lines.Count) { continue }
    $msg = $alert.most_recent_instance.message.text
    $varName = if ($msg -match "assignment to (\w+) is useless") { $Matches[1] } else { continue }
    $fullLine = $lines[$ln].Trim()
    $indent = $lines[$ln].Substring(0, $lines[$ln].Length - $lines[$ln].TrimStart().Length)
    $replacement = $null

    $constAssign = '^' + [regex]::Escape($varName) + '\s*=\s*(null|false|true|0|""|-1)\s*;?\s*(//.*)?$'
    if ($fullLine -match $constAssign) {
        $replacement = ''
    }

    if ($null -eq $replacement) {
        $declRe = '^(?:var|[A-Za-z_][A-Za-z0-9_<>,\.\[\]\?\s]*)\s+' + [regex]::Escape($varName) + '\s*=\s*(.+);\s*(//.*)?$'
        if ($fullLine -match $declRe) {
            $rhs = $Matches[1].Trim()

            if ($rhs -match '^(await\s+)?[A-Za-z_][A-Za-z0-9_\.]*\s*\(.*\)$') {
                $replacement = $indent + $rhs + ';'
            } elseif ($rhs -match '^\([^)]+\)\s*((?:await\s+)?[A-Za-z_][A-Za-z0-9_\.]*\s*\(.*\))$') {
                $replacement = $indent + $Matches[1] + ';'
            } elseif ($rhs -notmatch '\(') {
                $replacement = ''
            }
        }
    }

    if ($null -eq $replacement) {
        $assignRe = '^' + [regex]::Escape($varName) + '\s*=\s*(.+);\s*(//.*)?$'
        if ($fullLine -match $assignRe) {
            $rhs = $Matches[1].Trim()

            if ($rhs -match '^(await\s+)?[A-Za-z_][A-Za-z0-9_\.]*\s*\(.*\)$') {
                $replacement = $indent + $rhs + ';'
            } elseif ($rhs -notmatch '\(') {
                $replacement = ''
            }
        }
    }

    if ($null -eq $replacement) { continue }

    # Group by file
    if (-not $byFile.ContainsKey($absPath)) { $byFile[$absPath] = [System.Collections.Generic.List[int]]::new() }
    $byFile[$absPath].Add($ln)
    if (-not $script:replacements) { $script:replacements = @{} }
    $script:replacements["$absPath::$ln"] = $replacement
}

Write-Host "Lines to remove: $($byFile.Values | ForEach-Object { $_.Count } | Measure-Object -Sum | Select-Object -ExpandProperty Sum)"

$totalFixed = 0
foreach ($kvp in $byFile.GetEnumerator()) {
    $absPath = $kvp.Key
    $lineNumbers = $kvp.Value | Sort-Object -Descending -Unique
    $lines = [System.Collections.Generic.List[string]]([System.IO.File]::ReadAllLines($absPath))
    foreach ($ln in $lineNumbers) {
        if ($ln -lt $lines.Count) {
            $key = "$absPath::$ln"
            $replacement = $script:replacements[$key]
            if ([string]::IsNullOrEmpty($replacement)) {
                $lines.RemoveAt($ln)
            } else {
                $lines[$ln] = $replacement
            }
            $totalFixed++
        }
    }
    [System.IO.File]::WriteAllLines($absPath, $lines, [System.Text.UTF8Encoding]::new($false))
    Write-Host "Removed $($kvp.Value.Count) lines in $($absPath.Split('\')[-1])"
}
Write-Host "Total: $totalFixed"
