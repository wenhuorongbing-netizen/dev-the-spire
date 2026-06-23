# RitsuLib Integration - Current Record

## Status

Compile and manifest dependency are active. `EZMicroBalance.csproj` references `STS2.RitsuLib` only, and `EZMicroBalance.json` declares only `STS2-RitsuLib` as the runtime dependency. Current source has 169 migrated RitsuLib `IPatchMethod` patch classes and 0 raw Harmony declarations remaining after the ascension localization fallback, visual-hover UI, A20 reward proceed/portrait and boss-flow, Meat Cleaver rest-site UI, Preview transform prediction source/lifetime, Ascension selection/lobby UI, Neow/Vakuu event-option UI, Act Ancient unlock-list UI, Vakuu event-state UI, core inline-localization fallback, Ancient reward getter/relic hook, low-risk reward hook, Aeonglass intent UI, Enemy Damage polish getter, RitsuLib compatibility, Urda transform/Seedbed hook, Urda Root Sight room-routing, Ascension map generation, multiplayer join/lobby/run-state/save-quit diagnostics, and debug-only StS1 replacement-prototype migrations. Current beta.135 package parity, runtime preflight, and source-workspace validation exist on Slay the Spire 2 `v0.107.1` with official `STS2-RitsuLib` `v0.4.34` in direct NuGet runtime layout. Previous beta.128 runtime proof applies 152/152 registered Spire Plus RitsuLib patches from that older packaged source state; recapture beta.135 runtime proof before claiming current in-game coverage. Older beta.123/beta.99/beta.96/beta.93 and earlier packets are previous-package or previous-game-version evidence only.

- Compile package: `STS2.RitsuLib` `0.4.34` from NuGet.
- Runtime dependency: manifest declares `STS2-RitsuLib` with `min_version: 0.4.34`.
- Installed runtime: official `STS2-RitsuLib` `v0.4.34` direct NuGet layout under the E-drive game root.
- Historical validated game target: Slay the Spire 2 `v0.106.1`.
- Current local installed game: Slay the Spire 2 `v0.107.1`.
- Only current runtime dependency target: `STS2-RitsuLib` for Spire Plus.

Future migration work has two first checks: confirm the latest stable RitsuLib
package line, then inspect the unpacked local game source under
`source code/src/Core/` before changing game-facing behavior.

The current `v0.107.1` game install uses the official RitsuLib `v0.4.34` direct NuGet runtime files. Installed beta.135 package parity is recorded on 2026-06-23. Packaged beta.135 evidence covers build, publish, package parity, runtime preflight 28 / 0, and source-workspace validation 57 / 0 with retained GDRE warnings only. Latest clicked Ancient UI smoke proof remains previous beta.128 evidence at `.tools/runtime-evidence/monkey-stability-20260623-062913/`; it covered `URDA`, `MORVI`, `LOTHA`, and normal `VAKUU` with command ACKs, screenshots, clean audits, StS1 Off verifier pass, exact game/Ritsu/package markers, 152/152 default runtime Spire Plus ModPatcher patches applied, and packet verification 1621 / 0. This smoke is previous-package smoke-level clicked Ancient UI proof only. Previous beta.123/beta.99 settings/off proof, beta.96 direct Off proof, beta.93 AdditiveBatch1 proof, and older loader packets remain previous-package or previous-game-version context.

Revision M source-fix context exists under `.tools/runtime-evidence/v01070-current-source-getter-targets-20260610-1000/`: it reached main menu on `v0.107.0`, selected RitsuLib compat branch `0.107.0`, applied 25/25 Spire Plus ModPatcher patches, and audited clean. Current beta.135 proof includes package parity, runtime preflight, and source-workspace validation only; previous beta.128 covers forced clicked Ancient UI smoke. Gameplay, current enabled-mode registration/gameplay, gated Vakuu fight-option/victory return, save-load, image/render, replacement functional proof, co-op/fail-closed proof, independent QA, and versioned tester-package handoff remain pending. Handoff must recapture HEAD and worktree status after any later edits; post-baseline no-game/doc-governance recaptures do not close manual gameplay gates.

Package metadata decision: the beta.99 RitsuLib refresh bumped both `STS2.RitsuLib` and the `STS2-RitsuLib` manifest minimum to `0.4.34` after NuGet and Nexus showed `0.4.34` as the latest available package line. The current package version is `v0.1.0-private-beta.135` after SavedAttachedState initialization hardening, RitsuLib default localization alias coverage, visual-hover UI getter migration, rest-site UI migration, Preview transform prediction source/lifetime migration, Act Ancient unlock-list and Vakuu event-state UI migration, Ancient reward getter/relic hook migration, low-risk reward hook migration, Aeonglass intent UI migration, Enemy Damage polish getter migration, Urda Root Sight room-routing migration, multiplayer join/lobby/run-state/save-quit diagnostic migration, and beta.135 package refresh.

Bootstrap structure: `RitsuLibBootstrap` now applies migrated patches through
`RitsuLibFramework.ApplyRequiredPatcher(...)` instead of calling
`patcher.PatchAll()` directly. If a required migrated patch fails to resolve or
apply, Spire Plus stops before saved-state, settings, content, or feature
registration so the runtime cannot continue in a partially migrated state.
This file owns the dependency/API ledger and detailed RitsuLib ownership plan;
`docs/features/ritsulib-migration/README.md` is kept as the compact entry point,
and `docs/patch-inventory.md` owns generated patch counts.
`SpirePlusMigratedPatchRegistry.cs` now keeps only the ordered registration
entry point. Its sibling partials own the feature domains:
`SpirePlusMigratedPatchRegistry.Ui.cs` for event, clicked, map, settings, and
selection UI; `SpirePlusMigratedPatchRegistry.PreviewUi.cs` for local-only
preview-tool UI; `SpirePlusMigratedPatchRegistry.DisplayUi.cs` for display-only
icon, hover, intent, and damage-number registrations;
`SpirePlusMigratedPatchRegistry.Rewards.cs` for card, relic, and reward hooks;
`SpirePlusMigratedPatchRegistry.Localization.cs` for localization and
RitsuLib compatibility hooks; and
`SpirePlusMigratedPatchRegistry.Gameplay.cs` for gameplay and diagnostics. This
keeps the completed clicked-UI migration auditable in code, not only in docs.

## What Is RitsuLib

RitsuLib is a shared framework library for Slay the Spire 2 mods. It provides `CreateContentPack`, `CreatePatcher`, `SubscribeLifecycle<TEvent>`, `BeginModDataRegistration/GetDataStore`, and `RegisterModSettings` entry points for structured mod registration.

References:

- RitsuLib Getting Started: `https://sts2-ritsulib.ritsukage.com/guide/getting-started`
- RitsuLib GitHub: `https://github.com/BAKAOLC/STS2-RitsuLib`
- RitsuLib Framework Design: `https://sts2-ritsulib.ritsukage.com/guide/framework-design`
- RitsuLib Mod Settings: `https://sts2-ritsulib.ritsukage.com/guide/mod-settings`
- RitsuLib Patching Guide: `https://sts2-ritsulib.ritsukage.com/guide/patching-guide`
- RitsuLib Persistence Guide: `https://sts2-ritsulib.ritsukage.com/guide/persistence-guide`

## External Version Recheck

2026-06-23 recheck:

- `dotnet list EZMicroBalance.csproj package --outdated --include-transitive`
  found no `STS2.RitsuLib` update; only transitive `System.IO.Hashing`
  reported `9.0.0 -> 10.0.9`.
- The focused repeatable check for the NuGet latest package, the project
  `PackageReference`, and the manifest `STS2-RitsuLib` runtime minimum is:
  ```powershell
  scripts/check-ritsulib-latest-package.ps1 -ExpectedLatestVersion 0.4.34 -FailOnMismatch
  ```
- The NuGet flat-container index reports `STS2.RitsuLib` latest `0.4.34`
  across 165 listed versions, with last five `0.4.28` through `0.4.34`.
- The Nexus files page also lists `0.4.34` for the current public package line.
- GitHub release API now reports release tag/name `v0.4.34` / `0.4.34`
  published on 2026-06-22, and the raw `main` branch `mod_manifest.json`
  reports `0.4.34`. Keep NuGet plus the installed XML/runtime as the primary
  dependency floor, with GitHub as corroborating evidence.
- Official docs recheck found the current settings-page API names used by this
  repo: `RegisterModSettings`, `AddSection`, and `AddToggle`; the patching
  guide exposes `ApplyRequiredPatcher`, `CreatePatcher`, `RegisterPatch`, and
  `PatchAll`. Keep future settings and patch work aligned with those names
  before adding local wrappers.
- Local XML confirms `ApplyRequiredPatcher` applies all patches on a
  `ModPatcher`, calls the supplied disable callback on failure, and returns
  false. Spire Plus wraps that false result in an exception to fail closed
  before feature bootstrap.
- Official Mod Settings docs now describe multiple in-game host surfaces for
  registered pages: the main-menu RitsuLib shortcut, pause-menu, and settings
  surfaces. The main-menu shortcut is controlled by RitsuLib's
  `main_menu_mod_settings_button_enabled` setting, so tester docs should name
  both the main-menu path and the in-run pause/settings path when settings
  visibility matters.
- Official Mod Settings docs also state that interactive controls need stable
  entry ids. Spire Plus keeps these ids as source constants in
  `SpirePlusModConfig.SettingsPage.Ids.cs` because clicked UI screenshots and future automation use
  them as evidence anchors.
- Local NuGet XML confirms `BeginModDataRegistration`, `ModDataStore.Register`,
  and scoped global initialization for persisted data slots. Spire Plus settings
  registration now uses that scope before registering the settings page.
- The local source-workspace checker now fail-closes if installed
  `STS2-RitsuLib.xml` is missing the RitsuLib API markers Spire Plus currently
  depends on: `RegisterModSettings`, `BeginModDataRegistration`,
  `ModDataStore.Register`, `CreateContentPack`, `CreatePatcher`, and
  `SavedAttachedState`.
- Do not use a dev build for a tester package unless the owner explicitly
  approves a dev-runtime validation lane.
- GitHub releases: `https://github.com/BAKAOLC/STS2-RitsuLib/releases`.
- NuGet flat-container index: `https://api.nuget.org/v3-flatcontainer/sts2.ritsulib/index.json`.
- Nexus files page: `https://www.nexusmods.com/slaythespire2/mods/137?tab=files`.
- GitHub manifest: `https://github.com/BAKAOLC/STS2-RitsuLib/blob/main/mod_manifest.json`.
- The current public Slay the Spire 2 update target remains Major Update #2
  `v0.107.1` per SteamDB `https://steamdb.info/patchnotes/23811903/`.
  Its Workshop and RNG-system changes are dependency-sensitive, so any future
  game update must rerun the source-workspace checker and the RitsuLib variant check before claiming compatibility.

## Runtime Installation

The official runtime files should be installed to the game mods directory, not the repo. The current local install was produced from the `STS2.RitsuLib` `0.4.34` NuGet deploy target:

```text
<GameRoot>/mods/STS2-RitsuLib/
  STS2-RitsuLib.dll
  mod_manifest.json
  STS2-RitsuLib.xml
  viewer/
```

Do not commit `STS2-RitsuLib.dll`, `.pck`, `.zip`, or other downloaded runtime binaries into this repository.

Ignored local leftovers `docs/STS2-RitsuLib.0.3.3.variant-pack.zip` and
`docs/codex-app-better-token-main.zip` were removed on 2026-06-21 after a
tracked-reference scan showed no current docs or code depend on them.

## Runtime Contents

The installed official `STS2.RitsuLib` `0.4.34` NuGet runtime deploy contains the direct runtime DLL, XML docs, manifest, and viewer assets:

| File | Size | Purpose |
| --- | ---: | --- |
| `STS2-RitsuLib.dll` | 5.4 MB | Runtime assembly deployed into the mod folder |
| `STS2-RitsuLib.xml` | 1.9 MB | API XML docs |
| `mod_manifest.json` | 408 B | Mod manifest, id `STS2-RitsuLib`, version `0.4.34` |
| `viewer/` | directory | Runtime viewer assets shipped by RitsuLib |

## NuGet Package Status

The installed runtime now covers the current local `v0.107.1` game target through the main `STS2.RitsuLib` package `0.4.34`; no separate compat NuGet package is used.

| NuGet Package | Version | Status |
| --- | --- | --- |
| `STS2.RitsuLib` | `0.4.34` | Current repo compile package |
| `STS2.RitsuLib.Compat.0.103.2` | `0.3.2` | Available |
| `STS2.RitsuLib.Compat.0.104.0` | `0.2.40` | Available |
| `STS2.RitsuLib.Compat.0.105.1` | `0.3.2` | Available |
| `STS2.RitsuLib.Compat.0.106.1` | `0.4.16` | Available for historical `v0.106.1` API |
| `STS2.RitsuLib.Compat.0.107.1` | none | Not used; use main package for current highest API |

Current compile dependency:

```xml
<PackageReference Include="STS2.RitsuLib" Version="0.4.34" PrivateAssets="All" />
```

Current manifest dependency:

```json
{ "id": "STS2-RitsuLib", "min_version": "0.4.34" }
```

## RitsuLib API Ownership Plan

- Bootstrap and patching: `RitsuLibBootstrap` uses `CreatePatcher` and
  `ApplyRequiredPatcher(...)` so critical patch failure disables Spire Plus
  before feature registration. Historical loader packets remain previous-package
  context; beta.135 package parity/source validation is current on `v0.107.1`
  with RitsuLib `v0.4.34`, and beta.135 runtime recapture is still pending.
- Content registration: future new cards, relics, or potions should register
  through RitsuLib `CreateContentPack(...)`. Spire Plus currently does not add
  new card/relic/potion model content through this path.
- StS1 event registration is mode-split under
  `EZMicroBalanceCode/Sts1Events/Runtime`: the dispatcher selects Off,
  CanaryOnly, AdditiveBatch1, or draft modes, while each partial file owns the
  explicit `CreateContentPack` registration calls for that mode.
- Persistence: saved gameplay markers use RitsuLib `SavedAttachedState`; no
  additional RitsuLib data-store sidecar is planned for this pass.
- Settings persistence: preview-tool settings use RitsuLib `ModDataStore` under
  `BeginModDataRegistration(...)`, then bind the RitsuLib settings UI entries to
  that store. The entry ids are source constants because settings screenshots
  and future automation use them as evidence anchors.
- Settings text has two active resource surfaces by design: the native game
  table `EZMicroBalance/localization/{eng,zhs}/settings_ui.json` for
  `LocString("settings_ui", ...)`, and the RitsuLib I18N table
  `EZMicroBalance/localization/settings_ui/{eng,zhs}.json` for
  `ModSettingsText.I18N(...)`. Keep them identical until RitsuLib-backed
  virtual tables replace the remaining native `LocString` caller.
- Card/relic behavior patches: Fiddle, Choices Paradox, Distinguished Cape,
  Black Star, Crossbow, Brightest Flame, Debt, CardModel, Seal of Gold, and
  initial Ancient pickup balancing now use explicit RitsuLib `IPatchMethod`
  registration.
- Clicked/UI migration: 50 UI/input and selection/lobby patch classes now use `IPatchMethod` for Neow reroll,
  Act Ancient unlock-list UI, Urda option/map clicks and visuals, Vakuu fight-option/event-state injection,
  shared map hover, Ascension map/boss hover,
  Sere Talon event/relic visuals, Crystal Sphere peek, transform preview,
  transform prediction source/lifetime tracking, Prismatic Gem reward-screen
  hint, A20 reward-screen wording, Spire Plus mod-info localization, combat
  hand stale-input safety, Meat Cleaver rest-site Cook option UI/click
  replacement, and Ascension selection/lobby A11-A20 selector paths.
- Ascension localization fallback: six patch classes now use `IPatchMethod`
  for `LocString`, `LocManager`, and `LocTable` fallback paths.
- Visual-hover UI: Sere Talon relic icon/texture getters, Prismatic Gem hover
  tips, Jewelry Box hover tips, and the A20 courtyard portrait now use
  `IPatchMethod` targets. Getter targets use `MethodType.Getter` instead of
  compiler getter names.
- Ancient reward hook migration: Iron Club, Brilliant Scarf, Beautiful Bracelet,
  Music Box, and Velvet Choker getter/relic hook patches now use `IPatchMethod`
  targets. This is source registration migration, not live gameplay proof.
- Low-risk Ancient reward hook migration: Jeweled Mask, Jewelry Box, Pael's Horn,
  Pael's Tooth, Preserved Fog, pickup reward gates, Sere Talon, Sovereign Blade,
  Tanx Claws, Toasty Mittens, and Whispering Earring now use `IPatchMethod`
  targets. This is source registration migration only until repackaged and
  smoked in-game.
- Intent UI migration: Aeonglass Laser Echo intent label and total-damage patches
  now use RitsuLib `IPatchMethod` targets.
- Enemy Damage polish migration: Decimillipede, Terror Eel, and Phantasmal
  Gardener damage getter patches now use RitsuLib `IPatchMethod` targets with
  `MethodType.Getter`.
- Urda Root Sight room-routing migration: `RunManager.RollRoomTypeFor` and
  `RunManager.CreateRoom` now use explicit RitsuLib `IPatchMethod` targets for
  committed preview room type/model routing. This is source registration
  migration only until repackaged and smoked in-game.
- Inline localization fallback: four core `LocTable` fallback patches now use
  `IPatchMethod` for Spire Plus inline strings that are intentionally supplied
  by the local provider registry instead of copied game tables.
- RitsuLib compatibility patch: the `ModSettingsGameSettingsEntryButton`
  SelectionReticle compatibility patch uses RitsuLib `IPatchMethod` and keeps
  the settings-button startup path compatible with Slay the Spire 2 `v0.107.1`.
  Previous beta.128 clicked Ancient UI smoke applied all 152 default runtime registered patch classes from the packaged beta.128 state; beta.135 runtime smoke, gameplay, enabled-mode proof, and the debug-only replacement-prototype branch remain pending.
- High-risk surfaces: run lifecycle, map generation, reward-state, save-load,
  death, multiplayer/lobby, and A20 boss-flow patches remain blocked on
  live/manual evidence and explicit owner approval.

Current migrated total: 169 patch classes.

Current raw Harmony remaining: 0 declarations, tracked in `docs/patch-inventory.md`.

## Current Evidence Pointers

- Loader proof and command status, including current `v0.107.1` runtime dependency status: `docs/reviews/current-validation.md`.
- Runtime smoke checklist: `docs/features/ritsulib-migration/runtime-smoke-checklist.md`.
- Archived localization fallback migration record:
  `docs/archive/feature-audits/ritsulib-migration/batch-4c-candidates-20260623.md`.
- Patch inventory: `docs/patch-inventory.md`.
