# Refactor Goal

## Current Target

Date: 2026-06-19

Active branch target: GitHub `main`

Current package target: Spire Plus `v0.1.0-private-beta.88`

Runtime dependency target: BaseLib `v3.3.0`, STS2-RitsuLib `v0.4.24`, Slay the Spire 2 `v0.107.1`

Use these current files as the source of truth before acting on this goal:

- `PROJECT_STATE.md`
- `docs/test-ready-development-goal.md`
- `docs/goals/migration.md`
- `docs/features/ritsulib-migration/next-overnight-run.md`
- `docs/features/sts1-events/status-board.md`

The previous long-form strict audit is archived at
`docs/archive/legacy-planning/refactor-goal-strict-audit-20260618.md`. Keep this active file compact,
current, and action-oriented.

## Current Conclusion

The original runtime blocker is resolved for loader and registration proof. The beta.84 failure root
cause was Spire Plus runtime API drift, not missing or too-old BaseLib/RitsuLib. The later `v0.107.1`
clean-audit blocker was BaseLib `v3.2.1` patch drift; the dependency floor is now aligned on
BaseLib `v3.3.0` and STS2-RitsuLib `v0.4.24`.

Current beta.85/beta.86/beta.87 loader proof remains previous-package/game-version context, and current beta.88 AdditiveBatch1 loader/registration proof exists under `.tools/runtime-evidence/v01071-beta88-baselib330-additive-batch1-direct-cleanlog-20260619-103937/`.

This is still source/static/no-game governance plus loader/registration proof. Do not claim enabled-mode safe beyond the retained loader evidence. Gameplay, Mod Settings UI page proof,
save-load, image/render, replacement functional proof, co-op/fail-closed proof, independent QA,
release readiness, and tester-package handoff remain pending.

## Status

| Area | Current state | Evidence / notes |
| --- | --- | --- |
| Runtime dependency blocker | Closed for loader/registration | BaseLib `v3.3.0` and STS2-RitsuLib `v0.4.24` are installed for Slay the Spire 2 `v0.107.1`; beta.88 AdditiveBatch1 direct proof reached main menu with clean retained verifiers. |
| CanaryOnly proof | Historical loader pass | CanaryOnly beta.85 / v0.107.0 smoke remains previous-package loader proof only: 4 event types / 6 registration calls. |
| AdditiveBatch1 proof | Current loader/registration pass | beta.88 direct proof records 10 event types / 14 registration calls, 25/25 Spire Plus patches, clean audit, enabled-mode verifier 31 / 0, and packet verifier 0 mismatches. |
| AutoSlay/runtime evidence governance | Current no-game hardening | Packet/analyzer guards reject malformed, escaped, noncanonical, missing, or blank retained artifact paths before owner routing; this protects future runtime packets from stale or shadow evidence. |
| Batch 4c | Proposal only / static review recaptured | The 2026-06-18 recapture confirmed 10 low-risk candidates, no forbidden high-risk categories, and no migration performed. Owner approval is still required before any migration. |
| Documentation compactness | In progress | Player-facing naming guard coverage may be split into source-preserving partial files when the assertions remain unchanged and focused tests pass. |
| Manual proof | Pending | Mod Settings screenshots, gameplay, save-load, image/render, replacement behavior, co-op/fail-closed behavior, independent QA, and handoff recapture are not complete. |

## Allowed Refactor Scope

- Source-preserving partial-file splits for oversized test/governance classes.
- Small helper extraction in scripts/tests when it makes invalid evidence fail cleanly instead of crashing.
- Documentation compaction that points active readers to current files and archives old prompt-heavy notes.
- Batch 4c candidate review and owner-decision recording only.
- Current enabled-mode proof collection only when the coordination boundary allows runtime work; current enabled-mode proof is still loader/registration scoped until gameplay evidence exists.

## Blocked Scope

- Do not perform Batch 4c migration without explicit owner approval.
- Do not migrate high-risk run lifecycle, save/load, map generation, multiplayer/lobby, death, A20 boss-flow, or reward-state patches.
- Do not bump package/dependency versions without an owner-approved package pass.
- Do not use AllDraft or Replacement as tester/release paths without fresh targeted validation.
- Do not claim private beta, live gameplay, release readiness, full parity, or handoff readiness from loader/registration proof.
- Do not implement Ascension 21-30 or a custom character this cycle.

## Next Actions

1. Validate the current evidence-governance and documentation split work with build, focused tests, current-doc claims, static-file hygiene, format, diff-check, patch inventory, and worktree batch classification.
2. Refresh Mod Settings UI proof for the current Spire Plus display-name package when manual UI access is available.
3. Capture CanaryOnly gameplay proof for Big Fish, Golden Idol, The Lab, and Divine Fountain.
4. Capture AdditiveBatch1 gameplay, save-load, image/render, replacement, and co-op/fail-closed proof only after loader/registration evidence remains clean.
5. Record an owner decision for Batch 4c before any source migration.
6. Recapture pushed HEAD, git status, validation status, and package state before any tester handoff.

## Validation

Use focused lanes for this refactor goal:

```text
dotnet build EZMicroBalance.sln -m:1 --no-incremental -p:UseSharedCompilation=false
dotnet test tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build --filter "FullyQualifiedName~DocumentationCompactnessGuardTests" --logger "console;verbosity=minimal" -- RunConfiguration.MaxCpuCount=1
dotnet test tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build --filter "FullyQualifiedName~RitsuLibMigrationGuardTests|FullyQualifiedName~RuntimeMonkeyStabilityGuardTests" --logger "console;verbosity=minimal" -- RunConfiguration.MaxCpuCount=1
scripts/check-sts1-event-current-doc-claims.ps1 -FailOnMismatch
scripts/check-sts1-static-file-hygiene.ps1 -FailOnMismatch
dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
git diff --check
scripts/generate-patch-inventory.ps1 -Check
scripts/report-worktree-batches.ps1 -FailOnUnclassified
```

Run publish/package/runtime only after resource/package/runtime surfaces change or when the manual proof lane
explicitly resumes. This refactor slice does not close gameplay, UI, save-load, co-op, QA, release, or handoff gates.
