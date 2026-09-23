---
description: Run tests for the relevant scope
---
Build and run the FuseCP test suite:

```
cd FuseCP/Sources
dotnet build FuseCP.Tests.sln
dotnet test FuseCP.Tests.sln --configuration Release --no-build -v n
```

Report test results, failures, and any tests that could not run. For failures, analyze root cause and suggest fixes.
