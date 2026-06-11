# RitsuLib Integration - Staging Record

## Status

Compile and manifest dependency are active. Runtime loader-gate validation exists for the historical Slay the Spire 2 `v0.106.1` environment and for the current beta.85 Off-mode loader smoke on Slay the Spire 2 `v0.107.0`. Current-game RitsuLib is installed as official `v0.4.16` with `lib\0.107.0`.

- Compile package: `STS2.RitsuLib` `0.3.2` from NuGet.
- Runtime dependency: manifest declares `STS2-RitsuLib` with `min_version: 0.3.2`.
- Installed runtime: official `STS2-RitsuLib` `v0.4.16` variant pack under the E-drive game root.
- Historical validated game target: Slay the Spire 2 `v0.106.1`.
- Current local installed game: Slay the Spire 2 `v0.107.0`.
- Current BaseLib target: `v3.1.4`.

Clean diagnostic loader evidence exists for historical Off, CanaryOnly, and AdditiveBatch1 modes with BaseLib, RitsuLib, and Spire Plus loaded, 25/25 migrated ModPatcher patches applied, and 30 SavedSpireFields observed. This remains historical `v0.106.1` loader-gate proof. The current `v0.107.0` game install has matching RitsuLib `lib\0.107.0` runtime files. Installed beta.85 package parity passed on 2026-06-11, and the beta.85 Off smoke under `.tools/runtime-evidence/v01070-beta85-current-package-runtime-fix-20260611-0510/` reached main menu, applied 25/25 Spire Plus ModPatcher patches, and audited clean. The prior beta.84 package-parity Off smoke under `.tools/runtime-evidence/v01070-off-package-parity-20260610-092045/` is retained as root-cause evidence for stale Spire Plus API targets.

Revision M source-fix context exists under `.tools/runtime-evidence/v01070-current-source-getter-targets-20260610-1000/`: it reached main menu on `v0.107.0`, selected RitsuLib compat branch `0.107.0`, applied 25/25 Spire Plus ModPatcher patches, and audited clean. Current beta.85 Off proof is `.tools/runtime-evidence/v01070-beta85-current-package-runtime-fix-20260611-0510/`: it logs Spire Plus `v0.1.0-private-beta.85`, selects RitsuLib compat branch `0.107.0`, applies 25/25 Spire Plus ModPatcher patches, reaches main menu, and audits clean. Gameplay, event screenshots, save-load, image/render, replacement functional proof, co-op/fail-closed proof, independent QA, clean-worktree decision, and versioned tester-package handoff remain pending.

Package metadata decision: the current dirty source state does not bump the compile package or manifest minimum. Keep `STS2.RitsuLib` and `STS2-RitsuLib` at `0.3.2` unless the owner chooses a dependency-version slice. A future owner-approved `v0.107.0` tester package may move both values to `0.4.16` in the same package-version increment, then refresh publish/package artifacts and runtime evidence.

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
  ritsulib-variants.json
  lib/
    0.103.2/
    0.106.1/
    0.107.0/
```

Do not commit `STS2-RitsuLib.dll`, `.pck`, `.zip`, or other downloaded runtime binaries into this repository.

## Variant Pack Contents

The installed official `STS2-RitsuLib.0.4.16.variant-pack.zip` contains three game-version variants plus a root loader:

| File | Size | Purpose |
| --- | ---: | --- |
| `STS2-RitsuLib.dll` | 27 KB | Root loader that selects a runtime variant |
| `STS2-RitsuLib.Loader.pdb` | 16 KB | Debug symbols for the loader |
| `mod_manifest.json` | 414 B | Mod manifest, id `STS2-RitsuLib`, version `0.4.16` |
| `ritsulib-variants.json` | 644 B | Variant selection config, schema 1 |

| Variant Directory | Game Version | DLL Size | Compatibility |
| --- | --- | ---: | --- |
| `lib/0.103.2/` | Slay the Spire 2 `v0.103.2` | 4.9 MB | Does not match current local `v0.107.0` install |
| `lib/0.106.1/` | Slay the Spire 2 `v0.106.1` | 4.9 MB | Matches historical validated target; does not match current local `v0.107.0` install |
| `lib/0.107.0/` | Slay the Spire 2 `v0.107.0` | 4.9 MB | Matches current local install; beta.85 Off smoke is clean |

Each variant directory contains `STS2-RitsuLib.dll`, `.pdb`, `.xml`, and `compat-target.txt`.

## NuGet Package Status

The installed runtime variant pack now matches the current local `v0.107.0` game target. The compile-time package has not been bumped in source: Spire Plus still references `STS2.RitsuLib` `0.3.2`. NuGet now has `STS2.RitsuLib` `0.4.16`, while no separate `STS2.RitsuLib.Compat.0.107.0` package is published; RitsuLib documentation directs current-highest API users to the main `STS2.RitsuLib` package and older API users to `STS2.RitsuLib.Compat.<api-version>`.

| NuGet Package | Version | Status |
| --- | --- | --- |
| `STS2.RitsuLib` | `0.4.16` | Available latest; repo currently uses `0.3.2` |
| `STS2.RitsuLib.Compat.0.103.2` | `0.3.2` | Available |
| `STS2.RitsuLib.Compat.0.104.0` | `0.2.40` | Available |
| `STS2.RitsuLib.Compat.0.105.1` | `0.3.2` | Available |
| `STS2.RitsuLib.Compat.0.106.1` | `0.4.16` | Available for historical `v0.106.1` API |
| `STS2.RitsuLib.Compat.0.107.0` | none | Not published; use main package for current highest API |

Current compile dependency:

```xml
<PackageReference Include="STS2.RitsuLib" Version="0.3.2" PrivateAssets="All" />
```

Current manifest dependency:

```json
{ "id": "STS2-RitsuLib", "min_version": "0.3.2" }
```

Current-highest API compile-package upgrade path:

```xml
<PackageReference Include="STS2.RitsuLib" Version="0.4.16" PrivateAssets="All" />
```

Current-highest runtime manifest upgrade path for a future versioned package:

```json
{ "id": "STS2-RitsuLib", "min_version": "0.4.16" }
```

## RitsuLib API Adoption Plan

- Batch 1: bootstrap and diagnostics scaffold complete; historical `v0.106.1` loader-gate validated by diagnostic smoke, and current beta.85 Off loader proof is clean on `v0.107.0` with RitsuLib `v0.4.16`.
- Batch 2: future new content registration is not currently applicable because Spire Plus does not register new cards, relics, or potions through RitsuLib.
- Batch 3: persistence sidecar experiments are not currently applicable because existing SavedSpireFields work and no RitsuLib data store is needed.
- Batch 4a: 9 low-risk patch classes migrated to `IPatchMethod` and historical `v0.106.1` loader-gate validated.
- Batch 4b: 16 medium-risk patch classes migrated to `IPatchMethod` and historical `v0.106.1` loader-gate validated.
- Batch 4c: proposal-only candidate list lives in `docs/features/ritsulib-migration/batch-4c-candidates.md`; no migration is approved yet.
- Batch 5: high-risk run, map, reward, save, and multiplayer patches remain blocked on live/manual evidence and owner approval.

Current migrated total: 25 patch classes.

Current raw Harmony remaining: 142 declarations, tracked in `docs/patch-inventory.md`.

## Current Evidence Pointers

- Loader proof and command status, including current `v0.107.0` runtime dependency status: `docs/reviews/current-validation.md`.
- Runtime smoke checklist: `docs/features/ritsulib-migration/runtime-smoke-checklist.md`.
- Batch 4c candidate proposal: `docs/features/ritsulib-migration/batch-4c-candidates.md`.
- Patch inventory: `docs/patch-inventory.md`.
