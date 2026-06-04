function Test-IsAdministrator {
	$identity = [Security.Principal.WindowsIdentity]::GetCurrent()
	$principal = New-Object Security.Principal.WindowsPrincipal($identity)
	return $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
}

function Resolve-PowerShellCommand {
	$pwshCommand = Get-Command pwsh -ErrorAction SilentlyContinue
	if ($null -ne $pwshCommand) {
		return "pwsh"
	}

	$windowsPowerShellCommand = Get-Command powershell -ErrorAction SilentlyContinue
	if ($null -ne $windowsPowerShellCommand) {
		return "powershell"
	}

	throw "Neither 'pwsh' nor 'powershell' was found in PATH."
}

function Ensure-ElevatedSession {
	if (Test-IsAdministrator) {
		return
	}

	if ($env:FUSECP_START_WEBSITE_ELEVATED -eq "1") {
		return
	}

	$args = @(
		"-NoProfile",
		"-ExecutionPolicy", "Bypass",
		"-File", $PSCommandPath
	)

	Write-Host "Current shell is not elevated. Requesting Administrator permissions to manage IIS websites..." -ForegroundColor Yellow

	try {
		$childProcess = Start-Process -FilePath $script:PowerShellCommand -ArgumentList $args -Verb RunAs -Wait -PassThru -WorkingDirectory (Get-Location) -Environment @{ FUSECP_START_WEBSITE_ELEVATED = "1" }
		exit $childProcess.ExitCode
	}
	catch {
		throw "Unable to start elevated shell. Please approve UAC prompt or run an Administrator shell."
	}
}

$script:PowerShellCommand = Resolve-PowerShellCommand

Ensure-ElevatedSession

Write-Host -ForegroundColor Green "
Ensure that you have created the EnterpriseServer Database
by executing test-createDB.bat and compiled FuseCP by
executing build-test.bat .

Configuration:

FuseCP Portal:

URL: http://localhost:9001
Login: serveradmin
Password: 1234


FuseCP Enterprise Server:
URL: http://127.0.0.1:9002
Database Login: FuseCP
Database Password: Password12


FuseCP Server:
URL: http://localhost:9003
Password: Password12
"

try {
	Import-Module WebAdministration -ErrorAction Stop

	$siteNames = @(
		"FuseCP Portal",
		"FuseCP Enterprise Server",
		"FuseCP Server"
	)

	foreach ($siteName in $siteNames) {
		$sitePath = "IIS:\Sites\$siteName"
		if (Test-Path $sitePath) {
			$siteState = (Get-Website -Name $siteName).State
			if ($siteState -ne "Started") {
				Start-Website -Name $siteName
				Write-Host "Started IIS website: $siteName" -ForegroundColor Yellow
			}
			else {
				Write-Host "IIS website already started: $siteName" -ForegroundColor DarkYellow
			}
		}
		else {
			Write-Host "IIS website not found: $siteName" -ForegroundColor DarkYellow
		}
	}
}
catch {
	Write-Host "Unable to manage IIS websites. Run as Administrator and ensure IIS + WebAdministration are installed." -ForegroundColor Red
	Write-Host $_.Exception.Message -ForegroundColor Red
}

Start-Process "http://localhost:9001"

Read-Host "Press a key"


	
	
	
