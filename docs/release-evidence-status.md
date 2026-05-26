# Release Evidence Status

Current target: `Spire Plus` manual-test build for user validation. This page is a compact dashboard for evidence state; it does not replace `docs/release-checklist.md`.

Do not mark a row passed from source review alone. A runtime row needs live evidence, logs, screenshots, or an explicit owner-approved deferral.

## Current Package

Source of truth: `docs/issues.md`.

| Artifact | SHA256 |
| --- | --- |
| ZIP | `C1EF178937D7F7572A6AAEB273272862CC011EAED8FBF89FE585236C64947EF8` |
| DLL | `70295B203C70577E58D258247F13634A443B94F57E7C1F1719F9C4F2E0EE57D3` |
| PCK | `D5B709617AF8EB600DA390B96687F3A1B5E67F88D3836A9FB8AF6C9A251343EB` |
| Manifest | `D7254BD0E8CB8B9A4036B1FD7B3CAA349C17512166731DE181126A92F8DEA5AD` |
| README_INSTALL | `C361FD7D16F286BC0AB3E437B7CF511A5294F34CAF4A49640BD58CD513081EE4` |

## Automation Summary

This row is not a live verifier row. It records the latest no-game package automation status only.

| Row | Status | Owner | Evidence Needed | Notes |
| --- | --- | --- | --- | --- |
| Current package automation | Partial | Codex | Build, normal tests, format, package sync, real installed package checker, opt-in artifact tests | Latest hashes are in `docs/issues.md`. Source/package automation passes for beta.63, but live loader/gameplay/manual evidence remains pending. |

## Verifier Row IDs

These are the exact row IDs required by `scripts/verify-spire-plus-release-evidence.ps1`. Keep a row pending until the matching live files exist or the owner explicitly accepts a deferral and reruns the verifier with `-AllowDeferred`. The beta.19 loader smoke is historical startup evidence only; beta.63 loader proof and gameplay, clicked UI, save-load, preview-tools, Vakuu, and co-op rows remain pending.

| Row ID | Kind | Status | Owner | Evidence Needed |
| --- | --- | --- | --- | --- |
| fresh-current-package-loader-smoke | loader | Pending | User | Capture a beta.63 `godot.log`, clean `godot-log-audit.json`, `environment.json`, `package-hashes.json`, `enabled-mods.txt`, current package hash parity, BaseLib plus Spire Plus only, startup completion, stopped game, and restored mod isolation. The beta.19 loader smoke remains historical startup evidence only. |
| ancient-ui-urda | clicked-ui | Pending | User | Urda clicked Ancient screenshot, foreground preflight, route note, log, and log audit. |
| ancient-ui-morvi | clicked-ui | Pending | User | Morvi clicked Ancient screenshot, foreground preflight, route note, log, and log audit. |
| ancient-ui-lotha | clicked-ui | Pending | User | Lotha clicked Ancient screenshot, foreground preflight, route note, log, and log audit. |
| ancient-ui-vakuu-normal | clicked-ui | Pending | User | Vakuu normal reward-screen screenshot, foreground preflight, route note, log, and log audit. |
| ancient-ui-vakuu-fight | clicked-ui | Pending | User | Vakuu fight option screenshot, foreground preflight, route note, log, and log audit. |
| ancient-reward-visible-relics | gameplay | Pending | User | Urda, Morvi, Lotha, and Vakuu reward choices appear as visible option relics; selected lasting rewards appear in the relic bar with readable hover text; Sere Talon / Tanx Claws routing spot checks are filled; `ancient-reward-relics-checklist.md` has every reward row filled. |
| player-text-tooltip-readability | gameplay | Pending | User | Player-facing card, relic, power, map-hover, event, and tooltip text fits and reads clearly in English and Simplified Chinese; `player-text-qa-checklist.md` has every row filled. |
| art-resource-routing-live-preview | clicked-ui | Pending | User | Title/home, Ancient screens, large art, thumbnails, option relic art, card art, and power art route to the intended assets; `art-resource-routing-checklist.md` has every surface row filled. |
| vakuu-victory-no-black-screen | gameplay | Pending | User | Gated Vakuu fight victory returns to the parent event or map without black screen or soft lock; `vakuu-victory-checklist.md` has every scenario row filled. |
| vakuu-failure-death-path | gameplay | Pending | User | Vakuu failure and death paths log cleanly and do not corrupt room, reward, or combat state; `vakuu-failure-death-checklist.md` has every scenario row filled. |
| vakuu-active-fight-save-load | save-load | Pending | User | Save/load during active and post-fight Vakuu child-combat flows restores safely or has an owner-approved deferral; `vakuu-save-load-checklist.md` has every scenario row filled. |
| ancient-state-save-load | save-load | Pending | User | Urda, Morvi, Lotha, Root Sight, Seed Bank, Rootblight, and related deck mirrors restore correctly. |
| rootblight-visual-behavior | gameplay | Pending | User | Rootblight and Blight Sprout visuals, hover, combat-end notice timing, combat behavior, cap/split behavior, and save-load persistence work in game; `rootblight-behavior-checklist.md` has every scenario row filled. |
| a11-natural-route-traversal | gameplay | Pending | User | A11 natural map traversal, route clicks, width/row layout, and logs behave correctly. |
| ascension-selector-localization | clicked-ui | Pending | User | Character-select A11-A20 Ascension panels show localized titles/descriptions. A20 must show `烙印形态` / `Branded Form`, not raw keys like `ascension.LEVEL_20.title`. |
| a19-a20-dedicated-boss-abilities | gameplay | Pending | User | Every A19/A20 v4.1 Boss ability is tested on its matching Boss, attack-changing effects show final intent, Branded Form applies only to the second Act 3 Boss, and `boss-ability-checklist.md` has every Boss row filled with live result and evidence files. |
| disable-mod-gameplay | gameplay | Pending | User | A run with Spire Plus disabled and BaseLib enabled behaves as expected beyond startup. |
| preview-tools-live-proof | preview-tools | Pending | User | Crystal Sphere, transform preview, Prismatic Gem preview, save/reopen, and co-op local UI-only proof match live behavior; `preview-tools-checklist.md` has every scenario row filled. |
| coop-disposition | coop | Pending | User | Two-client runbook evidence or explicit unsupported/unverified release-note deferral; `coop-disposition-checklist.md` has every scenario row filled. |

## Verification Command

When manual evidence folders are filled, run:

```powershell
.\scripts\collect-release-evidence.ps1 -NoLaunch
.\scripts\verify-spire-plus-release-evidence.ps1 -WritePassMarker
```

Use `collect-release-evidence.ps1` to create the verifier-readable manifest and one pending evidence subfolder per required row. Use `-AllowDeferred` only when the project owner explicitly accepts a release-note deferral for a row. `-WritePassMarker` writes `release-evidence-verifier-pass.json` only after all rows pass or are accepted deferrals.

## Runtime Evidence Logs

Set this before launching the game to add grep-friendly evidence lines to `godot.log`:

```powershell
$env:SPIREPLUS_RELEASE_EVIDENCE_LOG='1'
```

Plain marker for scripts and test guards: `SPIREPLUS_RELEASE_EVIDENCE_LOG=1`.
Legacy alias: `EZMB_RELEASE_EVIDENCE_LOG=1`.

Current source guard expects markers on Vakuu fight return paths, Root Eyes, Seed Bank, Rootblight, preview tools, A20 map/combat paths, and co-op gates.

Expected marker format:

```text
[SPIREPLUS-EVIDENCE] <Feature> <Event> run=<run> player=<player> net=<single/host/client> data=<json-ish>
```

The marker helps collect proof; it does not turn a pending row into passed evidence by itself.
