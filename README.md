# dev-the-spire

Slay the Spire 2 mod workspace for private beta development.

## Current Release Target

The active private-beta deliverable is `Spire Plus`: a Slay the Spire 2 balance and progression expansion mod.

The original scaffold manifest id was `EzDailyContent`. That id must not be renamed in-place. The active private-beta deliverable is one mod: `Spire Plus`. Players should see and enable `Spire Plus` as the package name. `EZMicroBalance` remains only as the technical manifest id, compatibility package folder, saved-field prefix, namespace, and legacy alias surface for this cycle.

Ascension 11-20 expansion work is an active development track after the 2026-05-06 overnight sprint goal. A11-A20 selection is default-on for single-player standard lobbies only. After the 2026-05-25 multiplayer crash logs, host-multiplayer A11-A20 selection and gameplay fail closed by default until two-client proof exists. Use `SPIREPLUS_ALLOW_UNVERIFIED_COOP_GAMEPLAY=1` only for deliberate co-op debugging; otherwise keep it unset. Set `SPIREPLUS_ASCENSION_DISABLE_PUBLIC_SELECTION=1` to restore vanilla A1-A10 selection for comparison. `SPIREPLUS_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1` remains a narrower multiplayer selector rollback switch. `EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1` is legacy-compatible and no longer required. Full live Ascension verification is pending. Ascension 21-30 and custom character work remain out of scope.

## Current State

- Active Ancient reward rebalance implementation exists under `EZMicroBalanceCode/Ancients/`; v4.3 is current. It covers Distinguished Cape's `lose 30% of current Max HP, at least 18` trade gate with same-pool Vakuu replacement when unaffordable, Prismatic Gem's "Every second standard card reward contains only off-color cards" behavior with reward-screen hint fallback diagnostics, Velvet Choker's retained v4.2 soft limit, and no-space Simplified Chinese number formatting. v4.2 rightmost-slot Prismatic Gem is historical only. v4.2 Distinguished Cape 40% min15 is historical only.
- Legacy Ancient work has been removed from the active root; historical scaffold metadata stays in `docs/archive/legacy-planning/`.
- `Spire Plus` is the single active mod. Its technical solution, project, manifest, resource folder, code folder, DLL, and PCK still use `EZMicroBalance` for compatibility.
- Preview tools live inside Spire Plus under `EZMicroBalanceCode/Preview/`.
- English and Simplified Chinese localization files exist for changed Ancient rewards under `EZMicroBalance/localization/`.
- The latest local implementation state targets Slay the Spire 2 `v0.107.1` with official STS2-RitsuLib `v0.4.32` installed from the NuGet package deploy target. The active manifest is `v0.1.0-private-beta.99`; the latest RitsuLib-only package pass refreshed the zip, installed folder, game-root zip, and hash docs. Previous beta.96 RitsuLib-only Off loader proof is clean under `.tools/runtime-evidence/v01071-beta96-ritsulib0431-off-direct-20260621-185056/`; previous beta.93 AdditiveBatch1 loader/registration proof remains previous-package context only.
- Runtime update after the latest RitsuLib target fix: diagnostic Off, CanaryOnly, and AdditiveBatch1 loader smokes remain clean historical `v0.106.1` evidence only. Current RitsuLib `v0.4.32` is installed as a direct root runtime DLL; beta.85 Off and CanaryOnly proof remain previous-package loader context, beta.87 AdditiveBatch1 proof remains clean `v0.107.0` loader context, and beta.88/beta.90 proof rows are previous-package contexts. The previous beta.96 RitsuLib-only Off proof reached main menu with exactly `STS2-RitsuLib` and `EZMicroBalance` loaded, clean audit, StS1Events disabled with 0 registration lines, and packet verifier 43 / 0; previous beta.93 AdditiveBatch1 registered 10 event types through 14 calls with verifier 31 / 0 and packet 61 / 0 for the previous package only. Manual feature verification, gameplay, clicked UI, save-load, preview-tools, Vakuu, beta.99 loader/settings proof, current enabled-mode registration/gameplay, co-op, independent QA rerun, and versioned tester-package handoff are still pending.
- Current source defines 30 SavedAttachedState fields. Historical loader rows that reported `Found 22 previous saved-state registrations` or `Found 30 previous saved-state registrations` are previous-package evidence only.
- Local cleanup note: raw local `.tools` runtime-evidence folders may be pruned; those historical entries remain documentation records, not proof that raw evidence folders are currently present.
  previous beta.96 RitsuLib-only Off evidence covers startup/log loading only and does not prove beta.99 disable-mod gameplay inside a run. Gameplay, clicked Ancient UI, save-load, current enabled-mode proof, co-op, and independent QA evidence are still required before any live-ready or release-ready claim. Beta.99 loader/settings proof, preview-tools, and Vakuu evidence also remain pending.
- Ascension 11-20 has a v2.2 development checklist at `docs/features/ascension-11-20/development-checklist-v2.md`. Active prototype slices live under `EZMicroBalanceCode/Ascension/`: default-on original-UI A11-A20 selection for single-player, fail-closed host-multiplayer selection/gameplay unless `SPIREPLUS_ALLOW_UNVERIFIED_COOP_GAMEPLAY=1` is deliberately set, A11 +1 map column with an inserted-column optional route plus extra route rows in Act 1/2/3 (`+1/+1/+2`) without A11-specific map markers and with a source-boundary `ActModel.CreateMap` geometry patch, Rootblight/Blight Sprout gameplay with a four-Rootblight deck cap, source-hardened combat-end notice overlays, and generated portrait art, A12 Firemarked Elite/Forge Token with dedicated map/status indicators and Heal/Smith payout, A13 Fission at 10% normal / 15% Banner / 20% Firemarked Elite / 5% Boss reward rates with stricter eligibility and icon support, A16 Banner Rooms, A17 optional Act 2/3 Deep Branches with a guarded enhanced treasure reward, A19 source-guarded boss-specific dedicated ability hooks with Boss-map hover text, and A20 vanilla double-boss map creation with Boss 2 Branded Form metadata/parameters, Boss-map Branded Form hover text, Boss 1 post-combat recovery, a Boss 1 card reward, Boss 1 reward-screen intermission wording, and a fixed courtyard event inserted before Boss 2 through the vanilla terminal-reward path. These prototype slices still need full live Ascension verification; the current A11 source-boundary hardening still needs fresh visible row/width proof in current gameplay; Rootblight v2.2 full combat-end behavior and generated-art visual verification remain pending; co-op ownership/desync proof is blocked by the default fail-closed policy until deliberate two-client debugging. Forge Token special rest-site payout and a bespoke full-screen intermission remain deferred. Startup/log checks are not the same as live co-op verification.

## Requirements

- Slay the Spire 2 public beta. Historical loader proof used `v0.106.1`; the current local install is `v0.107.1` with STS2-RitsuLib `v0.4.32` direct runtime layout. Installed beta.99 package parity is current package evidence; beta.96 Off loader proof and beta.93 RitsuLib-only AdditiveBatch1 loader proof are previous-package evidence. Gameplay/manual verification remains pending.
- .NET SDK 9.0.313 or compatible
- Godot .NET / Mono 4.5.1
- STS2-RitsuLib runtime `v0.4.32` or newer installed under `<GameRoot>\mods\STS2-RitsuLib`
- Local `Directory.Build.props` copied from `Directory.Build.props.example`

## Local Path Configuration

`Directory.Build.props` is local and gitignored because it contains machine-specific absolute paths.

On a new machine:

1. Copy `Directory.Build.props.example` to `Directory.Build.props`.
2. Fill in `GodotPath`.
3. Fill in `Sts2Path`.
4. Install STS2-RitsuLib under `<GameRoot>\mods\STS2-RitsuLib`.

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
- Keep the active private-beta surface as one `Spire Plus` mod unless the owner explicitly approves a new separate manifest.
- After a successful implementation pass, commit the intended changes and push the current branch so testers can pick up the latest package.
