# Direct regex-based approach: Remove "static " from all field declarations
# Only in files identified by CodeQL

$wsRoot = "c:\git\FuseCPDevOPS-FuseCP"
Set-Location $wsRoot

# Files we need to fix  
$filesWithStaticFields = @(
    "FuseCP/Sources/FuseCP.WebPortal/DesktopModules/FuseCP/FileManager.ascx.cs",
    "FuseCP/Sources/FuseCP.WebPortal/Code/SecureSessionModule.cs",
    "FuseCP/Sources/FuseCP.Providers.Web.IIs80/SSL/SSLModuleService80.cs",
    "FuseCP/Sources/FuseCP.Providers.Web.IIs100/SSL/SSLModuleService100.cs",
    "FuseCP/Sources/FuseCP.Providers.Web.IIS70/SSL/SSLModuleService.cs",
    "FuseCP/Sources/FuseCP.Providers.Virtualization.HyperV-vmm/PowerShellManager.cs",
    "FuseCP/Sources/FuseCP.Providers.Virtualization.HyperV-2025/PowerShellManager.cs",
    "FuseCP/Sources/FuseCP.Providers.Virtualization.HyperV-2022/PowerShellManager.cs",
    "FuseCP/Sources/FuseCP.Providers.Virtualization.HyperV-2019/PowerShellManager.cs",
    "FuseCP/Sources/FuseCP.Providers.Virtualization.HyperV-2016/PowerShellManager.cs",
    "FuseCP/Sources/FuseCP.Providers.Virtualization.HyperV-2012R2/PowerShellManager.cs",
    "FuseCP/Sources/FuseCP.Providers.StorageSpaces.Windows2016/Windows2016.cs",
    "FuseCP/Sources/FuseCP.Providers.OS.Windows2025/Windows2025.cs",
    "FuseCP/Sources/FuseCP.Providers.OS.Windows2016/Windows2016.cs",
    "FuseCP/Sources/FuseCP.Providers.OS.Windows2022/Windows2022.cs",
    "FuseCP/Sources/FuseCP.Providers.OS.Windows2019/Windows2019.cs",
    "FuseCP/Sources/FuseCP.Providers.EnterpriseStorage.Windows2016/SyncShareService.cs",
    "FuseCP/Sources/FuseCP.Web.Services/TunnelHandler.cs",
    "FuseCP/Sources/FuseCP.Web.Clients/ClientAssemblyBase.cs",
    "FuseCP/Sources/FuseCP.Server.Utils/PowerShellManager.cs",
    "FuseCP/Sources/FuseCP.Server.Utils/LogParser/LogReader.cs",
    "FuseCP/Sources/FuseCP.Server/Code/ServerConfiguration.cs",
    "FuseCP/Sources/FuseCP.Providers.Base/OS/TunnelSockets/TunnelService.cs",
    "FuseCP/Sources/FuseCP.EnterpriseServer.Data/DbContext.cs",
    "FuseCP/Sources/FuseCP.EnterpriseServer.Code/Tasks/TaskManager.cs",
    "FuseCP/Sources/FuseCP.EnterpriseServer/Code/EnterpriseServerTunnelService.cs",
    "FuseCP/Sources/FuseCP.EnterpriseServer/WebServices/FuseCP.Build/EnterpriseServerProxyConfigurator.cs",
    "FuseCP/Sources/FuseCP.EnterpriseServer.Code/Data/DataProvider.cs",
    "FuseCP/Sources/FuseCP.Providers.HostedSolution.SfB2019/SfBBase.cs",
    "FuseCP/Sources/FuseCP.Providers.HostedSolution.SharePoint2016/HostedSharePointServer2016Impl.cs",
    "FuseCP/Sources/FuseCP.Providers.HostedSolution.SharePoint2016Ent/HostedSharePointServer2016EntImpl.cs",
    "FuseCP/Sources/FuseCP.Providers.HostedSolution.SharePoint2019/HostedSharePointServer2019Impl.cs",
    "FuseCP/Sources/FuseCP.Providers.HostedSolution/Exchange2010SP2.cs",
    "FuseCP/Sources/FuseCP.Providers.OS.Unix/Unix.cs"
)

Write-Host "Applying regex-based static field fixes to $($filesWithStaticFields.Count) files..." -ForegroundColor Green
$applied = 0

foreach ($relPath in $filesWithStaticFields) {
    $absPath = Join-Path $wsRoot $relPath
    
    if (-not (Test-Path $absPath)) {
        Write-Host "✗ File not found: $relPath" -ForegroundColor Yellow
        continue
    }
    
    $content = Get-Content $absPath -Raw
    
    # Replace: "static " (with proper word boundary) in field declarations  
    # Pattern: (private|protected|public|nothing) + white space + "static" + spaces + type + spaces + name +  = or ;
    $original = $content
    
    # Conservative pattern: only remove static in contexts that look like field declarations
    [System.Text.RegularExpressions.RegexOptions]$opts = 'Multiline'
    $content = [System.Text.RegularExpressions.Regex]::Replace($content, '(^\s*(?:private|protected|public)?)\s+static\s+(\w+\s+\w+\s*=)', '$1 $2', $opts)
    $content = [System.Text.RegularExpressions.Regex]::Replace($content, '(^\s*(?:private|protected|public)?)\s+static\s+(\w+\s+\w+\s*;)', '$1 $2', $opts)
    
    if ($content -ne $original) {
        Set-Content $absPath $content -Encoding UTF8 -NoNewline
        $applied++
        Write-Host "✓ $relPath" -ForegroundColor Green
    }
}

Write-Host ""
Write-Host "Total applied: $applied / $($filesWithStaticFields.Count)" -ForegroundColor Green
