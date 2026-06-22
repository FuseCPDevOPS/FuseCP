---
description: Build the relevant solution scope
---
Build the narrowest relevant solution for the current changes:

1. Identify which layer(s) are affected (Portal, Enterprise, Server)
2. Run the appropriate build from `FuseCP/Sources`:
   - Portal: `dotnet build FuseCP.WebPortalAndEnterpriseServer.sln`
   - Enterprise: `dotnet build FuseCP.EnterpriseServer.sln`
   - Server: `dotnet build FuseCP.Server.sln`
   - Tests: `dotnet build FuseCP.Tests.sln`
3. If build fails due to `w3wp` locking `bin_dotnet`, run `pwsh -File FuseCP/Tools/Unlock-WebPortal-Build.ps1`
4. Report build results, warnings, and any issues
