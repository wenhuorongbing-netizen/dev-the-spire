# PROJECT_STATE

## Active target

- `Spire Plus` (`EZMicroBalance` manifest id)
- Naming rule: `Spire Plus` is the player-facing display name. `EZMicroBalance` remains the stable manifest id, package folder, environment-variable prefix, and saved-field namespace for this cycle.

## Current reviewed state

- Ancient Expansion v2.2 audit baseline reviewed on 2026-05-12: `a2183ee (HEAD -> main, origin/main, origin/HEAD) 1`.
- Refresh the exact HEAD again before release packaging or final handoff, because documentation and validation passes may create newer commits.
- Superseded per-pass validation/package history was moved to `docs/archive/project-state-history-20260516.md` so this first-read file stays focused on current state.

## Game and BaseLib target

- Slay the Spire 2 game snapshot: `v0.105.0`.
- Latest live startup log observed release `v0.105.1`.
- BaseLib runtime: `v3.1.2` under `<GameRoot>\mods\BaseLib`.

## Top-level status

### Build / test / package

- `dotnet build`, `dotnet publish`, and default `dotnet test` pass locally.
- Release artifact checks pass when `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1` is enabled.
- The latest script-packaged artifacts were hash-parity synced (`DLL`, `manifest`, `PCK`, `zip`) on 2026-05-18 after the strict source audit follow-up, RootDeck notice split, Lotha combat lifecycle split, RootDeck deck-card split, RootDeck pending-downgrade split, Banner Pressing Line split, Vakuu combat-state split, Prismatic Gem hover split, and Root Sight event queue fix.
- Current package scope includes the Spire Plus display-name refresh, A11-A20 hardening, multiplayer mismatch diagnostics, Ancient reward rebalance v4.3, Urda/Morvi/Lotha source-ready v2.2 slices, hidden-by-default Vakuu fight source slice, preview tools, Ancient player-facing text polish, promoted generated/source-local art, and the `SpirePlus` archive name.
- Current manual-test package is not a release-readiness claim: live gameplay, clicked Ancient UI, save-load, route-click, death/failure-path, disable-gameplay, and co-op verification remain pending.
- Current cleanup/refactor audit is `docs/worktree-cleanup-audit.md`; top-level legacy project metadata was moved into `docs/archive/legacy-planning/legacy-project-files/`, targeted `.tools/` clutter was pruned, and the ignored website draft was deleted after preserving the `.tools/archive/local-website-preview-20260516/` snapshot. The current `publish/` package output, local game `source code/`, and remaining `.tools/` evidence/tool folders are intentionally retained.

### Runtime

- Latest normal Steam-client startup/log evidence is historical for the pre-review Spire Plus package under `.tools/runtime-evidence/live-spire-plus-session-20260515-211414`: exactly BaseLib plus Spire Plus / `EZMicroBalance` loaded, config registered, `Found 22 SavedSpireFields`, main menu reached, 0 release-blocking log signatures found, and helper restore completed. Current source defines 25 SavedSpireFields and the 2026-05-18 package was not live-loader-smoked after the static fixes and RootDeck cleanup passes, so fresh live loader parity remains pending.
- Headless installed-PCK Ancient resource verification passed under `.tools/runtime-evidence/current-package-smoke-20260514-015901`: Urda, Morvi, and Lotha background scenes instantiate, Ancient textures load from the installed PCK, option marker paths exist/export, and referenced EN/zhs localization keys exist. This is resource-load evidence, not clicked live Ancient UI evidence.
- BaseLib-only plug-off startup/log verification passed under `.tools/runtime-evidence/live-spire-plus-disabled-session-20260513-143020`; this proves loader isolation only. Disable-mod gameplay in an actual run remains pending.
- Refreshed Mod Settings UI list evidence for the current display-name package is under `.tools/runtime-evidence/current-spire-plus-modsettings-20260513-111342/02-mod-config-list.png`; historical page-level Mod Settings UI evidence remains under the old `EZ Micro Balance` display name.
- Full gameplay and manual matrix rows remain pending.

### Multiplayer

- A11-A20 selection is now default-on in this private-beta multiplayer test candidate.
- `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1` restores vanilla A1-A10 public selection for comparison.
- `EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1` disables only host-multiplayer A11-A20 selection.
- `EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1` is legacy-compatible and no longer required.
- Multiplayer join mismatch diagnostics log host/local version, ModelDb hash, and gameplay-relevant mod-list evidence before disconnecting.
- Full live Ascension verification is pending. Multiplayer ownership/desync and live co-op traversal matrices are still pending.

## Active feature areas

- Ancient reward rebalance v4: v4.3 active; full live Ancient reward gameplay and save/load rows remain pending.
- Ascension 11-20: implemented as gated/default-on slices for this private-beta multiplayer test candidate; live verification pending.
- Rootblight polish: v2.2 source-hardened; combat-end behavior, generated-art visual proof, save/load, and co-op ownership verification remain pending.
- Urda: default-on Act 1 Ancient selection with ten source-backed blessing test rows, custom scene/icon/resource routing, disable/force gates, and source-safe deviations documented. Live gameplay, clicked UI, save/load, and co-op verification remain pending.
- Morvi: default-on Morvi source implementation with all eight v2.2 blessing ids, art/text/hover support, disable/force gates, generated-card guards, Red Ink/Debt fallback hardening, and source-safe deviations documented. Live load, gameplay, save/load, and co-op verification remain pending.
- Lotha: default-on Lotha source implementation with all eight v2.2 blessing ids, event/option art, disable/force gates, and source-safe deviations documented. Live load, gameplay, save/load, lethal-path, and co-op verification remain pending.
- Vakuu fight: hidden by default and gated by `EZMB_ENABLE_VAKUU_FIGHT=1`, `SPIREPLUS_ENABLE_VAKUU_FIGHT=1`, `EZMB_FORCE_VAKUU_FIGHT=1`, or `SPIREPLUS_FORCE_VAKUU_FIGHT=1`; source uses a dedicated Vakuu monster, custom encounter scene, direct parent-room stack transition with parent event `Node` cleanup, no active `ParentEventId` on the child combat room, duplicate prefinished parent-restore Ancient-heal suppression, Contract hand injection, Stolen Vault locks, Blood Debt scaling, broken-lock blessing choices, and 50 Gold per broken lock. Live victory return, save/load, failure/death, clicked UI, and co-op verification remain pending.
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
