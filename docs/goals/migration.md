# RitsuLib Migration Goal

## Current Target

Date: 2026-06-19

Active branch target: GitHub `main`

Current package target: Spire Plus `v0.1.0-private-beta.88`

Installed game target: Slay the Spire 2 `v0.107.1`

Runtime dependency target: official `STS2-RitsuLib` `v0.4.24` with `lib/0.107.0`

Use these files as the current source of truth before acting on this goal:

- `PROJECT_STATE.md`
- `docs/reviews/current-validation.md`
- `docs/features/sts1-events/status-board.md`
- `docs/features/ritsulib-migration/next-overnight-run.md`
- `docs/test-ready-development-goal.md`

The previous long-form contents of this file were mojibake-heavy prompt notes and are archived at
`docs/archive/legacy-planning/migration-goal-mojibake-20260618.md`. Keep this active file compact,
current, and action-oriented.

## Current Conclusion

Current runtime dependency drift is resolved for `v0.107.1` loader and AdditiveBatch1
registration proof after the BaseLib `v3.3.0` update. The first current-version
direct AdditiveBatch1 recapture reached main menu and matched the expected StS1
registration shape, but failed clean-loader proof because BaseLib `v3.2.1` logged 2 patch
failures on Slay the Spire 2 `v0.107.1`; the beta.88 recapture with BaseLib `v3.3.0`
passed clean audit and packet verification.
The migration is not release-ready. Gameplay, Mod Settings UI page refresh, event screenshots,
save-load, image/render, replacement functional proof, co-op/fail-closed proof, independent QA,
and tester-package handoff remain pending. Worktree and pushed-HEAD status must be recaptured
before any later handoff.

## Status

| Area | Current state | Evidence / notes |
| --- | --- | --- |
| RitsuLib install | Pass | `STS2-RitsuLib` `v0.4.24` is installed with `lib/0.107.0`. |
| Root cause history | Resolved for loader | The beta.84 Off failure was Spire Plus API drift, including `EctoplasmGoldGatePatch` and getter-target drift, not missing BaseLib/RitsuLib. |
| beta.85 Off loader proof | Historical pass | `v0.107.0` beta.85 package runtime proof reached main menu with 25/25 Spire Plus patches and clean audit. Treat it as previous-package/game-version loader context. |
| beta.85 CanaryOnly proof | Historical pass | Previous-package loader proof only: 4 event types / 6 registration calls. |
| beta.85 AdditiveBatch1 proof | Historical fail | Previous-package mismatch: 13/14 registration calls because the installed package/source shape was stale. |
| beta.87 build/publish/package | Pass | `dotnet build`, `dotnet publish`, package creation, and installed package parity passed for `v0.1.0-private-beta.87`. |
| beta.87 AdditiveBatch1 proof | Retained loader/registration pass | `.tools/runtime-evidence/v01070-beta87-additive-batch1-direct-20260618-152531/` reached main menu on `v0.107.0` with BaseLib, RitsuLib, and Spire Plus loaded; 25/25 patches; 30 SavedSpireFields; 10 event types / 14 registration calls; clean audit; retained log verifier 31/0 and packet verifier 52/0. Recapture before treating it as current `v0.107.1` runtime evidence. |
| v0.107.1 AdditiveBatch1 recapture | Failed clean-loader gate | `.tools/runtime-evidence/v01071-beta87-additive-batch1-direct-20260619-102309/` reached main menu in 33.81s, loaded Spire Plus `v0.1.0-private-beta.87`, selected RitsuLib `0.4.24` compat branch `0.107.0`, applied 25/25 Spire Plus patches, and matched AdditiveBatch1 10 event types / 14 calls with exact act/shared tuple parity. It is not passing proof: BaseLib `v3.2.1` logged `Applied 241 patches successfully, 2 failed`, audit found 3 BaseLib patch-failure hits and 2 Godot ERROR lines, enabled-mode verifier mismatches=2, packet verifier mismatches=1. |
| beta.88 BaseLib `v3.3.0` build/publish/package | Pass | `dotnet build EZMicroBalance.sln -m:1 --no-incremental -p:UseSharedCompilation=false`, `dotnet publish EZMicroBalance.sln -m:1`, package creation, and installed package parity passed for `v0.1.0-private-beta.88`; the package requires BaseLib `v3.3.0`. |
| beta.88 AdditiveBatch1 proof | Current loader/registration pass | `.tools/runtime-evidence/v01071-beta88-baselib330-additive-batch1-direct-cleanlog-20260619-103937/` reached main menu on `v0.107.1` with BaseLib `v3.3.0`, RitsuLib `0.4.24`, and Spire Plus loaded; 25/25 patches; 10 event types / 14 registration calls; clean audit; retained log verifier 31/0; packet verifier 0 mismatches. |
| Tests | Current no-game pass | Recapture `git log -1 --oneline --decorate` and `git status --short --branch` at the start of each continuation and immediately before handoff; older run-start hashes are historical notes, not current status. The beta.88 follow-up passed build 0 warnings / 0 errors; split no-build runtime-harness coverage 81 / 0 / 0 / 81; Ancient behavior/UI guards 35 / 0 / 2 / 37; Ascension guards 16 / 0 / 3 / 19; boss guards 9 / 0 / 1 / 10; installed package parity; retained beta.88 AdditiveBatch1 packet verification 62 / 0; and static/doc checks listed below. |
| Opt-in artifact subset | Current pass | Release/package/source opt-in checks passed 31 / 0 / 1 / 32 with `SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS=1` and the local `STS2_PATH`. |
| StS1 static/governance lanes | Current pass | Current-doc claims 1164/0 after beta.88 doc alignment, AutoSlay proof-mode `-ExpectedAncientIds` target coverage, exact `AncientIdCounts` summary aggregation guards including extra zero-count key rejection, non-positive `MinRuns` rejection, ordered-selection AncientId guarding, proof-command exact target/report switch enforcement, exact `VAKUU,URDA,MORVI,LOTHA` target guarding, `-AllowMissingEventTraversal` bypass rejection, runtime-monkey escaped-path and malformed-path hardening, duplicate/missing iteration-number coverage hardening, runtime-monkey plan `ExpectedPatchCount` binding hardening, runtime-monkey runner path/hash binding hardening, runtime-monkey command-corpus file/hash binding hardening, analyzer noncanonical-path checks, probe process identity checks, malformed AutoSlay retained-path rejection, direct-smoke analyzer hardening, and live-session restore-state hardening; runtime preflight 27/0 for local `v0.107.1`; retained AdditiveBatch1 packet verifier 62/0; static suite 15/0; static-file hygiene 12/0. |
| Batch 4a/4b migration | Source-level complete | Current patch inventory records 25 migrated `IPatchMethod` patch classes and 142 remaining raw `HarmonyPatch` declarations. |
| Batch 4c migration | Proposal only / static review recaptured | 2026-06-18 recapture confirmed 10 low-risk candidates, no forbidden high-risk categories, and no migration performed. Owner approval is still required before any migration work. |
| Mod Settings UI scaffold | Prepared / live pending | No-launch scaffold refreshed at `.tools/runtime-evidence/mod-settings-current-display-20260618-223145/` with `GitHead` `1cb58dbcbfcdb08defe50a4687023aa59d4a229d`, clean `GitStatusShort`, package `v0.1.0-private-beta.87`, `Capture=None`, and `NoLaunch=true`; the preflight recorded Slay the Spire 2 not running. This is not screenshot, log/audit, or gameplay proof. |
| Manual-test handoff scaffold | Prepared / live pending | No-launch handoff scaffold refreshed at `.tools/runtime-evidence/manual-test-handoff-20260619-120202/` on pushed HEAD `2400ec4b`; generated `handoff-summary.json` recorded 21 required live rows, 21 expected pending failures, 0 warnings, package ZIP `D547847874919EE923E2281A495D5389BAB22BBDB9F1090DC57B77033668A36D`, and git handoff metadata with `GitHeadMatchesUpstream=true`. This is a template scaffold only; no game was launched. |
| Manual proof | Pending | Gameplay, clicked UI, save-load, image rendering, replacement behavior, co-op/fail-closed behavior, independent QA, and tester handoff are still open. |

Current beta.85/beta.86 loader proof remains previous-package/game-version context, retained beta.87 AdditiveBatch1 loader/registration proof exists under `.tools/runtime-evidence/v01070-beta87-additive-batch1-direct-20260618-152531/`, and current beta.88 `v0.107.1` clean-loader proof exists under `.tools/runtime-evidence/v01071-beta88-baselib330-additive-batch1-direct-cleanlog-20260619-103937/`. The BaseLib `v3.2.1` patch failures in `.tools/runtime-evidence/v01071-beta87-additive-batch1-direct-20260619-102309/` are root-cause history.

## Boundaries

- Do not claim private beta, live gameplay, or release readiness from loader/registration proof.
- Do not use `AllDraft` or `Replacement` as a tester/release path without owner approval and fresh targeted validation.
- Do not perform Batch 4c migration without explicit owner approval.
- Do not implement Ascension 21-30 or custom character work.
- Do not bump compile package, manifest minimums, or dependency minimums again unless an owner-approved package pass requires it.
- Prefer BaseLib/template-supported APIs and keep Harmony patches narrow.
- Treat the previous `v0.106.1` Off/CanaryOnly/AdditiveBatch1 smokes as historical loader evidence only.

## Next Actions

1. Keep the current BaseLib `v3.3.0` / beta.88 package target aligned before any later loader or gameplay proof; treat the BaseLib `v3.2.1` failure as resolved root-cause history.
2. Refresh Mod Settings UI proof for the current Spire Plus display-name package; the current no-launch scaffold is prepared at `.tools/runtime-evidence/mod-settings-current-display-20260618-223145/`, and it must be rerun with `-Capture List` / `-Capture Page` only after manually opening the relevant UI.
3. Use `.tools/runtime-evidence/manual-test-handoff-20260619-120202/TESTER_START_HERE.md` as the current no-launch manual-test scaffold, then fill its rows with live files before any pass claim.
4. Capture Canary gameplay proof for Big Fish, Golden Idol, The Lab, and Divine Fountain only after recapturing any required current-version CanaryOnly loader packet or explicitly accepting the retained previous-package context for that narrow purpose.
5. Capture AdditiveBatch1 gameplay proof only while the current beta.88 loader/registration evidence remains clean and package-matched.
6. Capture save-load and image/render proof for event and replacement surfaces.
7. Verify multiplayer fail-closed behavior and any owner-approved two-client diagnostics.
8. Record an owner decision for Batch 4c. The candidate list has static-review coverage; do not migrate unless the owner approves the scope.
9. Recapture git status, pushed HEAD, and validation status before any later handoff.

## Validation Snapshot

Current validated commands for the beta.87 migration pass and follow-up no-game recapture:

```text
dotnet build EZMicroBalance.sln -m:1 --no-incremental
dotnet publish EZMicroBalance.sln -m:1
scripts/package-spire-plus.ps1 -GameRoot 'E:\Steam\steamapps\common\Slay the Spire 2'
scripts/check-installed-spire-plus-package.ps1 -ModDirectory 'E:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance'
scripts/check-sts1-event-current-doc-claims.ps1 -FailOnMismatch
scripts/check-sts1-v19-gate-ledger.ps1 -FailOnMismatch
scripts/check-sts1-runtime-preflight.ps1 -FailOnMismatch
scripts/check-sts1-event-static-suite.ps1
dotnet test tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build --filter "FullyQualifiedName~RuntimeFailureAnalyzer|FullyQualifiedName~RuntimeMonkeyPacketChecker|FullyQualifiedName~GameNativeAutoSlayPacketVerifier|FullyQualifiedName~RuntimeMonkeyDocs" --logger "console;verbosity=minimal" -- RunConfiguration.MaxCpuCount=1
dotnet test tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build --filter "FullyQualifiedName~LothaLocalizationHoverAndRichTextAreReadable" --logger "console;verbosity=minimal" -- RunConfiguration.MaxCpuCount=1
dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
git diff --check
scripts/generate-patch-inventory.ps1 -Check
scripts/report-worktree-batches.ps1 -FailOnUnclassified
```

Additional June 19 `v0.107.1` no-launch drift and handoff-scaffold follow-up:

```text
scripts/prepare-current-manual-test-handoff.ps1
scripts/check-installed-spire-plus-package.ps1 -ModDirectory 'E:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance'
scripts/check-sts1-runtime-preflight.ps1 -FailOnMismatch
dotnet build EZMicroBalance.sln -m:1 --no-incremental -p:UseSharedCompilation=false
dotnet test tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build --filter "FullyQualifiedName~MultiplayerVersionMismatchDiagnosticsExposeModelHashHandshakeWithoutBypass" --logger "console;verbosity=minimal" -- RunConfiguration.MaxCpuCount=1
dotnet test tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build --filter "FullyQualifiedName~Sts1RuntimeEvidencePacketVerifier" --logger "console;verbosity=minimal" -- RunConfiguration.MaxCpuCount=1
dotnet test tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build --filter "FullyQualifiedName~SimplifiedChineseLocalizationContainsNo" --logger "console;verbosity=minimal" -- RunConfiguration.MaxCpuCount=1
scripts/check-sts1-event-current-doc-claims.ps1 -FailOnMismatch
scripts/check-sts1-event-static-suite.ps1
scripts/check-sts1-static-file-hygiene.ps1 -FailOnMismatch
scripts/check-sts1-v19-gate-ledger.ps1 -FailOnMismatch
scripts/check-sts1-v20-final-gate-overlay.ps1 -FailOnMismatch
scripts/check-sts1-v19-subagent-coverage.ps1 -FailOnMismatch
```

Current-version failed loader recapture attempt, retained as root-cause history:

```text
scripts/check-installed-spire-plus-package.ps1 -ModDirectory 'E:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance'
scripts/check-sts1-runtime-preflight.ps1 -FailOnMismatch
direct AdditiveBatch1 launch with temporary steam_appid.txt and SPIREPLUS_STS1_EVENT_MODE=AdditiveBatch1 -> .tools/runtime-evidence/v01071-beta87-additive-batch1-direct-20260619-102309/
scripts/audit-godot-log.ps1 -Path .tools/runtime-evidence/v01071-beta87-additive-batch1-direct-20260619-102309/godot.log.current-iteration -OutFile .tools/runtime-evidence/v01071-beta87-additive-batch1-direct-20260619-102309/godot-log-audit.json -FailOnHit
scripts/check-sts1-enabled-mode-runtime-log.ps1 -Mode AdditiveBatch1 -LogPath .tools/runtime-evidence/v01071-beta87-additive-batch1-direct-20260619-102309/godot.log.current-iteration -AuditPath .tools/runtime-evidence/v01071-beta87-additive-batch1-direct-20260619-102309/godot-log-audit.json -ExpectedPackageVersion v0.1.0-private-beta.87 -ExpectedRitsuCompatBranch 0.107.0 -ExpectedRitsuLibVersion 0.4.24 -ExpectedGameVersion 0.107.1 -OutFile .tools/runtime-evidence/v01071-beta87-additive-batch1-direct-20260619-102309/enabled-mode-log-check.json -FailOnMismatch
scripts/check-sts1-runtime-evidence-packet.ps1 -Mode AdditiveBatch1 -EvidenceDir .tools/runtime-evidence/v01071-beta87-additive-batch1-direct-20260619-102309 -ExpectedPackageVersion v0.1.0-private-beta.87 -ExpectedRitsuCompatBranch 0.107.0 -ExpectedRitsuLibVersion 0.4.24 -ExpectedGameVersion 0.107.1 -OutFile .tools/runtime-evidence/v01071-beta87-additive-batch1-direct-20260619-102309/runtime-evidence-packet-check.json -FailOnMismatch
```

That attempt is a blocker packet, not passing proof: main menu and StS1 registration shape passed, but the clean-audit gate failed on BaseLib patch failures.

Pause-safe analyzer follow-up: `analyze-spire-plus-runtime-failure.ps1` now treats this failed direct smoke evidence root as a `DirectSmoke` target via `direct-smoke-summary.json`, `godot.log.current-iteration`, and `godot-log-audit.json`. The retained `.tools/runtime-evidence/v01071-beta87-additive-batch1-direct-20260619-102309/` packet now triages as `PackageRuntimeDrift` with 1 analyzed target, 2 package blockers, 0 harness blockers, and 0 gameplay blockers; `BaseLibPatchFailures` pinpoints `AdjustCustomMessageKeys::Fuckery()` as an undefined target-method failure, `NRelicCollectionCategory::LoadRelics` as an instruction matcher failure, and the 241-applied / 2-failed BaseLib patch summary; the analyzer no longer turns explanatory `SPIREPLUS_ALLOW_UNVERIFIED_COOP_*` startup text into a false co-op override blocker.

Current-version clean loader recapture:

```text
direct AdditiveBatch1 clean-log launch with temporary steam_appid.txt and SPIREPLUS_STS1_EVENT_MODE=AdditiveBatch1 -> .tools/runtime-evidence/v01071-beta88-baselib330-additive-batch1-direct-cleanlog-20260619-103937/
scripts/check-sts1-enabled-mode-runtime-log.ps1 -Mode AdditiveBatch1 -LogPath .tools/runtime-evidence/v01071-beta88-baselib330-additive-batch1-direct-cleanlog-20260619-103937/godot.log.current-iteration -AuditPath .tools/runtime-evidence/v01071-beta88-baselib330-additive-batch1-direct-cleanlog-20260619-103937/godot-log-audit.json -ExpectedPackageVersion v0.1.0-private-beta.88 -ExpectedRitsuCompatBranch 0.107.0 -ExpectedRitsuLibVersion 0.4.24 -ExpectedGameVersion 0.107.1 -OutFile .tools/runtime-evidence/v01071-beta88-baselib330-additive-batch1-direct-cleanlog-20260619-103937/sts1-enabled-mode-report.json -FailOnMismatch
scripts/check-sts1-runtime-evidence-packet.ps1 -Mode AdditiveBatch1 -EvidenceDir .tools/runtime-evidence/v01071-beta88-baselib330-additive-batch1-direct-cleanlog-20260619-103937 -LogFileName godot.log.current-iteration -ExpectedPackageVersion v0.1.0-private-beta.88 -ExpectedRitsuCompatBranch 0.107.0 -ExpectedRitsuLibVersion 0.4.24 -ExpectedGameVersion 0.107.1 -OutFile .tools/runtime-evidence/v01071-beta88-baselib330-additive-batch1-direct-cleanlog-20260619-103937/sts1-runtime-evidence-packet.json -FailOnMismatch
```

The direct beta.88 AdditiveBatch1 smoke is loader/registration proof only. It does not close
gameplay, UI, save-load, co-op, QA, release, or handoff gates.
