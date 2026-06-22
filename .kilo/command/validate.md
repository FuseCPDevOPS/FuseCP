---
description: Run fast local validation on changed files
---
Run the FuseCP local validation for the current changes:

```
pwsh -File FuseCP/Tools/run-local-validation.ps1 -ChangedOnly -SkipIfNoChanges -DisableNuGetAudit
```

Report what was validated and what could not be validated locally. If validation fails, analyze the errors and suggest fixes.
