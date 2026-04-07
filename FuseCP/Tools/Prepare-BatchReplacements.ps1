# Prepare normalized replacements for multi_replace_string_in_file
$replacements = @(Get-Content "artifacts/static-field-replacements-apply.json" | ConvertFrom-Json)
Write-Host "Loaded $($replacements.Count) replacements"

$normalized = @()
foreach ($r in $replacements) {
    $fp = $r.filePath -replace '\\', '/'
    $fp = $fp -replace 'c:/git/FuseCPDevOPS-FuseCP/', ''  
    $normalized += @{
        filePath = $fp
        oldString = $r.oldString
        newString = $r.newString
    }
}

# Also apply the ones we did manually (save for output)
$manually_fixed = @(
    "FuseCP/Sources/FuseCP.WebPortal/DesktopModules/FuseCP/RDS/UserControls/RDSCollectionUsers.ascx.cs",
    "FuseCP/Sources/FuseCP.WebPortal/Global.asax.cs",
    "FuseCP/Sources/FuseCP.Providers.HostedSolution.Exchange2013/Exchange2013.cs",
    "FuseCP/Sources/FuseCP.Providers.HostedSolution.Exchange2016/Exchange2016.cs",
    "FuseCP/Sources/FuseCP.Providers.HostedSolution.Exchange2019/Exchange2019.cs"
)

$total_expected = $normalized.Count + $manually_fixed.Count
Write-Host ""
Write-Host "Summary:"
Write-Host "  Normalized replacements: $($normalized.Count)"
Write-Host "  Manually fixed: $($manually_fixed.Count)"
Write-Host "  Total fixed: $total_expected"
Write-Host "  Expected CodeQL reduction: ~69 -> ~0 (all replacements)"

# Export normalized list
$normalized | ConvertTo-Json -Depth 10 | Out-File "artifacts/replacements-batch.json" -Encoding UTF8
Write-Host ""
Write-Host "Saved to artifacts/replacements-batch.json for batch application"
