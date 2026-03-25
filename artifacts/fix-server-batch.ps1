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
$repoRoot=(Get-Location).Path

function Split-IfCondition([string]$line) {
    $trimmed = $line.TrimStart()
    if ($trimmed -notmatch '^if\s*\(') { return $null }
    $start = $trimmed.IndexOf('(')
    $depth = 0
    $end = -1
    for ($k = $start; $k -lt $trimmed.Length; $k++) {
        if ($trimmed[$k] -eq '(') { $depth++ }
        elseif ($trimmed[$k] -eq ')') { $depth--; if ($depth -eq 0) { $end = $k; break } }
    }
    if ($end -lt 0) { return $null }
    return @($trimmed.Substring($start+1, $end-$start-1).Trim(), $trimmed.Substring($end+1).Trim())
}

$totalCatch=0
$totalTernary=0
$totalUseless=0

# catch-of-all (single-line only)
$targets = $srv | Where-Object { $_.rule.id -eq 'cs/catch-of-all-exceptions' }
$byFile=@{}
foreach($a in $targets){
  $loc=$a.most_recent_instance.location
  if(-not $byFile.ContainsKey($loc.path)){ $byFile[$loc.path]=[System.Collections.Generic.List[int]]::new() }
  $byFile[$loc.path].Add([int]$loc.start_line)
}
foreach($path in $byFile.Keys){
  $abs = Join-Path $repoRoot ($path -replace '/','\\')
  if(-not (Test-Path $abs)){ continue }
  $lines=[System.IO.File]::ReadAllLines($abs)
  $changed=$false
  foreach($ln in ($byFile[$path] | Sort-Object -Unique)){
    $i=$ln-1
    if($i -lt 0 -or $i -ge $lines.Count){ continue }
    $line=$lines[$i]
    if($line -match '^\s*catch\s*\(\s*(?:System\.)?Exception(?:\s+(\w+))?\s*\)\s*(?!when\s*\()'){
      $var=$Matches[1]
      if([string]::IsNullOrWhiteSpace($var)){ $var='ex' }
      $replacement = "catch (Exception $var) when (!($var is OutOfMemoryException) && !($var is StackOverflowException) && !($var is AccessViolationException))"
      $new=[regex]::Replace($line,'catch\s*\(\s*(?:System\.)?Exception(?:\s+\w+)?\s*\)',[System.Text.RegularExpressions.MatchEvaluator]{ param($m) $replacement },1)
      if($new -ne $line){
        $lines[$i]=$new
        $changed=$true
        $totalCatch++
      }
    }
  }
  if($changed){ [System.IO.File]::WriteAllLines($abs,$lines,[System.Text.UTF8Encoding]::new($false)) }
}

# missed-ternary (simple safe cases)
$targets = $srv | Where-Object { $_.rule.id -eq 'cs/missed-ternary-operator' }
$byFile=@{}
foreach($a in $targets){
  $loc=$a.most_recent_instance.location
  if(-not $byFile.ContainsKey($loc.path)){ $byFile[$loc.path]=[System.Collections.Generic.List[int]]::new() }
  $byFile[$loc.path].Add([int]$loc.start_line)
}
foreach($path in $byFile.Keys){
  $abs=Join-Path $repoRoot ($path -replace '/','\\')
  if(-not (Test-Path $abs)){ continue }
  $lines=[System.IO.File]::ReadAllLines($abs)
  $changed=$false
  foreach($lineNum in ($byFile[$path] | Sort-Object -Descending -Unique)){
    $ln0=$lineNum-1
    if($ln0 -lt 0 -or $ln0 -ge $lines.Count){ continue }
    $ifLine=$lines[$ln0].TrimEnd()
    $trimmedIf=$ifLine.Trim()
    $indent=$ifLine.Length-$ifLine.TrimStart().Length
    $spaces=$ifLine.Substring(0,$indent)
    if(-not ($trimmedIf -match '^if\s*\(') -or $trimmedIf -match '\{'){ continue }
    $ifSplit=Split-IfCondition $trimmedIf
    if($ifSplit -eq $null){ continue }
    $cond=$ifSplit[0]
    $inlineBody=$ifSplit[1]

    if($inlineBody -ne ''){
      $elseLn=$ln0+1
      while($elseLn -lt $lines.Count -and $lines[$elseLn].Trim() -eq ''){ $elseLn++ }
      if($elseLn -ge $lines.Count){ continue }
      $trimElse=$lines[$elseLn].Trim()
      if(-not $trimElse.StartsWith('else') -or $trimElse.StartsWith('else if') -or $trimElse -match '\{'){ continue }

      $ifRet=[regex]::Match($inlineBody,'^return\s+(.+);\s*$')
      $elRet=[regex]::Match($trimElse,'^else\s+return\s+(.+);\s*$')
      if($ifRet.Success -and $elRet.Success){
        $t=$ifRet.Groups[1].Value
        $f=$elRet.Groups[1].Value
        if($t -notmatch '\?' -and $f -notmatch '\?'){
          $lines[$ln0]="${spaces}return $cond ? $t : $f;"
          $lines[$elseLn]=$null
          $changed=$true
          $totalTernary++
          continue
        }
      }

      $ifAs=[regex]::Match($inlineBody,'^(\w[\w.]*)\s*=\s*(.+);\s*$')
      $elAs=[regex]::Match($trimElse,'^else\s+(\w[\w.]*)\s*=\s*(.+);\s*$')
      if($ifAs.Success -and $elAs.Success -and $ifAs.Groups[1].Value -eq $elAs.Groups[1].Value){
        $v=$ifAs.Groups[1].Value
        $t=$ifAs.Groups[2].Value
        $f=$elAs.Groups[2].Value
        if($t -notmatch '\?' -and $f -notmatch '\?'){
          $lines[$ln0]="${spaces}$v = $cond ? $t : $f;"
          $lines[$elseLn]=$null
          $changed=$true
          $totalTernary++
          continue
        }
      }
    } else {
      $bodyLn=$ln0+1
      while($bodyLn -lt $lines.Count -and $lines[$bodyLn].Trim() -eq ''){ $bodyLn++ }
      if($bodyLn -ge $lines.Count){ continue }
      $body=$lines[$bodyLn].Trim()
      if($body -match '\{' -or $body.StartsWith('else')){ continue }

      $elseLn=$bodyLn+1
      while($elseLn -lt $lines.Count -and $lines[$elseLn].Trim() -eq ''){ $elseLn++ }
      if($elseLn -ge $lines.Count){ continue }
      $trimElse=$lines[$elseLn].Trim()
      if(-not $trimElse.StartsWith('else') -or $trimElse.StartsWith('else if') -or $trimElse -match '\{'){ continue }

      $elseBody=''
      $elseBodyLn=-1
      if($trimElse -match '^else\s+(.+)$'){
        $elseBody=$Matches[1].Trim()
        $elseBodyLn=$elseLn
      } else {
        $elseBodyLn=$elseLn+1
        while($elseBodyLn -lt $lines.Count -and $lines[$elseBodyLn].Trim() -eq ''){ $elseBodyLn++ }
        if($elseBodyLn -ge $lines.Count){ continue }
        $elseBody=$lines[$elseBodyLn].Trim()
      }
      if($elseBody -match '\{'){ continue }

      $ifRet=[regex]::Match($body,'^return\s+(.+);\s*$')
      $elRet=[regex]::Match($elseBody,'^return\s+(.+);\s*$')
      if($ifRet.Success -and $elRet.Success){
        $t=$ifRet.Groups[1].Value
        $f=$elRet.Groups[1].Value
        if($t -notmatch '\?' -and $f -notmatch '\?'){
          $lines[$ln0]="${spaces}return $cond ? $t : $f;"
          $lines[$bodyLn]=$null
          $lines[$elseLn]=$null
          if($elseBodyLn -ne $elseLn){ $lines[$elseBodyLn]=$null }
          $changed=$true
          $totalTernary++
          continue
        }
      }

      $ifAs=[regex]::Match($body,'^(\w[\w.]*)\s*=\s*(.+);\s*$')
      $elAs=[regex]::Match($elseBody,'^(\w[\w.]*)\s*=\s*(.+);\s*$')
      if($ifAs.Success -and $elAs.Success -and $ifAs.Groups[1].Value -eq $elAs.Groups[1].Value){
        $v=$ifAs.Groups[1].Value
        $t=$ifAs.Groups[2].Value
        $f=$elAs.Groups[2].Value
        if($t -notmatch '\?' -and $f -notmatch '\?'){
          $lines[$ln0]="${spaces}$v = $cond ? $t : $f;"
          $lines[$bodyLn]=$null
          $lines[$elseLn]=$null
          if($elseBodyLn -ne $elseLn){ $lines[$elseBodyLn]=$null }
          $changed=$true
          $totalTernary++
          continue
        }
      }
    }
  }
  if($changed){ [System.IO.File]::WriteAllLines($abs,($lines|Where-Object{$_ -ne $null}),[System.Text.UTF8Encoding]::new($false)) }
}

# useless-assignment (trivial standalone only)
$targets = $srv | Where-Object { $_.rule.id -eq 'cs/useless-assignment-to-local' }
$byFile=@{}
foreach($a in $targets){
  $loc=$a.most_recent_instance.location
  $abs=Join-Path $repoRoot ($loc.path -replace '/','\\')
  if(-not (Test-Path $abs)){ continue }
  $lines=[System.IO.File]::ReadAllLines($abs)
  $ln=[int]$loc.start_line-1
  if($ln -lt 0 -or $ln -ge $lines.Count){ continue }
  $msg=$a.most_recent_instance.message.text
  if($msg -notmatch 'assignment to (\w+) is useless'){ continue }
  $var=$Matches[1]
  $full=$lines[$ln].Trim()
  $re='^' + [regex]::Escape($var) + '\s*=\s*(null|false|true|0|""|-1)\s*;?\s*(//.*)?$'
  if($full -notmatch $re){ continue }
  if(-not $byFile.ContainsKey($abs)){ $byFile[$abs]=[System.Collections.Generic.List[int]]::new() }
  $byFile[$abs].Add($ln)
}
foreach($abs in $byFile.Keys){
  $list=[System.Collections.Generic.List[string]]([System.IO.File]::ReadAllLines($abs))
  foreach($ln in ($byFile[$abs] | Sort-Object -Descending -Unique)){
    if($ln -lt $list.Count){ $list.RemoveAt($ln); $totalUseless++ }
  }
  [System.IO.File]::WriteAllLines($abs,$list,[System.Text.UTF8Encoding]::new($false))
}

Write-Output "SERVER_CATCH_FIXED=$totalCatch"
Write-Output "SERVER_TERNARY_FIXED=$totalTernary"
Write-Output "SERVER_USELESS_ASSIGN_FIXED=$totalUseless"
Write-Output "SERVER_TOTAL_FIXED=$($totalCatch+$totalTernary+$totalUseless)"
