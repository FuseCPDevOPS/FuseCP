# PR Creation / AI Disclosure

## PR Hygiene Checklist
- Include: concise summary, risk notes, exact validation commands run
- Mention what was **not** validated locally if anything was skipped
- Keep PR scope cohesive; separate docs/tooling/runtime concerns when possible

## AI Disclosure
- If AI materially assisted implementation, disclose usage in PR body
- Include: which AI tool(s), which parts were generated/summarized/transformed, what manual validation was performed

## Database Schema Work PR Notes
Include both views:
- **EF view**: entities/configuration/DbContext changes and migration names for all 4 providers
- **SQL view**: concrete SQL operations introduced (`migrationBuilder.Sql(...)` plus generated `install.*.sql` impacts), including known local generation exceptions
- **Provider retirement safety**: document how `Providers.ProviderID` dependencies were handled in both `Services` and `ServiceDefaultProperties` (remap preferred, delete only when intentional), before provider row removal

## Escalation Triggers
Escalate to maintainers when changes involve:
- Installer packaging (`.vdproj`, legacy MSI prerequisites)
- Security-sensitive defaults
- Multi-solution dependency graph changes
- Broad framework/package major upgrades

## GitHub Actions Artifact Naming
- Never use raw commit/PR text directly as `actions/upload-artifact` name
- Sanitize dynamic names to remove/replace invalid characters: `"`, `:`, `<`, `>`, `|`, `*`, `?`, `\r`, `\n`, `\\`, `/`

## Upstream PR Creation
- Use `FuseCP/Tools/Create-Upstream-PR.ps1` so `PR_DRAFT.md` is cleared only after successful PR creation