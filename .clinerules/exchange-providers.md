# Exchange Provider Patterns

## Provider Parity
Exchange providers for 2013, 2016, and 2019 (`FuseCP.Providers.HostedSolution.Exchange2013/2016/2019`) share identical method structure. Any change to `GetMailbox*`, `SetMailbox*`, or shared helper methods must be applied to **all three providers in the same commit** and all three must be built to confirm no compile regressions.

## Remoted PSObject Type Variance
Exchange PowerShell remoting returns PSObjects whose properties can have unexpected runtime shapes:
- `SmtpAddress` may arrive as a plain string
- Size properties may arrive as `Unlimited<ByteQuantifiedSize>` or as a formatted string
- Boolean properties may arrive as non-bool objects

**Never use direct casts** (`(bool)`, `(Unlimited<int>)`, `(Unlimited<ByteQuantifiedSize>)`) on `GetPSObjectProperty()` results. Use existing safe helpers:
- `ObjToBoolean`
- `ConvertByteSizePropertyToKB`
- `ConvertByteSizePropertyToMB`
- `ConvertUnlimitedIntPropertyToInt32`

## No-Language Runspace Restrictions
Exchange remoting runs in constrained/no-language mode. Setting `ConfirmPreference` and calling `Get-MailboxSearch` can throw "Script invocation is not supported in this session configuration" — always guard such calls with try-catch and provide a fallback code path.

## PSObject Property Access
Prefer `PSObject.Properties["name"]` over `PSObject.Members["name"]` when reading remoted Exchange objects; `Members` can hit script-backed properties that fail in constrained sessions.