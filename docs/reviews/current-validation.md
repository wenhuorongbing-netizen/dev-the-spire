# Current Validation

Status: compact active validation summary.
Full archived record: `docs/archive/feature-audits/current-validation-full-20260622.md`.

## Current Target

- Date: 2026-06-22; latest addendum: source now has 69 RitsuLib `IPatchMethod` classes and 102 raw Harmony declarations after the Batch 4c ascension-localization fallback, visual-hover UI, A20 reward proceed, and Meat Cleaver rest-site UI migrations. beta.113 build, publish, package refresh, installed-package parity, runtime preflight, focused migration/Ancient/release metadata tests, and current-doc claims are current for this source pass; beta.108 clicked UI smoke is retained as previous-package evidence only.
- Current package target is Spire Plus `v0.1.0-private-beta.113` on Slay the Spire 2 `v0.107.1`.
- `EZMicroBalance.csproj` references `STS2.RitsuLib` `0.4.34`; `EZMicroBalance.json` declares only `STS2-RitsuLib >= 0.4.34` as the runtime dependency.
- The unpacked local game source under `source code/src/Core/` is the primary API authority for game behavior. RitsuLib docs/XML are the modding API authority.

## Latest Dependency Recheck

- NuGet flat-container `https://api.nuget.org/v3-flatcontainer/sts2.ritsulib/index.json` still reports `0.4.34` as the latest `STS2.RitsuLib` package across 165 listed versions.
- `dotnet list EZMicroBalance.csproj package --outdated --include-transitive` found no newer `STS2.RitsuLib`; it reported only transitive `System.IO.Hashing 9.0.0 -> 10.0.9`.
- Keep the stable `0.4.34` dependency target unless the owner explicitly approves a separate dev-runtime validation lane.

## Current Evidence

- beta.113 package refresh passed `dotnet build` with 0 warnings / 0 errors, `dotnet publish` with only the known Godot ignored `source code` project warning, `scripts/package-spire-plus.ps1`, installed-package parity, runtime preflight 28 / 0, and source-workspace validation 58 / 0 with retained GDRE warnings only.
- The 69-patch Meat Cleaver rest-site UI migration pass passed focused RitsuLib/Ancient/release metadata guards 65 / 0 / 22 / 87, opt-in release artifact/website guards 47 / 0 / 0 / 47, current-doc claims 1321 / 0 after the beta.113 guard update, installed-package parity, runtime preflight 28 / 0, source-workspace validation 58 / 0 with the retained 2 GDRE warnings, RitsuLib latest-package guard 9 / 0, repository hygiene, `dotnet format`, and `git diff --check`.
- Source-workspace validation passed 58 checks / 0 mismatches against installed `v0.107.1`, package `v0.1.0-private-beta.113`, and STS2-RitsuLib `0.4.34`, with retained GDRE warnings only. The checker also verifies local `STS2-RitsuLib.xml` API-doc markers for `RegisterModSettings`, `BeginModDataRegistration`, `ModDataStore.Register`, `CreateContentPack`, `CreatePatcher`, and `SavedAttachedState`.
- Previous beta.108 clicked Ancient UI smoke proof passed at `.tools/runtime-evidence/monkey-stability-beta108-20260622-172312/`: 4 / 4 `AncientUiSmoke` iterations for `URDA`, `MORVI`, `LOTHA`, and normal `VAKUU`, each with command ACK, screenshot, clean log audit, StS1 Off verifier pass, exact game/Ritsu/package markers, all 64 migrated Spire Plus ModPatcher patches applied, and packet verification 1621 / 0.

## Evidence Boundary

- This is previous-package smoke-level clicked Ancient UI migration proof; recapture it for beta.113 before tester handoff or release claims.
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
