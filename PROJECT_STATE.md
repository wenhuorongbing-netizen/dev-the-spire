# PROJECT_STATE

## Active target

- `Spire Plus`
- Naming rule: all player-facing docs, UI, and tester instructions should call the mod `Spire Plus`. `EZMicroBalance` remains only as the stable technical manifest id, compatibility package folder, saved-field namespace, and legacy alias surface for this cycle.

## Current reviewed state

- Ancient Expansion v2.2 audit baseline reviewed on 2026-05-12: `a2183ee`.
- Latest pushed migration validation baseline: HEAD `f885d64d (HEAD -> main, origin/main, origin/HEAD) Guard migration runtime preflight`. Refresh the exact pushed HEAD with `git log -1 --oneline --decorate` before release packaging or final handoff.
- Active M5 Revision M truth, 2026-06-11: the current runtime blocker is resolved for loader/patch application. Root cause was Spire Plus runtime API drift, not missing or too-old BaseLib/RitsuLib: the beta.84 `v0.107.0` Off smoke reached main menu with BaseLib/RitsuLib loaded but applied only 17/25 Spire Plus ModPatcher patches and threw the stale `EctoplasmGoldGatePatch` initializer exception. This pass fixed the getter-target drift and packaged beta.85; fresh isolated `v0.107.0` Off smoke under `.tools/runtime-evidence/v01070-beta85-current-package-runtime-fix-20260611-0510/` reached main menu, selected RitsuLib compat branch `0.107.0`, applied 25/25 Spire Plus patches, and audited clean. This is loader proof only, not live gameplay or release readiness.
- Refresh the exact `git log -1 --oneline --decorate` HEAD again before release packaging or final handoff, because future documentation and validation passes may create newer commits.
- Superseded per-pass validation/package history was moved to `docs/archive/project-state-history-20260516.md` so this first-read file stays focused on current state.

## Game and BaseLib target

- Dependency configurations were aligned on BaseLib `v3.1.4` and Slay the Spire 2 `v0.106.1` for the historical clean loader evidence.
- Current local installed game root is `E:\Steam\steamapps\common\Slay the Spire 2` at Slay the Spire 2 `v0.107.0` (`release_info.json` date `2026-06-04`). Official `STS2-RitsuLib` `v0.4.16` is installed locally with `lib\0.107.0`; a backup of the prior `v0.3.10` install is under `%TEMP%\codex-ritsulib-backup-20260610-090338`. Installed beta.85 package parity passed on 2026-06-11 via `scripts\check-installed-spire-plus-package.ps1`, and fresh isolated beta.85 Off smoke under `.tools/runtime-evidence/v01070-beta85-current-package-runtime-fix-20260611-0510/` is clean.

## Top-level status

### Build / test / package

- Historical `dotnet build`, `dotnet publish`, default `dotnet test`, and package refresh passed locally after the `v0.106.1` / BaseLib `v3.1.4` API update. Current dirty-source validation must use the more specific Revision L notes below; split no-build test validation is the current trustworthy lane because exact one-shot runs can still destabilize around `ReleaseEvidenceGateTests`.
- Latest beta.85 runtime-fix validation has 0 build errors and 0 warnings. The isolated `ReleaseEvidenceGateTests` class passed 9 / 0 failed / 0 skipped / 9 total, and the complementary no-build test-project lane excluding `ReleaseEvidenceGateTests` passed 466 / 0 failed / 21 skipped / 487 total, for split coverage of 475 passed / 0 failed / 21 skipped / 496 total after clearing stale current-repo `testhost` locks. Earlier cross-thread `testhost` crashes are runner-contamination evidence only, not the current validation truth.
- June 17 validation-lane helper/source-doc continuation at HEAD `f885d64d`: `dotnet build EZMicroBalance.sln -m:1 --no-incremental` passed with 0 warnings / 0 errors; isolated `ReleaseEvidenceGateTests` passed 9 / 0 / 0; focused governance/compactness guards passed 49 / 0 / 0; the complementary no-build lane excluding `ReleaseEvidenceGateTests` passed 448 / 0 / 39 / 487; the final expanded focused guard lane passed 147 / 0 / 13 / 160; StS1 no-launch current-doc claims passed 897 / 0, runtime preflight passed 23 / 0, static suite passed 14 / 0, static-file hygiene passed 11 / 0, v19 gate ledger passed 531 / 0, and v19 subagent coverage passed 66 / 0; `dotnet format`, patch inventory, worktree batch classification, and `git diff --check` passed with CRLF normalization warnings only. This did not run publish/package, live runtime smoke, gameplay, clicked UI, save-load, co-op, QA, or release handoff.
- Current beta.85 opt-in package/artifact validation passed with `STS2_PATH=E:\Steam\steamapps\common\Slay the Spire 2` and `SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS=1`, filtered to release/package artifact, artifact parity, Ascension milestone, and Ancient behavior checks: 67 passed / 0 failed / 0 skipped / 67 total.
- June 11 dirty changes now have build, publish/package, package-checker, opt-in artifact subset, and current-package loader-smoke validation for the runtime-blocker surface. The repository is clean at HEAD `f885d64d` after the June 17 no-launch governance/preflight push, but final tester handoff must still recapture exact HEAD and worktree status. Gameplay, clicked UI, save-load, route traversal, preview-tools, Vakuu, co-op, and independent QA remain pending.
- Release artifact checks are opt-in with `SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS=1`. They must pass after any installed folder or game-root zip refresh. Legacy `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1` still works.
- Latest package/runtime target is `v0.1.0-private-beta.85`: `publish/SpirePlus-v0.1.0-private-beta.85.zip`, `E:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance`, and the game-root zip were refreshed and hash-checked on 2026-06-11. `.tools\publish-game-root` is tooling/isolated-publish context only, not the current package-parity source. The beta.85 loader smoke is clean, but gameplay, clicked UI, save-load, route traversal, preview-tools, Vakuu, and co-op rows remain pending.
- Historical guard context remains archived from 2026-05-24 after the Sere Talon `NRelic` fallback package refresh: focused Sere Talon/release-evidence/documentation/website guards passed. Manual feature results are pending.
- The previous Steam-client loader smoke under `.tools/runtime-evidence/release-ready-manual/fresh-current-package-loader-smoke` remains beta.13 historical startup context. The beta.17 and beta.19 packages have historical loader/startup evidence; the beta.19 Steam-client loader smoke remains historical context. Beta.85 now has current `v0.107.0` loader/startup evidence. Vakuu Sere Talon event-option, relic-bar, inspect-screen, hover, gameplay, save-load, and co-op rows remain pending.
- The latest script-packaged artifacts were hash-parity synced on 2026-06-11 after the beta.85 runtime-fix package refresh. Current package hashes are recorded in `docs/issues.md`, `docs/toreview.md`, `docs/private-beta-verification-handoff.md`, and `docs/release-checklist.md`; `docs/review.md` intentionally keeps compact source-review context instead of package hash tables.
- Current package scope includes the Spire Plus display-name refresh, A11-A20 hardening, multiplayer mismatch diagnostics, Ancient reward rebalance v4.3, Urda/Morvi/Lotha source-ready v2.2 slices, hidden-by-default Vakuu fight source slice, preview tools, Ancient player-facing text polish, Seedbed planting support for Rootblight, promoted generated/source-local art, and the `SpirePlus` archive name.
- Current manual-test package is not a release-readiness claim: live gameplay, clicked Ancient UI, save-load, route-click, death/failure-path, disable-gameplay, and co-op verification remain pending.
- Current cleanup/refactor audit is `docs/worktree-cleanup-audit.md`; top-level legacy project metadata was moved into `docs/archive/legacy-planning/legacy-project-files/`, targeted `.tools/` clutter was pruned, and the ignored website draft was deleted after preserving the `.tools/archive/local-website-preview-20260516/` snapshot. The current `publish/` package output and local game `source code/` are intentionally retained. Godot 4.5.1 mono and `.tools\publish-game-root` were restored after the latest local cleanup so publish/package validation can still run; raw local `.tools` runtime-evidence folders were pruned and historical evidence notes in docs should be treated as records, not currently present local raw artifacts.

### Runtime

- Current RitsuLib dependency prerequisite is locally installed: official `STS2-RitsuLib` `v0.4.16` is present at `E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib` with `lib\0.107.0\STS2-RitsuLib.dll`. Fresh beta.85 `v0.107.0` package Off smoke under `.tools/runtime-evidence/v01070-beta85-current-package-runtime-fix-20260611-0510/` reached main menu, loaded BaseLib and RitsuLib, selected RitsuLib compat branch `0.107.0`, applied 25/25 Spire Plus ModPatcher patches, and audited clean. The prior failed beta.84 smoke under `.tools/runtime-evidence/v01070-off-package-parity-20260610-092045/` remains useful root-cause evidence for stale Spire Plus API targets, not the current state. Do not claim live-ready or release-ready because gameplay, UI, save-load, co-op, independent QA rerun, clean worktree, and tester-package handoff decisions remain pending.
- Source-fix context also exists under `.tools/runtime-evidence/v01070-current-source-getter-targets-20260610-1000/`; it helped confirm the getter-target and Ectoplasm fixes before the beta.85 package refresh.

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
- Historical `v0.106.1` RitsuLib loader-gate smoke has clean Off, CanaryOnly, and AdditiveBatch1 diagnostic proof at HEAD `8f2d79b4`. Current `v0.107.0` beta.85 Off loader proof is clean; CanaryOnly, AdditiveBatch1, gameplay, replacement, multiplayer, and release QA claims still need their own fresh evidence. Batch 4c remains proposal-only.

## Commands that work

- `dotnet build`
- `dotnet publish`
- `dotnet test`
- `dotnet test -c Release` (optional)
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`
- `git diff --check`

## Next best action

- Use `docs/test-ready-development-goal.md` as the single active long-scope implementation directive. Current Codex target is a coherent user-test handoff, not release closure. The user will run live/manual Urda/Morvi/Lotha/Vakuu validation with the promoted art, including clicked UI previews, save-load checks, Vakuu failure/death-path testing, and broader manual validation; Codex should respond to concrete findings from that pass.
