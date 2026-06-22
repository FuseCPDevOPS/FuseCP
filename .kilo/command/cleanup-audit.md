---
description: Audit completeness of a feature/provider removal
---
Review the current changes for completeness of removal. Check ALL of these:

1. All `.sln` files — project declarations and ProjectConfigurationPlatforms removed
2. Source files and project directories deleted
3. EF seed data, entities, configs, migrations cleaned up
4. NuGet/assembly references removed
5. `Languages/Resources.xml` strings cleaned
6. Config files (web.config, app.config) updated
7. `build.xml` and build/deploy scripts updated
8. Test fixtures and examples removed
9. Generated reports noted as stale
10. Provider contracts verified if applicable

List any gaps found. DO NOT mark cleanup as complete until ALL items pass.
