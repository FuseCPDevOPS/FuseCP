# Full Standards / Legal / Licensing

## Accountability
All AI-assisted output must be reviewed and approved by a human contributor before merge. The person opening the PR is responsible for correctness, security, licensing, and production impact.

## Transparency
If a change is materially AI-assisted, disclose it in the PR description with:
- Which AI tool(s) were used
- Which parts were generated, summarized, or transformed
- What manual validation was performed

## Security and Privacy
Do not share secrets or sensitive operational data with third-party AI systems, including:
- Credentials, keys, connection strings, and tokens
- Customer or tenant data
- Internal infrastructure details not required for the task
- Environment-specific `Web.config` secrets (production/local DB credentials, machineKey values, private hostnames/endpoints)

Use sanitized examples whenever possible.

When `Web.config` changes are required, commit only structural/runtime-safe updates (module wiring, handlers, non-secret defaults), use a sanitized commit-safe file for source control, and restore secret-bearing local values after commit so secrets remain local-only.

## Code Quality and Testing
AI-generated changes must meet the same quality standard as human-authored code:
- Compile and pass relevant tests
- Prefer repository build orchestrators (`build.xml`, `build-*.bat`, `deploy-*.bat`) over independently building isolated solution files when validating broad changes
- Prefer streamlined local validation entrypoint `FuseCP/Tools/run-local-validation.ps1` for repeatable local checks
- Use `run-local-validation.ps1 -ChangedOnly` for fast local loops when scope can be derived from changed files
- Prefer `-JsonOutputPath` for machine-readable validation evidence in PR workflows
- Use `-DisableNuGetAudit` only for local iteration noise reduction; do not treat it as a security fix
- Use `-SkipIfNoChanges` with `-ChangedOnly` when you want no-op runs to complete without full builds
- Before adding/removing/changing any `ProjectReference`, check all relevant solution files under `FuseCP/Sources/*.sln` (at minimum Portal/Enterprise/Server paths) and validate the affected solution builds to avoid cross-solution breakage
- Keep `FuseCP.sln` synchronized with `FuseCP/Sources/FuseCP.WebPortal.sln`, `FuseCP/Sources/FuseCP.EnterpriseServer.sln`, and `FuseCP/Sources/FuseCP.Server.sln` for project add/remove/rename changes when those projects are part of build scope
- Treat project/solution graph updates as a required validation gate: project file changes (`*.csproj`, `*.vbproj`, `*.vcxproj`, `*.shproj`, `*.sln`) must include explicit solution-sync verification notes in PR validation output
- Treat FuseCP as a migrated codebase: if implementation intent is unclear, consult project origins in branch `origin/SolidCPv1` (typically under `SolidCP/Sources/...`) to recover legacy behavior and architecture context before changing contracts or build wiring
- For package/dependency/CVE updates, require compatibility validation across affected target frameworks (`net48`, `net10.0`, `netstandard2.0` where applicable) and affected solution scopes (Portal/Enterprise/Server)
- For package/dependency/CVE updates, update related validation scripts/docs/PR notes whenever requirements, tooling assumptions, or recommended commands change
- Follow existing project patterns and style
- Include documentation updates when behavior changes
- Avoid introducing unused dependencies or broad refactors unrelated to the task
- If changes touch legacy installer `.vdproj` packaging, explicitly validate legacy MSI prerequisites (`check-test-environment.ps1 -Profile Package -RequireLegacyMsi`)
- When prerequisite checks fail, report each missing dependency explicitly (for example: SQLExpress instance, WSL distro, WiX MSBuild targets) and provide the concrete install/enable command used for local remediation
- For local integration tooling that starts background services (IIS websites, SQLExpress, WSL), prefer documented start/stop scripts so contributors can reduce idle system usage
- For first-time environment setup guidance, prefer the bootstrap installer workflow (`FuseCP/Tools/bootstrap-test-environment.ps1`) with explicit flags needed for the contributor scenario (integration, packaging, legacy installer)
- When asked to create an upstream PR, use `FuseCP/Tools/Create-Upstream-PR.ps1` so `PR_DRAFT.md` is cleared only after successful PR creation
- For GitHub Actions artifact publishing, never use raw commit/PR text directly as `actions/upload-artifact` name; sanitize dynamic names to remove/replace invalid characters (`"`, `:`, `<`, `>`, `|`, `*`, `?`, `\r`, `\n`, `\\`, `/`) before upload

## Legal and Licensing
Contributions must comply with repository licensing and third-party license requirements. Do not submit generated code or content if you cannot verify legal use or attribution obligations.

Copyright header policy:
- For source/project files that use copyright headers/metadata, enforce exact text format: `Copyright (C) 2026 FuseCP`
- Keep the year current when annual updates occur, including generator inputs and generated outputs (notably `FuseCP/build.xml`, `FuseCP/Sources/VersionInfo.cs`, `FuseCP/Sources/VersionInfo.vb`, and installer `VersionInfo.cs` files)

## Disallowed Uses
AI tools must not be used to:
- Generate abusive, discriminatory, or harassing project content
- Fabricate test results, benchmarks, incident data, or release notes
- Bypass review, approval, or security controls

## Maintainer Enforcement
Maintainers may request edits, additional testing, provenance details, or reject changes that do not comply with these directives.

These directives supplement (and do not replace) the project Code of Conduct.