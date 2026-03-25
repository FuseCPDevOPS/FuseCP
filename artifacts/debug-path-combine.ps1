Set-Location 'c:\git\FuseCPDevOPS-FuseCP'
$j = Get-Content 'artifacts/codeql-open-alerts.json' | ConvertFrom-Json
$pc = $j | Where-Object { $_.most_recent_instance.category -eq '/language:csharp-server' -and $_.rule.id -eq 'cs/path-combine' }
$pattern = [System.Text.RegularExpressions.Regex]::new('(.*?Path\.Combine\([^()]+,\s*)([A-Za-z_][A-Za-z0-9_.]*)(\s*\))')
$already = 0; $nomatch = 0; $samples = @()
foreach ($a in ($pc | Select-Object -First 80)) {
    $loc = $a.most_recent_instance.location
    $abs = Join-Path (Get-Location) ($loc.path -replace '/', '\')
    if (-not (Test-Path $abs)) { continue }
    $lines = [System.IO.File]::ReadAllLines($abs)
    $i = $loc.start_line - 1
    if ($i -lt 0 -or $i -ge $lines.Count) { continue }
    $line = $lines[$i]
    if ($line -match 'TrimStart\(Path\.Directory') { $already++; continue }
    if (-not $pattern.IsMatch($line)) { $nomatch++; $samples += "${($loc.path)}:$($loc.start_line): $($line.Trim())" }
}
Write-Output "ALREADY=$already NOMATCH=$nomatch"
$samples | Select-Object -First 20
