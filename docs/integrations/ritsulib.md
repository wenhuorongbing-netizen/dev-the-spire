# RitsuLib Integration - Staging Record

## Status

**Staging only.** RitsuLib is not yet a compile or runtime dependency of Spire Plus.
This document records the variant pack contents, installation instructions, and
the version mismatch that blocks hard dependency adoption.

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
variants:

| Variant | Game Version | Compatibility |
| --- | --- | --- |
| `0.103.2` | Slay the Spire 2 v0.103.2 | Not applicable to current target |
| `0.105.1` | Slay the Spire 2 v0.105.1 | Not applicable to current target |
| `0.106.1` | Slay the Spire 2 v0.106.1 | Closest to current target |

## Version Mismatch Blocker

| Item | Value |
| --- | --- |
| Current repo StS2 target | v0.106.0 |
| Current repo BaseLib target | v3.1.4 |
| Available RitsuLib runtime variants | 0.103.2, 0.105.1, 0.106.1 |
| Missing RitsuLib runtime variant | **0.106.0** |

**No 0.106.0-compatible RitsuLib variant exists in the uploaded pack.**

### NuGet Package Status

| NuGet Package | Version | Status |
| --- | --- | --- |
| `STS2.RitsuLib` | 0.3.2 (latest) | Available |
| `STS2.RitsuLib.Compat.0.103.2` | 0.3.2 | Available |
| `STS2.RitsuLib.Compat.0.104.0` | 0.2.40 | Available |
| `STS2.RitsuLib.Compat.0.105.1` | 0.3.2 | Available |
| `STS2.RitsuLib.Compat.0.106.0` | -- | **Not published** |
| `STS2.RitsuLib.Compat.0.106.1` | -- | **Not published** |

The restructure plan referenced RitsuLib 0.3.3 but the latest on NuGet is 0.3.2.
No compat package exists for game version 0.106.0 or 0.106.1.

Until this is resolved, do **not** add RitsuLib to the manifest dependencies:

```json
"dependencies": [
  {
    "id": "BaseLib",
    "min_version": "v3.1.4"
  }
]
```

### Resolution options

1. **Update repo target to v0.106.1** -- build, test, and runtime smoke against
   0.106.1, then adopt the 0.106.1 RitsuLib variant.
2. **Obtain a 0.106.0-compatible RitsuLib build** -- confirm it exists and is
   tested before adding the hard dependency.
3. **Wait** -- keep RitsuLib as a staged runtime companion until a compatible
   variant is confirmed.

## Future Migration Plan (When Unblocked)

When the version blocker is resolved:

1. Add `PackageReference` to the `.csproj`:
   ```xml
   <PackageReference Include="STS2.RitsuLib" Version="0.3.3" PrivateAssets="All" />
   ```
2. Add manifest dependency:
   ```json
   { "id": "STS2-RitsuLib", "min_version": "0.3.3" }
   ```
3. Adopt RitsuLib APIs in batches:
   - Batch 1: Bootstrap, diagnostics, optional settings page
   - Batch 2: Future new content registration (not existing high-risk content)
   - Batch 3: Persistence sidecar experiments (not current 30 SavedSpireFields)
   - Batch 4: Low-risk patch wrappers
   - Batch 5: High-risk run/map/reward/save/multiplayer patches (only after manual
     evidence backlog is reduced)

## References

- [RitsuLib Getting Started](https://sts2-ritsulib.ritsukage.com/guide/getting-started)
- [RitsuLib GitHub](https://github.com/BAKAOLC/STS2-RitsuLib)
- [RitsuLib Framework Design](https://sts2-ritsulib.ritsukage.com/guide/framework-design)
