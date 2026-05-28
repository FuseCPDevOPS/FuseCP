# PowerShell Hosting Troubleshooting for FuseCP Providers

This guide captures patterns that fixed recent provider failures after upgrading to `System.Management.Automation` 7.x and running under IIS.

## Symptoms This Guide Addresses

- Build errors such as missing `RunspaceInvoke` and `RunspaceConfiguration`.
- Runtime errors where cmdlets are available in an interactive shell but fail in FuseCP modules.
- DNS/other provider tasks that complete backend work but surface ambiguous portal failures.
- IIS-hosted module loading differences (execution policy, module export shape, constrained session behavior).

## 1) API Migration Patterns (Compile-Time)

### Replace `RunspaceInvoke`

Legacy `RunspaceInvoke` usage is not available in PowerShell 7 hosting scenarios used by providers.

Use:

- `ScriptBlock.Create(scriptText)` for script creation.
- Existing runspace/pipeline invocation helpers for execution.

### Replace `RunspaceConfiguration`

Legacy `RunspaceConfiguration` should be replaced with `InitialSessionState`.

Use:

- `InitialSessionState.CreateDefault2()` as the baseline session state.
- Configure modules/variables/functions via `InitialSessionState` before opening the runspace.

## 2) Module Loading Patterns (Runtime)

### Prefer module import over snap-ins

Do not depend on `AddPSSnapIn` in modern provider codepaths.

Use:

- Explicit module import (`Import-Module ...`) with deterministic module paths when possible.
- Command-availability verification (`Get-Command <Cmdlet>`) immediately after import.
- A fallback import path when Windows inbox modules require compatibility mode.

### Windows inbox module compatibility fallback

For modules such as `DnsServer` that may rely on Windows PowerShell components, keep a compatibility fallback:

- First attempt normal module import.
- If command export is missing, retry with `Import-Module -UseWindowsPowerShell`.
- Re-check command availability before continuing.

## 3) Execution Policy in Hosted Runspaces

In IIS-hosted runtime, avoid relying on `Set-ExecutionPolicy` as a startup operation.

Recommended pattern:

- Set `InitialSessionState.ExecutionPolicy` (for example `Bypass`) before runspace creation.
- Set process-scoped policy hint via environment variable where needed:
  - `PSExecutionPolicyPreference=Bypass`

This avoids failures when `Microsoft.PowerShell.Security` is unavailable in the constrained hosting context.

## 4) Validate Availability, Not Just Import Success

A successful `Import-Module` call is not enough.

Always validate:

1. Module import returns no hard errors.
2. Required cmdlets are present via `Get-Command`.
3. Provider operation is retried only after command presence is confirmed.

## 5) IIS-Specific Operations and Diagnostics

### Handle locked DLLs during rebuild/deploy

If provider binaries fail to copy due to file locks, IIS worker processes may still hold previous assemblies.

Operational steps:

1. Recycle only the relevant app pool (preferred).
2. Rebuild/redeploy provider.
3. Confirm a fresh `w3wp` instance is serving requests.

### App pool recycle checklist for stuck portal task state

When UI remains on a progress message after backend success:

1. Verify backend outcome directly (for example, DNS zone exists).
2. Recycle the portal app pool.
3. Hard-refresh UI and re-check task completion state.
4. If needed, monitor current `stdout_*.log` in `FuseCP.WebPortal/App_Data/logs` during a fresh reproduction.

## 6) Provider Parity Rule for Shared Patterns

When a fix is applied to one provider implementing a shared PowerShell pattern, apply equivalent changes to sibling providers in the same family before merge.

Examples:

- OS providers (2016/2019/2022/2025)
- HostedSolution provider implementations
- Web/IIS helper layers
- Exchange provider variants where method structure is intentionally parallel

## 7) Recommended Verification Sequence

1. Build the narrow set of touched provider projects.
2. Execute one real operation through the portal.
3. Validate backend result directly (for example, `Get-DnsServerZone`).
4. Confirm no new stack traces in active portal log.
5. If UI state is stale, recycle portal app pool and re-validate.

## 8) Quick Checklist for New PowerShell Provider Work

- Use `InitialSessionState.CreateDefault2()`.
- Avoid `RunspaceInvoke`/`RunspaceConfiguration`.
- Avoid snap-in dependencies.
- Import modules explicitly and verify cmdlets exist.
- Include compatibility fallback for Windows PowerShell-only modules.
- Prefer runspace-level/process-level execution policy configuration over startup `Set-ExecutionPolicy` calls.
- Validate behavior under IIS, not only in interactive PowerShell.
