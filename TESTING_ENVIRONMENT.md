# FuseCP Testing Environment

This page describes what is needed to run local builds, tests, and packaging
reliably.

## 1) Baseline Requirements

* Windows development environment
* PowerShell (`pwsh` preferred, `powershell` supported)
* .NET SDK (CI currently uses .NET 10)
* Visual Studio / MSBuild tooling (include Web Build Tools workload)
* Repository submodules initialized:
  * `git submodule update --init --recursive`

## 1.1) Fastest First-Time Setup (Recommended)

Open **elevated** PowerShell in repo root and run:

* `powershell -File FuseCP/Tools/bootstrap-test-environment.ps1 -Install -RunAllProfiles -InstallSqlExpress -InstallIIS -InstallWsl -InstallWixToolset`

If Linux package generation is not needed, omit `-InstallWsl`.
If legacy installer packaging is not needed, omit `-InstallWixToolset`.

## 2) Required by Scenario

### Unit and solution tests

* `dotnet` CLI available
* Build/test solutions under `FuseCP/Sources`

### Legacy integration-style local test setup (`FuseCP/Tools/*.bat`)

* IIS + PowerShell `WebAdministration` module
* Local SQL Server instance reachable as `(local)\SQLExpress`
* SQL command-line tooling (repo-bundled at `tools/sqlcmd/SQLCMD.EXE`)
* Microsoft ODBC Driver for SQL Server (required by `SQLCMD.EXE`)
* Run terminal as Administrator for setup tasks (IIS site creation/scripts)

Note: `WebAdministration` is a Windows PowerShell module; the checker validates
it through `powershell.exe` even when invoked from `pwsh`.

Note: `Administrator shell` is reported as warning-only during checks; it is
required when executing setup actions that modify IIS/system configuration.

### Full deploy/package build

* WiX tooling (repo-bundled at `tools/WIX`)
* Web Deploy tooling (repo-bundled at `tools/WebDeploy/msdeploy.exe`)
* 7-Zip tooling (repo-bundled at `tools/7-Zip/7z.exe`)
* SQL command-line tooling (repo-bundled at `tools/sqlcmd/SQLCMD.EXE`)
* MSBuild web targets (`Microsoft.WebApplication.targets`) via Visual Studio
  Web Build Tools workload
* Full Visual Studio + Installer Projects extension only if building legacy
  `.vdproj` MSI output
* WiX MSBuild targets (`Wix.CA.targets`) for legacy installer projects

### Linux package generation (optional but enabled in release build defaults)

* WSL2
* Linux distro with `rpmbuild`

If WSL/rpm is not available locally, you can disable Linux package creation by
setting `BuildLinuxInstallPackages=false` in your build invocation.

## 3) Quick Verification Commands

From repo root:

* `dotnet --info`
* `cd FuseCP/Sources`
* `dotnet build FuseCP.Server.sln`
* `dotnet build FuseCP.EnterpriseServer.sln`
* `dotnet build FuseCP.Tests.sln`
* `dotnet test FuseCP.Tests.sln --configuration Release --no-build -v n`

From `FuseCP` folder for full build/package flow:

* `build-release.bat`

## 4) Environment Check Script

Run the helper script to check common prerequisites:

* `[pwsh|powershell] -File FuseCP/Tools/check-test-environment.ps1`
* `[pwsh|powershell] -File FuseCP/Tools/check-test-environment.ps1 -Profile Integration`
* `[pwsh|powershell] -File FuseCP/Tools/check-test-environment.ps1 -Profile Package`
* `[pwsh|powershell] -File FuseCP/Tools/check-test-environment.ps1 -Profile Full`
* `[pwsh|powershell] -File FuseCP/Tools/check-test-environment.ps1 -Profile Package -RequireLegacyMsi`

Profiles:

* `Unit`: PowerShell + dotnet + msbuild
* `Integration`: Unit + IIS/WebAdministration + SQLExpress reachability
* `Package`: Unit + bundled WiX/WebDeploy/7-Zip/sqlcmd + WSL check
* `Full`: Integration + Package

`-RequireLegacyMsi` adds a hard check for full Visual Studio (`devenv.com`) for
legacy `.vdproj` MSI packaging.

## 5) Streamlined Local Validation

Use a single command wrapper for local validation:

* `[pwsh|powershell] -File FuseCP/Tools/run-local-validation.ps1`
* `[pwsh|powershell] -File FuseCP/Tools/run-local-validation.ps1 -Scope Portal,Enterprise`
* `[pwsh|powershell] -File FuseCP/Tools/run-local-validation.ps1 -Scope Server -IncludeTests`
* `[pwsh|powershell] -File FuseCP/Tools/run-local-validation.ps1 -ChangedOnly`
* `[pwsh|powershell] -File FuseCP/Tools/run-local-validation.ps1 -ChangedOnly -JsonOutputPath artifacts/validation/summary.json`
* `[pwsh|powershell] -File FuseCP/Tools/run-local-validation.ps1 -ChangedOnly -DisableNuGetAudit`
* `[pwsh|powershell] -File FuseCP/Tools/run-local-validation.ps1 -ChangedOnly -SkipIfNoChanges`
* `[pwsh|powershell] -File FuseCP/Tools/run-local-validation.ps1 -ChangedOnly -ScopeMapPath FuseCP/Tools/validation-scope-map.json`
* `[pwsh|powershell] -File FuseCP/Tools/check-sln-scope-sync.ps1`

Notes:

* Default scope is `Shared`, which runs orchestrated `build.xml`.
* `-ChangedOnly` derives scope from changed files (via git) and selects the
  smallest safe validation set.
* `-JsonOutputPath` writes a machine-readable run summary (status, scope,
  changed files, and executed commands).
* `-DisableNuGetAudit` reduces noisy `NU190x` audit warnings for local loops
  without changing project files.
* `-SkipIfNoChanges` exits successfully without building when `-ChangedOnly`
  finds no touched files.
* `-ScopeMapPath` optionally extends scope routing from a JSON file.
  See `FuseCP/Tools/validation-scope-map.example.json` for schema and examples.
* Add `-UseOrchestratedBuild` to force ordered build for any scope.
* For broad changes, prefer the orchestrated path.

## 5.1) Project Relationship Guardrail (Required for project graph changes)

When a change adds/removes/renames a project, or modifies `ProjectReference`
relationships, validate solution relationships in the same PR:

* Keep `FuseCP.sln` synchronized with scoped build solutions:
  * `FuseCP/Sources/FuseCP.WebPortal.sln`
  * `FuseCP/Sources/FuseCP.EnterpriseServer.sln`
  * `FuseCP/Sources/FuseCP.Server.sln`
* If a project participates in build scope, update both the scoped `.sln` and
  `FuseCP.sln` together.
* Validate affected scopes with `run-local-validation.ps1` (minimum affected
  scope, then broaden when uncertain).
* For broad dependency graph changes, run orchestrated validation:
  * `[pwsh|powershell] -File FuseCP/Tools/run-local-validation.ps1 -Scope Shared`
* Include a short note in PR validation output confirming solution-sync checks.
* Use `check-sln-scope-sync.ps1 -WriteReport` when attaching machine-readable
  solution inclusion evidence under `artifacts/`.

## 6) Bootstrap Script (Install + Check)

Use this when setting up a fresh machine.

Check-only (no installs):

* `[pwsh|powershell] -File FuseCP/Tools/bootstrap-test-environment.ps1`
* `[pwsh|powershell] -File FuseCP/Tools/bootstrap-test-environment.ps1 -RunAllProfiles`

Install + check (run elevated PowerShell):

* `[pwsh|powershell] -File FuseCP/Tools/bootstrap-test-environment.ps1 -Install -RunAllProfiles`
* `[pwsh|powershell] -File FuseCP/Tools/bootstrap-test-environment.ps1 -Install -RunAllProfiles -InstallSqlExpress`
* `[pwsh|powershell] -File FuseCP/Tools/bootstrap-test-environment.ps1 -Install -RunAllProfiles -InstallSqlExpress -InstallIIS`

Optional flags:

* `-SkipPwsh` skips `PowerShell 7 (pwsh)` installation
* `-SkipDotNet` skips `.NET SDK` installation
* `-SkipBuildTools` skips `Visual Studio Build Tools` installation
* `-SkipSqlOdbc` skips `Microsoft ODBC Driver for SQL Server`
* `-InstallWsl` installs `Microsoft.WSL` and a distro package
* `-WslDistroId` selects distro package ID (default: `Canonical.Ubuntu.2204`)
* `-InstallWixToolset` installs `WiX Toolset v3` package
* `-InstallSqlExpress` includes local SQL Server 2022 Express install
* `-InstallIIS` enables IIS Web Server role + IIS Management Scripting Tools
* `-SqlExpressOverride` sets custom SQL Express installer arguments
* `-Profiles Unit,Package` selects specific profiles

Notes:

* Bootstrap installs `PowerShell 7 (pwsh)` first so subsequent script-based checks can run consistently.
* SQL Server Express installation can invoke an interactive installer and may be
  cancelled if the session cannot present installer prompts.
* Bootstrap now retries SQL Express installation with a direct ENU installer
  fallback if winget returns language error `1009`.
* Bootstrap also repairs/re-caches `msodbcsql.msi` before SQL Express install to
  avoid MSI source error `1706`.
* You can still customize arguments with `-SqlExpressOverride "/ENU /Q /IACCEPTSQLSERVERLICENSETERMS"`.
* Integration profile expects instance `(local)\SQLExpress` to be reachable.
* If orchestrated build fails with missing
  `Microsoft.WebApplication.targets`, modify Build Tools installation to include
  `Microsoft.VisualStudio.Workload.WebBuildTools`.
* If restore/build reports missing `Wix.CA.targets`, install Visual Studio WiX
  build integration (WiX v3 build tools/extension) so
  `MSBuild\Microsoft\WiX\v3.x\Wix.CA.targets` is present.
* `-InstallWixToolset` installs the WiX toolset package, but legacy installer
  projects may still require Visual Studio WiX MSBuild integration to provide
  `Wix.CA.targets` in MSBuild paths.
* Installer projects now include fallback lookup for WiX v3 SDK targets under
  `C:\Program Files (x86)\WiX Toolset v3.14\SDK\wix.ca.targets` (and v3.11)
  when Visual Studio WiX targets path is unavailable.
* If you are on Build Tools only, legacy MSI packaging is skipped automatically.
  To force MSI build, use `/p:BuildInstallerMsi=true` and ensure `devenv.com`
  plus Installer Projects are installed.

## 7) Local Dev Service Control (Reduce Background Usage)

Legacy local test flow creates IIS websites named:

* `FuseCP Portal` (9001)
* `FuseCP Enterprise Server` (9002)
* `FuseCP Server` (9003)

Start/stop helpers:

* `FuseCP/Tools/start-test.bat` (opens portal and starts known IIS sites)
* `FuseCP/Tools/stop-test.bat` (stops known IIS sites)
* `FuseCP/Tools/set-dev-services-manual-start.bat` (one-time setup to keep SQLExpress + IIS from auto-starting at boot)
* VS Code task: `Done for today` (runs `FuseCP/Tools/Done-For-Today.ps1`)
* VS Code task: `Closing VS` (runs `FuseCP/Tools/Closing-VS.ps1`)

Optional extra shutdown when not actively developing:

* Stop SQL Express service: `powershell -Command "Stop-Service -Name 'MSSQL$SQLEXPRESS'"`
* Shutdown WSL VM(s): `wsl --shutdown`

Optional startup when returning to integration work:

* Start SQL Express service: `powershell -Command "Start-Service -Name 'MSSQL$SQLEXPRESS'"`
* Run start-of-day checks (environment + solution sync):
  `powershell -NoProfile -ExecutionPolicy Bypass -File "FuseCP/Tools/Start-Of-Day.ps1"`

One-time policy (recommended if machine is not dev-only):

* Run elevated once: `powershell -File FuseCP/Tools/SetDevServicesManualStart.ps1 -StopNow`
* This sets `MSSQL$SQLEXPRESS` and `W3SVC` to `Manual` startup and optionally stops them now.

Session handoff notes:

* The `Done for today` task writes a handoff file to `artifacts/session-notes/<timestamp>.md`
* Use it next session to resume from branch, local changes, and service state.
* VS Code task `Start of day` runs `FuseCP/Tools/Start-Of-Day.ps1`.

