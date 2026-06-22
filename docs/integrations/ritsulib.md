# RitsuLib Integration - Current Record

## Status

Compile and manifest dependency are active. `EZMicroBalance.csproj` references `STS2.RitsuLib` only, and `EZMicroBalance.json` declares only `STS2-RitsuLib` as the runtime dependency. Current source has 64 migrated RitsuLib `IPatchMethod` patch classes and 107 raw Harmony declarations remaining after the Batch 4c localization and visual-hover UI migrations. Current beta.108 package parity exists on Slay the Spire 2 `v0.107.1` with official `STS2-RitsuLib` `v0.4.33` in direct NuGet runtime layout. Previous beta.107 clicked Ancient UI smoke covers the earlier 46-patch migrated package under `.tools/runtime-evidence/monkey-stability-beta107-rerun-20260622-144051/`. Older beta.99/beta.96/beta.93 and earlier packets are previous-package or previous-game-version evidence only.

- Compile package: `STS2.RitsuLib` `0.4.33` from NuGet.
- Runtime dependency: manifest declares `STS2-RitsuLib` with `min_version: 0.4.33`.
- Installed runtime: official `STS2-RitsuLib` `v0.4.33` direct NuGet layout under the E-drive game root.
- Historical validated game target: Slay the Spire 2 `v0.106.1`.
- Current local installed game: Slay the Spire 2 `v0.107.1`.
- Only current runtime dependency target: `STS2-RitsuLib` for Spire Plus.

Future migration work has two first checks: confirm the latest stable RitsuLib
package line, then inspect the unpacked local game source under
`source code/src/Core/` before changing game-facing behavior.

The current `v0.107.1` game install uses the official RitsuLib `v0.4.33` direct NuGet runtime files. Installed beta.108 package parity is recorded on 2026-06-22. Packaged beta.108 evidence covers build, focused guards, publish, package parity, runtime preflight 28 / 0, and source-workspace validation 58 / 0 with retained GDRE warnings only. Previous beta.107 clicked Ancient UI smoke proof at `.tools/runtime-evidence/monkey-stability-beta107-rerun-20260622-144051/` covered `URDA`, `MORVI`, `LOTHA`, and normal `VAKUU` with command ACKs, screenshots, clean audits, StS1 Off verifier pass, exact game/Ritsu/package markers, all then-current 46 Spire Plus ModPatcher patches applied, and packet verification 1620 / 0. That smoke predates the Batch 4c localization and visual-hover UI source migrations to 64 migrated patch classes. Previous beta.99 settings/off proof, beta.96 direct Off proof, beta.93 AdditiveBatch1 proof, and older loader packets remain previous-package or previous-game-version context.

Revision M source-fix context exists under `.tools/runtime-evidence/v01070-current-source-getter-targets-20260610-1000/`: it reached main menu on `v0.107.0`, selected RitsuLib compat branch `0.107.0`, applied 25/25 Spire Plus ModPatcher patches, and audited clean. Current beta.108 proof includes package parity, runtime preflight, and source-workspace validation; previous beta.107 clicked Ancient UI smoke remains previous-package UI evidence. Gameplay, current enabled-mode registration/gameplay, gated Vakuu fight-option/victory return, save-load, image/render, replacement functional proof, co-op/fail-closed proof, independent QA, and versioned tester-package handoff remain pending. Handoff must recapture HEAD and worktree status after any later edits; post-baseline no-game/doc-governance recaptures do not close manual gameplay gates.

Package metadata decision: the beta.99 RitsuLib refresh bumped both `STS2.RitsuLib` and the `STS2-RitsuLib` manifest minimum to `0.4.33` after NuGet and Nexus showed `0.4.33` as the latest available package line. The current package version is `v0.1.0-private-beta.108` after SavedAttachedState initialization hardening, RitsuLib default localization alias coverage, visual-hover UI getter migration, and beta.108 package refresh.

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

2026-06-22 recheck:

- `dotnet list EZMicroBalance.csproj package --outdated --include-transitive`
  found no `STS2.RitsuLib` update; only transitive `System.IO.Hashing`
  reported `9.0.0 -> 10.0.9`.
- The focused repeatable check for the NuGet latest package, the project
  `PackageReference`, and the manifest `STS2-RitsuLib` runtime minimum is:
  ```powershell
  scripts/check-ritsulib-latest-package.ps1 -ExpectedLatestVersion 0.4.33 -FailOnMismatch
  ```
- The NuGet flat-container index reports `STS2.RitsuLib` latest `0.4.33`
  across 164 listed versions, with last five `0.4.28` through `0.4.33`.
- The Nexus files page also lists `0.4.33` for the current public package line.
  GitHub releases can lag those package channels; GitHub is not the dependency-floor source for this pass.
- A live 2026-06-21 check of the raw `main` branch `mod_manifest.json` returned
  an older version string, so the main branch manifest is not the dependency-floor source.
- Official docs recheck found the current settings-page API names used by this
  repo: `RegisterModSettings`, `AddSection`, and `AddToggle`; the patching
  guide exposes `ApplyRequiredPatcher`, `CreatePatcher`, `RegisterPatch`, and
  `PatchAll`. Keep future settings and patch work aligned with those names
  before adding local wrappers.
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

The official runtime files should be installed to the game mods directory, not the repo. The current local install was produced from the `STS2.RitsuLib` `0.4.33` NuGet deploy target:

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

The installed official `STS2.RitsuLib` `0.4.33` NuGet runtime deploy contains the direct runtime DLL, XML docs, manifest, and viewer assets:

| File | Size | Purpose |
| --- | ---: | --- |
| `STS2-RitsuLib.dll` | 5.4 MB | Runtime assembly deployed into the mod folder |
| `STS2-RitsuLib.xml` | 1.9 MB | API XML docs |
| `mod_manifest.json` | 408 B | Mod manifest, id `STS2-RitsuLib`, version `0.4.33` |
| `viewer/` | directory | Runtime viewer assets shipped by RitsuLib |

## NuGet Package Status

The installed runtime now covers the current local `v0.107.1` game target through the main `STS2.RitsuLib` package `0.4.33`; no separate compat NuGet package is used.

| NuGet Package | Version | Status |
| --- | --- | --- |
| `STS2.RitsuLib` | `0.4.33` | Current repo compile package |
| `STS2.RitsuLib.Compat.0.103.2` | `0.3.2` | Available |
| `STS2.RitsuLib.Compat.0.104.0` | `0.2.40` | Available |
| `STS2.RitsuLib.Compat.0.105.1` | `0.3.2` | Available |
| `STS2.RitsuLib.Compat.0.106.1` | `0.4.16` | Available for historical `v0.106.1` API |
| `STS2.RitsuLib.Compat.0.107.1` | none | Not used; use main package for current highest API |

Current compile dependency:

```xml
<PackageReference Include="STS2.RitsuLib" Version="0.4.33" PrivateAssets="All" />
```

Current manifest dependency:

```json
{ "id": "STS2-RitsuLib", "min_version": "0.4.33" }
```

## RitsuLib API Adoption Plan

- Batch 1: bootstrap and diagnostics scaffold complete; historical `v0.106.1` loader-gate validated by diagnostic smoke, beta.85/beta.86 Off/CanaryOnly loader proof remains previous-package context, beta.87 AdditiveBatch1 proof remains retained `v0.107.0` context, beta.108 package parity/source validation is current on `v0.107.1` with RitsuLib `v0.4.33`, previous beta.107 clicked Ancient UI smoke is previous-package UI context, enabled-mode recapture is pending, and beta.99/beta.96/beta.93 loader proof remains previous-package evidence.
- Batch 2: future new content registration is not currently applicable because Spire Plus does not register new cards, relics, or potions through RitsuLib.
- StS1 event registration is mode-split under
  `EZMicroBalanceCode/Sts1Events/Runtime`: the dispatcher selects Off,
  CanaryOnly, AdditiveBatch1, or draft modes, while each partial file owns the
  explicit `CreateContentPack` registration calls for that mode.
- Batch 3: persistence sidecar experiments are not currently applicable because current saved-state usage has migrated to RitsuLib `SavedAttachedState`; no additional RitsuLib data-store sidecar is planned for this pass.
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
- Batch 4a: 9 low-risk patch classes migrated to `IPatchMethod` and historical `v0.106.1` loader-gate validated.
- Batch 4b: 16 medium-risk patch classes migrated to `IPatchMethod` and historical `v0.106.1` loader-gate validated.
- Clicked/UI migration: 21 UI/input patch classes now use `IPatchMethod` for Urda
  option/map clicks and visuals, shared map hover, Ascension map/boss hover,
  Sere Talon event/relic visuals, Crystal Sphere peek, transform preview,
  Prismatic Gem reward-screen hint, A20 reward-screen wording, and Spire Plus
  mod-info localization, plus combat hand stale-input safety.
- Batch 4c: six ascension localization fallback patch classes now use
  `IPatchMethod` for `LocString`, `LocManager`, and `LocTable` fallback paths.
- Visual-hover UI: Sere Talon relic icon/texture getters, Prismatic Gem hover
  tips, and Jewelry Box hover tips now use `IPatchMethod` getter targets with
  `MethodType.Getter` instead of compiler getter names.
  Runtime proof for the new 64-patch source state remains pending.
- Batch 5: high-risk run, map, reward, save, and multiplayer patches remain blocked on live/manual evidence and owner approval.

Current migrated total: 64 patch classes.

Current raw Harmony remaining: 107 declarations, tracked in `docs/patch-inventory.md`.

## Current Evidence Pointers

- Loader proof and command status, including current `v0.107.1` runtime dependency status: `docs/reviews/current-validation.md`.
- Runtime smoke checklist: `docs/features/ritsulib-migration/runtime-smoke-checklist.md`.
- Batch 4c localization migration record: `docs/features/ritsulib-migration/batch-4c-candidates.md`.
- Patch inventory: `docs/patch-inventory.md`.
