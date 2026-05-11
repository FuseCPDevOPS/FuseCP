# Update outdated NuGet packages in FuseCP projects
param(
    [switch]$WhatIf = $false
)

$RepoRoot = Split-Path -Parent $PSScriptRoot
Set-Location $RepoRoot

# Define packages to update with their new versions
# Format: @{ PackageName = "NewVersion" }
$PackagesToUpdate = @{
    "SharpCompress" = "0.41.1"
    "System.Security.Cryptography.Xml" = "10.0.7"
    "Mono.Cecil" = "0.11.7"
    "Octokit" = "14.0.1"
}

function Update-PackageInFile {
    param(
        [string]$FilePath,
        [hashtable]$PackagesToUpdate,
        [switch]$WhatIf
    )
    
    if (-not (Test-Path $FilePath)) {
        return
    }
    
    [xml]$xml = Get-Content $FilePath
    $changed = $false
    
    foreach ($pkg in $PackagesToUpdate.Keys) {
        $newVersion = $PackagesToUpdate[$pkg]
        $nodes = $xml.SelectNodes("//PackageReference[@Include='$pkg']")
        
        foreach ($node in $nodes) {
            $oldVersion = $node.Version
            if ($oldVersion -ne $newVersion) {
                Write-Host "Updating $pkg in $(Split-Path $FilePath -Leaf):"
                Write-Host "  $oldVersion -> $newVersion"
                
                if (-not $WhatIf) {
                    $node.Version = $newVersion
                    $changed = $true
                }
            }
        }
    }
    
    if ($changed -and -not $WhatIf) {
        $xml.Save($FilePath)
    }
}

# Update global packages
Write-Host "=== Updating Global Packages ===" -ForegroundColor Green
Update-PackageInFile -FilePath "Directory.Build.props" -PackagesToUpdate $PackagesToUpdate -WhatIf:$WhatIf

# Find and update all project files in main FuseCP sources
Write-Host "`n=== Updating Project Packages ===" -ForegroundColor Green

$projectFiles = @(
    "FuseCP.Installer\Sources\FuseCP.UniversalInstaller.Core\FuseCP.UniversalInstaller.Core.csproj",
    "FuseCP.Installer\Sources\FuseCP.UniversalInstaller\FuseCP.UniversalInstaller.csproj",
    "FuseCP\Sources\FuseCP.EnterpriseServer.Base\FuseCP.EnterpriseServer.Base.csproj",
    "FuseCP.HyperV.Utils\Sources\FuseCP.HyperV.Utils\FuseCP.HyperV.Utils.csproj",
    "FuseCP.VmConfig\Sources\FuseCP.VmConfig.Common\FuseCP.VmConfig.Common.csproj"
)

foreach ($projFile in $projectFiles) {
    $fullPath = Join-Path $RepoRoot $projFile
    Update-PackageInFile -FilePath $fullPath -PackagesToUpdate $PackagesToUpdate -WhatIf:$WhatIf
}

Write-Host "`nPackage update complete!" -ForegroundColor Green
if ($WhatIf) {
    Write-Host "This was a WhatIf run. Run without -WhatIf to apply changes." -ForegroundColor Yellow
}
