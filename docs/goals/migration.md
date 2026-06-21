# RitsuLib Migration Goal

## Current Target

Date: 2026-06-21

Active branch target: GitHub `main`

Current package target: Spire Plus `v0.1.0-private-beta.99`

Installed game target: Slay the Spire 2 `v0.107.1`

Runtime dependency target: official `STS2-RitsuLib` `v0.4.32` installed from the NuGet package deploy target

Recapture `git log -1 --oneline --decorate` and `git status --short --branch` at the start of each continuation and immediately before handoff; older run-start hashes are historical notes, not current status.

Spire Plus is a RitsuLib-only mod. Future implementation work should start from:

- `PROJECT_STATE.md`
- `docs/integrations/ritsulib.md`
- `docs/reviews/current-validation.md`
- local game source under `source code/src/Core/`
- installed `STS2-RitsuLib` package docs/XML and the public RitsuLib docs

Do not add another shared runtime framework dependency unless the owner explicitly approves a new dependency decision and the package version, manifest, release docs, and validation guards are updated in the same pass.

## Success Criteria

- `EZMicroBalance.csproj` references `STS2.RitsuLib` `0.4.32`.
- `EZMicroBalance.json` declares only `STS2-RitsuLib >= 0.4.32` as the shared runtime framework dependency.
- Spire Plus settings, content/model registration, lifecycle hooks, and saved marker fields use RitsuLib or game-native APIs.
- Active setup docs and developer guides direct agents to RitsuLib documentation and local game source.
- Current validation includes build, publish/package refresh when artifacts change, installed package parity, focused migration/source guards, and current RitsuLib loader evidence.
- Loader evidence is not treated as gameplay, save-load, multiplayer, QA, release, or handoff proof.

## Current Conclusion

The code, manifest, package metadata, and current setup docs have moved to the RitsuLib-only target. The current manifest/package target is beta.99 after updating to STS2-RitsuLib `0.4.32` and refreshing package hashes. Beta.99 clicked settings UI proof is captured under `.tools/runtime-evidence/mod-settings-beta99-ritsulib-click-20260621-223210/` with clean log audit and StS1 Off runtime shape verification 21 / 0; beta.99 direct Off loader proof remains pending. Previous beta.96 Off proof under `.tools/runtime-evidence/v01071-beta96-ritsulib0431-off-direct-20260621-185056/` reached main menu with exactly `STS2-RitsuLib` and `EZMicroBalance` loaded, clean audit, StS1Events disabled with 0 registration lines, and Off packet verifier 43 / 0. Previous beta.93 AdditiveBatch1 proof under `.tools/runtime-evidence/v01071-beta93-ritsulib0431-additivebatch1-direct-20260621/` reached main menu, registered 10 event types through 14 calls, audited clean, passed enabled-mode verifier 31 / 0, and passed packet verifier 61 / 0 for the previous package only.

The migration is not release-ready. Gameplay, event screenshots, save-load, image/render, replacement functional proof, co-op/fail-closed proof, independent QA, and tester-package handoff remain pending.

## Dependency Recheck

- NuGet flat-container and `dotnet list package --outdated` show `STS2.RitsuLib` `0.4.32` as the latest package.
- Nexus files list the variant-pack main file as `0.4.32`; direct automated Nexus download was blocked by the site challenge in this session.
- The GitHub release page/API can lag the NuGet/Nexus package version; do not use a lagging GitHub release marker as the dependency-floor source when NuGet and Nexus both expose a newer stable package.
- The current local runtime is deployed from the official NuGet package via `RitsuLibDeployDir`, producing `mods/STS2-RitsuLib/mod_manifest.json`, root `STS2-RitsuLib.dll`, XML docs, and viewer files.
- Keep Spire Plus on stable `0.4.32`, not a dev build, unless the owner explicitly approves a separate dev-runtime validation lane.

## Batch 4c Boundary

| Item | Status | Evidence |
| --- | --- | --- |
| Batch 4c migration | Proposal only / static review recaptured | 2026-06-18 recapture confirmed 10 low-risk candidates, no forbidden high-risk categories, and no migration performed. Owner approval is still required before any migration work. |

Record an owner decision for Batch 4c. The candidate list has static-review coverage; do not migrate unless the owner approves the scope.

## Validation Snapshot

Use these checks after migration-related edits:

```text
git log -1 --oneline --decorate
git status --short --branch
dotnet list EZMicroBalance.csproj package --include-transitive
dotnet list EZMicroBalance.csproj package --outdated --include-transitive
$blocked = -join ([char[]](66,97,115,101,76,105,98)); git grep -n -i $blocked -- ':!docs/archive/**' ':!source code/**' ':!bin/**' ':!obj/**' ':!.tools/**' ':!publish/**'
scripts/check-local-godot-source-workspace.ps1 -SourceRoot 'source code' -GameRoot 'E:\Steam\steamapps\common\Slay the Spire 2' -ExpectedGameVersion 'v0.107.1' -ExpectedPackageVersion 'v0.1.0-private-beta.99' -ExpectedRitsuLibVersion '0.4.32' -ExpectedRitsuCompatBranch '0.107.1' -RequireCurrentSourceSnapshot -FailOnMismatch
dotnet build EZMicroBalance.sln -m:1 --no-incremental -p:UseSharedCompilation=false
scripts/check-sts1-event-current-doc-claims.ps1 -FailOnMismatch
dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
git diff --check
```

When package artifacts change, also run:

```text
dotnet publish EZMicroBalance.sln -m:1
scripts/package-spire-plus.ps1 -GameRoot 'E:\Steam\steamapps\common\Slay the Spire 2'
scripts/check-installed-spire-plus-package.ps1 -ModDirectory 'E:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance'
scripts/check-sts1-runtime-preflight.ps1 -FailOnMismatch
```

## Next Actions

1. Recapture beta.99 direct Off loader proof before making a current startup/runtime claim; current clicked RitsuLib Mod Settings proof already lives under `.tools/runtime-evidence/mod-settings-beta99-ritsulib-click-20260621-223210/`.
2. Capture current AdditiveBatch1 enabled-mode and gameplay proof; previous beta.93 evidence proves loader/registration only for the older package.
3. Capture save-load, image/render, replacement, and multiplayer fail-closed proof.
4. Record an owner decision before any Batch 4c or higher-risk patch migration.
5. Recapture git status, pushed HEAD, and validation status before any later handoff.
