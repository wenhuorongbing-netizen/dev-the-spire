# Current Validation

Status: compact active validation summary.
Full archived record: `docs/archive/feature-audits/current-validation-full-20260622.md`.

## Current Target

- Date: 2026-06-22; latest addendum 2026-06-23: source now has 153 RitsuLib `IPatchMethod` classes and 17 raw Harmony declarations after the Batch 4c ascension-localization fallback, visual-hover UI, A20 reward proceed, Meat Cleaver rest-site UI, Preview transform prediction source/lifetime, Ascension selection/lobby UI, Neow/Vakuu event-option UI, A20 courtyard portrait, inline-localization fallback, Ancient reward getter/relic hook, low-risk reward hook, Aeonglass intent UI, Enemy Damage polish getter, RitsuLib settings-button compatibility, Ascension map generation, and debug-only StS1 replacement-prototype migrations. beta.128 packages this 153/17 source state and has no-launch validation; beta.128 game-launch/runtime patch-count proof is still pending.
- Current package target is Spire Plus `v0.1.0-private-beta.128` on Slay the Spire 2 `v0.107.1`.
- `EZMicroBalance.csproj` references `STS2.RitsuLib` `0.4.34`; `EZMicroBalance.json` declares only `STS2-RitsuLib >= 0.4.34` as the runtime dependency.
- The unpacked local game source under `source code/src/Core/` is the primary API authority for game behavior. RitsuLib docs/XML are the modding API authority.

## Latest Dependency Recheck

- NuGet flat-container `https://api.nuget.org/v3-flatcontainer/sts2.ritsulib/index.json` still reports `0.4.34` as the latest `STS2.RitsuLib` package across 165 listed versions.
- GitHub release API reports `v0.4.34` / `0.4.34` published on 2026-06-22, and raw `main` `mod_manifest.json` reports `0.4.34`; NuGet plus installed XML/runtime remain the primary dependency-floor evidence.
- `dotnet list EZMicroBalance.csproj package --outdated --include-transitive` found no newer `STS2.RitsuLib`; it reported only transitive `System.IO.Hashing 9.0.0 -> 10.0.9`.
- Keep the stable `0.4.34` dependency target unless the owner explicitly approves a separate dev-runtime validation lane.

## Current Evidence

- beta.128 package refresh passed `dotnet build` with 0 warnings / 0 errors, `dotnet publish` with only the known Godot ignored `source code` project warning, and `scripts/package-spire-plus.ps1`.
- Installed-package parity passed for `publish/SpirePlus-v0.1.0-private-beta.128.zip`, the game-root zip, installed DLL, manifest, PCK, README, and Sere Talon/Tanx Claws package content. beta.128 artifact hashes are recorded in `docs/private-beta-verification-handoff.md`.
- 2026-06-23 beta.128 migration/package validation passed `dotnet build EZMicroBalance.sln -m:1 --no-incremental -p:UseSharedCompilation=false` with 0 warnings / 0 errors. Split test-project validation passed `ReleaseEvidenceGateTests` 9 / 0 / 0 / 9 and the complementary `FullyQualifiedName!~ReleaseEvidenceGateTests` lane 654 / 0 / 40 / 694 after the StS1 replacement prototype migration.
- Focused RitsuLib migration, StS1 feature, documentation, release coverage, and release-safety guards passed 131 / 0 / 0 / 131. The opt-in release artifact lane passed 50 / 0 / 0 / 50. Current-doc claims passed 1326 / 0.
- 2026-06-23 current-doc claims were rerun after the validation summary update; repository hygiene, `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`, full tracked-text retired-dependency scan, active old-GitHub-lag wording scan, and `git diff --check` passed. `git diff --check` reported only CRLF normalization warnings.
- Runtime preflight passed 28 checks / 0 mismatches. RitsuLib latest-package guard passed 9 checks / 0 mismatches, keeping `STS2.RitsuLib` at `0.4.34`.
- Source-workspace validation passed 58 checks / 0 mismatches against installed `v0.107.1`, package `v0.1.0-private-beta.128`, and STS2-RitsuLib `0.4.34`, with two retained GDRE warnings only. The checker also verifies local `STS2-RitsuLib.xml` API-doc markers for `RegisterModSettings`, `BeginModDataRegistration`, `ModDataStore.Register`, `CreateContentPack`, `CreatePatcher`, and `SavedAttachedState`.
- Repository hygiene, `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`, active retired-dependency/old-package/count/hash residue scans, and `git diff --check` passed. `git diff --check` reported only retained CRLF normalization warnings for `docs/features/ancients-rework-v4/manual-verification-matrix.md`, `docs/patch-inventory.md`, and `docs/release-evidence-status.md`.
- Current beta.123 clicked Ancient UI smoke proof passed at `.tools/runtime-evidence/monkey-stability-20260622-235746/`: 4 / 4 `AncientUiSmoke` iterations for `URDA`, `MORVI`, `LOTHA`, and normal `VAKUU`, each with command ACK, screenshot, clean log audit, StS1 Off verifier pass, exact game/Ritsu/package markers, all 127 migrated Spire Plus ModPatcher patches applied, and packet verification 1621 / 0.

## Evidence Boundary

- beta.128 is validated through build, publish, package creation, installed-package parity, runtime preflight, and source-workspace checks only. It has not been game-launched or clicked-UI-smoked.
- The retained beta.123 clicked Ancient UI proof covers the earlier packaged 127-patch state only; it does not prove beta.128 runtime patch registration.
- It does not prove event encounter gameplay, gated Vakuu fight-option/victory return, live gameplay, save-load, image rendering, replacement functional behavior, multiplayer fail-closed behavior, independent QA, release handoff, live-ready, or private-beta release readiness.
- Previous beta.99 settings/Off proof, beta.96 Off proof, beta.93 AdditiveBatch1 proof, and beta.85-beta.90 rows are retained previous-package or previous-game-version context only.

## Guard And Runtime Rows

- The current O0-O76 gate map is `docs/features/sts1-events/v19-gate-evidence-map.md`, and the per-gate ledger is `docs/features/sts1-events/v19-gate-ledger.csv` guarded by `scripts/check-sts1-v19-gate-ledger.ps1`.
- The O76-O84 final documentation/handoff overlay is `docs/features/sts1-events/v20-final-gate-overlay.csv` guarded by `scripts/check-sts1-v20-final-gate-overlay.ps1`; it is static-only and does not close runtime or handoff gates.
- The current StS1 static suite expected summary is 15 static steps, 0 suite failures, with static-file hygiene, current-doc claims, v19 ledger, v20 overlay, and subagent coverage as no-launch guard support only.
- Runtime monkey and AutoSlay proof-mode packets must use exact current package/game/Ritsu target switches and require `-ExpectedAncientIds`; retained `autoslay-plan.json` `ExpectedAncientIds`, retained `autoslay-summary.json`, and sidecar-plus-current-log event traversal must bind to the same Ancient id before future gameplay proof can count.
- `docs/features/sts1-events/localization-gap-closure-plan.md` remains the StS1 localization gap closure plan.
- Do not start overlapping validation lanes when a long `dotnet`, package, or game-runtime validation lane is already active.

## Current Open Proof

Gameplay, gated Vakuu fight-option UI, Vakuu victory return/no-black-screen, save-load, replacement behavior, co-op/fail-closed proof, current enabled-mode gameplay, independent QA, and final tester handoff remain open.
