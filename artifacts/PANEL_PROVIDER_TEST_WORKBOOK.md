# FuseCP Panel & Provider — Manual Test Workbook

**Date:** 2026-10-29 / 2026-10-30 / 2026-10-31
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
| 4 | OS — Windows (host is 2025) | Yes | PARTIAL | 2026-10-30 |
| 5 | OS — Unix (systemd/openrc/launchd) | No | SKIP | — |
| 6 | Web — IIS 10 | Yes | PARTIAL | 2026-10-30 |
| 7 | Web — Apache 2.4 | No | SKIP | — |
| 8 | Web — WebDAV | No | SKIP | — |
| 9 | Web — Let's Encrypt (IIS module) | No | SKIP | — |
| 10 | FTP — IIS 10 (MsFTP100) | Yes | PARTIAL | 2026-10-30 |
| 11 | FTP — FileZilla | No | SKIP | — |
| 12 | FTP — ServU | No | SKIP | — |
| 13 | FTP — Cerberus 6 | No | SKIP | — |
| 14 | FTP — VsFtp 3 | No | SKIP | — |
| 15 | FTP — Gene6 | No | SKIP | — |
| 16 | Mail — SmarterMail 100.x+ | Yes | PASS (F-012 fixed + validated 2026-10-31) | 2026-10-31 |
| 17 | DNS — Windows DNS (MsDNSPS) | Yes | PARTIAL | 2026-10-30 |
| 18 | Database — SQL Server 2025 | Yes | PASS (F-014/F-015 fixed + validated 2026-10-31; F-016 open) | 2026-10-31 |
| 19 | Database — MariaDB 12.2 | Yes | PASS (F-009 fixed + validated 2026-10-30) | 2026-10-30 |
| 20 | Virtualization — Hyper-V 2025 | Yes (role + template) | PASS (F-011 fixed 2026-10-30) | 2026-10-30 |
| 21 | Virtualization — Proxmox | No | SKIP | — |
| 22 | Terminal Services / RDS (2012–2025) | No | SKIP | — |
| 23 | Hosted — Exchange 2019 | No | SKIP | — |
| 24 | Hosted — SharePoint 2016/2019 | No | SKIP | — |
| 25 | Hosted — SfB 2019 / Lync | No | SKIP | — |
| 26 | Statistics — AWStats / SmarterStats | No | SKIP | — |
| 27 | Storage — EnterpriseStorage / StorageSpaces | No | SKIP | — |
| 28 | Filters — MailCleaner / SpamExperts | No | SKIP | — |

Environment notes: providers registered in DB (Providers table) include many products not installed on this host; IIS100 (Web, ID 112), MSFTP100 (FTP, ID 113), MsSQL 2025 (ID 1707), Windows2025 (OS, ID 1804/1805), MsDNSPS (DNS, ID 1902 — host runs the Windows DNS Server role), MariaDB (ID 1586 — host runs MariaDB 12.2), HyperV2025 (ID 1805 — host has the Hyper-V role + template `C:\HyperV\Templates\blank.vhdx`) and SmarterMail 100.x+ (ID 67, ServiceID 11 — host runs SmarterMail 100.0.9784.23326 on `http://localhost:9998`) were exercised or provisioned against the live host.

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

Overall: **PARTIAL** — serveradmin login + space lifecycle exercised; full admin CRUD (users / customers / plans / announcements / server management) not tested this session. (Orders / billing / tickets features are no longer part of the panel — support tickets are handled by a separate system.)

| # | Step | Expected | Actual | Status | Notes | Last tested |
|---|------|----------|--------|--------|-------|-------------|
| 2.1 | Users: create user, assign role, disable | User works / is blocked | (not exercised) | TODO | | — |
| 2.2 | Customers: create customer + login | Customer can log in | testsite (UserID 5) exists but was not created this session | TODO | | — |
| 2.3 | Products/plans: create a plan | Plan saves with item list | PlanID 2 exists (used by test space) but not created this session | TODO | | — |
| 2.4 | Suspend customer or hosting space | Services suspended (offline) | Space-level suspend: IIS site testsite.local → Stopped, panel → Suspended; propagation ~28s | PARTIAL | via SpaceDetails "Suspended" button (WCF ChangePackageStatus, async) | 2026-10-30 |
| 2.5 | Resume customer or hosting space | Services back online | Space-level resume: IIS site → Started, panel → Active; propagation ~26s | PARTIAL | via SpaceDetails "Active" button | 2026-10-30 |
| 2.6 | Announcements: publish + dismiss | Shows to customers | (not exercised) | TODO | | — |
| 2.7 | Admin → Servers: add server | Server added, handshake OK | (server already registered: ServerID 1) | TODO | | — |
| 2.8 | Server: change credentials | New creds used next op | (not exercised) | TODO | | — |
| 2.9 | Server: delete server | Removed; services flagged | (not exercised) | TODO | | — |

Note: space lifecycle (create / suspend / resume) is the customer-facing space flow — see section 3.

---

## 3. Panel — Customer Portal

Overall: **PARTIAL** — create space + space details + suspend/resume verified (performed while logged in as serveradmin); file manager / mail / DNS / profile not exercised.

| # | Step | Expected | Actual | Status | Notes | Last tested |
|---|------|----------|--------|--------|-------|-------------|
| 3.0 | Create space (new hosting package) | Space created, services provisioned | Create Space page: plan dropdown populated, selected TestProvision, submitted → SpaceID 4; IIS site + FTP vdir + physical dir + 4 services provisioned | PASS | F-002 fix (Page_Init rebind) verified in browser | 2026-10-30 |
| 3.1 | Service list + detail pages | All services visible w/ correct status | SpaceDetails shows space status + provisioned services (System, SQL Server 2025, Web Sites 10, FTP); space home reachable | PASS | | 2026-10-30 |
| 3.2 | Start / stop / restart a service | Job completes, status reflects | Suspend/resume via SpaceDetails status buttons works (site state follows); restart not separately tested | PARTIAL | see F-003 / F-005 for task/audit gaps | 2026-10-30 |
| 3.3 | File manager: upload / edit / delete | Changes visible in host dir | (not exercised) | TODO | wwwroot was seeded by provider | — |
| 3.4 | E-mail tab: change mail password | New password works | (no mail product on host) | SKIP | | — |
| 3.5 | DNS tab: add / remove a record | Record visible in zone | Add A record verified end-to-end (see 17.2); remove not re-verified | PARTIAL | see section 17 | 2026-10-30 |
| 3.6 | Account: change profile / password | Works; re-login if expected | (not exercised) | TODO | | — |

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

Overall: **PASS** — SmarterMail 100.0.9784.23326 installed locally (IIS site `SmarterMail`, `http://localhost:9998`); provider `SmarterMail 100.x +` registered (ProviderID 67, ServiceID 11, admin login). Full provider lifecycle validated end-to-end **2026-10-31** both **directly** (deployed provider DLL, no WCF) and **through the live FuseCP Server WCF over IIS** (`http://127.0.0.1:9003`, HMAC-only auth) — **29/29 checks pass**. The 9770 `clientId`/session fix (F-012) and the two 100.0.9784 API adaptations are live-validated. Panel-level mail ordering is not exercised (no service item bound to the test space).

| # | Step | Expected | Actual | Status | Notes | Last tested |
|---|------|----------|--------|--------|-------|-------------|
| 16.0 | Register provider + status | connected | ProviderID 67 "SmarterMail 100.x +" registered; ServiceID 11 (ServiceUrl `http://localhost:9998`, admin login); SmarterMail 100.0.9784.23326 answering on :9998 | PASS | | 2026-10-31 |
| 16.1 | Auth / 9770 session handling | login issues refresh token; sessions terminated cleanly | Pre-fix: login sent no `clientId` → `refreshToken` empty (9770 root cause). Post-fix: per-instance `clientId` on every login → refresh token returned; admin session logged out after each provider command (token → HTTP 401 afterward); unrelated sessions untouched (HTTP 200) | **PASS** | F-012 fix (`f0fe245d0`) validated live | 2026-10-31 |
| 16.2 | GetDomains | list of domains | empty when no domains; correct list after domain create | PASS | direct + WCF | 2026-10-31 |
| 16.3 | CreateDomain | domain exists on SmarterMail | `domain-put` + `domain-settings` → `smailtest.local` (direct) / `smailwcf.local` (WCF) created; `DomainExists` true | PASS | max-1-domain license respected (cleanup between runs) | 2026-10-31 |
| 16.4 | GetDomain | settings mapped | name + limits/flags mapped correctly | **PASS** | pre-fix FAIL — lost `(int)` casts (F-012) | 2026-10-31 |
| 16.5 | CreateAccount | mailbox created | `user-put` via domain impersonation + `auto-responder` via user impersonation; `AccountExists` true | **PASS** | pre-fix FAIL — null `forwardList` rejected by 9784 (F-012) | 2026-10-31 |
| 16.6 | GetAccounts / GetAccount | mailbox listed + full read | `list-users` contains the account; `GetAccount` returns full settings | **PASS** | pre-fix FAIL — lost casts (F-012) | 2026-10-31 |
| 16.7 | UpdateAccount | password change applied | `post-user` password change succeeds; account still present | PASS | | 2026-10-31 |
| 16.8 | DeleteAccount | mailbox removed | `user-delete` via domain impersonation; `AccountExists` false | PASS | | 2026-10-31 |
| 16.9 | DeleteDomain | domain removed | domain gone; `GetDomains` empty | PASS | | 2026-10-31 |
| 16.10 | Domain settings visible in SmarterMail admin UI | limits set via the provider appear in the admin console | Domain `smverify.local` created via the provider with userLimit 7 / aliases 3 / lists 2 / 150 MB. Admin console → Manage → smverify.local shows **Users 2/7**, **Mailing Lists 0/2**, **Disk 0.600 KB / 150 MB**; domain-configuration shows **Domain Aliases 3**, **Users 7**, **Disk Space 150 MB** | **PASS** | pre-fix the alias limit was lost at create (9784 `domain-put` ignores `domainAliasCount`) — F-013 fix (`4ff3fb6ca`) validated: admin UI now shows 3 | 2026-10-31 |
| 16.11 | User settings visible + mailbox works | user created via the provider shows correct settings in the SmarterMail UI and can send/receive | `verifyuser@smverify.local` created via the provider (fullName "Verify User", 200 MB quota, auto-responder ON, signature). Verified in the SmarterMail UI: profile **Display Name "Verify User"** (First "Verify" / Last "User"), auto-responder **ON** (subject "Away - smverify", body "Out of office (smverify verification)"), signature **"SM-verify signature 20261031"**, quota **1 KB / 200.0 MB (0%)**. Webmail login succeeds; an authenticated SMTP send to the address is accepted (`235`/`250 OK`) and the message **arrives in the webmail inbox**; unauthenticated relay is correctly refused (`550 Authentication is required for relay`) | **PASS** | full create→visible→delivered E2E, 2026-10-31 | 2026-10-31 |

Note: all checks run via the `.kilo/smartermail-test` harness (untracked, not committed). Part A = direct provider on the deployed DLL (login/impersonation/logout semantics + full domain + account CRUD); Part B = the same CRUD through the live `FuseCP.Server` WCF (IIS, HMAC-only auth); Part C = create a domain + user with distinct settings, leave them in place, and confirm them in the SmarterMail admin UI + webmail (16.10/16.11). SmarterMail panel flows (ordering a mail domain/mailbox via the portal) were not exercised — no service item is bound to the test space.

## 17. DNS — Windows DNS (MsDNSPS)

Overall: **PASS** — MsDNSPS provider (ID 1902) provisioned for space vServer01 (ServiceID 6). Zone creation via portal verified end-to-end after the F-006 fix (17.1 PASS: `Domains.ZoneItemID` set, DNSZone ServiceItem registered, host zone with SOA/NS). DNS record add + remove verified end-to-end (17.2/17.3 PASS). Domain detach (17.4) validated after the F-007 fix: the SpaceDomains page loads cleanly (no "There was an error while loading module", no NRE), the domain grid renders, and Detach correctly returns a business error (`ERROR_DOMAIN_POINTS_TO_WEB_SITE`) because sibling domains (DomainID 2, 3) point to WebSiteID 13. The original F-007 portal/ES defect (stored-proc truncation + NRE) is resolved.

| # | Step | Expected | Actual | Status | Notes | Last tested |
|---|------|----------|--------|--------|-------|-------------|
| 17.0 | Register provider + status | connected | MsDNSPS provider registered (ProviderID 1902); DNS service provisioned for vServer01 space | PASS | | 2026-10-30 |
| 17.1 | Add domain with DNS (zone create) | zone created on host, NS/SOA set, service item registered | Zone `dnstest3.local` created on host (Primary, IsAutoCreated=False) after the F-006 fix: `Domains.ZoneItemID=23`, DNSZone ServiceItem ItemID=23 (type 12, service 6, pkg 2), host zone SOA `fcp-dev003.` / `hostmaster.` + provider timers 900/600/86400/3600, NS ns1/ns2 | **PASS** | F-006 fix (`Add_DnsServerPrimaryZone` mismatch→`pwsh` fallback) validated end-to-end 2026-10-30 | 2026-10-30 |
| 17.2 | Add DNS record (A) | record appears in zone, resolves | A record `test-a` → 192.0.2.100 added via portal on `dev03-vserver01.local`; verified with `Resolve-DnsName` | PASS | | 2026-10-30 |
| 17.3 | Remove DNS record | record gone from zone | A record `test` → 192.0.2.1 added via portal on `dnstest3.local`, verified present on host (`dnscmd /ZonePrint`), then removed via portal (confirm dialog); verified gone on host (serial bumped to 6, only NS+SOA remain) | **PASS** | end-to-end via portal 2026-10-30 | 2026-10-30 |
| 17.4 | Delete domain (detach) | page loads, domain listed, detach returns proper result | **PASS.** After the F-007 fix (stored-proc `nvarchar(2500)`→`nvarchar(4000)` + NRE null-guard in `PackageController.GetPackageSettings`), the SpaceDomains page (`Default.aspx?pid=SpaceDomains&SpaceID=2`) loads cleanly — no "There was an error while loading module", no 500, no NRE. Domain `dev03-vserver01.local` (DomainID 1) appears in the grid with a Detach action. Detach attempted → confirm dialog → postback → page reloaded; domain persists in DB (correct: sibling domains DomainID 2, 3 point to WebSiteID 13 → `ERROR_DOMAIN_POINTS_TO_WEB_SITE`). A full detach (host zone removal + DB row deletion) requires a domain whose siblings have no web-site associations, which does not exist on this test space. | **PASS** | F-007 fix (stored-proc truncation + NRE null-guard) validated end-to-end 2026-10-30; the original "There was an error while loading module" error is gone | 2026-10-30 |

Note: SOA/NS defaults left by the zone creation are a known limitation of the in-process CIM failure (F-006) — the provider's `UpdateSoaRecord`/NS-cleanup steps are now best-effort with logging, so they no longer abort the flow.

## 18. Database — SQL Server 2025

Overall: **PASS** — MsSQL 2025 provider (ID 1707) registered, SQL service provisioned for space 4. Panel **create user + default-DB + per-DB grant (18.1)**, **connect (18.2)**, **permissions/db_owner (18.3)**, and **delete user (18.4)** all verified end-to-end on the host (2026-10-31, post F-014/F-015 fix). **F-014 (High):** provider `CreateUser` could not create the login — `CREATE LOGIN` requires a string-literal password (a parameter fails with Msg 102) — fixed with `CREATE LOGIN … WITH PASSWORD=<literal>` via `QuoteSqlStringLiteral`. **F-015 (High):** the panel save lost the default-database selection on the postback (WFFC TrackViewState class, same as F-009) → the login was created with `DEFAULT_DATABASE=master`; fixed by reading the raw posted value from `Request.Form` in `MSSQL_EditUser.SaveItem`. **F-016 (Medium, open):** the space user list's **Detach** link is a no-op (swallows the `DeletePackageItem` failure, removes neither the host login nor the panel metadata); the per-user detail-page **Delete** button is the working path (drops host login + DB user + schema + panel metadata). Rows below reflect the post-fix (PASS) results.

| # | Step | Expected | Actual | Status | Notes | Last tested |
|---|------|----------|--------|--------|-------|-------------|
| 18.0 | Register provider + status | connected | MsSQL 2025 provider registered (ProviderID 1707); SQL service provisioned for space 4 | PASS | | 2026-10-30 |
| 18.1 | Create SQL DB + user via panel | DB + login created on host | DB `dbtest31` (ServiceItem 30) + login `dbtest31_user` created via the portal with **Default database = dbtest31** selected and the `dbtest31` DB ticked. Host verified: `sys.sql_logins` → `dbtest31_user` with `default_database_name = dbtest31` (pre-F-015: `master`); `dbtest31.sys.database_principals` → the DB user exists | **PASS** | F-014 (provider CREATE LOGIN) + F-015 (default-DB selection) fixed + live-verified 2026-10-31 | 2026-10-31 |
| 18.2 | Connect with panel creds | login works, user mapped | `sqlcmd -U dbtest31_user -P TestPass123` (no `-d`) → lands in **dbtest31** (the default DB); `SUSER_SNAME()` = `dbtest31_user`; DML against `dbtest31` succeeds (db_owner) | **PASS** | login + default-DB + access live-verified on host 2026-10-31 | 2026-10-31 |
| 18.3 | Set size/permissions via panel | applied on instance | User granted **db_owner** on `dbtest31` (host `sys.database_role_members` → `db_owner` / `dbtest31_user`); default DB set to `dbtest31` (size/quota not separately exercised) | **PASS** | db_owner + default DB verified on host 2026-10-31 | 2026-10-31 |
| 18.4 | Delete user via panel | login + DB user dropped | Detail-page **Delete** button (confirm) → host `sys.sql_logins` + `dbtest31.sys.database_principals` + schema all empty; panel `ServiceItems`/`ServiceItemProperties` ItemID 34 removed; list shows "No items found" (0 of 2) while the `dbtest31` DB (ItemID 30) remains. Note: the space list's **Detach** link is a no-op (F-016) | **PASS** | delete via detail-page Delete button (working path) live-verified 2026-10-31; list Detach broken (F-016, open) | 2026-10-31 |

Note: DB schema/migrations validated separately — `Orchestrate-Database-Workflow.ps1` verification passed 27/27 (all providers).

---

## 19. Database — MySQL / MariaDB

Overall: **PASS** — MariaDB 12.2 installed; provider registered (ProviderID 1586), service provisioned (ServiceID 10); existing DB `db01` (item 19). Panel **create-DB (19.1)**, **create-user + per-DB grant (19.1/19.2)**, and **delete (19.3)** verified end-to-end on the host (2026-10-30, post F-009 fix). **F-009 (High):** panel user/login creation creates the login but does **not** apply the per-database GRANT — the created login can authenticate (correct password) but is denied access to its database (`ERROR 1044 Access denied ... to database`); with a manual `GRANT ALL` the *same* login connects and DML works, proving the credentials are correct and the only gap is the missing grant. Reproduced on both the create and edit paths (19.2 FAIL via panel). **Update (2026-10-30):** F-009 root cause pinned to the WFFC ViewState-timing class (CheckBoxList bound in `Page_Load` is empty on the save postback) and a `Page_Init` re-bind fix applied to `SqlEditUser.ascx.cs` (built, 0 errors; portal recovered from the recurring F-001 Roslyn bin-mix). **Live re-test completed 2026-10-30** (portal login serveradmin + TOTP): created DB `dbtest21` + user `dbtest21_user` with the DB ticked via the portal → host `SHOW GRANTS` shows `GRANT ALL PRIVILEGES ON dbtest21.*` (pre-fix: only `GRANT USAGE ON *.*`); the login connects to `dbtest21` and `CREATE`/`INSERT`/`SELECT`/`DROP` all succeed; the edit path was verified too (user edit adding `db01` → `GRANT ALL ON db01.*` applied). Test artifacts were then removed via the panel (19.3 flow) and confirmed gone on the host. The rows below reflect the post-fix (PASS) results; pre-fix FAIL evidence is in F-009.

| # | Step | Expected | Actual | Status | Notes | Last tested |
|---|------|----------|--------|--------|-------|-------------|
| 19.0 | Register provider + status | connected | MariaDB provider registered (ProviderID 1586); service provisioned for vServer01; existing DB item `db01` (ServiceItem 19) | PASS | | 2026-10-30 |
| 19.1 | Create DB + login via panel | DB + login created on host | (post-fix re-test 2026-10-30) DB `dbtest21` created on host (`SHOW DATABASES`). Login `dbtest21_user@%` created with the correct password (`TestPass123`) **and `GRANT ALL PRIVILEGES ON dbtest21.*` applied** (host `SHOW GRANTS` verified). Pre-fix (`dbtest20`): only `GRANT USAGE ON *.*` — see F-009 | **PASS** | DB create ✓ · login create ✓ (credentials correct) · per-DB grant ✓ (F-009 fix live-verified) | 2026-10-30 |
| 19.2 | Connect with panel creds | login works, user mapped | (post-fix re-test 2026-10-30) `mariadb.exe -u dbtest21_user -pTestPass123 -h localhost dbtest21` → `SELECT CURRENT_USER()` → `dbtest21_user@%`; `CREATE`/`INSERT`/`SELECT`/`DROP` all succeed. Edit path verified too: user edit adding `db01` → `GRANT ALL PRIVILEGES ON db01.*` applied on host, login connects to `db01`. Pre-fix: `ERROR 1044 Access denied` — see F-009 | **PASS** | login + per-DB access live-verified on host after F-009 fix (create + edit paths) | 2026-10-30 |
| 19.3 | Delete DB via panel | DB + login dropped on host | DB `dbtest20` dropped on host (detail-page **Delete** + confirm → 302; `SHOW DATABASES` no longer lists it). Login `dbtest20_user` removed via its own **Delete** (host `mysql.user` no longer lists it) | **PASS** | delete via detail-page Delete button + confirm; both items verified gone on host | 2026-10-30 |

---
## 20. Virtualization — Hyper-V

Overall: **PASS (list + start/stop + create + delete)** — Hyper-V role installed; HyperV2025 provider registered (ProviderID 1805), service provisioned (ServiceID 9). Panel list, start/stop, **create**, and **delete** all verified end-to-end on the host (2026-10-30): the "Create VPS" wizard (F-010) is fixed and provisions a new VM (e.g. `vmtest4.local`, blank - 1st Gen, 1 CPU / 512 MB / 20 GB) that boots **Running** on the host, and Delete (F-011) now removes the VM + its VHD from the host and clears the panel item. **No remaining findings in this section** (F-010 and F-011 both fixed + validated 2026-10-30).

| # | Step | Expected | Actual | Status | Notes | Last tested |
|---|------|----------|--------|--------|-------|-------------|
| 20.0 | Register provider + status | connected | HyperV2025 provider registered (ProviderID 1805); service provisioned; VM `test03` exists on host | PASS | | 2026-10-30 |
| 20.1 | List VMs via panel | VMs shown | Panel lists `test03.vServer01Usr.local` (ItemID 18, Private IP 172.16.0.1, Delete/Reinstall/Move/Detach actions) — matches host `Get-VM` | PASS | `Default.aspx?pid=SpaceVPS2012&SpaceID=2` | 2026-10-30 |
| 20.2 | Start / stop VM via panel | host state follows | Panel **Start** → host `Running` (Uptime 00:00:34, panel reflects Running); panel **Turn Off** (confirm) → host `Off`. Both verified with `Get-VM` | PASS | `vps_general` detail page, ItemID 18 | 2026-10-30 |
| 20.3 | Delete VM via panel | VM removed from host | **PASS.** Delete (checkbox "Delete this Virtual Server" confirmed, "Do not delete VPS files" left off) removes the VM from the host — `Get-VM` returns nothing for the VM and the VHD file is gone — and clears the panel item. Verified twice on 2026-10-30: a fresh VM (`vmtest4.local`, created → Running → deleted) and a previously-stuck Error-state VM (`vmtest3.local`). See F-011 (fixed). | PASS | `vps_tools_delete` → `btnDelete`; provider `HardDriveHelper.Delete` local path uses `File.Delete` (root-cause fix, F-011) | 2026-10-30 |
| 20.4 | Create VM via panel | new VM provisioned | **PASS.** "Create VPS" wizard completes end-to-end (F-010 fixed): step 1 Name & OS (hostname `vmtest3.local`, OS `blank - 1st Gen`, generated password) → Next → step 2 Configuration (CPU 1, RAM 512, HDD 20) → Next → Summary (OS/CPU/RAM/HDD all correct) → Finish. New VM `vmtest3.local` (ItemID 26) created on host, state **Running**, 512 MB RAM, VHD expanded 1 GB→20 GB; space quotas updated (VPS 1→2, CPU 1→2, RAM 512→1024, HDD 100→120). Verified via `Get-VM`. | PASS | `vdc_create_server` wizard | 2026-10-30 |

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
| F-002 | High | 3 (portal) | Create Space → select a plan (AutoPostBack) | Plan dropdown keeps options + selection | Dropdown loses options on postback | WebFormsForCore TrackViewState runs at end of Init; Page never tracked the control. Fixed by re-binding hosting plans in `Page_Init` (UserCreateSpace.ascx.cs); verified in browser (3 options + selection 2=TestProvision retained). Committed as `295fb7655`. | fixed-committed |
| F-003 | Medium | 2/3 (panel) | Any space operation (suspend/resume) | Completed task + log retained for audit | Completed BackgroundTasks row + BackgroundTaskLogs deleted on completion → no audit trail; Running Tasks UI shows only live tasks | `TaskController.UpdateTask` (lines 126–141) calls the `DeleteBackgroundTask` SP on Completed/Abort, cascading to logs. | open |
| F-004 | Medium | 6 (web) | Provision a web site on a host without .NET 2.0 | Site serves content | Site "Started" in IIS but returns HTTP 503 | Site assigned app pool "ASP.NET 2.0 (Classic)" (plan default, v2.0). Host (Windows Server 2025) has no .NET 2.0 CLR (v2.0.50727 absent) → pool can't host a worker (auto-stops) → 503. IIS-level create/start/stop/bindings/FTP all work; only content serving fails. Provider does not validate the chosen app pool against installed runtimes. | open |
| F-005 | Low | 2/3 (panel) | First suspend after a w3wp recycle | Task row created, site stopped | No task row created, site not stopped | First suspend at 00:20:09 created no BackgroundTasks row and did not stop the site; subsequent suspends worked (~28s). Likely artifact of a mid-operation w3wp kill during the F-001 bin fix (environmental). | unconfirmed |
| F-006 | High | 17 (DNS) | Portal Add Domain with "Enable DNS" on the IIS-hosted stack | zone created, NS/SOA set, service item registered | (before fix) zone created on host but provisioning aborted mid-flow: `Domains.ZoneItemID` NULL, no ServiceItem; task log 3793: CIM `Append` failure. (after fix) **17.1 PASSES**: ZoneItemID=23, DNSZone ServiceItem ItemID=23, host zone with SOA/NS. | **Root cause:** the in-process PowerShell SDK (SMA 7.6.0.500) in the IIS `w3wp` cannot materialize CIM objects that have array out-properties (`DnsServerPrimaryZone.NotifyServers`/`.Secondaries`/`.SecondaryServers`). `Add-DnsServerPrimaryZone` creates the zone on the host first, then throws `MissingMethodException` (`EnumerableExtensions.Append`) on the return; `AddPrimaryZone` had no matching catch, so the exception hit AddZone's catch (swallowed via `TaskManager.WriteError`, no rethrow) → `zoneItemId`=0. **Fix (uncommitted, deployed + VALIDATED 2026-10-30):** `DnsCommands.cs` `Add_DnsServerPrimaryZone` — try in-process → catch `IsEnumerableAppendMismatch` → verify via `ZoneExists` → idempotent `AddDnsServerPrimaryZoneViaPwsh` (`pwsh -EncodedCommand`, skip if already exists) → best-effort `SetDnsServerPrimaryZoneViaPwsh`. Same mismatch→`pwsh` treatment added to `Remove_DnsServerZone` (`RemoveDnsServerZoneViaPwsh`) for the delete path. Built (49152-byte DLL) + deployed to `FuseCP.Server\bin\Providers\DNS`; committed as `c82f3b102` (2026-10-30). | fixed-validated-committed |
| F-007 | High | 17 (DNS) | Portal Domains → Detach a domain | zone removed on host, DB row deleted, page rebinds cleanly | (before fix) Page rendered `There was an error while loading module` (ref `4c2b3459290a40cb8570099df676c1ea`); host zone + DomainID + DNSZone ServiceItem all remained. (after fix) Page loads cleanly, domain grid renders, Detach returns proper business error. | **Root cause (two defects, both fixed + validated 2026-10-30):** (1) `dbo.GetDomainsPaged` declared `@sql nvarchar(2500)`; the POST-case SQL was 2545 chars → truncated → `SqlException: Incorrect syntax near 'S'` on the ObjectDataSource rebind → "There was an error while loading module". Fixed to `nvarchar(4000)` (live DB + source `v1.5.1/StoredProcedures.sql` + `v2.0.0/StoredProcedures` SP sources + 5 generated install/update scripts). (2) NRE at `PackageController.GetPackageSettings` (line 1987): `GetPackage(packageId)` can return null, then `package.ServerId` is dereferenced unconditionally. The `GetPackageSettings` SP walks the parent chain and can return a "Dump" row with `PackageID=1` for a child package, making `settings.PackageId < packageId` and entering the root-settings block where the null dereference occurs. Fixed with a null guard (lines 1984–1989). **Validation:** SpaceDomains page (`Default.aspx?pid=SpaceDomains&SpaceID=2`) loads cleanly (no module error, no 500, no NRE); domain `dev03-vserver01.local` (DomainID 1) appears in the grid; Detach correctly returns `ERROR_DOMAIN_POINTS_TO_WEB_SITE` because sibling domains (DomainID 2, 3) point to WebSiteID 13. Committed as `aef5f7c26` (2026-10-30), which also widened `GetStorageSpacesPaged` and completed the v2.0.0 SP sources so fresh EF installs get the fix. | fixed-validated-committed |
| F-008 | Low | 17 (DNS) | Zone creation (NS/SOA defaults) | provider primary NS + admin contact set in SOA | zone retains default SOA/NS on this host | Documented limitation of the in-process CIM failure (F-006); the provider's `UpdateSoaRecord` is now best-effort with logging so it no longer aborts the flow. | known-limitation |
| F-009 | High | 19 (MariaDB) | Portal MariaDB → Create User, tick a database checkbox, Save | login created **with access** to the selected database (`GRANT ALL ON db.*`) | login created with **only `GRANT USAGE ON *.*`** — no per-DB GRANT; connecting to the database → `ERROR 1044 Access denied ... to database`. **Reproduced on both the create and edit paths.** | **Evidence (2026-10-30):** created `dbtest20` (DB) + `dbtest20_user` (login, pw `TestPass123`) via the portal. `SHOW GRANTS FOR 'dbtest20_user'@'%'` → only `GRANT USAGE ON *.*`; `mariadb.exe -u dbtest20_user -pTestPass123 -h localhost dbtest20` → `1044 Access denied`. After a **manual** `GRANT ALL PRIVILEGES ON dbtest20.* TO 'dbtest20_user'@'%'` (root), the same login connects and `CREATE/INSERT/SELECT/DROP` succeed → credentials are correct; the only gap is the missing per-DB GRANT. **Root-cause trail:** `MariaDB.cs` `CreateUser` (L359) always issues `GRANT USAGE`, then grants per-DB only for `user.Databases` (L378–379 → `AddUserToDatabase` L733). The panel `SqlEditUser.SaveItem` (L162–168) fills `local_item.Databases` from the `dlDatabases` CheckBoxList and `AddSqlUser` (DatabaseServerController.cs:659) passes it to `CreateUser`; yet the grant is never issued → `user.Databases` was empty at `CreateUser` time despite the checkbox being ticked in the browser. Provider `ConnectionString` (L109–115) runs GRANT as the `RootLogin` (has GRANT OPTION), so this is **not** a privilege issue — it points to the panel not capturing the selected databases into the `SqlUser`. Contributing: the `dlDatabases` CheckBoxList sits inside a default-collapsed `CollapsiblePanel` (`UsersPanel Height=0`), and both the `MariaDB_EditUser` and `MySQL_EditUser` portal controls are empty stubs, so the panel relies solely on the main editor's CheckBoxList reading. **Root cause pinned (2026-10-30):** the `dlDatabases` CheckBoxList is `DataBind`-ed in `Page_Load` (i.e. after Init / `TrackViewState`). Under WebFormsForCore, list items bound in `Page_Load` do not reach ViewState, so on the save postback the control is **empty before `ProcessPostData`** → no checkbox values are applied → `local_item.Databases` is empty → `MariaDB.CreateUser` issues only `GRANT USAGE ON *.*`. Same WFFC ViewState-timing class as F-002 / F-010. **Fix (applied 2026-10-30, `SqlEditUser.ascx.cs`):** added a `Page_Init` handler that re-binds `dlDatabases` **only on postback** via `BindDatabases(PanelSecurity.PackageId)` so the items exist before `ProcessPostData` applies the posted checkbox values (mirrors the live-validated F-002 `UserCreateSpace` fix and the pre-existing `HostingPlansEditPlan` pattern). `PanelSecurity.PackageId` is used because `SaveItem` unconditionally sets `local_item.PackageId = PanelSecurity.PackageId` for both add and update, so the bound database list matches what is saved. Applies to both MySQL and MariaDB (shared `SqlEditUser` editor). **Build:** `FuseCP.WebPortalAndEnterpriseServer.sln` 0 errors; portal recovered from the F-001 Roslyn bin-mix that recurred on build (Roslyn realigned to 4.14.0 + 3 pools recycled → HTTP 200). Committed as `f1de53e5c` (2026-10-30). **Live validation (2026-10-30, portal login serveradmin + TOTP):** (1) **create path** — created DB `dbtest21` + user `dbtest21_user` (pw `TestPass123`) with `dbtest21` ticked via the portal; host `SHOW GRANTS FOR 'dbtest21_user'@'%'` → `GRANT USAGE ON *.*` **plus `GRANT ALL PRIVILEGES ON dbtest21.*`** (pre-fix: only `GRANT USAGE`); `mariadb.exe -u dbtest21_user -pTestPass123 -h localhost dbtest21` connects and `CREATE`/`INSERT`/`SELECT`/`DROP` all succeed. (2) **edit path** — user edit adding `db01` → `GRANT ALL PRIVILEGES ON db01.*` applied on host; login connects to `db01`. (3) **cleanup** — user + `dbtest21` deleted via the panel (19.3 flow); both confirmed gone on host (`mysql.user` / `SHOW DATABASES`). | fixed-validated-committed |
| F-010 | High | 20 (Hyper-V) | Portal Hyper-V → "Create VPS" wizard → fill Name & OS (hostname, OS, generated password) → click **Next** | wizard advances to Configuration step | (before fix) **HTTP 500** `System.NullReferenceException` at `VdcCreateServer.ascx.cs` (`listOperatingSystems.SelectedItem.Text` in `BindSummary()`); no new VM could be created. (after fix) wizard completes end-to-end and provisions the VM. | **Root cause (2026-10-30):** the WebFormsForCore `Wizard` does not preserve data-bound `DropDownList` state (items **and** selection) across steps. Two sub-issues: (1) `ddlCpu` (step 2) is bound on initial load while step 1 is active, so it is never rendered → its items never reach ViewState → it is empty when the user reaches the Configuration step; (2) even for step-1 controls, after a step-change postback the `DropDownList` items are not restored, so `SelectedItem`/`SelectedValue` are null when `BindSummary()`/the finish handler read them (TextBox `.Text` survives via posted data, but DDL `SelectedValue` does not). **Fix (deployed + VALIDATED 2026-10-30, committed as `1762751fa`) in `VPS2012/VdcCreateServer.ascx.cs`:** (a) extracted CPU binding into `BindCpuCores()` and re-invoke it from `wizard_ActiveStepChanged` when `ActiveStepIndex == 1` so the CPU dropdown is populated when the Configuration step activates; (b) rewrote `SaveWizardStepState()` to capture values from `Request.Form[control.UniqueID]` (raw posted data, reliable) instead of the controls, with the OS display name resolved from the `LibraryItem[]` `Path`; (c) `BindSummary()` and the finish handler read from `ViewState["…"]` set by (b). **Reproduced the full wizard with a valid hostname + OS + CPU/RAM/HDD → Finish → `vmtest3.local` created on the host and Running (512 MB RAM).** Same latent pattern exists in `VPS/`, `Proxmox/`, `VPSForPC/` `VdcCreateServer.ascx.cs` (not yet replicated; those platforms are SKIP on this host). | fixed-validated |
| F-011 | High | 20 (Hyper-V) | Portal Hyper-V → VM list → Delete (trash) → "Delete this Virtual Server" checkbox (confirmed checked) → **Delete** | VM removed from host + panel item deleted | **FIXED + validated 2026-10-30.** Delete now removes the VM from the host (`Get-VM` returns nothing; VHD file gone) and clears the panel item. Verified on a fresh VM (`vmtest4.local`, created → Running → deleted) and a previously-stuck Error-state VM (`vmtest3.local`). | **Root cause (confirmed via a live file-based diagnostic in `DeleteVirtualMachineInternal`, 2026-10-30):** NOT the F-006 CIM array-out-property class — `GetVirtualMachineExtendedInfo` SUCCEEDS (adapter + disk read OK, Hyper-V CIM cmdlets work). The delete failed at the **disk-delete step**: `HardDriveHelper.Delete` ran the `Remove-item` PowerShell cmdlet via the in-process PowerShell **Core** (pwsh 7) runspace, which resolves `Remove-item` to the Windows PowerShell 5.1 `Microsoft.PowerShell.Management` module → `CommandNotFoundException: Cannot find the built-in module 'Microsoft.PowerShell.Management' that is compatible with the 'Core' edition`. Diagnostic log (`FuseCP.Server\bin\HyperV_DeleteDiag.log`): `GetVirtualMachineExtendedInfo OK → State check Off → adapter deleted → DISK DELETE FAILED (Remove-item module error)`. **Fix (applied, `HardDriveHelper.cs` `Delete(VirtualHardDiskInfo)`):** for the **local** host (Server on the Hyper-V host — `RemoteComputerName` empty) delete the VHD file directly with `System.IO.File.Delete` (guarded by `File.Exists`) instead of the `Remove-item` cmdlet; the **remote** host path (`Invoke-Command -ComputerName`) is kept unchanged since full Windows PowerShell is available there. The temporary diagnostic was removed and the provider rebuilt/redeployed to `bin\Providers\HyperV2025\`. Post-fix diagnostic run shows the full clean flow: `Disks deleted → Snapshots deleted → VirtualMachineHelper.Delete OK`. Committed as `15937ee7b` (2026-10-30). | fixed-validated-committed |

| F-012 | High | 16 (Mail) | Any SmarterMail 100.x provider operation against a 100.0.9784 install | domain/account CRUD works | (1) every login sent no `clientId` → 9770 issued no refresh token and provider sessions broke; (2) `GetDomain`/`GetAccount` threw `RuntimeBinderException` (long→int) — the explicit `(int)` casts were lost in the SolidCP→FuseCP port; (3) `CreateAccount`/`UpdateAccount` failed with HTTP 400 `Value cannot be null. (Parameter 'collection')` — 100.0.9784 rejects a null `forwardList`; (4) `AccountExists` always false — 100.0.9784's `settings/sysadmin/user/{email}` returns `Domain does not exist.` for every input | Live repro against SmarterMail 100.0.9784.23326 on :9998 (2026-10-31). Fixes: per-instance `clientId` on login + logout of the admin session after each command (`f0fe245d0`); restored `(int)` casts on quota fields (`d13fa31ac`); empty-array `forwardList` + `AccountExists` via `settings/sysadmin/list-users/{domain}` (`a3164805f`). Full 29/29 harness pass after the fixes (direct + live WCF). | fixed-validated-committed |

| F-013 | Medium | 16 (Mail) | Create a mail domain via the provider with a domain-alias limit | the alias limit is stored and shown in the SmarterMail admin console | pre-fix: the limit set in the panel was **lost at creation** — SmarterMail 100.0.9784's `settings/sysadmin/domain-put` silently ignores the `domainAliasCount` field, so the domain stored the default alias limit instead of the one the provider sent | Live repro on SmarterMail 100.0.9784.23326 (2026-10-31): provider sent `domainAliasCount=3` on `domain-put` → stored `maxDomainAliases=2` (default); a direct `domain-settings` POST with `maxDomainAliases` **is** honored. **Fix (`4ff3fb6ca`):** `CreateDomain` now includes `maxDomainAliases = domain.MaxDomainAliases` in the `domain-settings` call that immediately follows `domain-put` (the update path already sent it). Validated: admin console → smverify.local → domain-configuration shows **Domain Aliases 3**. | fixed-validated-committed |

| F-014 | High | 18 (SQL Server 2025) | Panel → SQL Server 2025 → Create User (any user) | SQL login created on the host | (before fix) `MsSqlServer.CreateUser` could not create the login: the original `CREATE LOGIN` used a parameterized `PASSWORD` value, but `CREATE LOGIN … WITH PASSWORD=` requires a **string literal** — a parameter fails with `Msg 102 (Incorrect syntax near the parameter)`. | **Root cause:** `CREATE LOGIN`'s `PASSWORD` clause only accepts a quoted string literal; a `SqlParameter` placeholder is not valid T-SQL there. **Fix (applied + live-validated 2026-10-31) in `MsSqlServer.CreateUser`:** `CREATE LOGIN {0} WITH PASSWORD={1}, DEFAULT_DATABASE={2}, CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF` where the password is built with `QuoteSqlStringLiteral(password)` (a properly escaped, quoted literal) and the identifier/DB via `QuoteSqlIdentifier`. Live re-test: `dbtest31_user` login created on the host. | fixed-validated |
| F-015 | High | 18 (SQL Server 2025) | Panel → SQL Server 2025 → Create User → select a Default database + tick a DB → Save | login created with `DEFAULT_DATABASE=<selected DB>` | (before fix) the login was created with `default_database_name = master` — the user's **Default database** selection was lost on the save postback. | **Evidence (2026-10-31):** created `dbtest31_user` with Default database = `dbtest31` selected + `dbtest31` DB ticked → host `sys.sql_logins` showed `default_database_name = master`; the panel stored only the `Password` property (no `DefaultDatabase`). **Root cause (two iterations, 2026-10-31):** `ddlDefaultDatabase` (the provider control's `MSSQL_EditUser` DropDownList) is bound in `Page_Load`; under WebFormsForCore, list items bound after `TrackViewState` are not restored from ViewState, so the posted selection is not applied on the save postback (same WFFC class as F-009). A first fix (re-binding the provider control in `SqlEditUser.Page_Init` on postback) was **insufficient** — the posted value still was not applied. The working fix reads the raw posted value directly from `Request.Form` in `MSSQL_EditUser.SaveItem`, matching the stable trailing field-name segment `ddlDefaultDatabase` (the form field `name` is the control's UniqueID, which can shift between the render and the postback). Live re-test: `dbtest31_user` created with `default_database_name = dbtest31`, connection (no `-d`) lands in `dbtest31`, `db_owner` on `dbtest31`. | fixed-validated |
| F-016 | Medium | 18 (SQL Server 2025) | Panel → SQL Server 2025 space → user list → **Detach** a user | login + DB user dropped on host, panel metadata removed | (before fix) clicking **Detach** on a SQL user in the space list is a **no-op**: the host login, the DB user, and the panel metadata all remain. | **Root cause (2026-10-31):** the space-list Detach routes to `SpaceServiceItems.ascx.cs` → ES `DetachPackageItem` (`PackageController.cs:1775`) → `DeletePackageItem`, but the return value is **ignored** and the method always returns 0; `DeleteServiceItem` (`DataProvider.cs:8359`) is metadata-only, and even the metadata row was not removed (the failure is swallowed). Legacy SolidCP is also metadata-only → a design gap, not a regression. **The working delete path** is the per-user detail-page **Delete** button (`SqlEditUser.btnDelete_Click` → `ES.Services.DatabaseServers.DeleteSqlUser`), which drops the host login + DB user + schema **and** the panel metadata (verified live 2026-10-31: ItemID 34 fully removed, host clean). The SQL user detail page has no separate Delete control in `MSSQL_EditUser.ascx` (unlike MariaDB 19.3) — the delete lives on the main editor. **Status:** open (the list Detach no-op should either be wired to the working delete or removed). | open |

Severity: `Critical` (data loss / outage / security) | `High` (feature broken) | `Medium` (degraded, workaround exists) | `Low` (cosmetic / polish)

**F-001 workaround detail:** stop w3wp (`FuseCP\Tools\Unlock-WebPortal-Build.ps1`) → copy the consistent Roslyn 4.14.0 set from `FuseCP\Lib\WebFormsForCore\src\WebFormsForCore.Compilers\Roslyn.net10.0\` over `FuseCP\Sources\FuseCP.WebPortal\bin\` → recycle the 3 pools (Portal / Enterprise Server / Server). Recurs after every build. Proper fix = align Roslyn versions in `FuseCP.WebPortal.csproj` (maintainer decision; ReportViewerCore is compiled against 5.0.0).
## Summary (filled at the end)

- **Providers tested (live):** IIS100 (Web), MSFTP100 (FTP), MsSQL 2025 (DB), Windows2025 (OS), MsDNSPS (DNS — zone create 17.1 + record add/remove 17.2/17.3 + detach 17.4 all verified end-to-end, see section 17 / F-006 / F-007), **MariaDB 12.2** (section 19 — DB create 19.1 + user create / per-DB GRANT 19.1/19.2 + delete 19.3 all verified on host after the F-009 fix, 2026-10-30), **SmarterMail 100.x+** (section 16 — full domain + account CRUD 16.0–16.9 validated 2026-10-31, direct + live WCF, after the F-012 fix) — plus panel auth/core and the full space lifecycle (create / suspend / resume / browse).
- **Providers tested (live) — Hyper-V 2025 (section 20):** list (20.1 PASS) + start/stop (20.2 PASS) + create (20.4 **PASS** — F-010 fixed, new VM provisioned and Running on the host) + delete (20.3 **PASS** — F-011 fixed, VM + VHD removed from host, panel item cleared) all verified end-to-end on the host (2026-10-30).
- **Providers tested (live) — SQL Server 2025 (section 18, 2026-10-31):** full panel user lifecycle verified end-to-end on the host after the F-014 (provider `CREATE LOGIN` string-literal password) and F-015 (default-database selection lost on postback → read from `Request.Form`) fixes: create user `dbtest31_user` with Default database = `dbtest31` + per-DB tick (18.1 **PASS** — host `default_database_name = dbtest31`, DB user present), connect (18.2 **PASS** — `sqlcmd` with no `-d` lands in `dbtest31`), db_owner grant (18.3 **PASS**), delete via the detail-page Delete button (18.4 **PASS** — host login + DB user + schema + panel ItemID 34 all removed). Remaining open finding: F-016 (Medium) — the space user list **Detach** link is a no-op; the working delete path is the detail-page Delete button.
- **Providers provisioned, panel testing still pending:** (none remaining — all installable providers on this host have been passed through).
- **Providers skipped (not installed):** Unix OS, Apache, WebDAV, Let's Encrypt, all non-IIS FTP (FileZilla / ServU / Cerberus 6 / VsFtp 3 / Gene6), other Mail products (SmarterMail 100.x+ is installed + tested — see section 16 / F-012), Proxmox, RDS, Exchange, SharePoint, SfB/Lync, Statistics, Storage, Filters.
- **Findings by severity:** High: 10 total — open: F-001 (1) · fixed-validated (+ committed where noted): F-002 (`295fb7655`), F-006 (`c82f3b102`), F-007 (`aef5f7c26`), F-009 (`f1de53e5c`), F-010 (`1762751fa`), F-011 (`15937ee7b`), F-012 (`f0fe245d0` + `d13fa31ac` + `a3164805f`, 2026-10-31), F-014, F-015 (SQL Server 2025, fixed + live-validated 2026-10-31) · Medium: 4 (F-003, F-004, F-013 — F-013 fixed + committed as `4ff3fb6ca` 2026-10-31; F-016 — SQL user list **Detach** no-op, open) · Low: 2 (F-005, F-008).
- **DNS pass (2026-10-30, completed):** root-caused the zone-provisioning abort (F-006 — in-process PowerShell SDK in IIS `w3wp` cannot materialize CIM objects with array out-properties) and deployed a `pwsh`-fallback fix to `FuseCP.Server\bin\Providers\DNS` (uncommitted). **Retested 2026-10-30: 17.1 zone create PASS** (ZoneItemID=23, DNSZone ServiceItem 23, host zone with SOA/NS), **17.3 record remove PASS** (added + removed `test` A record, verified on host), **17.4 detach PASS** (F-007 fixed: stored-proc `nvarchar(2500)`→`nvarchar(4000)` + NRE null-guard in `PackageController.GetPackageSettings`; SpaceDomains page loads cleanly, domain grid renders, Detach correctly returns `ERROR_DOMAIN_POINTS_TO_WEB_SITE`). **MariaDB (19) tested 2026-10-30:** all PASS on host after the F-009 fix — DB create 19.1 (login `dbtest21_user` created with `GRANT ALL ON dbtest21.*`, live-verified) + connect 19.2 (login connects, DML works; edit path also verified — adding `db01` via user edit applies its GRANT) + delete 19.3 (user + `dbtest21` removed via the panel, confirmed gone on host). (Pre-fix: login create PARTIAL / connect FAIL on F-009 — missing per-DB GRANT → `1044 Access denied`; fix = `Page_Init` re-bind in `SqlEditUser.ascx.cs`, committed as `f1de53e5c`, live-validated 2026-10-30.) **Hyper-V (20) tested 2026-10-30:** list 20.1 + start/stop 20.2 + create 20.4 + delete 20.3 all **PASS** on the host (verified via `Get-VM` — a new `vmtest4.local` was provisioned via the now-fixed Create-VPS wizard, booted Running, then fully deleted: VM + VHD removed from the host and the panel item cleared). F-006, F-007, F-010, and F-011 are all fixed + validated. All installable providers on this host are now passed through. **SmarterMail (16) tested 2026-10-31:** full provider lifecycle — auth/9770 session handling (16.1), domain CRUD (16.2/16.3/16.4/16.9) and account CRUD (16.5/16.6/16.7/16.8) — validated end-to-end against a live SmarterMail 100.0.9784.23326 install, both directly on the deployed provider DLL and through the live FuseCP Server WCF over IIS (29/29 checks). The 9770 `clientId`/logout fix plus the two 100.0.9784 API adaptations (empty `forwardList`, `AccountExists` via `list-users`) are live-validated (F-012). **Admin-UI + delivery pass (2026-10-31):** a domain + user created via the provider with distinct settings (16.10/16.11) are all shown correctly in the SmarterMail admin console (domain limits 7/3/2/150 MB) and the user UI (display name, 200 MB quota, auto-responder, signature); the mailbox logs into webmail and an authenticated SMTP send is delivered to its inbox. F-013 (domain-alias limit lost at create) found and fixed in this pass (`4ff3fb6ca`).
- **Builds / runs:** `FuseCP.Tests.sln` Release build — 0 errors. `FuseCP.Tests` project: **97/97 pass**. `FuseCP.EnterpriseServer.Tests`: **33/34 pass** — 1 fail (`TestESAccess`) is environmental: it hardcodes the default `serveradmin` credentials and sends no MFA code, but the local env uses a non-default admin password with TOTP (MfaMode 2). DB workflow verification (`Orchestrate-Database-Workflow.ps1`): **27/27 pass**. Rebuild after committing F-006/F-007/F-010/F-011 (2026-10-30): `FuseCP.WebPortalAndEnterpriseServer.sln` 0 errors, `FuseCP.Server.sln` 0 errors. Rebuild after the F-012 SmarterMail fixes (2026-10-31): `FuseCP.Server.sln` 0 errors, 0 warnings (provider `FuseCP.Providers.Mail.SmarterMail100` + full Server solution); the `.kilo/smartermail-test` harness passes 29/29 against the live install. Rebuild after the F-009 fix (2026-10-30): `FuseCP.WebPortalAndEnterpriseServer.sln` 0 errors (the `Page_Init` re-bind in `SqlEditUser.ascx.cs`); portal recovered from the recurring F-001 Roslyn bin-mix (Roslyn realigned to 4.14.0 + 3 pools recycled → HTTP 200). F-009 live re-test (2026-10-30, serveradmin + TOTP): create + edit + delete paths all verified end-to-end on the host (see section 19 / F-009).
- **Overall panel health:** Core auth, space creation, and IIS / FTP / OS / DNS / Hyper-V provisioning all work end-to-end. High finding that needs attention: F-001 (Roslyn bin mix — recurring on build, workaround applied). **Fixed & validated + committed (2026-10-30):** F-006 (DNS zone create — `pwsh`-fallback for in-process CIM array-out-property failure), F-007 (DNS detach — stored-proc `nvarchar(2500)`→`nvarchar(4000)` truncation + NRE null-guard in `PackageController.GetPackageSettings`; SpaceDomains page now loads cleanly and Detach returns proper business errors), F-009 (MariaDB login — the panel was losing the ticked databases on postback so no per-DB GRANT was applied; `Page_Init` re-bind of the database CheckBoxList in `SqlEditUser.ascx.cs`, committed as `f1de53e5c`; live-validated 2026-10-30 on both the create and edit paths — `GRANT ALL ON db.*` now applied, login connects and DML works), F-010 (Hyper-V "Create VPS" wizard 500 on step 1→2 — now provisions a VM end-to-end), and F-011 (Hyper-V Delete-VPS — local VHD deletion via `File.Delete` instead of the `Remove-item` cmdlet, which fails under the in-process PowerShell Core runspace; VM + VHD now removed from the host and the panel item cleared). F-004 means provisioned sites do not serve content on hosts without .NET 2.0 (fix = set the plan's app pool to an installed framework, e.g. .NET v4.5 / .NET Core). F-003 (no task audit trail) and F-005 (first-suspend no-op, environmental) are lower priority.

