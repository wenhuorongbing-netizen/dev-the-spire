# RitsuLib Migration Goal

## Current Target

Date: 2026-06-19

Active branch target: GitHub `main`

Current package target: Spire Plus `v0.1.0-private-beta.87`

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

Current runtime dependency drift was resolved for `v0.107.0` loader and AdditiveBatch1
registration proof, but the local game is now `v0.107.1`; recapture loader proof before
using it as current-runtime evidence.
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
| Tests | Current no-game pass | Recapture `git log -1 --oneline --decorate` and `git status --short --branch` at the start of each continuation and immediately before handoff; older run-start hashes are historical notes, not current status. The beta.87 pass recorded build 0 warnings / 0 errors, split no-build guards 139 passed / 0 failed / 15 skipped / 154 total, opt-in artifact/package coverage 46 passed / 0 failed / 1 skipped / 47 total, and later follow-ups passed Batch 4c static-review/classifier coverage plus malformed runtime-monkey result-path hardening: build 0 warnings / 0 errors, focused migration/compactness/governance/runtime-monkey guards 123 / 0 / 0 / 123, focused `RuntimeMonkeyStabilityGuardTests` 62 / 0 / 0 / 62, focused runtime/docs/migration guards 99 / 0 / 0 / 99, current-doc claims 1090 / 0, static suite 15 / 0, static-file hygiene 12 / 0, parser checks, format, diff-check, patch-inventory, and batch classification with 8 dirty entries / 0 unclassified. The package/runtime baseline is now beta.87 loader/registration proof, not the older beta.86 package baseline. |
| Opt-in artifact subset | Current pass | 67 passed / 0 failed / 2 skipped / 69 total with release/package artifact tests enabled. |
| StS1 static/governance lanes | Current pass | Current-doc claims 1090/0 after AutoSlay proof-mode `-ExpectedAncientIds` target coverage, runtime-monkey escaped-path and malformed-path hardening, analyzer noncanonical-path checks, probe process identity checks, and malformed AutoSlay retained-path rejection; v19 gate ledger 534/0; v20 final-gate overlay 29/0; runtime preflight target updated for local `v0.107.1`; static suite 15/0; static-file hygiene 12/0. |
| Batch 4a/4b migration | Source-level complete | Current patch inventory records 25 migrated `IPatchMethod` patch classes and 142 remaining raw `HarmonyPatch` declarations. |
| Batch 4c migration | Proposal only / static review recaptured | 2026-06-18 recapture confirmed 10 low-risk candidates, no forbidden high-risk categories, and no migration performed. Owner approval is still required before any migration work. |
| Mod Settings UI scaffold | Prepared / live pending | No-launch scaffold refreshed at `.tools/runtime-evidence/mod-settings-current-display-20260618-223145/` with `GitHead` `1cb58dbcbfcdb08defe50a4687023aa59d4a229d`, clean `GitStatusShort`, package `v0.1.0-private-beta.87`, `Capture=None`, and `NoLaunch=true`; the preflight recorded Slay the Spire 2 not running. This is not screenshot, log/audit, or gameplay proof. |
| Manual-test handoff scaffold | Prepared / live pending | No-launch handoff scaffold refreshed at `.tools/runtime-evidence/manual-test-handoff-20260619-095527/` on pushed HEAD `4e7aa523`; generated `handoff-summary.json` recorded 21 required live rows, 21 expected pending failures, 0 warnings, package ZIP `97C65F040F7269738778368878E7946D1563F622D0D8959644C54DBC6806A0B1`, and git handoff metadata with `GitHeadMatchesUpstream=true`. This is a template scaffold only; no game was launched. |
| Manual proof | Pending | Gameplay, clicked UI, save-load, image rendering, replacement behavior, co-op/fail-closed behavior, independent QA, and tester handoff are still open. |

Current beta.85/beta.86 loader proof remains previous-package/game-version context, and retained beta.87 AdditiveBatch1 loader/registration proof exists under `.tools/runtime-evidence/v01070-beta87-additive-batch1-direct-20260618-152531/`.

## Boundaries

- Do not claim private beta, live gameplay, or release readiness from loader/registration proof.
- Do not use `AllDraft` or `Replacement` as a tester/release path without owner approval and fresh targeted validation.
- Do not perform Batch 4c migration without explicit owner approval.
- Do not implement Ascension 21-30 or custom character work.
- Do not bump compile package, manifest minimums, or dependency minimums unless an owner-approved package pass requires it.
- Prefer BaseLib/template-supported APIs and keep Harmony patches narrow.
- Treat the previous `v0.106.1` Off/CanaryOnly/AdditiveBatch1 smokes as historical loader evidence only.

## Next Actions

1. Refresh Mod Settings UI proof for the current Spire Plus display-name package; the current no-launch scaffold is prepared at `.tools/runtime-evidence/mod-settings-current-display-20260618-223145/`, and it must be rerun with `-Capture List` / `-Capture Page` only after manually opening the relevant UI.
2. Use `.tools/runtime-evidence/manual-test-handoff-20260619-095527/TESTER_START_HERE.md` as the current no-launch manual-test scaffold, then fill its rows with live files before any pass claim.
3. Capture Canary gameplay proof for Big Fish, Golden Idol, The Lab, and Divine Fountain.
4. Capture AdditiveBatch1 gameplay proof only after loader/registration evidence remains clean.
5. Capture save-load and image/render proof for event and replacement surfaces.
6. Verify multiplayer fail-closed behavior and any owner-approved two-client diagnostics.
7. Record an owner decision for Batch 4c. The candidate list has static-review coverage; do not migrate unless the owner approves the scope.
8. Recapture git status, pushed HEAD, and validation status before any later handoff.

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

The direct beta.87 AdditiveBatch1 smoke is loader/registration proof only. It does not close
gameplay, UI, save-load, co-op, QA, release, or handoff gates.
