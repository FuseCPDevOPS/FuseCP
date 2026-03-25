$ErrorActionPreference='Stop'

$owner='FuseCPDevOPS'
$repo='FuseCP'

$page=1
$all=@()
while($true){
  $arr=(gh api "/repos/$owner/$repo/code-scanning/alerts?state=open&per_page=100&page=$page" | ConvertFrom-Json)
  if(-not $arr -or $arr.Count -eq 0){ break }
  $all += $arr
  if($arr.Count -lt 100){ break }
  $page++
}

$srv = $all | Where-Object { $_.most_recent_instance.category -eq '/language:csharp-server' }

$priority = @(
  'cs/path-combine',
  'cs/linq/missed-where',
  'cs/useless-assignment-to-local',
  'cs/missed-ternary-operator',
  'cs/catch-of-all-exceptions',
  'cs/nested-if-statements',
  'cs/linq/missed-select'
)

$selected = New-Object System.Collections.Generic.List[object]
foreach($rule in $priority){
  $bucket = $srv | Where-Object { $_.rule.id -eq $rule } | Sort-Object @{Expression={$_.number};Ascending=$true}
  foreach($a in $bucket){
    if($selected.Count -ge 1000){ break }
    $selected.Add([pscustomobject]@{
      number = $a.number
      rule = $a.rule.id
      path = $a.most_recent_instance.location.path
      line = $a.most_recent_instance.location.start_line
      category = $a.most_recent_instance.category
      url = $a.html_url
    }) | Out-Null
  }
  if($selected.Count -ge 1000){ break }
}

if($selected.Count -lt 1000){
  $rest = $srv | Where-Object { $priority -notcontains $_.rule.id } | Sort-Object @{Expression={$_.number};Ascending=$true}
  foreach($a in $rest){
    if($selected.Count -ge 1000){ break }
    $selected.Add([pscustomobject]@{
      number = $a.number
      rule = $a.rule.id
      path = $a.most_recent_instance.location.path
      line = $a.most_recent_instance.location.start_line
      category = $a.most_recent_instance.category
      url = $a.html_url
    }) | Out-Null
  }
}

$selected | ConvertTo-Json -Depth 5 | Out-File -FilePath artifacts/server-next-1000-match.json -Encoding utf8
$selected | Export-Csv -NoTypeInformation -Path artifacts/server-next-1000-match.csv -Encoding utf8

Write-Output "SERVER_OPEN_TOTAL=$($srv.Count)"
Write-Output "MATCH_SET_SIZE=$($selected.Count)"
$selected | Group-Object rule | Sort-Object Count -Descending | ForEach-Object { "{0}`t{1}" -f $_.Count,$_.Name }
