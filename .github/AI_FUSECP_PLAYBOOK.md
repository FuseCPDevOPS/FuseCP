# AI FuseCP Playbook

This playbook gives AI coding agents a safe, fast starting workflow for FuseCP.

## 1) First 5 Minutes

1. Read:
   - `.github/AI_DIRECTIVES.md`
   - `.github/copilot-instructions.md`
   - `CONTRIBUTING.md`
   - `TESTING_ENVIRONMENT.md`
2. Confirm prerequisites:
  - If `pwsh` is missing, install it first: `winget install --id Microsoft.PowerShell -e --accept-package-agreements --accept-source-agreements`
   - `pwsh -File FuseCP/Tools/check-test-environment.ps1 -Profile Unit`
  - First-time full setup (elevated): `powershell -File FuseCP/Tools/bootstrap-test-environment.ps1 -Install -RunAllProfiles -InstallSqlExpress -InstallIIS -InstallWsl -InstallWixToolset`
3. Run a fast local validation baseline:
   - `pwsh -File FuseCP/Tools/run-local-validation.ps1 -ChangedOnly -SkipIfNoChanges -DisableNuGetAudit`
3.1 Run start-of-day routine before feature work:
  - `pwsh -File FuseCP/Tools/Start-Of-Day.ps1`
4. If integration websites were started, stop them after work to reduce idle usage:
  - `FuseCP/Tools/stop-test.bat`
5. If the machine is not dev-only, apply one-time manual startup policy:
  - `powershell -File FuseCP/Tools/SetDevServicesManualStart.ps1 -StopNow` (run elevated)
6. End of day handoff (VS Code task):
  - Run task `Done for today` to stop local dev services and write `artifacts/session-notes/*.md` for next-day continuation.

## 2) Build/Validation Strategy

- Use **smallest relevant scope first**:
  - Portal: `-Scope Portal`
  - Enterprise: `-Scope Enterprise`
  - Server: `-Scope Server`
- Scope note:
  - `Portal` already builds `FuseCP.WebPortalAndEnterpriseServer.sln`.
  - If both `Portal` and `Enterprise` are selected, validation now skips the redundant Enterprise-only build automatically.
- Solution sync rule:
  - Keep `FuseCP.sln` in sync with `FuseCP/Sources/FuseCP.WebPortal.sln`, `FuseCP/Sources/FuseCP.EnterpriseServer.sln`, and `FuseCP/Sources/FuseCP.Server.sln` for project add/remove/rename changes.
  - If a project is intended to participate in build scope solutions, update both the scoped solution(s) and `FuseCP.sln` in the same PR.
- Mandatory relation gate (when project graph files change):
  - If `*.sln`, `*.csproj`, `*.vbproj`, `*.vcxproj`, or `*.shproj` files are touched, include explicit solution-sync verification in PR notes and run affected scope validation before merge.
  - Run `pwsh -File FuseCP/Tools/check-sln-scope-sync.ps1` to enforce that `FuseCP.sln` remains synchronized with Portal/Enterprise/Server scope solutions.
- Use orchestrated build when touching shared/build/deployment flows:
  - `pwsh -File FuseCP/Tools/run-local-validation.ps1 -Scope Shared`
- For repeated local loops after restore:
  - `pwsh -File FuseCP/Tools/run-local-validation.ps1 -Scope Enterprise -NoRestore`

## 3) Safe Change Rules

- Keep edits minimal and task-focused.
- Preserve behavior unless explicitly requested.
- Prefer root-cause fixes.
- Avoid broad package churn in unrelated projects.
- Keep PR scope cohesive; separate docs/tooling/runtime concerns when possible.

## 4) Scope Mapping Tips

`run-local-validation.ps1 -ChangedOnly` derives scope from changed paths.
Use optional overrides with:

- `-ScopeMapPath FuseCP/Tools/validation-scope-map.example.json`

Choose `-Scope Shared` when uncertain.

## 5) Warning Reduction Policy

- Prioritize low-risk fixes first:
  - exact-version alignment for existing package constraints
  - removal of unnecessary explicit references
- Validate warning deltas in batches (not per-file loops) before broadening.
- Use `FuseCP/build-debug.bat` as the default validation gate for warning-remediation batches.
- Do not hide warnings globally; prefer explicit fixes.

Regression-prevention requirements during warning work:

- Do not remove or weaken project dependency edges/output-copy behavior to silence warnings.
- If any package/reference/project-reference/output-path change is made, verify critical runtime assemblies still resolve:
  - `FuseCP.EnterpriseServer.dll`
  - `FuseCP.Server.dll`
  - `FuseCP.EnterpriseServer.Client.dll`
- Treat Portal/Enterprise/Server as coupled scopes for compatibility checks.

For dependency/CVE updates:

- Validate compatibility across all affected TFMs (`net48`, `net10.0`, `netstandard2.0` as applicable).
- Validate all affected solution scopes (`Portal`, `Enterprise`, `Server`) before merge.
- Update related scripts/docs if package requirements or recommended commands change.

## 6) PR Hygiene Checklist

- Include:
  - concise summary
  - risk notes
  - exact validation commands run
  - AI usage disclosure when materially assisted
- Mention what was **not** validated locally if anything was skipped.

For database schema work, include both views in PR notes and AI handoff notes:

- EF view: entities/configuration/DbContext changes and migration names for all 4 providers.
- SQL view: concrete SQL operations introduced (`migrationBuilder.Sql(...)` plus generated `install.*.sql` impacts), including any known local generation exceptions.
- Provider retirement safety: document how `Providers.ProviderID` dependencies were handled in both `Services` and `ServiceDefaultProperties` (remap preferred, delete only when intentional), before provider row removal.

## 7) Escalation Triggers

Escalate to maintainers when changes involve:

- installer packaging (`.vdproj`, legacy MSI prerequisites)
- security-sensitive defaults
- multi-solution dependency graph changes
- broad framework/package major upgrades

## 8) Prereq Triage Notes

- Common local blockers for full deploy flow:
  - missing local `(local)\SQLExpress` instance/connectivity
  - WSL installed without a distro (`wsl --list --quiet` empty)
  - missing `Wix.CA.targets` in Visual Studio MSBuild WiX v3 path
- Legacy installer projects now support fallback to WiX Toolset SDK targets
  (`WiX Toolset v3.14` / `v3.11`) when VS WiX targets path is absent.
- Always report which prerequisite is missing and the exact command used to verify it.
- Bootstrap flags for faster onboarding:
  - `-InstallWsl [-WslDistroId <winget-id>]`
  - `-InstallWixToolset`
