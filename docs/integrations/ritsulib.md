# RitsuLib Integration - Staging Record

## Status

**Compile/manifest dependency attempted; runtime unverified.** RitsuLib 0.3.2 is a compile dependency
via NuGet and a declared runtime dependency via manifest. Using the base package because
no compat package for game v0.106.1 exists on NuGet. Players must install the
RitsuLib runtime variant pack to `<GameRoot>/mods/STS2-RitsuLib/`.

Runtime verification status: no RitsuLib loader smoke, no godot.log evidence,
and no in-game RitsuLib loading proof exists. The manifest declaration is done,
but the runtime dependency has never been exercised in a live game session.

## What is RitsuLib

RitsuLib is a shared framework library for Slay the Spire 2 mods. It provides
`CreateContentPack`, `CreatePatcher`, `SubscribeLifecycle<TEvent>`,
`BeginModDataRegistration/GetDataStore`, and `RegisterModSettings` entry points
for structured mod registration.
([docs](https://sts2-ritsulib.ritsukage.com/guide/getting-started),
[GitHub](https://github.com/BAKAOLC/STS2-RitsuLib))

## Runtime Installation (Player/Tester)

The variant pack should be extracted to the game mods directory, not the repo:

```text
<GameRoot>/mods/STS2-RitsuLib/
  STS2-RitsuLib.dll          (root loader)
  mod_manifest.json
  ritsulib-variants.json
  lib/
    0.103.2/
    0.105.1/
    0.106.1/
```

Do not commit `STS2-RitsuLib.dll`, `*.pck`, or `*.zip` into this repository.

## Variant Pack Contents (Uploaded)

The uploaded `STS2-RitsuLib.0.3.3.variant-pack.zip` contains three game-version
variants plus a root loader:

| File | Size | Purpose |
| --- | --- | --- |
| `STS2-RitsuLib.dll` | 22 KB | Root loader (selects variant at runtime) |
| `STS2-RitsuLib.Loader.pdb` | 15 KB | Debug symbols for loader |
| `mod_manifest.json` | 413 B | Mod manifest (id: `STS2-RitsuLib`, version `0.3.3`) |
| `ritsulib-variants.json` | 644 B | Variant selection config (schema 1) |

| Variant Directory | Game Version | DLL Size | Compatibility |
| --- | --- | --- | --- |
| `lib/0.103.2/` | Slay the Spire 2 v0.103.2 | 3.7 MB | Not applicable to current target |
| `lib/0.105.1/` | Slay the Spire 2 v0.105.1 | 3.7 MB | Not applicable to current target |
| `lib/0.106.1/` | Slay the Spire 2 v0.106.1 | 3.7 MB | **Matches current target** |

Each variant directory contains `STS2-RitsuLib.dll`, `.pdb`, `.xml` (XML doc),
and `compat-target.txt`.

## Blocker: NuGet Compat Package Missing

| Item | Value |
| --- | --- |
| Current repo StS2 target | v0.106.1 |
| Current repo BaseLib target | v3.1.4 |
| Available RitsuLib runtime variants | 0.103.2, 0.105.1, **0.106.1** |
| Runtime variant status | **0.106.1 available** |

The runtime variant pack now matches the game target. The remaining blocker
is the NuGet compilation package.

### NuGet Package Status (checked 2026-05-28)

| NuGet Package | Version | Status |
| --- | --- | --- |
| `STS2.RitsuLib` | 0.3.2 (latest) | Available |
| `STS2.RitsuLib.Compat.0.103.2` | 0.3.2 | Available |
| `STS2.RitsuLib.Compat.0.104.0` | 0.2.40 | Available |
| `STS2.RitsuLib.Compat.0.105.1` | 0.3.2 | Available |
| `STS2.RitsuLib.Compat.0.106.1` | -- | **Not published** |

The variant pack ships RitsuLib 0.3.3 but the latest on NuGet is 0.3.2.
No compat package exists for game version 0.106.1. The variant pack DLLs
(0.3.3) are runtime-only; compilation uses the NuGet base package.

### Current dependency setup

csproj (compile):
```xml
<PackageReference Include="STS2.RitsuLib" Version="0.3.2" PrivateAssets="All" />
```

manifest (runtime):
```json
{ "id": "STS2-RitsuLib", "min_version": "0.3.2" }
```

### Upgrade path

When `STS2.RitsuLib.Compat.0.106.1` is published on NuGet, replace the base
package with the compat package:
```xml
<PackageReference Include="STS2.RitsuLib.Compat.0.106.1" Version="0.3.x" PrivateAssets="All" />
```

## RitsuLib API Adoption Plan

Adopt RitsuLib APIs in batches (PR 6+):
- **Batch 1: Bootstrap + diagnostics — scaffold complete; runtime unverified.** RitsuLib logger initialized,
  Harmony patches applied through RitsuLibBootstrap, framework status reported. No live game session has
  exercised the RitsuLib bootstrap path.
- Batch 2: Future new content registration (not applicable — Spire Plus doesn't
  register new cards/relics/potions through RitsuLib)
- Batch 3: Persistence sidecar experiments (not applicable — existing
  SavedSpireFields work, no RitsuLib data store needed)
- **Batch 4a: Low-risk patch migration — Done.** 9 patch classes migrated to
  `IPatchMethod` (Fiddle, ChoicesParadox, DistinguishedCape, BlackStar).
- **Batch 4b: Medium-risk patch migration — Done.** 16 patch classes migrated
  to `IPatchMethod` (Crossbow, BrightestFlame, DebtAndCard, SealOfGold,
  PickupReward). Total migrated: 25.
- Batch 5: High-risk run/map/reward/save/multiplayer patches (blocked on
  evidence backlog reduction)

## References

- [RitsuLib Getting Started](https://sts2-ritsulib.ritsukage.com/guide/getting-started)
- [RitsuLib GitHub](https://github.com/BAKAOLC/STS2-RitsuLib)
- [RitsuLib Framework Design](https://sts2-ritsulib.ritsukage.com/guide/framework-design)
