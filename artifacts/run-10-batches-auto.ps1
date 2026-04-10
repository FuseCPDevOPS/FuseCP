#!/usr/bin/env pwsh
$ErrorActionPreference = 'Continue'
Set-Location 'c:\git\FuseCPDevOPS-FuseCP'

$scripts = @(
  'fix-constant-condition-safe.ps1',
  'fix-useless-assign-cs-safe.ps1',
  'fix-js-superfluous-args-safe.ps1',
  'fix-js-useless-expression-safe.ps1',
  'fix-cookie-hardening-safe.ps1',
  'fix-js-missing-var.ps1',
  'fix-js-constant-span-alerts.ps1',
  'fix-null-deref-forgiving-span.ps1',
  'fix-bool-simplify.ps1',
  'fix-path-combine.ps1',
  'fix-bool-regex.ps1',
  'fix-bool-regex.ps1'
)

$batchCommitted = 0
$attempt = 0

foreach ($s in $scripts) {
  if ($batchCommitted -ge 10) { break }

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

  $nextBatch = 2 + $batchCommitted + 1
  $msg = "security: CodeQL remediation batch $nextBatch - $s"
  git commit -m $msg
  if ($LASTEXITCODE -eq 0) {
    $batchCommitted++
    Write-Host "Committed batch $nextBatch" -ForegroundColor Green
  }
  else {
    Write-Host "Commit failed for $s" -ForegroundColor Red
  }
}

Write-Host "BATCH_COMMITS=$batchCommitted"
