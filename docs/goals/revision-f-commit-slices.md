# Revision F Commit Slices — M3 Week 1

Date: 2026-05-29
HEAD: `d290598c` ("debugging")
Status: **Commit plan prepared — NO actual commits without owner authorization**

## Summary

15 dirty entries (12 tracked + 3 untracked) organized into 6 commit slices. Each slice is independently reviewable and rollback-safe. No production behavior changes in any slice.

## Slice 1: Test Infrastructure Stubs and Compile Links

**Purpose**: Add test stubs and compile links for architecture skeleton coverage.

| # | File | Change |
|---|---|---|
| 1 | `tests/EZMicroBalance.Tests/Stubs/DiagnosticsNamespaceStub.cs` | **UNTRACKED** — test stub for diagnostics namespace |
| 2 | `tests/EZMicroBalance.Tests/EZMicroBalance.Tests.csproj` | Compile links for UrdaStateCodec, UrdaBlessingService.StateSchema, RewardPipeline, CardPlayContext |

**Risk**: None. Test-only changes. No production code affected.

**Commit message**: `test: add diagnostics stub and compile links for architecture skeletons`

---

## Slice 2: Architecture Behavioral Tests

**Purpose**: Add behavioral test coverage for architecture skeletons.

| # | File | Change |
|---|---|---|
| 3 | `tests/EZMicroBalance.Tests/UrdaStateCodecGuardTests.cs` | 15 new behavioral tests (28-field constructor) |
| 4 | `tests/EZMicroBalance.Tests/ActiveSourceManifestGuardTests.cs` | Expected string updated for new source files |
| 5 | `tests/EZMicroBalance.Tests/ArchitectureSkeletonGuardTests.cs` | Assertion changes for CardPlayContext/RewardPipeline |

**Risk**: None. Test-only changes. All tests pass (387 passed, 0 failed).

**Commit message**: `test: expand architecture behavioral tests (UrdaStateCodec, ActiveSource, ArchitectureSkeleton)`

---

## Slice 3: EngineeringGovernance Assertion Split

**Purpose**: Split assertion for more precise failure diagnostics.

| # | File | Change |
|---|---|---|
| 6 | `tests/EZMicroBalance.Tests/EngineeringGovernanceGuardTests.cs` | Assertion split (single assert → multiple targeted asserts) |

**Risk**: None. Test-only change. Improves failure diagnostics.

**Commit message**: `test: split EngineeringGovernance assertions for precise diagnostics`

---

## Slice 4: Script and Config Updates

**Purpose**: Fix batch script classification and update source manifest.

| # | File | Change |
|---|---|---|
| 7 | `scripts/report-worktree-batches.ps1` | .csproj added to ignore list |
| 8 | `docs/issues.md` | Test count updated 361→387 in REFACTOR-PHASE0-1-VALIDATION |

**Risk**: None. Script classification fix and doc count update.

**Commit message**: `fix: add .csproj to batch script ignore list, update test counts in issues.md`

---

## Slice 5: Architecture Source Stubs

**Purpose**: Add untracked source stubs for DeathProtectionService and MultiplayerPolicy.

| # | File | Change |
|---|---|---|
| 9 | `EZMicroBalanceCode/Core/Architecture/DeathProtectionService.cs` | **UNTRACKED** — death protection service skeleton |
| 10 | `EZMicroBalanceCode/Core/Architecture/MultiplayerPolicy.cs` | **UNTRACKED** — multiplayer policy skeleton |

**Risk**: Low. Skeleton stubs with no behavioral impact. Already covered by compile links in Slice 1.

**Commit message**: `feat: add DeathProtectionService and MultiplayerPolicy architecture stubs`

---

## Slice 6: Docs Count Reconciliation

**Purpose**: Update stale counts across all goal/status/ledger docs.

| # | File | Change |
|---|---|---|
| 11 | `docs/goals/debug.md` | Major M3 rewrite + stale count fixes (87→92, 361→387, 9→15) |
| 12 | `docs/goals/event.md` | StS1 Event Port audit content |
| 13 | `docs/goals/migration.md` | Review doc + stale count fixes (87→92, 361→387) |
| 14 | `docs/features/ritsulib-migration/monthly-dev-spec.md` | Test/guard counts updated |
| 15 | `docs/features/ritsulib-migration/next-overnight-run.md` | Pre-run checklist test count updated |

**Risk**: None. Doc-only changes. No code or test modifications.

**Commit message**: `docs: reconcile stale counts across goal/status/ledger docs (92 warnings, 387 tests, 15 dirty)`

---

## Commit Order

Recommended commit order (least to most risk):

1. **Slice 1** — Test infrastructure stubs (safe foundation)
2. **Slice 2** — Architecture behavioral tests (depends on Slice 1)
3. **Slice 3** — EngineeringGovernance assertion split (independent)
4. **Slice 4** — Script and config updates (independent)
5. **Slice 5** — Architecture source stubs (independent, but compile links from Slice 1 should be committed first)
6. **Slice 6** — Docs count reconciliation (independent, but should be last to capture final numbers)

## Rollback Strategy

Each slice is independently rollback-safe:
- Slices 1-4: Test-only changes. Revert individual commits without affecting production.
- Slice 5: Source stubs with no behavioral impact. Revert without affecting existing code.
- Slice 6: Doc-only changes. Revert without affecting code or tests.

## Owner Decision Required

- **Authorize commit**: Owner reviews each slice and authorizes commit in order.
- **Modify slices**: Owner can reorder, merge, or split slices before committing.
- **Reject slices**: Owner can reject specific slices and request changes.

No commits will be made without explicit owner authorization.
