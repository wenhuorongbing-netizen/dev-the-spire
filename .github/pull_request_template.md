## Scope

- Bounded context:
- Player-visible behavior changed:
- Manifest/resource/package impact:

## Required Checks

- [ ] `EZMicroBalance` manifest id remains unchanged.
- [ ] Active deliverable remains one `Spire Plus / EZMicroBalance` mod; duplicate root mod surfaces are not restored.
- [ ] No official Slay the Spire 2 assets or large decompiled code were copied.
- [ ] New or moved Harmony patches are reflected in `docs/patch-inventory.md`.
- [ ] English and Simplified Chinese localization keys/placeholders stay aligned when text changed.
- [ ] Save/load, co-op, and UI risks are recorded when touched.
- [ ] Manual evidence rows stay open unless live proof exists.

## Validation

```powershell
dotnet build EZMicroBalance.sln
dotnet test EZMicroBalance.sln --no-build
dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
git diff --check
```

For resource, localization, manifest, export, or package changes:

```powershell
dotnet publish EZMicroBalance.sln
.\scripts\package-spire-plus.ps1
$env:EZMB_RUN_RELEASE_ARTIFACT_TESTS='1'
dotnet test EZMicroBalance.sln --no-build
Remove-Item Env:\EZMB_RUN_RELEASE_ARTIFACT_TESTS
```

## Manual Evidence

- Required:
- Evidence directory:
- Deferred with owner approval:
