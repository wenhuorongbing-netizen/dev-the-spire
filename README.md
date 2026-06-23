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
- The latest local implementation state targets Slay the Spire 2 `v0.107.1` with official STS2-RitsuLib `v0.4.34` installed from the NuGet package deploy target. The active manifest is `v0.1.0-private-beta.130`; the current package pass refreshed the zip, installed folder, game-root zip, hash docs, installed-package parity, runtime preflight, and source-workspace validation. Forced clicked Ancient UI smoke remains previous beta.128 package evidence. Beta.99 or older proof rows are previous-package context.
- Current source has completed the clicked/input UI migration, visual/hover UI getter migration, rest-site Meat Cleaver UI migration, Preview transform prediction source/lifetime migration, Ascension selection/lobby UI migration, Neow/Vakuu event-option UI migration, Act Ancient unlock-list UI migration, Vakuu event-state UI migration, A20 courtyard portrait migration, Batch 4c localization fallback migration, core inline-localization fallback migration, Ancient reward getter/relic hook migration, low-risk Ancient reward hook migration, Aeonglass intent UI migration, Enemy Damage polish getter migration, Urda Root Sight room-routing migration, and multiplayer Ancient event diagnostic migration to RitsuLib `IPatchMethod` / `ModPatcher`; `docs/patch-inventory.md` records 156 migrated source patch classes and 14 raw Harmony declarations remaining. beta.130 contains this source state, but no beta.130 runtime smoke has been captured yet.
- Runtime update after the latest RitsuLib target fix: previous beta.128 clicked Ancient UI smoke proof is captured under `.tools/runtime-evidence/monkey-stability-20260623-062913/`; 4 / 4 `AncientUiSmoke` iterations passed for Urda, Morvi, Lotha, and normal Vakuu, with screenshots, command ACKs, clean audits, StS1 Off verifier pass, exact game/Ritsu/package markers, 152/152 default runtime Spire Plus ModPatcher patches applied, and packet verification 1621 / 0. beta.130 runtime smoke, manual feature gameplay, save-load, preview-tools live behavior, gated Vakuu fight-option/victory return, current enabled-mode registration/gameplay, co-op, independent QA rerun, and versioned tester-package handoff are still pending.
- Current source defines 30 SavedAttachedState fields. Historical loader rows that reported `Found 22 previous saved-state registrations` or `Found 30 previous saved-state registrations` are previous-package evidence only.
- Local cleanup note: raw local `.tools` runtime-evidence folders may be pruned; those historical entries remain documentation records, not proof that raw evidence folders are currently present.
  previous beta.128 clicked Ancient UI smoke covers only the forced UI paths for that package. Manual feature verification, gameplay, save-load, current enabled-mode proof, co-op, and independent QA evidence are still pending before any live-ready or release-ready claim. Preview-tools live behavior and gated Vakuu fight-option/victory-return evidence also remain pending.
- Ascension 11-20 has a v2.2 development checklist at `docs/features/ascension-11-20/development-checklist-v2.md`. Active prototype slices live under `EZMicroBalanceCode/Ascension/`: default-on original-UI A11-A20 selection for single-player, fail-closed host-multiplayer selection/gameplay unless `SPIREPLUS_ALLOW_UNVERIFIED_COOP_GAMEPLAY=1` is deliberately set, A11 +1 map column with an inserted-column optional route plus extra route rows in Act 1/2/3 (`+1/+1/+2`) without A11-specific map markers and with a source-boundary `ActModel.CreateMap` geometry patch, Rootblight/Blight Sprout gameplay with a four-Rootblight deck cap, source-hardened combat-end notice overlays, and generated portrait art, A12 Firemarked Elite/Forge Token with dedicated map/status indicators and Heal/Smith payout, A13 Fission at 10% normal / 15% Banner / 20% Firemarked Elite / 5% Boss reward rates with stricter eligibility and icon support, A16 Banner Rooms, A17 optional Act 2/3 Deep Branches with a guarded enhanced treasure reward, A19 source-guarded boss-specific dedicated ability hooks with Boss-map hover text, and A20 vanilla double-boss map creation with Boss 2 Branded Form metadata/parameters, Boss-map Branded Form hover text, Boss 1 post-combat recovery, a Boss 1 card reward, Boss 1 reward-screen intermission wording, and a fixed courtyard event inserted before Boss 2 through the vanilla terminal-reward path. These prototype slices still need full live Ascension verification; the current A11 source-boundary hardening still needs fresh visible row/width proof in current gameplay; Rootblight v2.2 full combat-end behavior and generated-art visual verification remain pending; co-op ownership/desync proof is blocked by the default fail-closed policy until deliberate two-client debugging. Forge Token special rest-site payout and a bespoke full-screen intermission remain deferred. Startup/log checks are not the same as live co-op verification.

## Requirements

- Slay the Spire 2 public beta. Historical loader proof used `v0.106.1`; the current local install is `v0.107.1` with STS2-RitsuLib `v0.4.34` direct runtime layout. Installed beta.130 package artifacts and no-launch validation are current; beta.128 forced clicked Ancient UI smoke and beta.99 or older rows are previous-package evidence. Gameplay/manual verification remains pending.
- .NET SDK 9.0.313 or compatible
- Godot .NET / Mono 4.5.1
- STS2-RitsuLib runtime `v0.4.34` or newer installed under `<GameRoot>\mods\STS2-RitsuLib`
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
