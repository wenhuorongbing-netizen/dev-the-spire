# RitsuLib Migration Goal

## Current Target

Date: 2026-06-18

Active branch target: GitHub `main`

Current package target: Spire Plus `v0.1.0-private-beta.87`

Installed game target: Slay the Spire 2 `v0.107.0`

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

Current runtime dependency drift is resolved for loader and AdditiveBatch1 registration proof.
The migration is not release-ready. Gameplay, Mod Settings UI page refresh, event screenshots,
save-load, image/render, replacement functional proof, co-op/fail-closed proof, independent QA,
and tester-package handoff remain pending. Worktree and pushed-HEAD status must be recaptured
before any later handoff.

## Status

| Area | Current state | Evidence / notes |
| --- | --- | --- |
| RitsuLib install | Pass | `STS2-RitsuLib` `v0.4.24` is installed with `lib/0.107.0`. |
| Root cause history | Resolved for loader | The beta.84 Off failure was Spire Plus API drift, including `EctoplasmGoldGatePatch` and getter-target drift, not missing BaseLib/RitsuLib. |
| beta.85 Off loader proof | Historical pass | `v0.107.0` beta.85 package runtime proof reached main menu with 25/25 Spire Plus patches and clean audit. Treat it as previous-package loader context. |
| beta.85 CanaryOnly proof | Historical pass | Previous-package loader proof only: 4 event types / 6 registration calls. |
| beta.85 AdditiveBatch1 proof | Historical fail | Previous-package mismatch: 13/14 registration calls because the installed package/source shape was stale. |
| beta.87 build/publish/package | Pass | `dotnet build`, `dotnet publish`, package creation, and installed package parity passed for `v0.1.0-private-beta.87`. |
| beta.87 AdditiveBatch1 proof | Current loader/registration pass | `.tools/runtime-evidence/v01070-beta87-additive-batch1-direct-20260618-152531/` reached main menu with BaseLib, RitsuLib, and Spire Plus loaded; 25/25 patches; 30 SavedSpireFields; 10 event types / 14 registration calls; clean audit; retained log verifier 31/0 and packet verifier 52/0. |
| Tests | Current no-game pass | The run-start pushed HEAD for the blank-path/log-growth runtime probe follow-up was `77d46f23`; recapture `git log -1 --oneline --decorate` and `git status --short --branch` again before handoff. The beta.87 pass recorded build 0 warnings / 0 errors, split no-build guards 139 passed / 0 failed / 15 skipped / 154 total, opt-in artifact/package coverage 46 passed / 0 failed / 1 skipped / 47 total, and the latest focused follow-up validation passed `RuntimeFailureAnalyzer` 20 / 0 / 0 / 20, `RuntimeMonkeyPacketChecker` 20 / 0 / 0 / 20, `AncientUiReadinessGuardTests` 13 / 0 / 0 / 13, and `DocumentationCompactnessGuardTests` 25 / 0 / 0 / 25 after rebuilding. The package/runtime baseline is now beta.87 loader/registration proof, not the older beta.86 package baseline. |
| Opt-in artifact subset | Current pass | 67 passed / 0 failed / 2 skipped / 69 total with release/package artifact tests enabled. |
| StS1 static/governance lanes | Current pass | Current-doc claims 1028/0 after runtime probe blank-path/log-growth hardening and packet/doc alignment; v19 gate ledger 534/0; v20 final-gate overlay 29/0; runtime preflight 27/0; static suite 15/0; static-file hygiene 12/0. |
| Batch 4a/4b migration | Source-level complete | Current patch inventory records 25 migrated `IPatchMethod` patch classes and 142 remaining raw `HarmonyPatch` declarations. |
| Batch 4c migration | Proposal only | Requires owner approval before any migration work. |
| Mod Settings UI scaffold | Prepared / live pending | No-launch scaffold prepared at `.tools/runtime-evidence/mod-settings-current-display-20260618-143758/` against HEAD `6be72076` and package `v0.1.0-private-beta.87`; preflight recorded that Slay the Spire 2 was not running. This is not screenshot, log/audit, or gameplay proof. |
| Manual proof | Pending | Gameplay, clicked UI, save-load, image rendering, replacement behavior, co-op/fail-closed behavior, independent QA, and tester handoff are still open. |

Current beta.85/beta.86 loader proof remains previous-package context, and current beta.87 AdditiveBatch1 loader/registration proof exists under `.tools/runtime-evidence/v01070-beta87-additive-batch1-direct-20260618-152531/`.

## Boundaries

- Do not claim private beta, live gameplay, or release readiness from loader/registration proof.
- Do not use `AllDraft` or `Replacement` as a tester/release path without owner approval and fresh targeted validation.
- Do not perform Batch 4c migration without explicit owner approval.
- Do not implement Ascension 21-30 or custom character work.
- Do not bump compile package, manifest minimums, or dependency minimums unless an owner-approved package pass requires it.
- Prefer BaseLib/template-supported APIs and keep Harmony patches narrow.
- Treat the previous `v0.106.1` Off/CanaryOnly/AdditiveBatch1 smokes as historical loader evidence only.

## Next Actions

1. Refresh Mod Settings UI proof for the current Spire Plus display-name package; the no-launch scaffold is prepared at `.tools/runtime-evidence/mod-settings-current-display-20260618-143758/`, and it must be rerun with `-Capture List` / `-Capture Page` only after manually opening the relevant UI.
2. Capture Canary gameplay proof for Big Fish, Golden Idol, The Lab, and Divine Fountain.
3. Capture AdditiveBatch1 gameplay proof only after loader/registration evidence remains clean.
4. Capture save-load and image/render proof for event and replacement surfaces.
5. Verify multiplayer fail-closed behavior and any owner-approved two-client diagnostics.
6. Review Batch 4c candidates without migrating them unless the owner approves the scope.
7. Recapture git status, pushed HEAD, and validation status before any later handoff.

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
dotnet test tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build --filter "FullyQualifiedName~RuntimeMonkeyStabilityGuardTests|FullyQualifiedName~ReleaseEvidenceGateTests" --logger "console;verbosity=minimal" -- RunConfiguration.MaxCpuCount=1
dotnet test tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build --filter "FullyQualifiedName~DocumentationCompactnessGuardTests" --logger "console;verbosity=minimal" -- RunConfiguration.MaxCpuCount=1
dotnet test tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build --filter "FullyQualifiedName~EngineeringGovernanceGuardTests" --logger "console;verbosity=minimal" -- RunConfiguration.MaxCpuCount=1
dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
git diff --check
scripts/generate-patch-inventory.ps1 -Check
scripts/report-worktree-batches.ps1 -FailOnUnclassified
```

The direct beta.87 AdditiveBatch1 smoke is loader/registration proof only. It does not close
gameplay, UI, save-load, co-op, QA, release, or handoff gates.
