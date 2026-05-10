# PROJECT_STATE

## Active target

- `EZ Micro Balance` (`EZMicroBalance` manifest id)

## Current latest commit

- `9fd9e3f` (`1.05.07`) on `main`.

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

### Runtime

- Normal single-player smoke and controlled `--force-steam off` smoke are known-good with only `BaseLib + EZMicroBalance` to main menu from fresh install.
- Mod Settings visibility is verified in current handoff docs.
- Full gameplay and manual matrix rows remain pending.

### Multiplayer

- A11-A20 source selection is default-on for private-beta test scope.
- Multiplayer ownership/desync and live co-op traversal matrices are still pending.
- Development warnings are documented for A20 multiplayer downgrade behavior.

### Urda

- Urda is a **prototype/debug-only** feature in this private-beta cycle.
- Urda only enables under `EZMB_FORCE_ANCIENT=URDA`.
- `IsUrdaEnabled` is guarded only by this environment variable.
- Active ids are `urda_seedbed`, `urda_humus_pact`, `urda_molting`, `urda_moss_map` (selection state only in this pass).
- No Morvi/Lotha/Vakuu gameplay is active.

### Rootblight

- Source and package state are complete with generated card portraits and combat-end notice pipeline.
- Full live visual and co-op ownership verification remains pending.

## Active feature areas

- Ancient reward rebalance v4: v4.3 active.
- Ascension 11-20: implemented as gated slices; live verification pending.
- Rootblight polish: source-complete, visual/manual evidence partial.
- Urda: prototype/debug-only scaffolded under explicit gating.

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

- Complete required manual matrix rows (Ancient + multiplayer + Rootblight visuals), then refresh package artifacts only if source/resources/docs changed.
