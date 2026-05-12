# PROJECT_STATE

## Active target

- `EZ Micro Balance` (`EZMicroBalance` manifest id)

## Current reviewed state

- Pre-planning-ingest HEAD reviewed on 2026-05-12: `c8bcaa9` (`update`) on `main`.
- Current latest commit recorded on 2026-05-12 with `git log -1 --oneline --decorate`: `c8bcaa9 (HEAD -> main, origin/main, origin/HEAD) update`.
- Refresh the exact HEAD again before release packaging or final handoff, because this planning pass may create a newer commit.

## Game and BaseLib target

- Slay the Spire 2 game snapshot: `v0.105.0`.
- BaseLib runtime: `v3.1.2` under `<GameRoot>\mods\BaseLib`.

## Top-level status

### Build / test / package

- `dotnet build` passes locally with 0 warnings and 0 errors.
- `dotnet publish` succeeds.
- `dotnet test` passes in default mode.
- Release artifact checks pass when `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1` is enabled.
- Published artifacts are hash-parity synced (`DLL`, `manifest`, `PCK`, `zip`) and validated by tests.
- Current package refresh includes the A11-A20 v2.0 hardening pass and multiplayer join mismatch diagnostics; live hover/gameplay/co-op verification remains pending.

### Runtime

- Normal single-player smoke and controlled `--force-steam off` smoke are known-good with only `BaseLib + EZMicroBalance` to main menu from fresh install.
- Mod Settings visibility is verified in current handoff docs.
- Full gameplay and manual matrix rows remain pending.

### Multiplayer

- A11-A20 source selection is default-on for private-beta test scope.
- Multiplayer join failures that vanilla reports as "game version differs" now log host/local version, ModelDb hash, and gameplay-relevant mod-list evidence before disconnecting.
- Multiplayer ownership/desync and live co-op traversal matrices are still pending.
- Development warnings are documented for A20 multiplayer downgrade behavior.

### Urda

- Urda is default-on for private-beta testing, but remains a prototype until manual gameplay and save/load checks are complete.
- Set `EZMB_DISABLE_URDA=1` to hide Urda for comparison.
- `EZMB_FORCE_ANCIENT=URDA` is legacy-compatible and no longer required.
- Active ids are `urda_seedbed`, `urda_humus_pact`, `urda_molting`, `urda_moss_map`; source gameplay hooks now cover Seedbed, Humus Pact, Molting, and Moss Map. Current source hardening removed Humus Pact's `OnSkipped` reward reentry path, keeps the third payoff pending until resolver success, and tightened Seedbed accounting/cost/max-HP behavior.
- Morvi has a default-off v2.2 prototype behind `EZMB_ENABLE_MORVI_V22=1`; it is not in default private-beta runs. Latest source hardening routes Misprint generated copies through cleanup-safe helper logic and keeps Debt payoff pending until the payoff resolver succeeds.
- Lotha/Vakuu gameplay is not active. Lotha remains blocked this pass by missing explicit event-art/background source files and by unresolved live testing.
- Ancient Expansion v2.2 is tracked under `docs/features/ancient-expansion-v2.2/`; current Urda stabilization and default-off Morvi prototyping are active, while Lotha, Vakuu fight, and additional Urda blessings remain planning-only.

### Rootblight

- Source and package state are complete with generated card portraits and combat-end notice pipeline.
- Full live visual and co-op ownership verification remains pending.

## Active feature areas

- Ancient reward rebalance v4: v4.3 active.
- Ascension 11-20: implemented as gated slices; live verification pending.
- Rootblight polish: source-complete, visual/manual evidence partial.
- Urda: default-on Act 1 selection plus first source-backed blessing gameplay slice with an explicit disable gate; live verification remains pending.
- Ancient expansion v2.2: current Urda stabilization plus default-off Morvi prototype hardening; Lotha/Vakuu and extra Urda roadmap items remain planning-only and require explicit future milestone approval.

## Current blockers

- Multiplayer co-op verification matrix (save/load, route traversal, ownership checks).
- Ancient reward manual matrix and save/load-sensitive rows remain pending.
- Natural route traversal and boss-reachability confirmation for A11 remain pending.
- Live visual verification for generated Rootblight art remains pending.
- Release-note-ready blocker closure after manual verification evidence is complete.

## Commands that work

- `dotnet build`
- `dotnet publish`
- `dotnet test`
- `dotnet test -c Release` (optional)
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`
- `git diff --check`

## Next best action

- Complete required manual matrix rows (Ancient + Urda + multiplayer + Rootblight visuals), then refresh package artifacts only if source/resources/docs changed.
