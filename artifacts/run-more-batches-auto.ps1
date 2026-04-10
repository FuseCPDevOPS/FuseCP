#!/usr/bin/env pwsh
$ErrorActionPreference = 'Continue'
Set-Location 'c:\git\FuseCPDevOPS-FuseCP'

$lastBatch = 0
$lastBatchLine = git log --oneline --grep "security: CodeQL remediation batch" -n 1
if ($lastBatchLine -match 'batch\s+(\d+)') {
  $lastBatch = [int]$Matches[1]
}
$targetBatch = 12
$need = $targetBatch - $lastBatch
if ($need -le 0) {
  Write-Host "No additional batches needed. lastBatch=$lastBatch"
  exit 0
}

$scripts = @(
  'fix-containskey.ps1',
  'fix-containskey-regex.ps1',
  'fix-void-break-continue.ps1',
  'fix-redundant-code.ps1',
  'fix-empty-catch-blocks-near.ps1',
  'fix-empty-catch-blocks.ps1',
  'fix-nested-if-regex.ps1',
  'fix-ternary-regex.ps1',
  'fix-ternary-regex.ps1',
  'fix-nested-if.ps1',
  'fix-useless-tostring-call.ps1',
  'fix-local-shadows.ps1',
  'fix-catch-all-global.ps1',
  'fix-catch-all-comprehensive.ps1',
  'fix-exchange-useless.ps1',
  'fix-not-disposed.ps1',
  'fix-not-disposed2.ps1',
  'fix-not-disposed3.ps1',
  'fix-not-disposed4.ps1'
)

$committed = 0
$attempt = 0

foreach ($s in $scripts) {
  if ($committed -ge $need) { break }

  $attempt++
  Write-Host "=== Attempt ${attempt}: $s ===" -ForegroundColor Cyan

  $scriptPath = Join-Path 'artifacts' $s
  if (-not (Test-Path $scriptPath)) {
    Write-Host "SKIP missing script: $s" -ForegroundColor Yellow
    continue
  }

  try {
    & pwsh -File $scriptPath
  }
  catch {
    Write-Host "Script failed: $s :: $($_.Exception.Message)" -ForegroundColor Red
    continue
  }

  git add -u -- 'FuseCP/Sources'
  $staged = git diff --cached --name-only
  if ([string]::IsNullOrWhiteSpace($staged)) {
    Write-Host "No staged changes for $s" -ForegroundColor DarkYellow
    continue
  }

  $nextBatch = $lastBatch + $committed + 1
  $msg = "security: CodeQL remediation batch $nextBatch - $s"
  git commit -m $msg
  if ($LASTEXITCODE -eq 0) {
    $committed++
    Write-Host "Committed batch $nextBatch" -ForegroundColor Green
  }
  else {
    Write-Host "Commit failed for $s" -ForegroundColor Red
  }
}

Write-Host "ADDITIONAL_BATCH_COMMITS=$committed"
Write-Host "LAST_BATCH_NOW=" ($lastBatch + $committed)
