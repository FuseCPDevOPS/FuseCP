$ErrorActionPreference = 'Stop'

$root = "C:\git\FuseCPDevOPS-FuseCP\FuseCP\Sources"
$files = Get-ChildItem -Recurse -Path $root -Include "*.cs" | Where-Object { $_.FullName -notmatch "\\(bin|obj|\.git)\\" }

$pattern = 'when \((?<v>[A-Za-z_]\w*) is not \(OutOfMemoryException or StackOverflowException or AccessViolationException\)\)'
$changedFiles = 0
$totalReplacements = 0
$skippedLocked = 0

foreach ($f in $files) {
    $content = [System.IO.File]::ReadAllText($f.FullName)
    $matches = [regex]::Matches($content, $pattern)
    if ($matches.Count -eq 0) { continue }

    $newContent = [regex]::Replace($content, $pattern, {
        param($m)
        $v = $m.Groups['v'].Value
        return "when (!($v is OutOfMemoryException) && !($v is StackOverflowException) && !($v is AccessViolationException))"
    })

    try {
        [System.IO.File]::WriteAllText($f.FullName, $newContent, [System.Text.UTF8Encoding]::new($false))
        $changedFiles++
        $totalReplacements += $matches.Count
    }
    catch [System.IO.IOException] {
        $skippedLocked++
        Write-Host "Skipped locked file: $($f.FullName)"
    }
}

Write-Host "Normalized $totalReplacements catch filters across $changedFiles files."
if ($skippedLocked -gt 0) {
    Write-Host "Skipped locked files: $skippedLocked"
}
