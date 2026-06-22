---
description: Check solution file synchronization across scopes
---
Check if solution files are in sync:

```
pwsh -File FuseCP/Tools/check-sln-scope-sync.ps1
```

Report any synchronization issues found. If out of sync, fix the solution files to keep `FuseCP.sln` synchronized with `FuseCP/Sources/FuseCP.WebPortal.sln`, `FuseCP/Sources/FuseCP.EnterpriseServer.sln`, and `FuseCP/Sources/FuseCP.Server.sln`.
