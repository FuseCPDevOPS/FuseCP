# FuseCP Cline Rules — Core (Always Loaded)

## Principles
- Keep changes minimal and task-focused. Preserve existing behavior unless explicitly requested.
- Match existing architecture and coding style in the touched project.
- Do not modify unrelated files.
- Prefer root-cause fixes over cosmetic patches.
- Validate null handling, error paths, and permission checks.
- Keep backward compatibility in shared contracts unless explicitly approved.
- Update docs when behavior, configuration, or deployment steps change.

## Security
- Never expose secrets, credentials, tokens, or private tenant data.
- Avoid introducing insecure defaults. Flag security-sensitive changes for maintainer review.
- Commit only structural/runtime-safe Web.config changes; keep secrets local-only.

## Quick References
- **First 5 minutes**: `pwsh -File FuseCP/Tools/Start-Of-Day.ps1`
- **Fast validation**: `pwsh -File FuseCP/Tools/run-local-validation.ps1 -ChangedOnly -SkipIfNoChanges -DisableNuGetAudit`
- **Solution sync check**: `pwsh -File FuseCP/Tools/check-sln-scope-sync.ps1`
- **Create upstream PR**: `pwsh -File FuseCP/Tools/Create-Upstream-PR.ps1`

## Contextual Rules (load when relevant)
- Database / EF / migrations → `.clinerules/database-workflow.md`
- LESS / CSS / Bootstrap / UI → `.clinerules/ui-styling.md`
- Builds / tests / validation → `.clinerules/build-validation.md`
- Removing features / providers / components → `.clinerules/cleanup-checklist.md`
- Exchange 2013/2016/2019 providers → `.clinerules/exchange-providers.md`
- PR creation / AI disclosure → `.clinerules/pr-hygiene.md`
- Full standards / legal / licensing → `.clinerules/core-standards.md`

## Copyright
- Use exact text: `Copyright (C) 2026 FuseCP` where headers are required.