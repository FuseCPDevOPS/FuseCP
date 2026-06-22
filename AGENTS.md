# FuseCP Agent Instructions

## Architecture

FuseCP is a hosting control panel migrated from SolidCP to ASP.NET Core on .NET 10. Three primary layers:

- **Portal** (`FuseCP/Sources/FuseCP.WebPortal/`): Front-end GUI, ASP.NET WebForms on .NET 10
- **Enterprise** (`FuseCP/Sources/FuseCP.EnterpriseServer*/`): Business logic, EF Core database access
- **Server** (`FuseCP/Sources/FuseCP.Server*/`): Execution layer on managed hosts

Solutions are split across `FuseCP/Sources/`. Many have ordering/dependency relationships.

## Core Principles

- Keep changes minimal and task-focused. Preserve existing behavior unless explicitly requested.
- Match existing architecture and coding style in the touched project.
- Do not modify unrelated files.
- Prefer root-cause fixes over cosmetic patches.
- Validate null handling, error paths, and permission checks.
- Keep backward compatibility in shared contracts unless explicitly approved.
- Update docs when behavior, configuration, or deployment steps change.
- FuseCP is a migrated codebase: if implementation intent is unclear, consult `origin/SolidCPv1` branch for legacy behavior before changing contracts or build wiring.

## Tech Stack

- **Runtime**: .NET 10 (SDK 10.0.200), .NET Framework 4.8 (legacy providers)
- **Web**: ASP.NET Core (Server), ASP.NET WebForms on Core (Portal), CoreWCF
- **Database**: Entity Framework Core on .NET 10, EF 6 on .NET Framework; SqlServer/MySQL/PostgreSQL/SQLite
- **Testing**: MSTest (`Microsoft.VisualStudio.TestTools.UnitTesting`)
- **Build**: MSBuild orchestration (`build.xml`), `dotnet build` for individual solutions
- **Installer**: WiX, Avalonia UI
- **Frontend**: LESS/CSS, Bootstrap 5.3 (migrating from 3), jQuery, Bootstrap Icons

## C# Coding Conventions

- Copyright header: `Copyright (C) 2026 FuseCP` (GPL v3 license block)
- Namespace pattern: `FuseCP.{Layer}.{Feature}` (e.g. `FuseCP.EnterpriseServer`, `FuseCP.Providers.Web`)
- Use `#region` blocks for logical grouping in large controller classes
- Security checks at method entry: `SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive)`
- Provider pattern: `ServiceProviderProxy.Init(provider, serviceId)` for provider initialization
- Exception filter pattern: `catch (Exception ex) when (!(ex is OutOfMemoryException) && !(ex is StackOverflowException) && !(ex is AccessViolationException))`
- Tab indentation (not spaces) in most files
- `using` directives at top, not file-scoped namespaces (legacy files); file-scoped namespaces in newer test files
- Prefer existing safe helpers over direct casts for remoted PSObject properties

## Security

- Never expose secrets, credentials, tokens, or private tenant data.
- Never commit environment-specific `Web.config` secrets (connection strings, machineKey, private endpoints).
- Commit only structural/runtime-safe Web.config changes; keep secrets local-only.
- Runtime auth config: write to `appsettings.hardened.json` as narrow overlay, not base `appsettings.json`.
- Avoid introducing insecure defaults. Flag security-sensitive changes for maintainer review.
- Never delete `FuseCP.Installer/Sources/FuseCP.InstallPackages/src/bin/fusecp-installer` unless explicitly requested.

## Build & Validation

### Quick References
- **Start of day**: `pwsh -File FuseCP/Tools/Start-Of-Day.ps1`
- **Fast validation**: `pwsh -File FuseCP/Tools/run-local-validation.ps1 -ChangedOnly -SkipIfNoChanges -DisableNuGetAudit`
- **Solution sync**: `pwsh -File FuseCP/Tools/check-sln-scope-sync.ps1`
- **Unlock portal build**: `pwsh -File FuseCP/Tools/Unlock-WebPortal-Build.ps1`

### Validation Strategy
Use **smallest relevant scope first**:
- Portal: `dotnet build FuseCP.WebPortalAndEnterpriseServer.sln` (from `FuseCP/Sources`)
- Enterprise: `dotnet build FuseCP.EnterpriseServer.sln`
- Server: `dotnet build FuseCP.Server.sln`
- Tests: `dotnet build FuseCP.Tests.sln` then `dotnet test FuseCP.Tests.sln --configuration Release --no-build -v n`

### Validation Options (`run-local-validation.ps1`)
- `-ChangedOnly`: fast iteration, path-based scope inference
- `-SkipIfNoChanges`: avoid unnecessary builds
- `-DisableNuGetAudit`: reduce local noise during iteration only
- `-JsonOutputPath`: machine-readable output for PR tooling
- `-NoRestore`: skip NuGet restore for repeated local loops
- `-ScopeMapPath`: extend path-to-scope routing from JSON

### Build Orchestration
- Prefer orchestrated builds (`build.xml`, `build-debug.bat`, `build-release.bat`) for end-to-end validation.
- If `w3wp` locks `bin_dotnet`, stop IIS worker processes first or use `Unlock-WebPortal-Build.ps1`.
- Keep `FuseCP.sln` synchronized with `FuseCP/Sources/FuseCP.WebPortal.sln`, `FuseCP/Sources/FuseCP.EnterpriseServer.sln`, and `FuseCP/Sources/FuseCP.Server.sln` for project add/remove/rename.

### Warning Policy
- Prioritize low-risk fixes: exact-version alignment, removal of unnecessary references.
- Do not hide warnings globally; prefer explicit fixes.
- Never weaken project dependency edges to silence warnings.
- Verify critical runtime assemblies still resolve after any reference change: `FuseCP.EnterpriseServer.dll`, `FuseCP.Server.dll`, `FuseCP.EnterpriseServer.Client.dll`.

## Database / Entity Framework Workflow

### Schema Changes
1. Edit Entity classes in `FuseCP/Sources/FuseCP.EnterpriseServer.Data/Entities/`
2. Update Fluent API configuration in `Configuration/` if needed
3. Add `ApplyConfiguration(model, new MyEntityConfiguration());` in `DbContextBase.OnModelCreating()`
4. Add DbSet property in `DbContext.Sets.cs`
5. Run `MigrationAdd.bat` to generate migrations for all 4 providers (SqlServer/MySQL/PostgreSQL/SQLite)

### SQLite FK Constraint Rule
`PRAGMA foreign_keys = OFF` is a no-op inside transactions. When deleting from parent tables (e.g. `Providers`), emit `migrationBuilder.Sql(...)` to delete child rows from non-cascading FK tables FIRST. Non-cascading FK tables to `Providers`: `ServiceDefaultProperties`, `Services`.

### Provider Removal Dependency Rule
Never delete from `Providers` first. Handle `ServiceDefaultProperties` -> `Services` -> then `Providers`. Remap `Services` rows to replacement provider (preferred) or explicitly remove.

### Known Local Issues
- **MySQL**: Bat hardcodes password but local has none. Re-run with `Uid=root;` (no Pwd).
- **SQL Server**: "Stream was not readable" is pre-existing; CI regenerates correctly.
- **PostgreSQL**: `Host=localhost;Port=5433;User ID=postgres;Password=Password12`

### Rules
- Never hand-edit EF model snapshots or migration files.
- `install.*.sql` are generated artifacts, not source of truth.
- SQLite upgrades run through EF migrations only.
- Verification automated via `FuseCP/Tools/Orchestrate-Database-Workflow.ps1`.
- Show both EF impact AND SQL impact in responses for DB changes.

## UI / LESS / CSS Workflow

### Source Files (edit these)
- `FuseCP/Sources/FuseCP.WebPortal/App_Themes/Default/Styles/main.less`
- `FuseCP/Sources/FuseCP.WebPortal/App_Themes/Default/Styles/Menus.less`
- `FuseCP/Sources/FuseCP.WebPortal/App_Themes/Default/Styles/defaultVariables.less`
- `FuseCP/Sources/FuseCP.WebPortal/App_Themes/Default/Styles/defaultTheme.less`

### Compiled Output (NEVER edit directly): `main.css`

### Recompile
```
cd FuseCP/Sources/FuseCP.WebPortal/App_Themes/Default/Styles
npm run build:css
```

### Bootstrap 3 -> 5.3 Migration
Replace deprecated: `panel`, `well`, `input-group-addon`, `btn-default`, `pull-*`, `img-responsive`, `hidden-*`, `visible-*`. Replace Glyphicons with Bootstrap Icons. Do not break existing behaviors or remove accessibility semantics.

## Exchange Provider Patterns

- **Provider parity**: Exchange 2013/2016/2019 share identical structure. Apply changes to ALL THREE in the same commit.
- **PSObject type variance**: NEVER direct cast on `GetPSObjectProperty()` results. Use: `ObjToBoolean`, `ConvertByteSizePropertyToKB/MB`, `ConvertUnlimitedIntPropertyToInt32`.
- **No-language runspace**: Guard `ConfirmPreference` and `Get-MailboxSearch` with try-catch + fallback.
- **Property access**: Prefer `PSObject.Properties["name"]` over `PSObject.Members["name"]`.

## Cleanup Checklist (Removing Features/Providers/Components)

DO NOT perform partial removals. Must satisfy ALL:
1. **Solution files**: Remove from ALL `.sln` + `ProjectConfigurationPlatforms`
2. **Source code**: Delete source files and project directories
3. **Database**: Remove EF seed, entities, configs, migrations (regenerate install scripts)
4. **Dependencies**: Remove NuGet/assembly refs, unused usings
5. **Localization**: Search `Languages/Resources.xml` for component strings
6. **Config/Docs**: Update web.config, README, CHANGELOG, upgrade scenarios
7. **Build**: Remove from `build.xml`, build/deploy scripts
8. **Tests**: Delete test fixtures and examples
9. **Artifacts**: Regenerate scope reports
10. **Provider verification**: Targeted build, verify contract stability

## Branch & Commit Convention

### Branch Names
- `portal/<issue-or-topic>`
- `enterprise/<issue-or-topic>`
- `server/<issue-or-topic>`
- `shared/<issue-or-topic>` for cross-module work

### Commit Messages
Module prefix + imperative summary:
- `Portal: fix login redirect loop`
- `Enterprise: handle null quota rows`
- `Server: retry app pool recycle`
- `Shared:` for multi-module commits

## PR Hygiene

- Include: concise summary, risk notes, exact validation commands run, what was NOT validated.
- AI disclosure: tools used, parts generated/transformed, manual validation performed.
- DB schema PRs: EF view + SQL view + provider retirement safety.
- Sanitize GitHub Actions artifact names (remove `"`, `:`, `<`, `>`, `|`, `*`, `?`, `\r`, `\n`, `\\`, `/`).
- Use `FuseCP/Tools/Create-Upstream-PR.ps1` for upstream PRs.
- Escalate to maintainers: installer packaging, security defaults, multi-sln deps, major upgrades.

## Dependency/CVE Updates

- Validate compatibility across all affected TFMs (`net48`, `net10.0`, `netstandard2.0`).
- Validate all affected solution scopes (Portal, Enterprise, Server) before merge.
- Update related scripts/docs if package requirements or commands change.

## Legal

- Copyright header: `Copyright (C) 2026 FuseCP`
- Keep year current in generator inputs and outputs (`build.xml`, `VersionInfo.*`).
- All AI-assisted output must be reviewed and approved by a human contributor before merge.
- Never fabricate test results, benchmarks, incident data, or release notes.
