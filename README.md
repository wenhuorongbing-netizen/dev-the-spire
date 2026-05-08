# dev-the-spire

Slay the Spire 2 mod workspace for private beta development.

## Current Release Target

The active one-month deliverable is `EZ Micro Balance`: a focused Ancient reward rebalance mod for Slay the Spire 2.

The original project and manifest id are `EzDailyContent`. That id must not be renamed in-place. The architecture decision for the private beta is to create a new independent mod project with stable id `EZMicroBalance`, so players can enable or disable EZ Micro Balance separately from later mods.

Ascension 11-20 expansion work is now an active development track after the 2026-05-06 overnight sprint goal. A11-A20 selection is now default-on in this private-beta multiplayer test candidate for single-player and host-multiplayer standard lobbies. Set `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1` to restore vanilla A1-A10 selection for comparison. Set `EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1` to disable only host-multiplayer A11-A20 selection. `EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1` is legacy-compatible and no longer required. A20 multiplayer selection is not full A20 co-op support. Dual King Brands / second-boss Brand gameplay remains disabled or downgraded in co-op pending live verification; A11-A19 inherited systems may still apply subject to live testing. Full live Ascension verification is pending. Ascension 21-30 and custom character work remain out of scope.

## Current State

- Active Ancient reward rebalance implementation exists under `EZMicroBalanceCode/Ancients/`; v4.3 is current. It covers Distinguished Cape's `lose 30% of current Max HP, at least 18` trade gate with same-pool Vakuu replacement when unaffordable, Prismatic Gem's "Every second standard card reward contains only off-color cards" behavior with reward-screen hint fallback diagnostics, Velvet Choker's retained v4.2 soft limit, and no-space Simplified Chinese number formatting. v4.2 rightmost-slot Prismatic Gem is historical only. v4.2 Distinguished Cape 40% min15 is historical only.
- Legacy Ancient work remains under `EzDailyContentCode/Ancients/` for traceability and is not part of the active solution.
- `EZMicroBalance` has its own solution, project, manifest, resource folder, code folder, DLL, and PCK.
- English and Simplified Chinese localization files exist for changed Ancient rewards under `EZMicroBalance/localization/`.
- The latest automated pass has been refreshed for Slay the Spire 2 `v0.105.0` with BaseLib `v3.1.2`: build succeeds with 0 warnings/errors, normal source/localization/docs tests pass with release artifact/runtime evidence tests skipped unless `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1`, and controlled `--force-steam off` smoke loaded only BaseLib plus EZ Micro Balance with `Found 12 SavedSpireFields`. Normal Steam-client Mod Settings now has RC1 evidence for BaseLib and EZ Micro Balance; live gameplay/manual feature verification is still pending.
- Ascension 11-20 has a v2.0 development checklist at `docs/features/ascension-11-20/development-checklist-v2.md`. Active prototype slices live under `EZMicroBalanceCode/Ascension/`: default-on original-UI A11-A20 selection for this private-beta multiplayer test candidate, A11 +1 map column with an inserted-column optional route plus extra route rows in Act 1/2/3 (`+1/+1/+2`) without A11-specific map markers, Rootblight/Blight Sprout gameplay, A12 Firemarked Elite/Forge Token with dedicated map/status indicators and Heal/Smith payout, A13 Fission with stricter eligibility, higher visibility, and icon support, A16 Banner Rooms, A17 optional Act 2/3 Deep Branches with a guarded enhanced treasure reward, A19 source-guarded boss-specific Royal Seal hooks with Boss-map hover text, and A20 vanilla double-boss map creation with Boss 2 Brand metadata/parameters, Boss-map Brand hover text, Boss 1 post-combat recovery, a Boss 1 card reward, Boss 1 reward-screen intermission wording, and a fixed courtyard event inserted before Boss 2 through the vanilla terminal-reward path. These prototype slices still need live Ascension verification; Forge Token special rest-site payout and a bespoke full-screen intermission remain deferred. Controlled smoke and Mod Settings passed are not the same as live co-op verification.

## Requirements

- Slay the Spire 2 public beta, current local target `v0.105.0`
- .NET SDK 9.0.313 or compatible
- Godot .NET / Mono 4.5.1
- BaseLib runtime `v3.1.2` installed under `<GameRoot>\mods\BaseLib`
- Local `Directory.Build.props` copied from `Directory.Build.props.example`

## Local Path Configuration

`Directory.Build.props` is local and gitignored because it contains machine-specific absolute paths.

On a new machine:

1. Copy `Directory.Build.props.example` to `Directory.Build.props`.
2. Fill in `GodotPath`.
3. Fill in `Sts2Path`.
4. Install BaseLib under `<GameRoot>\mods\BaseLib`.

## Build

```powershell
dotnet build
```

## Publish

```powershell
dotnet publish
```

Run publish after resource, localization, packaging, or manifest changes.

## Documentation

Start with `docs/README.md` for the documentation index.

High-frequency docs:

- `docs/PROJECT_MAP.md`: current repository layout and active/legacy boundaries.
- `docs/dev-environment.md`: local versions, paths, latest command results, and smoke evidence.
- `docs/release-checklist.md`: private beta release gates and pending manual checks.
- `docs/private-beta-verification-handoff.md`: concise tester handoff.
- `docs/features/ancients-rework-v4/README.md`: Ancient reward rebalance entry point.
- `docs/features/ascension-11-20/README.md`: Ascension 11-20 development entry point.
- `docs/archive/README.md`: archived historical planning policy.

## Release Policy

- Do not change an existing manifest id in-place.
- Do not copy original Slay the Spire 2 assets into this repo.
- Do not copy large decompiled game code bodies into this repo.
- Keep future mods in independent mod folders/projects.
- Push private beta changes only after explicit user approval.
