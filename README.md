# dev-the-spire

Slay the Spire 2 mod workspace for private beta development.

## Current Release Target

The active private-beta deliverable is `Spire Plus`: a Slay the Spire 2 balance and progression expansion mod.

The original scaffold manifest id was `EzDailyContent`. That id must not be renamed in-place. The active private-beta deliverable is one mod with stable id `EZMicroBalance`, so players see and enable `Spire Plus` as a single package. The player-facing display name is `Spire Plus`; the manifest id, package folder, env vars, saved-field prefixes, and namespaces remain `EZMicroBalance` for compatibility this cycle.

Ascension 11-20 expansion work is now an active development track after the 2026-05-06 overnight sprint goal. A11-A20 selection is now default-on in this private-beta multiplayer test candidate for single-player and host-multiplayer standard lobbies. Set `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1` to restore vanilla A1-A10 selection for comparison. Set `EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1` to disable only host-multiplayer A11-A20 selection. `EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1` is legacy-compatible and no longer required. A20 multiplayer selection is not full A20 co-op support. A20 Branded Form / second-boss enhanced dedicated ability gameplay remains disabled or downgraded in co-op pending live verification; A11-A19 inherited systems may still apply subject to live testing. Full live Ascension verification is pending. Ascension 21-30 and custom character work remain out of scope.

## Current State

- Active Ancient reward rebalance implementation exists under `EZMicroBalanceCode/Ancients/`; v4.3 is current. It covers Distinguished Cape's `lose 30% of current Max HP, at least 18` trade gate with same-pool Vakuu replacement when unaffordable, Prismatic Gem's "Every second standard card reward contains only off-color cards" behavior with reward-screen hint fallback diagnostics, Velvet Choker's retained v4.2 soft limit, and no-space Simplified Chinese number formatting. v4.2 rightmost-slot Prismatic Gem is historical only. v4.2 Distinguished Cape 40% min15 is historical only.
- Legacy Ancient work has been removed from the active root; historical scaffold metadata stays in `docs/archive/legacy-planning/`.
- `EZMicroBalance` is the single active solution, project, manifest, resource folder, code folder, DLL, and PCK.
- Preview tools live inside Spire Plus under `EZMicroBalanceCode/Preview/`.
- English and Simplified Chinese localization files exist for changed Ancient rewards under `EZMicroBalance/localization/`.
- The latest implementation pass has been refreshed for Slay the Spire 2 `v0.106.0` with BaseLib `v3.1.4`: `source code/` was cleaned and regenerated from the current installed PCK/DLL, build succeeds with 0 warnings/errors, default tests pass, and publish/package refresh succeeded for `publish/SpirePlus-v0.1.0-private-beta.0.zip` with `EZMicroBalance/` as the install folder. Release-artifact tests remain an opt-in gate after hash/doc refreshes. Historical package smoke/resource evidence under `.tools/runtime-evidence/current-package-smoke-20260514-015901` verified an earlier installed package with headless Ancient scene/texture loading and a normal Steam helper startup. current source now defines 26 SavedSpireFields after later static fixes, so refreshed runtime smoke remains pending for the current package. A BaseLib-only plug-off startup/log pass under `.tools/runtime-evidence/live-spire-plus-disabled-session-20260513-143020` loaded 1 mod, did not initialize Spire Plus / `EZMicroBalance`, and audited clean. A refreshed Mod Settings UI screenshot shows `Spire Plus` in the Mods list for the current display-name package; full live gameplay/manual feature verification is still pending.
- Ascension 11-20 has a v2.2 development checklist at `docs/features/ascension-11-20/development-checklist-v2.md`. Active prototype slices live under `EZMicroBalanceCode/Ascension/`: default-on original-UI A11-A20 selection for this private-beta multiplayer test candidate, A11 +1 map column with an inserted-column optional route plus extra route rows in Act 1/2/3 (`+1/+1/+2`) without A11-specific map markers and with a source-boundary `ActModel.CreateMap` geometry patch, Rootblight/Blight Sprout gameplay with a four-Rootblight deck cap, source-hardened combat-end notice overlays, and generated portrait art, A12 Firemarked Elite/Forge Token with dedicated map/status indicators and Heal/Smith payout, A13 Fission at 10% normal / 15% Banner / 20% Firemarked Elite / 5% Boss reward rates with stricter eligibility and icon support, A16 Banner Rooms, A17 optional Act 2/3 Deep Branches with a guarded enhanced treasure reward, A19 source-guarded boss-specific dedicated ability hooks with Boss-map hover text, and A20 vanilla double-boss map creation with Boss 2 Branded Form metadata/parameters, Boss-map Branded Form hover text, Boss 1 post-combat recovery, a Boss 1 card reward, Boss 1 reward-screen intermission wording, and a fixed courtyard event inserted before Boss 2 through the vanilla terminal-reward path. These prototype slices still need full live Ascension verification; the current A11 source-boundary hardening still needs fresh visible row/width proof in current gameplay; Rootblight v2.2 full combat-end behavior, generated-art visual verification, and co-op ownership/desync remain pending; Forge Token special rest-site payout and a bespoke full-screen intermission remain deferred. Startup/log checks are not the same as live co-op verification.

## Requirements

- Slay the Spire 2 public beta, current local target `v0.106.0`
- .NET SDK 9.0.313 or compatible
- Godot .NET / Mono 4.5.1
- BaseLib runtime `v3.1.4` installed under `<GameRoot>\mods\BaseLib`
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
- `docs/test-ready-development-goal.md`: current long-scope development goal for taking Spire Plus to a test-ready candidate.
- `docs/dev-environment.md`: local versions, paths, latest command results, and smoke evidence.
- `docs/release-checklist.md`: private beta release gates and pending manual checks.
- `docs/private-beta-verification-handoff.md`: concise tester handoff.
- `docs/features/ancients-rework-v4/README.md`: Ancient reward rebalance entry point.
- `docs/features/ascension-11-20/README.md`: Ascension 11-20 development entry point.
- `website/README.md`: public static site, effect tables, GitHub Pages workflow, and download-link maintenance.
- `docs/archive/README.md`: archived historical planning policy.

## Release Policy

- Do not change an existing manifest id in-place.
- Do not copy original Slay the Spire 2 assets into this repo.
- Do not copy large decompiled game code bodies into this repo.
- Keep the active private-beta surface as one `Spire Plus / EZMicroBalance` mod unless the owner explicitly approves a new separate manifest.
- Push private beta changes only after explicit user approval.
