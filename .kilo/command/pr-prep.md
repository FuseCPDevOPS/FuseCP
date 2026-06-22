---
description: Prepare PR description with validation summary
---
Help prepare a PR description. Include:

1. Concise summary of changes with module prefix (Portal/Enterprise/Server/Shared)
2. Risk assessment
3. What was validated (exact commands run)
4. What was NOT validated locally
5. AI disclosure (tools used, parts generated/transformed, manual validation)
6. For DB changes: EF view + SQL view + provider retirement safety
7. Solution sync status
8. Dependency/CVE compatibility evidence if applicable

Use the PR template at `.github/PULL_REQUEST_TEMPLATE.md` as the base structure.
