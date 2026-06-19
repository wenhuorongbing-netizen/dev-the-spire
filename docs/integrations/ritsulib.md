# RitsuLib Integration - Staging Record

## Status

Compile and manifest dependency are active. Runtime loader-gate validation exists for the historical Slay the Spire 2 `v0.106.1` environment and beta.85/beta.86/beta.87 previous-package loader context on Slay the Spire 2 `v0.107.0`. Current beta.88 AdditiveBatch1 direct proof is clean for `v0.107.1` loader/registration only. Current-game RitsuLib is installed as official `v0.4.24` with `lib\0.107.0`.

- Compile package: `STS2.RitsuLib` `0.4.24` from NuGet.
- Runtime dependency: manifest declares `STS2-RitsuLib` with `min_version: 0.4.24`.
- Installed runtime: official `STS2-RitsuLib` `v0.4.24` variant pack under the E-drive game root.
- Historical validated game target: Slay the Spire 2 `v0.106.1`.
- Current local installed game: Slay the Spire 2 `v0.107.1`.
- Current BaseLib target: `v3.3.0`.

Clean diagnostic loader evidence exists for historical Off, CanaryOnly, and AdditiveBatch1 modes with BaseLib, RitsuLib, and Spire Plus loaded, 25/25 migrated ModPatcher patches applied, and 30 SavedSpireFields observed. This remains historical `v0.106.1` loader-gate proof. The current `v0.107.1` game install uses matching RitsuLib `lib\0.107.0` runtime files. Installed beta.88 package parity is recorded on 2026-06-19, and fresh current-package direct proof covers AdditiveBatch1 at `.tools/runtime-evidence/v01071-beta88-baselib330-additive-batch1-direct-cleanlog-20260619-103937/`. It reached main menu, applied 25/25 Spire Plus ModPatcher patches, audited clean, and passed retained mode/packet verifiers with 0 mismatches. Beta.85/beta.86 Off and CanaryOnly evidence remains previous-package loader context. The prior beta.84 package-parity Off smoke under `.tools/runtime-evidence/v01070-off-package-parity-20260610-092045/` is retained as root-cause evidence for stale Spire Plus API targets.

Revision M source-fix context exists under `.tools/runtime-evidence/v01070-current-source-getter-targets-20260610-1000/`: it reached main menu on `v0.107.0`, selected RitsuLib compat branch `0.107.0`, applied 25/25 Spire Plus ModPatcher patches, and audited clean. Current beta.88 proof includes `.tools/runtime-evidence/v01071-beta88-baselib330-additive-batch1-direct-cleanlog-20260619-103937/` (AdditiveBatch1, 10 event types / 14 registration calls). The beta.87 `v0.107.1` blocker was BaseLib `v3.2.1` patch drift, not RitsuLib; BaseLib `v3.3.0` resolves the clean-loader audit. Gameplay, event screenshots, save-load, image/render, replacement functional proof, co-op/fail-closed proof, independent QA, and versioned tester-package handoff remain pending. Handoff must recapture HEAD and worktree status after any later edits; post-baseline no-game/doc-governance recaptures do not close manual gameplay gates.

Package metadata decision: the beta.87 dependency-floor pass intentionally bumped both `STS2.RitsuLib` and the `STS2-RitsuLib` manifest minimum to `0.4.24`; the beta.88 follow-up bumps BaseLib to `v3.3.0` and package version `v0.1.0-private-beta.88` for Slay the Spire 2 `v0.107.1` compatibility recapture.

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

The installed official `STS2-RitsuLib.0.4.24.variant-pack.zip` contains game-version variants plus a root loader:

| File | Size | Purpose |
| --- | ---: | --- |
| `STS2-RitsuLib.dll` | 27 KB | Root loader that selects a runtime variant |
| `STS2-RitsuLib.Loader.pdb` | 16 KB | Debug symbols for the loader |
| `mod_manifest.json` | 414 B | Mod manifest, id `STS2-RitsuLib`, version `0.4.24` |
| `ritsulib-variants.json` | 644 B | Variant selection config, schema 1 |

| Variant Directory | Game Version | DLL Size | Compatibility |
| --- | --- | ---: | --- |
| `lib/0.103.2/` | Slay the Spire 2 `v0.103.2` | 4.9 MB | Does not match current local `v0.107.1` install |
| `lib/0.106.1/` | Slay the Spire 2 `v0.106.1` | 4.9 MB | Matches historical validated target; does not match current local `v0.107.1` install |
| `lib/0.107.0/` | Slay the Spire 2 `v0.107.0` | 4.9 MB | Selected compatibility branch for current local `v0.107.1`; beta.88 AdditiveBatch1 loader/registration proof is clean |

Each variant directory contains `STS2-RitsuLib.dll`, `.pdb`, `.xml`, and `compat-target.txt`.

## NuGet Package Status

The installed runtime variant pack now covers the current local `v0.107.1` game target through the selected `lib\0.107.0` compatibility branch. Spire Plus references the main current-highest `STS2.RitsuLib` package `0.4.24`; no separate `STS2.RitsuLib.Compat.0.107.0` package is used.

| NuGet Package | Version | Status |
| --- | --- | --- |
| `STS2.RitsuLib` | `0.4.24` | Current repo compile package |
| `STS2.RitsuLib.Compat.0.103.2` | `0.3.2` | Available |
| `STS2.RitsuLib.Compat.0.104.0` | `0.2.40` | Available |
| `STS2.RitsuLib.Compat.0.105.1` | `0.3.2` | Available |
| `STS2.RitsuLib.Compat.0.106.1` | `0.4.16` | Available for historical `v0.106.1` API |
| `STS2.RitsuLib.Compat.0.107.0` | none | Not published; use main package for current highest API |

Current compile dependency:

```xml
<PackageReference Include="STS2.RitsuLib" Version="0.4.24" PrivateAssets="All" />
```

Current manifest dependency:

```json
{ "id": "STS2-RitsuLib", "min_version": "0.4.24" }
```

Historical upgrade path, now applied:

```xml
<PackageReference Include="STS2.RitsuLib" Version="0.4.24" PrivateAssets="All" />
```

Current-highest runtime manifest dependency:

```json
{ "id": "STS2-RitsuLib", "min_version": "0.4.24" }
```

## RitsuLib API Adoption Plan

- Batch 1: bootstrap and diagnostics scaffold complete; historical `v0.106.1` loader-gate validated by diagnostic smoke, beta.85/beta.86 Off/CanaryOnly loader proof remains previous-package context, beta.87 AdditiveBatch1 proof remains retained `v0.107.0` context, and current beta.88 AdditiveBatch1 loader/registration proof is clean on `v0.107.1` with RitsuLib `v0.4.24` and BaseLib `v3.3.0`.
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
