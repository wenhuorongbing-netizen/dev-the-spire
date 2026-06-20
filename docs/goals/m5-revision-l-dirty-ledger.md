# M5 Revision L Dirty Ledger

Date: 2026-06-10
Baseline HEAD: `f32c6767`

Revision M supersession note, 2026-06-11: this dirty-ledger scope is historical owner-review context. Current beta.85 has clean `v0.107.0` default-Off loader proof only; current CanaryOnly, AdditiveBatch1, gameplay, save-load, replacement, multiplayer, QA, clean-worktree, and release-ready proof remain pending. Use `PROJECT_STATE.md` and the Revision M docs for current proof claims.

Current supersession, 2026-06-20: beta.91 has RitsuLib-only Off and AdditiveBatch1 loader/registration proof on Slay the Spire 2 `v0.107.1` with STS2-RitsuLib `0.4.28`; gameplay, UI, save-load, replacement, co-op, QA, clean-worktree handoff, and release-ready proof remain pending. Use `PROJECT_STATE.md` and `docs/test-ready-development-goal.md` for current proof claims.

## Scope

This ledger classifies the dirty worktree for owner review. It does not authorize commit, rollback, package refresh, or push.

## Dirty Slices

| Slice | Files | Purpose | Risk |
|---|---|---|---|
| 1. Source API build fix | `EZMicroBalanceCode/Ancients/Expansion/Lotha/*`, `EZMicroBalanceCode/Ancients/Patches/MeatCleaverCookPatches.cs`, `EZMicroBalanceCode/Ancients/Patches/PickupRewardGatePatches.cs`, `EZMicroBalanceCode/Ascension/Powers/MartyrOathPowers.cs`, `tests/EZMicroBalance.Tests/AncientBehaviorGuardTests.cs` | Align Lotha, Martyr Oath, Ectoplasm, and Meat Cleaver hooks with the installed game DLL API and update source guards. | Medium-high: behavior-touching combat/reward/rest-site code. |
| 2. Sts1Events warning burn-down | `Sts1ShiningLight.cs`, `Sts1UpgradeShrine.cs`, `Sts1GoldenShrine.cs`, `Sts1OldBeggar.cs`, `Sts1Purifier.cs`, `Sts1TheCleric.cs` | Add `Owner` guards for AdditiveBatch1 rows and reduce nullable warning debt. | Medium: staging feature, but event behavior changes when enabled. |
| 3. RitsuLib truth docs | `docs/features/ritsulib-migration/*`, `docs/integrations/ritsulib.md`, `docs/migration.md`, `docs/features/ritsulib-migration/batch-4c-candidates.md` | Move from missing-runtime-blocker wording to historical `v0.106.1` loader-gate evidence, record current `v0.107.0` proof as blocked, and keep Batch 4c proposal-only. | Medium: truth-claim risk. |
| 4. Revision L goal cleanup | `docs/goals/debug.md`, `event.md`, `migration.md`, `refactor.md`, deleted `m5-week1-*`, deleted old overnight docs, new `m5-revision-l-*`, recreated overnight docs | Replace stale Week 1 owner-review docs with Revision L packet docs. | High: broad user-owned docs cleanup. |
| 5. Current validation docs | `PROJECT_STATE.md`, `AGENTS.md`, `README.md`, `docs/issues.md`, `docs/review.md`, `docs/reviews/current-validation.md`, `docs/goals/warning-ledger.md`, `harness/*`, and related warning/status docs | Align active truth with current dirty-source build and historical runtime boundary. | Medium: docs can accidentally overclaim readiness. |
| 6. Manual handoff harness | `scripts/prepare-current-manual-test-handoff.ps1`, `tests/EZMicroBalance.Tests/ReleaseEvidenceGateTests.EvidenceHelpers.cs` | Keep the no-launch handoff verifier on its default manifest path and harden PowerShell script tests against `testhost` stream/timeout instability. | Medium: test harness and release-evidence scaffolding; requires clean rerun before acceptance. |
| 7. Generated patch inventory | `docs/patch-inventory.md` | Refresh generated row-level patch inventory after patch-target changes. | Low: generated documentation. |

## Owner Decisions

- Accept slice 1 only after the final validation commands are green.
- Accept slice 2 only while Sts1Events remains staging-only or after separate promotion proof.
- Accept slice 3 only if the owner agrees historical loader proof is enough to close the old dependency blocker.
- Accept slice 4 separately from source changes; it is broad documentation cleanup.
- Accept slice 6 only after the paused validation lane is clean; it is not runtime proof.
- Do not package, commit, or push any slice until the owner chooses the intended commit plan.

## Rollback Notes

Rollback should be per slice, not a broad reset. Do not revert unrelated user changes. If the owner rejects the Revision L doc rename, restore or archive the old `m5-week1-*` files intentionally rather than relying on accidental deleted-file state.
