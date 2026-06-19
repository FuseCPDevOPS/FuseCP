# LESS / CSS / Bootstrap / UI Workflow

## LESS Source Files (edit these)
- `FuseCP/Sources/FuseCP.WebPortal/App_Themes/Default/Styles/main.less` — all main theme rules
- `FuseCP/Sources/FuseCP.WebPortal/App_Themes/Default/Styles/Menus.less` — navigation and menu rules
- `FuseCP/Sources/FuseCP.WebPortal/App_Themes/Default/Styles/defaultVariables.less` — shared LESS variables
- `FuseCP/Sources/FuseCP.WebPortal/App_Themes/Default/Styles/defaultTheme.less` — root entry point

## Compiled Output (never edit directly)
- `main.css` — generated from LESS sources

## Recompile Command
Run from the Styles directory:
```
cd FuseCP/Sources/FuseCP.WebPortal/App_Themes/Default/Styles
npm run build:css
```

## Rules
- Every CSS change must start in the relevant `.less` source file.
- Direct edits to `main.css` will be overwritten on next recompile and must not be committed.
- Verify output contains expected rules before committing both `.less` and `.css` together.

## Bootstrap Modernization (Bootstrap 3 → 5.3.x)
When modernizing portal UI files:
- Replace deprecated patterns: `panel`, `well`, `input-group-addon`, `btn-default`, `pull-*`, `img-responsive`, `hidden-*`, `visible-*`
- Replace Glyphicons with Bootstrap Icons, preserving accessible labels
- Resolve JS compatibility: remove obsolete plugin calls, adapt data attributes/events to Bootstrap 5 expectations
- Do not break existing portal behaviors, permission checks, or server-side contracts
- Do not remove accessibility semantics during markup refactors
- Keep changes minimal per page while completing full migration from deprecated Bootstrap 3 APIs