# Fix cs/static-field-written-by-instance alerts by removing "static" keyword
param([switch]$DryRun = $false)

$wsRoot = "c:\git\FuseCPDevOPS-FuseCP"
Set-Location $wsRoot

# Key files with their static field names and lines
$targetFiles = @(
    @{
        Path = "FuseCP/Sources/FuseCP.WebPortal/DesktopModules/FuseCP/RDS/UserControls/RDSCollectionUsers.ascx.cs"
        FieldName = "LocalAdmins"
        DeclLine = 32
    },
    @{
        Path = "FuseCP/Sources/FuseCP.WebPortal/Global.asax.cs"
        FieldName = "keepAliveUrl|timer"
        DeclLine = "42-43"
    },
    @{
        Path = "FuseCP/Sources/FuseCP.Providers.HostedSolution.Exchange2013/Exchange2013.cs"
        FieldName = "connectionInfo|ExchangePath"
        DeclLine = "6833-6834"
    },
    @{
        Path = "FuseCP/Sources/FuseCP.Providers.HostedSolution.Exchange2016/Exchange2016.cs"
        FieldName = "connectionInfo|ExchangePath"
        DeclLine = "6867-6868"
    },
    @{
        Path = "FuseCP/Sources/FuseCP.Providers.HostedSolution.Exchange2019/Exchange2019.cs"
        FieldName = "connectionInfo|ExchangePath"
        DeclLine = "6867-6868"
    }
)

$json = Get-Content 'artifacts/codeql-open-alerts-fresh.json' | ConvertFrom-Json
$staticAlerts = @($json | Where-Object { $_.rule.id -eq 'cs/static-field-written-by-instance' })

Write-Host "Processing $($staticAlerts.Count) static-field-written-by-instance alerts"
Write-Host ""

$filePathToAlerts = @{}
foreach ($alert in $staticAlerts) {
    $path = $alert.most_recent_instance.location.path
    if (-not $filePathToAlerts.ContainsKey($path)) {
        $filePathToAlerts[$path] = @()
    }
    $filePathToAlerts[$path] += $alert.most_recent_instance.location.start_line
}

$replacementList = @()
$fileCount = 0

foreach ($filePath in ($filePathToAlerts.Keys | Sort-Object)) {
    $absPath = Join-Path $wsRoot $filePath
    
    if (-not (Test-Path $absPath)) {
        Write-Warning "File not found: $absPath"
        continue
    }
    
    $fileContent = Get-Content $absPath -Raw
    $lines = @(Get-Content $absPath)
    
    # Find all "static" field declarations (typically " static " keyword in field declarations)
    $staticFieldPattern = '(^\s*(?:private|protected|public)?)\s+static\s+'
    
    $matches = [System.Text.RegularExpressions.Regex]::Matches($fileContent, $staticFieldPattern, 'Multiline')
    
    if ($matches.Count -gt 0) {
        Write-Host "$filePath : $($matches.Count) static field declarations" -ForegroundColor Cyan
        $fileCount++
        
        # Build replacement: replace each " static " with a single space
        $newContent = $fileContent -replace '(^\s*(?:private|protected|public)?)\s+static\s+', '$1 '
        
        if ($newContent -ne $fileContent) {
            if ($DryRun) {
                # Show diff lines
                $origLines = $fileContent -split "`n"
                $newLines = $newContent -split "`n"
                for ($i = 0; $i -lt [Math]::Min($origLines.Count, $newLines.Count); $i++) {
                    if ($origLines[$i] -ne $newLines[$i] -and $origLines[$i] -match 'static') {
                        Write-Host "  L$($i+1):" -ForegroundColor Gray
                        Write-Host "    - $($origLines[$i])" -ForegroundColor Red
                        Write-Host "    + $($newLines[$i])" -ForegroundColor Green
                    }
                }
            } else {
                Set-Content $absPath $newContent -Encoding UTF8 -NoNewline
                $replacementList += $filePath
            }
        }
    }
}

Write-Host ""
Write-Host "Processing complete. Updated $fileCount files." -ForegroundColor Green

if (-not $DryRun) {
    Write-Host "Modified files: $($replacementList.Count)" -ForegroundColor Green
    $replacementList | forEach { Write-Host "  ✓ $_" }
}
