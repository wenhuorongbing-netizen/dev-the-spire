# Current Validation

Status: compact active validation summary.
Full archived record: `docs/archive/feature-audits/current-validation-full-20260622.md`.

## Current Target

- Date: 2026-06-22; latest addendum 2026-06-23: source now has 165 RitsuLib `IPatchMethod` classes and 4 raw Harmony declarations after the Batch 4c ascension-localization fallback, visual-hover UI, A20 reward proceed, Meat Cleaver rest-site UI, Preview transform prediction source/lifetime, Ascension selection/lobby UI, Neow/Vakuu event-option UI, A20 courtyard portrait, inline-localization fallback, Ancient reward getter/relic hook, low-risk reward hook, Aeonglass intent UI, Enemy Damage polish getter, RitsuLib settings-button compatibility, Urda Root Sight room-routing, Ascension map generation, multiplayer run-state/save-quit diagnostics, and debug-only StS1 replacement-prototype migrations. beta.133 packages this 165/4 source state. Previous beta.128 runtime proof registers 152 patches because the StS1 replacement prototype is compile-symbol gated behind `REPLACEMENT_PROTOTYPE_ENABLED`; recapture beta.133 runtime proof before claiming current in-game coverage.
- Current package target is Spire Plus `v0.1.0-private-beta.133` on Slay the Spire 2 `v0.107.1`.
- `EZMicroBalance.csproj` references `STS2.RitsuLib` `0.4.34`; `EZMicroBalance.json` declares only `STS2-RitsuLib >= 0.4.34` as the runtime dependency.
- The unpacked local game source under `source code/src/Core/` is the primary API authority for game behavior. RitsuLib docs/XML are the modding API authority.

## Latest Dependency Recheck

- NuGet flat-container `https://api.nuget.org/v3-flatcontainer/sts2.ritsulib/index.json` still reports `0.4.34` as the latest `STS2.RitsuLib` package across 165 listed versions.
- GitHub release API reports `v0.4.34` / `0.4.34` published on 2026-06-22, and raw `main` `mod_manifest.json` reports `0.4.34`; NuGet plus installed XML/runtime remain the primary dependency-floor evidence.
- `dotnet list EZMicroBalance.csproj package --outdated --include-transitive` found no newer `STS2.RitsuLib`; it reported only transitive `System.IO.Hashing 9.0.0 -> 10.0.9`.
- Keep the stable `0.4.34` dependency target unless the owner explicitly approves a separate dev-runtime validation lane.

## Current Evidence

- beta.133 package refresh passed `dotnet build` with 0 warnings / 0 errors, `dotnet publish` with only the known Godot ignored `source code` project warning, and `scripts/package-spire-plus.ps1`.
- Installed-package parity passed for `publish/SpirePlus-v0.1.0-private-beta.133.zip`, the game-root zip, installed DLL, manifest, PCK, README, and Sere Talon/Tanx Claws package content. beta.133 artifact hashes are recorded in `docs/private-beta-verification-handoff.md`.
- 2026-06-23 beta.133 migration validation passed `dotnet build EZMicroBalance.sln -m:1 --no-incremental -p:UseSharedCompilation=false` with 0 warnings / 0 errors.
- Focused RitsuLib migration, documentation, release, Ancient behavior, website, and Urda release guards passed 121 / 0 / 7 / 128. Governance/source-drift guards passed 48 / 0 / 1 / 49, Ancient/Urda/Vakuu guards passed 110 / 0 / 7 / 117, and Ascension/co-op/save-state guards passed 39 / 0 / 5 / 44.
- The broad `FullyQualifiedName!~ReleaseEvidenceGateTests` no-build lane timed out twice and left a testhost process that was cleaned up; do not count that runner-hang attempt as a pass. Keep using the smaller split lanes until the runner contamination is fixed.
- Current-doc claims passed 1327 / 0. Repository hygiene, `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`, full tracked-text retired-dependency scan, active old-GitHub-lag wording scan, and `git diff --check` passed. `git diff --check` reported only CRLF normalization warnings.
- Runtime preflight passed 28 checks / 0 mismatches for beta.133. RitsuLib latest-package guard passed 9 checks / 0 mismatches, keeping `STS2.RitsuLib` at `0.4.34`.
- Source-workspace validation passed 57 checks / 0 mismatches against installed `v0.107.1`, package `v0.1.0-private-beta.133`, and STS2-RitsuLib `0.4.34`, with two retained GDRE warnings only. The checker also verifies local `STS2-RitsuLib.xml` API-doc markers for `RegisterModSettings`, `BeginModDataRegistration`, `ModDataStore.Register`, `CreateContentPack`, `CreatePatcher`, and `SavedAttachedState`.
- Repository hygiene, active retired-dependency/old-package/count/hash residue scans, and raw Harmony source counting passed. The active tracked-text scan found no retired framework/API names outside ignored/local output, and `EZMicroBalanceCode` contains 15 remaining raw `[HarmonyPatch]` declarations.
- Previous beta.128 clicked Ancient UI smoke proof passed at `.tools/runtime-evidence/monkey-stability-20260623-062913/`: 4 / 4 `AncientUiSmoke` iterations for `URDA`, `MORVI`, `LOTHA`, and normal `VAKUU`, each with command ACK, screenshot, clean log audit, StS1 Off verifier pass, exact game/Ritsu/package markers, 152/152 default runtime Spire Plus ModPatcher patches applied, and packet verification 1621 / 0.

## Evidence Boundary

- beta.133 is validated through build, publish, package creation, installed-package parity, runtime preflight, and source-workspace checks.
- The previous beta.128 clicked Ancient UI proof covers forced Urda, Morvi, Lotha, and normal Vakuu UI only for that package; it does not prove beta.133 gameplay, save-load, gated Vakuu fight-option/victory return, co-op, or release readiness.
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
