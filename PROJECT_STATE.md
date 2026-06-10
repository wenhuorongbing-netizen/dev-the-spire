# PROJECT_STATE

## Active target

- `Spire Plus`
- Naming rule: all player-facing docs, UI, and tester instructions should call the mod `Spire Plus`. `EZMicroBalance` remains only as the stable technical manifest id, compatibility package folder, saved-field namespace, and legacy alias surface for this cycle.

## Current reviewed state

- Ancient Expansion v2.2 audit baseline reviewed on 2026-05-12: `a2183ee`.
- Latest pushed cleanup/package evidence baseline: current beta.84 Urda Seedbed Harmony patch bugfix; current Revision J owner-review baseline is HEAD `8f2d79b4 (HEAD -> main, origin/main, origin/HEAD) sprint3`. Refresh the exact pushed HEAD with `git log -1 --oneline --decorate` before release packaging or final handoff.
- Refresh the exact `git log -1 --oneline --decorate` HEAD again before release packaging or final handoff, because future documentation and validation passes may create newer commits.
- Superseded per-pass validation/package history was moved to `docs/archive/project-state-history-20260516.md` so this first-read file stays focused on current state.

## Game and BaseLib target

- Dependency configurations were aligned on BaseLib `v3.1.4` and Slay the Spire 2 `v0.106.1` for the historical clean loader evidence.
- Current local installed game root is `E:\Steam\steamapps\common\Slay the Spire 2` at Slay the Spire 2 `v0.107.0` (`release_info.json` date `2026-06-04`). Official `STS2-RitsuLib` `v0.4.16` is now installed locally with `lib\0.107.0`; a backup of the prior `v0.3.10` install is under `%TEMP%\codex-ritsulib-backup-20260610-090338`. Installed beta.84 package parity was restored on 2026-06-10 by replacing the stale June 2 DLL with the packaged DLL and rerunning `scripts\check-installed-spire-plus-package.ps1` successfully. Fresh current-package runtime proof is blocked by the `v0.107.0` Off smoke under `.tools/runtime-evidence/v01070-off-package-parity-20260610-092045/`: the game reached main menu with RitsuLib `v0.4.16` / compat `0.107.0`, but Spire Plus had 8 optional ModPatcher failures and an `EctoplasmGoldGatePatch` initializer exception from API drift.

## Top-level status

### Build / test / package

- Historical `dotnet build`, `dotnet publish`, default `dotnet test`, and package refresh passed locally after the `v0.106.1` / BaseLib `v3.1.4` API update. Current dirty-source validation must use the more specific Revision L notes below; exact solution-level no-build test validation was refreshed after the cross-thread `testhost` overlap ended.
- Latest local refactor validation at HEAD `f32c6767` plus dirty worktree code/docs changes has 0 build errors and 0 warnings after installed-game API compatibility fixes and expanded Sts1Events owner guards. The current migration validation lane reran the handoff tests, full test project, and exact solution-level `dotnet test EZMicroBalance.sln --no-build` without overlapping test/build processes; both test lanes passed with 464 passed / 0 failed / 21 skipped / 485 total. Format, diff-check, patch-inventory, and batch-classifier checks were green. Earlier cross-thread `testhost` crashes are runner-contamination evidence only, not the current validation truth.
- Release artifact checks are opt-in with `SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS=1`. The latest beta.84 run must pass after the installed folder and game-root zip are refreshed. Legacy `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1` still works.
- Latest in-progress validation target 2026-05-27 is the beta.84 Urda Seedbed Harmony patch bugfix: `AncientPlayerFacingPolishGuardTests.Localization.cs` now owns Ancient dialogue/localization parity plus active source/docs mojibake guard coverage. `AncientPlayerFacingPolishGuardTests.cs` keeps current-doc backtick balance, option relic, concept, custom-card, hover-preview, Urda/Vakuu/doc-status, Ascension text, and Forge Token player-facing text guards. Test names, assertions, package evidence semantics, and player-visible mod behavior are unchanged. `publish/SpirePlus-v0.1.0-private-beta.84.zip`, `D:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance`, and the game-root zip were refreshed and hash-checked before handoff. `.tools\publish-game-root` is tooling/isolated-publish context only, not the current package-parity source. The beta.19 Steam-client loader smoke under `.tools/runtime-evidence/beta19-loader-smoke-20260525-213336` is still useful historical startup proof for the same 30-field source family, but beta.84 loader/gameplay proof was not recaptured in this pass. Gameplay, clicked UI, save-load, route traversal, preview-tools, Vakuu, and co-op rows remain pending.
- Historical guard context remains archived from 2026-05-24 after the Sere Talon `NRelic` fallback package refresh: focused Sere Talon/release-evidence/documentation/website guards passed. Manual feature results are pending.
- The previous Steam-client loader smoke under `.tools/runtime-evidence/release-ready-manual/fresh-current-package-loader-smoke` remains beta.13 historical startup context. The beta.17 and beta.19 packages have historical loader/startup evidence; beta.84 startup remains pending. Vakuu Sere Talon event-option, relic-bar, inspect-screen, hover, gameplay, save-load, and co-op rows remain pending.
- The latest script-packaged artifacts were hash-parity synced on 2026-05-27 after the beta.84 Urda Seedbed Harmony patch bugfix. Current package hashes are recorded in `docs/issues.md`, `docs/toreview.md`, `docs/private-beta-verification-handoff.md`, and `docs/release-checklist.md`; `docs/review.md` intentionally keeps compact source-review context instead of package hash tables.
- Current package scope includes the Spire Plus display-name refresh, A11-A20 hardening, multiplayer mismatch diagnostics, Ancient reward rebalance v4.3, Urda/Morvi/Lotha source-ready v2.2 slices, hidden-by-default Vakuu fight source slice, preview tools, Ancient player-facing text polish, Seedbed planting support for Rootblight, promoted generated/source-local art, and the `SpirePlus` archive name.
- Current manual-test package is not a release-readiness claim: live gameplay, clicked Ancient UI, save-load, route-click, death/failure-path, disable-gameplay, and co-op verification remain pending.
- Current cleanup/refactor audit is `docs/worktree-cleanup-audit.md`; top-level legacy project metadata was moved into `docs/archive/legacy-planning/legacy-project-files/`, targeted `.tools/` clutter was pruned, and the ignored website draft was deleted after preserving the `.tools/archive/local-website-preview-20260516/` snapshot. The current `publish/` package output and local game `source code/` are intentionally retained. Godot 4.5.1 mono and `.tools\publish-game-root` were restored after the latest local cleanup so publish/package validation can still run; raw local `.tools` runtime-evidence folders were pruned and historical evidence notes in docs should be treated as records, not currently present local raw artifacts.

### Runtime

- Current RitsuLib dependency prerequisite is locally installed: official `STS2-RitsuLib` `v0.4.16` is present at `E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib` with `lib\0.107.0\STS2-RitsuLib.dll`. Installed beta.84 package parity currently passes after the 2026-06-10 DLL restore; the stale DLL backup and restore record are under `.tools/runtime-evidence/package-parity-restore-20260610-091943/`. Fresh K1 runtime evidence at HEAD `8f2d79b4` (2026-06-02) under `.tools/runtime-evidence/smoke-k1-off-20260602-145938/` and `.tools/runtime-evidence/smoke-k1-canary3-20260602-151104/` remains historical `v0.106.1` loader evidence only, along with the prior Revision J target-fix evidence. The fresh beta.84 `v0.107.0` package-parity Off smoke under `.tools/runtime-evidence/v01070-off-package-parity-20260610-092045/` is a non-clean current-package blocker: main menu was reached, BaseLib and RitsuLib loaded, RitsuLib selected compat branch `0.107.0`, but Spire Plus applied only 17/25 ModPatcher patches and threw an initializer exception on the stale Ectoplasm patch target. Do not claim live-ready or release-ready because clean current-runtime smoke, gameplay, UI, save-load, co-op, independent QA rerun, clean worktree, current-source package decision, and versioned tester-package handoff remain pending.

- Current source defines 30 SavedSpireFields. Historical beta.19 startup context remains useful only as history; older 22-field and `Found 22 SavedSpireFields` loader notes remain historical. Current diagnostic loader evidence now records `Found 30 SavedSpireFields` with BaseLib, RitsuLib, and Spire Plus enabled. Full gameplay and current versioned tester-package proof remain pending.
- Headless installed-PCK Ancient resource verification passed under `.tools/runtime-evidence/current-package-smoke-20260514-015901`: Urda, Morvi, and Lotha background scenes instantiate, Ancient textures load from the installed PCK, option marker paths exist/export, and referenced EN/zhs localization keys exist. This is resource-load evidence, not clicked live Ancient UI evidence.
- BaseLib-only plug-off startup/log verification passed under `.tools/runtime-evidence/live-spire-plus-disabled-session-20260513-143020`; this proves loader isolation only. Disable-mod gameplay in an actual run remains pending.
- Refreshed Mod Settings UI list evidence for the current display-name package is under `.tools/runtime-evidence/current-spire-plus-modsettings-20260513-111342/02-mod-config-list.png`; older page-level Mod Settings UI evidence predates the display-name refresh.
- Full gameplay and manual matrix rows remain pending.

### Multiplayer

- A11-A20 selection is default-on for single-player standard lobbies. Host-multiplayer A11-A20 selection and gameplay fail closed by default after the 2026-05-25 crash logs unless `SPIREPLUS_ALLOW_UNVERIFIED_COOP_GAMEPLAY=1` is deliberately set for two-client debugging.
- `SPIREPLUS_*` names are the preferred manual-test gates for Ancient and evidence-log work. Older `EZMB_*` gates remain compatibility aliases where source already supports them.
- `SPIREPLUS_ASCENSION_DISABLE_PUBLIC_SELECTION=1` restores vanilla A1-A10 public selection for comparison.
- `SPIREPLUS_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1` disables only host-multiplayer A11-A20 selection.
- `EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1` is legacy-compatible and no longer required.
- Multiplayer join mismatch diagnostics log host/local version, ModelDb hash, and gameplay-relevant mod-list evidence before disconnecting.
- Full live Ascension verification is pending. Multiplayer ownership/desync and live co-op traversal matrices are still pending.

## Active feature areas

- Ancient reward rebalance v4: v4.3 active; full live Ancient reward gameplay and save/load rows remain pending.
- Ascension 11-20: implemented as gated/default-on single-player slices with co-op gameplay fail-closed by default; live verification pending.
- Rootblight polish: v2.2 source-hardened; combat-end behavior, generated-art visual proof, save/load, and co-op ownership verification remain pending.
- Urda: default-on Act 1 Ancient selection with eleven source-backed blessing test rows, including Elite Root, custom scene/icon/resource routing, disable/force gates, and source-safe deviations documented. Live gameplay, clicked UI, save/load, and co-op verification remain pending.
- Morvi: default-on Morvi source implementation with all eight v2.2 blessing ids, art/text/hover support, disable/force gates, generated-card guards, Red Ink/Debt fallback hardening, and source-safe deviations documented. Live load, gameplay, save/load, and co-op verification remain pending.
- Lotha: default-on Lotha source implementation with all eight v2.2 blessing ids, event/option art, disable/force gates, and source-safe deviations documented. Live load, gameplay, save/load, lethal-path, and co-op verification remain pending.
- Vakuu fight: hidden by default and gated with preferred `SPIREPLUS_ENABLE_VAKUU_FIGHT=1` / `SPIREPLUS_FORCE_VAKUU_FIGHT=1` controls, with old `EZMB_*` aliases still accepted; source uses a dedicated Vakuu monster, custom encounter scene, direct parent-room stack transition with parent event `Node` cleanup, no active `ParentEventId` on the child combat room, duplicate prefinished parent-restore Ancient-heal suppression, Contract hand injection, Stolen Vault locks, Blood Debt scaling, broken-lock blessing choices, and 50 Gold per broken lock. Live victory return, save/load, failure/death, clicked UI, and co-op verification remain pending.
- Preview tools: Crystal Sphere peek and deterministic transform preview now live under `EZMicroBalanceCode/Preview/` and are configured through the single Spire Plus mod settings page. They run in co-op as local UI-only previews without adding choices, rewards, or real RNG calls; live Crystal Sphere, transform-result match, save/reopen, and two-client proof remain pending.
- Ancient expansion v2.2 source docs live under `docs/features/ancient-expansion-v2.2/`; current implementation planning starts from `docs/test-ready-development-goal.md`, `docs/issues.md`, and the feature README/source docs.

## Current blockers

- Manual feature results are pending.
- Clicked Ancient UI verification still needs Urda, Morvi, Lotha, and gated Vakuu screenshots/logs showing event art, dialogue, option art, and expected choices.
- Ancient reward manual matrix and save/load-sensitive rows remain pending.
- Disable-mod gameplay in an actual run remains pending; current plug-off evidence covers only BaseLib-only startup/log loading.
- Natural A11 click-by-click traversal remains pending; visible width/row, route-click, save-load, and co-op proof are still required.
- Multiplayer co-op verification matrix remains pending.
- Live visual verification for generated Rootblight art remains pending.
- Release-note closure is blocked until manual verification evidence is complete.
- Historical `v0.106.1` RitsuLib loader-gate smoke has clean Off, CanaryOnly, and AdditiveBatch1 diagnostic proof at HEAD `8f2d79b4`. Current `v0.107.0` package proof is red after the non-clean beta.84 Off smoke; fix the package/source runtime drift and recapture clean Off proof before CanaryOnly, AdditiveBatch1, gameplay, replacement, multiplayer, or release QA claims. Batch 4c remains proposal-only.

## Commands that work

- `dotnet build`
- `dotnet publish`
- `dotnet test`
- `dotnet test -c Release` (optional)
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`
- `git diff --check`

## Next best action

- Use `docs/test-ready-development-goal.md` as the single active long-scope implementation directive. Current Codex target is a coherent user-test handoff, not release closure. The user will run live/manual Urda/Morvi/Lotha/Vakuu validation with the promoted art, including clicked UI previews, save-load checks, Vakuu failure/death-path testing, and broader manual validation; Codex should respond to concrete findings from that pass.
