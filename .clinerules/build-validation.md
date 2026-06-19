# Builds / Tests / Validation Workflow

## Start of Day
- Run `pwsh -File FuseCP/Tools/Start-Of-Day.ps1` before feature work
- If docs-only or redundant, at minimum run `pwsh -File FuseCP/Tools/check-sln-scope-sync.ps1`

## Validation Strategy
- Use **smallest relevant scope first**:
  - Portal: `-Scope Portal`
  - Enterprise: `-Scope Enterprise`
  - Server: `-Scope Server`
- Scope note: `Portal` already builds `FuseCP.WebPortalAndEnterpriseServer.sln`. If both `Portal` and `Enterprise` selected, validation skips redundant Enterprise-only build automatically.
- For repeated local loops after restore: `pwsh -File FuseCP/Tools/run-local-validation.ps1 -Scope Enterprise -NoRestore`
- For broad validation: `pwsh -File FuseCP/Tools/run-local-validation.ps1 -Scope Shared`

## Solution Sync Rule
- Keep `FuseCP.sln` synchronized with `FuseCP/Sources/FuseCP.WebPortal.sln`, `FuseCP/Sources/FuseCP.EnterpriseServer.sln`, and `FuseCP/Sources/FuseCP.Server.sln` for project add/remove/rename changes.
- If `*.sln`, `*.csproj`, `*.vbproj`, `*.vcxproj`, or `*.shproj` files are touched, include explicit solution-sync verification in PR notes.
- Run `pwsh -File FuseCP/Tools/check-sln-scope-sync.ps1` to enforce synchronization.

## Build Orchestration
- Prefer orchestrated builds (`build.xml`, `build-debug.bat`, `build-release.bat`, `deploy-*.bat`) over independent `.sln` builds for end-to-end validation.
- If `FuseCP.Server`, `FuseCP.WebPortal`, or `DesktopModules/FuseCP` build fails due to `w3wp` locking `bin_dotnet`, stop IIS worker processes first.
- Use `FuseCP/Tools/Unlock-WebPortal-Build.ps1` when rebuilding portal modules; it stops `w3wp` and can rerun with `-RunBuild`.

## Validation Options
- `-ChangedOnly`: fast iteration when path-based scope can be inferred safely
- `-SkipIfNoChanges`: avoid unnecessary builds when no files are touched
- `-DisableNuGetAudit`: reduce local warning noise during iteration only
- `-JsonOutputPath`: machine-readable output for PR tooling
- `-ScopeMapPath`: extend path-to-scope routing from JSON

## Warning Reduction Policy
- Prioritize low-risk fixes: exact-version alignment, removal of unnecessary explicit references
- Validate warning deltas in batches before broadening
- Use `FuseCP/build-debug.bat` as default validation gate for warning-remediation batches
- Do not hide warnings globally; prefer explicit fixes

## Regression Prevention During Warning Work
- Do not remove or weaken project dependency edges/output-copy behavior to silence warnings
- If any package/reference/project-reference/output-path change is made, verify critical runtime assemblies still resolve:
  - `FuseCP.EnterpriseServer.dll`
  - `FuseCP.Server.dll`
  - `FuseCP.EnterpriseServer.Client.dll`
- Treat Portal/Enterprise/Server as coupled scopes for compatibility checks

## Dependency/CVE Updates
- Validate compatibility across all affected TFMs (`net48`, `net10.0`, `netstandard2.0`)
- Validate all affected solution scopes (`Portal`, `Enterprise`, `Server`) before merge
- Update related scripts/docs if package requirements or recommended commands change

## Legacy Installer
- If work affects `.vdproj` packaging, verify prerequisites with `check-test-environment.ps1 -Profile Package -RequireLegacyMsi`
- Report each missing dependency explicitly and provide concrete install/enable command

## Reporting
- Report what was validated and what could not be validated locally
- Mention what was **not** validated locally if anything was skipped