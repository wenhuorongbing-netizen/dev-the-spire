# PROJECT_STATE

## Active target

- `Spire Plus`
- Naming rule: all player-facing docs, UI, and tester instructions should call the mod `Spire Plus`. `EZMicroBalance` remains only as the stable technical manifest id, compatibility package folder, saved-field namespace, and legacy alias surface for this cycle.

## Current reviewed state

- Ancient Expansion v2.2 audit baseline reviewed on 2026-05-12: `a2183ee (HEAD -> main, origin/main, origin/HEAD) 1`.
- Refresh the exact HEAD again before release packaging or final handoff, because documentation and validation passes may create newer commits.
- Superseded per-pass validation/package history was moved to `docs/archive/project-state-history-20260516.md` so this first-read file stays focused on current state.

## Game and BaseLib target

- Slay the Spire 2 game snapshot: `v0.106.0` from the installed public-beta build.
- Local `source code/` was cleaned and regenerated on 2026-05-22 from `SlayTheSpire2.pck` plus `sts2.dll` using GDRE Tools `v2.5.0-beta.5`.
- BaseLib runtime/project package: `v3.1.4` under `<GameRoot>\mods\BaseLib` and `Alchyr.Sts2.BaseLib` `3.1.4`.

## Top-level status

### Build / test / package

- `dotnet build`, `dotnet publish`, default `dotnet test`, and package refresh pass locally after the `v0.106.0` / BaseLib `v3.1.4` API update.
- Release artifact checks pass when `SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS=1` is enabled. Legacy `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1` still works.
- Latest validation 2026-05-24 after the Sere Talon `NRelic` fallback package refresh and package README wording refresh: `dotnet build`, focused Sere Talon/release-evidence/documentation/website guards, default `dotnet test`, opt-in artifact tests, website syntax checks, `dotnet format`, `git diff --check`, installed package check, worktree batch report, and release verifier fail-closed check all succeeded in their expected modes. The current-package loader row is pending again; current package Steam-client loader smoke because the ZIP/README hash changed without opening the game; the verifier fails closed with 19 pending live/manual rows and 0 warnings.
- The most recent Steam-client loader smoke under `.tools/runtime-evidence/manual-test-handoff-20260524-161744/release/fresh-current-package-loader-smoke` loaded exactly BaseLib plus Spire Plus, registered config for `EZMicroBalance`, reported `Found 30 SavedSpireFields`, reached startup completion, and audited clean for the same DLL/PCK/manifest but a prior ZIP/README hash. Treat it as startup/log context only until the current ZIP is launched; Vakuu Sere Talon event-option, relic-bar, inspect-screen, hover, gameplay, save-load, and co-op rows remain pending.
- The latest script-packaged artifacts were hash-parity synced (`DLL`, `manifest`, `PCK`, `zip`) on 2026-05-24 after the Sere Talon surface-log package refresh. Current package hashes are recorded in `docs/issues.md`, `docs/toreview.md`, and `docs/review.md`.
- Current package scope includes the Spire Plus display-name refresh, A11-A20 hardening, multiplayer mismatch diagnostics, Ancient reward rebalance v4.3, Urda/Morvi/Lotha source-ready v2.2 slices, hidden-by-default Vakuu fight source slice, preview tools, Ancient player-facing text polish, promoted generated/source-local art, and the `SpirePlus` archive name.
- Current manual-test package is not a release-readiness claim: live gameplay, clicked Ancient UI, save-load, route-click, death/failure-path, disable-gameplay, and co-op verification remain pending.
- Current cleanup/refactor audit is `docs/worktree-cleanup-audit.md`; top-level legacy project metadata was moved into `docs/archive/legacy-planning/legacy-project-files/`, targeted `.tools/` clutter was pruned, and the ignored website draft was deleted after preserving the `.tools/archive/local-website-preview-20260516/` snapshot. The current `publish/` package output, local game `source code/`, and remaining `.tools/` evidence/tool folders are intentionally retained.

### Runtime

- Current source defines 30 SavedSpireFields. The latest Steam-client startup/log evidence under `.tools/runtime-evidence/manual-test-handoff-20260524-161744/release/fresh-current-package-loader-smoke` loaded exactly BaseLib plus Spire Plus, config registered, `Found 30 SavedSpireFields`, startup completion reached, 0 release-blocking log signatures were found, and helper restore completed for the same DLL/PCK/manifest as the current package. The current ZIP/README hash still needs a fresh loader row; previous `20260523-current`, `Found 22 SavedSpireFields`, and 16-field loader logs remain historical context only.
- Headless installed-PCK Ancient resource verification passed under `.tools/runtime-evidence/current-package-smoke-20260514-015901`: Urda, Morvi, and Lotha background scenes instantiate, Ancient textures load from the installed PCK, option marker paths exist/export, and referenced EN/zhs localization keys exist. This is resource-load evidence, not clicked live Ancient UI evidence.
- BaseLib-only plug-off startup/log verification passed under `.tools/runtime-evidence/live-spire-plus-disabled-session-20260513-143020`; this proves loader isolation only. Disable-mod gameplay in an actual run remains pending.
- Refreshed Mod Settings UI list evidence for the current display-name package is under `.tools/runtime-evidence/current-spire-plus-modsettings-20260513-111342/02-mod-config-list.png`; older page-level Mod Settings UI evidence predates the display-name refresh.
- Full gameplay and manual matrix rows remain pending.

### Multiplayer

- A11-A20 selection is now default-on in this private-beta multiplayer test candidate.
- `SPIREPLUS_*` names are the preferred manual-test gates for Ancient and evidence-log work. Older `EZMB_*` gates remain compatibility aliases where source already supports them.
- `SPIREPLUS_ASCENSION_DISABLE_PUBLIC_SELECTION=1` restores vanilla A1-A10 public selection for comparison.
- `SPIREPLUS_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1` disables only host-multiplayer A11-A20 selection.
- `EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1` is legacy-compatible and no longer required.
- Multiplayer join mismatch diagnostics log host/local version, ModelDb hash, and gameplay-relevant mod-list evidence before disconnecting.
- Full live Ascension verification is pending. Multiplayer ownership/desync and live co-op traversal matrices are still pending.

## Active feature areas

- Ancient reward rebalance v4: v4.3 active; full live Ancient reward gameplay and save/load rows remain pending.
- Ascension 11-20: implemented as gated/default-on slices for this private-beta multiplayer test candidate; live verification pending.
- Rootblight polish: v2.2 source-hardened; combat-end behavior, generated-art visual proof, save/load, and co-op ownership verification remain pending.
- Urda: default-on Act 1 Ancient selection with eleven source-backed blessing test rows, including Elite Root, custom scene/icon/resource routing, disable/force gates, and source-safe deviations documented. Live gameplay, clicked UI, save/load, and co-op verification remain pending.
- Morvi: default-on Morvi source implementation with all eight v2.2 blessing ids, art/text/hover support, disable/force gates, generated-card guards, Red Ink/Debt fallback hardening, and source-safe deviations documented. Live load, gameplay, save/load, and co-op verification remain pending.
- Lotha: default-on Lotha source implementation with all eight v2.2 blessing ids, event/option art, disable/force gates, and source-safe deviations documented. Live load, gameplay, save/load, lethal-path, and co-op verification remain pending.
- Vakuu fight: hidden by default and gated with preferred `SPIREPLUS_ENABLE_VAKUU_FIGHT=1` / `SPIREPLUS_FORCE_VAKUU_FIGHT=1` controls, with old `EZMB_*` aliases still accepted; source uses a dedicated Vakuu monster, custom encounter scene, direct parent-room stack transition with parent event `Node` cleanup, no active `ParentEventId` on the child combat room, duplicate prefinished parent-restore Ancient-heal suppression, Contract hand injection, Stolen Vault locks, Blood Debt scaling, broken-lock blessing choices, and 50 Gold per broken lock. Live victory return, save/load, failure/death, clicked UI, and co-op verification remain pending.
- Preview tools: Crystal Sphere peek and deterministic transform preview now live under `EZMicroBalanceCode/Preview/` and are configured through the single Spire Plus mod settings page. Live Crystal Sphere, transform-result match, save/reopen, and co-op proof remain pending.
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

## Commands that work

- `dotnet build`
- `dotnet publish`
- `dotnet test`
- `dotnet test -c Release` (optional)
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`
- `git diff --check`

## Next best action

- Use `docs/test-ready-development-goal.md` as the single active long-scope implementation directive. Current Codex target is a coherent user-test handoff, not release closure. The user will run live/manual Urda/Morvi/Lotha/Vakuu validation with the promoted art, including clicked UI previews, save-load checks, Vakuu failure/death-path testing, and broader manual validation; Codex should respond to concrete findings from that pass.
