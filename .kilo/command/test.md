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

If the task also involves manual test coverage, open findings, or updating coverage results, read `artifacts/PANEL_PROVIDER_TEST_WORKBOOK.md` (source of truth) and keep it in sync with the results.
