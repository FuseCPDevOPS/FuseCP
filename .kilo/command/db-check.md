---
description: Run automated database workflow verification
---
Run the automated database workflow verification:

```
pwsh -File FuseCP/Tools/Orchestrate-Database-Workflow.ps1 -Mode Quick
```

Report the EF and SQL impact, any migration issues, and known local generation exceptions. For database changes, always show:
- **EF side**: Entities/Configuration touched, DbContext wiring changes, migration names for all 4 providers
- **SQL side**: Summary of SQL operations and where they appear
- **Artifacts**: Which generated scripts changed and known local generation exceptions
