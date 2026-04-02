$ErrorActionPreference = 'Continue'

$owner = 'FuseCPDevOPS'
$repo = 'FuseCP'

$all = @()
$page = 1
while ($true) {
    $arr = gh api "/repos/$owner/$repo/code-scanning/alerts?state=open&per_page=100&page=$page" | ConvertFrom-Json
    if (-not $arr -or $arr.Count -eq 0) { break }
    $all += $arr
    if ($arr.Count -lt 100) { break }
    $page++
}

$vendor = $all | Where-Object {
    $_.most_recent_instance.location.path -match 'jquery\.dataTables\.js|jquery\.window\.js|/Scripts/|/App_Themes/.*/js/|/addons/.*/js/'
}
$generated = $all | Where-Object {
    $_.most_recent_instance.location.path -match '\.Designer\.cs$|\.g\.cs$|/obj/'
}

$candidates = @($vendor + $generated | Sort-Object number -Unique)
Write-Host "CANDIDATES=$($candidates.Count)"

$dismissed = 0
$failed = 0

foreach ($a in $candidates) {
    $num = $a.number
    $path = $a.most_recent_instance.location.path
    $comment = "Bulk triage: vendor/minified or generated artifact."

    try {
        gh api --method PATCH "/repos/$owner/$repo/code-scanning/alerts/$num" --field state=dismissed --field dismissed_reason="won't fix" --field dismissed_comment="$comment" | Out-Null
        $dismissed++
    }
    catch {
        $failed++
    }
}

Write-Host "DISMISSED=$dismissed"
Write-Host "FAILED=$failed"
