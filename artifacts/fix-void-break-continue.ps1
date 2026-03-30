$base = "C:\git\FuseCPDevOPS-FuseCP\FuseCP\Sources"
$jsFiles = Get-ChildItem -Recurse -Path $base -Include "*.js" | Where-Object { $_.FullName -notmatch "\\(bin|obj|\.git|Build|Deploy)\\" }
$totalBreak = 0
$totalCont = 0
$changed = [System.Collections.Generic.List[string]]::new()

foreach ($f in $jsFiles) {
    $c = [System.IO.File]::ReadAllText($f.FullName)
    $nb = ([regex]::Matches($c, 'void break;')).Count
    $nc = ([regex]::Matches($c, 'void continue;')).Count
    if ($nb -gt 0 -or $nc -gt 0) {
        $c = $c.Replace('void break;', 'break;').Replace('void continue;', 'continue;')
        [System.IO.File]::WriteAllText($f.FullName, $c)
        $totalBreak += $nb
        $totalCont += $nc
        $changed.Add($f.Name)
    }
}

Write-Host "Fixed: $totalBreak void break, $totalCont void continue in $($changed.Count) files:"
$changed | ForEach-Object { Write-Host "  $_" }

# Also fix the spurious "void i;" line in dataTables.fixedColumns.js
$fixedColumnsPath = "C:\git\FuseCPDevOPS-FuseCP\FuseCP\Sources\FuseCP.WebDavPortal\Scripts\DataTables\dataTables.fixedColumns.js"
$fc = [System.IO.File]::ReadAllText($fixedColumnsPath)
if ($fc -match '\r?\n void i;') {
    $fc = $fc -replace '\r?\n void i;', ''
    [System.IO.File]::WriteAllText($fixedColumnsPath, $fc)
    Write-Host "Fixed spurious 'void i;' line in dataTables.fixedColumns.js"
} else {
    Write-Host "Pattern 'void i;' not found (or already fixed) in dataTables.fixedColumns.js"
}
