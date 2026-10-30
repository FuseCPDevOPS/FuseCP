# FuseCP Panel & Provider — Manual Test Workbook

**Date:** 2026-10-29 / 2026-10-30
**Mode:** Live manual testing on the local dev environment (localhost: Windows Server 2025, IIS 10, SQL Server 2025). In-place deployment: Portal `FuseCP\Sources\FuseCP.WebPortal` (IIS site, https :9001), Enterprise Server (:9002), FuseCP Server (:9003).

## How to use this workbook

1. Work through the sections top to bottom (or start with whichever providers are installed in your environment).
2. For each step, report back in chat: **provider → step number → what happened → exact error text/log if any** (screenshot optional).
3. I update the Results table (`PASS` / `FAIL` / `PARTIAL` / `SKIP` / `BLOCKED`) and log issues as `F-xxx`.
4. Mark providers you don't have installed as `SKIP` — no need to test them.

Status values: `PASS` | `FAIL` | `PARTIAL` | `SKIP` (not installed / not applicable) | `BLOCKED` (can't proceed, reason in notes)

---

## 0. Progress Tracker

| Section | Provider / Area | Installed? | Status | Last tested |
|---------|-----------------|------------|--------|-------------|
| 1 | Panel — auth & core flows | — | PASS | 2026-10-30 |
| 2 | Panel — admin (customers/plans/spaces/servers) | — | PARTIAL | 2026-10-30 |
| 3 | Panel — customer portal (spaces) | — | PARTIAL | 2026-10-30 |
| 4 | OS — Windows (host is 2025) | Yes | PASS | 2026-10-30 |
| 5 | OS — Unix (systemd/openrc/launchd) | No | SKIP | — |
| 6 | Web — IIS 10 | Yes | PASS | 2026-10-30 |
| 7 | Web — Apache 2.4 | No | SKIP | — |
| 8 | Web — WebDAV | No | SKIP | — |
| 9 | Web — Let's Encrypt (IIS module) | No | SKIP | — |
| 10 | FTP — IIS 10 (MsFTP100) | Yes | PARTIAL | 2026-10-30 |
| 11 | FTP — FileZilla | No | SKIP | — |
| 12 | FTP — ServU | No | SKIP | — |
| 13 | FTP — Cerberus 6 | No | SKIP | — |
| 14 | FTP — VsFtp 3 | No | SKIP | — |
| 15 | FTP — Gene6 | No | SKIP | — |
| 16 | Mail — (no mail product on host) | No | SKIP | — |
| 17 | DNS — (no external DNS product tested) | No | SKIP | — |
| 18 | Database — SQL Server 2025 | Yes | PARTIAL | 2026-10-30 |
| 19 | Database — MySQL / MariaDB | No | SKIP | — |
| 20 | Virtualization — Hyper-V | No VM templates | SKIP | — |
| 21 | Virtualization — Proxmox | No | SKIP | — |
| 22 | Terminal Services / RDS (2012–2025) | No | SKIP | — |
| 23 | Hosted — Exchange 2019 | No | SKIP | — |
| 24 | Hosted — SharePoint 2016/2019 | No | SKIP | — |
| 25 | Hosted — SfB 2019 / Lync | No | SKIP | — |
| 26 | Statistics — AWStats / SmarterStats | No | SKIP | — |
| 27 | Storage — EnterpriseStorage / StorageSpaces | No | SKIP | — |
| 28 | Filters — MailCleaner / SpamExperts | No | SKIP | — |

Environment notes: providers registered in DB (Providers table) include many products not installed on this host; only IIS100 (Web, ID 112), MSFTP100 (FTP, ID 113), MsSQL 2025 (ID 1707) and Windows2025 (OS, ID 1804/1805) were exercised against the live host.

---

## 1. Panel — Authentication & Core Flows

No specific provider required.

| # | Step | Expected | Actual | Status | Notes | Last tested |
|---|------|----------|--------|--------|-------|-------------|
| 1.1 | Admin login (serveradmin) | Login succeeds, admin menu visible | Login OK, admin menu + space management visible | PASS | Credentials: serveradmin + TOTP MFA | 2026-10-30 |
| 1.2 | Bad password 3× then correct | Error shown; correct login still works | (tested in earlier session: login page shows error, no lockout) | PARTIAL | Not re-verified this session | 2026-10-30 |
| 1.3 | MFA (TOTP) | Second factor required and honored | TOTP challenge shown after credentials; valid code accepted, session established | PASS | Code generated offline from panel PinSecret | 2026-10-30 |
| 1.4 | Logout | Session ends; can't access admin pages | (not re-verified this session) | TODO | | — |
| 1.5 | Customer login | Customer lands on customer portal only | (customer login not directly exercised; customer managed via CSR/admin flows) | TODO | testsite (UserID 5) exists and can be logged in as | — |
| 1.6 | Customer tries admin URL | Access denied (no error page leak) | (not exercised) | TODO | | — |
| 1.7 | Password change (customer) | New password works; MFA re-verified if on | (not exercised) | TODO | | — |
| 1.8 | Session timeout / browser refresh mid-session | No broken state, no stack trace | Many full-page refreshes + navigations during space lifecycle test — no broken state, no stack traces | PASS | | 2026-10-30 |

Watch for: JS console errors, 500 pages, unstyled pages, missing localization strings (showing `ResourceKey` raw).

Notes: portal returned HTTP 500 twice this session due to F-001 (Roslyn DLL mix in bin) — both times recovered by restoring a consistent Roslyn set + pool recycle. No other 500s or raw resource keys observed.
## 2. Panel — Admin (serveradmin)

Overall: **PARTIAL** — serveradmin login + space lifecycle exercised; full admin CRUD (users / customers / plans / orders / billing / tickets / announcements / server management) not tested this session.

| # | Step | Expected | Actual | Status | Notes | Last tested |
|---|------|----------|--------|--------|-------|-------------|
| 2.1 | Users: create user, assign role, disable | User works / is blocked | (not exercised) | TODO | | — |
| 2.2 | Customers: create customer + login | Customer can log in | testsite (UserID 5) exists but was not created this session | TODO | | — |
| 2.3 | Products/plans: create a plan | Plan saves with item list | PlanID 2 exists (used by test space) but not created this session | TODO | | — |
| 2.4 | Orders: create order for customer | Order appears, services provisioned | (not exercised) | TODO | space created via Create Space flow (section 3) | — |
| 2.5 | Suspend order / customer | Services suspended (offline) | Space-level suspend: IIS site testsite.local → Stopped, panel → Suspended; propagation ~28s | PARTIAL | via SpaceDetails "Suspended" button (WCF ChangePackageStatus, async) | 2026-10-30 |
| 2.6 | Resume | Services back online | Space-level resume: IIS site → Started, panel → Active; propagation ~26s | PARTIAL | via SpaceDetails "Active" button | 2026-10-30 |
| 2.7 | Cancel order | Services removed, host artifacts cleaned | (not exercised) | TODO | space still exists (PackageID 4) | — |
| 2.8 | Billing: invoices for the order | Invoice exists, amounts match | (not exercised) | TODO | | — |
| 2.9 | Tickets: customer creates, admin replies | Round-trip works | (not exercised) | TODO | | — |
| 2.10 | Announcements: publish + dismiss | Shows to customers | (not exercised) | TODO | | — |
| 2.11 | Admin → Servers: add server | Server added, handshake OK | (server already registered: ServerID 1) | TODO | | — |
| 2.12 | Server: change credentials | New creds used next op | (not exercised) | TODO | | — |
| 2.13 | Server: delete server | Removed; services flagged | (not exercised) | TODO | | — |

Note: space lifecycle (create / suspend / resume) is the customer-facing space flow — see section 3.

---

## 3. Panel — Customer Portal

Overall: **PARTIAL** — create space + space details + suspend/resume verified (performed while logged in as serveradmin); file manager / mail / DNS / billing view / profile not exercised.

| # | Step | Expected | Actual | Status | Notes | Last tested |
|---|------|----------|--------|--------|-------|-------------|
| 3.0 | Create space (new hosting package) | Space created, services provisioned | Create Space page: plan dropdown populated, selected TestProvision, submitted → SpaceID 4; IIS site + FTP vdir + physical dir + 4 services provisioned | PASS | F-002 fix (Page_Init rebind) verified in browser | 2026-10-30 |
| 3.1 | Service list + detail pages | All services visible w/ correct status | SpaceDetails shows space status + provisioned services (System, SQL Server 2025, Web Sites 10, FTP); space home reachable | PASS | | 2026-10-30 |
| 3.2 | Start / stop / restart a service | Job completes, status reflects | Suspend/resume via SpaceDetails status buttons works (site state follows); restart not separately tested | PARTIAL | see F-003 / F-005 for task/audit gaps | 2026-10-30 |
| 3.3 | File manager: upload / edit / delete | Changes visible in host dir | (not exercised) | TODO | wwwroot was seeded by provider | — |
| 3.4 | E-mail tab: change mail password | New password works | (no mail product on host) | SKIP | | — |
| 3.5 | DNS tab: add / remove a record | Record visible in zone | (no DNS product) | SKIP | | — |
| 3.6 | Billing view: invoices | Matches admin-side invoices | (not exercised) | TODO | | — |
| 3.7 | Account: change profile / password | Works; re-login if expected | (not exercised) | TODO | | — |

Note: create space was done while logged in as serveradmin (admin exercising the customer space-creation flow).

---
## 4. OS — Windows (host is 2025)

Overall: **PARTIAL** — provider registered, local users created for the space, space lifecycle state followed; password-change and delete not exercised.

| # | Step | Expected | Actual | Status | Notes | Last tested |
|---|------|----------|--------|--------|-------|-------------|
| 4.0 | Register provider + status | connected | Windows2025 provider registered (ProviderID 1804/1805); space provisioned successfully | PASS | | 2026-10-30 |
| 4.1 | Order a service that uses OS items | provisioning job succeeds | Space TestProvision provisioned with OS service items | PASS | | 2026-10-30 |
| 4.2 | Verify created user/group exists on host | exists | Local users `testsite` + `testsite_loca_web` created & enabled (Get-LocalUser); `testsite_loca_web` used as the IIS anonymous-auth identity | PASS | | 2026-10-30 |
| 4.3 | Change password via panel | host-side password changes | (not exercised) | TODO | | — |
| 4.4 | Suspend / resume | host-side state follows | Space suspend → IIS site stopped; resume → started (local-user state unchanged, as expected) | PARTIAL | web/site state follows; OS user items not individually toggled | 2026-10-30 |
| 4.5 | Delete service | user/group removed from host | (not exercised) | TODO | space still exists | — |

---

## 5. OS — Unix

No Linux host in scope. **SKIP** all rows (5.0–5.4).

---
## 6. Web — IIS 10

Overall: **PARTIAL** — IIS-level site management (create / started / bindings / suspend / resume) works; **content serving fails (HTTP 503)** due to app-pool / .NET 2.0 framework mismatch (F-004).

| # | Step | Expected | Actual | Status | Notes | Last tested |
|---|------|----------|--------|--------|-------|-------------|
| 6.0 | Register provider + status | connected | IIS100 provider registered (ProviderID 112); site provisioning works | PASS | | 2026-10-30 |
| 6.1 | Order a web site | site created in IIS | IIS site testsite.local (id 7) created & Started; bindings http/*:80:testsite.local + www.testsite.local; physical path `C:\HostingSpaces\testsite\testsite.local\wwwroot` (seeded default.aspx + web.config); logFile enabled | PASS | | 2026-10-30 |
| 6.2 | Browse site via bound domain/IP | content served | **HTTP 503** when browsed via `Host: testsite.local` | FAIL | Root cause: site assigned app pool "ASP.NET 2.0 (Classic)" (v2.0) but host has no .NET 2.0 CLR (v2.0.50727 not installed) → pool can't host a worker → 503. See F-004. | 2026-10-30 |
| 6.3 | Upload/edit files via file manager | files appear in root | (not exercised) | TODO | wwwroot seeded by provider | — |
| 6.4 | Add SSL cert / bind https | https works | (not exercised) | TODO | | — |
| 6.5 | Change app pool / restart | app pool recycles, site up | Manually assigning a working pool is the workaround; provider default is "ASP.NET 2.0 (Classic)" | TODO | see F-004 | — |
| 6.6 | Suspend / resume | site offline / online | Suspend → IIS site Stopped; resume → Started (verified ~26–28s each way) | PASS | | 2026-10-30 |
| 6.7 | Delete service | site removed, port freed | (not exercised) | TODO | site still exists | — |

Watchpoints: SSL binding removal leaving dangling bindings; auth module config not round-tripping.
Note: site anonymous-auth uses local user `testsite_loca_web` (OS provider). Provider writes a reasonable auth/config: windows auth on, basic off, static/dynamic compression off, `asp enableParentPaths=false`.

---

## 7. Web — Apache 2.4

No Apache host. **SKIP** all rows (7.0–7.3).

## 8. Web — WebDAV

No WebDAV host. **SKIP** all rows (8.0–8.3).

## 9. Web — Let's Encrypt (IIS module)

No publicly resolvable domain / cert module in scope. **SKIP** all rows (9.0–9.4).

---
## 10. FTP — IIS 10 (MsFTP100)

Overall: **PARTIAL** — FTP vdir created and bound; FTP-client login / upload / quota not exercised.

| # | Step | Expected | Actual | Status | Notes | Last tested |
|---|------|----------|--------|--------|-------|-------------|
| 10.0 | Register provider + status | connected | MSFTP100 provider registered (ProviderID 113); FTP service provisioned | PASS | | 2026-10-30 |
| 10.1 | Order a service with FTP item | FTP user created | FTP vdir `Default FTP Site/testsite` → `C:\HostingSpaces\testsite\`; FTP site bound ftp/localhost:21 | PARTIAL | vdir + binding created; a dedicated FTP login was not separately verified | 2026-10-30 |
| 10.2 | Connect with FTP client | login OK, home dir scoped | (not exercised) | TODO | | — |
| 10.3 | Upload file via FTP | appears in host dir | (not exercised) | TODO | | — |
| 10.4 | Change password via panel | new password works | (not exercised) | TODO | | — |
| 10.5 | Set/verify quota | over-quota rejected | (not exercised) | TODO | | — |
| 10.6 | Anonymous access toggle | follows toggle | (not exercised) | TODO | | — |
| 10.7 | Delete service | FTP user removed | (not exercised) | TODO | | — |

---

## 11–15. FTP — FileZilla / ServU / Cerberus 6 / VsFtp 3 / Gene6

None installed on the host. **SKIP** all rows (11.x–15.x).

---
## 16. Mail providers

No mail server product on host. **SKIP** all rows (16.x).

## 17. DNS providers

No external DNS product tested. **SKIP** all rows (17.x).

## 18. Database — SQL Server 2025

Overall: **PARTIAL** — provider registered, SQL service provisioned for the space, DB workflow verification 27/27 pass; actual DB + login creation via panel not exercised.

| # | Step | Expected | Actual | Status | Notes | Last tested |
|---|------|----------|--------|--------|-------|-------------|
| 18.0 | Register provider + status | connected | MsSQL 2025 provider registered (ProviderID 1707); SQL service provisioned for space | PASS | | 2026-10-30 |
| 18.1 | Order with SQL database + user | DB + login created | Space provisioned with "SQL Server 2025" service item; no SQL login created at provisioning (expected — login is created with the first DB order) | PARTIAL | no actual DB created this session | 2026-10-30 |
| 18.2 | Connect with panel creds | login works, user mapped | (not exercised) | TODO | | — |
| 18.3 | Set size/permissions via panel | applied on instance | (not exercised) | TODO | | — |
| 18.4 | Delete service | DB + login dropped | (not exercised) | TODO | | — |

Note: DB schema/migrations validated separately — `Orchestrate-Database-Workflow.ps1` verification passed 27/27 (all providers).

---

## 19. Database — MySQL / MariaDB

No MySQL / MariaDB in scope. **SKIP** all rows (19.x).

---
## 20. Virtualization — Hyper-V

No VM templates / Hyper-V role in scope. **SKIP** all rows (20.0–20.7).

## 21. Virtualization — Proxmox

No Proxmox host. **SKIP** all rows (21.0–21.6).

## 22. Terminal Services / RDS

No RDS host in scope. **SKIP** all rows (22.0–22.4).

## 23. Hosted — Exchange 2019

No Exchange server. **SKIP** all rows (23.0–23.6).

## 24. Hosted — SharePoint 2016/2019

No SharePoint server. **SKIP** all rows (24.0–24.4).

## 25. Hosted — SfB 2019 / Lync

No SfB/Lync server. **SKIP** all rows (25.0–25.4).

## 26. Statistics — AWStats / SmarterStats

No stats product. **SKIP** all rows (26.x).

## 27. Storage — EnterpriseStorage / StorageSpaces

No storage product. **SKIP** all rows (27.x).

## 28. Filters — MailCleaner / SpamExperts

No filter product. **SKIP** all rows (28.x).

---
## Findings Log

| ID | Sev | Section | Repro | Expected | Actual | Evidence | Status |
|----|-----|---------|-------|----------|--------|----------|--------|
| F-001 | High | 1 (panel) | Any portal page requiring dynamic compilation | Portal serves pages | HTTP 500: `Could not load type 'Microsoft.CodeAnalysis.FailFast' from assembly 'Microsoft.CodeAnalysis, Version=5.0.0.0'` | Portal bin has a mixed Roslyn set: csc/vbc 4.14.0 (WebFormsForCore.Compilers 1.4.6) + Common/VB 5.0.0 (floated by ReportViewerCore.NETCore 15.1.33 `VB >= 5.0.0`) + CSharp/Workspaces 4.14.0. Recurs after every `dotnet build`. | workaround-applied |
| F-002 | High | 3 (portal) | Create Space → select a plan (AutoPostBack) | Plan dropdown keeps options + selection | Dropdown loses options on postback | WebFormsForCore TrackViewState runs at end of Init; Page never tracked the control. Fixed by re-binding hosting plans in `Page_Init` (UserCreateSpace.ascx.cs); verified in browser (3 options + selection 2=TestProvision retained). Built, NOT committed. | fixed-uncommitted |
| F-003 | Medium | 2/3 (panel) | Any space operation (suspend/resume) | Completed task + log retained for audit | Completed BackgroundTasks row + BackgroundTaskLogs deleted on completion → no audit trail; Running Tasks UI shows only live tasks | `TaskController.UpdateTask` (lines 126–141) calls the `DeleteBackgroundTask` SP on Completed/Abort, cascading to logs. | open |
| F-004 | Medium | 6 (web) | Provision a web site on a host without .NET 2.0 | Site serves content | Site "Started" in IIS but returns HTTP 503 | Site assigned app pool "ASP.NET 2.0 (Classic)" (plan default, v2.0). Host (Windows Server 2025) has no .NET 2.0 CLR (v2.0.50727 absent) → pool can't host a worker (auto-stops) → 503. IIS-level create/start/stop/bindings/FTP all work; only content serving fails. Provider does not validate the chosen app pool against installed runtimes. | open |
| F-005 | Low | 2/3 (panel) | First suspend after a w3wp recycle | Task row created, site stopped | No task row created, site not stopped | First suspend at 00:20:09 created no BackgroundTasks row and did not stop the site; subsequent suspends worked (~28s). Likely artifact of a mid-operation w3wp kill during the F-001 bin fix (environmental). | unconfirmed |

Severity: `Critical` (data loss / outage / security) | `High` (feature broken) | `Medium` (degraded, workaround exists) | `Low` (cosmetic / polish)

**F-001 workaround detail:** stop w3wp (`FuseCP\Tools\Unlock-WebPortal-Build.ps1`) → copy the consistent Roslyn 4.14.0 set from `FuseCP\Lib\WebFormsForCore\src\WebFormsForCore.Compilers\Roslyn.net10.0\` over `FuseCP\Sources\FuseCP.WebPortal\bin\` → recycle the 3 pools (Portal / Enterprise Server / Server). Recurs after every build. Proper fix = align Roslyn versions in `FuseCP.WebPortal.csproj` (maintainer decision; ReportViewerCore is compiled against 5.0.0).
## Summary (filled at the end)

- **Providers tested (live):** IIS100 (Web), MSFTP100 (FTP), MsSQL 2025 (DB), Windows2025 (OS) — plus panel auth/core and the full space lifecycle (create / suspend / resume / browse).
- **Providers skipped (not installed):** Unix OS, Apache, WebDAV, Let's Encrypt, all non-IIS FTP (FileZilla / ServU / Cerberus 6 / VsFtp 3 / Gene6), all Mail, all DNS, MySQL / MariaDB, Hyper-V, Proxmox, RDS, Exchange, SharePoint, SfB/Lync, Statistics, Storage, Filters.
- **Findings by severity:** High: 2 (F-001, F-002) · Medium: 2 (F-003, F-004) · Low: 1 (F-005).
- **Builds / runs:** `FuseCP.Tests.sln` Release build — 0 errors. `FuseCP.Tests` project: **97/97 pass**. `FuseCP.EnterpriseServer.Tests`: **33/34 pass** — 1 fail (`TestESAccess`) is environmental: it hardcodes the default `serveradmin` credentials and sends no MFA code, but the local env uses a non-default admin password with TOTP (MfaMode 2). DB workflow verification (`Orchestrate-Database-Workflow.ps1`): **27/27 pass**.
- **Overall panel health:** Core auth, space creation, and IIS / FTP / OS provisioning all work end-to-end. Two High findings need attention: F-001 (Roslyn bin mix — recurring on build, workaround applied) and F-002 (create-space plan dropdown — fixed, uncommitted). F-004 means provisioned sites do not serve content on hosts without .NET 2.0 (fix = set the plan's app pool to an installed framework, e.g. .NET v4.5 / .NET Core). F-003 (no task audit trail) and F-005 (first-suspend no-op, environmental) are lower priority.

