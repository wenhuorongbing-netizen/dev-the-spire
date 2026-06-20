# RitsuLib Integration - Staging Record

## Status

Compile and manifest dependency are active. Current Spire Plus no longer depends on BaseLib: `EZMicroBalance.csproj` references `STS2.RitsuLib` only, and `EZMicroBalance.json` declares only `STS2-RitsuLib` as the shared runtime framework dependency. Runtime loader-gate validation exists for the current Slay the Spire 2 `v0.107.1` environment with official `STS2-RitsuLib` `v0.4.28` and `lib\0.107.1`. Beta.85/beta.86/beta.87, beta.88 BaseLib-backed, and beta.90 RitsuLib-only packets are previous-package or previous-dependency-context loader evidence.

- Compile package: `STS2.RitsuLib` `0.4.28` from NuGet.
- Runtime dependency: manifest declares `STS2-RitsuLib` with `min_version: 0.4.28`.
- Installed runtime: official `STS2-RitsuLib` `v0.4.28` variant pack under the E-drive game root.
- Historical validated game target: Slay the Spire 2 `v0.106.1`.
- Current local installed game: Slay the Spire 2 `v0.107.1`.
- Current BaseLib target: none for Spire Plus; BaseLib may remain installed locally only for other mods or historical comparison.

Clean diagnostic loader evidence exists for historical Off, CanaryOnly, and AdditiveBatch1 modes with BaseLib, RitsuLib, and Spire Plus loaded, 25/25 migrated ModPatcher patches applied, and 30 SavedSpireFields observed. This remains historical `v0.106.1` loader-gate proof. The current `v0.107.1` game install uses matching RitsuLib `lib\0.107.1` runtime files. Installed beta.91 package parity is recorded on 2026-06-20, and fresh current-package direct proof covers RitsuLib-only Off at `.tools/runtime-evidence/v01071-beta91-ritsulib0428-off-direct-20260620/` plus RitsuLib-only AdditiveBatch1 at `.tools/runtime-evidence/v01071-beta91-ritsulib0428-additivebatch1-direct-20260620/`. Both reached main menu with exactly `STS2-RitsuLib` and `EZMicroBalance` loaded, applied 25/25 Spire Plus ModPatcher patches, audited clean, and passed retained packet verifiers; the AdditiveBatch1 proof registered 10 event types through 14 calls. Beta.85/beta.86/beta.87, beta.88, and beta.90 evidence remains previous-package loader context. The prior beta.84 package-parity Off smoke under `.tools/runtime-evidence/v01070-off-package-parity-20260610-092045/` is retained as root-cause evidence for stale Spire Plus API targets.

Revision M source-fix context exists under `.tools/runtime-evidence/v01070-current-source-getter-targets-20260610-1000/`: it reached main menu on `v0.107.0`, selected RitsuLib compat branch `0.107.0`, applied 25/25 Spire Plus ModPatcher patches, and audited clean. Current beta.91 proof includes `.tools/runtime-evidence/v01071-beta91-ritsulib0428-off-direct-20260620/` and `.tools/runtime-evidence/v01071-beta91-ritsulib0428-additivebatch1-direct-20260620/`. The beta.87 `v0.107.1` blocker was BaseLib `v3.2.1` patch drift, not RitsuLib; the beta.91 package removes BaseLib from Spire Plus' dependency chain instead of carrying that compatibility requirement forward. Gameplay, event screenshots, save-load, image/render, replacement functional proof, co-op/fail-closed proof, independent QA, and versioned tester-package handoff remain pending. Handoff must recapture HEAD and worktree status after any later edits; post-baseline no-game/doc-governance recaptures do not close manual gameplay gates.

Package metadata decision: the beta.91 dependency-floor pass intentionally bumped both `STS2.RitsuLib` and the `STS2-RitsuLib` manifest minimum to `0.4.28`, removed BaseLib from current Spire Plus package requirements, and set the package version to `v0.1.0-private-beta.91` for the Slay the Spire 2 `v0.107.1` compatibility recapture.

## What Is RitsuLib

RitsuLib is a shared framework library for Slay the Spire 2 mods. It provides `CreateContentPack`, `CreatePatcher`, `SubscribeLifecycle<TEvent>`, `BeginModDataRegistration/GetDataStore`, and `RegisterModSettings` entry points for structured mod registration.

References:

- RitsuLib Getting Started: `https://sts2-ritsulib.ritsukage.com/guide/getting-started`
- RitsuLib GitHub: `https://github.com/BAKAOLC/STS2-RitsuLib`
- RitsuLib Framework Design: `https://sts2-ritsulib.ritsukage.com/guide/framework-design`

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

## Variant Pack Contents

The installed official `STS2-RitsuLib.0.4.28.variant-pack.zip` contains game-version variants plus a root loader:

| File | Size | Purpose |
| --- | ---: | --- |
| `STS2-RitsuLib.dll` | 27 KB | Root loader that selects a runtime variant |
| `STS2-RitsuLib.Loader.pdb` | 16 KB | Debug symbols for the loader |
| `mod_manifest.json` | 414 B | Mod manifest, id `STS2-RitsuLib`, version `0.4.28`, min game `0.107.1` |
| `ritsulib-variants.manifest` | 240 B | Variant selection config, schema 1 |

| Variant Directory | Game Version | DLL Size | Compatibility |
| --- | --- | ---: | --- |
| `lib/0.107.1/` | Slay the Spire 2 `v0.107.1` | 5.3 MB | Selected compatibility branch for current local `v0.107.1`; beta.91 Off and AdditiveBatch1 loader/registration proof is clean |

Each variant directory contains `STS2-RitsuLib.dll`, `.pdb`, `.xml`, and `compat-target.txt`.

## NuGet Package Status

The installed runtime variant pack now covers the current local `v0.107.1` game target through the selected `lib\0.107.1` compatibility branch. Spire Plus references the main current-highest `STS2.RitsuLib` package `0.4.28`; no separate compat NuGet package is used.

| NuGet Package | Version | Status |
| --- | --- | --- |
| `STS2.RitsuLib` | `0.4.28` | Current repo compile package |
| `STS2.RitsuLib.Compat.0.103.2` | `0.3.2` | Available |
| `STS2.RitsuLib.Compat.0.104.0` | `0.2.40` | Available |
| `STS2.RitsuLib.Compat.0.105.1` | `0.3.2` | Available |
| `STS2.RitsuLib.Compat.0.106.1` | `0.4.16` | Available for historical `v0.106.1` API |
| `STS2.RitsuLib.Compat.0.107.1` | none | Not used; use main package for current highest API |

Current compile dependency:

```xml
<PackageReference Include="STS2.RitsuLib" Version="0.4.28" PrivateAssets="All" />
```

Current manifest dependency:

```json
{ "id": "STS2-RitsuLib", "min_version": "0.4.28" }
```

Historical upgrade path, now superseded by beta.91:

```xml
<PackageReference Include="STS2.RitsuLib" Version="0.4.28" PrivateAssets="All" />
```

Current-highest runtime manifest dependency:

```json
{ "id": "STS2-RitsuLib", "min_version": "0.4.28" }
```

## RitsuLib API Adoption Plan

- Batch 1: bootstrap and diagnostics scaffold complete; historical `v0.106.1` loader-gate validated by diagnostic smoke, beta.85/beta.86 Off/CanaryOnly loader proof remains previous-package context, beta.87 AdditiveBatch1 proof remains retained `v0.107.0` context, beta.88 remains previous BaseLib-backed context, and current beta.91 Off/AdditiveBatch1 loader/registration proof is clean on `v0.107.1` with RitsuLib `v0.4.28` and no Spire Plus BaseLib dependency.
- Batch 2: future new content registration is not currently applicable because Spire Plus does not register new cards, relics, or potions through RitsuLib.
- Batch 3: persistence sidecar experiments are not currently applicable because existing SavedSpireFields work and no RitsuLib data store is needed.
- Batch 4a: 9 low-risk patch classes migrated to `IPatchMethod` and historical `v0.106.1` loader-gate validated.
- Batch 4b: 16 medium-risk patch classes migrated to `IPatchMethod` and historical `v0.106.1` loader-gate validated.
- Batch 4c: proposal-only candidate list lives in `docs/features/ritsulib-migration/batch-4c-candidates.md`; no migration is approved yet.
- Batch 5: high-risk run, map, reward, save, and multiplayer patches remain blocked on live/manual evidence and owner approval.

Current migrated total: 25 patch classes.

Current raw Harmony remaining: 142 declarations, tracked in `docs/patch-inventory.md`.

## Current Evidence Pointers

- Loader proof and command status, including current `v0.107.1` runtime dependency status: `docs/reviews/current-validation.md`.
- Runtime smoke checklist: `docs/features/ritsulib-migration/runtime-smoke-checklist.md`.
- Batch 4c candidate proposal: `docs/features/ritsulib-migration/batch-4c-candidates.md`.
- Patch inventory: `docs/patch-inventory.md`.
