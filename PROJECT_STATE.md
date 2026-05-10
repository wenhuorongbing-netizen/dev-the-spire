# PROJECT_STATE

## Active target

- `EZ Micro Balance` (`EZMicroBalance` manifest id)

## Current latest commit

- `f201508` (`implement urda`) on `main`.

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

- Normal single-player loader smoke and controlled `--force-steam off` smoke are known-good with only `BaseLib + EZMicroBalance` when `SavedSpireFields` count is updated to 13.
- Mod Settings visibility in normal Steam Client is validated by handoff docs.
- Full gameplay and manual matrix rows remain pending.

### Multiplayer

- A11-A20 source selection is default-on for this private-beta candidate.
- Multiplayer ownership/desync and live co-op traversal matrix are still pending.
- Development warnings are documented for A20 multiplayer downgrade behavior.

### Urda

- Urda framework/registry is present in code and documentation.
- Blessing behavior is not fully implemented in gameplay; treat Urda as prototype/vertical-slice state until live runtime and full blessing implementations are completed.
- Urda remains documented as active-documents-only for controlled follow-up.

### Rootblight

- Source and package state are complete with generated card portraits and combat-end notice pipeline.
- Full live visual and co-op ownership verification remains pending.

## Active feature areas

- Ancient reward rebalance v4: v4.3 active.
- Ascension 11-20: implemented as gated slices; live verification pending.
- Rootblight polish: source-complete, visual/manual evidence partial.
- Urda: prototype/vertical-slice scaffolding documented; gameplay completion pending.

## Current blockers

- Multiplayer co-op verification matrix (save/load, route traversal, ownership checks).
- Ancient reward manual matrix live pass, save/load-sensitive rows, and disable-mod gameplay.
- Natural route traversal and boss-reachability confirmation for A11.
- In-game visual verification for generated Rootblight art.
- Release-note-ready blocker closure after manual verification evidence is complete.

## Commands that work

- `dotnet build`
- `dotnet publish`
- `dotnet test`
- `dotnet test -c Release` (optional)
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`
- `dotnet diff --check`

## Next best action

- Finish required manual matrix rows (Ancient + multiplayer + Rootblight visuals), then refresh package artifacts only if source/resources/docs changes.
