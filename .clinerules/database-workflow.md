# Database / EF / Migrations Workflow

## Schema Changes
1. Edit Entity classes in `FuseCP/Sources/FuseCP.EnterpriseServer.Data/Entities/`
2. Update Fluent API configuration in `Configuration/` if needed
3. Add `ApplyConfiguration(model, new MyEntityConfiguration());` in `DbContextBase.OnModelCreating()`
4. Add DbSet property in `DbContext.Sets.cs`
5. Run `FuseCP/Sources/FuseCP.EnterpriseServer.Data/MigrationAdd.bat` to generate migrations for all 4 providers

## Known Local Generation Issues
- **MySQL auth failure**: Bat hardcodes `Uid=root;Pwd=Password12` but local dev MySQL root has no password. After bat finishes, manually re-run:
  ```
  dotnet ef migrations add --framework net10.0 --no-build -o Migrations\MySql --context MySqlDbContext <MigrationName> -- "DbType=MySql;Server=localhost;Database=FuseCP;Uid=root;"
  dotnet ef migrations script --framework net10.0 --no-build -o Migrations\MySql\install.mysql.sql --context MySqlDbContext -i -- "DbType=MySql;Server=localhost;Database=FuseCP;Uid=root;"
  Copy-Item -Force "Migrations\MySql\install.mysql.sql" "..\..\Database\install.mysql.sql"
  ```
- **SQL Server "Stream was not readable"**: Pre-existing issue on this dev box. Previous file is copied in its place; CI regenerates correctly.
- **PostgreSQL connection**: `Host=localhost;Port=5433;User ID=postgres;Password=Password12` — works.

## SQLite FK Constraint Rule
`PRAGMA foreign_keys = OFF` is a **no-op inside transactions**. When an EF SQLite migration deletes rows from a parent table (e.g. `Providers`), emit `migrationBuilder.Sql(...)` to delete child rows from every non-cascading FK table first.

Tables with non-cascading FKs to `Providers`: `ServiceDefaultProperties` and `Services`.

Pattern:
```csharp
migrationBuilder.Sql(@"DELETE FROM ""ServiceDefaultProperties"" WHERE ""ProviderID"" IN (1, 2, 3);");
migrationBuilder.DeleteData(table: "Providers", keyColumn: "ProviderID", keyValue: 1);
```

## Provider Removal Dependency Rule
- Never delete from `Providers` first.
- `Providers.ProviderID` is referenced by `Services.ProviderID` and `ServiceDefaultProperties.ProviderID`.
- For provider retirement: remap `Services` rows to replacement provider (preferred), or explicitly remove dependent rows when intended.
- Handle `ServiceDefaultProperties` before removing provider rows.
- Apply consistently across SqlServer/MySql/PostgreSql/Sqlite migrations.

## AI Response Requirements for DB Changes
Explicitly show both EF and SQL impact:
- **EF side**: Entities/Configuration touched, DbContext wiring changes, migration names for all 4 providers.
- **SQL side**: Summary of SQL operations (`INSERT/UPDATE/DELETE`, data remap, seed add/remove, FK safety deletes) and where they appear (`migrationBuilder.Sql(...)` and/or generated `install.*.sql` deltas).
- **Artifacts**: Which generated scripts changed under `Migrations/*/install.*.sql` and `FuseCP/Database/install.*.sql`, and known local generation exceptions.

## Rules
- Never hand-edit EF model snapshot files or migration files.
- Treat `install.*.sql` as generated artifacts, not source of truth.
- SQLite upgrades run through EF migrations; do not use `install.sqlite.sql` as upgrade script.
- `update_db.sql` is legacy bridge for pre-v2.0.0 upgrades; do not change for normal post-v2.0.0 work.
- `LegacyScripts/master.update_db.sql` is archival baseline; do not modify.
- Squash dev-only intermediate migrations before release using `MigrationRemove.bat` / snapshot-revert approach.

## Verification
- Database workflow verification is **fully automated** — never manually run verification scripts.
- Single entry point: `FuseCP/Tools/Orchestrate-Database-Workflow.ps1` (modes: Quick, Full, Verify, Fix, Report)
- Enforced at CI, local builds, and pre-commit hooks.
- Reference: `DATABASE_WORKFLOW_COMPLETE.md`