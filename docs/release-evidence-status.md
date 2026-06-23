# Release Evidence Status

Current target: `Spire Plus` manual-test build for user validation. This page is a compact dashboard for evidence state; it does not replace `docs/release-checklist.md`.

Do not mark a row passed from source review alone. A runtime row needs live evidence, logs, screenshots, or an explicit owner-approved deferral.

## Current Package

Source of truth: `docs/issues.md`.

| Artifact | SHA256 |
| --- | --- |
| ZIP | `20077741751E2252254987466E213A3CF119AE04F1BFFE7115FC96099C895FF4` |
| DLL | `626871157BA5C898ABDD52081E6B107E9E3AB57B5E27B88F5E9D93296BB04CC1` |
| PCK | `8BCFCD730EE68416827B7E7A200DE8CDE27ACCD9FEE4A1E97B3346C0AEF9F3CE` |
| Manifest | `2AF698BFB78B87B2AA471083F3255A5B2C704A9A12C7E582ED2A42B7B4CFEE79` |
| README_INSTALL | `234C405E77CCFF940EB24CA5778E480DCE47B0512D48427FF3CC4D621F801E38` |

## Automation Summary

This row is not a live verifier row. It records the latest no-game package automation status only.

| Row | Status | Owner | Evidence Needed | Notes |
| --- | --- | --- | --- | --- |
| Current package automation | Partial | Codex | Build, normal tests, format, package sync, opt-in artifact tests | Latest hashes are in `docs/issues.md`. Beta.128 publish/package sync, installed package parity, runtime preflight, source-workspace validation, and clicked Ancient UI smoke are refreshed. Beta.123/beta.99 settings/Off proof, previous beta.96 Off proof, and previous beta.93 RitsuLib-only AdditiveBatch1 direct loader smoke remain previous-package context only. Gameplay/manual evidence remains pending. |

## Verifier Row IDs

These are the exact row IDs required by `scripts/verify-spire-plus-release-evidence.ps1`. Keep a row pending until the matching live files exist or the owner explicitly accepts a deferral and reruns the verifier with `-AllowDeferred`. The beta.19 loader smoke is historical startup evidence only; beta.99 RitsuLib settings/off proof is previous-package context after beta.129. Current beta.129 covers package parity, runtime preflight, and source-workspace identity; previous beta.128 covers smoke-level clicked Ancient UI for Urda, Morvi, Lotha, and normal Vakuu. Gameplay, gated Vakuu fight-option/victory return, save-load, preview-tools live behavior, current enabled-mode proof, co-op, and full release-evidence packaging rows remain pending.

| Row ID | Kind | Status | Owner | Evidence Needed |
| --- | --- | --- | --- | --- |
| fresh-current-package-loader-smoke | loader | Pass | Codex | Beta.99 Off loader proof is captured under `.tools\runtime-evidence\v01071-beta99-ritsulib0432-off-direct-20260621-234221`: main menu reached with exactly STS2-RitsuLib and Spire Plus loaded, clean audit, 25/25 Spire Plus patches, StS1Events disabled with 0 registration lines, Off verifier 21 / 0, and packet verifier 43 / 0. This is startup/default-Off evidence only, not gameplay or release readiness. |
| mod-settings-current-display | clicked-ui | Pass | Codex | Beta.99 normal Steam-client Settings -> `Mod Settings (RitsuLib)` proof is captured under `.tools\runtime-evidence\mod-settings-beta99-ritsulib-click-20260621-223210`. It proves settings UI visibility only, not beta.99 gameplay or release readiness. |
| ancient-ui-urda | clicked-ui | Pass | Codex | Beta.128 smoke proof `.tools\runtime-evidence\monkey-stability-20260623-062913\iteration-0001` captured `spireplus_test_ancient URDA confirm`, screenshot, command ACK, clean log audit, and packet-verified retained files. |
| ancient-ui-morvi | clicked-ui | Pass | Codex | Beta.128 smoke proof `.tools\runtime-evidence\monkey-stability-20260623-062913\iteration-0002` captured `spireplus_test_ancient MORVI confirm`, screenshot, command ACK, clean log audit, and packet-verified retained files. |
| ancient-ui-lotha | clicked-ui | Pass | Codex | Beta.128 smoke proof `.tools\runtime-evidence\monkey-stability-20260623-062913\iteration-0003` captured `spireplus_test_ancient LOTHA confirm`, screenshot, command ACK, clean log audit, and packet-verified retained files. |
| ancient-ui-vakuu-normal | clicked-ui | Pass | Codex | Beta.128 smoke proof `.tools\runtime-evidence\monkey-stability-20260623-062913\iteration-0004` captured `spireplus_test_ancient VAKUU confirm`, screenshot, command ACK, clean log audit, and packet-verified retained files. |
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
| disable-mod-gameplay | gameplay | Pending | User | A run with Spire Plus disabled and STS2-RitsuLib enabled behaves as expected beyond startup. |
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
