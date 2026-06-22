# RitsuLib Migration Goal

## Current Target

Date: 2026-06-22

Active branch target: GitHub `main`

Current package target: Spire Plus `v0.1.0-private-beta.107`

Installed game target: Slay the Spire 2 `v0.107.1`

Runtime dependency target: official `STS2-RitsuLib` `v0.4.33` installed from the NuGet package deploy target

Recapture `git log -1 --oneline --decorate` and `git status --short --branch` at the start of each continuation and immediately before handoff; older run-start hashes are historical notes, not current status.

Spire Plus is a RitsuLib-only mod. Future implementation work should start from:

- `PROJECT_STATE.md`
- `docs/integrations/ritsulib.md`
- `docs/reviews/current-validation.md`
- unpacked local game source under `source code/src/Core/`
- installed `STS2-RitsuLib` package docs/XML and the public RitsuLib docs

Do not add any runtime dependency besides STS2-RitsuLib unless the owner explicitly approves a new dependency decision and the package version, manifest, release docs, and validation guards are updated in the same pass.

## Success Criteria

- `EZMicroBalance.csproj` references `STS2.RitsuLib` `0.4.33`.
- `EZMicroBalance.json` declares only `STS2-RitsuLib >= 0.4.33` as the runtime dependency.
- Spire Plus settings, content/model registration, lifecycle hooks, and saved marker fields use RitsuLib or game-native APIs.
- Active setup docs and developer guides direct agents to RitsuLib documentation and local game source.
- Git-tracked text surfaces stay free of retired shared-runtime names; `EngineeringGovernanceGuardTests.RetiredSharedRuntimeNameDoesNotReappearInTrackedText` is the automated guard for this.
- Current validation includes build, publish/package refresh when artifacts change, installed package parity, focused migration/source guards, and current RitsuLib loader evidence.
- Loader evidence is not treated as gameplay, save-load, multiplayer, QA, release, or handoff proof.

## Current Conclusion

The code, manifest, package metadata, and current setup docs have moved to the RitsuLib-only target. Current source has completed the clicked/input UI migration to RitsuLib `IPatchMethod` / `ModPatcher`, with 46 migrated patch classes and 125 raw Harmony declarations remaining. The current manifest/package target is beta.107 after updating to STS2-RitsuLib `0.4.33`, refreshing package hashes, adding RitsuLib default public-entry localization aliases for Ancient event, option relic, and power text, and fixing the Urda option-relic prefix so RitsuLib discovers that patch. Beta.107 clicked Ancient UI smoke proof is captured under `.tools/runtime-evidence/monkey-stability-beta107-rerun-20260622-144051/`: 4 / 4 `AncientUiSmoke` iterations passed for `URDA`, `MORVI`, `LOTHA`, and normal `VAKUU`, with command ACKs, screenshots, clean log audits, StS1 Off verifier pass, exact game/Ritsu/package markers, all 46 Spire Plus ModPatcher patches applied, and packet verification 1620 / 0. Previous beta.99 settings/off proof, beta.96 Off proof, and beta.93 AdditiveBatch1 proof remain previous-package context only.

Current developer entry points and tracked text files are guarded so future work starts from RitsuLib docs, installed RitsuLib XML/API evidence, and unpacked local game source instead of retired runtime-framework assumptions.

The migration is not release-ready. Smoke-level clicked Ancient UI is covered, but gameplay, gated Vakuu fight-option/victory return, save-load, image/render, replacement functional proof, co-op/fail-closed proof, independent QA, and tester-package handoff remain pending.

## Dependency Recheck

- 2026-06-22: NuGet flat-container and `dotnet list package --outdated --include-transitive` show `STS2.RitsuLib` `0.4.33` as the latest package. The flat-container index lists 164 versions and ends at `0.4.33`; `dotnet list` found no `STS2.RitsuLib` update and reported only transitive `System.IO.Hashing 9.0.0 -> 10.0.9`.
- Nexus files list the variant-pack main file as `0.4.33`; direct automated Nexus download was blocked by the site challenge in this session.
- The GitHub release page/API can lag the NuGet/Nexus package version; do not use a lagging GitHub release marker as the dependency-floor source when NuGet and Nexus both expose a newer stable package.
- The current local runtime is deployed from the official NuGet package via `RitsuLibDeployDir`, producing `mods/STS2-RitsuLib/mod_manifest.json`, root `STS2-RitsuLib.dll`, XML docs, and viewer files.
- Keep Spire Plus on stable `0.4.33`, not a dev build, unless the owner explicitly approves a separate dev-runtime validation lane.

## Batch 4c Boundary

| Item | Status | Evidence |
| --- | --- | --- |
| Batch 4c migration | Partial targeted UI/input migration / remaining candidates proposal-only | 2026-06-22 owner goal drove migration of the UI/input subset through RitsuLib; 6 localization candidates remain proposal-only with no forbidden high-risk categories. |

Record a fresh owner decision before migrating any of the remaining Batch 4c candidates.

## Validation Snapshot

Use these checks after migration-related edits:

```text
git log -1 --oneline --decorate
git status --short --branch
dotnet list EZMicroBalance.csproj package --include-transitive
dotnet list EZMicroBalance.csproj package --outdated --include-transitive
$blocked = -join ([char[]](66,97,115,101,76,105,98)); git grep -n -i $blocked -- ':!docs/archive/**' ':!source code/**' ':!bin/**' ':!obj/**' ':!.tools/**' ':!publish/**'
scripts/check-local-godot-source-workspace.ps1 -SourceRoot 'source code' -GameRoot 'E:\Steam\steamapps\common\Slay the Spire 2' -ExpectedGameVersion 'v0.107.1' -ExpectedPackageVersion 'v0.1.0-private-beta.107' -ExpectedRitsuLibVersion '0.4.33' -ExpectedRitsuCompatBranch '0.107.1' -RequireCurrentSourceSnapshot -FailOnMismatch
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

1. Capture current AdditiveBatch1 enabled-mode and gameplay proof; previous beta.93 evidence proves loader/registration only for the older package.
2. Keep beta.107 clicked Ancient UI smoke scoped to forced UI visibility; it does not prove gameplay or release readiness.
3. Capture save-load, image/render, replacement, and multiplayer fail-closed proof.
4. Record an owner decision before any remaining Batch 4c or higher-risk patch migration.
5. Recapture git status, pushed HEAD, and validation status before any later handoff.
