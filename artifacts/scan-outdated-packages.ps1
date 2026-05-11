# Scan for outdated packages across main FuseCP projects
$RepoRoot = Split-Path -Parent $PSScriptRoot
Set-Location $RepoRoot

Write-Host "=== Global NuGet packages (Directory.Build.props) ===" -ForegroundColor Cyan
$buildPropsPath = Join-Path $RepoRoot "Directory.Build.props"
[xml]$buildProps = Get-Content $buildPropsPath
$buildProps.SelectNodes('//PackageReference') | ForEach-Object {
    Write-Host "  {0,-40} {1}" -f $_.Include, $_.Version
}

Write-Host "`n=== Main project packages ===" -ForegroundColor Cyan

$keyProjects = @(
    "FuseCP.Installer\Sources\FuseCP.UniversalInstaller.Core\FuseCP.UniversalInstaller.Core.csproj",
    "FuseCP.Installer\Sources\FuseCP.UniversalInstaller\FuseCP.UniversalInstaller.csproj",
    "FuseCP\Sources\FuseCP.EnterpriseServer.Base\FuseCP.EnterpriseServer.Base.csproj"
)

foreach ($projFile in $keyProjects) {
    $fullPath = Join-Path $RepoRoot $projFile
    if (Test-Path $fullPath) {
        Write-Host "`n$projFile" -ForegroundColor Yellow
        [xml]$proj = Get-Content $fullPath
        $proj.SelectNodes('//PackageReference') | ForEach-Object {
            Write-Host "  {0,-40} {1}" -f $_.Include, $_.Version
        } | Sort-Object
    }
}

# Check for common outdated packages pattern
Write-Host "`n=== Known outdated packages ===" -ForegroundColor Cyan
$outdatedPatterns = @{
    "SharpCompress" = "0.40.0 -> 0.41.1";
    "Newtonsoft.Json" = "13.0.4 -> 13.0.4 (current)";
    "System.Security.Cryptography.Xml" = "10.0.6 -> 10.0.7 (or later)"
}

foreach ($pkg in $outdatedPatterns.Keys) {
    Write-Host "  $pkg : $($outdatedPatterns[$pkg])"
}
