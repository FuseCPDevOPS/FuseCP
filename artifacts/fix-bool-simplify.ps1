$j = Get-Content "C:\git\FuseCPDevOPS-FuseCP\artifacts\codeql-open-alerts.json" | ConvertFrom-Json
$alerts = $j | Where-Object { $_.rule.id -eq "cs/simplifiable-boolean-expression" }
Write-Host "Alerts: $($alerts.Count)"

$byFile = $alerts | Group-Object { $_.most_recent_instance.location.path }
$totalFixed = 0

foreach ($group in ($byFile | Sort-Object Name)) {
    $relPath = $group.Name
    $absPath = "C:\git\FuseCPDevOPS-FuseCP\" + $relPath.Replace('/', '\')
    if (-not (Test-Path $absPath)) { Write-Host "MISSING: $relPath"; continue }

    $lines = [System.IO.File]::ReadAllLines($absPath)
    $changed = $false

    $sorted = $group.Group | Sort-Object { $_.most_recent_instance.location.start_line }, { $_.most_recent_instance.location.start_column } -Descending

    foreach ($alert in $sorted) {
        $loc = $alert.most_recent_instance.location
        $ln = $loc.start_line - 1
        $sc = [Math]::Max(0, $loc.start_column - 1)
        $ec = $loc.end_column - 1
        if ($ln -lt 0 -or $ln -ge $lines.Count) { continue }
        $line = $lines[$ln]
        if ($ec -gt $line.Length) { $ec = $line.Length }
        if ($sc -ge $ec) { continue }

        $span = $line.Substring($sc, $ec - $sc)
        $msg = $alert.most_recent_instance.message.text
        $fixed = $span

        if ($msg -match "'A == false'") {
            # Remove ' == false' suffix, prepend '!'
            if ($span -match '^(.+) == false$') { $fixed = "!($($Matches[1]))" }
        } elseif ($msg -match "'A != true'") {
            if ($span -match '^(.+) != true$') { $fixed = "!($($Matches[1]))" }
        } elseif ($msg -match "'A == true'") {
            if ($span -match '^(.+) == true$') { $fixed = $Matches[1] }
        } elseif ($msg -match "'A \? true : false'") {
            if ($span -match '^(.+?)\s*\?\s*true\s*:\s*false$') { $fixed = $Matches[1].Trim() }
        } elseif ($msg -match "'A \? false : true'") {
            if ($span -match '^(.+?)\s*\?\s*false\s*:\s*true$') { $fixed = "!(" + $Matches[1].Trim() + ")" }
        } elseif ($msg -match "'A \|\| false'") {
            if ($span -match '^(.+) \|\| false$') { $fixed = $Matches[1] }
        } elseif ($msg -match "'A && true'") {
            if ($span -match '^(.+) && true$') { $fixed = $Matches[1] }
        } elseif ($msg -match "'true \|\| A' is always 'true'") {
            $fixed = "true"
        } elseif ($msg -match "'!\(A == B\)'") {
            if ($span -match '^!\((.+) == (.+)\)$') { $fixed = "$($Matches[1]) != $($Matches[2])" }
        } elseif ($msg -match "'!\(A != B\)'") {
            if ($span -match '^!\((.+) != (.+)\)$') { $fixed = "$($Matches[1]) == $($Matches[2])" }
        } elseif ($msg -match "'A \? B : true'") {
            if ($span -match '^(.+?) \? (.+) : true$') { $fixed = "!($($Matches[1])) || $($Matches[2])" }
        } elseif ($msg -match "'A \? B : false'") {
            if ($span -match '^(.+?) \? (.+) : false$') { $fixed = "$($Matches[1]) && $($Matches[2])" }
        } elseif ($msg -match "'A \? false : B'") {
            if ($span -match '^(.+?) \? false : (.+)$') { $fixed = "!($($Matches[1])) && $($Matches[2])" }
        } elseif ($msg -match "'A \? true : B'") {
            if ($span -match '^(.+?) \? true : (.+)$') { $fixed = "$($Matches[1]) || $($Matches[2])" }
        }

        if ($fixed -ne $span) {
            $lines[$ln] = $line.Substring(0, $sc) + $fixed + $line.Substring($ec)
            $changed = $true; $totalFixed++
        } else {
            Write-Host "NOMATCH: [$span] MSG:$msg"
        }
    }

    if ($changed) {
        [System.IO.File]::WriteAllLines($absPath, $lines, [System.Text.UTF8Encoding]::new($false))
        Write-Host "Fixed $($group.Group.Count) in $($relPath.Split('/')[-1])"
    }
}
Write-Host "Total: $totalFixed"
