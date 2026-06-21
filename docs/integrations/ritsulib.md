# RitsuLib Integration - Current Record

## Status

Compile and manifest dependency are active. `EZMicroBalance.csproj` references `STS2.RitsuLib` only, and `EZMicroBalance.json` declares only `STS2-RitsuLib` as the shared runtime framework dependency. Current beta.97 package parity exists on Slay the Spire 2 `v0.107.1` with official `STS2-RitsuLib` `v0.4.31` and `lib\0.107.1`. Older beta.85/beta.86/beta.87/beta.88/beta.90 packets are previous-package or previous-game-version loader evidence only; beta.97 Off loader and clicked settings UI proof remain pending after the settings-page I18N resource migration. Previous beta.96 RitsuLib Mod Settings clicked UI proof is captured under `.tools/runtime-evidence/beta96-ritsulib-mod-settings-clicked-ui-20260621-160701/`, and previous beta.96 Off loader proof is captured under `.tools/runtime-evidence/v01071-beta96-ritsulib0431-off-direct-20260621-185056/`.

- Compile package: `STS2.RitsuLib` `0.4.31` from NuGet.
- Runtime dependency: manifest declares `STS2-RitsuLib` with `min_version: 0.4.31`.
- Installed runtime: official `STS2-RitsuLib` `v0.4.31` variant pack under the E-drive game root.
- Historical validated game target: Slay the Spire 2 `v0.106.1`.
- Current local installed game: Slay the Spire 2 `v0.107.1`.
- Current shared runtime framework target: `STS2-RitsuLib` only for Spire Plus.

The current `v0.107.1` game install uses matching RitsuLib `lib\0.107.1` runtime files. Installed beta.97 package parity is recorded on 2026-06-21. Previous beta.96 settings-page proof shows the RitsuLib Mods tree with `RitsuLib` and `Spire Plus` only and the Spire Plus page with Migration Status, `STS2-RitsuLib >= 0.4.31`, evidence-boundary, technical-id, and Preview Tools controls. Previous beta.96 direct Off proof at `.tools/runtime-evidence/v01071-beta96-ritsulib0431-off-direct-20260621-185056/` reached main menu with exactly `STS2-RitsuLib` and `EZMicroBalance` loaded, applied 25/25 Spire Plus ModPatcher patches, audited clean, logged StS1Events disabled with 0 registration lines, and passed packet verification 43 / 0. Previous beta.93 RitsuLib-only AdditiveBatch1 at `.tools/runtime-evidence/v01071-beta93-ritsulib0431-additivebatch1-direct-20260621/` remains previous-package registration context: it registered 10 event types through 14 calls and passed retained verifiers for that older package. Older loader packets remain previous-package or previous-game-version context. The prior beta.84 package-parity Off smoke under `.tools/runtime-evidence/v01070-off-package-parity-20260610-092045/` is retained as root-cause evidence for stale Spire Plus API targets.

Revision M source-fix context exists under `.tools/runtime-evidence/v01070-current-source-getter-targets-20260610-1000/`: it reached main menu on `v0.107.0`, selected RitsuLib compat branch `0.107.0`, applied 25/25 Spire Plus ModPatcher patches, and audited clean. Current beta.97 proof is package parity; settings UI and Off loader startup need recapture after the RitsuLib I18N settings resource migration. Previous beta.96 proof includes `.tools/runtime-evidence/beta96-ritsulib-mod-settings-clicked-ui-20260621-160701/` for settings UI and `.tools/runtime-evidence/v01071-beta96-ritsulib0431-off-direct-20260621-185056/` for Off loader startup. Gameplay, current enabled-mode registration/gameplay, event screenshots, save-load, image/render, replacement functional proof, co-op/fail-closed proof, independent QA, and versioned tester-package handoff remain pending. Handoff must recapture HEAD and worktree status after any later edits; post-baseline no-game/doc-governance recaptures do not close manual gameplay gates.

Package metadata decision: the beta.93 dependency-floor pass intentionally bumped both `STS2.RitsuLib` and the `STS2-RitsuLib` manifest minimum to `0.4.31` for the Slay the Spire 2 `v0.107.1` compatibility recapture. The beta.97 pass keeps that dependency floor and bumps the package version to `v0.1.0-private-beta.97` for the RitsuLib settings page I18N resource migration.

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

2026-06-21 web recheck:

- RitsuLib GitHub release `v0.4.31` is marked Latest, the NuGet package index
  includes `0.4.31`, and the Nexus files page now lists the variant-pack main
  file as `0.4.31`; the earlier Nexus `0.4.28` lag is historical only.
- GitHub latest-release API and the `v0.4.31` tag remain the GitHub version
  authority for the current stable dependency target. A live 2026-06-21 check of
  the raw `main` branch `mod_manifest.json` returned an older version string, so
  the main branch manifest is not the dependency-floor source.
- Official docs recheck found the current settings-page API names used by this
  repo: `RegisterModSettings`, `AddSection`, and `AddToggle`; the patching
  guide exposes `ApplyRequiredPatcher`, `CreatePatcher`, `RegisterPatch`, and
  `PatchAll`. Keep future settings and patch work aligned with those names
  before adding local wrappers.
- Local NuGet XML confirms `BeginModDataRegistration`, `ModDataStore.Register`,
  and scoped global initialization for persisted data slots. Spire Plus settings
  registration now uses that scope before registering the settings page.
- Do not use a dev build for a tester package unless the owner explicitly
  approves a dev-runtime validation lane.
- GitHub release: `https://github.com/BAKAOLC/STS2-RitsuLib/releases/tag/v0.4.31`.
- NuGet flat-container index: `https://api.nuget.org/v3-flatcontainer/sts2.ritsulib/index.json`.
- Nexus files page: `https://www.nexusmods.com/slaythespire2/mods/137?tab=files`.
- GitHub manifest: `https://github.com/BAKAOLC/STS2-RitsuLib/blob/main/mod_manifest.json`.
- The current public Slay the Spire 2 update target remains Major Update #2
  `v0.107.1` per SteamDB `https://steamdb.info/patchnotes/23811903/`.
  Its Workshop and RNG-system changes are dependency-sensitive, so any future
  game update must rerun the source-workspace checker and the RitsuLib variant check before claiming compatibility.

## Runtime Installation

The variant pack should be extracted to the game mods directory, not the repo:

```text
<GameRoot>/mods/STS2-RitsuLib/
  STS2-RitsuLib.dll
  mod_manifest.json
  ritsulib-variants.manifest
  lib/
    0.107.1/
```

Do not commit `STS2-RitsuLib.dll`, `.pck`, `.zip`, or other downloaded runtime binaries into this repository.

Ignored local leftovers `docs/STS2-RitsuLib.0.3.3.variant-pack.zip` and
`docs/codex-app-better-token-main.zip` were removed on 2026-06-21 after a
tracked-reference scan showed no current docs or code depend on them.

## Variant Pack Contents

The installed official `STS2-RitsuLib.0.4.31.variant-pack.zip` contains game-version variants plus a root loader:

| File | Size | Purpose |
| --- | ---: | --- |
| `STS2-RitsuLib.dll` | 27 KB | Root loader that selects a runtime variant |
| `STS2-RitsuLib.Loader.pdb` | 16 KB | Debug symbols for the loader |
| `mod_manifest.json` | 414 B | Mod manifest, id `STS2-RitsuLib`, version `0.4.31`, min game `0.107.1` |
| `ritsulib-variants.manifest` | 240 B | Variant selection config, schema 1 |

| Variant Directory | Game Version | DLL Size | Compatibility |
| --- | --- | ---: | --- |
| `lib/0.107.1/` | Slay the Spire 2 `v0.107.1` | 5.3 MB | Selected compatibility branch for current local `v0.107.1`; beta.97 package parity is current evidence, beta.97 loader recapture is pending, and beta.96/beta.93 loader proof is previous-package evidence |

Each variant directory contains `STS2-RitsuLib.dll`, `.pdb`, `.xml`, and `compat-target.txt`.

## NuGet Package Status

The installed runtime variant pack now covers the current local `v0.107.1` game target through the selected `lib\0.107.1` compatibility branch. Spire Plus references the main current-highest `STS2.RitsuLib` package `0.4.31`; no separate compat NuGet package is used.

| NuGet Package | Version | Status |
| --- | --- | --- |
| `STS2.RitsuLib` | `0.4.31` | Current repo compile package |
| `STS2.RitsuLib.Compat.0.103.2` | `0.3.2` | Available |
| `STS2.RitsuLib.Compat.0.104.0` | `0.2.40` | Available |
| `STS2.RitsuLib.Compat.0.105.1` | `0.3.2` | Available |
| `STS2.RitsuLib.Compat.0.106.1` | `0.4.16` | Available for historical `v0.106.1` API |
| `STS2.RitsuLib.Compat.0.107.1` | none | Not used; use main package for current highest API |

Current compile dependency:

```xml
<PackageReference Include="STS2.RitsuLib" Version="0.4.31" PrivateAssets="All" />
```

Current manifest dependency:

```json
{ "id": "STS2-RitsuLib", "min_version": "0.4.31" }
```

## RitsuLib API Adoption Plan

- Batch 1: bootstrap and diagnostics scaffold complete; historical `v0.106.1` loader-gate validated by diagnostic smoke, beta.85/beta.86 Off/CanaryOnly loader proof remains previous-package context, beta.87 AdditiveBatch1 proof remains retained `v0.107.0` context, beta.97 package parity is current on `v0.107.1` with RitsuLib `v0.4.31`, beta.97 loader recapture is pending, and beta.96/beta.93 loader proof remains previous-package evidence.
- Batch 2: future new content registration is not currently applicable because Spire Plus does not register new cards, relics, or potions through RitsuLib.
- Batch 3: persistence sidecar experiments are not currently applicable because current saved-state usage has migrated to RitsuLib `SavedAttachedState`; no additional RitsuLib data-store sidecar is planned for this pass.
- Settings persistence: preview-tool settings use RitsuLib `ModDataStore` under
  `BeginModDataRegistration(...)`, then bind the RitsuLib settings UI entries to
  that store. The entry ids are source constants because settings screenshots
  and future automation use them as evidence anchors.
- Batch 4a: 9 low-risk patch classes migrated to `IPatchMethod` and historical `v0.106.1` loader-gate validated.
- Batch 4b: 16 medium-risk patch classes migrated to `IPatchMethod` and historical `v0.106.1` loader-gate validated.
- Batch 4c: proposal-only candidate list lives in `docs/features/ritsulib-migration/batch-4c-candidates.md`; no migration is approved yet.
- Batch 5: high-risk run, map, reward, save, and multiplayer patches remain blocked on live/manual evidence and owner approval.

Current migrated total: 25 patch classes.

Current raw Harmony remaining: 146 declarations, tracked in `docs/patch-inventory.md`.

## Current Evidence Pointers

- Loader proof and command status, including current `v0.107.1` runtime dependency status: `docs/reviews/current-validation.md`.
- Runtime smoke checklist: `docs/features/ritsulib-migration/runtime-smoke-checklist.md`.
- Batch 4c candidate proposal: `docs/features/ritsulib-migration/batch-4c-candidates.md`.
- Patch inventory: `docs/patch-inventory.md`.
